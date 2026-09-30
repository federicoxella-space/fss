using System.Collections.Generic;
using NUnit.Framework;

namespace Sim.Tests
{
    /// <summary>
    /// AC-02: the same seed and commands produce the same state hash sequence. Two runs
    /// here are two worlds built from the same values in one process; the two runs of the
    /// CI workflow are two processes, compared byte for byte. Neither is another build or
    /// another machine.
    /// </summary>
    [TestFixture]
    public sealed class DeterminismTests
    {
        /// <summary>The gate of phase 3 (SIM-REQ §18): 100k empty ticks.</summary>
        private const int Ticks = 100_000;

        /// <summary>P-02, the generator default, as the runner uses it.</summary>
        private const int SettlementCount = 2000;

        /// <summary>
        /// AC-02: two independent runs of the same seed give the same hash at every tick,
        /// empty and with commands; a different seed, or the same commands left out, gives
        /// another sequence, so the equality is not one every world would pass.
        /// </summary>
        [Test]
        public void AC02_DeterminismAcrossRuns()
        {
            ulong[] first = Empty(seed: 1);
            Assert.That(first, Has.Length.EqualTo(Ticks));
            Assert.That(Empty(seed: 1), Is.EqualTo(first), "the same seed, empty");
            Assert.That(new HashSet<ulong>(first), Has.Count.EqualTo(Ticks), "every tick leaves its own hash");
            Assert.That(Empty(seed: 2), Is.Not.EqualTo(first), "another seed");

            ulong[] commanded = Commanded(seed: 1);
            Assert.That(Commanded(seed: 1), Is.EqualTo(commanded), "the same seed and commands");
            Assert.That(commanded, Is.Not.EqualTo(first), "the commands did something");
        }

        /// <summary>The host's entry, as the runner calls it: one tick, then the hash.</summary>
        private static ulong[] Empty(ulong seed)
        {
            var sim = new Simulation(seed, new GenerationParams(SettlementCount));
            var hashes = new ulong[Ticks];
            for (int t = 0; t < Ticks; t++)
            {
                sim.Advance(1);
                hashes[t] = sim.StateHash;
            }

            return hashes;
        }

        /// <summary>
        /// A command of the stand-in kind of <see cref="CommandQueueTests"/> before every
        /// tick whose index is a multiple of 97, since phase 3 defines no kind a host could
        /// submit through <see cref="Simulation"/>.
        /// </summary>
        private static ulong[] Commanded(ulong seed)
        {
            var state = new WorldState(seed, new GenerationParams(SettlementCount));
            var commands = new CommandQueue();
            var hashes = new ulong[Ticks];
            for (int t = 0; t < Ticks; t++)
            {
                if (t % 97 == 0)
                {
                    commands.Submit(CommandQueueTests.Fold(t));
                }

                TickLoop.Advance(state, commands, 1, default(NoLevels), default(CommandQueueTests.FoldEffects));
                hashes[t] = StateHash.Of(state);
            }

            return hashes;
        }
    }
}
