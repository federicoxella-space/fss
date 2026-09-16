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
currently in progress and nothing else. It is written before the work starts and
**deleted when the branch closes into `main`** — in the last commit on the
branch, after the pull request is approved and immediately before the merge, so
that whoever reviews the branch can still read the plan while reviewing it.
`main` never carries a plan. The history of closed plans lives in git and in the
decisions register.

## Format

```markdown
# Plan — <name of the development>

**Exit condition:** <what makes the whole development finished, cited from docs/>
**Phase:** 3 (SIM-REQ §18)   **Branch:** <branch name>

## 1. <title of the point>
- **Does:** one line.
- **Serves:** `FR-P-13`, `AC-01` — or "nothing in docs/ covers this", which is a
  decision and goes in the register.
- **Closed by:** the runnable check. `AC01_SmallStocksAreNotImmortal` green.
- **Core:** yes | no

## 2. <title of the point>
...
```

Everything above the `Esito:` line of a point is **frozen when written**. If a
criterion turns out to be wrong, say so and record why; do not rewrite it to
match what was built. That failure — a contract quietly adjusted to fit the
delivery — is the one this whole design exists to prevent.

`Core: yes` is declared up front because it decides whether the reviewers run.
Declaring it at the start removes the chance to decide, once the work is done
and the reviewers look expensive, that the point never really touched the core.

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
