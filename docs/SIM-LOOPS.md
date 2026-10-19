# Phase 1 — Causal model

**Document:** SIM-LOOPS
**Status:** Draft
**Revision:** 2026-10-19
**Gate:** No flow without a source and a sink. Every reinforcing loop has a named balancing loop.

---

## Stocks

| Stock | Held by | Flows in | Flows out |
|---|---|---|---|
| Population | cohort, migrants in transit | births, immigration | deaths, emigration |
| Goods | cohort, transient | production, harvest, purchase, inbound trade, migrants arriving, seizure received | consumption, sowing, sale, outbound trade, spoilage, transit loss, emigrants leaving, seizure |
| Standing crop | cohort | sowing | harvest, loss before harvest or when the harvest window closes |
| Currency | cohort, treasury, transient | sale, wage, contract payment, recovery, kingdom spending, borrowing, migrants arriving | purchase, wage paid, tax, changer fee, repayment, interest, emigrants leaving |
| Debt | cohort, agent | borrowing, advances, accrued interest | repayment, seizure, write-off |
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
**B2 — Partial adjustment.** Allocation moves a capped fraction per year toward the best return, for activities that carry no rate of their own. Day labour carries its own and is held by its wage instead.
*FR-P-15, FR-P-16. Tuned by P-19 and P-73.*

### R3 — Urban growth
City attracts migrants → labour rises → production rises → wages rise → attracts more migrants.
**B3a — Import ceiling.** A city can import only what its links carry, origin surplus times haulage capacity over at most six links, and what the rows routed to it sell, which reach bounds. Attractiveness reads food actually available, so the city stops drawing migrants as it approaches the ceiling.
**B3b — Crowding mortality.** Density raises baseline deaths above births, so a city holds its size only by continued immigration.
*FR-M-03, FR-D-12 to FR-D-14, FR-M-18, DEC-071. Tuned by P-50 and P-79.*

**B1 does not apply here.** Land per capita raises output per worker for farming, and a city's wage comes from crafts. **Price does not brake this loop either**: it clamps at four times reference under DEC-017, so it saturates exactly where the pressure is greatest. The brake is the ceiling, not the signal.

### R13 — Market places rise and fall
A place grows past a threshold → rises a type, trades more groups and draws its neighbours' trade → its crafts and wage gain → attracts migrants → grows. In reverse, a place that falls a type loses its market for those groups and the rows routed to it, and its own people trade farther away, at a higher transit loss.
**B13a — The ceilings of R3.** Growth is held by the import ceiling and crowding mortality, B3a and B3b.
**B13b — Land per capita**, as B1, holds the decline: fewer people share the same land.
A place falls only below P-78 of its threshold, so a market lost is not regained by the next small rise. That keeps the loop from oscillating; it is not its brake.
*FR-M-16 to FR-M-18, FR-D-05, DEC-071, DEC-103. Tuned by P-77 and P-78.*

### R4 — Poverty, banditry, isolation
Poverty → displaced people → danger on links → haulage wages rise → trade stops → poverty deepens.
Poverty is the destitute row (FR-J-19). The displaced people are those of it who emigrate (FR-D-04a, B12b), and each raises danger on the link it takes.
**B4a — Contracts.** Danger raises contract issuance → adventurers suppress it.
**B4b — Garrison spending.** Kingdom treasury funds order on its roads.
*FR-J-16, FR-C-08, FR-M-13, FR-M-14, FR-D-04a. Tuned by P-40 and P-41.*

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
Hunger → seed eaten → less land sown → smaller harvest → deeper hunger, for years.
**B5c — Seed protection.** The reserve is untouched until the hunger deficit exceeds P-66, and is released only as far as the tier 0 need left unmet requires, which holds it there.
**B5d — Imported seed.** Grain imported over the trade graph is also seed.
*FR-D-03, FR-D-05, FR-P-10a, FR-P-10d. Tuned by P-66 against the onset of hunger mortality.*

### R6 — Link churn
Route becomes profitable → promoted to a link → flow rises → differential collapses → link dissolves → differential returns.
**B6 — Hysteresis.** Creation threshold strictly above dissolution threshold.
*FR-M-09. Tuned by P-33 and P-34.*

### R7 — Rationing by ability to pay
The poor cannot buy → they weaken and leave → labour falls → prices rise → more cannot buy.
**B7 — Own production first.** Producers eat before selling, so the loop reaches the people who grow the food only once they eat their seed reserve (B5c).
*FR-C-03, DEC-043.*

### R12 — Destitute labour
More destitute → more labour offered → lower wage → more members below the threshold → more destitute.
**B12a — Day labour leaves.** A lower wage drives the day-labourer share down under its own rate, and labour offered falls.
**B12b — The destitute emigrate.** Emigration draws on the destitute row first, so those a falling wage pushes below the threshold are the first to leave, and the labour they offered leaves with them. They leave as refugees and raise danger on the links they take, which feeds R4.
*FR-J-19, FR-J-21, FR-C-05, FR-D-04a, FR-M-13, DEC-090, DEC-096. Tuned by P-68 and P-69, which set who enters and leaves the row; B12a by the day labourer's own rate under FR-J-21, B12b by P-46.*

B12a fails once the day-labourer share is near zero. B12b acts only where a neighbour is more attractive by more than the hysteresis band of FR-D-09. In a region poor throughout nobody leaves, and the loop stops at the lower clamp of the wage (DEC-045), which bounds it without braking it, and at hunger (R5).

### R14 — Cold
Fuel short in winter → deaths → labour lost → less timber and charcoal made → fuel shorter.
**B14a — Fewer hearths.** Population loss reduces demand against the same stock, as B5a.
**B14b — The weather turns.** The need follows the warmth curve, so the loop slackens as the weeks warm and stops where the curve reaches zero.
*FR-P-03b, FR-P-08, FR-D-03, DEC-105. Tuned by P-81 and the warmth curves.*

Where the warmth curve never reaches zero, B14b only slackens the loop, and B14a holds it.

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
| Day labour against wage | a high wage draws day labourers, which lowers it | FR-J-21, FR-C-05 |

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
| Every reinforcing loop has a named balancing loop | Met for R1, R2, R4 to R7, R9 to R14; R3 below; R8 balances itself and terminates |
| Brakes are implemented by a requirement, not assumed | Met |
| Brakes have a tuning parameter | Met, several still Open, save B7, an order of phases with nothing to tune |
| R3 | Braked by B3a and B3b. Neither runs through price, which is what makes them hold. Verify in Phase 4 that the ceiling binds before starvation does |

---

## Open

1. Confirm that B3 alone holds urban growth, or add a second brake.
2. Decide whether land can be cleared, which would give it an inflow.
3. Confirm that no loop crosses between subsystems in a way this document misses. The candidate is contagion against trade volume, which suppresses its own transmission.
4. Full care under DEC-094 raises yield per unit of land up to twice. B1 and B3a assume it fixed; recheck both before care is enabled.
