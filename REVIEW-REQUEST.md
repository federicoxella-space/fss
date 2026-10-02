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

# Current request — 2026-10-01 — moment three, promotion candidates

Replaces the moment-two request of the same day, which is answered and kept in
git: `SQ-007` by `R-039`; the look at the whole taken in conversation, leaving
`SQ-008` and `D-106` to phase 4; **the gate of phase 3 confirmed by the user** —
"Confermo il gate di fase 3, procedi con il merge in main" — and the merge done
without squash.

**Moment:** three — the promotion pass has produced candidates for `SIM-DEC`, and
`SQ-008` is open.

**Branch:** `main`, after the merge of `fase-3-kernel`. The phase 3 plan is
archived at `.claude/plans/2026-09-20-fase-3-kernel.md`; no plan is live.

**What changed, in one line:** the phase 3 plan closed and archived, and the pass
over its register entries run.

## Decisions taken outside the specification

`D-107`: where the plan went and what the pass left out, and why. Not yet
audited; the watermark stands at `D-104`, so `D-105` to `D-107` are the next
audit.

## Open questions about the specification

**`SQ-008`, non-blocking.** A full state hash at every tick costs the whole state
every tick; the checks of `AC-02` take seven sequences of 100k ticks per test
step, and phase 4 makes the state large. The cost is estimated, not measured.
Three readings, none picked. The first phase 4 point that adds a table is where
it starts to bite.

## Promotion candidates

Three, in `PROMOTIONS.md`, pass of 2026-10-01: `C-5` from `D-077`, commands
apply at the start of the tick; `C-6` from `D-081` and `D-083`, only the rows in
use are state; `C-7` from `D-091`, a load refuses what no build could have
written. Near misses named there with their reasons: `D-073`, `D-090`, `D-106`.

## What was not verified

- **The pass read titles first.** Every entry from `D-044` to `D-106` was judged
  by its title and the ruling that audited it; `D-073`, `D-077`, `D-081`,
  `D-083`, `D-090` and `D-091` were read in full, with `R-029`. An entry whose
  title undersells a constraint would have been missed.
- **"Already in `docs/`" was checked by searching**, not by reading `SIM-DEC`
  and `SIM-STATE` whole: the rulings that changed `docs/` were listed and their
  lines grepped for the start of the tick, capacity and rows in use.
- **The drafts were written by the author of the code they describe.** Whether
  a candidate states the rule the code keeps or the rule the author meant is
  for the reader to check against `TickLoop`, `Chronicle` and `SaveFormat`.
- **The merge commit has not been through CI.** `main` is not pushed; its tree
  equals the branch's last commit, which carries only Markdown past `c645462`,
  green in CI run `36788322902`.

## What I need from you

1. ~~**The three candidates**, promoted, amended or rejected with a reason. The
   decider answers `PROMOTIONS.md` under `R-001`; numbering is yours or its.~~
   **Answered:** promoted as DEC-087 to DEC-089 by `R-040` to `R-042`, two amended.
2. ~~**`SQ-008`**, before or at the first phase 4 point that adds a table.~~
   **Answered:** closed by `R-043`, the full hash at every tick, on request.
3. ~~**The push of `main`.** A push is yours.~~
   **Answered:** main pushed 2026-10-02, CI run 36942531774 green.
4. ~~**Phase 4's moment one**, when you are ready to plan it: the question carried
   from moment two — whether the `Fails when:` lines are read by someone other
   than their author, and whether the agent workflow of `D-086` to `D-089` is
   built first.~~
   **Answered:** the workflow first, by `R-046`; the `Fails when:` lines read by
   the user at moment one and the reviewers at close, by `R-047`; moment one on a
   draft and the first point waiting for the user, by `R-048` and `R-049`. The
   next moment one is the process plan's.
