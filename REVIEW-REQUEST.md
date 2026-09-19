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

# Current request — 2026-09-19

**Moment:** all three at once, which is itself a finding. The mechanism was
built during a single absence, so the first request covers a branch that has
already run three plans, closed a phase gate's worth of tooling, and opened two
questions against the specification. Had the file existed on 2026-09-17, this
would have been four shorter requests.

**Branch:** `plan-e-skill`, at the tip. The remote holds it at `046751f`, the
closing commit of the *first* plan: **everything after that is unpushed**, which
is two whole plans, and there is no pull request.

**What changed, in one line:** a plan mechanism — `PLAN.md`, three skills, a
decisions register, a specification-question channel and a promotion pass —
built, reviewed against thirteen gaps you named, and corrected.

## Decisions taken outside the specification

`D-011` through `D-032` in `DECISIONS-OUTSIDE-SPEC.md`. Twenty-two entries, all
process. The ones that would change how you work, rather than how the tooling
works:

- `D-020` — `Core:` now defaults to **yes**, and skipping review costs a reason
  a diff can falsify. A judgement about size is not a reason.
- `D-024` — trivial work is exempt from planning, defined by three properties
  and not by how small it feels.
- `D-031` — no squash merge, ever, on this repository. The `Plan-point:`
  trailer is the key the record is indexed by.

## Open questions about the specification

Both in `SPEC-QUESTIONS.md`, **neither blocking**:

- `SQ-001` — DEC-002 and `SIM-STATE:25` specify a five-coordinate draw;
  FR-X-02 and `SIM-STATE:123` specify a three-coordinate one, and FR-X-02 is a
  requirement. Which is normative decides how sampled transients are keyed, and
  the implementation cannot choose without fixing every generated world to an
  unrecorded decision.
- `SQ-002` — A-11 is listed among the invariants asserted **every tick**; the
  overflow guard is `[Conditional("DEBUG")]` and a release build does not check
  it. Either the guard is wrong, or A-11 means something narrower than its
  heading.

And four promotion candidates in `PROMOTIONS.md`, `C-1` to `C-4`, drafted as
`SIM-DEC` entries for you to write or reject. They are the architecture from the
`Hash64` work that currently lives only in the register.

## What was not verified

- **The review protocol has never run.** No point in three plans declared
  `Core: yes`, so the two reviewers — the part of this with the most moving
  parts — has no evidence behind it. Its first real test is the first core point
  of phase 4.
- **Almost every check was satisfied by reading.** Of twenty-one closed points
  across the last two plans, one had a fully mechanical check. Three markers
  claimed `weak` and turned out merely shallow, which is what produced the
  distinction now in the design.
- **Point 12's second clause is unmet.** It required the no-squash note in this
  branch's pull request; there is no pull request.
- **The skills were only ever run by their author**, in the session that wrote
  them, and a session holds the copy of a skill it loaded rather than the file
  on disk. They are asserted for the versions that ship, not proved.
- **One defect is known and unfixed:** the `Plan-point:` lookup is not qualified
  by plan, so on a branch carrying several plans it returns one commit per plan
  for the same point number. Disambiguating by commit order works and is not a
  guarantee. It needs its own point, and that point needs your approval.

## What I need from you

1. Whether to push and open the pull request. I have not: seventeen commits and
   an outward-facing action nobody asked for.
2. `SQ-001` and `SQ-002`, in your own time — neither blocks, and both decide
   code that is not written yet.
3. Approval for the plan point that fixes the lookup.
4. Whether the four promotion candidates go into `SIM-DEC`, and with which
   numbers.
