namespace Sim
{
    /// <summary>
    /// The whole simulation state, in one place (FR-W-01, NFR-10, DEC-003,
    /// SIM-STATE §World).
    /// </summary>
    /// <remarks>
    /// <para>
    /// FR-W-01: anything that influences a future tick lives here. A field kept
    /// anywhere else is a determinism hole, and one that never reaches the state hash
    /// is the same hole with a test that passes.
    /// </para>
    /// <para>
    /// <b>This point puts the World row here and nothing else.</b> Settlements,
    /// cohorts, links, stocks, prices, agents and kingdoms are named by FR-W-01 and
    /// belong to phase 4; the chronicle is named by FR-W-01 too but belongs to this
    /// phase, which SIM-REQ §18 lists as "scheduler, RNG, serialisation, chronicle,
    /// CLI runner". All of them arrive as parallel arrays appended to this class,
    /// rather than hidden behind a subsystem type. That is DEC-003: the state
    /// serialises, copies, hashes and compares in bulk, which is what AC-02 and AC-03
    /// ask for, and an object graph does none of those.
    /// </para>
    /// <para>
    /// The container is a class; the contents are not. NFR-10 bans object references
    /// <em>inside</em> serialised state, and the arrays and value types below hold
    /// none — an <see cref="EntityId"/> is how one row names another.
    /// <c>NFR10_StateHoldsNoObjectReferences</c> walks these fields and fails on
    /// anything else, so a later phase cannot reach for a list or a string here
    /// without the suite saying so.
    /// </para>
    /// <para>
    /// The fields are public and mutable because systems mutate state and nothing else
    /// does (AGENTS.md). Hiding them behind properties would suggest this class
    /// defends an invariant, and it does not: the invariants of SIM-STATE are asserted
    /// over the whole state each tick, not at the point of assignment.
    /// </para>
    /// </remarks>
    public sealed class WorldState
    {
        /// <summary>The only clock: one tick is one world day (DEC-005, FR-T-01).</summary>
        public long Tick;

        /// <summary>Root of every random draw, and the generator's seed (DEC-002, FR-G-02).</summary>
        public ulong WorldSeed;

        /// <summary>The generator's other inputs, so a save reproduces its world (FR-G-02, NFR-08).</summary>
        public GenerationParams GenerationParams;

        /// <summary>Which rule version produced this state, driving materialisation on patch (NFR-09, DEC-033).</summary>
        public int RuleVersion;

        /// <summary>Currency in circulation, in smallest units: the target of A-02 and A-03.</summary>
        public long CurrencyTotal;

        /// <summary>
        /// A world at tick 0 with nothing in it. The three arguments are what the host
        /// supplies; <see cref="Tick"/> and <see cref="CurrencyTotal"/> belong to the
        /// simulation and start where an empty world puts them.
        /// </summary>
        public WorldState(ulong worldSeed, GenerationParams generationParams, int ruleVersion)
        {
            WorldSeed = worldSeed;
            GenerationParams = generationParams;
            RuleVersion = ruleVersion;
        }
    }
}
