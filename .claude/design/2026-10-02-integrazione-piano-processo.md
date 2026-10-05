# Proposta — integrazione alla bozza del piano di processo

**Data:** 2026-10-02
**Stato:** proposta, non approvata. Non è la richiesta del momento uno: è il
materiale da cui quella richiesta verrà scritta, dopo la tua risposta.
**Base:** la tabella "The plan that builds it" di
`.claude/design/2026-09-30-agent-workflow.md`, punti 1–5. Il punto 6 è uscito
dal piano per `R-046`.
**Origine:** la valutazione delle skill del 2026-10-02, problemi 1–4 e 7.
Il 5 (lunghezza) è qui come domanda, il 6 (`/decide`) resta fuori: è riserva 1
di `R-001`.

I criteri in bozza sono in inglese perché diventeranno testo del piano. Si
congelano solo nel commit che installa `PLAN.md` sul suo branch (`R-048`).

---

## 1. Il piano risultante

| # | Punto | Rispetto al design | Perché |
|---|---|---|---|
| 1 | `path-guard` e `git-guard` | invariato | — |
| 2 | `mutate`, e `tree-state` | **esteso** | lo stato dell'albero serve a `mutate` e al controllo sugli agenti in sola lettura: uno script, due usi |
| 3 | `plan-state` | **nuovo** | la lettura di un piano esce dalla prosa di tre skill e diventa codice testato |
| 4 | le sei definizioni in `.claude/agents/` | invariato | il code-reviewer chiede i fault mancanti, già dovuto per `R-047` |
| 5 | `AGENTS.md`: le tre serie di regole | invariato | vi atterrano già `R-045` e `R-048`; con il punto 3 la parte di `R-045` sulle skill si riduce alle parole dei marker |
| 6 | `/plan-next` e `reviewers.md` ricablati | **esteso** | allineamento a `R-048`, `R-049` e ad `AGENTS.md`; controllo dell'albero |
| 7 | `/plan-explain` e `/plan-status` su `plan-state` | **nuovo** | `R-045` le tocca comunque; il design le escludeva |

**Ordine.** Il 3 precede il 6 e il 7, che lo chiamano. Il 5 precede il 6 e il 7,
così le skill ricablate nascono con i marker inglesi e non vanno ritoccate.

Ogni punto è `Core: no`, con una ragione verificabile sul diff: nessun path sotto
`core/`.

---

## 2. Bozza dei punti nuovi ed estesi

### Punto 2 — `mutate`, e `tree-state`

- **Does:** as in the design, plus: the tree state `mutate` records is computed
  by `.claude/hooks/tree-state.ps1`, which prints one hash of the working tree —
  `git status --porcelain=v1 -uall`, `git diff HEAD`, and the content of every
  untracked file not ignored. The skills call the same script around every
  read-only agent.
- **Closed by:** the design's check, plus a self-test: the hash is stable across
  two calls on an unchanged tree, and changes on each case below.
- **Fails when:** an edit to a file that is already untracked leaves the hash
  unchanged; the patch stays applied after `mutate` exits.

### Punto 3 — `plan-state`

- **Does:** `.claude/hooks/plan-state.ps1 [-Path <plan>]` reads a plan — `PLAN.md`
  by default, an archived one by path — and prints JSON: `Kind`, the
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
  `Core: no` without a reason. And, run on each of the five archived plans, it
  reports 31 points closed, each resolved to exactly one commit — the figure
  measured by hand on 2026-10-02.
- **Fails when:** a point closed by `*Outcome:*` is reported open (only the
  Italian form recognised); the trailer search is unanchored, so `Plan-point: 1`
  in the twelve-gaps plan also returns the commits of points 10 to 13; a plan
  stopped at a `FAILED` point is reported as running.

### Punto 6 — `/plan-next` e `reviewers.md` ricablati

- **Does:** as in the design, plus:
  - step 1 reads "Owed by the implementer" in `RULINGS.md`, as `AGENTS.md`
    requires before each point and the skill today does not say;
  - step 1 refuses a plan's first point until the user's answer to its
    moment-one request is recorded, blocking question or not (`R-049`);
  - it refuses a live `PLAN.md` on `main` (`R-048`);
  - the state of the plan comes from `plan-state`, not from rules restated in
    the skill;
  - `tree-state` is called before and after every read-only agent, and a
    difference stops the point; `reviewers.md` no longer calls the agent type a
    guarantee, because `Plan` and `Explore` hold `Bash`;
  - the `description` names when to use the skill, not its steps.
- **Closed by:** the design's check, plus greps: `plan-state.ps1` and
  `tree-state.ps1` are invoked; step 1 names "Owed by the implementer" and the
  moment-one answer; the `description` contains none of "implement", "verify",
  "commit". `Check: shallow` for the skill's prose: the scripts it calls carry
  their own tests in points 2 and 3.
- **Fails when:** a step dispatching no agent; step 1 starting a first point
  whose moment-one request is unanswered; a read-only agent dispatched with no
  `tree-state` call after it.

### Punto 7 — `/plan-explain` e `/plan-status` su `plan-state`

- **Does:** both skills take the plan's state from `plan-state` and keep only
  what needs judgement — the objections of `/plan-explain` step 7, the
  exemptions and the proved-by-reading count of `/plan-status`. Both call
  `tree-state` around their read-only agent and drop the word "guarantee". Both
  `description`s name triggers only. Depending on question 2 below,
  `/plan-explain` also reads a draft held in `REVIEW-REQUEST.md`.
- **Closed by:** each skill run on the fixture "a `FAILED` point with open points
  after it" reports the plan stopped, as `plan-state` does, and the tree hash is
  unchanged; `/plan-status` on the archived phase 3 plan reports ten points with
  the commits it reports today.
- **Fails when:** a skill still defines the next point in its own words (a grep
  for "first `## <n>.` section with no" finds it); `/plan-status` reports a plan
  stopped at `FAILED` as in progress.

---

## 3. Cosa cambia rispetto al design approvato

Il design è ratificato da `R-028`. Queste modifiche richiedono la tua
approvazione, e vanno registrate quando il piano viene installato:

1. **"Any change to `/decide`, `/plan-explain` or `/plan-status`" era escluso.**
   Il punto 7 cambia le ultime due. `R-045` le tocca comunque, e `R-048` ha
   registrato come costo che `/plan-explain` non può leggere una bozza.
2. **Il controllo dell'albero sugli agenti in sola lettura** era previsto solo
   per gli agenti di `/plan-next`. Si estende alle altre due skill.
3. **Due script che non sono hook** (`tree-state`, `plan-state`) vanno in
   `.claude/hooks/`: il path è già nella denylist del coder, e la convenzione
   pwsh con self-test è quella del punto 1.

---

## 4. Domande per te

| # | Domanda | Raccomandazione |
|---|---|---|
| 1 | Dove vivono `plan-state` e `tree-state`? | `.claude/hooks/`, già protetta. In alternativa `.claude/scripts/`, ma va aggiunta alla denylist del coder, altrimenti un agente può modificare il proprio vincolo. |
| 2 | `/plan-explain` deve poter leggere la bozza al momento uno? | Sì, da una sezione di `REVIEW-REQUEST.md` con un titolo fisso (es. `## Draft plan`). **Mai** come file sotto `.claude/plans/`: `/plan-status` tratta come archiviazione ogni file aggiunto lì, e una bozza spezzerebbe i range del lookup. |
| 3 | Un tetto alla lunghezza di `/plan-next`? | Sì: il punto 6 non supera le 2.755 parole di oggi, nonostante il dispatch aggiunto. È un criterio meccanico contro la crescita per accumulo; il "perché" resta nella design note. |
| 4 | Sette punti invece di cinque: va bene? | Sì. I punti 3 e 7 tolgono lavoro ai punti 5 e 6 e riducono a una sola le modifiche future a regole di formato. Se preferisci un piano più corto, il 7 è l'unico rinviabile, ma `R-045` resterebbe dovuto su quelle due skill. |
| 5 | L'archiviazione di `RULINGS.md` per fase | Fuori da questo piano: è un cambio a `/decide`, riserva 1 di `R-001`. Da portare a una sessione `/decide` quando vuoi. |

---

## 5. Non verificato

- Il formato JSON di `plan-state` non è progettato campo per campo: la lista al
  punto 3 è ciò che le tre skill leggono oggi, non uno schema.
- `tree-state` non vede i file ignorati da `.gitignore` (`bin/`, `obj/`): un
  agente che scrive lì non viene rilevato.
- Non è verificato che una skill possa chiamare `pwsh -File` con lo stesso
  comportamento da sessione e da agente. È verificato solo che `pwsh` 7.6.6 è
  installato.
- Il "31 punti, un commit ciascuno" è stato misurato oggi con lo stesso lookup
  di `/plan-status`, non da uno strumento indipendente.
- Il costo in token e tempo dei punti 3 e 7 non è stimato.
