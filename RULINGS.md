# Rulings

The decider's record, and its only memory. Every session of `/decide` starts by
reading this file whole and ends by committing to it.

The implementer raises questions (`SPEC-QUESTIONS.md`), records the choices the
specification did not make (`DECISIONS-OUTSIDE-SPEC.md`), proposes promotions
(`PROMOTIONS.md`) and asks for review (`REVIEW-REQUEST.md`). This file is where
those are answered. A ruling here binds the implementer as `docs/` does; where it
changed `docs/`, `docs/` is the authority and the ruling is the reason.

**Identifiers** `R-NNN`, in order, never reused, never renumbered. A ruling is
reversed only by a later one that names it.

---

## State

**Register watermark:** D-126 — audited by `R-106`. The next audit starts at the
first `D-NNN` after it.

The watermark is the last `D-NNN` audited, or `none`. `/plan-next` reads this
line to tell the human how many entries are outstanding, so keep its shape.

## Owed by the implementer

Actions a ruling requires of the code or of the plan. The implementer cites the
`R-NNN` in the commit that does it; the decider marks it done with that commit.

- ~~**`R-003`**, within plan point 5: no public member of the core exposes mutable
  world state for reading or writing.~~ **Done in `3d7fb31`**, checked by `R-023`.
- ~~**`R-021`**, within plan point 7: a save carries the commands still pending,
  in submission order, outside the state hash; a load puts them back in the queue.~~
  **Done in `7e2bdd0`**, checked by `R-030`.
- ~~**`R-022`**, no later than plan point 7: a command is a value with a kind and
  integer fields, and the drain applies it from `Runtime/Systems/`; `ICommand`'s
  `Apply` goes.~~ **Done in `7e2bdd0`**, checked by `R-030`.
- ~~**`R-024`**, no later than plan point 7: a chronicle entry records its cause's
  kind — event, player action or exogenous root — beside the cause id, as a
  column hashed and saved like the others; `Append` takes it.~~ **Done in
  `7e2bdd0`**, checked by `R-030`.
- ~~**`R-026`**: `core/Runtime/Chronicle/` goes, `.gitkeep` and folder.~~ **Done in
  `7e2bdd0`**, checked by `R-030`.
- ~~**`R-004`**, no later than plan point 7: a fresh world takes the core's current
  rule version; a different value enters state only through loading a save.~~
  **Done in `3d7fb31` and `7e2bdd0`**, checked by `R-030`.
- ~~**`R-034`**, as refined by **`R-036`**, before the phase 3 gate is declared
  passed: two hash sequences of 100k ticks are pinned in the repository, each as
  a value produced by a run — the gate's empty run, and the run with commands of
  `AC02_DeterminismAcrossRuns` — and every CI run compares its own against them.~~
  **Done in `2ec9fad`**, checked by `R-038`; its first run in CI is point 10's
  reserve.
- **`R-037`**, no later than the plan point that first builds cohort rows: the
  step "Tests, three wealth bands" builds worlds with three wealth bands, and the
  point's `Fails when:` names the three-band run ignoring its configuration.
- **`R-041`**, no later than the next plan point that changes the save format: a
  test that two worlds holding the same chronicle entries in columns of different
  capacity save to the same bytes, its `Fails when:` naming a writer that walks to
  the array's length.
- **`R-042`**, no later than the plan point that adds the first command kind: a
  load refuses a pending command whose kind this build does not know or whose
  fields fall outside what its kind declares, and the invariants of `SIM-STATE`
  that are claims about state are checked at load. `D-106`'s first two questions
  stay that point's to answer in its criteria.
- **`R-043`**, within the first phase 4 plan point that adds a table: the cost of
  one full state hash is measured against the cost of the tick, on the world the
  checks of `AC-02` run, and recorded in the register with the size chosen for
  those checks.
- ~~**`R-045`**, within the process plan point that writes `D-089`'s language
  rule: the five outcome markers become English in `AGENTS.md` and in the three
  plan skills, which write the English form and recognise both.~~ **Done in
  `58d8557`**, checked by `R-097`.
- **`R-046`**, as amended by **`R-066`**: the process plan holds the design's
  points 1 to 5, `plan-state` and the two read-only skills on it, and no
  simulator change; the first point of the phase 4 plan is run by the agents,
  and its criterion carries the design's point 6 check and fault. **`R-067`**:
  it stays there, and no earlier plan carries it.
- ~~**`R-067`**, in the cost measurement plan: each outcome line names the agents
  dispatched and those that do not apply to a `bench/` diff, with the reason, and
  none claims `R-046`'s check.~~ **Done in `93ef064` to `853e342`**, checked by
  `R-106`.
- **`R-047`**, within the process plan point that rewires `/plan-next` and
  `reviewers.md`: the reviewers' brief quotes the `Fails when:` line and asks for
  missing faults. And every moment-one request carries the `Fails when:` lines.
  The code-reviewer's half is done in `1ed624a`, checked by `R-093`; the brief's
  in `7b1fdbe`, checked by `R-099`. The moment-one half binds every plan.
- ~~**`R-048`**, from the next plan: the draft goes in the moment-one request, and
  `PLAN.md` is installed on its branch after the user's answer, in a commit that
  cites it; the process plan point that edits `AGENTS.md` writes the rule.~~
  **Done in `6d6d098` and `58d8557`**, checked by `R-097`: the rule now binds from
  `AGENTS.md`.
- ~~**`R-049`**, within the process plan point that rewires `/plan-next`: step 1
  refuses a plan's first point until the user has answered its moment-one
  request; until then the ruling binds as written.~~ **Done in `7b1fdbe`**,
  checked by `R-099`.
- **`R-050`**, from the phase 4 plan: every simulation constant cites a P-number
  of `SIM-REQ` §16, a new one proposed in the point that needs it.
- **`R-052`**, in every phase 4 plan: `Satisfies:` and `Serves` are read against
  the table of `SIM-REQ` §18.
- **`R-053`**, at the closing point of each phase 4 subsystem: `AC-01` and
  `AC-04` over what exists, against provisional bounds derived from `docs/`.
- **`R-065`**, within the first phase 4 plan point that adds static data or a
  simulation parameter: the rules file of DEC-100 — a binary format with a reader
  per version, its compiler in the harness from a text source in the repository,
  validation whole at creation and at load, the file held in World and carried by
  the save, and the rule version extended by the core's digest of it. The content
  of `R-063` and the curves of `R-064` go into it, and the check of `R-060` into its
  validation. **`R-088`**: and the base rules compiled into the core by the same
  compiler, used when the host hands no file; validation that returns a refusal
  and never fails, with the caps P-74 to P-76. **`R-092`**: and the three age
  bands with their labour flags, from the point that builds cohort rows.
- ~~**`R-070`**, before point 7 of the process plan runs: point 8 added to
  `PLAN.md`, placed between points 6 and 7, carrying `R-068`'s three changes.
  **`R-071`**: and a fourth, `tree-state` reading untracked names unquoted and
  printing no hash when a file cannot be read, with its `Fails when:` line.
  **`R-073`**: and a fifth, `mutate` refusing a patch whose rename or copy
  source lies outside `core/Runtime/` and `harness/`, with its `Fails when:`.
  **`R-074`**: and a sixth, `plan-state` reading "after the failure" by position
  in the file, not by number, with its `Fails when:`.
  **`R-094`**: and a seventh, `.claude/agents/coder.md`'s list of the paths it
  may not write gaining the two settings files, with its `Fails when:`.
  Point 8 added in `02293d6`, checked by `R-095`.~~ **Done in `9d8ea3c`**, its seven
  items, run before point 7; checked by `R-099`.
- ~~**`R-095`**, within point 8: a self-test case of `mutate` in which a call to
  `tree-state` fails and `mutate` exits 3, and its fault — `mutate` ignoring the
  failure — seen red, beside the frozen `Closed by:` under `R-005`'s case 1.~~
  **Done in `9d8ea3c`**, checked by `R-099`.
- ~~**`R-096`**, before the cost measurement plan's draft is next read through
  `plan-state -Draft`, and no later than the commit installing that plan: the
  current request's draft under `## Draft plan`, and "What a request holds"
  stating the heading for every moment-one request.~~ **Done in `e276547`**,
  checked by `R-099`.
- ~~**`R-098`**, before the process plan is archived: once the user has accepted
  workspace trust for the folder, point 4's live probe is run again. Green, the
  *Reserve discharged* line goes under point 4's outcome; red with trust
  accepted, the result goes to the register and the decider, and the reserve
  stays open.~~ **Done in `2801c19`**, checked by `R-100`.
- ~~**`R-100`**, before the first plan point dispatches the coder: point 4's live
  probe run again with `coder` dispatched in the background, the harness's
  default. Green, recorded in the register; red, the register and the decider,
  and no point dispatches the coder until the decider has ruled.~~ **Done in
  `93ef064`**, green, checked by `R-101`.
- ~~**`R-101`**, within point 2 of the cost measurement plan, beside its
  criterion: each cell's checksum at a fixed update count compared against a
  plain reference implementation of the five passes; every cell run on a state
  where rationing binds, so that the ranking reaches the checksum; faults (a)
  and (c) of `D-121` seen red against it; point 1's check rerun after any change
  to the stand-in's devices. Red on the last: the register and the decider
  before point 3.~~ **Done in `9c3a0b8`**, checked by `R-102`.
- ~~**`R-102`**, within point 3 of the cost measurement plan, beside its
  criterion: the bytes column, for point 2's cells and point 3's, counts the
  settlement state of `SIM-STATE` at its widths — the arrays standing for
  `goods`, `goodsCarry`, `currency` and the per-good price, and any index a
  layout holds — and leaves out content-derived rates and scratch; the figure
  recomputed independently from the sizes and compared, with an array dropped
  or counted twice seen red.~~ **Done in `385e713`**, checked by `R-103`.
- ~~**`R-103`**, within point 4 of the cost measurement plan, beside its
  criterion: each configuration's checksum — clearing at market places with
  the villages routed to them, and K = 1, 4 and 16 — at a fixed update count
  against an independent reference of the same clearing, on a state where
  rationing binds at a market for a good that routed demand asks for; routed
  demand left out of the market's ranking, and a split that drops a variant,
  seen red against it; the register stating how routed demand enters the
  ranking, and if as fewer entries than the cohort rows it comes from, the cost
  with every routed row ranked measured beside it.~~ **Done in `1faa5c0`**,
  checked by `R-104`.
- ~~**`R-104`**, within point 5 of the cost measurement plan, beside its
  criterion: every cell of the table from one run, in one session, on `Mkt`'s
  update code for all four layouts; the dense layout at market places measured
  under both readings of routed demand, every new configuration checked against
  `MktRef` where rationing binds, `RoutedUnranked` and a dense walk dropping its
  last good seen red; both readings in the table with per-type cells; beside each
  recomputed day the directly timed p99 of a day for every sparse layout and any
  dense one under 20 ms, the verdict on the higher; each day against 10 ms and
  5 ms; the MB column the world's total by `R-102`'s rule, recomputed
  independently; the measurement inputs named.~~ **Done in `853e342`**, checked
  by `R-105`.
- ~~**`R-105`**, before the cost measurement plan is archived: a line under point
  5's outcome, never in its place, reading *criterion defective* (`R-005`, case
  1) and naming `R-105`, in a commit without a `Plan-point:` trailer.~~ **Done in
  `c24e818`**, checked by `R-106`.

- **`R-107`**, from the phase 4 plan: state sparse under DEC-101 with A-18
  checked every tick, the first point building rows or holdings naming a zero
  entry left stored; FR-P-03d's declaration and refusal in the rules file; its
  price and split with `variantCarry` in market clearing, a check naming a
  variant offer that never sells.
- **`R-108`**, from the phase 4 plan: five types and assignments at
  generation, no village out of reach; every routed row one entry of its
  market's ranking, read without walking every settlement; type and assignment
  hysteresis both ways and eviction on a fall; P-79 and P-80 proposed.

## Escalated to the user

Rulings with verdict `escalated` or `pending`, until the user answers.

- **`R-106`, decision 1 of `D-126`**: whether an implementer session may
  dispatch the decider as an agent at the user's instruction. Reserve 1.

`R-082` was answered by `R-091`, `R-089` by `R-092`, `R-093` by `R-094`.

---

## Rulings

### R-001 — The decider holds the human's authority over `docs/`, within reserves

**Date:** 2026-09-23   **Origin:** user
**Verdict:** resolved
**docs/:** `docs/CLAUDE.md` rewritten to name the decider. No specification text
changed.

Granted by Federico Xella on 2026-09-23, in these words: *"Vorrei che tu fossi
l'agente decisore che mi aiuta in progettazione e analisi del simulatore. Hai
quindi il permesso di scrivere e modificare `/doc`, il dovere di valutare le
decisioni prese dall'implementatore e risolverne le obiezioni e i conflitti
sollevati."*

**What it covers.** Writing and editing `docs/`; answering `SPEC-QUESTIONS.md`
entries, `REVIEW-REQUEST.md` items and `PROMOTIONS.md` candidates; auditing
`DECISIONS-OUTSIDE-SPEC.md` and ratifying, promoting or rejecting its entries;
numbering new `DEC-` and requirement identifiers. The procedure is
`.claude/skills/decide/SKILL.md`.

**Two roles, kept apart.** The same model plays both, so the independence
`AGENTS.md` protects — a specification that is not an account of its own
implementation — has to come from procedure. A decider session never writes
code, tests or `PLAN.md`; an implementer session never writes `docs/`. And a
ruling's rationale must be valid on the merits, as it would have been before the
code existed; existing code counts as cost only.

**Reserves — escalated to the user, never ruled.**

1. **The governance itself**: this entry, the decide skill, `AGENTS.md`, and the
   split between the two roles.
2. **Weakening the gates**: removing or loosening an acceptance criterion, a
   phase gate of `SIM-REQ` §18, or `NFR-01` to `NFR-04`. Tightening them is
   within the delegation.
3. **`SIM-OBS`**: adding, removing or rewriting an observable. It states what the
   game is for, and its ceiling of five is declared a decision with a cost.
4. **`SIM-REQ` §20**: the downstream constraints belong to a consumer this
   repository cannot see.
5. **Anything the user decided personally** before this delegation, recorded as
   such in `docs/` or in the files above, unless the user reopens it.

**Cost.** Questions the human used to answer are answered by a model that also
writes the code, so a class of error both roles share can pass both. The user
remains the review of the reviewer: every ruling is a commit, readable and
revertible.
**Owed by the implementer:** nothing. `AGENTS.md` now tells it where answers
come from.
**Would overturn it:** the user withdrawing or narrowing the delegation, or a
ruling found to have edited `docs/` to match the code.
**Not verified:** the workflow has not run once. The first `/decide` session is
its test.

### R-002 — `/plan-next` ends its report with the count of entries awaiting the decider

**Date:** 2026-09-23   **Origin:** user
**Verdict:** resolved
**docs/:** unchanged

Nothing in the workflow of `R-001` told the human when `/decide` was due: the
decider's inbox is built only when someone runs it, and the only automatic
signal was `/plan-status` at a phase gate. The register audit is cheapest a
point at a time, before the next point builds on what the last one decided, so
the reminder belongs at the end of every point.

Step 8 of `.claude/skills/plan-next/SKILL.md` now closes the report with one
line, `Decider: N register entries not yet reviewed (D-xxx to D-yyy) — run
/decide`, counted from the watermark above against the `### D-NNN` headings of
`DECISIONS-OUTSIDE-SPEC.md`. The watermark line's shape is now part of that
contract.

This is governance, reserve 1 of `R-001`: made at the user's request, not on the
decider's initiative.

**Cost.** The watermark line can no longer be reworded freely; a change of shape
makes the count read zero or everything.
**Owed by the implementer:** nothing. The skill carries the step.
**Would overturn it:** the line being ignored in practice, which would argue for
a hook rather than a sentence in a report.
**Not verified:** the count ran once, by hand, against today's files, giving 67
entries from `D-001` to `D-067`. No `/plan-next` run has produced the line yet.

### R-003 — Mutable world state is not part of the core's public surface

**Date:** 2026-09-23   **Origin:** D-063, D-067 (the question passed to the human)
**Verdict:** resolved; the rest of `D-063` ratified
**docs/:** unchanged

`D-063` asked whether `WorldState`'s public mutable fields should become
`internal`, leaving FR-A-01 enforced by the compiler rather than by convention.
The specification already answers it, in three sentences read together:
FR-A-01, "Nothing outside the core writes world state directly"; FR-A-02,
"Nothing outside the core reads mutable world state"; FR-A-03, the game reads "a
read-only snapshot". `D-063` weighed only the first. The second is the stronger
one: it forbids the host from *reading* the live state, so a public field is a
violation available to every caller, not merely a write left unguarded. DEC-030
states the same boundary as architecture — commands in, events out, snapshot
for reading — and DEC-031 is why it matters beyond tidiness: once the core runs
on its own thread, a host reading a live field reads it mid-tick.

In a C# library the only enforcement that does not depend on every future
caller behaving is that the mutable state has no public member. That is the
ruling. The shape is the implementer's — `internal` fields, an `internal` type,
or a public handle with nothing mutable on it — provided no public member lets
code outside the core read or write the live state. What the host legitimately
needs goes through the core's own surface: point 5's queue for writes, the state
hash as a value for point 8's runner, the serialiser's bytes for saves, and in a
later phase the snapshot of FR-A-03.

The argument against, that every core type so far is public and the runner will
want some surface, is a statement about cost. The runner wants a surface; it
does not want the fields.

**The rest of `D-063` is ratified**: the container is a class, and NFR-10's "no
object references *inside*" governs its contents. The public-fields rationale in
`WorldState`'s remarks — "systems mutate state and nothing else does" — argues
for this ruling rather than against it, since systems live in the core.

**Cost.** Point 5 widens by one change to `WorldState` and whatever tests read
its fields. `FRW01_TheWorldRowCarriesTheDeclaredTypes` looks fields up with
`GetField(name)` and default binding flags, which see public members only, and
will fail with a null reference until it passes `BindingFlags.NonPublic`, as the
walker beside it already does. The harness does not touch `WorldState` today, so
it costs nothing there. Every later point writes against the narrower surface,
which is the reason to do it now.
**Owed by the implementer:** within point 5, which serves FR-A-01: no public
member of the core exposes mutable world state for reading or writing. Cite
`R-003` in that commit.
**Would overturn it:** a host requirement, in `SIM-REQ` or DEC-030's successor,
to read live state in place of the snapshot — which would be a change to FR-A-02
first.
**Not verified:** that the harness of point 8 can be written against the
narrower surface without a public accessor for something it needs; the runner
does not exist yet.

### R-004 — A new world carries the core's own rule version, not one the host chooses

**Date:** 2026-09-23   **Origin:** D-067 (the second question passed to the human)
**Verdict:** resolved
**docs/:** unchanged

`WorldState`'s constructor takes `ruleVersion` from its caller. `D-067` flagged
that a host can then record a version that produced nothing, and DEC-033's
materialisation would later run against it.

The specification decides this. NFR-09: "State records which rule version
*produced* it." `SIM-STATE` §Static data: goods, recipes and the other rule
tables are "Versioned with `ruleVersion`, never mutated at runtime". The rules
are part of the build, so the version that produces a fresh world is the version
of the build that creates it; there is nothing for a caller to elect. DEC-033
then needs exactly two sources of the field and no third: a new world, stamped
with the core's current version, and a loaded save, carrying the version it was
written under until materialisation brings it forward. A host-supplied value is
a third source, and the only one that can lie.

Where the current version lives — a constant in the core — is an implementation
detail the specification need not name, and it carries no domain: phase 3 may
hold it.

**Cost.** One constructor parameter goes, and
`FRW01_TheWorldRowCarriesTheDeclaredTypes`, which constructs a world with
version 3 and asserts it back, changes with it. Tests that need a state under a
foreign version reach the field through `InternalsVisibleTo`, as they will for
everything after `R-003`.
**Owed by the implementer:** a fresh world takes the core's current rule
version; a different value enters state only through loading a save. No later
than plan point 7, whose load path is that one legitimate source. Cite `R-004`.
**Would overturn it:** a requirement for a host to create a world under an older
rule set — a replay tool, say — which would be a new requirement in `SIM-REQ`,
not a constructor argument.
**Not verified:** nothing in `harness/` constructs a `WorldState` today, so no
host use of the parameter was found to break; checked by grep, not by build.

### R-005 — The failure path for a criterion found wrong in flight: two cases, one receiver

**Date:** 2026-09-23   **Origin:** review request 2026-09-20, item 5
**Verdict:** escalated — reserve 1 of `R-001`: the path lives in `AGENTS.md` and
`.claude/skills/plan-next/SKILL.md`
**docs/:** unchanged

Item 5 asks what happens between a point failing (`D-023`: closed `FALLITO`,
plan stops) and the rule that a wrong criterion is reported, never rewritten:
who is told, where the defect is written, whether the plan stops. `D-055`
deferred it to the close of the phase 3 plan, unless "the first point of phase 3
hitting a wrong criterion" made it concrete first.

**That has happened twice.** Point 1's criterion under-covered its "Does"
(`D-059`); point 2's contradicted its own "Does" and could not fail for the
reason it exists (`D-064`, `D-065`). Both times the implementer took the same
path unprompted: the point closed against the criterion as written, a test beside
it made the criterion mean something, the register named the defect, the
criterion stayed frozen, and the plan went on. The path exists in practice. What
is missing is the text, and a receiver — which since `R-001` exists: the decider
reads the register.

**Recommendation.** Write down the two cases the practice already distinguishes:

1. **The criterion is weaker than the point, or its words diverge from its
   "Does", and the code is right.** The point closes on the criterion as frozen,
   with the extra check beside it; the outcome line says *criterion defective*
   and names the register entry; the plan continues. The decider rules on the
   entry in its next audit, and any consequence lands under "Owed by the
   implementer", never in `PLAN.md`. This is what points 1 and 2 did.
2. **The criterion cannot be met by correct code**, or meeting it would build
   the wrong thing. The point closes `FALLITO` under `D-023`, with *criterion
   wrong* as the reason, and the plan stops. The receiver is the decider, which
   rules; resumption is a new point added with approval under `D-014`, never an
   amendment of the old one.

The line between the two is whether correct code can pass the check as written.
The cost of the recommendation is that case 1 lets a plan continue past a
defect nobody has yet ruled on; the alternative, stopping on every defect, would
have stopped phase 3 at each of its first two points for findings that changed
no code.

**Timing.** The text is two paragraphs in `/plan-next` step 8 and one sentence in
`AGENTS.md`. `D-055`'s objection — process work needs a plan, and two live plans
break the point lookup — is right for building mechanism, and weaker for
writing down a path already walked twice. Whether that counts as trivial under
`AGENTS.md` is itself the governance question, and is the user's.

**Cost.** Until answered, case 2 has no written path; the first criterion that
correct code cannot pass will be handled by judgement.
**Owed by the implementer:** nothing until the user answers.
**Would overturn it:** the user preferring that any defective criterion stop
the plan, which makes case 1 disappear into case 2.
**Not verified:** that no point of phase 3 has already met case 2 without
recording it as such; only the outcome lines of points 1 and 2 and `D-059` to
`D-067` were read.

### R-006 — State arrays hold values, never arrays

**Date:** 2026-09-23   **Origin:** D-064
**Verdict:** promoted as DEC-085
**docs/:** `SIM-DEC` gains DEC-085 under "Core representation"; `SIM-STATE`
§Serialisation notes gains one line pointing the per-row array fields at it.
Both revisions bumped.

`D-064` recorded that point 2's field walker admits one level of array and
rejects anything nested, and that this "decides part of the phase-4 state
layout, from a private helper in a test file". It does, and that is the reason
to promote it: every `int[4]`, `int[]`, `fixed[]`, `id[]` and bounded field in
`SIM-STATE` — a dozen rows across six tables — must now arrive in one shape, and
the only statement of which shape was a red test waiting for whoever met it.

The rule stands on the merits, independently of the walker. NFR-10 excludes
object references from serialised state, and an array of arrays is exactly that:
each inner array is a reference and a separate allocation. DEC-003's rationale,
bulk serialise, copy and hash, is the second argument. Both were written before
the code.

`D-064`'s own "would overturn it" — a field of genuinely variable width per row —
is already in `SIM-STATE`: the chronicle's `entities id[]`, the kingdom's
`holdings id[]`, the transient's `link or route id[]`. The chronicle is point 6 of
this plan. So DEC-085 does not stop at fixed widths; it names the two shapes a
variable-length field may take, a fixed capacity or a shared pool with offset and
count, and chooses neither. Which one each field takes is a decision for the
point that builds it, recorded in the register as usual.

**Cost.** Index arithmetic on every access to a wide field; a width change
becomes a reshape inside the NFR-08 migration; a pooled field needs a compaction
rule ordered by id. Existing code costs nothing: `WorldState` has no array field
yet, and the walker already enforces the rule.
**Owed by the implementer:** nothing beyond `docs/`, which now binds point 6's
`entities` field.
**Would overturn it:** a field whose shape neither a capacity nor a pool can
express without a cost the specification would not accept — a variable-length
field inside a variable-length field is the candidate.
**Not verified:** that every bounded field in `SIM-STATE` has a cap in `SIM-REQ`
§16 that makes the fixed-capacity shape available; `P-32` was seen for route
candidates, the others were not checked.

### R-007 — A tick in state is non-negative; how prior history is dated stays open

**Date:** 2026-09-23   **Origin:** D-057
**Verdict:** ratified; the question it uncovered recorded as open
**docs/:** `SIM-STATE` §Open gains item 4.

`D-057` guards `tick >= 0` in `Calendar`, debug only, reading FR-G-03's "for
P-18 years before tick 0" through DEC-040's "then takes the result as tick 0":
prehistory is relabelled, not numbered backwards. For the `tick` field of the
World row that reading is right, and the entry stands.

It leaves one thing unsaid, which is why this is not in the batch. Prior history
writes chronicle entries — DEC-040's rationale is that dynasties and ruins "rest
on events that actually happened" — and each entry carries a tick. If the run is
relabelled, those ticks are negative, and `Calendar` is asked for the date of
something its guard says cannot exist. If it is not, the run's first tick is not
0 and DEC-040 is read loosely. Neither document chooses, and the answer shapes
how the game shows "forty years ago". It belongs to phase 14, which implements
FR-G-03; it is recorded where the next reader of `SIM-STATE` will find it rather
than decided now, since nothing in phases 3 to 13 depends on it.

**Cost.** None now. If the relabelling reading wins, `Calendar` needs floor
division for negative ticks, which `D-057` rejected for a state the simulation
could not then occupy.
**Owed by the implementer:** nothing.
**Would overturn it:** a phase-14 ruling on the open item.
**Not verified:** whether any requirement in `SIM-OBS` or `SIM-REQ` §12 already
implies how old chronicle entries are dated for the player; §12 and `SIM-OBS`
were not read for it.

### R-008 — The register from `D-001` to `D-067` is ratified, save what R-003 to R-007 ruled

**Date:** 2026-09-23   **Origin:** register audit, the first
**Verdict:** ratified
**docs/:** unchanged

Every entry was read against the documents it cites. Ratified as written:
`D-001` to `D-056`, `D-058` to `D-062`, `D-065` to `D-067`. The rest were ruled
on their own: `D-063` by `R-003`, `D-067`'s two open questions by `R-003` and
`R-004`, `D-064` by `R-006`, `D-057` by `R-007`.

**By group.**

- **`D-001` to `D-010`, the primitives.** The four the human promoted (`D-001`,
  `D-002`, `D-006`, `D-007` as DEC-081 to DEC-084) and `D-004`, superseded when
  `SQ-002` split A-11, stand as marked; A-11b is in `SIM-STATE` and CI runs the
  Debug build, both checked. The rest are local choices with no reach beyond
  their code.
- **`D-011` to `D-055`, process.** How plans, reviews, questions and promotions
  work. Process is ratified or rejected, never promoted (`D-011`), and nothing
  here is wrong on the merits. Three carry something forward:
  `D-024` leaves open whether approving a plan that names `AGENTS.md` is
  permission to edit it — governance, reserve 1 of `R-001`, escalated below
  rather than ruled. `D-031` names a third outcome the mechanism lacks, a point
  done with one clause pending a human; it is the same family as `R-005` and
  should be answered with it. `D-055` became `R-005`.
- **`D-056` to `D-067`, phase 3.** Readings of `docs/` that hold: zero-based
  calendar divisions agree with FR-T-05a's formulas (`D-056`); `record` in
  `SIM-STATE` means a composite (`D-061`); one generation parameter until the
  generator needs more, under NFR-08's migration path (`D-062`). `D-059` and
  `D-065` are criterion defects handled as `R-005`'s case 1 recommends. `D-066`
  stands as an approximation until `SIM-STATE` §World rows carry identifiers,
  which is not worth adding for five fields.

**Cost.** None.
**Owed by the implementer:** nothing.
**Would overturn it:** any entry found to contradict `docs/` as it stood when the
entry was written; `R-008` does not make an entry right, it records that one
reading found nothing wrong.
**Not verified:** entries were checked against `docs/` and, for the claims named
above, against the repository; claims about commit hashes, test counts and
reviewer behaviour in the process entries were taken as written.

### R-009 — `R-005` accepted: a criterion found wrong in flight takes one of two paths

**Date:** 2026-09-23   **Origin:** user, on `R-005`
**Verdict:** resolved
**docs/:** unchanged

The user approved `R-005`'s recommendation as written, and asked for it to be
applied now rather than when the phase 3 plan closes: "approvo R-005", then
"Subito". This is governance, reserve 1 of `R-001`, applied at the user's
request.

Applied in two places. `AGENTS.md`, the "Criteria are frozen" rule, gains the
two cases in one sentence. `.claude/skills/plan-next/SKILL.md` gains them in
full: step 6, beside the rule that a wrong criterion is never rewritten, where
the outcome line is written, and a bullet in "When the point fails" saying how a
plan stopped on a wrong criterion resumes. `R-005` placed the text in step 8;
step 6 is where the outcome line is written, and that is where it belongs.

The two cases, as recommended: correct code passes a criterion that proves less
than the point asks — the point closes *criterio difettoso*, the plan continues,
the decider rules at its next audit; correct code cannot pass it — the point
closes `FALLITO`, the plan stops, and work resumes as a new point under `D-014`.

`D-055`'s objection, that process work needs a plan and two live plans break the
point lookup, does not arise: no plan was installed, and the change is text in
two instruction files, made by a decider session at the user's request, as
`R-002` was.

**Cost.** Case 1 lets a plan continue past a defect not yet ruled on, for as
long as the decider takes to run.
**Owed by the implementer:** nothing. The skill carries the procedure.
**Would overturn it:** a case-1 defect that turns out, at audit, to have changed
what later points built on — which would argue for stopping on every defect.
**Not verified:** `/plan-explain` and `/plan-status` were grepped, not read
whole, for text contradicting the new cases; both treat `FALLITO` as a stop and
any other outcome line as closed, which is what the cases need.

### R-010 — A plan authorises an edit to `AGENTS.md` only where its point says so

**Date:** 2026-09-23   **Origin:** user, on `D-024`
**Verdict:** resolved
**docs/:** unchanged

`D-024` left open whether approving a plan that names `AGENTS.md` is permission
to edit it. Three answers were put to the user: the plan suffices; each edit
needs its own sentence; or the plan suffices only for a point that states, in
its own text, that it edits the file and which rule it adds or changes. The user
chose the third.

The reasoning, as presented: plans are written by the agent, and in a plan of
nine points a line touching the standing instructions is easy to approve
without seeing, which would let an agent rewrite its own rules by announcing it.
Requiring the point to name the file and the rule makes the approval a
conscious one without a second round trip. The record matters more than usual
here: every commit carries the human's name whoever wrote it, so `git log`
cannot tell an agent's edit to `AGENTS.md` from the human's — at least four
past edits came from plan points, and only `D-012` and `D-024` say so.

Applied as a new section of `AGENTS.md`, after the decider's, stating the rule
and where the permission is recorded: `DECISIONS-OUTSIDE-SPEC.md`, or the ruling
in a decider session. This ruling is itself the record for that edit and for
`R-009`'s, both authorised by the user in this session. The decider's own limit
is unchanged: reserve 1 of `R-001` still keeps `AGENTS.md` out of its
initiative.

**Cost.** A plan point that edits `AGENTS.md` must be written with the rule it
changes stated up front, which is harder for a rule discovered in flight; that
case now needs its own permission.
**Owed by the implementer:** nothing. The rule applies from the next plan.
**Would overturn it:** an edit that passed under a point which named the rule
and still surprised the user, which would argue for the stricter answer.
**Not verified:** which of the past edits to `AGENTS.md` came from plan points
was inferred from their commit subjects matching plan points, not checked
against each plan.

### R-011 — A clause only a human can discharge closes the point with a reserve

**Date:** 2026-09-23   **Origin:** user, on `D-031`
**Verdict:** resolved
**docs/:** unchanged

`D-031` recorded point 12 of the twelve-gaps plan closing with one clause done
and one waiting on a human — a pull request nobody had asked the agent to open —
and named the gap: a point is either closed, which would have claimed the clause
met, or `FALLITO`, which would have stopped the plan for something that had not
failed. Three answers were put to the user: keep two outcomes and leave such a
point open; add a third outcome; or forbid criteria that depend on a human. The
user chose the second: "sì, applica B".

The case is not historical. CI runs on push (`on: [push, pull_request]`), the
agent does not push unasked, and `fase-3-kernel` is fifteen commits ahead of its
remote. Points 8 and 9 of the phase 3 plan each carry a clause that CI must run,
so both would meet `D-031` with no rule to follow.

**The rule.** The work done and verified, and missing only an action the agent
may not take, the point closes *Chiuso con riserva*, naming the clause and the
action; the plan continues. When the human acts, the clause's check is run and a
*Riserva sciolta* line is added under the outcome line, never in its place, in a
commit without a `Plan-point:` trailer. **A reserve still open keeps the plan's
exit condition unmet** — for the phase 3 plan, the gate — which is what stops a
reserve from being forgotten. A reserve is not for work the agent could do.

Applied in `AGENTS.md`, one sentence beside `R-005`'s cases; in `/plan-next`,
step 6 and the "every point closed" check; in `/plan-status`, steps 3 and 7; in
`/plan-explain`, step 4. This ruling is the record of the user's permission for
the `AGENTS.md` edit, under `R-010`.

Option C, keeping human-dependent clauses out of criteria and into a plan's
preconditions as the phase 3 plan did for §20, stays good practice for whoever
writes the next plan. It is not a rule: it cannot help points already frozen.

**Cost.** A plan can read as all points closed while its exit condition is not
met; the skills now say why, but a reader of `PLAN.md` alone has to spot the
marker.
**Owed by the implementer:** nothing. The skills carry the procedure.
**Would overturn it:** reserves used for work the agent could have done, which
would argue for C as a rule.
**Not verified:** that a `*Riserva sciolta*` line under an outcome line survives
step 6's re-read table in `/plan-next` without reading as a change to a closed
point; the table checks a point's own text and an existing outcome line, not
lines below it.

### R-012 — Consumer constraints are cited as `SIM-REQ` section 20, in all four places

**Date:** 2026-09-23   **Origin:** SQ-005
**Verdict:** specified
**docs/:** `SIM-STATE` §Serialisation notes, the no-reflection line: section 19 →
20. `SIM-DEC` DEC-034a, decision and cost: section 17 → 20, twice. `SIM-REQ`
NFR-11: section 17 → 20; revision bumped. No rule's content changed.

`SQ-005` is right: §19 of `SIM-REQ` is Open items, six parameters awaiting
measurement, with nothing on reflection or serialisation. The rule the note
invokes — no reflection-based serialisation, so an ahead-of-time consumer links
the core unchanged — is the first of §20's "rules that follow", and
`core/BannedSymbols.txt` already cites it there.

Grepping `docs/` for every numbered section reference found four, and none
pointed where it meant. The other three are the same failure one step removed:
NFR-11 and DEC-034a place the runtime profile and language level in "section
17", which is Acceptance criteria and names neither; both are §20's first two
rows. DEC-034a is the decision that such constraints live in *one place*, so a
wrong address there defeats the decision itself. All four now say section 20.

The entry's worry, that the two readings differ in what the note inherits, is
answered by the text: §20's rows are `Decided` and belong to the consumer, and
that is exactly the status the no-reflection rule has. Nothing in §19 could have
been meant. This touches only references *to* §20, not §20, so reserve 4 of
`R-001` does not apply.

**Cost.** None to code. The references are still bare numbers, so the next
renumbering breaks them again; naming the section as well would prevent it and
was not done, since no entry asked for it.
**Owed by the implementer:** nothing.
**Would overturn it:** evidence that §19 or §17 once held the cited content and
something was meant to move back; `git log -S` finds §20 present since the
first commit.
**Not verified:** references written as a section title rather than a number,
or pointing into `SIM-REQ` from outside `docs/` other than `BannedSymbols.txt`,
were not searched.

### R-013 — A numbered section reference in `docs/` carries the section's title

**Date:** 2026-09-23   **Origin:** user, on `R-012`
**Verdict:** specified
**docs/:** the four references `R-012` corrected now read `section 20,
"Downstream constraints"`: `SIM-STATE` §Serialisation notes, `SIM-DEC` DEC-034a
twice, `SIM-REQ` NFR-11. No rule's content changed.

`R-012` left the references as bare numbers and named the cost: the next
renumbering breaks them again, silently, as it broke these. The user asked for
the title to be added: "Aggiungilo". With the title beside the number, a stale
number is visible to any reader — the title and the heading disagree — and the
target is still findable by name.

Applied to the four references that exist. The same form is expected of any
numbered reference added to `docs/` from now on; the decider holds its own edits
to it, and a bare number found later is a defect of the same kind as `SQ-005`.

**Cost.** A retitled section now breaks its references the way a renumbered one
did, but visibly, which is the point.
**Owed by the implementer:** nothing.
**Would overturn it:** references moving to stable anchors — identifiers for
sections, as requirements have — which would make both number and title
redundant.
**Not verified:** nothing beyond `R-012`'s own gaps; the grep for numbered
references in `docs/` was rerun and finds these four only.

### R-014 — The id of the settlement bucket is the row index

**Date:** 2026-09-28   **Origin:** D-074
**Verdict:** specified; the rest of `D-074` ratified
**docs/:** `SIM-REQ` FR-T-06 and `SIM-DEC` DEC-008 name `id` as the row index,
not the packed key of DEC-081. Both revisions bumped.

`D-074` records that the loop takes a settlement's id to be its row index and
that phase 4's generator must not issue ids that differ from it. The second half
is true by construction — NFR-10 indexes every row by `EntityId`, whose index
*is* the row — so the entry's real content is the first half, and there the
specification has two readings. FR-T-06 and DEC-008 write `id % 7`; `SIM-STATE`
types a settlement's id as `EntityId`, index plus generation; and DEC-081 packs
an `EntityId` into one 64-bit key, generation high, index low. Taken modulo 7,
the packed key and the index differ by `4 × generation`: the same buckets,
shifted. Nothing today chooses between them, and a phase-4 author holding
DEC-081's helper could reasonably reach for the key.

The index is right on the merits. FR-W-10 forbids deleting a settlement and
fixes the count, so no settlement row is ever reused and its generation carries
no information for the whole run; the index is the stable part of the identity,
and it is an integer id in the sense `AGENTS.md` requires of any ordering. The
packed key exists for the draw's coordinate space (DEC-081), not for scheduling.
Both readings satisfy DEC-008's rationale equally, so the tie is broken by the
one that depends on less.

**Cost.** None to code: `TickLoop` steps row indices. Should settlements ever be
deleted, which FR-W-10 forbids, the bucket of a reused row would not move with
its generation.
**Owed by the implementer:** nothing.
**Would overturn it:** a change to FR-W-10 that lets settlement rows be reused.
**Not verified:** nothing; FR-T-06, DEC-008, DEC-081, `SIM-STATE` §Settlement and
`TickLoop.cs` were read.

### R-015 — A-13 forbids drift as well as double firing, and is verified by test

**Date:** 2026-09-28   **Origin:** D-075
**Verdict:** specified; the rest of `D-075` ratified
**docs/:** `SIM-STATE` §Invariants: A-13 gains a second clause, consecutive
firings exactly one period apart; a paragraph under the table says how A-13 is
verified. Revision bumped.

`D-075` raises two things. The first is a defect in point 4's criterion, handled
as `R-005`'s case 1 and ratified as such. But the criterion only restated A-13,
so the defect is A-13's: "fires exactly once per its period" is satisfied by a
kingdom firing every 365 ticks for 364 years, which is the one-day-a-year drift
DEC-006a exists to exclude. Counting firings per period cannot see a firing that
slides a day at a time inside its period. Requiring consecutive firings to be
exactly one period apart can, and implies the first clause. This tightens an
invariant; no acceptance criterion or gate moves, so reserve 2 of `R-001` does
not apply.

The second is the blind reviewer's: `SIM-STATE` lists A-13 among asserts "every
tick", and nothing asserts it per tick. The other fourteen asserts are claims
about state, which a check can read after any tick. A-13 is a claim about the
schedule, and a firing leaves no trace in state — in phase 3 nothing at all, and
in phase 4 only the effects of the update, from which "this settlement ran
exactly once this week" cannot be recovered. A per-tick check would have to keep
a record of firings outside state, or be the loop checking its own arithmetic.
Neither buys anything a test does not: the schedule is a function of the tick
modulo 364, so a test over one year covers every case it can produce. That was
true of A-13 before any code, which is the test this answer has to pass.

Nothing of phase 3 owes the per-tick check of the other asserts: none has
anything to read until phase 4's subsystems exist, and AC-01, "invariants hold
at every tick of every run", is that phase's gate.

**Cost.** A-13 is the one entry of the table not checked at run time; a change to
the loop that only a long run would expose is caught by the suite, not by the
run. The name `FRT04_ConsecutiveFiringsOfALevelAreOnePeriodApart` no longer says
that it now verifies A-13's second clause.
**Owed by the implementer:** nothing. The spacing test point 4 wrote beside its
criterion already checks the new clause for every level.
**Would overturn it:** a schedule that depends on state — a settlement whose
cadence changes at run time — which would make A-13 a claim about state again.
**Not verified:** nothing; `FRT04_ConsecutiveFiringsOfALevelAreOnePeriodApart`
was read after this ruling's commit and checks the spacing of daily, basin,
kingdom and every settlement's bucket.

### R-016 — `Runtime/Systems/` holds every writer of state, not only the phases

**Date:** 2026-09-28   **Origin:** D-072
**Verdict:** escalated — reserve 1 of `R-001`: the sentence is in `AGENTS.md`,
and repeated in `core/Runtime/Systems/CLAUDE.md`
**docs/:** unchanged

"Structure" in `AGENTS.md` says `Runtime/Systems/` holds one file per phase of
the settlement update, and that those files are the only ones that write to
state. The tick loop writes `WorldState.Tick` and is not a phase, so it breaks
one half wherever it goes. `D-072` kept the write boundary and put it in
`Systems/`. The briefed reviewer is right that it will not be the last: point
5's command drain writes state, and so will phase 4's generator.

`docs/` does not state the sentence; FR-A-01 and DEC-030 say only that nothing
outside the core writes state. The folder rule is an instruction to the
implementer about where to look, and of its two halves the write boundary is
the one a reader relies on — to find every mutation of state by opening one
folder. "One file per phase, in `SIM-ECON` order" is a claim about the phases,
and survives as a claim about *those* files.

**Recommendation.** Reword the sentence in both files so the write boundary is
the rule and the phases are its largest member: *`Runtime/Systems/` holds every
file that writes simulation state, and nothing outside it does. One file per
phase of the settlement update, in the order given by `SIM-ECON`; the few
writers that are not phases — the tick loop, the command drain, the generator —
are named for what they do.* `D-072`'s placement then complies as it stands.

**Cost.** None to code. The folder's file listing stops being a readout of
`SIM-ECON`'s phase order, which the phases' own names still give.
**Owed by the implementer:** nothing until the user answers; `D-072`'s placement
stands meanwhile.
**Would overturn it:** the user preferring the phase rule, which would move the
loop out of `Systems/` and leave the write boundary to a second sentence.
**Not verified:** that no other file in the core writes `WorldState` today; the
core was listed, not grepped for writes.

### R-017 — A plan's criterion names the fault it must catch

**Date:** 2026-09-28   **Origin:** D-070 (the question passed to the decider)
**Verdict:** escalated — reserve 1 of `R-001`: how a plan's criteria are
written lives in `AGENTS.md` and the plan skills
**docs/:** unchanged

`D-070` asks whether a plan's "Closed by" clause should be required to name
every test that keeps the named one honest. The count behind the question: four
of the phase 3 plan's first four criteria were defective — `D-059`, `D-065`,
`D-070`, `D-075` — and in every case correct code passed, so `R-005`'s case 1
carried the plan on. Two were vacuous (an empty enumeration satisfied them), one
under-covered its "Does", one missed a fault (the 365-tick year). The implementer
found each one only by mutating the code after the fact.

Naming every guard test does not reach the cause: the guard tests did not exist
when the criterion was frozen, and a criterion that must name them could only be
written after the work. What the four defects share is that each criterion said
what should pass and nothing about what should fail.

**Recommendation.** A criterion states, beside the check, at least one fault it
must catch — *fails when a field never reaches the hash*, *fails when the kingdom
fires every 365 ticks* — and the point is not closed until that fault has been
introduced and seen to turn the check red. The implementer already does this
mutation by hand at every point; writing the fault first moves it before the
code, where a criterion that cannot fail for the reason it exists is visible on
reading. It applies to the next plan written, not to criteria already frozen.

**Cost.** Criteria take longer to write, and a fault named in advance can itself
be the wrong one. A plan point without a natural fault — a `Check: shallow` under
`D-032` — needs to say so.
**Owed by the implementer:** nothing until the user answers.
**Would overturn it:** the next plan's criteria turning out defective at the same
rate with faults named, which would say the defect is elsewhere.
**Not verified:** that `/plan-next` and the plan design note do not already ask
for this; neither was reread for it; `/plan-next` was
grepped and does not.

### R-018 — The register from `D-068` to `D-076` is ratified, save what R-014 to R-017 ruled

**Date:** 2026-09-28   **Origin:** register audit, `D-068` to `D-076`
**Verdict:** ratified
**docs/:** unchanged

Ruled on their own: `D-074` by `R-014`, `D-075` by `R-015`, `D-072` by `R-016`,
and the question in `D-070` by `R-017`. The rest of those entries, and the
following, are ratified as written.

- **`D-068`**, the digest as a static fold outside `WorldState`, reusing
  `Hash64.Mix` made `internal`. Checked: `Mix` is `internal`, the core's public
  surface is unchanged, and `InternalsVisibleTo` names the test assembly. One copy
  of the mixing constants is right; two would be a determinism hazard guarded by
  half the tests.
- **`D-069`**, the fold guarantees single-field injectivity and no more. The
  two-field collision was reproduced outside the repository: both states digest to
  `0xCE31A6F9C074C14D`. The correction to `Hash64.Of`'s comment is a comment. A
  save-file integrity check, if one is ever required, needs its own digest, as the
  entry says.
- **`D-070`**, criterion defect under `R-005`'s case 1; the pinned digest and the
  tripwire test beside it are the right additions, and the pin is what makes
  AC-02's "across builds" checkable at all.
- **`D-071`**: the hand-off to point 5 is folded into `R-003`'s owed action
  above; `SQ-005` was closed by `R-012`. **The two walkers stay apart**, as the
  entry left them: one answers NFR-10, the other NFR-01, and a single walker would
  make a change to what may be in state move what reaches the hash. The rejection
  of tying `StateHash.Seed` to `Hash64.GoldenGap` is right for the reason given.
- **`D-073`**, levels without a stagger fire on the first tick of their period,
  finest first, and the phase is deliberately not pinned by a test. It invents
  least — FR-T-06's formula with the id at zero — and `SIM-ECON`'s open item 4
  stays the place where phase 4 decides it.
- **`D-076`**, the reviewers' findings; nothing rejected, nothing left beyond
  `D-072`.

**Cost.** None.
**Owed by the implementer:** nothing beyond `R-003`'s note above.
**Would overturn it:** as for `R-008`, any entry found to contradict `docs/` as it
stood when written.
**Not verified:** test counts, reviewer behaviour and mutation results reported
in the entries were taken as written, except the 365-tick blind spot, which
follows from the arithmetic in `D-075`, and the spacing test, read for `R-015`.

### R-019 — `R-016` accepted: `Runtime/Systems/` holds every writer of state

**Date:** 2026-09-28   **Origin:** user, on `R-016`
**Verdict:** resolved
**docs/:** unchanged

The user approved `R-016`'s recommendation and asked for it to be applied:
"approvo R-016 e R-017, applicali". Governance, reserve 1 of `R-001`, applied at
the user's request.

Applied in the two files `R-016` named, with its wording: "Structure" in
`AGENTS.md`, and `core/Runtime/Systems/CLAUDE.md`. The write boundary is now the
rule, and the phases of the settlement update its largest member; `D-072`'s
placement of the tick loop complies as it stands. The second file is under
`core/`, which the decider does not otherwise touch; it is an instruction file,
not code, and the approval named it.

This ruling is the record of the user's permission for the `AGENTS.md` edit,
under `R-010`.

**Cost.** As in `R-016`: the folder's listing no longer reads as `SIM-ECON`'s
phase order.
**Owed by the implementer:** nothing.
**Would overturn it:** as in `R-016`.
**Not verified:** nothing; the repository's `CLAUDE.md` files were grepped for
the old sentence and only `Systems/` carried it.

### R-020 — `R-017` accepted: a plan's check names a fault and is seen to catch it

**Date:** 2026-09-28   **Origin:** user, on `R-017`
**Verdict:** resolved
**docs/:** unchanged

The user approved `R-017`'s recommendation and asked for it to be applied:
"approvo R-016 e R-017, applicali". Governance, reserve 1 of `R-001`, applied at
the user's request.

**The rule.** A plan point carries a `Fails when:` line naming at least one fault
its check must catch; the point closes only after that fault has been introduced
and seen to turn the check red, and the outcome line says so. A point with no
natural fault says so in the field. The rule applies to plans written from now;
the phase 3 plan's criteria are frozen and carry no such line, and its remaining
points are not faulted for it.

Applied in four places: `AGENTS.md`, one sentence under "A point is one commit
with a runnable check"; the plan design note, the field in the format and a
section giving the reason; `/plan-next` step 3, where the fault is introduced
and the check run; `/plan-explain`, where the field is reported and objected to
when it could not fail. `/plan-status` reads outcome lines and needed nothing.

This ruling is the record of the user's permission for the `AGENTS.md` edit,
under `R-010`.

**Cost.** As in `R-017`: criteria take longer to write, a named fault can itself
be the wrong one, and a check that catches its named fault can still miss
another.
**Owed by the implementer:** nothing. The rule binds whoever writes the next plan.
**Would overturn it:** as in `R-017`.
**Not verified:** that `/plan-status` reports nothing a `Fails when:` line would
change; it was grepped for `Closed by`, not read whole.

### R-021 — Pending commands are input, not state, and a save carries them

**Date:** 2026-09-29   **Origin:** SQ-006
**Verdict:** specified
**docs/:** `SIM-STATE` §Rule gains a paragraph, input is not state; §Serialisation
notes gains a line on pending commands. `SIM-DEC` DEC-032, decision and
rationale; `SIM-REQ` NFR-08. All three revisions bumped.

`SQ-006` sets `SIM-STATE` §Rule, "anything that influences a future tick lives
here", against AC-02 and NFR-01, which name the command sequence beside the seed
as what a run is given. A command submitted and not yet drained influences the
next tick, and is not in the inventory.

The two readings are not equal. If pending commands were state, the state hash
at the end of tick *t* would differ between a command submitted during *t* and
the same command submitted after it, though both apply at *t + 1* and every
later hash is equal. FR-A-01 applies commands "at a defined point in the tick"
precisely so that the moment of submission does not matter; putting the queue in
the hash makes it matter. NFR-01 is the stronger text: the command sequence is
an input the host supplies over time, and a command handed over early is still
that input. The number of ticks the caller asks for (FR-T-09) influences the
future too, and no one would list it in state. §Rule is about what the
simulation must carry to be closed; input is what it is given. The paragraph
added says so.

That settles the hash, not the save, and DEC-032 is where the readings part. Of
the three answers the entry lists, dropping loses the player's last action on
reload, and breaks DEC-032's own rationale, that a save reproduces a reported
bug exactly. Refusing a save while commands wait makes the host arrange an
empty queue, which it can only do by advancing time: a save would move the
world. Carrying them costs one section of the save file, and nothing else: they
stay outside the hash, and AC-03's round trip compares bytes, which include
them.

**Cost.** Point 7's serialiser writes a section for pending commands, so every
command kind needs a codec from the day it exists. Phase 3 defines none, so the
section is a count of zero, but it is versioned from the first write like the
rest (NFR-08). How commands are represented is `R-022`.
**Owed by the implementer:** within point 7: a save carries the pending
commands in submission order, outside the state hash, and a load returns them to
the queue. Cite `R-021`.
**Would overturn it:** a host that must save from inside a tick, between the
drain and the tick's end, where "pending" would need redefining; or a
requirement that the hash identify a world together with its queued input.
**Not verified:** nothing; FR-A-01, NFR-01, NFR-08, AC-02, AC-03, DEC-030 to
DEC-032, `SIM-STATE` §Rule and the queue and drain in `3d7fb31` were read.

### R-022 — A command is data, applied by a system

**Date:** 2026-09-29   **Origin:** D-077, D-080 (the question left to the human)
**Verdict:** promoted as DEC-086
**docs/:** `SIM-DEC` gains DEC-086 under "Integration and operations", after
DEC-030. Revision already bumped today by `R-021`.

Point 5 made a command an object with `Apply(WorldState)`, and the briefed
reviewer asked the question `D-077` left open: under `R-019` every future kind's
`Apply` writes state and must live in `Runtime/Systems/`, while commands as
immutable data dispatched by the drain keep the writers there by construction.
`D-077` deferred it because choosing the representation is choosing the replay
format. It is, and the replay format is no longer the only thing asking.

Three boundaries now require a command to serialise: the host hands it over
(FR-A-01), DEC-030 makes the command log a replay format, and since `R-021` a
save carries every command still pending. §20 forbids reflection, so each of
those needs a written codec either way; what differs is what it encodes. A
value with a kind and integer fields encodes as a state row does. An object's
behaviour does not encode at all: the codec writes a tag and fields and rebuilds
the object on load, which is the data representation with a class hierarchy in
front of it. The second argument is the rule that predates the code, "systems
mutate state, nothing else does": an `Apply` on the command is a writer defined
wherever the kind is. The third is FR-A-01 itself: the host is outside the core,
and a system applying a value can check it against the state of its tick; an
object applying itself carries whatever logic its builder gave it, and today
only `internal` stands in the way.

This is architecture — it binds every command kind of every later phase — so it
is promoted rather than specified.

**Cost.** `ICommand`, its `Apply`, and the tests' `Fold` stand-in reshape: a
value type and a dispatch in the drain. Every future kind costs a codec and a
dispatch entry besides its effect. The queue's `Queue<ICommand>` becomes a queue
of values, which also removes the one object reference per pending command.
**Owed by the implementer:** no later than plan point 7, whose serialiser must
write pending commands under `R-021`: a command is a value with a kind and
integer fields, and the drain applies it from `Runtime/Systems/`. Cite `R-022`.
**Would overturn it:** a command kind whose content cannot be stated as integer
fields — a free-text name typed by the player is the candidate — which would
need a bounded encoding, not behaviour.
**Not verified:** that FR-J-17 and the other requirements naming player actions
all fit integer fields; they were not reread for it.

### R-023 — The register from `D-077` to `D-080` is ratified, save what R-021 and R-022 ruled

**Date:** 2026-09-29   **Origin:** register audit, `D-077` to `D-080`
**Verdict:** ratified
**docs/:** unchanged

Ruled on their own: the queue's status and what a save does with it (`D-077`,
`SQ-006`) by `R-021`; commands as code or data (`D-077`, `D-080`) by `R-022`.

- **`D-077`, the rest.** The drain is the first thing a tick does, takes only the
  commands waiting when it starts, in submission order. `docs/` says "a defined
  point"; the start lets a command see the date its tick's levels see and puts
  `SIM-ECON`'s Arrivals after it, which is the order a player's action should
  have against the world's response. The single-threaded queue is right for the
  core, which may hold no threading primitive; DEC-031's handover is the host's.
- **`D-078`**, `R-003` met by an internal state type and a public `Simulation`
  handle. Checked in `3d7fb31`: `WorldState` and `StateHash` are `internal`;
  `Simulation` exposes a constructor, `Advance` and the hash as a value, and holds
  the state and queue privately. `R-003` is marked done. That its cost note
  wrongly predicted `FRW01_…` would break is `R-003`'s error, not the entry's.
  `Advance(-1)` throwing on the public entry is what `D-074` asked for. `R-004` is
  met on the public path; the rest stays owed to point 7, as `R-004` allows.
- **`D-079`**, the check and what it catches. The finding that reversing the
  order passes the named test is correct and is not a defect of the criterion:
  the criterion compares runs, and the order is pinned by the test beside it.
  Point 5 predates `R-020`; the faults were introduced anyway, which is what that
  rule now asks.
- **`D-080`**, the reviewers' findings; nothing rejected.

**Cost.** None.
**Owed by the implementer:** nothing new; `R-004`, `R-021` and `R-022` stand.
**Would overturn it:** as for `R-008`.
**Not verified:** build and test results and the mutation outcomes in `D-079`
were taken as written; the suite was not run.

### R-024 — A chronicle entry says which of FR-E-07's three causes it has

**Date:** 2026-09-30   **Origin:** D-085 (a finding left to the human)
**Verdict:** specified
**docs/:** `SIM-STATE` §Chronicle gains `causeKind`, and `cause` says when it
holds an id; `SIM-DEC` DEC-026 names the three causes. Both revisions bumped.

The briefed reviewer of point 6 found that `cause = 0` makes an exogenous root
and a player action alike, and passed it on as a phase-4 matter. It is not only
the code's: `docs/` disagrees with itself. FR-E-07 — "Every event records the
event that triggered it, or records the player action, or records itself as an
exogenous root" — requires three outcomes, and AC-11 tests two of them as the
ends a chain may reach. `SIM-STATE` types the one field that could record them as
"chronicle id", which can say only "this entry" or "none", and DEC-026 names only
"the event that triggered it". Point 6 built what `SIM-STATE` says, correctly.
AC-11 could never be checked against it: a chain ending in `None` might end in
either of the two causes AC-11 accepts, and in nothing at all.

The kind is a field of its own, not a sentinel in `cause`: the field list is
what `SIM-STATE` states, and two meanings folded into one integer are the reading
`D-085` found hidden. A player-action entry does not point at the command. A
command is input, not state (`R-021`), so a pointer from state into input would
make a save's meaning depend on how long a command log is kept; what the action
did is the entry's own tick, entities and location.

Timing. Phase 3 writes no event and no command kind, so nothing yet has a cause.
But point 7's serialiser writes the chronicle's columns into the first save
format, and a column added after it is a migration under NFR-08 for a field
known to be missing now. This is structure, not domain: it states the shape of
an entry, and no event is implemented to fill it.

**Cost.** A seventh column in the chronicle, its hash fold and the fixture that
walks it; `NFR01_TheDigestIsPinned` moves again; `Append`'s callers, all in
tests, pass a kind. A consistency rule — `cause` is none unless the kind is
event — is one more comparison in the throw of `D-082`.
**Owed by the implementer:** no later than plan point 7, a column recording the
cause's kind beside the cause id, hashed and saved like the others, and taken by
`Append`. Cite `R-024`.
**Would overturn it:** a fourth kind of cause — an entry caused by something no
event, player or exogenous draw produces — or a ruling that chains are traced
outside the chronicle.
**Not verified:** that FR-E-07's "player action" means only the host's commands
and not also a promoted agent's choices (phase 5); `SIM-REQ` §13 was not read
for it.

### R-025 — Whether an `EntityId` names its table stays open, until a second table issues one

**Date:** 2026-09-30   **Origin:** D-085 (a finding left to the human)
**Verdict:** specified — as an open item, not decided
**docs/:** `SIM-STATE` §Open gains item 5. Revision already bumped today by
`R-024`.

The briefed reviewer of point 6 found that `EntityId` carries no kind, so the
chronicle's mixed `entities` list cannot say which table an id belongs to. The
finding is right and wider than the chronicle. NFR-10 and DEC-081 define an
`EntityId` as index plus generation; nothing says whether the index is per table
or drawn from one space shared by all. DEC-083's rationale — "the generation
makes two distinct entities distinct keys" — is true under a shared space and
false under per-table spaces, where settlement 7 and agent 7 of the same
generation are one key and, in a shared channel, one draw. So `docs/` leans on an
answer it never gives.

It is not decided here. The two answers — one index space, or a kind carried
with the handle — differ in the generator, the row allocator, DEC-081's packing
and every column typed `id`, and the tables whose layout would pay for it arrive
in phase 4 (settlements, kingdoms) and phase 5 (agents). Deciding now would
choose phase 4's allocator from a phase-3 chronicle that holds no real entity.
Deciding late costs nothing yet: no state table other than the chronicle stores
an `EntityId`, and the chronicle writes none that name a row. The item says when
it falls due, which is what `R-007`'s open item did not and should have.

**Cost.** Until answered, a phase-4 author could pick per-table indices without
seeing that DEC-083 then needs a channel per entity table.
**Owed by the implementer:** nothing now; the item binds whichever point first
gives a second table `EntityId`s.
**Would overturn it:** a phase-4 plan point issuing settlement ids, which must
answer the item before it closes.
**Not verified:** whether `SIM-STATE` means kingdoms and trade links to carry
`EntityId`s at all; only the agent row types its id, and the settlement's
`EntityId` rests on NFR-10's "indexed by `EntityId`".

### R-026 — The empty `Chronicle/` folder goes; the rest of `D-085` is ratified

**Date:** 2026-09-30   **Origin:** D-085
**Verdict:** ratified, save what `R-024` and `R-025` ruled; one action owed
**docs/:** unchanged

`D-085` left three things to the human. The two phase-4 findings are `R-024` and
`R-025`. The third is two items of layout and taste.

- **`core/Runtime/Chronicle/.gitkeep`.** The rule exists: "Folders named after
  subsystems would suggest an isolation that does not exist" (`AGENTS.md`,
  Structure). The folder dates from the skeleton, before the chronicle had code;
  now that it has, the writer is `Systems/Chronicle.cs` and the columns are in
  `State/WorldState.cs`, and an empty folder of the subsystem's name points a
  reader at the one place the chronicle is not. Nothing is decided here that
  `AGENTS.md` had not; the implementer removes it. `Data/`, the other empty
  folder, names a kind of content — static data, `SIM-STATE` §Static data — not a
  subsystem, and is not ruled.
- **The two test names kept narrower than what they check** (`D-083`). Kept.
  Their prefixes still carry the requirement, which is what `AGENTS.md` asks of
  a name; the register entries citing them are a record and are not rewritten,
  so a rename would leave `D-065` and `D-070` pointing at nothing. `D-083`'s
  summary remark is the right mitigation.
- **"Every cast into the fold widens"**, not applied. The rejection stands: the
  claim the wording supports holds, and the correction is a comment any later
  commit may make as a trivial change.

The rest of `D-085` — the findings applied — is ratified. Each was read against
the entry it points to (`D-081` to `D-084`), not against the code; those entries
themselves are above the watermark and are audited in their turn.

**Cost.** None beyond one deleted file.
**Owed by the implementer:** remove `core/Runtime/Chronicle/`. Trivial under
`AGENTS.md`; cite `R-026`.
**Would overturn it:** a decision to organise the core by subsystem, which
`AGENTS.md` rules out.
**Not verified:** the applied findings were not checked in the code; that the
seven folders' other `.gitkeep` files are harmless beside real files was
assumed.

### R-027 — The register from `D-081` to `D-084` is ratified

**Date:** 2026-09-30   **Origin:** register audit, `D-081` to `D-084`
**Verdict:** ratified
**docs/:** unchanged

`D-080` was ratified by `R-023` and `D-085` by `R-024` to `R-026`; this covers
what lies between, point 6's chronicle. Each entry was read against `docs/` and
its claims about the code against `Systems/Chronicle.cs`, `State/WorldState.cs`
and `State/StateHash.cs`.

- **`D-081`, the chronicle's shape.** Checked: ids are row plus one from 1,
  `Chronicle.None = 0`, the tick is read from state, location is an `EntityId`,
  entities sit in one pool with start and count per row — DEC-085's second shape,
  as `R-006` left to this point — and the fold runs over the rows in use, never
  the capacity. `docs/` is silent on each and nothing contradicts it. Two later
  rulings bear on it without reversing it: under `R-024`, `0` means "no triggering
  entry", no longer "a root", and the kind says which of FR-E-07's causes it is;
  under `R-025`, how an `EntityId` names its table is open, which reaches
  `location` as well as `entities`. `int` ids hold 2^31 entries, far above what a
  sixty-year run writes at any rate the specification implies; retention (Open
  item 3) decides the rest.
- **`D-082`, a bad cause throws in Release.** Right on the merits: an asserted-away
  forward cause is a chain with no root carried into every save, which AC-11
  could never pass, and no ruling says internal code may not throw. It departs
  from the convention of `D-074` and `D-078` for a stated reason, which is what a
  convention allows. `R-024`'s consistency rule joins this throw.
- **`D-083`, the hash and its fixture.** Checked: after §World, the count, the six
  columns over the rows in use, then the pool up to `EntitiesInUse`. Folding the
  pool last is the one departure from `SIM-STATE`'s field order, named in the
  remark; `docs/` requires every field in the hash (§Serialisation notes), not an
  order. The narrowed injectivity remark is correct: the three fields that set
  how many folds follow fall outside the single-field argument. Perturbing every
  row in use rather than row 0 is the right fix to the reviewer's fault. The pin
  moving before any save exists costs nothing. The two test names are `R-026`'s.
- **`D-084`, the declared check.** A criterion defect under `R-005`'s case 1: "the
  greps" name nothing, correct code passes, and the proof of shape is in the
  three tests beside it. Point 6 predates `R-020`; the faults were introduced
  anyway. FR-I-04 served as structure only is right for phase 3, which has no
  reader.

**Cost.** None.
**Owed by the implementer:** nothing new; `R-024` and `R-026` stand.
**Would overturn it:** as for `R-008`, any entry found to contradict `docs/` as it
stood when written.
**Not verified:** the suite was not run; mutation results in `D-083` and `D-084`
and the reviewers' findings were taken as written.

### R-028 — The register from `D-086` to `D-089` is ratified: the agent workflow, as designed with the user

**Date:** 2026-09-30   **Origin:** register audit, `D-086` to `D-089`
**Verdict:** ratified
**docs/:** unchanged

The four entries record the design of the agent workflow
(`.claude/design/2026-09-30-agent-workflow.md`), approved by the user in
conversation and not yet built. All four are process: ratified or rejected,
never promoted (`D-011`). Their governance content — what each role may write,
what `AGENTS.md` gains, the language of the repository — is the user's decision
and reserve 1 of `R-001`; this ruling does not rule on it. It checks that the
entries say what was decided and that the facts they rest on hold, and it grants
no permission: each edit to `AGENTS.md` still needs a plan point naming it
(`R-010`).

- **`D-086`, built after phase 3.** Checked: `/plan-status` bounds a plan's range
  by archive commits and says itself that two live plans break the lookup first;
  `main` has neither `RULINGS.md` nor `.claude/skills/decide/`, and its
  `AGENTS.md` does not mention the decider. The reasons hold.
- **`D-087`, what each role may write.** Stands. One reading is recorded so the
  plan that builds it does not word it otherwise: "written only by the
  orchestrator" is a limit among the six agents, not on the decider. Rulings
  `R-009`, `R-011`, `R-019` and `R-020` applied governance to
  `.claude/skills/` from a decider session at the user's request, and the rule
  the design proposes for `AGENTS.md` — change only with the permission
  `AGENTS.md` requires for itself — allows that path. The converse matters as
  much: an orchestrator point touching `.claude/skills/decide/` is reserve 1
  whatever its plan says, and the design's "any change to `/decide`" is rightly
  among what is not built.
- **`D-088`, the tester first, the code-reviewer on every code point.** Checked:
  `D-065` and `D-070` each record a named test passing against an emptied walker;
  `reviewers.md` is read only for `Core: yes`, so a `Core: no` point is read by
  no reviewer today. Five of six closed points had a defective criterion (`D-059`,
  `D-065`, `D-070`, `D-075`, `D-084`; point 5's was not, `R-023`).
- **`D-089`, comment rules and English.** Stands, with one inaccuracy noted:
  the entry says every rule cites the entry that applied it, and C10 cites "the
  code's practice", not a finding. The plan point that writes the section should
  either cite a finding for C10 or say it records practice. `D-060` and `D-067`
  were read for C5 and C3 and support them; the other citations were not.

**Cost.** None now. The workflow's cost per point is the design's own open item.
**Owed by the implementer:** nothing; the entries bind the process plan that
builds the workflow, which is not yet written.
**Would overturn it:** the user revising the design before the plan is written;
or, for `D-087`'s reading, the user meaning to bar decider sessions from
`.claude/skills/`, which is theirs to say.
**Not verified:** citations of C1, C2, C4, C6 to C9 and S1; every claim in the
design's own "Not verified" list, none of which this audit could test.

### R-029 — The save format of `D-090` is ratified, and binds every later format to keep its reader

**Date:** 2026-09-30   **Origin:** D-090
**Verdict:** ratified
**docs/:** unchanged

`NFR-08`, DEC-032 and `SIM-STATE` §Serialisation notes say what a save holds and
that it is versioned with a migration path; §20 forbids reflection. Encoding,
byte order, layout, header and API are left open, and `D-090` fills each. Checked
in `7e2bdd0`: `Simulation.Save()` and `Simulation.Load(byte[])` are the only public
surface, `SaveFormat` is `internal` in `Runtime/Systems/`; the writer's order is
`StateHash`'s field for field — §World, the seven chronicle columns over the rows
in use, the pool up to `EntitiesInUse` — then the pending commands; `Read` checks
the magic, switches on the version and refuses an unknown one;
`NFR08_TheFormatIsPinned` pins length and SHA-256.

Each choice stands on the merits. A `byte[]` keeps the core off files and streams,
which `BannedSymbols.txt` and DEC-034 require, and DEC-032's "full snapshot" is
bounded by what the host holds anyway. Little-endian through `BinaryWriter` is
fixed by the framework, not the platform, which is what a save moved between
machines needs. The hash's order makes the save and the hash one walk of one
field list, so the fixture that keeps the hash complete keeps the save complete;
two orders would be two lists to keep in step. Writing `ChronicleEntityStart`
rather than deriving it is right: it is a field of `SIM-STATE`, and a save that
drops a field on the ground that it can be recomputed is a save that trusts the
recomputation — the load checks it instead. The two versions stay distinct:
the format version says how the bytes are laid out (NFR-08), `ruleVersion` which
rules produced the state (NFR-09, DEC-033), and a save carries both.

**What this binds.** "A later format keeps the reader of every earlier one" is the
plain reading of NFR-08's "migration path from day one", and it is ratified as
such. It is architecture in effect: phase 4 adds the settlement tables, so its
format 2 must read a format-1 save — a world whose settlements have no rows — and
bring it to the new shape. Nothing in `docs/` limits the duty to saves a player
holds, and this ruling does not add that limit: it would loosen NFR-08, and no
entry has asked for it. It is not promoted: NFR-08 already says it, and a
`SIM-DEC` entry would only repeat a requirement.

**Cost.** Every format change from phase 4 on carries a reader for each earlier
format, and a migration for worlds that predate a table, for saves that before a
release only tests hold. A `byte[]` caps a save at what one allocation holds.
**Owed by the implementer:** nothing.
**Would overturn it:** a ruling that the migration duty begins at the first
release, which would be a change to NFR-08; a host needing to stream saves;
`D-090`'s own conditions.
**Not verified:** the suite was not run; the SHA-256 pin and the tests' results
were taken from `D-095` and `D-096`. `BinaryWriter`'s byte order was taken from
its documentation, not tested on a big-endian machine, which DEC-034 excludes.

### R-030 — The register from `D-091` to `D-096` is ratified; the actions owed to point 7 are done

**Date:** 2026-09-30   **Origin:** register audit, `D-091` to `D-096`
**Verdict:** ratified
**docs/:** unchanged

`D-090` was ratified by `R-029`; this covers the rest of point 7. Each entry was
read against `docs/` and its claims about the code against `7e2bdd0`.

- **`D-091`, the load refuses what no build could have written.** Right on the
  merits: a save is input from outside the core, and a half-read world carried
  into later ticks is a determinism defect with no trace of its cause. Refusing a
  rule version above the build's follows from NFR-09's "produced it"; DEC-033
  speaks only of an older save brought forward, and nothing in `docs/` lets a
  build run rules it does not have. One exception type for input and another for
  a caller's error is a distinction worth keeping. The unchecked command kind is
  recorded honestly: a loaded command of unknown kind is dequeued and then
  throws, and the loss is unreachable in phase 3 only because no kind exists.
  **It falls due with the first command kind**, which must decide whether a load
  checks kinds or the drain refuses without consuming; the entry's own "would
  overturn it" names that moment, and this ruling binds it.
- **`D-092`, `R-022` applied.** Checked: `Command` is an `internal readonly
  struct`, `ICommand` is gone and a test asserts it; `ICommandEffects`,
  `CommandDrain` and every `TickLoop.Advance` overload are `internal`, and
  `Simulation.Advance` passes `CommandDrain`. The effect injected by tests is
  therefore reachable from no host, which is what DEC-086's "rather than
  trusting whoever built it" requires. Four `long`s read DEC-086's "fields that
  kind declares" as the kind declaring which of them it reads: acceptable while
  no kind exists, and a width change is a format version under `R-029`. That the
  host has no public way to submit a command yet is consistent with phase 3,
  which defines no kind; FR-A-01's host path arrives with the first one.
- **`D-093`, `R-024` applied.** Checked: `CauseKind` is 1 to 3 with zero none of
  them, and the column precedes `cause` as `SIM-STATE` §Chronicle orders them.
  The converse rule, an event must name an earlier entry, is `SIM-STATE`'s "the
  triggering entry when causeKind is event" and not an addition.
- **`D-094`, `R-004` and `R-026` completed.** Checked: `WorldState`'s constructor
  stamps `Simulation.CurrentRuleVersion`; `core/Runtime/Chronicle/` is gone.
- **`D-095`, criterion defective.** `R-005`'s first case, correctly applied: a
  round trip cannot see a change made symmetrically to writer and reader, so it
  proves no version number; the tests beside it do. The finding is general and
  worth keeping for later plans: a round trip criterion needs a pin beside it.
- **`D-096`, the reviewers' findings.** Nothing rejected, nothing left open; the
  one finding recorded rather than changed is `D-091`'s command kind, bound above.

The five actions owed to point 7 are marked done under "Owed by the implementer".

**Cost.** None now. The first command kind carries the question `D-091` leaves.
**Owed by the implementer:** nothing new.
**Would overturn it:** as for `R-008`, any entry found to contradict `docs/` as it
stood when written.
**Not verified:** the suite was not run; build and test counts and the mutation
results in `D-095` and `D-096` were taken as written.

### R-031 — Whether the harness's `ProjectReference` meets §20 is the user's to say

**Date:** 2026-10-01   **Origin:** D-099 (a finding left to the decider)
**Verdict:** escalated — reserve 4 of `R-001`: the rule is in `SIM-REQ` section
20, "Downstream constraints"
**docs/:** unchanged

`D-099` records that `harness/Sim.Harness.csproj` reaches the core through a
`ProjectReference`, while §20 says "The consumer project and the harness project
each compile that folder in place. Neither holds a copy." Checked: the harness
has no `Compile Include` of core sources and no copy of them; the reference
builds `core/Sim.Core.csproj`, `netstandard2.1` at C# 9, and links the assembly.

Two readings. **The rule is about copies**: the folder is the single source of
truth, the harness builds it where it lies through the core's own project, and
"in place" is satisfied. **The rule is about how each side compiles**: the
harness includes the sources as the consumer does (§20's integration form is
source import), so the harness builds the core the way the consumer will.

**Recommendation: the first reading; no change.** On the merits it is the
better one. Compiling the sources into the harness assembly would make every
`internal` member visible to the harness and undo `R-003`, which rests on
FR-A-02; it would also build the core under the harness's rules rather than its
own analyzers and `BannedSymbols.txt`, and §20's own next rules — separate
projects, the core as its own assembly — point the same way. What the second
reading would buy, a harness built as the consumer builds, is already the job of
the standalone core project that CI builds on every commit.

It is not ruled: the sentence belongs to §20, whose constraints are the
consumer's, and whether the consumer meant the harness to mirror its source
import is something this repository cannot see.

**Cost.** None under the recommendation. Under the second reading, the harness
changes shape and `R-003`'s boundary needs another enforcement.
**Owed by the implementer:** nothing until the user answers.
**Would overturn it:** the user, or the consumer through them, meaning the
harness to compile the sources as the consumer does.
**Not verified:** that the consumer's source import compiles the folder under
the same settings as `core/Sim.Core.csproj`; the consumer project is not here.

### R-032 — The register from `D-097` to `D-099` is ratified; the chronicle's CSV is phase 6's

**Date:** 2026-10-01   **Origin:** register audit, `D-097` to `D-099`
**Verdict:** ratified, save the §20 finding of `D-099`, escalated by `R-031`
**docs/:** unchanged

Point 8, the runner. Each entry was read against NFR-12, DEC-035, `SIM-REQ` §17
and §18, and its claims against `c6f786f`. The CI step was run locally: exit 0,
1000 lines, all 16 lowercase hex digits, all distinct; the old `--hash` now
exits 2.

- **`D-097`, the interface.** Stands. Required `--ticks` and `--seed` follow
  from AC-02 being stated per seed; refusing an unknown argument is right on the
  merits, and the old smoke step is the proof. Output as fixed bytes on stdout is
  what point 9's byte-for-byte comparison needs. P-02 as a constant is the
  generator default `SIM-REQ` names; the runner reads nothing but `Simulation`'s
  public surface, as `R-003` requires.
- **`D-098`, the CI step.** Stands. The distinctness check holds because the
  tick is a field of the hash and `D-069` guarantees single-field injectivity,
  and an empty world changes only its tick. Moving the 100k ticks to point 9
  weakens no gate: they are §18's gate for phase 3 and point 9's criterion, and
  the gate is not passed until point 9 runs them.
- **`D-099`, verification.** Stands. *Chiuso con riserva* is `R-011` applied:
  the push is the human's, and **the reserve keeps the plan's exit condition, the
  phase 3 gate, unmet until the step has run in CI.** The first finding is
  answered here. NFR-12 and DEC-035 list what the runner does, not when; §18
  gives the when, placing "CSV output" in phase 6 without distinguishing the
  chronicle from the metric series, and phase 3's content and gate name the
  runner and no CSV. So the chronicle's CSV is owed by phase 6, with the metric
  series. It is not needed earlier: phase 4 checks AC-11 through the suite, which
  reads the chronicle directly. The second finding is `R-031`.

**Cost.** None now. Phase 6's plan owes the chronicle's CSV as well as the
metric series; a phase-4 or phase-5 need to inspect chains outside the suite
would pull it forward.
**Owed by the implementer:** nothing.
**Would overturn it:** as for `R-008`; for the CSV, a criterion before phase 6
that must read the chronicle through the runner.
**Not verified:** the suite was not run; the mutation results and test counts in
`D-099` were taken as written. The CI step has not run in CI, which is the
reserve.

### R-033 — `R-031` accepted: the harness's `ProjectReference` meets §20

**Date:** 2026-10-01   **Origin:** user, on `R-031`
**Verdict:** resolved
**docs/:** unchanged

The user approved `R-031`'s recommendation: "approvo R-031". Reserve 4 of
`R-001`, answered by the user.

`SIM-REQ` section 20, "Downstream constraints", "each compile that folder in
place. Neither holds a copy" is read as a rule about copies: the core's folder is
the single source of truth, and the harness builds it where it lies through
`core/Sim.Core.csproj`. The harness's `ProjectReference` complies as it stands,
and it keeps the core's `internal` members out of the harness's reach, which
`R-003` relies on. The text of §20 is unchanged; the reading is recorded here.

**Cost.** None.
**Owed by the implementer:** nothing.
**Would overturn it:** the consumer requiring the harness to compile the sources
as the consumer's source import does.
**Not verified:** as in `R-031`: the consumer's compile settings.

### R-034 — `AC-02`'s "across builds and machines" is owed by the phase 3 gate: a pinned sequence, not an OS matrix

**Date:** 2026-10-01   **Origin:** D-102 (a finding left to the decider)
**Verdict:** resolved; one action owed
**docs/:** unchanged

`D-102` records that both runs of point 9, in the test and in CI, are one build
on one machine, and asks whether the gate owes more. `AC-02` reads "Same seed and
commands produce the same state hash sequence across builds and machines", and
the gate of `SIM-REQ` §18 is "AC-02, AC-03 green". Two runs of one build in one
place show that a run reads nothing but its seed and commands — no static left
over, no clock — which is necessary and is what point 9 asked for. They cannot
show the second half of the sentence. `NFR01_TheDigestIsPinned` does cover builds,
for the hash of one fixture state (`D-070`), so the hash function is checked
across builds; the sequence the loop produces is not.

Declaring the gate green on what exists would read `AC-02` short of its text.
That reading loosens an acceptance criterion and a phase gate, reserve 2 of
`R-001`, and is not the decider's to take. Holding the criterion to its text is
within the delegation, so the question is only what "machines" costs.

**Not an OS matrix.** `NFR-01` bounds determinism to "any Windows x64 build" and
DEC-034 targets Windows x64 alone. A Linux or ARM runner tests a platform the
specification excludes: a difference there would not be a defect under `NFR-01`,
and a gate that can fail on something no requirement forbids is a wrong gate.

**A pinned sequence.** A digest of the gate's hash sequence, produced by a run
and committed, then compared by every CI run against its own. The value comes
from another build on another machine than any runner that checks it, so every
fresh runner is a comparison across both, and a later commit that moves the
sequence without meaning to is caught the way `NFR01_TheDigestIsPinned` catches a
moved digest. If the comparison lives in the suite, the Debug step compares a
second build of the same commit as well. The pin must come from a run, never be
computed by the code that checks it: a checker that derives its own expected
value is a round trip, and `D-095` already showed a round trip proves nothing
made symmetric. The shape — which digest, in a test or in the workflow step, one
seed or two — is the implementer's.

**Cost.** The pin moves whenever the state of an empty world changes shape, which
phase 4 does with every table it adds; each such commit re-pins, as the digest
pin already makes it do. A re-pin is a statement that the sequence was meant to
change, and should say so in its commit. The plan's points are closed and frozen,
so the action enters the plan as a point added with the user's approval
(`D-014`); the gate stays unmet until it lands and CI has run it, alongside the
reserves of points 8 and 9 (`R-011`).
**Owed by the implementer:** before the phase 3 gate is declared passed, the hash
sequence of the gate's 100k empty ticks is pinned in the repository, as a value
produced by a run, and every CI run compares its own sequence against it. Cite
`R-034`.
**Would overturn it:** the user ruling that `AC-02`'s "machines" is met by
DEC-001's integer discipline without a check, which is reserve 2 and theirs; or
DEC-034 widened beyond Windows x64, which would make a matrix owed.
**Not verified:** that the development machine and GitHub's `windows-latest`
runners differ in processor or JIT tiering in any way that could expose a
divergence; the pin is insurance whose value on an integer-only core was not
measured.

### R-035 — The register from `D-100` to `D-102` is ratified, save the finding `R-034` resolved

**Date:** 2026-10-01   **Origin:** register audit, `D-100` to `D-102`
**Verdict:** ratified
**docs/:** unchanged

Point 9, the gate. Each entry was read against `AC-02`, `NFR-01`, §17 and §18, and
its claims against `f1e825b`. `AC02_DeterminismAcrossRuns` was run alone in
Release: green.

- **`D-100`, the test.** Stands. Checked: `Simulation.Advance` calls
  `TickLoop.Advance` with `NoLevels`, as the commanded half does directly, so
  both halves step the same buckets over 2000 settlements and the difference
  between them is the commands' alone. The commanded half going through the
  internal loop with the stand-in kind is what `R-030` left: no public submit
  exists until the first command kind, and the entry names that as its
  overturning. The three controls are what keep an equality of two runs from
  passing a world that ignores its seed or its input.
- **`D-101`, the step and the measurement.** Stands. Raw bytes through
  `Start-Process` keep PowerShell's decoding out of a byte-for-byte comparison.
  Taking the figure on 10M ticks net of a run of none is right, and the negative
  figure on 100k is the proof. The step prints the figure and fails on nothing
  but the exit code, which is correct: `P-01` and `P-17` are phase 4's
  (`SQ-004`), and a threshold on a shared runner would fail for the runner's
  load. The durable record of the figure is the entry itself; CI logs expire.
- **`D-102`, verification.** Stands. *Chiuso con riserva* is `R-011` applied, and
  the reserve keeps the gate unmet until the step has run in CI. The first
  finding is `R-034`, which owes a pinned sequence before the gate is declared
  passed. The second, a hundred times the gate's ticks for about a second, is
  accepted as the cost of a measurement the plan asked for.

**The gate, as the files now stand.** Three things stand between phase 3 and its
gate: the pushes that discharge the reserves of points 8 and 9, and `R-034`'s pin.
The request for review at moment two, which `AGENTS.md` requires at the gate, is
owed by the implementer when those have landed; this audit is not it.

**Cost.** None beyond `R-034`.
**Owed by the implementer:** nothing new; `R-034` stands.
**Would overturn it:** as for `R-008`, any entry found to contradict `docs/` as it
stood when written.
**Not verified:** the full suite and the workflow step were not run; the mutation
results, test counts and the timing figures in `D-101` and `D-102` were taken as
written.

### R-036 — `R-034` refined: the run with commands is pinned beside the empty one

**Date:** 2026-10-01   **Origin:** user, on `R-034`
**Verdict:** resolved
**docs/:** unchanged

Reading `R-034` again for the user showed a gap it had not stated. In an empty
world the only field that moves from tick to tick is `Tick`: seed, generation
parameters, rule version, `CurrencyTotal` and the chronicle stay where the
constructor put them. The pinned empty sequence is therefore the hash function
applied to a hundred thousand values of one field, close to what
`NFR01_TheDigestIsPinned` already checks on its fixture. It adds the runtime and
the machine, and little else, until phase 4 gives ticks something to change.

Three options were put to the user: keep `R-034` as written; pin the run with
commands as well; or defer the pin to phase 4 and declare the gate without it,
which is reserve 2 of `R-001`. The user chose the second: "approvo l'opzione 2,
applicala".

**The refinement.** Beside the empty sequence, the sequence of the run with
commands in `AC02_DeterminismAcrossRuns` is pinned too: one command of the
stand-in kind every 97 ticks, over 100k ticks. That run exercises the drain, the
order of application and a write to `CurrencyTotal` on every command, so the pin
has content in phase 3 and not only from phase 4. The command kind is the test's
own, since phase 3 defines none a host could submit (`DEC-086`, `R-030`); the pin
therefore lives in the suite, which also puts it under the Debug step and
compares a second build of each commit. Everything else in `R-034` stands: no OS
matrix, each value produced by a run and never by the code that checks it, the
shape the implementer's.

**Cost.** A second value to re-pin, and it moves for one more reason than the
first: any change to the stand-in kind or to `FoldEffects`, which are test code.
A re-pin for that reason says so in its commit, as `R-034` asks of any re-pin.
**Owed by the implementer:** `R-034`'s action, widened: both sequences pinned and
compared on every CI run before the phase 3 gate is declared passed. Cite
`R-034` and `R-036`.
**Would overturn it:** the first real command kind, which moves the run with
commands onto `Simulation` (`D-100`) and should re-pin it with that kind in place
of the stand-in.
**Not verified:** that the stand-in's effect, a fold into `CurrencyTotal`, cannot
cancel over the run so that the pinned sequence says less than it appears to;
`AC02_DeterminismAcrossRuns`'s control that the run with commands differs from
the empty one argues against it, and was run green in this session.

### R-037 — The three-band CI step must vary the bands from the first cohort table

**Date:** 2026-10-01   **Origin:** D-104 (a finding recorded, not acted on)
**Verdict:** resolved; one action owed
**docs/:** unchanged

`D-104` found that `SIM_WEALTH_BANDS` is read by nothing. Checked: `ci.yml` sets
it on three steps, and no `.cs`, `.props` or project file names it, as has been
the case since the skeleton (`fe7c2c0`). Nor can the core read it:
`BannedSymbols.txt` bans `System.Environment` in the core. So "Tests, three wealth
bands" runs the same build and the same suite as the step before it.

In phase 3 the outcome is correct. No cohort exists, AC-28 has nothing to test,
and no gate of `SIM-REQ` §18 up to this one names it. The danger comes later.
FR-W-09 says the three-band configuration "exists to break any code that assumed
a single row per mestiere", and DEC-054 rests on it: shipping the general key is
safe *because* CI catches code that assumes the degenerate one. A step that
varies nothing passes that code, and on the day the cohort table arrives it
would read as AC-28 met. This is the defect of `D-065` and `D-070` a phase
early: a check that cannot fail for the reason it exists.

`docs/` needs nothing. FR-W-08, FR-W-09, AC-28, DEC-054 and `SIM-STATE` §Cohort
say what the configuration is and what it is for. How the band count reaches a
world is the implementer's: the core reads no environment, so the test side
supplies it, and where it lives follows §Rule as for any other value. That
point records it in the register as usual.

The step stays until then. It costs one run, and it is where the check will go.

**Cost.** The point that builds cohorts carries one more clause and one more
fault to introduce. Until it lands, AC-28 is satisfied vacuously, which is
correct while no cohort exists and wrong the moment one does.
**Owed by the implementer:** no later than the plan point that first builds
cohort rows, the three-band step builds worlds with three wealth bands per
mestiere, and the point's `Fails when:` names the three-band run ignoring its
configuration and sees it red. Cite `R-037`.
**Would overturn it:** DEC-054 revisited, with the wealth axis dropped or fixed
at one, which would leave FR-W-09 nothing to guard.
**Not verified:** that phase 4's first cohort point is demography, as §18's
order suggests; the band count's carrier (generation input, build property) was
not examined, and is that point's decision.

### R-038 — The register from `D-103` to `D-104` is ratified; `R-034`'s pin is done

**Date:** 2026-10-01   **Origin:** register audit, `D-103` to `D-104`
**Verdict:** ratified, save the three-band finding of `D-104`, ruled by `R-037`
**docs/:** unchanged

Point 10, the pin owed by `R-034` and `R-036`. Each entry was read against `AC-02`,
`NFR-01`, §18 and the two rulings, and its claims against `b43d57d` and `2ec9fad`.
`AC02_TheGateSequencesArePinned` and the other `AC02_` tests were run in Release
and in Debug: all green in both, so two builds of this commit reach the pinned
values.

- **`D-103`, the point added.** Stands. The user's approval is quoted, and
  `b43d57d` only appends to `PLAN.md`: nothing above point 10 changes, and the
  exit condition is left verbatim, as `D-014` requires. Putting both pins in the
  suite is within the shape `R-034` left to the implementer. The empty run is
  `Simulation.Advance` called as the runner calls it, so the suite's empty
  sequence is the gate's. The named faults are the right ones: a drain moved to
  the end of the tick is invisible to `AC02_DeterminismAcrossRuns`, because two
  runs of the same wrong code agree, and it is the fault the pin exists for.
- **`D-104`, the test.** Stands. Checked: two literals, the SHA-256 of each
  sequence written little-endian, and the same `Empty` and `Commanded` the
  determinism test compares. Taking the digest from the base class library
  rather than `Hash64` extends `R-034`'s rule, a value never computed by the code
  that checks it, to the function that condenses it; it is right for the same
  reason. The values were taken from a failing run with a placeholder, which
  is what "produced by a run" asks. `Core: no` holds: `2ec9fad` touches the
  register, `PLAN.md` and `core/Tests/DeterminismTests.cs` only.

`R-034`'s action is marked done. Its "every CI run compares" half is the reserve
of point 10: `2ec9fad` has not been pushed.

**The gate, as the files now stand.** CI run `36786627133`, on `b43d57d`, ran
every step green, including "Harness, one hash per tick" and "Gate, 100k empty
ticks in two runs". Those are the clauses held in reserve by points 8 and 9, so
the human's action has happened and the *Riserva sciolta* lines of `R-011` are
owed by the implementer. What stands between phase 3 and its gate is the push
of `2ec9fad` and a green run on it, then point 10's line. After that comes the
moment-two review request, which `AGENTS.md` requires at the gate and which the
decider answers as a phase gate.

**Cost.** None beyond `R-037`.
**Owed by the implementer:** nothing new. The *Riserva sciolta* lines are
`R-011`'s procedure, not a new action.
**Would overturn it:** as for `R-008`, any entry found to contradict `docs/` as it
stood when written; for the pin, the CI runner reaching a different digest,
which would be a determinism defect under `NFR-01` and no longer a matter of the
register.
**Not verified:** the full suite was not run, only the `AC02_` tests; the mutation
results in `D-104` were taken as written; that the literals came from a run
rather than from the checking code is taken from the entry, and the test's shape
only rules out the checking code computing them.

### R-039 — §17's harness is the headless driver, not the `sim` executable: the suite counts

**Date:** 2026-10-01   **Origin:** SQ-007
**Verdict:** specified
**docs/:** `SIM-REQ` §17, the opening sentence names the two drivers and what
"alone" excludes; `SIM-DEC` DEC-035, rationale. Both revisions bumped.

`SQ-007` reads §17's "driven by the command-line harness alone" two ways: the
harness as any headless driver, the suite included, or the `sim` executable and
nothing else. The entry cites AC-19 and DEC-034 for the first. Three more places
bear on it, and they settle it.

**The vocabulary.** §1 sets the dichotomy the sentence works in: the core is
"driven either by a game client or by a command-line harness". DEC-005 calls the
second "the test harness". In the specification's own terms the suite is a
headless driver that is not the game client, and "alone" excludes the other half
of §1's pair, as DEC-034's rationale says outright: the harness, "the CI suite"
and the sweeps run "with no editor and no game client".

**The narrow reading contradicts §17's own criteria.** AC-19 is met when the
core's "tests pass from the standalone project"; AC-28 when "the full suite
passes under the three-band CI configuration". Under the narrow reading neither
can be met by any code, since a criterion about the suite cannot be driven by
something that is not the suite. AC-56's "runs at load, not in a test" makes the
same point from the other side: the author treated a test as a criterion's
default vehicle. A reading under which correct code cannot pass two criteria is
not the stricter standard; it is a defective one.

**The narrow reading ties §18 in a knot.** The commanded half of AC-02 would need
a command a host can submit through `sim`. Phase 3 defines none: it holds no
domain (`AGENTS.md`, Scope), and `DEC-086` and `R-030` leave the first kind to
the phase that needs it. §18 says no phase starts before the previous gate is
green. The gate of phase 3 would wait for a kind no later phase could start to
supply, unless AC-02's "commands" were read as the empty sequence, which is
reading AC-02 short of its text, as `R-034` declined to do.

**The rulings already hold the broad reading.** `R-034` left the pin "in a test
or in the workflow step", and `R-036`, the user's choice, put both pins in the
suite so that the Debug step compares a second build. Under the narrow reading
those pins would count for nothing at the gate. `R-036` binds this ruling.

**Not reserve 2.** No criterion and no gate changes content: 100k empty ticks,
AC-02 and AC-03 stay exactly what they were. The sentence is ambiguous, and the
reading taken is the one the specification's own terms, criteria and past
rulings support; the other makes two criteria unsatisfiable and the phase
sequence impossible. `R-034`'s case was different: "across builds and machines"
had one meaning, and the evidence fell short of it.

DEC-035's rationale, "Every acceptance criterion in SIM-REQ runs through it",
said the narrow reading as a fact, and now names both drivers. Nothing else in
`docs/` states it: NFR-12, DEC-039, §20 and FR-W-09 were read and agree with
either reading or with this one. The plan's exit condition, "run by the
command-line harness in CI, as §17 requires of every criterion", defers to §17
and is read through it; `PLAN.md` is not touched.

That the suite reading costs no code is not its reason. The arguments above
held before any code existed.

**Cost.** A criterion driven by the suite can pass on a path no host takes,
since the suite sees the core's internals through `InternalsVisibleTo`. The
instance today is the commanded half of AC-02, through the internal loop with a
test-only kind; `R-036` already moves it onto `Simulation` with the first real
kind. A criterion stated as what a host observes should be checked on the host's
surface when one exists, but this ruling does not make that a rule. `sim` loses
the claim to be every criterion's driver; its reason to exist is the runs, hash
dumps and sweeps of NFR-12.
**Owed by the implementer:** nothing. The gate's evidence stands as it is; the
gate itself is declared, or not, by the moment-two review, which this ruling is
not.
**Would overturn it:** a requirement that acceptance be checked only through the
core's public surface, black-box. That would reword AC-19 and AC-28, and would
put a public command path before the gate of phase 3.
**Not verified:** what the author of §17 meant, which no file records; the
suite was not run in this session, and the green CI run `36788322902` on
`c645462` is taken from the review request.

### R-040 — Commands apply at the start of the tick, in submission order: DEC-087

**Date:** 2026-10-02   **Origin:** C-5, from D-077
**Verdict:** promoted as DEC-087
**docs/:** `SIM-DEC` gains DEC-087 under "Integration and operations", after
DEC-086, and DEC-030 points at it; `SIM-REQ` FR-A-01 names the point and the
order. Both revisions bumped.

FR-A-01 and DEC-030 both say "a defined point in the tick" and neither defines
it. `D-077` chose the start, and `R-023` ratified the choice as a reading of
point 5. It is more than that: every system phase 4 adds runs after the drain or
before it, and the replay log of DEC-030 means nothing until it is known on
which tick a command took effect. A constraint on all later code that a reader
of `docs/` could not infer is what a `SIM-DEC` entry is for.

The start is right on the merits, against the only other candidate, the end. Both
make the tick a command lands on a function of submission against tick
boundaries, which is what NFR-01 needs. They differ in who sees the command
first. Applied at the end of tick *t*, it changes the state hashed at *t* while no
level of *t* saw it, and the world responds at *t + 1*; a player's action would
be dated one day and answered the next. Applied at the start, the action and the
response share a tick, and `SIM-ECON`'s Arrivals, where reported outcomes land,
come after it. Submission order follows from NFR-01's "identical command
sequence": a sequence has an order, and the host's is the only one available
that no state decides.

The candidate's text is kept, with one sentence added to the cost. Under DEC-031
the core runs on its own thread, so the boundary a command meets depends on
when the host hands it over, a wall-clock fact. The run is still deterministic
given the ticks the commands landed on, and that is what a replay must record.
This was true before any code; nothing in phase 3 has a second thread.

`SIM-ECON`'s open item 4 and `D-073`'s order of levels are untouched: DEC-087
places the drain before every level, not the levels among themselves.

**Cost.** None to code: `TickLoop.Advance` takes the count waiting and drains it
before the first level. Every phase 4 system is written against a world where
the tick's commands have already applied.
**Owed by the implementer:** nothing.
**Would overturn it:** a command kind that must apply mid-tick, between levels,
which would need a second drain point and a reason `docs/` does not yet hold.
**Not verified:** that the command log of DEC-030, not yet built, will record the
tick of application; the entry now says it must.

### R-041 — A table's capacity is not state; only the rows in use are: DEC-088

**Date:** 2026-10-02   **Origin:** C-6, from D-081 and D-083
**Verdict:** promoted as DEC-088
**docs/:** `SIM-DEC` gains DEC-088 under "Core representation", after DEC-085;
`SIM-STATE` §Serialisation notes gains one line pointing at it. `SIM-DEC`'s
revision already bumped today by `R-040`; `SIM-STATE`'s bumped.

DEC-085 says how a variable-length field is laid out and `SIM-STATE` says the hash
covers every field. Neither says what of an array is state. `SIM-STATE` §Rule
implies part of it: capacity is not listed and is not derivable from what is, so
it may not influence a future tick. It does not imply the rest, that the hash and
the save stop at the count, and that is where the fault lives. Point 6 met it
first, with the chronicle; every growable table after it — pools, agents,
transients — meets it again, and `D-083`'s finding is that the wrong walk passes
every test on a world whose arrays happen to be full. A constraint every later
table must keep and nobody could infer from `docs/` is architecture.

On the merits the rule holds without the code. The capacity a doubling table
reaches records how it grew, not what it holds, so hashing it makes two equal
worlds unequal, which breaks NFR-01 between a world built in one run and the same
world built in another order. Saving it breaks AC-03 directly: a load allocates
for what the bytes say, and a world loaded into columns of another length saves
to other bytes. Both arguments stand on NFR-01, AC-03 and DEC-085 as written
before point 6.

The candidate's text is kept, with two changes. The pool's count is named
explicitly, since DEC-085's pool is the one place where "rows" is not the unit.
And the cost says what the entry does not settle: a row released inside the
count, by retention (`SIM-STATE` open item 3) or by DEC-085's compaction, is
still inside what is walked, and whether it is reset, compacted away or made
unreachable is those items' decision.

**Cost.** None to code: `StateHash` and `SaveFormat` walk `ChronicleCount` rows
and the pool to `EntitiesInUse`, and `FRI01_CapacityDoesNotReachTheHash` checks
the hash half (`R-027`). Each later growable table carries its count in state and
a check that its capacity reaches neither hash nor save; the chronicle lacks the
save half, owed below.
**Owed by the implementer:** the save half's check. No test today shows that two
worlds holding the same chronicle entries in columns of different capacity save
to the same bytes: `FRI01_CapacityDoesNotReachTheHash` covers the hash only, and
AC-03's round trip can pass a writer that saves capacity if the reader allocates
what it reads. No later than the next plan point that changes the save format,
a test of that equality, with its `Fails when:` naming a writer that walks to the
array's length. Cite `R-041`.
**Would overturn it:** a table whose capacity is itself a quantity of the world
— a cap a system reads and changes — which would make it a field, and state, not
capacity.
**Not verified:** that `SaveFormat`'s writer stops at the count; `D-090` and
`R-029` say it does, and the code was grepped, not read for it. `core/Tests/` was
grepped for a capacity test of the save and none was found.

### R-042 — A load refuses what no build could have written, pending commands included: DEC-089

**Date:** 2026-10-02   **Origin:** C-7, from D-091; D-106's third question
**Verdict:** promoted as DEC-089
**docs/:** `SIM-DEC` gains DEC-089 under "Integration and operations", after
DEC-033; `SIM-REQ` NFR-08 points at it. Both revisions already bumped today by
`R-040`. `PROMOTIONS.md`'s pass of 2026-10-01 is closed.

NFR-08 and DEC-032 say what a save holds; nothing in `docs/` says what a load does
with bytes that do not hold it. `D-091` answered for point 7 and `R-030` ratified
it. Every reader phase 4 adds meets the question again, with more fields and
more rules, so it is promoted. On the merits it stands on what `docs/` said
before the code: a save is the one path by which state enters the core from
outside (`R-004`), DEC-032 exists so that a reported bug reproduces, and a world
that loads and then fails ticks later reproduces nothing. NFR-09's "produced it"
excludes running a rule version the build does not have.

**Two additions to the candidate.** The first is the invariants. "The rules its
writer keeps" read narrowly is each writer's local check, as `Chronicle.Append`'s
cause rule; but a world violating A-01 is equally one no build could have written,
and the title says such a world is refused. The decision names the invariants of
`SIM-STATE` that are claims about state, which excludes A-13 (`R-015`).

The second is the pending commands. `D-091` left their kinds unchecked, and
`R-030` bound the choice — the load checks kinds, or the drain refuses without
consuming — to the first command kind. `D-106` asked the same as its third
question. The candidate's cost line, "checked only as far as the build knows
their kinds", would have left a contradiction inside the entry: a save holding a
kind no build knows is the clearest case of what the title refuses. A command's
form needs no state, so it is checked where the save is; whether it fits the
state of its tick stays the drain's, as DEC-086 requires. This decides the
direction of one of `D-106`'s questions. It does not move its timing: the user
left `D-106` to phase 4 as a finding, and the action falls due at the point
`D-106` and `R-030` named. Its first two questions — what a refused command does
to the tick, and whether the host hears of it — are not touched.

**Cost.** Each writer's rule is stated twice, and the invariants checked at load
run in Release. In phase 3 no kind is known, so applied now the check would refuse
every save with a command pending, the tests' stand-in kind included; the tests
that round-trip pending commands (`NFR08_ASaveCarriesThePendingCommandsInOrder`,
`AC03_SaveRoundTrip`'s "with commands pending") need the load to know the kinds
the drain is given, and the second saves a kind 7 that no drain knows. That is why the action waits for the first real kind.
**Owed by the implementer:** no later than the plan point that adds the first
command kind: a load refuses a pending command whose kind this build does not
know or whose fields fall outside what its kind declares, and checks the
invariants of `SIM-STATE` that are claims about state. Cite `R-042`.
**Would overturn it:** a ruling that a load may accept a world and let the first
tick find what is wrong, which would need a reason a reported bug still
reproduces; a host that must load saves from a later build, which is NFR-09's to
change first.
**Not verified:** that every phase 4 invariant can be checked on a loaded state
in a time a load can afford; `SaveFormat`'s other checks were taken from its
remarks and from `D-091`, not read line by line.

### R-043 — The state hash is checked in full at every tick, and only when a check asks for it

**Date:** 2026-10-02   **Origin:** SQ-008
**Verdict:** specified
**docs/:** `SIM-REQ` NFR-01 gains one sentence: the hash is computed in full on
request, never by the tick, and NFR-05's budget excludes it. Revision already
bumped today by `R-040`.

`SQ-008` estimates that a full hash at NFR-07's bound costs about one P-01 budget,
and that the checks of `AC-02`, seven sequences of 100k ticks per test step,
would then run for hours. It offers three answers: the full sequence always, an
incremental hash, or a sampled sequence.

**What `docs/` already says.** NFR-01 asks for an identical hash "at every tick",
`AC-02` for "the same state hash sequence", and `SIM-STATE` for a hash over every
field. None of them asks the game to hash. The finder reads NFR-01 as stating a
property, not a computation the core must run, and that reading is right: the
game has no use for a hash, and DEC-034 keeps the core indifferent to which host
drives it. It is not written anywhere, though, and an implementer reading "at
every tick" beside NFR-05 could put the hash inside the tick and spend the
budget on it. The added sentence closes that.

**The sample is refused**, on the merits and by reserve. A divergence can heal:
a transient value differs for a tick and is overwritten. A sampled check passes
it, and NFR-01 says every tick. Reading "sequence" as every *k*-th term
loosens `AC-02`, which is reserve 2 of `R-001`, as `R-034` found for "machines".

**The incremental hash is refused.** It makes every write to state also a write
to the hash, so a system that forgets one leaves a field outside the hash: the
determinism hole `SIM-STATE` §Serialisation notes names, created by the
mechanism meant to detect it. Catching that needs the full hash computed beside
it, which is the cost it was meant to avoid. `AGENTS.md`'s "systems mutate
state" would also gain a second thing every system must keep in step.

**So the full hash, every tick, at check time.** The estimate overstates the
problem in one direction. A hash folds each word once; a tick at target scale
does the settlement update on a seventh of the settlements, sorting and
clearing markets, within the same 10 ms. The hash is therefore of the order of
the tick, not orders of magnitude above it, and a check that runs a world for
100k ticks pays for the simulation about as much as for the hashes. What makes
a check affordable is the size of the world and the length of the run, and
`AC-02` fixes neither. Only phase 3's gate fixed a length, "100k empty ticks",
and that gate is passed. Sizing each check is the implementer's, in the point
that writes it, as `R-034` left the pins' shape. A check on a smaller world
proves NFR-01 on that world, which is all any finite check proves.

**Cost.** The checks of `AC-02` grow with every table and will need resizing in
phase 4, and the pins of `R-034` and `R-036` re-pinned when they are. A world
small enough for CI may not reach a code path a large one does. The estimate in
`SQ-008` stays unmeasured until the measurement owed below.
**Owed by the implementer:** within the first phase 4 plan point that adds a
table: measure the cost of one full state hash against the cost of the tick, on
the world the checks of `AC-02` run, and record it in the register with the size
chosen for those checks. Cite `R-043`.
**Would overturn it:** that measurement showing the full hash many times the
cost of the tick at a world size the checks need, which would reopen the
incremental hash with a full hash cross-checking it on a cadence.
**Not verified:** the cost of a fold and of a settlement update, both estimated;
nothing exists in phase 3 to measure either against.

### R-044 — The register from `D-105` to `D-107` is ratified; phase 3 is closed

**Date:** 2026-10-02   **Origin:** register audit, `D-105` to `D-107`
**Verdict:** ratified
**docs/:** unchanged

The close of phase 3: the moment-two request, the look at the whole, the archive
and the merge. Each entry was read against `SIM-REQ` §17 and §18 and the rulings
it cites, and its claims against `3933e1d`, `c943618` and the archive.

- **`D-105`, the moment-two request.** Stands. Filing `SQ-007` as blocking rather
  than declaring the gate on the suite's reading is what `AGENTS.md` asks: an
  objection is never resolved by its finder. `R-039` resolved it. Checked: the
  *Riserva sciolta* lines of points 8 and 9 cite run `36786627133` on `b43d57d`
  and run `36788322902` on `c645462`, point 10's only the second, as the entry
  says.
- **`D-106`, a refused command.** Stands as a finding: the description of
  `TickLoop` and `CommandDrain` agrees with `R-030`'s reading of the same code,
  and DEC-086 places the check in the drain. Its third question was answered by
  `R-042`; the first two, what a refusal does to the tick and whether the host
  hears of it, remain owed to the point that adds the first command kind, as the
  user left them.
- **`D-107`, the close.** Stands. Checked: `3933e1d` renames `PLAN.md` to
  `.claude/plans/2026-09-20-fase-3-kernel.md`, which `git log --diff-filter=A`
  finds as the one file added under `.claude/plans/`, so `/plan-status`'s lookup
  holds; `c943618` is a merge with two parents; no `PLAN.md` is live on `main`.
  Process throughout, so ratified, never promoted (`D-011`).

**The gate.** The entry's "would overturn it" asks whether the gate's record
belongs in the archive or here. Both: the archive holds the user's words under
the last point, and this file, the decider's only memory, records that **the
gate of phase 3 in `SIM-REQ` §18 — "100k empty ticks; AC-02, AC-03 green" — was
confirmed by the user on 2026-10-01**, in the words "Confermo il gate di fase 3,
procedi con il merge in main", after `R-039` and with green CI run `36788322902`.
The decider did not pass it and does not re-pass it; the confirmation was the
user's.

**Cost.** None.
**Owed by the implementer:** nothing new. Phase 4 opens with `R-037`, `R-041`,
`R-042` and `R-043` owed, and with moment one of `REVIEW-REQUEST.md` before its
plan starts.
**Would overturn it:** as for `R-008`, any entry found to contradict `docs/` as it
stood when written.
**Not verified:** the merge commit has not run in CI; `main` is not pushed, which
is item 3 of the current review request and the user's.

### R-045 — `D-089` widened: the markers of an outcome line are English too

**Date:** 2026-10-02   **Origin:** user, on D-089
**Verdict:** resolved
**docs/:** unchanged

`D-089` writes every artefact in English from the point that writes the rule,
outcome lines included, and says nothing of the markers inside them. Five are
Italian, and they are not prose but keys the skills match: `*Esito:*`, which
closes a point; `FALLITO`, closed as failed, the plan stops (`D-023`);
*Chiuso con riserva* and *Riserva sciolta* (`R-011`); *criterio difettoso*
(`R-009`). They stand in `AGENTS.md`, under "Criteria are frozen", and in
`/plan-next`, `/plan-explain` and `/plan-status`. Left as they are, an English
outcome line would carry Italian keys, an exception to `D-089` that nobody wrote
down. Asked whether the markers stay or change, the user answered: "D-089: anche
i marker in inglese". Governance, reserve 1 of `R-001`: the user's decision,
recorded here.

**The markers.** The English forms are the words the rulings already use for the
same things in prose — "criterion defective" (`R-005`), "a reserve" (`R-011` and
after):

| Until now | From the rule |
|---|---|
| `*Esito:*` | `*Outcome:*` |
| `FALLITO` | `FAILED` |
| *Chiuso con riserva* | *Closed with reserve* |
| *Riserva sciolta* | *Reserve discharged* |
| *criterio difettoso* | *criterion defective* |

**When.** With the rest of `D-089`: the point of the process plan that writes the
language rule, not now. Points before it close under the forms in use, as
`D-089` leaves points already written as they were.

**Both forms are read.** `D-089` does not translate records already written. The
five plans archived in `.claude/plans/` close their points with `*Esito:*`, and
`/plan-status` reads an archived plan when none is live; the process plan itself
will hold points closed before the rule and after it. So the three skills write
the English form and recognise both, for as long as any plan carries the Italian
one — which, for the archives, is always.

**Cost.** Three skills and `AGENTS.md` change in one point, and every search for
a closed point needs two patterns from then on.
**Owed by the implementer:** within the process plan point that writes
`D-089`'s language rule, the five markers become English in `AGENTS.md` and in
`/plan-next`, `/plan-explain` and `/plan-status`, which write the English form
and recognise both. The point names `AGENTS.md` and the rule it changes, as
`R-010` requires. Cite `R-045`.
**Would overturn it:** the user preferring the markers kept as fixed Italian keys
inside English prose.
**Not verified:** which point of the process plan writes the rule, since the plan
is not written; that nothing outside the files named matches the markers was
checked by a grep of the repository, which found them only there, in the two
design notes, in the archived plans and in the two registers.

### R-046 — The agent workflow is built before phase 4; its first real run is phase 4's first point

**Date:** 2026-10-02   **Origin:** review request 2026-10-01, item 4; D-086
**Verdict:** resolved
**docs/:** unchanged

Item 4 carried from moment two the question whether the workflow of `D-086` to
`D-089` is built before phase 4. `R-028` ratified the design and left the
question to the user, as governance under reserve 1 of `R-001`. Put to the user
with three alternatives, the user chose to build it first, with the design's
point 6 moved out of the process plan; asked how the move meets `R-011`, the user
chose to carry it in the criterion of phase 4's first point.

**Why first.** Of `D-086`'s three reasons to wait, two have lapsed: `main` now
holds `/decide`, `RULINGS.md` and the decider's sections of `AGENTS.md`, and the
serialiser of point 7 is behind us. The third, that two live plans break the
lookup of `/plan-status`, still holds and sets the order: one plan, then the
other. And the phase 4 plan cannot be written yet in any case: its scope, the
gate's `AC-04` and the order of its subsystems are open in `docs/`, and those
questions go to the decider first. The process plan uses that time.

**Why point 6 moves.** The design's point 6 is "the first real point run by the
agents … on a real change". A real change is a simulator change, and inside a
`Kind: process` plan it would carry no `Serves` or `Satisfies` and leave its
`Plan-point:` trailer in the process plan's range. Moved to the phase 4 plan,
each plan stays of one kind.

**Why not a reserve.** `R-011` says a reserve "is not for work the agent could
do", and phase 4's first point is the agent's work. So the process plan closes on
its points 1 to 5, its exit condition being the workflow built and wired, and the
proof that it runs belongs to the next plan instead: the first point of the phase
4 plan carries the design's point 6 check as part of its own criterion. Its
outcome line shows the tester's test red then green, the named fault seen red,
and every agent the workflow dispatches as having run; its `Fails when:` names an
agent skipped with no word of it in the outcome.

**Cost.** Phase 4 starts one plan later. Until its first point closes, the
workflow is built and unproven, and the process plan reads as finished while
that is so; the line under "Owed by the implementer" is what keeps it visible.
**Owed by the implementer:** the process plan holds the design's points 1 to 5
and no simulator change. The first point of the phase 4 plan is run by the
agents, and its criterion includes the design's point 6 check and fault as above.
Cite `R-046`.
**Would overturn it:** the process plan stalling long enough that phase 4 is
wanted without it, which reopens the order; a failure of the first real run,
which is a `FAILED` point of phase 4 and a finding about the workflow, not a
reason to reopen the process plan's outcome lines.
**Not verified:** the design's own "Not verified" list — hooks under a skill,
`pwsh` on Windows, `effort` per agent, the cost of a point — none of which this
ruling could test.

### R-047 — A `Fails when:` is read by the user at moment one and by the reviewers at close

**Date:** 2026-10-02   **Origin:** review request 2026-10-01, item 4
**Verdict:** resolved
**docs/:** unchanged

The moment-two request of phase 3 recorded that no one but the author ran the
mutations, and item 4 carried the question: is a `Fails when:` read by anyone
other than the one who wrote it? Today no one is. The reviewers' brief
(`.claude/skills/plan-next/reviewers.md`) passes the point's statement, what it
cites and its check, and does not name the `Fails when:` line; the reviewers run
only for `Core: yes`, after the code; `/plan-explain` objects to a weak fault but
runs in the author's session; and the decider has taken mutation results as
written in every audit.

`D-088`'s tester does not close the gap: it runs the faults the plan names, so it
makes the *running* independent, not the *choosing*. A fault the author did not
think of is missed by the author and by the tester alike.

Put to the user with four readers, combinable, the user chose two:

1. **The user, at moment one.** The request carries the plan's criteria, its
   `Fails when:` lines included, and the user reads them before they are
   frozen (`R-048`). This is the one reading by someone who is not the model that
   wrote them; the decider reading at moment one was offered and not chosen, for
   that reason.
2. **The reviewers, at close.** Their brief quotes the `Fails when:` line with the
   rest of the frozen criterion, and asks them, besides whether the check was met,
   which fault the check would miss. A fault proposed is a finding: it goes to the
   register with the others, accepted or rejected with its reason, and the
   criterion does not change, as `R-005` and `R-009` require. Whether the
   implementer also introduces it is the implementer's call, recorded in the same
   entry. Once `D-088` is built, the code-reviewer on every code point is a
   reviewer for this purpose.

**Cost.** Moment one asks more of the user: the criteria are read, not skimmed.
The reviewers' findings grow by one kind, and the register with them.
**Owed by the implementer:** within the process plan point that rewires
`/plan-next` and `reviewers.md`, the brief quotes the `Fails when:` line and asks
for missing faults, and the code-reviewer's definition does the same. Every
review request at moment one carries the plan's `Fails when:` lines verbatim.
Cite `R-047`.
**Would overturn it:** proposed faults that turn out, at audit, to be noise at a
rate that costs more than they find; or the user finding the moment-one reading
adds nothing the reviewers do not.
**Not verified:** that a reviewer briefed to look for missing faults finds the
ones that matter; nothing has run it.

### R-048 — A plan reaches moment one as a draft, and is frozen by the commit that installs it on its branch

**Date:** 2026-10-02   **Origin:** review request 2026-10-01, item 4; D-051
**Verdict:** resolved
**docs/:** unchanged

`AGENTS.md` freezes criteria "when written", and moment one exists because "a
wrong criterion costs one conversation rather than every point built on it".
Phase 3 did both at once: `D-051` installed the plan on `main` beside the
request, and `D-053` then left its points untouched when the answers came. Read
together, moment one could add a point (`D-014`) or stop the plan, but not
correct a criterion, and the reading of `Fails when:` lines that `R-047` gives
the user would have nowhere to land.

Put to the user with three forms, the user chose this one:

- **The plan comes to moment one as a draft, inside the request.** Not
  `PLAN.md`: the request holds the draft whole, criteria and `Fails when:` lines
  verbatim, and the user's answer may change any of it.
- **After the answer, one commit installs `PLAN.md` on the plan's branch**, cut
  from `main` for it. That commit is the moment of writing in `AGENTS.md`'s
  sense: from it the criteria are frozen and every later rule about them
  applies. It cites the answer it follows.
- **`main` never carries a live `PLAN.md`**, as the plan design note has said
  since 2026-09-17 and `D-051` departed from. `D-051`'s reason, that a request
  nobody can see does not interrupt, is met by the request on `main`; its other
  reason, that a branch makes answering cost a merge, falls away because nothing
  on the branch exists until the answer.

This reverses `D-051`'s placement for later plans; `R-008` ratified it as right
for its plan, and that stands.

**Cost.** A plan is written twice, as draft and as file; the installing commit
must match the draft as amended by the answer, and a difference between them is
a change no one reviewed. `/plan-explain` reads `PLAN.md` only, so it cannot be
run on a draft.
**Owed by the implementer:** from the next plan, the process plan included: the
draft goes in the moment-one request, and `PLAN.md` is installed on its branch
after the user's answer, in a commit that cites it and says whether it matches
the draft as amended. The process plan point that edits `AGENTS.md` states this
in "Criteria are frozen when written", naming the rule as `R-010` requires. Cite
`R-048`.
**Would overturn it:** drafts drifting from their installed plans in ways the
installing commit does not report; or the user preferring to review the file
itself, which reopens `D-051`.
**Not verified:** how large a request becomes with a whole plan inside it; the
phase 3 plan, at ten points, was not measured for it.

### R-049 — A plan's first point waits for the user's answer at moment one

**Date:** 2026-10-02   **Origin:** review request 2026-10-01, item 4
**Verdict:** resolved
**docs/:** unchanged

`/plan-next` step 1 says a moment-one request is owed and forbids going on only
"if the human has not answered a blocking question": a request with no blocking
question does not stop the first point. And `R-001` lets the decider answer
`REVIEW-REQUEST.md`, so the answer to moment one could come from the same model
that wrote the plan.

Put to the user with three answers — the user's always, the decider's except for
permissions, or the rule as it stands — the user chose the first. It follows from
`R-047` and `R-048`: the user reads the `Fails when:` lines at moment one, and
the plan is installed only after the answer, so a first point that starts
without it starts on a plan nobody installed.

**The rule.** No point of a plan starts before the user has answered its
moment-one request. The decider still answers what the request asks about
`docs/` — a `SPEC-QUESTIONS.md` entry, a promotion — under `R-001`; the answer
that lets the plan start is the user's alone. This narrows `R-001`'s coverage of
`REVIEW-REQUEST.md` for moment one, at the user's choice; moments two and three
are unchanged.

**Cost.** Every plan waits on the user once before it starts, even one with no
open question.
**Owed by the implementer:** within the process plan point that rewires
`/plan-next`: step 1 refuses a plan's first point until the user's answer to its
moment-one request is recorded, whether or not the request holds a blocking
question. Until that point lands, the rule binds as this ruling. Cite `R-049`.
**Would overturn it:** plans held up for an answer that, once given, never
changes anything, which would argue for the decider's answer except where the
user is asked to read.
**Not verified:** nothing; the step of `/plan-next` and `R-001` were read.

### R-050 — A value used before its sweep is Provisional, numbered in §16; static content is the user's

**Date:** 2026-10-02   **Origin:** user, design session
**Verdict:** specified
**docs/:** `SIM-REQ` §16, "Parameters to fix", gains a paragraph above the table
defining the status Provisional and who sets static content. Revision already
today's.

§16 holds 33 values `TBD`, all but a handful `TBD by sweep`, and sweeps are
Phase 6 (§18). Phase 4 must run its subsystems before then, so it runs on values
nobody has fixed. Many more have no P-number at all: the shape of `f` in
FR-M-01, `desired_stock`, the per-capita needs of `SIM-ECON` ("The tier table
has a shape and no numbers"), the share of tax a settlement retains. Left as they
are, each would be chosen by the implementer inside a point and appear only in
the register, which makes it design hidden in the diff, and makes `AC-04`'s
outcome depend on numbers no document names.

Put to the user with three answers, the user chose this one. **A value used
before it is fixed is Provisional**: the implementer proposes it in the point
that first needs it, the decider writes it into §16 with a P-number and the
phase that replaces it, and the point cites the P-number. The decider's part is
`R-001`'s numbering of identifiers. A constant driving the simulation with no
P-number is a defect, found by review like any other.

**Static content is different in kind.** Goods, occupations, recipes, crops and
building types decide which world is simulated, not how fast a variable moves.
They are approved by the user before the plan that uses them is written, and are
not proposed point by point. P-09 fixes seven goods and the list itself is still
"proposed, pending confirmation" in §6; that confirmation is the first of these
approvals.

**Cost.** A point needing a new value waits on a decider session to number it, or
carries the proposal to its outcome and the number arrives at the next audit;
which of the two is the plan's to say. Phase 4's plan cannot be written before
the content is approved.
**Owed by the implementer:** from the phase 4 plan, every simulation constant
cites its P-number; a proposal for a new one is part of the point that needs it.
Cite `R-050`.
**Would overturn it:** provisional values outliving Phase 6 unswept, which would
say the status hides rather than marks.
**Not verified:** how many unnumbered values Phase 4 will need; the list above is
what one reading of `SIM-ECON` and §5 to §8 found.

### R-051 — Phase 4 builds production first and demography fifth, on generated state rather than stubs

**Date:** 2026-10-02   **Origin:** user, design session
**Verdict:** specified
**docs/:** `SIM-REQ` §18, "Delivery phases", the content of Phase 4: the order
changed, and a sentence on what a subsystem runs on before the others exist.
Revision already today's.

§18 called its order "dependency order" and put demography first. Demography is
the subsystem that depends on the most: births on food per capita (FR-D-02),
migration on food, real wage and safety (FR-D-04), mortality on the hunger deficit
that phases 6 and 7 produce (`SIM-ECON` §Demand). Built first, it would run on
food, wage and safety held constant by stubs, and "AC-04 green" for it would
certify a world the game never contains. `R-037` noted the same doubt in its
"Not verified".

No order is free of a cycle: production needs labour, which is population. What
breaks the cycle without inventing anything is that the generator already writes
a population (FR-G-01). A subsystem built before demography runs on the population
as generated, unchanging; that is real state, of the shape the game ships, and
nothing has to be removed later. Read so, "dependency order" is the order in which
each subsystem's inputs become dynamic: production, then needs, which consumes
what production made, then the market that clears what own consumption left, then
labour and income, which price the work production used, then demography, which
reads all four, then mobility, which moves people and goods between settlements
by what demography and the market compute, then events, which act through all of
them (FR-E-06).

Put to the user with the current order on stubs and with a single minimal pass
over the whole update, the user chose this one.

**Cost.** Until demography lands, no run shows a population move, which is what
`SIM-OBS` says the player notices first; the early gates test economies with a
fixed population. The first cohort rows arrive with production, not demography,
so `R-037`'s three-band check binds there.
**Owed by the implementer:** nothing beyond the order itself; `R-037` binds the
first point that builds cohort rows, whichever subsystem it serves.
**Would overturn it:** a subsystem that cannot run on a fixed population without
a stub — production needing births to be meaningful, say — which would argue for
the single minimal pass.
**Not verified:** that market clearing before labour and income runs without a
wage: phase 2 of `SIM-ECON` computes the wage with the prices, so the market
subsystem may need a wage from the generator until labour lands; checked against
the update table, not worked through.

### R-052 — Phase 4 is the whole twelve-phase settlement update and the generator, mapped onto its seven subsystems

**Date:** 2026-10-02   **Origin:** user, design session
**Verdict:** specified
**docs/:** `SIM-REQ` §18, "Delivery phases", gains a table mapping each
subsystem of Phase 4 to its `SIM-ECON` phases and its requirements, and a line on
what lies outside Phase 4. Revision already today's.

§18 named seven subsystems and none of credit, information, denominations,
taxation, guilds or transients. §19, "Open items", closes P-17 and P-01's validity
in Phase 4 by measuring "the full twelve-phase settlement update", which contains
borrowing and repayment (phases 3 and 8), the knowledge table and `changeCapacity`
(phase 12) and taxation among its dated steps. The two could be read as a phase 4
of seven narrow subsystems with a measurement it could not take, or as a phase 4
of the whole update named by its seven largest parts. The generator, FR-G-01,
was in no phase at all, though every subsystem needs settlements to run on.

Put to the user with the narrow reading and with a new phase between 4 and 5, the
user chose the whole update. The table now in §18 places every requirement the
update implements under one subsystem, in the order of `R-051`. Three placements
are this ruling's own and carry its reasons:

- **Migration is mobility, not demography.** `SIM-ECON` runs both in phase 11, but
  births and deaths read a settlement's own state, while migration reads its
  neighbours' and moves people along links, as trade does. Splitting them lets
  demography close on a closed population first.
- **Sampled transients are mobility.** Itinerant merchants and adventurers
  journey as sampled transients (FR-J-08, FR-J-12), and merchant journeys feed the
  candidate list that rewires the graph (FR-M-08). Observing and interacting with
  them, FR-X-04 and FR-X-05, is what the host does, and goes to Phase 7 with §13.
- **The generator opens Phase 4, without prior history.** FR-G-01, FR-G-01a and
  FR-G-02 come before production. FR-G-03 runs "the simulator itself, at full
  fidelity", which needs every subsystem, and P-18, its length, closes in Phase 6;
  it goes there with FR-G-04. This also places `R-007`'s open item, which
  `R-007` assigned to a "phase 14" that §18 does not contain: it falls due in
  Phase 6.

**Cost.** Phase 4 is large, and the plan will be long or split into several plans
in sequence. Placing a requirement under a subsystem is not a claim that it is
easy to build there.
**Owed by the implementer:** a phase 4 plan's `Satisfies:` and each point's
`Serves` are read against this table; a requirement served by a subsystem other
than its own is a decision for the register. Cite `R-052`.
**Would overturn it:** a requirement found to need a later phase's machinery —
an agent, the host — which would move it out of the table by a ruling.
**Not verified:** that every requirement of §5 to §12 appears in the table;
FR-W, FR-T and FR-A belong to phase 3 or 7 and were not listed, and the ranges
were written from the section headings and the first line of each requirement.

### R-053 — Phase 4 checks AC-04 against provisional bounds, per subsystem; Phase 6 sets the final ones

**Date:** 2026-10-02   **Origin:** user, design session
**Verdict:** specified — the user's decision, reserve 2 of `R-001`
**docs/:** `SIM-REQ` §17 `AC-04`, the provisional bounds and M; §16 gains P-65;
§18 says what "per subsystem" means; §19's row for the bounds names what it
replaces. Revision already today's.

Phase 4's gate is "AC-01, AC-04 green per subsystem". `AC-04` fails a run when a
monitored variable "leaves its declared range", and the ranges are "set in Phase
6". Read literally, the gate cannot be passed in Phase 4 by any code, since there
is no range to stay inside; M was undefined; and "per subsystem" said nothing of
variables that do not exist yet — legitimacy before labour and income, infection
before events.

Three answers were put to the user: provisional bounds now; only the unbounded
growth half now; or `AC-04` moved whole to Phase 6. The second and third loosen a
gate, reserve 2 of `R-001`. The first changes what the gate is read against, and
whether that holds `AC-04` to its text is itself a judgement about a gate, so all
three were the user's. The user chose the first.

**The bounds.** Until Phase 6 each hard bound is Provisional under `R-050`, and is
derived from a bound the specification already states rather than chosen: no
stock below zero, prices within P-10, a settlement's population within the import
bound of FR-D-14. A bound derived so can fail, which a bound chosen wide enough
never would; that is what keeps the provisional gate from being vacuous. The
other half of `AC-04`, "none grows without bound", runs beside it, with its
method stated in the point's frozen criterion. Soft bands only warn and start in
Phase 6. M is P-65, proposed by the first point that runs `AC-04` against the cost
of sixty simulated years.

**Per subsystem.** When a subsystem closes, `AC-01` and `AC-04` are green over the
invariants and monitored variables that exist by then, and stay green as later
subsystems land. A variable joins `AC-04` with the subsystem that makes it.

**Cost.** A provisional bound derived from a structural limit is loose: a world
can stay inside it and still be wrong in ways Phase 6 will find. Every subsystem's
close reruns the long runs of every earlier one.
**Owed by the implementer:** each phase 4 subsystem's closing point runs `AC-01`
and `AC-04` over what exists, against provisional bounds numbered under `R-050`,
with a `Fails when:` naming a variable that leaves its derived bound. Cite
`R-053`.
**Would overturn it:** a monitored variable with no structural bound to derive
from, which would need a chosen provisional value and so the second reading for
that variable; Phase 6 finding the envelopes far inside the provisional bounds,
which would say the phase 4 gate proved little.
**Not verified:** that every variable in `AC-04`'s table has a structural bound in
`docs/`. Migration flow, urban share, legitimacy and infection prevalence were
not checked for one; prices, stocks and population were.

### R-054 — Emigrants take their share of what can be carried, and travel as a scheduled transient

**Date:** 2026-10-04   **Origin:** user, the poverty and seed proposal (revision 9), open item O-5
**Verdict:** resolved
**docs/:** unchanged by this commit. The text is deposited in
`.claude/design/2026-10-04-proposal-poverty-seed.md` and enters `docs/` with the
proposal, which the user holds until B12b is written: `SIM-REQ` FR-D-04a, FR-P-10d,
§16 P-72, §18 Mobility; `SIM-STATE` §Transient; `SIM-ECON` phase 11.

Nothing in `docs/` says what a migrant takes with them. Three texts say they
take something. FR-J-18 and DEC-068 make buried stock of "what its people could
not carry", which presumes both that people carry and that carrying has a limit.
FR-P-22 has resettling cohorts carry durables. The proposal's seed reserve
(PD-4) passes the emigrant's seed to this rule. And phase 1 of `SIM-ECON` lands
migrants who left in an earlier update, so between one settlement's phase 11
and another's phase 1 they exist nowhere in `SIM-STATE`. Any currency among
them breaks A-02 unless it is transient contents.

**The rule.** Members leaving a row take its mean share per member: all of its
currency, and of its goods and durables outside the seed reserve as much as P-72
weight per member allows. They take the highest value per unit of weight first,
ties by good id. Buildings never move. What they leave stays with the row.

- **The mean**, because the row cannot say which of its members leave: the
  wealth curve has no memory. The mean is also the one share that leaves the
  per-member currency of those who stay unchanged. Under any other share,
  emigration would make the stayers richer or poorer.
- **A weight limit**, because without one FR-J-18 is empty: the last to leave
  would carry everything movable, and abandonment would make no hoard. The
  limit gives the hoard its content and follows DEC-016: cloth and tools
  travel, grain stays.
- **Currency has no weight here.** A cohort holds no denominations (FR-C-13), and
  weight applies to none of its coin (FR-C-18). Emigrants are a fraction of a
  cohort.
- **The seed reserve stays**, as it does for every use but sowing. It is
  computed on the land the vector allocates, and land does not leave.

**The candidate is rejected.** The proposal's candidate was to convert goods to
currency at the local price, within demand. That is a sale outside phase 7: a
second clearing pass in phase 11, against demand that phase 7 has already met.
It would run the most expensive part of the update twice. It is also the forced
sale that `SIM-ECON` lists as not covered (item 1), and it would reopen choice
two. And it buys nothing. Holdings above a cohort's own needs are already supply
in phase 7 of every update, so what an emigrant would sell has already been
offered.

**On the road** emigrants are a scheduled transient of kind *migrants*. Each
carries a count, the key of the row it left, the hunger deficit of FR-D-03a, its
currency as one integer and its goods. A-02 counts that currency among transient
contents, and A-04 those goods among goods in transit. Which row they join on
arrival is not decided here: Z-14 of the phase 4 note.

**Cost.** A transient kind with fields the others lack. A row that loses most of
its members keeps the heavy goods the leavers could not carry, so in goods it
holds more per head than before. One more parameter. FR-P-22 is left unsettled:
either the resettlers' endowment is what they carry, or P-54 makes durables
from nothing. This ruling does not choose.
**Owed by the implementer:** nothing now. The text binds the mobility subsystem
once it is in `docs/`.
**Would overturn it:** the user wanting emigrants to sell up before leaving, which
would first need a rule for who buys; or a sweep finding P-72 small enough to make
migration irrelevant to goods, in which case the limit is not worth its field.
**Not verified:** that a transient with these extra fields fits the shapes of
DEC-085; that no other text in `docs/` assumes migrants arrive with nothing. Only
§5, §6, §7, `SIM-ECON` and `SIM-STATE` were read for it.

### R-055 — The destitute emigrate first, and on the road they are the displaced population of FR-M-13

**Date:** 2026-10-04   **Origin:** user, the poverty and seed proposal (revision 9), open item O-6
**Verdict:** resolved
**docs/:** unchanged by this commit. The text is deposited in the proposal and
enters `docs/` with it: `SIM-REQ` FR-D-04a, first sentence, and FR-M-13; `SIM-ECON`
phase 11; `SIM-LOOPS` B12b, the paragraph under R12, and R4.

The proposal makes B12b, "the destitute emigrate", the brake that holds R12, and
leaves it two questions: what emigrants carry, which is `R-054`, and whether the
destitute who leave are the "displaced population" of FR-M-13, which is O-6.
B12b needs a third answer that neither question names: **who emigrates**.
Nothing in `docs/` says which rows emigrants come from. Drawn in proportion to
count, the destitute would leave only at their share of the population. B12b
would then be no stronger than emigration in general, and R12 would keep its
pressure.

**Emigrants are drawn from the destitute row first**, and from the other rows
in proportion to their counts for the rest. On the merits: the destitute own no
buildings, durables or seed (DEC-090), so leaving costs them nothing they hold,
and the landless poor were who moved. It also weakens R1. When those who leave
were selling labour nobody fully bought, output falls less than when farmers
go.

**O-6: yes.** FR-M-13 names "famine refugees" first. FR-I-03a says what a
refugee is: "Refugees from a famine are first-hand sources about it." They are
migrants who flee hunger. `SIM-OBS` OBS-2 has them "arriving and saying what
they fled". The destitute row is where members go when they can no longer feed
themselves at the local price (DEC-090), so its emigrants are those refugees.
Emigrants from the other rows leave with their property for a better wage, and
are not displaced. **Each destitute emigrant raises the danger of the link it
travels by P-40**, which is P-40's own unit: "per unit of displaced
population". Danger falls only through contracts and garrisons, not by decay,
so a flow of refugees builds a stock that outlasts them. R4 gains the source it
lacked: poverty, the destitute row, sends displaced people, its emigrants, onto
the links.

**The alternative is rejected.** It would make the destitute row itself, in
place, the displaced population. "Displaced" means moved. That reading would
give danger a source in every poor settlement whether or not anyone travels, a
mechanism FR-M-13 does not contain.

**The coupling is named, not hidden.** B12b brakes R12 by exporting it to R4,
whose brakes B4a and B4b already stand. That is DEC-064's intent: the world
makes its threats from its own failures. **B12b is not unconditional, and the
text says so.** It acts only where a neighbour is more attractive by more than
the hysteresis band (FR-D-09). In a region poor throughout, nobody leaves. R12
then stops at the lower clamp of the wage (DEC-045), which bounds it without
braking it, and at hunger, R5. The proposal's sentence "B12b is the brake that
holds" is replaced by one that says when it holds. This is a statement about a
loop, not a change to the gate of `SIM-LOOPS`. R12 has two named balancing
loops, and the gate asks for one.

"Failed migration" and soldiers disbanded at the end of a war, the other two
sources FR-M-13 names, are not touched.

**Cost.** Refugees take the roads towards more attractive places, usually
towards towns, so a famine can raise danger on the route that would relieve it.
That is R4 working, and P-40 is tuned against P-41 for it. The destitute arrive
somewhere. If they join the destination's destitute row (Z-14 decides), they
carry R12's pressure there, bounded by the inbound cap of FR-D-10.
**Owed by the implementer:** nothing now.
**Would overturn it:** a sweep in which refugee danger cuts relief routes faster
than famine ends, which would argue for danger from refugees on arrival rather
than in transit; or regions where R12 settles at the wage clamp for years, which
would argue for a brake that does not need a destination.
**Not verified:** that attractiveness under FR-D-04 falls with the wage fast
enough for B12b to act before hunger does; its weights over food, wage and
safety are not set.

### R-056 — A good's record says only whether it is durable, and processing creates no food along the chain

**Date:** 2026-10-04   **Origin:** user, the poverty and seed proposal (revision 9), open item O-15
**Verdict:** resolved
**docs/:** unchanged by this commit. The text is deposited in the proposal and
enters `docs/` with it: `SIM-REQ` FR-P-03 and FR-P-03c; `SIM-DEC` DEC-098, the
decision, rationale and cost.

The user decided in PD-14 that FR-P-03's roles describe only behaviour in
production and over time, and that urgency moves to the needs. That closes Z-07
of the phase 4 note. The form of the roles was left to the decider, as was the
rule that processing creates no food. Both are answered here inside PD-14 as the
user decided it. Grain is food of grade 0, flour is an ingredient and not food,
and values are carried per class. That is reserve 5 of `R-001`, and nothing in
it moves.

**The roles.** The proposal's text gives every good one of three roles:
consumable, input, durable. PD-14 itself breaks that. Grain meets the food need,
so it is consumable. Grain→Flour takes it, so it is an input. It is also a
planting good. Of the three roles only one is a property the state needs to
know from the record:

- durables are held apart from goods (`SIM-STATE` §Cohort, `durables`);
- they wear (FR-P-23);
- they are seized after goods (FR-C-27);
- the destitute own none (DEC-090).

Consumable and input follow from the data that names a good. A need that
accepts it makes it consumable, and a recipe that takes it makes it an input.
Storing them in the record would be a second statement of the same fact, and
the two could disagree. **So the record says whether a good is durable. A good
that is not takes the consumable role where a need accepts it, the input role
where a recipe takes it, or both, as grain does.** AC-09's table, which the
user decided, reads correctly under this form. "A consumable takes the row of
the most urgent need it can meet" puts grain under tier 0. A good no need
accepts takes the input row.

**The conservation rule.** Recipe by recipe the rule fails, as the proposal
notes. Flour belongs to no food class, so Grain→Flour consumes food and yields
none, and Flour→Bread yields food from none. The proposal's other way out, flour
carrying a food value that no need accepts, is closed by PD-14 itself. A need on
the food class accepts every subclass at or above its grade, and survival
accepts grade 0. Flour with a food value would therefore be eaten, and the user
decided it is not food. **So the rule runs along the chain.** Every good carries
an embodied food value:

- a good of the food class carries its value there;
- another good that recipes produce carries the least food value any recipe
  producing it consumes per unit, inputs counted at their own embodied value;
- anything else carries zero.

No recipe yields more food value than its inputs embody. Validation at load
refuses a table that breaks this, or that passes food value around a cycle.

- **The least**, because the guarantee has to hold on the path that consumes
  least. If flour can also be made from something that embodies no food, then
  bread from that flour would create food. The rule refuses it, and should.
- **The food class only**, the one the tier 0 need names (FR-P-03b), not every
  class. A rule per class would refuse brewing a drink from grain, which
  converts between classes by design. What protects the famine is that no oven
  makes food, and no stock of any other class carries that guarantee.

**Cost.** A validation pass over the recipe graph at load, and an author can
trip it by setting a food value one unit too high, three recipes away from the
error. A recipe cycle that food value passes through refuses the load, even
where the content would have been harmless.
**Owed by the implementer:** nothing now. The rule binds the production subsystem
and the validation at load, once in `docs/`.
**Would overturn it:** content in which processing truly adds nutrition and the
user wants it, which is the user's to decide as an exception; or a recipe cycle
the content needs.
**Not verified:** PD-14 does not say in whose units a good of a subclass carries
its value: the subclass's, or its parent's. "Its value in the food class" reads
both the same only if a subclass measures in its parent's units. The user should
settle that in FR-P-03a, as a finding on a decision of their own. Nor was it
checked that no recipe proposed in K-03 forms a cycle.

### R-057 — Needs add: each is a quantity of its own, and a unit consumed meets one need

**Date:** 2026-10-04   **Origin:** user, the poverty and seed proposal (revision 9), PD-15 (open item O-13)
**Verdict:** resolved
**docs/:** unchanged by this commit. The text is deposited in the proposal and
enters `docs/` with it: `SIM-REQ` FR-P-03b, one sentence; `SIM-DEC` DEC-099, its
first sentence and the first half of its rationale.

PD-15 proposes that two needs on the same class add. Survival asks for a
quantity of food of any grade, and tier 1 asks for a further quantity of grade
at least 1. The alternative is one ration whose grade the tier 1 need raises.

**PD-15 is adopted, for a reason other than the one it gives.** The proposal
argues that under one ration, whoever loses purchasing power "goes straight
from eating well to dying". That does not follow. Tier 1 is deferrable, so a
cohort priced out of bread eats grain, meets its survival need, and only its
discontent rises, under either option. **The two options differ in quantity,
not in grade.** With one ration, an ordinary year's consumption equals the
survival minimum, so every unit short is hunger. With needs that add, an
ordinary year eats the minimum plus the tier 1 share. A shortfall then takes
first the share, which is deferrable, and only after it the minimum. That
margin is what `SIM-ECON` asks for when it makes a famine "a season rather than
a tick". It also gives DEC-044's severity a scale before mortality:
discontent, which feeds fragility (FR-E-03), and then hunger.

**Stated generally.** Each need is a quantity of its own, and a unit consumed
meets one need only. That covers two needs on one class and needs on a class
and its subclass alike. Otherwise one loaf could count once for survival and
again for the tier 1 share.

**Cost.** An ordinary year's demand for food is the minimum plus the tier 1
share. Production, the generator's initial stocks (K-09) and the import bound of
FR-D-14 are all sized on that larger figure, and the needs table's quantities
are tuned so that the total is plausible. In every lean year discontent rises
before anyone is hungry, so fragility moves more often.
**Owed by the implementer:** nothing now.
**Would overturn it:** a sweep in which the margin makes famine too rare to see,
which would argue for a smaller tier 1 share rather than for one ration.
**Not verified:** how large the tier 1 share must be to give the margin
`SIM-ECON` wants; that is the needs table's content, which has no numbers yet.

### R-058 — The plate follows the purse: needs are bought at their cheapest, and a better grade comes after every need

**Date:** 2026-10-04   **Origin:** user, the poverty and seed proposal (revision 9), PD-16 (open item O-13)
**Verdict:** resolved
**docs/:** unchanged by this commit. The text is deposited in the proposal and
enters `docs/` with it: `SIM-DEC` DEC-099, decision, rationale and cost;
`SIM-REQ` FR-C-04, steps 2 and 4; `SIM-ECON` §Demand, the paragraph "Choice
within a class".

PD-16 has a cohort feed itself in four steps. It eats from its own holdings
first, lowest grade first. It buys survival at the lowest price per unit of
value. With what remains, it replaces units of a lower grade with a higher one,
the share of members able to pay read from the wealth curve. The markets of a
class clear in descending grade, and demand rationed out of one grade falls to
the next within the update. **Adopted**, for what the proposal says it buys.
Within one mestiere the poorer eat grain and the better-off bread, which makes
OBS-3 visible at the table. Demand for grain gains elasticity. And reading the
curve rather than the cohort's mean keeps a small price move from switching a
whole cohort between goods. Three corrections, each needed for the rule to hold
together with what `docs/` already says:

1. **The better grade comes after every need, in spending and in rationing.**
   The proposal spends what remains after survival on the better grade, which
   would put a rich cohort's bread before a poorer cohort's tier 1 need.
   `SIM-ECON` says that tiers are met in order and that rationing runs "within
   a tier, never across tiers: nobody buys cloth while another cohort … cannot
   buy bread and could pay for it". A better grade is a preference, not a
   need. So it is bought from the currency left once tiers 0 and 1 are met,
   before tier 2 takes its fraction. In a market it ranks after every demand
   that meets a need: nobody eats better while another cannot eat and could
   pay. Every need is bought at its cheapest, tier 1 as well as survival.
2. **Rationed demand falls to any good that meets the same need, not only to
   the grade below.** One pass in descending grade lets a cohort rationed out
   of bread fall back to grain. It does not let a cohort rationed out of grain
   buy bread left unsold in a market that has already cleared. That cohort
   would starve beside the bread, and the cause would be the order of a loop,
   not price or purse. So the rule is stated as its principle. Demand rationed
   out of one good falls, within the update, to the other goods that meet the
   same need, cheapest per unit of value first. The order in which a class's
   markets clear is derived from grade, price and good id, and may take a
   second pass over what supply remains. The implementer records it.
3. **Alternation within a grade is a cost, written down.** All of a need's
   demand goes to the cheapest good per unit of value. Two foods of one grade
   at nearly equal prices then take turns: the one bought down becomes dearer,
   and the next update buys the other. Stock bounds the swing, and with one food
   per grade in the baseline content it cannot arise yet. Content that adds a
   second food of a grade (O-11) is where it is measured.

**Cost.** Every food is one more market to ration in the most expensive phase,
so the number of foods is set after a measurement. The markets of a class
acquire an order, and demand may cross them more than once in an update. In
ordinary years the plate is mixed, and the needs table is tuned so that the
total is plausible.
**Owed by the implementer:** nothing now.
**Would overturn it:** the measurement of phase 7 with the food markets the
content fixes coming in over the budget of DEC-041, which would argue for
dropping the upgrade first, since it is the part that serves only legibility;
or alternation within a grade large enough to show in AC-04's soft bands, which
would argue for splitting a need's demand across goods of a grade rather than
giving it whole to the cheapest.
**Not verified:** that the curve, read for currency left after needs rather than
for wealth, gives a plausible share of upgraders. The proposal's own
approximation in DEC-097, one shape for wealth and currency, is carried one step
further here.

### R-059 — The poverty and seed proposal enters `docs/`

**Date:** 2026-10-04   **Origin:** user: "sì, trascrivi la proposta in docs/"
**Verdict:** specified
**docs/:** all six documents, each revision bumped to today.
- `SIM-DEC`: DEC-090 to DEC-099 new; DEC-015, DEC-038, DEC-043, DEC-047, DEC-076
  and DEC-080 edited.
- `SIM-REQ`:
  - §5: FR-D-03a and FR-D-04a.
  - §6: FR-P-01, FR-P-03, FR-P-04, FR-P-10, FR-P-15, FR-J-01 and FR-J-03; new
    FR-P-03a to FR-P-03c, FR-P-10a to FR-P-10e and FR-J-19 to FR-J-22.
  - §7: FR-C-03 to FR-C-05 and FR-C-25.
  - §8: FR-M-13.
  - §14: FR-G-01.
  - §16: P-66 to P-72, and P-09 at 8.
  - §17: AC-09 and AC-56.
  - §18: the Phase 4 table.
  - §19: P-17.
- `SIM-ECON`: phases 4 to 7, 11 and 12; new phase 8a; the paragraph on phase 7
  moved below the table; dated steps, §Demand, spoilage, and items 2 and 5 of
  what the sequence does not cover.
- `SIM-STATE`: §Cohort, §Transient, static data, A-16 and A-17.
- `SIM-LOOPS`: B2, R4, R5, R7, new R12, the standalone loops, the gate check,
  and open item 4.
- `SIM-OBS`: OBS-3, "Served by".

The proposal of 2026-10-04, revision 10, records the decisions the user took in
the design sessions of revisions 2 to 9. It also holds the text `R-054` to
`R-058` decided for what the user left to the decider. The user conditioned its
entry on B12b being written, so that the gate of `SIM-LOOPS` would not reopen.
`R-055` wrote it, and the user then asked for the transcription. **This ruling
decides nothing new. It transcribes.**

**How.** A script read each quoted block of the proposal and replaced or
inserted it at the place the proposal names. It refused any target that was not
found exactly once. It stripped the ruling markers, and refused any block still
holding a `[DECIDER]` placeholder. The diff was then read against the proposal,
document by document.

**What the proposal reserved to the user, and is applied here on the user's
authority.** The new AC-09 table and AC-56's wording are reserve 2 of `R-001`,
decided by the user in revision 9. The addition to OBS-3's "Served by" is
reserve 3, authorised by the user in the same revision. P-09 at 8, with flour,
is the user's. The decider applied these and did not decide them.

**Where the text differs from the proposal.** Each is form, not content:

- DEC-098's cost said "approved under R-050". `docs/` does not cite rulings, and
  that rule is already stated in §16, so the cost now reads "approved by the
  user as static content under SIM-REQ section 16, \"Parameters to fix\"", in
  the form of `R-013`.
- Four instructions in the proposal are prose in Italian, not text to insert:
  - FR-J-01's two additions;
  - P-17's re-measurement in §19;
  - DEC-047's sentence;
  - the R4 lines.

  Each was written in English as the proposal describes it. The P-17 row now
  says the measurement counts 11 and 33 rows, eight goods and the food markets.
  P-63 and P-64 stay as measured, with the row stating what they were measured
  on.
- §18: Production's "FR-P" now reads "FR-P, save FR-P-03b", since the proposal
  places FR-P-03b under Needs. Otherwise one requirement would sit under two
  subsystems.
- P-72's description drops the ruling number from the parameter's name.
- "Choice within a class" goes after the Hunger paragraph of §Demand. The
  proposal gave no place.

**Findings left open.** Each one is the user's or a later point's, not this
ruling's:

1. PD-14 does not say in whose units a subclass carries its value (`R-056`).
2. FR-P-22's endowment is either what resettlers carry or durables made from
   nothing (`R-054`).
3. OBS-2 and OBS-4 could cite FR-D-04a. That would be reserve 3, and it is not
   done.
4. DEC-047's title, "once per sowing", still contradicts FR-P-15's settlement
   cadence for other activities (X-3, Z-05).
5. `SIM-ECON` phase 6 implements FR-P-10d, which §18 places under Production.
   The point that builds the seed reserve must record which subsystem releases
   it (`R-052`).
6. FR-P-04's list and all the static content of §16 are still awaiting the
   user's approval (`R-050`).

**Cost.** Phase 4's scope grows by the destitute and day-labourer rows, the crop
stages and the goods classes. P-17, P-63 and P-64 must be measured again. The
static content to approve before the plan grows with them.
**Owed by the implementer:** nothing new beyond `docs/` as it now stands. The
phase 4 plan reads its `Satisfies:` against the amended §18 table (`R-052`).
**Would overturn it:** a passage of `docs/` found to differ from the proposal
beyond the differences listed above.
**Not verified:** the diff was read whole, but the transcribed passages were not
compared word for word against the proposal. That rests on the script's exact
copying. A grep of `docs/` finds no remaining "Subsistence", "Luxury", "need
tier", `wealthSpread` or `creditLine`, but references to the old vocabulary in
other words were not searched for.

### R-060 — A subclass measures in its root's units, and a good carries one value per class tree

**Date:** 2026-10-04   **Origin:** user, design session, on the finding of `R-056`
**Verdict:** specified — the user's decision, reserve 5 of `R-001` (PD-14 is theirs)
**docs/:** `SIM-REQ` FR-P-03a gains one sentence. Revision already today's.

PD-14 let classes nest and gave a good a value per class, without saying in whose
units a subclass counts. A need on a class accepts its subclasses "in proportion to
its value" (FR-P-03b), and the conservation rule reads a good's value "in the food
class" (FR-P-03c); both need one scale per tree. The alternative, units per class
with a conversion factor to the parent, is more data that can be wrong and buys no
behaviour the single scale lacks. Put to the user with that recommendation, the user
approved it: a subclass measures in its parent's units, a good carries one value per
class tree, and validation at load refuses two values in one tree.

**Cost.** None now: the content approved by `R-063` has no subclass.
**Owed by the implementer:** the check at load, with the validation of FR-P-03b.
**Would overturn it:** a class whose subclasses are genuinely incommensurable, which
should then be two trees, not one.
**Not verified:** nothing; FR-P-03a to FR-P-03c were read.

### R-061 — Resettlement creates no goods: the endowment is the generator's, resettlers bring what they carried

**Date:** 2026-10-04   **Origin:** user, design session, on the finding of `R-054`
**Verdict:** specified
**docs/:** `SIM-REQ` FR-P-22 reworded; §16 P-54 renamed. Revision already today's.

FR-P-22 gave founders and resettlers alike an endowment of durables. Since `R-054`,
emigrants carry their durables (FR-D-04a), so an endowment at resettlement on top
of that would create durables from nothing, and emigrating to an empty settlement
would become a way to mint tools. What FR-P-22 protected against, a cohort that
cannot work for lack of tools, is already handled by the crude variants of FR-P-21
and DEC-073; and tools, light and valuable, are what FR-D-04a loads first. Put to
the user with that recommendation, the user approved it: the generator's founders
carry P-54, resettlers bring what they carried, and resettlement creates nothing.

**Cost.** A resettled village restarts slower than one founded by the generator.
B9, the brake on abandonment, runs on land per capita and is not weakened.
**Owed by the implementer:** nothing beyond `docs/`.
**Would overturn it:** a sweep in which resettled villages never recover their
tools, which would argue for cheaper crude variants rather than for minting.
**Not verified:** that no other text gives resettlers goods; FR-D-07, DEC-067,
DEC-070 and FR-J-18 were read and do not.

### R-062 — OBS-2 and OBS-4 already cover FR-D-04a; `SIM-OBS` is not changed

**Date:** 2026-10-04   **Origin:** user, design session, on the finding of `R-059`
**Verdict:** resolved — the user's decision, reserve 3 of `R-001`
**docs/:** unchanged

`R-059` left open whether OBS-2 and OBS-4 should cite FR-D-04a. Neither needs to.
OBS-2's "Served by" reads "FR-D-01 to FR-D-11", a range FR-D-04a and FR-D-03a fall
inside; OBS-4's reads "FR-M-12 to FR-M-14", and the refugee sentence lives in
FR-M-13. `SIM-OBS` lists the requirements that lose their justification if the
entry is cut, and FR-D-04a would keep its own through OBS-2 if OBS-4 went. The user
approved leaving the document as it is.

**Cost.** None.
**Owed by the implementer:** nothing.
**Would overturn it:** OBS-2 being cut, which would leave FR-D-04a to be justified
by OBS-4 explicitly.
**Not verified:** nothing; the five "Served by" lines were read.

### R-063 — The static content of Phase 4 is approved

**Date:** 2026-10-04   **Origin:** user, design session: "approvo tutto"
**Verdict:** resolved — the user's approval under `R-050`
**docs/:** `SIM-REQ` FR-P-04 confirmed, §19's P-17 row at 12 and 36 rows; `SIM-STATE`
§Cohort at 12 rows, open item 1 closed. Revisions already today's.

`R-050` makes static content the user's, approved before the plan that uses it. The
decider proposed it from the phase 4 note (K-01 to K-11) as amended by the proposal
of `R-059`, and the user approved the whole. It is recorded here because it is
data, not specification: under `R-065` it lives in the rules file, and `docs/`
keeps only what a requirement states. **Every number in it is Provisional under
`R-050`**: the implementer proposes it in the point that first needs it, and the
decider numbers it in §16. The user approved structure, not values.

**Goods, classes, grades.**

| Good | Class | Grade | Behaviour |
|---|---|---|---|
| Grain | Food | 0 | consumable, input, planting good |
| Flour | Ingredient | 1 | input |
| Bread | Food | 2 | consumable |
| Cloth | Clothing | 1 | consumable: the need is the rate of replacement |
| Ore, Timber, Wool | Material | 0 | input |
| Tools | Implement | 1 | durable |

No subclass. Ingredient, Material and Implement are labels that no need reads. A
grade counts processing steps, so bread is 2.

**Needs.** Tier 0: Food, grade 0 or more, the survival minimum. Tier 1: Food, grade
1 or more, the share above the minimum (`R-057`); Clothing, grade 1 or more. Tier 2:
none in the baseline; the code path is tested with a test good.

**Recipes and buildings.**

| Recipe | Building | Durable | Crude variant |
|---|---|---|---|
| Grain → Flour | Mill, site water | independent | yes: a hand quern, no building, far slower |
| Flour → Bread | Oven | independent | no |
| → Ore | Mine, site ore | requires Tools | yes |
| → Timber | none, site forest | improved by Tools | — |
| Ore + Timber → Tools | Forge | requires Tools | yes |
| → Wool | none; continuous, modulated by climate | independent | — |
| Wool → Cloth | Loom | improved by Tools | — |

The quern is content, not FR-P-21's obligation: without it every settlement with no
running water would leave its tier 1 food need unmet for good, and fragility would
mark the same places for ever.

**Mestieri.** Eleven in the allocation vector: farmer, miller, baker, miner,
woodcutter, smith, shepherd, weaver, caravanner, merchant, day labourer; and the
destitute row outside it. Mean wealth at generation, descending: merchant, miller,
smith, baker and weaver, farmer, shepherd, miner and woodcutter, caravanner, day
labourer, destitute. Spread within a row: high for merchant and farmer, middling for
the crafts, low for day labourer and destitute. The day labourer adjusts faster than
P-19 and has no yearly cap.

**Crops.** Grain alone, sown with itself; a harvest window of a few updates; care
levels zero in the shipping configuration (DEC-094).

**Age bands.** Three, closing `SIM-STATE` open item 1.

**Generator (K-09), as rules.** Kingdoms counted by a Provisional parameter; five
terrains, plain, hill, forest, mountain, water, each with a transport cost; sites by
terrain, forest in forest, ore in hill and mountain, water along watercourses;
initial stocks that cover each settlement's tier 0 for a season; the destitute row
from the wealth curves (FR-G-01).

**Deferred to their subsystem.** Event classes (K-10) and contracts, dungeons and rare
materials (K-11): no dungeon-only material in Phase 4, so P-09 stays at 8. Climate
curves are `R-064`.

**Cost.** Twelve rows per settlement where `SIM-STATE` said eleven and phase 3
measured nine: P-17, P-63 and P-64 are measured on twelve and thirty-six.
**Owed by the implementer:** the content as above, in the rules file of `R-065`, with
each value proposed as a P-number when first needed (`R-050`).
**Would overturn it:** the user revising any item, which is theirs at any time before
the plan that uses it.
**Not verified:** that the conservation rule (FR-P-03c) admits the chain with the
values the implementer will propose; nothing is numbered yet.

### R-064 — Each climate modifier has its own pair of curves

**Date:** 2026-10-04   **Origin:** user, design session (K-08 of the phase 4 note)
**Verdict:** specified
**docs/:** `SIM-REQ` FR-P-08 and §16 P-14; `SIM-DEC` DEC-037, title, decision,
rationale and cost. Revisions already today's.

FR-P-08 named two curves and five modifiers, and did not say whether the five share
the pair. P-14's "2 curves" read as sharing. One pair cannot render a winter that
stops building sites without raising mortality as well, so the five would move in
lockstep, and the seasons of trade, construction and death could not differ. Put to
the user with a pair per modifier, the user approved it. Only the curves a subsystem
uses are written when it lands; the implementer prepares smooth starting curves and
the user approves them, as static content under `R-050`.

**Cost.** Five pairs to author and tune instead of one: 520 numbers.
**Owed by the implementer:** the curves of each modifier in the rules file of
`R-065`, written with the subsystem that first reads it.
**Would overturn it:** two modifiers found always to want the same curves, which
could then share a pair by data, with no change to the rule.
**Not verified:** nothing; FR-P-07 to FR-P-11, DEC-036, DEC-037 and P-14 were read.

### R-065 — Static content and simulation parameters are a rules file the host supplies: DEC-100; `R-004` revised

**Date:** 2026-10-04   **Origin:** user, design session: "approvo C"
**Verdict:** promoted as DEC-100 — the user's decision; it touches NFR-01 and AC-02,
reserve 2 of `R-001`, and revises `R-004`
**docs/:** `SIM-DEC` DEC-100 new, after DEC-089; DEC-032 title and decision, DEC-033
and DEC-089 one clause each. `SIM-REQ` FR-A-05 new; NFR-01, NFR-08, NFR-09, NFR-12
and AC-02; §16's opening paragraph. `SIM-STATE` §World gains `rulesData`, and
`ruleVersion` is the logic's version; §Static data says where the data come from.
Revisions already today's.

The user objected to recipes living in the core, since every change is a rebuild.
Three answers were put to the user: tables in the core's source; tables generated
into the core from a text source; or a binary rules file the host reads and hands to
the core. The decider recommended the third, and the user chose it.

**Why.** A rebuild of the core costs seconds, so speed of editing is not the
argument. The arguments are three:

- **Sweeps.** Phase 6 sweeps run many worlds that differ only in content and
  parameters, and with the data in the core each variant would be a build.
- **Content patches.** Content can be patched without releasing logic.
- **Authors.** Content can be written by people who do not touch the C#.

The core still reads no file: the host hands over bytes, as it does for saves
(DEC-034, D-090).

**What it costs the specification**, each part applied above:

1. **The data are input, like the seed.** NFR-01 and AC-02 name the rules file
   beside the seed and the commands. Without that, two runs with different files
   would break the criterion as written. This is reserve 2, and the user's
   decision.
2. **The world carries its rules file, and the save with it** (NFR-08, DEC-032).
   A reported bug reproduces only with the exact file. DEC-033's resumption under
   the old rules needs the old rules at hand. A loaded world runs on the file its
   save carries. Moving an existing world to new content would need an id
   mapping and a reshape of state, so it is refused until a requirement asks for
   it.
3. **`R-004` revised.** `R-004` held that "the rules are part of the build", so
   that a new world's rule version is the build's and nothing the host chooses.
   Half of the rules now come from the host. The rule version becomes the logic's
   version, still the build's, together with a digest of the rules file computed
   by the core from the bytes (NFR-09). `R-004`'s point survives: a host-declared
   version was "the only one that can lie", and a digest the core computes cannot.
4. **Binary, from text.** The source is text in the repository, readable and
   reviewed where the user approves content. An offline tool in the harness
   converts it to integers and fixed point, and checks the result by handing it
   to the core (NFR-12). Parsing text in the core would put number formatting
   and locale on a path that must be deterministic.
5. **Validation is the only check** of content now that the compiler is not. It
   already exists in pieces (FR-P-03b, FR-P-03c, AC-56, DEC-089). Under FR-A-05
   it refuses a file whole.

**Cost.**
- A second versioned format.
- A compiler in the harness.
- A host that must ship and pass the file.
- Validation that must be complete.
- Existing code:
  - `WorldState`'s constructor stamps `Simulation.CurrentRuleVersion` alone;
  - `SaveFormat` writes no rules file;
  - the pins of `R-034` and `R-036` move once `World` gains the field.

**Owed by the implementer:** listed above under "Owed by the implementer". Cite
`R-065`.
**Would overturn it:** a consumer that cannot ship a data file beside the core,
which would bring the tables back into the source (the second answer); or a need to
move an existing world to new content, which would need the id mapping this ruling
refuses.
**Not verified:** that the consumer of §20 can ship and read such a file; that
hashing the file's digest rather than its bytes is enough for the state hash. The
file is immutable, so the digest stands for it, but the fold's injectivity
(`D-069`) was not reconsidered for a 64-bit digest.

### R-066 — The process plan's seven points and its changes to the design are the user's: `R-046` and `R-028` amended

**Date:** 2026-10-05   **Origin:** D-110
**Verdict:** resolved — the user's decision, reserve 1 of `R-001`; the rest of
`D-110` ratified
**docs/:** unchanged

`D-110` records the user's answers to the proposal of 2026-10-02
(`.claude/design/2026-10-02-integrazione-piano-processo.md`) and passes to the
decider the two that depart from rulings. Asked in this session whether both
are the user's, the user answered: "Confermo". Governance, reserve 1 of `R-001`:
recorded here, not ruled.

- **Seven points against `R-046`'s five.** `R-046` owed "the process plan holds
  the design's points 1 to 5 and no simulator change". Its reason was the second
  half: each plan of one kind, and the design's point 6, a simulator change, out
  of a process plan. The two points added — 3, `plan-state`, and 6, `/plan-explain`
  and `/plan-status` read through it — are process work and touch no path under
  `core/`, so that half stands. The first half is amended: the design's points 1
  to 5, plus `plan-state` and the two skills on it.
- **Changes to the design `R-028` ratified.** Its "deliberately not built"
  excluded any change to `/decide`, `/plan-explain` or `/plan-status`; the last
  two now change, and `/decide` stays excluded. The proposal's fifth question,
  the archive of `RULINGS.md`, is rightly left out of the plan: it is a change to
  `/decide`, and reserve 1 whatever a plan says (`R-028`). The tree check around
  read-only agents extends from `/plan-next` to both skills. `tree-state` and
  `plan-state` live in `.claude/hooks/`, which the coder's denylist already
  covers; the proposal's reason is the right one, that a script an agent can
  edit is no constraint on it.

**The rest of `D-110` is ratified**, each checked against the draft in
`REVIEW-REQUEST.md`:

- **The rewiring last.** Point 7 rewires `/plan-next`, so every point of the plan
  runs under today's procedure and `R-046`'s first real run stays outside it. The
  proposal's one order constraint holds: points 3 and 5 precede 6 and 7.
- **`plan-state -Draft`** reads the fenced block under `## Draft plan`, and the
  request uses that heading, so point 6's check runs on a real draft.
- **The branch** follows `R-048`: the request on `design/process-plan`, the plan
  installed on `agent-workflow`, cut from `main` after the answer.
- **The word cap of point 7** is flagged in the request as unanswered and put to
  the user in its item 1. Checked: `/plan-next` is 2,755 words today.
- **The number.** `D-108` and `D-109` are on `design/poverty-seed`, so `D-110` is
  the next free across branches. This ruling is `R-066` for the same reason:
  `R-054` to `R-065` are on `design/poverty-seed`. An identifier is never reused,
  and two branches issuing the same one would collide at the merge.
- **The request replacing the moment-three request**, and the conflict it will
  meet when `design/poverty-seed` merges: rightly noted.

**The watermark is not advanced.** `D-108` and `D-109` sit between it and
`D-110` and are not on this branch; moving it to `D-110` would record them as
audited. `D-110` carries its own marker, and the audit that reaches `D-108` and
`D-109` after the merge passes over it.

**Cost.** The process plan is two points longer, and builds a workflow that is
not the one the design note describes: the note is not amended, so a reader of
it alone sees the old scope, and `R-028` now reads with this ruling beside it.
**Owed by the implementer:** `R-046`'s first action, as amended above. Cite
`R-046` and `R-066`.
**Would overturn it:** the user's answer at moment one dropping point 6, the one
the proposal calls deferrable, which leaves `R-045` owed on those two skills.
**Not verified:** the user's words of 2026-10-05 for the four answers were not
seen; this session has only the confirmation of the two departures. The "31
points, one commit each" of point 3 was not re-measured.

### R-067 — The agents' first real run stays at phase 4's first point; the measurement plan runs under them without proving them

**Date:** 2026-10-05   **Origin:** review request 2026-10-05, the open question;
D-110; D-109
**Verdict:** resolved — the user's decision, reserve 1 of `R-001`
**docs/:** unchanged

`R-046` puts the design's point 6 check — the tester's test red then green, the
named fault seen red, every agent dispatched seen to have run — in the criterion
of "the first point of the phase 4 plan", the first plan a rewired `/plan-next`
was then expected to run. The user has since placed the cost measurement plan
(`D-109`, drafted on `design/poverty-seed`) between the process plan and phase 4,
so its points are the first the rewired skill runs. The request asks whether its
first point carries the check.

**It cannot carry the check as `R-046` states it.** The measurement plan writes
only `bench/`. The workflow is built on the core: the tester writes in
`core/Tests/` only (the roles table of `.claude/design/2026-09-30-agent-workflow.md`),
and `mutate` records and restores `core/Runtime/` and `harness/` (point 2 of the
process plan's draft). In `bench/` there is no test for the tester to write first
and no tree for `mutate` to guard. The check would fail because `bench/` does not
fit the workflow, which says nothing about the workflow, or it would need the
scripts widened, a change to the process plan for a plan that is not phase 4's.
`R-046`'s own reason still points at phase 4: a real change is a simulator
change, and phase 4's first point is the first change the workflow was designed
for.

Three answers were put to the user: the check stays with phase 4, and the
measurement plan runs under the agents without claiming it; the check moves to
the measurement plan, with the scripts extended to `bench/`; or the measurement
plan runs before the rewiring, which reverses the order the user chose in
`D-110`. The user chose the first: "Resta alla fase 4".

**The rule.** `R-046` stands as written. The measurement plan runs under the
rewired `/plan-next` as far as its roles apply to a `bench/` diff. Each outcome
line names the agents dispatched and those that did not apply, with the reason,
and no outcome counts as the proof `R-046` asks for, which stays in the criterion
of phase 4's first point.

**Cost.** Five points run under a workflow no one has yet seen run correctly, and
their figures feed `SQ-023`'s ruling on the shape of state. What guards those
figures is `D-109`'s checksum per cell, which does not depend on the agents.
**Owed by the implementer:** in the cost measurement plan, each outcome line
names the agents dispatched and those that do not apply to its diff, with the
reason, and none claims `R-046`'s check. Cite `R-067`.
**Would overturn it:** the workflow extended to `bench/`, which would make the
measurement plan's first point eligible; phase 4 starting before the measurement
plan, which puts `R-046` back where it was.
**Not verified:** the measurement draft and `D-109` were read on
`design/poverty-seed`, not on this branch. Whether the coder's denylist admits
`bench/` was read from the draft of point 1, not from a script that does not yet
exist. Item 2 of that branch's request, "Where it sits against `R-046`", is
answered here but not marked there, since the file lives on the other branch.
The marker is owed by the decider when the branches meet.

### R-068 — The coder may not write the settings files that can switch its hooks off

**Date:** 2026-10-05   **Origin:** D-111 (a finding noted, not acted on)
**Verdict:** escalated — reserve 1 of `R-001`: what each role may write is
`D-087`'s, and the design `R-028` ratified is the user's
**docs/:** unchanged

`D-111` notes that the coder's denylist does not hold `.claude/settings.json` or
`CLAUDE.md`, and leaves it there on one premise: "A settings file can add hooks,
not remove an agent's own." The premise does not hold as stated. Claude Code's
documentation (hooks guide, "Configure hook location") gives a settings key,
`disableAllHooks`, that turns hooks off, and says a settings file edited during a
session is normally picked up by a file watcher in that session. Whether the key
reaches hooks declared in an agent's frontmatter the documentation does not say.
So one `Write` the coder is allowed today may disable `path-guard` and
`git-guard` for every role, or may not; nothing in the repository knows which.

The design gives the reason for its own last three entries: they "keep an agent
from loosening its own constraint". A settings file is the same kind of file as
`.claude/hooks/`, and the reason applies to it whether or not the key reaches
frontmatter hooks: a settings file adds hooks, and a hook is a command run on
every tool call of every role. The orchestrator's check after the coder, "no
path of its denylist changed", inherits the gap, since it reads the same list.

`CLAUDE.md` is a different case. The root file only imports `AGENTS.md`, and
`core/Runtime/Systems/CLAUDE.md` holds a rule `R-019` changed as governance.
They are standing instructions, as `AGENTS.md` is, and the design keeps
`AGENTS.md` off the denylist on purpose: a process point may edit it, and the
orchestrator stops if the diff touches it and the point does not name it.

**A second finding, on `git-guard`.** `D-111` defines an identifier as `NFR-`,
`FR-`, `DEC-`, `AC-`, `SQ-`, `A-`, `R-` or `D-` followed by a letter or digit.
The script matches with `-notmatch`, which in PowerShell ignores case, so a
subject reading "Tidy the a-priori notes" passes as carrying one; checked in
this session. The guard stops mistakes, and this one lets a subject with no
identifier through.

**Recommendation.** One point added to the process plan under `D-014`, before
point 7 so that the rewired orchestrator reads the final list:

1. `.claude/settings.json` and `.claude/settings.local.json` join the coder's
   denylist, in the design and in `path-guard`, with a self-test case each.
2. Every `CLAUDE.md` is treated as `AGENTS.md` is: not denied, and the
   orchestrator stops when a diff touches one the point does not name. Point 7
   writes that check, so the point adds it to point 7's scope by naming it.
3. `git-guard` matches the identifier case-sensitively, with a self-test case
   for a lowercase `a-`.

Point 1 is closed and frozen; none of this reopens it. The fault each change
must catch is the case it adds.

**Cost.** One more point in a plan of seven, and a design note amended by a
point rather than by `R-028`. A coder can no longer fix a settings file a
simulator point needs changed; the orchestrator does it, in a point that says so.
**Owed by the implementer:** nothing until the user answers.
**Would overturn it:** a probe at point 4 showing that `disableAllHooks` does not
reach frontmatter hooks and that a settings file cannot otherwise weaken an
agent's guard, which leaves item 1 resting on the design's general reason alone.
**Not verified:** whether `disableAllHooks` reaches frontmatter hooks, which the
documentation leaves open and this session did not probe; the guide was read
through a subagent, not opened directly.

### R-069 — `D-111` is ratified, save what `R-068` escalated

**Date:** 2026-10-05   **Origin:** register audit, `D-111`
**Verdict:** ratified
**docs/:** unchanged

Point 1 of the process plan, the first two guards. Process, so ratified or
rejected, never promoted (`D-011`). The entry was read against the design's
"Enforcement" items 1 and 2 and "What the coder may not write", and its claims
against `0ea4e42`. Both self-tests were run in this session: `path-guard` 32
cases, `git-guard` 43, exit 0 each.

- **The role as `-Role`, an unknown role denied.** Stands; it is the only way a
  single script can be keyed by role, and point 4's probe is where it is shown
  to work. Denying the unknown is the allowlist reading the design implies.
- **Outside the repository, denied to every role.** Stands, and answers the
  entry's own "would overturn it": "everything except the denylist" is read
  inside the repository. Nothing in the roles writes elsewhere, and a coder
  writing outside the tree escapes every check the orchestrator runs on it.
- **Paths resolved before matching, case-insensitively.** Checked: the cases for
  `DOCS\`, `..` and a sibling folder named `repo-other` are in the self-test,
  and a trailing dot (`docs.\x`) resolves to `docs\x` under .NET's own
  normalisation. 8.3 short names do not arise on this volume.
- **Insert-only refuses an empty `old_string`.** Right on the merits: every
  string begins with the empty one, so without it insert-only admits a
  rewrite of any file the documenter holds.
- **`git-guard` refusing more than the design lists**, and `Plan-point` counted
  anywhere in the message. Each follows from "admits only" read as an allowlist,
  and from the skill's rule that only a closing commit carries the word.
- **Exit 2 with the reason on stderr.** The hook contract's blocking form.
- **The phrase "no identifier applies"** as the words that say none applies.
  Stands. The identifier half is `R-068`'s second finding: the script matches it
  without regard to case, which the entry's definition does not.

The note on `.claude/settings.json` and `CLAUDE.md` is `R-068`'s first finding.

**The watermark is not advanced**, for `R-066`'s reason: `D-108` and `D-109` lie
between it and this entry and are not on this branch. `D-110` and `D-111` carry
their markers, and the audit that reaches `D-108` after the merge passes over
both.

**Cost.** None beyond `R-068`.
**Owed by the implementer:** nothing new.
**Would overturn it:** as for `R-008`, any entry found to contradict `docs/` or
the design as it stood when written; point 4's probe showing the role cannot be
passed as an argument.
**Not verified:** the faults of `Fails when:` seen red were taken from the
outcome line; that Claude Code runs the scripts from frontmatter is point 4's.
That the commit `0ea4e42` and the outcome line are in Italian is consistent
with the plan's own rule that points 1 to 4 close in the forms in use, and was
not otherwise examined.

### R-070 — `R-068` accepted: point 8 of the process plan closes the settings files and the identifier match, before point 7

**Date:** 2026-10-06   **Origin:** user, on `R-068`
**Verdict:** resolved — the user's decision, reserve 1 of `R-001`
**docs/:** unchanged

The user approved `R-068`'s recommendation and asked for it to be applied:
"approvo R-068, applicalo". The changes are to the design of 2026-09-30, to
`path-guard` and to `git-guard`, none of which a decider session writes. So
applying it means owing a point, which the implementer adds to `PLAN.md` under
`D-014` and runs as any other. This ruling records the user's approval that
`D-014` requires.

**Where the point goes.** `R-068` placed it "before point 7". `D-014` says a
point added mid-plan runs last, so as not to renumber points already cited.
Both hold if the point is **numbered 8 and placed in `PLAN.md` between points 6
and 7**. No number moves. `/plan-next` runs "the first `## <n>.` section with no
outcome line", so it runs 8 before 7. Running it last would not work. Point 7
wires the agents in, so a point after it would be run by them, and its coder
cannot write `.claude/hooks/`. It would also contradict the plan's premise that
no point of it runs under the agents (`R-046`, `R-067`). This departs from
`D-014`'s "runs last" for this point only, at the user's request. `D-014`'s
reason, no renumbering, is kept whole. The plan's preamble, which calls point 7
"the last", is not edited; point 8's own text says it runs before 7, citing this
ruling.

**What it does**, `R-068`'s three items, with one refinement to the second:

1. `.claude/settings.json` and `.claude/settings.local.json` join "What the coder
   may not write" in the design and `$CoderDenied` in `path-guard`, with a
   self-test case each.
2. In the design's "The hole that `Bash` leaves", the check reading
   "`AGENTS.md` in the diff only if the point names it" covers every
   `CLAUDE.md` as well. `R-068` had point 7's scope widened "by naming it".
   That is not needed: point 7's frozen text already has the orchestrator "run
   the checks of 'The hole that `Bash` leaves'", so point 7 builds the check
   from the amended list, and its text stays as it is. The same holds for the
   coder's denylist, which point 7's check after the coder reads from the
   design.
3. `git-guard` matches the identifier case-sensitively.

**Its `Fails when:`**, the cases `R-068` named: the coder may write
`.claude/settings.json`; a subject "Tidy the a-priori notes" passes with no
identifier; the design's list has no line for `CLAUDE.md`. It declares
`Core: no`.

**Cost.** One more point. The preamble's "point 7, the last" is now inaccurate
and stays so, since editing the plan's text above an added point is what
`D-014` avoids. A reader of `PLAN.md` alone learns the order from point 8's own
line.
**Owed by the implementer:** before point 7 runs, point 8 is added to `PLAN.md`
between points 6 and 7, with the content and `Fails when:` above, in a commit
citing `R-068` and `R-070`, and is run in its turn. Cite `R-070`.
**Would overturn it:** the user preferring point 8 after point 7, run by the
orchestrator alone; or point 4's probe showing that no settings key reaches a
frontmatter hook, which leaves item 1 on the design's general reason, as
`R-068` said.
**Not verified:** that `plan-state`, built at point 3, reads points in file
order and not by number. Its frozen criterion was not checked for it, and if
it sorts, point 8 would read as next only after 7. The user approved `R-068`
before this ruling stated the `Fails when:` lines, so the user reads them here,
as `R-047` asks of moment one, and has not yet done so.

**`Fails when:` read by the user, 2026-10-06:** "approvo le condizioni di
fallimento di R-070". The three lines stand as stated above, and point 8 carries
them verbatim.

### R-071 — `tree-state` misses edits to an untracked file whose name git quotes: a fourth item for point 8

**Date:** 2026-10-06   **Origin:** D-112 (found at audit)
**Verdict:** resolved — the user's approval under `D-014`
**docs/:** unchanged

Point 2's `Fails when:` names, first, "an edit to a file that is already
untracked leaves the hash unchanged". `tree-state` commits that fault for one
class of file. It lists untracked files with `git ls-files --others`, which under
`core.quotePath`, on by default and unset here, prints a name holding a
non-ASCII byte in C quotes: `"citt\303\240.txt"`. `Join-Path` makes that a path
to nothing, `Get-FileHash` fails with a non-terminating error, the line for the
file carries an empty hash, and the script prints a hash all the same. Checked in
this session on a throwaway repository: `città.txt` untracked, edited between two
calls, the same hash printed twice, an error on stderr each time, nothing that
stops a caller.

The second half is the wider defect. Whatever the cause — a quoted name, a file
locked by a running build, a permission — a file `tree-state` cannot read
leaves its hash where it was, and the script still answers. A check that cannot
look should say so, not report that nothing moved. `mutate` is not exposed
through its own refusal: `git apply --numstat` quotes the same names, a quoted
path does not begin with `core/Runtime/`, and the patch is refused. It is exposed
through the hash it compares, and so are the skills of points 6 and 7, which call
`tree-state` on the whole tree.

The point stays closed. Its check was met, the self-test's untracked file is
ASCII, and the outcome line is not rewritten; the defect is in code the check did
not reach. In practice the names under `core/Runtime/` and `harness/` are ASCII,
and the repository's are; that is why this is not urgent, and why it is not
nothing either, since an Italian-speaking repository is where a name like
`città.txt` turns up.

Put to the user with three answers — a fourth item in point 8, a point 9 placed
before point 6, or the limit accepted and recorded — the user chose the first.
Point 8 is owed by `R-070`, is not yet in `PLAN.md`, and runs before point 7,
the first after which an agent runs under `tree-state`. It is the same kind of
change, a guard closed after an audit found a way past it.

**The fourth item.** `tree-state` reads untracked names unquoted — `-z`, or
`core.quotePath` off for its own calls — and, when any file it must hash cannot
be read, exits non-zero and prints no hash. A self-test case each.

**Its `Fails when:`**, read by the user in the question that chose it, under
`R-047`: an edit to an untracked file named `città.txt` leaves the hash
unchanged; a file that cannot be read still yields a hash.

**Cost.** Point 8 grows by one item and one line. A caller of `tree-state` must
treat a non-zero exit as "unknown", which `mutate` does not yet do: it compares
two strings, and an empty `$before` against an empty `$after` would read as
"returned". Point 8 makes `mutate` stop with exit 3 when either call fails.
**Owed by the implementer:** point 8, as `R-070` owes it, carries this fourth
item and its `Fails when:` line, and `mutate` treats a failed `tree-state` as a
tree not returned. Cite `R-070` and `R-071`.
**Would overturn it:** none foreseen short of the user moving it to its own point.
**Not verified:** a locked file, which was not produced; only the quoted name was.
That `-z` output reaches PowerShell intact through a native call was not tried.

### R-072 — `D-112` is ratified, save what `R-071` resolved

**Date:** 2026-10-06   **Origin:** register audit, `D-112`
**Verdict:** ratified
**docs/:** unchanged

Point 2 of the process plan, `mutate` and `tree-state`. Process, so ratified or
rejected, never promoted (`D-011`). The entry was read against the design's
"Enforcement" item 3 and point 2's frozen text, and its claims against
`5cd1db4`. Both self-tests were run in this session: `tree-state` 11 cases,
`mutate` 9, exit 0 each.

- **The check as a command string run by `pwsh`.** Stands. The design names no
  form, and the tester already holds `Bash`, so the string runs nothing the
  tester could not.
- **Four exit codes.** Stands, with one consequence for later points. The design
  asks only for non-zero when the tree did not return, and exit 1, a fault not
  caught, is non-zero too: a caller that reads zero against non-zero would take
  a missed fault for a tree left dirty. The tester's definition at point 4 and
  its brief at point 7 read the four codes, not the sign.
- **A patch outside `core/Runtime/` and `harness/` refused.** Right on the
  merits: the restore is proved only where `tree-state` looks. Checked on a
  throwaway repository: a path through `..` passes the filter, since `--numstat`
  prints it as written, and is then refused by `git apply --check` ("invalid
  path"), so `mutate` exits 2. A rename is not refused: `--numstat` prints only
  its destination, so a patch moving `docs/a.txt` to `core/Runtime/a.txt`
  passes, applies, and leaves `docs/a.txt` absent while the check runs; the
  reverse put it back, and nothing proved it. That is `R-073`'s.
- **The working tree, the reversal in `finally`.** Stands; the self-test's fault
  is written against uncommitted code, as the coder's will be.
- **`--binary`, `--no-ext-diff`, `--no-textconv`.** Stands; the binary case is in
  the self-test and the two flags keep a user's configuration out of the hash.
- **`-Path` and `-Repo`; the self-test of `mutate` beyond the criterion.** Stand.

The untracked file the hash cannot read is `R-071`'s, and the rename `R-073`'s; both owe the fix in
point 8. The outcome line in Italian is the plan's own rule for points 1 to 4.

**The watermark is not advanced**, for `R-066`'s reason: `D-108` and `D-109` are
not on this branch.

**Cost.** None beyond `R-071` and `R-073`.
**Owed by the implementer:** nothing new.
**Would overturn it:** as for `R-008`; point 4's probe showing the tester cannot
run `pwsh` from its `Bash`.
**Not verified:** the known patch through `mutate` against the real suite was not
rerun, to keep the decider's session off `core/Runtime/`; the two faults seen red
were taken from the outcome line.

### R-073 — `mutate` lets a rename out of the guarded folders through: a fifth item for point 8

**Date:** 2026-10-06   **Origin:** D-112 (found at audit)
**Verdict:** resolved — the user's approval under `D-014`
**docs/:** unchanged

`D-112` decides that "a patch touching any path outside `core/Runtime/` and
`harness/` is refused", because the restore is proved only for what `tree-state`
records. The filter reads `git apply --numstat`, which prints one path per file:
for a rename, the destination only. Checked in this session on a throwaway
repository: a patch renaming `docs/a.txt` to `core/Runtime/a.txt` passes
`--numstat` and `--check`, applies, and removes `docs/a.txt` for as long as the
check runs. The reverse put it back. Nothing proved that it did, because
`tree-state` looks only at the two folders, and that proof is the reason the
filter exists. A copy reads its source and writes nothing outside, so it is
harmless today, but a filter that reads both sides of each entry covers it at no
extra cost.

The risk is small. The tester holds `Bash` and can reach any file without a
patch, and the guard stops mistakes, not an agent set on getting round it (the
design's "The hole that `Bash` leaves"). But the rule as `D-112` wrote it does
not hold, and the next reader of the entry would rely on it. Put to the user
with the fix in point 8 or the limit recorded, the user chose the fix, beside
`R-071`'s item, for the same reasons.

**The fifth item.** `mutate` refuses, with exit 2, a patch in which either side
of any entry, a rename's or copy's source included, lies outside `core/Runtime/`
and `harness/`. A self-test case.

**Its `Fails when:`**, read by the user in the question that chose it, under
`R-047`: a patch that renames `docs/a.txt` to `core/Runtime/a.txt` is applied.

**Cost.** One more item in point 8. The tester cannot move a file into the guarded
folders by a patch; it never needs to, since a fault is a change to code that
exists.
**Owed by the implementer:** point 8, as `R-070` and `R-071` owe it, carries
this fifth item and its `Fails when:` line. Cite `R-070` and `R-073`.
**Would overturn it:** none foreseen short of the user moving it to its own point.
**Not verified:** how `git apply --numstat -z` prints a rename, which is one way
to read both sides; the implementer's choice, and its self-test is the check.

### R-074 — `plan-state` reads "after the failure" by number, not by position: a sixth item for point 8

**Date:** 2026-10-06   **Origin:** D-113 (found at audit)
**Verdict:** resolved — the user's approval under `D-014`
**docs/:** unchanged

`D-113` decides that "`stopped` needs an open point after the failure, as
`/plan-next` words it". `/plan-next` step 1 says "with points still open after
it", and its next point is "the first `## <n>.` section with no outcome line":
both read the plan in file order. `plan-state` reads the next point the same
way, taking the first open point in the list it built from the file. That
answers `R-070`'s "Not verified": point 8, placed between 6 and 7, is reported
next after 6. But the test for `stopped` compares numbers: an open point counts
only if its number is greater than the failed one's.

The two orders agree in every plan so far, since `D-014` appends an added point
at the end. `R-070` made the one plan where they part: point 8 before point 7.
Checked in this session on a fixture in the scratchpad, points 6, 8 and 7 in
that order, 6 closed, 8 `FAILED`, 7 open: the script prints `state: running`,
`next: 7`, `stoppedAt: null`. That is the third fault of point 3's
`Fails when:`, "a plan stopped at a `FAILED` point is reported as running",
committed for the case the fixtures did not hold. After point 6, `/plan-explain`
and `/plan-status` read that state; after point 7, `/plan-next` does too.

The point stays closed, as `R-071` left point 2: its check was met, and the
outcome line is not rewritten. Put to the user with the fix in point 8 or the
limit recorded, the user chose the fix.

**The sixth item.** `plan-state` decides `stopped` by position in the file: a
failed point with an open point anywhere after it in the plan is `stopped`,
whatever the numbers. A self-test fixture whose points are numbered out of file
order.

**Its `Fails when:`**, read by the user in the question that chose it, under
`R-047`: a plan whose point 8, placed before point 7, has failed is reported
`running` with next point 7.

**Cost.** One more item in point 8. Until it lands, a failure of point 8 is
misreported by the two skills that point 6 puts on `plan-state`; the outcome
line `FAILED` is still there for a human to read.
**Owed by the implementer:** point 8, as `R-070` owes it, carries this sixth
item and its `Fails when:` line. Cite `R-070` and `R-074`.
**Would overturn it:** none foreseen short of the user moving it to its own point.
**Not verified:** whether any other rule of `plan-state` mixes number and
position; the script was read whole for it and none was found, but only the
`stopped` case was probed.

### R-075 — `D-113` is ratified, save what `R-074` resolved

**Date:** 2026-10-06   **Origin:** register audit, `D-113`
**Verdict:** ratified
**docs/:** unchanged

Point 3 of the process plan, `plan-state`. Process, so ratified or rejected,
never promoted (`D-011`). The entry was read against point 3's frozen text and
`/plan-status` step 4, and its claims against `b509ca5`, the script read whole.
The self-test was run in this session: 58 cases, exit 0. Run on the live
`PLAN.md` it reports points 1 to 3 closed, one commit each, and next point 4.

- **The JSON shape.** Stands; the point lists what is read, not names, and the
  names are the implementer's.
- **An abandoned plan reports no next point.** Right on the merits: a next point
  `/plan-next` must refuse is a report that contradicts the skill reading it.
- **`stopped` needs an open point after the failure; a failed last point is
  `finished`.** Stands as a reading of `/plan-next`'s words. How "after" is
  computed is `R-074`'s.
- **A reserve open leaves `state` alone.** Stands: `R-011` puts the unmet exit
  condition on the skills' reading, and `reservesOpen` gives them what they need.
- **A marker counts only right after the date sentence.** Checked: the regex
  strips the outcome marker and the first sentence, then anchors. The prose case
  it guards against, `FALLITO` named in a closed point, is in the fixture.
- **The check marker, `Core:` absent as `null`, commits resolved only for the
  live plan and the archives, the `main` warning, "no plan" as a missing path,
  the 31 points in the self-test.** Stand. Tying the self-test to the
  repository's history is right for the reason given: a rewrite that breaks it
  breaks the lookup it proves.

**One reading of the decider's count**, not a defect of the entry. Run today,
`decider` reports four entries, `D-110` to `D-113`, though three carry rulings:
the watermark stays at `D-107` while `D-108` and `D-109` are on
`design/poverty-seed` (`R-066`), and `R-002`'s count is taken from the watermark,
not from markers. The script does what `R-002` says. The count stays high until
the branches meet and the audit reaches `D-109`.

**The watermark is not advanced**, for `R-066`'s reason.

**Cost.** None beyond `R-074`.
**Owed by the implementer:** nothing new.
**Would overturn it:** as for `R-008`; a skill at point 6 or 7 needing a field
in another shape, as the entry says.
**Not verified:** the three faults seen red were taken from the outcome line.
Calling the script from an agent's `Bash` is point 4's probe. Headings inside a
fence of a plan, which the entry records, were not probed.

### R-076 — `D-108` and `D-109` are ratified; the watermark reaches `D-113`

**Date:** 2026-10-06   **Origin:** register audit, `D-108` and `D-109`
**Verdict:** ratified
**docs/:** unchanged

The two entries `R-066`, `R-069`, `R-072` and `R-075` passed over, because they
were on `design/poverty-seed`. The merge `eb62bfb` brought them here. Both are
process, so ratified or rejected, never promoted (`D-011`). Each was read
against what it cites, and its claims against the commits it names.

- **`D-108`, the review of the poverty and seed batch.** Stands. Checked:
  `9250363` adds `SPEC-QUESTIONS.md` alone, and the entry follows in `5485132`,
  as it says; `SQ-009` to `SQ-022` are the fourteen entries it counts, `SQ-020`
  marked the user's under reserve 4. A branch for a batch of design work is what
  `R-048` later made the rule for plans, and the branch was merged without
  squashing. Gathering the low findings in `SQ-019` and `SQ-022` is right: each
  item there is a correction of wording that a single ruling can apply, and
  splitting them would cost a ruling each for no difference in what is decided.
- **`D-109`, the draft of the cost measurement plan.** Stands. Checked:
  `64147ba` touches `REVIEW-REQUEST.md` alone. `Kind: process` follows from what
  the plan moves: no acceptance criterion, nothing in `core/`, and a world that
  is a stand-in. The tolerances are what the entry says: half and twice P-63's
  3.3 µs, and a fifth either side of 15.8 / 3.3 ≈ 4.8, which excludes a ranking
  linear in rows. Keeping `bench/` after the plan answers `SQ-004`'s finding, a
  stand-in thrown away whose contents nobody can now recover. Sizes as
  measurement inputs without P-numbers read `R-050` correctly: it numbers values
  that drive the simulation, and these drive a benchmark. The question the entry
  raised as blocking, where the plan sits against `R-046`, is `R-067`'s, and the
  request's item 2 now carries that marker, owed since `R-067` because the file
  was on the other branch.

**The watermark advances to `D-113`.** `D-110` to `D-113` were ruled by `R-066`,
`R-069`, `R-072` and `R-075`, and only `D-108` and `D-109` held it back.

**Cost.** None.
**Owed by the implementer:** nothing new.
**Would overturn it:** as for `R-008`, any entry found to contradict `docs/` as it
stood when written.
**Not verified:** the user's words for the sizes per settlement type, which
`D-109` reports as approved and `SQ-024` records; the plan's draft was not run,
and its calibration rests on P-63 and P-64 as measured on a stand-in nobody
holds.

### R-077 — FR-K-15 states the dungeon check as AC-56 does: on needs, not on goods

**Date:** 2026-10-06   **Origin:** SQ-009
**Verdict:** specified
**docs/:** `SIM-REQ` FR-K-15, its second sentence; revision bumped.

FR-K-15 required that "no tier 0 or tier 1 good depends … on such a material".
Since `R-059` a good has no tier: FR-P-03 and DEC-098 put tiers on needs, and the
user rewrote AC-56 and DEC-080 to protect needs (reserve 2 of `R-001`, revision 9
of the proposal). FR-K-15 was not in the proposal, so the same check at load had
two definitions, one of them with no referent. A grep of `docs/` for a tier
attached to a good finds FR-K-15 alone.

The two are not equivalent, and AC-56's is the right one on the merits, not only
because it is the user's. "No subsistence good depends on a dungeon" forbids
a dungeon material from entering any good a need accepts, which would rule out
a luxury bread made with a rare spice. "Every subsistence need can be met
without a dungeon" forbids only what the rule exists for, a famine caused by a
dungeon closing. FR-K-15 now carries AC-56's predicate and cites it, so the
requirement and its test say one thing.

**Cost.** None to code; no validation exists yet.
**Owed by the implementer:** nothing beyond `docs/`.
**Would overturn it:** none foreseen; AC-56 is the user's and this follows it.
**Not verified:** nothing; FR-K-11 to FR-K-15, AC-56 and DEC-080 were read.

### R-078 — A good names at most one class in each class tree

**Date:** 2026-10-06   **Origin:** SQ-010
**Verdict:** specified
**docs/:** `SIM-REQ` FR-P-01 and FR-P-03a; `SIM-DEC` DEC-015 and DEC-098. Both
revisions bumped.

`R-060` appended to FR-P-03a that a good carries one value per class tree and that
validation refuses two values in one tree, and left standing the sentence before
it: a good "belongs to one or more classes and carries a value per unit in each".
A good in a class and in its subclass needed two values by the first and was
refused for them by the second. FR-P-01, DEC-015 and DEC-098 repeated the first.

`R-060` is the user's decision and is kept whole; what was missing is the
membership rule that makes it consistent. **A good names at most one class in
each tree, and belonging to a subclass is belonging to every class above it.**
That is what FR-P-03b already assumes, since a need on a class accepts its
subclasses, and it is the only reading under which "one value per tree" and "a
value in each class it belongs to" are both true: the classes a good names are
one per tree, and each carries one value in the root's units. Validation refuses a
good naming two classes of one tree, which is the same check as `R-060`'s, stated
on what the record holds rather than on a value derived from it.

The other reading, a good naming several classes of a tree with only one value,
would make the extra memberships inert: a need on any of them is met by the one
value through the subclass rule. It adds data that says nothing.

**Cost.** None to code. An author who wants a good in two branches of one tree
cannot; that is two trees, as `R-060`'s overturning already says.
**Owed by the implementer:** the check at load, with FR-P-03b's validation, as
`R-060` owed it.
**Would overturn it:** as for `R-060`.
**Not verified:** nothing; FR-P-01, FR-P-03a to FR-P-03c, DEC-015 and DEC-098 were
read, and `docs/` grepped for "a value in each" and "value per unit in each".

### R-079 — The hunger deficit is written once per update, at the end of phase 7

**Date:** 2026-10-06   **Origin:** SQ-011
**Verdict:** specified
**docs/:** `SIM-REQ` FR-D-03a gains one sentence, §18's Needs row names the
write; `SIM-ECON` phases 1, 7, 8a and 12 cite FR-D-03a, phase 7 says what it writes,
§Demand "Hunger" says when it is written and who reads it. `SIM-ECON` revision
bumped; `SIM-REQ`'s already today's.

FR-D-03a defines the deficit and how it moves between rows, and no row of the
update table wrote it. Three places read it: phase 6 for the seed reserve
(FR-P-10d), phase 11 for mortality (FR-D-03), and FR-D-04a carries it with
emigrants.

**At the end of phase 7**, because that is the first moment the update's food is
known. Tier 0 is met in phase 6 from own holdings and in phase 7 by purchase; a
deficit written after phase 6 would count as hunger what the market was about to
feed, and one written in phase 11 would need the unmet need carried from 7 to 11
as a value outside state. Written at 7, mortality in phase 11 reads this update's
hunger, and the seed reserve in phase 6 reads the deficit as it stood after the
last update's market, one update behind. That lag is the right one: the reserve
is released when hunger has already accumulated, which is what P-66's threshold
means, and it cannot read a market that has not run.

**Before the market subsystem exists.** `R-051` builds needs before the market,
and `R-052` places FR-D-03a under needs. The rule is stated on phases, not on
subsystems, so it holds unchanged: while phase 7 does nothing, the tier 0 need
still unmet after phase 6 is what the end of phase 7 reads. The needs subsystem
builds the write in that slot, and the market lands before it. §18's Needs row
now names the write so `R-052`'s check reads it as needs' own.

**Movements between rows.** FR-D-03a's weighted average applies wherever members
change row: phase 1, where migrants join; phase 8a, where members enter and
leave the destitute row; and phase 12, where FR-J-03 realigns the share of
population in each mestiere to the vector. All three now cite it. Phase 11's emigrants carry the deficit
by FR-D-04a, which already says so.

**Cost.** None to code. Phase 7 gains a pass over rows after clearing, one
comparison and one write each.
**Owed by the implementer:** nothing beyond `docs/`.
**Would overturn it:** a sweep in which the one-update lag of the seed reserve
lets hunger kill before the reserve opens, which would argue for phase 6
reading this update's shortfall after own consumption as well.
**Not verified:** nothing; every row of the update table was read for a
movement of members, and FR-J-03, FR-J-19, FR-J-20 and FR-D-04a were read.

### R-080 — The seed reserve releases this update's unmet survival need, which holds the deficit at P-66

**Date:** 2026-10-06   **Origin:** SQ-012
**Verdict:** specified
**docs/:** `SIM-REQ` FR-P-10d; `SIM-DEC` DEC-092; `SIM-LOOPS` B5c; `SIM-ECON`
phase 6. `SIM-LOOPS` revision bumped; the others already today's.

FR-P-10d released the reserve "as far as needed to bring the deficit back to
P-66"; DEC-092 and B5c said "to hold it there". The finder is right that under
FR-D-03a the first has no quantity. The deficit grows by what is missing and
decays by P-70 only in an update whose need is met in full. No amount eaten in one
update brings a deficit above P-66 back to it; the most food can do is meet the
need, and then decay does the rest at its own rate.

**The quantity is the update's tier 0 need left unmet by the cohort's other
holdings**, converted to the planting good by its nutrition and capped by the
reserve. Released so, the need is met, the deficit decays by P-70, and once it
is at or below P-66 the reserve closes until hunger grows again. The deficit
therefore stays near P-66, which is what DEC-092 and B5c said and what B5c needs
to be a brake: seed is protected until desperation, then spent only to stop
desperation from deepening. "Bring back" is dropped; it described the effect over
several updates as if it were the quantity of one.

The deficit read in phase 6 is the one `R-079` writes at the end of phase 7,
one update behind; FR-P-10d says so, so the implementer does not look for a
fresher one that does not exist.

Releasing more — enough to make the deficit decay faster, or the whole reserve —
would spend next year's harvest to shorten this year's hunger by a margin P-70
already sets, and would weaken B5c without a sweep asking for it.

**Cost.** None to code. Phase 6 computes the unmet tier 0 need before it touches
the reserve, which it computes anyway to meet needs from holdings.
**Owed by the implementer:** nothing beyond `docs/`.
**Would overturn it:** a sweep in which a deficit held at P-66 still kills at a
rate the famine tuning cannot accept, which would argue for a lower P-66, not for
a larger release.
**Not verified:** how P-67's share of the release, zero in the shipping
configuration, interacts with a release sized to the need: a non-zero share
leaves part of the need unmet by construction. That is the forced sale DEC-092's
cost names, and P-71's citation is `SQ-022`'s item 13.

### R-081 — DEC-085's widths come from the logic or from the rules file, and content widths never change within a world

**Date:** 2026-10-06   **Origin:** SQ-013
**Verdict:** specified
**docs/:** `SIM-DEC` DEC-085, rationale and cost. Revision already today's.

DEC-085 predates DEC-100. Its rationale said widths come from "static data
versioned with `ruleVersion`", and its cost that a patch adding a good reshapes
the arrays. Under DEC-100 `ruleVersion` is the logic's version, static data come
from the rules file the world carries, and an existing world cannot take new
content. `R-065` listed the entries it touched and missed this one; it is the
use site of a rule updated and its definition left behind, the failure of
`SQ-003`.

The fix separates what DEC-100 separated. **A width the logic sets** — the
four denominations — changes with the logic and reshapes its arrays inside
NFR-08's migration, as DEC-085 said. **A width the content sets** — one stock per
good — is fixed for the life of the world, because the world's rules file is.
DEC-085's decision is untouched: values in contiguous arrays, nothing nested.

`SQ-023` may make per-good state sparse per settlement. If it does, the width of
the sparse field is a capacity or a pool under DEC-085's own second sentence, and
this ruling does not anticipate which.

**Cost.** None to code.
**Owed by the implementer:** nothing beyond `docs/`.
**Would overturn it:** a requirement to move an existing world to new content,
which DEC-100 refuses until one exists.
**Not verified:** nothing; DEC-085, DEC-100, DEC-033 and `SIM-STATE` §World were
read, and `docs/` grepped for `ruleVersion`.

### R-082 — Generation parameters are a second input beside the seed, and only what a host chooses per world

**Date:** 2026-10-06   **Origin:** SQ-014
**Verdict:** escalated — reserve 2 of `R-001`: the answer edits NFR-01 and AC-02
**docs/:** unchanged

Since `R-065`, NFR-01 and AC-02 name three inputs: seed, rules file, commands.
FR-G-02, NFR-08, DEC-032 and `SIM-STATE`'s `generationParams` keep a fourth, the
generation parameters, saved beside the seed. Two runs with the same seed, rules
file and commands but a different settlement count differ from tick 0, so AC-02
as written fails for any code the moment a host may choose a parameter. And no
text says which values are generation parameters and which belong to the rules
file: P-02, P-12, P-54 and the kingdom count of `R-063` could sit in either.

**Two answers.**

- **A. Generation parameters go into the rules file.** One input fewer, and
  NFR-01 and AC-02 stay as they are. But the rules file is what NFR-09's digest
  identifies as the rules: a world of 500 settlements and one of 2,000 would run
  under different rule versions with the same content and logic, and a sweep
  over world size would be a sweep over rule versions. The rules file describes
  how the world behaves; the size of the map is not a rule.
- **B. They stay a separate input, and the determinism texts name them.**
  NFR-01 and AC-02 read "the same seed and generation parameters, rules file and
  commands". FR-G-02 says what they are: the values a host may choose per world
  when creating it — settlement count (P-02), settlement mix (P-12), kingdom
  count. Every other value the generator reads, how sites follow terrain, the
  founders' endowment (P-54), initial stocks, is content and lives in the rules
  file. Each value then has one home, and the test of which is whether a player
  could pick it on a new-game screen.

**Recommendation: B.** It keeps NFR-09's rule version meaning what DEC-100 made
it mean, and it is the only answer that needs no change to what the save already
carries. The cost is the wording of NFR-01 and AC-02, which is why it is not
ruled here: adding an input to the premise of a determinism criterion is the
same kind of change `R-065` put to the user as reserve 2.

**Cost.** Under B: NFR-01 and AC-02 gain three words; FR-G-02 gains a sentence;
P-54 moves from "generator" to rules content in its description. The host gains
a second thing to pass at creation, which it already had to store.
**Owed by the implementer:** nothing until the user answers. The question
binds the generator's point (`R-052`), the first of phase 4.
**Would overturn it:** a host that must vary a rules-file value per world —
a difficulty setting that changes P-66, say — which would argue for a
per-world overlay of parameters, a third shape neither answer has.
**Not verified:** whether DEC-040's prior history reads any generation
parameter after tick 0; FR-W-10 fixes the settlement count, and nothing else was
checked.

### R-083 — FR-A-05 opens Phase 4, with the generator

**Date:** 2026-10-06   **Origin:** SQ-015
**Verdict:** specified
**docs/:** `SIM-REQ` §18, "Delivery phases": the Phase 4 table's Production row
and the line on what lies outside Phase 4. Revision already today's.

`R-052` sent "§13, what the host observes and does" to Phase 7, and `R-065` later
put FR-A-05, the rules file, in §13. `R-063` and `R-064` place every piece of phase
4's content in that file, and `R-065` owes its format from the first phase 4 point
that adds static data. So the requirement the first point must serve sat in a
phase three later, and `R-052`'s check of `Satisfies:` against §18 would have
flagged the point that does what `R-065` requires.

FR-A-05 is not host behaviour in the sense `R-052` meant. It is how content enters
a world, and no world exists without it: the generator reads terrains,
mestieri and recipes before it writes a row. It goes where the generator goes,
before production, and the line on Phase 7 names the exception. What the host
does with the file — ship it, read it from disk, offer a mod — stays the host's
and Phase 7's; Phase 4 builds the core's half, the format, validation and the
digest, and the harness's compiler.

**Cost.** None to code; Phase 4 already owed the work under `R-065`.
**Owed by the implementer:** nothing beyond `docs/`. `R-065` stands.
**Would overturn it:** none foreseen.
**Not verified:** nothing; §13, §18 and `R-052`, `R-063` to `R-065` were read.

### R-084 — `SIM-STATE` lists every carry: one per good for holdings, one per building type, one for interest, one per good on each link

**Date:** 2026-10-06   **Origin:** SQ-016
**Verdict:** specified
**docs/:** `SIM-STATE` §Cohort gains `goodsCarry`, `buildingsCarry`,
`interestCarry` and a paragraph stating the rule, `durables` names its wear as
the carry; §Trade link gains `lossCarry`. `SIM-ECON` "Spoilage and integer
rounding" says where transit loss carries. `SIM-STATE` revision bumped,
`SIM-ECON`'s already today's.

`SIM-ECON` makes the remainder accumulator a rule of the codebase and names five
rates it applies to; `AGENTS.md` repeats it as the first rule the build cannot
catch. `SIM-STATE` §Rule says anything that influences a future tick is listed,
and §Cohort listed one carry, the destitution share's. Every other carry would
have been a field the implementer added outside the inventory, which is the
determinism hole §Serialisation notes names, and which this file's reader could
not check for.

**One carry per good for holdings, not one per rate.** Production and harvest add
fractions to a stock, spoilage takes them away, and all three act on the same
holding. A carry is the fraction of a unit the holding has beyond its integer
count, in [0, 1); each rate adds to it or takes from it, and a whole unit moves
between carry and stock when reached. Separate carries per rate on one stock
would each be right and together would let a stock hold two fractions of the same
unit. One carry per holding conserves the good exactly, which is what A-04 asks.
It is laid out as `goods` is, so whatever `SQ-023` decides for the layout of
holdings, the carry follows.

**The others.** Building decay at P-51 acts on a count per building type, so a
carry per building type. Wear at P-53 already had its place: `durables` carries
"accumulated wear", which is the carry, and now says so. Interest acts on one
debt per row, so one carry. **Transit loss is the case `SIM-ECON`'s "stays with
its row" did not fit**: a shipment lands once, so a carry on the transient
vanishes with it and a small shipment never loses anything. It is held on the
link, per good, as `flowAccumulator` already is, and cleared when the link is
evicted.

**The rule stated once.** A paragraph under §Cohort's table says every rate on an
integer field of a row keeps a carry beside it, in its layout, and that a rate
added later brings its carry into the table. The list is what the update names
today; taxation on cohort currency and any rate a later phase adds are bound by
the paragraph and enter the table with the point that builds them.

**Cost.** Four fields, two of them as wide as the goods or the building types: the
holdings' carry doubles the per-good state of a cohort, which is the figure
`SQ-023` already counts ("twice that with a carry per good"). Every carry is
hashed and saved.
**Owed by the implementer:** nothing beyond `docs/`; each carry lands with the
point that first applies its rate.
**Would overturn it:** `SQ-023`'s measurement showing the per-good carry too
costly at the catalogue's size, which would argue for holdings in fixed point
with no separate carry, a change to the type of `goods`.
**Not verified:** whether the advance of FR-C-25 accrues interest, which would
need a carry of its own; FR-C-25 was not read for it. That interest whole units
reach the lender's credit is `SQ-018`'s territory and is not stated here.

### R-085 — The yearly cap is P-73, applied per adjustment, and needs no state

**Date:** 2026-10-06   **Origin:** SQ-017
**Verdict:** specified
**docs/:** `SIM-REQ` FR-P-15 and §16, P-73 new; `SIM-DEC` DEC-096. Revisions
already today's.

FR-P-15 and DEC-096 made "P-19 and the yearly cap" the default for an activity
whose record carries neither. The cap had no P-number, which §16 calls a defect,
and enforcing a cap on the year's movement needed the movement already made this
year per activity, which no table held, nor anything that reset it.

**The number.** P-73, the default yearly cap, Open and set by sweep, as P-19 is.

**The state.** Two answers: a field per settlement and activity holding the
year's movement, with a reset at the year's start; or the cap applied at each
adjustment, divided by the number of adjustments the activity makes in a year —
one per sowing for a crop, 52 at settlement cadence. The second bounds the year's
total exactly as the first does, since no year holds more adjustments than the
divisor counts, and it needs no field, no reset and no dated step. What it gives
up is the freedom to spend a whole year's cap in one step after a shock. That
freedom is what FR-P-16 prohibits in spirit — a partial, damped adjustment — so
losing it costs nothing the specification wants. For a crop, adjusted once a
year, the two are identical.

**Cost.** For non-agricultural activities the response to a shock is spread
over the year rather than taken at once; a sweep of P-73 sees that as a slower
response than the same number would give under the first answer.
**Owed by the implementer:** nothing beyond `docs/`; P-73 cited by the point that
builds allocation adjustment (`R-050`).
**Would overturn it:** a requirement that an activity respond to a shock within
the year by more than an even share of its cap; the day labourer, which needs
that, already carries no cap.
**Not verified:** nothing; FR-P-14 to FR-P-16, FR-J-21, DEC-047, DEC-096 and
§16 were read.

### R-086 — An advance is a loan like any other: the crop is collateral, the lender holds the claim, interest runs

**Date:** 2026-10-06   **Origin:** SQ-018
**Verdict:** specified
**docs/:** `SIM-REQ` FR-C-24 and FR-C-25; `SIM-STATE` §Cohort, `advance` and
`interestCarry`. Revisions already today's.

FR-C-25 calls an advance "a loan collateralised by that output", and then
departs from what every other loan has. FR-C-24 lists collateral as goods,
durables and buildings, so a cohort holding only a standing crop could not take
the advance FR-C-25 offers it. `advance` was a bare `int64`, while FR-C-23 gives
debt and credit a counterparty category. And FR-C-27 puts a write-off's loss "on
the lender" with nothing saying who that is.

The answer is in FR-C-25's own first sentence: it is a loan. FR-C-22 says a loan
transfers currency that exists, so someone's currency fell when the advance was
paid, and their claim has to be somewhere or a write-off has nothing to fall on
and a repayment has nowhere to go. The lender's credit stock is where FR-C-23 puts
every claim a cohort holds. So:

- **FR-C-24 lists the standing crop**, for an advance only. It does not become
  collateral for ordinary debt: FR-C-25 binds it to the crop's own output, and
  pledging a crop twice would need a rule for which claim it serves first.
- **`advance` carries the counterparty's category**, as debt does, and the
  lender holds the matching claim in its `credit`. A write-off falls on it by the
  same path as any loan's under FR-C-27, whatever that path turns out to be.
- **It bears interest under FR-C-26.** A loan with no price for time would make
  the advance the cheapest credit in the world for anyone with a crop.
  `interestCarry` widens to two, one for debt and one for the advance, under
  `R-084`'s rule; the two balances are kept apart, so their fractions are.

How a write-off held by category reaches a particular lender's credit is not this
entry's question. It is FR-C-27's for every loan, and the credit point decides it.

**Cost.** None to code. The credit point builds the advance as a second balance
on the machinery of the first.
**Owed by the implementer:** nothing beyond `docs/`.
**Would overturn it:** content in which the advance is paid by the settlement
or a guild rather than a cohort, which would need a lender outside the cohort
table and a claim held there.
**Not verified:** how FR-C-27's write-off distributes a loss among lenders that
share a category; FR-C-23 to FR-C-30 were read and none says.

### R-087 — `SIM-LOOPS`' tables and gate check are brought back to agreement

**Date:** 2026-10-06   **Origin:** SQ-019
**Verdict:** specified
**docs/:** `SIM-LOOPS` §Stocks: Population, Goods, Currency and Debt rows, and a
Standing crop row; B2's and R12's tuning lines; two rows of §Gate check.
Revision already today's.

The finder is right on each count. The poverty and seed batch added flows the
stock table did not show, R12 arrived with no tuning line, and the gate check
counted loops by a list older than three of them. The user conditioned the batch
on the phase 1 gate staying closed, so a status reading "Met" over tables that
do not support it is the claim to fix, not to leave.

- **§Stocks.** The standing crop is a stock: sowing puts the planting good into
  it, harvest takes it out, and so does loss before harvest or when the window
  closes (FR-P-10a, FR-P-10b, FR-P-10e, FR-C-25). Goods gain sowing, seizure and
  emigrants among outflows, and arriving migrants and seizure received among
  inflows; goods in transit are held by transients. Population and currency
  gain migrants in transit (FR-D-04a). Debt gains advances (`R-086`). Every row
  has a source and a sink, so the first row of the check stays "Met".
- **Tuning.** B2 is tuned by P-73 as well since `R-085`. R12's brakes are tuned
  by P-68 and P-69, which set who enters and leaves the destitute row, by the day
  labourer's own rate for B12a and by P-46 for B12b. B7 has no parameter: it is
  the order of phases 6 and 7, and there is nothing to tune. The check now says
  so instead of reading "Met" over it.
- **The count.** R9, R10 and R11 have named brakes and were missing from the
  list; R3 has its own row; R8 balances itself and terminates, as its section
  says.

Nothing changes in what the gate asks. Every status still reads "Met"; one now
names the brake it does not apply to.

**Cost.** None.
**Owed by the implementer:** nothing.
**Would overturn it:** a reading of "brakes have a tuning parameter" as a
requirement on every brake, which would make B7 a defect; that would be a change
to the gate, reserve 2, and the user's.
**Not verified:** that no other flow of `docs/` is missing from §Stocks; the
batch's flows were checked, the older ones were taken as they stood.

### R-088 — `R-065` revised by the user's answer to `SQ-020`: base rules in the core, a file replaces them, validation never fails

**Date:** 2026-10-06   **Origin:** SQ-020, the user's answer of 2026-10-05
**Verdict:** specified — the user's decision on §20, reserve 4 of `R-001`; the
sentence on AC-04 is the user's, reserve 2
**docs/:** `SIM-DEC` DEC-100, title, decision, rationale and cost; `SIM-REQ`
FR-A-05, AC-04 one sentence, §16 P-74 to P-76 new; `SIM-STATE` §Static data.
Revisions already today's.

`R-065` left unverified whether the consumer of §20 can ship and read a rules
file. Only the user could answer, and did: a hybrid. The base rules are compiled
into the core, as bytes written by the same compiler from the same text source;
an external file, where the game can read one, replaces them. The rest of
`R-065` stands, and the user named three consequences for the decider to write.
This ruling transcribes them; it decides only their wording and the numbers.

- **The default.** FR-A-05 and DEC-100 say a world runs on the base rules when
  the host hands no file, and that a file replaces them whole. Both go through
  the same validation, digest and save, so a world on the base rules is no
  different in kind from one on a file; `R-004`'s point, as `R-065` kept it,
  survives — the core computes the digest of whatever bytes it runs on.
- **Validation that never fails.** Players will write files. A malformed one is
  refused by a result the host receives, never by an exception or a crash of the
  core. That is a property of the validator, stated in FR-A-05, and the reason
  the cost line of DEC-100 now says validation must hold against files the
  repository's compiler did not write.
- **Caps.** A file may not declare more goods, mestieri or recipes than P-74,
  P-75 and P-76, Open, set after the Phase 4 measurement of P-17, so that no file
  takes the core past P-01 without anyone noticing. Three numbers, not one,
  because the three grow different costs: goods widen holdings, mestieri add rows,
  recipes add production work.
- **AC-04.** Its bounds are a claim about the base rules; it says it makes no
  claim about a world on another file. The user asked for this sentence. It
  narrows the domain of an acceptance criterion, which is reserve 2, and is
  applied as theirs.

`SQ-023` bears on the caps: if state becomes sparse per settlement, the cost that
P-74 guards is the goods present in a place, not the catalogue, and the cap may be
looser. That is for the ruling on `SQ-023`, after the measurement.

**Cost.** The base rules are a build output: the build regenerates them when the
source changes, and a check that the bytes in the core match the source is a
natural test for the point that builds them. Three more parameters.
**Owed by the implementer:** `R-065`'s action, widened as noted under "Owed by
the implementer". Cite `R-065` and `R-088`.
**Would overturn it:** the user revising the hybrid; a platform that can read a
file but must not accept mods, which would need a way to refuse a valid file.
**Not verified:** how the base rules reach the core as bytes — a generated source
file, an embedded resource — which `BannedSymbols.txt` and §20 constrain and the
implementer chooses.

### R-089 — Age bands are content, each marking whether it works; the base values await the user

**Date:** 2026-10-06   **Origin:** SQ-021
**Verdict:** specified; the base values escalated — static content is the
user's under `R-050`
**docs/:** `SIM-STATE` §Cohort `ageDistribution`, open item 1, and open item 6
new; `SIM-REQ`
FR-C-05 gains one sentence. Revisions already today's.

`R-063` approved "Age bands. Three, closing `SIM-STATE` open item 1." The text
that `R-059`'s transcription put into `docs/` said more: the names children,
working age and elderly, that only working age offers labour, and that a wider
split is a format migration. No ruling decided the second and third, and the
finder is right that the labour rule is not a label: it decides who sells labour
under FR-C-05, and so the dependency ratio of every settlement.

**The structure is specified here.** Age bands are content, as `R-063` made them,
and each band carries whether its members offer labour. FR-C-05 now says labour
offered is the members of the bands so marked. Stating it per band keeps the
rule in the rules file with the bands themselves: a content author who splits
working age in two, or lets the elderly work, changes data, not code, which is
what DEC-100 is for.

**The remark on migration was wrong** after `R-081`. A band count set by content
is a width the content sets, fixed for the life of a world; a wider split is new
content, not a migration of an existing world's format. The item now says so.

**The base values are not the decider's.** Three bands named children, working
age and elderly, with working age alone offering labour, is a plausible base and
the one the text already held, and the decider recommends it: it gives the
model a dependency ratio, which is how age structure reaches the economy, and
leaves child and elderly work to content that wants it. But values in the rules
file are static content, approved by the user before the plan that uses them
(`R-050`). They stand in `docs/` as the base, marked under "Escalated to the
user" until approved.

**Cost.** One flag per band in the rules file. A world where nobody but working
age works feels a demographic shock in its labour supply a generation later,
which is the point of a dependency ratio and a tuning question for Phase 6.
**Owed by the implementer:** nothing beyond `docs/` once the user approves; the
point that builds cohort rows reads the flags.
**Would overturn it:** the user approving other values, which changes content
only.
**A gap found beside it.** Nothing in `docs/` says how members move from one
band to the next; a grep for ageing finds only the knowledge table's. It is
recorded as `SIM-STATE` open item 6, due with the demography subsystem, rather
than decided here: the rate per band is a parameter and its shape follows from
how births and deaths are written.

**Not verified:** nothing further; FR-D-01 to FR-D-03, FR-D-13 and FR-C-05 were
read.

### R-090 — The fourteen leftovers of the batch are corrected; two are decided on the way

**Date:** 2026-10-06   **Origin:** SQ-022
**Verdict:** specified
**docs/:** `SIM-REQ` FR-P-01, FR-P-10d, FR-C-04, FR-C-29, FR-A-05, §16 P-17,
P-17a, P-63, P-64, §18 Phase 4, §19 P-17 and P-01; `SIM-DEC` DEC-015, DEC-047
title, DEC-052, DEC-076, DEC-092, DEC-041's cost (P-17's figure); `SIM-ECON`
phases 4 and 7, the allocation step, §Demand; `SIM-STATE` §Static data.
Revisions already today's.

Item by item, as the entry numbers them.

1. **Phase counts.** The update has phases 1 to 12 and 8a; §18 and §19 now say
   "all its phases". The measured figures keep "5 of 12": that was the update
   they were taken on, and DEC-041's cost line now says so. §19's unmeasured
   phases are eight since 8a.
2. **9 and 27 cohorts.** The four rows record measurements, and are relabelled
   "measured on 9 rows" and "27 rows" rather than restated at 12 and 36: §19
   already says the real measurement counts 12 and 36.
3. **DEC-015** starts with "the goods of P-09", which cannot go stale again.
4. **DEC-052**: one mechanism, damped at each activity's rate (DEC-096).
5. **DEC-047's title**: "at each activity's adjustment".
6. **The allocation step** adjusts at sowing for a crop's activity and at
   settlement cadence for the others, as FR-P-15 says.
7. **Implements.** Phase 4 cites FR-P-10e, phase 7 FR-P-10d and FR-J-22, and
   §18's Production row lists phase 12 for FR-J-03's realignment.
8. **A crop yields one good — decided.** DEC-092 let a crop whose seed is not food
   yield its seed "alongside its harvest"; FR-P-10 and FR-P-10b give a crop one
   yield. One yield wins: the content of `R-063` has one crop sown with its own
   harvest, and a co-yield would add a second output to the crop record, the
   harvest, the carry of `R-084` and the conservation of FR-P-03c for a case no
   content holds. A crop whose seed is not its harvest names a planting good
   obtained as any good is, by a recipe or by trade.
9. **FR-P-01 and DEC-015** say "whether it is durable", not "role" (`R-056`).
10. **`rulesData`.** §Static data says the content is not state and the bytes
    are, as World's `rulesData`, immutable.
11. **FR-A-05** refuses a file "this build cannot read or whose content fails
    its validation", which is what DEC-089 asks of a save, and no longer admits
    a file only a later build could read.
12. **Rent — decided.** FR-C-29 said rent "reopens when the wealth axis is
    added", and DEC-054 added it long ago, so read literally rent was already
    in scope. It is not: no requirement, phase or content carries it, and
    `SIM-ECON`'s "does not cover" item 1 still treats rent as a future change.
    The condition was a pointer to what would carry rent, written before the
    axis existed. Both texts now say that: the wealth axis is what would carry
    rent and tenancy if they enter the model. Bringing them in is a scope
    decision that belongs with the user and a phase, not with a wording fix.
13. **P-71** is cited by FR-P-10d and DEC-092 as the CI share of the release.
14. **"Per good".** FR-C-04 keeps a pass per good, in DEC-099's order within a
    need; §Demand's quantity is per cohort per need. Whether the pass is per good
    or per clearing is `SQ-023`'s, after the measurement.

**Cost.** None to code.
**Owed by the implementer:** nothing beyond `docs/`.
**Would overturn it:** for item 8, content needing a crop with two outputs; for
item 12, the user bringing rent into scope.
**Not verified:** that every other mention of the old counts is gone; `docs/`
was grepped for "twelve", "seven goods", "once per sowing", "role" in a good's
record and "no build could", and what remains is historical and says so.

### R-091 — `R-082` accepted: generation parameters are a second input, named in NFR-01 and AC-02

**Date:** 2026-10-07   **Origin:** user, on `R-082`
**Verdict:** specified — the user's decision, reserve 2 of `R-001`
**docs/:** `SIM-REQ` NFR-01, AC-02, FR-G-02, §16 P-54's description; `SIM-STATE`
§World `generationParams`. Both revisions bumped.

The user approved `R-082`'s recommendation and asked for it to be applied:
"approvo R-082 e R-089, applicali". Answer B, as `R-082` wrote it.

- **NFR-01 and AC-02** name the seed and generation parameters together, beside
  the rules file and the commands. The criterion now lists every input a run is
  given, so correct code can pass it when a host chooses a world's size.
- **FR-G-02** defines generation parameters as the values a host may choose for
  each world: settlement count (P-02), settlement mix (P-12), kingdom count.
  Every other value the generator reads is content in the rules file. The
  kingdom count has no P-number yet; `R-063` left it Provisional, and the
  generator's point proposes one under `R-050`.
- **P-54**, the founders' endowment, is described as rules content read by the
  generator, which is the home FR-G-02 now gives it.
- **`generationParams`** in `SIM-STATE` says what it holds.

The rule version of NFR-09 keeps meaning what DEC-100 made it mean: two worlds of
different size on the same content and logic run under the same rule version.

**Cost.** As `R-082`: the host passes a second input at creation, which it
already had to store. The pins of `R-034` and `R-036` do not move: phase 3's
worlds have no generation parameter beyond the one `D-062` kept.
**Owed by the implementer:** nothing beyond `docs/`; the generator's point reads
FR-G-02 as it now stands.
**Would overturn it:** as `R-082`: a per-world choice of a rules-file value.
**Not verified:** as `R-082`: whether prior history reads a generation parameter
after tick 0.

### R-092 — `R-089` accepted: the base rules hold three age bands, and only working age offers labour

**Date:** 2026-10-07   **Origin:** user, on `R-089`
**Verdict:** resolved — the user's approval of static content under `R-050`
**docs/:** unchanged; `SIM-STATE` open item 1 already states the values.

The user approved `R-089`'s recommendation: "approvo R-082 e R-089, applicali".
The base rules hold three age bands, children, working age and elderly, and only
working age offers labour. `SIM-STATE` open item 1 carried these values since
`R-089`, marked as awaiting approval only here; they now stand as approved
content, beside the rest of `R-063`. As every number of `R-063`, the bands are
structure, and the rates that move members between them are open item 6.

**Cost.** None beyond `R-089`'s.
**Owed by the implementer:** the point that builds cohort rows writes the three
bands and their labour flags into the rules file of `R-065`, as it writes the
rest of `R-063`. Cite `R-089` and `R-092`.
**Would overturn it:** the user revising the content before that point.
**Not verified:** nothing.

### R-093 — `D-114` is ratified; the watermark reaches `D-114`

**Date:** 2026-10-07   **Origin:** register audit, `D-114`
**Verdict:** ratified; one recommendation escalated — reserve 1 of `R-001`: point
8's content is the user's (`R-070`)
**docs/:** unchanged

Point 4 of the process plan, the six agent definitions. Process, so ratified or
rejected, never promoted (`D-011`). The entry was read against point 4's frozen
text and the design's "The roles", "What the coder may not write",
"Enforcement", "The tester, in two dispatches" and "Where the rules live", and
its claims against `1ed624a`, the six files and the diff of `reviewers.md` read
whole.

- **The frontmatter.** Checked by hand against the roles table, since the
  entry's script was not committed: model, effort and tools agree for all six,
  each `description` says "Dispatched by /plan-next only", no reviewer holds
  `Edit` or `Write`, and the three `path-guard` roles and the committer's
  `git-guard` carry the matchers and commands the entry states.
- **No hook on the reviewers; `Write` in the documenter's matcher.** Stand. A
  hook on `Edit|Write` sees nothing an agent without those tools can do, and
  what `Bash` leaves is the orchestrator's check, as the design says. The extra
  matcher costs nothing and guards a tool list widened by mistake.
- **The code-reviewer reads `AGENTS.md` whole.** Stands. Point 5 writes the
  section the design points at, and point 5's own check runs the code-reviewer
  against C1 and C5, so reading the whole file reaches the section the day it
  exists. Its body asks which fault the check would miss, the code-reviewer's
  half of `R-047`, now marked under "Owed by the implementer".
- **The five rules moved, a pointer left in `reviewers.md`.** Stands. Rules 1 to
  4 are verbatim and rule 5 loses only its heading. Until point 7, `/plan-next`
  dispatches agent types that do not read `revisioner.md`, so the sentence
  telling the brief to copy them keeps the rules reaching a reviewer. No other
  file names the old headings; grepped.
- **The bodies add no step.** Checked against the design: the empty
  implementation in the tester's second dispatch is the design's (`D-065`,
  `D-070`); the documenter's STOP cases are `/plan-next` step 6's; the
  committer's comparison of the staged set is "Enforcement" item 2.
- **The check left outside the repository.** Accepted, as a cost. `AGENTS.md`
  asks a point for a runnable check, and this one ran once and went with the
  session's scratchpad; a later edit to an agent file meets no check. The
  comparison is six frontmatter blocks against six rows, and this audit redid it
  in minutes, so a script is not worth a home that the design reserves for
  hooks. The first real run, `R-046`, is the check that matters.

**The reserve of point 4.** This session started after `1ed624a` and lists the six
types among its agents, with the tool sets above: the definitions load in a fresh
session, which is half of what the reserve waited for. Whether the coder's hook
fires on a write to `docs/probe.md` was not probed here: the probe is the
implementer's, under `R-011`, and the definitions say they are dispatched by
`/plan-next` only. The reserve stays open until the implementer runs it and
writes the *Riserva sciolta* line.

**A finding, escalated.** `coder.md` restates its denylist in prose, a third copy
beside the design and `path-guard`. Point 8 (`R-070`) adds the two settings
files to the other two copies and does not name this one, so after point 8 the
coder's own instructions would list less than its hook denies. The hook is what
enforces, so nothing is breached; but the body is what the coder reads.
**Recommendation:** point 8 also names `.claude/agents/coder.md` and adds the
settings files to its list. The content of point 8 was approved by the user item
by item (`R-070`, `R-071`, `R-073`, `R-074`), so a seventh change is theirs to
approve; until then point 8 carries the six.

**The watermark advances to `D-114`.**

**Cost.** None beyond the check that cannot be rerun.
**Owed by the implementer:** nothing new; the code-reviewer's half of `R-047` is
marked done.
**Would overturn it:** as for `R-008`; the probe showing the hook does not fire
or `$CLAUDE_PROJECT_DIR` does not expand, which the entry names and which would
change every `command:` line.
**Not verified:** that the hooks fire for a subagent, that `effort` is honoured,
and `disableAllHooks` (`R-068`); none was probed. The `command:` lines were not
rerun.

### R-094 — `R-093` accepted: point 8 also updates `coder.md`'s list, a seventh item

**Date:** 2026-10-07   **Origin:** user, on `R-093`
**Verdict:** resolved — the user's decision, reserve 1 of `R-001`
**docs/:** unchanged

The user approved `R-093`'s recommendation and asked for it to be applied:
"approvo la raccomandazione di R-093, applicala". The change is to
`.claude/agents/coder.md`, which a decider session does not write, so applying it
means owing it to point 8, as `R-071`, `R-073` and `R-074` did. This ruling
records the user's approval that `D-014` requires for the addition.

**The seventh item.** `.claude/agents/coder.md`, where its body lists the paths
the coder may not write, adds `.claude/settings.json` and
`.claude/settings.local.json`, so that its prose matches the denylist point 8
gives the design and `path-guard`. Point 8 names the file in its own text: the
design reserves `.claude/agents/` to the orchestrator "in a point that names
them", and point 8 is run by the orchestrator, before point 7 (`R-070`).

The alternative, replacing the list with a pointer to `path-guard`, would leave
one home for the rule, as the design prefers. It was not what the user approved,
and a coder that reads its limits in its own instructions is told them before the
hook refuses it; the list stays, kept in step.

**Its `Fails when:`**, proposed here and not yet read by the user (`R-047`):
after point 8, `coder.md` names no settings file while `path-guard` denies them.

**Cost.** One more item in point 8, and three copies of the denylist to keep in
step instead of two, until a point decides the body should point rather than list.
**Owed by the implementer:** point 8, as `R-070` owes it, carries this seventh item
and its `Fails when:` line, and names `.claude/agents/coder.md`. Cite `R-070` and
`R-094`.
**Would overturn it:** the user preferring the pointer to the list.
**Not verified:** nothing; `coder.md` was grepped and names no settings file today.

**`Fails when:` read by the user, 2026-10-07:** "approvo la condizione di
fallimento di R-094". The line stands as stated above, and point 8 carries it
verbatim.

### R-095 — `D-115` is ratified; point 8 owes a self-test of `mutate`'s exit 3 beside its criterion

**Date:** 2026-10-07   **Origin:** register audit, `D-115`
**Verdict:** ratified; one action owed
**docs/:** unchanged

Point 8 as written into `PLAN.md` by `02293d6`, owed by `R-070`. Process, so
ratified or rejected, never promoted (`D-011`). The point was read against
`R-068`, `R-070`, `R-071`, `R-073`, `R-074` and `R-094`, and the commit's diff
read whole: it appends point 8 between points 6 and 7 and the entry, and
changes nothing above.

- **The seven items** are the rulings' own, in their order, each citing its
  ruling. Item 3 restoring `R-068`'s self-test case for a lowercase `a-`, and item
  4 carrying `R-071`'s `mutate` exit 3 from its cost paragraph, are both what the
  rulings owe.
- **The `Fails when:` lines** are verbatim, the eight faults the user read.
- **The placement.** `plan-state` reports the points in file order, 8 before 7,
  and `next: 5`; `R-070`'s reading holds.
- **`Closed by:` is the implementer's.** No ruling stated one, and a criterion
  written by the point's author is the rule, not an exception. Each named fault
  turns it red: the five self-tests carry the cases of items 1 and 3 to 6, and the
  greps catch items 2 and 7.

**One gap, under `R-005`'s case 1.** Item 4 asks `mutate` to stop with exit 3 when
either call to `tree-state` fails. That is the half of `R-071` that matters: a
`tree-state` that refuses to hash is only safe if its caller does not read two
empty answers as "returned". No `Fails when:` names it, and the `Closed by:` asks
`mutate`'s self-test for "the cases of items 1 and 3 to 6" without saying that
item 4 gives `mutate` one. A `mutate` that ignores the failure passes the point as
written. The criterion is frozen and is not rewritten; correct code passes it, so
the point is not `FAILED`. The case goes beside it.

**Cost.** One more self-test case in `mutate`, and one more fault introduced.
**Owed by the implementer:** within point 8, a self-test case of `mutate` in which
a call to `tree-state` fails and `mutate` exits 3, with the fault of `mutate`
ignoring the failure seen red; the outcome line names it as the check beside the
criterion. Cite `R-095`.
**Would overturn it:** reading "the cases of items 1 and 3 to 6" as already
including `mutate`'s exit 3, which the implementer may show by the case being
there; the action is then met by the point itself.
**Not verified:** the live probe of point 4's reserve. The coder's `command:` line
was run by hand, through `bash` with `CLAUDE_PROJECT_DIR` set to the Windows path
and Windows paths in the JSON: `docs/probe.md` and `core/Tests/` denied with exit
2, `core/Runtime/`, `harness/` and `.claude/settings.json` allowed, the last as
point 8 expects to change. Whether Claude Code fires the hook for a subagent is
still unproven.

### R-096 — A moment-one request holds its draft under `## Draft plan`, and the current one is brought to it

**Date:** 2026-10-07   **Origin:** D-117 (a finding for the decider)
**Verdict:** resolved; one action owed
**docs/:** unchanged

`D-117` found that `plan-state -Draft` cannot read the cost measurement plan's
request. Checked in this session: run on `REVIEW-REQUEST.md` as it stands, the
script prints `exists: false`. The draft is there, in a fenced block, under
`## The draft plan`; the script looks for the exact line `## Draft plan`, which
point 3's frozen text names. The request was written on `design/poverty-seed`
on 2026-10-05, before `plan-state` existed, and nothing told its writer the
heading: `R-048` and the rule `AGENTS.md` now carries say "a draft inside the
review request" and name none, and "What a request holds" at the top of
`REVIEW-REQUEST.md` lists six parts and no draft. The heading is a contract that
lives only in its reader.

**The request conforms; the reader does not widen.** A parser that accepts
variants of a heading is reading prose, and the next variant is the next miss.
One exact line is something a self-test can pin, and point 3's text already
fixes it, so a `plan-state` matching anything else would depart from its own
frozen "Does". Renaming one heading in a request changes nothing the user reads.

**And the writer is told.** The defect is not the one heading but that the next
moment-one request would be written the same way. The place a writer of a
request reads is "What a request holds"; one sentence there, under the six
parts, closes it: a request at moment one carries the draft whole in one fenced
block under `## Draft plan`, which `plan-state -Draft` reads.

**Timing.** The user has not yet answered this request (item 1), and once the
plan is installed `-Draft` has nothing to read. But `/plan-explain` reads a
draft through `-Draft` today (point 6), and is the skill a user would run on a
draft before answering. So the heading moves before that, not after.

**Cost.** One heading and one sentence. The convention binds every later
request, and a request written by hand must match it exactly.
**Owed by the implementer:** the current request's draft sits under
`## Draft plan`, so that `plan-state -Draft` reports it, before the draft is
next read through the script and no later than the commit installing that plan;
and "What a request holds" in `REVIEW-REQUEST.md` states that a moment-one
request carries its draft in one fenced block under that heading. Neither is
trivial under `AGENTS.md`, since this ruling decided it. Cite `R-096`.
**Would overturn it:** a second reader of drafts that needs another shape, which
would argue for writing the format once, in the plan design note, and pointing
both at it.
**Not verified:** that nothing outside `.claude/` reads `## The draft plan`; the
skills and hooks were grepped for "Draft plan", and only `plan-state` and
`/plan-explain` name it, both as `## Draft plan`.

### R-097 — `D-116` and `D-117` are ratified; the watermark reaches `D-117`

**Date:** 2026-10-07   **Origin:** register audit, `D-116` and `D-117`
**Verdict:** ratified
**docs/:** unchanged

Points 5 and 6 of the process plan. Process, so ratified or rejected, never
promoted (`D-011`). Each entry was read against its point's frozen text and the
rulings it applies, and its claims against `58d8557` and `e8f7590`.

- **`D-116`, point 5.** Stands. Checked: the permission for `AGENTS.md` is
  recorded as `R-010` requires, and the diff of `AGENTS.md` changes the "Criteria
  are frozen" bullet and adds the three sections, nothing else; the markers
  `FAILED` and *with reserve* replace the Italian ones there. A grep of
  `AGENTS.md` and the three skills finds the Italian markers only in "Language"'s
  table and in each skill's recognising sentence, five forms in `/plan-next`,
  four in the other two. `/plan-next` is 2,793 words, as the entry says.
  - **The first defect is `R-005`'s case 1, and the reading by identifier is
    the right one.** The section's effect on a reviewer is that its findings
    name the rule they apply; on a fixture whose defects other parts of
    `AGENTS.md` already reach, that is the only effect a check can see. So the
    entry's "would overturn it" does not arise.
  - **The second is case 1 too**, and the check beside it is the right one: a
    grep for markers present cannot see a marker gone.
  - **Point 8 owes a code-reviewer** dispatched by hand, as the entry says: it
    changes PowerShell scripts, and "Review and self-governance" applies from
    `58d8557`.
  - **The 38 words** are point 7's to recover, as the entry says.
- **`D-117`, point 6.** Stands. Checked: `e8f7590` touches the two skills, the
  register and `PLAN.md`. Reading "this request's draft" as the process plan's
  own request, from `main`, is the only reading under which the point could be
  checked as written, since the request on this branch cannot be read by
  `plan-state` (`R-096`).
  - **The session running `plan-state`, a `tree-state` difference stopping the
    skill, "guarantee" dropped with its sibling, the range sentence kept for
    C1.** Stand.
  - **The second fault green on the first injection, red on the second.** Not
    a defect of the criterion: the first change did not produce the fault
    named, since the agent still reported "stopped"; the second did, and the
    check caught it.
  - **The two findings.** The first is `R-096`'s. The second, a check as strong
    as the sentence the agent obeys, is a property of every agent-run check
    and is accepted as such.

`R-045` and `R-048` are marked done under "Owed by the implementer".

**Cost.** None beyond `R-096`.
**Owed by the implementer:** nothing new.
**Would overturn it:** as for `R-008`, any entry found to contradict `docs/` or
the design as it stood when written.
**Not verified:** the agents' runs and the faults seen red were taken from the
entries; the code-reviewer's dispatch on the fixtures was not repeated. The
citations of C1, C2, C4 and C6 to C9 and S1 remain unread, as `R-028` left them.

### R-098 — Point 4's reserve stays open: a red probe discharges nothing, and workspace trust is the first suspect

**Date:** 2026-10-07   **Origin:** D-119 (the line under point 4, left to the human)
**Verdict:** resolved; one action owed
**docs/:** unchanged

`D-119` ran point 4's live probe in a session that sees the six agents: `coder`,
asked to write `docs/probe.md`, wrote it, with no hook message. The same
`command:` run by hand on the same input exits 2, so the hook did not fire.
The entry leaves "the line under point 4" to the human.

**`R-011` already answers which line: none.** A reserve closes a point whose
work is done but for an action only a human can take, and it is discharged
when the clause's check, run after that action, passes. This check failed.
So no *Reserve discharged* line can be written, the reserve stays open, and
the plan's exit condition, which names "the six agents", stays unmet. `R-011`
gives an open reserve exactly this job: to keep a plan from reading as done
while a clause is not. None of this is a new rule, so reserve 1 of `R-001`
does not apply.

**What the human action is.** Point 4 took it to be a session started after
`1ed624a`. The documentation names another. The subagents page
(code.claude.com/docs/en/sub-agents.md, read through a guide agent, not
opened directly) says frontmatter hooks of a project agent need the workspace
trust dialog accepted for the folder. Without it the agent runs and its hooks
are skipped, with the error only in the debug log. This has been so since
v2.1.218, and this session runs 2.1.283. `~/.claude.json` holds this project
under two keys: `F:\fantasy-socioeconomic-simulator` with trust accepted, and
`F:/fantasy-socioeconomic-simulator` without. If the session that ran the
probe was keyed by the second, it explains the result exactly: the agent
runs, the hook is silent. Accepting trust is a security setting and the
user's alone, so the reserve is still the right status. The missing action
is the human's, not work the agent could do.

**If the probe stays red with trust accepted,** that is no longer a reserve:
the clause fails for a reason the agent must find and fix, which `R-011` says
a reserve is not for. Then it is reported in the register as a finding. A
point to fix it is added only with the user's approval under `D-014`, and
whether the workflow may run with the hashes of step 3 as its only barrier is
governance, the user's.

**What waits.** Under `R-048`, `main` carries no live `PLAN.md`. The agents
reach `main`, and the cost measurement plan's branch is cut from `main`, only
once the process plan is archived, and a plan is not archived with its exit
condition unmet. So the measurement plan waits on this probe. It needs the
hooks: `R-067` runs it under the agents.

**Cost.** The measurement plan, and with it the ruling on `SQ-023` and the
phase 4 plan, wait on one user action and one probe. Until then the coder's
denylist is enforced only by the hashes `/plan-next` takes around step 3
(`D-119`). These catch a breach after it happens, not before.
**Owed by the implementer:** once the user has accepted workspace trust for
the folder, run point 4's live probe again. Green: the *Reserve discharged* line
under point 4's outcome, in a commit without a `Plan-point:` trailer (`R-011`).
Red with trust accepted: the result in the register, for the decider. Cite
`R-098`.
**Would overturn it:** the probe red with trust accepted, which ends the reserve
reading as above; the user choosing to archive the plan with the reserve open.
That would be a change to `R-011`, and theirs.
**Not verified:** that trust is the cause. Which of the two keys the probing
session used was not established, and the probe was not rerun here. The
documentation was read through a summary, not quoted from the page.
`disableAllHooks` is unset, and `.claude/settings.json` does not exist, so it
was not the cause. Whether a hook fires for an agent run in the background was
not documented, and was not tested.

### R-099 — `D-118` and `D-119` are ratified; the watermark reaches `D-119`

**Date:** 2026-10-07   **Origin:** register audit, `D-118` and `D-119`
**Verdict:** ratified
**docs/:** unchanged

Points 8 and 7 of the process plan. Process, so ratified or rejected, never
promoted (`D-011`). Each entry was read against its point's frozen text and the
rulings it applies, and its claims against `9d8ea3c` and `7b1fdbe`. Both
commits touch only the files their points name, and nothing under `core/`. The
five self-tests were run in this session: `path-guard` 34 cases, `git-guard`
44, `tree-state` 14, `mutate` 12, `plan-state` 59, exit 0 each, and the tree
was clean after them.

- **`D-118`, point 8.** Stands.
  - **`tree-state` failing when a git call fails, not only when a file cannot
    be read.** Right on the merits, and for `R-071`'s own reason: a check that
    cannot look says so.
  - **`mutate` stopping before the patch when the first call fails.** Right on
    the merits: it is the only way the tree is known to be untouched.
  - **A rename's or copy's source read from the header lines.** Stands. A quoted
    source is refused, which is the safe side.
  - **UTF-8 decoding, the locked file, the fixture.** Stand as recorded.
  - **The check beside the criterion.** `R-095`'s case is in `mutate`'s
    self-test, and its fault was seen red.
  - **The code-reviewer's three findings.** The `copy from` case was applied.
    The rejection of a case for the second call's failure is right, since it
    would test a message.
  - **The finding on point 2, left to the decider: no case covers `harness/` in
    `$Guarded`.** Accepted as a limit, with no action. Dropping `harness` would
    refuse the tester's valid patches with exit 2. That failure is loud and
    stops the point, and lets nothing through. A case would pin a configuration
    line, not a behaviour anyone could miss.
- **`D-119`, point 7.** Stands.
  - **The moment-one answer recorded in the installing commit or in the
    request.** This reads `R-049`'s "recorded" as `AGENTS.md` and `R-048`
    already place it. The cost measurement plan's answer stands in both:
    `8f9c698`, and item 1 of the request.
  - **The orchestrator introducing a fault `mutate` cannot reach, named in the
    outcome line.** Stands: the design is silent, and saying who introduced the
    fault keeps the record honest.
  - **The checks of "The hole that `Bash` leaves" as hashes over pathspecs.**
    Checked: step 3's list holds the same fourteen entries as `path-guard`'s
    `$CoderDenied`.
  - **The `R-047` question in every brief, the code-reviewer's included.**
    That is what `R-047` asked of both halves.
  - **The word count by `wc -w`.** It is the measure the cap was taken in.
  - **The check left in the scratchpad.** Accepted as for `R-093`.
  - **One cost, recorded rather than acted on.** The coder's denylist now has
    four copies: the design, `path-guard`, `coder.md` and `/plan-next` step 3.
    Point 8's grep keeps two of them in step for the settings files, and
    nothing ties the skill's copy to the rest. The next change to the denylist
    must name all four. Whether one home and three pointers would be better is
    governance, and no one has asked.
  - **Point 4's reserve.** `R-098`'s.

The actions owed by `R-047` (the brief's half), `R-049`, `R-070`, `R-095` and
`R-096` are marked done under "Owed by the implementer". `R-070` covers
`R-071`, `R-073`, `R-074` and `R-094`.

**Cost.** None beyond `R-098`.
**Owed by the implementer:** nothing new.
**Would overturn it:** as for `R-008`, any entry found to contradict `docs/` or
the design as it stood when written.
**Not verified:** the faults seen red were taken from the outcome lines; the
check script of point 7 was not rerun, since it is not in the repository.
No point has yet run under the rewired `/plan-next`, so whether its dispatches
work as written is the first real run's to show (`R-046`).

### R-100 — `D-120` is ratified; the watermark reaches `D-120`, and the probe is owed once more, in the background

**Date:** 2026-10-07   **Origin:** register audit, `D-120`
**Verdict:** ratified; one action owed
**docs/:** unchanged

`D-120` closes the process plan: the reserve of point 4 discharged, the archive,
the promotion pass. Process, so ratified or rejected, never promoted (`D-011`).
Its claims were checked against `2801c19`, `875c629` and the merge `1b10a00`.

- **The reserve discharged.** Stands, and it is what `R-098` asked. `2801c19`
  adds only the *Reserve discharged* line under point 4, carries no
  `Plan-point:` trailer and cites `R-098`. The line names trust accepted under
  one key, the probe rerun in a new session and the denial by `path-guard`.
- **The archive.** Stands. `875c629` moves `PLAN.md` to
  `.claude/plans/2026-10-05-agent-workflow.md` unchanged, and `git log
  --diff-filter=A` on `.claude/plans/` finds it, so `/plan-status` can bound the
  plan's range. `plan-state` run here on the archive reads `state: finished`,
  `reservesOpen` empty, point 4 `reserve-discharged`. `main` carries no
  `PLAN.md` (`R-048`).
- **The pass over `D-107` to `D-120`.** Stands: nothing promoted. `D-107` to
  `D-120` describe how the simulator is built, and `D-108`'s content reached
  `docs/` through rulings, as the pass says.
- **`REVIEW-REQUEST.md` not touched.** Stands: no commit between `32f20af` and
  `1b10a00` touches it, and the measurement plan's moment one is still its own.
- **No merge.** True when written. The merge followed in `1b10a00`, with two
  parents and no squash, as "Commits" in `AGENTS.md` requires. The register
  does not record who asked for it.

**The gap the entry names, and why it is owed now.** The probe ran with the
coder in the foreground. `R-098` left open whether frontmatter hooks fire for
an agent run in the background, and `D-120` leaves it open again. The harness
runs subagents in the background unless told otherwise, and `/plan-next` says
nothing on the matter. So the mode the first real point will most likely use is
the one never probed. `R-067` runs the cost measurement plan under the agents,
and that plan is next. If the hook stays silent in the background, the coder's
denylist rests on the hashes of step 3 alone. They catch a breach after it
happens (`D-119`), and only for the paths they hash. The check is the probe
already written, with one parameter changed, so it costs one session. Waiting
until a real point breaches would cost a reverted point and a register entry.
This would have been the right call before any code existed: a guard claimed
for a run is probed in the mode the run uses.

The probe of `Edit` is not owed: the coder's hook matches `Edit|Write` in one
entry, with one command, so the `Write` probe exercised the hook that `Edit`
reaches. The other roles' hooks are not owed either. The coder's denylist is
the one guarding `docs/` and the tests, which is what the gate protects.

`R-098` is marked done under "Owed by the implementer", in `2801c19`.

**Cost.** One probe session before the measurement plan's first point that
dispatches the coder. If it is red, that plan waits on a ruling, and possibly on
the user, since how `/plan-next` dispatches is a change to `.claude/skills/`
(`D-087`).
**Owed by the implementer:** before the first plan point dispatches the coder,
point 4's live probe again, with `coder` dispatched in the background. Green:
the result in the register. Red: the register and the decider, and no point
dispatches the coder until ruled. Cite `R-100`.
**Would overturn it:** the harness's documentation stating that frontmatter
hooks fire in both modes alike, quoted from the page, not summarised; or
`/plan-next` dispatching in the foreground, by a change the user permits.
**Not verified:** the probe itself; the two commits' claims were read, not
rerun. That the harness's default is the background was taken from this
session's tool description, not from documentation. Who asked for the merge in
`1b10a00` was not established.

### R-101 — `D-121` is ratified; the stand-in's market never rations, so point 2 owes a reference check on a state where it does

**Date:** 2026-10-08   **Origin:** register audit, `D-121`
**Verdict:** ratified; decision 3, left to the human, ruled here; one action owed
**docs/:** unchanged

Point 1 of the cost measurement plan, the stand-in. Process, so ratified or
rejected, never promoted (`D-011`). The entry was read against point 1's frozen
text, the plan's header, `DEC-054`, P-63 and P-64, and its claims against
`93ef064`, `bench/Program.cs` read whole. CI was grepped: it builds and tests by
project path and never names `Sim.sln` or `bench/`. Point 1's check was run in
this session: PASS, 9 rows 2.60 µs, 27 rows 14.90 µs, ratio 5.73.

- **Decision 1, the selection sort.** Stands. `DEC-054` says the clearing's cost
  "grows with the square" because rationing sorts the cohorts; a sort whose
  comparisons do not depend on the data is the plainest reading of that, and
  choosing it against the bounds is what a calibration point is for. What it
  entails is recorded, not faulted: the ratio check now holds by construction,
  so it shows that a quadratic term of this weight reproduces P-63 and P-64 over
  rows, and nothing about cost per good, which the draft's own "What was not
  verified" already said. The stand-in's row slope runs above `DEC-054`'s 4.8
  (5.3 to 5.7), so the row terms of points 2 to 5 err on the expensive side.
- **Decision 2, cells interleaved in chunks.** Stands. "Every cell is measured
  the same way" is met when the cells measured together are measured alike.
- **Decision 3, the thread pinned through the process, to logical core 2.**
  Ruled here; it falls in no reserve of `R-001`. The plan asks for "one thread
  pinned to one core", and a one-bit process mask meets it more strictly, since
  the runtime's own threads share the core. A busy SMT sibling adds noise, and
  noise shows in the spread of the three p99s the method prints. The question
  `SQ-023` brings to this plan is one of order of magnitude against P-01, which
  the estimate puts a hundred times over; noise of a tenth does not move it.
  Ratified, with no action.
- **Decisions 4 to 7.** Stand: the devices are measurement inputs (`R-050`, as
  `R-076` read it for `D-109`); `TieredCompilation` off, the platform
  attributes, and the criterion's numbers used as written, which a frozen
  criterion requires.
- **The incident, and the rejected finding.** Stand as recorded. A fault on
  `bench/` restored by copy can leave a stale build; it fails loudly either way.

**The criterion under-covers, and the missing check is blind too.** The point
closed under `R-005`'s case 1 with the missing check named — a checksum per
cell against a plain reference of the five passes — and deferred to point 2.
The code-reviewer claimed that check catches faults (a) to (d). It does not
catch (c), and the reason is fault (e), which the entry marks "not tested
whether it happens". It happens. The stand-in was ported to a script in the
scratchpad and run for 3,000 updates at 9, 27 and 41 rows: supply never ran
out, the least residual was 918 of about 1,000 units, the even goods' prices
sat at the floor of 50 and the odd goods' never moved from 110, 130 and 150.
Refilled at 600 per update, a row buys a handful of units across seven goods;
supply is a thousand per good. So every row is served whatever its rank, and
the order the sort computes never reaches stock, cash or prices. A ranking on a
constant key, or one never read, gives the same checksum as the real one, and
costs less: no swaps and a perfectly predicted branch.

That matters beyond point 1. Points 2 and 3 rewrite the stand-in for new
layouts, and the key is indexed by row and good, `stock[r * Goods + g]`, which
is exactly what a sparse layout re-indexes. Point 3's "same checksum, sparse
and dense" cannot see a key read from the wrong good. The table those points
produce is the evidence `SQ-023` is ruled on, so a check that cannot see the
term `DEC-054` is about would leave that ruling resting on numbers no one has
shown to come from a working market. This would have held before any code was
written: a checksum proves only what reaches it.

**Cost.** Point 2 grows by a reference implementation and a change to the
devices, so that some good runs short. That changes point 1's calibration a
little, since the allocation loop then stops early. The ratio already sits close
to its bound: 5.73 against 5.8 in this session's run, with the 9-row figure at
26 ticks of a 100 ns clock. A rerun may go red for that reason alone.
**Owed by the implementer:** within point 2, beside its criterion, under
`R-005`'s case 1 as `D-121` names it:
1. each cell's checksum at a fixed update count compared against a plain
   reference implementation of the five passes;
2. every cell run on a state where rationing binds for at least one good within
   the counted updates, so that the ranking reaches the checksum;
3. faults (a) and (c) of `D-121`, spoilage without its carry and a ranking on a
   constant key, seen red against it;
4. point 1's check rerun after any change to the stand-in's devices, and its
   result recorded.

If the last is red, the result goes to the register and the decider before
point 3 runs. Cite `R-101`.
**Would overturn it:** a layout whose cost per simulated day lands within the
spread of its runs of P-01, where noise could decide the verdict; then that
layout is measured again on an idle physical core, which reopens decision 3.
**Not verified:** the port was written from `Program.cs` by reading, not
generated from it, and was not checked against the C# checksum. `R-100`'s probe
was taken from `D-121`'s quoted hook message, not rerun. That the faults (a),
(b) and (d) are caught by a reference check once rationing binds was reasoned,
not run. The SMT topology of this machine was not examined.

### R-102 — `D-122` is ratified, save decision 8: the bytes column counts the settlement state of `SIM-STATE`, at its widths

**Date:** 2026-10-08   **Origin:** register audit, `D-122`
**Verdict:** ratified; decision 8, left to the human, rejected; the open regime
question ruled; one action owed
**docs/:** unchanged

Point 2 of the cost measurement plan, the dense sweep. Process, so ratified or
rejected, never promoted (`D-011`). The entry was read against point 2's frozen
text, `R-101`, NFR-07, `SIM-STATE` §Rule and §Cohort, and DEC-001; its claims
against `9c3a0b8`, `bench/Program.cs` read whole. Point 2's check was run in this
session: PASS, the seven checksums equal to the reference and to those `D-122`
records, rationing binding in every cell.

- **`R-101`'s four items** are met, and decision 7 goes further: the check
  itself turns red when the ranking does not reach the checksum. `R-101` is
  marked done.
- **Decisions 1 to 4, the devices.** Stand. They make every good move stock and
  cash and every third good ration, which is what `R-101` asked for. Demand no
  longer depends on price, which costs the stand-in a resemblance to a market
  and costs the measurement nothing: the work per update does not read it.
- **Decisions 5 to 7.** Stand. Cells measured one after another trade drift
  protection for simplicity. The cost shows: the 12 × 100 cell, measured twice
  in one run, gave 61.8 and 52.8 µs here, and 71.2 and 77.6 in `D-122`'s. Every
  figure of this sweep carries a noise of about a fifth, which `R-101`'s
  overturning condition already covers. Reading the duplicated cell as the
  plan's seventh is the plain reading of a sweep that lists it twice.
- **The "costs less" half of the `Fails when:`** was not observable: one good in
  a hundred is a percent of the work, inside the noise. The checksum caught the
  fault, which is what the check is for.
- **The rejected finding on `Supply`'s visibility** stands. It is taste and
  nothing for the human.

**The regime question, left to the decider.** The code-reviewer found that the
timed instance runs far longer than the checked one, with scarce goods' prices
rising by one or two every update without bound. Does that change what the
costs mean? No. The work per update is fixed by loop counts, and those are the
same in both regimes. The sort makes n(n − 1)/2 comparisons whatever the keys. A
scarce good's supply of 4 runs out within four rows whatever the prices. A
plentiful good's thousand units are never exhausted by at most 41 × 4 wants. So
the longer run changes values and branch outcomes, not how much is done.
Branch prediction differs, and that is inside the noise above. One part of the
finding is wrong: cash is negative only inside an update. The refill,
`1000 + (cash & 4095)`, puts it back in 1000 to 5095 every time. A fault that
shows only after 2,000 updates stays a limit of the check, as the entry
records.

**Decision 8, rejected.** The bytes column exists to be read against NFR-07:
"Settlement state stays under 50 MB". And `SIM-STATE` §Rule says what state is:
what influences a future tick and cannot be derived from what is listed.
`D-122` counts every array of the stand-in, and three kinds are not state:

- **Scratch.** `order`, `key` and `residual` are written before they are read
  in every update. In the simulator they belong to the thread, not to each of
  2,000 settlements.
- **Rates derived from content.** `prod`, `need` and `spoilNum` stand for what
  the update derives from the rules file, the allocation and the count. None
  is a field of §Cohort.
- **Width.** The stand-in holds everything in `long`. §Cohort gives `goods` as
  `int`, so a holding is 4 bytes, while its carry is fixed point over `long`
  (DEC-001), 8 bytes. `currency` is `int64`.

At 12 × 100 the column reads 49,840 bytes. The state it stands for is 15,296:
4,800 of goods, 9,600 of carry, 96 of currency and 800 of prices. That is a
third, and the bytes are the half of `SQ-023` that the estimate said fails
first. An overstatement by three would reach the ruling on the shape of state
as a verdict.

The price per good is counted though §Cohort and §Settlement list no price
field. The stand-in carries it across updates, and where prices live is
`SQ-024`'s question.

This would have held before any code: NFR-07 caps a quantity `SIM-STATE`
defines, and a measurement read against it counts that quantity. The question
was put to the human as one of meaning, and `SIM-STATE` answers it, so it is
not a reserve.

**Cost.** Point 3 recomputes point 2's column as well as its own, and states
which arrays of each layout stand for which field. A sparse layout's index
(offsets, the ids of the goods present) is state under §Rule, since the layout
cannot be read without it, and counts.
**Owed by the implementer:** within point 3, beside its criterion:
1. the bytes column, for point 2's cells and point 3's, counts the settlement
   state of `SIM-STATE` at its widths: the arrays standing for `goods` (4 bytes
   each), `goodsCarry` (8), `currency` (8) and the per-good price (8), and any
   index the layout holds. Content-derived rates and scratch are left out, or
   printed apart;
2. the figure is recomputed independently from the sizes and compared, the
   missing check `D-122` names, with an array dropped or counted twice seen
   red.

Cite `R-102`.
**Would overturn it:** a field of §Cohort that the stand-in's rates stand for
after all, such as an allocation held per row and good, which would bring that
array back into the count at its width.
**Not verified:** whether `SIM-STATE` should list prices, and where. That
belongs to `SQ-024`, and `SIM-ECON` was not read for whether a price carries
over from one update to the next. The run-to-run spread was observed in one run
here and taken from the entry for the rest.

### R-103 — `D-123` is ratified; point 4's served check runs where nothing is rationed, so it owes a reference check where routed demand is

**Date:** 2026-10-16   **Origin:** register audit, `D-123`
**Verdict:** ratified; decision 5, left to the human, ruled; one action owed
**docs/:** unchanged

Point 3 of the cost measurement plan, sparse state per settlement type. Process,
so ratified or rejected, never promoted (`D-011`). The entry was read against
point 3's frozen text, `R-102`, `SQ-024`'s five types and sizes, `SIM-STATE`
§Rule and §Cohort, `SIM-ECON` phase 7 and §Rationing; its claims against
`385e713`, `bench/Program.cs` from `StandIn.Bytes` to the end, and the diff from
`9c3a0b8`, which leaves `StandIn`'s update code untouched. Point 3's check was run
in this session: PASS, exit 0, the five checksums sparse = dense = reference and
equal to those `D-123` records, the bytes equal to `ExpectedBytes` in every
type, rationing binding and reaching the checksum, the timed pair the layouts
they are labelled. The costs came out at about half of `D-123`'s (sparse 3.0,
13.2, 72.5, 280.6 and 832.4 µs; dense 618 to 1,016), the ratios close to its.

- **`R-102`'s two items** are met. `ExpectedBytes` follows `R-102`'s widths: 12
  bytes per cell, 8 per row, 8 per good, 4 per id of a sparse index. Recomputed
  here by hand: village 1,332, dense 150,328, point 2's 12 × 100 at 15,296, the
  figure `R-102` gave. `R-102` is marked done.
- **Decisions 1 to 4, 6 and 7.** Stand. Ids spread evenly put the scarce pattern
  and the tie-break inside every type. Running the dense layout on the type's
  own world is the plain reading of "the dense cost of the same type", and the
  sort it runs for every absent good is the cost a dense layout pays. One update
  code for both layouts makes the comparison about data, which is what point 3
  asks. The shared `World` and `Supply` are a limit the entry names.
- **Decision 5, left to the human: ruled here**, since it falls in no reserve of
  `R-001`. It stands. A dense layout holds every good of the catalogue, so its
  positions are the ids, and its masks exist only so the stand-in can run a
  smaller world in a dense array. Neither is state under §Rule. The class
  comment already says the ids are positions; the comment on `Bytes` repeating
  it is taste, and no action.
- **`stock` counted at 4 bytes while held in `long`.** Bounded by reasoning, not
  by run: a cell gains at most 24 − 4 + 4 = 24 units in an update and loses at
  least a thousandth of its stock to spoilage, so it settles below about 24,000,
  far inside an `int`.
- **The criterion is defective** under `R-005`'s case 1, as the entry says, and
  the missing check was built and run. A criterion of equality between two
  paths that share their code and their geometry cannot fail where they
  coincide. The reference is what catches the off-by-one.
- **The rejected findings** stand, as taste.

**What point 4 inherits.** `R-101` found that a check run where nothing is
rationed cannot see the ranking. Point 4's "Closed by" asks that, "with supply
ample, every unit a village demands is served at its market". That is the same
blind spot. With supply ample, routed demand can be served in any order, or
outside the ranking altogether, and the check stays green while the cost falls.
The cost is the thing measured. And the `Fails when:` names a market that
ignores routed demand, not one that serves it unranked.

The second gap is what the ranking ranks. `SIM-ECON` §Rationing allocates "by
descending ability to pay", the share of a cohort able to pay read from its
wealth curve: an order over cohorts. `SQ-024` item 4 leaves where that runs,
once villages trade at a market, open. The two readings differ by more than the
noise. Under P-12 as answered, about 1,900 villages share about 100 market
places. A borgo's market ranking its own 10 rows and 19 villages' 6 rows each
sorts 124 entries, some 7,600 comparisons per good. Ranking each village as one
entry gives 29 entries, about 400 comparisons. Ranking its own rows alone gives
45. Phase 7 is the term `SQ-023` is about, so a table measuring only the cheaper
reading would decide the question on the reading that is not yet chosen. This
would have held before any code: a measurement that informs an open question
measures the readings that question still allows, or says which one it took.

The check is the same as `R-101`'s and `D-123`'s: the checksum against a plain
reference, on a state where the ranking binds. This is not a ruling on `SQ-024`
item 4. Point 4 picks a reading as a measurement input and says so.

**Cost.** Point 4 grows by a reference of market clearing with routed demand and
variants, a device that makes some market good short, and possibly a second timed
configuration. A second configuration that ranks every routed row may cost a
large multiple of the first, which is the finding, not a defect.
**Owed by the implementer:** within point 4, beside its criterion, under
`R-005`'s case 1:
1. each configuration's checksum at a fixed update count — clearing at market
   places with the villages routed to them, and the class with K = 1, 4 and 16
   variants — against an independent reference of the same clearing, on a state
   where rationing binds at a market for at least one good that routed demand
   asks for, so that the order of routed demand reaches the checksum;
2. two faults seen red against it: routed demand served outside the market's
   ranking, and a split that drops a variant;
3. the register stating how routed demand enters the ranking. If it enters as
   fewer entries than the cohort rows it comes from, the cost with every routed
   row ranked is measured beside it.

Cite `R-103`.
**Would overturn it:** a ruling on `SQ-024` item 4 placing rationing before point
4 runs, which makes item 3 measure that reading only. Or the two readings
measured to cost within the noise of each other on the P-12 day, which empties
item 3.
**Not verified:** the comparison counts are arithmetic on the sizes of `SQ-024`
and P-12 as answered, not measured; how many villages a market serves per group
of goods depends on `SQ-024` item 1, which the user deferred. `RULINGS.md` was
read in its state, owed actions and the rulings from `R-100` on, not whole. The
run-to-run drift of a factor two between this session and `D-123`'s was observed,
not explained; point 5 combines figures of different sessions, and whether that
moves a verdict against P-01 was not checked.

### R-104 — `D-124` is ratified; point 5's table is four layouts on one update code in one run, and the dense layout at market places is measured, not copied

**Date:** 2026-10-16   **Origin:** register audit, `D-124`
**Verdict:** ratified; the items left open ruled; one action owed
**docs/:** unchanged

Point 4 of the cost measurement plan, market places and goods variants. Process,
so ratified or rejected, never promoted (`D-011`). The entry was read against
point 4's and point 5's frozen text, the plan's header and exit condition,
`R-103`, `SQ-023`, `SQ-024`, NFR-05, NFR-07, P-01, P-17, P-63 and FR-T-06; its
claims against `1faa5c0`, `bench/Market.cs` from `Layout` to the end of `Mkt`
and `PointFour` whole. Point 4's check was run in this session: the eleven
checksums engine = reference and equal to those `D-124` records, rationing
binding where routed rows ask, every fault red, the ample check 8,128,546 of
8,128,546 served; PASS, exit 0. The timed figures came out within a tenth of
`D-124`'s: p99 per day 3.29 ms everywhere, 5.73 and 3.07 ms at market places
by the two readings; K = 16 at 1.04 times K = 1; per type, the market with its
nineteen villages 261.2, 331.4, 621.1 and 1,287.1 µs, a village 0.94 to 0.97 µs.

- **`R-103`'s three items** are met. Every configuration, the four timed
  per-type cells included, is checked against `MktRef`; `RoutedUnranked` and
  `DropVariant` go red; decision 1 records both readings of routed demand and
  measures each. `R-103` is marked done.
- **Decisions 1 to 3, 5, 6 and 8 to 11.** Stand, as measurement inputs that say
  what they stand in for. Decision 1 does not rule on `SQ-024` item 4, and the
  table keeps both readings for that reason.
- **The code-reviewer's (c), the binding condition.** Answered by the code, no
  action. `Check` folds every fault's red into the run's verdict, so a run in
  which routed demand could be served out of order without changing the
  checksum fails. The ranking deciding is shown by the fault on every run, the
  practice `R-102` ratified in `D-122`'s decision 7.
- **The code-reviewer's (d), ties.** Stands, no action. The key is
  `(cash + stock) × 1024 + (1023 − e)`, and the constructor refuses more than 1023
  entries, so no two entries share a key and the order is total. How ties fall
  changes who is served, not how much work is done, and work is what this plan
  measures.
- **The coder's doubts.** K on the one class good (decision 4): stands. Point 4
  asks for "a class with K variants", and the 1.07 is read as the marginal cost
  of one class, as the entry says. Nineteen villages per market whatever its
  type (decision 5): stands as a declared limit. `SQ-024` items 1, 2 and 5 are
  open, so no other assignment has a source. A control of sixteen separate goods
  against sixteen variants is not asked by the plan. The day's p99 and dense at
  market places are point 5's, below.

**What point 5 inherits.** Its "Does" asks for one table "for each of the four
layouts (dense or sparse, clearing everywhere or at market places)", with µs
per update per type, ms per simulated day and MB of state. The exit condition
asks for the same 2 × 2. Its `Fails when:` says it "gathers numbers already
measured". Three cells of the four exist, and not in a form one table can hold:

- **Dense at market places was never measured.** `D-124` decision 8 says so.
  Nothing else in the record gives it, and a figure derived without a run would
  be the kind of estimate `SQ-023` was raised against.
- **The cells come from two update codes.** Dense and sparse clearing
  everywhere, per type, are point 3's, from `StandIn`. The market places'
  per-type cells are point 4's, from `Mkt`, which measured sparse clearing
  everywhere too, but only as a whole day. `R-103` ratified point 3's single
  update code for both layouts because it "makes the comparison about data".
  The same holds across the 2 × 2: its two axes are layout and where clearing
  runs, so the update code is held fixed.
- **The cells come from sessions a factor two apart.** `R-103` observed that
  drift and did not explain it. Point 4's figures sit within that factor of
  P-01. Summed by point 5's own formula from `D-124`'s per-type cells, the sparse
  layout at market places with every routed row ranked gives
  (2 × 1,278.3 + 8 × 609.5 + 30 × 323.8 + 60 × 253.7 + 1,900 × 0.93) / 7 ≈ 4.9 ms.
  Doubled, that is 9.8 ms against a budget of 10. Figures copied from different
  sessions cannot carry that verdict.
- **Only one reading has per-type cells.** Point 4 timed per type the market
  with every routed row ranked; one entry per village only as a whole day.
  `R-103` held that a measurement informing an open question measures the
  readings it still allows, so both readings reach the table, each through the
  criterion's recomputation.

**The day against P-01.** NFR-05 and P-01 put the budget on a simulated day at
p99. The criterion recomputes a day as the sum over types of count × cost per
update ÷ 7. That figure is the average bucket at each type's p99. The p99 of
days falls on the heaviest bucket, and under FR-T-06 the buckets do not hold the
mix evenly: two metropolises cannot sit in seven buckets. Point 4 measured the
day directly as well, 5.53 ms against the 4.9 recomputed from its own cells;
this session's run gives 5.73 against 5.0. The
criterion is frozen and is computed as written; the direct p99 is printed beside
it where a verdict can turn on it.

The stand-in is five passes calibrated to P-63, which is "Measured, 5 of 12
phases". P-17 derives the ceiling at P-01 from that subset with a "2x allowance
for unmeasured phases", and §19 keeps the factor open until phase 4. A stand-in
day set against 10 ms alone would read the subset as the whole. The table sets
it against 10 ms as the plan says, and beside it against 5 ms, the same budget
with P-17's allowance. The sparse layout at market places with every routed row
ranked sits on that line in this session's run: 5.0 ms recomputed, 5.73 timed.

**The bytes.** `R-102`'s rule covers point 2's and point 3's cells. Point 4
computed no bytes, so the market-places layouts have none. The MB column is the
world's, the sum over the P-12 mix of count × bytes per settlement, which bytes
per type alone do not give. A sum from `D-123`'s figures puts sparse at about
3.9 MB and dense at 2,000 × 150,328 bytes, about 301 MB, six times NFR-07
whatever clearing does. That is the half of `SQ-023` the estimate said fails
first, and it holds before the dense market cell is run. The cell is owed all
the same: the "Does" and the exit condition name it, and narrowing a plan the
user approved is not the decider's.

All of this would have held before any code. A table read against a budget
compares figures taken alike; a cell it names is measured or marked missing,
never derived; and a subset is read against the whole only with the allowance
the specification already carries.

**Cost.** Point 5 is no longer only a gathering. `Mkt` gains a dense layout and
per-type cells for clearing everywhere and for one entry per village. The dense
layout at market places with every routed row ranked sorts 41 + 19 × 41 = 820
entries per good over 300 goods: by the stand-in's selection sort some 10⁸
comparisons per market update, so its per-type cells, at the plan's 10,000
updates and three runs, may take hours of wall clock. `StandIn`'s cells from
points 2 and 3 do not enter the table.
**Owed by the implementer:** within point 5, beside its criterion, under
`R-005`'s case 1:
1. every cell of the table measured in one run of the runner, in one session,
   by the plan's method, on `Mkt`'s update code for all four layouts, the layouts
   differing only in what each place holds and where clearing runs; nothing
   copied from the register or from an earlier run;
2. the dense layout at market places measured under both readings of routed
   demand, and each new configuration — dense everywhere, dense at market
   places, and the per-type cells added — checked against `MktRef` on a state
   where rationing binds, with `RoutedUnranked` red wherever routed rows exist
   and the off-by-one of point 3's `Fails when:`, applied to the dense walk so
   that it drops its last good, red;
3. the market-places layouts in the table once per reading of routed demand,
   each with per-type cells and its day recomputed by the criterion's formula;
4. beside each recomputed day, the p99 of a day timed directly in the same run,
   for every sparse layout and for any dense layout whose recomputed day is
   under 20 ms; the verdict against P-01 read on the higher of the two;
5. each day set against 10 ms and, beside it, against 5 ms, P-01 with P-17's
   allowance for the phases the stand-in does not run;
6. the MB column as the world's total over the P-12 mix, from bytes per
   settlement counted by `R-102`'s rule for every layout, the market-places
   ones included, with the village-to-market assignment counted if the layout
   holds it, and recomputed independently;
7. the table naming the measurement inputs it rests on: nineteen villages per
   market by a fixed assignment, both readings of routed demand, K on one class.

Cite `R-104`.
**Would overturn it:** the user waiving the dense cell at market places, a
change to the plan they approved and theirs to make. Or a ruling on `SQ-024`
item 4 before point 5 runs, which takes item 3 down to that reading.
**Not verified:** the hours are arithmetic on the entry count, not a run. The
3.9 MB and 301 MB are sums of `D-123`'s bytes, not recomputed from the code. That
the buckets' composition moves the p99 of a day by more than noise was inferred
from two direct figures, 5.53 against 4.9 and 5.73 against 5.0, on one
layout. `RULINGS.md` was read in its state,
owed actions and the rulings from `R-101` on, not whole.

### R-105 — `D-125` is ratified; point 5's criterion is defective under `R-005`'s case 1, and the table it closed on stands

**Date:** 2026-10-16   **Origin:** register audit, `D-125`
**Verdict:** ratified; the question left to the decider, and decisions 3 and 6,
ruled; one action owed
**docs/:** unchanged

Point 5 of the cost measurement plan, the table. Process, so ratified or
rejected, never promoted (`D-011`). The entry was read against point 5's frozen
text, the plan's header and exit condition, `R-104`, `R-005`, `R-084`, `SQ-023`
with its new section, NFR-05, NFR-07, P-01 and P-17; its claims against
`853e342`, `bench/Table.cs` from `Run` to the table's printing and `Mkt.Bytes`
and `Cur` in `bench/Market.cs`. The full check takes four hours and was not
rerun. `point5-smoke` was run in this session: every reference check equal,
rationing binding, every fault red, bytes counted equal bytes recomputed, the
synthetic table's controls caught, exit 3 as designed.

- **`R-104`'s seven items** are met, as the code-reviewer found and this reading
  confirms. `R-104` is marked done.
- **Decisions 1, 2, 4, 5 and 7 to 11.** Stand, as measurement inputs. Decision
  5 checks the dense market worlds that are timed, the per-type groups, and the
  dense P-12 world at market places is never timed, so nothing in the table runs
  unchecked.
- **Decision 3, `Cur`.** Stands for this table, on a reason narrower than the
  entry's. `Cur` is a remainder accumulator for the even split among variants,
  and at K = 1 it is `(cur + take) % 1`, always 0. A field that never changes
  influences no future tick, so it is not state under `SIM-STATE` §Rule here.
  With variants it is, at 4 bytes per row and class good. Counted on every cell,
  it would add 1.13 MB to the sparse world and 98.4 MB to the dense one, which
  moves no verdict against NFR-07.
- **Decision 6, the village cell.** Stands. The day sums costs, and a p99 of the
  19 villages' sum divided by 19 is the closer figure for a sum than 19 separate
  p99s. Villages weigh 0.32 ms of the 5.16 ms day at market places; no plausible
  reading of their cost moves a verdict against 10 ms, and against 5 ms that row
  is already over on its timed day.
- **Dense at market places ranking dead slots.** Stands. It is an upper bound,
  and the dense layouts fail NFR-07 by a factor of six on bytes alone, whatever
  their cost.
- **Point 1 failing once at 6.00**, against a ceiling of 5.8, `StandIn`
  untouched. Noise near the bound, as `R-101` and `R-103` foresaw. No action:
  point 1 closed, and its figures do not enter the table.

**The criterion is defective, case 1.** The frozen check proves that the day
column is the sum of the cells. The point's "Does" asks for more: the MB column,
each figure set against P-01 and NFR-07, and, since `R-104`, cells measured in
the run rather than gathered. The `Fails when:` rests on "the point gathers
numbers already measured", which `R-104` made untrue. Correct code passes the
check, and the check proves less than the point does. That is `R-005`'s case
1, as for points 1 to 4. The missing checks beside it are the reference checks,
the bytes recomputation and the table's own controls, all of which ran red when
broken.

**The three faults it would miss do not touch the table as committed.**
- (A) Read at `853e342`: every timed world is built from the `Cfg` named in its
  row (`Layout.One(t, c.Dense)`, `Layout.Group(1, 0, t, true, c.Dense,
  c.PerVillage)`, `Layout.P12(c.AtMarkets, c.PerVillage)`), and `Spec` gives
  each row the flags its name states. The figures corroborate it: the borgo,
  city and metropolis markets with every routed row ranked come out at 262.3,
  642.0 and 1,343.1 µs, against 261.2, 621.1 and 1,287.1 in point 4's separate
  run.
- (B) Read: `Ns[t]` is filled by type index, and the market groups map type `t`
  to `tt[t - 1]` without a shift. Every sparse row also rises from village to
  metropolis, as the sizes do.
- (C) The transcription in `SQ-023` was recomputed here: the six days from its
  cells by count × cost ÷ 7 (2.671, 204.048, 5.156, 2.463, 2774.615, 73.366
  ms), and the bytes from the sizes by `R-102`'s rule (3,945,144 and 300,656,000,
  plus 7,600 of assignment). All equal what the entry and `SQ-023` print.

So the weakness is real, and it matters only for a rerun. The table stands as
the measurement `SQ-023` was owed. This would have held before any code: a
check of a table's arithmetic says nothing of its labels, so the labels are
read, and here they were.

**What the ruling on `SQ-023` inherits**, recorded so that the next session does
not rediscover it. Neither point is ruled here: `SQ-023`'s direction is the
user's.
1. The verdict at 5 ms, P-01 with P-17's allowance, divides the readings of
   `SQ-024` item 4. With every routed row ranked, the sparse layout at market
   places is read at 6.62 ms, within 10 and over 5. With one entry per village
   it is read at 3.70, within both. Sparse clearing everywhere is read at 4.26.
   Short runs swing far more than that: this session's smoke run, 100 updates
   and not the plan's method, gave a recomputed day of 8.13 ms for sparse
   clearing everywhere. Only runs by the method count.
2. Variants of a class need a remainder accumulator for the split, per row and
   class good with variants (decision 3). `R-084`'s list of carries has none. If
   the user takes the sparse direction with variants, `SIM-STATE` gains one.

**Cost.** One line in the archived plan. A rerun of the table for a verdict
would first need the timed and checked worlds built from the same layout
objects, which `D-125` declined to do, since the fix meant a rerun.
**Owed by the implementer:** before the cost measurement plan is archived, a
line under point 5's outcome, never in its place: *criterion defective*
(`R-005`, case 1), `R-105`, in a commit without a `Plan-point:` trailer. Cite
`R-105`.
**Would overturn it:** a run of `point5` at `853e342` printing a cell more than
the run-to-run drift away from `SQ-023`'s, which would mean a mislabel that
reading missed. Or the user asking for the table again, which brings (A) back as
a check owed before that run.
**Not verified:** the full `point5` run was not repeated; the smoke run proves
the controls, not the figures. (A) and (B) were read, not mutated. `RULINGS.md`
was read in its state, owed actions, `R-001`, `R-005`, `R-009` to `R-011` and the
rulings from `R-101` on, not whole.

### R-106 — `D-126` is ratified, save decision 1, which is the user's; the watermark reaches `D-126`

**Date:** 2026-10-17   **Origin:** register audit, `D-126`
**Verdict:** ratified, decisions 2 to 4; decision 1 escalated, reserve 1 of `R-001`
**docs/:** unchanged

`D-126` records how the implementer session ran points 4 and 5 of the cost
measurement plan under the user's instruction of 2026-10-16. Process, so
ratified or rejected, never promoted (`D-011`). Read against `AGENTS.md`,
`R-001`, `R-048`, `R-067`, the plan's outcome lines and the commits it names
(`b18b267`, `c12b061`, `853e342`, `c24e818`).

- **Decision 2, `main` merged into the plan branch rather than the branch
  recut.** Stands. `R-048` asks that the branch be cut from `main` for the plan
  and that one commit install `PLAN.md` on it; both hold at `c12b061`. A recut
  would have rewritten the installing commit, and with it the record the frozen
  criteria are read against. Merging keeps the record and brings the workflow
  in.
- **Decision 3, point 5's step 4 run by the session that dispatched the
  agents.** Stands, on the reason given: a four-hour run that pins a core was
  run once, faults first, with `tree-state` equal before and after. The outcome
  line says so, as `R-067` asks.
- **Decision 4, the table transcribed into `SQ-023` by that session.** Stands.
  `SPEC-QUESTIONS.md` is the implementer's file, and `R-105` recomputed the
  transcription from its cells.
- **Decision 1, the decider run as an agent dispatched by an implementer
  session.** Not ruled. It is the split between the two roles, reserve 1 of
  `R-001`, and the user gave the instruction, on 2026-10-16 and again on
  2026-10-17 for this session. The decider cannot ratify its own mode of
  dispatch. **Recommendation:** keep it, on two conditions that would have
  held before any session ran it: the brief the implementer gives is facts
  only, never a proposed answer, and is kept where the user can read it; and
  the dispatched decider reads `R-001` and its procedure itself, as this one
  did. If the user wants it as a standing practice, it is an `AGENTS.md` rule
  by the user's permission; until then it holds instruction by instruction.

**`R-067` and `R-105` are done.** Every outcome line of the cost measurement
plan names the agents dispatched and those that do not apply, and none claims
`R-046`'s check (points 1 to 5, `93ef064` to `853e342`). `R-105`'s line under
point 5's outcome is in `c24e818`, without a `Plan-point:` trailer.

**Cost.** None beyond the user's answer on decision 1.
**Owed by the implementer:** nothing.
**Would overturn it:** the user declining decision 1, which would make each
decider session one the user opens; a brief found to carry a proposed answer.
**Not verified:** the briefs given to the decider agents of `R-103` to `R-105`
are not in the repository, so their being facts only rests on `D-126`'s word.
`RULINGS.md` was read in its state, owed actions, `R-001`, `R-050`, `R-052`,
`R-053`, `R-063` to `R-065` and the rulings from `R-103` on, not whole.

### R-107 — State is sparse per settlement, and a good clears on its own market or as a variant of its class: DEC-101, DEC-102, FR-P-03d

**Date:** 2026-10-17   **Origin:** SQ-023
**Verdict:** promoted as DEC-101 and DEC-102, with FR-P-03d specified — the
direction and the reading of AC-09 and AC-10 the user's (reserve 2 of `R-001`),
the rules the decider's
**docs/:** `SIM-DEC` DEC-101 and DEC-102 new; DEC-085 its example and cost,
DEC-054 and DEC-099 their cost. `SIM-REQ` FR-W-08 one sentence; FR-P-03d new;
FR-C-04 its opening and step 4; AC-09 and AC-10 per clearing; P-09; §19's rows
for P-17 and AC-09. `SIM-STATE` §Rule a paragraph; §Settlement `variantCarry`;
§Cohort its key note, `goods`, `durables`; §Trade link `flowAccumulator`,
`lossCarry`; A-18 new. `SIM-ECON` phases 2 and 7, the paragraph on phase 7's
cost, "Choice within a class". Every revision line to today.

`SQ-023` asked what the architecture must carry for a catalogue of hundreds of
goods and tens of mestieri, when `SIM-STATE` laid every row out over the whole
catalogue. The measurement it owed is the table of 2026-10-16. The user chose
the direction on 2026-10-17, state sparse per settlement, and read AC-09 and
AC-10 per clearing; on 2026-10-05 the user had set that each good declares a
market of its own or that it is a variant of a named class, validation checking
that the class has a market. What was left to the decider is the rules. The
grep of step 1 covered `per good`, `one per good`, `each good`, `clearing`,
`12 rows`, and the identifiers the entry cites.

**Sparse as "absent reads as zero" (DEC-101).** The user's words are a row only
for the mestieri present and holdings only for the goods present. Read as a
storage rule that changes no meaning, the update is defined on the values, and a
missing entry is a zero. Two other readings were set aside. A row created and
removed by its own rule, say when a mestiere's share in the vector becomes
non-zero, would give the state a second meaning beside its values, and a row
whose members died while it held goods or claims would lose them, against A-04
and A-02. A canonical form is also what keeps NFR-01 honest: if a zero could be
stored or not, two worlds holding the same values would hash apart by history,
the failure DEC-088 closed for capacity. Hence A-18, which a test can check
every tick. The rule is written for cohort rows and per-good fields, the two the
user named and the measurement covered. Building types and crops stay laid out
over the catalogue: they are tens, not hundreds, and nothing yet says otherwise.

**A clearing is a good or a class (DEC-102, FR-P-03d).** Three rules were the
decider's.

1. **The variant's class is one it belongs to.** A variant's price is the class's
   price per unit of value times its value in the class, and a value exists only
   in a class the good belongs to (FR-P-03a). The user's "validation checks only
   that the class has a market" is kept, and one refusal joins it, a variant
   naming a class it does not belong to: without membership the price is not
   defined.
2. **Price by value per unit.** "Their price differs by quality, not by market"
   needs a measure of quality the content already has. Value per unit in the
   class is that measure, and DEC-099 already buys "at the lowest price per unit
   of value". A separate quality factor per variant would be a second number with
   no other reader. The cost is named in DEC-102: for food, value is nutrition.
3. **Shares in a long clearing, with a carry.** When supply is short every offer
   sells whole, and rationing is on the buyers. When it is long, which variant
   sells is the split. Taking variants in id order would let the first starve the
   rest of buyers, and their producers would leave through the allocation
   vector; a split by largest remainders in each update would leave a small offer
   beside a large one at zero for ever, the integer trap of `SIM-ECON`. So each
   sells the same share of its offer and the fraction carries, at the clearing,
   per variant. `R-105` foresaw a carry per row and class good from the stand-in's
   even split of each buyer's purchase; the carry here is the sellers', so it sits
   at the clearing. The buyers' side is a split within one update and leaves no
   carry, under the rule `SIM-ECON` already has for such splits.

None of this rests on the stand-in's code. It would have held before it: a
storage rule that changes meaning is a second model, a price needs a defined
unit, and a recurring proportional split on small integers needs a carry.

**AC-09 and AC-10.** Applied as the user decided. The intent table's rows were by
good; a clearing takes the row of the most urgent need one of its goods meets,
which is the old rule for a consumable widened to a clearing, and nothing else.

**Not decided here.** `SQ-024`'s market places, the groups and the types: `R-108`.
The P-17 measurement on the sizes the catalogue is meant to carry: `R-108`, with
the types it is stated in.

**Cost.** Every per-good and per-row access becomes a search by id, and pools
grow and compact inside the update. A variant's own scarcity shows in no price,
and the author's choice of market or variant decides what can signal scarcity.
One carry per variant per clearing settlement. Existing code: none in `core/`
holds cohort rows or goods yet; the stand-in in `bench/` is not the simulator.
**Owed by the implementer:** from the phase 4 plan, cite `R-107`:
1. the point that first builds cohort rows or holdings builds them sparse under
   DEC-101 and checks A-18 every tick, its `Fails when:` naming a zero entry left
   stored, so that two worlds with the same values hash apart;
2. the point that builds the rules file carries FR-P-03d's declaration and its
   two refusals, with a `Fails when:` naming a variant whose class holds no
   market, or that names a class it does not belong to, accepted at load;
3. the point that builds market clearing builds FR-P-03d's price and split, with
   `variantCarry`, and a check where a long clearing holds a variant offering
   less than one unit's share in an update, its `Fails when:` naming that offer
   never selling.
**Would overturn it:** a measurement of the full update showing that searches by
id cost more than the dense layout saves at the sizes the user gave; content in
which a minor good needs its own scarcity signal, which is a market of its own
and needs no rule; a need for quality apart from value, which would add a factor
per variant.
**Not verified:** that every phase of the update can run on "absent reads as
zero" without a phase that must write a zero entry; checked for phases 5 to 9
by reading `SIM-ECON`, not for credit and demography. That building types and
crops stay small. `RULINGS.md` was read in its state, owed actions, `R-001`,
`R-050` to `R-053`, `R-063` to `R-065`, `R-084` and the rulings from `R-103`
on, not whole.

### R-108 — Prices form at market places: five types that follow population, villages routed per group, every routed row in the ranking; DEC-103

**Date:** 2026-10-17   **Origin:** SQ-024
**Verdict:** promoted as DEC-103, with FR-M-16 to FR-M-20 specified — the
direction, the types, the thresholds, the mix and the groups the user's, the
rules the decider's
**docs/:** `SIM-REQ` glossary, Settlement, and Group and Market place new;
FR-D-14; FR-M-03 one sentence; FR-M-04's table for five types; FR-M-16 to
FR-M-20 new; FR-G-01 one sentence; P-06 and P-12 for five types; P-77 to P-80
new; §18's Market row; §19's P-17 row. `SIM-STATE` §Settlement `type` and
`marketAssignment`. `SIM-ECON` phases 2, 7 and 12, the paragraph on phase 7's
cost, and one on routed rows. `SIM-DEC` DEC-103 new; DEC-019's caps; DEC-071's
cost. `SIM-LOOPS` B3a, R13 new with B13a and B13b, the gate check. Revisions to
today.

`SQ-024` proposed that prices form at market places. The user answered the
direction on 2026-10-17, "si ai market places"; the five types, their
populations and the hysteresis at about 80%, the mix at tick 0 and the
generator's placing of borghi on 2026-10-05; the groups on 2026-10-17. Point 4,
how routed demand enters the ranking, and points 3, 5 and 6 were the decider's.
The grep of step 1 covered `town`, `type`, `hub`, `market`, `degree`, `P-06`,
`P-12`, `FR-M-04`, `import`, and the identifiers the entry cites. `SIM-OBS`
speaks of towns and cities in prose; it is reserve 3 and was not touched, and
nothing in it reads otherwise under five types.

**The content approved by the user, recorded here as `R-063` was**, since it goes
to the rules file of `R-065`, not to `docs/`. Every value Provisional under
`R-050`. Nine groups, each traded from the type named, a place trading every
group from its type and below:

| Group | Contents | Phase 4 goods | From |
|---|---|---|---|
| G1 Provisions | grain, flour, bread, ale, salt, meat | grain, flour, bread | borgo |
| G2 Countryside | wool, hides, timber, flax | wool, timber | borgo |
| G3 Common manufactures | coarse cloth, shoes, pottery, candles | cloth | borgo |
| G4 Iron and tools | bar iron, tools, nails | iron, tools | borgo |
| G5 Small livestock | pigs, sheep, poultry | — | borgo |
| G6 Large livestock | oxen, horses | — | cittadina |
| G7 Fine manufactures | dyed cloth, worked leather, cutlery | — | cittadina |
| G8 Metals and arms | iron wholesale, weapons, armour | — | city |
| G9 Luxury | spices, silk, fine wine, jewellery | — | metropolis |

Ore is on no group. The number of markets a place shows is informational only.

**The rules, and why each.**

1. **Routed rows enter the ranking one by one (point 4).** The user preferred
   it, "il povero compete col povero"; the decider's reason is the rationing
   rule of `SIM-ECON`: "nobody buys cloth while another cohort in the same
   settlement cannot buy bread and could pay for it." One entry per village would
   rank a village by its mean, and its poor would be served before the market
   town's rich whenever the village as a whole could pay more. The measured cost,
   6.62 ms a stand-in day, within P-01 and over it with P-17's allowance, is
   named in DEC-103 and goes to the P-17 measurement, which now runs on this
   reading at the sizes the catalogue is meant to carry.
2. **When and where (point 3).** A routed row is settled when its market place
   clears, from its state as it stands; no queue, no delay. Settlement updates
   are weekly, and a market within reach is days away, so a delay would be
   below the resolution of the update, and a queue would be state that a market
   place holds for others. The market place therefore writes rows of
   settlements in other buckets; `SIM-ECON` now says so, and where the row's
   hunger deficit is written. Transport cost is a transit loss on the route,
   P-21 per link, borne by the routed row: FR-C-11 says it is never a currency
   term, and putting it on the row leaves the market's quantities whole. Routed
   shipments count in their links' volume, or a village's link to its market,
   carrying no price difference, would dissolve and cut it off.
3. **Out of reach.** A settlement with no market place within reach clears the
   group itself. The alternative, no exchange at all, would starve day labourers
   beside a farmer's grain in every cut-off village, the unbraked spiral
   DEC-043 was written against. A good in no group, ore, is cleared where it is
   held and never traded away: that is the user's "used where it is mined", and
   it lets a miner sell ore to a bloomery in the same place.
4. **What stays local (point 4).** Labour, own consumption, credit,
   destitution, spoilage, the seed reserve and the hunger deficit, each by its
   own phase in its own settlement. Rationing by ability to pay runs at the
   market, by rule 1.
5. **Assignment (point 5).** State, since hysteresis reads the current choice:
   the market place and the route's links, bounded by reach. Reassessed with the
   type every 28 ticks, in phase 12 of the settlement's own update; moved only
   when the current place stops trading the group, falls out of reach, or is
   beaten by more than P-80; ties by id. The search is bounded by reach, so that
   no update walks every settlement (AC-07). Between reassessments a broken
   route or a market lost sends the group to local clearing, so no row is left
   trading at a place that does not clear it.
6. **Type.** The highest type whose threshold the population reaches; down only
   below P-78 of the current type's, as the user set. A settlement that falls a
   type evicts links over its new cap, the weakest first, as FR-M-10 does, or A-06
   would fail on the day it falls.
7. **The rest of the model (point 6).** A settlement's price for a good it trades
   at a market place is that place's (FR-M-20): attractiveness, knowledge and
   expected return read it. Caps for five types map the old three, town to borgo
   and cittadina, city to city and metropolis, so P-06's decided values carry
   over unchanged. The import bound of FR-D-14 and B3a gains the catchment: routed
   trade needs no haulage, peasants carry their own goods to market, and reach
   bounds what it can bring.
8. **A new loop, R13.** A place that rises draws its neighbours' trade and
   grows, and one that falls loses it. `SIM-LOOPS` requires a named brake: the
   ceilings of R3 for the rise, land per capita for the fall. The hysteresis of
   P-78 is named as what keeps the loop from oscillating, not as its brake.

Each of these would have held before the stand-in existed; the stand-in's
nineteen villages per market and its fixed assignment are measurement inputs and
entered nothing here.

**Cost.** Phase 7 at a market place ranks its catchment. A market place writes
rows it does not own, so a point that builds it must order those writes by
bucket and id. Five types, four thresholds, a reach and a margin to tune. A
village's own scarcity reaches its people through rationing and hunger, not
through a price of its own. Ore and any good in no group tie their recipes to
their site. Existing code: none in `core/`; the stand-in's routing in `bench/`
is not the simulator.
**Owed by the implementer:** from the phase 4 plan, cite `R-108`:
1. the generator point builds five types from P-12 and P-77, borghi placed where
   villages are dense, and each settlement's assignment, with a check that every
   village starts within reach of a borgo or larger, its `Fails when:` naming a
   village generated out of reach;
2. the market point ranks every routed row as one entry (FR-M-19), with a check
   on a state where rationing binds at a market for a good routed rows ask for,
   its `Fails when:` naming routed demand merged per settlement or served outside
   the ranking; and it reads the rows routed to a market place without walking
   every settlement;
3. the point that builds type and assignment checks the hysteresis both ways and
   the eviction on a fall, its `Fails when:` naming a settlement that oscillates
   across a threshold or keeps more links than its new cap;
4. P-79 and P-80 proposed as Provisional values in the point that builds routing
   (`R-050`).
**Would overturn it:** the P-17 measurement on this reading putting the full
update over P-01 at the sizes of §19, which would bring back one entry per
village or a cheaper ranking; a cut-off place found to need no exchange, which
would drop rule 3's local clearing; the user changing any content above.
**Not verified:** that phase 10, run in a village's own update, cannot export
food its rows will want at the next clearing of their market; the "surplus" of
FR-M-03 was read as protecting needs, not checked against routed timing. Which
measure FR-M-09's dissolution reads, volume or the candidate's profitability,
is not stated in `docs/`; rule 2's volume credit assumes volume. That the
generator can meet P-12 and P-77 together on every seed. `RULINGS.md` was read
as for `R-107`.
