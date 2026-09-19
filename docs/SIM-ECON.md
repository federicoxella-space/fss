# Economic core — settlement update

**Document:** SIM-ECON
**Status:** Draft
**Revision:** 2026-09-19
**Companion:** SIM-REQ, SIM-DEC, SIM-STATE

---

## Purpose

SIM-REQ says how prices, flows, and allocation behave. It does not say what a settlement update does, in what order. Order changes the economy: consuming before or after arrivals, clearing the local market before or after exporting, borrowing before or after buying are different worlds. This document fixes the sequence.

---

## Three choices embedded in the order

These are design decisions, not implementation details. Each is stated with the world it produces and the alternative it rejects.

| Choice | Taken | Alternative |
|---|---|---|
| Arrivals before consumption | A settlement does not starve while grain is on the road | Consumption first, so relief always arrives a step late and famine is sharper |
| Local clearing before export | Nobody exports while their own people go hungry | Export first, so obligations pull food out of starving settlements |
| Borrowing before clearing | A producer can borrow to buy the inputs it needs this update | Borrowing after, so credit always helps one update late |

The second is the one to revisit when rent, tax arrears, and forced sales enter the model.

---

## Settlement update sequence

Runs at settlement cadence, every 7 ticks, on the staggered bucket of FR-T-06. Every phase reads the state left by the previous one.

| # | Phase | Does | Implements |
|---|---|---|---|
| 1 | Arrivals | Goods in transit landing this tick enter stock, with transit loss applied. Migrants join cohorts. Chronicle entries arrive and age. | FR-C-11, FR-D-04, FR-I-02 |
| 2 | Prices | Compute local price per good from current stock. Compute the wage from labour supply against expected demand. | FR-M-01, FR-C-05 |
| 3 | Borrowing | Cohorts short of currency for inputs or needs borrow against collateral. Interest rate set. Advances against future output issued here. | FR-C-22 to FR-C-26 |
| 4 | Labour | Labour market clears. Workers are assigned to activities up to building capacity. Unhired labour is unemployment; wages are paid. | FR-C-05, FR-P-17 |
| 5 | Production | Recipes execute on inputs held. Crude variants run where durables are missing. Durables wear, buildings decay. Dated crop events fire if due. | FR-P-02, FR-P-10, FR-P-12, FR-P-20 to FR-P-23 |
| 6 | Own consumption | Each cohort meets its needs from its own holdings, by tier. | FR-C-03 |
| 7 | Market clearing | Unmet need becomes demand, capped by currency and credit line. Surplus becomes supply. Short supply is allocated by ability to pay. Currency moves buyer to seller. | FR-C-04 |

Phase 7 dominates the cost of the update and is the only part of it that grows with the square of the cohort count, because short supply is allocated in order of ability to pay. The ordering key must be computed once per cohort per good and then sorted, never recomputed inside the comparison: doing the division in the comparison turns a sort of a few dozen entries into a few thousand integer divisions per settlement update. This is the first thing to optimise and the thing that decides whether the wealth axis of DEC-054 fits the budget.

| 8 | Repayment | Debts falling due are repaid. Default seizes goods, then durables, then buildings. Residual is written off. Default marks are set and aged. | FR-C-27, FR-C-28 |
| 9 | Spoilage | Perishability applied to all held goods, with remainder carry. | FR-P-13 |
| 10 | Export | Outbound flow computed per link from the price differential, capped by surplus stock and by haulage capacity. Goods leave and enter transit. | FR-M-03, FR-M-03a, FR-J-04 |
| 11 | Demography | Births, deaths including hunger and crowding mortality, migration decisions under the hysteresis band. | FR-D-01 to FR-D-13 |
| 12 | Bookkeeping | Fragility metrics updated. Chronicle entries emitted. Knowledge table refreshed for neighbours and aged for rumours. `changeCapacity` recomputed. | FR-E-03, FR-I-01, FR-I-05, FR-C-16 |

Phases 3 and 8 are the two halves of credit: borrowing before the market, settlement after it.

---

## Dated and annual steps

These run inside phase 5 or phase 12 when their date falls in the current update window, subject to the six-tick lag of FR-P-12.

| Step | When | Does |
|---|---|---|
| Sowing | crop sowing date, shifted by climate | Commits land and labour to a crop for the season |
| Harvest | crop harvest date, shifted by climate | Delivers full yield to the growing cohort's holdings |
| Allocation adjustment | at sowing | Moves the allocation vector a damped step toward the best expected return; occupational shares move with it |
| Taxation | annual | Settlement collects from cohort currency, retains a share, sends the rest as a convoy |
| Construction | continuous | Consumes timber and labour; completed buildings raise capacity |

---

## Demand

A need is defined per good per person per day, by tier.

| Tier | Name | Deferrable | Unmet consequence |
|---|---|---|---|
| 0 | Survival | no | hunger accumulates, mortality rises |
| 1 | Basic | yes | discontent rises, feeding fragility |
| 2 | Comfort | yes | no direct consequence |

**Quantity demanded at market**, per cohort per good:

```
need        = perCapitaNeed(tier, good) × count
unmet       = max(0, need − ownHoldings)
affordable  = (currency + creditLine) / price
demand      = min(unmet, affordable)
```

Tiers are satisfied in order: a cohort spends on tier 0 before tier 1 before tier 2. Tier 2 demand additionally takes only a fraction of currency remaining after the lower tiers, so comfort goods are bought from surplus rather than to the last coin.

**Rationing.** When supply is short, allocation runs by descending ability to pay within a tier, never across tiers: nobody buys cloth while another cohort in the same settlement cannot buy bread and could pay for it.

**Hunger.** Unmet tier 0 accumulates as a deficit rather than killing instantly. The deficit drives mortality in phase 11 and decays when feeding resumes. This is what makes a famine a season rather than a tick.

---

## Spoilage and integer rounding

Perishability is a fraction per good per update, applied to everything held: cohort stocks, settlement treasuries hold no goods, transients apply theirs on arrival as transit loss.

**The integer trap.** A 2% rate on a stock of 10 units is 0.2, which truncates to zero. Small stocks would never spoil and would become immortal, while large stocks decay normally. The fix is a **remainder accumulator per cohort per good**: the fractional part carries to the next update and is applied when it reaches a whole unit. Deterministic, conserving, and it removes the size threshold.

The same accumulator pattern applies anywhere a rate multiplies a small integer: durable wear, building decay, interest on small debts, transit loss. It is a general rule of the codebase, not a spoilage detail.

---

## What the sequence does not yet cover

1. **Forced sale.** Nothing makes a cohort sell food it needs in order to pay tax or debt. The order in phase 7 and 8 protects it. Adding rent or arrears reopens choice two above.
2. **Stockpiling.** `desired_stock` in FR-M-01 is a parameter, not a decision. No cohort chooses to hold grain back for a bad year, which is a real medieval behaviour and a source of price dynamics.
3. **Contract issuance.** Closed: FR-K-09 places it in phase 12, where fragility metrics generate proposals and issuers fund them. Guild trading runs in phases 7 and 10 as an ordinary merchant cohort.
4. **Kingdom spending.** Annual, at kingdom cadence, and not yet placed relative to the settlement update that receives it.
5. **Per-capita need values.** The tier table has a shape and no numbers.
