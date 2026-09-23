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

**Register watermark:** none — no `D-NNN` has been audited yet. The next audit
starts at `D-001`. Entries already marked by the human (promoted, closed) are
ratified in bulk unless something in them no longer holds.

The watermark is the last `D-NNN` audited, or `none`. `/plan-next` reads this
line to tell the human how many entries are outstanding, so keep its shape.

## Owed by the implementer

Actions a ruling requires of the code or of the plan. The implementer cites the
`R-NNN` in the commit that does it; the decider marks it done with that commit.

- **`R-003`**, within plan point 5: no public member of the core exposes mutable
  world state for reading or writing.
- **`R-004`**, no later than plan point 7: a fresh world takes the core's current
  rule version; a different value enters state only through loading a save.

## Escalated to the user

Rulings with verdict `escalated` or `pending`, until the user answers.

- **`R-005`** — the failure path for a criterion found wrong in flight. Needs:
  accept, amend or reject the two-case recommendation, and say whether it is
  written now or after the phase 3 plan closes.

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
