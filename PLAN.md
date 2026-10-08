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

*Outcome:* 2026-10-07. Closed, *criterion defective* (`R-005`, case 1): the
check ran PASS, the 9-row cost 3.50 to 3.80 µs and the ratio 5.33 to 5.49, and
both faults went red (linear ranking, ratio 2.85; the work removed, 0.10 µs),
but it reads only the 9-row cost and the ratio and cannot see what the five
passes compute. The missing check, beside it: the checksum of each cell's
outputs, at a fixed update count, against a plain reference implementation of
the five passes; not run here, point 2 introduces checksums against a reference
loop. The criterion is not rewritten. `D-121`. Agents (`R-067`): tester,
coder, code-reviewer, documenter and committer dispatched; the code-reviewer's
findings resolved in `D-121`, one rejected, one left open for the human. Not
run: the revisioners, `Core: no` held at step 5; `mutate.ps1`, `bench/` outside
its reach, so the orchestrator introduced the faults; no tester-written test,
the check is a run of `bench/`. This outcome does not claim `R-046`'s check.

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

*Outcome:* 2026-10-08. Closed, *criterion defective* (`R-005`, case 1): the
check ran PASS twice, all seven cells matching the reference loop over every
good and row, rationing binding, the ranking reaching the checksum, the
checksums identical across the two runs; the Fails-when fault (clearing loop to
`Goods - 1`) went red 7 of 7, as did `D-121`'s (a) and (c), and point 1's check
rerun on the changed devices was green every time (`R-101`, items 1 to 4). The
criterion under-covers the "Does": the bytes per settlement are in it and nothing
checks them. The missing check, beside it: the bytes per cell recomputed
independently from the array sizes and compared; not run here. The criterion is
not rewritten. `D-122`. Agents (`R-067`): tester, coder, code-reviewer,
documenter and committer dispatched; the code-reviewer's findings resolved in
`D-122`, one rejected, two left open for the human. Not run: the revisioners,
`Core: no` held at step 5; `mutate.ps1`, `bench/` outside its reach, so the
orchestrator introduced the faults; no tester-written test. This outcome does
not claim `R-046`'s check.

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
