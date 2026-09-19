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

**Say so in `SPEC-QUESTIONS.md`**, not only in conversation. Every objection to
the specification goes there — a requirement that contradicts another, one that
cannot be built, one that is simply wrong — and none of them is resolved by
whoever found it. A conversation is outside the repository and gone by the next
session, and this is the only channel that runs back towards `docs/`.

## Plan the development before starting it

Work starts from a written `PLAN.md` at the repository root, describing the
development in progress and nothing else. Four rules govern it; the procedure
belongs to `/plan-next`, `/plan-explain` and `/plan-status`, and the reasoning to
`.claude/design/2026-09-17-plan-e-skill.md`.

- **The plan is written before the work.** Its points cite `docs/`, or say
  plainly that `docs/` does not cover them. A plan that invents requirements is a
  specification edited to match its implementation, one level down.
- **Criteria are frozen when written.** A criterion that turns out to be wrong is
  reported and recorded, never rewritten to match what was built.
- **A point is one commit with a runnable check.** The commit carries the record
  with it: the register entry and the plan's own outcome line are written first
  and committed together with the code.
- **A point touching `core/Runtime/` passes two reviewers**, one briefed with the
  author's doubts and one blind to them. Their findings reach
  `DECISIONS-OUTSIDE-SPEC.md` even when rejected, with the reason.

### Work that needs no plan

Trivial work is exempt. Trivial is defined by properties, not by how small the
change feels, and all three must hold:

- **No change in behaviour.** A typo, a comment, a reworded message, a
  formatting fix. Nothing a test could notice.
- **No decision the specification does not already make.** If you are choosing
  between two defensible answers, you are deciding.
- **One commit.**

**If a decision appears while you work, the change was never trivial.** Stop,
and write the plan. This is the one rule here that has to hold even when it is
inconvenient: a rule bypassed once is a rule that can be bypassed, and the
exemption is the obvious place to bypass it from.

Two consequences worth stating. A trivial change leaves no register entry,
because by definition there was no decision to record — the exemption and the
rule above turn on the same property. And several trivial commits that together
change behaviour are not several trivial changes: the exemption applies to the
work, not to the slices it is cut into.

## Close every task with the decisions it required

Before reporting a task finished, write the choices it forced that the
specification does not make into `DECISIONS-OUTSIDE-SPEC.md`, under a heading
naming the task. Say what was decided, what `docs/` does or does not say about
it, and what would overturn it. A task that genuinely decided nothing says so in
one line.

This is where a decision goes when `docs/` is silent, since `docs/` is not yours
to edit. A choice left in the diff alone stops being a decision and becomes an
assumption in someone else's code, found years later by whoever it breaks.

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