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
  one. If what was asked for is trivial under the exemption in `AGENTS.md` — no
  change in behaviour, no decision, one commit — it needed no plan and no
  invocation of this skill; say so and do the work directly.
- **Abandoned** — the header carries an `**Abandoned:**` declaration: refuse.
  The plan stopped on purpose and only a human restarts it.
- **A point closed as failed** — an outcome line saying `FALLITO`, with points
  still open after it: refuse. The plan stopped at that failure and has not been
  restarted. Report which point failed and what it says.
- **Every point closed:** do not invent more. Say the plan is finished, check
  the exit condition, and say that the plan is ready to be archived to
  `.claude/plans/` and the promotion pass run over its register entries. If a
  point is *Chiuso con riserva* with no *Riserva sciolta* line under it, the
  plan is not finished: name the reserve and the human action it waits on.
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

**Then open every identifier the point cites and read what it actually says.**
`FR-P-13`, `AC-01`, `DEC-002` — find each one in `docs/` and confirm it says
what the point claims. Do not work from the plan's summary of it, and do not
work from memory of a document read earlier.

A citation that is plausible and wrong passes every other check in this
mechanism: the work gets done, the test goes green, the register records a
decision against a requirement that says something else, and the plan reads as
though the specification had been consulted. That is the drift this whole
apparatus exists to stop, and reading the citation is the only place it can be
caught.

If the citation does not hold, **stop.** Two cases, and they are not the same
one:

- The point cites the wrong identifier, or the right one for the wrong reason —
  a defect in a frozen plan. Report it and ask; the criteria are not yours to
  correct.
- The requirement itself is contradictory, unachievable, or wrong — a defect in
  `docs/`. **Write it in `SPEC-QUESTIONS.md`**, with the citation and whether it
  blocks. It is not yours to fix and not yours to resolve; it is the most
  valuable thing this process can find, and it is lost the moment it is only
  said out loud.

A blocking entry **fails the point** and stops the plan. A non-blocking one is
recorded and the work continues. This holds wherever the objection surfaces —
step 1, the implementation, a reviewer's finding — not only here.

Either way, **a review request is owed**: moment three. The work has met the
specification, and that is one of the three things worth interrupting a human
for. The same applies when the promotion pass produces candidates.

**On the first point of a `Kind: simulator` plan, confirm the header's
`Satisfies:` too.** Open each `AC-` in `docs/SIM-REQ.md` and read it. The field
is mandatory in a simulator plan; missing, or citing a criterion that says
something else, it is a defect in the plan — report it and ask. It is checked
once, at the first point, not at every one.

`Kind` in the plan header decides whether the point carries a `Serves` field.
Under `Kind: simulator` it is mandatory, and a point without one is a defect in
the plan: stop and ask, the same as for an ambiguous criterion, rather than
deciding for yourself which requirement the point serves. Under `Kind: process`
the field is absent by design and its absence means nothing.

A process plan that still carries `Serves` fields, or a plan with no `Kind` line
at all, was written before this rule. Read it as it is; a frozen contract is not
tidied.

**On the first point of a plan, say a review request is owed.** Moment one of
`REVIEW-REQUEST.md`: the plan is short and a wrong criterion costs one
conversation here against every point built on it later. Say it is owed; do not
write it for them, and do not treat having said so as permission to continue if
the human has not answered a blocking question.

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

**A `Check: weak` marker is a claim, and it is tested here.** Before accepting
that reading is all there is, look for something mechanical: a grep, a file that
exists or does not, a command that exits non-zero, a diff that is empty. If one
exists, run it — the check was not weak but `shallow`, which is a finding for
the outcome line and the register. The criterion stays as written.

The two are different claims and the outcome line reports which one held:

- **`Check: weak`** — nothing can be executed. Rare, and usually wrong.
- **`Check: shallow`** — it executes and proves little. Run it anyway: a shallow
  check still catches the gross failures, and running it is what tells you it
  was shallow rather than weak.

A point whose check reads `PLAN.md` itself is verified after step 6, on the
state the commit will contain; verifying earlier measures a state that never
ships.

**Then check the `Core:` declaration against the diff:**

```
git diff --name-only HEAD -- core/
```

If the point declared `Core: no` and this prints anything, the reason it gave was
false. The reviewers run, and the contradiction is recorded in the register. A
downgrade nobody verifies is a downgrade available whenever review is
inconvenient.

### 4. Review, unless the point earned an exemption

`Core:` defaults to **yes**. The reviewers run unless the point declared
`Core: no` with a reason, and step 3 found that reason to hold. A point with no
reason is treated as `yes` and its missing reason reported; a point whose reason
the diff contradicts is treated as `yes` and the contradiction recorded.

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

**First re-read `PLAN.md` from disk**, and compare the point against what step 1
read. The work took time, and a human may have edited the plan during it; an
append onto a remembered version discards that edit without either of you
seeing it happen.

| What changed | What to do |
|---|---|
| Nothing | Append. |
| The point's own text — its statement, citations, check, `Core:` | **Stop, do not append.** The work was done against a criterion that no longer exists. Report that the work is finished and unrecorded, and let the human choose: re-verify against the new text, or discard. |
| The point already carries an `*Esito:*` line | **Stop.** Someone else closed it. Nothing here is safe to assume. |
| The point is gone, or the plan was replaced | **Stop.** |
| Other points, or a point added after this one | Append, and say so in the report. Their text is not this point's contract. |

Then, in this order:

1. The entry in `DECISIONS-OUTSIDE-SPEC.md`, under this development's section,
   naming the plan point. It holds the choices the specification does not make,
   and the findings that were **rejected**, with the reason. A rejection nobody
   can audit is most of the value thrown away.
2. The `*Esito:*` line appended to the point in `PLAN.md`: the date, what the
   check produced, the register reference, and the outcome of the review —
   including "not run, `Core: no`".

**Anchor both appends on text you have just read**, with an exact-match edit
rather than a rewrite of the file. An edit that cannot find its anchor fails and
says so; a rewrite from memory silently wins against whatever the human wrote.
The re-read above diagnoses the divergence, but this is what makes missing it
survivable.

Everything above the outcome line stays untouched. A criterion that turned out
to be wrong is reported and recorded, never rewritten to match what was built.
Two cases, and the line between them is whether correct code can pass the check
as written (`R-005`):

- **Correct code passes it, but it proves less than the point asks** — it
  under-covers the "Does", its words diverge from it, or it cannot fail for the
  reason it exists. Close the point on the criterion as frozen, with the missing
  check written beside it. The outcome line says *criterio difettoso* and names
  the register entry; the plan continues. The decider rules on the entry at its
  next audit, and whatever follows lands under "Owed by the implementer" in
  `RULINGS.md`, never in `PLAN.md`.
- **Correct code cannot pass it**, or passing it would build the wrong thing.
  That is a failure: close the point as below, with *criterio sbagliato* as the
  reason.

**A clause that only a human can discharge closes the point with a reserve**
(`R-011`). The work is done and verified, and what is missing is an action this
skill may not take — a push, so that CI runs; a pull request; a confirmation
from outside the repository. The outcome line reads `*Esito:* <date>. Chiuso con
riserva — <the clause> attende <the human action>.`, and the plan continues.
Not for anything you could do yourself: a reserve is not a way to defer work.

When the human acts, the check the clause names is run and a line is added
under the outcome line, never in place of it: `*Riserva sciolta:* <date> — <what
was done, what the check produced>.` It is committed on its own or with the next
point, and never carries a `Plan-point:` trailer. A reserve still open keeps the
exit condition unmet, whatever the point count says.

### 7. Commit

Code, register and plan in one commit, so no part of the record can be left
behind uncommitted.

**Stage the paths the point touched, by name. Never `git add -A`.** The tree may
hold an edit that is not yours — the one step 6 looks for — and `-A` absorbs it
into this point's commit, where it is attributed to the point and to whoever
wrote the message. If a file you must stage also carries someone else's change,
stop and ask rather than deciding for them what their edit belongs to.

- The subject names the requirement or decision it implements, as `AGENTS.md`
  requires. If no identifier applies, say so in the message rather than
  inventing one.
- A trailer line `Plan-point: <n>`. **It means this commit closes point n.**
  The value is a bare integer and nothing else; a commit that closes no point —
  one that installs a plan, adds a point, closes a plan — **does not write the
  word at all**, not even to say it closes nothing. That is what keeps
  `/plan-status` a lookup instead of a guess, and it has already been broken
  three times by the author of the rule, each time by writing prose where the
  integer goes.

### When the point fails

The check does not pass, the work turns out impossible, or a citation sent the
point back. Close it as **failed** rather than leaving it open:

- Outcome line: `*Esito:* <date>. FALLITO — <what was attempted, what the check
  did>.` Register entry as for any point; a road already walked is worth more
  written down than a road not taken.
- Commit with `Plan-point: <n>`. A point closed as failed is closed.
- **The commit must leave the repository green.** Keep whatever of the attempt
  survives the build and the suite so the next person can see it; describe the
  rest in the register and discard it. A red commit makes every later bisect
  lie.
- **Then stop the plan**, not just the point. Report and wait. Later points may
  rest on this one, and a failure is information the human needs before more
  work is spent against it.
- **A wrong criterion fails the same way.** The decider rules on its register
  entry, and the work resumes as a new point added with the human's approval
  (`D-014`), never as an amendment of the failed one.

**Never declare the plan abandoned.** That is a human's word — a procedure that
could abandon its own plan when the work got hard is not a procedure. Report the
failure and let them choose.

### 8. Stop

Report: what was done, what the check produced, what the reviewers found
including what was rejected, and what the next point is. Then stop. The next
point does not start by itself, and this skill does not chain.

**End the report with one line for the decider**, counted rather than
estimated. The watermark in `RULINGS.md` is the last `D-NNN` the decider
audited, or `none`; the line counts the register entries above it:

```
w=$(grep -m1 -oE '^\*\*Register watermark:\*\* (none|D-[0-9]+)' RULINGS.md | grep -oE '[0-9]+$'); w=${w:-0}
grep -oE '^### D-[0-9]+' DECISIONS-OUTSIDE-SPEC.md | grep -oE '[0-9]+$' | awk -v w="$w" '$1+0>w{n++; if(!f)f=$1; l=$1} END{print n+0, f, l}'
```

```
Decider: N register entries not yet reviewed (D-xxx to D-yyy) — run /decide
```

With none outstanding the line still appears, saying zero. It is a reminder to
the human and nothing more: this session does not run `/decide` and does not
play the decider (`AGENTS.md`).

## What this skill does not do

- It does not merge, push, or open a pull request unless asked.
- It does not delete `PLAN.md`. That belongs to the last commit before the
  merge, after the pull request is approved, while the plan is still useful to
  whoever is reviewing it.
- It does not add points to the plan. A plan may gain one, but that is a human's
  approval, not a step in this procedure.
