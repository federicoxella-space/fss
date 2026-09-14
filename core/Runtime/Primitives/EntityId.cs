namespace Sim
{
    /// <summary>
    /// Handle into the struct-of-arrays state (DEC-003, NFR-10): a row index plus
    /// the generation that row carried when the handle was taken.
    /// </summary>
    /// <remarks>
    /// A row is reused when the entity occupying it dies. Bumping its generation on
    /// reuse is what makes a handle kept across that death resolve as stale rather
    /// than silently resolving to the entity that took its place, which matters in a
    /// world with generational turnover.
    ///
    /// Live handles carry a generation of at least 1, so <see cref="None"/> — index 0,
    /// generation 0 — is never a live row and a defaulted field reads as absent.
    /// Serialised state holds these and no object references.
    /// </remarks>
    public readonly struct EntityId : System.IEquatable<EntityId>
    {
        /// <summary>The absent handle.</summary>
        public static readonly EntityId None = default;

        /// <summary>Row index into the parallel arrays of the owning entity type.</summary>
        public readonly int Index;

        /// <summary>Generation of that row when this handle was taken. 0 means absent.</summary>
        public readonly int Generation;

        public EntityId(int index, int generation)
        {
            Index = index;
            Generation = generation;
        }

        public bool IsNone => Generation == 0;

        public bool Equals(EntityId other) => Index == other.Index && Generation == other.Generation;

        public override bool Equals(object obj) => obj is EntityId other && Equals(other);

        public override int GetHashCode() => (Index * 397) ^ Generation;

        public static bool operator ==(EntityId a, EntityId b) => a.Equals(b);

        public static bool operator !=(EntityId a, EntityId b) => !a.Equals(b);

        public override string ToString() => Index.ToString() + ":" + Generation.ToString();
    }
}
