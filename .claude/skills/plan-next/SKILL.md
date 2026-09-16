---
name: plan-next
description: Runs the next open point of PLAN.md end to end - implement, verify, review if it touches the core, record the decisions, commit, stop. Use when asked to start or carry out the next point, to continue the current development, or to go ahead with a plan. Writes. For an explanation without doing the work use /plan-explain, for where things stand use /plan-status.
---

# /plan-next

Carry out one point of the plan, then stop. One point, one commit, one report.

The rules this serves are in `AGENTS.md`; the reasoning is in
`.claude/design/2026-09-17-plan-e-skill.md`. This skill is the procedure.

## Before starting

Read `PLAN.md` at the repository root.

- **Absent:** refuse. Work here starts from a written plan, and writing it is the
  conversation where a human decides what the work is. Do not offer to write
  one.
- **Every point closed:** do not invent more. Say the plan is finished, check
  the exit condition, and say that the branch is ready to close and `PLAN.md` to
  be deleted in the last commit before the merge, once the pull request is
  approved.
- **Otherwise:** the point to run is the first `## <n>.` section with no
  `*Esito:*` line. Points run in plan order, including a point added later than
  the ones after it — running them out of order contradicts the rule all three
  skills share.

## The eight steps

### 1. Read the point

Its statement, what it cites, its check, and its `Core:` declaration. If the
criterion can be read two ways, **stop and ask.** A criterion interpreted is a
criterion invented, and the plan is frozen precisely so that the interpretation
is not the author's to make.

A plan with no `Serves` fields, or with no `Kind` line, is a plan whose format
predates those fields or a process plan that omits them. Read it as it is.

### 2. Implement

Nothing here overrides `AGENTS.md`. When the point touches the core, the rules
the build cannot catch still apply — remainder accumulators, no iteration order
that is not derived from integer ids, nothing outside the state hash.

### 3. Verify

Run the point's declared check. Then, whenever the point touched code:

```
dotnet build core/Sim.Core.csproj -c Release
dotnet test core/Tests/Sim.Core.Tests.csproj -c Release
dotnet test core/Tests/Sim.Core.Tests.csproj -c Debug
```

**Run the check, do not read it.** On a point that builds nothing — a document,
a skill, a workflow — the temptation is to reread the work and call it verified.
That is where this repository has already found a defect that reading had
missed.

A point whose check reads `PLAN.md` itself is verified after step 6, on the
state the commit will contain; verifying earlier measures a state that never
ships.

### 4. Review, if the point declares `Core: yes`

`Core:` was declared when the plan was written, and is not reconsidered now that
the reviewers look expensive. Read `reviewers.md` in this skill's folder and
dispatch both briefs **in parallel, in one message**. The blind reviewer must
not see the briefed one's output.

### 5. Resolve the findings

- Verifiable against the specification, the code, or a test → apply it.
- A matter of taste, or the two reviewers disagreeing → it becomes an open point
  for the human. Do not settle it. The author of the code under review does not
  arbitrate its review.
- Wrong on the facts → say so, with the fact.

### 6. Write the record

In this order, before the commit:

1. The entry in `DECISIONS-OUTSIDE-SPEC.md`, under this development's section,
   naming the plan point. It holds the choices the specification does not make,
   and the findings that were **rejected**, with the reason. A rejection nobody
   can audit is most of the value thrown away.
2. The `*Esito:*` line appended to the point in `PLAN.md`: the date, what the
   check produced, the register reference, and the outcome of the review —
   including "not run, `Core: no`".

Everything above the outcome line stays untouched. A criterion that turned out
to be wrong is reported and recorded, never rewritten to match what was built.

### 7. Commit

Code, register and plan in one commit, so no part of the record can be left
behind uncommitted.

- The subject names the requirement or decision it implements, as `AGENTS.md`
  requires. If no identifier applies, say so in the message rather than
  inventing one.
- A trailer line `Plan-point: <n>`. **It means this commit closes point n.**
  Nothing else may carry it — not a commit that amends the plan, adds a point,
  or prepares one. That is what keeps `/plan-status` a lookup instead of a
  guess.

### 8. Stop

Report: what was done, what the check produced, what the reviewers found
including what was rejected, and what the next point is. Then stop. The next
point does not start by itself, and this skill does not chain.

## What this skill does not do

- It does not merge, push, or open a pull request unless asked.
- It does not delete `PLAN.md`. That belongs to the last commit before the
  merge, after the pull request is approved, while the plan is still useful to
  whoever is reviewing it.
- It does not add points to the plan. A plan may gain one, but that is a human's
  approval, not a step in this procedure.
