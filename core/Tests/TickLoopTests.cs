using System.Collections.Generic;
using NUnit.Framework;

namespace Sim.Tests
{
    /// <summary>
    /// A-13, FR-T-04, FR-T-06 and FR-T-09. The loop's levels are empty in phase 3, so what
    /// is under test is the schedule itself: which level fires on which tick, counted over
    /// every tick of the span rather than sampled.
    /// </summary>
    [TestFixture]
    public sealed class TickLoopTests
    {
        /// <summary>The span the plan's criterion names.</summary>
        private const int Span = 100_000;

        /// <summary>
        /// Not a multiple of seven, so buckets 0 and 1 hold five settlements and the other
        /// five hold four: an off-by-one in where the bucket starts or stops shows up as a
        /// settlement that never fires or fires in the wrong week.
        /// </summary>
        private const int Settlements = 30;

        /// <summary>
        /// A-13: each cadence bucket fires exactly once per its period. Every firing of
        /// every level over 100k ticks is counted into its period; complete periods must
        /// hold exactly one, and the trailing partial period at most one.
        /// </summary>
        /// <remarks>
        /// A settlement bucket is the set of ids with the same <c>id % 7</c>. It fires once
        /// per week when each of its settlements fires once per week on a tick with
        /// <c>tick % 7 == id % 7</c>: a week holds one such tick, so the settlements of a
        /// bucket cannot fire on different days of it.
        /// </remarks>
        [Test]
        public void A13_EachCadenceBucketFiresOncePerPeriod()
        {
            // FR-T-04: each cadence divides the next.
            Assert.That(Calendar.DaysPerMonth % Calendar.DaysPerWeek, Is.Zero);
            Assert.That(Calendar.DaysPerYear % Calendar.DaysPerMonth, Is.Zero);

            var counts = new Counts(Settlements, Span);
            var state = NewWorld(Settlements);

            TickLoop.Advance(state, Span, counts);

            Assert.That(state.Tick, Is.EqualTo(Span));
            Assert.That(counts.Misplaced, Is.Empty, "settlements fired off their bucket (FR-T-06)");

            ExactlyOncePerPeriod("daily", counts.Daily, 1);
            ExactlyOncePerPeriod("basin", counts.Basin, Calendar.DaysPerMonth);
            ExactlyOncePerPeriod("kingdom", counts.Kingdom, Calendar.DaysPerYear);
            for (int id = 0; id < Settlements; id++)
            {
                var perWeek = new int[counts.Settlement.GetLength(1)];
                for (int w = 0; w < perWeek.Length; w++)
                {
                    perWeek[w] = counts.Settlement[id, w];
                }

                ExactlyOncePerPeriod("settlement " + id, perWeek, Calendar.DaysPerWeek);
            }
        }

        /// <summary>
        /// FR-T-04: each level fires exactly its period apart, from a first firing inside
        /// its first period. Not named by the plan's criterion: A-13's once-per-period
        /// count passes a kingdom firing every 365 ticks over 100k ticks, since the first
        /// year it skips starts at tick 132,860. The drift DEC-006a exists to rule out.
        /// </summary>
        [Test]
        public void FRT04_ConsecutiveFiringsOfALevelAreOnePeriodApart()
        {
            var log = new Log(new List<long>());
            TickLoop.Advance(NewWorld(Settlements), Span, log);

            // Keyed by the log's level code; the test project may use a Dictionary, the core may not.
            var last = new Dictionary<long, long>();
            foreach (long f in log.Firings)
            {
                long tick = f / 100_000, code = f % 100_000;
                if (last.TryGetValue(code, out long previous))
                {
                    Assert.That(tick, Is.EqualTo(previous + Period(code)), "level " + code + " after tick " + previous);
                }
                else
                {
                    Assert.That(tick, Is.LessThan(Period(code)), "level " + code + " first fired late");
                }

                last[code] = tick;
            }

            Assert.That(last.Count, Is.EqualTo(3 + Settlements), "a level or settlement never fired");
            foreach (var entry in last)
            {
                Assert.That(entry.Value, Is.AtLeast(Span - Period(entry.Key)), "level " + entry.Key + " stopped early");
            }
        }

        /// <summary>The period of a <see cref="Log"/> level code.</summary>
        private static long Period(long code) =>
            code == 0 ? 1 : code == 1 ? Calendar.DaysPerMonth : code == 2 ? Calendar.DaysPerYear : Calendar.DaysPerWeek;

        /// <summary>
        /// FR-T-09: the caller decides how many ticks run. The loop runs exactly the count
        /// it is given, zero included, and cutting a run into calls of any size fires the
        /// same levels on the same ticks as one call.
        /// </summary>
        [Test]
        public void FRT09_TheCallerDecidesHowManyTicksRun()
        {
            const int total = 3 * Calendar.DaysPerYear + 5;

            var whole = new Log(new List<long>());
            var once = NewWorld(Settlements);
            TickLoop.Advance(once, total, whole);

            var cut = new Log(new List<long>());
            var inPieces = NewWorld(Settlements);
            long[] pieces = { 0, 1, 6, 0, 21, 27, Calendar.DaysPerYear, 2, Calendar.DaysPerYear - 1, 1 };
            long ran = 0;
            foreach (long n in pieces)
            {
                long before = inPieces.Tick;
                TickLoop.Advance(inPieces, n, cut);
                Assert.That(inPieces.Tick - before, Is.EqualTo(n), "a call of " + n + " ticks");
                ran += n;
            }

            TickLoop.Advance(inPieces, total - ran, cut);

            Assert.That(inPieces.Tick, Is.EqualTo(once.Tick));
            Assert.That(cut.Firings, Is.EqualTo(whole.Firings));

            // The entry phase 3 runs, with every level empty, counts the same.
            var empty = NewWorld(Settlements);
            TickLoop.Advance(empty, total);
            Assert.That(empty.Tick, Is.EqualTo(once.Tick));
        }

        private static WorldState NewWorld(int settlements) =>
            new WorldState(worldSeed: 1, new GenerationParams(settlements));

        private static void ExactlyOncePerPeriod(string level, int[] perPeriod, int period)
        {
            int complete = Span / period;
            for (int p = 0; p < complete; p++)
            {
                Assert.That(perPeriod[p], Is.EqualTo(1), level + ", period " + p + " of " + period + " ticks");
            }

            for (int p = complete; p < perPeriod.Length; p++)
            {
                Assert.That(perPeriod[p], Is.LessThanOrEqualTo(1), level + ", trailing period " + p);
            }
        }

        /// <summary>Counts every firing into the period it falls in.</summary>
        private readonly struct Counts : ICadenceLevels
        {
            public readonly int[] Daily;
            public readonly int[,] Settlement;
            public readonly int[] Basin;
            public readonly int[] Kingdom;
            public readonly List<string> Misplaced;

            public Counts(int settlements, int span)
            {
                Daily = new int[span];
                Settlement = new int[settlements, Periods(span, Calendar.DaysPerWeek)];
                Basin = new int[Periods(span, Calendar.DaysPerMonth)];
                Kingdom = new int[Periods(span, Calendar.DaysPerYear)];
                Misplaced = new List<string>();
            }

            void ICadenceLevels.Daily(WorldState state) => Daily[state.Tick]++;

            void ICadenceLevels.Settlement(WorldState state, int id)
            {
                if (state.Tick % Calendar.DaysPerWeek != id % Calendar.DaysPerWeek)
                {
                    Misplaced.Add("settlement " + id + " on tick " + state.Tick);
                }

                Settlement[id, state.Tick / Calendar.DaysPerWeek]++;
            }

            void ICadenceLevels.Basin(WorldState state) => Basin[state.Tick / Calendar.DaysPerMonth]++;

            void ICadenceLevels.Kingdom(WorldState state) => Kingdom[state.Tick / Calendar.DaysPerYear]++;

            private static int Periods(int span, int period) => (span + period - 1) / period;
        }

        /// <summary>Records every firing as one number: tick, level and settlement id.</summary>
        private readonly struct Log : ICadenceLevels
        {
            public readonly List<long> Firings;

            public Log(List<long> firings) => Firings = firings;

            public void Daily(WorldState state) => Firings.Add(state.Tick * 100_000 + 0);

            public void Settlement(WorldState state, int id) => Firings.Add(state.Tick * 100_000 + 10 + id);

            public void Basin(WorldState state) => Firings.Add(state.Tick * 100_000 + 1);

            public void Kingdom(WorldState state) => Firings.Add(state.Tick * 100_000 + 2);
        }
    }
}
