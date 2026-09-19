using NUnit.Framework;

namespace Sim.Tests
{
    /// <summary>
    /// FR-T-02, FR-T-03 and FR-T-05a. The calendar is arithmetic with no state, so what
    /// is worth testing is not that a call returns something but that the six formulas
    /// hold on every tick of a span rather than on a sample, and that the weekday
    /// regularity DEC-006a buys with a tickless festival is actually there.
    /// </summary>
    [TestFixture]
    public sealed class CalendarTests
    {
        /// <summary>Four consecutive years, as the plan's criterion asks. 1456 ticks.</summary>
        private const int Years = 4;

        private const long Span = (long)Years * Calendar.DaysPerYear;

        /// <summary>
        /// FR-T-02: every calendar date falls on the same weekday in every year.
        /// </summary>
        /// <remarks>
        /// Weekday is not the core's to define and the core does not define it: a week is
        /// seven consecutive ticks and nothing interrupts the stream, so the weekday of a
        /// tick is <c>tick % 7</c>. What is under test is the calendar putting the same
        /// date on the same one of those seven in every year — the regularity DEC-006a
        /// gives up a 365th day to keep, and the one a player learns market days from.
        /// </remarks>
        [Test]
        public void FRT02_EveryDateFallsOnTheSameWeekday()
        {
            var weekdayOfDate = new int[Calendar.MonthsPerYear, Calendar.DaysPerMonth];
            for (int m = 0; m < Calendar.MonthsPerYear; m++)
            {
                for (int d = 0; d < Calendar.DaysPerMonth; d++)
                {
                    weekdayOfDate[m, d] = -1;
                }
            }

            for (long tick = 0; tick < Span; tick++)
            {
                int month = Calendar.Month(tick);
                int dayOfMonth = Calendar.DayOfMonth(tick);
                int weekday = (int)(tick % Calendar.DaysPerWeek);

                if (weekdayOfDate[month, dayOfMonth] < 0)
                {
                    weekdayOfDate[month, dayOfMonth] = weekday;
                    continue;
                }

                Assert.That(
                    weekday,
                    Is.EqualTo(weekdayOfDate[month, dayOfMonth]),
                    "tick " + tick + " is month " + month + " day " + dayOfMonth +
                    ", which fell on another weekday in an earlier year.");
            }

            // A hole here would make the loop above vacuous for the date it missed.
            for (int m = 0; m < Calendar.MonthsPerYear; m++)
            {
                for (int d = 0; d < Calendar.DaysPerMonth; d++)
                {
                    Assert.That(weekdayOfDate[m, d], Is.GreaterThanOrEqualTo(0),
                        "month " + m + " day " + d + " never occurred in " + Years + " years.");
                }
            }
        }

        /// <summary>
        /// FR-T-05a, its six formulas, on every tick of four consecutive years.
        /// </summary>
        /// <remarks>
        /// The expected sides are written with the literal constants of the requirement,
        /// not with <see cref="Calendar"/>'s own named ones. Reusing the named constants
        /// would make the test agree with a wrong constant, which is the single most
        /// likely mistake in a file that is otherwise six divisions.
        /// </remarks>
        [Test]
        public void FRT05a_DateArithmeticIsIntegerDivision()
        {
            for (long tick = 0; tick < Span; tick++)
            {
                long dayOfYear = tick % 364;

                Assert.That(Calendar.Year(tick), Is.EqualTo(tick / 364), "year at tick " + tick);
                Assert.That(Calendar.DayOfYear(tick), Is.EqualTo(dayOfYear), "dayOfYear at tick " + tick);
                Assert.That(Calendar.Season(tick), Is.EqualTo(dayOfYear / 91), "season at tick " + tick);
                Assert.That(Calendar.Week(tick), Is.EqualTo(dayOfYear / 7), "week at tick " + tick);
                Assert.That(Calendar.Month(tick), Is.EqualTo(dayOfYear / 28), "month at tick " + tick);
                Assert.That(Calendar.DayOfMonth(tick), Is.EqualTo(dayOfYear % 28), "dayOfMonth at tick " + tick);

                // The ranges the requirement writes beside the formulas.
                Assert.That(Calendar.DayOfYear(tick), Is.InRange(0, 363));
                Assert.That(Calendar.Season(tick), Is.InRange(0, 3));
                Assert.That(Calendar.Week(tick), Is.InRange(0, 51));
                Assert.That(Calendar.Month(tick), Is.InRange(0, 12));
                Assert.That(Calendar.DayOfMonth(tick), Is.InRange(0, 27));

                // FR-T-02's ratios, as a decomposition rather than as three divisions
                // that happen to be consistent with each other.
                Assert.That(
                    Calendar.Month(tick) * 28 + Calendar.DayOfMonth(tick),
                    Is.EqualTo(dayOfYear),
                    "month and dayOfMonth do not recompose dayOfYear at tick " + tick);
            }
        }

        /// <summary>
        /// FR-T-03 and DEC-006a: the festival sits between two ticks and consumes none.
        /// </summary>
        /// <remarks>
        /// The thing that could go wrong is not the predicate but the year: a festival
        /// implemented as a day would show up as a tick whose date is neither the last of
        /// one year nor the first of the next. Asserting that the year turns over exactly
        /// across the boundary the predicate names is what rules that out.
        /// </remarks>
        [Test]
        public void FRT03_NewYearsDayConsumesNoTick()
        {
            int boundaries = 0;
            for (long tick = 0; tick < Span - 1; tick++)
            {
                bool yearTurns = Calendar.Year(tick + 1) != Calendar.Year(tick);

                Assert.That(Calendar.NewYearsDayFollows(tick), Is.EqualTo(yearTurns),
                    "tick " + tick + ": the festival and the year boundary disagree.");

                if (!yearTurns)
                {
                    continue;
                }

                boundaries++;
                Assert.That(Calendar.DayOfYear(tick), Is.EqualTo(363), "tick " + tick + " ends a year");
                Assert.That(Calendar.DayOfYear(tick + 1), Is.EqualTo(0), "tick " + (tick + 1) + " opens a year");
                Assert.That(Calendar.Year(tick + 1), Is.EqualTo(Calendar.Year(tick) + 1));
            }

            Assert.That(boundaries, Is.EqualTo(Years - 1), "one boundary between each pair of years, and no other.");
        }
    }
}
