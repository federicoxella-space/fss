# Review request

The mechanism in this repository records everything and asks for nothing. A
human opinion therefore arrives whenever one happens to be offered, which on a
long branch means after the work that needed it is already built on.

This file interrupts. It holds the current request for a human's attention, and
is replaced by the next one; the old ones are in git.

## The three moments

Only three are worth interrupting for. Outside them the record is enough.

1. **Before a plan starts.** The cheapest moment there is: a wrong criterion
   found here costs one conversation, found at the end it costs every point
   built on it. And the plan is short — a few hundred words against a branch.
2. **At a phase gate**, from `docs/SIM-REQ.md` §18. Phase 3 closed, phase 4
   closed. This is the moment for looking at the whole rather than the point,
   which nothing else in this mechanism does: `/plan-next` sees one point,
   `/plan-status` sees one plan.
3. **When the work meets the specification.** A `SPEC-QUESTIONS.md` entry is
   raised, or the promotion pass produces candidates for `SIM-DEC`. These are
   the return channel, and a return channel nobody reads is a drawer.

A request is written **at** those moments, not near them, and never instead of
them: writing one does not authorise continuing past a blocking question.

## What a request holds

Six parts, all of them:

- **Which moment**, of the three.
- **Which branch or commit**, so that what is being talked about can be read.
- **What changed, in one line.** If it needs two, the request is covering more
  than one moment.
- **The decisions taken outside the specification in this block**, by their
  `D-NNN`, not restated.
- **The open questions about the specification**, by their `SQ-NNN`, with
  whether they block.
- **What was not verified.** Checks satisfied by reading, reviewers that did not
  run, clauses left pending. This is the part with no other home, and the part
  a reader cannot reconstruct.

The sixth is the reason the file exists. Everything above it is discoverable
from the repository by someone willing to look; what was *not* done leaves no
trace, and only the author knows it.

---

# Current request — 2026-10-05 — moment one, the cost measurement plan

Replaces the moment-three request of 2026-10-01, whose four items are all
answered and recorded there; it is kept in git.

**Moment:** one — before a plan starts. The draft below is the plan; under
`R-048` it is installed as `PLAN.md` on its branch only after the answer, in a
commit that cites it, and under `R-049` its first point waits for that answer.

**Branch:** the request is written on `design/poverty-seed`, which carries
`SQ-009` to `SQ-024`. The plan would run on its own branch, `misura-costi`.

**What changed, in one line:** a draft plan to measure what a settlement update
costs as goods, rows and market places vary, owed by `SQ-023` before the state
format is fixed.

## Decisions taken outside the specification

`D-109`: why the plan is `Kind: process`, why the stand-in lives in its own
project and stays after the plan, the calibration tolerances, and the sizes per
settlement type. `D-108` precedes it, on the review this request follows. Neither
is audited; the watermark stands at `D-107`.

## Open questions about the specification

- **Blocking this plan: where it sits against `R-046`.** `R-046` says that "the
  two plans that follow" are the process plan of the agent workflow and then
  phase 4. This plan is a third, and it must come before phase 4, since
  `SQ-023` blocks phase 4's plan and is owed this measurement. Whether it runs
  before the process plan, between the two, or as phase 4's first point is a
  reading of `R-046` for the decider, or for the user under reserve 1 of
  `R-001` if it is read as governance. The draft assumes a plan of its own.
- **`SQ-023` and `SQ-024`, blocking the phase 4 plan**, not this one: this plan
  is what `SQ-023` asks for before its ruling.
- **`SQ-009` to `SQ-022`, non-blocking**, for the decider; `SQ-020` answered by
  the user.

## The draft plan

```markdown
# Plan — the cost of a settlement update, measured on a stand-in

**Kind:** process
**Exit condition:** the table that `SQ-023` owes before its ruling — the cost
of one settlement update per settlement type, and of one simulated day on the
mix of P-12 as the user answered it in `SQ-024`, dense against sparse state and
every settlement clearing against market places only — recorded in the
register and in `SQ-023`, read against P-01, NFR-05 and NFR-07.
**Phase:** before 4 (SIM-REQ §18; `SQ-023`)   **Branch:** `misura-costi`

The stand-in is not the simulator. It touches arrays the way the update will,
in integer arithmetic only, and its numbers mean what it resembles (`SQ-004`).
It decides the shape of the state, not P-17: that is measured on the real
update in phase 4 (§19). Nothing in `core/` changes. The sizes it runs on are
measurement inputs, not simulation constants, and take no P-number (`R-050`).

Every cell is measured the same way: Release build, one thread pinned to one
core, a warm-up, then at least 10,000 updates of which the 99th percentile is
kept (P-01 is a p99); three runs, the median of the three p99 reported. The
machine, the runtime and the clock's resolution are printed with the results.

## 1. The stand-in and its calibration

- **Does:** a project `bench/`, outside the solution's test run and outside CI,
  with a stand-in settlement update of five passes — production, own
  consumption, market clearing with a ranking by ability to pay, prices,
  spoilage with a carry per row and good — over dense per-good arrays.
- **Closed by:** the run at 9 and at 27 rows with 7 goods reports a cost at 9
  rows between 1.6 and 6.6 µs, half and twice P-63, and a ratio of the 27-row
  cost to the 9-row cost between 3.8 and 5.8, the measured 4.8 of DEC-054 and
  P-64 within a fifth.
- **Fails when:** the clearing ranks rows in a way linear in their number, so
  the ratio falls to about 3; or the JIT removes the work, so the cost falls
  below the floor. Both are introduced and seen red.
- **Core:** no — the diff touches only `bench/` and the solution file.

## 2. Dense state, scaled

- **Does:** the stand-in swept at 12 rows over 8, 30, 100 and 300 goods, and at
  100 goods over 12, 24 and 41 rows, with the bytes of state per settlement
  computed from the array sizes.
- **Closed by:** a table with all seven cells, each from the method above, and
  a checksum per cell of the stand-in's outputs, printed beside its cost; a
  rerun gives the same checksums.
- **Fails when:** a cell is sized for its goods but loops over fewer, which
  costs less and changes the checksum against a reference loop over every good.
- **Core:** no — the diff touches only `bench/`.

## 3. Sparse state per settlement type

- **Does:** a second layout holding, per settlement, rows only for the mestieri
  present and holdings only for the goods present, run on the five types of
  `SQ-024` at the sizes the user approved: village 15 goods and 6 rows, borgo
  30 and 10, cittadina 60 and 18, city 150 and 30, metropolis 300 and 41.
- **Closed by:** per type, the sparse cost against the dense cost of the same
  type at the catalogue's 300 goods and 41 rows; and on the same world the
  sparse and the dense layouts give the same checksum.
- **Fails when:** the sparse walk drops the last local good or the last row, an
  off-by-one that is faster and changes the checksum.
- **Core:** no — the diff touches only `bench/`.

## 4. Market places and goods variants

- **Does:** clearing runs only at market places, 100 of 2,000 by P-12 as
  answered in `SQ-024`, with each village's demand and surplus routed to its
  nearest market; and a class with K variants, at K = 1, 4 and 16, clears once
  with its demand split among the variants.
- **Closed by:** the cost of one simulated day on the P-12 mix with clearing at
  market places against clearing everywhere; the cost per variant count; and,
  with supply ample, every unit a village demands is served at its market.
- **Fails when:** a market clears its own cohorts and ignores the villages
  routed to it, which is cheaper and leaves village demand unserved.
- **Core:** no — the diff touches only `bench/`.

## 5. The table, read against the budget

- **Does:** the results gathered in one table — µs per update per type, ms per
  simulated day on the P-12 mix, MB of state — for each of the four layouts
  (dense or sparse, clearing everywhere or at market places), each set against
  P-01's 10 ms and NFR-07's 50 MB, and written into `SQ-023` for the decider.
- **Closed by:** the ms per day in the table equals the sum over types of count
  × cost per update ÷ 7, recomputed from the cells by the runner itself.
- **Fails when:** shallow — the point gathers numbers already measured; the
  recomputation catches a table edited by hand.
- **Core:** no — the diff touches only `bench/`, `SQ-023` and the register.
```

## What was not verified

- **What the stand-in of P-63 and P-64 contained.** It was thrown away
  (`SQ-004`), and "5 of 12 phases" does not say which five. The five passes of
  point 1 are a guess at them, and the calibration tolerances are wide for that
  reason and because the machine differs. A stand-in that calibrates can still
  differ from the old one in what it costs per good.
- **The reference machine.** P-01 names no machine. The plan prints the one it
  runs on; whether that is the machine the budget is read on is not settled.
- **The sizes per type** are the user's orders of magnitude, approved as given;
  they are not derived from any content, since the catalogue does not exist.
- **The cost of routing and of assignment** — a village finding its nearest
  market per group, and reassigning — is not in point 4, which routes along a
  fixed assignment. `SQ-024`'s point 5 leaves the rule open, so its cost cannot
  be measured yet.
- **No reviewer has read this draft.** Every point declares `Core: no`, so none
  is owed at close either; the `Fails when:` lines are read by the user here,
  under `R-047`.

## What I need from you

1. **The draft**, approved, amended, or rejected. In particular the five
   `Fails when:` lines, and the calibration tolerances of point 1.
2. **Where it sits against `R-046`** — or that the decider answers it first.
3. Then the plan is installed on `misura-costi` and point 1 starts.
