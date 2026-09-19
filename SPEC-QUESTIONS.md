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
**Status:** **closed 2026-09-19.** FR-X-02, DEC-055 and `SIM-STATE:123` now give
the five-coordinate form with the link as subject and the kind of transient as
channel.

The answer carried a clarification the question had not thought to ask, and it
is the more important half: **the tick coordinate is the one at which the
trajectory is generated, not the one being observed.** Without that, FR-X-03
does not hold — a draw keyed on the observing tick would re-roll every tick and
the caravans would flicker instead of being followable. The question had found
an inconsistency in arity and missed that the arity was hiding a semantics.

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
**Status:** **closed 2026-09-19.** The invariant split rather than the code
changing. A-11 is now "every fixed-point quantity held in state is inside its
declared range", checked always; **A-11b** is "no accumulator has overflowed",
checked in debug builds, with the affordability reason written beside it in
`SIM-STATE`.

The guard was right and the text of the invariant was wrong — the outcome this
channel exists to make possible, and the one a decision register cannot reach on
its own, because a register can only record that the code departed from the
specification.

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

### SQ-003 — `NFR-03` and `DEC-002` still carry the entity form that the documents citing them no longer use

**Raised:** 2026-09-19, reading `docs/` at revision 2026-09-19 to confirm what
`SQ-001` became.
**Cites:** `docs/SIM-REQ.md:500` (NFR-03), `docs/SIM-DEC.md:31` (DEC-002),
against `docs/SIM-REQ.md:401` (FR-X-02), `docs/SIM-DEC.md:744` (DEC-081).
**Blocks:** no. DEC-081 states the general form plainly and the implementation
already follows it.
**Status:** open.

FR-X-02 now reads "`Hash(worldSeed, subject, tick, channel, index)` **under
NFR-03**". NFR-03 reads `(world_seed, entity_id, tick, channel, index)`. The
requirement cites as its authority a text that does not state the form it
attributes to it, and DEC-002 has the same wording one level down.

This is the residue of `SQ-001`: the documents that *use* the draw were updated
and the two that *define* it were not. Possibly deliberate — DEC-081 can be read
as generalising DEC-002 rather than replacing it, with the entity form kept as
the canonical case. If so, FR-X-02's citation should point at DEC-081, which is
where the subject form is actually written.

Small, and worth filing precisely because it is small: `SQ-001` was the same
shape, went unrecorded for three days, and turned out to be hiding a semantic
question about which tick a trajectory is keyed on.

**Resolution chosen 2026-09-19: the definitions move.** `SIM-REQ` states what is
required, so a decision that changes the shape of the primitive leaves the
requirement stating it stale; and `DEC-081` already treats `DEC-002` as the
general statement it proves once. Rejected: re-pointing FR-X-02's citation at
DEC-081, which would leave two definitions of the draw disagreeing and make the
reader pick.

**Pending a human edit**, because `docs/` is not ours. The text below is exact
and mechanical — two lines, one in each document.

`docs/SIM-REQ.md:500`, NFR-03, from:

> **NFR-03 — Randomness.** Random values come from a stateless indexed hash over `(world_seed, entity_id, tick, channel, index)`. No sequential RNG stream in the core.

to:

> **NFR-03 — Randomness.** Random values come from a stateless indexed hash over `(world_seed, subject, tick, channel, index)`, the subject being a 64-bit key of which an entity handle is one case, under DEC-081. No sequential RNG stream in the core.

`docs/SIM-DEC.md:31`, DEC-002, from:

> Random values are computed as `Hash(world_seed, entity_id, tick, channel, index)`. No sequential RNG stream exists in the core.

to:

> Random values are computed as `Hash(world_seed, subject, tick, channel, index)`, the subject being a 64-bit key of which an entity handle is one case, under DEC-081. No sequential RNG stream exists in the core.

Its **Rationale** paragraph says "Any entity's random draw at any past tick can
be recomputed directly". That reads narrow now and is true of any subject; the
clause about deferred agents that follows it stands either way.

Nothing else in `docs/` needs to move: FR-X-02, DEC-055 and `SIM-STATE:123`
already carry the subject form, and DEC-081 to DEC-084 were written against it.
