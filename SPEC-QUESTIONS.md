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
- **An objection is never resolved by its finder.** A human or the decider answers. The entry
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
  `docs/` by a human or the decider, and the entry is then marked with what it became.

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
**Status:** **closed 2026-09-20.** Resolution A applied to `docs/` on explicit
instruction: NFR-03, DEC-002 and the World note in `SIM-STATE` now carry the
subject form. Nothing in `docs/` states the entity-only form any more.

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

**That last sentence was wrong**, found while applying the change. The World
note at `SIM-STATE:26` — "No RNG state. Randomness is `Hash(worldSeed, entityId,
tick, channel, index)`" — carried the entity form too, and after the two agreed
lines it would have been the only place in `docs/` still saying it. The
inconsistency would have been created by the edit itself, so the same
substitution was applied there and declared, rather than leaving the
specification disagreeing with itself or hiding that the deposited text was
incomplete. Applied as:

> No RNG state. Randomness is `Hash(worldSeed, subject, tick, channel, index)`, under NFR-03 and DEC-081.

The **Rationale** of DEC-002 was left alone, deliberately. "Any entity's random
draw at any past tick can be recomputed directly" is narrower than the decision
above it and still true; narrowing is not falsehood, and rewriting a rationale
that was not part of the agreed change would be the edit creeping.

---

### SQ-004 — two phase-3 open items need a measurement phase 3 cannot take

**Raised:** 2026-09-20, writing the plan for the rest of phase 3.
**Cites:** `docs/SIM-REQ.md:756` §19, rows `P-17` and `P-01`;
`docs/SIM-REQ.md:748` §18, the phase 3 row; `AGENTS.md`, "Phase 3 contains no
domain logic".
**Blocks:** no. The plan's nine points stand either way; only the two items do
not close.

§19 says both close in phase 3, and gives the procedure:

> | P-17, settlement ceiling | Phase 3 | Measure per-settlement update cost, divide the budget by it |
> | P-01 validity | Phase 3 | Confirm the 5 µs and 10 µs per-entity estimates the budget rests on |

Both measure the cost of updating a settlement. Phase 3's own gate is **100k
empty ticks**, and a settlement update in phase 3 would be domain logic, which
the phase excludes and `AGENTS.md` says not to reach forward for. An empty tick
measures the scheduler and nothing under it, so neither number can be taken
where §19 puts it.

NFR-05 carries the same estimate with the same instruction — "*(estimate —
validate in Phase 3)*" — and has the same problem.

Three readings, none of them ours to pick:

1. **The items move to phase 4**, where the first subsystem gives a real update
   to time. The phase 3 gate stays as it is.
2. **Phase 3 builds a synthetic update** of declared cost — a stand-in that
   touches the arrays the way a real one will — and the measurement is of the
   harness, not of the economy. Cheap, and its number means only what the
   stand-in resembles.
3. **The estimates are confirmed on paper** from the array widths in
   `SIM-STATE`, which is arithmetic rather than measurement, and says so.

Filed rather than picked because the answer sets what phase 3's exit actually
proves. The plan records the one measurement empty ticks *can* give — ms per
empty tick — and does not call it either of these.

**Resolution chosen 2026-09-20: reading 1, and the entry was posed short.**
`P-63` and `P-64` were already in §16 — cost per settlement update at 9 and at
27 cohorts, 3.3 µs and 15.8 µs, "Measured, 5 of 12 phases" — and the entry above
does not mention them, because it was written from §19, §18 and NFR-05 without
reading §16. Reading 2, a synthetic update, is the benchmark those two figures
came from: already built, already thrown away, and proposing it was proposing to
pay twice for an answer the specification had.

**Applied to `docs/` by `0114ad2`**, the human's edit: `P-17` and `P-01` move to
Phase 4, each row now saying why phase 3 cannot take the measurement and what
the partial figures already cover; NFR-05's "*validate in Phase 3*" becomes
"*validate in Phase 4, when a settlement update exists to measure*". Phase 3's
gate is untouched, and ms per empty tick stays what the plan says it is:
neither of the two.

**Marked, not removed**, under `D-036`. What this entry cost, and it is the
half worth keeping: an objection raised against four sections of a document
whose parameter table was never opened. The citation rule at the top of this
file says the document, the identifier and the line; it does not say *read the
neighbouring sections first*, and this entry is the argument that it should.

---

### SQ-005 — `SIM-STATE`'s no-reflection note cites section 19, which is Open items

**Raised:** 2026-09-23, plan point 3, by the blind reviewer.
**Cites:** `docs/SIM-STATE.md:186` §Serialisation notes; `docs/SIM-REQ.md:756`
§19 "Open items"; `docs/SIM-REQ.md:771` §20 "Downstream constraints".
**Blocks:** no. Point 3 uses no reflection in the core either way, and the rule
the note states is not in doubt — only where it points.
**Status:** **closed 2026-09-23 by R-012.**

The note reads:

> - No reflection: the serialiser is generated or hand-written, per section 19 of SIM-REQ.

§19 of `SIM-REQ` is **Open items** — a table of six parameters awaiting
measurement, with no mention of reflection or serialisation. The rule lives in
§20, Downstream constraints, which is also where the ahead-of-time row sits and
which `core/BannedSymbols.txt` already cites correctly ("no reflection,
ahead-of-time safe (NFR-08, section 20)").

Most likely a section that renumbered and a cross-reference that did not. Filed
rather than assumed, because the two readings differ in what else the note
inherits: §20's rows are marked `Decided` and belong to a downstream consumer,
§19's are measurements this repository still owes.

### SQ-006 — commands waiting at a tick boundary influence the next tick, and `SIM-STATE` does not list them

**Raised:** 2026-09-29, plan point 5, by both reviewers.
**Cites:** `docs/SIM-STATE.md:12` §Rule; `docs/SIM-REQ.md:470` FR-A-01;
`docs/SIM-REQ.md:600` AC-02; `docs/SIM-DEC.md:318` DEC-030;
`docs/SIM-DEC.md:332` DEC-032.
**Blocks:** no. Point 5 needs only that commands apply at one point of the tick,
and the hash sequence it compares is the same either way. Point 7 is the first
place the answer changes code.
**Status:** **closed 2026-09-29 by R-021.**

`SIM-STATE` §Rule: "Anything that influences a future tick lives here." A
command the host has submitted and the tick loop has not yet drained influences
the next tick, and `SIM-STATE` lists no queue. Two worlds with equal state hashes
at a boundary diverge if their queues differ.

AC-02 reads the other way: "same seed and commands produce the same state hash
sequence" names commands beside the seed as an *input*, and DEC-030 makes the
command log a replay format, which is a record of inputs. Point 5 took that
reading — the queue sits beside the state, outside the hash (`D-077`).

The readings part at DEC-032, "a save is a full state snapshot": a save taken
while commands are pending either carries them, and the queue is state; or drops
them, and a reloaded world diverges from the one saved; or is refused until the
queue is empty, which the host must then arrange. Nothing in `docs/` chooses.

### SQ-007 — §17 wants every criterion driven by the harness alone, and `AC-03` is driven by the test suite

**Raised:** 2026-10-01, writing the moment-two request for the phase 3 gate.
**Cites:** `docs/SIM-REQ.md:596` §17; `docs/SIM-REQ.md:602` AC-03;
`docs/SIM-REQ.md:656` AC-19; `docs/SIM-REQ.md:748` §18, phase 3;
`docs/SIM-DEC.md:360` DEC-034, rationale.
**Blocks:** yes — the declaration of the phase 3 gate, not a point. No point is
open for it to fail: the plan's ten points are closed and their reserves
discharged. The plan's exit condition repeats §17's words, so the gate cannot be
called met until the reading is chosen.
**Status:** **closed 2026-10-01 by R-039.**

§17 opens: "Every criterion below runs headless in CI, driven by the
command-line harness alone." In CI today the harness drives one thing: the
100k-tick empty run, twice, compared byte for byte. `AC03_SaveRoundTrip`, the
in-process half of `AC02_DeterminismAcrossRuns` and the pinned sequences of
`AC02_TheGateSequencesArePinned` run under `dotnet test`. `sim` has no flag that
saves or loads, and it cannot submit a command, since phase 3 defines no kind a
host could submit (`DEC-086`, `R-030`).

`docs/` reads both ways:

- **The suite counts.** AC-19 asks that the core's "tests pass from the
  standalone project … on every commit", and DEC-034's rationale names "the CLI
  harness, the CI suite, and the parameter sweeps" as three things host
  independence allows. On this reading §17's "alone" means with no editor and no
  game client, and the gate is met as it stands.
- **The harness drives each criterion.** Read literally, "alone" excludes the
  suite. `AC-03` then needs a round-trip mode in `sim`, and the commanded half of
  `AC-02` needs a command a host can submit, which no phase 3 kind provides. The
  first is a small harness change; the second is not possible until the first
  command kind exists.

Nothing in `docs/` chooses between them. The reading taken so far, never stated,
is the first: points 7, 9 and 10 put their checks in the suite, and no audit
remarked on it.

### SQ-008 — a state hash at every tick costs the whole state every tick, and phase 4 makes the state large

**Raised:** 2026-10-01, in the look at the whole of phase 3 asked for by the
moment-two request.
**Cites:** `docs/SIM-REQ.md:496` NFR-01; `docs/SIM-REQ.md:600` AC-02;
`docs/SIM-REQ.md:508` NFR-07; `docs/SIM-REQ.md:526` P-01;
`docs/SIM-STATE.md:192` §Serialisation notes.
**Blocks:** no. Phase 3's state is five fields and an empty chronicle. The
question is phase 4's, from the first table it adds.
**Status:** **closed 2026-10-02 by R-043.**

NFR-01 asks for "an identical state hash at every tick" and AC-02 for "the same
state hash sequence"; `SIM-STATE` says the hash covers every field. Today
`StateHash.Of` folds every field and every chronicle row in use, so one hash
costs the whole state, and the checks take one per tick: `AC02_DeterminismAcrossRuns`
and `AC02_TheGateSequencesArePinned` build seven sequences of 100k ticks between
them, in each of CI's three test steps, and `sim --hashes` one per tick.

In phase 3 that is free. From phase 4 it grows with the state. At NFR-07's bound,
50 MB of settlement state, a hash folds some six million 64-bit values, and at
one or two nanoseconds a fold — **an estimate, not measured** — one hash is of
the order of P-01's whole budget of 10 ms per simulated day. Seven sequences of
100k ticks at that cost run for hours per test step. At a smaller phase 4 state
the figures shrink in proportion, but they grow with every table and with the
chronicle, which only appends.

The game itself need not hash at every tick: NFR-01 states a property of the
sequence, not a computation the core must run. What is at stake is how the
property is checked. `docs/` leaves open:

- **The full sequence, always.** Every tick hashed in full in CI, and the cost
  accepted, or the tick counts of the checks lowered — which the gate's "100k"
  does not allow for its own run.
- **An incremental hash**, maintained by the systems as they write. Every tick
  then has its hash at little cost, but every write to state must also update
  it, and a write that forgets is exactly the hole `SIM-STATE` names.
- **A sampled sequence in CI**: the full hash every k ticks, or on a cadence.
  Cheaper, and reads AC-02's "sequence" as less than every tick, which is the
  reading `R-034` declined for "machines" and not the implementer's to take.

The first phase 4 point that adds a table is the first place the answer changes
code, and the pins move there anyway (`R-034`, Cost).

### SQ-009 — FR-K-15 still defines the dungeon check on goods with a tier

**Raised:** 2026-10-05, reviewing `R-054` to `R-065` on `design/poverty-seed`.
**Cites:** `docs/SIM-REQ.md:420` FR-K-15; `docs/SIM-REQ.md:774` AC-56;
`docs/SIM-DEC.md` DEC-080; `docs/SIM-REQ.md:145` FR-P-03.
**Blocks:** no. Dungeons are events-subsystem work, and `R-063` puts no
dungeon-only material in phase 4. It must be answered before the point that
writes load validation, since the check is defined twice.
**Status:** **closed 2026-10-06 by R-077.**

FR-K-15 requires that "no tier 0 or tier 1 good depends, directly or
transitively, on such a material". Goods no longer carry a tier: FR-P-03 and
DEC-098 put it on needs, and the user rewrote AC-56 and DEC-080 to protect needs
rather than goods (proposal revision 9, reserve 2 of `R-001`). FR-K-15 was not in
the proposal and was not touched. The same load check now has two definitions,
and FR-K-15's has no referent. The obvious reading is AC-56's wording, which is
the user's decision. The implementer's own review of revision 8 missed this
requirement.

### SQ-010 — FR-P-03a says both "a value in each class" and "one value per class tree"

**Raised:** 2026-10-05, as SQ-009.
**Cites:** `docs/SIM-REQ.md:155` FR-P-03a; `docs/SIM-REQ.md:141` FR-P-01;
`docs/SIM-DEC.md:186` DEC-015; `docs/SIM-DEC.md:238` DEC-098; `R-060`.
**Blocks:** no. `R-063`'s content has no subclass, so no good meets the case. It
must be answered before the point that writes the goods record and its
validation.
**Status:** **closed 2026-10-06 by R-078.**

FR-P-03a's second sentence says a good "belongs to one or more classes and
carries a value per unit in each". `R-060` appended to the same requirement that
"a good carries one value per class tree … and validation at load refuses a good
with two values in one tree". A good in a class and its subclass must carry two
values by the first sentence and is refused for it by the last. FR-P-01, DEC-015
and DEC-098 repeat "a value in each".

### SQ-011 — no phase of the update writes the hunger deficit

**Raised:** 2026-10-05, as SQ-009.
**Cites:** `docs/SIM-REQ.md:111` FR-D-03a; `docs/SIM-ECON.md:36` phase 1,
`:41` phase 6, `:42` phase 7, `:47` phase 11, `:95` §Demand "Hunger";
`docs/SIM-REQ.md:803` §18, Needs.
**Blocks:** no. It must be answered before the needs subsystem's first point,
which by `R-052` owns FR-D-03a.
**Status:** **closed 2026-10-06 by R-079.**

FR-D-03a says unmet tier 0 need accumulates as a deficit. §18 places it under
Needs, whose `SIM-ECON` phase is 6. Need left unmet is known only after market
clearing in phase 7, and the deficit is read in phase 11 for mortality and in
phase 6 for the seed reserve. No row of the update table says where the deficit
grows or decays. Phase 1, where migrants join cohorts, does not cite FR-D-03a's
count-weighted average either. The order matters: a deficit updated in phase 7
reaches phase 6 one update late, and phase 11 in the same update.

### SQ-012 — how much seed is released is not defined

**Raised:** 2026-10-05, as SQ-009.
**Cites:** `docs/SIM-REQ.md:181` FR-P-10d; `docs/SIM-DEC.md:562` DEC-092;
`docs/SIM-LOOPS.md:86` B5c; `docs/SIM-REQ.md:111` FR-D-03a; P-66, P-70.
**Blocks:** no. It must be answered before the point that builds the seed
reserve.
**Status:** open.

FR-P-10d releases the reserve "only as far as needed to bring the deficit back
to P-66"; DEC-092 and B5c say "to hold it there". Under FR-D-03a the deficit
does not fall in proportion to food eaten: it grows by what is missing and
decays by P-70 per update only when the need is fully met. Eating released seed
can stop the growth but cannot bring back a deficit above P-66, so the quantity
is either "enough to meet this update's tier 0 need" (hold) or undefined (bring
back). The two texts also differ from each other.

### SQ-013 — DEC-085 still ties array widths to `ruleVersion` and allows content patches

**Raised:** 2026-10-05, as SQ-009.
**Cites:** `docs/SIM-DEC.md:58` and `:60` DEC-085; `docs/SIM-DEC.md:396`
DEC-100; `docs/SIM-STATE.md:26` `rulesData`; `R-065`.
**Blocks:** no. It must be answered before the first phase 4 point that adds
static data, the point `R-065` binds.
**Status:** open.

DEC-085's rationale says "widths come from static data versioned with
`ruleVersion`", and its cost that "a patch that changes a width, a new good for
instance, reshapes the arrays". Under DEC-100 `ruleVersion` is the logic's
version, widths come from the rules file, and an existing world cannot take new
content. `R-065` lists DEC-032, DEC-033 and DEC-089 as touched, not DEC-085.

### SQ-014 — generation parameters and the rules file are two inputs, and NFR-01 names one

**Raised:** 2026-10-05, as SQ-009.
**Cites:** `docs/SIM-REQ.md:527` NFR-01; `docs/SIM-REQ.md:641` AC-02;
`docs/SIM-REQ.md:517` FR-G-02; `docs/SIM-REQ.md:541` NFR-08;
`docs/SIM-STATE.md:24` `generationParams`; DEC-100.
**Blocks:** no. It must be answered before the point that builds the generator,
the first of phase 4 (`R-052`).
**Status:** open.

NFR-01 and AC-02 now list the inputs as seed, rules file and commands. FR-G-02,
NFR-08 and `SIM-STATE`'s `generationParams` keep generation parameters as a
separate saved input. Values such as P-02, P-12, P-54 and the kingdom count of
`R-063` either belong to the rules file, and then have two homes, or to
`generationParams`, and then the determinism requirement omits an input.

### SQ-015 — FR-A-05 sits in Phase 7, and phase 4 needs it first

**Raised:** 2026-10-05, as SQ-009.
**Cites:** `docs/SIM-REQ.md:507` FR-A-05; `docs/SIM-REQ.md:810` §18 "Outside
Phase 4"; `R-063`, `R-064`, `R-065`.
**Blocks:** no. It must be answered before the phase 4 plan is written, since
`R-052` reads `Satisfies:` against §18.
**Status:** open.

§18 sends "§13, what the host observes and does" to Phase 7, and FR-A-05 is in
§13. `R-063` and `R-064` place phase 4's content in the rules file, and `R-065`
owes its format from the first phase 4 point that adds static data. FR-A-05 has
no phase 4 subsystem in the table.

### SQ-016 — `SIM-STATE` lists one carry, and `SIM-ECON` now says every row holds them

**Raised:** 2026-10-05, as SQ-009.
**Cites:** `docs/SIM-ECON.md:105-107` §Spoilage and integer rounding;
`docs/SIM-STATE.md:59-75` §Cohort; `docs/SIM-REQ.md:177` FR-P-10b.
**Blocks:** no. It must be answered before the first point that applies a rate
to a cohort stock.
**Status:** open.

`SIM-ECON` says "the carry stays with its row and is cleared when the row's
count reaches zero", and names spoilage, wear, decay, interest and transit loss.
§Cohort lists only `transitionRemainder`: no spoilage carry per good, none for
wear, decay or interest. The harvest, area reaped × yield × condition, leaves a
fraction too. `SIM-STATE`'s rule is that anything influencing a future tick is
listed, and a carry is such a thing.

### SQ-017 — the yearly cap of FR-P-15 has no P-number and no state

**Raised:** 2026-10-05, as SQ-009.
**Cites:** `docs/SIM-REQ.md:193` FR-P-15; §16 opening paragraph;
`docs/SIM-DEC.md:520` DEC-047; `docs/SIM-STATE.md` §Settlement.
**Blocks:** no. It must be answered before the point that builds allocation
adjustment.
**Status:** open.

FR-P-15 makes "P-19 and the yearly cap" the default for an activity whose record
carries neither. The cap has no P-number, and §16 calls a driving constant
without one a defect. Enforcing a yearly cap needs the movement already made
this year per activity, which no state table holds.

### SQ-018 — the advance's collateral and its lender are not where credit looks for them

**Raised:** 2026-10-05, as SQ-009.
**Cites:** `docs/SIM-REQ.md:298` FR-C-24; `:300` FR-C-25; `:304` FR-C-27;
`docs/SIM-STATE.md:75` `advance`.
**Blocks:** no. It must be answered before the labour and income subsystem's
credit point.
**Status:** open.

FR-C-25 now collateralises an advance with the standing crop. FR-C-24 lists
collateral as "goods, durables, buildings", so a cohort with only a crop cannot
borrow by one requirement and can by the other. `advance` is a bare `int64`
with no counterparty category, while `debt` and `credit` carry one (FR-C-23).
FR-C-27 puts a write-off's loss "on the lender", and for an advance nothing says
who that is or where the matching claim sits.

### SQ-019 — `SIM-LOOPS`' gate check no longer matches its own tables

**Raised:** 2026-10-05, as SQ-009.
**Cites:** `docs/SIM-LOOPS.md:10-25` §Stocks; `:100` R12; `:152-155` §Gate
check; `docs/SIM-REQ.md:790` §18, phase 1 gate.
**Blocks:** no for code. It is a claim about the phase 1 gate, which the user
conditioned this batch on keeping closed.
**Status:** open.

- "Every flow has a source and a sink: Met", but §Stocks has no row for the
  standing crop (in by sowing; out by harvest and by loss when the window
  closes). Goods omit sowing, emigrants and seizure among outflows; population
  and currency omit emigrants in transit.
- R12 has no "Tuned by" line, while "Brakes have a tuning parameter" reads Met.
- The row "Met for R1, R2, R4, R5, R6, R7, R12" omits R9, R10 and R11, which
  have brakes. This predates the batch.

### SQ-020 — whether the consumer of §20 can ship and read the rules file

**Raised:** 2026-10-05, as SQ-009.
**Cites:** `docs/SIM-REQ.md` §20 "Downstream constraints"; DEC-100; FR-A-05;
`R-065` "Not verified"; reserve 4 of `R-001`.
**Blocks:** no. It must be answered before the point that builds the rules file,
since the format is what the consumer would ship.
**Status:** open — **the user's**, not the decider's: §20 is reserve 4. Answered
by the user on 2026-10-05, below; the decider turns the answer into a revision
of `R-065`.

`R-065` records as not verified "that the consumer of §20 can ship and read such
a file", and names it among what would overturn the ruling. §20's constraints
belong to a consumer this repository cannot see, so only the user can answer.

**The user's answer, 2026-10-05: a hybrid.** The base rules are compiled into
the core, as bytes generated from the same text source by the same compiler. An
external rules file, where the game can read one, replaces them: content patches
and modding where the platform allows, the base rules everywhere else. The rest
of `R-065` stands: the core validates whatever bytes it receives, the digest
names them, and the save carries the file the world runs on, which modding
makes indispensable, since a modded save cannot be reproduced without the
player's file. Raised with it, for the decider:

- DEC-100 and FR-A-05 gain the default and the rule that it applies when the
  host hands no file.
- Validation must refuse any malformed file without failing, since players will
  write them, and must cap the sizes a file may declare: goods, mestieri,
  recipes. The caps are parameters for §16, set after the phase 4 measurement,
  so that a mod cannot silently take the core past P-01.
- AC-04's bounds are a claim about the shipped content, not about modded
  content; a sentence should say so.

### SQ-021 — `SIM-STATE` open item 1 closed with more than `R-063` decided

**Raised:** 2026-10-05, as SQ-009.
**Cites:** `docs/SIM-STATE.md:210`; `R-063` "Age bands".
**Blocks:** no. It must be answered before the point that builds cohort rows.
**Status:** open.

`R-063` says "Three, closing `SIM-STATE` open item 1". The text in `docs/` says
"three age bands, children, working age and elderly, of which only working age
offers labour. A wider split is a format migration (NFR-08)". The names, the
rule that only working age offers labour, and the remark on migration are in no
ruling or proposal. The labour rule decides who sells labour under FR-C-05.

### SQ-022 — stale counts and leftover wording after the batch

**Raised:** 2026-10-05, as SQ-009.
**Cites:** below, item by item.
**Blocks:** no.
**Status:** open.

1. Phase counts: `docs/SIM-REQ.md:798` and `:821` "twelve phases",
   `:573-574` "5 of 12 phases", `:825` and `docs/SIM-DEC.md:478` the
   unmeasured phases, now that 8a exists.
2. `docs/SIM-REQ.md:571-574`: P-17, P-17a, P-63 and P-64 labelled "9 cohorts"
   and "27 cohorts", against the 12 and 36 rows of `R-063`.
3. `docs/SIM-DEC.md:188` DEC-015 "start with seven goods"; P-09 is 8.
4. `docs/SIM-DEC.md:600` DEC-052 "one damping constant", against DEC-096's rate
   per activity.
5. `docs/SIM-DEC.md:520` DEC-047's title "once per sowing", against FR-P-15
   (finding 4 of `R-059`, X-3, still open).
6. `docs/SIM-ECON.md:64` allocation adjustment "When: at sowing", while
   non-agricultural activities adjust at settlement cadence.
7. "Implements" column: phase 4 (`docs/SIM-ECON.md:39`) assigns open fields
   without FR-P-10e; phase 7 (`:42`) uses the seed reserve and the wealth curve
   without FR-P-10d and FR-J-22; FR-J-03's realignment runs in phase 12, which
   §18's Production row does not list (finding 5 of `R-059`).
8. `docs/SIM-DEC.md:562` DEC-092: a crop whose seed is not food "yields [it]
   alongside its harvest", while FR-P-10 and FR-P-10b (`docs/SIM-REQ.md:173`,
   `:177`) give a crop one yield.
9. `docs/SIM-REQ.md:141` FR-P-01 still lists "role", while FR-P-03 says the
   record says only whether a good is durable (`R-056`).
10. `docs/SIM-STATE.md:26` `rulesData` is state, but it is described again under
    §Static data, not state (`:161`).
11. `docs/SIM-REQ.md:507` FR-A-05 refuses "a file no build could accept", which
    admits a file only a later build accepts; DEC-089 refuses on what this build
    reads.
12. `docs/SIM-REQ.md:308` FR-C-29 and `docs/SIM-DEC.md:828` DEC-076 tie rent to
    "the wealth axis", which the key has had since DEC-054.
13. `docs/SIM-REQ.md:626` P-71 is cited by no requirement; FR-P-10d and DEC-092
    name only P-67.
14. `docs/SIM-REQ.md:269` FR-C-04 still says "a clearing pass per good", and
    `SIM-ECON` §Demand "per cohort per good", while rationed demand now falls
    from one good to another within a class (DEC-099).

### SQ-023 — the user wants hundreds of goods and tens of mestieri, and the state is dense per good

**Raised:** 2026-10-05, by the user, in the review of the rules file (`SQ-020`)
and of P-01.
**Cites:** `docs/SIM-REQ.md:559` P-01; `:535` NFR-05; `:539` NFR-07;
`:571-574` P-17, P-17a, P-63, P-64; `:628` P-09; `:631` P-12;
`docs/SIM-STATE.md:64` `goods int[]` "one per good"; `docs/SIM-REQ.md:53`
FR-W-08; `:217` FR-J-02; `docs/SIM-DEC.md:612` DEC-054; `docs/SIM-REQ.md:155`
FR-P-03a; `docs/SIM-DEC.md:238` DEC-098; `docs/SIM-REQ.md:269` FR-C-04;
`docs/SIM-ECON.md:42` phase 7; `docs/SIM-REQ.md:670` AC-09; `:682` AC-10.
**Blocks:** **yes, the phase 4 plan.** Its first points fix the shape of state,
the save format and the hash. Dense per-good arrays written there would need a
format migration to become sparse (NFR-08).
**Status:** open — the direction is the user's; the requirements are the
decider's; AC-09 and AC-10 are reserve 2 of `R-001`.

**What the user wants.** A catalogue of hundreds of goods and tens of mestieri.
`R-063` approved 8 goods for phase 4 as starting content; the question is what
the architecture must carry, not what phase 4 ships.

**Why it does not fit as written.** An estimate by extrapolation from P-63 and
DEC-054, **not measured**: at 300 goods and about 41 cohort rows, market,
production and spoilage grow about ×40 with goods, and rationing ×8 to ×12 with
rows, since its cost grows with the square of the row count. A settlement update
would cost some 3–5 ms, and a simulated day at 2,000 settlements about one
second, a hundred times P-01. Memory fails first: one integer per good per
cohort, 2,000 × 41 × 300 × 4 bytes, is about 100 MB of holdings, twice that with
a carry per good, against NFR-07's 50 MB.

**The user's answers.**

1. **Goods and mestieri live in the world, not everywhere.** A village holds a
   few goods and a few mestieri; a city many. The reading is **state sparse per
   settlement**: a settlement holds rows only for the mestieri present and
   holdings only for the goods present, so cost follows what is local, not the
   catalogue.
2. **Whether a good has its own market depends on the good.** Important goods
   (grain, iron, wool) clear on a market of their own, with their own price.
   Minor goods are variants of their class: they share the class's clearing,
   and their price differs by quality, not by market. The number of clearings
   stays in the tens while the goods run to hundreds.

**What it touches.**

| Where | Change |
|---|---|
| `SIM-STATE` §Cohort | `goods int[]` "one per good", and every carry per good, become sparse per settlement |
| FR-W-08, FR-J-02, DEC-054 | Rows exist for the mestieri present in a settlement, not for the whole vector |
| FR-P-03a, DEC-098 | A good says whether it clears on its own market or as a variant of its class; how a variant is priced and how demand splits among variants |
| FR-C-04, `SIM-ECON` phase 7 | The pass is per clearing, a principal good or a class, not per good (`SQ-022` item 14) |
| AC-09, AC-10 | Shock response and "every good reaches scarcity" are read per clearing, not per good; reserve 2, the user's |
| P-09, P-17, P-63, P-64, NFR-07 | P-09 becomes the catalogue's size; the budget is measured against local goods and rows |

**What is owed before the ruling.** A measurement, not an estimate: the cost of
one settlement update as local goods, clearings and rows vary, taken before the
state format is fixed. The figures above are the implementer's arithmetic. The
user also gives an order of magnitude, as content, for the clearings and rows of
a village against a city.

**The user's answer on who decides a good's market, 2026-10-05.** The author,
good by good: each good in the content declares either a market of its own or
that it is a variant of a named class, and validation checks only that a
variant's class has a market. The eight goods of `R-063` all have a market of
their own. The order of magnitude of clearings per place is answered through
`SQ-024`'s market places.

### SQ-024 — markets held by market places: five settlement types, and villages that trade at the nearest market

**Raised:** 2026-10-05, by the user, following `SQ-023`.
**Cites:** `docs/SIM-ECON.md:34-50` the update table, phase 7 above all;
`docs/SIM-REQ.md:338` FR-M-01; `:348` FR-M-03; `:370` FR-M-08; `:372` FR-M-09;
`:564` P-06; `:631` P-12; `docs/SIM-STATE.md:39` settlement `type`;
`docs/SIM-REQ.md:113` FR-D-04; `:125` FR-D-09; `:482` FR-I-05; `:513` FR-G-01.
**Blocks:** **yes, the phase 4 plan**, with `SQ-023`: where prices form and
where rationing runs is the shape of the market subsystem and of its state.
**Status:** open — the direction and the content are the user's; the rules are
the decider's.

**What the user proposes.** Prices form at market places, not in every
settlement. Settlement types become five, each with a number of markets:

| Type | Markets |
|---|---|
| Village | 0 — its people trade at the market of a larger settlement |
| Borgo | 1 |
| Cittadina | 1–2 |
| City | 2–4 |
| Metropolis | 3–5 |

**The user's answers to the implementer's questions.**

1. **Markets are specialised by goods.** Each market trades a group of goods
   (grain and food, livestock, cloth, metals…). A settlement's markets say which
   groups it trades, and within a settlement a good has one price. Higher-order
   goods are therefore found only in larger places.
2. **A village trades at the nearest market, dynamically.** Nearest by transport
   cost, and recomputed when places rise or fall. Since markets are
   specialised, the reading is **per group of goods**: a village may sell grain
   at the borgo next door and buy tools in a distant city.
3. **Type follows population.** A settlement rises or falls a type across
   thresholds; a borgo that declines loses its market, and its villages move to
   another.

**Why it matters beyond realism.** Phase 7, the dominant cost of the update,
would run only where there are markets, about 80 places under today's P-12
rather than 2,000, and in each only for the groups it trades. With `SQ-023`'s
sparse state it is the most concrete route found so far to hundreds of goods
within P-01. Not measured.

**Open, for the decider unless marked the user's.**

1. **The groups of goods each market trades, and the order in which a place
   gains them** as it rises a type. Static content: **the user's**.
2. **The population thresholds** of each type, with hysteresis and the cadence
   at which type is reassessed. Parameters for §16; the user gives an order of
   magnitude as content.
3. **Village to market.** In which phase of the update a village's surplus and
   demand reach its market and what it bought comes back; at what transport cost
   and delay; and what happens to a village whose market, for a group, is out of
   reach.
4. **What stays local.** Presumably labour, own consumption (phase 6) and credit
   stay in the village, and the price comes from its market. The destitute row,
   the seed reserve and rationing by ability to pay must each be placed.
5. **Assignment.** The hysteresis that keeps a village from flapping between two
   near-equal markets, the cadence of reassignment, ties by id. Whether it is
   state or derived each time.
6. **The rest of the model.** Migration attractiveness (FR-D-04) and price
   knowledge (FR-I-05) read the market's price; trade links (FR-M-03, FR-M-08,
   FR-M-09) join market places; degree caps (P-06) and the settlement mix (P-12)
   for five types instead of three; the generator (FR-G-01) places types and
   markets.

**The user's answers on the open points, 2026-10-05.**

- **Point 1, the groups of goods per market: deferred**, to be discussed with
  the user. The implementer's draft (food and staples from the borgo,
  manufactures from the cittadina, metals and tools and livestock from the
  city, luxuries at the metropolis) is neither approved nor rejected.
- **Within a type, population decides the number of markets.**
- **Point 2, population per type**, on a scale about half of Europe around
  1300:

  | Type | Population | Markets |
  |---|---|---|
  | Village | up to ~250 | 0 |
  | Borgo | 250 – 1,000 | 1 |
  | Cittadina | 1,000 – 5,000 | 1 below ~2,500, 2 above |
  | City | 5,000 – 25,000 | 2, 3 or 4, in three bands |
  | Metropolis | above 25,000 | 3, 4 or 5 |

  A settlement rises at a threshold and falls only below about 80% of it. The
  80% becomes a P-number; the thresholds are Provisional under `R-050`.
- **P-12, the mix at tick 0:** 2 metropolises, 8 cities, 30 cittadine, 60
  borghi, 1,900 villages, about 100 market places. Types then follow
  population, so P-12 describes the generated world only.
- **The generator places borghi where villages are dense**, so that no village
  starts too far from its food market.
