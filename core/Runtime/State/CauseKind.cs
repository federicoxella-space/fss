namespace Sim
{
    /// <summary>
    /// Which of FR-E-07's three causes a chronicle entry has (SIM-STATE §Chronicle,
    /// DEC-026, R-024).
    /// </summary>
    /// <remarks>
    /// Zero is none of them, so a row left at its default is not silently read as one of
    /// the three: <see cref="Chronicle.Append"/> refuses it, and so does a load. The
    /// values are written into every save and cannot be renumbered without a migration.
    /// </remarks>
    internal enum CauseKind
    {
        /// <summary>Triggered by an earlier entry, which the entry's cause names.</summary>
        Event = 1,

        /// <summary>A player's action. The entry does not point at the command, which is input (R-021).</summary>
        PlayerAction = 2,

        /// <summary>A root: nothing in the world caused it.</summary>
        ExogenousRoot = 3,
    }
}
