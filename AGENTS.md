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
| What has been ruled about the specification, and what you owe | `RULINGS.md` |

## `docs/` is not yours to edit

Not to record what was built, not to reconcile a discrepancy, not to tidy. If
the code cannot satisfy a requirement, stop and say so. A specification edited
to match an implementation is no longer an independent account of the design.

One exception: a human may ask you to apply a change to
`docs/`. When that happens:

- The text or patch comes from the human. You transcribe it, you do not
  compose it, and you do not extend it to the places it "should" also touch.
- Something that ought to change and is not in what you were given is a
  finding, not a licence. Say so, and leave it.
- Prefer `git apply` over editing by hand, so that the diff cannot exceed the
  text you were given.
- Record the permission in `DECISIONS-OUTSIDE-SPEC.md`: who granted it, what
  it covered, and whether the applied diff matched the deposit exactly.

### The decider answers what this file sends to a human

`docs/` has one other writer: the **decider**, a separate role run by `/decide`
and holding the human's authority by delegation (`R-001` in `RULINGS.md`). It
answers `SPEC-QUESTIONS.md`, `REVIEW-REQUEST.md` and `PROMOTIONS.md`, audits
`DECISIONS-OUTSIDE-SPEC.md`, and edits `docs/`.

Nothing above changes for you. An implementer session never edits `docs/` and
never plays the decider; a decider session never writes code. A ruling binds you
as `docs/` does. Before writing a plan and before each point, read "Owed by the
implementer" in `RULINGS.md`, and name the `R-NNN` in the commit that acts on
one.

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

## Ask for a review at three moments, and only three

This mechanism records everything and asks for nothing, so a human opinion
arrives whenever one is offered — which on a long branch is after the work that
needed it was already built on. Write a request into `REVIEW-REQUEST.md`:

- **before a plan starts**, where a wrong criterion costs one conversation
  rather than every point built on it;
- **at a phase gate** from `docs/SIM-REQ.md` §18, the only moment anything looks
  at the whole rather than at one point;
- **when the work meets the specification** — a `SPEC-QUESTIONS.md` entry, or
  promotion candidates for `SIM-DEC`.

Six parts, the last of which is the reason the file exists: which moment, which
branch or commit, what changed in one line, the decisions taken outside the
specification in this block, the open questions about the specification, and
**what was not verified**. Everything but the last is discoverable by a reader
willing to look; what was not done leaves no trace and only the author knows it.

A request does not authorise continuing past a blocking question, and is never
written on the human's behalf: the reminder is that one is owed.

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

**Merge without squashing.** A commit that closes a plan point carries a
`Plan-point: <n>` trailer, and `/plan-status` resolves every closed point to its
commit by looking that trailer up. Squashing a branch collapses those trailers
into one message and destroys the lookup — not for the branch being merged, but
for the whole history, retroactively and silently. This is not a preference
about tidy history: it is the key the record is indexed by.

## Scope

Work is organised in phases with exit gates, listed in `docs/SIM-REQ.md`.
Phase 3 contains no domain logic: no demography, no prices, no trade, no
agents. Implementing a subsystem early looks helpful and removes the meaning
of the gate. If a task seems to need something from a later phase, that is a
signal to stop and ask, not to reach forward.

## When unsure

Stop and ask. A wrong guess written confidently into a codebase with 169
requirements costs more to find than to prevent.