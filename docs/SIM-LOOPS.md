# Phase 1 — Causal model

**Document:** SIM-LOOPS
**Status:** Draft
**Gate:** No flow without a source and a sink. Every reinforcing loop has a named balancing loop.

---

## Stocks

| Stock | Held by | Flows in | Flows out |
|---|---|---|---|
| Population | cohort | births, immigration | deaths, emigration |
| Goods | cohort | production, harvest, purchase, inbound trade | consumption, sale, outbound trade, spoilage, transit loss |
| Currency | cohort, treasury, transient | sale, wage, contract payment, recovery, kingdom spending, borrowing | purchase, wage paid, tax, changer fee, repayment, interest |
| Debt | cohort, agent | borrowing, accrued interest | repayment, seizure, write-off |
| Land | settlement | none | none, fixed by the generator |
| Buildings | cohort | construction | decay, destruction by events |
| Durables | cohort | production, trade | wear, destruction by events |
| Buried stock | world | none | recovery |
| Danger | trade link | displaced population | contracts fulfilled, garrison spending |
| Knowledge | settlement | neighbour refresh, rumour, adventurer deposit | ageing |
| Route candidacy | settlement | merchant journeys | decay, promotion to link |
| Legitimacy | kingdom | spending, order | tax pressure, lost territory, unrest |

Land has neither inflow nor outflow, which is a modelling choice rather than an oversight: clearing new land is not simulated.

---

## Loops

Each reinforcing loop is followed by the balancing loop that holds it, the requirement that implements the brake, and the parameter that tunes it. A reinforcing loop with no line beneath it is a defect.

### R1 — Migration spiral
Village loses people → output falls → attractiveness falls → loses more people.
**B1 — Land per capita.** Fewer people share the same land → output per worker rises → attractiveness recovers.
*FR-D-05. Tuned against the migration cap in FR-D-04.*

### R2 — Cobweb allocation
High price at sowing → everyone plants it → glut at harvest → price collapses → nobody plants it → shortage.
**B2 — Partial adjustment.** Allocation moves a capped fraction per year toward the best return.
*FR-P-15, FR-P-16. Tuned by P-19.*

### R3 — Urban growth
City attracts migrants → labour rises → production rises → wages rise → attracts more migrants.
**B3a — Import ceiling.** A city can import only what its links carry: origin surplus times haulage capacity, over at most six links. Attractiveness reads food actually available, so the city stops drawing migrants as it approaches the ceiling.
**B3b — Crowding mortality.** Density raises baseline deaths above births, so a city holds its size only by continued immigration.
*FR-M-03, FR-D-12 to FR-D-14, DEC-071. Tuned by P-50.*

**B1 does not apply here.** Land per capita raises output per worker for farming, and a city's wage comes from crafts. **Price does not brake this loop either**: it clamps at four times reference under DEC-017, so it saturates exactly where the pressure is greatest. The brake is the ceiling, not the signal.

### R4 — Poverty, banditry, isolation
Poverty → displaced people → danger on links → haulage wages rise → trade stops → poverty deepens.
**B4a — Contracts.** Danger raises contract issuance → adventurers suppress it.
**B4b — Garrison spending.** Kingdom treasury funds order on its roads.
*FR-J-16, FR-C-08, FR-M-13, FR-M-14. Tuned by P-40 and P-41.*

### R9 — Abandonment
Settlement empties → ruins appear → danger rises on nearby links → trade avoids the area → neighbours weaken → more abandonment.
**B9 — Resettlement.** An empty settlement has clamped, high land per capita, which under B1 makes it among the most attractive destinations there is.
*FR-D-07 to FR-D-11, FR-M-15, DEC-067, DEC-070. Tuned by P-46, P-47, P-48 against the ruin danger contribution.*

B9 oscillates on its own unless damped: attractiveness inverts as the migrants arrive. The hysteresis band, the land-proportional inbound cap, and the dwell time are what make it converge.

### R10 — Capital accumulation
Buildings raise capacity → output rises → surplus rises → more buildings.
**B10a — Maintenance.** Decay consumes output in proportion to the stock held.
**B10b — Labour.** Capacity beyond the workers available sits idle, and population is bounded by B3a.
**B10c — Price.** Producing more lowers the local price, so the allocation vector moves away from building.
*FR-P-17, FR-P-18, DEC-072. Tuned by P-51.* Site attributes under FR-P-19 bound what can be built at all, but they are a prerequisite rather than a brake.

### R11 — Wealth concentration
Lender collects interest → borrower loses capital → borrower produces less → borrows again on worse terms.
**B11a — Write-off.** Default stops at buildings and the residual falls on the lender, so bad lending is punished.
**B11b — Taxation.** Tax takes from where the currency sits.
**B11c — Kingdom spending.** Treasury returns currency to cohorts.
*FR-C-22 to FR-C-28, DEC-075. Tuned by P-55 and P-56.*

### R5 — Famine
Hunger → deaths → labour lost → output falls → deeper hunger.
**B5a — Fewer mouths.** Population loss reduces demand against the same stock.
**B5b — Land per capita**, as B1.
*FR-D-03, FR-D-05.*

### R6 — Link churn
Route becomes profitable → promoted to a link → flow rises → differential collapses → link dissolves → differential returns.
**B6 — Hysteresis.** Creation threshold strictly above dissolution threshold.
*FR-M-09. Tuned by P-33 and P-34.*

### R7 — Rationing by ability to pay
The poor cannot buy → they weaken and leave → labour falls → prices rise → more cannot buy.
**B7 — Own production first.** Producers eat before selling, so the loop cannot reach the people who grow the food.
*FR-C-03, DEC-043.*

### R8 — Treasure age
Rich hoards → adventurer income high → share of the mestiere grows → hoards deplete faster → income falls.
This one is balancing on its own, but it terminates: the stock does not return.
**B8 — Contract income** is what keeps the mestiere alive past the terminus.
*FR-J-16, AC-34.*

---

## Balancing loops that stand alone

| Loop | Mechanism | Requirement |
|---|---|---|
| Price against stock | scarcity raises price, which suppresses demand | FR-M-01 |
| Trade against differential | goods flow toward the high price, which lowers it | FR-M-03 |
| Wage against labour supply | scarce labour raises the wage, which draws labour in | FR-C-05 |
| Contagion against susceptibles | infection runs out of people to infect | FR-E-05 |
| Tax against legitimacy | pressure raises fragility, which limits what can be taken | FR-C-09 |

---

## Currency circuit

Currency has no sink. It moves and must return.

```
cohort ──purchase──> cohort
cohort ──wage──────> cohort
cohort ──tax───────> settlement treasury ──remittance──> kingdom treasury
kingdom treasury ──garrisons, works, holdings──> cohort
buried stock ──recovery──> cohort
kingdom metal stock ──minting──> kingdom treasury
```

The return path from kingdom treasury to cohorts is the one that can be forgotten. Without it the circuit is open and the world deflates over decades with no visible symptom until it is severe.

*FR-C-06, FR-C-08, DEC-046, AC-21.*

---

## Gate check

| Requirement of the gate | Status |
|---|---|
| Every flow has a source and a sink | Met, with land declared as a fixed stock |
| Every reinforcing loop has a named balancing loop | Met for R1, R2, R4, R5, R6, R7 |
| Brakes are implemented by a requirement, not assumed | Met |
| Brakes have a tuning parameter | Met, several still Open |
| R3 | Braked by B3a and B3b. Neither runs through price, which is what makes them hold. Verify in Phase 4 that the ceiling binds before starvation does |

---

## Open

1. Confirm that B3 alone holds urban growth, or add a second brake.
2. Decide whether land can be cleared, which would give it an inflow.
3. Confirm that no loop crosses between subsystems in a way this document misses. The candidate is contagion against trade volume, which suppresses its own transmission.
