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

# Current request — 2026-10-01 — moment two, the phase 3 gate

Replaces the request of 2026-09-20, which was moment one. All five of its items
are closed, the last by `R-009`; it is kept in git.

**Moment:** two — the gate of phase 3, `docs/SIM-REQ.md` §18: "100k empty ticks;
AC-02, AC-03 green".

**Branch:** `fase-3-kernel`, 56 commits above `main` at `4ed3a94`. The code as it
stands is `c645462`, pushed, and CI run `36788322902` is green on it in every
step. The commits above it touch only Markdown and are not pushed: the *Riserva
sciolta* lines, `SQ-007`, and this request.

**What changed, in one line:** the headless kernel — calendar, `WorldState`,
state hash, tick loop, command queue, chronicle, serialiser, the `sim` runner,
the gate in CI and its pinned sequences — in ten points, all closed, with the
three reserves discharged.

## The gate, clause by clause

| Clause | Evidence | State |
|---|---|---|
| 100k empty ticks | CI step "Gate, 100k empty ticks in two runs": two processes of `sim`, output compared byte for byte | green in CI |
| `AC-02`, across runs | `AC02_DeterminismAcrossRuns`, empty and with commands, three controls | green in CI, three steps |
| `AC-02`, across builds and machines | `AC02_TheGateSequencesArePinned`: two digests taken on the development machine, reached again on the runner in Release and in Debug (`R-034`, `R-036`) | green in CI, three steps |
| `AC-03` | `AC03_SaveRoundTrip` | green in CI, three steps |
| "driven by the command-line harness alone" (§17, repeated in the plan's exit condition) | only the empty 100k run goes through `sim`; the rest goes through `dotnet test` | **`SQ-007`, open** |

**So the gate is not declared.** Everything the gate names is green; whether it
was run the way §17 requires is a question about the specification, and it is
not mine to answer.

## Decisions taken outside the specification

`D-044` to `D-104`, all ratified by the decider, watermark at `D-104`. None
since the last request has been put to you except through the rulings you asked
for.

- `D-044` to `D-055` — writing the plan, moment one, `SQ-004`, the §20
  precondition.
- `D-056` to `D-085`, `D-090` to `D-104` — points 1 to 10, entries by point.
- `D-086` to `D-089` — the agent workflow designed with you, to be built after
  phase 3 (`R-028`).

**The pattern worth looking at as a whole.** Six of the ten criteria turned out
defective once the code existed: `D-059` (point 1 under-covers its own "Does"),
`D-065` (point 2 passes against an empty walker), `D-070` (point 3 passes against
an empty enumeration), `D-075` (point 4 passes a 365-tick year), `D-084` (point 6
cites greps that name nothing), `D-095` (point 7 passes a serialiser with no
version number). Each was caught by introducing a fault, each check was left as
written, and the missing check sits beside it. All six were written by one
session in one sitting, before `R-017` and `R-020` made a criterion name its
fault. Point 10 is the only one written under that rule, and it is not a sample.

**Rulings still open, owed later:** `R-007` (how prior history is dated) and
`R-025` (whether an `EntityId` names its table), both until a later phase needs
them; `R-037`, the three-band step, by the first point that builds cohort rows;
`R-032`, the chronicle's CSV, by phase 6.

## Open questions about the specification

**`SQ-007`, blocking the gate's declaration.** §17 wants every criterion driven
by the harness alone; `AC-03` and most of `AC-02` are driven by the suite. AC-19
and DEC-034 name the suite as its own channel, which argues that it counts. Two
readings are set out in the entry; none is picked.

`SQ-001` to `SQ-006` are closed.

## What was not verified

- **"Machines" means two.** The development machine and one GitHub runner image,
  `windows-2025-vs2026`, both Windows x64. Whether their processors or JIT tiering
  differ in any way that could expose a divergence was not measured (`R-034`).
- **The empty pin says little.** In an empty world only `Tick` moves, so the
  pinned empty sequence is close to the hash function applied to one field
  (`R-036`). The pin with content is the run with commands.
- **The run with commands uses a test-only kind** through the internal tick
  loop. Nothing public submits a command yet, so `Simulation`'s command path is
  not under any pin (`D-100`). Whether the stand-in's fold into `CurrencyTotal`
  can cancel over the run was argued against, not proven (`R-036`).
- **The runner's output is not compared with the pin.** The suite's empty run and
  `sim` both call `Simulation.Advance(1)` and read the hash, so they should give
  the same sequence. That was checked by reading, not by comparing bytes.
- **Three green test steps are two configurations.** "Tests, three wealth bands"
  sets a variable nothing reads, and repeats the first step (`D-104`, `R-037`).
- **Reviewers ran on points 1 to 7 only.** Points 8 to 10 declared `Core: no`,
  and the diff held them to it. Whether the blind reviewer ever saw the briefed
  one's output, the concern you raised at moment one, was not checked by this
  request.
- **Points 1 to 9 predate `Fails when:`.** Their faults were chosen after the
  code, by its author. Only point 10's were named first.
- **No one but the author ran the mutations.** The decider's audits `R-035` and
  `R-038` ran some of the tests and took the mutation results as written.
- **The chronicle is shallow by declaration.** Its shape is tested; no real
  event has filled it (point 6).
- **`P-01` and `P-17` are unmeasured**, moved to phase 4 by `SQ-004`. The only
  figure is the floor under them: 0.000120 ms per empty tick on the runner,
  0.00012–0.00013 locally, over 10M ticks net of start-up.
- **I found no record of a human reading the phase 3 code.** All review in the
  repository is by agents and by the decider, which is an agent holding your
  authority by delegation.

## What I need from you

1. ~~**`SQ-007`: which reading of §17.** This blocks the gate. If the suite counts,
   the gate is met as it stands. If it does not, `AC-03` needs a round-trip mode
   in `sim`, and the commanded half of `AC-02` waits for the first command kind,
   which phase 3 cannot supply.~~ **Answered by `R-039`: the suite counts.**
2. **A look at the whole.** Especially the six defective criteria: whether the
   `Fails when:` rule is enough, or whether phase 4's plan should come to moment
   one with its faults read by someone other than their author.
   **Taken 2026-10-01, in conversation.** Two findings left to phase 4 at your
   request: the cost of a full state hash at every tick, `SQ-008`, non-blocking;
   and a refused command stopping the tick halfway, `D-106`. The question on the
   faults, and on building the agent workflow of `D-086` to `D-089` first, is
   carried to phase 4's moment one.
3. **The gate, declared or not**, once item 1 is answered. That is yours, or the
   decider's under `R-001`; the plan cannot declare it for itself.
4. **The merge into `main`, without squashing**, after the gate. The
   `Plan-point:` trailers are how `/plan-status` finds each point, and squashing
   destroys them for the whole history.
