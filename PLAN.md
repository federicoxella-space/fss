# Plan — the plan mechanism and its three skills

**Exit condition:** `PLAN.md`, the three skills and the rule in `AGENTS.md` exist
and agree with `.claude/design/2026-09-17-plan-e-skill.md`; `/plan-explain` and
`/plan-status` answer correctly about this plan itself.

**Phase:** none. This is process work, and `docs/` says nothing about it —
`SIM-REQ` §18 plans the simulator, not how the simulator gets built. Every point
below therefore declares `docs/` silent, which is one decision recorded once in
`DECISIONS-OUTSIDE-SPEC.md` rather than five times here.

**Branch:** `plan-e-skill`

---

## 1. The rule in `AGENTS.md`

- **Does:** adds a section holding the four standing rules — plan written first,
  criteria never rewritten to match delivery, a point is one commit with a
  runnable check, a point touching `core/Runtime/` passes two reviewers. The
  procedure stays in the skills; only the rule goes here.
- **Serves:** nothing in `docs/`. Permission to edit `AGENTS.md` was given
  explicitly for this change.
- **Closed by:** the section exists, states four rules, and duplicates no step of
  the procedure that the skills own.
- **Core:** no

*Esito:* 2026-09-17. Section "Plan the development before starting it", four
rules, procedure delegated to the three skills by name. Decisions: register,
section 2026-09-17, point 1 — two entries, one of them the fact that an agent
edited the standing instructions under a one-off permission. Review: not run,
`Core: no`.

## 2. `/plan-explain`

- **Does:** a read-only skill that names the next open point, what it requires,
  what it cites, and what will close it.
- **Serves:** nothing in `docs/`.
- **Closed by:** invoked against this file it names point 3 — the first point
  with no `Esito:` at the time it runs — and does not modify anything.
- **Core:** no

## 3. `/plan-status`

- **Does:** a read-only skill reporting closed points with their commits,
  decisions and findings; the current point; what remains; the exit condition.
- **Serves:** nothing in `docs/`.
- **Closed by:** invoked against this file it reports points 1 and 2 closed with
  their commits, point 4 as next, and the exit condition above.
- **Core:** no

## 4. `/plan-next` and the reviewer briefs

- **Does:** the skill that runs the life of a point — the eight steps of the
  design — plus the two briefs it dispatches, one informed and one blind, with
  their shared rules: read the specification, run the build and tests, verify
  numerical claims, never edit code, and report nothing when there is nothing.
- **Serves:** nothing in `docs/`.
- **Closed by:** a weaker check than the others, stated plainly rather than
  dressed up: the skill enumerates the eight steps and both briefs carry the four
  shared rules. A skill cannot run itself, so the real verification is the first
  development that uses it, and that is outside this plan.
- **Core:** no
