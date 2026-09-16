---
name: plan-explain
description: Explains the next open point of PLAN.md without touching anything. Use when asked what comes next, what the current development still owes, what a point requires, or before deciding whether to start the next point. Read-only companion to /plan-next and /plan-status.
---

# /plan-explain

Say what the next point of the plan asks for, and stop. This skill changes
nothing — not the plan, not the code, not a typo in passing. Asking where the
work stands must never be able to move it.

## Procedure

1. Read `PLAN.md` at the repository root, header included: the exit condition,
   the branch, and `Kind` if the plan declares one. `Kind` decides whether the
   points carry a `Serves` field at all — see step 5.

2. **If it is absent**, say so and stop. Work here starts from a written plan;
   see `AGENTS.md` and `.claude/design/2026-09-17-plan-e-skill.md`. Do not offer
   to write one — the plan is the conversation where a human decides what the
   work is.

3. **Find the next point:** the first `## <n>.` section with no `*Esito:*` line.
   The outcome line is what closes a point; nothing else counts, and a point
   whose work looks done but carries no outcome line is open.

4. **If every point has one**, say the plan is finished: the branch is ready to
   close, and `PLAN.md` is deleted in the last commit before the merge, after
   the pull request is approved. Then stop.

5. **Otherwise report, for the point found:**
   - its number and title, and what it does;
   - **under `Kind: simulator`**, what it serves: the requirement or decision
     identifiers it cites, or the plain statement that `docs/` does not cover
     this, which is itself a decision for the register. A simulator point with
     no `Serves` field is a defect in the plan — report it and refer it to a
     human; the criteria are frozen and this skill does not fill them in.
   - **under `Kind: process`**, nothing about `docs/`. The specification is not
     silent on how the simulator gets built, it is unrelated to it, and a line
     per point saying so reads as information while carrying none. A process
     plan that still carries `Serves` fields, or a plan with no `Kind` line at
     all, was written before this rule: read it as it is and do not tidy it.
   - what will close it: the runnable check, quoted from the plan;
   - whether it declares `Core: yes`, and therefore whether the two reviewers
     run when it closes;
   - where it sits: how many points are closed, how many remain, and the plan's
     exit condition.

6. **Read the point critically, and say so if it does not hold.** A criterion
   that can be read two ways, a check that cannot actually be run, a point that
   has been overtaken by what the earlier points turned out to do — these are
   worth more before the work starts than after. Report the objection; do not
   edit the plan to fix it. The criteria are frozen, and changing them is a
   human's call.

7. Stop. Do not start the point. `/plan-next` does that, on its own invocation.

## What this skill does not do

- It does not write, stage, commit, or run anything that changes the tree.
- It does not summarise the whole development: that is `/plan-status`.
- It does not judge whether a closed point was closed well. Its subject is the
  next point, not the last one.
