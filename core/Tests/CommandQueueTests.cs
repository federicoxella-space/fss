using System.Collections.Generic;
using NUnit.Framework;

namespace Sim.Tests
{
    /// <summary>
    /// FR-A-01 and AC-02: the host's commands apply at one point of the tick, and the same
    /// commands in the same order produce the same hash sequence whenever they were
    /// submitted before that point.
    /// </summary>
    [TestFixture]
    public sealed class CommandQueueTests
    {
        private const int Ticks = 40;

        /// <summary>
        /// What the host submits before each tick: tick → values, in submission order.
        /// Tick 0 has commands, so a drain that skips the first tick shows; tick 7 has
        /// three, so a drain that applies only one shows.
        /// </summary>
        private static readonly Dictionary<long, long[]> Script = new Dictionary<long, long[]>
        {
            [0] = new long[] { 5, 11 },
            [1] = new long[] { 2 },
            [7] = new long[] { 3, 1, 4 },
            [28] = new long[] { 9 },
            [39] = new long[] { 6, 6 },
        };

        /// <summary>Where <see cref="InCallsOfManyTicks"/> ends each call and submits.</summary>
        private static readonly int[] Stops = { 1, 7, 8, 28, 29, 39, 40 };

        /// <summary>
        /// FR-A-01: the same commands submitted in the same order produce the same hash
        /// sequence, and submitting them at a different moment of the caller's loop — at
        /// the boundary before the tick, or while the tick before it is running, in calls
        /// of one tick or of many — changes nothing.
        /// </summary>
        /// <remarks>
        /// The sequence is read inside each tick, from the first level, after the drain:
        /// the one point every way of calling the loop shares. A caller that runs forty
        /// ticks in one call sees no boundary between them.
        /// </remarks>
        [Test]
        public void FRA01_CommandsApplyAtOnePointInTheTick()
        {
            var reference = AtTheBoundary();
            Assert.That(reference.Count, Is.EqualTo(Ticks));

            Assert.That(AtTheBoundary(), Is.EqualTo(reference), "the same commands, submitted the same way");
            Assert.That(DuringThePreviousTick(), Is.EqualTo(reference), "submitted while the tick before was running, in one call");
            Assert.That(InCallsOfManyTicks(), Is.EqualTo(reference), "submitted between calls of several ticks");

            // The commands did something: without them the sequence is another one.
            var idle = new Host(new CommandQueue(), new Dictionary<long, long[]>());
            TickLoop.Advance(NewWorld(), idle.Commands, Ticks, idle, default(FoldEffects));
            Assert.That(idle.Hashes, Is.Not.EqualTo(reference));
        }

        /// <summary>
        /// Commands apply in the order submitted. Not named by the plan's criterion, which
        /// compares runs with one another: a queue that reversed every tick's commands
        /// would reverse them in every run alike and pass it.
        /// </summary>
        [Test]
        public void FRA01_CommandsApplyInTheOrderSubmitted()
        {
            var state = NewWorld();
            var commands = new CommandQueue();
            commands.Submit(Fold(3));
            commands.Submit(Fold(1));
            commands.Submit(Fold(4));

            TickLoop.Advance(state, commands, 1, default(NoLevels), default(FoldEffects));

            // Fold at tick 0 is c * 31 + v: ((3 * 31) + 1) * 31 + 4.
            Assert.That(state.CurrencyTotal, Is.EqualTo(2918));
        }

        /// <summary>A command submitted during a tick is not applied within it.</summary>
        [Test]
        public void FRA01_ACommandSubmittedDuringATickWaitsForTheNext()
        {
            var state = NewWorld();
            var host = new Host(new CommandQueue(), new Dictionary<long, long[]> { [0] = new long[] { 7 } });

            TickLoop.Advance(state, host.Commands, 1, host, default(FoldEffects));
            Assert.That(state.CurrencyTotal, Is.Zero, "applied inside the tick it was submitted in");
            Assert.That(host.Commands.Count, Is.EqualTo(1));

            TickLoop.Advance(state, host.Commands, 1, host, default(FoldEffects));
            Assert.That(host.Commands.Count, Is.Zero);
            Assert.That(state.CurrencyTotal, Is.EqualTo(7 + 1), "Fold at tick 1");
        }

        /// <summary>
        /// DEC-086: the core applies a command by its kind, and phase 3 defines none, so the
        /// core's own drain refuses every command before writing anything. A drain that
        /// skipped what it does not know would drop the host's input without a word.
        /// </summary>
        [Test]
        public void FRA01_ACommandOfNoKnownKindIsRefused()
        {
            var state = NewWorld();
            var commands = new CommandQueue();
            commands.Submit(Fold(3));
            ulong before = StateHash.Of(state);

            Assert.Throws<System.ArgumentOutOfRangeException>(() => TickLoop.Advance(state, commands, 1));
            Assert.That(StateHash.Of(state), Is.EqualTo(before));
        }

        /// <summary>
        /// R-022 and DEC-086: a command is a value, a kind and integer fields, with no
        /// behaviour of its own. The interface a command applied itself through is gone.
        /// </summary>
        [Test]
        public void FRA01_ACommandIsAValue()
        {
            Assert.That(typeof(Command).IsValueType, Is.True);
            foreach (var field in typeof(Command).GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
            {
                Assert.That(field.FieldType == typeof(int) || field.FieldType == typeof(long), field.Name + " is an integer field");
            }

            Assert.That(typeof(Command).GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly), Is.Empty, "no behaviour");
            Assert.That(typeof(Simulation).Assembly.GetType("Sim.ICommand"), Is.Null);
        }

        /// <summary>
        /// FR-A-01, FR-A-02, R-003: the live state is not visible outside the core, so the
        /// compiler refuses any public member that takes or returns it. That does not stop
        /// a public member returning one of its arrays once phase 4 adds them; this
        /// assertion is the floor, not the whole guard.
        /// </summary>
        [Test]
        public void FRA01_WorldStateIsNotVisibleOutsideTheCore()
        {
            Assert.That(typeof(WorldState).IsVisible, Is.False);
            Assert.That(typeof(CommandQueue).IsVisible, Is.False);
        }

        /// <summary>
        /// The public entry runs the same loop: its hash after N ticks is the hash of a
        /// state advanced N ticks directly, and it refuses to run backwards.
        /// </summary>
        [Test]
        public void FRA01_ThePublicEntryAdvancesAndHashesTheSameWorld()
        {
            var sim = new Simulation(worldSeed: 1, new GenerationParams(settlementCount: 10));
            var state = new WorldState(worldSeed: 1, new GenerationParams(settlementCount: 10));
            Assert.That(sim.StateHash, Is.EqualTo(StateHash.Of(state)));

            sim.Advance(9);
            TickLoop.Advance(state, 9);
            Assert.That(sim.StateHash, Is.EqualTo(StateHash.Of(state)));

            Assert.That(() => sim.Advance(-1), Throws.InstanceOf<System.ArgumentOutOfRangeException>());
        }

        /// <summary>Submits each tick's commands between calls of one tick.</summary>
        private static List<ulong> AtTheBoundary()
        {
            var state = NewWorld();
            var host = new Host(new CommandQueue(), new Dictionary<long, long[]>());
            for (int t = 0; t < Ticks; t++)
            {
                SubmitFor(host.Commands, t);
                TickLoop.Advance(state, host.Commands, 1, host, default(FoldEffects));
            }

            return host.Hashes;
        }

        /// <summary>
        /// Submits tick t's commands from inside tick t − 1, in one call of every tick;
        /// tick 0's have no tick before them and are submitted first.
        /// </summary>
        private static List<ulong> DuringThePreviousTick()
        {
            var shifted = new Dictionary<long, long[]>();
            foreach (var entry in Script)
            {
                if (entry.Key > 0)
                {
                    shifted[entry.Key - 1] = entry.Value;
                }
            }

            var host = new Host(new CommandQueue(), shifted);
            SubmitFor(host.Commands, 0);
            TickLoop.Advance(NewWorld(), host.Commands, Ticks, host, default(FoldEffects));
            return host.Hashes;
        }

        /// <summary>Advances from one stop to the next in a single call, submitting at each stop.</summary>
        private static List<ulong> InCallsOfManyTicks()
        {
            Assert.That(Stops[Stops.Length - 1], Is.EqualTo(Ticks));

            var state = NewWorld();
            var host = new Host(new CommandQueue(), new Dictionary<long, long[]>());
            SubmitFor(host.Commands, 0);
            foreach (int stop in Stops)
            {
                TickLoop.Advance(state, host.Commands, stop - state.Tick, host, default(FoldEffects));
                SubmitFor(host.Commands, stop);
            }

            return host.Hashes;
        }

        private static void SubmitFor(CommandQueue commands, long tick)
        {
            if (Script.TryGetValue(tick, out long[] values))
            {
                foreach (long v in values)
                {
                    commands.Submit(Fold(v));
                }
            }
        }

        private static WorldState NewWorld() =>
            new WorldState(worldSeed: 1, new GenerationParams(settlementCount: 10));

        /// <summary>A kind no phase 3 core knows, which only <see cref="FoldEffects"/> applies.</summary>
        internal const int FoldKind = 1;

        internal static Command Fold(long value) => new Command(FoldKind, value);

        /// <summary>
        /// Folds a command's value and the tick into <see cref="WorldState.CurrencyTotal"/>
        /// in a way that does not commute, so applying the same commands in another order,
        /// or on another tick, leaves another hash. A test stand-in for the core's dispatch:
        /// phase 3 has no command kinds.
        /// </summary>
        internal readonly struct FoldEffects : ICommandEffects
        {
            public void Apply(WorldState state, in Command command)
            {
                Assert.That(command.Kind, Is.EqualTo(FoldKind));
                state.CurrencyTotal = state.CurrencyTotal * 31 + command.A + state.Tick;
            }
        }

        /// <summary>
        /// A host that lives inside the tick: the Daily level records the state hash,
        /// then submits whatever its script holds for the current tick.
        /// </summary>
        private readonly struct Host : ICadenceLevels
        {
            public readonly CommandQueue Commands;
            public readonly List<ulong> Hashes;
            private readonly Dictionary<long, long[]> byTick;

            public Host(CommandQueue commands, Dictionary<long, long[]> byTick)
            {
                Commands = commands;
                Hashes = new List<ulong>();
                this.byTick = byTick;
            }

            public void Daily(WorldState state)
            {
                Hashes.Add(StateHash.Of(state));
                if (byTick.TryGetValue(state.Tick, out long[] values))
                {
                    foreach (long v in values)
                    {
                        Commands.Submit(Fold(v));
                    }
                }
            }

            public void Settlement(WorldState state, int id) { }

            public void Basin(WorldState state) { }

            public void Kingdom(WorldState state) { }
        }
    }
}
