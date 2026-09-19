# Plan — the point lookup, qualified by plan

**Kind:** process
**Exit condition:** `/plan-status` resolves every closed point of every plan in
`.claude/plans/` to exactly one commit, and resolves nothing for a point number
a plan never carried.

**Phase:** none, process work.
**Branch:** `main`, one point, no branch of its own — see the point's own note.

---

Approved on 2026-09-20. The defect was found by the read-only agent dispatched
at point 11 of the twelve-gaps plan and recorded as `D-030`, which said it
needed its own point rather than a correction smuggled into another one.

## 1. The lookup is bounded by the plan's commit range

- **Does:** `/plan-status` resolves a point's commit within the range of the
  plan that contains it, rather than across the whole history. The range is the
  archive commit of the previous plan, exclusive, to the archive commit of this
  plan, inclusive — or to `HEAD` for the plan still live. Archive commits come
  from `git log --diff-filter=A -- .claude/plans/`.
- **Closed by:** run against all three archived plans, every closed point
  resolves to exactly one commit, and point 3 of the review-request plan — a
  number that plan never carried — resolves to none. Today, unbounded, points 1
  and 2 return three commits each and point 3 returns two.
- **Core:** no — touches no file under `core/`.

*Esito:* 2026-09-20. `/plan-status` step 4 bounds the lookup to the plan's
range, derived from `git log --reverse --diff-filter=A -- .claude/plans/`. Run
across all three archived plans: 21 closed points, each resolving to exactly one
commit, and the first number past each plan's last point resolving to none.
Unbounded, points 1 and 2 returned three commits each. The check's first version
passed vacuously — see `D-043`, which is the more useful half of this point.
Decisions: register, `D-042` and `D-043`. Review: not run, `Core: no`, exemption
verified against the diff.

No `Check:` marker: this one runs and proves what it claims. It is the first
point in four plans of which that is true without qualification, which is a
comment on the three plans before it rather than on this one.

**On the branch:** this point is committed to `main` directly, against the habit
of the last three plans. One point, no code, and the human is about to open a
clean session against `main`; a branch would leave them a merge to do before
they can start. Recorded rather than done quietly.
