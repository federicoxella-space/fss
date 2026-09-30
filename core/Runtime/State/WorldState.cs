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
    /// <b>The World row and the chronicle, and nothing else.</b> Settlements,
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
    /// The fields are mutable because systems mutate state and nothing else does
    /// (AGENTS.md). Hiding them behind properties would suggest this class defends an
    /// invariant, and it does not: the invariants of SIM-STATE are asserted over the
    /// whole state each tick, not at the point of assignment.
    /// </para>
    /// <para>
    /// <b>The type is <c>internal</c>, and that is FR-A-01 and FR-A-02 enforced by the
    /// compiler (R-003).</b> No public member of the core can take or return it, so code
    /// outside the core can neither write the live state nor read it. The host writes
    /// through the command queue, which the tick loop drains at one point of the tick.
    /// </para>
    /// </remarks>
    internal sealed class WorldState
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

        // The chronicle (FR-I-01, FR-I-04, SIM-STATE §Chronicle), one row per entry in
        // append order, under DEC-085: the columns are sized at a capacity and only the
        // first ChronicleCount rows are state. An entry's id is its row plus one, so 0
        // is "no entry". Entities are variable in length and live in one shared pool,
        // each row naming its slice by start and count. Only Systems/Chronicle.cs
        // appends to these, and Systems/SaveFormat.cs fills them on a load. Propagation,
        // the last row of SIM-STATE §Chronicle, is domain and arrives with phase 4.

        /// <summary>Rows of the chronicle in use; also the id of the last entry.</summary>
        public int ChronicleCount;

        /// <summary>The tick each entry was written on.</summary>
        public long[] ChronicleTick = new long[0];

        /// <summary>Where each entry happened: a settlement, or <see cref="EntityId.None"/>.</summary>
        public EntityId[] ChronicleLocation = new EntityId[0];

        /// <summary>Which of FR-E-07's three causes each entry has (R-024).</summary>
        public CauseKind[] ChronicleCauseKind = new CauseKind[0];

        /// <summary>The id of the entry that caused each one when its kind is event, 0 otherwise.</summary>
        public int[] ChronicleCause = new int[0];

        /// <summary>What drives each entry's propagation (SIM-STATE §Chronicle, DEC-027).</summary>
        public int[] ChronicleImportance = new int[0];

        /// <summary>Where each entry's entities begin in <see cref="ChronicleEntities"/>.</summary>
        public int[] ChronicleEntityStart = new int[0];

        /// <summary>How many entities each entry names.</summary>
        public int[] ChronicleEntityCount = new int[0];

        /// <summary>The shared pool of entities, addressed by start and count.</summary>
        public EntityId[] ChronicleEntities = new EntityId[0];

        /// <summary>
        /// A world at tick 0 with nothing in it. The two arguments are what the host
        /// supplies; the rule version is this build's (R-004), and <see cref="Tick"/> and
        /// <see cref="CurrencyTotal"/> start where an empty world puts them. Another rule
        /// version enters state only through a load.
        /// </summary>
        public WorldState(ulong worldSeed, GenerationParams generationParams)
        {
            WorldSeed = worldSeed;
            GenerationParams = generationParams;
            RuleVersion = Simulation.CurrentRuleVersion;
        }
    }
}
