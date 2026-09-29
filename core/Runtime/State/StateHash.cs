namespace Sim
{
    /// <summary>
    /// The 64-bit digest of a <see cref="WorldState"/> (NFR-01, AC-02, SIM-STATE
    /// §Serialisation notes).
    /// </summary>
    /// <remarks>
    /// <para>
    /// SIM-STATE: "the state hash covers every field above. A field excluded from the
    /// hash is a determinism hole." That is the whole contract. AC-02 compares hash
    /// sequences across builds and machines, so a field that never reaches this fold
    /// is a field two runs may disagree on while the comparison stays green.
    /// </para>
    /// <para>
    /// <b>The fold is written out, field by field, and that is deliberate.</b> NFR-08
    /// and §20 ban reflection in the core, so nothing here can enumerate. What keeps
    /// the list complete is not this file but
    /// <c>NFR01_EveryStateFieldEntersTheHash</c>, which enumerates from the test
    /// assembly, perturbs each field in turn, and fails when the digest does not move.
    /// A phase-4 array appended to <see cref="WorldState"/> and forgotten here fails
    /// that test without anyone remembering it exists.
    /// </para>
    /// <para>
    /// The mixing is <see cref="Hash64"/>'s, not a second function: the same
    /// multiply-xorshift avalanche, written in integer operations so the digest is
    /// identical on every build. The order of the folds is part of the digest —
    /// reordering them changes every hash — which is why they follow SIM-STATE,
    /// §World then §Chronicle, with one exception: the chronicle's shared pool of
    /// entities is folded after the per-row columns, since its length is read from them.
    /// </para>
    /// <para>
    /// <b>What the fold guarantees, exactly.</b> A state differing from another in
    /// <em>one</em> field can never share its digest. Every cast into the fold widens
    /// and is therefore injective, xor with the accumulator is a bijection, and
    /// <see cref="Hash64.Mix"/> is a bijection, so with the other fields held fixed the
    /// digest is an injective function of each one. The perturbation test below is
    /// therefore exact and not probabilistic. The ceiling: a later fold that
    /// <em>narrows</em> — <c>(ulong)(byte)x</c> — breaks that chain, and a test
    /// perturbing by one would not see it.
    /// </para>
    /// <para>
    /// <b>Three fields fall outside that argument</b>: <c>ChronicleCount</c>, and the
    /// last row's <c>ChronicleEntityStart</c> and <c>ChronicleEntityCount</c>. Each one
    /// decides how many folds follow it, so two values of it give chains of different
    /// lengths and the bijection argument does not apply. For those three a shared
    /// digest is a 64-bit collision, improbable and not impossible.
    /// </para>
    /// <para>
    /// <b>It guarantees nothing about two fields.</b> Two states differing in two
    /// fields can be made to collide in two evaluations of <see cref="Hash64.Mix"/>,
    /// by choosing the second field to absorb the difference the first made to the
    /// accumulator; this is a property of any <c>h = Mix(h ^ x)</c> chain and not a
    /// 64-bit birthday collision. It costs nothing here — AC-02 compares two runs of
    /// the same build, where a divergence is a systematic difference and not an
    /// adversarial one — but it is the reason this is a digest and not a checksum
    /// against tampering.
    /// </para>
    /// </remarks>
    internal static class StateHash
    {
        // An empty fold has to start somewhere, and the value it starts from is
        // arbitrary: no choice of it makes a prefix of the folds distinguishable from a
        // shorter state, because the accumulator is never returned, only the last fold
        // is. It shares its bits with Hash64's golden gap because both wanted a 64-bit
        // constant with no structure and that one was already in the file. Nothing has
        // to keep them equal — they play unrelated roles, and tying this one to the
        // RNG's additive tweak would make a change to the RNG rewrite every saved
        // world's hash. Changing this line changes every hash ever written, which is
        // what NFR01_TheDigestIsPinned is for.
        private const ulong Seed = 0x9E3779B97F4A7C15UL;

        /// <summary>
        /// The digest of every field of <paramref name="state"/>, in the order
        /// SIM-STATE declares them: §World, then §Chronicle.
        /// </summary>
        public static ulong Of(WorldState state)
        {
            unchecked
            {
                ulong h = Seed;
                h = Hash64.Mix(h ^ (ulong)state.Tick);
                h = Hash64.Mix(h ^ state.WorldSeed);
                h = Fold(h, state.GenerationParams);
                h = Hash64.Mix(h ^ (uint)state.RuleVersion);
                h = Hash64.Mix(h ^ (ulong)state.CurrencyTotal);

                // The chronicle, column by column, over the rows in use only: capacity
                // is not state, and two worlds with the same entries hash the same
                // whatever their arrays have grown to.
                int rows = state.ChronicleCount;
                h = Hash64.Mix(h ^ (uint)rows);
                for (int i = 0; i < rows; i++) { h = Hash64.Mix(h ^ (ulong)state.ChronicleTick[i]); }
                for (int i = 0; i < rows; i++) { h = Fold(h, state.ChronicleLocation[i]); }
                for (int i = 0; i < rows; i++) { h = Hash64.Mix(h ^ (uint)state.ChronicleCause[i]); }
                for (int i = 0; i < rows; i++) { h = Hash64.Mix(h ^ (uint)state.ChronicleImportance[i]); }
                for (int i = 0; i < rows; i++) { h = Hash64.Mix(h ^ (uint)state.ChronicleEntityStart[i]); }
                for (int i = 0; i < rows; i++) { h = Hash64.Mix(h ^ (uint)state.ChronicleEntityCount[i]); }

                int entities = Chronicle.EntitiesInUse(state);
                for (int i = 0; i < entities; i++) { h = Fold(h, state.ChronicleEntities[i]); }

                return h;
            }
        }

        /// <summary>A handle, its index then its generation.</summary>
        private static ulong Fold(ulong h, EntityId id)
        {
            unchecked
            {
                h = Hash64.Mix(h ^ (uint)id.Index);
                return Hash64.Mix(h ^ (uint)id.Generation);
            }
        }

        /// <summary>
        /// The members of <see cref="GenerationParams"/>, folded one at a time.
        /// </summary>
        /// <remarks>
        /// Folding the struct as a unit is not an option — there is no unit to fold
        /// without reflection or a layout assumption — and it would not be wanted if
        /// there were: a member added to the struct and not to this method is the same
        /// determinism hole as a field added to <see cref="WorldState"/> and not to
        /// <see cref="Of"/>. The test recurses for exactly that reason.
        /// </remarks>
        private static ulong Fold(ulong h, GenerationParams p)
        {
            unchecked
            {
                return Hash64.Mix(h ^ (uint)p.SettlementCount);
            }
        }
    }
}
