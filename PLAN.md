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

*Esito:* 2026-09-17. Step 6 of `/plan-next` re-reads `PLAN.md` from disk and
tabulates five divergences, three of which stop the work without appending —
a changed criterion, an outcome line already present, a vanished point. Beyond
the point's statement: both appends are anchored on just-read text with an
exact-match edit, so the protection survives the re-read being skipped; and
step 7 forbids `git add -A`, a rule written against this author's habit in the
three preceding commits. This commit stages by name. Decisions: register,
section "The twelve gaps in the plan mechanism", point 3 — four entries.
Review: not run, `Core: no`, exemption verified against the diff.

## 4. A weak check is declared in the plan, with why and what replaces it

- **Does:** a check satisfiable by reading rather than running is marked
  `Check: weak` when the plan is written, with the reason and with what would
  verify it properly. Declared late, in an outcome line, it is an excuse;
  declared early it is an argument you can still lose.
- **Closed by:** the design states the rule and requires both halves — why it is
  weak and what replaces it; the three skills report the marker.
- **Check: weak**, and the self-reference is the honest version of the problem.
- **Core:** no — touches no file under `core/`.

*Esito:* 2026-09-19. The design states the rule, requires both halves and makes
looking for a mechanical check a step; all three skills report the marker.
The check was declared weak and ran as three greps — the second point in a row
where the marker was wrong, which makes it a pattern rather than a slip. The
register records what the marker was actually mislabelling: these criteria run
but prove little, which is *shallow*, not *weak*. That distinction is beyond
what this point states and is not written into the design; it is raised with the
human instead. Decisions: register, section "The twelve gaps in the plan
mechanism", point 4 — four entries. Review: not run, `Core: no`, exemption
verified against the diff.

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

*Esito:* 2026-09-19. The design defines both outcomes under separate headings;
`/plan-next` says what to do on a failure, refuses to run on an abandoned or
failure-stopped plan, and is forbidden to declare abandonment itself;
`/plan-explain` refuses to present the point after a failure as next;
`/plan-status` reports a failed point apart from the closed ones and an
abandonment before anything else. The marker said weak; the check ran as five
greps, so it was shallow in the sense point 13 introduces — the third
misapplication, and the last one written before the distinction existed. Two
rules beyond the point's statement: the failure commit must leave the tree
green, and abandonment is a human's word. Decisions: register, section "The
twelve gaps in the plan mechanism", point 5 — six entries. Review: not run,
`Core: no`, exemption verified against the diff.

## 6. The threshold below which no plan is needed

- **Does:** defines trivial by properties and not by size: no change in
  behaviour, no decision the specification does not already make, one commit. If
  a decision appears, the work was never trivial and stops for a plan. Goes in
  `AGENTS.md`, where the rule it qualifies lives.
- **Closed by:** `AGENTS.md` states the three properties and the stop condition.
- **Check: weak** — satisfied by reading. Replaced by the first argument about
  whether something was trivial.
- **Core:** no — touches no file under `core/`.

*Esito:* 2026-09-19. `AGENTS.md` carries the three properties and the stop
condition under "Work that needs no plan"; `/plan-next` points at the exemption
when asked to run with no plan. Two rules beyond the point's statement: several
trivial commits that together change behaviour are not several trivial changes,
and a trivial change leaves no register entry because the property that exempts
it is the property that empties the register. The permission to edit `AGENTS.md`
was read from this approved plan, which names the file — recorded, not assumed.
Marker says weak; it is shallow, and stays as written because point 13 is not
closed. Decisions: register, section "The twelve gaps in the plan mechanism",
point 6 — five entries. Review: not run, `Core: no`, exemption verified against
the diff.

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

*Esito:* 2026-09-19. The design defines the field in the header block, makes it
mandatory under `Kind: simulator`, subjects its identifiers to the confirmation
rule of point 1, and tabulates how it differs from `Serves`; all three skills
report it. Beyond the point's statement: it is confirmed once at the first
point rather than at every one, and a partial contribution is declared with
what remains. Untried — a process plan has no `Satisfies:` line and no `AC-` to
open. Marker says weak; shallow. Decisions: register, section "The twelve gaps
in the plan mechanism", point 7 — four entries. Review: not run, `Core: no`,
exemption verified against the diff.

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

*Esito:* 2026-09-19. `SPEC-QUESTIONS.md` exists with its rules; `AGENTS.md`
points at it from the section that used to say "stop and say so";
`/plan-next` sends objections there and fails the point on a blocking one. The
criterion said the weak check would be replaced by the first real entry, and it
was, in the same commit: `SQ-001` records a genuine inconsistency between
DEC-002 and FR-X-02 over the arity of the sampled-transient draw, found three
days ago and unrecorded until this file existed. Left open on purpose, with the
plausible reading written down but not chosen. Decisions: register, section
"The twelve gaps in the plan mechanism", point 8 — four entries. Review: not
run, `Core: no`, exemption verified against the diff.

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

*Esito:* 2026-09-19. The design states archival, its trigger and its two
reasons, plus the identifier scheme; `.claude/plans/` holds the previous plan;
all 26 register entries carry `D-001` to `D-026`, assigned by one `awk` pass.
The three skills no longer speak of deletion. This point's check was fully
mechanical — three counts and a listing — and carried no weak marker, the first
in this plan. Decisions: register, `D-027` — six entries, including that the
register's entries are not uniform in what they hold and were left that way.
Review: not run, `Core: no`, exemption verified against the diff.

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

## 13. `Check: shallow`, distinguished from `Check: weak`

Added on 2026-09-19, after the marker was misapplied on points 2 and 4 of this
plan and the misapplication turned out to be systematic. Added with the human's
approval; the criteria of points 1 to 12 are untouched, including their wrong
markers.

- **Does:** separates two defects that one marker was hiding. **Weak:** the
  check cannot be executed at all. **Shallow:** it executes and proves little,
  because what matters is not mechanically observable — a grep proves the text
  exists, never that the text is any good. The remedies differ, which is the
  reason the distinction earns its place: a weak check waits for a tool that
  does not exist, a shallow one waits for a better criterion, which its author
  could write today. So `Check: shallow` must say what a better criterion would
  look like, or why none is available, and writing a deeper criterion is to be
  attempted before the marker is reached for.
- **Closed by:** the design defines both markers, their different remedies and
  the attempt-first rule; the three skills report them as distinct.
- **Check: shallow** — greps prove the text is present, not that the
  distinction is well drawn. A better criterion would be an audit of the next
  plan's markers by someone who did not write them.
- **Core:** no — touches no file under `core/`.

The markers already written in this plan stay wrong. They were frozen when the
plan was written, the register records the misclassification, and correcting
them would be the move this whole mechanism exists to prevent, performed for the
best of reasons.
