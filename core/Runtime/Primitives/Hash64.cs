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

        /// <summary>
        /// The draw at <c>(worldSeed, entity, tick, channel, index)</c>. Uniform over the
        /// full 64-bit range; identical for identical coordinates, on any build.
        /// </summary>
        public static ulong Of(ulong worldSeed, EntityId entity, long tick, HashChannel channel, int index)
        {
            unchecked
            {
                // The coordinates fold in one at a time, each through a full avalanche, so
                // that no two different coordinate tuples cancel out into the same state.
                ulong h = Mix(worldSeed + GoldenGap);
                h = Mix(h ^ (((ulong)(uint)entity.Generation << 32) | (uint)entity.Index));
                h = Mix(h ^ (ulong)tick);
                h = Mix(h ^ (uint)(int)channel);
                h = Mix(h ^ (uint)index);
                return h;
            }
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
