namespace Sim
{
    /// <summary>
    /// The only source of randomness in the core (DEC-002, NFR-03): a 64-bit value
    /// indexed by <c>(worldSeed, entityId, tick, channel, index)</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// There is no stream and no state. Every draw is a pure function of its five
    /// coordinates, so calls may happen in any order, be skipped, or be repeated years
    /// later without changing what any other draw returns. That is what makes a deferred
    /// agent resumable in constant time and what keeps a new subsystem from
    /// desynchronising the existing ones.
    /// </para>
    /// <para>
    /// <paramref name="index"/> is the draw number within one entity, tick, and channel.
    /// An algorithm needing several values takes several indices; one needing a variable
    /// number of them has to be rewritten to a fixed count.
    /// </para>
    /// <para>
    /// The mixing function is written out here in integer operations rather than taken
    /// from the base class library, because the result has to be identical on every
    /// build and every machine, and because <c>string</c> and object hash codes are
    /// randomised per process.
    /// </para>
    /// </remarks>
    public static class Hash64
    {
        // SplitMix64's finaliser: three multiply-xorshift rounds with these constants
        // avalanche every input bit across all 64 output bits.
        private const ulong GoldenGap = 0x9E3779B97F4A7C15UL;
        private const ulong MixA = 0xBF58476D1CE4E5B9UL;
        private const ulong MixB = 0x94D049BB133111EBUL;

        /// <summary>The subject of a draw that belongs to the world rather than to anything in it.</summary>
        public const ulong NoSubject = 0;

        /// <summary>
        /// The draw at <c>(worldSeed, entity, tick, channel, index)</c>. Uniform over the
        /// full 64-bit range; identical for identical coordinates, on any build.
        /// </summary>
        public static ulong Of(ulong worldSeed, EntityId entity, long tick, HashChannel channel, int index)
            => Of(worldSeed, ((ulong)(uint)entity.Generation << 32) | (uint)entity.Index, tick, channel, index);

        /// <summary>
        /// The same draw for a subject that is not an entity: a link, an event, a sample.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Not everything that draws occupies a row with a generation. SIM-STATE enumerates
        /// sampled transients from <c>Hash(worldSeed, link, index)</c>, and a link is a row
        /// in the edge arrays; world generation and events draw against no row at all. The
        /// entity overload is this one with the handle packed into the subject, so the two
        /// share one coordinate space instead of dividing the hash in two.
        /// </para>
        /// <para>
        /// Build the key with <see cref="Subject"/>, or use <see cref="NoSubject"/>. Two
        /// different kinds of non-entity subject drawing in the same channel share one key
        /// space and would have to divide it by agreement; giving them separate channels is
        /// the answer that does not depend on anyone remembering.
        /// </para>
        /// </remarks>
        public static ulong Of(ulong worldSeed, ulong subject, long tick, HashChannel channel, int index)
        {
            unchecked
            {
                // The coordinates fold in one at a time, each through a full avalanche, so
                // that no two different coordinate tuples cancel out into the same state.
                ulong h = Mix(worldSeed + GoldenGap);
                h = Mix(h ^ subject);
                h = Mix(h ^ (ulong)tick);
                h = Mix(h ^ (uint)(int)channel);
                h = Mix(h ^ (uint)index);
                return h;
            }
        }

        /// <summary>
        /// Subject key for a row that is not an entity.
        /// </summary>
        /// <remarks>
        /// The high half stays zero. That is the generation no live <see cref="EntityId"/>
        /// carries, so a row key and a live entity key cannot name the same draw, and a
        /// negative row cannot sign-extend into one either. <c>Subject(0)</c> is
        /// <see cref="NoSubject"/>, which is <see cref="EntityId.None"/> packed.
        /// </remarks>
        public static ulong Subject(int row) => (uint)row;

        /// <summary>
        /// Reduces a draw to <c>0 .. count - 1</c>, so that no caller writes the modulo
        /// itself and each one asks about the bias once, here.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The reduction is a plain modulo, and the next reader is going to ask about the
        /// bias, so: 2^64 is not a multiple of <paramref name="count"/>, and the lowest
        /// <c>2^64 mod count</c> results therefore come up once more often than the rest.
        /// The relative excess is at most <c>count / 2^64</c> — 5e-17 for an interval of a
        /// thousand. Seeing it would take on the order of 2^64 draws; sixty years of world
        /// time across two thousand settlements produce on the order of 1e12. This is not a
        /// small bias, it is one that cannot be observed in principle.
        /// </para>
        /// <para>
        /// Rejecting the tail and re-mixing would not buy exactness anyway. <see cref="Mix"/>
        /// is a bijection, so it relocates the discarded set onto another set of the same
        /// size rather than spreading it evenly, leaving a deviation of the same order; and
        /// iterating a bijection is a permutation, so the loop has no proof of termination.
        /// </para>
        /// <para>Callers wanting <c>min .. max</c> write <c>min + Range(draw, max - min + 1)</c>.</para>
        /// </remarks>
        public static int Range(ulong draw, int count)
        {
            System.Diagnostics.Debug.Assert(count > 0, "Range needs a non-empty interval.");

            return (int)(draw % (ulong)count);
        }

        private static ulong Mix(ulong z)
        {
            unchecked
            {
                z = (z ^ (z >> 30)) * MixA;
                z = (z ^ (z >> 27)) * MixB;
                return z ^ (z >> 31);
            }
        }
    }
}
