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

*Esito:* 2026-10-05. Chiuso. `path-guard.ps1 -SelfTest` 32 casi, `git-guard.ps1
-SelfTest` 43 casi, entrambi exit 0. I tre guasti del `Fails when:` introdotti su
copie e visti rossi: `docs/` tolto dalla denylist (5 casi rossi), il rifiuto delle
opzioni di `git add` tolto (`-A`, `-u`, `--all` rossi), `&` tolto dai separatori
(`git add a && rm b` rosso). `git diff --name-only HEAD -- core/` vuoto: `Core: no`
regge, revisori non eseguiti. Build e suite .NET non eseguite: nessun file C#
toccato. Registro `D-111`.

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

*Esito:* 2026-10-06. Chiuso. Patch nota su `core/Runtime/Time/Calendar.cs`
(`Month` diviso per `DaysPerMonth + 1`) applicata da `mutate`:
`FRT05a_DateArithmeticIsIntegerDivision` rosso, exit 0, `Calendar.cs` identico
byte per byte e hash di `core/Runtime,harness` invariato. `tree-state.ps1
-SelfTest` 11 casi e `mutate.ps1 -SelfTest` 9 casi, exit 0. I due guasti del
`Fails when:` visti rossi su copie: file non tracciati hashati per nome (2 casi
rossi), `git apply -R` tolto (6 casi rossi). `git diff --name-only HEAD -- core/`
vuoto: `Core: no` regge, revisori non eseguiti. Build e suite .NET non eseguite:
nessun file C# toccato. Registro `D-112`.

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

*Esito:* 2026-10-06. Chiuso. `plan-state.ps1 -SelfTest` 58 casi, exit 0: le
nove fixture, i 31 punti dei cinque piani archiviati risolti a un commit
ciascuno, il totale, l'entry point. I tre guasti del `Fails when:` visti rossi su
copie: `*Outcome:*` non riconosciuto (4 casi rossi), trailer non ancorato (2
rossi: punto 1 delle dodici lacune trova 5 commit, punto 1 della fase 3 ne trova
2), ramo `stopped` disattivato (2 rossi). `git diff --name-only HEAD -- core/`
vuoto: `Core: no` regge, revisori non eseguiti. Build e suite .NET non eseguite:
nessun file C# toccato. Registro `D-113`.

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

*Esito:* 2026-10-07. Chiuso con riserva — la probe dal vivo (il coder chiamato a
scrivere `docs/probe.md` è bloccato) attende una sessione avviata dopo questo
commit, in cui l'utente o l'orchestratore dispaccia `coder`: questa sessione non
vede le definizioni (`coder` "not found") e `claude -p` non si autentica. Il
frontmatter dei sei file confrontato con la tabella dei ruoli da uno script,
exit 0; il guasto del `Fails when:`, `Edit` aggiunto a `code-reviewer` e poi a
`revisioner`, visto rosso due volte e ripristinato. Le due grep sul corpo del
`code-reviewer` trovano `Fails when` e la domanda sui guasti mancanti; il corpo
dice che la riga è citata nel brief, perché varia per punto. Ogni `command:` del
frontmatter eseguito a mano con JSON d'esempio nega e ammette come atteso.
`git diff --name-only HEAD -- core/` vuoto: `Core: no` regge, revisori non
eseguiti. Build e suite .NET non eseguite: nessun file C# toccato. Registro
`D-114`.

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

## 8. The settings files, the identifier match, and four guards closed after audit

*Added 2026-10-07 under `D-014`, owed by `R-070`: no point renumbered, no
criterion above rewritten. Placed between points 6 and 7 and run before point 7,
at the user's request, departing from `D-014`'s "runs last" for this point only
(`R-070`); the preamble's "point 7, the last" is left as written. Approved by the
user: `R-068` ("approvo R-068, applicalo"), `R-071`, `R-073` and `R-074` in the
questions that chose them, `R-093` ("approvo la raccomandazione di R-093,
applicala", `R-094`); every `Fails when:` line below read by the user (`R-047`).*

- **Does:**
  1. `.claude/settings.json` and `.claude/settings.local.json` join "What the
     coder may not write" in the design and `$CoderDenied` in `path-guard`, with
     a self-test case each (`R-070`).
  2. In the design's "The hole that `Bash` leaves", the check reading
     "`AGENTS.md` in the diff only if the point names it" covers every
     `CLAUDE.md` as well. Point 7's text stays as it is: it builds the check from
     the amended list (`R-070`).
  3. `git-guard` matches the identifier case-sensitively, with a self-test case
     for a lowercase `a-` (`R-068`, `R-070`).
  4. `tree-state` reads untracked names unquoted — `-z`, or `core.quotePath` off
     for its own calls — and, when any file it must hash cannot be read, exits
     non-zero and prints no hash. A self-test case each. `mutate` treats a failed
     `tree-state` as a tree not returned, and stops with exit 3 when either call
     fails (`R-071`).
  5. `mutate` refuses, with exit 2, a patch in which either side of any entry, a
     rename's or copy's source included, lies outside `core/Runtime/` and
     `harness/`. A self-test case (`R-073`).
  6. `plan-state` decides `stopped` by position in the file: a failed point with
     an open point anywhere after it in the plan is `stopped`, whatever the
     numbers. A self-test fixture whose points are numbered out of file order
     (`R-074`).
  7. `.claude/agents/coder.md`, where its body lists the paths the coder may not
     write, adds `.claude/settings.json` and `.claude/settings.local.json`
     (`R-094`).
- **Closed by:** `pwsh -File <script> -SelfTest` exits 0 for `path-guard.ps1`,
  `git-guard.ps1`, `tree-state.ps1`, `mutate.ps1` and `plan-state.ps1`, each
  holding the cases of items 1 and 3 to 6; a grep finds `CLAUDE.md` in the
  design's list under "The hole that `Bash` leaves", and both settings files in
  the design's "What the coder may not write" and in `coder.md`'s body.
- **Fails when:** the coder may write `.claude/settings.json`; a subject "Tidy the
  a-priori notes" passes with no identifier; the design's list has no line for
  `CLAUDE.md` (`R-070`). An edit to an untracked file named `città.txt` leaves the
  hash unchanged; a file that cannot be read still yields a hash (`R-071`). A
  patch that renames `docs/a.txt` to `core/Runtime/a.txt` is applied (`R-073`). A
  plan whose point 8, placed before point 7, has failed is reported `running`
  with next point 7 (`R-074`). After point 8, `coder.md` names no settings file
  while `path-guard` denies them (`R-094`).
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
