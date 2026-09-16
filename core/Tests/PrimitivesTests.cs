using NUnit.Framework;

namespace Sim.Tests
{
    /// <summary>
    /// NFR-03 and AC-02. The indexed hash is the core's only randomness, so its output
    /// has to be a pure function of its five coordinates and nothing else.
    /// </summary>
    [TestFixture]
    public sealed class Hash64Tests
    {
        private const ulong Seed = 0xA5A51234DEADBEEFUL;

        /// <summary>
        /// AC-02 across builds. These are the values this mixing function produces; a
        /// change to it is a change to every world ever saved, so it fails here first.
        /// </summary>
        [Test]
        public void AC02_HashIsStableForKnownCoordinates()
        {
            var e = new EntityId(7, 3);
            Assert.That(Hash64.Of(Seed, e, 0, HashChannel.Demography, 0), Is.EqualTo(0x1DE0905CE03D803FUL));
            Assert.That(Hash64.Of(Seed, e, 1, HashChannel.Demography, 0), Is.EqualTo(0xF075C8D0F0491831UL));
            Assert.That(Hash64.Of(Seed, e, 0, HashChannel.Market, 0), Is.EqualTo(0x114C50543F9A2C27UL));
            Assert.That(Hash64.Of(Seed, e, 0, HashChannel.Demography, 1), Is.EqualTo(0xB381867AF4EBCC3AUL));
            Assert.That(Hash64.Of(Seed, new EntityId(7, 4), 0, HashChannel.Demography, 0), Is.EqualTo(0x26FB2399389F670EUL));
            Assert.That(Hash64.Of(0, EntityId.None, 0, HashChannel.WorldGen, 0), Is.EqualTo(0x73A3EE95AACE0D70UL));
            Assert.That(
                Hash64.Of(ulong.MaxValue, new EntityId(-1, -1), long.MinValue, HashChannel.AgentSampling, int.MinValue),
                Is.EqualTo(0xB4BA76F6469C3B9DUL));
        }

        /// <summary>
        /// The property the whole design rests on: a subsystem may draw, skip a draw, or
        /// draw again decades later without moving anyone else's numbers.
        /// </summary>
        [Test]
        public void AC02_HashDoesNotDependOnCallOrder()
        {
            const int n = 512;
            var forward = new ulong[n];
            for (int i = 0; i < n; i++)
            {
                forward[i] = Draw(i);
            }

            // Same draws backwards, each one interleaved with unrelated draws from other
            // channels, entities and ticks. A hidden stream would show up here.
            for (int i = n - 1; i >= 0; i--)
            {
                Hash64.Of(Seed ^ 0xFFFF, new EntityId(i + 1000, 9), i * 31, HashChannel.Events, i);
                Assert.That(Draw(i), Is.EqualTo(forward[i]), "draw " + i.ToString() + " moved when replayed out of order");
                Hash64.Of(Seed, EntityId.None, -i, HashChannel.WorldGen, i);
            }

            // And again, repeated: a draw is free to be recomputed any number of times.
            for (int i = 0; i < n; i++)
            {
                Assert.That(Draw(i), Is.EqualTo(forward[i]));
                Assert.That(Draw(i), Is.EqualTo(forward[i]));
            }
        }

        private static ulong Draw(int i)
        {
            return Hash64.Of(
                Seed,
                new EntityId(i % 37 + 1, i % 5 + 1),
                i / 7,
                (HashChannel)(i % 11 + 1),
                i % 13);
        }

        /// <summary>
        /// Every coordinate has to matter on its own, or two subsystems silently share a
        /// stream. Checked by collision over a grid that varies one coordinate at a time.
        /// </summary>
        [Test]
        public void NFR03_EveryCoordinateChangesTheDraw()
        {
            var seen = new System.Collections.Generic.List<ulong>();
            for (int entity = 1; entity <= 12; entity++)
            {
                for (int generation = 1; generation <= 4; generation++)
                {
                    for (long tick = 0; tick < 12; tick++)
                    {
                        for (int channel = 1; channel <= 11; channel++)
                        {
                            for (int index = 0; index < 6; index++)
                            {
                                seen.Add(Hash64.Of(Seed, new EntityId(entity, generation), tick, (HashChannel)channel, index));
                            }
                        }
                    }
                }
            }

            seen.Sort();
            for (int i = 1; i < seen.Count; i++)
            {
                Assert.That(seen[i], Is.Not.EqualTo(seen[i - 1]), "two distinct coordinate tuples returned the same draw");
            }
        }

        /// <summary>A different world seed is a different world, at every coordinate.</summary>
        [Test]
        public void NFR03_SeedSeparatesWorlds()
        {
            for (int i = 0; i < 256; i++)
            {
                var e = new EntityId(i + 1, 1);
                Assert.That(
                    Hash64.Of(Seed, e, i, HashChannel.Production, 0),
                    Is.Not.EqualTo(Hash64.Of(Seed + 1, e, i, HashChannel.Production, 0)));
            }
        }

        /// <summary>
        /// NFR-03 for the things that draw without occupying a row with a generation:
        /// links, events, samples. The subject has to separate them the way the entity
        /// coordinate separates rows, and must not land on a live entity's draw.
        /// </summary>
        [Test]
        public void NFR03_SubjectSeparatesDrawsWithoutAnEntity()
        {
            var seen = new System.Collections.Generic.List<ulong>();
            for (int row = -8; row < 64; row++)
            {
                for (int index = 0; index < 4; index++)
                {
                    seen.Add(Hash64.Of(Seed, Hash64.Subject(row), 5, HashChannel.Transients, index));
                }
            }

            seen.Sort();
            for (int i = 1; i < seen.Count; i++)
            {
                Assert.That(seen[i], Is.Not.EqualTo(seen[i - 1]), "two distinct subjects returned the same draw");
            }

            // Row keys keep the generation half at zero, which no live handle carries.
            for (int row = 0; row < 64; row++)
            {
                for (int generation = 1; generation <= 4; generation++)
                {
                    Assert.That(
                        Hash64.Of(Seed, Hash64.Subject(row), 5, HashChannel.Transients, 0),
                        Is.Not.EqualTo(Hash64.Of(Seed, new EntityId(row, generation), 5, HashChannel.Transients, 0)),
                        "a link drew the same value as a live entity");
                }
            }

            // No subject is the absent handle, and the entity overload is the subject one.
            Assert.That(
                Hash64.Of(Seed, Hash64.NoSubject, 3, HashChannel.Events, 0),
                Is.EqualTo(Hash64.Of(Seed, EntityId.None, 3, HashChannel.Events, 0)));
            Assert.That(Hash64.Subject(0), Is.EqualTo(Hash64.NoSubject));
        }

        /// <summary>
        /// The reduction lands inside the interval and spreads across it.
        /// </summary>
        /// <remarks>
        /// The band is 5% of each bucket, about 5.4 standard deviations of the sampling
        /// noise at this sample size. It is not tightened around exact uniformity and was
        /// never sized for it: the modulo's own deviation is of order 2^-61 here, some
        /// forty orders of magnitude under what a histogram of seventy thousand draws can
        /// resolve. What this catches is a reduction that masks, folds, or drops the top of
        /// the interval — the failures that move whole buckets, not parts per quintillion.
        /// </remarks>
        [Test]
        public void NFR03_RangeCoversTheIntervalEvenly()
        {
            const int count = 7;
            const int draws = 70000;
            var buckets = new int[count];

            for (int i = 0; i < draws; i++)
            {
                int value = Hash64.Range(Hash64.Of(Seed, EntityId.None, 0, HashChannel.WorldGen, i), count);
                Assert.That(value, Is.InRange(0, count - 1));
                buckets[value]++;
            }

            const int expected = draws / count;
            for (int i = 0; i < count; i++)
            {
                Assert.That(buckets[i], Is.InRange(expected - expected / 20, expected + expected / 20), "bucket " + i.ToString());
            }

            // The degenerate interval, which is where an off-by-one in the tail would land.
            for (int i = 0; i < 64; i++)
            {
                Assert.That(Hash64.Range(Hash64.Of(Seed, EntityId.None, i, HashChannel.WorldGen, 0), 1), Is.EqualTo(0));
            }
            Assert.That(Hash64.Range(0, int.MaxValue), Is.EqualTo(0));
        }

    }

    /// <summary>
    /// AC-01. The accumulator is what keeps a rate from creating or destroying units
    /// when it is applied to a small integer stock (SIM-ECON, codebase-wide rule).
    /// </summary>
    [TestFixture]
    public sealed class RemainderAccumulatorTests
    {
        [Test]
        public void AC01_AccumulatorConservesTheTotalOverManyIterations()
        {
            // A 2% rate over a stock that changes size, including through zero and into
            // negative territory, which is what a debit or a correction looks like.
            const int rate = 200;
            int carry = 0;
            long applied = 0;
            long expected = 0;

            for (int i = 0; i < 100000; i++)
            {
                int stock = (i * 7919) % 2001 - 500;
                expected += (long)stock * rate;
                applied += RemainderAccumulator.ApplyRate(stock, rate, ref carry);
                Assert.That(carry, Is.InRange(0, Fixed.One - 1), "carry left its range at iteration " + i.ToString());
            }

            // Nothing was rounded away: every fraction is either spent or still carried.
            Assert.That(applied * Fixed.One + carry, Is.EqualTo(expected));
        }

        [Test]
        public void AC01_AccumulatorConservesTheTotalWithAnArbitraryDenominator()
        {
            const long numerator = 3;
            const long denominator = 7;
            long carry = 0;
            long applied = 0;
            long expected = 0;

            for (int i = 0; i < 10000; i++)
            {
                long stock = i % 13;
                expected += stock * numerator;
                applied += RemainderAccumulator.Apply(stock, numerator, denominator, ref carry);
                Assert.That(carry, Is.InRange(0L, denominator - 1));
            }

            Assert.That(applied * denominator + carry, Is.EqualTo(expected));
        }

        /// <summary>
        /// The integer trap itself: without the carry a 2% rate on a stock of 10 is
        /// nothing, forever, and small stocks become immortal while large ones decay.
        /// </summary>
        [Test]
        public void AC01_SmallStocksAreNotImmortal()
        {
            Assert.That(Fixed.MulDiv(10, 200, Fixed.One), Is.EqualTo(0), "the trap this utility exists to avoid");

            int carry = 0;
            int spoiled = 0;
            for (int update = 0; update < 100; update++)
            {
                spoiled += RemainderAccumulator.ApplyRate(10, 200, ref carry);
            }

            // 100 updates at 2% of 10 units is 20 units, to the unit.
            Assert.That(spoiled, Is.EqualTo(20));
        }

        /// <summary>
        /// The size threshold is gone: over the same span, a small stock loses the same
        /// share as a large one rather than losing nothing.
        /// </summary>
        [Test]
        public void AC01_SmallAndLargeStocksLoseTheSameShare()
        {
            int smallCarry = 0;
            int largeCarry = 0;
            long small = 0;
            long large = 0;
            for (int update = 0; update < 1000; update++)
            {
                small += RemainderAccumulator.ApplyRate(10, 200, ref smallCarry);
                large += RemainderAccumulator.ApplyRate(1000000, 200, ref largeCarry);
            }

            Assert.That(small * 100000, Is.EqualTo(large));
        }

#if DEBUG
        /// <summary>
        /// A-11, debug build only. The guard is compiled out of a release build, which is
        /// what CI runs, so this test only means anything locally. Without it the guard
        /// would be a comment.
        /// </summary>
        [Test]
        public void A11_ApplyRefusesAProductThatLeaves64Bits()
        {
            long carry = 0;
            Assert.Throws<System.OverflowException>(
                () => RemainderAccumulator.Apply(long.MaxValue / 3, 4, Fixed.One, ref carry));

            // The largest product that still fits goes through untouched.
            Assert.DoesNotThrow(() => RemainderAccumulator.Apply(long.MaxValue / 4, 4, Fixed.One, ref carry));
        }
#endif
    }

    /// <summary>
    /// FR-M-07 and NFR-02. Rounding behaviour is defined at every division, and the
    /// definition is: toward negative infinity.
    /// </summary>
    [TestFixture]
    public sealed class FixedTests
    {
        [Test]
        public void FRM07_DivisionRoundsTowardNegativeInfinity()
        {
            Assert.That(Fixed.DivFloor(7, 2), Is.EqualTo(3));
            Assert.That(Fixed.DivFloor(-7, 2), Is.EqualTo(-4));
            Assert.That(Fixed.DivFloor(7, -2), Is.EqualTo(-4));
            Assert.That(Fixed.DivFloor(-7, -2), Is.EqualTo(3));
            Assert.That(Fixed.DivFloor(6, 2), Is.EqualTo(3));
            Assert.That(Fixed.DivFloor(-6, 2), Is.EqualTo(-3));
        }

        /// <summary>
        /// The rule does not change with the sign, which is what C# division does and
        /// what would make a quantity crossing zero round in two directions.
        /// </summary>
        [Test]
        public void FRM07_RoundingDoesNotChangeWithTheSign()
        {
            for (int a = -500; a <= 500; a++)
            {
                for (int b = -17; b <= 17; b++)
                {
                    if (b == 0)
                    {
                        continue;
                    }

                    long q = Fixed.DivFloor(a, b);
                    long remainder = a - q * b;

                    // Floor leaves a remainder with the sign of the divisor and a strictly
                    // smaller magnitude. That remainder is what the accumulator carries.
                    Assert.That(q * b + remainder, Is.EqualTo(a));
                    Assert.That(
                        b > 0 ? remainder >= 0 && remainder < b : remainder <= 0 && remainder > b,
                        Is.True,
                        "remainder out of range for " + a.ToString() + " / " + b.ToString());
                }
            }
        }

        [Test]
        public void FRM07_FixedPointProductAndQuotientRoundTheSameWay()
        {
            Assert.That(Fixed.Mul(Fixed.One / 2, Fixed.One / 2), Is.EqualTo(2500));
            Assert.That(Fixed.Mul(Fixed.One, 12345), Is.EqualTo(12345));
            Assert.That(Fixed.Mul(3, 3), Is.EqualTo(0), "0.0003 squared floors away, as documented");
            Assert.That(Fixed.Mul(-3, 3), Is.EqualTo(-1), "and floors down, not toward zero");
            Assert.That(Fixed.Div(1, 3), Is.EqualTo(3333));
            Assert.That(Fixed.Div(-1, 3), Is.EqualTo(-3334));
        }

        [Test]
        public void NFR02_IntAndLongOverloadsAgree()
        {
            for (int value = -1000; value <= 1000; value += 7)
            {
                for (int rate = -20000; rate <= 20000; rate += 1013)
                {
                    Assert.That(Fixed.Mul(value, rate), Is.EqualTo((int)Fixed.Mul((long)value, rate)));
                    Assert.That(Fixed.MulDiv(value, rate, 7), Is.EqualTo((int)Fixed.MulDiv((long)value, rate, 7)));
                }
            }
        }

        /// <summary>The int overloads widen before multiplying, so nothing overflows midway.</summary>
        [Test]
        public void NFR02_IntMulDivDoesNotOverflowMidway()
        {
            Assert.That(Fixed.MulDiv(int.MaxValue, 2, 4), Is.EqualTo(1073741823));
            Assert.That(Fixed.MulDiv(int.MinValue, 2, 4), Is.EqualTo(-1073741824));
        }
    }
}
