# Promotion candidates for `docs/SIM-DEC.md`

`DECISIONS-OUTSIDE-SPEC.md` accumulates and never empties. Some of what lands
there is working detail that belongs nowhere else, and some of it is
architecture: a constraint on all future code, which a later implementer would
need and could not infer. Left in the register, the second kind turns the
register into a shadow specification that grows while `docs/` ages — the same
failure the prohibition on editing `docs/` exists to prevent, arriving slowly
instead of at once.

This file is the proposal. **A human or the decider writes `docs/`.** Nothing here is in the
specification until someone puts it there, and an entry that proposes to change
`SIM-DEC` has no authority whatsoever until they do.

The other direction — the specification is wrong — is `SPEC-QUESTIONS.md`.

## The pass

Runs when a plan is archived, over the register entries written since the last
pass. For each, one question: **does this constrain code beyond the change that
produced it?**

- **Yes** → candidate. Cite the `D-NNN`, say why in one line, and draft the
  `SIM-DEC` entry in that document's own shape: the decision, then
  `**Rationale.**`, then `**Cost.**`.
- **No** → not listed. Process choices, one-off trade-offs, "left as found",
  anything about how the work was organised rather than what the simulator is.

When a human writes one into `docs/`, the register entry is marked with the
identifier it became, and the candidate is struck from here. A candidate they
reject is struck too, with their reason: an unanswered proposal re-proposed
every archival is how a rejected idea gets in by attrition.

Numbering is the human's or the decider's: `SIM-DEC` runs to DEC-080 today, and a number claimed
here would collide with whatever else is in flight.

---

## Pass of 2026-10-01 — three candidates, phase 3

Run over `D-044` to `D-106` on archiving the phase 3 plan, the headless kernel.
The first pass over a development that built the simulator rather than its
apparatus, and the first to find candidates since the first pass.

**Not listed, by kind.** `D-044` to `D-055` and `D-086` to `D-089` are process:
how the plan, the requests and the agent workflow work (`D-011`). The criterion
defects — `D-059`, `D-064`, `D-065`, `D-070`, `D-075`, `D-084`, `D-095` — are
about checks, not the simulator. The test and CI entries of points 8 to 10 bind
tooling, not the core. What the rulings already put into `docs/` is not
re-proposed: `D-057` (`R-007`), `D-064` (`R-006`, DEC-085), `D-074` (`R-014`),
the queue as input (`R-021`), the command as data (`R-022`, DEC-086), the cause
kind (`R-024`).

**Near misses.**
- `D-073`, the level order and the first tick of the period. It binds every
  phase 4 system, but `docs/` holds the question open in `SIM-ECON`'s item 4,
  and the entry itself declined to pin it. Closing an open item is the
  decider's, not a promotion.
- `D-090`, the save format. `R-029` read "a later format keeps the reader of
  every earlier one" as NFR-08's plain meaning and declined to promote it.
  Not re-proposed.
- `D-106`, a refused command. A finding for phase 4, with no answer to promote.

### ~~C-5 — commands apply at the start of the tick~~ — promoted as DEC-087 by R-040

From `D-077`. **Why:** FR-A-01 asks for "a defined point in the tick" and
`docs/` does not say which. Every phase 4 system runs after it or before it,
and the replay log of DEC-030 depends on it.

> ### DEC-0NN — Commands apply at the start of the tick, in the order submitted
>
> A tick opens by applying, in submission order, every command waiting when it
> starts; only then does any level run. A command submitted while a tick runs
> waits for the next one.
>
> **Rationale.** The tick a command lands on then depends only on when it was
> submitted relative to the tick boundaries, not on how the host calls the loop
> — one tick per call or many — which is what makes AC-02's "same commands"
> reproducible and what DEC-030's log records. Applying before the levels means
> every level of the tick, the settlement update included, sees the action on
> the day it lands.
>
> **Cost.** Anything submitted during a tick waits up to a whole tick, which is
> DEC-030's cost made exact. What the drain does with a command the state
> refuses is not settled by this entry.

### C-6 — only the rows in use are state

From `D-081` and `D-083`. **Why:** DEC-085 fixes how a variable-length field is
laid out, not what of it is state. Every table that grows — the chronicle now,
pools and agents later — needs the answer, and a walk that reads to the array's
length rather than the count is a determinism hole with passing tests.

> ### DEC-0NN — Only the rows in use are state
>
> A column may be sized beyond the rows it holds. Its capacity is not state: a
> count in state says how many rows are in use, and the state hash, the save and
> every check walk those rows and no further.
>
> **Rationale.** Growing by doubling is how an append-only table stays cheap, and
> the capacity it reaches depends on the history of its growth, not on what the
> world holds. Counted by capacity, two worlds with the same entries would hash
> differently and a save would carry rows nothing wrote.
>
> **Cost.** Every growable table carries its count in state, and every walk of it
> must stop at the count. A walk that reads to the length passes every test on a
> world whose arrays happen to be exactly full.

### C-7 — a load refuses what no build could have written

From `D-091`. **Why:** NFR-08 and DEC-032 say what a save holds, not what a load
does with bytes that do not hold it. Every reader phase 4 adds meets the
question, and a load that accepts a world its own systems could not have
produced fails later, far from the bytes.

> ### DEC-0NN — A load refuses what no build could have written
>
> A save is input from outside the core. Reading one checks every count against
> the bytes left before allocating, every field against the rules its writer
> keeps, and the format and rule versions against those this build knows. Any
> failure refuses the whole save, with one error type, and returns no world.
>
> **Rationale.** A save crosses the host's disk and a player's hands. A world
> partly loaded, or holding a row no system could have written, fails ticks
> later where nothing points back at the save, and the bug report DEC-032 wants
> reproducible does not reproduce. A rule version later than the build's is
> refused because the state would claim rules it does not run (NFR-09).
>
> **Cost.** Each writer's rule is stated twice, where it writes and where the load
> checks it, and a rule changed in one place only refuses valid saves or admits
> invalid ones. Pending commands are checked only as far as the build knows
> their kinds (`D-106`).

---

## Pass of 2026-09-19 — closed

**All four candidates were written into `SIM-DEC` on 2026-09-19, as `DEC-081` to
`DEC-084`, under a new `Drawing` section.** The human kept the drafted text close
to intact. The register entries they came from are marked with what they became,
and the candidates below are struck: they are kept as the record of what was
proposed, not as an outstanding ask.

| Candidate | From | Became |
|---|---|---|
| C-1 | `D-001` | `DEC-081` |
| C-2 | `D-002` | `DEC-082` |
| C-3 | `D-007` | `DEC-083` |
| C-4 | `D-006` | `DEC-084` |

`D-004`, listed below as belonging in the other file, closed the other way:
`SQ-002` split A-11 rather than adding a decision. The guard was right and the
invariant's text was wrong.

First pass ever run. It therefore covers the whole register rather than one
development, and the development just archived — the plan mechanism, `D-011`
to `D-027` — contributes **nothing**. That is not an oversight: `D-011` records
that process work has no business in the specification, and a pass that found
promotable architecture in its own tooling would be the first sign the rule had
stopped being observed.

Four candidates, all from the `hash64-soggetto-e-range` development, plus one
entry that may belong in the other file instead.

### ~~C-1~~ — promoted as `DEC-081`

<!-- from `D-001`: the draw subject is a 64-bit key, not an entity

**Why:** DEC-002 fixes the draw's coordinates, and `entity_id` is now one case
of a wider coordinate. Every future subsystem that draws for a link, an event or
a sample depends on this, and nothing in `docs/` says it.

> ### DEC-0NN — The draw's subject coordinate is a 64-bit key, not an entity handle
>
> `Hash(world_seed, subject, tick, channel, index)`, where `subject` is a 64-bit
> key. An `EntityId` is one such key, packed as generation in the high half and
> row index in the low half. A row that is not an entity — a trade link, an
> event — uses a key whose high half is zero.
>
> **Rationale.** Not everything that draws occupies a row with a generation.
> FR-X-02 enumerates sampled transients per link, and a link has no generation.
> One coordinate space rather than two means the properties of DEC-002 are
> proved once. The high half is what separates the cases at no cost: no live
> handle carries generation 0, so a row key and a live entity key cannot name
> the same draw.
>
> **Cost.** A caller holding something other than an `EntityId` has to build its
> key through the provided helper rather than casting, or a negative row index
> sign-extends into the entity space.
-->

### ~~C-2~~ — promoted as `DEC-082`

<!-- from `D-002`: generation 0 is reserved

**Why:** an invariant on `EntityId` that the whole subject scheme rests on, and
`SIM-STATE` describes the handle without stating it.

> ### DEC-0NN — Generation 0 is reserved and never live
>
> A live `EntityId` carries a generation of at least 1. Index 0, generation 0 is
> the absent handle, and no row is ever issued with generation 0.
>
> **Rationale.** A defaulted field then reads as absent rather than as a valid
> handle to row 0, and the generation half of a packed key is free for the hash
> to use as a namespace separator. Both properties are relied on elsewhere and
> neither survives issuing generation 0.
>
> **Cost.** Row reuse has to increment past 0 on wraparound, which a naive
> counter does not.
-->

### ~~C-3~~ — promoted as `DEC-083`

<!-- from `D-007`: one channel per kind of non-entity subject

**Why:** a rule binding every subsystem added from here on, currently recorded
only in a source comment.

> ### DEC-0NN — Each kind of non-entity subject draws in its own channel
>
> Where two different kinds of subject that are not entities draw — links and
> events, say — they take separate `HashChannel` values.
>
> **Rationale.** Entity keys separate themselves: the generation makes two
> distinct entities distinct keys. Row keys do not, so in a shared channel row 7
> of one kind and row 7 of another name the same draw and move together for the
> life of the world. The channel is the only thing that keeps their key spaces
> apart.
>
> **Cost.** Channels are a numbered contract that cannot be renumbered, so this
> spends them faster than one per subsystem would.
-->

### ~~C-4~~ — promoted as `DEC-084`

<!-- from `D-006`: the range reduction is a plain modulo, and its bias is accepted

**Why:** DEC-002 names rejection sampling as a cost to be rewritten away; this
records what was done instead, and forecloses a future contributor
reintroducing rejection to "fix" the bias.

> ### DEC-0NN — Reducing a draw to an interval uses a plain modulo
>
> `draw % count`. No rejection, no re-mixing.
>
> **Rationale.** The relative excess of the low results is at most
> `count / 2^64` — of order 1e-17 for an interval of a thousand — and observing
> it would take on the order of 2^64 draws against the 1e12 a sixty-year world
> produces. Rejection would not remove it: the mixing function is a bijection,
> so re-mixing relocates the discarded set onto another set of the same size
> instead of spreading it, and iterating a bijection has no proof of
> termination.
>
> **Cost.** The reduction is not exactly uniform, and says so where it is
> written. Anyone who reopens this has to re-derive the two paragraphs above.
-->

## Pass of 2026-09-20 — nothing

Run over `D-038` to `D-043` on archiving the lookup plan. No candidates. Two of
them are about this mechanism's own commit trailers and one about how a check
reports itself; the rest record an edit to `docs/` and why it was bounded.
`SIM-DEC` describes the simulated world, and none of this is in it.

`D-040` is the near miss, for the second pass running: it is a finding about how
an agent should treat the specification, which sounds architectural and is not.
It belongs to `AGENTS.md` if anywhere, not to `SIM-DEC`, and it is already
reflected in the `SPEC-QUESTIONS.md` entry it came from.

---

## Pass of 2026-09-19, second run — nothing

Run over `D-033` and `D-034`, written since the first pass, on archiving the
review-request plan. Both are about how the mechanism asks a human to look. No
candidates, for the reason that will hold for every process plan: the
specification describes the simulated world, not the apparatus that builds it.

---

### Later in the first pass — nothing from `D-028` to `D-032`

Those entries were written after this pass began, closing points 10 to 13 of the
same process plan. Run against them, the pass finds **no candidates**: they are
about how work is organised — a promotion ritual, a read-only dispatch, a commit
trailer, a marker on a criterion — and none of them constrains the simulator.

`D-030` is the one worth naming as a near miss. It records three defects in the
mechanism's own commit trailer and lookup. Real defects, and still nothing for
`SIM-DEC`: the specification describes the simulated world, not the apparatus
that builds it, and an entry there about commit trailers would be the first
crack in that.

### Not a candidate — `D-004`, filed as a question instead

A-11 in `SIM-STATE` lists "no accumulator has overflowed" among the invariants
**asserted every tick**, and the guard implemented is `[Conditional("DEBUG")]`.
That is not a decision to add to `SIM-DEC`; it is the implementation departing
from a requirement, which is the other file's business. Filed as `SQ-002`.

The pass found it while classifying, which is worth noting as a property of the
pass rather than of this entry: reading the register for promotable architecture
is also the moment someone reads every decision back against the specification
it was taken under.
