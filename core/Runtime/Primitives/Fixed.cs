namespace Sim
{
    /// <summary>
    /// Fixed-point arithmetic over <see cref="int"/> and <see cref="long"/> (DEC-001).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Scale is <see cref="One"/> = 10000: a value of 10000 reads as 1, of 2500 as 0.25.
    /// This is the same 0–10000 scale the specification gives normalised quantities,
    /// so a rate, a share, and a climate index are all the same kind of number here.
    /// </para>
    /// <para>
    /// <b>Rounding rule, applied at every division in this file: toward negative
    /// infinity (floor).</b> C# divides by truncating toward zero, which means the rule
    /// changes with the sign of the operands and a quantity crossing zero rounds in two
    /// directions. Floor does not: the result never rises, the remainder left behind is
    /// never negative, and that remainder is exactly what
    /// <see cref="RemainderAccumulator"/> carries to the next update. A call site that
    /// needs a different rule has to say so with its own code and a comment; nothing
    /// here rounds any other way.
    /// </para>
    /// <para>
    /// The <see cref="int"/> overloads widen to <see cref="long"/> before multiplying, so
    /// no intermediate overflows. The <see cref="long"/> overloads cannot widen further:
    /// the caller keeps <c>value * numerator</c> inside 64 bits. Invariant A-11 asserts
    /// that at state level.
    /// </para>
    /// </remarks>
    public static class Fixed
    {
        /// <summary>1 in fixed point. Normalised quantities live on 0..One.</summary>
        public const int One = 10000;

        /// <summary>a / b, rounded toward negative infinity.</summary>
        public static long DivFloor(long a, long b)
        {
            long q = a / b;
            // C# truncated toward zero. With opposite signs and a remainder left over
            // that truncation rounded up; step back down.
            if (a % b != 0 && (a < 0) != (b < 0))
            {
                q--;
            }
            return q;
        }

        /// <summary>a / b, rounded toward negative infinity.</summary>
        public static int DivFloor(int a, int b) => (int)DivFloor((long)a, b);

        /// <summary>value * numerator / denominator, rounded toward negative infinity.</summary>
        public static long MulDiv(long value, long numerator, long denominator)
            => DivFloor(value * numerator, denominator);

        /// <summary>value * numerator / denominator, rounded toward negative infinity.</summary>
        public static int MulDiv(int value, int numerator, int denominator)
            => (int)DivFloor((long)value * numerator, denominator);

        /// <summary>Fixed-point product of two fixed-point values, rounded toward negative infinity.</summary>
        public static long Mul(long a, long b) => MulDiv(a, b, One);

        /// <summary>Fixed-point product of two fixed-point values, rounded toward negative infinity.</summary>
        public static int Mul(int a, int b) => MulDiv(a, b, One);

        /// <summary>Fixed-point quotient of two fixed-point values, rounded toward negative infinity.</summary>
        public static long Div(long a, long b) => MulDiv(a, One, b);

        /// <summary>Fixed-point quotient of two fixed-point values, rounded toward negative infinity.</summary>
        public static int Div(int a, int b) => MulDiv(a, One, b);
    }
}
