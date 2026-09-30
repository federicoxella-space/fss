using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Sim.Tests
{
    /// <summary>
    /// AC-03 and NFR-08: a save is the whole state, the seed and parameters within it, and
    /// the commands still pending; it carries a version, and save → load → save gives the
    /// same bytes.
    /// </summary>
    /// <remarks>
    /// The worlds come from <see cref="StateHashTests.Reference"/>, whose every field is
    /// distinct and none of them zero but the two that cannot be, so a field the reader
    /// drops or puts in the wrong place changes the second save rather than hiding in a
    /// default. Its rule version is one no build has yet, so <see cref="Saved"/> brings it
    /// to this build's before anything loads it. Commands of the stand-in kind of
    /// <see cref="CommandQueueTests"/> wait in its queue, since phase 3 defines no kind of
    /// its own, with <see cref="Wide"/> between them so that every field of a command is
    /// in use.
    /// </remarks>
    [TestFixture]
    public sealed class SaveTests
    {
        /// <summary>
        /// AC-03: save → load → save produces identical bytes, for an empty world, for one
        /// with every field in use and commands pending, and through the host's entry.
        /// </summary>
        [Test]
        public void AC03_SaveRoundTrip()
        {
            AssertRoundTrip(new WorldState(0, default), new CommandQueue(), "an empty world");
            AssertRoundTrip(Saved(), new CommandQueue(), "every field in use");
            AssertRoundTrip(Saved(), Mixed(), "with commands pending");

            var sim = new Simulation(worldSeed: 5, new GenerationParams(settlementCount: 12));
            sim.Advance(400);
            byte[] first = sim.Save();
            Simulation loaded = Simulation.Load(first);
            Assert.That(loaded.Save(), Is.EqualTo(first), "through the host's entry");
            Assert.That(loaded.StateHash, Is.EqualTo(sim.StateHash));

            sim.Advance(29);
            loaded.Advance(29);
            Assert.That(loaded.StateHash, Is.EqualTo(sim.StateHash), "a loaded world goes on as the saved one does");
        }

        /// <summary>
        /// NFR-08, "a full state snapshot": every field of the state, perturbed one at a
        /// time, changes the bytes. The walk is <see cref="StateHashTests"/>' own, so a
        /// field a later phase appends to <see cref="WorldState"/> and leaves out of the
        /// writer fails here without anyone remembering this test exists.
        /// </summary>
        [Test]
        public void NFR08_EveryStateFieldEntersTheSave()
        {
            byte[] reference = SaveFormat.Write(StateHashTests.Reference(), new CommandQueue());
            var unwritten = new List<string>();
            int walked = 0;

            foreach (StateHashTests.Path path in StateHashTests.Paths(typeof(WorldState), "WorldState"))
            {
                WorldState state = StateHashTests.Reference();
                path.Perturb(state);
                walked++;

                if (SaveFormat.Write(state, new CommandQueue()).AsSpan().SequenceEqual(reference))
                {
                    unwritten.Add(path.Name);
                }
            }

            Assert.That(walked, Is.GreaterThan(20), "the walk reached the state");
            Assert.That(unwritten, Is.Empty, "fields a save does not carry (NFR-08):" + Environment.NewLine + string.Join(Environment.NewLine, unwritten));
        }

        /// <summary>
        /// R-021, DEC-032: a save carries the commands still pending, in submission order and
        /// outside the state hash, and a load returns them to the queue. The stand-in kind
        /// does not commute, so the same commands drained in another order leave another
        /// world.
        /// </summary>
        [Test]
        public void NFR08_ASaveCarriesThePendingCommandsInOrder()
        {
            WorldState state = Saved();
            CommandQueue pending = Pending(3, 1, 4);

            Assert.That(
                SaveFormat.Write(state, pending),
                Is.Not.EqualTo(SaveFormat.Write(state, Pending(4, 1, 3))),
                "the order is in the bytes");

            var restored = new CommandQueue();
            WorldState loaded = SaveFormat.Read(SaveFormat.Write(state, pending), restored);
            Assert.That(StateHash.Of(loaded), Is.EqualTo(StateHash.Of(state)), "the commands are not state");
            Assert.That(restored.Count, Is.EqualTo(3));

            TickLoop.Advance(state, pending, 1, default(NoLevels), default(CommandQueueTests.FoldEffects));
            TickLoop.Advance(loaded, restored, 1, default(NoLevels), default(CommandQueueTests.FoldEffects));
            Assert.That(StateHash.Of(loaded), Is.EqualTo(StateHash.Of(state)), "the loaded commands apply as the saved ones do");
        }

        /// <summary>
        /// R-004 and NFR-09: a new world is this build's rule version, and a loaded one keeps
        /// its save's, which is read from the bytes and refused when no build up to this one
        /// has it: a world run here under a later version would claim rules that did not
        /// produce it. Version 1 is the only one, so keeping and refusing are what can be
        /// seen of the reader in phase 3.
        /// </summary>
        [Test]
        public void NFR09_ALoadedWorldKeepsTheRuleVersionOfItsSave()
        {
            Assert.That(new WorldState(0, default).RuleVersion, Is.EqualTo(Simulation.CurrentRuleVersion));

            byte[] save = SaveFormat.Write(Saved(), new CommandQueue());
            Assert.That(BitConverter.ToInt32(save, RuleVersionAt), Is.EqualTo(Simulation.CurrentRuleVersion), "the offset reads the rule version");
            Assert.That(SaveFormat.Read(save, new CommandQueue()).RuleVersion, Is.EqualTo(Simulation.CurrentRuleVersion));

            Assert.Throws<InvalidDataException>(() => Load(With(save, RuleVersionAt, Simulation.CurrentRuleVersion + 1)), "a later rule version");
            Assert.Throws<InvalidDataException>(() => Load(With(save, RuleVersionAt, 0)), "no rule version");
        }

        /// <summary>
        /// NFR-08: a save says which format wrote it, and a format this build does not know
        /// is refused rather than read as if it were the current one.
        /// </summary>
        [Test]
        public void NFR08_ASaveCarriesItsFormatVersion()
        {
            byte[] save = SaveFormat.Write(Saved(), new CommandQueue());
            Assert.That(BitConverter.ToInt32(save, 0), Is.EqualTo(SaveFormat.Magic));
            Assert.That(BitConverter.ToInt32(save, 4), Is.EqualTo(SaveFormat.Version));

            Assert.Throws<InvalidDataException>(() => Load(With(save, 4, SaveFormat.Version + 1)), "a later version");
            Assert.Throws<InvalidDataException>(() => Load(With(save, 4, 0)), "no version");
            Assert.Throws<InvalidDataException>(() => Load(With(save, 0, 0)), "not a save");
            Assert.Throws<ArgumentNullException>(() => Simulation.Load(null));
        }

        /// <summary>
        /// A save comes from outside the core. A load refuses bytes that end early, run
        /// past the end, or hold a chronicle <see cref="Chronicle.Append"/> would not have
        /// written, and throws one exception type for all of them.
        /// </summary>
        [Test]
        public void NFR08_ALoadRefusesWhatNoSaveHolds()
        {
            byte[] save = SaveFormat.Write(Saved(), Pending(2));

            for (int length = 0; length < save.Length; length++)
            {
                byte[] cut = new byte[length];
                Array.Copy(save, cut, length);
                Assert.Throws<InvalidDataException>(() => Load(cut), "cut at " + length);
            }

            byte[] longer = new byte[save.Length + 1];
            Array.Copy(save, longer, save.Length);
            Assert.Throws<InvalidDataException>(() => Load(longer), "a byte after the end");

            // Header 8, World row 32, row count 4; then per column, two rows each: tick 8,
            // location 8, kind 4, cause 4, importance 4, start 4, count 4.
            const int Rows = 8 + 32;
            const int Ticks = Rows + 4;
            const int Kinds = Ticks + 2 * 8 + 2 * 8;
            const int Causes = Kinds + 2 * 4;
            const int Starts = Causes + 2 * 4 + 2 * 4;
            Assert.That(BitConverter.ToInt32(save, Rows), Is.EqualTo(2), "the offsets read the row count");
            Assert.That(BitConverter.ToInt32(save, Kinds + 4), Is.EqualTo((int)CauseKind.Event), "the offsets read the second row's kind");

            Assert.Throws<InvalidDataException>(() => Load(With(save, Kinds + 4, 0)), "a kind that is none of the three");
            Assert.Throws<InvalidDataException>(() => Load(With(save, Kinds + 4, (int)CauseKind.ExogenousRoot)), "a root that names a cause");
            Assert.Throws<InvalidDataException>(() => Load(With(save, Causes + 4, 2)), "a cause pointing at itself");
            Assert.Throws<InvalidDataException>(() => Load(With(save, Starts + 4, 0)), "a pool that is not contiguous");
            Assert.Throws<InvalidDataException>(() => Load(With(save, Rows, int.MaxValue)), "more rows than the save holds");
            Assert.That(BitConverter.ToInt64(save, Ticks + 8), Is.EqualTo(43), "the offsets read the second row's tick, the world's own");
            Assert.Throws<InvalidDataException>(() => Load(With(save, Ticks + 8, 44)), "an entry dated after the world");
            Assert.Throws<InvalidDataException>(() => Load(With(save, Ticks + 8, 40)), "an entry dated before the one preceding it");
            Assert.Throws<InvalidDataException>(() => Load(With(With(save, 8, -1), 12, -1)), "a negative tick");
        }

        /// <summary>
        /// The bytes of one save, pinned. Nothing else here would notice a field reordered,
        /// a width changed or the header moved, each of which leaves every round trip green
        /// and every save already written unreadable. A change here is a new format
        /// version, with a reader for the old one (NFR-08).
        /// </summary>
        [Test]
        public void NFR08_TheFormatIsPinned()
        {
            byte[] save = SaveFormat.Write(Saved(), Mixed());
            Assert.That(save.Length, Is.EqualTo(8 + 32 + 4 + 2 * 36 + 3 * 8 + 4 + 3 * 36));
            Assert.That(
                Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(save)),
                Is.EqualTo("390C267E9AE2329DF536A2DD6CB9810A4C6133DC4BE927E5B624263FB3DB7343"));
        }

        private static void AssertRoundTrip(WorldState state, CommandQueue commands, string what)
        {
            byte[] first = SaveFormat.Write(state, commands);
            var restored = new CommandQueue();
            WorldState loaded = SaveFormat.Read(first, restored);

            Assert.That(SaveFormat.Write(loaded, restored), Is.EqualTo(first), what);
            Assert.That(StateHash.Of(loaded), Is.EqualTo(StateHash.Of(state)), what + ": the same world");
            Assert.That(restored.Count, Is.EqualTo(commands.Count), what + ": the same commands");
        }

        /// <summary>Where the rule version sits: after the header, the tick, the seed and the settlement count.</summary>
        private const int RuleVersionAt = 8 + 8 + 8 + 4;

        /// <summary>A command of another kind with every field in use and distinct, which is only saved, never drained.</summary>
        private static readonly Command Wide = new Command(7, 11, 13, 17, 19);

        /// <summary><see cref="StateHashTests.Reference"/>, at a rule version this build loads.</summary>
        private static WorldState Saved()
        {
            WorldState state = StateHashTests.Reference();
            state.RuleVersion = Simulation.CurrentRuleVersion;
            return state;
        }

        private static CommandQueue Mixed()
        {
            CommandQueue queue = Pending(3);
            queue.Submit(Wide);
            queue.Submit(CommandQueueTests.Fold(4));
            return queue;
        }

        private static CommandQueue Pending(params long[] values)
        {
            var queue = new CommandQueue();
            foreach (long v in values)
            {
                queue.Submit(CommandQueueTests.Fold(v));
            }

            return queue;
        }

        private static void Load(byte[] save) => SaveFormat.Read(save, new CommandQueue());

        /// <summary>A copy of <paramref name="save"/> with the integer at <paramref name="offset"/> replaced.</summary>
        private static byte[] With(byte[] save, int offset, int value)
        {
            byte[] copy = (byte[])save.Clone();
            BitConverter.GetBytes(value).CopyTo(copy, offset);
            return copy;
        }
    }
}
