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

# Current request — 2026-09-19, evening

Replaces the request of the same day, which was answered within hours: both its
specification questions are closed, its four promotion candidates are in
`SIM-DEC`, the branch it described is merged and its commits are pushed. Kept in
git; nothing in it is true of the repository any more.

**Moment:** three. The specification answered, and answering it opened one more
question. Written also as the handoff to whatever session comes next, since this
one built the mechanism and holds context no file does.

**Branch:** `main` at `64bcc7f`, pushed, CI green. No branches outstanding: the
three that existed are merged and deleted, locally and on the remote.

**What changed, in one line:** the specification moved — `DEC-081` to `DEC-084`
written, A-11 split into A-11 and A-11b, FR-X-02 given the subject form and the
tick clarification — and the repository was reconciled to it.

## Decisions taken outside the specification

`D-035` to `D-037` in `DECISIONS-OUTSIDE-SPEC.md`, all from the reconciliation:

- `D-035` — both questions closed, neither the way it was posed. `SQ-001`'s
  answer carried a semantics the question had not thought to ask: the tick
  coordinate is the one at which a trajectory is **generated**, not the one
  being observed, without which FR-X-03 does not hold.
- `D-036` — promoted entries are marked and kept, not removed. The pass empties
  the queue, never the record.
- `D-037` — why `SQ-003` was filed rather than read charitably.

Four earlier entries are now marked with what they became: `D-001` → `DEC-081`,
`D-002` → `DEC-082`, `D-007` → `DEC-083`, `D-006` → `DEC-084`. `D-004` is marked
superseded: it closed through the other channel, when A-11 split.

## Open questions about the specification

One, `SQ-003`, **not blocking**, and **already decided by you**.

`NFR-03` and `DEC-002` still read `entity_id` while FR-X-02 now writes the
subject form and cites `NFR-03` as its authority. Resolution A chosen: the two
definitions move. The exact replacement text for both lines is written into the
entry in `SPEC-QUESTIONS.md`, ready to paste.

**It is waiting on you** because `docs/` is not ours to edit. Two lines. Once
they are in, tell whoever is working and the entry gets marked closed.

## What was not verified

Unchanged from the last request except where noted, because nothing since has
tested any of it.

- **The review protocol has never run.** No point in three plans declared
  `Core: yes`. The two reviewers, the read-only agent type, the briefed-and-blind
  split — the part of this with the most moving parts still has no evidence
  behind it. Its first real test is the first point of phase 3's remaining work,
  which is all `core/Runtime/` and therefore all `Core: yes`.
- **Almost every check was satisfied by reading.** Of twenty-three closed points
  across three plans, two had a fully mechanical check. Three markers claimed
  `weak` and turned out merely shallow, which produced the `Check: shallow`
  distinction now in the design.
- **The skills have been run by their author, in one session.** `/plan-status`
  was dispatched once as a read-only agent and found three real defects, which
  is evidence the dispatch works; but a session holds the copy of a skill it
  loaded, not the file on disk, and these were edited repeatedly after loading.
  **A fresh session is the only thing that proves the versions that ship.**
- **One defect is known and unfixed:** the `Plan-point:` lookup is not qualified
  by plan, so on a branch carrying several plans it returns one commit per plan
  for the same point number. Disambiguating by commit order works and is not a
  guarantee. It needs its own plan point, and that point needs your approval.
- **`.claude/plans/` holds three archived plans that nobody but their author has
  read.** They are the record of what was intended; whether they are legible to
  someone who was not there is untested.

## What I need from you

1. The two lines of `SQ-003`, whenever convenient — or say the word and I apply
   them.
2. Approval for the plan point that qualifies the lookup.
3. The next real decision: the plan for the rest of phase 3. `Time/`, `State/`,
   `Data/`, `Chronicle/`, `Commands/` are empty, and the gate is 100k empty
   ticks with `AC-02` and `AC-03` green. That conversation is moment one, and it
   is where a request is owed next.
