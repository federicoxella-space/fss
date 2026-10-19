# Plan — the workflow review, applied

**Kind:** process
**Exit condition:** the directions the user approved in the workflow review of
2026-10-19 and 20, recorded in `.claude/design/2026-10-19-workflow-review.md`
(review items A, B, P1 to P4, G2, C3), written into `AGENTS.md`, the plan
skills and the hooks, each script's self-test green, and point 4's live check
run in a new session. Whether they make criteria stronger and points lighter is
measured by the next plan, not this one (`R-046`).
**Phase:** none, process work; before phase 4.
**Branch:** `workflow-review`, cut from `main` after the answer to moment one.

The review's labels are written "review A", "review C3" and so on, so as not to
be read as the rows C1 to C10 and S1 of `AGENTS.md`'s table, written "table C8".
No point touches a path under `core/`. Points that edit `AGENTS.md`,
`.claude/agents/`, `.claude/hooks/` or `.claude/skills/` say so, which is the
permission `AGENTS.md` requires (`R-010`, `D-087`). Points 1 to 6 run under the
`/plan-next` of today; point 6 rewrites it, and points 7 and 8 run under the
rewritten one, which is read afresh at each invocation.

## 1. The criteria checklist (review B)

- **Does:** adds `.claude/skills/plan-explain/criteria.md`: rules numbered `K1`
  onward, each a heading of its own, each saying what a point's "Closed by" must
  prove and citing the ruling that found the gap:
  - `K1` a computation is checked against an independent reference, on a state
    where the branch that matters binds (`R-101`, `R-103`);
  - `K2` a check of equality between two paths that share their code and their
    geometry cannot fail where they coincide; the check needs a reference that
    shares neither (`R-103`);
  - `K3` a reported number is recomputed from an independent source, and counts
    the quantity the specification defines, not the one the code holds
    (`R-102`);
  - `K4` cells that are compared are measured alike, in one run, and a cell the
    point names is measured or marked missing, never derived (`R-104`);
  - `K5` a check that sums or compares its own outputs proves their arithmetic,
    not their labels (`R-105`);
  - `K6` a `Fails when:` that rests on a premise is read again when a later
    ruling touches that premise (`R-105`).
  Edits `.claude/skills/`. Adds `.claude/hooks/criteria-cite.ps1`: every `K`
  heading must carry at least one identifier, and every identifier must have a
  heading in `RULINGS.md` or `DECISIONS-OUTSIDE-SPEC.md`. Edits
  `.claude/hooks/`.
- **Closed by:** `criteria-cite.ps1 -SelfTest` green, then the script on the real
  checklist exits 0; and the code-reviewer reads each citation against the
  ruling it names and finds none that says something else (table C1).
- **Fails when:** a rule cites `R-999`, or cites nothing, or cites a ruling that
  exists and says something else; the first two turn the script red on a copy of
  the checklist, the third is put to the code-reviewer as a planted citation
  (`K2` citing `R-102`) and must be reported.
- **Core:** no — the diff touches only `.claude/skills/plan-explain/` and
  `.claude/hooks/`.

*Outcome:* 2026-10-20. Closed: `criteria-cite.ps1 -SelfTest` ran 10 cases, 0
failed, and the script on the real checklist exits 0; the code-reviewer read
every citation against `R-101` to `R-105` and found none that says something
else. The faults went red: `K1` citing `R-999` (exit 1), `K4` citing nothing
(exit 1), the script's body emptied (self-test 7 of 10 failed), and the planted
`K2` citing `R-102` was reported by the code-reviewer while the script gives it
exit 0, as the point expects. The code-reviewer's two verifiable findings
(a `K` rule at another heading level, a duplicate `K1`) and one inference in
`K3` are applied; the rest are taste or answered, rejected in `D-129`. The check
misses a citation that exists and says something else, and a rule drifting from
its ruling with the citation right; recorded beside it. `D-129`. Agents
(`R-067`): tester, code-reviewer, documenter and committer dispatched. Not run:
the coder, the files being in `.claude/`; the revisioners, `Core: no` held at
step 5; the tester at step 4, the orchestrator ran the self-test and the faults.
This outcome does not claim `R-046`'s check.

## 2. Criteria attacked before they freeze (review A)

- **Does:** `/plan-explain -Draft` reads every point of a draft, not the first
  only, and for each answers which fault the check would miss and which part of
  the "Does" it leaves unchecked, against `criteria.md`. The reading is
  dispatched to a read-only agent given the draft, the checklist and the files
  the draft names, told not to read `RULINGS.md`, the register or
  `.claude/plans/`, and given nothing of the author's reasoning. The skill
  accepts `-Path` for a draft held in another file. Edits
  `.claude/skills/plan-explain/SKILL.md`. In `AGENTS.md`, "Criteria are frozen
  when written" gains the rule: before moment one the draft is read this way and
  the answers go into the request, quoted, beside the draft, for the user to
  amend it or not. Edits `AGENTS.md`. `REVIEW-REQUEST.md`'s "What a request
  holds" names the answers as part of a moment-one request.
- **Closed by:** the fixture `.claude/hooks/fixtures/plan-explain/misura-costi.md`
  is the request as committed in `64147ba`, before the user's answer and before
  any outcome existed. `/plan-explain -Draft -Path` on it returns one entry per
  point, five; its entry for point 4 names supply that never runs short as a
  state where the ranking is not seen; and its report cites no `R-`
  identifier, which it was told not to read. A grep finds the rule in
  `AGENTS.md` and the line in `REVIEW-REQUEST.md`.
- **Fails when:** the skill reads the first point only, as today: one entry,
  point 4's missing. Or the reader is handed the register: its report cites
  `R-103`.
- **Check: shallow** — the reader is an agent, and `K1` is in the checklist it
  reads, so naming point 4's gap proves the brief and the checklist, not that
  the reader finds a gap the checklist does not name; what would prove it is the
  next plan's points closing without *criterion defective*.
- **Core:** no — the diff touches only `.claude/skills/plan-explain/`,
  `.claude/hooks/fixtures/`, `AGENTS.md` and `REVIEW-REQUEST.md`.

## 3. `plan-state` reads a point's frozen text and whether the decider is due (review P1, P4)

- **Does:** `plan-state.ps1` finds the commit that installed `PLAN.md`, the
  oldest commit in the plan's range that adds it, and reports per point:
  - `changed`, when the point's text differs from that commit's — the text from
    its heading to the next heading, without the lines that begin with an
    outcome marker (`*Outcome:*`, `*Esito:*`, `*Ruled:*`, `*Reserve
    discharged:*`, `*Riserva sciolta:*`);
  - `recorded`, when it already carries an outcome line; `added`, when it was not
    in the installing commit; and, at plan level, `gone`, the points of the
    installing commit no longer present.
  These are the documenter's three STOP cases (`/plan-next` step 8). And at plan
  level `deciderDue`: true when the last closed point's `*Outcome:*` line, and
  only that line, carries *criterion defective*, *criterio difettoso*, `FAILED`,
  `FALLITO` or *left open*. *Left open* is a new marker: the outcome line carries
  it when a finding is left to the human, a `SPEC-QUESTIONS.md` entry is
  raised, or an escalation is made, so that the trigger is read, not inferred.
  Edits `.claude/hooks/`.
- **Closed by:** `plan-state -SelfTest` green with a fixture repository: a plan
  installed; point 1 closed; point 2's "Closed by" edited in a later commit;
  point 3 deleted and point 4 added; `changed` names point 2 only, `gone` point
  3, `added` point 4, `recorded` point 1. And three `deciderDue` fixtures: an
  outcome line with *criterion defective* (true), a clean outcome line followed
  by a `*Ruled:* … criterion defective` line (false), a phase 3 outcome with
  `**criterio difettoso**` (true).
- **Fails when:** the baseline is the latest commit touching `PLAN.md` rather
  than the installing one, so an edit committed with an earlier point is
  blessed: point 2 is not reported. Or the outcome markers are not excluded, so
  closing a point marks it changed: point 1 is reported.
- **Core:** no — the diff touches only `.claude/hooks/`.

## 4. The register written insert-only by a hook (review P1)

- **Does:** a project `.claude/settings.json` wires `path-guard` to every
  session's `Edit|Write` under a new role, `record`, which decides only
  `DECISIONS-OUTSIDE-SPEC.md` and `PLAN.md` and allows every other path, inside
  the repository or not: an Edit of either passes only if its `new_string`
  begins with its `old_string`; a Write only if the file does not exist. It
  applies to every session, the decider's included. Before wiring, `git log -p`
  over the decider's commits since `R-001` shows whether every decider edit of
  the register was an insertion; if one was not, the point stops and asks.
  `SPEC-QUESTIONS.md` is left out: the decider closes entries by editing their
  status line. Writes through a shell are not seen by the hook, a limit stated in
  `path-guard`'s description. Edits `.claude/hooks/path-guard.ps1` and its
  self-test.
- **Closed by:** `path-guard -SelfTest` green with cases for `record`: an
  insertion passes; a replacement is denied with `record`'s reason, not
  "unknown role" or "not JSON"; a Write over an existing `PLAN.md` is denied, a
  Write of a new one passes; an Edit of `README.md` and a Write to a path
  outside the repository pass. Then the exact command line in
  `.claude/settings.json`, fed the JSON of an Edit replacing a word of
  `DECISIONS-OUTSIDE-SPEC.md`, exits 2 with `record`'s reason, and fed an Edit of
  `README.md`, exits 0. Then, in a new session, a live Edit replacing a word of
  `DECISIONS-OUTSIDE-SPEC.md` is denied; on a clean tree, and if it is not,
  `git checkout -- DECISIONS-OUTSIDE-SPEC.md` restores it and the point fails.
- **Fails when:** the command line in `settings.json` names the role wrongly, so
  every Edit is denied as "unknown role": the `README.md` case goes red. Or
  `record` inherits the denial of paths outside the repository: the outside
  case goes red.
- **Reserve:** the live Edit needs a session started after `settings.json` exists,
  which only the human can start: the point closes with reserve on that clause
  (`R-011`).
- **Core:** no — the diff touches only `.claude/hooks/` and
  `.claude/settings.json`.

## 5. `mutate` reaches every file of the point (review P3)

- **Does:** `mutate.ps1` admits a patch on any path the point's diff touches —
  `git diff --name-only HEAD` and the untracked files — except `core/Tests/`,
  `docs/`, `.claude/`, `AGENTS.md`, every `CLAUDE.md`, `core/BannedSymbols.txt`
  and the record files `PLAN.md`, `DECISIONS-OUTSIDE-SPEC.md`,
  `SPEC-QUESTIONS.md`, `REVIEW-REQUEST.md`, `PROMOTIONS.md`, `RULINGS.md` and
  `DECIDER-BRIEFS.md`; a rename's source counts as a path of the patch, as
  `R-073` requires today; the tree is recorded and proved restored over the
  admitted set. `core/Tests/` is excluded because a patch that breaks a test
  turns the check red without the code being at fault. Edits `.claude/hooks/`.
  The sentences that say `mutate` reaches only `core/Runtime/` and `harness/` —
  its own description, `.claude/agents/tester.md`, `/plan-next` step 4 — say
  what it reaches now (table C8). Edits `.claude/agents/`, `.claude/skills/`.
- **Closed by:** `mutate -SelfTest` green with new cases: a patch on an
  untracked `bench/` file in the diff applies, goes red and is restored; a patch
  on a file outside the diff is refused with exit 2; a patch on `docs/` or
  `core/Tests/` is refused even when the path is in the diff; a rename from a
  path outside the set is refused. A grep finds no sentence saying `mutate`
  reaches only `core/Runtime/` and `harness/`.
- **Fails when:** the admitted set is read from `git diff` only, which never
  lists untracked files: the `bench/` case goes red. Or `core/Tests/` is
  admitted: its case goes red.
- **Core:** no — the diff touches only `.claude/`.

## 6. `/plan-next` lighter (review P1, P2, P4, G2)

- **Does:** edits `.claude/skills/plan-next/SKILL.md`:
  - step 2 dispatches the tester only when the point's check is a test under
    `core/Tests/`; otherwise the check is written with the point's files at step
    3, as the skill already says (review P2);
  - step 8 is the orchestrator's: it reads `changed`, `recorded`, `added` and
    `gone` from `plan-state` and stops on them as the documenter does today,
    then writes the register entry and the outcome line by insertion, the outcome
    line carrying *left open* when step 7 left a finding to the human, a
    `SPEC-QUESTIONS.md` entry was raised or an escalation made (review P1);
  - step 9 keeps the committer, whose `git-guard` admits only the commands a
    commit needs, and lists `git-guard`'s rules in the committer's brief: one git
    command a call, no `-C`, the trailer's word never outside the trailer, paths
    with forward slashes (review G2);
  - step 10 reads `deciderDue`: true, it appends the brief to
    `DECIDER-BRIEFS.md` and dispatches the decider (`R-116`, `R-118`); false, it
    reports the count of entries for the audit at the plan's end or the phase
    gate (review P4). The sentence "this session does not run `/decide`", which
    `R-116` overtook, goes (table C8).
  Edits `.claude/skills/`. The documenter is no longer dispatched; it is retired
  in point 7, after this point has been recorded by it.
- **Closed by:** a grep finds no dispatch of `documenter` in `.claude/skills/`,
  finds the condition of step 2, the four fields of step 8, the four rules of
  step 9 and the `DECIDER-BRIEFS.md` append of step 10, and does not find
  "does not run `/decide`".
- **Fails when:** step 8 drops one of the documenter's STOP cases, so a point
  that already carries an outcome line is recorded twice: the grep for
  `recorded` goes red.
- **Check: shallow** — the skill is prose and a grep proves the words, not the
  procedure; what would prove it is points 7 and 8 of this plan, run under it.
- **Core:** no — the diff touches only `.claude/skills/`.

## 7. The documenter retired (review P1)

- **Does:** deletes `.claude/agents/documenter.md` and the `documenter` role of
  `path-guard`, which nothing dispatches after point 6 (table S1). Edits
  `.claude/agents/`, `.claude/hooks/`. This point is the first recorded by the
  orchestrator under point 6's step 8.
- **Closed by:** `path-guard -SelfTest` green without the role; the agent file
  absent; a grep finds `documenter` nowhere in `.claude/` but the design notes
  and the archived plans.
- **Fails when:** the role is removed from `path-guard` but its self-test still
  calls it, so the self-test fails as "unknown role"; or a skill still names the
  documenter: the grep goes red.
- **Core:** no — the diff touches only `.claude/`.

## 8. Design belongs to the decider (review C3)

- **Does:** in `AGENTS.md`, under "The decider answers what this file sends to a
  human", a rule: a design or content conversation belongs to `/decide`, whose
  free-text form is the design session (`.claude/skills/decide/SKILL.md`,
  "Design sessions"); an implementer session asked for one records what the user
  has already said, verbatim, in the relevant `SPEC-QUESTIONS.md` entry, and
  sends the user to `/decide`; a question arising inside a point, which blocks
  it, may be answered on the spot, recorded verbatim the same way and marked for
  the decider. Edits `AGENTS.md`.
- **Closed by:** a grep finds in the rule each of: "`/decide`", "verbatim",
  "`SPEC-QUESTIONS.md`", "inside a point", "marked for the decider"; and the
  code-reviewer, given the diff and this point's "Does", reports any element of
  the "Does" the rule leaves out and any citation that does not say what the rule
  claims (table C1, C2).
- **Fails when:** the exception for a question inside a point, which the user
  approved, is left out: the grep for "inside a point" goes red and the reviewer
  reports it.
- **Check: shallow** — a rule of conduct; what proves it is the next design
  conversation going to `/decide`.
- **Core:** no — the diff touches only `AGENTS.md`.
