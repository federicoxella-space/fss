namespace Sim
{
    /// <summary>
    /// Advances the world a number of ticks the caller chooses, firing each level on its
    /// cadence (FR-T-04, FR-T-06, FR-T-08, FR-T-09; DEC-007, DEC-008; A-13).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The caller decides how many ticks run.</b> There is no clock here and no
    /// budget: <see cref="Advance{T}"/> runs exactly the count it is given and returns
    /// (FR-T-09). Nothing in it knows where a player is, so fast travel and an unwatched
    /// world advance by the same code (FR-T-08, DEC-007).
    /// </para>
    /// <para>
    /// <b>Cadence is arithmetic on the tick, as FR-T-04 gives it.</b> Daily fires every
    /// tick; the settlement bucket fires on tick <c>d</c> for every id with
    /// <c>id % 7 == d % 7</c>, and the loop reaches those ids by stepping seven from
    /// <c>d % 7</c>, so the bucket derives from the id and not from how anything is
    /// traversed (FR-T-06, DEC-008). Basin and kingdom fire on the first tick of their
    /// month and year. <c>docs/</c> does not say which tick of the period a level
    /// without a stagger fires on, nor in which order the levels run within a tick;
    /// both are recorded in <c>DECISIONS-OUTSIDE-SPEC.md</c> under plan point 4.
    /// </para>
    /// <para>
    /// <b>Commands drain first (FR-A-01, DEC-030).</b> A tick starts by applying, in
    /// order, the commands waiting when it starts, and only then runs its levels; a
    /// command submitted while the tick runs waits for the next one. The start is the
    /// point <c>docs/</c> leaves open; why it is the start is recorded under plan point 5.
    /// </para>
    /// <para>
    /// A tick is processed at the state's current <see cref="WorldState.Tick"/>, and the
    /// counter moves only after every level due on it has run, so each level sees the
    /// date it fires on. Phase 3 has no domain, so the levels are empty; the settlement
    /// level becomes the update sequence of SIM-ECON in phase 4.
    /// </para>
    /// </remarks>
    internal static class TickLoop
    {
        /// <summary>Runs <paramref name="ticks"/> ticks with every level empty, as phase 3 has them.</summary>
        public static void Advance(WorldState state, long ticks) => Advance(state, new CommandQueue(), ticks, default(NoLevels));

        /// <summary>Runs <paramref name="ticks"/> ticks with every level empty, draining <paramref name="commands"/>.</summary>
        public static void Advance(WorldState state, CommandQueue commands, long ticks) =>
            Advance(state, commands, ticks, default(NoLevels));

        /// <summary>Runs <paramref name="ticks"/> ticks with no commands, handing each firing to <paramref name="levels"/>.</summary>
        public static void Advance<T>(WorldState state, long ticks, T levels)
            where T : struct, ICadenceLevels => Advance(state, new CommandQueue(), ticks, levels);

        /// <summary>
        /// Runs <paramref name="ticks"/> ticks, draining <paramref name="commands"/> at the
        /// start of each and handing each firing to <paramref name="levels"/>.
        /// </summary>
        public static void Advance<T>(WorldState state, CommandQueue commands, long ticks, T levels)
            where T : struct, ICadenceLevels
        {
            System.Diagnostics.Debug.Assert(ticks >= 0, "The caller asks for ticks to run, never for time to go back.");

            int settlements = state.GenerationParams.SettlementCount;
            for (long i = 0; i < ticks; i++)
            {
                long d = state.Tick;

                // Only those waiting now: one submitted from here on lands on the next tick.
                for (int n = commands.Count; n > 0; n--)
                {
                    commands.Take().Apply(state);
                }

                levels.Daily(state);

                for (int id = (int)(d % Calendar.DaysPerWeek); id < settlements; id += Calendar.DaysPerWeek)
                {
                    levels.Settlement(state, id);
                }

                if (d % Calendar.DaysPerMonth == 0)
                {
                    levels.Basin(state);
                }

                if (d % Calendar.DaysPerYear == 0)
                {
                    levels.Kingdom(state);
                }

                state.Tick = d + 1;
            }
        }
    }

    /// <summary>
    /// The four levels of FR-T-04, as the loop calls them. A struct type parameter, so the
    /// call is direct and costs nothing when the level does nothing.
    /// </summary>
    internal interface ICadenceLevels
    {
        /// <summary>Hot agents, every tick. Fast travel runs this with none (FR-T-08).</summary>
        void Daily(WorldState state);

        /// <summary>One settlement, on the tick of its bucket.</summary>
        void Settlement(WorldState state, int id);

        /// <summary>Basins, on the first tick of each month.</summary>
        void Basin(WorldState state);

        /// <summary>Kingdoms, on the first tick of each year.</summary>
        void Kingdom(WorldState state);
    }

    /// <summary>Phase 3's levels: the cadence is real and nothing is behind it.</summary>
    internal readonly struct NoLevels : ICadenceLevels
    {
        public void Daily(WorldState state) { }

        public void Settlement(WorldState state, int id) { }

        public void Basin(WorldState state) { }

        public void Kingdom(WorldState state) { }
    }
}
