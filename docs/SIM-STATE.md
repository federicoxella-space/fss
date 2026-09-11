# Phase 2 — State inventory and invariants

**Document:** SIM-STATE
**Status:** Draft
**Gate:** The assert list is executable.

---

## Rule

Anything that influences a future tick lives here. Anything not listed here must be derivable from what is, or it is a determinism defect.

---

## World

| Field | Type | Notes |
|---|---|---|
| tick | int64 | the only clock |
| worldSeed | uint64 | also the RNG root |
| generationParams | record | travels with the save so any report reproduces |
| ruleVersion | int | drives materialisation on patch, DEC-033 |
| currencyTotal | int64 | in smallest units, the invariant target |

No RNG state. Randomness is `Hash(worldSeed, entityId, tick, channel, index)`.

---

## Settlement

Always simulated. Roughly 2,000 rows.

| Field | Type | Notes |
|---|---|---|
| id, position, type | | type drives degree caps |
| climateIndex | fixed | 0–10000, from the generator |
| land | int | fixed, feeds B1 |
| allocationVector | fixed[] | sums to one |
| changeCapacity | enum | largest denomination breakable |
| treasury | int[4] | explicit denominations |
| knowledgeTable | bounded record[] | neighbour entries plus rumours, each with age |
| routeCandidates | bounded record[] | capped at P-32, decaying |
| cropSchedule | derived | from climateIndex, not stored |
| siteAttributes | bitfield | ore, water, forest, dungeon; gates building types |
| guildHall | flag plus record | contract book, escrow int[4], rank distribution int[] |
| dungeonState | enum plus int | infested, cleared, repopulating; regenerating stock |
| knownRecipes | bounded set | recipes recovered or learned here |

---

## Cohort

Keyed `(settlementId, mestiere, wealthBand)`. Wealth band has one value in the shipping configuration, three in CI. Roughly 9 rows per settlement at Phase 4.

| Field | Type | Notes |
|---|---|---|
| count | int | |
| ageDistribution | int[] | bands |
| currency | int64 | single integer in smallest units |
| goods | int[] | one per good |
| wealthSpread | fixed | drives the closed-form affordability fraction |
| buildings | int[] | per building type, owned by this cohort |
| durables | int[] | per durable good, with accumulated wear |
| debt | int64 plus category | owed, counterparty category only |
| credit | int64 plus category | owed to this cohort |
| defaultMark | int | ticks remaining |

---

## Trade link

Edge count bounded by the degree caps.

| Field | Type | Notes |
|---|---|---|
| from, to | id | |
| transportCost | fixed | from terrain, fixed |
| danger | fixed | FR-M-12 |
| volume | fixed | rolling, drives weakest-edge eviction |
| flowAccumulator | int[] | per good |

---

## Kingdom

| Field | Type |
|---|---|
| treasury | int[4] |
| metalStock | int |
| taxRate | fixed |
| legitimacy | fixed |
| holdings | id[] |

---

## Agent

Capped at roughly 300 hot. Dormant agents hold a difference record only.

| Field | Type | Notes |
|---|---|---|
| id | EntityId | index plus generation |
| home, mestiere | | |
| currency | int[4] | explicit denominations |
| inventory | int[] | |
| relations | bounded edge[] | fixed max degree, weakest evicted |
| debts | bounded edge[] | amount, term, rate, collateral |
| difference | sparse record | DEC-012 |

---

## Transient

| Field | Type | Notes |
|---|---|---|
| kind | enum | caravan, convoy, merchant, adventurer |
| authoritative | bool | scheduled versus sampled |
| link or route | id[] | |
| departureTick, speed | int | |
| contents | int[] plus int[4] | goods and coin |
| weight | int | |

Sampled transients are not stored. They are enumerated from `Hash(worldSeed, link, index)` on demand.

---

## Buried stock

| Field | Type |
|---|---|
| location | position or settlement id |
| contents | int[] plus int[4] |
| recovered | bool |

---

## Chronicle

| Field | Type | Notes |
|---|---|---|
| id, tick, location | | |
| entities | id[] | |
| cause | chronicle id | the pointer that makes chains testable |
| importance | int | drives propagation |
| propagation | bounded per settlement | which entries have reached where, with degradation |

---

## Static data, not state

Goods, recipes, mestieri, crops, climate curves, event class definitions. Versioned with `ruleVersion`, never mutated at runtime.

---

## Invariants, asserted every tick

| # | Assert |
|---|---|
| A-01 | Sum of cohort counts equals settlement population, for every settlement |
| A-02 | Sum of cohort currency, plus all treasuries, plus transient contents, plus buried stock, equals `currencyTotal` |
| A-03 | `currencyTotal` changes only on an explicit minting or destruction event |
| A-04 | Goods leaving a settlement arrive, spoil, or are lost in transit, with the loss recorded |
| A-05 | Every allocation vector sums to one |
| A-06 | No settlement exceeds its degree cap |
| A-07 | Total edge count is within the bound implied by the degree caps |
| A-08 | Every knowledge table and route candidate list is within its cap |
| A-09 | Every agent relation list is within its degree cap |
| A-10 | Hot agent count is at or below the cap |
| A-11 | Every fixed-point quantity is inside its declared range; no accumulator has overflowed |
| A-12 | Danger on every link is inside its range |
| A-13 | Each cadence bucket fires exactly once per its period |
| A-14 | No cohort holds negative count, currency, or goods |
| A-15 | Buried stock only decreases |

A-02 and A-03 together are the currency invariant. A-01 and A-14 are the population invariant. A-04 is the goods invariant.

---

## Serialisation notes

- Struct of arrays, `EntityId` as index plus generation, no object references.
- Version number and migration path from the first write.
- No reflection: the serialiser is generated or hand-written, per section 19 of SIM-REQ.
- The state hash covers every field above. A field excluded from the hash is a determinism hole.

---

## Open

1. Confirm the age band count for cohorts, which multiplies cohort width.
2. Decide whether `flowAccumulator` needs per-good history or a single rolling figure.
3. Decide the chronicle retention policy. Entries accumulate over 60 years and nothing above discards them.
