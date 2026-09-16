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
    ///
    /// Every kind of non-entity subject that draws gets its own channel. Entity keys
    /// separate themselves, because <c>EntityId</c> carries a generation no other key
    /// uses; two kinds of non-entity subject — links and events, say — do not, and in a
    /// shared channel row 7 of one is row 7 of the other, drawing the same value. The
    /// channel is what keeps their key spaces apart. See <see cref="Hash64.Subject"/>.
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
