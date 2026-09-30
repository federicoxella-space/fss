# Design — the agent workflow

**Date:** 2026-09-30
**Status:** approved in conversation with Federico Xella, 2026-09-30. Not built.
It is to be implemented by a `Kind: process` plan written **after** the phase 3
plan is archived; the decisions it rests on are `D-086` to `D-089`.

---

## What this is for

`/plan-next` runs a point in one session: the same model reads the criterion,
writes the test, writes the code, verifies it, arbitrates the review of its own
work, writes the record and commits. Only the two reviewers are separate, and
their separation is the one part of the mechanism that is enforced by tools
rather than by willingness: they are read-only agent types (`reviewers.md`).

This design extends that principle to every step. Four aims, all four wanted:

1. **Separation enforced by tools.** Each role can do only what its tools and
   hooks let it do.
2. **Independence between roles.** Whoever writes the test has not seen the
   code's reasoning; whoever reviews is not the author.
3. **Cost.** Cheaper models on mechanical roles, the strong one where judgement
   is needed. Where cost and independence conflict, independence wins.
4. **A clean orchestrating context.** Diffs and build output stay in the agent
   that produced them; the orchestrator receives conclusions.

The evidence for the second aim is the phase 3 record. In five of its six closed
points the criterion proved to cover less than the point asked: two named tests
passed against an empty implementation (`D-065`, `D-070`), one missed a 365-tick
year (`D-075`), one left out the New Year's Day its point required (`D-059`), one
named greps that do not exist (`D-084`). Each was found by the author mutating
their own code afterwards. A tester who writes the test from the frozen
criterion, before any code exists and without the coder's reasoning, attacks the
first two directly.

## What it is not

- **Not a change to `docs/`.** The documenter is not a writer of the
  specification; `docs/` stays the decider's (`R-001`).
- **Not a change to what a point is.** One commit, a runnable check, a fault it
  must catch, criteria frozen when written. The agents carry out the steps of
  `/plan-next`; they do not replace them.
- **Not a change to the decider.** `/decide` is untouched, and never dispatches
  the agents that write.
- **Not an agent that writes `PLAN.md`.** Excluded by the design of 2026-09-17
  for a reason that still holds: writing the plan is where a human decides what
  the work is.

## The roles

The session running `/plan-next` is the **orchestrator**. It keeps what needs
judgement or authority — reading the point and stopping on it, arbitrating
findings, reporting — and dispatches everything else.

| Agent | Model / effort | Tools | May write | Receives | Returns |
|---|---|---|---|---|---|
| `tester` | opus / high | Read, Grep, Glob, Edit, Write, Bash | `core/Tests/` only | the point verbatim; nothing from the coder | tests written and their state; per fault: patch, result, why red; suite counts |
| `coder` | sonnet / medium | Read, Grep, Glob, Edit, Write, Bash | everything except the denylist below | the point, the `R-NNN` owed, the tester's tests | files touched; decisions, structured; doubts as questions; tests that need changing |
| `code-reviewer` | sonnet / medium | Read, Grep, Glob, Bash | nothing | the diff, the point verbatim, the task as stated | findings as `file:line` + rule, or "no findings" |
| `revisioner` | opus / high | Read, Grep, Glob, Bash | nothing | the brief of `reviewers.md`, briefed or blind | as today |
| `documenter` | sonnet / low | Read, Grep, Glob, Edit — no Write, no Bash | insert-only on `DECISIONS-OUTSIDE-SPEC.md`, `PLAN.md`, `SPEC-QUESTIONS.md` | the coder's decisions, the orchestrator's resolution of findings, the tester's results, the point text as read at step 1 | the `D-NNN` assigned and the outcome line, or STOP |
| `committer` | haiku / low | Bash, Read | nothing | the paths to stage, a message file, the point number or none | the commit hash, or a refusal with its reason |

Every agent receives `CLAUDE.md`, and through it `AGENTS.md`; none receives the
conversation. What an agent needs beyond the standing rules is in its brief or
nowhere.

Every agent's `description` says it is dispatched by `/plan-next` only, so that
no session delegates to it outside the procedure. A `/decide` session never
dispatches `coder`, `tester` or `committer`: the decider writes no code.

### What the coder may not write

`docs/`, `core/Tests/`, `PLAN.md`, `DECISIONS-OUTSIDE-SPEC.md`,
`SPEC-QUESTIONS.md`, `REVIEW-REQUEST.md`, `PROMOTIONS.md`, `RULINGS.md`,
`core/BannedSymbols.txt`, `.claude/agents/`, `.claude/hooks/`, `.claude/skills/`.

A denylist rather than an allowlist, because a process point legitimately writes
files a simulator point never does.

The last three entries keep an agent from loosening its own constraint. The
agent definitions, the hook scripts and the skills that dispatch the agents are
written only by the orchestrator, in a point that names them, under the same
permission `AGENTS.md` requires for itself (`R-010`).

`AGENTS.md` is not on the list: a process point may edit it. If the diff touches
it and the point's text does not name it, the orchestrator stops.

## The life of a point

```
orchestrator   1. read the point, open every citation, read "Owed by the implementer"
                  └─ ambiguous, or a citation that does not hold → stop, as today
tester         2. write the tests the "Closed by" names, from the frozen criterion
coder          3. implement until green; never edit core/Tests/
tester         4. build, suite in Release and Debug, the "Fails when" fault → red → restored
orchestrator   5. check `Core:` against the diff, as today
review         6. in parallel, one message, all read-only:
                  code-reviewer (every point touching code)
                  revisioner briefed + revisioner blind (Core: yes)
orchestrator   7. resolve: verifiable → coder or tester applies, tester re-verifies;
                  taste or disagreement → open for the human
documenter     8. re-read PLAN.md, write the D-NNN entry and the outcome line
committer      9. stage by name, validate, one commit
orchestrator  10. report, and the line for the decider
```

**The coder's doubts go to the briefed revisioner only**, passed as the coder
wrote them. The tester, the code-reviewer and the blind revisioner never see
them.

**At most two rounds of fixes.** If the tester's re-verification is still red
after the second, the point stops and the orchestrator reports. Reviewers do not
run a second time; if a fresh look is needed, a fresh blind reviewer is
briefed, as `reviewers.md` already says.

**An existing test that must change** because an interface legitimately changed
— the case of `R-003` — is reported by the coder and changed by the tester.

**What skips what.** A `Core: no` point skips the two revisioners; the tester,
the code-reviewer, the documenter and the committer still run. A point that
touches no code — a document, a skill — also skips the code-reviewer. The coder
writes the point's files, whatever they are, except the three folders reserved
to the orchestrator.

**The orchestrator is no longer the author.** Today the author applies findings
and may not arbitrate them. Under this design the orchestrator arbitrates code
it did not write; the rule that taste and disagreement go to the human is
unchanged.

### The tester, in two dispatches

**Before the coder.** Writes the tests from the point's "Closed by" and "Fails
when", the cited identifiers opened and read, and nothing of the implementation.
Reports each named test's state at that moment. A named test that is already
green before any code is written is reported, and the orchestrator decides:
either the behaviour already exists, or the test cannot fail.

**After the coder.** Runs the declared check, the build and both suites — the
commands step 3 of `/plan-next` lists — then introduces each fault of "Fails
when" through `mutate`, and, where the point's code has a body that can be
emptied, the empty implementation too (`D-065`, `D-070`). It reports each fault
with its patch and why the check went red.

## Enforcement

Tools are restricted by name only; a path cannot be denied in the `tools` list.
Paths are denied by hooks declared in each agent's frontmatter, which apply to
that agent alone.

Three scripts in `.claude/hooks/`, in PowerShell 7: it is installed, it parses
the hook's JSON input without `jq`, which is not, and CI already runs on
Windows.

1. **`path-guard`** — `PreToolUse` on `Edit|Write`, keyed by role: the coder's
   denylist, the tester's `core/Tests/`, the documenter's three files. For the
   documenter it also enforces **insert-only**: an `Edit` passes only if its
   `new_string` begins with its `old_string`. Text is added after an anchor and
   never removed or rewritten, so "criteria are frozen" is enforced by the tool
   rather than by the documenter.
2. **`git-guard`** — `PreToolUse` on the committer's `Bash`. Admits only `git
   status`, `diff`, `log`, `show`, `add` with explicit paths, and `commit -F
   <file>`. Refuses `-A`, `.`, `-u`, `--all`, `--amend`, `push`, and any command
   joined to another (`;`, `&&`, `|`, `` ` ``, `$(`). On `commit` it validates:
   - no path under `docs/` is staged;
   - the subject carries an identifier — `FR-`, `NFR-`, `DEC-`, `AC-`, `A-`,
     `R-`, `SQ-`, `D-` — or says in words that none applies;
   - `Plan-point:` appears at most once, as `Plan-point: <integer>` and nothing
     else.

   The orchestrator writes the message file, since it composes the subject from
   what the point cites; the committer stages, checks the staged set against the
   list it was given, and commits.
3. **`mutate`** — the tester has no write access to `core/Runtime/`. It writes a
   fault as a patch; the script records the state of `core/Runtime/` and
   `harness/`, applies the patch, runs the named check, reverses the patch, and
   compares. It exits non-zero if the tree did not return to where it was.

### The hole that `Bash` leaves

Any agent holding `Bash` can write a file without `Edit` or `Write`, and no
hook on those two sees it. The hooks stop mistakes, not an agent set on getting
round them. The last barrier is mechanical and belongs to the orchestrator:

- the tree's state — `git status --porcelain` and the hash of `git diff` —
  identical before and after every read-only agent;
- after the tester's second dispatch, nothing outside `core/Tests/` different
  from before it;
- after the coder, no path of its denylist changed;
- no `docs/` path in an implementer commit;
- no new `#pragma warning disable` or `SuppressMessage` in the diff: a
  suppression is a decision, and the orchestrator stops and asks (`AGENTS.md`);
- `AGENTS.md` in the diff only if the point names it (`R-010`);
- `Core:` against the diff, as step 3 already does.

## The rules this adds to `AGENTS.md`

Each lands through a plan point that names it, as `R-010` requires.

### Comments and simplicity

The code-reviewer checks against these, and the coder writes to them. Every one
is a finding reviewers have already applied; none is invented here. New rules
arrive the same way — a finding, a register entry, a ruling.

| # | Rule | Applied in |
|---|---|---|
| C1 | Every citation in a comment — `FR-`, `DEC-`, `§`, `D-`, `R-` — points to text that exists and says that | `D-085` |
| C2 | A paraphrase of `docs/` neither narrows nor widens it | `D-080`, `D-085` |
| C3 | No claim about phases or the plan unless checked against `SIM-REQ` §18 | `D-067` |
| C4 | Claims of scope — "covers the whole surface", "safe", "injective", "cannot overflow" — are verified, or narrowed to what is true | `D-080`, `D-085` |
| C5 | Dead code is deleted, not commented | `D-060`, `D-067` |
| C6 | No reference to a register entry that does not exist yet | `D-080` |
| C7 | A rule stated only in a comment, enforced by neither the build nor a test, is a decision and goes to the register | `D-007`, promoted as `DEC-083` |
| C8 | A comment a ruling contradicts is updated in the commit that acts on the ruling | `D-085` |
| C9 | A doc comment sits on the member it describes | `D-085` |
| C10 | English; `summary` says what, `remarks` says why | the code's practice |
| S1 | Code that nothing reads and no test pins is deleted | `D-060`, `D-067` |

### Language

Every artefact of the repository is written in English: code, comments, the
register, outcome lines, plans, commit messages, agent definitions. Records
already written are not translated; they are frozen. The rule takes effect with
the point that writes it, so points 7 to 9 of the phase 3 plan are written as
points 1 to 6 were.

### Review and self-governance

- Every point that touches code passes the code-reviewer. This tightens the
  review; `Core: no` no longer means that no one reads the code.
- `.claude/agents/`, `.claude/hooks/` and `.claude/skills/` change only with the
  permission `AGENTS.md` requires for itself, recorded the same way.

### Where the rules live

`reviewers.md` keeps what the orchestrator puts in each brief; the five shared
rules move into the `revisioner` definition, so that each rule has one home. The
code-reviewer's checklist is its definition's body, pointing at the section of
`AGENTS.md` rather than copying it.

## The plan that builds it

A `Kind: process` plan, written with the human after the phase 3 plan is
archived. The criteria below are a draft; they are frozen when the plan is
written, not here. A review request, moment one, is owed before its first point.

| # | Point | Check, draft | Fails when |
|---|---|---|---|
| 1 | `path-guard` and `git-guard` | a self-test feeding JSON cases and expecting allow or deny | the coder may write `docs/x`; `git add -A` passes; `git add a && rm b` passes |
| 2 | `mutate` | a known patch on `core/Runtime/` turns a named test red, and the tree is restored | the patch stays applied after exit |
| 3 | the six definitions in `.claude/agents/` | frontmatter checked against the table above, and a live probe: the coder asked to write `docs/probe.md` is blocked | a reviewer lists `Edit` |
| 4 | `AGENTS.md`: the three rule sets above | the code-reviewer on a fixture diff with one C1 and one C5 violation reports both, and on a clean diff reports none | the section removed |
| 5 | `/plan-next` and `reviewers.md` rewired | each step's dispatch found in the skill | a step dispatching no agent |
| 6 | the first real point run by the agents | its outcome line shows the tester red then green, the fault seen red, every reviewer run | an agent skipped with no word of it in the outcome |

**Bootstrapping.** Points 1 to 5 run under the current `/plan-next`, because the
agents are not wired in until point 5. Point 6 is their first run, on a real
change chosen when the plan is written, not a synthetic one.

**Why after phase 3, and not beside it.** `/plan-status` resolves a point by its
`Plan-point:` trailer within a range bounded by the archive commits in
`.claude/plans/`, and says itself that two plans live at once would break that
lookup first. A process plan merged into `fase-3-kernel` would put an archive
commit inside the phase 3 range and hide points 1 to 6 from it. A branch from
`main` would design the agents against rules `main` does not have: it lacks
`/decide`, `RULINGS.md` and the decider's sections of `AGENTS.md`. And point 7,
the serialiser, is the hardest point of the phase; it should not be the first
run of an untried workflow.

## What is deliberately not built

- An orchestrator agent. The orchestrator is the session.
- An agent that writes `PLAN.md`.
- Any change to `/decide`, `/plan-explain` or `/plan-status`.
- Worktree isolation for the tester. A worktree starts from the last commit, and
  the point's code is not committed until the point closes; the tester would
  mutate a tree without the implementation in it.
- Chaining. `/plan-next` still runs one point and stops.

## Not verified

- Which shell runs a hook command on Windows, and whether `pwsh -File` with
  `$CLAUDE_PROJECT_DIR` works from an agent's frontmatter.
- That an agent's frontmatter hooks fire when the agent is dispatched from inside
  a skill.
- That this version honours `effort` per agent. The documentation says it does;
  nothing here has run it.
- Whether the precedence of project agents over plugin agents holds for these
  names. The documentation orders the scopes and says nothing about a clash.
- The cost of a point: six or more dispatches, not measured.
- That Haiku writes the commit correctly; `git-guard` is what stops it if not.
- The `Bash` hole, covered only by the orchestrator's checks above.
