# Socio-Economic Simulator — Requirements

**Document:** SIM-REQ
**Status:** Draft 1
**Revision:** 2026-09-19
**Scope:** Headless simulation core, runnable outside the game

---

## 1. Purpose

The simulator advances a fantasy-medieval world from a generated initial state. The world runs on its own; the player lives inside it, participates in events, and is not its protagonist. The core ships as a C# class library with no host dependency, driven either by a game client or by a command-line harness.

**Estimates in this document are marked as such.** Any figure labelled *(estimate)* comes from back-of-envelope sizing and requires prototype measurement before it becomes a commitment.

---

## 2. Glossary

| Term | Meaning |
|---|---|
| **Tick** | One logical day of world time. The smallest unit the simulator advances. |
| **Cadence** | How often a given simulation level updates, expressed in ticks. |
| **Settlement** | A village, town, or city. The unit at which markets, production, and population live. |
| **Cohort** | A population group inside a settlement, described by statistics rather than individuals. |
| **Climate index** | A continuous fixed-point value per settlement driving weekly climate modifiers and crop dates. |
| **Crop calendar** | Per-crop sowing and harvest dates, shifted per settlement by its climate index. |
| **Agent** | A named individual with identity and persistent state. |
| **Hot agent** | An agent currently simulated at full fidelity. |
| **Trade link** | A directed edge between two settlements along which goods and information move. |
| **Bacino / Basin** | The set of settlements trading with a common hub. Derived from the trade graph. |
| **Chronicle entry** | A record of a world event, carrying cause, location, tick, and importance. |
| **Divergence** | Two runs with identical seed and inputs producing different state. |

---

## 3. World representation

**FR-W-01** — The world state holds settlements, cohorts, trade links, goods stocks, prices, agents, kingdoms, and the chronicle log. Anything that influences future ticks lives inside the world state.

**FR-W-02** — Every settlement is simulated explicitly for the whole run. No settlement is generated on demand at runtime.

**FR-W-03** — Population inside a settlement is represented as cohorts. Cohorts carry counts, age distribution, occupation, wealth band, and food stock.

**FR-W-04** — An agent is created from cohort statistics when the player comes within interaction range. Creating an agent subtracts it from its cohort so that no person is counted twice.

**FR-W-05** — An agent that the player leaves without meaningful interaction returns to its cohort and stops consuming state. An agent the player has affected persists as a difference from what the deterministic rules would have produced.

**FR-W-06** — Each measurable quantity (population, currency, goods) has exactly one owning level. Levels below it sum to it.

**FR-W-07** — The simulator references no game engine, rendering, input, or asset API, and knows nothing about the host that drives it.

**FR-W-08** — A cohort is keyed by a tuple. The shipping configuration uses `(mestiere, wealthBand)` with a single wealth band value. No code may assume one cohort per mestiere, and none may index cohorts by mestiere alone.

**FR-W-09** — CI runs the full suite against a configuration with three wealth bands. It is not shipped; it exists to break any code that assumed a single row per mestiere.

**FR-W-10** — A settlement is never deleted. Population zero means an empty row, not a removed one: identity, position, land, ruins, and buried stock persist. Settlement count is fixed for the run.

---

## 4. Time

**FR-T-01** — Time is an integer tick counter. The simulator reads no system clock and no frame delta.

**FR-T-02** — The year is 364 ticks: 13 months of 28 days, each month exactly 4 weeks of 7 days, so 52 weeks per year. Every calendar date falls on the same weekday in every year.

**FR-T-03** — New Year's Day sits between the last tick of one year and the first tick of the next and consumes no tick. The world does not advance during it.

**FR-T-04** — Cadence is a property of the simulation level, not of distance from the player:

| Level | Cadence | Ticks |
|---|---|---|
| Hot agent | daily | 1 |
| Settlement (market, production, demography) | weekly | 7 |
| Basin | monthly | 28 |
| Kingdom (taxation, budget, holdings) | annual | 364 |

Each cadence divides the next: 7 × 4 = 28, 28 × 13 = 364.

**FR-T-05** — A season is 91 ticks, four per year, starting at days 1, 92, 183 and 274. Season 1 begins the year and is spring. Seasons name the time of year for presentation and dialogue; they carry no mechanical effect of their own.

**FR-T-05a** — Date arithmetic is integer:

```
year       = tick / 364
dayOfYear  = tick % 364          // 0..363
season     = dayOfYear / 91      // 0..3
week       = dayOfYear / 7       // 0..51
month      = dayOfYear / 28      // 0..12
dayOfMonth = dayOfYear % 28
```

**FR-T-06** — Settlements update in staggered buckets. On tick *d*, the simulator updates settlements whose id satisfies `id % 7 == d % 7`. The bucket derives from the id, never from iteration order.

**FR-T-07** — Political and military change (war declaration, succession, rebellion, alliance) is triggered by endogenous thresholds under FR-E-04, not by the kingdom cadence.

**FR-T-08** — The simulator advances by the same rules whether the player is present, absent, or fast-travelling. Fast travel advances N ticks with no hot agents.

**FR-T-09** — The caller controls how many ticks run. The simulator never decides to run "in real time".

---

## 5. Population and demography

**FR-D-01** — Settlements gain and lose population through three flows: births, deaths, migration. All three run on integers.

**FR-D-02** — Birth rate responds to food availability per capita and crowding.

**FR-D-03** — Death rate is per age band and responds to hunger, disease, and violence.

**FR-D-04** — Migration moves people along trade links toward higher attractiveness, where attractiveness combines food per capita, real wage, and safety. Migration per tick is capped.

**FR-D-05** — Land per capita raises output per worker, which raises attractiveness. This is the balancing term against the migration feedback loop and is required, not optional.

**FR-D-06** — Agents age, reproduce, and die under the same demographic model that drives cohorts.

**FR-D-07** — A settlement at population zero remains a migration destination. Abandoned land is resettled and abandonment is not an absorbing state.

**FR-D-08** — Land per capita is clamped at P-48. An empty settlement is highly attractive, not infinitely so, and the quantity is defined at population zero.

**FR-D-09** — Migration responds to an attractiveness differential compared against a hysteresis band. A settlement receiving migrants keeps receiving until the differential falls below the lower bound (P-46).

**FR-D-10** — Inbound migration per tick is capped in proportion to the destination's land, not its population, so an empty settlement has a non-zero ceiling.

**FR-D-11** — A settlement cannot reverse between receiving and sending within P-47 ticks. Together with FR-D-08 to FR-D-10 this stops attractiveness from inverting in the tick the migrants arrive.

**FR-D-12** — Attractiveness includes food per capita computed from stock actually available after consumption, not from stock ordered or expected. A settlement approaching its import ceiling stops attracting migrants before it starts starving them.

**FR-D-13** — Crowding raises baseline mortality (P-50), independently of contagion. A dense settlement is a demographic sink that grows only by immigration.

**FR-D-14** — A settlement's sustainable population is bounded by what it can import: the sum over its links of origin surplus and haulage capacity. The bound is not a rule of its own; it falls out of the caps already in FR-M-03, and rewiring under FR-M-09 can move it.

---

## 6. Goods and production

**FR-P-01** — A good is a data record: unit weight, value reference, perishability, role, need tier. Adding a good requires no code change.

**FR-P-02** — A recipe is a data record: inputs, output, labour required, building required. Adding a recipe requires no code change.

**FR-P-03** — Every good occupies one of four roles:

| Role | Behaviour |
|---|---|
| Subsistence | consumed daily, cannot be deferred |
| Input | consumed by production |
| Durable | not consumed, decays slowly, raises productivity |
| Luxury | bought at market only from currency left after needs are met |

**FR-P-04** — Baseline goods set *(proposed, pending confirmation)*: Grain, Bread, Ore, Timber, Tools, Wool, Cloth. Three chains: Grain→Bread, Ore+Timber→Tools, Wool→Cloth.

**FR-P-05** — Weight-to-value ratio governs how far a good travels. Heavy cheap goods stay in their basin; light valuable goods cross kingdoms. This falls out of transport cost and needs no per-good code.

**FR-P-06** — Production consumes labour from cohorts. Missing labour reduces output.

**FR-P-07** — Each settlement carries a `climateIndex`, a fixed-point value on 0–10000 assigned by the generator. The simulator consumes it without knowing what produced it.

**FR-P-08** — Two authored 52-entry weekly curves, `cold` and `warm`, define climate modifiers across the year. A settlement's modifier for week *w* interpolates between them on its `climateIndex`, in integer arithmetic. Climate modifiers govern travel cost, mortality, livestock growth, timber yield, and available construction labour.

**FR-P-09** — Named climate bands (3–4) group `climateIndex` ranges for presentation only. No simulation rule reads a band.

**FR-P-10** — Crops are not produced continuously. A crop is a data record carrying sowing date, maturation length, harvest date, and yield per unit of land and labour. Harvest delivers the full amount to settlement stock in one event.

**FR-P-11** — A settlement's sowing and harvest dates shift by an integer day offset derived from its `climateIndex`. Adjacent settlements differ by at most one day.

**FR-P-12** — A dated crop event fires on the first settlement update at or after its date, so it can lag its nominal date by up to six ticks.

**FR-P-13** — Stored crop stock decays under the good's perishability between one harvest and the next.

**FR-P-14** — Each settlement holds an allocation vector in fixed point over its available activities, summing to one, covering land and labour.

**FR-P-15** — At each crop's sowing date the settlement computes expected return per activity as expected yield × current price − input cost − wage cost, then moves the allocation toward the highest by adjustment rate P-19, capped per year. Non-agricultural activities adjust the same way at settlement cadence.

**FR-P-16** — The adjustment is partial and damped. Full adjustment to last year's prices produces a diverging cobweb oscillation and is prohibited.

### Buildings and durables

**FR-P-17** — Buildings are an explicit stock owned by cohorts, built by consuming timber and labour, decaying at P-51, and destroyable by events.

**FR-P-18** — Construction competes for land and labour inside the allocation vector of FR-P-14. It is an activity, not a separate budget.

**FR-P-19** — A building type may require a site attribute placed by the generator: an ore deposit, flowing water, standing forest. Where the attribute is absent the building cannot exist. Comparative advantage between settlements comes from here, and without it settlements converge on identical economies and trade disappears.

**FR-P-20** — A recipe declares its relation to durables: required, improved by, or independent of. The field lives on the recipe record.

**FR-P-21** — Every recipe that requires a durable has a crude variant requiring none, at a much worse input-to-output ratio (P-52). Iron can be worked with a stone, badly. This breaks the bootstrap circle with data rather than with a special case in code.

**FR-P-22** — Cohorts founding or resettling a settlement carry a basic endowment of durables (P-54).

**FR-P-23** — Durables wear with use at P-53 and must be replaced. This is the demand for durable goods, without which they are produced and never bought.

### Occupations

**FR-J-01** — A mestiere is a data record: the activity it performs (a recipe, or a service with no material output), building required, productivity per unit of labour, placement as settled or itinerant, wealth tendency, and social rank. Adding a mestiere requires no code change.

**FR-J-02** — Cohorts are partitioned by mestiere, under the key of FR-W-08.

**FR-J-03** — The share of population in each mestiere moves under the same damped allocation vector as land and labour. There is one allocation mechanism, not two.

**FR-J-04** — Caravanner is a service mestiere with no material output. Its population at a settlement supplies haulage capacity to the trade links leaving that settlement.

**FR-J-05** — Haulage capacity responds to wage like any other labour, and route danger raises the wage a caravanner requires. A dangerous route loses its haulers before it loses its link.

**FR-J-06** — Itinerant merchant is a service mestiere. Its function is to arbitrage price differences the standing trade graph does not serve, not to substitute for missing haulage on a served link.

**FR-J-07** — A merchant's carrying capacity derives from its wealth: on foot at the low end, an own cart in the middle, hired haulage at the top. Weight-to-value ratio then decides what it can afford to carry, so a poor merchant moves cloth and a rich one can move grain. There is no per-tier rule.

**FR-J-08** — A merchant journey is a sampled transient under FR-X-02. Destination and cargo are computed at departure as a pure function of the origin settlement's knowledge table and the hash. Merchants hold no state between journeys.

**FR-J-09** — A merchant hiring haulage consumes caravanner capacity at the origin, competing with standing trade flow for it.

**FR-J-10** — Merchant viability requires the shock return time Y of AC-09 to exceed typical travel time on the routes concerned. Below that, merchants always arrive after the opportunity has closed and the mestiere goes extinct through the allocation vector.

**FR-J-11** — Adventurer is a mestiere holding a cohort share, concentrated in settlements from which contracts and buried stock are reachable. Its income comes from contracts and from recovery of buried stock.

**FR-J-12** — Adventurer journeys are sampled transients under FR-X-02.

**FR-J-13** — An adventurer returning to a settlement deposits the prices it observed into that settlement's rumour table. The deposit is unpaid. Because it is unpaid, its distribution follows contracts and buried stock rather than existing trade, which is what carries knowledge to settlements no merchant has reason to visit.

**FR-J-14** — Buried stock is a finite quantity of currency and goods placed by the generator in ruins, barrows, and hoards. Recovery moves it into circulation. It is never created.

**FR-J-15** — Total buried stock at tick 0 is P-38 as a share of currency in circulation.

**FR-J-16** — Contracts are issued by settlements, kingdoms, and merchants and paid from their treasuries or wealth, for escort, bounty, and route clearing. Contract income is renewable; recovery income is not.

**FR-J-17** — The simulator models the adventurer's income, the danger a contract addresses, and the knowledge brought back. It does not model what happens on the journey. The host resolves that and reports the outcome through the command queue.

**FR-J-18** — An abandoned settlement leaves behind what its people could not carry. That becomes buried stock under FR-J-14, so the world keeps producing new hoards from its own failures after the ones placed at tick 0 are gone.

---

## 7. Money and income

**FR-C-01** — Currency is held by cohorts, by settlement treasuries, by kingdom treasuries, and by goods in transit. It is not held by markets.

**FR-C-02** — A cohort owns the output of the activity it performs. Settlement stock is the sum of cohort holdings, not a separate pool.

**FR-C-03** — Consumption order is fixed: a cohort meets its needs from its own holdings first, then buys the remainder at market.

**FR-C-04** — Each settlement update runs a clearing pass per good:

1. Cohorts consume from their own holdings.
2. Unmet need becomes demand, capped by the cohort's currency at the current price.
3. Holdings above the cohort's own needs become supply.
4. If supply is short, it is allocated in descending order of ability to pay.
5. Currency moves from buyers to sellers within the settlement.
6. Price is recomputed from the resulting settlement stock for the next update.

**FR-C-05** — Labour is traded like a good that cannot be stored: its price is set by the same bounded function of supply against demand, and it expires each update. Cohorts owning land or buildings hire labour from cohorts that own neither, and pay wages in currency.

**FR-C-06** — Currency is conserved. It enters only by an explicit minting event and leaves only by an explicit destruction event. Taxation, wages, purchases, and trade move currency, never create or destroy it.

**FR-C-07** — Settlements collect tax annually from cohort currency at the rate set by their kingdom, retain a share, and pass the rest to the kingdom treasury.

**FR-C-08** — Kingdoms spend from their treasury on garrisons, works, and holdings, returning currency to cohorts. Taxation without a spending path drains the economy and is prohibited.

**FR-C-09** — Tax pressure, defined as tax taken against surplus after needs, feeds the fragility metrics of FR-E-03.

**FR-C-10** — Inter-settlement trade moves currency opposite to goods. A merchant cohort in the destination settlement pays the origin price, sells at the destination price, and keeps the spread as income.

**FR-C-11** — Transport cost enters the flow decision as a threshold and is realised as a fraction of the goods lost in transit. It is never an unpaid currency term.

### Credit and debt

**FR-C-22** — A loan transfers currency that already exists. No instrument creates money. Debt is a claim, not currency, so the conservation invariant is untouched.

**FR-C-23** — Cohorts hold a debt stock and a credit stock, each carrying the counterparty's category rather than its identity. Agents hold explicit debt edges with amount, term, rate, and collateral.

**FR-C-24** — Borrowing capacity is bounded by collateral: goods, durables, buildings. A cohort with none cannot borrow. Credit therefore does not reach the poorest, and the rationing order of FR-C-04 stands: a starving cohort with nothing to pledge is not rescued by lending.

**FR-C-25** — Payment advanced against future output is a loan collateralised by that output. It uses the same mechanism with a different collateral field.

**FR-C-26** — The interest rate is the bounded price function applied to credit supply against credit demand, plus a risk premium derived from the debtor's collateral ratio. It is clamped like every other price (P-55).

**FR-C-27** — Default seizes, in order: goods, durables, buildings. It stops there. Residual debt is written off and the loss falls on the lender. No person is seized.

**FR-C-28** — A cohort or agent that has defaulted carries a mark for P-57 ticks, raising its risk premium.

**FR-C-29** — Land is not owned by cohorts. It remains a settlement attribute, and rent and tenancy are out of scope. This reopens when the wealth axis is added to the cohort key.

**FR-C-30** — Selling labour under FR-C-05 is always available as an alternative to borrowing, and is the only route open to a cohort with no collateral.

### Denominations

**FR-C-12** — Currency exists in four denominations at the fixed integer ratios in P-23. All accounting, all prices, and all invariants are expressed in the smallest unit.

**FR-C-13** — Cohorts hold currency as a single integer in smallest units. A cohort is a statistical aggregate whose coins change hands continuously, so denominations never block cohort-level clearing.

**FR-C-14** — Agents, the player, settlement treasuries, and kingdom treasuries hold an explicit count per denomination. These are single holders and can be blocked by what they carry.

**FR-C-15** — Conversion downward happens automatically as change during a purchase. Conversion upward requires a money changer, available only in settlements flagged for it, and costs the fee in P-24. The fee is income to a local cohort, never destroyed.

**FR-C-16** — Each settlement carries `changeCapacity`, the largest denomination its sellers can break. It is derived from the settlement's wealth distribution and updated at settlement cadence.

**FR-C-17** — A seller refuses a denomination above the settlement's `changeCapacity`. The buyer may instead overpay in that denomination and forfeit the difference. The overpayment path is always available, so no holder is ever unable to transact.

**FR-C-18** — Each denomination carries a unit weight (P-25). Weight applies to agents, to the player, and to convoys, never to cohorts.

**FR-C-19** — Tax remittance from a settlement treasury to a kingdom treasury travels as a convoy: an entity with a route over the trade graph, contents per denomination, total weight, departure and arrival ticks, and exposure to interception.

**FR-C-20** — Convoy contents count toward the world currency total while in transit. Interception transfers ownership. Only an explicit destruction event removes currency.

**FR-C-21** — Minting draws on a kingdom metal stock. A kingdom cannot mint without it.

---

## 8. Markets and trade

**FR-M-01** — Price is local and derived from stock:

```
price = reference_price × f(desired_stock / actual_stock)
```

`f` is monotonic and bounded on both sides. The bounds are part of the model.

**FR-M-02** — No global equilibrium is computed. Prices propagate by diffusion along trade links.

**FR-M-03** — Trade flow between linked settlements:

```
flow = k × max(0, price_dest − price_orig − transport_cost)
```

capped by available stock at origin and by haulage capacity on the link, with `k < 1`.

**FR-M-03a** — With no caravanners at the origin, outbound flow is zero. Trade is a service someone performs, not a property of the price difference.

**FR-M-04** — Trade graph topology, built once by the generator:

| Settlement type | Links |
|---|---|
| Village | 1 hub (nearest town or city by transport cost) + 0–2 neighbouring villages. Degree ≤ 3. |
| Town | 2–4 towns + 1–2 cities. Degree ≤ 6. |
| City | 3–6 cities, including cross-kingdom. Degree ≤ 6. |

**FR-M-05** — Goods leaving a settlement arrive somewhere or are explicitly lost to spoilage or banditry. Nothing evaporates.

**FR-M-06** — Events can sever a trade link (war, collapsed bridge, banditry).

**FR-M-08** — Merchant journeys accumulate profitability per origin-destination pair in a candidate list held by the origin settlement, capped at P-32 entries and decaying at P-35. Only pairs a merchant has actually travelled accumulate.

**FR-M-09** — A candidate crossing P-33 becomes a trade link. A link falling below P-34 dissolves. P-34 is strictly below P-33; the hysteresis is part of the model, without which links oscillate in and out as their own flow erases the differential that created them.

**FR-M-10** — The degree caps of FR-M-04 hold at all times. A new link at a saturated node evicts the weakest existing link by volume.

**FR-M-11** — The graph is rewired, never grown. Total edge count stays within the bound implied by the degree caps for the whole run.

**FR-M-12** — Each trade link carries a danger value. It raises the wage caravanners require, raises transit loss, and severs the link at the top of its range.

**FR-M-13** — Danger grows from displaced population: famine refugees, failed migration, soldiers disbanded at the end of a war. It falls through fulfilled contracts and through kingdom garrison spending.

**FR-M-14** — Poverty, banditry, and interrupted trade form a reinforcing loop. Its brakes are contract-funded suppression and garrison spending under FR-C-08. Neither is optional.

**FR-M-15** — A ruin raises danger on the links near it. Abandonment, banditry, and isolation reinforce one another; resettlement under FR-D-07 is the brake.

**FR-M-07** — Prices, stocks, and currency are integers in smallest units. Rounding behaviour is defined at every division.

---

## 9. Guilds and dungeons

**FR-K-01** — A guild hall is a settlement flag. It holds a bounded contract book, escrowed rewards, a rank distribution, and the goods it has bought.

**FR-K-02** — Anyone may post a contract: a publication fee (P-58) plus the reward deposited in escrow. Escrowed currency counts toward the world total and is covered by the conservation invariant.

**FR-K-03** — A contract carries a required rank derived from the difficulty of the condition behind it: link danger, dungeon depth, size of the threat.

**FR-K-04** — An adventurer of rank *r* may take contracts requiring rank *r+1* or below.

**FR-K-05** — Completing a contract at or above one's own rank accrues progress. Promotion moves counts between rank buckets in the hall's distribution.

**FR-K-06** — A contract whose required rank exceeds what the reachable halls can supply stays unfulfilled and its danger persists. Contracts are not an automatic solution to any crisis.

**FR-K-07** — The guild trades as a merchant cohort with a fixed margin (P-59): it buys loot at the local price less the margin and sells at the local price. It has no pricing system of its own.

**FR-K-08** — The guild buys equipment and consumables from local merchants and pays out rewards. This is the return path for the fees and margins it takes in; without it the halls become a currency sink.

**FR-K-09** — Fragility metrics generate contract proposals. Settlements, kingdoms, and merchants fund them from their treasuries and wealth.

**FR-K-10** — A guild hall's knowledge table is larger and refreshed more often than an ordinary settlement's, because petitioners bring the story behind the request.

**FR-K-11** — A dungeon is a site attribute carrying a regenerating stock of rare materials. Regeneration (P-60) is slower than the extraction an unconstrained adventurer population would apply.

**FR-K-12** — A dungeon is infested, cleared, or repopulating. Clearing is the outcome of a contract, and a cleared dungeon reverts after P-61 unless maintained.

**FR-K-13** — An infested dungeon raises danger on nearby links, as a ruin does.

**FR-K-14** — A cleared dungeon permits a mine under FR-P-19, so ordinary miners extract its material through the normal production path at ordinary risk.

**FR-K-15** — Recipes may require dungeon-only materials. Data validation at load must prove that no tier 0 or tier 1 good depends, directly or transitively, on such a material. A world that fails this check does not run.

**FR-K-16** — Recipes recovered from a dungeon enter that settlement's known recipe set, which is bounded per settlement state. Knowledge spreads from there like any other information.

---

## 10. Transient entities

**FR-X-01** — A transient entity is a caravan, convoy, traveller, patrol, or similar object that exists on a route rather than in a settlement.

**FR-X-02** — Sampled transients are derived from the aggregate flow that produces them. `Hash(worldSeed, subject, tick, channel, index)` under NFR-03 yields departure tick, contents, and speed, with the link as subject and the kind of transient as channel. The tick coordinate is the one at which the trajectory is generated, not the one being observed, which is what lets FR-X-03 hold. They carry no state.

**FR-X-03** — The derivation produces trajectories, not per-tick presence. An observer sees the same entity across consecutive ticks and can travel alongside it for its whole journey.

**FR-X-04** — Observing a transient writes nothing. It costs only the enumeration of entities in flight on the links in range.

**FR-X-05** — Interacting with a transient writes back to the aggregate that produced it, as a difference under FR-W-05. Robbing a caravan reduces the flow on that link.

**FR-X-06** — Scheduled transients are produced by an actual event rather than sampled from a flow. A tax convoy under FR-C-19 is one. They hold state and are authoritative.

**FR-X-07** — Sampled and scheduled transients share one entity type. They differ in origin, not in representation.

---

## 11. Events

**FR-E-01** — Three event classes, handled by separate mechanisms:

| Class | Trigger | Examples |
|---|---|---|
| Exogenous | authored frequency, target weighted by fragility | flood, drought, earthquake, frost |
| Endogenous | state threshold crossed | famine, revolt, insolvency, village abandonment, succession war, kingdom collapse |
| Contagious | compartmental spread along trade links | plague |

**FR-E-02** — Exogenous frequency is configurable per class. Target selection draws from a fragility-weighted distribution over eligible entities, using the indexed RNG on (seed, tick, event class).

**FR-E-03** — Entities maintain fragility metrics as part of their normal update: food stock below threshold, connectivity, legitimacy, tax pressure, defence state, social tension.

**FR-E-04** — Endogenous events have no configurable frequency. They have conditions. Thresholds and reaction times are the tuning surface.

**FR-E-05** — Contagion runs as integer compartments (susceptible / infected / removed) per settlement, transmitting along trade links in proportion to flow volume. Severing a link slows spread.

**FR-E-06** — Every event acts through the economy. A plague removes labour, which cuts production, which raises prices, which redirects trade, which drives migration. No event is a standalone counter.

**FR-E-07** — Every event records the event that triggered it, or records the player action, or records itself as an exogenous root.

---

## 12. Information

**FR-I-01** — Each event emits a chronicle entry: entities involved, cause, location, tick, importance.

**FR-I-02** — Chronicle entries cross at most one trade link per tick. The chance of crossing is a fixed floor (P-42) plus a term proportional to traffic on that link (P-43): caravans, merchants, migrants, adventurers. News rides on the people who move, and a link nobody travels carries only the floor.

**FR-I-03** — An entry degrades when it is retold, not when it travels. Each retelling rounds numbers, drops names, and simplifies causes. Distance alone leaves the account intact.

**FR-I-03a** — Migration flows carry entries in the direction people move, at zero retellings. Refugees from a famine are first-hand sources about it.

**FR-I-03b** — A settlement's own visible state is evidence in itself. Refugees arriving, stalls standing empty, caravans failing to come: none of these needs a chronicle entry to inform an observer.

**FR-I-04** — The chronicle is the single source for debugging causality, for UI, and for what characters can tell the player.

**FR-I-05** — Each settlement holds a bounded price knowledge table: accurate entries for its trade-link neighbours, refreshed at settlement cadence, and rumour entries for more distant settlements fed by chronicle propagation, each carrying its age.

**FR-I-06** — A knowledge entry carries two independent error terms:

| Term | Grows with | Meaning |
|---|---|---|
| Staleness | ticks elapsed since the event | the figure is exact for a moment that has passed |
| Fidelity | retellings | the figure itself is wrong |

Distance enters through travel time and therefore through staleness, never through fidelity. Both errors are deterministic functions, tuned by P-44 and P-45.

**FR-I-06a** — No economic decision consults a true remote price. A merchant acting on a stale but faithful figure can extrapolate it; one acting on a vague third-hand rumour cannot. Both can be wrong and lose money.

---

## 13. Player interaction

**FR-A-01** — The game pushes commands into a queue. The simulator applies them at a defined point in the tick. Nothing outside the core writes world state directly.

**FR-A-02** — The simulator emits a typed event stream. Nothing outside the core reads mutable world state.

**FR-A-03** — The game reads a read-only snapshot for rendering.

**FR-A-04** — Travel is walked by default and abstracted on fast travel. Both paths run the same simulation.

---

## 14. World generation

**FR-G-01** — The generator produces the initial state at tick 0: terrain, settlements, trade graph, kingdoms, initial populations and stocks.

**FR-G-01a** — The generator assigns each settlement a `climateIndex` varying continuously across the map, plus the land area available to it.

**FR-G-02** — The generator seed and all generation parameters are stored in the save file.

**FR-G-03** — World creation generates prior history by running the simulator itself, at full fidelity and with no hot agents, for P-18 years before tick 0. The resulting state is the initial state. No separate coarse rule set exists.

**FR-G-04** — If measured generation time exceeds the declared acceptable wait, the product ships pre-generated worlds and the player picks among their seeds rather than generating one.

---

## 15. Non-functional requirements

**NFR-01 — Determinism.** Identical seed plus identical command sequence produces an identical state hash at every tick, on any Windows x64 build.

**NFR-02 — Numeric representation.** All simulation state is integer or fixed-point. No floating point, no transcendental functions in the core. Floats exist only in the presentation layer.

**NFR-03 — Randomness.** Random values come from a stateless indexed hash over `(world_seed, entity_id, tick, channel, index)`. No sequential RNG stream in the core.

**NFR-04 — Iteration order.** No simulation logic iterates a hash-ordered collection. Order comes from integer ids.

**NFR-05 — Performance.** ≤ 10 ms per simulated day at p99, at target scale, after 60 simulated years, on one background thread *(estimate — validate in Phase 3)*. This implies roughly one simulated year per 3.6 seconds of wall clock.

**NFR-06 — Threading.** The core runs off the render thread. No `async`, no unordered parallelism inside the tick.

**NFR-07 — Memory.** Settlement state stays under 50 MB at target scale *(estimate)*.

**NFR-08 — Save format.** A save is a full state snapshot plus generation seed and parameters. Save format carries a version number and a migration path from day one.

**NFR-09 — Rule versioning.** State records which rule version produced it. On first load after a patch, all deferred entities materialise under the old rules, then continue under the new ones.

**NFR-10 — State layout.** Struct-of-arrays, indexed by `EntityId { int index; int generation; }`. No object references inside serialised state.

**NFR-11 — Platform.** Windows x64. The core compiles against the target runtime profile declared in section 17 and uses no language feature above the declared level.

**NFR-12 — Tooling.** A command-line runner executes N ticks headless, dumps state hashes, writes chronicle and metric series to CSV, and runs parameter sweeps.

---

## 16. Parameters to fix

| # | Parameter | Value | Status |
|---|---|---|---|
| P-01 | Budget per simulated day | 10 ms p99 | Decided, supported by measurement |
| P-02 | Settlement count, generator default | 2,000 | Decided |
| P-03 | Inhabited fraction of map | 15% | Decided |
| P-04 | Map extent | 1,000 km per side | Decided |
| P-05 | Hot agent cap | ~300 | Decided |
| P-06 | Trade degree caps | 3 / 6 / 6 | Decided |
| P-07 | Cadences | 1 / 7 / 28 / 364 | Decided |
| P-08 | Calendar | 364 days, 13 × 28, week of 7 | Decided |
| P-13 | Season length | 91 ticks, presentation only | Decided |
| P-14 | Climate curve resolution | 52 weekly entries, 2 curves | Decided |
| P-15 | Named climate bands | 3–4, presentation only | Decided |
| P-16 | Harvest offset, coldest to warmest | 28 days | Decided |
| P-17 | Settlement ceiling at P-01, 9 cohorts | ~7,000 | Measured subset, 2x allowance for unmeasured phases |
| P-17a | Settlement ceiling at P-01, 27 cohorts | ~1,500 | Measured subset, same allowance |
| P-63 | Cost per settlement update, 9 cohorts | 3.3 us | Measured, 5 of 12 phases |
| P-64 | Cost per settlement update, 27 cohorts | 15.8 us | Measured, 5 of 12 phases |
| P-18 | Prior history length | TBD in Phase 6 | Open |
| P-19 | Allocation adjustment rate | TBD by sweep | Open |
| P-20 | Base tax rate | 10% of surplus | Proposed |
| P-21 | Transit loss fraction | 2% per hop | Proposed |
| P-22 | Wage bounds | same as P-10 | Proposed |
| P-23 | Denomination ratios | 1 : 10 : 10 : 10 | Proposed |
| P-24 | Money changer fee, upward conversion | 2% | Proposed |
| P-25 | Coin unit weights | TBD | Open |
| P-26 | `changeCapacity` thresholds | TBD by sweep | Open |
| P-27 | Haulage capacity per caravanner | TBD | Open |
| P-28 | Caravan size and departure spacing | derived from link flow | Decided |
| P-29 | Route danger to caravanner wage coupling | TBD by sweep | Open |
| P-30 | Wealth bands, shipping configuration | 1 | Decided |
| P-31 | Wealth bands, CI configuration | 3 | Decided |
| P-32 | Candidate routes per settlement | 4 | Proposed |
| P-33 | Link creation threshold | TBD by sweep | Open |
| P-34 | Link dissolution threshold | TBD by sweep, below P-33 | Open |
| P-35 | Candidate decay rate | TBD by sweep | Open |
| P-36 | Neighbour price knowledge error | small, non-zero | Proposed |
| P-37 | Rumour error growth per hop and per week | TBD by sweep | Open |
| P-38 | Buried stock at tick 0, share of circulating currency | 30% | Proposed |
| P-39 | Recovery rate cap per adventurer | TBD by sweep | Open |
| P-40 | Danger growth per unit of displaced population | TBD by sweep | Open |
| P-41 | Danger decay from suppression spending | TBD by sweep | Open |
| P-42 | News propagation floor per link | TBD by sweep | Open |
| P-43 | News propagation per unit of link traffic | TBD by sweep | Open |
| P-44 | Staleness error per week elapsed | TBD by sweep | Open |
| P-45 | Fidelity error per retelling | TBD by sweep | Open |
| P-46 | Attractiveness hysteresis band | TBD by sweep | Open |
| P-47 | Minimum ticks before reversing migration role | TBD by sweep | Open |
| P-48 | Land per capita clamp | TBD | Open |
| P-49 | Perceptible population change over a decade | 25% | Proposed |
| P-50 | Crowding mortality coefficient | TBD by sweep | Open |
| P-51 | Building decay rate | TBD by sweep | Open |
| P-52 | Crude recipe yield ratio | TBD by sweep | Open |
| P-53 | Durable wear rate per unit of output | TBD by sweep | Open |
| P-54 | Settlement founding endowment | TBD | Open |
| P-55 | Interest rate bounds | TBD by sweep | Open |
| P-56 | Collateral ratio required to borrow | TBD by sweep | Open |
| P-57 | Default mark duration | TBD | Open |
| P-58 | Contract publication fee | TBD by sweep | Open |
| P-59 | Guild trading margin | TBD by sweep | Open |
| P-60 | Dungeon regeneration rate | TBD by sweep | Open |
| P-61 | Time before a cleared dungeon reverts | TBD by sweep | Open |
| P-62 | Adventurer rank count | 4 | Proposed |
| P-09 | Goods count | 7 | Decided |
| P-10 | Price bounds | 0.25× to 4× reference | Decided |
| P-11 | Trade damping `k` | TBD by sweep | Open |
| P-12 | Settlement mix | 8 cities / 75 towns / 1,917 villages at 2,000 total | Decided |

---

## 17. Acceptance criteria

Every criterion below runs headless in CI, driven by the command-line harness alone.

**AC-01 — Conservation.** Population, currency, and goods invariants hold at every tick of every run.

**AC-02 — Determinism.** Same seed and commands produce the same state hash sequence across builds and machines.

**AC-03 — Save round trip.** save → load → save produces identical bytes.

**AC-04 — Stability.** Across M seeds × 60 simulated years, no monitored variable leaves its declared range and none grows without bound. Monitored variables:

| Variable | Scope |
|---|---|
| Total population | world |
| Population | per settlement |
| Settlements below viability threshold | count |
| Currency in circulation | world |
| Price per good | median, min, max across settlements |
| Food stock per capita | 10th percentile |
| Migration flow per tick | as a fraction of population |
| Urban share of population | world |
| Kingdom legitimacy | minimum |
| Infection prevalence | maximum |

Each carries a hard bound that fails the test and a soft band that raises a warning. Values are set in Phase 6.

**AC-05 — Wake equivalence.** An entity simulated continuously for N ticks and the same entity deferred and resumed at tick N reach the same state within declared tolerance.

**AC-06 — Cross-level consistency.** Materialising a level of detail and re-aggregating it returns the parent level's values within declared tolerance.

**AC-07 — Scale independence.** ms/tick at 2,000 settlements and at 20,000 settlements differ only by the staggered-bucket workload, not by a hidden full scan.

**AC-08 — Long-run degradation.** ms/tick and save size measured at 1, 10, and 40 simulated years follow a non-superlinear curve.

**AC-09 — Shock response.** Removing 30% of a region's stock of a good moves its local price within X ticks and returns it to band within Y ticks. X and Y are measured in Phase 6 against this intent:

| Role | X, first price move | Y, return to band |
|---|---|---|
| Subsistence | short, within two settlement updates | long, up to the next harvest |
| Input | medium | within the season |
| Durable | long | long |
| Luxury | medium, bounded by travel | short |

**AC-10 — No inert goods.** Over a long run, every good reaches scarcity somewhere at least once.

**AC-11 — Causal chains.** Every high-importance chronicle entry traces back to an exogenous root or a player action.

**AC-12 — Configuration sensitivity.** Doubling catastrophe frequency degrades world metrics monotonically.

**AC-13 — Intrinsic dynamics.** With catastrophes disabled, settlements still grow and decline.

**AC-14 — News reach.** Over 60 simulated years, the fraction of high-importance events reaching the player's position falls inside a declared band.

**AC-15 — No player-position dependence.** Two runs with identical seed and identical commands, differing only in where the player stood, produce identical world state outside the player's interaction radius.

**AC-16 — Spatial continuity.** Annual output and harvest date of neighbouring settlements vary smoothly with distance. No step appears at a climate band boundary.

**AC-17 — Annual price cycle.** Grain price at any settlement shows a yearly period: a trough after its harvest and a peak before the next one.

**AC-18 — Harvest wave.** Across a full map, harvest dates span the range declared in P-16, and grain flows from earlier-harvesting settlements toward later ones during that window.

**AC-19 — Host-free build.** The core assembly compiles and its tests pass from the standalone project, with no editor and no game client, on every commit.

**AC-20 — Currency conservation.** Cohort holdings plus treasuries plus currency in transit equal the world total at every tick, changing only through explicit minting or destruction events.

**AC-21 — No monetary drain.** Over 60 simulated years with no minting, the share of currency held in treasuries stays inside a declared band.

**AC-22 — Damped allocation.** After a one-off price shock, the amplitude of the allocation response decays year over year rather than growing.

**AC-23 — Rationing bites the buyers.** Under a food shortage, cohorts that buy their food suffer before cohorts that grow it.

**AC-24 — No transaction deadlock.** No holder, at any wealth composition and in any settlement, is unable to complete a purchase it can afford in total value.

**AC-25 — Denomination accounting.** The sum of every holder's coins converted to smallest units, plus convoy contents, equals the world currency total.

**AC-26 — Trajectory consistency.** An observer following a sampled transient sees continuous motion across ticks, and observing the same link and tick twice yields identical entities.

**AC-27 — Interaction back-action.** Robbing or destroying a sampled transient reduces the aggregate flow on its link by the corresponding amount.

**AC-28 — Cohort key generality.** The full suite passes under the three-band CI configuration.

**AC-29 — Haulage constraint.** Removing caravanners from a settlement reduces its outbound trade flow, and removing all of them stops it.

**AC-30 — Edge bound.** Total trade links never exceed the bound implied by the degree caps, at any tick of any run.

**AC-31 — Graph convergence.** Over 60 simulated years the rate of link creation and dissolution declines into a band, and no link enters and leaves more than a declared number of times.

**AC-32 — Merchant survival.** Under default parameters the itinerant merchant share stays above zero in every kingdom across 60 simulated years.

**AC-33 — Bounded knowledge.** No economic decision reads a true remote price. Every one reads the knowledge table.

**AC-34 — Adventurers outlive the hoards.** After buried stock is exhausted, the adventurer share stays above zero, sustained by contracts.

**AC-35 — Buried stock conservation.** Buried stock plus circulating currency is constant absent explicit minting. Recovery moves value and never creates it.

**AC-36 — Bounded banditry.** Danger does not grow without bound on any link in any run, and no region reaches permanent isolation without an event that caused it.

**AC-37 — Discovery reach.** Over 60 simulated years, settlements with no trade link into a given region still appear in that region's rumour tables.

**AC-38 — Spatial price gradient.** The price of a good differs between settlements by an amount rising with distance along the trade graph, above a declared floor at one hop and within a declared ceiling across a kingdom.

**AC-39 — Hub volatility.** Price variance at the highest-degree settlements stays above a declared floor across 60 simulated years. Damping that stabilises the world must not flatten its cities.

**AC-40 — Every death is reconstructible.** Every settlement that empties leaves a chronicle chain explaining why, readable from the log. Whether anyone in the world learns of it is a separate matter and is not required: a village killed by bandits may go unknown for years, and a player finding it later is intended.

**AC-41 — First-hand accounts survive distance.** An entry carried by a migrant or a traveller from the affected settlement arrives with fidelity error at the minimum, regardless of distance covered.

**AC-42 — Silence follows severance.** Cutting every link into a region reduces news crossing into it to the floor rate.

**AC-43 — Abandonment reverses.** Over 60 simulated years the count of empty settlements fluctuates rather than rising monotonically, and settlements that emptied are observed to be resettled.

**AC-44 — Stale entries persist until corrected.** A knowledge entry about a settlement that has emptied continues to age rather than being silently removed, so a merchant eventually travels there and becomes the first-hand source that reports it.

**AC-45 — Migration does not flicker.** No settlement reverses between receiving and sending more than a declared number of times per decade, and no settlement's population oscillates around a mean with a period under P-47.

**AC-46 — Change is perceptible.** Across 60 simulated years, a declared fraction of settlements change population by at least P-49 over some decade. A world where everything holds steady fails this as surely as one where everything collapses.

**AC-47 — Cities stop growing.** Urban share of population stabilises rather than rising monotonically across 60 simulated years, and no settlement holds a population above what its inbound food capacity supports for more than a declared number of ticks.

**AC-48 — Cities are demographic sinks.** In settlements above a declared density, deaths exceed births, and their growth comes from immigration.

**AC-49 — No bootstrap deadlock.** A settlement stripped of every durable and cut off from trade recovers production over time through crude recipes.

**AC-50 — Settlements stay different.** Output mix differs across settlements throughout a 60-year run, and the differences persist rather than converging. Convergence means comparative advantage has been lost and trade will follow it.

**AC-51 — Credit creates no money.** Total currency is unchanged by any loan, repayment, default, or write-off.

**AC-52 — The poorest cannot borrow.** Cohorts below the collateral threshold hold no debt, and a food shortage still bites them first.

**AC-53 — Concentration is bounded.** Across 60 simulated years the share of currency held by the wealthiest cohorts stays within a declared band rather than rising monotonically.

**AC-54 — Escrow is counted.** Rewards held by guild halls are part of the world currency total at every tick.

**AC-55 — Hard problems stay hard.** A region whose halls hold only low ranks leaves high-rank contracts unfulfilled, and the danger behind them persists.

**AC-56 — No subsistence depends on dungeons.** Static validation proves no tier 0 or tier 1 good depends transitively on a dungeon-only material. The check runs at load, not in a test.

**AC-57 — Guilds do not hoard.** The share of currency held by guild halls stays within a declared band across 60 simulated years.

**AC-58 — No floating point in the core.** An inspection of the compiled core assembly finds no float, double, or decimal in any field, property, parameter, return type, or local slot.


---

## 18. Delivery phases

Each phase has an exit gate. No phase starts before the previous gate is green.

| Phase | Content | Exit gate |
|---|---|---|
| 0 | Observables brief, budget numbers, invariants list | Written and frozen |
| 1 | Conceptual model, causal loop diagram | No flow without source and sink; every reinforcing loop has a named balancing loop |
| 2 | State representation, conservation invariants | Assert list executable |
| 3 | Headless kernel: scheduler, RNG, serialisation, chronicle, CLI runner | 100k empty ticks; AC-02, AC-03 green |
| 4 | Subsystems in dependency order: demography → production → market → labour and income → needs → mobility → events | AC-01, AC-04 green per subsystem |
| 5 | Agent promotion and persistence by difference | AC-05, AC-06 green |
| 6 | Tuning: sweeps, CSV output, charts | P-11, P-18, AC-04 bounds and AC-09 values recorded |
| 7 | Host integration | Command queue and snapshot API only |

---

## 19. Open items

All remaining items depend on measurement and close in the phase named.

| Item | Closes in | Procedure |
|---|---|---|
| P-11, trade damping `k` | Phase 6 | Sweep `k` against transport cost; accept on AC-09 and AC-04 |
| P-17, settlement ceiling | Phase 3 | Measure per-settlement update cost, divide the budget by it |
| P-18, prior history length | Phase 6 | Measure cost per simulated year, set against the acceptable wait |
| AC-04 bounds | Phase 6 | Long runs across seeds, bounds set from observed envelopes |
| AC-09 X and Y per good | Phase 6 | Shock injection per good, values recorded against the intent table |
| P-01 validity | Phase 3 | Confirm the 5 µs and 10 µs per-entity estimates the budget rests on |

---

## 20. Downstream constraints

The core knows nothing about its consumers. This section records what the consumers require of it, so that no requirement above has to name a host.

| Constraint | Value | Status |
|---|---|---|
| Target framework | `netstandard2.1` | Proposed |
| Maximum C# language level | 9 | Proposed |
| Ahead-of-time compilation must be supported | Yes | Proposed |
| Allowed base class library surface | `netstandard2.1` only, no third-party packages | Proposed |
| Integration form | Source import into the consumer project | Decided |

Rules that follow from this section regardless of the values chosen:

- No reflection-based serialisation, no reflection emit, no runtime code generation, so that a consumer compiling ahead of time can link the core unchanged.
- No dependency outside the base class library.
- The simulation assembly and the harness are separate projects; the harness may use anything, the core may not.
- Core sources live in one folder that is the single source of truth. The consumer project and the harness project each compile that folder in place. Neither holds a copy.
- The core compiles as its own assembly with automatic host references disabled, so a call into the host fails to compile instead of succeeding inside the editor.
- CI builds the core from the standalone project and runs its tests on every commit. This is what catches a host dependency introduced from inside an editor.
- The consumer project's API compatibility level is set to the target framework declared above.

Why these values: `netstandard2.1` is consumable both by the current runtime generation and by the .NET 10 generation that follows it, so the choice does not depend on which one the project ships against. C# 9 is the level available when the core is compiled by the consumer rather than shipped as a prebuilt assembly, and nothing above it is needed for integer arithmetic over parallel arrays.

Confirm before Phase 3. A wrong guess costs a rewrite of the serialiser.