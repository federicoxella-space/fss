using System;
using NUnit.Framework;

namespace Sim.Tests
{
    /// <summary>
    /// FR-I-01 and SIM-STATE §Chronicle: an entry carries id, tick, location, entities,
    /// cause kind, cause and importance, and its id is a pointer a chain can be followed by.
    /// </summary>
    /// <remarks>
    /// No subsystem emits an entry in phase 3, so the entries come from a synthetic
    /// source whose every field is a function of its position in the stream. That proves
    /// the shape and the append, not that a real event fills it: the first emitter of
    /// phase 4 is the check that does.
    /// </remarks>
    [TestFixture]
    public sealed class ChronicleTests
    {
        private const int Entries = 1000;

        /// <summary>
        /// Ids follow the order of writing, from 1, and never move: every entry read back
        /// after the whole stream — after the columns have grown several times — is the
        /// one its id named when it was written, and its cause still names an earlier one.
        /// Two worlds fed the same stream give the same ids and the same hash.
        /// </summary>
        [Test]
        public void FRI01_ChronicleIdsAreStableAndOrdered()
        {
            WorldState state = Fresh();
            int[] ids = Feed(state);

            for (int n = 0; n < Entries; n++)
            {
                Assert.That(ids[n], Is.EqualTo(n + 1), "the n-th entry written has id n");
                AssertEntry(state, ids[n], n);
            }

            Assert.That(state.ChronicleCount, Is.EqualTo(Entries));

            WorldState again = Fresh();
            Assert.That(Feed(again), Is.EqualTo(ids));
            Assert.That(StateHash.Of(again), Is.EqualTo(StateHash.Of(state)));
        }

        /// <summary>
        /// A cause names an entry already written, or none; a pointer forward or to itself
        /// is a chain with no root, and is refused before anything is written.
        /// </summary>
        [Test]
        public void FRI01_ACauseNamesAnEarlierEntry()
        {
            WorldState state = Fresh();
            int root = Chronicle.Append(state, EntityId.None, ReadOnlySpan<EntityId>.Empty, CauseKind.ExogenousRoot, Chronicle.None, 1);
            ulong before = StateHash.Of(state);

            Assert.Throws<ArgumentOutOfRangeException>(() => Chronicle.Append(state, EntityId.None, ReadOnlySpan<EntityId>.Empty, CauseKind.Event, root + 1, 1), "itself");
            Assert.Throws<ArgumentOutOfRangeException>(() => Chronicle.Append(state, EntityId.None, ReadOnlySpan<EntityId>.Empty, CauseKind.Event, root + 2, 1), "forward");
            Assert.Throws<ArgumentOutOfRangeException>(() => Chronicle.Append(state, EntityId.None, ReadOnlySpan<EntityId>.Empty, CauseKind.Event, -1, 1));
            Assert.That(state.ChronicleCount, Is.EqualTo(1), "a refused entry leaves nothing behind");
            Assert.That(StateHash.Of(state), Is.EqualTo(before));

            Assert.That(Chronicle.Append(state, EntityId.None, ReadOnlySpan<EntityId>.Empty, CauseKind.Event, root, 1), Is.EqualTo(root + 1));
        }

        /// <summary>
        /// R-024 and SIM-STATE §Chronicle: the kind is one of FR-E-07's three, and the cause
        /// is "the triggering entry when causeKind is event, none otherwise". An event with
        /// no cause and a root with one are the two readings the kind exists to separate.
        /// </summary>
        [Test]
        public void FRI01_ACauseIdGoesWithAnEventAndOnlyWithOne()
        {
            WorldState state = Fresh();
            int root = Chronicle.Append(state, EntityId.None, ReadOnlySpan<EntityId>.Empty, CauseKind.ExogenousRoot, Chronicle.None, 1);
            ulong before = StateHash.Of(state);

            Assert.Throws<ArgumentOutOfRangeException>(() => Chronicle.Append(state, EntityId.None, ReadOnlySpan<EntityId>.Empty, CauseKind.Event, Chronicle.None, 1), "an event with no cause");
            Assert.Throws<ArgumentOutOfRangeException>(() => Chronicle.Append(state, EntityId.None, ReadOnlySpan<EntityId>.Empty, CauseKind.PlayerAction, root, 1), "a player action naming an entry");
            Assert.Throws<ArgumentOutOfRangeException>(() => Chronicle.Append(state, EntityId.None, ReadOnlySpan<EntityId>.Empty, CauseKind.ExogenousRoot, root, 1), "a root naming an entry");
            Assert.Throws<ArgumentOutOfRangeException>(() => Chronicle.Append(state, EntityId.None, ReadOnlySpan<EntityId>.Empty, default(CauseKind), Chronicle.None, 1), "none of the three");
            Assert.Throws<ArgumentOutOfRangeException>(() => Chronicle.Append(state, EntityId.None, ReadOnlySpan<EntityId>.Empty, (CauseKind)4, Chronicle.None, 1), "none of the three");
            Assert.That(state.ChronicleCount, Is.EqualTo(1), "a refused entry leaves nothing behind");
            Assert.That(StateHash.Of(state), Is.EqualTo(before));

            Assert.That(Chronicle.Append(state, EntityId.None, ReadOnlySpan<EntityId>.Empty, CauseKind.PlayerAction, Chronicle.None, 1), Is.EqualTo(root + 1));
        }

        /// <summary>
        /// Capacity is not state: a world whose columns grew further hashes the same as one
        /// holding the same entries.
        /// </summary>
        [Test]
        public void FRI01_CapacityDoesNotReachTheHash()
        {
            WorldState grown = Fresh();
            WorldState tight = Fresh();
            Feed(grown);
            Feed(tight);

            Array.Resize(ref grown.ChronicleTick, grown.ChronicleTick.Length * 2);
            Array.Resize(ref grown.ChronicleLocation, grown.ChronicleLocation.Length * 2);
            Array.Resize(ref grown.ChronicleCauseKind, grown.ChronicleCauseKind.Length * 2);
            Array.Resize(ref grown.ChronicleCause, grown.ChronicleCause.Length * 2);
            Array.Resize(ref grown.ChronicleImportance, grown.ChronicleImportance.Length * 2);
            Array.Resize(ref grown.ChronicleEntityStart, grown.ChronicleEntityStart.Length * 2);
            Array.Resize(ref grown.ChronicleEntityCount, grown.ChronicleEntityCount.Length * 2);
            Array.Resize(ref grown.ChronicleEntities, grown.ChronicleEntities.Length * 2);

            Assert.That(StateHash.Of(grown), Is.EqualTo(StateHash.Of(tight)));
        }

        /// <summary>
        /// The column types SIM-STATE gives, or the one this plan chose where it gives
        /// none (register, point 6). The hash folds the int columns through
        /// <c>(uint)</c>, so a column widened to <c>long</c> would be narrowed there
        /// silently, and no perturbation by one would see it.
        /// </summary>
        [Test]
        public void FRI01_TheChronicleCarriesTheDeclaredTypes()
        {
            Assert.That(typeof(WorldState).GetField("ChronicleCount").FieldType, Is.EqualTo(typeof(int)));
            Assert.That(typeof(WorldState).GetField("ChronicleTick").FieldType, Is.EqualTo(typeof(long[])), "tick is int64");
            Assert.That(typeof(WorldState).GetField("ChronicleLocation").FieldType, Is.EqualTo(typeof(EntityId[])));
            Assert.That(typeof(WorldState).GetField("ChronicleCauseKind").FieldType, Is.EqualTo(typeof(CauseKind[])), "causeKind is enum");
            Assert.That(Enum.GetUnderlyingType(typeof(CauseKind)), Is.EqualTo(typeof(int)), "the hash and the save write it as int");
            Assert.That(new[] { (int)CauseKind.Event, (int)CauseKind.PlayerAction, (int)CauseKind.ExogenousRoot }, Is.EqualTo(new[] { 1, 2, 3 }), "every save holds these numbers");
            Assert.That(typeof(WorldState).GetField("ChronicleCause").FieldType, Is.EqualTo(typeof(int[])));
            Assert.That(typeof(WorldState).GetField("ChronicleImportance").FieldType, Is.EqualTo(typeof(int[])), "importance is int");
            Assert.That(typeof(WorldState).GetField("ChronicleEntityStart").FieldType, Is.EqualTo(typeof(int[])));
            Assert.That(typeof(WorldState).GetField("ChronicleEntityCount").FieldType, Is.EqualTo(typeof(int[])));
            Assert.That(typeof(WorldState).GetField("ChronicleEntities").FieldType, Is.EqualTo(typeof(EntityId[])), "entities are id[]");
        }

        private static WorldState Fresh() => new WorldState(1, new GenerationParams(21));

        /// <summary>
        /// The synthetic source. Entry n names n % 4 entities, so empty slices sit
        /// between full ones in the pool; its cause is an earlier entry for most n and
        /// none for every fifth, alternately a player action and a root; ticks advance
        /// through the loop every third entry.
        /// </summary>
        private static int[] Feed(WorldState state)
        {
            var ids = new int[Entries];
            for (int n = 0; n < Entries; n++)
            {
                if (n % 3 == 0)
                {
                    TickLoop.Advance(state, 1);
                }

                var entities = new EntityId[n % 4];
                for (int k = 0; k < entities.Length; k++)
                {
                    entities[k] = Entity(n, k);
                }

                ids[n] = Chronicle.Append(state, Location(n), entities, Kind(n), Cause(n, ids), Importance(n));
            }

            return ids;
        }

        private static void AssertEntry(WorldState state, int id, int n)
        {
            int row = Chronicle.RowOf(id);
            Assert.That(state.ChronicleTick[row], Is.EqualTo(n / 3 + 1), "entry " + id + " tick");
            Assert.That(state.ChronicleLocation[row], Is.EqualTo(Location(n)), "entry " + id + " location");
            Assert.That(state.ChronicleImportance[row], Is.EqualTo(Importance(n)), "entry " + id + " importance");

            Assert.That(state.ChronicleCauseKind[row], Is.EqualTo(Kind(n)), "entry " + id + " cause kind");

            int cause = state.ChronicleCause[row];
            Assert.That(cause, Is.LessThan(id), "entry " + id + " points back");
            Assert.That(cause, Is.EqualTo(n % 5 == 0 ? Chronicle.None : n / 2 + 1), "entry " + id + " cause");

            Assert.That(state.ChronicleEntityCount[row], Is.EqualTo(n % 4), "entry " + id + " entity count");
            int start = state.ChronicleEntityStart[row];
            for (int k = 0; k < n % 4; k++)
            {
                Assert.That(state.ChronicleEntities[start + k], Is.EqualTo(Entity(n, k)), "entry " + id + " entity " + k);
            }
        }

        private static EntityId Location(int n) => new EntityId(n % 21, 1);

        private static EntityId Entity(int n, int k) => new EntityId(n * 4 + k, n % 7 + 1);

        private static int Importance(int n) => n * 37 % 101;

        // The entry halfway back, written already, so chains cross the growth of the columns.
        private static int Cause(int n, int[] ids) => n % 5 == 0 ? Chronicle.None : ids[n / 2];

        private static CauseKind Kind(int n) =>
            n % 5 != 0 ? CauseKind.Event : n % 10 == 0 ? CauseKind.ExogenousRoot : CauseKind.PlayerAction;
    }
}
