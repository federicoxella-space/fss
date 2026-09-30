namespace Sim
{
    /// <summary>
    /// The core's public entry: a world the host can create, advance and hash, and
    /// nothing it can read or write in place (FR-A-01, FR-A-02, R-003).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The state and the queue are private. What crosses this surface is values: the
    /// seed and generation parameters in, a count of ticks, the state hash out. That is
    /// R-003's shape — "a public handle with nothing mutable on it" — and what point 8's
    /// runner needs to print the hash sequence.
    /// </para>
    /// <para>
    /// <b>There is no public submit.</b> A command is a value (DEC-086), but phase 3
    /// defines no kind a host could issue. The drain runs anyway, so the tick has its
    /// defined point from the first write; the host's way in arrives with the first kind.
    /// </para>
    /// <para>
    /// A new world takes the core's own rule version, not one the host chooses; a loaded
    /// one keeps the version its save records (R-004).
    /// </para>
    /// <para>
    /// <b>A save is bytes</b>, the world and the commands still waiting (NFR-08, R-021).
    /// Where they go is the host's: the core touches no file.
    /// </para>
    /// </remarks>
    public sealed class Simulation
    {
        /// <summary>
        /// The rule version this build produces. Phase 3 has no rules to version; this
        /// is the first.
        /// </summary>
        internal const int CurrentRuleVersion = 1;

        private readonly WorldState state;
        private readonly CommandQueue commands;

        /// <summary>A world at tick 0 with nothing in it.</summary>
        public Simulation(ulong worldSeed, GenerationParams generationParams)
            : this(new WorldState(worldSeed, generationParams), new CommandQueue())
        {
        }

        private Simulation(WorldState state, CommandQueue commands)
        {
            this.state = state;
            this.commands = commands;
        }

        /// <summary>
        /// The world a save holds, with its pending commands back in the queue (NFR-08,
        /// AC-03, R-021). Throws <see cref="System.IO.InvalidDataException"/> on bytes that
        /// are not a save this build reads.
        /// </summary>
        public static Simulation Load(byte[] save)
        {
            if (save == null)
            {
                throw new System.ArgumentNullException(nameof(save));
            }

            var commands = new CommandQueue();
            return new Simulation(SaveFormat.Read(save, commands), commands);
        }

        /// <summary>The whole world and the commands still waiting, as bytes (NFR-08, DEC-032).</summary>
        public byte[] Save() => SaveFormat.Write(state, commands);

        /// <summary>The digest of the whole state, as a value (NFR-01, AC-02).</summary>
        public ulong StateHash => Sim.StateHash.Of(state);

        /// <summary>Runs exactly <paramref name="ticks"/> ticks (FR-T-09).</summary>
        public void Advance(long ticks)
        {
            if (ticks < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(ticks), "Time does not go back.");
            }

            TickLoop.Advance(state, commands, ticks);
        }
    }
}
