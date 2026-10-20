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

*Outcome:* 2026-10-20. Closed: `/plan-explain -Draft -Path` on the fixture
returned five entries, one per point; point 4's names supply ample as the state
where the ranking is not seen ("With supply ample everything is served, so the
check stays green"); no `R-` or `D-` identifier cited; the rule is found in
`AGENTS.md` and the line in `REVIEW-REQUEST.md` (greps). The first run cited
`R-103`, the reader echoing the checklist's provenance; resolved in the skill's
brief (rules by `K` number), not in the criterion. The faults went red: the
first point only gave one entry, point 4's missing; a reader handed the register
cited `R-101` to `R-105` and `D-121` to `D-125`. Tree hash equal around every
reading. Recorded: the blindness test misses a reader that reads the register
and cites only `K` numbers; the check is `shallow`, as declared. `D-130`.
Review: no code-reviewer, the point touches no code; revisioners not run, `Core:
no` held. Agents (`R-067`): Plan readers, documenter and committer dispatched.
Not run: the coder, the files being the orchestrator's; the tester, not
dispatched, the check being a skill run, not a test. This outcome does not claim
`R-046`'s check.

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

*Outcome:* 2026-10-20. Closed: `plan-state -SelfTest` ran 84 cases, 0 failed,
on a fixture repository (point 1 closed with reserve, a *Ruled:* line and a
*Reserve discharged:* line; point 2's "Closed by" edited later; point 3 deleted;
point 4 added; point 5 closed with *Esito:*; point 6's heading edited): changed
= points 2 and 6, gone = 3, added = 4, recorded = 1 and 5; `deciderDue` true for
the defective and the Italian fixtures, false for the Ruled one, false with
nothing closed, true for a FAILED point. Live: install `a9ee621`, points 1 and 2
recorded, none changed, none gone, `deciderDue` false. The faults went red: the
baseline taken as the latest commit touching `PLAN.md` (4 failures, point 2 not
reported), record paragraphs not excluded (points 1 and 5 reported changed),
*Ruled:* dropped from the markers (point 1 changed), the heading comparison
removed (point 6 not changed); also `deciderDue` reading past the outcome
paragraph (the Ruled fixture true). Tree hash equal around each. Review:
code-reviewer, the check satisfied as written; the verifiable findings applied in
one round (record markers and the heading unexercised, a vacuous "nothing closed"
case, FAILED read from the status, a comment's wording). *Left open*, for the
decider: the archive count in the self-test, pinned at 31 points and red with 44
found, now counts the archives' `## n.` headings, outside the point's text and
weaker (a deleted archive no longer turns it red); and the reading of "lines that
begin with an outcome marker" as the paragraphs those lines open, since an
outcome wraps and the literal reading marks every closed point changed. `D-131`.
Agents (`R-067`): code-reviewer, documenter and committer dispatched. Not run:
the tester, the check being a script self-test run by the orchestrator; the
coder, the file being in `.claude/hooks/`; the revisioners, `Core: no` held. This
outcome does not claim `R-046`'s check.

## 9. A record opens at a paragraph, not at any line (added after point 3)

Added on 2026-10-20 with the user's approval, "sì, aggiungi il punto 3a e
procedi", under `D-014`: point 3's outcome wraps onto a line that begins with
`*Ruled:*`, and `plan-state` read it as a new record, cutting the outcome
paragraph before its *left open*; `deciderDue` came out false. Numbered 9 so
that `plan-state` reads it, and placed here so that it runs before point 4
(`R-074`).

- **Does:** in `plan-state.ps1`, a line that begins with a record marker opens
  a record only when it starts a paragraph — the first line of the section, or a
  line after a blank one — both where a point's frozen text is read and where
  `deciderDue` reads the outcome paragraph. A fixture plan whose outcome
  paragraph wraps onto a line beginning with `*Ruled:*` and carries *left open*
  after it. Edits `.claude/hooks/`.
- **Closed by:** `plan-state -SelfTest` green with the new fixture: `deciderDue`
  true, and the fixture's point not reported changed against an installed copy
  in the fixture repository; and `plan-state` on the live `PLAN.md` reads
  `deciderDue` true, point 3's outcome carrying *left open*.
- **Fails when:** a marker at the start of any line opens a record, as before
  this point: the new fixture's `deciderDue` goes false.
- **Core:** no — the diff touches only `.claude/hooks/`.

*Outcome:* 2026-10-20. Closed, *criterion defective* (`R-005`, case 1):
`plan-state -SelfTest` ran 87 cases, 0 failed; live `PLAN.md` reads `deciderDue`
true (point 3's *left open*), no point changed. The "Closed by" half "the
fixture's point not reported changed against an installed copy" cannot fail for
the reason it exists: under the old reading a wrapped marker opens the same
record, so a point whose outcome wraps is not changed either way; correct code
passes it, it proves less. The missing check, beside it, built and run: fixture
point 7, an edit after a wrapped marker line, reported changed, red under the
fault. Faults, restored, tree hash equal: a marker at the start of any line
opens a record (`Fails when`), `decider-wrapped` `deciderDue` false and point 7
not reported changed (2 failures), live `deciderDue` false; the
paragraph-start guard on the outcome dropped, `decider-midline` true (1
failure). Review: code-reviewer; the outcome opening mid-paragraph untested,
applied (`decider-midline.md`); a section's first line as paragraph start
untested, rejected (harmless, taste). *Left open*, for the decider: a record
directly after a bullet with no blank line is now part of the point's text; a
`*Ruled:*` line directly after an outcome with no blank line now belongs to the
outcome paragraph, so its *left open* would count. `D-132`. Agents (`R-067`):
code-reviewer, documenter and committer dispatched. Not run: the tester, the
check being a script self-test run by the orchestrator; the coder, the file being
in `.claude/hooks/`; the revisioners, `Core: no` held. This outcome does not
claim `R-046`'s check.

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

*Outcome:* 2026-10-20. Closed. Audit before wiring: 11 of the decider's 59
commits touching the register replaced text, each on a `**Ruling:**` line (`0b245ed`
filling "none yet"; `32f20af`, `4e7687d` and others appending a later ruling); the
point stopped and asked, and the user chose option A, "A, procedi": an Edit of a
`**Ruling:**` line passes even when it is not an insertion, everything else stays
insert-only. `path-guard -SelfTest` ran 50 cases, 0 failed. The exact command line
of `settings.json`, piped: register replacement exit 2 with `record`'s reason,
`README.md` exit 0, outside path exit 0. Live: an Edit replacing a word of the
register was denied by the hook in the orchestrator's session, already active, so
the reserve the point foresaw was not needed. Faults, tree hash equal: the role
misnamed on the command line, `README.md` exit 2 "unknown role"; `record`
inheriting the outside-repository denial, 2 failures; after the review, the wide
exemption restored, 3 failures. Review: code-reviewer, the check satisfied as
written; the verifiable findings applied in one round (the exemption wider than
option A, a missing register Write-over-existing case, a no-op pair and an
inflated count, a comment's citation); `MultiEdit` and `NotebookEdit` not matched,
rejected, the point says `Edit|Write`. `D-133`. Agents (`R-067`): code-reviewer,
documenter and committer dispatched. Not run: the tester, the check being a
script self-test and pipe tests run by the orchestrator; the coder, the files
being in `.claude/`; the revisioners, `Core: no` held. This outcome does not
claim `R-046`'s check.

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

*Outcome:* 2026-10-20. Closed. `mutate -SelfTest` ran 30 cases, 0 failed (new: a
fault in an untracked `bench/` file applies, goes red, is restored; a committed
file outside the diff refused, exit 2; `docs/` and `core/Tests/` refused though in
the diff; a rename from a file outside the diff refused; the twelve other excluded
paths refused; a check leaving a file in `bench/`, exit 3); `tree-state -SelfTest`
14, 0 failed; a grep finds no sentence saying `mutate` reaches only
`core/Runtime/` and `harness/`. Faults, tree hash equal: the admitted set read
from `git diff` only, the `bench/` case exit 2, red; `core/Tests/` admitted, its
case applied, red; after the review, the hash scope back to `core/Runtime` and
`harness`, and `AGENTS.md` dropped from the exclusions, each red. Review:
code-reviewer, the check satisfied as written; the verifiable findings applied in
one round (refusal cases for the twelve other excluded paths, the hash scope
pinned); *left open* for the human or decider: a check that writes unignored files
(`TestResults/`, `*.log`, CI's `hashes-*.txt`) now makes `mutate` exit 3, and
whether the tester's text should forbid it; question, recorded: a fault on the
source of a rename the point itself makes is refused; long lines, rejected as
taste. `D-134`. Agents (`R-067`): code-reviewer, documenter and committer
dispatched. Not run: the tester, the check being a script self-test run by the
orchestrator; the coder, the files being in `.claude/`; the revisioners, `Core: no`
held. This outcome does not claim `R-046`'s check.

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

*Outcome:* 2026-10-20. Closed. The declared check, a set of greps over
`.claude/skills/`: no dispatch or mention of `documenter`; the condition of step 2,
the four fields of step 8, the four rules of step 9 and the `DECIDER-BRIEFS.md`
append of step 10 found; "does not run `/decide`" not found: all hold, exit 0.
Fault, tree hash equal: step 8 dropping a STOP case, `recorded` removed from its
list, "field recorded missing", exit 1, red; restored. The criterion is `Check:
shallow`, as declared: the greps prove the words, not the procedure; points 7 and 8
run under it. The "Closed by" is the grep alone, so no `-SelfTest` was owed. No
reviewer ran: a skill, `Core: no` held; nothing left open. `D-135`. Agents
(`R-067`): documenter and committer dispatched. Not run: the tester, the check not
being a test under `core/Tests/`; the coder, the file being in `.claude/skills/`;
the code-reviewer, no code; the revisioners, `Core: no` held. This outcome does not
claim `R-046`'s check.

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

*Outcome:* 2026-10-20. Closed: `path-guard -SelfTest` ran 42 cases, 0 failed;
the agent file is absent; a grep finds `documenter` nowhere in `.claude/` but
the design notes and the archived plans. The faults went red: a self-test case
still calling the role (unknown role, 1 failure); a skill naming the documenter
(the grep finds it). Tree hash equal. Recorded by the orchestrator under point
6's step 8, `plan-state` reporting the point unchanged, unrecorded and present.
Review: code-reviewer, no verifiable defect. *Left open*, for the decider:
`deciderDue` matches its marker in prose, and read true after point 6 from "no
… open" wording. `D-136`. Agents (`R-067`): code-reviewer and committer
dispatched. Not run: the tester, the coder, the revisioners (`Core: no` held),
the documenter, retired here. This outcome does not claim `R-046`'s check.

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

*Outcome:* 2026-10-20. Closed: a grep finds in the rule "`/decide`",
"verbatim", "`SPEC-QUESTIONS.md`", "inside a point" and "marked for the
decider"; the code-reviewer, given the diff and the "Does", reports no element
missing and the citation of "Design sessions" accurate. The fault went red: the
exception left out, the grep reports two elements missing and the code-reviewer,
given that text blind, reports the third element absent. A clause the "Does"
does not hold, "it does not propose the design itself", was dropped (table
C2). The check is `shallow`, as declared. The permission to edit `AGENTS.md` is
recorded in `D-137`. Agents (`R-067`): code-reviewer and committer dispatched.
Not run: the tester, the coder, the revisioners (`Core: no` held). This outcome
does not claim `R-046`'s check.

## 10. A decider marker counts only in emphasis (added after point 8)

Added on 2026-10-21 with the user's approval, under `D-014`, as `R-120` owes
before this plan is archived: asked whether to add the point or close the plan
without it, the user chose "Aggiungi il point (Recommended)". `plan-state`
strips every `*` and `_` from the outcome paragraph and matches the three
phrase markers anywhere, so point 6's "nothing left open" made the decider due
(`D-136`).

- **Does:** in `plan-state.ps1`, *criterion defective*, *criterio difettoso*
  and *left open* count only as the whole of an emphasis span — `*…*` or
  `**…**`, case ignored, a line break inside it read as a space — in the last
  closed point's outcome paragraph; the bare phrase in prose does not count,
  nor a span that holds the phrase among other words. `FAILED` stays read from
  the point's status. Fixture plans: one whose last outcome reads "nothing left
  open" in plain prose; one whose last outcome reads `*nothing left open*`; one
  whose `*left open*` breaks across two lines inside its emphasis. Edits
  `.claude/hooks/`.
- **Closed by:** `plan-state -SelfTest` green: the prose fixture and the
  `*nothing left open*` fixture `deciderDue` false; the broken-span fixture
  true; `decider-defective`, `decider-italian`, `decider-wrapped` and
  `failed-then-open` still true; `decider-ruled` and `decider-midline` still
  false. And `plan-state -Path` on two copies of the live `PLAN.md`, one cut
  before point 7's heading, `deciderDue` false (point 6's "nothing left open",
  prose), and one cut before point 8's heading, `deciderDue` true (point 7's
  *Left open*).
- **Fails when:** the markers matched with emphasis stripped, as now: the prose
  fixture and the copy cut before point 7 turn true. Also: a marker counted
  anywhere inside an emphasis span: the `*nothing left open*` fixture turns
  true; and a span read line by line: the broken-span fixture turns false.
- **Core:** no — the diff touches only `.claude/hooks/`.
