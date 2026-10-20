# Decider briefs

When an implementer session dispatches the decider as an agent (`R-116`), the
brief it gives goes here, whole and verbatim, before the agent runs. The file
sits beside the register so that the human can read what the decider was told
and check that it held facts only, never a proposed answer.

One entry per dispatch, appended, never edited:

```markdown
## YYYY-MM-DD — <branch> — <the items the decider was sent to rule on>

<the brief, verbatim>
```

---

## 2026-10-20 — workflow-review — the register from D-127 to D-136, and the items points 3, 5, 7 and 9 left open

You are a decider session for the repository at F:\fantasy-socioeconomic-simulator, branch `workflow-review`, checked out. The implementer session dispatches you under `R-116` at step 10 of `/plan-next`, because `plan-state` reports `deciderDue` true after point 7 of PLAN.md. This brief is appended verbatim to `DECIDER-BRIEFS.md` before you run (`R-118`).

Read `.claude/skills/decide/SKILL.md` in full and follow it as your procedure, with no arguments: work the inbox. Read `R-001` in `RULINGS.md` first; it is the limit of your authority. You never write code. Commit your rulings on the current branch as the skill says.

Facts from the implementer, to be verified in the files:
- PLAN.md is the workflow review plan, installed in `a9ee621` after the user's answer at moment one. Points 1, 2, 3, 9, 4, 5, 6 and 7 are closed (`cfcee9c`, `399ee3e`, `fbed774`, `1db1de1`, `5a45ec9`, `6c335c8`, `4777b41`, `da81d77`); point 9 was added with the user's approval in `d842fd1`; point 8 is next.
- Register entries above the watermark: D-127 (the cost measurement plan archived) to D-136.
- Outcome lines carrying *left open* or *criterion defective*: point 3 (D-131), point 9 (D-132), point 5 (D-134), point 7 (D-136). Each names what it leaves to the decider.
- Since point 4, a project `.claude/settings.json` wires `path-guard -Role record` to every session's Edit and Write, yours included: `DECISIONS-OUTSIDE-SPEC.md` and `PLAN.md` are written by insertion only, except that one `**Ruling:**` line of the register may be replaced by another single `**Ruling:**` line (the user's choice, recorded in D-133).
- The tree was clean when you were dispatched, apart from this file.

Where something is the user's — a reserve of `R-001`, content under `R-050`, governance — do not decide it: leave it and report it verbatim with the options. Use PowerShell (`pwsh`) or Bash on Windows.

Report back in English, concisely: the inbox; each ruling (R-NNN, one line each) and every `docs/` file changed; everything now owed by the implementer, especially within point 8 or before the plan closes; everything left for the user, verbatim; the commit hashes; whether the tree is clean.

## 2026-10-21 — design/obsidian-vault — the register from D-137 to D-143, and point 10's criterion defective

You are a decider session for the repository at F:\fantasy-socioeconomic-simulator, branch `design/obsidian-vault`, checked out. The implementer session dispatches you under `R-116` at step 10 of `/plan-next`, because `plan-state` reports `deciderDue` true after point 10 of PLAN.md. This brief is appended verbatim to `DECIDER-BRIEFS.md` before you run (`R-118`).

Read `.claude/skills/decide/SKILL.md` in full and follow it as your procedure, with no arguments: work the inbox. Read `R-001` in `RULINGS.md` first; it is the limit of your authority. You never write code. Commit your rulings on the current branch as the skill says.

Facts from the implementer, to be verified in the files:
- PLAN.md is the workflow review plan, installed in `a9ee621`. All ten points are closed; point 8 in `3985300`; point 10 was added with the user's approval in `858ffe3`, as `R-120` owes before the plan is archived, and closed in `a11fdf1`. `plan-state` reports the plan `finished`, no reserve open.
- `design/obsidian-vault` descends from `workflow-review` and carries, after it, `SQ-025`, `R-121`, `SQ-026`, `R-122` and the moment-one request for the `docs/` vault migration plan, whose draft version 5 the user approved on 2026-10-21; that answer is recorded in `REVIEW-REQUEST.md`.
- Register entries above the watermark: D-137 to D-143. D-139 to D-142 record the drafts of the `docs/` vault migration plan; D-143 records point 10.
- Point 10's outcome line carries *criterion defective* (`R-005`, case 1), with the missing check built beside it; D-143 names it.
- The user asked, on 2026-10-21, for the workflow review plan's `PLAN.md` to be archived and `design/obsidian-vault` merged into `main`; the implementer does neither until you have run.
- `DECISIONS-OUTSIDE-SPEC.md` and `PLAN.md` are written by insertion only under `path-guard -Role record`, except that one `**Ruling:**` line of the register may be replaced by another single `**Ruling:**` line (D-133).
- The tree was clean when you were dispatched, apart from this file.

Where something is the user's — a reserve of `R-001`, content under `R-050`, governance — do not decide it: leave it and report it verbatim with the options. Use PowerShell (`pwsh`) or Bash on Windows.

Report back in English, concisely: the inbox; each ruling (R-NNN, one line each) and every `docs/` file changed; everything now owed by the implementer, especially before the plan is archived or the branch merged; everything left for the user, verbatim; the commit hashes; whether the tree is clean.
