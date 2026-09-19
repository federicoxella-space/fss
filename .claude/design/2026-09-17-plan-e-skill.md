# Design — `PLAN.md` and the three plan skills

**Date:** 2026-09-17
**Status:** approved. Implemented by the plan of the same name; see `PLAN.md`
while that branch is open, and git afterwards.

---

## What this is for

Work in this repository arrives as a task and leaves as a commit, and between
the two there is nothing anyone else can read: no statement of what would count
as done, no record of what was decided on the way, no way for a later session to
say where the work stopped.

`DECISIONS-OUTSIDE-SPEC.md` closed half of that gap — what was decided. This
design closes the other half: what was supposed to happen, in what order, and
how each step would be known to be finished.

## What it is not

`docs/SIM-REQ.md` section 18 already plans this project: eight phases, each with
an exit gate. **`PLAN.md` does not compete with it and does not add to it.** A
plan point either cites a requirement from `docs/`, or states plainly that
`docs/` does not cover it — and that statement is itself a decision, recorded in
`DECISIONS-OUTSIDE-SPEC.md`. A plan that invents requirements is the same defect
as a specification edited to match its implementation, one level down.

## Scope of one plan

One `PLAN.md` at a time, at the repository root, describing the development
currently in progress and nothing else.

It is written before the work starts and **archived to `.claude/plans/` when the
plan closes** — not when the branch merges. A branch can hold two plans in
sequence, and tying the archive to the merge would leave a finished plan sitting
at the root pretending to be current while the next one is written. `main` never
carries a live `PLAN.md`; it carries every closed one.

Archived rather than deleted, for two reasons. The closed plan is the only record
of what was *intended*, next to a git history that only shows what happened; and
archival is the moment the promotion pass runs, which needs the plan and its
register entries side by side.

An abandoned plan is archived the same way, declaration and open points included.

### The promotion pass

Runs when a plan is archived, over the register entries written since the last
pass. One question per entry: **does this constrain code beyond the change that
produced it?** Yes makes it a candidate; process choices, one-off trade-offs and
notes about how the work was organised do not.

The output goes in `PROMOTIONS.md`: the `D-NNN` it came from, one line of why,
and the `SIM-DEC` entry drafted in that document's own shape — decision,
`**Rationale.**`, `**Cost.**`.

**A human writes `docs/`.** The proposal is the agent's and the writing is not,
or promotion becomes the back door into the specification that the prohibition
exists to close. Nothing in `PROMOTIONS.md` is in the specification; an entry
there has no authority at all until someone puts it in `SIM-DEC`.

Once written, the register entry is marked with the identifier it became and the
candidate is struck. A rejected candidate is struck too, with the human's
reason — an unanswered proposal re-proposed at every archival is how a rejected
idea gets in by attrition.

Numbering belongs to the human: a number claimed in a proposal would collide
with whatever else is in flight.

Without this pass the register grows and never empties, and the architecture in
it ages out of sight while `docs/` says nothing about it. That is the same
failure as editing the specification to match the implementation, arriving
slowly rather than at once.

### Stable identifiers in the register

Every `###` entry in `DECISIONS-OUTSIDE-SPEC.md` carries an identifier,
`D-001` upward, assigned in order of appearance and **never reused, never
renumbered** — the same discipline as `HashChannel`, and for the same reason: an
identifier that moves is worse than none, because references to it silently start
pointing somewhere else.

Without identifiers a promotion cannot be recorded against the entry it came
from, and the register cannot say which of its contents have already reached
`SIM-DEC`.

The granularity is the entry, and the register is not uniform about what an entry
holds: the earliest sections give one decision per entry, the later ones bundle
several under a plan point. A promotion citing a bundled entry has to quote which
part of it was taken. Splitting them retroactively was rejected — it would mean
rewriting the register's history to make a numbering scheme tidy.

## Format

```markdown
# Plan — <name of the development>

**Kind:** simulator | process
**Satisfies:** AC-01, AC-04   ← simulator plans only, and mandatory there
**Exit condition:** <what makes the whole development finished, cited from docs/>
**Phase:** 3 (SIM-REQ §18)   **Branch:** <branch name>

## 1. <title of the point>
- **Does:** one line.
- **Serves:** `FR-P-13`, `AC-01`   ← simulator plans only, and mandatory there
- **Closed by:** the runnable check. `AC01_SmallStocksAreNotImmortal` green.
- **Core:** yes — or `no — <reason checkable against the diff>`

## 2. <title of the point>
...
```

### `Kind`, and what it does to `Serves`

`Serves` exists to stop a point from inventing a requirement, which is work
`docs/` can only do where it has authority.

- **`Kind: simulator`** — the points change the simulated world. `Serves` is
  mandatory on every point: the identifiers it cites, or the plain statement
  that `docs/` does not cover this, which is a decision for the register. A
  point with no `Serves` is a defect in the plan, and since criteria are frozen,
  it is reported and referred to a human rather than filled in.
- **`Kind: process`** — the points change how the work gets done: tooling,
  workflow, these skills. `Serves` is omitted. `docs/` is not silent on how the
  simulator gets built; it is unrelated to it, and a field repeating "nothing in
  `docs/`" once per point reads as information while carrying none.

The three skills tolerate the field's presence in a process plan and the absence
of a `Kind` line altogether: both mean a plan written before this rule, and a
plan is a frozen contract that a skill does not tidy.

The rule was added after the first run of `/plan-explain` on a process plan
showed four points each reporting that `docs/` had nothing to say.

Everything above the `Esito:` line of a point is **frozen when written**. If a
criterion turns out to be wrong, say so and record why; do not rewrite it to
match what was built. That failure — a contract quietly adjusted to fit the
delivery — is the one this whole design exists to prevent.

### `Satisfies`, and how it differs from `Serves`

A `Kind: simulator` plan declares in its header which acceptance criteria the
whole development contributes to. The field is mandatory there and absent from a
process plan, like `Serves` and for the same reason.

The two are not the same field at different scales:

| | Scope | Question it answers | What it prevents |
|---|---|---|---|
| `Serves` | one point | which requirement is this point for | a point inventing a requirement |
| `Satisfies` | the whole plan | which acceptance criteria this development moves | an acceptance criterion with nothing behind it |

`Satisfies` is what eventually answers *which acceptance criteria have a test*,
which is the real coverage question and the one nothing in this repository can
answer today. It costs a line now and cannot be reconstructed later: nobody
reading a merged branch in a year can tell which criteria it was aimed at.

**The identifiers are opened and confirmed, like any citation.** Same rule as
step 1 of `/plan-next`: find each `AC-` in `docs/SIM-REQ.md` and read what it
says. A field filled with plausible numbers is worse than no field, because it
answers the coverage question instead of leaving a hole where the answer should
be — and a hole is visible.

A criterion the plan will only partly move is still declared, with what remains.
Declaring nothing because the contribution is partial is how a criterion ends up
with several developments behind it and no record of any.

### `Check: weak`

A check satisfiable by reading rather than by running is marked when the plan is
written:

```markdown
- **Check: weak** — <why reading is all there is> Replaced by <what would prove it>.
```

Both halves are required. Declared late, in an outcome line, a weak check is an
excuse offered after the work; declared early it is an argument that can still be
lost, when changing the point costs nothing. And **without the second half the
marker authorises weakness instead of limiting it** — "this cannot be proved
today" is a different statement from "this cannot be proved", and only one of
them says when to come back.

**Look for a mechanical check before reaching for the marker.** Greps, a
`git diff --name-only`, a file that exists or does not, a command that exits
non-zero — a surprising number of documentation points have one. Over-applying
`Check: weak` is as corrosive as skipping it: a marker on every point lowers what
anyone expects to be proved, and a plan where everything is weak is a plan that
proves nothing while appearing candid about it.

This is not hypothetical. Point 2 of the plan in `.claude/plans/` carried the
marker and every clause of it turned out mechanically verifiable; the marker had
been applied out of habit.

**A marker that turns out wrong is a finding, not an edit.** If the check proves
executable, run it, report it in the outcome line and the register, and leave the
criterion alone. The reviewers are told to look for exactly this.

### `Core:` defaults to yes

`Core:` decides whether the reviewers run, and it is declared before the work so
that nobody decides it afterwards, when the reviewers look expensive and the
point can be remembered as never really touching the core.

**The default is `yes`.** Writing `no` costs a reason, on the same line, in the
plan, before the work starts:

```markdown
- **Core:** no — touches no file under `core/`
```

The reason has to be **checkable against the diff**. "Small change", "only
documentation as far as I can tell", "low risk" are judgements, and a judgement
by the author about the author's work is what the reviewers exist to replace.
"Touches no file under `core/`", "no change to any field in `WorldState`" are
statements someone can hold against the diff and find false.

**A missing reason is resolved as `yes`,** and reported as a defect in the plan.
The absence of a justification is settled in the direction that costs review
time, never in the direction that skips it. The plan is not stopped for it: a
blocked plan is a worse answer than a reviewed point.

**And the declaration is checked, not trusted.** A point claiming `Core: no`
whose diff touches `core/` has a false reason, so the reviewers run anyway and
the contradiction is recorded. A downgrade nobody verifies is a downgrade
available whenever review is inconvenient — the same self-certification problem
the read-only agent type solved for the reviewers, and it gets the same
treatment: the tools check it.

## The life of a point

A point is the smallest unit that leaves the repository green, and it is exactly
one commit.

1. **Read the point.** If its criterion can be read two ways, stop and ask. A
   criterion interpreted is a criterion invented.
2. **Implement.**
3. **Verify.** The declared check, plus `dotnet build core/Sim.Core.csproj -c
   Release`, plus the test suite in Release and in Debug.
4. **Review, if `Core: yes`.** Two agents in parallel — see below.
5. **Resolve the findings.** Apply what is verifiable against the specification,
   the code, or a test. Matters of taste, and disagreements between the two
   reviewers, become open points for the human; they are not settled by the
   author of the code under review.
6. **Write the record.** The entry in `DECISIONS-OUTSIDE-SPEC.md`, naming the
   plan point and including the findings that were **rejected**, with the
   reason. Then the `Esito:` line appended to the point in `PLAN.md`.
7. **Commit.** Code, register and plan in one commit, so no step of the record
   can be left behind uncommitted. The subject names the requirement or decision
   as `AGENTS.md` requires; a trailer line `Plan-point: <n>` links it back.
8. **Stop.** Report and wait. The next point does not start by itself.

Because the `Esito:` line is written before the commit that contains it, it
cannot cite that commit's hash. It carries the date, the result of the check,
the reference into the register, and the outcome of the review; the commit is
found from the `Plan-point:` trailer.

## When a point fails, and when a plan is abandoned

Two different outcomes. Confusing them loses the distinction between *this did
not work* and *we stopped wanting it*, which is most of what a record is for.

### A point fails

Its check does not pass, or the work turns out to be impossible, or a citation
does not hold and the human sends it back. The point is **closed as failed**,
not left open:

- The outcome line says so, with what was attempted and what the check did.
  `*Esito:* <date>. FALLITO — <what failed>.`
- The register entry is written as for any point. A failed attempt is often more
  informative than a successful one, and it is the only record of a road already
  walked.
- The commit carries `Plan-point: <n>` as usual. The trailer means the commit
  closes the point, and a point closed as failed is closed.
- **The commit must leave the repository green.** Whatever of the attempt can be
  kept without breaking the build or the suite is kept, so the next person can
  see it; the rest is described in the register and discarded. A red commit on
  the branch would make every later `git bisect` lie.
- **The plan stops.** The next point does not start. Later points may rest on
  this one, and even where they do not, a failure is information the human needs
  before more work is spent.

### A plan is abandoned

The direction changed, the work was overtaken, the problem dissolved. This is
**declared by a human**, never by `/plan-next` — a procedure that could abandon
its own plan when the work got hard is not a procedure.

- The plan header gains the declaration and the reason:
  `**Abandoned:** <date> — <why>`.
- Open points stay open. They record what was intended and not done, which is
  information; closing them to tidy the file would erase it.
- **The register entries stay valid.** They describe code that existed and
  decisions that were really taken, and neither becomes untrue because the plan
  stopped. An entry is wrong only if it was wrong when written.
- The plan is archived like any closed plan, and the promotion pass runs over it
  like any other: an abandoned development can still have produced a decision
  that belongs in `SIM-DEC`.

## The review protocol

Two agents, in parallel, when and only when the point declares `Core: yes` —
that is, when it changes behaviour under `core/Runtime/`, or touches
determinism, currency, or state.

|  | Briefed reviewer | Blind reviewer |
|---|---|---|
| The diff and the repository | yes | yes |
| `docs/`, `AGENTS.md` | yes | yes |
| The task as the human stated it | yes | yes |
| My doubts, my reasoning, the alternatives I rejected | yes | **no** |

Both may build and run the tests, and are asked to check the numerical claims
they find in comments. Neither edits code: they report, the author applies.

Doubts go to the briefed reviewer **as questions**, never as conclusions. "Does
this reading of DEC-002 hold?" invites a review; "I read DEC-002 this way,
confirm?" invites agreement.

**An empty result is a valid result.** Both reviewers are told so explicitly,
and a reviewer that found nothing is reported as having found nothing. The
common failure of a review agent is that it exists to find something, so it
finds something; the second common failure is the author quietly promoting the
weakest finding to fill the section.

Agent review does not replace human review. It is good at "you claimed X and the
code does Y" and at sweeping for specification compliance. It is weak at exactly
the kind of insight that has already overturned a design in this repository
once.

## The three skills

Project skills, in `.claude/skills/`, committed, so they belong to the
repository rather than to one machine.

| Skill | Does | Writes |
|---|---|---|
| `/plan-next` | Runs the life of a point, above, on the first point with no `Esito:`. Refuses if `PLAN.md` is absent, or if every point is closed — in which case it says the branch is ready to close and the plan to delete. | yes |
| `/plan-explain` | Says which point is next, what it requires, what it cites, and what will close it. Changes nothing. | no |
| `/plan-status` | Points closed, with their commits, decisions and findings; the current point; what remains; the exit condition. | no |

Two of the three are read-only on purpose: asking where the work stands should
never be able to move it.

## The rule in `AGENTS.md`

A short section holding four rules, with the procedure left to the skills:

- The plan is written before the development starts.
- Criteria are not rewritten to match what was built.
- A point is one commit with a runnable check.
- A point that touches `core/Runtime/` passes two reviewers, one briefed and one
  blind, and their findings reach the register even when rejected.

## What is deliberately not built

- **A skill that writes the plan.** Writing it is the conversation where a human
  decides what the work is, which is where that judgement is worth the most.
  Automating it would produce plans that agree with the agent that wrote them.
- A dashboard, a progress percentage, or any rendering of the plan.
- Any template for work outside this repository.
- Tests for the skills. There is nothing to assert; the check is running
  `/plan-explain` and `/plan-status` against the first real `PLAN.md` and seeing
  whether they say the right thing.

## Verification of this design

The first `PLAN.md` is the one that builds this mechanism. If the format is
awkward, that surfaces immediately, on a development where being wrong costs
nothing.
