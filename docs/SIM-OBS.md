# Phase 0 — Observables

**Document:** SIM-OBS
**Status:** Draft for correction
**Companion:** SIM-REQ, SIM-DEC

---

## How to use this document

This list names what the player must notice about the world. Everything in SIM-REQ exists to produce one of these. A requirement serving none of them is either unjustified or has found a gap in this list.

The list was drawn from requirements that already exist, so it validates them by construction. Its value is in what gets cut. Each entry therefore carries the requirements that lose their justification if it goes.

Five entries. Adding a sixth is a decision with a cost; the ceiling is deliberate.

---

## OBS-1 — Prices differ, in place and over time, and something distant explains it

The world is read two ways, and the player chooses which.

A player arriving from yesterday's village finds bread here at twice the price. A player who has kept a stall in the same city for six years watches the price of cloth climb week after week. Both then hear, later and second-hand, what caused it: a failed harvest upstream, a route closed by war, plague in a trading partner.

**Signal chain.** Price felt first, as a gradient by the traveller and as a series by the resident. Rumour arrives later, degraded, and names the cause. The gap between feeling it and understanding it is the point.

**Timescale.** A single visit for the gradient. Weeks for the series. Weeks to months for the explanation.

**The hard case is the resident in a large city.** Cities are high-degree, high-stock nodes, so diffusion damps them more than anywhere else. The place a settled player is most likely to live is the place where prices move least. Hubs must keep residual volatility, and tuning that stabilises the economy will flatten them first.

**Seasonality cuts both ways.** The resident sees the annual cycle repeat and learns to subtract it, so what remains is signal. The traveller has no history of any place and cannot separate the two, which is why the chronicle carries the cause explicitly rather than leaving it to be inferred from numbers.

**Served by.** FR-M-01 to FR-M-03 for local price and diffusion. FR-I-01 to FR-I-06a for the chronicle, carrier-borne propagation, and the two error terms. FR-J-08 and FR-J-13 for who carries what. DEC-017, DEC-018, DEC-020, DEC-027, DEC-059, DEC-065, DEC-066.

**Tested by.** AC-09 and AC-17 for the time signal. AC-38 and AC-39 for the spatial signal and for hub volatility. AC-40 to AC-42 and AC-44 for what reaches whom, and for what reaches nobody.

**Depends on the host.** FR-I-03b works only if the game shows a settlement filling with hungry strangers. The simulator produces the fact; whether the player sees it is outside this module.

**First moment.** To be named: the point in play where a player first notices this. Without one, the entry is aspiration.

**If cut.** Chronicle propagation loses its purpose beyond debugging. Diffusion pricing could be replaced by a global equilibrium. Merchant knowledge tables lose their reason to be imperfect, and DEC-059 loses its only justification.

---

## OBS-2 — Settlements grow and empty, and the player can learn where the people went

A village thins out over years. A town swells. A player returning after a decade finds a different place. Which people went where is not read from the demography, which is anonymous, but from the people themselves: a family the player knew, met again in a city; refugees arriving and saying what they fled; a settlement visibly full of strangers.

**Two halves, served by two subsystems.** Demography produces the fact that populations move. The chronicle and the agents produce the legibility. Looking for legibility in the demographic subsystem is a mistake worth avoiding in Phase 4.

**Signal chain.** Population visible on the ground → construction or abandonment → the destination is a real place the player can go to → refugees, promoted agents, and rumours name the link between the two.

**Timescale.** Years for the change, a decade for it to be unmistakable. This is the slowest entry in the list, and the one that justifies simulating decades at all.

**Served by.** FR-D-01 to FR-D-11 for the movement and its brakes. FR-P-14 to FR-P-16 for what makes a place worth moving to. FR-I-03a and FR-I-03b for the carriers and the visible evidence. DEC-012, DEC-028, DEC-029, DEC-047, DEC-067, DEC-070.

**Tested by.** AC-04 for the stability of the flows. AC-43 for reversibility. AC-45 for flicker. AC-46 for whether the change is large enough to see.

**Depends on the host.** A settlement at half its population must look different. If the game renders it identically, this observable does not exist regardless of what the simulation computes.

**First moment.** To be named.

**If cut.** Demography becomes a backdrop and could run as a fixed rate. Land per capita loses its role as the brake on the migration spiral. The allocation vector loses half its purpose, and the case for simulating decades weakens sharply.

---

## OBS-3 — Scarcity reaches people unequally

In a hungry settlement, some go without while the market still holds grain. Who suffers is not random: it is whoever buys food rather than grows it, and whoever has least.

**Signal chain.** Individuals the player knows are affected differently → the granary is visibly not empty → the reason is price and purse.

**Timescale.** A season.

**Served by.** FR-C-01 to FR-C-05, FR-W-08, FR-J-01 to FR-J-03, DEC-042, DEC-043, DEC-044, DEC-052, DEC-054.

**If cut.** Cohorts no longer need to hold currency or be partitioned by occupation. The clearing pass collapses into a consumption rate. This is the single most expensive observable in the list, and the one to cut first if the budget forces a cut.

---

## OBS-4 — Roads become dangerous or safe, and commerce follows

A route the player used last year now has no caravans on it. Imported goods cost more in the town at its end. Contracts appear offering payment to clear it. When someone does, the caravans come back.

**Signal chain.** Absence of caravans on the road → price of imported goods → contracts posted → restoration after action.

**Timescale.** Months.

**Served by.** FR-J-04 to FR-J-05, FR-J-11 to FR-J-17, FR-M-03a, FR-M-12 to FR-M-14, FR-C-08, DEC-053, DEC-061, DEC-064.

**If cut.** Caravanners can be a label rather than a capacity. Danger stops being state. Adventurers lose their renewable income and the discovery channel goes with them, which takes the dynamic trade graph with it.

---

## OBS-5 — A kingdom falls, and the signs were there beforehand

News arrives that a kingdom the player never visited has collapsed. A player who was paying attention had seen it coming: taxes rising, garrisons unpaid, roads unsafe, towns emptying.

**Signal chain.** Rumours over years → observable symptoms in any settlement of that kingdom → the collapse itself as news.

**Timescale.** A decade.

**Served by.** FR-C-06 to FR-C-09, FR-E-02 to FR-E-04, FR-E-07, FR-T-07, DEC-022, DEC-023, DEC-046.

**If cut.** Fragility metrics lose their consumer. Endogenous events could be scheduled instead of thresholded. Taxation becomes accounting rather than a cause.

---

## Requirements serving no observable

Two systems in SIM-REQ do not produce any entry above. Both may be right to keep, but their justification is gameplay rather than world simulation, and that should be explicit rather than assumed.

**Coin denominations, weight, change refusal, money changers, tax convoys.** FR-C-12 to FR-C-21, DEC-048 to DEC-051. These create friction the player feels when carrying and spending money. No entry above needs them. Either a sixth observable names that friction, or they are accepted as a player-facing system carried by the simulator.

**The treasure age.** FR-J-14, FR-J-15, DEC-062. Buried stock depleting over decades changes the money supply across a span longer than a single character's active life. A player may never perceive the arc. It stays justified as a constraint that keeps recovery from being free money, which is a correctness argument rather than an observable one.

---

## Open

1. Cut, merge, or rewrite entries. Cutting is the useful operation.
2. Decide whether monetary friction earns a sixth entry.
3. For each entry, name the one moment in play where the player would first notice it. If no such moment exists, the entry is aspiration rather than an observable.
