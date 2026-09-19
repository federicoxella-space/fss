# Socio-Economic Simulator — Design Decisions

**Document:** SIM-DEC
**Status:** Draft 1
**Revision:** 2026-09-19
**Companion:** SIM-REQ

---

## How to read and change this document

This document describes the design as it stands. It carries no history of its own revisions. When a decision changes, edit it in place; version control holds what it said before.

Each entry states the decision, why it holds, and what it costs. The cost section matters as much as the rationale: it tells you what you are buying and what you are paying, so that anyone reopening a decision knows which price they are proposing to stop paying.

Figures marked *(estimate)* need prototype measurement.

---

## Core representation

### DEC-001 — All simulation state is integer or fixed-point

Currency in smallest units, goods in whole units, normalised quantities on a 0–10000 integer scale, rates in fixed point over `long`. Floating point exists in the presentation layer only.

**Rationale.** Bit-exact reproducibility across builds requires numeric operations whose results do not depend on the runtime, the compiler, or the math library. Integers give that with no analysis. The domain also fits: coins, sacks of grain, and people are countable.

**Cost.** Every division needs an explicit rounding rule. Curves that a designer would express as a smooth function have to be written as fixed-point approximations or lookup tables.

### DEC-002 — Randomness comes from a stateless indexed hash

Random values are computed as `Hash(world_seed, entity_id, tick, channel, index)`. No sequential RNG stream exists in the core.

**Rationale.** Three properties fall out at once. Adding a subsystem does not desynchronise the others, because each holds its own channel. Any entity's random draw at any past tick can be recomputed directly, which is what makes deferred agents resumable in constant time. Nothing about the RNG needs serialising.

**Cost.** Algorithms consuming a variable number of draws, such as rejection sampling, need rewriting to a fixed draw count.

### DEC-003 — State lives in struct-of-arrays indexed by generational handles

`EntityId` carries an index and a generation counter. Serialised state holds no object references.

**Rationale.** State serialises, copies, hashes, and compares in bulk, which is what AC-02 and AC-03 need. Generation counters catch stale handles after an entity dies, which happens constantly in a world with generational turnover.

**Cost.** Code reads less like domain objects. Adding a field to an entity touches an array declaration and the serialiser.

### DEC-004 — Each quantity has one owning level

Population, currency, and goods each belong to exactly one simulation level. Levels below sum to it. Conservation invariants assert this every tick.

**Rationale.** A world represented at several resolutions holds several copies of the same facts. Naming one copy as the authority is what keeps them from drifting apart over decades of world time.

**Cost.** Producing a number sometimes means aggregating from the owning level rather than reading it where it feels natural.

---

## Time

### DEC-005 — Time is an integer tick equal to one world day

The simulator reads no system clock and no frame delta. The caller decides how many ticks to run.

**Rationale.** The core runs identically under the game, under the test harness, and under fast travel. Reproducibility comes from the tick counter being the only clock.

**Cost.** Anything the game wants to happen at a sub-day granularity lives outside the simulation core.

### DEC-006 — The year is 364 ticks: 13 months of 28 days, 52 weeks of 7

Cadence ratios are integers: 7 × 4 = 28, 28 × 13 = 364.

**Rationale.** Integer ratios let levels align on shared tick boundaries, make staggered updates exact, and make wake-equivalence testing unambiguous. A 52-week year also fixes every calendar date to the same weekday in every year, so market days, fairs, and festivals become regularities a player can learn and exploit. The setting is fantasy, so the calendar is free to be convenient.

**Cost.** Thirteen is prime, so a season is 3.25 months and no level can run on a seasonal cadence (DEC-007). Players comparing world dates to real-world intuition find the year slightly short.

### DEC-006a — New Year's Day consumes no tick

The festival sits between the last tick of one year and the first tick of the next. In the setting it is the day outside time, on which the world stands still.

**Rationale.** A 365th tick would leave the year coprime with 7, 13, and 28, so every cycle running on tick arithmetic would drift one day per year against the seasons and put sowing two months out of place within a lifetime. Keeping the festival outside the tick stream preserves exact alignment forever with no special case in the scheduler.

**Cost.** Nothing happens on New Year's Day, so the game cannot stage simulated events during it.

### DEC-006b — Seasons name the year, they do not drive it

A season is 91 ticks, four per year, anchored so that season 1 opens the year as spring. Seasons appear in UI, dialogue, and festivals.

**Rationale.** Sowing and harvest are calendar events rather than periodic updates, and climate acts through a weekly curve (DEC-036), so seasons keep their narrative role without any level running at 91 ticks.

**Cost.** Any subsystem that wants a true quarterly rhythm has to implement it against dates itself.

### DEC-007 — Cadence belongs to the simulation level, not to player proximity

Agents update daily, settlements weekly, basins monthly, kingdoms annually, everywhere on the map, at all times.

**Rationale.** With non-linear feedback, changing the step size changes the outcome. Tying cadence to the player's position would make world state depend on where the player walked, which breaks determinism testing and lets players notice that watched places behave differently.

**Cost.** The simulator pays for settlements the player will never see. The staggered-bucket scheme (DEC-008) is what keeps that affordable.

### DEC-007a — Political and military change is event-driven

War declaration, succession, rebellion, and alliance fire on endogenous thresholds (DEC-023), not on the kingdom update.

**Rationale.** The kingdom cadence carries taxation, budget, and holdings, which are annual by nature. Politics moving once a year would read as an inert world, so it fires when legitimacy, treasury, or border pressure crosses a threshold, which is also what lets the player influence it.

**Cost.** Kingdom-level tension metrics have to be maintained at settlement and basin cadence rather than being computed once a year.

### DEC-008 — Settlements update in staggered buckets keyed by id

On tick *d*, settlements with `id % 7 == d % 7` update. The bucket comes from the id, never from iteration order.

**Rationale.** Per-tick cost stays flat instead of spiking every sixth day, and the assignment stays deterministic regardless of how the collection is traversed.

**Cost.** Two settlements can be up to six days out of phase with each other. Interactions between them read a neighbour's state that is up to six days stale.

---

## Resolution and agents

### DEC-009 — Every settlement is simulated explicitly, forever

No settlement is generated on demand at runtime. The generator produces them all at tick 0.

**Rationale.** At the target scale a settlement update costs about 5 µs and a settlement record about 350 bytes *(estimate)*, so a few thousand settlements cost single-digit milliseconds per day and single-digit megabytes. Paying that removes an entire class of consistency problems between generated and simulated content. A player who walks for decades visits thousands of settlements, so lazy generation would be optimising for a case that does not occur.

**Cost.** Settlement count becomes a budget parameter with a hard ceiling *(estimate: 10,000–12,000)*, rather than an unbounded world size.

### DEC-010 — Settlement count derives from travel pace, not from historical density

The target is how often a walking player meets a settlement. At 25 km per day, one settlement every 10 km gives roughly 2.5 encounters per day of travel.

**Rationale.** The number that shapes play is encounter frequency. Wilderness between settlements is the intended texture of the setting, so density is a design lever rather than a realism constraint.

**Cost.** Cartographic density will not match any historical reference, and anyone checking it against real medieval Europe will find it sparse.

### DEC-011 — Agents are created from cohort statistics on player approach and subtracted from their cohort

A sampling function produces a complete individual from `(seed, settlement, index)` as a pure function, consulting nothing else.

**Rationale.** Purity gives the same person on the first and second visit without storing anything. Subtracting from the cohort keeps AC-01 conservation true, so aggregate statistics never double-count a promoted individual.

**Cost.** Every attribute an agent needs at creation must be derivable from cohort statistics plus the hash. Attributes that depend on history have to come from the difference record (DEC-012).

### DEC-012 — Agents and settlements persist as differences from deterministic rules

An entity stores only the divergence caused by player action. No difference means no stored state.

**Rationale.** Stored state then grows with meaningful player actions rather than with kilometres walked, which is what keeps a 60-year walked campaign from accumulating thousands of individual records. A village crossed once without incident leaves nothing behind.

**Cost.** Defining what counts as a meaningful action becomes a design decision per interaction type, and getting it wrong either loses player-visible consequences or leaks state.

### DEC-013 — Deferred entities resume by closed-form evaluation

An entity's state at tick T is computed directly, not by replaying T ticks. Attributes are classified at design time as summable or not; non-summable attributes keep their entity hot.

**Rationale.** Constant-time resumption is what makes decades of world time affordable with hundreds of hot agents and thousands of settlements.

**Cost.** The summable/non-summable classification constrains attribute design from the start. Social relations and local market feedback fall on the non-summable side.

### DEC-014 — Hot agents are capped at roughly 300

**Rationale.** The cap comes from the per-day budget. An agent the player has never interacted with is indistinguishable from one never created, so the cap costs nothing the player can perceive.

**Cost.** Systems wanting many simultaneous named characters, such as a large battle, need their own representation rather than hot agents.

---

## Economy

### DEC-015 — A good is a data record and a recipe is a data record

Unit weight, reference value, perishability, role, need tier. Recipes carry inputs, output, labour, building. No good and no recipe requires code.

**Rationale.** The plan is to start with seven goods and grow. That only works if growth costs data entry rather than branches.

**Cost.** Goods with genuinely unique mechanics need a general mechanism added to the schema rather than a special case, which is more work up front for the first such good.

### DEC-016 — Goods are differentiated by weight-to-value ratio

Heavy cheap goods stay inside their basin. Light valuable goods cross kingdoms.

**Rationale.** Spatial behaviour emerges from transport cost in the flow formula, so one number per good produces the whole difference between a local grain famine and a continent-wide cloth market.

**Cost.** That number carries a lot of tuning weight; getting it wrong for one good distorts that good's entire geography.

### DEC-017 — Price is a bounded local function of stock

```
price = reference_price × f(desired_stock / actual_stock)
```

`f` is monotonic and clamped, currently proposed at 0.25× to 4× reference.

**Rationale.** Local derivation costs nothing to compute and cannot oscillate between distant nodes. The clamp is part of the model rather than a safety net: it puts a mechanical ceiling on runaway prices, which is what AC-04 tests for.

**Cost.** A settlement in genuine collapse cannot express that through price alone, because price stops at the clamp. Famine severity has to reach the player through other signals.

### DEC-018 — Trade flows on price differential minus transport cost, damped

```
flow = k × max(0, price_dest − price_orig − transport_cost)
```

capped by origin stock, with `k < 1`.

**Rationale.** The formula is a balancing loop by construction: goods move toward the high price, the high price falls, the flow shrinks. Damping under 1 keeps it from overshooting.

**Cost.** Prices equalise slowly and with lag. A distant famine reaches the player's market as a gradual rise over weeks, which is the intended reading but rules out sharp market shocks at distance.

### DEC-019 — The trade graph has capped degree and is rewired, not grown

Village degree ≤ 3, town and city degree ≤ 6. The generator builds the initial graph. Merchant activity promotes candidate routes into links and lets unprofitable links dissolve, with a creation threshold strictly above the dissolution threshold. A new link at a saturated node evicts the weakest one.

**Rationale.** Capped degree keeps edge count linear in settlements, so trade cost scales with the world instead of squaring with it, and the hierarchy produces basins as a derived structure. Rewiring under that cap lets commerce follow where value actually moves without letting the cost grow over a campaign. Hysteresis is what stops a link from erasing, through its own flow, the differential that justified it and oscillating.

**Cost.** The graph becomes mutable state that worldgen no longer solely owns, so it must serialise and its churn has to be tested for convergence over decades.

### DEC-020 — No global equilibrium is computed

**Rationale.** Diffusion along the graph gives propagation delay, which is both cheaper and closer to how a medieval economy behaves. Distance means something.

**Cost.** Prices never reach a true equilibrium, so any design that assumes an arbitrage-free market has to be reconsidered.

---

## Events

### DEC-021 — Events split into exogenous, endogenous, and contagious

Each class has its own trigger mechanism.

**Rationale.** The three kinds answer different design needs. Exogenous events supply noise so the world never flattens. Endogenous events supply consequence. Contagion needs a propagation model rather than a modifier.

**Cost.** Three mechanisms to build, tune, and test rather than one event system.

### DEC-022 — Exogenous frequency is authored, exogenous targets are drawn from fragility

Entities maintain fragility metrics as part of their normal update. Target selection draws with probability proportional to fragility, using the indexed RNG on `(seed, tick, event class)`.

**Rationale.** This gives authored pacing and a valid cause at the same time. Famine strikes where granaries were already empty, revolt where taxes were already high, plague where trade was already busy. The reason is legitimate because the state already contained it before the event fired.

**Cost.** Fragility metrics have to be maintained even in the vast majority of entities that never suffer an event.

### DEC-023 — Endogenous events have conditions, not frequencies

A village empties because its attractiveness stayed negative for twenty ticks. A kingdom falls because legitimacy went below zero with an empty treasury.

**Rationale.** These events are consequences, and giving them a scheduler would sever them from what the player can influence.

**Cost.** They cannot be paced. If the thresholds are wrong the world produces either nothing for decades or a cascade, and finding the right thresholds is Phase 6 work.

### DEC-024 — Contagion runs as integer compartments over the trade graph

Susceptible, infected, and removed per settlement, transmitting in proportion to trade flow.

**Rationale.** Spread follows commerce, reaches cities first, and slows when routes close. Trade graph and contagion graph being the same structure turns quarantine into a real decision with a real cost.

**Cost.** Three more integer stocks per settlement and a tuning surface of its own.

### DEC-025 — Every event acts through the economy

A plague removes labour, which cuts production, which raises prices, which redirects trade, which drives migration.

**Rationale.** A player who sees only a counter fall has not seen a plague. Routing effects through existing flows produces the second-order consequences that make an event readable.

**Cost.** No event can be added in isolation. Each one needs its entry point into the economic model.

---

## Information

### DEC-026 — Events emit chronicle entries carrying their cause

Each entry names entities, location, tick, importance, and the event that triggered it.

**Rationale.** One structure serves debugging, causal-chain testing (AC-11), UI, and what a character can tell the player. The cause pointer is what turns a log into a chain.

**Cost.** Every state mutation worth explaining has to emit an entry, which is why the chronicle goes in during Phase 3 rather than later.

### DEC-027 — Chronicle entries propagate one hop per tick and degrade

Transmission probability rises with importance and falls with distance travelled. Numbers round, names drop, causes simplify.

**Rationale.** News arriving late and distorted is both correct for the setting and the mechanism that makes distant events observable at all. The player feels the cloth price rise, then hears weeks later about the war that closed the route.

**Cost.** The player's picture of the world is unreliable by design, so any UI presenting it as fact will mislead.

---

## Demography

### DEC-028 — Demography is a driver, not a backdrop

Births, deaths, and migration all run per settlement on integers, weekly.

**Rationale.** Settlements growing and declining is one of the observables the player is meant to notice and cause.

**Cost.** Three coupled flows to tune, and the migration loop needs the brake in DEC-029.

### DEC-029 — Land per capita brakes the migration loop

Fewer people means more land each, which raises output per worker, which raises attractiveness.

**Rationale.** Migration on its own is a reinforcing loop: a shrinking village produces less, attracts less, and shrinks further. Land per capita is the balancing term, and it is also what actually held medieval populations in place.

**Cost.** Land becomes a required per-settlement quantity, and the balance between it and the migration rate is delicate enough to need a parameter sweep.

---

## Integration and operations

### DEC-030 — Commands in, events out, snapshot for reading

The game pushes commands into a queue applied at a defined point in the tick, consumes a typed event stream, and renders from a read-only snapshot.

**Rationale.** One entry point for mutation makes determinism testable and makes the command log a replay format for debugging.

**Cost.** Anything the game wants to do immediately waits for the next tick boundary.

### DEC-031 — The core runs on a background thread

**Rationale.** The per-day budget is then independent of the frame budget, which is what allows fast travel at roughly one simulated year per 3.6 seconds *(estimate)*.

**Cost.** Snapshot handover needs a defined publication point, and the render layer always shows state that is at least one tick old.

### DEC-032 — A save is a full state snapshot plus seed and generation parameters

Format carries a version number and a migration path.

**Rationale.** Load time stays constant instead of growing with campaign length. The seed and parameters travel with the save so any player-reported bug reproduces exactly.

**Cost.** Save files are large and get written often.

### DEC-033 — State records its rule version, and a patch materialises deferred entities

On first load after an update, deferred entities resume under the rules that produced them, then continue under the new ones.

**Rationale.** Closed-form resumption means a deferred entity's past is recomputed rather than stored, so changing a formula would silently rewrite history the player already lived through. Materialising at the patch boundary freezes that history instead.

**Cost.** One slow load per patch, and the old evaluation path stays in the code until every save has migrated.

### DEC-034 — The core targets Windows x64 and knows nothing about its host

No engine, rendering, input, or asset API. No `async` in the tick, no unordered parallelism. No dependency outside the base class library.

**Rationale.** A single platform makes bit-exact determinism a matter of internal discipline. Host independence lets the CLI harness, the CI suite, and the parameter sweeps run with no editor and no game client, and it keeps the core portable to whatever consumes it.

**Cost.** Utilities a game engine would supply get written by hand, starting with math, RNG, and serialisation.

### DEC-034a — Consumer runtime constraints live in one place

The core is written against a declared runtime profile and language level, recorded in SIM-REQ section 17. No other requirement or decision names a consumer.

**Rationale.** The language level and runtime surface are set by whoever links the core, not by the core itself. Recording them once keeps that constraint enforceable without letting a specific consumer leak into the design.

**Cost.** Section 17 has to be revisited whenever a new consumer with a narrower profile appears, and the core may then have to drop a language feature it already uses.

### DEC-035 — A CLI harness exists from Phase 3

Runs N ticks headless, dumps state hashes, writes chronicle and metric series to CSV, runs parameter sweeps.

**Rationale.** Every acceptance criterion in SIM-REQ runs through it, and Phase 6 tuning is not feasible without sweeps.

**Cost.** A second entry point to maintain alongside the game integration.

---

## Climate and agriculture

### DEC-036 — Climate is a continuous per-settlement index; bands are labels

The generator assigns each settlement a fixed-point `climateIndex` on 0–10000. Named bands group ranges of it for presentation. No simulation rule reads a band.

**Rationale.** A continuous index keeps neighbouring settlements behaving like neighbours, so no seam appears on the map where two villages a few kilometres apart harvest weeks apart. It also keeps the simulator ignorant of how the generator decided climate, which lets worldgen change its terrain model without touching simulation code.

**Cost.** Designers cannot author a rule that applies to "the northern band". Anything band-specific has to be expressed as a function of the index.

### DEC-037 — Climate acts through two 52-entry weekly curves, interpolated

Authored `cold` and `warm` curves give a weekly modifier each; a settlement interpolates between them on its `climateIndex` in integer arithmetic. The modifiers govern travel cost, mortality, livestock growth, timber yield, and construction labour.

**Rationale.** Weekly resolution turns climate into a ramp rather than four jumps a year, so prices drift instead of stepping. Two curves plus interpolation cover the whole map in a few kilobytes and stay fully authorable without code.

**Cost.** Tuning happens across 104 authored numbers rather than a handful of constants, which needs a curve editor or a spreadsheet import before Phase 6.

### DEC-038 — Crops are dated events, not continuous production

A crop record carries sowing date, maturation length, harvest date, and yield per land and labour. Harvest delivers the full amount into settlement stock at once. Per-settlement date offsets derive from `climateIndex`. A dated event fires at the first settlement update at or after its date, so it can lag by up to six ticks.

**Rationale.** The annual price cycle, the irreversibility of a bad harvest, the value of storage, and the autumn grain wave across latitudes all fall out of this one choice with no dedicated code. A settlement that loses its harvest cannot produce more grain until next year and must buy, migrate, or starve, which is what separates a famine from a dip in output.

**Cost.** Grain output responds to a shock only at the next harvest, so any event meant to change food supply within the year has to act on stock, transport, or population instead.

### DEC-039 — The core is imported as source, isolated by its own assembly

One source folder is the single source of truth. The consumer project and the command-line harness each compile it in place, as a separate assembly with automatic host references disabled. CI builds that assembly standalone on every commit.

**Rationale.** Source import keeps iteration fast: a change to a simulation rule is visible in the consumer without a build-and-copy step. Assembly isolation restores the compile-time barrier that a prebuilt assembly would have given, so a call into the host fails immediately rather than working in the editor and breaking headless.

**Cost.** The barrier now depends on project configuration rather than on packaging, so a misconfigured assembly definition silently removes it. The standalone CI build is the only check that catches this, which makes it a release-blocking job rather than a convenience.

### DEC-040 — Prior history is generated by running the simulator

World creation runs the simulator for P-18 years with no hot agents, then takes the result as tick 0.

**Rationale.** The simulator already runs without a player, so prior history costs no new code and no second rule set to keep consistent with the first. Dynasties, ruins, abandoned villages, and old grievances then rest on events that actually happened rather than on plausible-looking generated facts.

**Cost.** World creation becomes an explicit step with a wait rather than an instant action, and its length is bounded by measured cost per simulated year rather than by design preference.

### DEC-041 — The performance budget is a tripwire, not a target

10 ms per simulated day, enforced in CI, at the generator default of 2,000 settlements and the cap of 300 hot agents.

**Rationale.** At the shipping configuration the simulator uses roughly 40% of the budget, which leaves room to add subsystems while keeping the failure close enough that an unexpected doubling of cost trips the test on the day it appears. A looser budget stays silent for years; a tighter one fires on legitimate work until someone raises it, which removes the signal either way.

**Cost.** The figure rests on a measurement covering five of the twelve phases of a settlement update, carried with a factor of two allowance for the rest. The allowance is a judgement, and Phase 4 will replace it with the real number.

### DEC-042 — Cohorts hold currency and own what they produce

Settlement stock is the sum of cohort holdings. Markets hold no currency and no goods of their own.

**Rationale.** Income, wages, tax, inequality, and a market the player can trade in all require someone to be poor and someone to be rich. A single settlement pool cannot express that. Ownership at cohort level gives it at the resolution the simulation already runs at.

**Cost.** Every good now needs a per-cohort holding rather than a single settlement number, which multiplies settlement state by the number of cohorts and makes the clearing pass the most expensive part of a settlement update.

### DEC-043 — Consumption goes to own holdings first, then to market by ability to pay

A cohort eats what it produced before buying, and market allocation under shortage runs in descending order of ability to pay.

**Rationale.** Ability to pay applied to the whole stock would make a farming cohort buy back its own grain and starve beside its field, and would create an unbraked spiral where the poor die, labour falls, output falls, and prices rise further. Ordering own production first confines the rule to the traded share, which is where it belongs. It also gives famine a legible shape: labourers, artisans, and townsfolk feel it before farmers.

**Cost.** Cohorts that both produce and trade need two accounting steps per update rather than one.

### DEC-044 — Rationing carries the severity that price cannot

Price is clamped at four times reference, so scarcity beyond that point shows as who goes without rather than as a higher number.

**Rationale.** The clamp keeps prices from running away, and the rationing order gives the model somewhere to put the severity it can no longer express, so a catastrophic shortage still reads differently from a bad one.

**Cost.** Any UI or design that infers severity from price alone will understate a severe famine.

### DEC-045 — Labour is a good that cannot be stored

Wages come from the same bounded price function applied to labour supply against demand. Labour expires each update.

**Rationale.** Reusing the price mechanism gives a real wage, a labour market, and unemployment with no new machinery, and it makes the real wage referenced by migration an actual quantity.

**Cost.** Labour inherits the price clamp, so a collapsed labour market bottoms out at a floor rather than reaching zero.

### DEC-046 — Currency is conserved, and taxation comes with spending

Currency enters only by an explicit minting event and leaves only by an explicit destruction event. Kingdoms spend from the treasury back to cohorts.

**Rationale.** A conserved quantity is testable every tick. Taxation on its own is a one-way drain that deflates the world over decades before any symptom appears, so the return path belongs in the model from the start rather than as a later balance fix.

**Cost.** Kingdom spending has to have somewhere to go from the first version, which means garrisons, works, and holdings become simulation objects earlier than they otherwise would.

### DEC-047 — Allocation adjusts partially, once per sowing

The settlement moves its land and labour allocation toward the highest expected return by a damped rate, capped per year.

**Rationale.** Sowing decisions made on current prices and acted on at the next harvest form a cobweb: full adjustment overshoots, and the oscillation grows instead of settling. Partial adjustment is what makes the loop converge, and it also matches how slowly a farming population actually changes what it plants.

**Cost.** A second damping constant to tune alongside the trade damping, and a slow supply response that cannot rescue a settlement inside the year a shortage appears.

### DEC-048 — Denominations are explicit only for single holders

Four denominations at fixed ratios. Cohorts hold one integer in smallest units; agents, the player, and treasuries hold a count per denomination.

**Rationale.** Asymmetric conversion, easy downward and gated upward, only blocks a holder who owns a pile of coins on their own. A cohort of hundreds of people always has someone with the right coin, so giving it four counters would multiply settlement state and the clearing pass for a friction it can never feel. Fixed ratios keep every conversion exact, so accounting in the smallest unit stays valid everywhere and the conservation invariant remains testable.

**Cost.** Two representations of money exist, and the boundary between them, promotion of an agent out of a cohort, needs a deterministic rule for splitting a sum into coins.

### DEC-049 — A seller refuses large coin, and the buyer may overpay

Denominations above a settlement's change capacity are refused. Overpaying and forfeiting the difference is always available.

**Rationale.** Refusal alone would let a player carrying only platinum starve in a village with a full granary and no way out. Letting the buyer choose to lose the difference keeps the friction, keeps the refusal meaningful, and puts the decision on the person who can judge whether it is worth it.

**Cost.** A player who does not understand the rule loses money silently, so the interface has to make the forfeit explicit before the sale.

### DEC-050 — Change capacity is a settlement scalar derived from wealth

`changeCapacity` names the largest denomination a settlement's sellers can break, recomputed at settlement cadence from the local wealth distribution.

**Rationale.** One integer per settlement gives the whole rule at negligible cost, stays deterministic, and is legible: villages break silver, cities break anything. It also carries a rhythm for free, rising after the harvest when cohorts have just sold and falling after tax collection.

**Cost.** The relationship between wealth distribution and change capacity is a tuned mapping rather than a simulated one, so it can be made to feel wrong before Phase 6 fixes P-26.

### DEC-051 — Tax remittance travels as a convoy

A settlement's annual remittance becomes an entity moving over the trade graph, carrying coins with weight and open to interception.

**Rationale.** Weight plus gated upward conversion turns tax collection into a real decision: ship heavy copper slowly, or pay a money changer to compress it first. Both sides of that are places the player can act. The volume stays small, roughly one convoy per settlement per year with a few dozen alive at once, so the cost is affordable even though goods trade stays abstract.

**Cost.** Convoys are a new entity class with a lifecycle, a route, and interception rules, which makes money movement more concrete than goods movement and invites the question of why goods are not convoys too.

---

## Occupations and transients

### DEC-052 — A mestiere is a data record

Activity, building, productivity, placement, wealth tendency, social rank. Cohorts are partitioned by it. Occupational shares move under the same damped allocation vector that moves land and labour.

**Rationale.** Occupations follow the rule that already governs goods and recipes, so the list grows by data entry. Reusing the allocation vector means a weaver's wage rising and a field switching from wool to grain are the same mechanism with one damping constant, rather than two systems that can disagree.

**Cost.** Any occupation with genuinely unique behaviour needs a general field added to the record rather than a special case.

### DEC-053 — Caravanners are haulage capacity, and trade is capped by it

A settlement's caravanner population supplies capacity to its outbound links. With none, outbound flow is zero.

**Rationale.** Trade becomes something people do rather than a property of a price difference, so killing haulers, making a road dangerous, or drawing them away with better wages all show up in the economy. Capacity is a multiplier on a formula that already exists, so this costs one term rather than a subsystem.

**Cost.** Trade now depends on a discrete population, which makes it possible to strangle a region's economy by accident during tuning.

### DEC-054 — The cohort key carries two axes from the first day

`(mestiere, wealthBand)`, shipping with one band. CI runs a three-band configuration.

**Rationale.** Deciding the wealth axis after measuring the clearing cost is only possible if adding it stays a data change. Shipping the general path from the start, with a degenerate value, means the code that would assume one row per mestiere never gets written, and the CI configuration catches whatever slips through anyway.

**Cost.** Every cohort lookup is slightly more verbose than it needs to be for the shipping configuration, and CI runs the suite twice. Measurement puts the three-band configuration at roughly 4.8 times the cost of the shipping one rather than 3, because rationing sorts the cohorts of a settlement and that term grows with the square of their number. Whether the axis can be afforded in Phase 6 depends on that sort being cheap, not on the axis itself.

### DEC-055 — Sampled transients are trajectories derived from flow

`Hash(worldSeed, subject, tick, channel, index)` with the link as subject yields a caravan's departure tick, contents, and speed. Observation is free and writes nothing. Interaction writes back to the aggregate as a difference.

**Rationale.** Deriving trajectories rather than per-tick presence is what lets a player walk beside a caravan for days and find it again on the way back, which per-tick sampling cannot do. Writing back on interaction is what keeps a robbery economically real; without it the player learns that caravans are scenery.

**Cost.** Every kind of transient has to be expressible as a schedule derived from a flow, so anything whose behaviour depends on its own history has to become a scheduled transient instead.

### DEC-056 — Caravans and tax convoys are one entity type

They differ in origin: a caravan is sampled from a flow, a convoy is produced by an annual remittance event and holds state.

**Rationale.** One representation means one set of interception, escort, and travel rules, and the player meets both the same way.

**Cost.** The type carries a flag distinguishing authoritative from derived contents, and code touching it has to respect the difference.

### DEC-057 — The itinerant merchant arbitrages, and does not replace haulage

A merchant serves price differences the standing graph does not, at low volume and high margin. A settlement with no caravanners still cannot move its grain.

**Rationale.** Letting merchants supply the missing capacity would quietly undo the caravanner constraint and make famine survivable everywhere. Confining them to arbitrage keeps isolation dangerous while still bringing cloth, tools, and news to a cut-off village.

**Cost.** Players will expect a merchant to solve a food shortage and will need to learn that he cannot.

### DEC-058 — Merchant capacity derives from merchant wealth

One scalar covers travelling on foot, owning a cart, and hiring caravanners.

**Rationale.** Weight-to-value ratio then decides what each merchant can afford to carry, so the three cases fall out of machinery that already exists rather than three rules. A rich merchant hiring haulage also creates the labour demand that keeps caravanners alive.

**Cost.** Merchant wealth becomes a quantity the sampling function has to produce consistently, since it determines what the player finds in the cargo.

### DEC-059 — Settlements know remote prices imperfectly, and merchants act on that

A bounded table holds accurate neighbour prices and aged rumours from further away. Known prices carry a deterministic error growing with age and hops.

**Rationale.** A merchant deciding from true global prices would be an oracle, and arbitrage would be risk-free. Deciding from the chronicle makes information quality an economic asset, gives rumour a mechanical purpose beyond flavour, and lets a merchant be wrong.

**Cost.** Merchant profitability now depends on the tuning of chronicle propagation, so a change to how news travels moves the economy. FR-J-10 is the constraint that keeps the mestiere alive.

### DEC-060 — Route candidates are bounded per settlement

Only pairs a merchant actually travelled accumulate, the list is capped, and it decays.

**Rationale.** Accumulating profitability for every possible pair would be quadratic in settlements, which is the cost the whole trade design exists to avoid.

**Cost.** A route can only become a link if merchants happened to travel it, so a genuinely valuable connection nobody tried stays invisible.

### DEC-061 — Adventurers are a mestiere paid by contracts and recovery

They hold a cohort share, travel as sampled transients, and take income from contracts issued by settlements, kingdoms, and merchants, and from recovering buried stock.

**Rationale.** A mestiere the allocation vector cannot price goes extinct, so adventurers need measurable income like any other occupation. Splitting it between a renewable source and a depleting one gives the profession an arc without letting it die when the arc ends.

**Cost.** Contract issuance becomes a real subsystem with issuers, budgets, and fulfilment, since it is what keeps the mestiere alive once the hoards are gone.

### DEC-062 — Buried stock is finite and placed at tick 0

Currency and goods sit in ruins and hoards, sized against circulating currency. Recovery moves them into circulation.

**Rationale.** Treasure that appears from nowhere is minting under another name and would break the currency invariant silently. A finite placed stock keeps conservation testable and gives the world an economic age: decades when old metal flows in and abundance follows, then exhaustion, after which only what kingdoms mint matters.

**Cost.** The generator has to place and size hoards against a quantity it also generates, and mis-sizing produces either an invisible treasure age or an inflationary one.

### DEC-063 — Discovery is an unpaid byproduct, and therefore unbiased

Adventurers deposit observed prices into a settlement's rumour table without being paid for it.

**Rationale.** Paying for information would send adventurers where merchants already suspect value lies, which is where trade already looks. Leaving it unpaid means their routes follow contracts and hoards instead, so knowledge reaches settlements no merchant would visit. That is what opens routes the candidate list of DEC-060 could never see.

**Cost.** The reach of discovery depends on where hoards and contracts happen to be, so a region with neither stays commercially invisible.

### DEC-064 — Danger is per-link state with an endogenous source and named brakes

Danger raises haulage wages and transit loss and can sever a link. It grows from displaced population and falls through fulfilled contracts and garrison spending.

**Rationale.** Contract income has to renew, or adventurers die out with the hoards and the world loses its discovery channel. Sourcing danger from famine refugees, failed migration, and disbanded soldiers makes the simulation generate its own threats from its own failures, and gives the player a loop to enter at any point: as the hired blade, as the merchant paying, as the bandit, or as the lord deciding whether to fund the expedition.

**Cost.** Poverty, banditry, and interrupted trade reinforce each other, so the brakes have to be tuned rather than assumed, and a region can spiral into isolation if they are set too weak.

### DEC-065 — News rides on carriers, above a floor

Propagation across a link is a fixed floor plus a term proportional to the traffic on it. Migration carries entries in the direction people flee.

**Rationale.** Information and matter then move through the same world. Refugees arriving from a famine and news of that famine stop being independent events that can contradict each other. A route with no caravans goes quiet, so isolation becomes informational as well as economic, and severing a road blinds the player in that direction. The floor exists because pilgrims, soldiers, and vagrants travel where commerce does not, and without it a cut-off settlement would never learn anything again.

**Cost.** Chronicle tuning is no longer separable from trade tuning: changing the trade damping moves the speed of news with it.

### DEC-066 — Staleness and fidelity are separate errors

Staleness grows with elapsed time, fidelity with retellings. Distance enters through travel time only.

**Rationale.** A witness who walked five hundred kilometres still knows what he saw, so degrading his account by distance would erase the first-hand source entirely. Splitting the two keeps distance dominant, since travel over the graph is slow, while leaving a primary source primary. It also gives the two errors different consequences: a merchant can extrapolate from an old but exact figure and can do nothing with a vague one.

**Cost.** Two error terms and two parameters where one would have done, and every knowledge entry carries both counters.

### DEC-067 — Settlements are permanent rows, and abandonment is reversible

Population zero empties a settlement without deleting it. Land, ruins, and whatever the people left behind stay. Migration can target an empty settlement, and unbounded land per capita is what draws people back.

**Rationale.** A fixed row count is what the budget was sized against, so keeping dead settlements costs nothing. Letting them be resettled is what keeps abandonment from being an absorbing state, and it reuses the brake that already holds the migration spiral rather than adding a rule. Land emptied by plague or famine filling again is also what happened.

**Cost.** A settlement that should stay dead for narrative reasons will refill unless something suppresses it, so ruins and danger have to be strong enough to keep the worst places empty on their own.

### DEC-068 — Abandonment feeds hoards and danger

What the departed could not carry becomes buried stock. Ruins raise danger on nearby links.

**Rationale.** The world keeps making new hoards out of its own failures, so adventurers have a renewable recovery source once the hoards placed at tick 0 run out, and ruins give banditry somewhere to live. Both attach to systems that already exist rather than adding any.

**Cost.** Abandonment, banditry, and isolation reinforce each other, and the only brake is resettlement, so the balance between ruin danger and land attractiveness decides whether a region recovers or rots.

### DEC-069 — Ignorance in the world is allowed; ignorance in the log is not

Every event is recorded at its source. Propagation decides only who in the world learns of it.

**Rationale.** A village wiped out by bandits nobody talks to should stay unknown, and finding it years later is worth more than being told. The chronicle already serves both purposes, so keeping the record complete at the source costs nothing and preserves the ability to explain, in Phase 6, why two hundred settlements died.

**Cost.** The chronicle keeps entries no character will ever read, which adds to a log that already has no retention policy.

### DEC-070 — Resettlement is braked by three terms, not one

A hysteresis band on the attractiveness differential, an inbound cap proportional to land rather than population, and a minimum dwell time before a settlement can reverse role. Land per capita is clamped so it is defined at population zero.

**Rationale.** The oscillation here does not come from crossing a threshold, it comes from attractiveness inverting the moment migrants arrive, so a band alone would be jumped in a single tick. The land-proportional cap is what lets an empty settlement refill at all, since a population-proportional cap on zero people is zero. Ruin danger under FR-M-15 gives a fourth brake for free, which is what keeps a massacred village empty without a special rule.

**Cost.** Four interacting parameters govern one behaviour, and a badly set clamp either freezes resettlement or makes empty land irresistible.

### DEC-071 — Urban growth is braked by the import ceiling and by urban mortality

A city's sustainable size is bounded by what its links can bring in. Attractiveness reads food actually available, so growth stops before starvation. Crowding raises baseline mortality independently of contagion.

**Rationale.** Import cost acting through price cannot hold this loop, because price stops at its clamp and the signal dies exactly where the pressure is highest. The caps already in the trade formula, origin surplus and haulage capacity, are an absolute bound instead of a saturating signal, so they hold. Crowding mortality adds a second brake that does not run through price at all, and makes a city what a medieval city was: a place that grows only because people keep arriving.

**Cost.** Maximum city size is largely set by the basin the generator placed around it, so something that looks emergent is partly authored. Rewiring moves the ceiling, which softens this without removing it.

### DEC-072 — Buildings are an explicit stock, gated by site attributes

Cohorts build them from timber and labour, they decay, and events destroy them. Some types require an attribute the generator placed: ore, running water, forest.

**Rationale.** An explicit stock turns surplus into future capacity, so a settlement can develop and a player can fund something, and a decade's absence shows in what has been built as well as in who lives there. Site attributes are what keep settlements from converging on identical economies: without them every place can make everything, price differences vanish, and trade goes with them.

**Cost.** Capital accumulation is a reinforcing loop, braked by maintenance, by available labour, and by falling prices. None of those three brakes is a site ceiling, so all three have to be tuned rather than assumed.

### DEC-073 — Every durable-requiring recipe has a crude variant

Same output, no durable needed, a far worse input-to-output ratio.

**Rationale.** Requiring tools to make tools is a deadlock for a village that loses them, for one just resettled, and for anything cut off from trade. A second recipe breaks the circle as data rather than as a special case, which keeps the promise that goods and recipes need no code. It also gives durables their demand: crude work is bad enough that replacing tools is worth paying for.

**Cost.** Every durable-requiring recipe needs a crude twin authored alongside it, and the ratio between them is a tuning lever that decides how badly a stripped settlement suffers.

---

## Credit

### DEC-074 — Credit moves existing money and reaches only those with collateral

A loan is a transfer. Borrowing capacity is bounded by goods, durables, and buildings pledged.

**Rationale.** Keeping money conserved makes the whole credit system testable against an invariant that already exists. Tying capacity to collateral means the poorest are excluded, which preserves the rationing order: a cohort with nothing to pledge is not saved by borrowing, and hunger still reaches it first. It also solves the input circle for anyone who owns productive capital, and leaves selling labour as the route for everyone else.

**Cost.** Credit is a tool of the propertied, so it widens the gap it is often expected to close. That is the intended reading, and any design assuming credit rescues the poor has to be revised.

### DEC-075 — Loans carry interest, priced like everything else

The rate is the bounded price function applied to credit supply against demand, plus a risk premium from the collateral ratio.

**Rationale.** Without interest nobody prefers lending to holding, so no lender could exist as an occupation the allocation vector can price. Interest also gives risk a price, which connects credit to harvests, routes, and danger instead of leaving it isolated. Reusing the price function adds no machinery.

**Cost.** Interest plus seizure concentrates wealth. The brakes are write-offs on default, taxation, and kingdom spending, and at least one has to bite or the economy grinds toward a few holders.

### DEC-076 — Default stops at buildings, and land stays unowned

Goods, then durables, then buildings. No person is taken. Land remains a settlement attribute.

**Rationale.** Stopping short of bonded labour keeps a defaulted cohort economically alive and able to sell its labour, so ruin is recoverable. Leaving land unowned protects the settlement-level land per capita that brakes the migration spiral, and keeps rent and tenancy out until the wealth axis exists to carry them.

**Cost.** No dispossession of peasants, no rent, no great estates. A substantial medieval dynamic sits outside the model until the cohort key gains its second axis.

---

## Guilds and dungeons

### DEC-077 — The guild is a demand aggregator and a merchant, not a pricing system

It converts needs into funded contracts, escrows the reward, and trades loot as a merchant cohort at a fixed margin.

**Rationale.** Contract issuance had no home in the update sequence, which left the adventurer's renewable income unfunded. The guild gives it one, and doing its trading through an ordinary merchant cohort keeps a single pricing rule in the world. Its purchases of equipment from local merchants return the fees and margins it collects, so it does not become a sink.

**Cost.** Guild halls hold escrowed currency, so the conservation invariant has one more place to look.

### DEC-078 — Ranks are a distribution per hall, and hard problems can stay unsolved

Contracts carry a required rank. An adventurer may take one rank above their own. Where the ranks are not there, the contract sits.

**Rationale.** Without this, contracts solve any crisis the moment money is posted, and the poverty-banditry loop has a brake that never fails. A region with only novices keeps its dangerous road, keeps its isolation, and keeps sliding, which is what makes that loop real. Holding the distribution on the hall rather than partitioning the cohort keeps the cost to one small vector.

**Cost.** Rank progression is a flow between buckets that has to be tuned, and a world seeded with too few high ranks can lock itself into decline.

### DEC-079 — A dungeon is a regenerating resource site, not a treasure box

Stock regenerates below what unconstrained extraction would take. Infested it raises danger; cleared it permits a mine and ordinary miners work it.

**Rationale.** Naming it a resource site puts renewable loot inside the production model instead of beside it, so no exception is needed for goods appearing from nowhere. Clearing as a site-attribute unlock reuses the building prerequisites already in place, and gives a full cycle the player can walk: danger, contract, clearing, mine, rare material on the market. Reversion keeps the cycle turning.

**Cost.** Rare material supply now depends on adventurer throughput, so a decline in that occupation propagates into the crafting economy.

### DEC-080 — Dungeon materials may be required, and a static check protects subsistence

Recipes may require dungeon-only materials. Validation at load proves nothing in tier 0 or tier 1 depends on one.

**Rationale.** Required materials give dungeon extraction a standing industrial demand, which is a steadier income for adventurers than contracts funded by other people's misfortune. Restricting the dependency to higher tiers means losing access costs the world its fine goods and its lost arts, never its bread.

**Cost.** The recipe graph gains a validation rule that a designer can trip by adding one ingredient, and the failure has to stop the world from loading rather than surface later.

---

## Drawing

### DEC-081 — The draw's subject coordinate is a 64-bit key, not an entity handle

`Hash(worldSeed, subject, tick, channel, index)`, where `subject` is a 64-bit key. An `EntityId` is one such key, packed as generation in the high half and row index in the low half. A row that is not an entity, a trade link or an event, uses a key whose high half is zero.

**Rationale.** Not everything that draws occupies a row with a generation: FR-X-02 enumerates sampled transients per link, and a link has no generation. One coordinate space rather than two means the properties of DEC-002 are proved once. The high half separates the cases at no cost, since no live handle carries generation 0, so a row key and a live entity key cannot name the same draw.

**Cost.** A caller holding something other than an `EntityId` builds its key through the provided helper rather than casting, or a negative row index sign-extends into the entity space.

### DEC-082 — Generation 0 is reserved and never live

A live `EntityId` carries a generation of at least 1. Index 0 with generation 0 is the absent handle, and no row is issued with generation 0.

**Rationale.** A defaulted field then reads as absent rather than as a valid handle to row 0, and the generation half of a packed key is free to act as a namespace separator for DEC-081. Both properties are relied on elsewhere and neither survives issuing generation 0.

**Cost.** Row reuse increments past 0 on wraparound, which a naive counter does not.

### DEC-083 — Each kind of non-entity subject draws in its own channel

Where two kinds of subject that are not entities draw, links and events for instance, they take separate channel values.

**Rationale.** Entity keys separate themselves, because the generation makes two distinct entities distinct keys. Row keys do not, so in a shared channel row 7 of one kind and row 7 of another name the same draw and move together for the life of the world. The channel is the only thing keeping their key spaces apart.

**Cost.** Channels are a numbered contract that cannot be renumbered, so this spends them faster than one per subsystem would.

### DEC-084 — Reducing a draw to an interval uses a plain modulo

`draw % count`. No rejection, no re-mixing.

**Rationale.** The relative excess of the low results is at most `count / 2^64`, of order 1e-17 for an interval of a thousand, and observing it would take on the order of 2^64 draws against the 1e12 a sixty-year world produces. Rejection would not remove it: the mixing function is a bijection, so re-mixing relocates the discarded set onto another set of the same size instead of spreading it, and iterating a bijection has no proof of termination.

**Cost.** The reduction is not exactly uniform, and says so where it is written. Anyone reopening this has to re-derive the two paragraphs above.