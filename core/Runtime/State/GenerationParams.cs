namespace Sim
{
    /// <summary>
    /// The generator's inputs, carried inside <see cref="WorldState"/> so that a save
    /// reproduces the world it came from (SIM-STATE §World, FR-G-02, NFR-08).
    /// </summary>
    /// <remarks>
    /// <para>
    /// SIM-STATE calls this a record, meaning a composite value rather than the C#
    /// keyword: a <c>record</c> is a class, and NFR-10 keeps object references out of
    /// serialised state. A <c>readonly struct</c> is the composite that also satisfies
    /// the layout rule, and it is what the field walker of
    /// <c>NFR10_StateHoldsNoObjectReferences</c> is prepared to descend into.
    /// </para>
    /// <para>
    /// The generator's seed is not here. SIM-STATE gives <c>worldSeed</c> as "also the
    /// RNG root", so FR-G-02's seed is that field and this one holds the rest.
    /// </para>
    /// <para>
    /// <b>One parameter, not the table.</b> Phase 3 has no generator, and enumerating
    /// what one would want is phase 4's decision to make. <see cref="SettlementCount"/>
    /// is here because it is the one generation parameter the state layout itself has a
    /// stake in, and because §16 fixes it as Decided at P-02 = 2,000. Map extent and
    /// inhabited fraction are map geometry, which nothing in phase 3 touches and neither
    /// of which can be stored without inventing a unit and a fixed-point scale that
    /// <c>docs/</c> does not give. They arrive with the generator, under the migration
    /// path NFR-08 requires from the first write.
    /// </para>
    /// </remarks>
    public readonly struct GenerationParams
    {
        /// <summary>
        /// P-02: how many settlements the generator places. Every settlement array is
        /// this long for the whole run — that is FR-W-10 ("settlement count is fixed for
        /// the run") and DEC-067, not P-02, which fixes only the default value.
        /// </summary>
        public readonly int SettlementCount;

        public GenerationParams(int settlementCount)
        {
            SettlementCount = settlementCount;
        }
    }
}
