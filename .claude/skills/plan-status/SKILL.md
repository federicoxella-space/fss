---
name: plan-status
description: Summarises where the current development stands - points closed with their commits, decisions and review findings, the point in progress, what remains, and the exit condition. Use when resuming work after a break, when asked how far along something is or what has been done so far, or before closing a branch. Read-only companion to /plan-next and /plan-explain.
---

# /plan-status

Report where the development stands, and stop. This skill changes nothing.
Asking how far along the work is must never be able to move it.

Its subject is what has already happened. For what comes next in detail, that is
`/plan-explain`.

## Procedure

1. Read `PLAN.md` at the repository root. **If it is absent**, say so: either no
   development is in progress, or the last one closed and its plan was deleted
   before the merge, which is what closing a branch does. `git log` holds the
   history either way. Stop.

2. Read the header: the exit condition, the phase, the branch, and `Kind` if the
   plan declares one.

3. **Split the points.** A point is closed when it carries an `*Esito:*` line,
   and open otherwise. Nothing else counts — not how finished the work looks,
   not what the commits suggest.

4. **For each closed point, resolve its commit exactly:**

   ```
   git log --grep="Plan-point: <n>" --format="%h %s"
   ```

   `Plan-point: <n>` means **this commit closes point n**, and nothing else may
   carry it — a commit that amends the plan, adds a point, or prepares one does
   not. That is what makes this a lookup and not a guess. Report what it
   returns. If it returns nothing for a point that carries an outcome line, say
   so plainly rather than searching for a likely candidate: an outcome line with
   no commit behind it is the one state this design cannot distinguish from
   finished work, and it needs a human. If it returns more than one, report them
   all and say the trailer has been used loosely somewhere, which needs a human
   too.

5. **For each closed point, report** its number and title, the commit, the
   register section its outcome line points to, and the review outcome recorded
   there — including "not run, `Core: no`", and including findings that were
   raised and rejected. A rejected finding that vanishes from the summary is a
   rejection nobody can audit.

6. **Report what remains:** the open points by number and title, which one is
   next, and whether any of them declares `Core: yes` and will therefore need
   the two reviewers.

7. **Report the exit condition verbatim, and say whether it is met.** Every point
   closed is not the same as the exit condition being met; check it, do not
   infer it from the count.

8. **Say if the plan is finished.** Then the branch is ready to close, and
   `PLAN.md` is deleted in the last commit before the merge, once the pull
   request is approved.

9. Stop. Do not start or close anything.

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
