namespace Sim
{
    /// <summary>
    /// Tick-to-date arithmetic, and nothing else (FR-T-01, FR-T-02, FR-T-03, FR-T-05,
    /// FR-T-05a; DEC-005, DEC-006, DEC-006a, DEC-006b).
    /// </summary>
    /// <remarks>
    /// <para>
    /// One tick is one world day (DEC-005) and the tick counter is the only clock the
    /// core has. Everything below is a division or a remainder on that counter: there is
    /// no state here, nothing to advance, and nothing to keep in sync.
    /// </para>
    /// <para>
    /// <b>New Year's Day is implemented by its absence.</b> FR-T-03 puts the festival
    /// between the last tick of one year and the first tick of the next, consuming no
    /// tick, and that is exactly what a 364-tick year with no 365th case is. There is no
    /// festival day to skip and no branch to get wrong; <see cref="NewYearsDayFollows"/>
    /// only names the boundary it falls in, for a caller that wants to show it.
    /// </para>
    /// <para>
    /// Season, week and month are zero-based, as the formulas of FR-T-05a give them:
    /// season 0 is the spring that FR-T-05 calls season 1, month 0 is the first of
    /// thirteen. Presentation adds one; the arithmetic does not.
    /// </para>
    /// <para>
    /// Ticks run forward from zero. On a non-negative tick C# truncation and floor
    /// agree, so the six methods below are the six formulas of FR-T-05a literally, with
    /// no rounding rule of their own to state. FR-G-03 says prior history runs "for P-18
    /// years before tick 0", which reads as though it needed negative ticks; DEC-040
    /// settles it the other way — the run "takes the result as tick 0", so prehistory is
    /// relabelled rather than numbered backwards. A negative tick is therefore a state
    /// the simulation cannot occupy, and a debug build says so rather than quietly
    /// stretching year 0 over the 727 ticks from -363 to 363.
    /// </para>
    /// </remarks>
    public static class Calendar
    {
        /// <summary>Ticks in a week (FR-T-02).</summary>
        public const int DaysPerWeek = 7;

        /// <summary>Ticks in a month: four whole weeks (FR-T-02).</summary>
        public const int DaysPerMonth = 28;

        /// <summary>Ticks in a season (FR-T-05).</summary>
        public const int DaysPerSeason = 91;

        /// <summary>Ticks in a year: 13 × 28, and 52 × 7 (FR-T-02, DEC-006).</summary>
        public const int DaysPerYear = 364;

        /// <summary>Months in a year (FR-T-02).</summary>
        public const int MonthsPerYear = 13;

        /// <summary>Years elapsed since the origin. Year 0 is the first.</summary>
        public static long Year(long tick)
        {
            AssertReachable(tick);
            return tick / DaysPerYear;
        }

        /// <summary>Day within the year, 0..363.</summary>
        public static int DayOfYear(long tick)
        {
            AssertReachable(tick);
            return (int)(tick % DaysPerYear);
        }

        /// <summary>Season within the year, 0..3. Season 0 opens the year and is spring.</summary>
        public static int Season(long tick) => DayOfYear(tick) / DaysPerSeason;

        /// <summary>Week within the year, 0..51.</summary>
        public static int Week(long tick) => DayOfYear(tick) / DaysPerWeek;

        /// <summary>Month within the year, 0..12.</summary>
        public static int Month(long tick) => DayOfYear(tick) / DaysPerMonth;

        /// <summary>Day within the month, 0..27.</summary>
        public static int DayOfMonth(long tick) => DayOfYear(tick) % DaysPerMonth;

        /// <summary>
        /// True when New Year's Day falls between <paramref name="tick"/> and the tick
        /// after it: <paramref name="tick"/> is the last of its year (FR-T-03, DEC-006a).
        /// </summary>
        public static bool NewYearsDayFollows(long tick) => DayOfYear(tick) == DaysPerYear - 1;

        /// <summary>
        /// A tick the simulation can reach. Ticks start at zero and only advance, and the
        /// formulas of FR-T-05a read as written only while truncation and floor agree.
        /// </summary>
        [System.Diagnostics.Conditional("DEBUG")]
        private static void AssertReachable(long tick)
        {
            System.Diagnostics.Debug.Assert(tick >= 0, "Ticks run forward from zero. There is no date before the origin.");
        }
    }
}
