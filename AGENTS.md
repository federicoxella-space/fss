# Working in this repository

## What this is

A headless socio-economic simulator. It has a written specification in `docs/`
that predates the code and outranks it. When code and specification disagree,
the specification is right until a human decides otherwise.

## Before any structural decision

Read the relevant document. Do not infer the design from the code.

| Question | Document |
|---|---|
| What is required, and what tests it | `docs/SIM-REQ.md` |
| Why something is the way it is | `docs/SIM-DEC.md` |
| What the player must notice | `docs/SIM-OBS.md` |
| Which feedback loops exist and what brakes them | `docs/SIM-LOOPS.md` |
| What lives in state, and what is asserted every tick | `docs/SIM-STATE.md` |
| What a settlement update does, in order | `docs/SIM-ECON.md` |

## Do not edit `docs/`

Not to record what you built, not to fix a discrepancy, not to tidy. If the
code cannot satisfy a requirement, stop and say so. Changing the specification
to match the implementation destroys the only independent account of the design.

## The build enforces most of the rules

`BannedSymbols.txt` blocks floating point, `System.Random`, wall clocks,
hash-ordered collections, reflection, threading, console output, and file
access inside the core. When an analyzer stops you, the rule is right and the
code is wrong. Do not suppress with `#pragma` without asking; a suppression is
a decision, and decisions belong to a human.

## Rules the build cannot catch

- Every rate applied to a small integer needs a remainder accumulator, or
  small stocks never change while large ones do. This is a codebase-wide rule,
  not a spoilage detail. See `docs/SIM-ECON.md`.
- Nothing in the core may depend on iteration order that is not derived from
  integer ids.
- Anything that influences a future tick belongs in `WorldState` and in the
  state hash. A field outside the hash is a determinism hole.
- Systems mutate state. Nothing else does.

## Structure

State lives in one place as parallel arrays. Folders named after subsystems
would suggest an isolation that does not exist. `Runtime/Systems/` holds one
file per phase of the settlement update, in the order given by `SIM-ECON`, and
those files are the only ones that write to state.

## Tests carry the number of what they verify

`AC02_DeterminismAcrossRuns`, `AC20_CurrencyConservation`, and so on. The
acceptance criteria are numbered in `docs/SIM-REQ.md`. A test that verifies
nothing in that list needs a reason to exist.

## Commits

Every commit that changes behaviour names the requirement or decision it
implements: `FR-C-04`, `DEC-002`. If no identifier applies, the work is either
out of scope or needs a decision first. Say so instead of inventing one.

## Scope

Work is organised in phases with exit gates, listed in `docs/SIM-REQ.md`.
Phase 3 contains no domain logic: no demography, no prices, no trade, no
agents. Implementing a subsystem early looks helpful and removes the meaning
of the gate. If a task seems to need something from a later phase, that is a
signal to stop and ask, not to reach forward.

## When unsure

Stop and ask. A wrong guess written confidently into a codebase with 169
requirements costs more to find than to prevent.