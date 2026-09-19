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
  `docs/`. Say so and stop. It is not yours to fix either, and it is the most
  valuable thing this process can find.

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
exists, run it — the check was stronger than the plan claimed, which is a
finding for the outcome line and the register. The criterion stays as written.

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
  Nothing else may carry it — not a commit that amends the plan, adds a point,
  or prepares one. That is what keeps `/plan-status` a lookup instead of a
  guess.

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

**Never declare the plan abandoned.** That is a human's word — a procedure that
could abandon its own plan when the work got hard is not a procedure. Report the
failure and let them choose.

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
