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

# Current request — 2026-09-20 — **answered, one item open**

A reviewer answered every item below on 2026-09-20 and left one thing that was
not asked for, now item 5. The request stays here rather than being replaced:
there is no new moment. The next one is **moment two**, the phase 3 gate, and
`/plan-status` will say so when the plan closes.

Replaces the request of 2026-09-19, twice amended. Everything it asked for is
closed but one item, carried forward below; it described moment three, and the
situation has moved to moment one. Kept in git.

**Moment:** one — with a caveat this request has to state, because it is the
first time that moment has come up. **No plan exists to read.** `PLAN.md` is
absent, the lookup plan closed and was archived at `4ed3a94`, and the next plan
is the rest of phase 3. The argument for interrupting here is that a plan is
short against a branch, and that argument assumes a plan on the page. There is
none, so what is offered instead is the criteria before they are written down.
If you would rather see a draft plan first, this request was written one step
early — see item 3 below.

**Amended the same day:** you asked for the plan, which answers item 3 in
practice. `PLAN.md` now holds nine points for the rest of phase 3, and the
criteria this request offered for review are on the page rather than in
prose. The rest of the request stands, and one item was added to it — the §20
confirmation below, which the plan cannot take for itself.

**Branch:** `main`. The code is unchanged since `4ed3a94`, which is pushed and
CI green; this request, `PLAN.md` and `SQ-004` sit in the commit above it, which
is not pushed. No branches outstanding — `fase-3-kernel`, which the plan names,
is not cut until the blocking item below is answered.

**What changed, in one line:** nothing was built — the lookup plan closed and was
archived, and the repository has been idle since, which is why this is moment
one and not moment three.

## Decisions taken outside the specification

`D-038` to `D-043`. Of these `D-040` and `D-043` were named in the amendments to
the last request; `D-038`, `D-039`, `D-041` and `D-042` have never been in front
of you.

- `D-038` — `SQ-003` decided, and the `docs/` edit left to a human.
- `D-039` and `D-041` — when a request is replaced and when it is amended. The
  pair is the rule this request has just applied to itself.
- `D-040` — applying `SQ-003` overran the deposited text by one line.
- `D-042` — the point lookup is bounded by the plan's commit range rather than by
  a new trailer. It rests on plans being sequential, which nothing enforces.
- `D-043` — a check that printed a verdict without having run. Prose inside a
  register entry passes through no check at all.

## Open questions about the specification

**`SQ-004`, non-blocking.** Raised the same day, writing the plan: §19 says
`P-17` and `P-01` close in phase 3, and both want the cost of a settlement
update, which a phase with no domain logic and a gate of empty ticks cannot
measure. Three readings are set out in the entry; none is picked. Nothing in the
plan waits on it.

`SQ-003` closed on 2026-09-20 and was applied to `docs/` by `7decada`.

## What was not verified

- **The review protocol has never run.** Unchanged, and now imminent: no point in
  four plans declared `Core: yes`, and every point of the next plan is in
  `core/Runtime/` and therefore `Core: yes`. Two reviewers, briefed and blind, a
  read-only agent type — the part of this with the most moving parts, still with
  nothing behind it.
- **Three sources give three different counts of the same thing.** The last
  request said twenty-three closed points across three plans; the lookup point's
  outcome says twenty-one; counting `*Esito:*` lines today gives twenty in the
  three plans archived then, and twenty-one including the lookup's own. The
  likely reading is that the lookup counted its own point, which was archived in
  the same commit — but that is a guess, and it is exactly the kind of claim
  `D-043` says enters a register unchecked. Nothing depends on the number. It is
  reported, not picked.
- **Of the twenty-one closed points, thirteen carried a marker** — ten `weak`,
  three `shallow` — and eight carried none. The eight are not eight mechanical
  checks: five predate the convention, which the first plan was written before;
  two are in the twelve-gaps plan; one is the lookup point, the only point that
  declared no marker with the convention available and a check that runs.
- **`/plan-status` has now been run by a fresh session** — this one, loading the
  file from disk and dispatching the read-only agent. That is the first evidence
  for the version that ships rather than the version its author held in memory.
  `/plan-next` and `/plan-explain` still have none.
- **Four archived plans that nobody but their author has read.** Whether they are
  legible to someone who was not there is untested, and the phase-3 plan is the
  first that a different session will have to execute.
- ~~**The phase-3 criteria do not exist yet.**~~ **Attempted 2026-09-20.** The
  gate does decompose into points that cite `docs/` — nine of them — and the
  attempt turned up two things reading had not: the §20 confirmation, which is
  now item 4 below, and `SQ-004`. What remains unverified is the decomposition
  itself: every point's `Serves` was read out of `docs/` by the same session
  that wrote the point, so a citation that does not support what the point
  claims would not have been noticed here. Step 1 of `/plan-next` opens each one
  independently, and that is the first check on it.
- **No point of this plan has been costed.** Nine points is a guess at the size
  of the rest of phase 3, not a measurement, and the serialiser is the one most
  likely to be two points wearing one number.

## What I need from you

1. ~~**Carried forward, unanswered:** a look at the three lines `7decada`
   changed in `docs/`.~~ **Approved 2026-09-20**, all three. The reviewer's
   account is that the inconsistency was theirs — `DEC-081` was promoted without
   its ascendants following — and that the agent found and fixed it correctly.
   **The principle in `D-040` stands and was restated:** the fix is not that the
   agent hold back the extra line, it is that the deposited text be complete.
   The rule is unchanged — **the agent deposits, a human applies.**
2. ~~**The criteria for the rest of phase 3.**~~ **Read 2026-09-20**, with two
   findings, both recorded in the register and neither changing a criterion:
   point 7 is the one likely to be two points wearing one number, and if it
   swells in flight it is **split, not widened**; and the first run of the
   reviewer protocol wants watching, **particularly that the blind reviewer does
   not see the briefed one's output.**
3. ~~**Whether moment one is served by a request like this one, or wants a draft
   plan attached.**~~ **Answered in practice** on 2026-09-20: you asked for the
   plan. Recorded as the precedent, not as a rule — the next moment one may want
   the other shape, and if so it should say why.
4. **The confirmation `SIM-REQ` §20 asks for, and asks for by name.** Its last
   line is "Confirm before Phase 3. A wrong guess costs a rewrite of the
   serialiser," and five of its rows still read `Proposed`: target framework,
   maximum language level, ahead-of-time support, allowed base class library
   surface. The code has been built on `netstandard2.1` and C# 9 since before
   this request — the guess is already made, it has simply never been confirmed.
   **This one blocks point 7**, the serialiser, and nothing else in the plan;
   points 1 to 6 and 8 to 9 can run while it is open.

   **Confirmed 2026-09-20**, applied to `docs/` by `0114ad2`: `netstandard2.1`,
   C# 9, ahead-of-time supported, base class library surface limited to
   `netstandard2.1` with no third-party packages. All four rows read `Decided`
   and §20's closing line now reads `Confirmed 2026-09-20`. Point 7 is unblocked
   with its criterion untouched.
5. **Open, and not raised by this request — the failure path for a criterion.**
   The reviewer's words: the field asked for is there and works, but nothing
   says what happens when a criterion turns out to be wrong *in flight*. The
   mechanism covers the neighbouring case — `/plan-next` closes a point whose
   check fails as `FALLITO` and stops the plan, from point 5 of the twelve-gaps
   plan — and it covers the rule — "a criterion that turns out to be wrong is
   reported and recorded, never rewritten". **What it does not have is the path
   between them:** who is told, where the wrong criterion is written down, and
   whether the plan stops or the point closes failed against a criterion nobody
   now believes.

   It was gap 8 of the reviewer's own list and is still open. It cannot be
   closed inside this plan: the points are frozen, and a second `PLAN.md` for
   process work would be two plans live at once, which `D-042` says is the first
   thing that breaks the point lookup. **So it waits for the phase 3 plan to
   close, and this is the note that it is owed** — nine points all `Core: yes`
   is the run where it will be wanted.
