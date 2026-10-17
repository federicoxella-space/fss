# Phase 2 — State inventory and invariants

**Document:** SIM-STATE
**Status:** Draft
**Revision:** 2026-10-17
**Gate:** The assert list is executable.

---

## Rule

Anything that influences a future tick lives here. Anything not listed here must be derivable from what is, or it is a determinism defect.

State is sparse per settlement (DEC-101). A cohort row that holds nothing in any field is not stored, and a field laid out per good stores only the goods whose value or carry is not zero. What is not stored reads as zero. The stored entries are ordered by id, and the hash and the save cover them and nothing else, so that two worlds holding the same values hash alike however they reached them.

Input is not state. A command the host has submitted and the simulator has not yet applied is input, as the rest of the command sequence of NFR-01 is: it sits outside this inventory and outside the state hash. A save carries it all the same (DEC-032).

---

## World

| Field | Type | Notes |
|---|---|---|
| tick | int64 | the only clock |
| worldSeed | uint64 | also the RNG root |
| generationParams | record | the values the host chose for this world, FR-G-02; travels with the save so any report reproduces |
| ruleVersion | int | version of the core's logic; drives materialisation on patch, DEC-033 |
| rulesData | byte[] | the rules file the world runs on, immutable; the hash folds its digest, DEC-100 |
| currencyTotal | int64 | in smallest units, the invariant target |

No RNG state. Randomness is `Hash(worldSeed, subject, tick, channel, index)`, under NFR-03 and DEC-081.

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
| variantCarry | fixed[] | per variant good of each class this settlement clears, sparse: the fraction of a unit the variant's share of a long clearing leaves, FR-P-03d; cleared when the settlement stops clearing the class |

---

## Cohort

Keyed `(settlementId, mestiere, wealthBand)`. Wealth band has one value in the shipping configuration, three in CI. A settlement holds a row only for a key present in it (DEC-101): at most 12 per settlement at Phase 4, eleven mestieri in the allocation vector and the destitute row.

| Field | Type | Notes |
|---|---|---|
| count | int | |
| ageDistribution | int[] | per age band of the rules file |
| currency | int64 | single integer in smallest units |
| goods | int[] | one per good the row holds, sparse |
| goodsCarry | fixed[] | laid out as goods: the fraction of a unit each holding carries, in [0, 1). Production, harvest and spoilage add to it or take from it, and a whole unit moves to or from goods when reached |
| buildings | int[] | per building type, owned by this cohort |
| buildingsCarry | fixed[] | per building type: decay at P-51 not yet a whole building |
| durables | int[] | per durable good the row holds, sparse, with accumulated wear, the carry of P-53 |
| debt | int64 plus category | owed, counterparty category only |
| interestCarry | fixed[2] | interest on debt and on advance not yet a whole smallest unit, one each |
| credit | int64 plus category | owed to this cohort |
| defaultMark | int | ticks remaining |
| hungerDeficit | fixed | per member, FR-D-03a; read by mortality and by the seed reserve; averaged by count when members arrive |
| transitionRemainder | fixed | carry of the share entering or leaving destitution; cleared when the row empties |
| sownLand | int[] | per crop; set at sowing, FR-P-10a |
| landInField | int[] | per crop; falls through the harvest window, FR-P-10b |
| condition | fixed[] | per crop, 0–20000, FR-P-10c |
| advance | int64 plus category | owed against the standing crop, FR-C-25; not included in debt; the lender's claim is in its credit |

Every rate applied to an integer field of a row keeps its fraction in a carry beside the field, in the field's layout, cleared when the row's count reaches zero (SIM-ECON, "Spoilage and integer rounding"). The carries above are those the update names; a rate added later brings its carry into this table.

---

## Trade link

Edge count bounded by the degree caps.

| Field | Type | Notes |
|---|---|---|
| from, to | id | |
| transportCost | fixed | from terrain, fixed |
| danger | fixed | FR-M-12 |
| volume | fixed | rolling, drives weakest-edge eviction |
| flowAccumulator | int[] | per good, sparse |
| lossCarry | fixed[] | per good, sparse: transit loss (FR-C-11) on this link's shipments not yet a whole unit; cleared when the link is evicted |

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
| kind | enum | caravan, convoy, merchant, adventurer, migrants |
| authoritative | bool | scheduled versus sampled |
| link or route | id[] | |
| departureTick, speed | int | |
| contents | int[] plus int[4] | goods and coin |
| weight | int | |

Migrants are a scheduled transient. Beside the fields above they carry a count, the key of the row they left and its hunger deficit (FR-D-03a); their currency is one integer in smallest units, as a cohort's (FR-C-13), not four denominations. A-02 counts it and A-04 their goods.

Sampled transients are not stored. They are enumerated on demand from the draw of NFR-03, with the link as subject.

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
| causeKind | enum | event, player action, exogenous root: the three FR-E-07 allows |
| cause | chronicle id | the triggering entry when causeKind is event, none otherwise; the pointer that makes chains testable |
| importance | int | drives propagation |
| propagation | bounded per settlement | which entries have reached where, with degradation |

---

## Static data, not state

Goods, recipes, mestieri, crops, climate curves, event class definitions, goods classes, needs table, wealth curves, and the values of the parameters that drive the simulation. They never change during a run. They arrive in the rules file the host supplies, or in the base rules compiled into the core when it supplies none (DEC-100), and World holds the bytes whole so that a save carries them. The content is not state; the bytes that carry it are, as World's `rulesData`, immutable for the life of the world.

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
| A-11 | Every fixed-point quantity held in state is inside its declared range |
| A-11b | No accumulator has overflowed. Checked in debug builds, where the cost of checking every intermediate is affordable |
| A-12 | Danger on every link is inside its range |
| A-13 | Each cadence bucket fires exactly once per its period, and consecutive firings of a bucket are exactly one period apart |
| A-14 | No cohort holds negative count, currency, or goods |
| A-15 | Buried stock only decreases |
| A-16 | For every cohort and crop, landInField ≤ sownLand; for every settlement, sownLand summed over its cohorts and crops ≤ land |
| A-17 | Destitute rows hold no buildings and no durables |
| A-18 | No stored cohort row is empty in every field, no stored entry of a sparse field is zero with a zero carry, and the stored entries of every sparse field are in id order |

A-02 and A-03 together are the currency invariant. A-01 and A-14 are the population invariant. A-04 is the goods invariant.

A-13 is the one assert about the schedule rather than about state: a firing leaves nothing in state for a per-tick check to read, and a check inside the loop would be the loop testing itself. It is verified by test over whole periods, at least one year, which covers every case, since the schedule is a function of the tick modulo 364. The second clause is what rules out drift: a level firing every 365 ticks still fires once in each of the first 364 years.

---

## Serialisation notes

- Struct of arrays, `EntityId` as index plus generation, no object references.
- The `int[4]`, `int[]`, `fixed[]`, `id[]` and bounded fields of the tables above are per-row quantities, stored flattened under DEC-085, never as an array per row.
- Version number and migration path from the first write.
- No reflection: the serialiser is generated or hand-written, per section 20 of SIM-REQ, "Downstream constraints".
- The state hash covers every field above. A field excluded from the hash is a determinism hole.
- A table's capacity is not state: the hash and the save cover the rows its count says are in use, and no further, under DEC-088.
- A save also carries the commands submitted and not yet applied, in submission order, beside the state and outside the hash.

---

## Open

1. Closed: age bands are content in the rules file, each saying whether its members offer labour. The base rules hold three, children, working age and elderly, and only working age offers labour. Their number is a width the content sets, fixed for the life of a world (DEC-085).
2. Decide whether `flowAccumulator` needs per-good history or a single rolling figure.
3. Decide the chronicle retention policy. Entries accumulate over 60 years and nothing above discards them.
4. Decide how the chronicle dates prior history. FR-G-03 runs it "before tick 0" and DEC-040 takes the result as tick 0, so entries written during it carry ticks that are negative once the run is relabelled, or later than the run's first tick if it is not.
5. Decide how an `EntityId` names its table, before a second table issues them. The chronicle's `entities` may mix settlements, kingdoms and agents, and an index plus a generation does not say which; DEC-083's "two distinct entities are distinct keys" holds across tables only if their indices share one space.
6. Decide how members move from one age band to the next: a rate per band, which under the rule of §Cohort needs a carry. Falls due with the demography subsystem, whose births enter the first band and whose deaths read every band (FR-D-03).