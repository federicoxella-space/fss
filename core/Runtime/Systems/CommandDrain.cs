namespace Sim
{
    /// <summary>
    /// Applies a <see cref="Command"/> to the state, dispatched on its kind (FR-A-01,
    /// DEC-086, R-022).
    /// </summary>
    /// <remarks>
    /// <para>
    /// A command is data; this is where its effect lives, beside every other writer of
    /// state. Each kind a later phase defines gains a case here. The save writes every
    /// kind through one codec, a kind and four fields, until a kind needs more than
    /// <see cref="Command"/> holds.
    /// </para>
    /// <para>
    /// <b>Phase 3 has no kind</b>, so every command this meets is one the core does not
    /// know, and it throws before writing anything. The tick loop takes the effects as a
    /// struct type parameter, as it takes the levels, so the tests can stand a kind in
    /// without the core defining one.
    /// </para>
    /// </remarks>
    internal readonly struct CommandDrain : ICommandEffects
    {
        public void Apply(WorldState state, in Command command)
        {
            switch (command.Kind)
            {
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(command), "No command of kind " + command.Kind.ToString() + " exists.");
            }
        }
    }

    /// <summary>What the drain does with each command it takes.</summary>
    internal interface ICommandEffects
    {
        /// <summary>Called by the tick loop, once, at the drain point of the tick the command lands on.</summary>
        void Apply(WorldState state, in Command command);
    }
}
