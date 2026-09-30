using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Security.Cryptography;
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

        /// <summary>
        /// R-034 and R-036: the two sequences of <see cref="AC02_DeterminismAcrossRuns"/>
        /// against values produced by a run on the development machine and written here as
        /// literals, so that every CI runner, and the Debug build beside the Release one,
        /// compares its sequence with another build's on another machine. The two runs of
        /// that test agree with each other whatever the loop does alike in both; this is
        /// what notices the loop doing it.
        /// </summary>
        /// <remarks>
        /// The digest is SHA-256 over each hash in little-endian order, from the test
        /// assembly, so the expected values do not depend on the core's own hash of hashes.
        /// Both move whenever an empty world's state changes shape, which phase 4 does with
        /// every table, and the commanded one also with the stand-in kind or
        /// <c>FoldEffects</c>. A re-pin says in its commit that the sequence was meant to
        /// change (R-034).
        /// </remarks>
        [Test]
        public void AC02_TheGateSequencesArePinned()
        {
            Assert.That(Digest(Empty(seed: 1)), Is.EqualTo("9C9CC60DFD31A2E0A10BD1D4036C9C460070C7D16F2F22EBF8C0D40EBACC93C4"), "the empty run");
            Assert.That(Digest(Commanded(seed: 1)), Is.EqualTo("72FA59375B411E651036188CF68A12EA6B8C0634239E99CDDBD83B23BC31F715"), "the run with commands");
        }

        private static string Digest(ulong[] hashes)
        {
            var bytes = new byte[hashes.Length * sizeof(ulong)];
            for (int i = 0; i < hashes.Length; i++)
            {
                BinaryPrimitives.WriteUInt64LittleEndian(bytes.AsSpan(i * sizeof(ulong)), hashes[i]);
            }

            return Convert.ToHexString(SHA256.HashData(bytes));
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
