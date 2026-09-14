namespace Sim
{
    /// <summary>
    /// Applies a rate to an integer stock while carrying the fractional part forward,
    /// so that small stocks change at the same proportional rate as large ones.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A 2% rate on a stock of 10 is 0.2, which floors to nothing. Applied straight, a
    /// small stock never spoils, never wears, never pays interest, and becomes immortal
    /// while a large one decays normally. The fix is to keep the fraction: the leftover
    /// numerator carries to the next update and is spent when it reaches a whole unit.
    /// This is a codebase-wide rule, not a spoilage detail — see SIM-ECON.
    /// </para>
    /// <para>
    /// The carry belongs to the caller, held in state next to the stock it belongs to,
    /// because anything that influences a future tick lives in <c>WorldState</c> and in
    /// the state hash. Nothing is stored here.
    /// </para>
    /// <para>
    /// Conservation, which is what makes this safe under AC-01:
    /// <c>value * numerator + carryBefore == result * denominator + carryAfter</c>,
    /// exactly, at every call. The carry stays in <c>[0, denominator)</c> for a positive
    /// denominator, so it cannot drift and no unit is created or lost by rounding.
    /// </para>
    /// </remarks>
    public static class RemainderAccumulator
    {
        /// <summary>
        /// Returns the whole units of <c>value * numerator / denominator</c> and leaves the
        /// remaining numerator in <paramref name="carry"/>. Rounds toward negative infinity.
        /// </summary>
        public static int Apply(int value, int numerator, int denominator, ref int carry)
        {
            long total = (long)value * numerator + carry;
            long whole = Fixed.DivFloor(total, denominator);
            // 0 <= remainder < denominator, so it fits the int the state holds.
            carry = (int)(total - whole * denominator);
            return (int)whole;
        }

        /// <summary>
        /// Returns the whole units of <c>value * numerator / denominator</c> and leaves the
        /// remaining numerator in <paramref name="carry"/>. Rounds toward negative infinity.
        /// The caller keeps <c>value * numerator</c> inside 64 bits.
        /// </summary>
        public static long Apply(long value, long numerator, long denominator, ref long carry)
        {
            long total = value * numerator + carry;
            long whole = Fixed.DivFloor(total, denominator);
            carry = total - whole * denominator;
            return whole;
        }

        /// <summary>Applies a fixed-point rate on the <see cref="Fixed.One"/> scale.</summary>
        public static int ApplyRate(int value, int rate, ref int carry)
            => Apply(value, rate, Fixed.One, ref carry);

        /// <summary>Applies a fixed-point rate on the <see cref="Fixed.One"/> scale.</summary>
        public static long ApplyRate(long value, long rate, ref long carry)
            => Apply(value, rate, Fixed.One, ref carry);
    }
}
