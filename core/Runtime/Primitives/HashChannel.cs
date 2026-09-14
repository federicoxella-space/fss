namespace Sim
{
    /// <summary>
    /// One channel per subsystem that draws (DEC-002, NFR-03). Separate channels are
    /// what let a subsystem be added, removed, or changed without moving anyone else's
    /// numbers, since no sequential stream is shared.
    /// </summary>
    /// <remarks>
    /// The numeric values are part of the determinism contract: a save replays with the
    /// draws it was written with. Entries may be added at the end; none may be
    /// renumbered, reordered, or reused for a different subsystem.
    ///
    /// Numbering starts at 1 so that a defaulted field is not a valid channel.
    /// </remarks>
    public enum HashChannel
    {
        WorldGen = 1,
        Demography = 2,
        Production = 3,
        Market = 4,
        Labour = 5,
        Needs = 6,
        Mobility = 7,
        Events = 8,
        Transients = 9,
        Knowledge = 10,
        AgentSampling = 11,
    }
}
