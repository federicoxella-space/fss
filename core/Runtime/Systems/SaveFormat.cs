using System.IO;

namespace Sim
{
    /// <summary>
    /// Writes a world and its pending commands to bytes, and reads them back (NFR-08,
    /// AC-03, DEC-032, SIM-STATE §Serialisation notes).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Hand-written, field by field</b>, because SIM-REQ section 20, "Downstream
    /// constraints", forbids reflection-based serialisation. Like <see cref="StateHash"/>,
    /// it cannot enumerate its own fields, and what keeps it complete is
    /// <c>NFR08_EveryStateFieldEntersTheSave</c>. The order is the hash's: §World, then
    /// §Chronicle column by column over the rows in use, then the shared pool of entities.
    /// Capacity is not state and is not written. Integers are little-endian, which
    /// <see cref="BinaryWriter"/> guarantees on every platform.
    /// </para>
    /// <para>
    /// <b>The pending commands follow the state</b>, oldest first (R-021). They are input,
    /// so the state hash never sees them, but the bytes do, and AC-03 compares bytes.
    /// </para>
    /// <para>
    /// <b>A version number, and the migration path from the first write</b> (NFR-08).
    /// Every save opens with <see cref="Magic"/> and <see cref="Version"/>; <see cref="Read"/>
    /// dispatches on the version. A later format keeps the reader of every earlier one,
    /// each bringing its save up to the current shape, and a version this build does not
    /// know is refused rather than guessed at. Version 1 is the first, and has no
    /// predecessor to migrate from.
    /// </para>
    /// <para>
    /// <b>A save is input from outside the core, and is checked as such.</b> A load refuses
    /// a chronicle <see cref="Chronicle.Append"/> could not have written — a cause that is
    /// not an earlier entry or does not match its kind, an entry dated before the one
    /// preceding it or after the world's tick, a pool that is not contiguous — and a count
    /// larger than the bytes left could hold, before allocating for it. It throws
    /// <see cref="InvalidDataException"/> and returns no half-read world. It does not check
    /// the kinds of the pending commands: the drain does, on the tick they land on (DEC-086).
    /// </para>
    /// <para>
    /// The rule version comes from the save, not from this build: loading is the one way
    /// another value enters state (R-004). A version later than this build's is refused,
    /// since running it here would leave state claiming rules that did not produce it
    /// (NFR-09). Materialising deferred entities on a patch (NFR-09, DEC-033) needs
    /// deferred entities, and phase 3 has none.
    /// </para>
    /// </remarks>
    internal static class SaveFormat
    {
        /// <summary>"SIMS", read as a little-endian integer: what the first four bytes of a save are.</summary>
        public const int Magic = 0x534D4953;

        /// <summary>The format this build writes.</summary>
        public const int Version = 1;

        // Bytes each item takes, for refusing a count the rest of the save cannot hold.
        private const int RowBytes = 8 + 8 + 4 + 4 + 4 + 4 + 4;
        private const int EntityBytes = 8;
        private const int CommandBytes = 4 + 8 * 4;

        /// <summary>The bytes of <paramref name="state"/> and the commands still waiting.</summary>
        public static byte[] Write(WorldState state, CommandQueue commands)
        {
            using (var buffer = new MemoryStream())
            {
                using (var w = new BinaryWriter(buffer))
                {
                    w.Write(Magic);
                    w.Write(Version);

                    w.Write(state.Tick);
                    w.Write(state.WorldSeed);
                    w.Write(state.GenerationParams.SettlementCount);
                    w.Write(state.RuleVersion);
                    w.Write(state.CurrencyTotal);

                    int rows = state.ChronicleCount;
                    w.Write(rows);
                    for (int i = 0; i < rows; i++) { w.Write(state.ChronicleTick[i]); }
                    for (int i = 0; i < rows; i++) { Write(w, state.ChronicleLocation[i]); }
                    for (int i = 0; i < rows; i++) { w.Write((int)state.ChronicleCauseKind[i]); }
                    for (int i = 0; i < rows; i++) { w.Write(state.ChronicleCause[i]); }
                    for (int i = 0; i < rows; i++) { w.Write(state.ChronicleImportance[i]); }
                    for (int i = 0; i < rows; i++) { w.Write(state.ChronicleEntityStart[i]); }
                    for (int i = 0; i < rows; i++) { w.Write(state.ChronicleEntityCount[i]); }

                    int entities = Chronicle.EntitiesInUse(state);
                    for (int i = 0; i < entities; i++) { Write(w, state.ChronicleEntities[i]); }

                    w.Write(commands.Count);
                    foreach (Command c in commands)
                    {
                        w.Write(c.Kind);
                        w.Write(c.A);
                        w.Write(c.B);
                        w.Write(c.C);
                        w.Write(c.D);
                    }
                }

                return buffer.ToArray();
            }
        }

        /// <summary>
        /// The world in <paramref name="save"/>, in the shape this build holds it, whichever
        /// format version wrote it; its pending commands go into <paramref name="commands"/>.
        /// </summary>
        public static WorldState Read(byte[] save, CommandQueue commands)
        {
            using (var r = new BinaryReader(new MemoryStream(save, writable: false)))
            {
                try
                {
                    if (r.ReadInt32() != Magic)
                    {
                        throw new InvalidDataException("Not a save.");
                    }

                    int version = r.ReadInt32();
                    WorldState state;
                    switch (version)
                    {
                        case 1:
                            state = ReadVersion1(r, commands);
                            break;
                        default:
                            throw new InvalidDataException("Save format version " + version.ToString() + " is not one this build reads.");
                    }

                    if (r.BaseStream.Position != r.BaseStream.Length)
                    {
                        throw new InvalidDataException("Bytes after the end of the save.");
                    }

                    return state;
                }
                catch (EndOfStreamException e)
                {
                    throw new InvalidDataException("The save ends early.", e);
                }
            }
        }

        private static WorldState ReadVersion1(BinaryReader r, CommandQueue commands)
        {
            long tick = r.ReadInt64();
            ulong worldSeed = r.ReadUInt64();
            int settlementCount = r.ReadInt32();
            int ruleVersion = r.ReadInt32();
            Refuse(tick < 0, "A tick is never negative (R-007).");
            Refuse(settlementCount < 0, "A settlement count is never negative.");
            Refuse(ruleVersion < 1 || ruleVersion > Simulation.CurrentRuleVersion, "Rule version " + ruleVersion.ToString() + " is not one this build has.");

            var state = new WorldState(worldSeed, new GenerationParams(settlementCount))
            {
                Tick = tick,
                RuleVersion = ruleVersion,
                CurrencyTotal = r.ReadInt64(),
            };

            int rows = Count(r, RowBytes);
            state.ChronicleTick = new long[rows];
            state.ChronicleLocation = new EntityId[rows];
            state.ChronicleCauseKind = new CauseKind[rows];
            state.ChronicleCause = new int[rows];
            state.ChronicleImportance = new int[rows];
            state.ChronicleEntityStart = new int[rows];
            state.ChronicleEntityCount = new int[rows];

            for (int i = 0; i < rows; i++) { state.ChronicleTick[i] = r.ReadInt64(); }
            for (int i = 0; i < rows; i++) { state.ChronicleLocation[i] = ReadEntity(r); }
            for (int i = 0; i < rows; i++) { state.ChronicleCauseKind[i] = (CauseKind)r.ReadInt32(); }
            for (int i = 0; i < rows; i++) { state.ChronicleCause[i] = r.ReadInt32(); }
            for (int i = 0; i < rows; i++) { state.ChronicleImportance[i] = r.ReadInt32(); }
            for (int i = 0; i < rows; i++) { state.ChronicleEntityStart[i] = r.ReadInt32(); }
            for (int i = 0; i < rows; i++) { state.ChronicleEntityCount[i] = r.ReadInt32(); }

            long pool = 0;
            for (int i = 0; i < rows; i++)
            {
                int id = i + 1;
                Refuse(Chronicle.CheckCause(state.ChronicleCauseKind[i], state.ChronicleCause[i], id));
                Refuse(state.ChronicleTick[i] < (i == 0 ? 0 : state.ChronicleTick[i - 1]) || state.ChronicleTick[i] > tick, "Entry " + id.ToString() + " is dated where no append could have put it.");
                Refuse(state.ChronicleEntityStart[i] != pool, "Entry " + id.ToString() + " does not start where the one before it ends.");
                Refuse(state.ChronicleEntityCount[i] < 0, "Entry " + id.ToString() + " names a negative number of entities.");
                pool += state.ChronicleEntityCount[i];
            }

            Refuse(pool > (r.BaseStream.Length - r.BaseStream.Position) / EntityBytes, "The pool of entities is longer than the save.");
            state.ChronicleEntities = new EntityId[pool];
            for (int i = 0; i < pool; i++) { state.ChronicleEntities[i] = ReadEntity(r); }
            state.ChronicleCount = rows;

            int pending = Count(r, CommandBytes);
            for (int i = 0; i < pending; i++)
            {
                commands.Submit(new Command(r.ReadInt32(), r.ReadInt64(), r.ReadInt64(), r.ReadInt64(), r.ReadInt64()));
            }

            return state;
        }

        private static void Write(BinaryWriter w, EntityId id)
        {
            w.Write(id.Index);
            w.Write(id.Generation);
        }

        private static EntityId ReadEntity(BinaryReader r)
        {
            int index = r.ReadInt32();
            return new EntityId(index, r.ReadInt32());
        }

        /// <summary>A count, refused if negative or if the bytes left cannot hold that many items.</summary>
        private static int Count(BinaryReader r, int bytesEach)
        {
            int n = r.ReadInt32();
            Refuse(n < 0 || n > (r.BaseStream.Length - r.BaseStream.Position) / bytesEach, "A count the save cannot hold.");
            return n;
        }

        private static void Refuse(bool refused, string why)
        {
            if (refused)
            {
                throw new InvalidDataException(why);
            }
        }

        private static void Refuse(string why) => Refuse(why != null, why);
    }
}
