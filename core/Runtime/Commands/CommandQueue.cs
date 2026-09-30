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
    /// <b>The queue is input, not state</b> (SIM-STATE §Rule, R-021). It holds what the
    /// host has asked and the world has not yet done. AC-02 names seed and commands as the
    /// two inputs whose repetition must repeat the hash sequence, so the queue sits beside
    /// <see cref="WorldState"/> rather than in it, as the count of ticks to run does, and
    /// stays out of the state hash. A save carries it all the same, in submission order,
    /// and a load puts it back (DEC-032).
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
        private readonly Queue<Command> pending = new Queue<Command>();

        /// <summary>How many commands are waiting.</summary>
        public int Count => pending.Count;

        /// <summary>Queues <paramref name="command"/> for the start of the next tick.</summary>
        public void Submit(Command command) => pending.Enqueue(command);

        /// <summary>The oldest waiting command, removed. Only the tick loop drains.</summary>
        public Command Take() => pending.Dequeue();

        /// <summary>The waiting commands, oldest first, left in place. What a save writes.</summary>
        public Queue<Command>.Enumerator GetEnumerator() => pending.GetEnumerator();
    }

    /// <summary>
    /// One thing the host asks of the world: a kind and the integer fields that kind
    /// declares, and nothing else (DEC-086, R-022).
    /// </summary>
    /// <remarks>
    /// <para>
    /// A value, so that it crosses the three boundaries DEC-086 names — the host's
    /// hand-over, the replay log, a save — the way a state row does. The core applies it,
    /// dispatching on <see cref="Kind"/> from <c>Runtime/Systems/</c>; nothing outside the
    /// core writes state (FR-A-01).
    /// </para>
    /// <para>
    /// Four fields, because phase 3 has no kind to count them from. A kind declares which
    /// it reads and leaves the rest at zero; a kind needing more widens this struct, and
    /// the save format with it, under a new format version.
    /// </para>
    /// </remarks>
    internal readonly struct Command
    {
        /// <summary>What the command is. Phase 3 defines none; the kinds are domain.</summary>
        public readonly int Kind;

        public readonly long A;
        public readonly long B;
        public readonly long C;
        public readonly long D;

        public Command(int kind, long a = 0, long b = 0, long c = 0, long d = 0)
        {
            Kind = kind;
            A = a;
            B = b;
            C = c;
            D = d;
        }
    }
}
