# Plan — the twelve gaps in the plan mechanism

**Kind:** process
**Exit condition:** every item of the review of 2026-09-17 is answered in
`.claude/design/2026-09-17-plan-e-skill.md`, the three skills, `AGENTS.md` and
`SPEC-QUESTIONS.md`; the previous development's plan is archived; `/plan-status`
runs as a read-only agent and reports this plan correctly.

**Phase:** none, process work.
**Branch:** `plan-e-skill`, continuing after the plan of the same name closed.

---

This plan uses two conventions it also introduces: `Check: weak`, declared at
writing time, and `Core:` requiring a written reason to be `no`. Using them from
the start is the only way to find out whether they are worth having. Most points
here carry a weak check, and that is the point of declaring it: a plan made of
documentation cannot verify itself by running anything, and pretending otherwise
was the habit this convention exists to break.

## 1. The reviewers receive the frozen criterion, and citations get opened

- **Does:** adds the point's check to what both reviewers receive, so that
  someone finally verifies whether the code satisfies the criterion it was
  written against. And makes step 1 of `/plan-next` open the cited requirement
  in `docs/` and confirm it says what the point claims, or stop.
- **Closed by:** `reviewers.md` lists the criterion among what both receive;
  step 1 of `/plan-next` requires opening the citation.
- **Check: weak** — satisfied by reading. Replaced by the first `Core: yes`
  point, where a reviewer either uses the criterion or does not.
- **Core:** no — touches no file under `core/`.

*Esito:* 2026-09-17. `reviewers.md` lists the frozen criterion among what both
reviewers receive, quoted verbatim, and asks them first whether the code
satisfies the declared check. Step 1 of `/plan-next` requires opening every
cited identifier in `docs/` and splits a failed citation into a plan defect and
a specification defect, stopping on either. Check weak as declared, and it got
no exercise here: a process plan cites nothing. Decisions: register, section
"The twelve gaps in the plan mechanism", point 1 — four entries. Review: not
run, `Core: no`.

## 2. `Core:` defaults to yes, and a downgrade needs a written reason

- **Does:** the plan format declares `Core: yes` by default; writing `no`
  requires a reason on the same line, in the plan, before the work starts. A
  point that cannot say why it is exempt is not exempt.
- **Closed by:** the design and the three skills state the default and the
  requirement; this plan's own points carry their reasons.
- **Check: weak** — satisfied by reading. Replaced by the first plan written by
  someone other than its author.
- **Core:** no — touches no file under `core/`.

*Esito:* 2026-09-17. The design and all three skills state the default and the
requirement; all twelve points of this plan carry a reason. The check was
declared weak and was not: every clause was verifiable mechanically, including
`git diff --name-only HEAD -- core/`, which ran and confirmed this point's own
exemption holds. The marker stays as written — a frozen criterion that turns out
pessimistic is recorded, not corrected. Two rules beyond the point's statement:
the reason must be falsifiable by a diff, and a false `no` runs the reviewers
anyway. Decisions: register, section "The twelve gaps in the plan mechanism",
point 2 — five entries. Review: not run, `Core: no`, exemption verified.

## 3. `/plan-next` re-reads `PLAN.md` before appending the outcome

- **Does:** step 6 re-reads the plan from disk before appending, since the human
  may have edited it during the work and an append onto a remembered version
  silently discards that edit.
- **Closed by:** step 6 requires the re-read and says what to do when the point
  has changed underneath: stop, do not append.
- **Check: weak** — satisfied by reading.
- **Core:** no — touches no file under `core/`.

## 4. A weak check is declared in the plan, with why and what replaces it

- **Does:** a check satisfiable by reading rather than running is marked
  `Check: weak` when the plan is written, with the reason and with what would
  verify it properly. Declared late, in an outcome line, it is an excuse;
  declared early it is an argument you can still lose.
- **Closed by:** the design states the rule and requires both halves — why it is
  weak and what replaces it; the three skills report the marker.
- **Check: weak**, and the self-reference is the honest version of the problem.
- **Core:** no — touches no file under `core/`.

## 5. A failed point and an abandoned plan are two different outcomes

- **Does:** a point whose check does not pass gets an outcome line saying so,
  with what failed, and the plan stops rather than continuing past it. A plan
  whose direction changed is closed by an abandonment declaration in its header,
  with the reason; its register entries stay valid, because they describe code
  that existed. Both go to the archive like any closed plan.
- **Closed by:** the design defines both outcomes and distinguishes them;
  `/plan-next` says what to do in each; `/plan-status` reports them as distinct
  from a closed point.
- **Check: weak** — satisfied by reading. Replaced by the first failure, which
  is the one case nobody can schedule.
- **Core:** no — touches no file under `core/`.

## 6. The threshold below which no plan is needed

- **Does:** defines trivial by properties and not by size: no change in
  behaviour, no decision the specification does not already make, one commit. If
  a decision appears, the work was never trivial and stops for a plan. Goes in
  `AGENTS.md`, where the rule it qualifies lives.
- **Closed by:** `AGENTS.md` states the three properties and the stop condition.
- **Check: weak** — satisfied by reading. Replaced by the first argument about
  whether something was trivial.
- **Core:** no — touches no file under `core/`.

## 7. `Satisfies:` in simulator plans, verified like any other citation

- **Does:** a `Kind: simulator` plan declares in its header which acceptance
  criteria the development contributes to, and the identifiers are opened and
  confirmed like any citation under point 1. A field filled with plausible
  numbers is worse than an absent one: it answers the coverage question instead
  of leaving a visible hole.
- **Closed by:** the design defines the field, makes it mandatory under
  `Kind: simulator`, and subjects it to the confirmation rule; the three skills
  report it.
- **Check: weak** — satisfied by reading. Replaced by the first simulator plan.
- **Core:** no — touches no file under `core/`.

## 8. `SPEC-QUESTIONS.md`

- **Does:** the channel for the one thing that has nowhere to go today — the
  discovery that a requirement is contradictory, unachievable, or wrong. `docs/`
  cannot receive it and the register is for decisions, so it ends in a
  conversation and leaves the repository. Each entry cites the requirement,
  states the problem, and **declares whether it blocks**, so that the file cannot
  become where objections are filed instead of acted on.
- **Closed by:** the file exists with its rules and an honest empty state;
  `AGENTS.md` points at it; `/plan-next` says that an objection to `docs/` goes
  there and, when it blocks, stops the point.
- **Check: weak** — satisfied by reading. Replaced by the first real entry.
- **Core:** no — touches no file under `core/`.

## 9. Closed plans are archived, and register entries get stable identifiers

- **Does:** replaces the deletion rule with archival to `.claude/plans/`,
  triggered when a plan closes rather than when a branch merges — a branch can
  hold two plans in sequence, as this one does. And gives register entries
  stable identifiers, without which a promotion cannot be recorded against the
  entry it came from.
- **Closed by:** the design states archival and the identifier scheme; the
  previous plan is in `.claude/plans/`; the register's existing entries carry
  identifiers.
- **Core:** no — touches no file under `core/`.

## 10. The promotion ritual

- **Does:** at archival, the architectural entries of the register are listed as
  promotion candidates, each with the `DEC-` text proposed for it. **A human
  writes `docs/`.** The proposal is mine and the writing is not, or promotion
  becomes the back door into the specification that the prohibition exists to
  close. Promoted entries are marked in the register with the identifier they
  became.
- **Closed by:** the design defines the ritual, its output, and who writes what;
  the candidate list for the development just closed exists.
- **Check: weak** — satisfied by reading, except for the candidate list, which
  is real work and either names the right entries or does not.
- **Core:** no — touches no file under `core/`.

## 11. `/plan-explain` and `/plan-status` run as read-only agents

- **Does:** both dispatch to an agent type without `Edit` and `Write`, so that
  the promise not to write becomes an inability to write — the principle already
  applied to the reviewers. The risk it closes is not malice, it is the drift
  into fixing something while passing through.
- **Closed by:** both skills require the read-only dispatch and say why;
  `/plan-status` invoked this way reports this plan correctly and the tree is
  unchanged afterwards. The relaying of its report stays mine, and both skills
  say so.
- **Core:** no — touches no file under `core/`.

## 12. No squash merge, where someone will read it

- **Does:** a squash merge destroys the `Plan-point:` trailers and with them
  `/plan-status` on the whole history, retroactively. Not a matter of style: the
  lookup key disappears. Goes in `AGENTS.md` and in the body of every pull
  request that closes a plan.
- **Closed by:** `AGENTS.md` states it with that reason; the pull request for
  this branch carries it.
- **Check: weak** — satisfied by reading, until someone squashes.
- **Core:** no — touches no file under `core/`.
