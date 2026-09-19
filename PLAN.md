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

## 5. The command queue, applied at one point in the tick

- **Does:** the queue the host pushes into and the defined point of the tick
  where it drains. Nothing outside the core writes state; the queue is that
  boundary, and `AC-02` names commands beside the seed.
- **Serves:** `FR-A-01`, `AC-02`
- **Closed by:** `FRA01_CommandsApplyAtOnePointInTheTick` green: the same
  commands submitted in the same order produce the same hash sequence, and
  submitting them at a different moment of the caller's loop changes nothing.
- **Core:** yes

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

## 8. The command-line runner

- **Does:** `sim` runs N ticks headless, dumps the state hash sequence, and
  writes it where CI can compare it. Sweeps and CSV metric series are named by
  NFR-12 and wait for phase 6, which is where the metrics exist.
- **Serves:** `NFR-12`, `AC-19`
- **Closed by:** `sim --ticks 1000 --seed 1 --hashes` printing one hash per tick,
  run from the CI workflow.
- **Core:** no — touches `harness/` only, verifiable by
  `git diff --name-only HEAD -- core/` being empty.

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
