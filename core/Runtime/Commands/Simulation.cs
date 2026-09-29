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
    /// <b>There is no public submit.</b> A command is an <see cref="ICommand"/>, which
    /// takes the internal state, and phase 3 defines no kind a host could issue. The
    /// drain runs anyway, so the tick has its defined point from the first write; the
    /// host's way in arrives with the first command kind.
    /// </para>
    /// <para>
    /// A new world takes the core's own rule version, not one the host chooses (R-004).
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
        private readonly CommandQueue commands = new CommandQueue();

        /// <summary>A world at tick 0 with nothing in it.</summary>
        public Simulation(ulong worldSeed, GenerationParams generationParams)
        {
            state = new WorldState(worldSeed, generationParams, CurrentRuleVersion);
        }

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
