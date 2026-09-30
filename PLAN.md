# Plan — the headless kernel, phase 3

**Kind:** simulator
**Satisfies:** `AC-02`, `AC-03`
**Exit condition:** the gate of phase 3 in `docs/SIM-REQ.md` §18, verbatim —
"100k empty ticks; AC-02, AC-03 green" — run by the command-line harness in CI,
as §17 requires of every criterion.

**Phase:** 3 (SIM-REQ §18)   **Branch:** `fase-3-kernel`

---

## A precondition this plan does not own

`docs/SIM-REQ.md` §20 ends: **"Confirm before Phase 3. A wrong guess costs a
rewrite of the serialiser."** Five of its rows still read `Proposed` — target
framework, maximum language level, ahead-of-time support, allowed base class
library surface. The code already builds on `netstandard2.1` and C# 9, so the
guess has been made in practice and never confirmed.

Points 1 to 6 do not depend on the answer. **Point 7 is the one §20 names**, and
starting it before the confirmation is the exact risk that sentence was written
about. This plan does not resolve it: the confirmation belongs to a human, and
`REVIEW-REQUEST.md` carries it.

Two items of §19 also say they close in this phase — `P-17`, the settlement
ceiling, and `P-01` validity — and both want measurements that empty ticks
cannot produce. Filed as `SQ-004`, not decided here.

*Resolved 2026-09-20, both of them, before any point started.* §20 is confirmed
and its four rows read `Decided`; **point 7 is unblocked** and its criterion is
unchanged, which is the whole reason the block was written as a precondition
rather than into the point. `SQ-004` closed on reading 1 — `P-17` and `P-01`
move to Phase 4 — and `NFR-05` moved with them. Both applied to `docs/` by
`0114ad2`.

*Nothing above this line was rewritten.* The points are frozen as written, and a
precondition that has since been met is recorded here, in the section that
declared it, rather than by editing the point that waited on it.

## 1. The integer calendar

- **Does:** `Time/` gets the tick-to-date arithmetic and nothing else: year,
  dayOfYear, season, week, month, dayOfMonth, and the New Year's Day that sits
  between two ticks without consuming one.
- **Serves:** `FR-T-01`, `FR-T-02`, `FR-T-03`, `FR-T-05`, `FR-T-05a`, `DEC-005`,
  `DEC-006`, `DEC-006a`, `DEC-006b`
- **Closed by:** `FRT02_EveryDateFallsOnTheSameWeekday` and
  `FRT05a_DateArithmeticIsIntegerDivision` green, the second checking the six
  formulas of FR-T-05a against every tick of four consecutive years rather than
  against a sample.
- **Core:** yes

*Esito:* 2026-09-20. Chiuso. `FRT02_EveryDateFallsOnTheSameWeekday` e
`FRT05a_DateArithmeticIsIntegerDivision` verdi — il secondo su tutti i 1456 tick
di quattro anni consecutivi, con i valori attesi scritti sulle costanti
letterali di FR-T-05a e non su quelle di `Calendar`. Suite intera verde in
Release (19) e in Debug (20), build del core senza avvisi. Registro: `D-056`
a `D-060`. Revisione: due revisori, `Core: yes`, entrambi concludono che il
punto regge; applicate la cancellazione di due costanti non lette e la
correzione di un commento impreciso, respinta una nota con la ragione in
`D-060`. **Rilievo sul criterio, non sanato per riscrittura:** il "Closed by"
non copre la New Year's Day che il "Does" richiede — terzo test aggiunto,
criterio lasciato com'è, `D-059`.

## 2. `WorldState` and the five world fields

- **Does:** one `WorldState`, parallel arrays, holding at first only the World
  row of `SIM-STATE` — tick, worldSeed, generationParams, ruleVersion,
  currencyTotal. No settlements, no cohorts: phase 3 has no domain.
- **Serves:** `FR-W-01`, `NFR-10`, `DEC-003`, `SIM-STATE` §World
- **Closed by:** `NFR10_StateHoldsNoObjectReferences` green, walking the declared
  fields from the test project and failing on any reference type.
- **Core:** yes

*Esito:* 2026-09-21. Chiuso. `NFR10_StateHoldsNoObjectReferences` verde in
Release (23) e in Debug (24), build del core senza avvisi. `WorldState` porta
le cinque righe di `SIM-STATE` §World e nient'altro; `GenerationParams` è una
`readonly struct` con il solo `SettlementCount`. Registro: `D-061` a `D-067`.
Revisione: due revisori, `Core: yes`, entrambi concludono che il punto chiude;
applicati sei rilievi — due affermazioni false nei commenti sulla fase della
cronaca, la guardia di ricorsione morta, lo spostamento in `Runtime/State/`, la
citazione P-02 corretta in FR-W-10, e la superficie di uguaglianza non letta
cancellata; nessuno respinto, due lasciati aperti all'umano in `D-063` e
`D-067`. **Due rilievi sul criterio, nessuno dei due sanato per riscrittura:**
il criterio dice "failing on any reference type" e il walker ammette un livello
di array, perché il "Does" dello stesso punto chiede array paralleli (`D-064`);
e il test dichiarato passa anche contro un walker vuoto — verificato svuotandolo
— per cui a tenerlo in piedi è un quarto test che il criterio non nomina
(`D-065`). Criterio lasciato com'è in entrambi i casi.

## 3. The state hash covers every field

- **Does:** the hash over `WorldState` required by `SIM-STATE`'s serialisation
  note — "a field excluded from the hash is a determinism hole" — and the test
  that keeps it honest as fields are added in later phases.
- **Serves:** `NFR-01`, `AC-02`, `SIM-STATE` §Serialisation notes
- **Closed by:** `NFR01_EveryStateFieldEntersTheHash` green: the test enumerates
  the fields of `WorldState`, perturbs each one in turn, and fails if the hash
  does not move. A field added later without reaching the hash fails this test
  without anyone remembering to extend it.
- **Core:** yes — the test project may use reflection to enumerate; the core may
  not, and does not.

*Esito:* 2026-09-23. Chiuso, **criterio difettoso**.
`NFR01_EveryStateFieldEntersTheHash` verde in Release (28) e in Debug (29),
build del core senza avvisi; il check dichiarato eseguito anche da solo. Che
possa fallire è verificato togliendo il fold di `GenerationParams`: rosso, e
nomina `WorldState.GenerationParams.SettlementCount`. Registro: `D-068` a
`D-071`; questione di specifica `SQ-005`, non bloccante. Revisione: due
revisori, `Core: yes`, entrambi concludono che il punto chiude e che il check è
soddisfatto come scritto. Applicati tutti i rilievi verificabili — due
affermazioni false nei commenti, il valore del digest fissato, il tripwire di
`Different` messo alla prova; respinto legare `StateHash.Seed` a
`Hash64.GoldenGap`, con la ragione in `D-071`, e rinviate a fase 4 le colonne
array; lasciato aperto all'umano il doppio walker su `WorldState`.
**Il criterio è difettoso e non è stato riscritto:** il test che nomina passa
anche contro un'enumerazione vuota — stessa forma di `D-065`, un punto dopo — e
a tenerlo in piedi sono due test che il criterio non nomina. Il difetto è
scritto accanto in `D-070`, il criterio sopra questa riga è intatto.

## 4. The tick loop, its cadences and its buckets

- **Does:** advancing N ticks, the four cadences dividing one another, and the
  staggered settlement bucket `id % 7 == d % 7` derived from the id. The caller
  says how many ticks run; the loop never decides.
- **Serves:** `FR-T-04`, `FR-T-06`, `FR-T-08`, `FR-T-09`, `DEC-007`, `DEC-008`,
  `A-13`
- **Closed by:** `A13_EachCadenceBucketFiresOncePerPeriod` green over 100k ticks,
  counting firings per bucket per period rather than sampling them. A-13 is an
  invariant of `SIM-STATE`, not an acceptance criterion, and the test is named
  for it under the same rule.
- **Core:** yes

*Esito:* 2026-09-23. Chiuso, **criterio difettoso**.
`A13_EachCadenceBucketFiresOncePerPeriod` verde su 100k tick, anche da solo;
suite intera verde in Release (31) e in Debug (32), build del core senza
avvisi. Il test conta ogni firing nel suo periodo, per settlement e per
settimana, e non è vacuo: sei mutazioni del loop lo fanno fallire da solo.
Registro: `D-072` a `D-076`. Revisione: due revisori, `Core: yes`, entrambi
concludono che il check è soddisfatto come scritto; applicati tutti i rilievi
verificabili, nessuno respinto, lasciata aperta all'umano la collocazione in
`Systems/` rispetto ad `AGENTS.md` (`D-072`). **Il criterio è difettoso e non è
stato riscritto:** un regno che scatta ogni 365 tick passa il test nominato su
100k tick, perché il primo anno saltato comincia al tick 132.860. Il controllo
mancante è accanto, `FRT04_ConsecutiveFiringsOfALevelAreOnePeriodApart`, e il
difetto è scritto in `D-075`.

## 5. The command queue, applied at one point in the tick

- **Does:** the queue the host pushes into and the defined point of the tick
  where it drains. Nothing outside the core writes state; the queue is that
  boundary, and `AC-02` names commands beside the seed.
- **Serves:** `FR-A-01`, `AC-02`
- **Closed by:** `FRA01_CommandsApplyAtOnePointInTheTick` green: the same
  commands submitted in the same order produce the same hash sequence, and
  submitting them at a different moment of the caller's loop changes nothing.
- **Core:** yes

*Esito:* 2026-09-29. Chiuso. `FRA01_CommandsApplyAtOnePointInTheTick` verde,
anche da solo; suite intera verde in Release (37) e in Debug (38), build del core
senza avvisi. I comandi si applicano all'inizio di ogni tick, prima di ogni
livello, in ordine di invio; la coda sta accanto a `WorldState`, fuori
dall'hash. Guasti introdotti uno alla volta e visti rossi sul test nominato:
drain una volta per chiamata, a fine tick, dopo Daily, senza applicare.
L'ordine invertito lo passa, e lo coglie `FRA01_CommandsApplyInTheOrderSubmitted`
(`D-079`). `R-003` onorato: `WorldState` e `StateHash` interni, `Simulation`
pubblico come ingresso dell'host; `R-004` sul solo percorso pubblico, il resto
resta al punto 7. Registro: `D-077` a `D-080`; questione di specifica `SQ-006`,
non bloccante. Revisione: due revisori, `Core: yes`, entrambi concludono che il
check è soddisfatto come scritto; applicati tutti i rilievi verificabili, tra cui
l'ingresso pubblico mancante e un test cieco al drain per chiamata; nessuno
respinto; lasciata aperta all'umano la forma dei comandi, codice o dati
(`D-077`).

## 6. The chronicle log, structure and append

- **Does:** the chronicle row of `SIM-STATE` — id, tick, location, entities,
  cause, importance — and a deterministic append. **Propagation is not built
  here:** FR-I-02, FR-I-03 and FR-I-05 are domain, and phase 3 has none.
- **Serves:** `FR-I-01`, `FR-I-04`, `SIM-STATE` §Chronicle
- **Closed by:** `FRI01_ChronicleIdsAreStableAndOrdered` green, with entries
  appended from a synthetic source since no subsystem emits any yet.
- **Check: shallow** — the greps and the synthetic source prove the shape is
  there, not that it carries what a real event needs. Replaced by the first
  subsystem of phase 4 that emits an entry, which is the first time FR-I-01's
  five fields are filled by something that did not exist to fill them.
- **Core:** yes

*Esito:* 2026-09-30. Chiuso, **criterio difettoso**.
`FRI01_ChronicleIdsAreStableAndOrdered` verde, anche da solo: 1000 voci da una
sorgente sintetica, rilette campo per campo dopo sette crescite delle colonne;
suite intera verde in Release (41) e in Debug (42), build del core senza avvisi.
Guasti introdotti uno alla volta e visti rossi sul test nominato: id da 0,
crescita senza copia, pool sempre da 0, tick non registrato; sui test accanto,
causa in avanti accettata, colonna fuori dall'hash, fold della sola riga 0, fold
sulla capacità. `Check: shallow` confermato: la forma è verificata, il contenuto
di un evento reale no, e `FR-I-04` è servito solo come struttura. Registro:
`D-081` a `D-085`. Revisione: due revisori, `Core: yes`, entrambi concludono che
il check è soddisfatto come scritto; applicati tutti i rilievi verificabili, tra
cui un fold che leggeva la sola riga 0 senza che alcun test lo vedesse; non
applicata una formulazione ratificata, con la ragione; lasciati aperti all'umano
i nomi di due test, la cartella vuota `Runtime/Chronicle/` e due note per la
fase 4 (`D-085`). **Il criterio è difettoso e non è stato riscritto:** "the
greps" non nomina alcun grep; i controlli che provano la forma sono accanto, in
`D-084`.

## 7. The serialiser and the round trip

- **Does:** hand-written save and load over the parallel arrays, carrying a
  version number and a migration path from the first write, with no reflection
  anywhere in the core.
- **Serves:** `NFR-08`, `AC-03`, `SIM-REQ` §20
- **Closed by:** `AC03_SaveRoundTrip` green — save, load, save, identical bytes.
- **Core:** yes
- **Blocked on the §20 confirmation above.** If it arrives after this point is
  built and contradicts the guess, the point is reported failed and rewritten,
  not amended: §20 says the cost is a rewrite of the serialiser, and pretending
  otherwise would be the criterion bending to fit the delivery.

*Esito:* 2026-09-30. Chiuso, **criterio difettoso**. `AC03_SaveRoundTrip`
verde, anche da solo: save, load, save, byte identici, per un mondo vuoto, per
uno con ogni campo in uso e comandi in attesa, e dall'ingresso pubblico; suite
intera verde in Release (50) e in Debug (51), build del core senza avvisi.
Guasti introdotti uno alla volta e visti rossi sul test nominato: reader che
perde `CurrencyTotal`, writer senza comandi, colonna omessa, entità del pool
saltata, comandi invertiti al load, campo `B` di un comando perso; sui test
accanto, controllo dei tick e della rule version tolti, header tolto, scambio
simmetrico di due campi. Debiti di `RULINGS.md` saldati nello stesso commit:
`R-021`, `R-022`, `R-024`, `R-004`, `R-026`. Registro: `D-090` a `D-096`.
Revisione: due revisori, `Core: yes`, entrambi concludono che il check è
soddisfatto come scritto; applicati tutti i rilievi verificabili, tra cui la
rule version successiva a quella della build rifiutata al load e i campi dei
comandi che nessun test vedeva; nessuno respinto, nessuno lasciato all'umano.
**Il criterio è difettoso e non è stato riscritto:** il round trip passa anche
contro un serialiser che non scrive alcun numero di versione, visto togliendo
l'header da writer e reader insieme; i controlli mancanti sono accanto, in
`D-095`.

## 8. The command-line runner

- **Does:** `sim` runs N ticks headless, dumps the state hash sequence, and
  writes it where CI can compare it. Sweeps and CSV metric series are named by
  NFR-12 and wait for phase 6, which is where the metrics exist.
- **Serves:** `NFR-12`, `AC-19`
- **Closed by:** `sim --ticks 1000 --seed 1 --hashes` printing one hash per tick,
  run from the CI workflow.
- **Core:** no — touches `harness/` only, verifiable by
  `git diff --name-only HEAD -- core/` being empty.

*Esito:* 2026-09-30. Chiuso con riserva — "run from the CI workflow" attende il
push del branch, che è dell'umano. In locale, lo script dello step CI eseguito in
pwsh sulla build Release: `sim --ticks 1000 --seed 1 --hashes` stampa 1000 hash,
tutti da 16 cifre esadecimali minuscole e tutti distinti. Guasti introdotti uno
alla volta e visti rossi sullo script: un hash per esecuzione invece che per
tick, nessun `Advance` nel ciclo, un tick di troppo, esadecimale maiuscolo. Il
vecchio step `--ticks 100000 --hash`, verde solo perché lo stub ignorava gli
argomenti, è sostituito; i 100k tick tornano al punto 9. `git diff --name-only
HEAD -- core/` vuoto: `Core: no` regge, revisori non eseguiti. Core senza
avvisi, suite verde in Release (50) e in Debug (51). Registro: `D-097` a
`D-099`, con due rilievi non trattati: il CSV della cronaca chiesto da `NFR-12`,
che nessun punto consegna né rinvia, e il `ProjectReference` dell'harness
rispetto al §20.

*Riserva sciolta:* 2026-10-01 — branch pushato dall'umano. Step "Harness, one
hash per tick" verde nel run CI `36786627133` su `b43d57d` e di nuovo nel run
`36788322902` su `c645462`.

## 9. The gate: 100k empty ticks, `AC-02` green in CI

- **Does:** the two runs the gate asks for, wired into the workflow that already
  builds the core standalone.
- **Serves:** `AC-02`, `AC-19`, `SIM-REQ` §18
- **Closed by:** `AC02_DeterminismAcrossRuns` green, and 100k empty ticks
  completing in CI with the hash sequence of two independent runs compared byte
  for byte.
- **Core:** no — adds a test file and a workflow step, touching no file under
  `core/Runtime/`. Verifiable by `git diff --name-only HEAD -- core/Runtime/`
  being empty.
- **This point also produces the first measurement** this project has: ms per
  empty tick. It is not `P-01` and not `P-17`, which need a settlement update to
  measure — see `SQ-004` — but it is the floor under both, and recording it
  costs one line of output.

*Esito:* 2026-10-01. Chiuso con riserva — "100k empty ticks completing in CI"
attende il push del branch, che è dell'umano. `AC02_DeterminismAcrossRuns`
verde, anche da solo: due mondi dello stesso seme, 100k tick, hash a ogni tick,
vuoti e con comandi, con tre controlli perché l'uguaglianza non sia banale;
suite intera verde in Release (51), con tre wealth band (51) e in Debug (52),
build del core senza avvisi. Lo script del nuovo step CI, eseguito in locale in
pwsh sulla build Release: due processi da 100k tick, output confrontati byte per
byte, verde. Guasti introdotti uno alla volta e visti rossi: un `WorldState` che
conta i mondi costruiti, sul test; seme diverso al secondo run, secondo run un
tick più corto, entrambi un tick più corti, sullo script. Misura: 0,00012–0,00013
ms per tick vuoto in locale, presa su 10M tick al netto di un run da zero,
perché su 100k tick il costo sta sotto il rumore dell'avvio e la cifra usciva
negativa (`D-101`). `git diff --name-only HEAD -- core/Runtime/` vuoto: `Core:
no` regge, revisori non eseguiti. Registro: `D-100` a `D-102`, con un rilievo
non trattato: `AC-02` dice "across builds and machines" e i due run sono una
build su una macchina.

*Riserva sciolta:* 2026-10-01 — branch pushato dall'umano. Step "Gate, 100k empty
ticks in two runs" verde nel run CI `36786627133` su `b43d57d` e di nuovo nel run
`36788322902` su `c645462`: due processi da 100k tick, output confrontati byte per
byte. Misura sul runner: 0,000120 ms per tick vuoto.

## 10. The two hash sequences of the gate, pinned

*Added 2026-10-01, after points 1 to 9 closed, with the user's approval ("approvo
l'aggiunta al piano di fase 3 di un punto per l'obbligo R-034/R-036"), under
`D-014`: appended, not inserted, and no criterion above rewritten. Owed by `R-034`
as refined by `R-036`; the gate of the exit condition is not declared passed
until this point has closed and CI has run it.*

- **Does:** pins in the suite a digest of each of the two 100k-tick sequences of
  `AC02_DeterminismAcrossRuns` — the empty run and the run with commands of the
  stand-in kind every 97 ticks — each value produced by a run and written as a
  literal, never computed by the code that checks it. Every CI run compares its
  own against both, in each of the three test steps.
- **Serves:** `AC-02`, `NFR-01`, `SIM-REQ` §18, `R-034`, `R-036`
- **Closed by:** `AC02_TheGateSequencesArePinned` green in Release and in Debug,
  and in CI.
- **Fails when:** the drain moves from the start of the tick to its end — the
  pin of the run with commands turns red while `AC02_DeterminismAcrossRuns`
  stays green, because two runs of the moved drain still agree with each other.
  And a fresh `WorldState` starting at tick 1 instead of 0 — the pin of the empty
  run turns red.
- **Core:** no — adds a test and touches no file under `core/Runtime/`.
  Verifiable by `git diff --name-only HEAD -- core/Runtime/` being empty.

*Esito:* 2026-10-01. Chiuso con riserva — "and in CI" attende il push del
branch, che è dell'umano. `AC02_TheGateSequencesArePinned` verde, anche da solo:
SHA-256 delle due sequenze da 100k tick di `AC02_DeterminismAcrossRuns`, vuota e
con comandi, contro due letterali presi da un run in Release sulla macchina di
sviluppo; la build Debug li ritrova. Suite intera verde in Release (52), con tre
wealth band (52) e in Debug (53), build del core senza avvisi. Guasti di `Fails
when:` introdotti uno alla volta e visti rossi: drain spostato dopo l'avanzamento
del tick, rosso il pin con comandi mentre `AC02_DeterminismAcrossRuns` resta
verde; `WorldState` nuovo al tick 1, rosso il pin vuoto. Nessuno step CI
aggiunto: il test gira nei tre step di test esistenti. `git diff --name-only HEAD
-- core/Runtime/` vuoto: `Core: no` regge, revisori non eseguiti. Registro:
`D-104`, con un rilievo non trattato: `SIM_WEALTH_BANDS` non è letto da nulla,
per cui lo step a tre band ripete il primo.

*Riserva sciolta:* 2026-10-01 — branch pushato dall'umano fino a `c645462`, che
contiene `2ec9fad`. Run CI `36788322902`, `windows-2025-vs2026`: tutti gli step
verdi; suite passata in Release (52), con tre wealth band (52) e in Debug (53),
nessun test saltato, gli stessi conteggi della macchina di sviluppo. Il runner
ritrova i due letterali presi in locale: build e macchina diverse, come chiede
`R-034`.
