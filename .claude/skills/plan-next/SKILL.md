---
name: plan-next
description: Use when asked to start or carry out the next point of PLAN.md, to continue the current development, or to go ahead with a plan. Writes. For an explanation without doing the work use /plan-explain, for where things stand use /plan-status.
---

# /plan-next

Carry out one point of the plan, then stop. One point, one commit, one report.

The rules this serves are in `AGENTS.md`; the reasoning is in
`.claude/design/2026-09-17-plan-e-skill.md`, and for the agents in
`.claude/design/2026-09-30-agent-workflow.md`. This skill is the procedure.

This session is the **orchestrator**. It reads the point and stops on it, runs
the mechanical checks, arbitrates findings and reports. Everything else is
dispatched, through the Agent tool, to the agents of `.claude/agents/`, named
below by their `subagent_type`. No agent receives this conversation: what it
needs is in its brief or nowhere.

## Before starting

```
pwsh -NoProfile -File .claude/hooks/plan-state.ps1
```

Its JSON is the plan's state, and this skill does not work it out again in its
own words. It reads outcome markers in both forms (`AGENTS.md`, "Language"), and
so does this skill: `*Esito:*`, `FALLITO`, *Chiuso con riserva*, *Riserva
sciolta* and *criterio difettoso*, from plans written before the rule, mean what
their English forms mean.

- **`warning` set:** a live `PLAN.md` sits on `main`. Refuse, and report it
  (`R-048`).
- **`state` is `none`:** refuse. Work here starts from a written plan, and
  writing it is the conversation where a human decides what the work is. Do not
  offer to write one. If what was asked for is trivial under the exemption in
  `AGENTS.md`, it needed no plan and no invocation of this skill; say so and do
  the work directly.
- **`abandoned`:** refuse. The plan stopped on purpose and only a human
  restarts it.
- **`stopped`:** refuse. Report the point at `stoppedAt` and what its outcome
  line says.
- **`finished`:** do not invent more. Check the exit condition, and say the plan
  is ready to be archived to `.claude/plans/` and the promotion pass run over its
  register entries. If `reservesOpen` is not empty the plan is not finished:
  name each reserve and the human action it waits on.
- **`running`:** the point to run is `next`. Points run in file order, a point
  added later than the ones after it included.

## The ten steps

### 1. Read the point — orchestrator

**First read "Owed by the implementer" in `RULINGS.md`**, as `AGENTS.md`
requires before each point. A ruling owed within this point is part of its work,
and its `R-NNN` is cited in the commit.

Then the point: its statement, what it cites, its check, its `Fails when:` and
its `Core:`. If the criterion can be read two ways, **stop and ask.** A
criterion interpreted is a criterion invented, and the plan is frozen precisely
so that the interpretation is not the author's to make.

**Open every identifier the point cites and read what it actually says** — in
`docs/`, `RULINGS.md` or the register — not the plan's summary of it, and not
memory of a document read earlier. A citation that is plausible and wrong passes
every other check in this mechanism; reading it is the only place it can be
caught. If it does not hold, **stop**. Two cases:

- The point cites the wrong identifier, or the right one for the wrong reason —
  a defect in a frozen plan. Report it and ask; the criteria are not yours to
  correct.
- The requirement itself is contradictory, unachievable or wrong — a defect in
  `docs/`. **Write it in `SPEC-QUESTIONS.md`**, with the citation and whether it
  blocks. It is not yours to resolve; it is the most valuable thing this process
  can find, and it is lost the moment it is only said out loud.

A blocking entry **fails the point** and stops the plan, wherever the objection
surfaces — here, in the coder's report, in a finding. A non-blocking one is
recorded and the work continues. Either way **a review request is owed**: moment
three. The same holds when the promotion pass produces candidates.

**On a plan's first point, refuse until the user's answer to its moment-one
request is recorded** — in the commit that installed `PLAN.md`, which
`AGENTS.md` requires to cite it, or in the request itself. Blocking question or
not, and the decider's answer does not count (`R-049`). Say what is missing and
stop.

On the first point of a `Kind: simulator` plan, also open each `AC-` of the
header's `Satisfies:` in `docs/SIM-REQ.md`. Missing, or citing a criterion that
says something else, it is a defect in the plan: report it and ask. Under `Kind:
simulator` a point without `Serves` is a defect too; under `Kind: process` the
field is absent by design. A plan with no `Kind` line was written before the
rule: read it as it is.

### 2. Tests first — `tester`

Dispatch `tester` with the point verbatim and nothing else: no plan of the
implementation, no reasoning of yours. It writes the tests the "Closed by"
names, under `core/Tests/`, and reports each one's state. A test already green
before any code exists is reported, and you decide which it is: the behaviour
already exists, or the test cannot fail.

A check that is not a test under `core/Tests/` — a script's self-test, a grep —
is not the tester's to write. It runs it as it stands and reports its state; the
check is written with the point's other files at step 3.

### 3. Implement — `coder`

Dispatch `coder` with the point verbatim, the `R-NNN` owed, and the tester's
tests by name, without the tester's reasoning beyond what its tests say. Nothing
in `AGENTS.md` is waived: remainder accumulators, no iteration order not derived
from integer ids, nothing that influences a future tick outside the state hash.

Files under `.claude/agents/`, `.claude/hooks/` and `.claude/skills/` are not the
coder's. This session writes them, and only in a point that names them (`R-010`).

Hash the coder's denylist before the dispatch and after it returns:

```
pwsh -NoProfile -Command "& ./.claude/hooks/tree-state.ps1 -Path docs,core/Tests,PLAN.md,DECISIONS-OUTSIDE-SPEC.md,SPEC-QUESTIONS.md,REVIEW-REQUEST.md,PROMOTIONS.md,RULINGS.md,core/BannedSymbols.txt,.claude/agents,.claude/hooks,.claude/skills,.claude/settings.json,.claude/settings.local.json"
```

A difference means the coder wrote round its hook: show `git status --short` and
stop.

The coder returns the files touched, its decisions, its doubts as questions, and
the tests that need changing. A test that must change because an interface
legitimately changed is changed by `tester`, dispatched again with the coder's
reason.

### 4. Verify, and make it fail — `tester`

Hash everything outside `core/Tests/` before the dispatch and after it returns:

```
pwsh -NoProfile -Command "& ./.claude/hooks/tree-state.ps1 -Path ':!core/Tests'"
```

A difference stops the point.

Dispatch `tester` again. It runs the declared check, then, whenever the point
touched code, the build and both suites its definition lists. Then it introduces
each fault the `Fails when:` names, through `.claude/hooks/mutate.ps1`, sees the
check red for that reason, and the fault is undone; where the point's code has a
body that can be emptied, the empty implementation too. The outcome line says
each fault was seen. A plan written before `R-020` has no `Fails when:` line, and
the point is not faulted for the absence.

A fault `mutate` cannot reach — a path outside `core/Runtime/` and `harness/` —
is introduced by this session: the check run, the fault undone, and
`tree-state.ps1` over the whole tree equal before and after. The outcome line
says who introduced it.

A check that stays green with the fault in place has not proved the point: that
is a defective criterion, handled at step 8.

**Run the check, do not read it.** On a point that builds nothing — a document,
a skill — the temptation is to reread the work and call it verified. That is
where this repository has already found a defect that reading had missed.

**A `Check: weak` marker is a claim, and it is tested here.** Look for something
mechanical first: a grep, a file that exists or does not, a command that exits
non-zero, a diff that is empty. If one exists, run it: the check was not weak
but `shallow`, a finding for the outcome line and the register, and the
criterion stays as written. `weak` means nothing can be executed — rare, and
usually wrong; `shallow`, that it executes and proves little. Run it anyway: a
shallow check still catches the gross failures.

A check that reads `PLAN.md` itself is run after step 8, on the state the commit
will contain.

### 5. The diff against the rules — orchestrator

Mechanical:

- `git diff --name-only HEAD -- core/`. If the point declared `Core: no` and this
  prints anything, the reason it gave was false: the revisioners run, and the
  contradiction is recorded. A downgrade nobody verifies is a downgrade
  available whenever review is inconvenient.
- No path under `docs/` in the diff.
- No new `#pragma warning disable` or `SuppressMessage`: a suppression is a
  decision, and decisions belong to a human.
- `AGENTS.md`, or any `CLAUDE.md`, in the diff only if the point's text names
  it (`R-010`, `R-070`).

Any of the last three failing stops the point, and you ask.

### 6. Review — `code-reviewer` and `revisioner`

In one message, in parallel:

- **`code-reviewer`**, on every point that touches code, whatever its `Core:`.
  Only a point that touches no code — a document, a skill — skips it, and the
  outcome line says so.
- **Two `revisioner`s, one briefed and one blind**, unless the point declared
  `Core: no` with a reason that step 5 found to hold. `Core:` defaults to yes; a
  point with no reason is treated as yes and the missing reason reported. It was
  declared when the plan was written, and is not reconsidered now that the
  reviewers look expensive.

The briefs are in `reviewers.md` in this skill's folder. Every brief quotes the
point verbatim, its `Fails when:` line included, and asks which fault the check
would miss (`R-047`). The coder's doubts go to the briefed revisioner only, as
the coder wrote them. The blind revisioner never sees the briefed one's output.

All three are read-only agents that hold `Bash`. Hash the whole tree before the
message and again after every one of them has returned:

```
pwsh -NoProfile -File .claude/hooks/tree-state.ps1
```

If the two differ, an agent changed the tree: say so first, show `git status
--short`, and stop. Undoing the change is the human's call.

### 7. Resolve the findings — orchestrator

- Verifiable against the specification, the code or a test → apply it:
  `coder` for the code, `tester` for the tests, and `tester` re-verifying as at
  step 4. **At most two rounds.** Still red after the second, the point stops
  and you report.
- A matter of taste, or the reviewers disagreeing → an open point for the human.
  Do not settle it.
- Wrong on the facts → say so, with the fact.
- A fault the check would miss → to the register with the other findings,
  accepted or rejected with its reason. The criterion does not change.

Reviewers do not run a second time. If a fresh look is needed, brief a fresh
blind revisioner.

### 8. The record — `documenter`

Dispatch `documenter` with the point's text as read at step 1, the coder's
decisions, your resolution of every finding — the rejected ones with their
reason — and the tester's results. It re-reads `PLAN.md` from disk, writes the
register entry and the outcome line by inserting only, and returns the `D-NNN`
and the line, or STOP.

**STOP** means the point's own text changed, it already carries an outcome line,
or it is gone. The work is finished and unrecorded: report that, and let the
human choose — re-verify against the new text, or discard. Other points
changing, or a point added after this one, is said in the report.

The outcome line, and so your brief, carries the date, what the check produced,
each fault seen red, the `D-NNN`, and which agents ran and which did not, with
the reason. A criterion that turned out wrong is recorded, never rewritten; the
line between the two cases is whether correct code can pass the check as
written (`R-005`):

- **Correct code passes it, but it proves less than the point asks** — it
  under-covers the "Does", its words diverge from it, or it cannot fail for the
  reason it exists. The point closes on the criterion as frozen, *criterion
  defective*, with the missing check beside it, and the plan continues. The
  decider rules on the entry, and whatever follows lands under "Owed by the
  implementer" in `RULINGS.md`, never in `PLAN.md`.
- **Correct code cannot pass it**, or passing it would build the wrong thing:
  the point fails, *criterion wrong*, as below.

**A clause that only a human can discharge closes the point with a reserve**
(`R-011`): a push, so that CI runs; a pull request; a confirmation from outside
the repository. The line reads `*Outcome:* <date>. Closed with reserve — <the
clause> awaits <the human action>.`, and the plan continues. Not for anything
this session could do: a reserve is not a way to defer work. When the human
acts, the check is run and a line added under the outcome line, never in its
place: `*Reserve discharged:* <date> — <what was done, what the check
produced>.` It is committed on its own or with the next point, and never carries
a `Plan-point:` trailer.

### 9. The commit — `committer`

Write the message file in the scratchpad. The subject names the requirement or
decision the point implements, as `AGENTS.md` requires; if no identifier
applies, the message says so rather than inventing one. A trailer `Plan-point:
<n>` means this commit closes point n, and the value is a bare integer. A commit
that closes no point — one that installs a plan, adds a point, closes a plan —
does not write the word at all, not even to say it closes nothing: that is what
keeps `/plan-status` a lookup instead of a guess.

Dispatch `committer` with the paths the point touched, by name, the message file,
and the point number. Code, register and plan go in one commit, so no part of
the record is left behind. **Never `git add -A`**: the tree may hold an edit
that is not yours, and `-A` attributes it to this point. If a file you must
stage also carries someone else's change, stop and ask.

The committer returns the hash, or a refusal with its reason; a refusal stops
the point.

### When the point fails

The check does not pass, the work turns out impossible, a citation or a blocking
`SPEC-QUESTIONS.md` entry sent the point back, or the rounds of step 7 ran out.
Close it as **failed** rather than leaving it open:

- The outcome line reads `*Outcome:* <date>. FAILED — <what was attempted, what
  the check did>.`, with a register entry as for any point: a road already
  walked is worth more written down than a road not taken. The commit carries
  `Plan-point: <n>`; a point closed as failed is closed.
- **The commit leaves the repository green.** Keep whatever of the attempt
  survives the build and the suite; describe the rest in the register and
  discard it. A red commit makes every later bisect lie.
- **Then stop the plan**, not just the point. Later points may rest on this one.
- **A wrong criterion fails the same way.** The decider rules on its entry, and
  the work resumes as a new point added with the human's approval (`D-014`),
  never as an amendment of the failed one.

**Never declare the plan abandoned.** That is a human's word: a procedure that
could abandon its own plan when the work got hard is not a procedure.

### 10. Report — orchestrator

What was done, what the check produced, what each agent returned — the findings
rejected included — and the next point, from `plan-state` run again. Then stop:
this skill does not chain.

**End with one line for the decider**, from the `decider` field of that run:

```
Decider: N register entries not yet reviewed (D-xxx to D-yyy) — run /decide
```

With none outstanding the line still appears, saying zero. It is a reminder to
the human and nothing more: this session does not run `/decide` and does not
play the decider (`AGENTS.md`).

## What this skill does not do

- It does not merge, push, or open a pull request unless asked.
- It does not delete `PLAN.md`. That belongs to the last commit before the
  merge, after the pull request is approved.
- It does not add points to the plan. A plan may gain one, but that is a human's
  approval, not a step in this procedure.
