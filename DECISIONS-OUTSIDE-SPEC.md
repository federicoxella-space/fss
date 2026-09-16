# Decisions taken outside the specification

`docs/` does not answer every question a task raises, and it is not ours to
edit. This file is where the answer goes instead: the choices invented while
working, kept visible rather than buried in a diff.

One section per task, newest last. Each entry names what was decided, what the
specification does or does not say about it, and what would overturn it.

**This file is not a specification.** Nothing here outranks `docs/`. An entry
found to contradict it is a defect in the entry, not in the document.

---

## 2026-09-16 — Hash64: non-entity subjects, range reduction, overflow guard

Commit `3f97af8`, branch `hash64-soggetto-e-range`. Three fixes in
`core/Runtime/Primitives/`, closing the primitives of Phase 3.

### 1. The draw subject is a 64-bit key, not an entity

`Hash64.Of` takes `ulong subject`; the `EntityId` overload is the same function
with the handle packed into it.

SIM-REQ and DEC-002 write the draw as `Hash(worldSeed, entityId, tick, channel,
index)`, but SIM-STATE enumerates sampled transients from `Hash(worldSeed, link,
index)`, and a link is a row in the edge arrays with no generation. Neither
document says how the two key spaces coexist. Rejected: a second hash function
for non-entity draws, which would need every property proved twice.

### 2. A non-entity row key keeps its high half at zero

`Subject(int row)` returns `(uint)row`, so the generation half stays zero — the
value no live `EntityId` carries. This makes a row key and a live entity key
unable to name the same draw, and stops a negative row from sign-extending into
the entity space. `NoSubject` is `EntityId.None` packed.

Not in the specification, which never states that generation 0 is reserved; it
follows from `EntityId.None` being index 0, generation 0. Overturned if live
handles ever start at generation 0.

**Constraint, not a limit:** two different *kinds* of non-entity subject drawing
in the same channel would share one key space, where row 7 of one is row 7 of
the other. So each kind gets its own channel. Recorded in the `HashChannel`
comment alongside the rest of the channel contract; nothing in the build
enforces it.

### 4. `Range` takes a count, not an interval

`Range(draw, count)` returns `0 .. count - 1`; callers wanting `min .. max`
write `min + Range(draw, max - min + 1)`. No specification input. Rejected:
overloads for inclusive and exclusive bounds, as unrequested surface.

### 5. A-11 is checked in debug builds only, and by `checked` rather than an assert

SIM-STATE lists A-11 among the invariants "asserted every tick" and includes "no
accumulator has overflowed". The overflow guard added to the `long` overload of
`RemainderAccumulator.Apply` is `[Conditional("DEBUG")]`, so a release build
does not check it.

Reason: the release build cannot afford a check on every rate applied in every
settlement update, and the `int` overload cannot overflow at all. `checked` is
used in place of `Debug.Assert` because it throws exactly where the unchecked
arithmetic would wrap, with no sign cases to hand-write.

Overturned by the A-11 assert pass of Phase 3, which may want this always on, or
want it at state level instead of per call.

### 7. Process choices

- One commit rather than two. Splitting DEC-002 from A-11 would have to split
  `PrimitivesTests.cs`, leaving an intermediate commit whose A-11 test fails in
  Debug. The message names both identifiers.
- Committed to a branch rather than to `main`, against the repository's own
  history of committing to `main` directly.

---

## 2026-09-16 — Review of `hash64-soggetto-e-range`

Three changes required by review of the section above. Entries 3 and 6 there are
gone: the first rested on a premise the review showed to be false, the second
described a gap that no longer exists. Entries 1, 2, 4, 5 and 7 keep their
numbers, so the series above now skips 3 and 6.

### 1. `Hash64.Range` is a plain modulo

The reduction is `(int)(draw % (ulong)count)`. Rejection and re-mixing are gone,
and with them the reading of DEC-002 that entry 3 argued for: nothing here
consumes a variable number of draws any more.

Two findings from review, kept because the code no longer shows them: the
relative excess of a modulo reduction over a 64-bit draw is at most
`count / 2^64`, some 1e12 draws of world time against the 2^64 needed to see it;
and re-mixing could not have removed it anyway, since `Mix` is a bijection and
moves the discarded set onto another set of the same size rather than spreading
it. The magnitude is documented at `Range`, where the next reader will ask.

### 2. One channel per kind of non-entity subject

Written into the `HashChannel` comment, next to the rest of the channel
contract, since that is what a caller reads when choosing one. Entity keys
separate themselves by generation; non-entity keys have nothing to separate them
but the channel. Unenforced by the build, like the rest of that contract.

### 3. CI runs the debug build

A `dotnet test -c Debug` step, so the A-11 guard and
`A11_ApplyRefusesAProductThatLeaves64Bits` run somewhere other than a
developer's machine. Added as a step in the existing job rather than a matrix
over configurations: a matrix would also fan out the wealth-band steps, which
are release-configuration checks and have their own reason to run twice.

### 4. The histogram tolerance was left alone

`NFR03_RangeCoversTheIntervalEvenly` keeps its 5% band. Review allowed widening
it if it had been sized around exact uniformity; it was not. 5% is about 5.4
standard deviations of sampling noise at seventy thousand draws, while the
modulo's own deviation is of order 2^-61. Widening would have loosened a bound
that has nothing to do with the claim that changed. The reasoning is now in the
test.

### 5. Left as found

The remark on `Hash64.Of` still phrases channel separation as the sensible
answer rather than as the rule it now is. `HashChannel` states the rule; further
edits to `Hash64` were outside the review.

---

## 2026-09-17 — The plan mechanism and its three skills

Branch `plan-e-skill`, planned in `PLAN.md`, designed in
`.claude/design/2026-09-17-plan-e-skill.md`. Entries name the plan point they
belong to.

### Whole plan — `docs/` is silent, and stays silent

Every point of this plan declares `docs/` silent, so the declaration is made once
here instead of five times in the plan. `SIM-REQ` §18 plans the simulator; it
does not plan how the simulator gets built, and it should not start. Process work
that wrote itself into the specification would be indistinguishable, a year from
now, from the design the specification exists to hold.

### Point 1 — the rule in `AGENTS.md`

**An agent edited the standing instructions.** The rule given on 2026-09-16 is
that permanent instructions are changed by a human. Permission for this one was
given explicitly and for this change only; it does not generalise. Recorded
because a reader finding an agent commit on `AGENTS.md` should be able to see
that it was authorised rather than assumed.

**Four rules, and the procedure kept out.** `AGENTS.md` holds what must always be
true — plan first, criteria frozen, a point is a commit with a check, two
reviewers on the core. How to carry it out lives in the three skills. Splitting
them this way means a change to the procedure does not touch the standing
instructions, which are the part a human owns. The cost is that the rule and its
procedure can drift apart, and nothing but reading catches it.

### Point 2 — `/plan-explain`

**A point is open until it carries an outcome line, and nothing else counts.**
The skill finds the next point by looking for the first one without `*Esito:*`,
not by reading git, not by judging whether the work looks done. One test, stated
once, that all three skills share. The cost is that a point whose work is
finished but whose outcome line was never written reads as open — which is the
failure this design prefers, since the alternative is work that counts as done
without leaving a record.

**The skill is allowed to object to the point, and forbidden to fix it.** A
criterion that can be read two ways is cheapest to catch before the work starts,
so `/plan-explain` reads critically and reports what does not hold. It may not
edit the plan: criteria are frozen, and a skill that could adjust them would
dissolve the rule it exists to serve. Overturning a criterion stays a human's
call.

**It does not offer to write a missing plan.** Refusing is the point. Writing the
plan is where the human decides what the work is; a skill that filled the gap
would produce plans that agree with the agent that wrote them.

**Verification ran after the outcome line, not before it.** The step order of the
design puts verification before the record. This point's check reads the plan
itself, so running it before the outcome line was written would have measured a
state that never ships. For a point whose check reads the plan, verification runs
last, on the state the commit will contain.

### Point 5, added mid-plan — a plan may gain points, with approval

The first run of `/plan-explain` in a fresh session showed the defect it was
built to show, one level up from where it was looking: every point of this plan
reports a `Serves` field that says nothing, because `docs/` is not silent about
how the simulator gets built, it is unrelated to it. The field earns its place on
a plan that touches the simulator and produces a line of noise on one that does
not.

**Decided:** a plan may gain a point while it runs, with the human's approval,
and the criteria of the points already written stay frozen. Renumbering was
rejected: point numbers are cited by outcome lines, register entries and commit
trailers, so inserting the new point where it logically belongs would break every
reference already written to name it.

**Decided:** the new point runs last, in plan order, even though points 3 and 4
would ideally be written against the format it introduces. Executing it early
would have contradicted the rule that the next point is the first one without an
outcome line — the rule all three skills share. Points 3 and 4 are instead
written without hardcoding the field, so point 5 amends rather than replaces
them.

The first finding of the review protocol therefore came from a read-only skill on
a plan, not from a reviewer on a diff. Worth noting for the same reason the
protocol exists: the cheapest place to find a defect is upstream of the work.

### Point 3 — `/plan-status`

**Commits are resolved by the `Plan-point:` trailer, not by heuristics.** The
first run of `/plan-explain` objected that this point's criterion asks for
commits the plan does not contain, and called the association its weakest part.
Half right: the outcome line cannot carry a hash, since it is written before the
commit that contains it, but the closing commit carries a `Plan-point: <n>`
trailer, so `git log --grep` is an exact lookup. The mechanism was named in the
design and in no file the skill reads, which is why it looked absent. Fixed by
naming the command in the skill itself rather than by changing the plan.

**An outcome line with no commit behind it is reported, not repaired.** It is the
one state this design cannot tell apart from finished work, so the skill says so
and stops instead of picking a likely commit. A summary that guesses is worse
than one that admits a gap, because a guess reads as a fact.

**Rejected findings stay in the summary.** The register already keeps them; the
skill is required to surface them too. A rejection that disappears from the
report is a rejection nobody can audit, which is most of the value of writing it
down.

**Every point closed is not the exit condition met.** The skill checks the exit
condition against the repository instead of inferring it from the point count.
The two came apart in this very plan: after point 4 every original point was
closed while the exit condition still required a format that did not exist.

**Plans written before a rule changed are read as they are.** No tidying, no
back-filling a missing `Kind` line. The plan is a frozen contract and a skill
that corrected it would be editing what it exists to report on.

**`Plan-point: <n>` means the commit closes point n, and nothing else may carry
it.** Running this point's own check found the trailer on the commit that *added*
point 5, so the lookup returned a commit for an open point. The commit message
was amended — the branch had never been pushed — rather than teaching the skill
to tell closing commits from others by parsing the text after the trailer. A
trailer whose meaning depends on its prose is not a lookup key. The amended
commit is `f969e88`, replacing the `cef3974` reported in conversation.

The defect was found by executing the check rather than by reading it, on a point
whose check needed no build and no test. Recorded as evidence for a rule that
already exists and is easy to skip on a documentation point: run the check.

### Point 4 — `/plan-next` and the reviewer briefs

**"Do not edit the code" is enforced by the reviewer's tools, not by its
compliance.** Both reviewers are dispatched as a read-only agent type — `Plan`,
or `Explore` — which has no `Edit` and no `Write`. An instruction not to edit is
a request; a missing tool is a guarantee. Rejected: a general-purpose agent with
a strongly worded brief.

Neither type is a purpose-built reviewer: `Explore` is described as a fan-out
search that reads excerpts, `Plan` as an architect for implementation plans. The
brief carries the role, and the agent type is chosen for what it cannot do. If a
read-only reviewer type ever exists, it replaces both.

**The blind reviewer never sees the briefed one's output**, and a second pass
gets a fresh blind reviewer rather than the same one shown what the other said.
Blindness is not a state a reviewer can return to once it has read the author's
reasoning.

**For the blind reviewer, code that needs an explanation to be judged is itself a
finding.** This is a deliberate stance, not an oversight in the brief: the
absence of an explanation the author would have supplied is information about the
code.

**Two files, not one.** `reviewers.md` is read at step 4 and only for a point
declaring `Core: yes`, so the briefs stay out of the way of every point that does
not need them. The cost is a second file that can fall out of step with the skill
that dispatches it.

**The check is a read-through, and the plan said so before the work started.** A
structural check was run — the eight steps enumerated, both briefs present, the
four shared rules and the fifth — which proves the parts are there and proves
nothing about whether the procedure works. A skill cannot run itself. The real
verification is the first development that uses it, and that is outside this
plan.

**What `/plan-next` deliberately cannot do:** merge, push, open a pull request,
delete `PLAN.md`, or add a point to the plan. The deletion belongs to the last
commit before the merge, while the plan is still useful to whoever reviews the
branch; adding a point needs a human's approval, which is not a step in a
procedure.

### Point 5 — `Kind` in the header, `Serves` made conditional

**A missing `Serves` under `Kind: simulator` stops the work rather than being
filled in.** The field is mandatory there, so its absence is a defect in a frozen
contract, and the three skills treat it the way they treat an ambiguous
criterion: report it, refer it to a human. A skill that supplied the missing
requirement would be inventing the thing the field exists to prevent.

**This plan keeps its `Serves` fields.** It declares `Kind: process`, under which
the field is omitted, and it carries one on every point because it was written
before the rule existed. Removing them would have edited frozen criteria to match
a rule that postdates them — the exact move this design prohibits, performed in
the name of tidiness. All three skills are instead told to tolerate the field in
a process plan and to tolerate a plan with no `Kind` line at all.

So the first plan written under this format is also the first plan
grandfathered by it, and it will read slightly wrong forever. That is the
cheaper of the two mistakes.

**`docs/` is unrelated, not silent.** The distinction is the whole content of the
change. A field reporting "nothing in `docs/`" on a point that installs a skill
suggests the specification was consulted and had no opinion; in fact the
specification has no jurisdiction. The second reading invites someone, later, to
go and add one.

### Closing the plan — what the exit condition could and could not verify

Three of its four clauses were checked directly: the plan, the three skills and
the rule in `AGENTS.md` exist and agree with the design.

The fourth — that `/plan-explain` and `/plan-status` answer correctly about this
plan — was checked by invoking both, and the answers were right. But **a session
holds the copy of a skill it loaded, not the file on disk.** Both skills had been
edited by point 5 after this session picked them up, so what ran was the earlier
text; the committed files are the current ones and a new session loads those. The
clause is therefore met for the version that ran and asserted, not proved, for
the version that ships. Proving it takes a fresh session, which is a human's
action and not one this plan can contain.

Recorded rather than worked around, because the same trap is waiting for anyone
who edits a skill and tests it in the session that is already running it.

**The review protocol never ran.** No point in this plan declared `Core: yes`, so
the two reviewers were never dispatched and the part of this development with the
most moving parts is the part with no evidence behind it. The first
`Core: yes` point, in phase 4, is also the protocol's first real test. If it
produces noise instead of findings, `reviewers.md` is where to look, not the code
under review.
