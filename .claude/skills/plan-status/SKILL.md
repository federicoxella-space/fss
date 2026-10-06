---
name: plan-status
description: Use when resuming work after a break, when asked how far along a development is or what has been done so far, when asked about an archived plan, or before closing a branch.
---

# /plan-status

Report where the development stands, and stop. This skill changes nothing.
Asking how far along the work is must never be able to move it.

Its subject is what has already happened. For what comes next in detail, that is
`/plan-explain`.

## How this skill runs

**Take the plan's state from `plan-state`**, in this session:

```
pwsh -NoProfile -File .claude/hooks/plan-state.ps1
```

`PLAN.md` by default; `-Path .claude/plans/<file>` for an archived plan. Its
JSON is the plan's state: whether the plan exists, `kind`, `abandoned`, each
point's `status` — `open`, `closed`, `failed`, `reserve-open`,
`reserve-discharged` — `check`, `core`, `coreReason`, `serves` and `commits`,
then `state` — `none`, `abandoned`, `stopped`, `finished` or `running` —
`next`, `stoppedAt`, `reservesOpen`, the `range` the commits were searched in,
and a `warning` when a live `PLAN.md` sits on `main` (`R-048`). Which points are
closed, whether the plan is running, and which commit closed each point are what
the script says; this skill does not work them out again in its own words.

The script reads outcome markers in both forms (`AGENTS.md`, "Language"), and so
does this skill in the lines it quotes: `*Esito:*`, `FALLITO`, *Chiuso con
riserva* and *Riserva sciolta*, from plans written before the rule, mean what
their English forms mean.

**Then dispatch the procedure below to a read-only agent** — `Plan`, or
`Explore` if that is unavailable, never a general-purpose agent — with the JSON
in its brief. Both lack `Edit` and `Write`, and both hold `Bash`: the tools make
a write unlikely, not impossible, and the check below is what notices one.

The risk this closes is not malice. It is the drift into fixing a typo while
passing through, on the one command someone runs precisely when they do not
know what state the tree is in.

**Hash the tree around the agent:**

```
pwsh -NoProfile -File .claude/hooks/tree-state.ps1
```

once before the dispatch and once after. If the two differ, the agent changed
the tree: say so first, show `git status --short`, and stop without relaying the
summary. Undoing the change is the human's call.

**What it does not cover: the relay.** The agent's report comes back to this
session, which repeats it to the human. Nothing stops that repetition from being
wrong. So quote the agent's findings rather than paraphrasing them, and when the
report says something is missing or contradictory, pass it on in the agent's
words.

## Procedure

1. **If `state` is `none`**, say so: either no development is in progress, or
   the last one closed and its plan was archived to `.claude/plans/`, which is
   what closing a plan does. Look there and in `git log` for the history. Stop.
   **If `warning` is set**, report it before anything else.

2. Read the plan's header: the exit condition, the phase, the branch, `Kind`,
   and under `Kind: simulator` the `Satisfies:` list. Report that list with the
   summary: it is the development's claim about which acceptance criteria it
   moves, and a summary of closed points that omits it says what was done
   without saying what it was for.

   An `**Abandoned:**` declaration is reported **first**, with its date and
   reason. Everything below it is the record of a development that stopped, and
   a summary read without knowing that misleads on every line.

3. **Split the points by `status`.** A `failed` point is closed as failed, and
   is reported apart from the others. Folding it in with the successes would let
   a plan read as complete while one of its points says the thing could not be
   done.

   If `state` is `stopped`, the plan is **stopped** at `stoppedAt`, not in
   progress: say so, and say that restarting it is a human's decision.

   A `reserve-open` point is closed, and reported apart as well, with the clause
   and the human action its outcome line says it waits on (`R-011`). A
   `reserve-discharged` point is closed, its *Reserve discharged* line quoted.

4. **For each closed point, report its `commits`.** They are the commits
   carrying its `Plan-point:` trailer inside `range`, which runs from the
   archive commit in `.claude/plans/` of the plan before, exclusive, to the
   plan's own archive commit, inclusive — `HEAD` for the plan still live at
   `PLAN.md`; the first plan ever written has no lower bound. Point numbers
   restart with every plan; the range is what keeps one plan's point 1 from
   matching another's, and it rests on plans being sequential — one `PLAN.md` at a time,
   each archived before the next is installed, which nothing but the procedure
   enforces.

   If `commits` is empty for a closed point, say so plainly rather than
   searching for a likely candidate: an outcome line with no commit behind it is
   the one state this design cannot distinguish from finished work, and it
   needs a human. If it holds more than one, the trailer has been used loosely;
   report them all, and that too needs a human.

5. **For each closed point, report** its number and title, its commits, the
   register section its outcome line points to, which check marker it carried —
   `check`: `weak`, `shallow`, or none — and whether the outcome reported
   the marker turning out wrong, and the review outcome recorded there — including "not run, `Core: no`", and including findings that were
   raised and rejected. A rejected finding that vanishes from the summary is a
   rejection nobody can audit.

6. **Report what remains:** the open points by number and title, which one is
   `next`, and which of them will need the two reviewers. `Core:` defaults to yes,
   so an open point needs them unless it declared `Core: no` with a reason —
   report the reason, and flag an exemption resting on a judgement about size or
   risk rather than on something a diff can contradict.

   For a **closed** point that was exempt, report the reason it gave and whether
   the diff bore it out, since `/plan-next` records the contradiction when it
   does not. An exemption that was wrong and reviewed anyway is part of the
   record; an exemption that was wrong and passed unreviewed is what this line
   exists to surface.

7. **Report the exit condition verbatim, and say whether it is met.** Every point
   closed is not the same as the exit condition being met; check it, do not
   infer it from the count. A reserve still open means it is not met.

   Then say **how much of the development was proved by reading**: how many
   closed points carry a `check`, `weak` or `shallow`, against how many
   carried none. Report the two separately. A plan that is mostly shallow is
   not thereby wrong — it may be documentation, where little else is possible —
   but whoever reads the summary is entitled to know it before treating "all
   points closed" as evidence. A plan that is mostly `weak` is a different
   claim: that nothing in it could be executed, which is rarely true and worth
   doubting out loud.

8. **Say if the plan is finished** — `state` is `finished` and `reservesOpen`
   is empty. Then it is ready to be archived to
   `.claude/plans/`, and the promotion pass is due over its register entries.

9. **Say when the last review request was written**, from the heading of
   `REVIEW-REQUEST.md`, and how many plans have closed since. This is the only
   skill that can notice: `/plan-next` sees a point and `/plan-explain` sees the
   next one.

   **At a phase gate** — the plan's `Phase:` closing against `docs/SIM-REQ.md`
   §18 — say a request is owed. Moment two, and the only moment anything in this
   mechanism looks at the whole rather than at one point. Say it is owed; do not
   write it.

10. Stop. Do not start or close anything.

## `Kind`, and reading a plan written before a rule changed

`Kind: simulator` means the points change the simulated world and each must
carry a `Serves` field; a point without one is a gap, and the summary says so
rather than passing over it. `Kind: process` means the points change how the
work gets done, and no point carries the field — its absence is not a gap and is
not reported as one.

Plans predating a format change are read as they are, not corrected. A plan with
no `Kind` line is a plan from before that field existed; a process plan whose
points still carry `Serves` fields is the same. Report what is there. The plan is
a frozen contract, and a skill that tidied it would be editing the contract it
exists to report on.

## What this skill does not do

- It does not write, stage, commit, or run anything that changes the tree.
- It does not explain the next point in detail: that is `/plan-explain`.
- It does not judge whether the work was good. It reports what was recorded,
  including the parts that reflect badly on the record.
