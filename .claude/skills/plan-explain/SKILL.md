---
name: plan-explain
description: Explains the next open point of PLAN.md without touching anything. Use when asked what comes next, what the current development still owes, what a point requires, or before deciding whether to start the next point. Read-only companion to /plan-next and /plan-status.
---

# /plan-explain

Say what the next point of the plan asks for, and stop. This skill changes
nothing — not the plan, not the code, not a typo in passing. Asking where the
work stands must never be able to move it.

## How this skill runs

**Dispatch the procedure below to a read-only agent** — `Plan`, or `Explore` if
that is unavailable. Neither has `Edit` or `Write`. The sentence above about not
changing a typo in passing is a promise; an agent without the tools cannot break
it. Never a general-purpose agent.

It matters more here than it looks: this skill reads the plan and forms
objections to it, and an agent holding an objection and a text editor is one
step from resolving the objection by editing the text. The criteria are frozen,
and this is what freezes them.

**The relay is not covered.** The agent reports to this session, which repeats
it. Quote its objections rather than restating them — an objection softened in
the retelling is an objection that did not happen.

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
   close, `PLAN.md` is archived to `.claude/plans/`, and the promotion pass is
   due over its register entries. Then stop. A point *Chiuso con riserva* is
   closed but the plan is not finished while its reserve is open — no *Riserva
   sciolta* line under it: name the human action it waits on (`R-011`).

   **If the plan header carries an `**Abandoned:**` declaration**, say so with
   its reason and stop. Do not present the next open point: it was not reached,
   it was given up, and offering it would invite the work to resume by
   accident.

   **If a closed point's outcome line says `FALLITO` and points remain open**,
   say the plan stopped at that failure, quote what failed, and stop. The point
   after a failure is not the next point — the plan is not running. Whether to
   retry, change the point, or abandon the plan is a human's decision.

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
   - what will close it: the runnable check, quoted from the plan. If it carries
     a `Check: weak` or `Check: shallow` marker, report which, the reason, and
     what the plan says would prove it properly; object if either half is
     missing, since a marker with no replacement authorises the weakness instead
     of limiting it. **Object if the marker is the wrong one of the two:**
     `weak` means nothing can be executed, and if a grep or a file test would
     run, the point is shallow and its remedy is a better criterion the author
     could write now, not a tool that does not exist. This is the last moment
     where changing the point costs nothing, and the marker has already been
     misapplied three times in this repository;
   - its `Core:` declaration, and therefore whether the two reviewers run when
     it closes. `Core:` defaults to yes, so report an exemption with the reason
     the plan gave for it, and say whether that reason is checkable against a
     diff or is a judgement about size or risk — a judgement is not a reason,
     and this is the last moment to say so cheaply. A point that declares `no`
     with no reason at all is a defect in the plan: report it, and say the point
     will be reviewed as `yes`;
   - where it sits: how many points are closed, how many remain, the plan's
     exit condition, and under `Kind: simulator` its `Satisfies:` list — the
     acceptance criteria the whole development is aimed at. Report its absence
     from a simulator plan as a defect; it is mandatory there and cannot be
     reconstructed once the branch is merged.

6. **If the point found is the first of its plan, say a review request is
   owed** — moment one of `REVIEW-REQUEST.md`. This skill is often what someone
   runs before deciding to start, which makes it the last cheap place to say
   that the plan has not been shown to anyone. Say it is owed; writing it is
   not this skill's business, and neither is writing it for the human.

7. **Read the point critically, and say so if it does not hold.** A criterion
   that can be read two ways, a check that cannot actually be run, a point that
   has been overtaken by what the earlier points turned out to do — these are
   worth more before the work starts than after. Report the objection; do not
   edit the plan to fix it. The criteria are frozen, and changing them is a
   human's call.

8. Stop. Do not start the point. `/plan-next` does that, on its own invocation.

## What this skill does not do

- It does not write, stage, commit, or run anything that changes the tree.
- It does not summarise the whole development: that is `/plan-status`.
- It does not judge whether a closed point was closed well. Its subject is the
  next point, not the last one.
