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

**Register watermark:** D-096 — audited by `R-030`. The next audit starts at the
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

## Escalated to the user

Rulings with verdict `escalated` or `pending`, until the user answers.

*None.*

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
