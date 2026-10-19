# Economic core — settlement update

**Document:** SIM-ECON
**Status:** Draft
**Revision:** 2026-10-19
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
| 1 | Arrivals | Goods in transit landing this tick enter stock, with transit loss applied. Migrants join cohorts, their hunger deficit averaged into the row's. Chronicle entries arrive and age. | FR-C-11, FR-D-04, FR-D-03a, FR-I-02 |
| 2 | Prices | Compute local price per clearing from current stock; a variant's price from its class's (FR-P-03d); a good traded at a market place takes that place's price (FR-M-20). Compute the wage from labour supply against expected demand. | FR-M-01, FR-C-05 |
| 3 | Borrowing | Cohorts short of currency for inputs or needs borrow against collateral. Interest rate set. Advances against future output issued here. | FR-C-22 to FR-C-26 |
| 4 | Labour | Labour market clears. Workers are assigned to activities up to building capacity, and to every crop stage active in this update; where labour is in excess, sellers are hired in proportion to what they offer. Labour still unhired is assigned to open fields (FR-P-10e); the rest is unemployment. Wages are paid. | FR-C-05, FR-P-17, FR-P-10e |
| 5 | Production | Recipes execute on inputs held, an input on a class taking the goods held lowest grade first. Crude variants run where durables are missing. Durables wear, buildings decay. Dated crop events fire if due. Sowing consumes the planting good. Harvest reaps what labour allows from the area still standing, open fields included. Recipes never draw on a seed reserve. | FR-P-02, FR-P-10, FR-P-10a, FR-P-10b, FR-P-10c, FR-P-10e, FR-P-12, FR-P-20 to FR-P-23 |
| 6 | Own consumption | Each cohort meets its needs from its own holdings, by tier, lowest grade first, leaving its seed reserve untouched unless its hunger deficit exceeds P-66, and then drawing on it only for the tier 0 need left unmet. | FR-C-03, FR-P-10d |
| 7 | Market clearing | Runs for the clearings this settlement clears: at a market place those of the groups it trades, first within each place taking part, among its own rows, then over its own rows and the rows routed to it in one ranking (FR-M-19); anywhere, the groups it has no market place for within reach and the goods in no group (FR-M-18). Unmet need becomes demand, capped by currency, which includes what phase 3 lent, and a producer demands the inputs its activity consumes in one update at capacity, less what it holds. Surplus, the seed reserve and those inputs excluded, becomes supply. Short supply is allocated by ability to pay, the share of a cohort able to pay read from its wealth curve; long supply in a class is shared among its variants by what each offers (FR-P-03d). Currency moves buyer to seller. Last, each cohort's hunger deficit grows by the tier 0 need still unmet, or decays if it was met. | FR-C-04, FR-P-10d, FR-J-22, FR-D-03a, FR-M-18, FR-M-19 |
| 8 | Repayment | Debts falling due are repaid. Default seizes goods, then durables, then buildings. Residual is written off. Default marks are set and aged. | FR-C-27, FR-C-28 |
| 8a | Destitution | Cohort members whose wealth falls below the threshold enter the destitute row. Destitute members above the exit threshold leave for their destinations: for agricultural activities only in an update holding a crop's sowing date. | FR-J-19, FR-J-20, FR-D-03a |
| 9 | Spoilage | Perishability applied to all held goods, with remainder carry. | FR-P-13 |
| 10 | Export | Outbound flow computed per link from the price differential, capped by surplus stock and by haulage capacity. Goods leave and enter transit. | FR-M-03, FR-M-03a, FR-J-04 |
| 11 | Demography | Births, deaths including hunger and crowding mortality, migration decisions under the hysteresis band. Emigrants are drawn from the destitute row first and leave with their share under FR-D-04a, as migrants in transit; destitute emigrants raise danger on the link they take (FR-M-13). | FR-D-01 to FR-D-13, FR-M-13 |
| 12 | Bookkeeping | Fragility metrics updated. Chronicle entries emitted. Knowledge table refreshed for neighbours and aged for rumours. `changeCapacity` recomputed. Mestiere shares are realigned to the allocation vector for the activities that adjusted in this update, members carrying their hunger deficit. Every 28 ticks, type and market assignments are reassessed. | FR-E-03, FR-I-01, FR-I-05, FR-C-16, FR-J-03, FR-D-03a, FR-M-17, FR-M-18 |

Phase 7 dominates the cost of the update and is the only part of it that grows with the square of the rows it ranks, routed rows included, because short supply is allocated in order of ability to pay. The ordering key must be computed once per cohort per clearing and then sorted, never recomputed inside the comparison: doing the division in the comparison turns a sort of a few dozen entries into a few thousand integer divisions per settlement update. This is the first thing to optimise and the thing that decides whether the wealth axis of DEC-054 fits the budget.

Phases 3 and 8 are the two halves of credit: borrowing before the market, settlement after it.

A market place's phase 7 writes the holdings and currency of the rows routed to it, whose own updates fall in other buckets (FR-T-06). A routed row therefore buys and sells between two of its own updates, and consumes what it bought in its next phase 6; its hunger deficit is written at the end of its own phase 7, from the tier 0 need its phase 6 left unmet.

---

## Dated and annual steps

These run inside phase 5 or phase 12 when their date falls in the current update window, subject to the six-tick lag of FR-P-12.

| Step | When | Does |
|---|---|---|
| Sowing | crop sowing date, shifted by climate | Commits land, labour, and the planting good to a crop for the season |
| Harvest | crop harvest date, shifted by climate | Opens the harvest window; each update reaps what harvest labour allows |
| Allocation adjustment | at sowing for a crop's activity, at settlement cadence for the others (FR-P-15) | Moves the allocation vector a damped step toward the best expected return; each activity moves at its own rate (FR-P-15); occupational shares are realigned in phase 12, after the destitute leave in phase 8a |
| Taxation | annual | Settlement collects from cohort currency, retains a share, sends the rest as a convoy |
| Construction | continuous | Consumes timber and labour; completed buildings raise capacity |

---

## Demand

A need is defined per goods class, minimum grade and tier, per person per day (FR-P-03b).

| Tier | Name | Deferrable | Unmet consequence |
|---|---|---|---|
| 0 | Survival | no | hunger accumulates, mortality rises |
| 1 | Basic | yes | discontent rises, feeding fragility |
| 2 | Comfort | yes | no direct consequence |

Tiers belong to needs, not goods.

**Quantity demanded at market**, per cohort per need, placed on the goods that meet it under "Choice within a class" below:

```
need        = perCapitaNeed(class, grade, tier) × count
unmet       = max(0, need − value of own holdings meeting it)
affordable  = currency / price per unit of value, currency including what phase 3 lent
demand      = min(unmet, affordable)
```

Tiers are satisfied in order: a cohort spends on tier 0 before tier 1 before tier 2. Tier 2 demand additionally takes only a fraction of currency remaining after the lower tiers, so comfort goods are bought from surplus rather than to the last coin.

**Inputs.** A cohort whose activity is a recipe demands, for each input, what one update at capacity consumes, less what it holds; capacity is the buildings it owns, or its own working members where the activity needs no building. The inputs are paid from what is left once tiers 0 and 1 are met, before anything else, and they rank in the same place (DEC-104). What a producer holds of its inputs up to that quantity is not supply.

**Rationing.** When supply is short, allocation runs by descending ability to pay within a tier, never across tiers, and the share of a cohort able to pay is read from its wealth curve: nobody buys cloth while another cohort in the same settlement cannot buy bread and could pay for it. Demand for inputs ranks after tiers 0 and 1 and before tier 2: nobody buys grain to mill while another cannot buy grain to eat and could pay for it.

**Hunger.** Unmet tier 0 accumulates as a deficit rather than killing instantly. The deficit is updated at the end of phase 7, once all the food of the update is known; it drives mortality in phase 11 of the same update, is read by the seed reserve in phase 6 of the next, and decays when feeding resumes. This is what makes a famine a season rather than a tick. The deficit is cohort state (FR-D-03a), and the seed reserve reads it (FR-P-10d).

**Choice within a class.** Needs add (FR-P-03b). A cohort meets them tier by tier from its own holdings, lowest grade first, and buys what remains of each at the lowest price per unit of value among the goods that meet it. With the currency left once tiers 0 and 1 are met, and before tier 2 takes its fraction, members replace units of a lower grade with units of a higher grade that meet the same need, the share of members able to pay for each grade read from the wealth curve. In rationing, demand that meets a need comes before demand that only raises a grade. Demand rationed out of one good falls, within the update, to the other goods that meet the same need, cheapest per unit of value first; the order in which a class's markets clear is derived from grade, price and good id. A class whose variants share a clearing (FR-P-03d) is one market in that order, at its price per unit of value.

---

## Spoilage and integer rounding

Perishability is a fraction per good per update, applied to everything held: cohort stocks, settlement treasuries hold no goods, transients apply theirs on arrival as transit loss.

**The integer trap.** A 2% rate on a stock of 10 units is 0.2, which truncates to zero. Small stocks would never spoil and would become immortal, while large stocks decay normally. The fix is a **remainder accumulator per cohort per good**: the fractional part carries to the next update and is applied when it reaches a whole unit. Deterministic, conserving, and it removes the size threshold.

The same accumulator pattern applies anywhere a rate multiplies a small integer: durable wear, building decay, interest on small debts, transit loss. It is a general rule of the codebase, not a spoilage detail. The carry stays with its row and is cleared when the row's count reaches zero. Transit loss is the exception in place, not in kind: a shipment lands once, so its carry is held by the link it travels and cleared when the link is evicted. SIM-STATE lists every carry. Splits made within one update, such as gleaning shares or the destinations of the destitute, use largest remainders with ties by id and leave no carry.

---

## What the sequence does not yet cover

1. **Forced sale.** Nothing makes a cohort sell food it needs in order to pay tax or debt. The order in phase 7 and 8 protects it. Adding rent or arrears reopens choice two above.
2. **Stockpiling.** `desired_stock` in FR-M-01 is a parameter, not a decision. No cohort chooses to hold grain back for a bad year, which is a real medieval behaviour and a source of price dynamics. The seed reserve of FR-P-10d is a forced reserve, not a choice: it holds next season's seed and nothing beyond it.
3. **Contract issuance.** Closed: FR-K-09 places it in phase 12, where fragility metrics generate proposals and issuers fund them. Guild trading runs in phases 7 and 10 as an ordinary merchant cohort.
4. **Kingdom spending.** Annual, at kingdom cadence, and not yet placed relative to the settlement update that receives it.
5. **Per-capita need values.** The needs table has a shape per class, grade and tier and no numbers.
