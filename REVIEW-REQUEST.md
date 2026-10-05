# Review request

The mechanism in this repository records everything and asks for nothing. A
human opinion therefore arrives whenever one happens to be offered, which on a
long branch means after the work that needed it is already built on.

This file interrupts. It holds the current request for a human's attention, and
is replaced by the next one; the old ones are in git.

## The three moments

Only three are worth interrupting for. Outside them the record is enough.

1. **Before a plan starts.** The cheapest moment there is: a wrong criterion
   found here costs one conversation, found at the end it costs every point
   built on it. And the plan is short — a few hundred words against a branch.
2. **At a phase gate**, from `docs/SIM-REQ.md` §18. Phase 3 closed, phase 4
   closed. This is the moment for looking at the whole rather than the point,
   which nothing else in this mechanism does: `/plan-next` sees one point,
   `/plan-status` sees one plan.
3. **When the work meets the specification.** A `SPEC-QUESTIONS.md` entry is
   raised, or the promotion pass produces candidates for `SIM-DEC`. These are
   the return channel, and a return channel nobody reads is a drawer.

A request is written **at** those moments, not near them, and never instead of
them: writing one does not authorise continuing past a blocking question.

## What a request holds

Six parts, all of them:

- **Which moment**, of the three.
- **Which branch or commit**, so that what is being talked about can be read.
- **What changed, in one line.** If it needs two, the request is covering more
  than one moment.
- **The decisions taken outside the specification in this block**, by their
  `D-NNN`, not restated.
- **The open questions about the specification**, by their `SQ-NNN`, with
  whether they block.
- **What was not verified.** Checks satisfied by reading, reviewers that did not
  run, clauses left pending. This is the part with no other home, and the part
  a reader cannot reconstruct.

The sixth is the reason the file exists. Everything above it is discoverable
from the repository by someone willing to look; what was *not* done leaves no
trace, and only the author knows it.

---

# Current request — 2026-10-05 — moment one, the process plan

Replaces the moment-three request of 2026-10-01, whose items are answered and
recorded on `design/poverty-seed`; it is kept in git.

**Moment:** one — before a plan starts. The draft below is the plan; under
`R-048` it is installed as `PLAN.md` on its branch only after the answer, in a
commit that cites it, and under `R-049` its first point waits for that answer
whether or not anything here blocks.

**Branch:** the request is written on `design/process-plan`, cut from `main` at
`54f3180`, which also carries the proposal it comes from,
`.claude/design/2026-10-02-integrazione-piano-processo.md`. After the answer the
branch merges into `main` without squash, and the plan runs on its own branch,
`agent-workflow`, cut from `main` for it.

**What changed, in one line:** a draft of the process plan that builds the agent
workflow of `D-086` to `D-089`, in seven points instead of the design's five.

## Decisions taken outside the specification

`D-110`: the user's answers to the proposal, and what writing the draft decided
besides — the rewiring of `/plan-next` moved to the last point, the draft read
from this file, the branch, the number. Not audited; the watermark stands at
`D-104` on `main`, and `D-108` and `D-109` are on `design/poverty-seed`.

**Two of the user's answers depart from rulings, and are recorded in `D-110`
until the decider records them:**

- **Seven points.** `R-046` says the process plan "holds the design's points 1
  to 5". The draft holds them, as points 1, 2, 4, 5 and 7, and adds two: 3,
  `plan-state`, and 6, `/plan-explain` and `/plan-status` on it. Neither is a
  simulator change, which is what `R-046` excludes.
- **Changes to the design `R-028` ratified.** Its "deliberately not built"
  excludes "any change to `/decide`, `/plan-explain` or `/plan-status`"; point 6
  changes the last two, which `R-045` touches anyway. The tree check around
  read-only agents extends from `/plan-next` to both. Two scripts that are not
  hooks go in `.claude/hooks/`.

## Open questions about the specification

- ~~**Non-blocking for this plan: the first real run.**~~ **Answered by `R-067`:**
  the check stays with phase 4's first point. The user placed this plan
  before the cost measurement plan drafted on `design/poverty-seed` (`D-109`),
  answering that request's question 2. `R-046` gives the first run of the agents
  to "the first point of the phase 4 plan"; with the measurement plan between the
  two, its points are the first a rewired `/plan-next` runs. Whether that point
  carries the design's point 6 check, or the measurement plan runs under the old
  procedure, is for the decider.
- **No `SQ-` entry is open against this plan.** It touches no file in `docs/`.

## Draft plan

```markdown
# Plan — the agent workflow, built and wired

**Kind:** process
**Exit condition:** the workflow of `.claude/design/2026-09-30-agent-workflow.md`
built and wired — the three scripts of its "Enforcement" and `tree-state`, the six
agents, the three rule sets in `AGENTS.md`, `/plan-next` dispatching the agents —
and the three plan skills reading a plan's state from `plan-state`. The proof that
the workflow runs belongs to the next plan, not this one (`R-046`).
**Phase:** none, process work; before phase 4 (`R-046`).
**Branch:** `agent-workflow`, cut from `main` after the answer to moment one.

Points 1 to 7 run under the `/plan-next` of today: the agents are wired in by
point 7, the last, so no point of this plan is run by them. Points 1 to 4 close
with the markers in use; from point 5, which writes the language rule, with the
English ones (`R-045`). Every point touches no path under `core/`.

## 1. `path-guard` and `git-guard`

- **Does:** two PowerShell 7 scripts in `.claude/hooks/`, as the design's
  "Enforcement" items 1 and 2 describe. `path-guard`, `PreToolUse` on
  `Edit|Write`, keyed by role: the coder's denylist of "What the coder may not
  write", the tester's `core/Tests/`, the documenter's three files, insert-only
  for the documenter. `git-guard`, `PreToolUse` on the committer's `Bash`: the
  commands it admits, the forms it refuses, and the three checks on `commit`.
- **Closed by:** `pwsh -File .claude/hooks/path-guard.ps1 -SelfTest` and the same
  for `git-guard.ps1` exit 0, each feeding JSON cases and expecting allow or deny.
- **Fails when:** the coder may write `docs/x`; `git add -A` passes;
  `git add a && rm b` passes.
- **Core:** no — touches no path under `core/`.

## 2. `mutate`, and `tree-state`

- **Does:** `.claude/hooks/mutate.ps1`, as the design's "Enforcement" item 3
  describes: it records the state of `core/Runtime/` and `harness/`, applies the
  tester's patch, runs the named check, reverses the patch, compares, and exits
  non-zero if the tree did not return to where it was. The state it records is
  computed by `.claude/hooks/tree-state.ps1`, which prints one hash of the working
  tree — `git status --porcelain=v1 -uall`, `git diff HEAD`, and the content of
  every untracked file not ignored. The skills call the same script around every
  read-only agent, from points 6 and 7.
- **Closed by:** a known patch on `core/Runtime/` turns a named test red, and the
  tree is restored; and `pwsh -File .claude/hooks/tree-state.ps1 -SelfTest` exits
  0: the hash is stable across two calls on an unchanged tree, and changes on a
  tracked file edited, a file staged, an untracked file added, and an untracked
  file edited.
- **Fails when:** an edit to a file that is already untracked leaves the hash
  unchanged; the patch stays applied after `mutate` exits.
- **Core:** no — the patch is reversed by the check itself; the diff touches no
  path under `core/`.

## 3. `plan-state`

- **Does:** `.claude/hooks/plan-state.ps1 [-Path <plan>] [-Draft]` reads a plan —
  `PLAN.md` by default, an archived one by path, or with `-Draft` the fenced block
  under `## Draft plan` in `REVIEW-REQUEST.md` — and prints JSON: `Kind`, the
  `**Abandoned:**` declaration, and per point its number, title, status (`open`,
  `closed`, `failed`, `reserve-open`, `reserve-discharged`), check marker
  (`weak`, `shallow`, none), `Core:` value and whether it gives a reason, and
  whether a `Serves` field is present. Then: the next point, or `stopped` at a
  failure, or `finished`; for each closed point the commits carrying its
  `Plan-point:` trailer within the plan's range, as `/plan-status` step 4 defines
  it; a warning if a live `PLAN.md` sits on `main` (`R-048`); the count for the
  decider's line of `/plan-next` step 8. Italian and English markers are both
  recognised from the first version (`R-045`).
- **Closed by:** `pwsh -File .claude/hooks/plan-state.ps1 -SelfTest` exits 0 on
  fixtures under `.claude/hooks/fixtures/plan-state/` — no plan; abandoned; a
  `FAILED` point with open points after it; a reserve open; a reserve
  discharged; every point closed; Italian and English markers in the same plan;
  `Core: no` without a reason; a request holding a draft under `## Draft plan`.
  And, run on each of the five archived plans, it reports 31 points closed, each
  resolved to exactly one commit — the figure measured by hand on 2026-10-02.
- **Fails when:** a point closed by `*Outcome:*` is reported open (only the
  Italian form recognised); the trailer search is unanchored, so `Plan-point: 1`
  in the twelve-gaps plan also returns the commits of points 10 to 13; a plan
  stopped at a `FAILED` point is reported as running.
- **Core:** no — touches no path under `core/`.

## 4. The six definitions in `.claude/agents/`

- **Does:** `tester`, `coder`, `code-reviewer`, `revisioner`, `documenter` and
  `committer`, each with the model, effort, tools and hooks of the design's roles
  table, and a `description` saying it is dispatched by `/plan-next` only. The
  five shared rules of `reviewers.md` move into the `revisioner`'s body. The
  `code-reviewer`'s body points at `AGENTS.md` rather than copying it, quotes the
  point's `Fails when:` line, and asks which fault the check would miss (`R-047`).
- **Closed by:** the frontmatter of each file checked against the roles table of
  `.claude/design/2026-09-30-agent-workflow.md`; a live probe: the coder asked to
  write `docs/probe.md` is blocked; a grep finds `Fails when` and the question on
  missing faults in the `code-reviewer`'s body.
- **Fails when:** a reviewer lists `Edit`.
- **Core:** no — touches no path under `core/`.

## 5. `AGENTS.md`: the three rule sets, and the English markers

- **Does:** edits `AGENTS.md`, adding the three rule sets of the design's "The
  rules this adds to `AGENTS.md`" — comments and simplicity (C1 to C10, S1), the
  language rule of `D-089`, review and self-governance — and, in "Criteria are
  frozen when written", the rule of `R-048`: a plan reaches moment one as a draft
  inside the request, and is installed on its branch after the answer, in a
  commit that cites it. The five outcome markers become English in `AGENTS.md`
  and in `/plan-next`, `/plan-explain` and `/plan-status`, which write the English
  form and recognise both (`R-045`).
- **Closed by:** the code-reviewer on a fixture diff with one C1 and one C5
  violation reports both, and on a clean diff reports none; a grep finds no
  Italian marker in the three skills outside the sentences that recognise it.
- **Fails when:** the section removed; a skill that writes `*Outcome:*` but no
  longer recognises `*Esito:*`, so the archived plans read as open.
- **Core:** no — touches no path under `core/`.

## 6. `/plan-explain` and `/plan-status` on `plan-state`

- **Does:** both skills take the plan's state from `plan-state` and keep only what
  needs judgement — the objections of `/plan-explain` step 7, the exemptions and
  the proved-by-reading count of `/plan-status`. Both call `tree-state` around
  their read-only agent and drop the word "guarantee", since `Plan` and `Explore`
  hold `Bash`. Both `description`s name triggers only. `/plan-explain` also reads
  a draft through `plan-state -Draft`.
- **Closed by:** each skill run on the fixture "a `FAILED` point with open points
  after it" reports the plan stopped, as `plan-state` does, and the tree hash is
  unchanged; `/plan-status` on the archived phase 3 plan reports ten points with
  the commits it reports today; `/plan-explain` on this request's draft names its
  first point.
- **Fails when:** a skill still defines the next point in its own words (a grep
  for "first `## <n>.` section with no" finds it); `/plan-status` reports a plan
  stopped at `FAILED` as in progress.
- **Core:** no — touches no path under `core/`.

## 7. `/plan-next` and `reviewers.md` rewired

- **Does:** `/plan-next` runs the life of a point of the design: each step
  dispatches the agent the design names, and the orchestrator runs the checks of
  "The hole that `Bash` leaves". Besides:
  - step 1 reads "Owed by the implementer" in `RULINGS.md`, as `AGENTS.md`
    requires before each point and the skill today does not say;
  - step 1 refuses a plan's first point until the user's answer to its
    moment-one request is recorded, blocking question or not (`R-049`);
  - it refuses a live `PLAN.md` on `main` (`R-048`);
  - the state of the plan comes from `plan-state`, not from rules restated in
    the skill;
  - `tree-state` is called before and after every read-only agent, and a
    difference stops the point; `reviewers.md` no longer calls the agent type a
    guarantee;
  - the reviewers' brief quotes the `Fails when:` line and asks which fault the
    check would miss (`R-047`);
  - the `description` names when to use the skill, not its steps.
- **Closed by:** each step's dispatch found in the skill; greps: `plan-state.ps1`
  and `tree-state.ps1` are invoked; step 1 names "Owed by the implementer" and the
  moment-one answer; `reviewers.md` names the `Fails when:` line; the
  `description` contains none of "implement", "verify", "commit"; the skill is no
  longer than the 2,755 words it has today. `Check: shallow` for the skill's
  prose: the scripts it calls carry their own tests in points 2 and 3.
- **Fails when:** a step dispatching no agent; step 1 starting a first point
  whose moment-one request is unanswered; a read-only agent dispatched with no
  `tree-state` call after it.
- **Core:** no — touches no path under `core/`.
```

## What was not verified

- **The design's own list:** which shell runs a hook on Windows, and whether
  `pwsh -File` with `$CLAUDE_PROJECT_DIR` works from an agent's frontmatter; that
  frontmatter hooks fire when the agent is dispatched from a skill; `effort` per
  agent; the precedence of project agents over plugin agents for these names;
  that Haiku writes a commit correctly. Only that `pwsh` 7.6.6 is installed.
- **The cost of a point** under the workflow, six or more dispatches, and the
  cost of points 3 and 6 of this plan: neither estimated.
- **`plan-state`'s JSON is not designed field by field.** The list in point 3 is
  what the three skills read today, not a schema.
- **`tree-state` does not see ignored files** (`bin/`, `obj/`): an agent writing
  there is not detected.
- **"31 points, one commit each"** was measured on 2026-10-02 with the lookup of
  `/plan-status` itself, not with an independent tool, and is not re-measured
  for this request.
- **The word cap of point 7** is the proposal's recommendation, put in the draft
  without an explicit answer.
- **No reviewer has read this draft.** Every point declares `Core: no`, so none
  is owed at close either; the `Fails when:` lines are read by the user here,
  under `R-047`.

## What I need from you

1. **The draft**, approved, amended, or rejected — in particular the seven
   `Fails when:` lines, the word cap of point 7, and the move of the rewiring to
   the last point.
2. **Nothing else blocks.** The question on the first real run is the decider's,
   and can wait for the measurement plan.
