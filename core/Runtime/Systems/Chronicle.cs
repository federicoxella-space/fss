namespace Sim
{
    /// <summary>
    /// Appends entries to the chronicle held in <see cref="WorldState"/> (FR-I-01,
    /// FR-I-04, SIM-STATE §Chronicle).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>An id is the order of writing.</b> The n-th entry appended has id n, from 1, and
    /// keeps it: nothing here moves or discards a row, so an id read once resolves to the
    /// same entry for the rest of the run. That is what makes a cause a pointer a test can
    /// follow. Retention is SIM-STATE Open item 3 and is not decided; whatever decides it
    /// has to keep ids stable across a discard, which a row offset would.
    /// </para>
    /// <para>
    /// <b>A cause is an entry already written, or none.</b> A pointer forward or to itself
    /// would make a chain that never reaches its root, so it is refused rather than stored.
    /// It throws, in Release too, unlike the loop's asserts: a bad cause is not a no-op but
    /// a pointer carried into every later save.
    /// </para>
    /// <para>
    /// The tick is the state's: an entry is dated when it is written. Dating the entries
    /// of prior history is SIM-STATE Open item 4, and belongs to the generator.
    /// </para>
    /// <para>
    /// Phase 3 has no event, so nothing calls this yet but the tests. From phase 4 every
    /// event emits one (FR-I-01, DEC-026), wherever it happens.
    /// </para>
    /// </remarks>
    internal static class Chronicle
    {
        /// <summary>The id no entry has: the cause of an entry with none.</summary>
        public const int None = 0;

        /// <summary>
        /// Writes one entry on the current tick and returns its id.
        /// </summary>
        public static int Append(
            WorldState state,
            EntityId location,
            System.ReadOnlySpan<EntityId> entities,
            int cause,
            int importance)
        {
            int row = state.ChronicleCount;
            int id = row + 1;

            if (cause < None || cause >= id)
            {
                throw new System.ArgumentOutOfRangeException(nameof(cause), "A cause is an entry already written, or none.");
            }

            int start = row == 0 ? 0 : state.ChronicleEntityStart[row - 1] + state.ChronicleEntityCount[row - 1];

            Reserve(ref state.ChronicleTick, id);
            Reserve(ref state.ChronicleLocation, id);
            Reserve(ref state.ChronicleCause, id);
            Reserve(ref state.ChronicleImportance, id);
            Reserve(ref state.ChronicleEntityStart, id);
            Reserve(ref state.ChronicleEntityCount, id);
            Reserve(ref state.ChronicleEntities, start + entities.Length);

            state.ChronicleTick[row] = state.Tick;
            state.ChronicleLocation[row] = location;
            state.ChronicleCause[row] = cause;
            state.ChronicleImportance[row] = importance;
            state.ChronicleEntityStart[row] = start;
            state.ChronicleEntityCount[row] = entities.Length;
            entities.CopyTo(new System.Span<EntityId>(state.ChronicleEntities, start, entities.Length));

            state.ChronicleCount = id;
            return id;
        }

        /// <summary>The row of the columns that entry <paramref name="id"/> occupies.</summary>
        public static int RowOf(int id) => id - 1;

        /// <summary>How much of <see cref="WorldState.ChronicleEntities"/> is in use.</summary>
        public static int EntitiesInUse(WorldState state)
        {
            int last = state.ChronicleCount - 1;
            return last < 0 ? 0 : state.ChronicleEntityStart[last] + state.ChronicleEntityCount[last];
        }

        // Capacity is not state: only the rows in use are hashed, and are all the
        // serialiser of point 7 may write, so doubling leaves nothing a future tick reads.
        private static void Reserve<T>(ref T[] column, int length)
        {
            if (column.Length < length)
            {
                int grown = column.Length < 8 ? 8 : column.Length * 2;
                System.Array.Resize(ref column, grown < length ? length : grown);
            }
        }
    }
}
