# Questions about the specification

`docs/` outranks the code and is not ours to edit. That leaves one thing with
nowhere to go: the discovery that a requirement is contradictory, unachievable,
or wrong. `AGENTS.md` says to stop and say so, and saying so has meant saying it
in a conversation, which is outside the repository and gone by the next session.

This file is where it goes instead. It is the return channel to the
specification, and the most valuable output this process can produce: everything
else here is work done under the specification, and this is the only thing that
can correct it.

## Rules

- **Every objection to `docs/` is written here.** Not resolved, not worked
  around, not dropped because the work found a way past it. An objection
  answered by the person who found it is an objection that never reached the
  only reader who can act on it.
- **An objection is never resolved by its finder.** A human answers. The entry
  stays open until then, and a stale open entry is a truer state of affairs than
  a closed one nobody decided.
- **Every entry declares whether it blocks.** Without that, this file becomes
  where objections are filed instead of acted on. A blocking entry fails the
  point and stops the plan, by the rule in
  `.claude/design/2026-09-17-plan-e-skill.md`; a non-blocking one is recorded and
  the work continues.
- **Cite precisely:** the document, the identifier, and the line. An objection
  that cannot be checked in a minute will not be checked.
- **This file does not amend `docs/`.** Nothing here is true of the
  specification. An answer that changes the specification is written into
  `docs/` by a human, and the entry is then marked with what it became.

## Entries

### SQ-001 — the sampled-transient draw has three coordinates, not five

**Raised:** 2026-09-19, while building `Hash64.Subject` for non-entity draws.
**Cites:** `docs/SIM-DEC.md:31` (DEC-002), `docs/SIM-STATE.md:25`,
`docs/SIM-REQ.md:400` (FR-X-02), `docs/SIM-STATE.md:123`.
**Blocks:** no. Transients are phase 4 work; phase 3 carries no domain logic.
**Status:** open.

Two arities are specified for the same function.

- DEC-002 and SIM-STATE line 25: `Hash(world_seed, entity_id, tick, channel, index)`.
- FR-X-02 and SIM-STATE line 123: `Hash(seed, link, index)`.

FR-X-02 is a requirement, so the three-coordinate form is normative rather than
loose prose, and it appears twice.

The omission may well be deliberate. FR-X-02 says the draw *yields* the departure
tick, so keying it on a tick would be circular: the set of sampled transients on
a link has to be stable rather than re-rolled every tick. If that is the intent,
then two things are unstated — which tick value the call passes, and which
channel. `HashChannel.Transients` exists for it and no document names it.

What it would take to close: a human saying whether FR-X-02 is shorthand for the
five-coordinate form with a fixed tick, or a deliberately different keying. The
implementation cannot choose: picking one silently would make the sampled
transients of a world depend on a decision nobody recorded, and `Hash64` is the
one place where an unrecorded choice cannot be found again from the output.

### SQ-002 — A-11 is asserted every tick, and the implementation checks it only in debug

**Raised:** 2026-09-19, by the first promotion pass while classifying `D-004`.
**Cites:** `docs/SIM-STATE.md:155` ff., invariant A-11 — "no accumulator has
overflowed", under the heading "Invariants, asserted every tick".
**Blocks:** no. The guard is more than existed before it, and phase 3 has no
accumulator running in a release build.
**Status:** open.

The overflow guard on the `long` overload of `RemainderAccumulator.Apply` is
`[Conditional("DEBUG")]`, so a release build does not check it. A-11 says every
tick, without qualification.

The implementation's reason is cost: the release build cannot afford a check on
every rate applied in every settlement update, and there are thousands per tick.
But cost is the implementation's argument, and A-11 is the specification's
requirement — the implementation does not get to narrow a requirement by finding
it expensive.

Two ways to close it, and they lead to different code. Either A-11 holds in
release, and the guard is wrong — in which case what is affordable is a
state-level assertion once per tick rather than one per call, which is a
different implementation from the one written. Or A-11 means something narrower
than its heading says, and the requirement should say which.

This is due in the A-11 assert pass of phase 3, which will have to answer it for
every invariant in the list, not only this one.
