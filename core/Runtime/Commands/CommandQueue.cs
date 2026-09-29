using System.Collections.Generic;

namespace Sim
{
    /// <summary>
    /// What the host asks of the world, waiting for the tick that applies it (FR-A-01,
    /// DEC-030, AC-02).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Submitting applies nothing.</b> A command waits here until the tick loop drains
    /// the queue, at the start of every tick it runs, before any level fires. So the
    /// moment the host submits — between two calls to the loop, or while a tick is
    /// running, in a call of one tick or of many — does not move the tick a command
    /// lands on: everything submitted before a tick starts applies at the start of that
    /// tick, and everything submitted during it waits for the next. That is DEC-030's
    /// cost stated as a rule: "anything the game wants to do immediately waits for the
    /// next tick boundary."
    /// </para>
    /// <para>
    /// "While a tick is running" means on the same thread, from code the tick calls.
    /// This queue is not safe to fill from another thread; DEC-031's handover belongs to
    /// the host integration, since the core may not hold threading primitives.
    /// </para>
    /// <para>
    /// Commands apply in the order submitted. The queue forgets a command once applied;
    /// DEC-030's replay format is the log of each command with the tick it landed on,
    /// which nothing writes yet.
    /// </para>
    /// <para>
    /// <b>The queue is input, not state.</b> It holds what the host has asked and the
    /// world has not yet done. AC-02 names seed and commands as the two inputs whose
    /// repetition must repeat the hash sequence, so the queue sits beside
    /// <see cref="WorldState"/> rather than in it, as the count of ticks to run does.
    /// What a save does with commands still waiting is point 7's to decide; see
    /// <c>DECISIONS-OUTSIDE-SPEC.md</c>, point 5.
    /// </para>
    /// <para>
    /// Everything here is <c>internal</c> because phase 3 has no command a host could
    /// issue: the kinds are domain, and arrive with the subsystems they act on. The
    /// host's public entry is <see cref="Simulation"/>, which gains a submit with the
    /// first of them.
    /// </para>
    /// </remarks>
    internal sealed class CommandQueue
    {
        private readonly Queue<ICommand> pending = new Queue<ICommand>();

        /// <summary>How many commands are waiting.</summary>
        public int Count => pending.Count;

        /// <summary>Queues <paramref name="command"/> for the start of the next tick.</summary>
        public void Submit(ICommand command)
        {
            if (command == null)
            {
                throw new System.ArgumentNullException(nameof(command));
            }

            pending.Enqueue(command);
        }

        /// <summary>The oldest waiting command, removed. Only the tick loop drains.</summary>
        public ICommand Take() => pending.Dequeue();
    }

    /// <summary>
    /// One thing the host asks of the world. The core applies it; nothing outside the core
    /// writes state (FR-A-01).
    /// </summary>
    internal interface ICommand
    {
        /// <summary>Called by the tick loop, once, at the drain point of the tick it lands on.</summary>
        void Apply(WorldState state);
    }
}
