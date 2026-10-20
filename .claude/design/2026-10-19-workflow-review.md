# Workflow review, 2026-10-19

A running record of the user's review of the development workflow, held after
the cost measurement plan was archived and merged (`ee63129`). Each item is a
direction the user approved in conversation; none is in force. They take effect
only through one process plan, drafted at the end of the review and shown at
moment one, whose points name the files they change (`AGENTS.md`, "This file
changes only with permission").

## 1. Criteria that are always defective

Facts: every point of the cost measurement plan closed *criterion defective*
(`R-005`, case 1), four of ten in the phase 3 kernel plan. The checks proved one
property while the "Does" promised several; the `Fails when:` named only the
fault the author had thought of; the decider's lesson (a reference check where
the interesting branch binds) reached only the next point, the later criteria
already frozen; and no reviewer read the draft before moment one.

Approved by the user, "Ok A più B":
- **A. Criteria attacked before they freeze.** Before moment one, a blind
  reviewer reads the draft plan and answers, per point, which fault the check
  would miss. The answers go into the review request; the user decides whether
  to amend.
- **B. A checklist of criteria by kind of point**, drawn from the rulings that
  found the gaps (`R-101`, `R-103`, `R-104`, `R-105`): a computation is checked
  against an independent reference on a state where the interesting branch
  binds; a reported number is recomputed from an independent source; two paths
  compared share no code. The plan's author applies it; A's reviewer reads
  against it.

## 2. Weight per point

Facts, from points 4 and 5: tester at step 2 did nothing where the check is not
a test under `core/Tests/`; committer and documenter are separation, not
judgement; `mutate.ps1` cannot reach outside `core/Runtime/` and `harness/`, so
the orchestrator introduced faults by hand; the decider between points cost
130–260k tokens and 8–20 minutes, and each run changed the next point.

Approved by the user, "Procedi come hai proposto":
- **P1.** An insert-only hook on the record (register, outcome line), refusing
  edits to existing text and a point whose text changed; with it, the
  orchestrator records and commits, without documenter and committer.
- **P2.** The tester at step 2 only when the check is a test under
  `core/Tests/`.
- **P3.** `mutate.ps1` extended to every file in the point's diff, restored and
  hashed as today.
- **P4.** The decider between points only when the point just closed left
  something open (a defective criterion, a finding left to the human, a
  `SPEC-QUESTIONS.md` entry, an escalation); otherwise the audit gathers at the
  plan's end or the phase gate.

## 3. Mechanical friction

Facts: the "Fact-Forcing Gate" that interrupted most sessions is the ECC
plugin's GateGuard, installed for the user, not a hook of this repository; its
questions ("who imports this file?") carry no signal for Markdown records.
`git-guard`'s three refusals of the committer (a joined command, `git -C`, the
trailer's word in prose) are its design, pinned by its own tests; the third was
the orchestrator's error against `/plan-next` step 9.

Approved by the user, "Sì, approvo G1 e G2":
- **G1, done 2026-10-20, outside any plan** (environment, not workflow):
  `.claude/settings.local.json` sets `GATEGUARD_BASH_ROUTINE_DISABLED=1` and
  `GATEGUARD_EXEMPT_GLOBS=*.md,.claude/design/**`; destructive-command checks
  stay on. The file is excluded locally in `.git/info/exclude`, not committed.
- **G2.** `git-guard` unchanged; with P1 the orchestrator commits, so step 9 of
  `/plan-next` lists its rules: one git command per call, no `-C`, the trailer's
  word never outside the trailer, paths with forward slashes.

## 4. Content decided outside the three moments

Facts: the groups of goods, the forge, fuel, water and heating were designed
with the user in an implementer session (2026-10-17 to 19), the user's words
recorded verbatim in `SQ-024` and the implementer's proposals labelled as its
own; the decider ruled them after (`R-110` to `R-115`). `/decide` with free text
is already the design session, and `AGENTS.md` says the implementer never plays
the decider.

Approved by the user, "C3":
- **C3.** Design and content conversations belong to `/decide`: an implementer
  session asked for one records what the user has already said, verbatim, in
  the relevant `SPEC-QUESTIONS.md` entry, and sends the user to `/decide`. A
  question arising inside a point, which blocks it, may be answered on the spot,
  recorded verbatim the same way and marked for the decider.
