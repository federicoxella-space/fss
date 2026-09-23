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
