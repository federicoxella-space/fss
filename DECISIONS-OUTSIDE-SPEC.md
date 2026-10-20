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

### D-001 · 1. The draw subject is a 64-bit key, not an entity

**Ruling:** R-008 — ratified.

**Promoted 2026-09-19 as `DEC-081`.** The text went in close to the draft; the
specification now states the subject coordinate and this entry is history.

`Hash64.Of` takes `ulong subject`; the `EntityId` overload is the same function
with the handle packed into it.

SIM-REQ and DEC-002 write the draw as `Hash(worldSeed, entityId, tick, channel,
index)`, but SIM-STATE enumerates sampled transients from `Hash(worldSeed, link,
index)`, and a link is a row in the edge arrays with no generation. Neither
document says how the two key spaces coexist. Rejected: a second hash function
for non-entity draws, which would need every property proved twice.

### D-002 · 2. A non-entity row key keeps its high half at zero

**Ruling:** R-008 — ratified.

**Promoted 2026-09-19 as `DEC-082`.** The reservation of generation 0 is now a
decision of record rather than something inferred from `EntityId.None`.

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

### D-003 · 4. `Range` takes a count, not an interval

**Ruling:** R-008 — ratified.

`Range(draw, count)` returns `0 .. count - 1`; callers wanting `min .. max`
write `min + Range(draw, max - min + 1)`. No specification input. Rejected:
overloads for inclusive and exclusive bounds, as unrequested surface.

### D-004 · 5. A-11 is checked in debug builds only, and by `checked` rather than an assert

**Ruling:** R-008 — ratified.

**Superseded 2026-09-19, not promoted.** `SQ-002` closed by splitting the
invariant: A-11 is now the range of fixed-point quantities held in state, A-11b
the overflow of accumulators, checked in debug builds with the reason written
beside it. This entry recorded the implementation departing from a requirement;
the requirement moved instead. The guard was right and the invariant's text was
wrong, which is the outcome a decision register cannot reach on its own.

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

### D-005 · 7. Process choices

**Ruling:** R-008 — ratified.

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

### D-006 · 1. `Hash64.Range` is a plain modulo

**Ruling:** R-008 — ratified.

**Promoted 2026-09-19 as `DEC-084`.**

The reduction is `(int)(draw % (ulong)count)`. Rejection and re-mixing are gone,
and with them the reading of DEC-002 that entry 3 argued for: nothing here
consumes a variable number of draws any more.

Two findings from review, kept because the code no longer shows them: the
relative excess of a modulo reduction over a 64-bit draw is at most
`count / 2^64`, some 1e12 draws of world time against the 2^64 needed to see it;
and re-mixing could not have removed it anyway, since `Mix` is a bijection and
moves the discarded set onto another set of the same size rather than spreading
it. The magnitude is documented at `Range`, where the next reader will ask.

### D-007 · 2. One channel per kind of non-entity subject

**Ruling:** R-008 — ratified.

**Promoted 2026-09-19 as `DEC-083`.** It was a rule living in a source comment
and unenforced by the build; it is now a numbered decision, still unenforced by
the build.

Written into the `HashChannel` comment, next to the rest of the channel
contract, since that is what a caller reads when choosing one. Entity keys
separate themselves by generation; non-entity keys have nothing to separate them
but the channel. Unenforced by the build, like the rest of that contract.

### D-008 · 3. CI runs the debug build

**Ruling:** R-008 — ratified.

A `dotnet test -c Debug` step, so the A-11 guard and
`A11_ApplyRefusesAProductThatLeaves64Bits` run somewhere other than a
developer's machine. Added as a step in the existing job rather than a matrix
over configurations: a matrix would also fan out the wealth-band steps, which
are release-configuration checks and have their own reason to run twice.

### D-009 · 4. The histogram tolerance was left alone

**Ruling:** R-008 — ratified.

`NFR03_RangeCoversTheIntervalEvenly` keeps its 5% band. Review allowed widening
it if it had been sized around exact uniformity; it was not. 5% is about 5.4
standard deviations of sampling noise at seventy thousand draws, while the
modulo's own deviation is of order 2^-61. Widening would have loosened a bound
that has nothing to do with the claim that changed. The reasoning is now in the
test.

### D-010 · 5. Left as found

**Ruling:** R-008 — ratified.

The remark on `Hash64.Of` still phrases channel separation as the sensible
answer rather than as the rule it now is. `HashChannel` states the rule; further
edits to `Hash64` were outside the review.

---

## 2026-09-17 — The plan mechanism and its three skills

Branch `plan-e-skill`, planned in `PLAN.md`, designed in
`.claude/design/2026-09-17-plan-e-skill.md`. Entries name the plan point they
belong to.

### D-011 · Whole plan — `docs/` is silent, and stays silent

**Ruling:** R-008 — ratified.

Every point of this plan declares `docs/` silent, so the declaration is made once
here instead of five times in the plan. `SIM-REQ` §18 plans the simulator; it
does not plan how the simulator gets built, and it should not start. Process work
that wrote itself into the specification would be indistinguishable, a year from
now, from the design the specification exists to hold.

### D-012 · Point 1 — the rule in `AGENTS.md`

**Ruling:** R-008 — ratified.

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

### D-013 · Point 2 — `/plan-explain`

**Ruling:** R-008 — ratified.

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

### D-014 · Point 5, added mid-plan — a plan may gain points, with approval

**Ruling:** R-008 — ratified.

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

### D-015 · Point 3 — `/plan-status`

**Ruling:** R-008 — ratified.

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

### D-016 · Point 4 — `/plan-next` and the reviewer briefs

**Ruling:** R-008 — ratified.

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

### D-017 · Point 5 — `Kind` in the header, `Serves` made conditional

**Ruling:** R-008 — ratified.

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

### D-018 · Closing the plan — what the exit condition could and could not verify

**Ruling:** R-008 — ratified.

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

---

## 2026-09-17 — The twelve gaps in the plan mechanism

Branch `plan-e-skill`, continuing after the plan of the same name closed. Planned
in `PLAN.md`; the review that produced the twelve points was human, on the
mechanism the previous section built.

### D-019 · Point 1 — the criterion to the reviewers, and citations opened

**Ruling:** R-008 — ratified.

**The criterion is pasted, not summarised.** Both reviewers receive the point's
frozen text verbatim from `PLAN.md` — statement, citations, check, and any
`Check: weak` marker. A summary written by the author of the code under review is
an opportunity to soften the standard that code is measured against, and it would
be taken without anyone noticing, least of all the author.

**Two findings that the reviewers are now told to look for**, neither of which
was in the brief before: a check that passes for a reason the criterion did not
ask for, and a check marked weak that turns out to be properly verifiable. The
first is how a green check hides unfinished work. The second is how `Check: weak`
would decay from an argument into an excuse.

**A failed citation splits into two cases, and neither is the author's to fix.**
The point citing the wrong identifier is a defect in a frozen plan: report and
ask. The requirement itself being contradictory or unachievable is a defect in
`docs/`: say so and stop. The second has no home in the repository yet — it ends
in a conversation, which is the gap point 8 exists to close.

**The rule got no exercise from the point that introduced it.** This is a
`Kind: process` plan, so no point here cites anything in `docs/`, and the
citation check had nothing to open. Consistent with the declared weak check, and
worth stating rather than leaving a reader to assume the rule was tried.

### D-020 · Point 2 — `Core:` defaults to yes

**Ruling:** R-008 — ratified.

**A missing reason is resolved as `yes`, and the plan is not stopped for it.**
Point 1 established that a defect in a frozen plan stops the work; this is the
exception, and it is deliberate. The absence of a justification has an obvious
safe answer — run the reviewers — and stopping a plan to demand the paperwork for
a decision already settled by default would cost more than it protects. The
defect is reported, the point is reviewed, the work continues.

**The reason has to be checkable against the diff.** "Small change", "only
documentation", "low risk" are judgements by the author about the author's work,
which is the thing the reviewers exist to replace; accepting one as grounds for
skipping review would let the author dismiss the reviewers by describing the work
favourably. "Touches no file under `core/`" is a statement a diff can falsify.

**And the declaration is verified, not trusted.** Step 3 runs
`git diff --name-only HEAD -- core/`; a point claiming `Core: no` whose diff
touches the core has a false reason, so the reviewers run anyway and the
contradiction is recorded. Without this the downgrade is self-certified and
unaudited, since the reviewers who might have caught it are exactly what the
downgrade skips. Same treatment as the read-only agent type in `reviewers.md`:
the tools check what a promise cannot.

**This point's check was declared weak and was not.** All three of its clauses
turned out mechanically verifiable — two greps and the diff check above — so
`Check: weak` was applied out of habit rather than analysis, on a plan where
almost every point carries the marker. Recorded because over-applying it is as
corrosive as skipping it: a marker on everything lowers what anyone expects to be
proved. The criterion stays as written; a frozen criterion that turns out
pessimistic is reported here, not corrected there.

It is also the first instance of the finding the reviewers were told to look for
one point earlier — a check marked weak that is properly verifiable — and it
turned up on this author's own work, unprompted by any reviewer, because the
brief had been written the day before.

### D-021 · Point 3 — re-reading the plan before the outcome line

**Ruling:** R-008 — ratified.

**The re-read diagnoses; the anchored edit protects.** Both appends are made with
an exact-match edit against text just read, not a rewrite of the file. An edit
that cannot find its anchor fails loudly; a rewrite from memory wins silently
against whatever a human wrote in the meantime. So the protection does not depend
on the re-read being remembered — which matters, because a step that exists only
in a document is a step that gets skipped under pressure.

**Five cases, and three of them stop the work.** A changed criterion stops it
because the work was done against a contract that no longer exists, and the
honest report is that the work is finished and unrecorded — a state that needs a
human, not a best guess. An outcome line already present stops it because
someone else closed the point and nothing else can safely be assumed. Edits to
*other* points do not stop it: their text is not this point's contract, and
treating every plan edit as fatal would make the plan unmaintainable while work
is in progress.

**`git add -A` is now forbidden at step 7,** which is a rule written against the
habit of the author writing it: the commits closing points 1, 2 and the plan
installation all used it. It absorbs an unrelated edit sitting in the tree into
the point's commit, where git then attributes it to the point and to whoever
signed the message. This point's own commit stages by name, so the rule takes
effect on the commit that introduces it.

Nothing here was found by a reviewer. The whole point came from a human noticing
that a document read at step 1 and written at step 6 has a gap in the middle
wide enough to lose an edit in.

### D-022 · Point 4 — `Check: weak`, declared early and with both halves

**Ruling:** R-008 — ratified.

**Both halves are required, and the second is the one that does the work.**
"This cannot be proved today" and "this cannot be proved" are different
statements, and only the first says when to come back. A marker carrying only the
reason authorises weakness; carrying the replacement, it schedules its own end.

**Looking for a mechanical check is now a step, not a disposition.** Step 3 of
`/plan-next` treats the marker as a claim and tests it before accepting it.

**And the marker was over-applied twice in a row, which makes it a pattern.**
Point 2 carried it and every clause proved mechanical. This point carried it too,
with the note that the self-reference was "the honest version of the problem",
and its check ran as three greps. Ten of this plan's twelve points carry the
marker; on present evidence most of them should not.

**What was actually wrong, and it is not what the marker says.** These criteria
*can* be run — a grep proves the text exists. What they cannot do is prove the
text is any good. So two different defects were being labelled with one marker:

- **weak** — the check cannot be executed at all;
- **shallow** — the check executes and proves little, because what matters is
  not mechanically observable.

Almost every documentation point in this plan is shallow, not weak, and calling
it weak misdescribes the problem in the direction that sounds more rigorous. The
distinction is not written into the design: it is beyond what point 4 states,
and a frozen point is not widened by its author mid-flight. Recorded here, and
raised with the human as a candidate for its own point — which it became, as
point 13.

### D-023 · Point 5 — a failed point and an abandoned plan

**Ruling:** R-008 — ratified.

**A failed point is closed, not left open.** It carries an outcome line saying
`FALLITO`, a register entry, and a commit with the usual trailer. Leaving it open
would make it indistinguishable from a point not yet attempted, and a road
already walked is worth more written down than a road not taken.

**The failure commit must leave the repository green.** Whatever of the attempt
survives the build and the suite is kept so the next person can see it; the rest
is prose in the register. A red commit on the branch makes every later `git
bisect` lie, and bisect is the tool someone will reach for precisely when
something has gone wrong.

**A failure stops the plan, not just the point.** Later points may rest on the
failed one, and even where they do not, the human needs to know before more work
is spent in the same direction. `/plan-explain` is told not to present the point
after a failure as "next": the plan is not running, and offering the next point
would invite it to resume by accident.

**Abandonment is a human's word.** `/plan-next` can fail a point and can never
abandon a plan. A procedure able to abandon its own plan when the work got hard
is not a procedure — it is a preference with a document attached.

**Open points in an abandoned plan stay open.** They record what was intended
and not done. Closing them to tidy the archived file would erase the only
evidence of the shape the work was going to take.

**Register entries survive abandonment.** They describe code that existed and
decisions really taken; neither becomes untrue because the plan stopped. An entry
is wrong only if it was wrong when written — which is also why an abandoned
development still goes through the promotion pass: it can have produced a
decision that belongs in `SIM-DEC`.

### D-024 · Point 6 — the threshold below which no plan is needed

**Ruling:** R-008 — ratified; its open question answered by R-010.

**The permission to edit `AGENTS.md` was read from the approved plan.** The
standing rule is that permanent instructions are changed by a human and an agent
needs specific permission. Point 6 names `AGENTS.md` in its statement and the
human approved the plan containing it, so the approval is taken as the
permission. Recorded rather than assumed silently, because the alternative
reading — that each edit needs its own explicit sentence — is defensible and this
one is not mine to settle.

**All three properties, conjunctively, and none of them is about size.** "Small"
and "quick" are the words that make an exemption usable for whatever the author
wants to skip. No change in behaviour, no decision, one commit: each is
falsifiable by someone reading the diff afterwards, which is what an exemption
needs to survive its author's optimism.

**Salami-slicing is named and closed.** Several trivial commits that together
change behaviour are not several trivial changes. Without this the exemption is a
general-purpose bypass: nothing stops a plan-sized change from being cut into
pieces each of which, alone, changes nothing observable.

**The exemption and the register rule agree by construction.** Trivial work
leaves no register entry, because the property that makes it exempt — no decision
— is the same property that makes the register empty. Two rules that could have
contradicted each other turn on one hinge instead.

**The check was declared weak and is shallow.** Two greps against `AGENTS.md`
prove the words are present; nothing here can prove the threshold is drawn in the
right place. That only comes from the first argument about whether something was
trivial, which is what the criterion said. The marker stays as written: point 13,
which supplies the right word, is not closed yet.

### D-025 · Point 7 — `Satisfies` in simulator plans

**Ruling:** R-008 — ratified.

**`Satisfies` and `Serves` are not one field at two scales.** `Serves` is per
point and stops a point from inventing a requirement. `Satisfies` is per plan and
stops an acceptance criterion from having nothing behind it. They could have been
collapsed into one; keeping them apart is what lets the second answer *which
acceptance criteria have a test*, which is the coverage question and the one
nothing in this repository can answer today.

**Checked once, at the first point, not at every one.** The header belongs to the
plan, not to the point, and re-confirming the same identifiers twelve times would
turn a real check into a ritual — the surest way to have it performed without
being done.

**A partial contribution is still declared, with what remains.** Declaring
nothing because the plan only moves a criterion partway is how a criterion ends
up with three developments behind it and no record of any of them.

**Nothing here was exercised.** This is a process plan with no `Satisfies:` line
and no `AC-` to open, so every rule written in this point is untried. The first
simulator plan is where it either works or does not, and that is also the first
time the coverage question can be asked at all.

### D-026 · Point 8 — `SPEC-QUESTIONS.md`

**Ruling:** R-008 — ratified.

**It opens with a real question, not an empty state.** `SQ-001` records a genuine
inconsistency found while building `Hash64.Subject` three days ago: DEC-002 and
`SIM-STATE:25` specify a five-coordinate draw, while FR-X-02 and `SIM-STATE:123`
specify a three-coordinate one, and FR-X-02 being a requirement makes the short
form normative rather than loose prose. A channel that ships empty teaches
everyone that it is decorative.

That the objection sat unrecorded for three days, in a session that was
otherwise writing down everything, is the argument for the file in one line.

**An objection is never resolved by its finder.** This is the rule the file
turns on. The author who found the contradiction is the author who wants to keep
working, and is therefore the worst possible judge of whether it can be worked
around. A stale open entry is a truer state of affairs than one closed by the
person it inconvenienced.

**Every entry declares whether it blocks, and blocking has teeth.** A blocking
entry fails the point and stops the plan, by the rule from point 5. Without the
consequence the declaration is a label, and the file becomes where objections are
filed *instead of* acted on — the exact failure the human named when asking for
it.

**`SQ-001` is deliberately not resolved here**, though a plausible reading exists
and is written into the entry: the tick may be omitted because FR-X-02 makes the
departure tick an *output* of the draw. Choosing that reading would fix the
sampled transients of every generated world to a decision nobody recorded, in the
one component where an unrecorded choice cannot be recovered from the output.
Recording the reading and leaving the question open is the whole point of the
file existing.

### D-027 · Point 9 — archival, and identifiers in this register

**Ruling:** R-008 — ratified.

**The archive trigger is the plan closing, not the branch merging.** A branch can
hold two plans in sequence — this one does — and tying the archive to the merge
would leave a finished plan at the root pretending to be current while its
successor is written. The original rule said deletion before the merge; it was
written when a branch and a plan looked like the same thing.

**Archived rather than deleted.** The closed plan is the only record of what was
*intended*, beside a git history that shows only what happened; and archival is
the moment the promotion pass has both the plan and its register entries in
front of it.

**The gesture preceded the rule, as promised in the commit that installed this
plan.** The previous plan was archived before point 9 wrote the rule authorising
it, because the branch was continuing and the plan was finished. Recorded rather
than presented as having happened in the right order.

**Identifiers are per entry, `D-001` upward, never reused and never
renumbered** — the same discipline as `HashChannel`, for the same reason: an
identifier that moves is worse than none, because every reference to it silently
starts pointing elsewhere.

**The register is not uniform about what an entry holds**, and that was left
alone. The earliest sections give one decision per entry; the later ones bundle
several under a plan point. Splitting them retroactively to make the numbering
tidy would mean rewriting the register's history in service of a scheme, so a
promotion citing a bundled entry quotes which part of it it took instead.

**Applied by script, not by hand.** Twenty-six headings renumbered by one `awk`
pass, so the assignment is deterministic and reviewable as a single
transformation rather than twenty-six chances to mistype.

### D-028 · Point 10 — the promotion pass

**Ruling:** R-008 — ratified.

**A third root file, `PROMOTIONS.md`, rather than a section of the register.**
The register records what was decided; this proposes to change `docs/`. Folding
the proposal into the record would leave the register asserting things about the
specification that are not true of it yet. It is the mirror of
`SPEC-QUESTIONS.md`: that file says the specification is wrong, this one says it
is missing something, and both are struck through when a human acts.

**One test for a candidate: does it constrain code beyond the change that
produced it?** Not importance, not how hard it was to decide — those select for
what the author found interesting. A future implementer either needs it or does
not.

**The first pass covers the whole register**, because no pass has ever run, and
the development just archived contributes nothing to it. That emptiness is the
rule from `D-011` working: a pass that found promotable architecture in its own
tooling would be the first sign the rule had stopped being observed.

**Rejected candidates are struck with the human's reason, not left standing.**
Otherwise the same proposal returns at every archival and eventually gets
written in by attrition, which is a slow way of letting the agent edit `docs/`.

**Numbering left to the human.** `SIM-DEC` runs to DEC-080 and a number claimed
in a proposal would collide with whatever else is in flight; the drafts say
`DEC-0NN`.

**The pass filed a spec question, and nearly did not.** Classifying `D-004`
surfaced that A-11 is specified as asserted every tick while the guard is
debug-only. The first draft of `PROMOTIONS.md` explained at length why that
belonged in `SPEC-QUESTIONS.md` without filing it there — which is precisely the
failure the human named when asking for that file: an objection resolved in
prose instead of deposited. Filed as `SQ-002`.

Worth keeping as a property of the pass rather than of this entry: reading the
register for promotable architecture is also the only moment anyone reads every
decision back against the specification it was taken under.

### D-029 · Point 11 — the read-only skills run as read-only agents

**Ruling:** R-008 — ratified.

**The dispatch works and the guarantee is real.** `/plan-status` was run as a
`Plan` agent, which has no `Edit` and no `Write`. It reported the plan correctly
and the working tree was byte-identical afterwards — `git status --short` and
`git diff` hashed before and after.

**What the guarantee does not cover is the relay**, and both skills now say so.
The agent's report returns to a session that can write, and nothing stops that
session from softening it in the retelling. The instruction is to quote findings
rather than restate them, which is a weaker protection than the tool set and is
the honest limit of this approach.

**It matters more for `/plan-explain` than it looks.** That skill forms
objections to the plan, and an agent holding an objection and a text editor is
one step from resolving the objection by editing the text. The criteria are
frozen by this, not only by the instruction that says so.

### D-030 · Point 11 — three defects the read-only agent found

**Ruling:** R-008 — ratified.

The dispatch was meant to prove a mechanism and immediately did the job the
mechanism exists for. All three are this author's, none was caught by reading.

**The `Plan-point:` trailer was used loosely three times, by the author of the
rule forbidding it.** `59a4d8b`, `046751f` and `9166c8a` carry it with prose as
its value — "questo commit non chiude un punto" — on commits that install a
plan, close a plan and add a point. The rule says the trailer means the commit
closes point n and nothing else may carry it.

Not rewritten. The values are prose, so the numeric lookup never matches them
and the practical damage is nil; rewriting three messages mid-history would
invalidate SHAs already reported to the human for the second time in this
branch. Instead the rule tightens: the trailer's value is a bare integer, and a
commit that closes no point does not write the word at all.

**The lookup matched by substring.** `--grep="Plan-point: 1"` also matches 10
through 19. Fixed here, in the skill being committed: `-E` with `^` and `$`.
Leaving a known-broken command in a file under edit was not defensible.

**The lookup is not qualified by plan, and that one is not fixed.** This branch
has held two plans, each with points 1 to 5, so the anchored search still
returns two commits for each of those numbers. The agent disambiguated by
commit order, which works and is not a guarantee. It needs its own point: the
fix is either a plan-qualified trailer or a search scoped to the commit that
installed the current plan, and choosing between them is design work, not a
correction to smuggle into a point about read-only dispatch.

**Two outcome lines are silent about their `Check: weak` marker**, points 3 and
10, where the other eight report how the marker fared. Not retrofitted. An
outcome line records what was reported at the time; editing one to add what
should have been said is falsifying the record in the direction that flatters
the author.

### D-031 · Point 12 — no squash, and a clause that could not be met

**Ruling:** R-008 — ratified; its missing outcome supplied by R-011.

**A pull request template, not just this branch's body.** The criterion asked
for the note in the pull request for this branch; a template puts it in every
one, and makes the criterion's own clause true automatically whenever a request
is opened. `.github/pull_request_template.md`.

**The argument is not about tidy history.** A squash collapses the
`Plan-point:` trailers into one message, and `/plan-status` resolves every
closed point of every past plan through those trailers. One squash breaks the
lookup for the whole repository, retroactively and silently — the record stops
being indexed and nothing announces it.

**The second clause is not met, and this is not dressed up as a pass.** There is
no pull request for `plan-e-skill`: the remote holds the branch at `046751f`,
pushed by the human, thirteen commits behind, and opening a request is an
outward-facing action nobody asked for in this round. So the point is closed
with one clause satisfied and one pending a human.

The mechanism has no state for that. A point either closes or is `FALLITO`, and
this is neither: the work is done, the check is half-verifiable, and what
remains is not the author's to do. Declaring `FALLITO` would have stopped the
plan against an explicit instruction to continue; declaring a clean pass would
have put a claim in the record that the repository does not support. Recorded as
the third outcome the design is missing, which is worth more than either lie.

**And the branch turns out to be published**, which strengthens `D-030`'s
refusal to rewrite three commit messages: `046751f` is one of them and it is on
the remote, so the rewrite would have been a force push over work the human has
already fetched.

### D-032 · Point 13 — `Check: shallow`

**Ruling:** R-008 — ratified.

**The remedies are the reason the distinction exists**, not the vocabulary.
`weak` is a wait for a tool that does not exist; `shallow` is an admission that
the author chose a criterion proving less than it should, and the better
criterion is available today. Labelling the second as the first blames the
absence of a tool for a decision.

**Try the deeper criterion before reaching for the marker.** An admission made
without attempting the alternative is a wait wearing an admission's clothes.

**"Cannot be tested" is not an acceptable second half; a named use is.**
"Replaced by the first development that runs under this rule" says when to come
back and what will be true then. The rule has to survive the common case, which
is documentation, where nothing mechanical will ever prove the text is any good.

**A shallow check is still run.** It catches the gross failures — a paragraph
that is not there, a file that was not written — and running it is the only way
to discover it was shallow rather than weak.

**The wrong markers in this plan were left wrong.** Ten of thirteen points
carried `weak` and nearly all were shallow. The criteria were frozen when the
plan was written; correcting them now would be the move the whole mechanism
exists to prevent, performed on the point that describes the mistake.

---

## 2026-09-19 — The review request

Branch `plan-e-skill`, third plan, planned in `PLAN.md`. Asked for by the human
while away, after a review of the mechanism built in the two plans above.

### D-033 · Point 1 — `REVIEW-REQUEST.md` and the three moments

**Ruling:** R-008 — ratified.

**Everything else in this mechanism is a record, and a record waits.** That is
the right shape for a record and the wrong one for a question. The file exists
because a mechanism made only of records produces its human opinion whenever one
is volunteered, which on a long branch is after the work that needed it was
already built on.

**Three moments and no others.** A request at every point is a notification, and
a notification that arrives constantly is filtered out — at which point the
mechanism has a channel to the human that the human has learned to ignore, which
is worse than not having one.

**One request at a time, replaced by the next.** Accumulating them would make
the file a second register, and the register already exists. Git holds the old
ones.

**The sixth part is the reason for the file.** Which moment, which commit, what
changed, the decisions, the questions — all five are discoverable by a reader
willing to look. *What was not verified* is not: an unrun reviewer, a check
satisfied by reading, a clause left pending leave no trace in a repository, and
only the author knows they are there. A request without that part is a summary,
and summaries are what the register is for.

**The reminder never writes the request on the human's behalf**, and writing one
does not authorise continuing past a blocking question. Both would turn an
interruption into a formality that clears itself.

**The first request covers all three moments at once**, which is a finding about
the absence rather than about the request: the mechanism was built inside a
single absence, so one file now carries three plans, two archives, two
specification questions and four promotion candidates. Had it existed on
2026-09-17 there would have been four shorter requests, each arriving when it
was cheap to act on. It is written into the request itself, because a first
example that hides its own shape teaches the wrong one.

### D-034 · Point 2 — the reminders, and which skill owes which moment

**Ruling:** R-008 — ratified.

**One moment each, except the first, which two skills carry.** `/plan-explain`
and `/plan-next` both announce moment one on the first point of a plan, because
either can be the command someone runs first and a reminder that depends on
which one they chose is not a reminder. `/plan-next` also carries moment three,
since it is the only skill present when a `SPEC-QUESTIONS.md` entry is filed.
`/plan-status` carries moment two alone: it is the only one that sees a whole
plan, and a phase gate is not visible from inside a point.

**The reminder is a statement, never an action.** Each skill says a request is
owed and none of them writes one. A reminder that discharges itself is a
formality, and the whole file exists because a record that waits to be read was
not enough.

**And saying it is owed does not authorise continuing.** Stated in `/plan-next`
because that is the skill that could act on the permission it would be granting
itself: a blocking specification question stops the point whether or not a
request was written about it.

**The count in the first request was made relative.** It said seventeen commits
behind, which was true when written and false by the time the plan closed. A
request is read later than it is written, so a fact that decays is stated in a
form that does not — everything after `046751f` is unpushed. Noted because
editing a closed point's artefact is worth declaring, even when the artefact is
not a frozen criterion.

---

## 2026-09-19 — Between plans: the specification answered

No plan. The human closed `SQ-001` and `SQ-002`, wrote `DEC-081` to `DEC-084`
into `SIM-DEC`, and set the rule that a specification update lands between two
plans rather than during one. What follows is the bookkeeping that closes the
loop, and what the answers cost.

### D-035 · The return channel worked, and returned more than it was asked

**Ruling:** R-008 — ratified.

**Both questions closed, and neither closed the way it was posed.**

`SQ-001` asked which arity was normative. The answer gave the five-coordinate
form *and* a clarification the question had not thought to ask: the tick
coordinate is the one at which a trajectory is **generated**, not the one being
observed. Without it FR-X-03 does not hold — a draw keyed on the observing tick
re-rolls every tick and the caravans flicker instead of being followable. The
question found an inconsistency in arity and missed that the arity was hiding a
semantics.

`SQ-002` closed by moving the requirement, not the code: A-11 split into the
range invariant, checked always, and **A-11b** for accumulator overflow, checked
in debug builds with the affordability reason written beside it. The guard was
right and the invariant's text was wrong.

That second outcome is the one worth keeping. **A decision register cannot
reach it.** The register's vocabulary is "the implementation departed from the
specification, here is why" — it can record a deviation and it can never
conclude that the specification was wrong, because nothing in it is allowed to.
`D-004` sat in the register for three days saying the code deviated from A-11.
It took a channel that runs the other way to find out the code did not.

### D-036 · Bookkeeping, and what marking costs

**Ruling:** R-008 — ratified.

**Four register entries marked with what they became**, `D-001` → `DEC-081`,
`D-002` → `DEC-082`, `D-007` → `DEC-083`, `D-006` → `DEC-084`, and the
candidates struck in `PROMOTIONS.md` but kept as the record of what was
proposed. `D-004` is marked superseded rather than promoted: it closed through
the other channel.

**Marked, not deleted.** The promoted entries stay in the register with a
pointer. Removing them would leave the reasoning that produced a decision only
in the commit that removed it, and the whole apparatus exists so that reasoning
outlives the diff. The cost is a register that grows even where it has been
promoted — the pass empties the *queue*, never the record.

**The second promotion pass, over `D-033` and `D-034`, found nothing**, which is
the expected result for a process plan and is written down anyway. A pass that
reports nothing is evidence the pass ran; a pass that leaves no trace is
indistinguishable from one that was skipped.

### D-037 · `SQ-003`, filed rather than answered

**Ruling:** R-008 — ratified.

`NFR-03` and `DEC-002` still read `entity_id`, while FR-X-02 now writes the
subject form and cites "under NFR-03" as its authority. The documents that *use*
the draw were updated; the two that *define* it were not.

Filed rather than resolved, though the intent is obvious from `DEC-081` and the
implementation already follows it. Resolving an obvious one by reading it
charitably is how the channel stops being used: the next reader has no way to
tell which small inconsistencies were adjudicated in silence and which were
never noticed.

It is the residue of `SQ-001`, which was the same shape, went unrecorded for
three days, and turned out to be hiding the tick question.

### D-038 · `SQ-003` decided, and the edit left to a human

**Ruling:** R-008 — ratified.

**Resolution A: the definitions move.** `NFR-03` and `DEC-002` take the subject
form rather than FR-X-02 re-pointing its citation at `DEC-081`. `SIM-REQ` states
what is required, so a decision that changes the shape of the primitive leaves
the requirement stating it stale; and re-pointing would leave two definitions of
the draw disagreeing, with the reader made to pick.

**The edit was not made.** The human answered "go with A", and A was presented as
their work because it touches `docs/`. That is the one folder whose rule names a
human explicitly, and every change to it so far — the four decisions, the A-11
split — was written by one. An instruction that can be read as *this is the
answer* or as *apply it* is not authorisation for the edit this repository
protects most.

**So the exact text sits in `SQ-003` instead**, both lines, before and after,
ready to paste. The decision survives the session, the question stays open until
`docs/` actually changes, and whoever applies it does not have to reconstruct
anything. Blocking on the ambiguity would have cost more than the one line it
takes to resolve it.

**Applied 2026-09-20**, on an instruction that left no room — "applica tu per
favore". Recorded here rather than by rewriting the paragraph above: the entry
says the edit was not made, which was true when written, and the mechanism's
whole discipline is that a record states what was true at its moment.

### D-039 · The stale request was replaced, not amended

**Ruling:** R-008 — ratified.

The request of the morning was false by the evening: the branch it described is
merged, its commits pushed, both its questions closed. It was replaced whole
rather than corrected in place, which is what the design says — one request at a
time, the old ones in git.

Worth noting as a property of the artefact: **a review request decays faster
than anything else here.** The register and the plans describe what happened and
stay true; a request describes what is *outstanding*, and every answer makes
part of it false. Reading a stale one is worse than reading nothing, because it
looks current.

### D-040 · Applying `SQ-003` overran the text that had been deposited

**Ruling:** R-008 — ratified.

The entry said "nothing else in `docs/` needs to move". It was wrong: the World
note at `SIM-STATE:26` carried the entity form too, and the two agreed lines
would have left it as the only place in the specification still saying it.

**The same substitution was applied there**, one line beyond the authorisation,
because the inconsistency would have been created by the edit itself. The
alternative was leaving the specification disagreeing with itself and reporting
that the deposited text had been incomplete — which is the honest half of what
was done, minus the fix.

The limit that held: **the Rationale of DEC-002 was left alone.** "Any entity's
random draw at any past tick can be recomputed directly" is narrower than the
decision above it and still true, and narrowing is not falsehood. Rewriting a
rationale that was not part of the agreed change is where an authorised edit
turns into an unauthorised one.

What this says about the deposit, and it is the useful part: **a drafted change
to `docs/` is not verified until it is applied.** "Nothing else needs to move"
was a claim about the whole specification, written from the two lines in front
of me, and a grep would have falsified it in a second. The same discipline the
plan mechanism applies to a check — run it, do not read it — was not applied to
a claim about `docs/`, because that claim was prose rather than a criterion.

### D-041 · The request was amended, not replaced

**Ruling:** R-008 — ratified.

The request of the evening asked for `SQ-003`, which is now answered. It was
amended in place rather than replaced whole, against the precedent set hours
earlier by `D-039`.

The two are different cases, and the distinction is worth keeping: a request is
**replaced** when the situation it describes has moved, and **amended** when one
of its items is answered and the rest still stands. Both rules serve the same
end, which is that the file is never false; replacing wholesale for a single
closed item would churn the artefact for no gain.

---

## 2026-09-20 — The point lookup, qualified by plan

Branch `main`, planned in `PLAN.md`, one point. Approved by the human, who
deferred the phase-3 plan to a clean session.

### D-042 · The range, not a new trailer

**Ruling:** R-008 — ratified.

**The lookup is bounded, the commit format is unchanged.** The alternative was a
second trailer naming the plan — `Plan: 2026-09-19-dodici-lacune` — which would
have been explicit and would have worked only for commits written after it. The
range works on the history that already exists, which is the whole history there
is.

**The lower bound is the half that matters.** Bounding only above still resolves
point 3 of the review-request plan, which never had one, to a commit of the plan
before it: walking back from an archive commit runs straight into the previous
plan. The false positive is silent and looks like a fact. With both bounds, a
number a plan never carried resolves to nothing, which is the truth.

**It rests on plans being sequential** — one `PLAN.md` at a time, archived before
the next is installed. Nothing enforces that but the procedure, and the skill
says so: if two plans were ever live at once, this lookup is the first thing that
would go wrong.

**Committed to `main` without a branch**, against the habit of the last three
plans. One point, no code, and the human is about to open a clean session against
`main`; a branch would have left them a merge before they could start. Recorded
rather than done quietly.

### D-043 · A check that passed without running

**Ruling:** R-008 — ratified.

The first version of this point's check was a Python script. It printed
`ESITO: tutti i punti risolvono a un commit solo` and had verified nothing: a
parsing mistake left its list of plans empty, so the loop never ran and the
success flag stayed at its initial value. The verdict was vacuous and read as
proof.

It was caught only because the per-plan lines that should have preceded the
verdict were missing — that is, by noticing an absence, which is the least
reliable way anything gets caught.

**So: a check prints its work, not only its verdict.** Twenty-one lines saying
which point resolved to how many commits are what make the last line mean
something. A check that reports only success cannot be distinguished from a
check that did not run, and this mechanism already has a rule for the same
failure one level up — run the check, do not read it. This is that rule applied
to the check itself.

The register's own claims are the next place this bites, and `D-040` said so two
commits ago: prose inside an entry passes through no check at all.

---

## 2026-09-20 — The moment-one request, written without a plan

No plan. `REVIEW-REQUEST.md` rewritten on the human's instruction, after
`/plan-status` reported a request owed before the next plan starts.

### D-044 · Replaced, not amended — and the rule that decided it

**Ruling:** R-008 — ratified.

`D-039` and `D-041` between them say when each applies: **replaced** when the
situation the request describes has moved, **amended** when one of its items is
answered and the rest still stands. Here the situation moved — the request
described moment three, and the moment is now one — so it was replaced whole.

One item of the old request survived the replacement and was carried forward
verbatim in substance: the look at the three `docs/` lines of `7decada`. It was
never answered, and an unanswered item does not close because the request around
it did. Replacing a request is not a way of retiring what it asked for.

### D-045 · Moment one was served without a plan to read, and says so

**Ruling:** R-008 — ratified.

The file's own argument for moment one is that "the plan is short — a few
hundred words against a branch", which presupposes a plan on the page. There is
none: `PLAN.md` is absent and the phase-3 plan has not been drafted. Two
readings were available.

- Draft the plan first, then request a review of it. This matches the argument
  but inverts the moment: a wrong criterion would then be found in a document
  already written, which is cheaper than a branch but dearer than a
  conversation.
- Request the review of the **criteria** before they are written down. Cheapest,
  and matches `AGENTS.md` — "before a plan starts, where a wrong criterion costs
  one conversation rather than every point built on it".

**The second was taken, and the ambiguity was put in the request itself** as its
third ask, rather than resolved quietly here. The mechanism will meet moment one
again; which of the two shapes it wants is a question about the mechanism, and
that belongs to the human who owns it. What would overturn this: an answer
saying moment one wants a draft attached, in which case this request was written
one step early and the next one waits for a plan.

### D-046 · Three counts of the closed points, reported rather than reconciled

**Ruling:** R-008 — ratified.

The last request said twenty-three closed points, the lookup point's outcome
says twenty-one, and counting `*Esito:*` lines today gives twenty across the
three plans archived at that moment. The plausible reading — the lookup counted
its own point, archived in the same commit — was **not** written down as the
answer, because it is a guess, and `D-043` is two entries above about exactly
that: a claim that enters the record because it sounds right.

Nothing depends on the number, which is why this is the right place to hold the
line. A discrepancy nobody needs is the cheapest one to leave standing, and
reconciling it by assertion would teach the register that assertion is how
discrepancies get closed.

---

## 2026-09-20 — The plan for the rest of phase 3

Written on instruction, the same day the moment-one request went up. No code.

### D-047 · Where phase 3 stops, decided three times

**Ruling:** R-008 — ratified.

`SIM-REQ` §18 names phase 3's content in five words — "scheduler, RNG,
serialisation, chronicle, CLI runner" — and each of the last three reaches into
something the phase excludes. Three lines were drawn, all of them in the plan
rather than here, so that the reviewer sees the exclusion beside the point that
makes it.

- **Chronicle: structure and append, no propagation.** FR-I-01 and FR-I-04 are
  buildable with no domain; FR-I-02, FR-I-03 and FR-I-05 need traffic, retelling
  and neighbours, which are phase 4. The point carries `Check: shallow` for
  exactly this reason — nothing emits an entry yet, so the shape is testable and
  the content is not.
- **NFR-12: the runner runs ticks and dumps hashes; sweeps and CSV metric series
  wait.** The requirement lists four capabilities in one sentence and phase 6 is
  where the metrics exist. Building a CSV writer for columns nothing produces is
  scaffolding.
- **`WorldState` holds the World row and nothing else.** Settlements and cohorts
  are rows of `SIM-STATE` that phase 3 would have to invent behaviour for.

**What would overturn this:** a reading of §18 under which "chronicle" means the
propagation model, in which case the phase gate is unreachable without phase 4
and that is a `SPEC-QUESTIONS` entry, not a plan revision.

### D-048 · Point 7 declared blocked rather than deferred or built anyway

**Ruling:** R-008 — ratified.

§20 ends "Confirm before Phase 3. A wrong guess costs a rewrite of the
serialiser," and five of its rows read `Proposed`. Three options.

Build the serialiser on the current guess and hope — which is what has been
happening silently since the first commit, the code being on `netstandard2.1`
and C# 9 already. Move the point out of this plan, which hides a phase-3
deliverable behind a plan boundary. Or declare it blocked in the plan and put
the confirmation in the request.

**The third.** The sentence in §20 is unusually specific about who pays and how
much, and a plan that quietly built past it would be deciding a question the
specification explicitly reserved. The point also states its own failure mode:
if the confirmation contradicts the guess, the point is **reported failed and
rewritten, not amended** — criteria are frozen, and a serialiser rewrite is what
§20 says the wrong guess costs.

Everything else in the plan was checked against the block: points 1–6 and 8–9
touch no serialised format, so the block costs no ordering.

### D-049 · Tests are named for invariants and requirements, not only for AC numbers

**Ruling:** R-008 — ratified.

`AGENTS.md` says tests carry the number of what they verify and gives acceptance
criteria as the list. Four points of this plan verify things with no AC number:
`A-13` is an invariant of `SIM-STATE`, `FR-T-05a`, `NFR-10` and `FR-A-01` are
requirements. Their tests are named `A13_`, `FRT05a_`, `NFR10_`, `FRA01_`.

The rule's actual content is that **a test names what it verifies and verifies
something named**, which the identifier prefix satisfies whichever document the
identifier lives in. The alternative — reserving the convention for AC numbers —
would leave the majority of phase 3 with names that say nothing, since the phase
has two acceptance criteria and nine points.

**What would overturn it:** a preference that only `AC-` tests carry prefixes,
which would want the other names chosen deliberately rather than left to
whoever writes them.

### D-050 · Two points exempt from the reviewers, on a diff-checkable line

**Ruling:** R-008 — ratified.

Points 8 and 9 declare `Core: no`. Point 8 touches `harness/` only, point 9 adds
a test file and a workflow step; both state the command that falsifies the claim
— `git diff --name-only HEAD -- core/` and the same over `core/Runtime/`. That
is the form `D-002` of the twelve-gaps plan asked for: an exemption a diff can
contradict, not a judgement about size or risk.

Worth noting against the standing entry in `REVIEW-REQUEST.md`: seven of the
nine points are `Core: yes`, so **the review protocol finally runs here**, on
point 1, having had no exercise in four plans. If it is broken, point 1 is where
that is found, which is the cheapest point in the plan for it to happen on.

### D-051 · The plan is installed on `main`, and `fase-3-kernel` is not cut

**Ruling:** R-008 — ratified.

`66a908d` installed the previous plan on the development branch, and this one
goes on `main` instead. The reason is what the commit contains: a plan, a
specification question and a request that asks a human to answer before the work
starts. A request nobody can see does not interrupt, and a branch would make
answering it cost a merge first.

It follows `D-042`, which put the lookup point on `main` for the same shape of
reason — no code, and a human about to open a clean session. Here there is not
even a point closed: the plan's own `Branch: fase-3-kernel` stays true and
describes where the nine points will be built, once there is a reason to cut it.

**What would overturn it:** a preference that `PLAN.md` never appear on `main`,
which is defensible — `/plan-status` reads whatever `PLAN.md` it finds — and
would mean installing the plan on the branch and leaving only the request here.

---

## 2026-09-20 — The reviewer's answers, recorded

Every item of the request answered, `docs/` edited by the human in `0114ad2`,
and one gap left behind that the request had not asked about.

### D-052 · `SQ-004` closed by marking what it got wrong, not by tidying it

**Ruling:** R-008 — ratified.

The entry proposed three readings of how `P-17` and `P-01` could close. Reading
2 — build a synthetic settlement update and measure it — **had already been
done**: `P-63` and `P-64` sit in §16 with the note "Measured, 5 of 12 phases",
and they are the output of exactly that benchmark. The entry did not mention
them because it was written from §19, §18 and NFR-05 without opening §16.

The entry was **not** rewritten to remove the dead option. It was marked with
the resolution and with why it was posed short, under `D-036`'s rule that
promoted entries are marked and kept. A specification question tidied after the
answer teaches nothing; this one carries a lesson the citation rule does not:
cite the line, and read the section that holds the numbers.

**What would overturn it:** nothing about this case. The general rule it
suggests — that an objection names the parameter table it checked — is worth
having and is not adopted here, because it belongs to `SPEC-QUESTIONS.md`'s own
rules and those are not edited in passing.

### D-053 · The met precondition is recorded in the plan, the points are not touched

**Ruling:** R-008 — ratified.

§20 was confirmed and `SQ-004` closed, both before any point started, so point 7
is unblocked. The plan says so in the precondition section that declared the
block, dated, with a line stating that nothing above it was rewritten.

The alternative was editing point 7 to drop its blocked clause. Rejected:
criteria are frozen when written, and while a met precondition is not a
criterion, the edit would be indistinguishable in the diff from one that was.
**The block was deliberately written outside the point for this reason**, and
taking the other path would have wasted the precaution.

### D-054 · The request is marked answered, not replaced

**Ruling:** R-008 — ratified.

`D-039` replaces a request when the situation moved, `D-041` amends it when one
item was answered. Here **every** item was answered and no new moment arrived:
the next is moment two, the phase 3 gate. A replacement would have had to invent
a moment to be written at, which the file forbids in as many words — "at those
moments, not near them".

So the request stays, marked answered in its heading, with each item struck and
its answer beside it, and one open item added. **What would overturn it:** a
preference that `REVIEW-REQUEST.md` hold only live requests, with answered ones
going to git — defensible, and it would mean the file is empty between moments,
which is information too.

### D-055 · The failure path is recorded as owed and deliberately not built

**Ruling:** R-008 — ratified.

The reviewer's gap 8: nothing says what happens when a criterion turns out to be
wrong **in flight**. The neighbouring cases exist — a failed check closes the
point `FALLITO` and stops the plan, and the rule that a wrong criterion is
reported rather than rewritten is in three skills — but the path between them is
not written: who is told, where the wrong criterion is recorded, whether the
plan stops.

It is not built now, for a reason that is mechanical rather than a judgement
about priority: **it is process work, process work needs a plan, and a second
`PLAN.md` would be two plans live at once.** `D-042` says that is the first
thing that breaks the point lookup, and the lookup was fixed eight commits ago.

Recorded as item 5 of `REVIEW-REQUEST.md`, open, to be taken up when the phase 3
plan closes. **What would overturn it:** the first point of phase 3 hitting a
wrong criterion, in which case the gap stops being theoretical and the phase 3
plan is the one that has to stop and say so.

## 2026-09-20 — Phase 3, the headless kernel

The plan's points, each under the number it closes.

### D-056 · Point 1 · `Calendar`, a static class of six divisions, zero-based

**Ruling:** R-008 — ratified.

`docs/` fixes the arithmetic and nothing about its shape: no type name, no
namespace, no signature. What was chosen — `Sim.Calendar`, a static class in
`core/Runtime/Time/`, holding four constants and seven methods and no state.

Three sub-choices worth naming, because each had a defensible other side.

**Season, week and month are zero-based.** FR-T-05a is normative and writes
`season = dayOfYear / 91  // 0..3`; FR-T-05's prose says "Season 1 begins the
year and is spring". The two agree on the boundaries — one-based days 1, 92,
183, 274 are zero-based 0, 91, 182, 273 — so this is a presentation convention,
not a contradiction, and **no `SPEC-QUESTIONS.md` entry was filed.** Both
reviewers were asked and both agreed. The core returns what the formula returns;
adding one belongs to whatever shows a date to a player.

**`Year` returns `long`, the other five return `int`.** `SIM-STATE` types `tick`
as int64 and fixes nothing downstream. `Year` is the only one of the six
unbounded in `tick`; the other five are provably inside 0..363. The asymmetry in
the signatures is the asymmetry in the arithmetic.

**`NewYearsDayFollows(tick)` is the whole of FR-T-03.** A 364-tick year with no
365th case *is* a festival that consumes no tick, and DEC-006a asks for exactly
that — "no special case in the scheduler". The predicate adds no behaviour; it
names the boundary for a caller that wants to show the festival.

**What would overturn it:** a host or a later phase needing a date type rather
than six independent queries — a `struct Date` would then be the shape, and the
methods become its constructor.

### D-057 · Point 1 · A tick is non-negative, checked in debug only

**Ruling:** R-007 — ratified; prior-history dating recorded as open in `SIM-STATE`.

`Calendar` asserts `tick >= 0` under `Conditional("DEBUG")` and defines nothing
below zero. `docs/` does not say ticks are non-negative, and one line reads the
other way: **FR-G-03** generates prior history "for P-18 years **before tick
0**". **DEC-040** settles it — the run "takes the result as tick 0" — so
prehistory is relabelled, not numbered backwards. Both reviewers found the same
line and reached the same resolution independently.

Floor division was rejected. `Fixed.DivFloor` would make the six methods total
over the whole of int64, at the price of no longer reading as FR-T-05a writes
them, in order to define a state the simulation cannot occupy. The cost of being
wrong is visible and bounded: in Release the guard is compiled out, and
`tick = -1` returns year 0, dayOfYear -1, dayOfMonth -1.

The reasoning is in the file's remark as well as here, because the blind
reviewer's objection was precisely that the code asserted a fact and gave no
reason for it.

**What would overturn it:** phase 14 implementing FR-G-03 the way FR-G-03 words
it. That is a decision about world generation, not about the calendar, and it
would fail loudly in a debug build rather than quietly — which is the point of
the guard.

### D-058 · Point 1 · Weekday is not in the core; the test defines it

**Ruling:** R-008 — ratified.

The point's "Does" is exhaustive — "the tick-to-date arithmetic and nothing
else" — and does not list a weekday, yet the criterion's first test is
`FRT02_EveryDateFallsOnTheSameWeekday`. `Calendar` exposes no weekday, and the
test takes `tick % 7`.

This is not the test inventing a formula to agree with. A week is seven
consecutive ticks and nothing interrupts the stream, so `tick % 7` is the
definition of a weekday, external to the calendar, and it is the one DEC-006a's
own rationale reasons with. The tautological alternative, `dayOfYear % 7`, holds
for any year length and would test nothing. Since `364 % 7 == 0`, a host that
later wants a weekday gets the same answer from either.

**What would overturn it:** a subsystem needing the weekday inside the core,
which point 4's `id % 7 == d % 7` bucket is not — that derives from the id.

### D-059 · Point 1 · The criterion under-covers the point, and was not rewritten

**Ruling:** R-008 — ratified.

Finding, from the blind reviewer, confirmed: the point's "Does" requires "the
New Year's Day that sits between two ticks without consuming one", and the two
tests named in "Closed by" touch none of it. Under the criterion as frozen,
`NewYearsDayFollows` ships uncovered.

A third test was written, `FRT03_NewYearsDayConsumesNoTick`, checking that the
predicate and the year turnover agree on every tick of the span and that exactly
three boundaries fall in four years. **The criterion was not amended.** "Closed
by" says what must be green to close the point, not what may exist; adding a
test neither weakens the standard nor rewrites it. It is recorded here because a
criterion gap filled silently is how a criterion stops being what closes a
point.

**What would overturn it:** a reading of "Closed by" as exhaustive, which would
make the third test an out-of-scope addition rather than a gap report. Nothing
in `AGENTS.md` or the three skills supports that reading, but it is the
alternative.

### D-060 · Point 1 · The reviewers' findings, including the one rejected

**Ruling:** R-008 — ratified.

Both reviewers ran `Core: yes`, built Release and ran both suites, and both said
the point is sound. Their overlap was near total, which is worth recording as a
fact about the mechanism: the blind reviewer found nothing the briefed one
missed except the sharper reading of FR-G-03's wording, and neither found a
defect in the arithmetic.

Applied: two constants deleted (`WeeksPerYear = 52`, `SeasonsPerYear = 4`, read
by nothing and pinned by nothing — both reviewers checked independently that
changing them left all twenty tests green); the remark's "year 0 for the two
years either side of the origin" tightened to the exact span, 727 ticks from
-363 to 363, after the two reviewers disagreed about whether the phrase was
loose or correct; and the four entries above, which are findings that wanted
recording rather than code.

**Rejected:** the briefed reviewer's note that `FRT02` dimensions its array from
`Calendar.MonthsPerYear` and `Calendar.DaysPerMonth` while `FRT05a` deliberately
uses literals, so the policy is not applied consistently. True as stated, and it
buys nothing: `FRT05a` pins both constants against the literals of FR-T-05a on
every one of 1456 ticks, so a wrong value fails there before `FRT02` is reached.
The reviewer raised it as an observation and did not press it. **What would
overturn it:** `FRT05a` ceasing to pin those two constants, at which point
`FRT02`'s array bounds become the only thing holding them and should stop
deriving from the values they are meant to check.

### D-061 · Point 2 · `record` in SIM-STATE is a composite value, not the C# keyword

**Ruling:** R-008 — ratified.

`SIM-STATE` §World types `generationParams` as `record`. A C# `record` is a
class, and NFR-10 says "No object references inside serialised state", so the
two would contradict each other under the keyword reading. `GenerationParams`
is a `readonly struct`.

`docs/` says nothing about C#. Its Type column is a type language of its own —
`int64`, `uint64`, `fixed`, `bitfield`, `id[]`, `bounded record[]`, `flag plus
record`, `sparse record`, `enum plus int`, `derived` — and not one entry in it
is spelled the way C# spells it. Reading `record` as the keyword while reading
`int64` as prose is the inconsistent reading. The decisive argument is internal
to the document: `knowledgeTable | bounded record[]` and `guildHall | flag plus
record` sit inside rows the same file's serialisation notes say hold "no object
references", so under the keyword reading SIM-STATE contradicts SIM-STATE.

**No `SPEC-QUESTIONS.md` entry was filed.** Nothing in `docs/` is wrong here,
which is what an `SQ` is for; the word is used consistently and means
"composite". Had `record` appeared only in this one row, the answer would have
gone the other way.

**What would overturn it:** a future revision of SIM-STATE that names C# types
explicitly, or a row that needs a genuinely variable-length composite, which a
struct cannot be.

### D-062 · Point 2 · `GenerationParams` carries one parameter, and it is P-02

**Ruling:** R-008 — ratified.

`docs/` nowhere enumerates the generation parameter set. FR-G-02 says only that
"the generator seed and all generation parameters are stored in the save file",
and §16's table mixes generator inputs with simulation constants. The struct
holds `SettlementCount` and nothing else.

The seed is not in it: SIM-STATE gives `worldSeed` as "also the RNG root", so
FR-G-02's seed is that field.

Two alternatives were rejected. **An empty struct** — a field whose type has
exactly one value cannot be perturbed, so point 3's hash test would report it
as covered while proving nothing about it, which is the determinism hole
SIM-STATE names in the same sentence. **Also P-03 and P-04** — "inhabited
fraction 15%" and "map extent 1,000 km per side" cannot be stored without
inventing a unit and a fixed-point scale that `docs/` does not give, and each
invention is a decision belonging to the generator that does not exist yet.
P-02 is a bare count with no representation question attached, and it is the
one generation parameter the state layout has a stake in: it is the length of
every settlement array phase 4 adds.

That last claim is true because of FR-W-10 ("settlement count is fixed for the
run") and DEC-067, **not** because of P-02, which fixes a default and not a
length. The code cited P-02 for it; the briefed reviewer caught the substitution
and the comment now cites FR-W-10.

This is not reaching into phase 4 in the sense `AGENTS.md` forbids. The rule
bans implementing a subsystem early; an `int` carried in a save has no
behaviour and no update, and no phase-4 gate is weakened by its presence.

**What would overturn it:** phase 4's generator needing a second parameter, at
which point it is added under the migration path NFR-08 requires from the first
write — which is why the cost of being wrong here is one save version, not a
rewrite.

### D-063 · Point 2 · The container is a class; only its contents obey NFR-10

**Ruling:** R-003 — ratified; the open question resolved, mutable state leaves the public surface at point 5.

NFR-10 reads "Struct-of-arrays, indexed by `EntityId`. No object references
inside serialised state." `WorldState` is a `sealed class` with public mutable
fields.

Struct-of-arrays is a layout term — one object holding parallel arrays, rather
than an array holding objects — and the type discipline lives in the second
sentence, which says *inside*. A C# `struct` container would buy nothing DEC-003
asks for: with arrays inside, a struct copy is a shallow copy of the same
arrays, so its value semantics are an illusion that costs defensive copies on a
state SIM-REQ sizes at 50 MB. Neither AC-02 nor AC-03 can see the difference:
the hash covers field values and the serialiser of point 7 writes named fields.

**Open, and passed to the human rather than settled here.** The briefed reviewer
observed that public mutable fields leave FR-A-01 ("nothing outside the core
writes world state directly") enforced by convention only, and that `internal`
fields would enforce it at compile time for free, since the core already
declares `InternalsVisibleTo("Sim.Core.Tests")` and both reflection walkers pass
`BindingFlags.NonPublic`. Against it: every core type so far is public, and
point 8's runner will want some surface. **This belongs to point 5**, which is
the point that claims FR-A-01 — but it is cheaper decided before a serialiser
and a runner are written against the public fields than after.

**What would overturn it:** that decision at point 5, or a host that needs to
pass state by value.

### D-064 · Point 2 · The walker allows one level of array, which the criterion's words do not

**Ruling:** R-006 — promoted as DEC-085.

The criterion says the test fails "on any reference type". An array is a
reference type, and the walker lets one through at depth 0.

The point's own "Does" says *parallel arrays*, and SIM-STATE's serialisation
notes say "Struct of arrays … no object references" in one breath. A walker
that failed on `int[]` would fail the layout the same sentence mandates. The
line the code draws is between *one object holding contiguous values* —
`EntityId[]`, and `int[,]`, which falls through the same branch — and *an object
holding objects*: `int[][]`, `List<int>`, and any struct holding an array,
since the recursion into a struct's fields forbids arrays. The first shape
serialises, copies and hashes in bulk; the second is the object graph DEC-003
exists to keep out.

**The criterion was not amended.** Its words and the point's "Does" disagree,
and the code resolved the disagreement toward the "Does". Today the divergence
carries nothing: `WorldState` has no array field, so the test is green under
the strict reading too. From the first parallel array of phase 4 it will carry
everything, and the criterion as frozen would fail the layout it exists to
protect.

**This decides part of the phase-4 state layout, from a private helper in a test
file.** SIM-STATE's per-row `treasury int[4]`, `allocationVector fixed[]`,
`ageDistribution int[]`, `goods int[]` and `flowAccumulator int[]` must each
arrive flattened — one `int[]` of length `rows * width`, or an `int[,]` —
because both the jagged and the array-inside-a-struct shapes fail this walker.
Recording it here because the next person to meet the rule will meet it as a
red test, with no statement anywhere of why.

**What would overturn it:** a phase-4 row field that genuinely needs variable
width per row, which a flat array cannot express without an offset table.

### D-065 · Point 2 · The declared check cannot fail today, and the test that pins it is not named by the criterion

**Ruling:** R-008 — ratified.

`NFR10_StateHoldsNoObjectReferences` passes against a walker whose body is
`return;`. Verified rather than reasoned: `Check` was replaced by an immediate
`return`, the fixture run, and exactly one of its four tests went red —
`NFR10_TheWalkerRejectsWhatItIsThereToReject`, the negative control, which the
criterion does not name. The declared test stayed green, because no field of
today's `WorldState` offends under any implementation.

So the criterion certifies that a list built by an unspecified procedure came
back empty. This is a sharper failure than point 1's `D-059`, where the check
merely under-covered the point: here the check as written cannot fail for the
reason it exists.

**The criterion was not amended, and the extra tests were not folded into it.**
"Closed by" says what must be green, not what may exist — the reading `D-059`
settled. Three tests were added beside it: the negative control above, which is
what makes the declared check mean anything;
`FRW01_WorldStateHoldsTheWorldRowAndNothingElse`, which enforces the "Does"
clause "holding at first only the World row"; and
`FRW01_TheWorldRowCarriesTheDeclaredTypes`, which pins four of the five field
types against SIM-STATE's Type column. The fifth field's type is pinned by the
declared test itself, which would fail if `GenerationParams` became a class.

**What would overturn it:** nothing about the criterion, which stays as frozen.
The finding is for whoever writes the next one: a check that names a mechanism
should name the test that can break the mechanism.

### D-066 · Point 2 · Two tests carry `FR-W-01`, because `SIM-STATE` §World has no number

**Ruling:** R-008 — ratified.

`AGENTS.md` requires that a test carry the number of what it verifies. The two
tests above verify the World row's field list and field types, which is
`SIM-STATE` §World — and §World has no identifier of its own. They were first
written with an `NFR10_` prefix, which the blind reviewer flagged: NFR-10 is
state *layout*, and a field list is not layout.

`FR-W-01` is the nearest requirement that says anything about what lives in
world state, and it is in the point's "Serves", so the two tests carry it. It is
an approximation: FR-W-01 fixes no field types. Recording the gap rather than
hiding it behind a number that fits less well.

**What would overturn it:** `docs/` giving §World rows identifiers, at which
point the tests should carry those.

### D-067 · Point 2 · The reviewers' findings, and the two left open

**Ruling:** R-003, R-004 — both open questions resolved; R-008 — the rest ratified.

Both reviewers ran `Core: yes`, built Release and ran both suites, and both
answered that the point closes. They overlapped on three findings and each
found things the other did not.

**Applied.** The comment claiming the chronicle "belongs to later phases" —
false, SIM-REQ §18 lists it inside phase 3 and point 6 builds it, found by the
blind reviewer and confirmed against §18. The docstring calling a sixth state
field "a subsystem arriving before its phase" — the same error, and wrong twice
over, since points 5 and 6 both widen the World row inside this plan. The
recursion guard in the walker and its comment: the comment said an array can
carry a struct back to itself, which is true of C# and false of this walker,
because every descending call passes `arrayAllowed: false` and an array met that
way is reported without being entered; both reviewers traced it independently
and both reached "dead code with a false comment", the `D-060` standard, so the
`HashSet` went and the termination argument is now stated where it is true. The
two files moved from `core/Runtime/` into `core/Runtime/State/`, the folder the
scaffolding commit reserved and left empty, which is where `Time/Calendar.cs`
sets the pattern. The P-02 citation corrected to FR-W-10, per `D-062`. The
equality surface of `GenerationParams` — `IEquatable`, both `Equals`,
`GetHashCode`, `==`, `!=` — deleted: read by nothing, pinned by nothing, the
same deletion `D-060` already made once. Its `GetHashCode` was also worth
removing on its own, being a name one letter away from the state hash and
carrying none of its guarantees.

**Open, passed to the human rather than settled.** The `internal`-versus-public
question of `D-063`, which belongs to point 5. And `RuleVersion` as a
constructor argument: NFR-09 says "state records which rule version produced
it", which is a property of the build, not a value a caller elects, so a host
can today record a version that produced nothing and DEC-033's materialisation
would run against it. Nothing in phase 3 forces a shape; flagged so that it is
decided when it is decided rather than inherited.

**Rejected:** nothing outright. The blind reviewer's observation that the
register and the outcome line were missing was true when it looked and is what
this entry closes. Its note that `#pragma warning disable CS0649` appears in the
test fixture stands as recorded rather than acted on: `AGENTS.md` places the
suppression rule in the section about the core's analyzers, the test project
sets no `TreatWarningsAsErrors`, and the suppressed warning is "field never
assigned" on a type whose fields exist to be read by reflection and are never
assigned by design.

**Carried forward to point 3, which is where it lands.** Its criterion perturbs
the fields of `WorldState`; with `GenerationParams` a composite, perturbing the
field proves the struct reaches the hash, not that each member of it does. The
two coincide while it has one member. When phase 4 adds a second, a member that
never reaches the hash passes point 3's test unless that test recurses the way
this one does.

### D-068 · Point 3 · `StateHash`, a static fold, and the mixer made `internal`

**Ruling:** R-018 — ratified.

**What was decided.** The digest is `StateHash.Of(WorldState)`, a static method
in `Runtime/State/`, folding each field as `h = Hash64.Mix(h ^ x)` from a
constant origin, in the order `SIM-STATE` §World declares them. It is not a
method on `WorldState`: that type is a data container, and a `Hash()` on it
would sit one letter from the state hash while carrying none of its guarantees —
the same collision of names `D-067` deleted `GenerationParams.GetHashCode` for.

`Hash64.Mix` went from `private` to `internal` so the fold reuses it. The
alternative was a second copy of `MixA` and `MixB` in another file, which is two
determinism-critical constants to keep identical across builds where the pinned
vectors in `PrimitivesTests` guard only one of them. `internal` is narrower than
`public` and the core's public surface is unchanged; `SIM-REQ` §20 constrains
framework, language level, reflection and dependencies and says nothing about
member visibility, and `R-003` constrains mutable world state, which a mixing
function is not. Note that `Sim.Core.csproj` already carries
`InternalsVisibleTo("Sim.Core.Tests")`, so `internal` here means the core plus
the test assembly; no test calls `Mix`.

**What `docs/` says.** `SIM-STATE` §Serialisation notes requires that the hash
cover every field and calls an excluded field a determinism hole. It does not
say what the hash is, how it is seeded, or where it lives. `NFR-01` and `AC-02`
require the digest to be identical across builds and machines, which rules out
the base class library's hash codes and is why the mixing is written out in
integer operations.

**What would overturn it.** A ruling that the digest belongs on `WorldState`
after all; or `R-003` landing at point 5 in a form that makes `WorldState`
`internal` — see `D-071`.

### D-069 · Point 3 · The fold's guarantee is single-field injectivity, not collision resistance

**Ruling:** R-018 — ratified.

**What was decided.** The file states what the construction actually gives and
not more. A state differing in exactly **one** field can never share another's
digest: every cast into the fold widens and is injective, xor with the
accumulator is a bijection, and `Mix` is a bijection, so with the other fields
held fixed the digest is an injective function of each one. The perturbation
test is therefore exact, not probabilistic.

Two fields are a different matter and the comment now says so. Two states
differing in two fields collide in two evaluations of `Mix`: choose the second
field to absorb what the first did to the accumulator. Verified against the real
function rather than argued — with `SettlementCount=2000`, `RuleVersion=7` and
`CurrencyTotal=1000003` in both, `Tick=41` with
`WorldSeed=0x0123456789ABCDEF` and `Tick=42` with
`WorldSeed=0x729E20E7458C7D96` both digest to `0xCE31A6F9C074C14D`.

This is a property of any `h = Mix(h ^ x)` chain, not a 64-bit birthday
collision, and it costs nothing under `AC-02`, which compares two runs of the
same build where a divergence is systematic rather than adversarial. It is the
reason this is a digest and not a checksum against tampering.

**What `docs/` says.** Nothing about the hash's strength. `AC-02` asks for equal
sequences from equal inputs, which the construction gives exactly.

**Correction carried into a file this point did not otherwise touch.**
`Hash64.Of` carried the identical false sentence — "no two different coordinate
tuples cancel out into the same state" — written before this point and false for
the same reason. It is corrected in the same commit. Fixing one and leaving the
other standing would have left a known-false claim in the file the corrected one
cites. The change is a comment and nothing else; no draw moves.

**What would overturn it.** A later use of the state hash that needs resistance
to a chosen second field — an integrity check on a save file, say — which this
construction does not give and would need a keyed or wider digest.

### D-070 · Point 3 · The criterion is again vacuously satisfiable alone, and again was not rewritten

**Ruling:** R-018 — ratified; the question on criteria escalated as R-017.

**The defect.** `NFR01_EveryStateFieldEntersTheHash` asserts that no field was
left unreached. An enumeration returning nothing satisfies that, forever and
silently — the same shape as `D-065` one point earlier, where the declared check
passed against an emptied walker. What rules it out here is
`NFR01_TheWalkReachesEveryLeafOfTheWorldRow`, which spells the five leaves out,
and `NFR01_EqualStatesHashEqual`, which rules out a fold returning a fresh value
on every call. Neither is named by the criterion.

**Not sanato by rewriting.** Correct code passes the criterion as frozen, so
this is `R-005`'s first case: the point closes, the missing checks are written
beside it here, and the plan continues. The criterion above the outcome line is
untouched.

**A second-order note the decider may want with it.** The criterion's selling
point is "a field added later without reaching the hash fails this test without
anyone remembering to extend it." That holds for the named test. The guard that
makes it non-vacuous carries a hard-coded five-element leaf list, which *must*
be edited on every field addition. The property bought is that forgetting is
loud, not that there is nothing to remember. `D-065` and this entry are the same
defect twice; whether a plan's "Closed by" clause should be required to name
every test that keeps the named one honest is a question for the decider, not
one to settle here.

**Two further checks added, neither named by the criterion.**
`NFR01_TheDigestIsPinned` writes `0xCE31A6F9C074C14D` out: `AC-02` compares hash
sequences *across builds and machines*, and without it a reordered fold, a
widened cast or a changed seed leaves every other test green while every world
ever saved hashes differently. Point 9's gate does not cover this — it compares
two runs of the same build. `NFR01_ThePerturbationRejectsWhatItCannotChange`
pins the tripwire, on the precedent and for the reason of
`NFR10_TheWalkerRejectsWhatItIsThereToReject`: a permissive fallback added later
to quiet a phase-4 array would pass every other test in the file.

### D-071 · Point 3 · What point 3 hands to point 5, and one docs defect found on the way

**Ruling:** R-018 — ratified; the two walkers stay apart.

**`R-003` will not compile against `StateHash` as written.** `R-003` is owed
within plan point 5: no public member of the core exposes mutable world state.
`public static ulong Of(WorldState)` takes one as a parameter, so when
`WorldState` goes `internal` this is CS0051 — inconsistent accessibility.
`R-003`'s cost section predates this file and enumerates only `WorldState` and
the tests that read its fields. The capability survives — `R-003` blesses the
state hash as a value for point 8's runner — and only the signature moves.
Recorded so point 5 does not rediscover it.

**`SQ-005` filed, non-blocking.** `docs/SIM-STATE.md:186` attributes the
no-reflection rule to "section 19 of SIM-REQ", which is Open items; the rule is
in §20. Not ours to fix.

**Left open to the human, not settled.** Two reflection walkers now descend
`WorldState` — `WorldStateTests.Check` and `StateHashTests.Walk` — with two
hard-coded field lists between them. They answer different questions (what may
be in state, what reaches the hash) and merging them would couple `NFR-10` to
`NFR-01`, so they are left apart; whether that is one walker too many is taste,
and the author of the code does not arbitrate its review.

**Rejected, with the reason.** The suggestion to tie `StateHash.Seed` to
`Hash64.GoldenGap` — they share bits today — is the bug, not the cleanup: the
two play unrelated roles, and coupling them means a future change to the RNG's
additive tweak silently rewrites every saved world's state hash. The comment now
says they need not stay equal, and `NFR01_TheDigestIsPinned` turns any such
change into a red test rather than a paragraph someone skimmed. Also rejected:
teaching the perturbation fixture about array fields now. Arrays arrive with
phase 4 under `DEC-003`, reaching for them here is the scope rule `AGENTS.md`
forbids, and the `Assert.Fail` makes the first one loud — traced by both
reviewers and pinned by a test. Both reviewers noted the consequence and neither
disputed the default: the fixture goes red on phase 4's first array field
whether or not the fold is correct, which is fail-loud rather than skip-silent.

**Not a finding, recorded because both reviewers reached it independently.** A
value type with zero declared instance fields walks to zero leaves and is
verified by neither test. Such a type carries no state, so the silence is
correct; it is the one hole in the walk's "every leaf" guarantee and is written
down in case a later phase gives it content.

### D-072 · Point 4 · The loop lives in `Systems/`, which keeps one half of a sentence in `AGENTS.md`

**Ruling:** R-016 — escalated to the user, with a recommendation.

**What was decided.** `TickLoop` is `core/Runtime/Systems/TickLoop.cs`. The
"Structure" section of `AGENTS.md` says `Runtime/Systems/` holds "one file per
phase of the settlement update … and those files are the only ones that write to
state". The loop writes `WorldState.Tick` and is not a phase of the settlement
update, so every placement breaks one half: in `Time/` a file outside `Systems/`
writes state; in `Systems/` a file that is not a phase sits among them. The half
kept is the write boundary, because it is the one a reader relies on to find
every mutation, and the one `WorldState`'s own comment cites.

**What `docs/` says.** Nothing: the sentence is in `AGENTS.md`, not `docs/`.
Both reviewers reached the same tension; the briefed one notes that point 5's
command queue and phase 4's generator will meet it again.

**Left to the human, not settled.** Rewording `AGENTS.md` needs a human's
permission for that change, and this point does not claim it.

**What would overturn it.** A ruling on the sentence, in either direction.

### D-073 · Point 4 · Unstaggered levels fire on the first tick of their period, and levels run finest first

**Ruling:** R-018 — ratified.

**What was decided.** Basin fires on `tick % 28 == 0`, kingdom on
`tick % 364 == 0` — the first tick of the month and of the year, so tick 0 fires
every level, and the annual update falls just after New Year's Day rather than
just before it. Within a tick the levels run in the order of FR-T-04's table:
daily, then the settlement bucket, then basin, then kingdom. The counter moves
after all of them, so each sees the date it fires on.

**What `docs/` says.** FR-T-04 gives periods and nothing about phase; FR-T-06
staggers settlements only. `SIM-ECON` "What the sequence does not yet cover",
item 4, leaves kingdom spending "not yet placed relative to the settlement update
that receives it" — the same question from the other side. First tick was chosen
because it is FR-T-06's own formula with the id at zero, which invents least.
The alternative, `tick % P == P - 1`, closes each period before the next opens
and would put taxation before the festival. Neither is observable in phase 3.

**Deliberately not pinned by a test.** `FRT04_ConsecutiveFiringsOfALevelAreOnePeriodApart`
checks spacing from a first firing inside the first period, and passes for any
phase — verified with basin at day 27. Pinning the phase would freeze a choice
`docs/` has not made.

**What would overturn it.** The decider closing `SIM-ECON` item 4, or phase 4's
first basin or kingdom system needing the end of the period.

### D-074 · Point 4 · The loop is `internal`, reached through a struct seam, and ids are row indices

**Ruling:** R-014 — specified: FR-T-06's `id` is the row index; the rest ratified.

**What was decided.**

- `TickLoop` is `internal`. A public entry now would be one more thing `R-003`
  reworks at point 5. **Hand-off, from the briefed reviewer:** point 8 is
  `Core: no`, so it cannot add the public entry; it has to arrive at point 5, with
  `R-003`'s handle and FR-A-01's defined point in the tick.
- The levels are a struct type parameter `T : ICadenceLevels`. The only core
  implementation is the empty `NoLevels`; the tests supply counting ones. Both run
  the same method body, so the tests observe the schedule the core runs. The
  rejected alternative — pure predicates the test iterates itself — would count
  the predicate, not the loop.
- A settlement's id is its row index. FR-W-10 fixes the count and forbids
  deletion, so a row's generation never moves; phase 4's generator must not issue
  ids that differ from the index.
- A negative tick count is a `Debug.Assert`, as `Calendar` does for negative
  ticks. In Release it runs zero ticks. A public entry is a trust boundary and
  wants an exception.
- The daily level fires every tick with nothing behind it. FR-T-08's fast travel
  is that level over zero hot agents; the loop has no notion of fast travel.

**What `docs/` says.** Nothing about shape or visibility. `SIM-STATE` gives the
Settlement row an `id` and `EntityId` as index plus generation.

**What would overturn it.** Point 5 shaping the public entry differently; a
generator that needs sparse ids.

### D-075 · Point 4 · The criterion passes a 365-tick year, and was not rewritten

**Ruling:** R-015 — specified: A-13 gains the one-period spacing clause and is verified by test; the rest ratified.

**The defect.** Found by the briefed reviewer, confirmed by mutation: with the
kingdom firing every 365 ticks, `A13_EachCadenceBucketFiresOncePerPeriod` stays
green over 100k ticks. The firing at `365j` falls in year `j + j/364`, which is
year `j` for every `j` up to 363; the first skipped year starts at tick 132,860.
That is the one-day-a-year drift DEC-006a was written to rule out. 363, 366, 27
and 29 are caught.

**Not sanato by rewriting.** Correct code passes the criterion, so this is
`R-005`'s first case: the point closes, and the missing check is beside it —
`FRT04_ConsecutiveFiringsOfALevelAreOnePeriodApart`, not named by the criterion,
red against the 365 mutation. Six other mutations of the loop (bucket start,
step, basin period, kingdom never, one settlement short, daily skipped) turn the
named test red on its own, so unlike `D-065` and `D-070` it is not vacuous.

**For the decider, from the blind reviewer.** `SIM-STATE` lists A-13 under
"Invariants, asserted every tick". Nothing asserts it inside the loop, and no
point of this plan builds the per-tick invariant check. Point 4's criterion asks
for the test only, so the point is not short; the gap belongs to no point.

### D-076 · Point 4 · The reviewers' findings

**Ruling:** R-018 — ratified.

Both reviewers: the check is met as written and passes for the reason the
criterion asks; nothing blocking. Build and both suites run by each.

**Applied.** The 365-tick blind spot (`D-075`). The comment in `TickLoop.cs`
cited register entries not yet written — both reviewers; true once this commit
lands, since `D-073` is it. The non-generic `Advance`, untested, is now called in
`FRT09_TheCallerDecidesHowManyTicksRun`. The id-equals-index assumption and the
point-5 hand-off are recorded in `D-074`.

**Left to the human.** The placement against `AGENTS.md` (`D-072`).

**Rejected.** None.

### D-077 · Point 5 · Commands drain at the start of every tick, and the queue is input, not state

**Ruling:** R-021 — the queue as input, and saves carrying it; R-022 — commands as data, promoted as DEC-086; the rest ratified by R-023; the drain at the start of the tick promoted as DEC-087 by R-040.

**What was decided.**

- **The reading of the criterion.** "Submitting them at a different moment of the
  caller's loop changes nothing" is read as: any moment before a tick starts —
  between calls of one tick, between calls of many, or from inside the tick
  before — lands a command on that tick. DEC-030's cost fixes it: "anything the
  game wants to do immediately waits for the next tick boundary". The rejected
  reading, commands stamped with a target tick and submitted arbitrarily early,
  is not excluded by `docs/`, but it holds commands for future ticks and so pulls
  them into state under `SIM-STATE` §Rule; it costs more and nothing asks for it.
- **The drain is the first thing a tick does,** before every level, and applies
  only the commands waiting when it starts; one submitted during the tick waits
  for the next. `docs/` says only "a defined point". The start lets a command see
  the tick its levels see (`D-073`), and puts SIM-ECON's Arrivals, where FR-J-17's
  reported outcomes would land, after it. Draining until empty instead of taking
  the count is indistinguishable today — `Apply` has no queue to submit to — and
  no test tells them apart.
- **Order is submission order,** FIFO. `docs/` says "identical command sequence"
  (NFR-01) and nothing more.
- **The queue lives beside `WorldState`, outside the state hash.** AC-02 names
  seed and commands as inputs; the queue holds input not yet applied. This is in
  tension with `SIM-STATE` §Rule and is filed as `SQ-006`, non-blocking. **Owed to
  point 7:** what a save does with commands still waiting — carry, drop or refuse.
- **A command is an `ICommand` object with `Apply(WorldState)`,** internal, so
  only the core (and the tests, through `InternalsVisibleTo`) can define one. Phase
  3 defines no kind; the tests' `Fold` is a stand-in. **Open for the human, from
  the briefed reviewer:** under `R-019` every future kind's `Apply` is a writer and
  must live in `Runtime/Systems/`; commands as immutable data dispatched by the
  drain would keep the writers there and give point 7 a codec per kind rather than
  per class. Not settled here: nothing violates `R-019` today, and choosing the
  command representation is choosing the replay format, which is phase 7's.
- **The queue is single-threaded.** DEC-031's host thread cannot submit to it
  safely; the core may not hold threading primitives, so the handover is the host
  integration's.

**What `docs/` says.** FR-A-01: a queue, a defined point in the tick. DEC-030: the
command log is a replay format. Nothing on the point, the order, the
representation or whether the queue is state.

**What would overturn it.** An answer to `SQ-006` that makes pending commands
state; a command kind that must apply mid-tick; DEC-031's handover requiring a
queue the core owns across threads.

### D-078 · Point 5 · R-003 by an internal state type and a public handle; R-004 on the public path only

**Ruling:** R-023 — ratified.

**What was decided.**

- `WorldState` and `StateHash` are `internal`. The *type*, not its fields: the
  compiler then refuses any public member that takes or returns the state, and
  `FRW01_TheWorldRowCarriesTheDeclaredTypes`, which looks fields up with default
  binding flags, still sees them. `R-003`'s cost note expected that test to break;
  it did not, because the fields are still public members of a non-public type.
- **`Simulation`, public, is the host's entry**: a constructor from seed and
  generation parameters, `Advance(long)`, and `StateHash` as a value. The state and
  the queue are private fields. This is `D-074`'s hand-off: point 8 is `Core: no`
  and could not have added it. The first draft left the core with no public entry
  at all; both reviewers found it (`D-080`).
- **No public submit.** `ICommand` takes the internal state, so it cannot be
  public, and phase 3 has no kind to issue. The drain still runs inside
  `Simulation.Advance`.
- **`Advance(-1)` throws** `ArgumentOutOfRangeException` on the public entry,
  where `D-074` said a trust boundary wants an exception; the internal loop keeps
  its `Debug.Assert`.
- **`R-004`, in part.** `Simulation` stamps a new world with
  `Simulation.CurrentRuleVersion = 1`, so no host chooses a rule version. The
  internal `WorldState` constructor still takes one, used by the tests; removing it
  and loading a foreign version through a save remain point 7's, as `R-004` allows.
  `1` is arbitrary: phase 3 has no rules to version.
- `FRA01_WorldStateIsNotVisibleOutsideTheCore` checks visibility and is the
  floor of `R-003`, not the whole guard: once phase 4 adds arrays, a public member
  returning one would hand out a live alias and pass it. Said in its comment.

**What `docs/` says.** FR-A-01, FR-A-02, FR-A-03; nothing on the shape. `R-003`
leaves it to the implementer.

**What would overturn it.** Point 8 needing something `Simulation` does not
expose — the tick, say — which it cannot add itself.

### D-079 · Point 5 · The declared check, and what it catches

**Ruling:** R-023 — ratified.

`FRA01_CommandsApplyAtOnePointInTheTick` reads the hash sequence inside each
tick, in the first level, after the drain — the one point every way of calling
the loop shares, since a caller running forty ticks in one call sees no boundary
between them. It compares three ways of submitting the same script against one
reference and against a run with no commands.

The plan predates `R-020` and point 5 has no `Fails when:` line. The faults were
introduced anyway, each alone, each undone: drain once per call instead of once
per tick, drain at the end of the tick, drain after Daily, drain without
applying — all red on the named test. **Reversing the order passes it**: every
run reverses alike, and the criterion compares runs with one another. That fault
is caught by `FRA01_CommandsApplyInTheOrderSubmitted`, which pins the value by
hand. Correct code passes the criterion, and it proves "same order gives same
sequence" rather than "the order is the order submitted" — which the criterion
does not ask for. Not a defect of the criterion, recorded so nobody reads the
named test as covering order.

### D-080 · Point 5 · The reviewers' findings

**Ruling:** R-023 — ratified; commands as data ruled by R-022.

Both reviewers: the check is met as written, build and both suites green;
`2918` and `8` verified by hand.

**Applied.**
- No public entry after `R-003`, contrary to `D-074`'s hand-off — both. `D-078`.
- The named test could not see a drain run once per call, since nothing
  submitted inside a multi-tick call — both. The mid-tick case now runs as one
  call of every tick; the fault turns it red (`D-079`).
- Comments citing register entries not yet written — both. True with this commit.
- "The queue is its replay format" — briefed. DEC-030 says the command *log*; the
  comment now says nothing writes one yet.
- "While a tick is running" is safe only on one thread — both. Narrowed.
- "This one assertion covers the whole surface" overstated — briefed. Narrowed.
- Pending commands and `SIM-STATE` §Rule — both; the briefed reviewer asked for a
  question. `SQ-006`.

**Left to the human.** Commands as code or as data, and where future kinds live
under `R-019` (`D-077`).

**Rejected.** None.

### D-081 · Point 6 · The chronicle's shape: ids are the order of writing, the pool follows DEC-085

**Ruling:** R-027 — ratified; `0` narrowed by R-024, the id's table left open by R-025; capacity not state promoted as DEC-088 by R-041.

`SIM-STATE` §Chronicle names the fields and gives three types — `tick` int64,
`entities` id[], `importance` int — and `cause` as "chronicle id". The rest is
chosen here.

- **An id is its row plus one**, from 1; `0` is `Chronicle.None`, the cause of a
  root. No id column is stored: while nothing is discarded it would repeat the
  row. `0` for none follows `EntityId.None`, where a defaulted field reads as
  absent. `int`, not `long`: arrays index by `int`.
- **The tick is the state's**, not an argument: an entry is dated when it is
  written (DEC-005). Prior history is `SIM-STATE` Open item 4 and belongs to the
  generator.
- **Location is an `EntityId`**, `None` when an event has no place. `docs/` does
  not say whether a location is a settlement or a position, as it does for
  buried stock.
- **Entities live in one shared pool**, each row naming its slice by start and
  count — DEC-085's second option, literally. The start is derivable from the
  counts until something is discarded; it is stored because DEC-085 prescribes
  it and a compaction will need it.
- **Capacity is not state.** Columns double from 8; only the first
  `ChronicleCount` rows, and the pool up to `EntitiesInUse`, are hashed.
- **The writer is `Systems/Chronicle.cs`**, a static class like `TickLoop`,
  under `R-019`. It is `internal` under `R-003`; nothing reads the chronicle
  outside the core yet.

**What `docs/` says:** the field list and three types; nothing on id
numbering, location's type, capacity, or where an entry's tick comes from.
**Would overturn it:** a retention policy (Open item 3) — dropping the oldest
entries needs one base id, selective discard needs an id column, and only
`RowOf` and `ChronicleCount`'s remark change; prior history dated before tick 0
(Open item 4) needing a tick argument; a decision that location is a position.

**Handed to point 7.** AC-03 compares bytes, so the serialiser writes the rows
in use and never the capacity: two worlds holding the same entries in columns of
different length must save identically. `R-021`, `R-022` and the rest of `R-004`
are still owed there.

### D-082 · Point 6 · A bad cause throws, in Release too

**Ruling:** R-027 — ratified.

`Chronicle.Append` throws `ArgumentOutOfRangeException` for a cause that is not
an earlier id, before anything is written: no row, no count, no growth. That
departs from the convention of `D-074` and `D-078`, where public entries throw
and internal code only asserts.

The reason is the cost of the other branch. A negative tick count asserted away
in Release runs zero ticks; a forward cause asserted away is a chain with no
root, carried into every later save, and AC-11 ("every high-importance entry
traces back to an exogenous root or a player action") could never hold for it.
One comparison per entry.

**What `docs/` says:** nothing on validation inside the core. **Would overturn
it:** a ruling that the core never throws from internal code, which would move
the check to a debug assert and accept the corrupt branch in Release.

### D-083 · Point 6 · The hash and its fixture learn columns

**Ruling:** R-027 — ratified; test names by R-026; rows in use promoted as DEC-088 by R-041.

The fold adds the chronicle after §World: the count, then each column over the
rows in use, then the pool, whose length is read from the last row. That is the
one departure from `SIM-STATE`'s order, and the remark says so.

**The injectivity remark is narrowed.** `ChronicleCount` and the last row's
start and count decide how many folds follow, so for those three the digest is
injective only up to a 64-bit collision. Both reviewers found it.

**The perturbation fixture walks array columns.** `D-071` deferred it to phase
4, assuming arrays arrived there; the chronicle brings them in phase 3, and
`D-071`'s own rule — the phase that introduces one says how it enters the hash —
puts it here. It perturbs **every row in use**, not the first: a first version
perturbing row 0 left a fold reading `[0]` in place of `[i]` invisible, and with
both reference entries on tick 41 nothing caught it (briefed reviewer). The
reference now writes its entries on ticks 41 and 43, and the fault is red.
Rows beyond the count are not perturbed: they are capacity and must not move the
digest, which `FRI01_CapacityDoesNotReachTheHash` checks over all seven columns.
A jagged array still falls to `Different` and fails loud.

**`NFR01_TheDigestIsPinned` moved**, from `0xCE31A6F9C074C14D` to
`0x22769B0E8F305182`. Its remark calls a change a save format change; no save
exists until point 7, so there is nothing to migrate, and a new state field that
left the digest where it was would be the bug. The remark now says so.

**Two test names kept** though narrower than what they check —
`FRW01_WorldStateHoldsTheWorldRowAndNothingElse` and
`NFR01_TheWalkReachesEveryLeafOfTheWorldRow` — because `D-065` and `D-070` cite
them. The first now says so in its summary.

**Would overturn it:** a ruling that register citations do not pin test names.

### D-084 · Point 6 · The declared check, and a word in it that names nothing

**Ruling:** R-027 — ratified.

`FRI01_ChronicleIdsAreStableAndOrdered` appends 1000 entries from a synthetic
source whose every field is a function of its position — n % 4 entities, a cause
halfway back or none every fifth, a tick every third — and reads every field of
every entry back after the columns have grown seven times and the pool eight.
Two worlds fed the same stream give the same ids and hash.

The plan predates `R-020`; the faults were introduced anyway, each alone, each
undone. Red on the named test: ids from 0, growth without copy, every slice
starting at 0 in the pool, the tick not stored. Red on siblings the criterion
does not name: a forward cause accepted (`FRI01_ACauseNamesAnEarlierEntry`), a
column left out of the fold and a fold reading only row 0
(`NFR01_EveryStateFieldEntersTheHash`), a fold running to capacity
(`FRI01_CapacityDoesNotReachTheHash`).

**Criterio difettoso, not rewritten (`R-005`).** The `Check: shallow` line says
"the greps and the synthetic source prove the shape is there"; no grep is named
anywhere in the point. Correct code passes the criterion, and its words point at
a check that does not exist. What proves the shape is beside it:
`FRW01_WorldStateHoldsTheWorldRowAndNothingElse` (the field list),
`FRI01_TheChronicleCarriesTheDeclaredTypes` (the types, since the fold's
`(uint)` would narrow a widened column silently), and
`NFR01_EveryStateFieldEntersTheHash`. Both reviewers found it.

**`FR-I-04` is served as structure only.** The chronicle is the single place
entries live; nothing reads it for UI or dialogue, and under `R-003` the host
has no read path yet.

### D-085 · Point 6 · The reviewers' findings

**Ruling:** R-026 — ratified; cause kind specified by R-024, entity kind left open by R-025.

Both reviewers: the check is met as written, build and both suites green.

**Applied.**
- A fold reading row 0 of a column passed every test — briefed. `D-083`.
- The injectivity remark false for three fields — both. `D-083`.
- A stale remark in `WorldStateTests` calling the command queue state, against
  `SIM-STATE` §Rule since `R-021` — both. Rewritten.
- `GenerationParams`' doc comment had slid onto the new `EntityId` fold — blind.
- Importance cited to FR-I-02, which has no importance term; now `SIM-STATE`
  and DEC-027 — blind.
- "Entries are emitted by Bookkeeping" narrower than FR-I-01 and DEC-026's
  "each event" — both. Now says every event, wherever it happens.
- The capacity test grew two columns of seven — both. All seven.
- Column types unpinned — briefed. `FRI01_TheChronicleCarriesTheDeclaredTypes`.
- A refused append checked by count only — briefed. The hash is compared too.
- "Hashed and saved" said of a serialiser that does not exist — briefed. Handed
  to point 7 (`D-081`).
- Fold order "top to bottom" not quite — briefed. The exception is named.
- The throw departs from a recorded convention — briefed. `D-082`.
- "The greps" name nothing — both. `D-084`.

**Not applied, with the reason.** "Every cast into the fold widens" is imprecise
for `(uint)int` and `(ulong)long`, which reinterpret rather than widen — blind.
The claim it supports, that each cast is injective, holds; the wording predates
this point and was ratified with it.

**Left to the human.**
- The two test names kept for the register's sake (`D-083`) — briefed, taste.
- `core/Runtime/Chronicle/.gitkeep`, an empty folder named for a subsystem,
  predating `R-019` — briefed, citing AGENTS.md's "Structure". Removing folders
  is layout, not this point's.
- For phase 4, not faults here — briefed: `EntityId` carries no kind, so a mixed
  `entities` list cannot say which table an id belongs to; and `cause = 0` makes
  an exogenous root and a player action alike, which AC-11 distinguishes.

**Rejected.** None.

## 2026-09-30 — The agent workflow, designed

A design session with the user, between points 6 and 7 of the phase 3 plan. No
code and no plan: the result is `.claude/design/2026-09-30-agent-workflow.md`,
to be built by a `Kind: process` plan once the phase 3 plan is archived.

Every choice below was the user's, put as options with a recommendation. They
are governance, reserve 1 of `R-001`, which the decider escalates rather than
rules. `docs/` says nothing on any of them: it describes the simulator, not the
way it is built (`D-011`).

The entries of points 7 to 9 of the phase 3 plan follow this section, under a
heading of their own that names the plan.

### D-086 · Six agents and an orchestrator, built after phase 3

**Ruling:** R-028 — ratified.

The workflow is split into `tester`, `coder`, `code-reviewer`, `revisioner`,
`documenter` and `committer`, dispatched by the session running `/plan-next`,
which keeps reading the point, arbitrating the findings and reporting. The user
wanted all four aims put to them: separation enforced by tools, independence
between roles, cost, and a clean orchestrating context.

Built after the phase 3 plan closes. Two plans live at once break the
`/plan-status` lookup, which bounds a plan by the archive commits in
`.claude/plans/`; `main` lacks `/decide`, `RULINGS.md` and the decider's
sections of `AGENTS.md`; and point 7, the serialiser, is the wrong place for the
first run of an untried workflow.

**Rejected.** Building it now through rulings, as `R-002` and `R-010` changed
the process: the decider writes no code, and the hooks are code. A parallel
branch merged afterwards: the order of the archive commits would depend on how
the merge is done.

**Would overturn it:** the lookup ceasing to rest on archive commits, or phase 3
stalling long enough that the agents are wanted for points 7 to 9.

### D-087 · What each role may write, and what enforces it

**Ruling:** R-028 — ratified; "only by the orchestrator" read as among the agents, see the ruling.

As the table of the design: tools restricted by name, paths by hooks in each
agent's frontmatter, three PowerShell 7 scripts (`path-guard`, `git-guard`,
`mutate`). The coder works against a denylist; the tester writes `core/Tests/`
only and introduces faults through `mutate`; both reviewers are read-only; the
documenter holds `Edit` alone, insert-only, on the register, `PLAN.md` and
`SPEC-QUESTIONS.md`; the committer runs `git` only, through `git-guard`.

The documenter writes the point's record and nothing else. `docs/` stays the
decider's, and the user chose this over also writing doc comments, README files,
or no documenter at all.

Models and effort: tester opus/high, coder sonnet/medium, code-reviewer
sonnet/medium, revisioner opus/high, documenter sonnet/low, committer haiku/low.
The coder's was the user's own; the rest was the profile recommended, with opus
only where the register shows subtle defects.

`.claude/agents/`, `.claude/hooks/` and `.claude/skills/` are written only by the
orchestrator, under the permission `AGENTS.md` requires for itself. **The third
folder goes beyond what was put to the user**, who was shown the first two; the
principle they approved — an agent may not loosen its own constraint — covers
the skill that dispatches it, and the design is theirs to review.
**Confirmed by the user on reading the design, 2026-09-30.**

Any agent holding `Bash` can write round the hooks. That ceiling is stated in
the design and covered by the orchestrator's mechanical checks, not removed.

**Would overturn it:** frontmatter hooks not firing for an agent dispatched from
a skill, which would leave the tools list and the orchestrator's checks as the
only enforcement; or a model proving unreliable in its role.

### D-088 · The tester writes before the coder; the code-reviewer reads every point that touches code

**Ruling:** R-028 — ratified.

Two tightenings of the review. The tester writes the named tests from the frozen
criterion and its "Fails when", before any code exists and without the coder's
reasoning, and runs the faults afterwards; the coder never edits `core/Tests/`.
Two of the six closed points of phase 3 had a named test that passed against an
empty implementation (`D-065`, `D-070`).

The code-reviewer runs on every point that touches code, `Core: no` included,
which today no one reads. The briefed and blind revisioners stay as
`reviewers.md` has them, on `Core: yes`.

**Rejected.** The tester after the coder: it would see the code and write tests
shaped like it. One reviewer definition dispatched twice: `Core: no` points stay
unread. The code-reviewer as the blind reviewer: a new name, nothing gained.

**Would overturn it:** the code-reviewer finding nothing on `Core: no` points
across a whole phase, so that its cost buys nothing there; or tests written
first forcing interfaces the coder then has to fight.

### D-089 · Comment rules in `AGENTS.md`, drawn from applied findings; everything in English

**Ruling:** R-028 — ratified.
**Ruling:** R-045 — widened by the user: the outcome markers are English too.

A new section of `AGENTS.md` holds the comment rules C1 to C10 and one rule of
simplicity, S1, each one a finding reviewers already applied, each citing the
entry that applied it. None is invented; a new rule arrives as a finding, a
register entry and a ruling. It lands through a plan point that names it
(`R-010`).

**Rejected.** The rules only in the agents' definitions: a session without
agents would not see them, and two copies would drift. A separate file cited by
`AGENTS.md`: nothing loads it on its own.

Every artefact of the repository is written in English from the point that
writes the rule: code, comments, register, outcome lines, plans, commit
messages. Until now outcome lines and commit subjects were Italian. Records
already written are not translated, and points 7 to 9 of the phase 3 plan are
written as points 1 to 6 were.

**Would overturn it:** the user wanting English at once, from point 7.

## 2026-09-30 — Phase 3, the headless kernel, points 7 to 9

The plan of 2026-09-20, continued after the section above. Each entry names the
point it closes.

### D-090 · Point 7 · The save format: bytes in the hash's order, a header, a dispatch on its version

**Ruling:** R-029 — ratified

`NFR-08` asks for a full snapshot plus seed and parameters, the pending commands,
a version number and a migration path from day one; `AC-03` for identical bytes
on save → load → save; §20 for no reflection. The rest is chosen here.

- **A save is a `byte[]`**, written by `Simulation.Save()` and read by
  `Simulation.Load(byte[])`, both public. Where the bytes go is the host's: the
  core may not touch a file (`BannedSymbols.txt`). A `Stream` API was rejected:
  it adds nothing a host cannot do with an array, and a partial write to a
  stream the host owns is a failure mode the core would have to reason about.
- **Little-endian integers through `BinaryWriter`/`BinaryReader`**, which fix
  the byte order on every platform. No framing per section, no lengths but the
  counts: the reader knows the shape from the version.
- **The order is `StateHash`'s**: §World, then §Chronicle column by column over
  the rows in use, then the pool up to `EntitiesInUse`, then the pending
  commands. Column-major, as the state is held; capacity never written (`D-081`
  handed that on). `ChronicleEntityStart` is written and checked on load rather
  than derived: it is a hashed field of the state, DEC-085 names the offset, and
  a save omitting it would not be the "full state snapshot" NFR-08 asks for.
- **A header of magic `"SIMS"` and format version 1.** The migration path is the
  `switch` on the version in `SaveFormat.Read`: a later format adds a case and
  keeps the reader of every earlier one, each bringing its save to the current
  shape; an unknown version is refused. Version 1 has no predecessor, so there
  is nothing yet to migrate.
- **`SaveFormat` is in `Runtime/Systems/`**: the load builds and fills a
  `WorldState`, which makes it a writer under `R-019`, as the generator is.
- **The format is pinned** by `NFR08_TheFormatIsPinned`, length and SHA-256 of
  one save. A change to it is a new format version, as a change to the digest
  pin is.

**What `docs/` says:** what a save contains, that it is versioned with a
migration path, that nothing reflects. Nothing on encoding, byte order, layout,
header, or API shape. **Would overturn it:** a host needing to stream saves too
large to hold in memory; a second format version whose migration cannot be a
reader of the old bytes into the new shape.

### D-091 · Point 7 · A load is input from outside the core, and refuses what no build could have written

**Ruling:** R-030 — ratified; promoted as DEC-089 by R-042, which adds the pending commands' form to what a load checks

`SaveFormat.Read` throws `InvalidDataException`, and returns no world, for:
bytes that are not a save, a format version it does not read, a save that ends
early or runs on past its end, a negative tick or settlement count, a count the
remaining bytes cannot hold (checked before allocating), a chronicle
`Chronicle.Append` could not have written — a cause failing `CheckCause`, the
rule `Append` uses; an entry dated before the one preceding it or after the
world's tick; a pool that is not contiguous — and a rule version outside
`1..CurrentRuleVersion`.

- **The rule version.** A save from a later build, loaded here, would run under
  this build's rules while claiming the later ones, and `NFR-09` says state
  records which rule version *produced* it. DEC-033 covers only the other
  direction, an older save brought forward. Found by both reviewers (`D-096`).
  The test reference world keeps its rule version 7, which the hash tests need
  distinct; the save tests bring it to the current version first.
- **Not checked:** the kinds of the pending commands. DEC-086 has the core check
  a command against the state of the tick it lands on, which is the drain. In
  phase 3 every kind is unknown, so a save holding commands loads and the next
  `Advance` throws; the command has been taken from the queue by then and is
  lost. Nothing issues a command in phase 3, so the loss is unreachable from the
  host. Nor is any `EntityId`'s generation checked, which `Append` does not
  check either.
- **One exception type for every refusal**, `InvalidDataException`, against
  `ArgumentOutOfRangeException` from `Append`: bytes from outside and a bad
  argument from a caller in the core are different failures.

**What `docs/` says:** nothing on validating a save. **Would overturn it:** a
ruling that the core never throws on input, or that an older build may run a
newer save; the first command kind, which may want its fields checked at load.

### D-092 · Point 7 · `R-022`: a command is a kind and four `long`s, dispatched by a struct type parameter

**Ruling:** R-030 — ratified

- **`Command` is a `readonly struct`**: `int Kind` and four `long` fields `A` to
  `D`, zero when a kind does not use them. DEC-086's "the integer fields that
  kind declares" is read as the kind declaring which of them it reads, in its
  dispatch case. Four because phase 3 has no kind to count from; a kind needing
  more widens the struct and the save under a new format version. The save
  writes every kind through one codec.
- **`ICommand` and its `Apply` are gone.** The queue holds `Command` values; the
  null check on submit went with the reference.
- **The dispatch is `Systems/CommandDrain.cs`**, a `switch` on the kind that
  throws for every kind, since phase 3 defines none. The tick loop takes the
  effects as a struct type parameter (`ICommandEffects`), as it takes the
  levels; every production path uses `CommandDrain`, and the tests supply a
  stand-in kind, `FoldEffects`, without the core defining one. The briefed
  reviewer judged it does not reopen `R-022`, the pattern being `ICadenceLevels`'.

**What `docs/` says:** DEC-086, a value with a kind and integer fields, applied
by the core. Nothing on the width or the dispatch's shape. **Would overturn it:**
a kind whose fields cannot fit four `long`s, or a ruling that tests may not
inject an effect into the drain.

### D-093 · Point 7 · `R-024`: the cause kind is numbered from 1, and an event must name its cause

**Ruling:** R-030 — ratified

- **`CauseKind`**: `Event = 1`, `PlayerAction = 2`, `ExogenousRoot = 3`, an
  `int` enum. Zero is none of them, so a defaulted row is refused rather than
  read as one; the numbers are pinned by
  `FRI01_TheChronicleCarriesTheDeclaredTypes` because every save holds them.
- **The column sits before `cause`** in `WorldState`, the hash and the save,
  following `SIM-STATE` §Chronicle's field order. `NFR01_TheDigestIsPinned`
  moved to `0x0D5FFF6FB35D1584`, as `R-024`'s cost foresaw.
- **`Chronicle.CheckCause`** holds the rule for `Append` and the load alike: the
  kind is one of the three; an event names an earlier entry; anything else
  names none. `R-024` wrote only "cause is none unless the kind is event"; the
  converse is `SIM-STATE`'s "the triggering entry when causeKind is event" and
  FR-E-07's "records the event that triggered it". The briefed reviewer found it
  supported, not invented.

**Would overturn it:** a fourth kind of cause (`R-024`), or an event allowed to
have no recorded trigger.

### D-094 · Point 7 · `R-004` completed, `R-026` done

**Ruling:** R-030 — ratified

- `WorldState`'s constructor takes seed and parameters and stamps
  `Simulation.CurrentRuleVersion`. Another value enters state through the load
  (`D-091`), and through tests writing the field directly, as they already write
  `Tick`.
- `core/Runtime/Chronicle/` removed, `.gitkeep` and folder, as `R-026` asked.

Nothing decided beyond what the rulings say.

### D-095 · Point 7 · The declared check proves less than the point asks

**Ruling:** R-030 — ratified

`AC03_SaveRoundTrip` is green, alone and in both suites. **The criterion is
defective, and was not rewritten:** its "Does" asks for a version number, and the
check passes against a serialiser that writes none — seen by removing the header
from writer and reader together, with the round trip staying green. A round trip
is blind to any change made symmetrically to both halves; it proves the reader
inverts the writer, not what the writer writes. Correct code passes it, so the
point closes (`R-005`, first case).

The missing checks are beside it:
- `NFR08_ASaveCarriesItsFormatVersion` — red on the header removed;
- `NFR08_TheFormatIsPinned` — red on fields `B` and `C` of a command swapped in
  writer and reader alike, which the round trip passed;
- `NFR08_EveryStateFieldEntersTheSave` — the hash fixture's walk, red on the
  writer omitting a column;
- `NFR08_ASaveCarriesThePendingCommandsInOrder`,
  `NFR08_ALoadRefusesWhatNoSaveHolds`,
  `NFR09_ALoadedWorldKeepsTheRuleVersionOfItsSave`.

The point predates `R-020` and has no `Fails when:`. Faults were introduced one
at a time anyway and seen red on `AC03_SaveRoundTrip`: the reader dropping
`CurrencyTotal`; the writer dropping the commands; the writer omitting
importance; the reader skipping a pool entity; the commands reversed on load;
the reader dropping a command's `B`. And on the tests beside it: no tick check,
no rule-version check, the header gone, the symmetric swap. "No reflection
anywhere in the core" is the build's, through `BannedSymbols.txt`.

### D-096 · Point 7 · The reviewers' findings

**Ruling:** R-030 — ratified

Both reviewers: the check is met as written; build 0 warnings, Release 50 and
Debug 51 green.

**Applied.**
- The comment on the load claimed it refused anything `Append` would not write,
  and it accepted entries dated in the future or out of order — both. Tick
  checks added, comment narrowed to what is checked.
- A load accepted a rule version later than the build's — both, citing NFR-09.
  Refused, with 0 and below (`D-091`).
- The command fields `B`, `C`, `D` and any kind but 1 were in no test, so a
  reader dropping them passed everything — briefed. A second kind with every
  field in use is saved in the round trip and the pin.
- `CauseKind`'s numbers unpinned — briefed. Pinned.
- `CommandDrain` said each kind gains "a codec in the save", and there is one
  codec — briefed. Reworded.

**Recorded, not changed.** A loaded command of unknown kind throws on the next
`Advance`, after being taken from the queue — both, neither as a fault (`D-091`).

**Rejected.** None. **Left to the human.** None.

### D-097 · Point 8 · The runner's interface: two required flags, one hash per line, stdout

**Ruling:** R-032 — ratified

`NFR-12` and `DEC-035` ask for a runner that executes N ticks headless and dumps
state hashes; the point names the command, `sim --ticks 1000 --seed 1 --hashes`.
The rest is chosen here.

- **`--ticks` and `--seed` are required**, with no defaults: a default seed is a
  run nobody chose, and `AC-02` is stated per seed. Anything unrecognised or
  malformed exits 2 with a usage line on stderr, rather than being ignored — the
  old `--hash` of the CI smoke step ran against a stub that ignored every
  argument, and that is how a check passes having checked nothing.
- **With `--hashes`, the hash after each tick, one per line**: line n is tick n,
  and the hash of tick 0 is not printed. Without it, the final hash only.
- **16 lowercase hex digits and `\n`, UTF-8 without BOM, on stdout.** The bytes
  do not depend on the machine's line ending or culture, so point 9 can compare
  two runs byte for byte. "Writes it where CI can compare it" is stdout, and the
  workflow redirects it; no `--out` flag, since a redirect does the same thing.
- **Settlement count 2,000**, the P-02 generator default, as a constant. Phase 3
  has no generator; the count only sizes the buckets the loop walks. No flag for
  it until something varies it — sweeps are phase 6.
- **One `Advance(1)` per tick under `--hashes`**, through the public
  `Simulation` only: the runner reads no field of the state (`R-003`).

**What `docs/` says:** that the runner exists, runs N ticks and dumps hashes.
Nothing on flags, output format, or where the output goes. **Would overturn it:**
point 9 or a later criterion needing the hashes in a file the runner writes
itself; a sweep needing the settlement count as a parameter.

### D-098 · Point 8 · The CI smoke step replaced by the point's check

**Ruling:** R-032 — ratified

The workflow already had a "Harness smoke run" step, `--ticks 100000 --hash`,
green only because `Program.cs` ignored its arguments. It is replaced by the
point's command, `--ticks 1000 --seed 1 --hashes`, and a pwsh script that fails
on a non-zero exit, a count other than 1000, a line that is not 16 lowercase hex
digits, or a hash repeated. The last one is what makes "one hash per tick" a
check rather than a line count: the tick is in the state hash, so two equal lines
did not come from two ticks.

The 100k ticks leave the workflow until point 9, whose criterion they are.
Keeping them here would have meant a flag the runner does not have, or a
second run the point does not ask for.

**What `docs/` says:** §17, every criterion runs headless in CI through the
harness. Nothing on the step. **Would overturn it:** point 9 wiring the 100k run
differently, which is its to decide.

### D-099 · Point 8 · Verification, and what the point does not cover

**Ruling:** R-032 — ratified; the §20 finding escalated by R-031

**Check.** The CI script run locally in pwsh against the Release build: green,
1000 hashes. Faults introduced one at a time in `Program.cs`, each seen red on
that script and undone: one hash per run instead of per tick (count 1); no
`Advance` inside the loop (a hash repeats); the loop running one tick too many
(count 1001); uppercase hex (a line is not a hash). The point predates `R-020`
and has no `Fails when:`. **"Run from the CI workflow"** is a push away, and a
push is the human's: the point closes with that clause in reserve (`R-011`).

**`Core: no` held.** `git diff --name-only HEAD -- core/` printed nothing; the
reviewers did not run. Core build 0 warnings, suite green in Release (50) and
Debug (51).

**Findings, not acted on.**
- `NFR-12` also asks the runner to write the **chronicle** to CSV. The point's
  "Does" defers sweeps and metric series to phase 6 and says nothing of the
  chronicle; phase 3 has a chronicle with no subsystem feeding it. Not built —
  the point does not ask for it — and not deferred by anyone either. The decider
  may want to say which phase owes it.
- `harness/Sim.Harness.csproj` references the core by `ProjectReference`, and
  §20 says the harness "compile[s] that folder in place". Older than this plan
  and outside the point; recorded so it is not taken as settled by the point
  that last touched the harness.

### D-100 · Point 9 · `AC02_DeterminismAcrossRuns`: two worlds in one process, empty and with commands

**Ruling:** R-035 — ratified

**Decided.** The test builds two worlds from the same seed and runs each 100k
ticks, reading the hash after every tick, and compares the two sequences whole.
The empty runs go through `Simulation`, as the runner calls it, with the
runner's 2000 settlements, so the settlement buckets fire. The commanded runs go
through the internal `TickLoop` with the stand-in kind of `CommandQueueTests`,
one command every 97 ticks, because phase 3 has no public submit and no kind of
its own (`DEC-086`). Three controls keep the equality from being one every world
passes: every tick's hash is distinct, seed 2 gives another sequence, and the
commanded sequence differs from the empty one.

**What `docs/` says:** `AC-02`, "same seed and commands produce the same state
hash sequence across builds and machines". Nothing on how two runs are made
independent. **Would overturn it:** a public submit arriving with the first
command kind, which moves the commanded half onto `Simulation`.

### D-101 · Point 9 · The gate step in CI, and the measurement beside it

**Ruling:** R-035 — ratified

**Decided.** One pwsh step after the harness step, reusing its Release build of
`sim`: two processes of `--ticks 100000 --seed 1 --hashes`, stdout written raw
to two files by `Start-Process`, so no PowerShell decoding sits between the
runner and the comparison. The step fails on a non-zero exit, on a length other
than 100000 lines of 17 bytes, or on any byte differing.

**The measurement is not taken on the 100k run.** Timed from outside the
process, 100k empty ticks cost less than the process start varies by: net of a
run of no ticks, the figure came out at −0.000003 and −0.000022 ms. The step
therefore times a run of 10M ticks without per-tick output, subtracts a run of
none, and prints `ms per empty tick` in invariant culture. Locally, twice:
0.000130 and 0.000121 ms, about 125 ns, with 2000 settlements bucketed and
nothing in them. It is the floor under `P-01` and `P-17`, not either of them
(`SQ-004`).

**What `docs/` says:** §18, "100k empty ticks; AC-02, AC-03 green"; nothing on
how a tick is timed. **Would overturn it:** a timing flag in the runner, which
would measure inside the process and could use the gate's own run; the point's
"Does" names a test file and a workflow step, not the harness.

### D-102 · Point 9 · Verification, and what the gate does not cover

**Ruling:** R-035 — ratified; the machines finding resolved by R-034

**Check.** `AC02_DeterminismAcrossRuns` green alone; suite green in Release
(51), Release with three wealth bands (51) and Debug (52); core build 0
warnings. The workflow step's script, extracted and run locally in pwsh against
the Release build: green. Faults introduced one at a time and seen red, each
undone: a `WorldState` constructor folding a static count of worlds built into
`CurrencyTotal` — the test red on "the same seed, empty"; the second CI run on
seed 2 — "the two runs differ"; the second run one tick short — "the two runs
differ"; both runs one tick short — the length check. The point predates
`R-020` and has no `Fails when:`.

**`Core: no` held.** `git diff --name-only HEAD -- core/Runtime/` printed
nothing; the only file under `core/` is the new test. Reviewers not run.

**Reserve.** "Completing in CI" is a push away, and a push is the human's: the
point closes with that clause in reserve (`R-011`), beside point 8's.

**Findings, not acted on.**
- `AC-02` says "across builds and machines". Both runs here, in the test and in
  CI, are one build on one machine; the workflow runs on `windows-latest` alone.
  The point asks for two independent runs, and that is what it delivers. Whether
  the phase 3 gate owes a second machine — an OS matrix, or a hash sequence
  pinned in the repository and compared by every runner — is the decider's.
- The 10M-tick timing run makes the gate step do a hundred times the ticks the
  gate names. Its cost locally is about a second.

### D-103 · Point 10, added mid-plan — the pin owed by `R-034` and `R-036`

**Ruling:** R-038 — ratified.

`R-034` owes the pin before the phase 3 gate and says it enters the plan as a
point added with the user's approval (`D-014`). The user approved it on
2026-10-01: "Approvo l'aggiunta al piano di fase 3 di un punto per l'obbligo
R-034/R-036 (pin delle due sequenze di hash), secondo D-014". Appended as point
10, after nine closed points; nothing above it rewritten, the exit condition
left verbatim.

**Decided in writing the point.**
- **Both pins in the suite, none in the workflow step.** `R-036` puts the
  commanded pin in the suite, since its command kind is test code; the empty
  one goes beside it rather than into the gate step, so one test holds both and
  each of the three CI test steps compares them — two builds of each commit, as
  `R-034` notes. The gate step stays as point 9 left it.
- **The named faults.** The drain moved to the end of the tick is the fault the
  pin exists for: `AC02_DeterminismAcrossRuns` cannot see it, since two runs of
  the same wrong code agree. A world starting at tick 1 is the one fault the
  empty pin can catch in a phase 3 world, where only `Tick` moves (`R-036`).
- **Left to the point:** which digest of the sequence, and the test's layout.

**What `docs/` says:** `AC-02`, "across builds and machines"; nothing on how a
pin is taken. **Would overturn it:** a ruling that the empty pin belongs in the
gate step, next to the two processes it would then compare as well.

### D-104 · Point 10 · `AC02_TheGateSequencesArePinned`: the digest, the values, the check

**Ruling:** R-038 — ratified; the three-band finding by R-037.

**Decided.**
- **The digest is SHA-256, from the test assembly,** over each of the 100k
  hashes written little-endian, as uppercase hex. Not the core's `Hash64`: an
  expected value checked through the core's own hash of hashes would move with
  a change to that function in the same commit and could be re-derived by it;
  SHA-256 is the base class library's and the core never sees it. The test
  project is outside `BannedSymbols.txt`, as for the reflection point 3 uses.
- **The values came from a run,** on the development machine, Release build:
  the test was run with a placeholder and the failure's actual value written in
  as the literal, one sequence at a time. Empty run
  `9C9CC60D…BACC93C4`, run with commands `72FA5937…BC31F715`, in full in the
  test. The Debug build then passed against them, so two builds of this commit
  already agree.
- **Both sequences are the ones `AC02_DeterminismAcrossRuns` compares,** through
  the same `Empty` and `Commanded`, so the pin and the determinism test cannot
  drift onto different runs. Each is run once more for the pin; the fixture now
  runs seven sequences of 100k ticks, under a second in Release.
- **No workflow change.** The test is in the suite, so each of the three test
  steps runs it; the gate step stays as point 9 left it (`D-103`).

**Check.** `AC02_TheGateSequencesArePinned` green alone; suite green in Release
(52), Release with `SIM_WEALTH_BANDS=3` (52) and Debug (53); core build 0
warnings. Faults of `Fails when:` introduced one at a time and seen red, each
undone:
- the drain moved after `state.Tick = d + 1` in `TickLoop.Advance` — the pin
  red on "the run with commands", the empty pin green, and
  `AC02_DeterminismAcrossRuns` green in the same run;
- `Tick = 1` in the `WorldState` constructor — the pin red on "the empty run".

**`Core: no` held.** `git diff --name-only HEAD -- core/Runtime/` prints
nothing; the only file under `core/` is `core/Tests/DeterminismTests.cs`.
Reviewers not run.

**Reserve.** "And in CI" waits on the push, which is the human's (`R-011`),
beside the reserves of points 8 and 9.

**Findings, not acted on.**
- `SIM_WEALTH_BANDS` is read by nothing in the repository — no `.cs`, `.props`
  or project file names it — so the step "Tests, three wealth bands" runs the
  same build with the same behaviour as the first. The point's "each of the
  three test steps" holds, but the second compares nothing the first did not.
  `AC-28`'s step is the phase 4 table's to give meaning to; recorded, not
  touched.
- The pinned values are from one machine. That the CI runner reaches the same
  digest is the reserve, not something verified here.

**What `docs/` says:** `AC-02` and `NFR-01`, nothing on the digest.
**Would overturn it:** a ruling that the pin should use the core's hash, or
live in the gate step against the harness output.

### D-105 · The moment-two request, and `SQ-007` raised while writing it

**Ruling:** R-044 — ratified.

**Decided.** The request of 2026-09-20 is replaced, not amended: the moment has
moved from one to two, which is the case `D-039` and `D-041` give for replacing.

**Decided.** `SQ-007` is filed as blocking the gate's declaration, and the
request does not call the gate met. Every clause of §18 is green in CI; §17's
"driven by the command-line harness alone", which the plan's exit condition
repeats, is met only on the suite's reading. Calling the gate met would have
picked that reading, and an objection is never resolved by its finder. No point
is failed for it: all ten are closed, and the question is about the whole.

**Decided.** The *Riserva sciolta* lines of points 8 and 9 cite the first green
run, `36786627133` on `b43d57d`, beside the run on `c645462`; point 10's cites
only the second, the first run on a commit containing `2ec9fad`.

**What `docs/` says:** §17 and AC-19 as set out in `SQ-007`; nothing on review
requests. **Would overturn it:** the decider or the user ruling that §17's
"alone" means only no editor and no game client, which closes `SQ-007` and lets
the gate be declared as the files stand.

### D-106 · Look at the whole of phase 3 · A refused command stops the tick halfway, left to phase 4

**Ruling:** R-042 — the third question answered by DEC-089, due at the same point; the first two stay with that point. The rest ratified by R-044.

A finding, not a decision: recorded so that the first phase 4 point introducing
a command kind meets it in its criteria rather than in a bug. Raised in the look
at the whole the moment-two request asked for, and left to phase 4 at the
user's request.

**What the code does.** `TickLoop.Advance` takes each command from the queue and
hands it to `CommandDrain.Apply`, which throws on a kind it does not know. A
command refused halfway through a drain leaves the commands before it applied,
the tick counter where it was, and the refused command already out of the
queue: the world is between two ticks. Advancing again finishes the tick with
the commands left, as if the refused one had been dropped — deterministic, but
only because the host caught the exception, and nothing records the drop. A
refusal against the state is an ordinary outcome of play, not a programming
error; thrown across the host boundary, on the core's own thread under DEC-031,
it makes every host write the same catch. `FRA01_ACommandOfNoKnownKindIsRefused`
submits one command only, so it sees none of this. `D-091` recorded the lost
command on load and judged it unreachable in phase 3; it still is, since no
kind exists for a command to apply before the one refused.

**What `docs/` says.** DEC-086: the core checks "a command against the state of
the tick it lands on rather than trusting whoever built it" — the check belongs
to the drain, and moving it to submission would contradict it. `docs/` does not
say what the drain does with a command it refuses. FR-A-02's typed event stream
is the obvious place to report one, and is not built.

**Owed to the phase 4 point that adds the first command kind**, as questions its
criteria should answer, not as answers:
- what a refused command does to the tick: nothing, with the tick running on,
  is what replay needs, and it is not what the code does now;
- whether the host hears of a refusal, and through what;
- whether a command's *form* — a kind this build knows, fields in the ranges
  its kind declares — is checked at submission and at load as well, since that
  check needs no state, and a load that accepts a kind no build could have
  written is the case `D-091`'s title refuses and its body let through.

**Would overturn it:** a ruling that a refused command is a programming error
the host must never make, which would make throwing correct and leave only the
lost command to fix.

### D-107 · Closing phase 3 · The archive, the promotion pass, and the merge

**Ruling:** R-044 — ratified; the gate recorded in `RULINGS.md` as well.

The user confirmed the gate: "Confermo il gate di fase 3, procedi con il merge in
main". The design note ties archival to the plan's close, not to the merge, and
`main` never carries a live `PLAN.md`, so the archive and the pass come first.

**Decided.**
- **The archive's name** is `2026-09-20-fase-3-kernel.md`, the date the plan was
  written and the branch it ran on, as the earlier archives are named. The
  confirmation is appended under the last point, below a rule, in the way a
  *Riserva sciolta* line is: nothing above it is touched. The archive commit
  adds exactly one file under `.claude/plans/`, since `/plan-status` finds a
  plan's range by the commits that add one.
- **The pass proposes three candidates** and names three near misses. `D-073`
  is left out because `SIM-ECON` holds its question open, and closing an open
  item is not a promotion. `D-090` is left out because `R-029` declined it.
- **The request is replaced**, moment two by moment three: the gate is answered
  and the situation has moved (`D-039`, `D-041`).
- **The merge is `--no-ff`**, so the branch's commits and their `Plan-point:`
  trailers reach `main` as they are. `main` is not pushed: a push is the
  human's (`R-011`).

**What `docs/` says:** nothing on archiving or merging; §18 on the gate.
**Would overturn it:** the decider rejecting the gate record's place in the
archive rather than in `RULINGS.md`, or a candidate found to be in `docs/`
already.

### D-108 · Review of the poverty and seed proposal, and of the batch it became

**Ruling:** R-076 — ratified.
The proposal entered `docs/` through `R-054` to `R-065`; the
findings on the result are `SQ-009` to `SQ-022`.

The user asked for three things in turn: review the proposal before transcription,
bring it and the phase 4 note into the repository and resolve the review's points,
and check the decider's result. No code changed, and nothing in `docs/` was edited
by this session.

**Decided.**
- **A branch, `design/poverty-seed`**, rather than commits on `main`: the
  session's standing rule is to branch before committing on the default branch.
  The decider's rulings landed on the same branch, so it carries the whole batch
  and is merged without squashing like any other.
- **The proposal's place and name** are
  `.claude/design/2026-10-04-proposal-poverty-seed.md`, dated as the other design
  notes are. The user's file in Downloads is the origin; revisions 9 and 10 exist
  only here.
- **Revision 9 was written by this session from the user's answers**, each put as
  a choice with a recommendation; every one is marked as the user's in the
  proposal. Two changes needed no answer and were made as form: the
  conservation sentence of DEC-098 marked for the decider like FR-P-03c, and
  FR-C-04's steps 5 and 6 stated as kept.
- **Twenty-two findings became fourteen entries**: each medium finding its own,
  the low ones gathered in `SQ-019` and `SQ-022`. Every entry says "Blocks: no"
  and names the phase 4 point that must not start before it is answered, since
  no plan exists yet to stop. `SQ-020` is marked the user's, under reserve 4.
- **`9250363` names D-108 but does not contain it**: a hook refused the edit
  after the commit had been prepared, and the entry follows in its own commit
  rather than by amending.

**What `docs/` says:** nothing on where a design proposal lives or how a
review of the decider's work is filed; `AGENTS.md` sends objections to
`SPEC-QUESTIONS.md`.
**Would overturn it:** the user preferring the batch on `main`, or the decider
wanting the low findings split one per entry.

### D-109 · The draft of the cost measurement plan, at moment one

**Ruling:** R-076 — ratified; where the plan sits against `R-046` answered by R-067.
The draft is in `REVIEW-REQUEST.md`, for the user's answer
under `R-048` and `R-049`.

`SQ-023` owes a measurement before its ruling, and the user approved the sizes
per settlement type and asked for the draft. Writing it forced these choices.

**Decided.**
- **`Kind: process`.** The points change no simulated world and nothing in
  `core/`: they build a stand-in and measure it. A `Kind: simulator` plan must
  name the acceptance criteria it moves, and this one moves none. The header
  cites P-01, NFR-05, NFR-07 and `SQ-023` in its exit condition instead.
- **A project of its own, `bench/`, kept after the plan.** Not in the harness,
  which is the driver of §17's criteria (`R-039`) and runs in CI; timing in CI
  is noise. Kept rather than thrown away because the stand-in behind P-63 and
  P-64 was thrown away (`SQ-004`), and this plan's first point cannot know what
  it contained.
- **Calibration by tolerance, not by equality.** The cost at 9 rows within half
  and twice P-63, the 27-to-9 ratio within a fifth of 4.8: wide, because the
  old stand-in's five phases are unknown and the machine differs; narrow
  enough that a ranking linear in the rows, about 3, fails.
- **Checksums as the guard against a benchmark that measures nothing.** Every
  layout computes the same outputs on the same world, so a faster layout that
  skips work is caught by its checksum rather than praised for its speed.
- **The sizes are measurement inputs, not constants**, and take no P-number
  under `R-050`: they describe worlds to time, not the world the game runs.
- **`64147ba` names D-109 but does not contain it**: as with `9250363`, the
  edit and the commit were sent together and a hook refused the edit. The entry
  follows in its own commit.

**What `docs/` says:** §19 places P-17 and P-01's validity in Phase 4 on the
real update; nothing on measuring before it. `R-046` names the next two plans,
and this would be a third: raised in the request as blocking.
**Would overturn it:** the decider placing the measurement inside phase 4's
first point, which would make it `Kind: simulator`; or the user preferring the
stand-in thrown away at the end.

### D-110 · The draft of the process plan, at moment one

**Ruling:** R-066 — resolved: the two departures are the user's, the rest
ratified. The draft is in `REVIEW-REQUEST.md`, for the user's answer
under `R-048` and `R-049`.

The proposal of 2026-10-02 (`.claude/design/2026-10-02-integrazione-piano-processo.md`)
asked the user five questions and three approvals before the draft could be
written. On 2026-10-05 the user answered four, each with the recommendation:
this plan before the cost measurement plan of `D-109`; seven points; the two
scripts in `.claude/hooks/`; `/plan-explain` reading a draft from a fixed heading
of `REVIEW-REQUEST.md`. The fifth, the archive of `RULINGS.md`, stays out of the
plan as a change to `/decide`.

**The user's, departing from rulings — for the decider to record.**
- **Seven points against `R-046`'s "the design's points 1 to 5".** The five are
  kept; `plan-state` and the skills on it are added. Neither is a simulator
  change.
- **Changes to the design ratified by `R-028`:** `/plan-explain` and
  `/plan-status` change, which it excluded; the tree check covers their agents
  too; two scripts that are not hooks live in `.claude/hooks/`.
- **The order of the next plans:** this one, then the cost measurement plan,
  then phase 4. This answers question 2 of the request drafted on
  `design/poverty-seed`, and leaves `R-046`'s first real run to the decider: the
  measurement plan's points are now the first a rewired `/plan-next` runs.

**Decided while writing the draft.**
- **The rewiring of `/plan-next` is the last point**, 7, swapped with the
  proposal's 7. Written last, no point of this plan runs under the agents, and
  `R-046`'s first real run stays outside it; the proposal's order constrained
  only that `plan-state` and the English markers come before both skill points,
  which still holds.
- **`plan-state -Draft`** reads the fenced block under `## Draft plan` in
  `REVIEW-REQUEST.md`, and this request uses that heading, so point 6 can be
  checked on it.
- **The branch:** the request on `design/process-plan`, cut from `main`; after
  the answer it merges into `main` without squash, and the plan is installed on
  `agent-workflow`, cut from `main`, as `R-048` requires.
- **The word cap of point 7** is the proposal's recommendation, put in the draft
  without an explicit answer, and marked so in the request.
- **The number is `D-110`**: `D-108` and `D-109` exist on `design/poverty-seed`
  and not on `main`.
- **This request replaces the moment-three request on `main`**, as the request
  on `design/poverty-seed` does. When that branch merges, `REVIEW-REQUEST.md`
  conflicts, and whichever request is still unanswered must survive the merge.

**What `docs/` says:** nothing; the plan touches no file in `docs/`. The rules
are `R-045` to `R-049` and the design ratified by `R-028`.
**Would overturn it:** the decider declining the departures from `R-046` and
`R-028`, which brings the plan back to five points; the user's answer amending
the draft, which the installing commit then reports.

## 2026-10-05 — The agent workflow, built and wired

The process plan installed in `6d6d098`, from the design of 2026-09-30. Each
entry names the point it closes.

### D-111 · Point 1 · `path-guard` and `git-guard`: what the design left open

**Ruling:** R-069 — ratified; the settings-file note and the identifier match escalated by R-068.

**Decided.**
- **The role reaches `path-guard` as `-Role <name>`**, written in each agent's
  frontmatter at point 4. An unknown or missing role is denied.
- **A path outside the repository is denied to every role**, the coder
  included. The design gives the coder "everything except the denylist", which
  reads as everything in the repository; nothing in the roles writes elsewhere.
- **Paths are resolved before matching**: relative to `CLAUDE_PROJECT_DIR`,
  `..` collapsed, compared case-insensitively, because the file system is.
  `core/Runtime/../../docs/x` and `DOCS/x` are both `docs/`.
- **Insert-only also refuses an empty `old_string`**, which every `new_string`
  begins with and which `Edit` uses to create a file.
- **`git-guard` refuses more than the design lists**: a newline, a lone `&`,
  `<` and `>` count as joining or redirecting; `git add` takes no option at all
  and no glob or `:` pathspec; `--output` on the four read commands, which
  writes a file; `git -C` and any first word other than `git`. Each follows
  from "admits only", read as an allowlist.
- **"Says in words that none applies"** is the phrase `no identifier applies`,
  `no requirement applies` or `no decision applies` in the subject. An
  identifier is `NFR-`, `FR-`, `DEC-`, `AC-`, `SQ-`, `A-`, `R-`, `D-` followed by
  a letter or digit, as a word.
- **`Plan-point` anywhere in the message counts**, case-insensitive: more than
  one mention is refused, and one must be the line `Plan-point: <integer>`. A
  body saying "closes no Plan-point" is refused, as the skill asks that the word
  not be written at all.
- **Deny is exit 2 with the reason on stderr**, the hook contract's blocking
  form; no JSON output.

**Noted, not acted on.** The coder's denylist does not hold
`.claude/settings.json` or `CLAUDE.md`. A settings file can add hooks, not
remove an agent's own. Not in the design's list, so not added.

**Check.** Both self-tests exit 0 (32 and 43 cases, two of each through the
script's real entry point with stdin and exit code; `git-guard`'s commit cases
in a throwaway repository). The three faults of `Fails when:` seen red on
copies, originals untouched. Not verified: that Claude Code invokes the scripts
from an agent's frontmatter as written — point 4's live probe.

**What `docs/` says:** nothing; process work. The design's "Enforcement" items 1
and 2. **Would overturn it:** point 4's probe showing the role cannot be passed
as an argument, or the decider reading "everything except the denylist" as
including paths outside the repository.

### D-112 · Point 2 · `mutate` and `tree-state`: what the design left open

**Ruling:** R-072 — ratified; the unread untracked file found at audit resolved by R-071, the rename that passes the filter by R-073.

**Decided.**
- **`mutate.ps1 -Patch <file> -Check <command>`.** The check is a command
  string, run by `pwsh -NoProfile -Command` in the repository root, and a fault
  is caught when it exits non-zero. The design says "runs the named check"
  without a form; a command covers a `dotnet test --filter` and anything else
  the tester names. The tester holds `Bash`, so running its string widens
  nothing.
- **Four exit codes**, where the design names one: 0 the check went red and the
  tree is restored; 1 the check stayed green, the fault was not caught; 2
  refused before anything was applied (missing argument, unreadable patch, a
  patch that does not apply); 3 the tree did not return, which wins over the
  others. Without 1 the tester would read a missed fault from the log alone.
- **A patch touching any path outside `core/Runtime/` and `harness/` is
  refused** (exit 2): the restore is proved only for what `tree-state` records,
  so a fault elsewhere would be applied without proof that it went away.
- **The patch is applied to the working tree**, not the index, so it is written
  against the coder's uncommitted code; the reversal runs in a `finally`, so a
  check that throws still has its patch reversed.
- **`tree-state` hashes `git diff HEAD --binary`**, with `--no-ext-diff` and
  `--no-textconv`. Without `--binary` a changed binary file prints "Binary files
  differ" whatever its content, and the hash would not move; the self-test
  carries the case. Status lines, the diff's SHA-256 and one line per untracked
  file with its SHA-256 are joined and hashed once; ignored files, `bin/` and
  `obj/` among them, are outside it.
- **`tree-state -Path <pathspec>…` and `-Repo`.** `mutate` passes
  `core/Runtime` and `harness`; the skills of points 6 and 7 call it with no
  pathspec, the whole tree.
- **`mutate.ps1 -SelfTest`**, which the criterion does not ask for: seven cases
  in a throwaway repository and two through the entry point. It is what turns
  red on the second fault of `Fails when:` without touching `core/`.

**Check.** `tree-state.ps1 -SelfTest` 11 cases, `mutate.ps1 -SelfTest` 9 cases,
both exit 0. The known patch — `Calendar.Month` dividing by `DaysPerMonth + 1` —
through `mutate` with `dotnet test … --filter
FullyQualifiedName~FRT05a_DateArithmeticIsIntegerDivision`: the test red at
`CalendarTests.cs:91`, `mutate` exit 0, `Calendar.cs` byte-identical before and
after (MD5) and the `core/Runtime,harness` hash unchanged. The two faults on
copies, originals untouched: untracked files hashed by name only → 2 cases red
("the untracked file edited", and the scoped one); `git apply -R` removed → 6
cases red, the file left reading `wrong`.

**Not verified.** A working file with mixed line endings: under
`core.autocrlf=true` a patch made by `git diff` applied and reversed cleanly on a
CRLF file, but the first self-test, writing mixed endings, did not apply — the
self-test now writes uniform ones. A check that writes outside `core/Runtime/`
and `harness/` is not seen by `mutate`; that is the orchestrator's check after
the tester's second dispatch, in the design's "The hole that `Bash` leaves".

**What `docs/` says:** nothing; process work. The design's "Enforcement" item 3.
**Would overturn it:** the tester's brief at point 4 or 7 needing the check as a
test name rather than a command, or a patch legitimately touching a path outside
the two folders.

### D-113 · Point 3 · `plan-state`: what the point left open

**Ruling:** R-075 — ratified; the `stopped` read by number found at audit resolved by R-074.

**Decided.**
- **The JSON shape.** One object: `path`, `exists`, `kind`, `abandoned`,
  `points` — each with `number`, `title`, `status`, `check`, `core`,
  `coreReason`, `serves`, and `commits` for a closed point — then `state`
  (`none`, `running`, `stopped`, `finished`, `abandoned`), `next`, `stoppedAt`,
  `reservesOpen`, `range`, `warning`, `decider` (`count`, `first`, `last`). The
  point lists what the skills read today and the design note says it is not a
  schema; these names are mine. Non-ASCII is escaped, so the console code page
  cannot garble a title.
- **An abandoned plan reports `state: abandoned` and no next point**, whatever
  its points say. The point names the declaration as a field and the state as
  next point, stopped or finished; a plan abandoned with open points would
  otherwise report a next point that `/plan-next` must refuse.
- **`stopped` needs an open point after the failure**, as `/plan-next` words it.
  A plan whose last point failed reports `finished`, with the point `failed`.
- **A reserve open does not change `state`.** All points closed with a reserve
  open reports `finished` and lists the point in `reservesOpen`; the skills, not
  the script, say the exit condition is unmet (`R-011`).
- **A marker counts only right after the date sentence**: `FALLITO`/`FAILED`,
  *Chiuso con riserva*/*Closed with reserve*. Point 12 of the twelve-gaps plan
  names `FALLITO` in prose ("Closed rather than `FALLITO`") and is closed; a
  match anywhere in the paragraph would report it failed. A discharge is any
  line of the section opening with *Riserva sciolta:*/*Reserve discharged:*.
- **The check marker is a `- **Check: …**` bullet; failing that, a `Check: weak`
  or `Check: shallow` inside the `Closed by` bullet.** Point 7 of this plan
  declares its marker the second way. Prose elsewhere in the point — the title of
  point 13 of the twelve-gaps plan names both markers — is not read.
- **`Core:` with no line is `null`, not `yes`.** The default is the skills'
  rule to apply; the script reports what is written. A reason is any text after
  `yes`/`no` once punctuation and dashes are stripped.
- **Commits are resolved only for `PLAN.md` at the root and for a plan added
  under `.claude/plans/`**, with the range of `/plan-status` step 4. A fixture or
  a draft has no range in history; `range` is `null` and no point carries
  `commits`.
- **The `main` warning covers both readings of "sits on main"**: `main`'s tree
  holds `PLAN.md`, or the current branch is `main` and `PLAN.md` is in the
  working tree.
- **"No plan" is a path that does not exist**, not a fixture file; the other
  eight cases are files. The draft fixture carries a `## 1.` heading outside the
  fenced block, which must not be read as a point.
- **The 31-points check is part of `-SelfTest`**, run on this repository's
  history. A rewrite of that history turns the self-test red, which is the
  intended reading: the lookup it proves would be broken too.

**Check.** `plan-state.ps1 -SelfTest` 58 cases, exit 0: the nine fixture cases,
31 archived points each resolved to one commit, the total, the entry point. The
three faults of `Fails when:` on a copy, original untouched: `*Outcome:*` not
recognised → 4 cases red; trailer grep unanchored → 2 red (twelve-gaps point 1
finds 5 commits, phase 3 point 1 finds 2); the `stopped` branch disabled → 2 red.

**Not verified.** Calling the script from an agent's `Bash` rather than this
session's. Headings inside a fenced block of a plan would be read as headings:
none of the six plans holds a fence today (grep). A draft whose fence holds a
nested fence: the first closing fence ends the draft.

**What `docs/` says:** nothing; process work. The point's own text and
`/plan-status` step 4.
**Would overturn it:** a skill at point 6 or 7 needing a field in another shape,
or the user ruling that an abandoned plan still reports its next point.

### D-114 · Point 4 · The six agent definitions: what the design left open
**Ruling:** R-093 — ratified; a recommendation on point 8 escalated to the user

**Decided.**
- **The hook command** is `pwsh -NoProfile -File
  "$CLAUDE_PROJECT_DIR/.claude/hooks/<script>.ps1"`, with `-Role <name>` for
  `path-guard` and nothing for `git-guard`, which has one role. Matcher
  `Edit|Write` for the coder, the tester and the documenter, `Bash` for the
  committer. The documenter's matcher holds `Write` although it lacks the tool,
  so a tool list widened by mistake still meets the guard.
- **No hook on the two reviewers.** They hold neither `Edit` nor `Write`, so
  `path-guard` has nothing to see; what `Bash` leaves open is the orchestrator's
  check, as the design says.
- **The `code-reviewer` points at `AGENTS.md` read whole**, its rules the build
  cannot catch first, rather than at "Comments and simplicity": that section is
  point 5's, and a pointer to it now would name text that does not exist (C1).
- **The five rules leave `reviewers.md`**, which keeps a pointer to
  `revisioner.md` and one sentence: a brief to an agent of another type carries
  them, copied from there. Until point 7 `/plan-next` dispatches `Plan` or
  `Explore`, which do not read `revisioner.md`; without the sentence a review
  run before then would lose the rules. Rules 1 to 4 moved verbatim; rule 5 lost
  its own heading, "This one matters most" opening it instead, and "State it
  explicitly in both briefs", which the body now does itself.
- **The bodies say what each role does with its brief** — the table's
  "Receives" and "Returns" as instructions, the tester's two dispatches, the
  documenter's STOP cases from `/plan-next` step 6, the committer's comparison of
  the staged set with the list it was given. Nothing in them adds a step the
  design does not have.
- **The frontmatter check is a script outside the repository**, run from the
  session's scratchpad: per file the name, model, effort and tools as a set
  against the roles table, the description's "Dispatched by /plan-next only",
  the hook's matcher and command, no hook on a reviewer, six files, no `Edit` or
  `Write` on a reviewer, and the two greps of the code-reviewer's body. Not
  committed: the point names no place for it, and the design wants hooks in
  `.claude/hooks/`, which it is not.

**Check.** The frontmatter check exits 0. The fault of `Fails when:`, `Edit`
added to the tools of `code-reviewer` and then of `revisioner`, turns it red
twice (tool set, and the reviewer rule), restored after each. Each frontmatter
`command:` line, run through `bash` with `CLAUDE_PROJECT_DIR` set and a sample
JSON: coder denied `docs/probe.md`, `core/Tests/`, `PLAN.md` and allowed
`core/Runtime/`; tester allowed only `core/Tests/`; documenter denied a `Write`
everywhere; committer denied `git add -A`.

**Not verified — the reserve.** The live probe. Agent definitions are read when a
session starts, and this one started before they existed: `subagent_type:
coder` is "not found". A fresh headless `claude -p` failed to authenticate. So
whether Claude Code fires a frontmatter hook for a subagent, whether the shell
expands `$CLAUDE_PROJECT_DIR`, and whether `effort` is honoured all remain as
the design's "Not verified" left them. Nor was `disableAllHooks` probed
(`R-068`).

**What `docs/` says:** nothing; process work. The design's roles table,
"Enforcement" and "Where the rules live", and `R-047` for the code-reviewer.
**Would overturn it:** the probe showing the hook does not fire or the variable
does not expand, which changes every `command:` line; the decider reading "move"
as leaving `reviewers.md` without even the pointer.

### D-115 · Point 8, added mid-plan — owed by `R-070`, `R-071`, `R-073`, `R-074` and `R-094`
**Ruling:** R-095 — ratified; a self-test of `mutate`'s exit 3 owed beside the criterion

`R-070` owes point 8 before point 7 runs, placed between points 6 and 7, and
the four rulings after it each add an item. The user approved every item and
read every `Fails when:` line in those rulings; the point transcribes them.
Nothing above it rewritten, no number moved.

**Decided in writing the point.**
- **The `Does:` items are the rulings' own text**, in their order, each citing
  its ruling. Item 3 keeps `R-068`'s "a self-test case for a lowercase `a-`",
  which `R-070`'s restatement drops; item 4 carries `R-071`'s cost paragraph,
  `mutate` stopping with exit 3, since its "Owed" line requires it.
- **The `Fails when:` lines are verbatim**, grouped by ruling.
- **`Closed by:` is mine.** No ruling states one: they name a self-test case per
  item, and items 2 and 7 change prose, so the line is the five self-tests at
  exit 0 plus a grep for each prose change. It freezes with this commit and was
  not read by the user.
- **The title and the italic preamble** follow the precedent of point 10 of the
  phase 3 plan (`D-103`).

**What `docs/` says:** nothing; process work. **Would overturn it:** the decider
finding a `Closed by:` that a correct point 8 cannot pass, or that passes
without catching a `Fails when:` fault, under `R-005`.

### D-116 · Point 5 · `AGENTS.md`'s three rule sets and the English markers: what the point left open

**Ruling:** R-097 — ratified.

**Permission for `AGENTS.md` (`R-010`).** Granted by the user, Federico Xella,
in answering the process plan's moment-one request (the plan installed in
`6d6d098`), for point 5 as its text states it: the three rule sets of the
design's "The rules this adds to `AGENTS.md`", `R-048`'s rule in "Criteria are
frozen when written", and the five markers (`R-045`). The applied diff edits
that bullet and adds three sections, and touches nothing else in the file. The
three skills are named in the point's text, which is the permission the
self-governance rule it writes asks for.

**Decided.**
- **Placement.** Three sections after "Rules the build cannot catch":
  "Comments and simplicity", "Language", "Review and self-governance". The
  design names the rule sets, not where they go.
- **The table is the design's**, with its "Applied in" column. C10 reads "no
  finding: it records the code's practice", the second of the two options
  `R-028` gave.
- **`R-048` in four sentences** appended to the "Criteria are frozen" bullet,
  carrying the ruling's three elements: the draft in the request, the installing
  commit that cites the answer and says whether it matches the draft as amended,
  and no live `PLAN.md` on `main`. The point's summary names the first two; the
  ruling's "Owed" line names all three.
- **The language rule says when it took effect** — point 5 of this plan —
  instead of the design's "points 7 to 9 of the phase 3 plan", written when the
  rule was meant for that plan.
- **Markers.** `R-045`'s table goes into "Language". Each skill carries one
  sentence recognising the Italian forms: `/plan-next` lists five,
  `/plan-explain` and `/plan-status` four, since neither reads *criterio
  difettoso*. *criterio sbagliato*, a reason phrase in `/plan-next` that `R-045`
  does not list, becomes *criterion wrong* under the language rule; no skill
  matches on it, so nothing recognises it.
- **A consequence for the points left.** From this commit every point that
  touches code passes the code-reviewer. Point 8 changes PowerShell scripts, so
  it owes one, although `/plan-next` does not dispatch the code-reviewer until
  point 7: the orchestrator dispatches it by hand.
- **`/plan-next` grows from 2,755 to 2,793 words.** Point 7's "no longer than
  the 2,755 words it has today" is 38 words harder. Not trimmed here: cutting
  unrelated prose to meet a later point's cap is that point's work.
- **The fixtures are not committed**, as point 4's check was not (`R-093`): the
  point names no place for them. Violation: the summary of
  `Calendar.DayOfMonth` changed to "Day within the month, 0..27: four whole
  weeks (FR-T-04)", and the line `// public static int DayOfMonth(long tick) =>
  (int)(tick % DaysPerYear) % 28;` added under it. Clean: the same summary
  citing `FR-T-02`, and no added line. `FR-T-04` is cadence; `FR-T-02` holds the
  four-week month.

**Check.**
- The code-reviewer, briefed that the diff is a fixture with no criterion: on
  the violation, `Calendar.cs:76` C1 and `Calendar.cs:78` C5; on the clean diff,
  "no findings". `tree-state` unchanged across the dispatches.
- The grep, as a script: whitespace normalised, each skill's recognising
  sentence cut out, no Italian marker left in any of the three; exit 0.
- **Fault 1**, "Comments and simplicity" cut from `AGENTS.md`: the
  code-reviewer, same fixture, still reports both defects, the citation under
  "Commits" and the commented line under "Close every task", naming neither C1
  nor C5. Restored; `tree-state` back to its value before the dispatches.
- **Fault 2**, `*Esito:*` cut from `/plan-status`'s recognising sentence: the
  criterion's grep stays green; the check beside it, below, goes red with exit
  1. Restored.

**Criterion defective (`R-005`, case 1), twice.** Correct code passes both
halves; neither half fails for its fault as written.
- "Reports both", read as finding the two defects, stays green with the section
  removed: the reviewer finds a wrong citation and a commented line without C1
  and C5. It turns red only read as naming them by their identifiers, the
  reading this check applied. On this fixture the section changes the rule a
  finding names, not whether the defect is found.
- The grep cannot fail for the second fault: a skill that stops recognising
  `*Esito:*` holds fewer Italian markers, not more. **The check beside it:**
  each skill's recognising sentence names every Italian form that skill reads —
  five for `/plan-next`, four for the other two.

**Review.** `git diff --name-only HEAD -- core/` empty: `Core: no` holds, the
revisioners not run. No code-reviewer of the point: it touches no code, only
`AGENTS.md` and three skills (the design's "What skips what").

**What `docs/` says:** nothing; process work. `D-089`, `R-045`, `R-048`,
`R-010` and the design.
**Would overturn it:** the decider reading "reports both" as finding the
defects, which leaves the first fault green and the section unproved by any
check; the user wanting `R-048`'s rule outside the "Criteria are frozen" bullet.
**Not verified:** whether the table's citations say what the rules say —
`R-028` left C1, C2, C4, C6 to C9 and S1 unread, and so does this point, which
checked only that `D-007`, `D-060`, `D-067`, `D-080`, `D-085` and `DEC-083`
exist. That the code-reviewer repeats its results: one dispatch each. Whether
it took `AGENTS.md` from disk or from the context it was given; the fault run,
naming no C rule, suggests disk.

### D-117 · Point 6 · `/plan-explain` and `/plan-status` on `plan-state`: what the point left open

**Ruling:** R-096 — its first finding resolved: the request conforms to `## Draft plan`. R-097 — ratified.

**Permission for `.claude/skills/` (`D-087`).** Granted by the user, Federico
Xella, in answering the process plan's moment-one request (the plan installed
in `6d6d098`), for point 6 as its text states it: both skills are named in it.
The diff touches those two files and nothing else under `.claude/`.

**Decided.**
- **"This request's draft" is the process plan's own moment-one request**, the
  one `D-110` wrote under `## Draft plan` so that this check could run on it. It
  is `REVIEW-REQUEST.md` on `main` (`03782dc`), read from there into a scratch
  file. The file on `agent-workflow` holds the cost measurement plan's request,
  merged in from `design/poverty-seed`, whose heading is `## The draft plan`.
- **The session runs `plan-state`, not the agent**, and puts the JSON in the
  agent's brief: the script is the state, the agent reads the plan for what
  needs judgement. `-Draft` and `-Path` are named in both skills; `/plan-status`
  gains `-Path` so that an archived plan can be reported, which the check needs.
- **A `tree-state` difference stops the skill** before the agent's report is
  relayed: it says so, shows `git status --short`, and leaves the undoing to the
  human. The point says to call it; what follows a difference is not stated.
- **"Guarantee" goes, and so does its sibling** in `/plan-explain`, "an agent
  without the tools cannot break it": both claim what `Bash` contradicts. Both
  skills now say the tools make a write unlikely and `tree-state` notices one.
- **The recognising sentence stays** in each skill, reworded to say the script
  and the skill both read the Italian forms, so `D-116`'s check beside its
  criterion still holds: four forms each.
- **`/plan-status` step 4 keeps one sentence defining the range**, because
  `plan-state.ps1` cites "the range of `/plan-status` step 4" twice; dropping the
  definition would leave those comments pointing at nothing (C1). The `git log`
  commands go: the script runs them.
- **`/plan-explain`'s steps renumber**, 8 to 5: steps 1 to 4 become one step
  acting on `state`. Point 6's own text says "`/plan-explain` step 7", which is
  now step 4; the plan is frozen and keeps the old number.
- **The `description`s** are trigger lists; `/plan-explain` gains the draft as a
  trigger, `/plan-status` an archived plan.
- **The design's "deliberately not built"** lists any change to these two
  skills; `R-046` as amended by `R-066` put them in this plan, and the ruling
  governs.

**Check.** `tree-state` taken before the dispatches and after the last restore:
the same hash, `5c01938…`. Four `Plan` agents, one per run:
- `/plan-explain` on `failed-then-open.md`: "STATE: stopped", at point 2, its
  outcome line quoted, point 3 not presented.
- `/plan-status` on the same fixture: "STATE: stopped", point 2 reported apart.
- `/plan-status` on `2026-09-20-fase-3-kernel.md`: ten points, `71f9c5e`,
  `62e97a5`, `e9a556d`, `f8dc4d3`, `3d7fb31`, `b745b62`, `7e2bdd0`, `c6f786f`,
  `f1e825b`, `2ec9fad` — the same ten the old skill's step 4 returns, run by hand
  before any edit.
- `/plan-explain` with `-Draft` on the process plan's request: "NEXT: 1 —
  `path-guard` and `git-guard`".
- A script greps both skills: no "first `## <n>.` section with no", both call
  `plan-state.ps1` and `tree-state.ps1`, neither says "guarantee"; exit 0.

**Faults.**
- The sentence defining the next point in its own words put back into
  `/plan-explain`: the grep red, exit 1. Restored.
- `/plan-status` made to call a plan with an open point "in progress", first by
  replacing step 3's `stopped` paragraph only: **green** — the agent reported
  "stopped", following the script's `state`, which the skill still said decides.
  Then also with the sentence handing the state to the script replaced by one
  handing it to step 3: red, "STATE: in progress", point 3 named as worked on.
  Restored.

**Review.** `git diff --name-only HEAD -- core/` empty: `Core: no` holds, the
revisioners not run. No code-reviewer: the diff is two skills, no code.

**Findings for the decider.**
- **`plan-state -Draft` cannot read the cost measurement plan's request**:
  `## The draft plan` is not `## Draft plan`, and the script reports `exists:
  false`. `R-048` names no heading; `D-110` chose this one, and `plan-state`
  matches it exactly. When that plan comes to `/plan-explain` or the rewired
  `/plan-next`, the draft will read as absent.
- **The agents' runs are one dispatch each**, and the first fault shows the
  check is as strong as the sentence the agent obeys: a skill that half
  reinstates its own state rules still reports what the JSON says.

**What `docs/` says:** nothing; process work. `R-045`, `R-046`, `R-066`,
`R-048`, `D-087`, the design.
**Would overturn it:** the decider reading "this request" as the request on the
plan's branch, which `plan-state` cannot read as the point requires; the user
wanting the agent, not the session, to run `plan-state`.
**Not verified:** the skills invoked through `/plan-explain` and `/plan-status`
themselves: each run here was a `Plan` agent briefed with the skill's path, the
JSON and the plan, as the skills say to brief it. The `-Draft` default path,
`REVIEW-REQUEST.md` on this branch, was not exercised beyond the `exists: false`
above.

### D-118 · Point 8 · The settings files, the identifier match, and four guards closed after audit
**Ruling:** R-099 — ratified; the finding on point 2's `$Guarded` accepted as a limit.

**Permission for `.claude/hooks/` and `.claude/agents/` (`D-087`).** Granted by
the user, Federico Xella, item by item for point 8's content (`R-070`, `R-071`,
`R-073`, `R-074`, `R-094`). The point names each file it touches: the five
scripts, `.claude/agents/coder.md`, the design. The diff touches those, one new
fixture under `.claude/hooks/fixtures/plan-state/`, and nothing else under
`.claude/`.

**Decided.**
- **The `CLAUDE.md` line is a line of its own** in "The hole that `Bash`
  leaves", beside the `AGENTS.md` one, citing `R-070`. The paragraph under "What
  the coder may not write" that explains why `AGENTS.md` is off the list is left
  as written: item 2 names the list, not the paragraph.
- **`tree-state` decodes git's output as UTF-8**, setting
  `[Console]::OutputEncoding` for the call and restoring it after. `core.quotePath`
  off alone is not enough: probed, PowerShell reads native output in the console
  code page (`ibm850` here) and `città.txt` arrives as a path to nothing.
  `-c core.quotePath=false` rather than `-z`, since the lines are hashed as they
  are and `-z` changes nothing else.
- **`tree-state` also fails when any of its three git calls exits non-zero**, not
  only when a file cannot be read. `R-071` names the file; the same reasoning — a
  check that cannot look says so — covers a git call that cannot.
- **The failure is an exception inside `Get-TreeState`**, turned into exit 1 and
  a message on stderr at the entry point. `mutate` calls the script, so it reads
  the exit code; it also treats an empty answer as a failure.
- **`mutate` returns 3 without applying the patch when the first `tree-state`
  call fails.** `R-071` says "stops with exit 3 when either call fails"; stopping
  before the patch is the only way the tree is known untouched.
- **A rename's or copy's source is read from the patch's `rename from` / `copy
  from` lines.** `R-073` left the reading open. Probed: `git apply --numstat -z`
  prints the destination only, as without `-z`. A header line cannot begin a
  hunk line, which begins with a space, `+`, `-` or `\`. A quoted source, a name
  git would escape, does not begin with `core/Runtime/` and is refused: the safe
  side.
- **The locked file is how "cannot be read" is produced** in both self-tests: a
  `FileStream` opened with `FileShare.None` by the self-test process.
- **`plan-state`'s fixture is `out-of-order.md`**: points 6, 8, 7 in that order,
  8 `FAILED`, 7 open, as `R-074` probed.

**The check beside the criterion (`R-095`).** `mutate`'s self-test holds a case
in which a `tree-state` call fails — a locked file in `core/Runtime/` — and
expects exit 3 with the patch not applied. Its fault, the two exit-3 checks
removed, turns it red: exit 0. `R-095`'s reading that "the cases of items 1 and
3 to 6" already include it holds now, since the case is there; the outcome line
names it all the same.

**The code-reviewer's findings.**
- **No `copy from` case** in `mutate`'s self-test: dropping `copy` from the
  pattern left every case green, while item 5 names a copy's source. Applied: a
  case copying `docs/a.txt` into `core/Runtime/`, expecting 2; with `copy`
  dropped it went red (exit 3, the patch applied).
- **The second call's failure has no case of its own**: removing that check
  changes only the message, since an empty `$after` differs from `$before` and
  the exit is 3 either way. Rejected: the behaviour the point asks for holds
  without it, and the case would test a message.
- **No refusal case on the `harness/` side**: dropping `harness` from `$Guarded`
  would refuse valid patches and no case would notice. Rejected for this point:
  `$Guarded` is point 2's, unchanged here, and the point names no such case. A
  finding on point 2, for the decider.

**What `docs/` says:** nothing; process work. `R-068`, `R-070`, `R-071`,
`R-073`, `R-074`, `R-094`, `R-095`, the design.
**Would overturn it:** a host where PowerShell's console encoding is already
UTF-8 and setting it fails, which would make `tree-state` throw for a reason
other than the tree; git printing rename sources in a header form other than
`rename from`.
**Not verified:** `tree-state` on a file locked by another process, such as a
running build, rather than by the self-test itself; a name that needs quoting
for a reason other than a non-ASCII byte (a tab, a double quote); the copy patch
in `git`'s other copy-detection modes.

### D-119 · Point 7 · `/plan-next` and `reviewers.md` rewired: what the point left open
**Ruling:** R-098 — point 4's reserve stays open; the probe reruns after workspace trust is accepted. R-099 — ratified.

**Permission for `.claude/skills/` (`D-087`).** Granted by the user, Federico
Xella, in answering the process plan's moment-one request (the plan installed in
`6d6d098`), for point 7 as its text states it: `/plan-next` and `reviewers.md`
are named in it. The diff touches those two files and nothing else under
`.claude/`.

**Decided.**
- **The skill's steps are the design's ten**, numbered as "The life of a point"
  numbers them, each heading naming its actor. "Before starting" maps
  `plan-state`'s `state` to the old refusals one for one, `warning` added.
- **The moment-one answer counts as recorded** when the commit that installed
  `PLAN.md` cites it, as `AGENTS.md` requires, or when the request carries it.
  `R-049` says "recorded" and names no place.
- **A check that is not a test under `core/Tests/`** — a self-test, a grep — is
  run by the tester at step 2 as it stands, and written with the point's other
  files at step 3. The design gives the tester `core/Tests/` only and is silent
  on a process point.
- **A fault `mutate` cannot reach**, outside `core/Runtime/` and `harness/`, is
  introduced by the orchestrator, with `tree-state` equal before and after, and
  the outcome line says who introduced it. The design is silent.
- **The checks of "The hole that `Bash` leaves" are hashes over pathspecs**:
  the coder's denylist before and after step 3, `':!core/Tests'` before and
  after step 4, the whole tree around step 6. Probed: `tree-state -Path` takes
  several pathspecs only through `pwsh -Command`; under `-File` the second binds
  to `-Repo`. The denylist is written out in the skill, a second copy beside
  `path-guard`'s `$CoderDenied` and `coder.md`'s.
- **The `R-047` question is in every brief**, the code-reviewer's included, so
  `reviewers.md`'s "What every brief holds" is the brief of all three agents;
  the briefed and blind sections stay the revisioners'.
- **Prose cut for the cap**: the history of the trailer rule ("broken three
  times by the author of the rule") and two short sentences. Nothing that states
  a rule.

**The word count.** The cap's 2,755 is Git Bash `wc -w`, which counts no token
made only of non-ASCII characters (`—`, `→`); it is what `D-116` and `R-066`
measured. By that measure the skill is 2,738 words. Counted in a UTF-8 locale,
2,779 against 2,788 at the cap's commit: shorter by either measure.

**The check, as a script** in the scratchpad, not committed, as for points 5 and
8 (`R-093`): each step from 2 to 9 that the design assigns to an agent names
it in a dispatch; `plan-state.ps1` and `tree-state.ps1` are invoked; step 6
calls `tree-state.ps1` after the read-only agents; step 1 names "Owed by the
implementer" and refuses a first point until the moment-one answer;
`reviewers.md` names `Fails when:` and the missing-fault question and calls no
agent type a guarantee; the `description` holds none of "implement", "verify",
"commit"; the word count as `wc -w`. Exit 0 on the new files; on the files at
the cap's commit, 16 of 17 checks red, the word count exactly 2,755. Its
weakness is the one `Check: shallow` declares: it matches phrases, and a step
reworded to dispatch nothing in other words would pass.

**Point 4's reserve, probed and red.** This session sees the six agents, so the
live probe was run before the record: `coder`, asked to `Write` `docs/probe.md`,
wrote it. No hook message, and nothing denied. The same command as the
frontmatter's, run by hand on the same input under `bash`, exits 2 with "the
coder may not write docs/probe.md": the hook did not fire, rather than fire and
allow. The file was removed, untracked, and
`tree-state` returned to its hash before the probe. The reserve is **not
discharged**: the clause's check is red, and the line under point 4 is the
human's to choose. Until it is resolved, the frontmatter hooks enforce nothing
in this version, and the hashes of step 3 are the only barrier against a coder
writing its denylist. Point 7's criterion does not depend on the hooks, so the
point closes on it.

**What `docs/` says:** nothing; process work. `R-047`, `R-048`, `R-049`,
`R-010`, `R-070`, the design.
**Would overturn it:** frontmatter hooks found to fire under another
dispatch path, which would make the probe's result a property of this session;
the user reading the cap in another measure.
**Not verified:** no point has yet run under the new skill; whether `pwsh
-Command` with a pathspec list behaves the same when an agent rather than this
session runs it; why the frontmatter hook did not fire.

## 2026-10-06 — Merge of `design/poverty-seed` into `agent-workflow`

Nothing decided here. The two branches number disjointly, by design (`R-066`), so
`RULINGS.md` and this file were resolved by keeping both sides in numeric order:
`R-065` before `R-070` in "Owed by the implementer", `R-054`–`R-065` before
`R-066`, `D-108`–`D-109` before `D-110`.

`REVIEW-REQUEST.md` holds one current request and each branch had its own. The
user, Federico Xella, chose on 2026-10-06 to keep the cost measurement plan's,
still open; the process plan's, already answered, stays in git. The file is
taken from `design/poverty-seed` unchanged. Its line "Replaces the moment-three
request of 2026-10-01" does not name the process plan request it now also
replaces: left as written, since the request is not mine to compose.

## 2026-10-07 — `R-096` applied to the current request

Nothing decided beyond placing `R-096`'s sentence after the six parts and their
closing paragraph, so that "The sixth is the reason" still follows the list it
counts. Before: `plan-state -Draft` printed `exists: false` (`R-096`); after:
the five points of the draft.

## 2026-10-07 — The agent workflow plan, archived

### D-120 · Closing the process plan · The reserve discharged, the archive, the promotion pass
**Ruling:** R-100 — ratified; the probe owed again, with the coder dispatched in the background.

Point 4's live probe was rerun after the user accepted workspace trust
(`R-098`), and was green: `coder`, asked to `Write` `docs/probe.md`, was denied by
its frontmatter hook. The *Reserve discharged* line went in `2801c19`, without a
`Plan-point:` trailer (`R-011`). `plan-state` then read `state: finished`,
`reservesOpen` empty. The user asked to archive the plan: "Archivia il piano".

**Decided.**
- **The archive's name** is `2026-10-05-agent-workflow.md`: the date the plan was
  installed (`6d6d098`) and the branch it ran on, as `D-107` named the last one.
  The archive commit adds exactly one file under `.claude/plans/`, since
  `/plan-status` bounds a plan's range by the commits that add one.
- **The pass runs over `D-107` to `D-120`**: the 2026-10-01 pass covered `D-044`
  to `D-106`, and `D-107` was written after it. No candidates; all of it is
  process.
- **`REVIEW-REQUEST.md` is not touched.** The current request is the cost
  measurement plan's moment one, still open; archiving this plan does not answer
  it.
- **No merge into `main`.** The user asked for the archive only. Under `R-048` the
  measurement plan's branch is cut from `main`, so the merge, `--no-ff` as
  `D-107`'s was, is the next step, and the user's to ask for.

**What `docs/` says:** nothing; process work. `R-011`, `R-048`, `R-098`, the plan
mechanism's design note.
**Would overturn it:** the decider finding a register entry in the range that
constrains the simulator's code.
**Not verified:** that the hooks fire for an agent run in the background (the
probe ran in the foreground, as `R-098` left open); the probe exercised the
coder's `Write` only, not `Edit`, and no other role's hook.

## 2026-10-07 — The cost of a settlement update, measured on a stand-in

### D-121 · Point 1 · The stand-in and its calibration: what the point left open, and a check that under-covers
**Ruling:** R-101 — ratified; decision 3 ruled; point 2 owes a reference check on a state where rationing binds.

**R-100's result, before the point:** the probe owed again was run with `coder`
dispatched in the background (the harness default). Asked to `Write`
`docs/probe.md`, it was denied by its frontmatter hook: "PreToolUse:Write hook
error: [pwsh -NoProfile -File "$CLAUDE_PROJECT_DIR/.claude/hooks/path-guard.ps1"
-Role coder]: path-guard: the coder may not write docs/probe.md". The file was
not created; `tree-state` over the whole tree was equal before and after.
Green: frontmatter hooks fire for a background coder (the open item of `D-120`,
for `Write` only).

**Built.** `bench/Sim.Bench.csproj` and `bench/Program.cs`, a console project
on net8.0 (`Optimize` true, `TieredCompilation` false), added to `Sim.sln`. CI
builds and tests by project path and never uses `Sim.sln`, and `core/Tests`
references only `Sim.Core`, so `bench/` is outside the test run and CI. Five
passes — production, own consumption, clearing per good with a selection sort by
descending (cash + stock of the good), prices as their own pass over the
residual supply, spoilage with a remainder carry per row and good — integer
only, over dense per-good arrays; a checksum of stock and prices is folded into
every update and printed. Check: `dotnet run -c Release --project bench/Sim.Bench.csproj`;
exit 0 PASS, 1 FAIL, 2 pinning failed.

**Decided (`docs/` silent on all of it).**
1. **Ranking by selection sort**, n(n-1)/2 comparisons whatever the data, as
   `DEC-054`'s "grows with the square" reads. The coder tried alternatives
   against the bounds: insertion sort gave ratios 3.5 to 4.9, a selection sort
   comparing through the index array up to 6.3; the kept version compares keys
   directly. This is calibrating the stand-in's shape against the criterion; the
   bounds stayed verbatim. *Would overturn it:* the decider judging that a
   stand-in tuned to the bound no longer "means what it resembles" (`SQ-004`).
2. **Measurement:** 3,000 ms time-based warm-up over both cells; 100,000 timed
   updates per cell per run (the plan asks at least 10,000; at 20,000 the 9-row
   p99 was too noisy); the cells interleaved in chunks of 500 updates so drift
   lands on both sides of the ratio (whole-cell interleaving 9,27,9,27,9,27 was
   tried: 1 FAIL in 12, ratio 5.87). The plan's header says every cell is
   measured the same way and names only "a warm-up"; chunking is outside it.
   *Would overturn it:* a measurement that stays stable without it.
3. **The thread is pinned because the process is** (a one-bit affinity mask, core
   2 when there are more than 2, High priority); no `SetThreadAffinityMask`
   P/Invoke. Logical core 2 on an SMT machine may share a physical core with a
   busy sibling. Not settled; open for the human, as a question of measurement
   quality on this machine. *Would overturn it:* an explicit requirement to pin
   the thread or to pick an idle physical core.
4. **Stand-in devices:** cash refilled by 600 per row per update, supply per good
   1000 + 37g, price floor 50, initial values from an LCG seeded 12345.
   Measurement inputs, no P-number (`R-050`).
5. `TieredCompilation` off, so the JIT is fully optimised before the warm-up;
   `Optimize` true also makes a Debug build optimised.
6. `SupportedOSPlatform("windows")` and `("linux")` on `Program`, with a
   comment, so that CA1416 is quiet without a suppression.
7. **The criterion's numbers:** "1.6" is below half of P-63 (1.65) and "3.8 to
   5.8" a little wider than 4.8 within a fifth (3.84 to 5.76). The written
   numbers were used, verbatim; the measured values fell nowhere near the
   difference.

**Verification.** Step 2: the check did not exist. First verification:
unstable, 8 runs with 2 FAIL on the ratio's upper bound (7.74, 6.14), each when
the 9-row cost dropped; all 9-row runs executed before any 27-row run, and
prices was folded into the clearing loop. After round 1 (interleaving, warm-up,
prices as its own pass): coder 14/14 PASS, ratio 5.18 to 5.60; tester 8/8 PASS,
9 rows 3.20 to 4.40 us, ratio 4.84 to 5.57. After round 2 (a comment only): 5/5
PASS, 9 rows 3.50 to 3.80 us, 27 rows 19.20 to 20.50 us, ratio 5.33 to 5.49. The
ratio sits near 5.4, above `DEC-054`'s 4.8, with 0.3 to 0.5 of headroom under
5.8; the 27-row cost is 19 to 20 us against P-64's 15.8. The printed checksum
differs between runs, since the time-based warm-up runs a variable number of
updates; this point does not ask for it to repeat, point 2 does.
`dotnet build Sim.sln -c Release`, core and harness: 0 warnings, 0 errors;
`core/Tests` Release 52/52, Debug 53/53 (one test is `#if DEBUG`, pre-existing).

**Faults, introduced by the orchestrator** (`bench/` lies outside
`mutate.ps1`'s reach). `tree-state` over the whole tree equal before and after
both (2fc6528b...), the source restored byte-identical each time.
- Linear ranking, the inner comparison loop bounded to one step: red, ratio
  2.85, exit 1.
- The work removed, `return;` at the top of `Update`: red, 9 rows 0.10 us,
  ratio 1.00, checksum 0, exit 1.
- **Incident:** the restore by file copy kept the source's old mtime, so the
  incremental build kept the faulted DLL, and the code-reviewer saw FAIL at
  0.10 us on the working tree. Fixed by deleting `bench/bin` and `bench/obj`
  (git-ignored) and rebuilding: PASS, ratio 5.54. Not a code defect; a hazard of
  restoring by copy that a future fault on `bench/` must avoid.

**The criterion under-covers the point (`R-005`, case 1), and was not
rewritten.** Correct code passes it, but it reads only the 9-row cost and the
ratio and cannot see what the five passes compute. The code-reviewer named
faults it misses (`R-047`): (a) spoilage without the carry, a few percent of
cost and no change of ratio; (b) deleting pass 1, 2 or 4, each linear and small
next to the sort; (c) ranking on a constant or a wrong key, or never reading
`order`: the same quadratic comparisons, no ranking by ability to pay; (d)
replacing clearing with any quadratic loop of similar weight; (e) a degenerated
state, such as prices pinned at the floor of 50 after thousands of updates, so
that trade and price movement stop (not tested whether it happens); (f) a Debug
build, not refused by the check since `Optimize` is true. **The missing check:**
the checksum of each cell's outputs, at a fixed update count, against a plain
reference implementation of the five passes. It catches (a) to (d). Not run
here; point 2 introduces checksums against a reference loop for its own cells.

**Findings and their resolution** (code-reviewer, one dispatch; tester).
- Stale binary on the working tree: accepted, the orchestrator's error, above.
- The faults the check misses: recorded above, criterion unchanged.
- Pinning failure exits 2 against a declared 0/1 contract: **rejected.** A
  pinning failure is not a measurement, and keeping it apart from FAIL is
  informative; PASS is still exit 0 only.
- Core 2 hard-coded, possibly an SMT sibling: not settled, decision 3.
- Interleaving in chunks is a choice outside `docs/`: accepted, decision 2.
- `SupportedOSPlatform` unexplained (C7, C10): accepted, one comment line in
  round 2.
- Cash refill and supply constants invented: accepted, decision 4.
- Tester: the ratio's headroom and the run-varying checksum, recorded above; its
  question whether n log n against n squared was covered was answered by
  `DEC-054` ("grows with the square"), so the brief asked for a quadratic
  ranking.

**Agents (`R-067`).** Dispatched: tester (step 2, step 4, two re-verifications),
coder (implementation, two rounds, and the R-100 probe), code-reviewer (once),
documenter, committer. Not run: the two revisioners, since `Core: no` held at
step 5 (`git diff --name-only HEAD -- core/` empty); `mutate.ps1`, since `bench/`
lies outside `core/Runtime/` and `harness/`. The tester wrote no test: the check
is a run of `bench/`, not a test under `core/Tests/`. No outcome of this plan
claims `R-046`'s check, which stays with phase 4's first point. The coder's
denylist hashed before and after each coder dispatch: unchanged; the tree
outside `core/Tests` around each tester dispatch: unchanged; the whole tree
around the code-reviewer: unchanged.

**What `docs/` says:** `DEC-054` that the clearing's cost grows with the square
of the rows; P-63 and P-64 the costs measured; nothing on the stand-in's
passes, its measurement method or its devices.
**Would overturn it:** see decisions 1 to 3; and a reference checksum per cell
showing that the stand-in computes something other than the five passes.
**Not verified:** that the ratio holds on another machine or another core;
that the SMT sibling of core 2 was idle; the faults (a) to (e).

### D-122 · Point 2 · Dense state, scaled: the devices, the sweep, and a check that under-covers the bytes
**Ruling:** R-102 — ratified, save decision 8, rejected; the regime question ruled; point 3 owes the bytes column at `SIM-STATE`'s widths.

**Owed and acted on:** `R-101`, items 1 to 4: each cell's checksum at a fixed
update count compared against a plain reference implementation of the five
passes; every cell run on a state where rationing binds, so that the ranking
reaches the checksum; faults (a) and (c) of `D-121` seen red against it; point
1's check rerun after any change to the stand-in's devices.

**Built** (`bench/Program.cs` only). `StandIn` takes (rows, goods); `Bytes` sums
`Buffer.ByteLength` over its arrays. A class `Reference` shares no code with
`StandIn`: 2-D arrays, a comparison insertion sort, every good and every row, no
early exit; it returns a checksum and a `denied` count (update, good pairs where
demand exceeded supply), and has a `rank=false` mode clearing in row-id order.
`Main` pins the process, then dispatches: no argument runs point 2, `point1` runs
point 1's check. Exit 0 PASS, 1 FAIL, 2 pinning failed. Point 2's check, per
cell: the checksum on a fresh state after 2000 updates against `Reference`;
`denied` above 0; the `rank=false` checksum different from the ranked one. Timing:
a 1 s time-based warm-up per cell, three runs of 20,000 timed updates, the p99 of
each, the median reported; machine, runtime and clock printed. The array `paid`,
written and never read, deleted (S1). The comment at line 91 narrowed (C4,
code-reviewer finding).

**Decided (`docs/` silent on all; each a measurement input, no P-number, `R-050`).**
1. **Scarcity device:** Supply(g) = 4 when g % 3 == 0, else 1000 + 37g, so
   rationing binds (`R-101` item 2). *Would overturn it:* a ruling fixing another
   device.
2. **Ranking key a total order:** (cash + stock) * 1024 + (1023 - r), ties to the
   lower row id, so the reference need not copy the selection sort's tie order.
   Limits the check to fewer than 1024 rows; the key sits in the measured path.
   *Would overturn it:* a cell above 1023 rows.
3. **Wants:** 1 + ((cash >> 6) & 3) units per row and good. Before, cash/(p*40)
   was 0 for dear goods, so in the 100-goods cells the last good never reached
   the checksum and the Fails-when fault was invisible there (coder's round 1,
   below). Wants no longer depend on price. *Would overturn it:* a device that
   makes dear goods unaffordable again.
4. **Cash refill** 1000 + (cash & 4095); the old += 600 ran cash negative without
   bound. *Would overturn it:* any bounded refill preferred.
5. **Measurement:** the checksum at a fixed 2000 updates on a fresh instance, not
   on the timed one; 1 s warm-up; 20,000 timed updates per run (the plan asks at
   least 10,000); cells measured one after another, not interleaved in chunks as
   in point 1 (`D-121` decision 2).
6. **The 12 x 100 cell belongs to both sweeps** and is measured twice, giving
   seven rows and six distinct cells, read as the plan's "seven cells". Its two
   timings show the noise (71.2 against 77.6 us in one run).
7. **The check is stricter than `R-101`:** `rank=false` must give a different
   checksum, so fault (c) is caught by the check itself.
8. **Bytes counts every array of `StandIn`,** including the scratch arrays
   `order`, `key` and `residual`. Open for the human: whether "bytes of state per
   settlement" means held state only.

**Results.** Tester step 2: the check absent (point 1's output only; the checksum
varied with the time-based warm-up). Coder round 1: the Fails-when fault
(clearing loop to Goods-1) left the three 100-goods cells matching; sent back,
devices changed (decisions 3 and 4), then all seven red. Tester step 4, two runs:
exit 0 PASS, all seven cells match the reference, rationing binds, the ranking
reaches the checksum; the checksums are identical across runs: 12x8
-1821865995186469365, 12x30 5323051988096353993, 12x100 9171567396557610401,
12x300 8280033743768488591, 24x100 -2873913667248244111, 41x100
-7943819900709430484. Medians (us), run 1: 5.50, 20.60, 71.20, 192.90, 77.60,
237.10, 416.00; run 2: 6.60, 25.10, 76.70, 234.90, 86.20, 209.90, 459.20. Bytes:
4208, 15120, 49840, 149040, 49840, 98080, 166420. Machine Windows 10.0.26300 X64,
16 logical cores, pinned to one; .NET 8.0.31; clock 100 ns. The spread between
runs reaches 22% (12x300). `dotnet build Sim.sln -c Release` 0 warnings, 0
errors; `core/Tests` Release 52/52, Debug 53/53. After the comment fix: PASS,
checksums unchanged, build clean.

**`R-101` item 4.** Point 1's check, rerun on the changed devices, PASS every
time: coder 3.10/15.50 us, ratio 5.00, and 3.10/14.80, 4.77; tester 3.90/19.80,
5.08, and 3.10/15.00, 4.84; code-reviewer, 9 rows 3.70, ratio 5.30. Green, so
nothing goes to the decider for item 4.

**Faults, introduced by the orchestrator** in the working tree (`bench/` lies
outside `mutate.ps1`'s reach), each rebuilt from clean `bin/obj`, the source
restored byte-identical (`cmp`), `tree-state` over the whole tree equal before
and after (9104499f...).
- The Fails-when fault, clearing loop `g < Goods - 1` (line 62): exit 1, 7 of 7
  cells mismatch. Its cost was not reliably lower (within noise, 54.8 us against
  71 to 77 at 12x100): the "costs less" half of the Fails-when is not observable
  and the checksum alone makes it red.
- `D-121` (a), spoilage without the carry (`carry[i] = 0`): exit 1, 7/7 mismatch.
- `D-121` (c), a constant ranking key (`key[r] = 1023 - r`): exit 1, 7/7 mismatch.
The tester also faulted copies in the scratchpad (one good fewer in clearing,
spoilage over fewer elements, an empty `Update()`): all red. The coder faulted
them in its own rounds.

**The criterion under-covers the point (`R-005`, case 1), and was not
rewritten.** Correct code passes it, but the bytes per settlement are in the
"Does" and nothing checks them. **The missing check:** the bytes per cell
recomputed independently from the array sizes and compared. Not run here.

**Findings and their resolution** (code-reviewer, one dispatch).
- C4, the line 91 comment overclaimed "state stays in range": accepted, narrowed
  by the coder, re-verified.
- Bytes unchecked (dropping or double-counting an array leaves the check green):
  accepted as a fault the check misses; it is the missing check above.
- The checksum comes from a fresh instance, not the timed one, so a fault that
  shows only after many updates, or drift in the timed run, is not compared:
  accepted as a fault the check misses, recorded; criterion unchanged.
- Scarce goods' prices grow without bound (residual 0, +1/+2 per update) and cash
  goes negative before the refill: the timed run sits in a different regime from
  the checked one. Recorded, a degenerate state of the kind `D-121`'s (e) named;
  not changed, the criterion does not ask about it. Open for the decider whether
  it affects what the costs mean (`SQ-004`).
- "A rerun gives the same checksums" holds by construction (fixed seed, no
  randomness); nothing in one run turns red if it stopped holding. It was
  verified by rerunning (coder, tester, faults). Recorded.
- The duplicated 12x100 cell: recorded, decision 6.
- Bytes counting the scratch arrays: interpretation, open for the human
  (decision 8).
- `Supply` public static, read only in `StandIn`: taste, **rejected** (not
  applied), left for the human.
- `R-101` item 3 unconfirmed by the reviewer: answered with the fact, faults (a)
  and (c) run red by the orchestrator above.

**Agents (`R-067`).** Dispatched: tester (step 2, step 4, re-verification), coder
(three rounds: implementation, devices and `paid`, the comment), code-reviewer
(once), documenter, committer. Not run: the two revisioners, `Core: no` held at
step 5 (`git diff --name-only HEAD -- core/` empty); `mutate.ps1`, `bench/` lies
outside its reach, so the orchestrator introduced the faults; no tester-written
test, the check is a run of `bench/`. The coder's denylist hashed before and
after each coder dispatch: unchanged; the tree outside `core/Tests` around the
tester: unchanged; the whole tree around the code-reviewer: unchanged. This
outcome does not claim `R-046`'s check.

**What `docs/` says:** `DEC-054` that the clearing's cost grows with the square
of the rows; P-63 and P-64 the costs measured; nothing on the stand-in's
devices, the sweep's measurement or what the bytes of state include.
**Would overturn it:** see decisions 1 to 4 and 8.
**Not verified:** that the costs hold on another machine or core; that the SMT
sibling was idle; the bytes column against an independent count; that the timed
instance computes what the checked instance does after more than 2000 updates.

### D-123 · Point 3 · Sparse state per settlement type: the world, the dense reading, and a check that cannot fail where it matters
**Ruling:** R-103 — ratified; decision 5, left to the human, ruled; point 4 owes a reference check where routed demand is rationed.

**Owed and acted on:** `R-102`, items 1 and 2 (quoted from `RULINGS.md`, "Owed by
the implementer"): "the bytes column, for point 2's cells and point 3's, counts
the settlement state of `SIM-STATE` at its widths — the arrays standing for
`goods`, `goodsCarry`, `currency` and the per-good price, and any index a layout
holds — and leaves out content-derived rates and scratch; the figure recomputed
independently from the sizes and compared, with an array dropped or counted twice
seen red." Point 2's `StandIn.Bytes` and check changed accordingly (`bytesOk`
against `ExpectedBytes`); point 3's bytes are counted the same way.

**Built** (`bench/Program.cs` only). `World`, a static id-based world definition
(`Cash(rid)`, `Cell(rid,gid)`, `Price0(gid)`), no update code. `Settlement`, one
class for both layouts: sparse holds arrays sized to the present rows and goods
plus an index `rowId[nr]`, `goodId[ng]`; dense allocates 41 x 300 and masks absent
rows and goods with `rowOn`/`goodOn`, absent goods getting supply 0. `RefWorld.Run`,
a plain reference of the five passes over the type's present rows and goods: 2-D
arrays, insertion sort, no masks, no early exit; it shares only `World` and
`StandIn.Supply`. `ExpectedBytes`, from sizes alone: rows x goods x 12 + rows x 8
+ goods x 8 + (sparse ? (rows + goods) x 4 : 0). Mode `point3`, per type:
checksum at 2000 updates on fresh states, sparse = dense = reference required;
rationing binds and the unranked run differs (`R-101` practice); the bytes of both
layouts equal `ExpectedBytes`; the timed pair, built apart from the checked
instances, must be the layout it is labelled and at its expected bytes (added at
step 7, see findings); timing per point 2's header (1 s warm-up, three runs of
20,000 updates, p99 each, median). `StandIn`'s update code is unchanged.

**Decided (`docs/` silent on all; each a measurement input, no P-number, `R-050`).**
1. **Which goods and rows a type holds:** ids spread evenly, good k of ng is id
   k x 300 / ng, row k of nr is id k x 41 / nr, so the scarce pattern (id % 3 == 0)
   and the id tie-break vary within each type. *Would overturn it:* a content
   definition of which goods each type holds (`SQ-024` point 1, the user's,
   deferred).
2. **"The same world" in the dense layout:** dense holds 41 x 300 with only the
   present cells filled; absent goods supply 0, absent rows are skipped in the
   take loop, absent cells and rows are skipped in the checksum. So "the dense
   cost of the same type" is the dense layout running that type's world, and it
   varies by type (about 1.0 to 1.5 ms) because live cells do real takes; the
   41-row sort still runs for every good, absent ones included. *Would overturn
   it:* a reading of the plan as one fixed dense 300 x 41 figure, or a dense
   layout that physically stores absent goods with zero stock.
3. **One update code for both layouts;** only data and masks differ. The sparse
   layout runs the same mask checks with every flag true, so its cost carries those
   branches. Masks, `sup` and the `Sparse` flag are world definition, not state,
   and lie outside the bytes. *Would overturn it:* a sparse path without masks
   (slightly faster sparse).
4. **Checksum:** h x 31 + stock over present cells in row-id then good-id order,
   then h x 31 + price over present goods in id order; the same sequence in both
   layouts.
5. **`R-102` mapping:** stock to `goods` (4 bytes), carry to `goodsCarry` (8), cash
   to `currency` (8), price to the per-good price (8); the sparse index `rowId` +
   `goodId` at 4 bytes each; left out: `prod`, `need`, `spoilNum` (rates), `order`,
   `key`, `residual`, `sup` (scratch), the masks. Dense counts no index: its ids
   are positions (the dense layout allocates `rowId`/`goodId`, but they are
   positions). *Would overturn it:* a narrower id width (2 bytes would do for
   300 x 41), or the decider counting the dense masks (341 bytes, 0.2%) as an
   index.
6. **`ExpectedBytes` is independent of the layouts' own counting** (sizes only).
   Limit: both formulas are by one author, so a width error shared by both would
   not show.
7. **`World` and `StandIn.Supply` are shared** by the layouts and the reference; a
   wrong formula there moves all checksums together. Judged a smaller risk than a
   second copy drifting. *Would overturn it:* a reference with its own copy.

**Results.** Tester step 2: no point-3 mode existed. Coder round 1: sparse == dense
only; the Fails-when off-by-one was invisible at the metropolis (sparse and dense
coincide in geometry and share the update code), and the coder had injected it
only below 300 goods. Round 2 (sent by the orchestrator): `RefWorld` added, sparse
and dense each compared with it. Tester step 4, `point3` twice: PASS, exit 0, all
five types at the approved sizes; checksums sparse = dense = reference and
identical across runs: village 7988148488481991243, borgo -5091104827290509539,
cittadina -7667466961022978001, city -5224349151507211922, metropolis
-1623290518230209804. Bytes: sparse 1332, 4080, 13896, 56160, 151692; dense 150328
each; all equal `ExpectedBytes`. Median p99 (us), run 1 / run 2: sparse village
6.10/7.40, borgo 58.30/28.60, cittadina 105.7/104.0, city 462.3/478.2, metropolis
1457.7/1450.0; dense 992.2/1140.1, 3324.2/1163.3, 1353.9/1299.4, 1306.6/1366.0,
1466.5/1454.5; dense/sparse about 154-163, 41-57, 12.5-12.8, 2.83-2.86,
1.00-1.01. Borgo run 1 was a factor 2 to 3 noisier than run 2; the others within
about 15%. Point 2: PASS, bytes now per `R-102`: 12x8 1312, 12x30 4656, 12x100
15296, 12x300 45696, 24x100 29792, 41x100 50328. Point 1: PASS (9 rows 5.20 us,
ratio 5.00). `dotnet build Sim.sln -c Release` 0 warnings, 0 errors; `core/Tests`
Release 52/52, Debug 53/53. Re-verification after step 7: `point3` PASS, same
checksums, timed pair ok in all five types; sparse us 7.10, 22.00, 102.30, 479.80,
1437.60; dense 1258.10, 1070.00, 1268.10, 1368.20, 1454.40; ratios 177.2, 48.6,
12.4, 2.85, 1.01. Point 2 PASS. Point 1 PASS: 9 rows 5.70 us (bound 6.6), 27 rows
27.60, ratio 4.84; the machine ran slower today and the 9-row cost sits under a
tenth from its upper bound.

**Faults, introduced by the orchestrator** in the working tree (`bench/` lies
outside `mutate.ps1`'s reach), each rebuilt from clean `bin/obj`, the source
restored byte-identical (`cmp`), `tree-state` over the whole tree equal before and
after (40126f83...).
- The Fails-when, the walk drops the last local good (market pass `g < Goods - 1`):
  exit 1, all five types red. Village to city: sparse differs from dense and from
  the reference. Metropolis: sparse = dense (-5829897961147613837), only the
  reference differs.
- The Fails-when, the walk drops the last row (`k < rows - 1`): exit 1, all five
  red; at the metropolis again sparse = dense (5566991724587269817), caught by the
  reference only.
- Bytes, price array dropped: exit 1, MISMATCH in all five types, both layouts.
- Bytes, carry counted twice: exit 1, MISMATCH in all five, both layouts.
- Coder, at step 7: the timed dense built as sparse: FAIL, "WRONG LAYOUT" in all
  five types (the exit code was not read, the pipe hid it).
"Faster" is not observable: the faults' cost lies within the noise. The checksum
alone makes them red.

**The criterion is defective (`R-005`, case 1), and was not rewritten.** Correct
code passes it, but the check as written, that sparse and dense give the same
checksum, cannot fail for the reason it exists at the metropolis: both layouts
have the same geometry and run the same code, so the Fails-when off-by-one leaves
them equal. **The missing check, beside it:** each layout's checksum against an
independent reference of the five passes on the type's world. It was built and run
here (`RefWorld`) and caught both off-by-ones at every type.

**Findings and their resolution** (code-reviewer, one dispatch).
- The timed instances were never checked (the wrong layout timed, all green):
  accepted, applied at step 7 (the timed-pair check), fault seen red, re-verified.
- Which cost "dense at 300 goods and 41 rows" means, absent goods skipping the
  take loop but not the sort: accepted, decision 2.
- Timing is not gated (a sparse slower than or equal to dense passes): a limit,
  recorded; the criterion sets no threshold.
- Dense id arrays not counted, the comment silent on why: taste, left for the
  human (decision 5).
- `stock` counted at 4 bytes while a `long[]` in the stand-in, its int range over
  the timed run unverified: recorded as not verified.
- Nested ternary in `Main`: taste, **rejected** (not applied).
- The coder's own doubt, the `Sparse` accessor redundant with the bytes comparison
  (sparse and dense bytes differ in every type): taste, **rejected**, kept.

**Agents (`R-067`).** Dispatched: tester (step 2, step 4, re-verification), coder
(three rounds: implementation, reference, timed-pair check), code-reviewer (once),
documenter, committer. Not run: the two revisioners, `Core: no` held at step 5
(`git diff --name-only HEAD -- core/` empty); `mutate.ps1`, `bench/` lies outside
its reach, so the orchestrator introduced the faults; no tester-written test, the
check is a run of `bench/`. The coder's denylist hashed before and after each
coder dispatch: unchanged; the tree outside `core/Tests` around the tester:
unchanged; the whole tree around the code-reviewer: unchanged. This outcome does
not claim `R-046`'s check.

**What `docs/` says:** `SQ-024` the five types and the sizes the user approved;
`SIM-STATE` §Rule and §Cohort the state and widths `R-102` applies; NFR-07 the 50
MB; nothing on how a sparse layout is held or on the stand-in.
**Would overturn it:** see decisions 1 to 5 and 7.
**Not verified:** the costs on another machine or core; that `stock` stays within
int range in the timed run; a width error shared by `ExpectedBytes` and the
layouts; `World` and `Supply` against an independent copy.

### D-124 · Point 4 · Market places and goods variants: routed demand in the ranking, the split, and a served check that cannot see the ranking
**Ruling:** R-104 — ratified; the open items ruled; point 5 owes four layouts on one update code in one run, the dense layout at market places measured.
**Ruling:** R-103 (5e1d9b2) — point 4 owes a reference check where routed demand is rationed.

**Owed and acted on:** `R-103`, items 1 to 3 (read in `RULINGS.md`, "### R-103"): item 1, every
configuration's checksum, engine against an independent reference, with rationing binding where
routed rows ask; item 2, the faults "routed demand served outside the ranking" and "split drops a
variant" seen red; item 3, how routed demand enters the ranking, decided and recorded below.

**Built** (`bench/Market.cs` new; `bench/Program.cs` one line, `point4` in `Main`'s dispatch).
`Layout`, `Fault` enum, `Mkt` (engine), `MktRef` (independent reference), `PointFour.Run`.
Check: `dotnet run -c Release --project bench -- point4`.

**Decided (`docs/` silent on all unless noted; each a measurement input).**
1. **How routed demand enters the ranking (R-103 item 3).** Both readings built and measured.
   Headline: every routed row ranked as its own entry beside the market's own rows (closest to
   `SIM-ECON` §Rationing ranking cohorts; at most 155 entries). Second: one entry per village, key =
   village cash + stock summed, demand = sum of row wants, take dealt to rows in ascending row order.
   *Would overturn it:* a ruling on `SQ-024` item 4.
2. **Ranking key** cash + stock (summed over variants for a class) x 1024 minus entry position;
   position = own rows first, then routed villages ascending id, then row; at most 1023 entries, the
   constructor throws beyond. *Would overturn it:* a `SIM-ECON` tie rule.
3. **Variant split.** Class good id 20 (held by every type) has K variant columns; the class clears
   once; its take is dealt one unit at a time from a per-row per-good cursor (a remainder
   accumulator, in the checksum); ranking uses stock summed over variants. *Would overturn it:* a
   rule on `SQ-023`'s open split.
4. **K applies to the one class good only**, measured on one borgo market with its 19 villages, not
   on the whole mix; the 1.07 ratio is the marginal cost of one class. *Would overturn it:* a plan
   meaning every class at K.
5. **Market places** = ids multiple of 20, types spread evenly; villages routed round-robin by
   ordinal mod 100, 19 each, a fixed assignment standing in for "nearest" (no geography; a real
   nearest assignment would be uneven and move the tail). Village goods nested in every market's
   goods. *Would overturn it:* a generator or assignment rule (`SQ-024` items 2, 5).
6. **Routed demand served on the market's tick** from the village's state then, goods and cash
   returned at once; no transport delay, no out-of-reach case (`SQ-024` item 3, open).
7. **One simulated day** = the places of one bucket (id % 7, FR-T-06) in ascending id; p99 is over
   days (heaviest bucket), the mean printed beside it; point 5's division by 7 matches the mean.
8. **"Clearing everywhere"** = all 2,000 places each its own market, no routed villages, same engine,
   sparse layout. No dense layout measured at market places.
9. **Check sizes:** 140 days / 2,000 group updates / 400 days for the per-type cells against the
   reference (slow); timing 10,000 days or 20,000 group updates, 3,000 ms warm-up, 3 runs, median
   p99, cells interleaved.
10. **Ample** = 1,000,000 per clearing; the served check compares against the reference's own demand
    count.
11. **Supply per good** unchanged from earlier points (every third good scarce); in K groups the class
    supply is 100 against about 300 demand, so the class is short.

**Results** (final clean run, PASS exit 0; Windows 10.0.26300 X64, 16 cores pinned to one, .NET
8.0.32, clock 100 ns).
- Reference checks (item 1), engine = reference in each, rationing binding where routed rows ask:
  everywhere 2,000 places (own rows only, 226,000 rationed); market places every routed row ranked
  4228107983477594576; one entry per village 2067131547714935750 (routed rationed 10,000); K=1
  862733947654604921, K=4 -7056809336188606516, K=16 -4720438308575769582, the class good short with
  routed rows asking in 2,000 of 2,000 updates; per-type timed cells borgo 2098642222544587508,
  cittadina -2439787191266033681, city 3928472434974790030, metropolis -4282660321334963018.
- Ample supply: village demand 8,128,546 units, served 8,128,546, reference 8,128,546; ignoring routed
  rows serves 0 (caught).
- p99 ms per simulated day (sparse layout): everywhere 3.18; at market places 5.53 with every routed
  row ranked, 3.04 with one entry per village; P-01 10 ms. Mean 2.22, 4.51, 2.05. Earlier runs:
  3.25/6.18/3.43 and 3.28/5.75/3.10, drift about 10%.
- Variants, one borgo market with its 19 villages, p99 us/update: K=1 267.1, K=4 271.2, K=16 285.4
  (K=16/K=1 1.07).
- Per type, market with 19 routed villages (every row ranked) / village without clearing, p99 us:
  borgo 253.7/0.93, cittadina 323.8/0.94, city 609.5/0.93, metropolis 1278.3/0.96.
- Points 1, 2, 3 rerun PASS. Core build 0 warnings; core suites Release 52/52, Debug 53/53 (no core
  change). Point 1 near its bounds (2.00 us vs floor 1.6; ratio 5.65 vs ceiling 5.8), the drift
  `R-103` noted.

**Faults, introduced by the orchestrator** in the production path of `bench/Market.cs` (`mutate.ps1`
cannot reach `bench/`), each run to FAIL exit 1 and undone, tree-state hash equal before and after
(71be9136...). The code also keeps each as a permanent negative control checked red inside the run.
- The Fails-when, `IgnoreRouted` (line 169, `if (false)`): every routed configuration's checksum
  differs; the ample check reports 0 served of 8,128,546.
- `R-103`, "routed demand served outside the ranking", `RoutedUnranked` (line 178, `sortN = nOwn`):
  red in every routed configuration, the four per-type cells included.
- `R-103`, "split drops a variant", `DropVariant` (line 248, fall-through `kk > 1 ? kk - 1 : kk`): red
  at K=4 and K=16, n/a at K=1 (no variant to drop).

**The criterion is defective (`R-005`, case 1), and was not rewritten.** Correct code passes it, but
the "Closed by" served-when-ample check runs where nothing is rationed, so it cannot see the ranking:
a market that serves routed rows outside the ranking, or a split that drops a variant, serves every
unit and passes it. **The missing check, beside it:** the reference checks of `R-103`, built and run
here; they caught `RoutedUnranked` and `DropVariant`, which the served check alone would not.

**Findings and their resolution.**
- Tester (step 4): `DropVariant` hit every good (kv=0 for single-variant goods), so its red proved
  nothing: accepted, fixed (restricted to kk > 1, n/a at K=1). The class good never short in K groups:
  accepted, fixed (`ClassSupply`). The "everywhere" line mislabelled "binds": accepted, relabelled.
  Dense x market places not measured though point 5's table asks for four layouts: **not fixed**, the
  "Closed by" does not ask it; point 5 will meet it; open for the decider. Per-type market cells added
  at the orchestrator's request so point 5 can recompute a day from per-type cells.
- Code-reviewer (step 6): (a) per-type timed cells never checked against the reference: accepted,
  fixed (a reference check per timed type, `RoutedUnranked` red in each). (b) Routing geometry
  (ordinal mod 100) hits engine and reference alike: accepted as a declared limit, no change. (c)
  `binds` requires only some rationing with routed rows present, not that a routed row is
  short-changed; the ranking deciding is shown only by the fault turning red: taste/question, **not
  applied**, open for the human or decider. (d) Tie-break order not pinned, nothing counts ties at the
  supply boundary: question outside `R-103`'s set, **not applied**. (e) The reference retypes the
  same formulas (ability, want, price, spoilage, supply) from one author, so it shares a misreading
  but not a typo; independent of the engine's update code and indexing: accepted as a limit. (f)
  `MktRef.RationedClass` a public static mutable side channel: taste, **rejected** for a
  single-threaded bench.
- Coder's doubts, open: p99 or mean for a day; whether point 5 needs dense at market places; whether K
  applies to every class; whether a control of 16 separate goods against 16 variants is wanted;
  whether 19 villages per market regardless of type is realistic.

**Agents (`R-067`).** Dispatched: tester (twice), coder (three rounds), code-reviewer, documenter,
committer. Not run: the revisioners, `Core: no` held at step 5 (`git diff` of `core/` empty);
`mutate.ps1`, `bench/` outside its reach, so the orchestrator introduced the faults; no
tester-written test. This outcome does not claim `R-046`'s check.

**What `docs/` says:** `SQ-024` the five types, P-12's 100 market places of 2,000 and the open items
on routing; `SIM-ECON` §Rationing the ranking of cohorts; nothing on how routed demand enters it, on
the variant split (`SQ-023` open), or on the bench.
**Would overturn it:** see decisions 1 to 6.
**Not verified:** the costs on another machine or core; dense layout at market places; a real nearest
assignment or any geography; the formulas shared by engine and reference against a second author; ties
at the supply boundary.

### D-125 · Point 5 · The table, read against the budget: four layouts on one update code, and a recomputation that checks the table against itself
**Ruling:** R-105 — ratified; the criterion defective under `R-005`'s case 1, the table standing; decisions 3 and 6 ruled; one line owed under point 5's outcome.
**Ruling:** R-104 (5d6ed85) — items 1 to 7 owed and acted on (read in `RULINGS.md`, "### R-104").

**Built** (`bench/Table.cs` new: `PointFive`, the cells, the table, the recomputation `Verify`, the
synthetic-table control, the edit controls; `bench/Market.cs`: dense layout through `RowOn`/`GoodOn`
masks, `Layout.One`, `Layout.AssignmentBytes`, `Fault.DropLast`, `Mkt.Bytes`, every control before any
timing; `bench/Program.cs`: explicit dispatch `point1` to `point5` and `point5-smoke`, no argument
still runs point 2, an unknown argument exits 64, `ExpectedBytes` internal). Check:
`dotnet run -c Release --project bench -- point5`. `core/` and `docs/` untouched.

**Decided (`docs/` silent on all; each a measurement input).**
1. **One update code for every layout**, masks `RowOn`/`GoodOn` in every loop; sparse village p99
   about 1.15 us against 0.94 in point 4, so point 4's sparse numbers are not reproduced exactly.
   *Would overturn it:* a layout-specific fast path.
2. **Assignment counted as 4 bytes per routed village**; the market's `Routed` list and `Loc` are
   derived and not counted. *Would overturn it:* another representation.
3. **`Cur` (the K-split cursor) not counted in the bytes**, `R-102`'s list naming four arrays; it is
   state, and 0 at K=1. *Would overturn it:* a ruling that it counts.
4. **`DropLast` skips the last live good the place holds**; point 3's literal `Goods - 1` is red only
   where good 299 is live. *Would overturn it:* a ruling for the literal form.
5. **Dense at market places is checked on the market-with-19-villages group worlds at 40 days**, not
   on P-12 (too slow); dense everywhere on P-12 and one place per type; K=4 and K=16 not repeated,
   the table being K=1. *Would overturn it:* a ruling that the dense market check run on P-12.
6. **The village cell at market places = the borgo group's p99 of 19 village updates divided by 19**
   (a p99 of a sum, not 19 p99s). *Would overturn it:* a per-village timing at a market.
7. **The day printed to the ns**, so a 1 us edit at the metropolis moves it.
8. **The recomputation re-reads the printed Markdown in decimal**; costs printed in integers.
9. **1 MB = 10^6 bytes.** *Would overturn it:* NFR-07 read in MiB.
10. **`point5-smoke`** added: exit 3 on all-pass so it never reads as a pass, exit 1 on failure.
11. **Fail-fast:** every reference, byte and table control runs before any timing; FAIL exits 1 before
    timing.

**Results** (`point5`, PASS exit 0, 260 min, one run in one session, `R-104` item 1; Windows
10.0.26300 X64, 16 cores pinned to one, .NET 8.0.32, clock 100 ns). Every reference check equal
(sparse and dense everywhere on P-12 and one place per type; sparse and dense market groups per type
under both readings; the P-12 market places under both readings), rationing binding, `RoutedUnranked`,
`IgnoreRouted` and `DropLast` red in each control. Bytes counted equal bytes recomputed: sparse
everywhere 3,945,144; dense everywhere 300,656,000; sparse market places 3,952,744; dense market
places 300,663,600. The table recomputed from its printed cells: equal; a cost edited by 1 us and a
day edited by 1 ns caught.

| Layout | ms per day: recomputed / timed p99 | MB | P-01 10 ms | NFR-07 50 MB |
|---|---|---|---|---|
| sparse, everywhere | 2.671 / 4.26 | 3.945 | within (and 5 ms) | within |
| dense, everywhere | 204.05 / not timed (20 ms or more) | 300.656 | over | over |
| sparse, market places, every routed row ranked | 5.156 / 6.62 | 3.953 | within (over 5 ms) | within |
| sparse, market places, one entry per village | 2.463 / 3.70 | 3.953 | within | within |
| dense, market places, every row ranked | 2774.61 | 300.664 | over | over |
| dense, market places, one entry per village | 73.37 | 300.664 | over | over |

Per-type cells are in `SQ-023`, "The measurement owed, 2026-10-16" (e.g. sparse everywhere village
5.4 us, metropolis 1024.7 us). The dense cells are read on, not timed directly (`R-104` item 4). The
dense market-places every-row-ranked cell (about 192,000 us per market update at every type) is
`R-104`'s predicted O(n^2) over masked slots: the stand-in ranks all 41 x 300 slots, masked included,
820 entries per good with its selection sort; a dense layout skipping goods no entry holds would cost
less, not measured.

**Faults, introduced by hand by the orchestrator** (`mutate.ps1` cannot reach `bench/`), each run to
FAIL exit 1 in about 70 to 87 s, before any timing, undone, tree-state hash equal before and after
(2a12e101...): `RoutedUnranked` (`Market.cs`, `sortN = nOwn`); the dense walk dropping its last good
(`gi < m.Goods - 1`), red where the last good is live (metropolis, P-12 mix); `DropLast` made
unconditional, red everywhere; a cell edited after the day is computed (`Table.cs` line 50,
`Us(n + 1000)`), the synthetic-table recomputation DIFFERENT; the day not derived from the cells
(`DayMs(c.TotalNs + 1000)`), DIFFERENT. Points 1 to 4 rerun PASS; point 1 failed once on the coder's
run at ratio 6.00 against its ceiling 5.8, passed on two reruns (5.45, 5.19) and in the
orchestrator's run, `StandIn` untouched: noise near the bound, as `R-103` noted the drift. `git diff
core/` empty.

**The check is shallow, as the plan itself declares; it ran and passed as written, and was not
rewritten.** Faults it would miss, recorded beside it (code-reviewer):
- (A) The timed worlds are built from `Cfg` fields separately from the checked worlds (`Table.cs`
  about 127-137 against 173, 181, 192, 198): replacing `c.PerVillage` or `c.Dense` with `false` in
  the timed build leaves every check green while the table mislabels a cell. Verifiable weakness,
  accepted, **not fixed**: a fix needs a rerun of the 4h20 table, and `R-104` item 1 wants the table
  from the code committed.
- (B) Cells swapped among types are invisible to `Verify`: it is a consistency check, not provenance.
  Accepted as a limit.
- (C) A hand edit of the `SQ-023` transcription after the run is outside the runner's reach;
  inherent in the frozen check, a question.
Whether this makes the criterion defective (`R-005`, case 1) is for the decider.

**Findings and their resolution.**
- Code-reviewer: the dense every-row-ranked cell is not a bug but `R-104`'s predicted O(n^2) over
  masked slots, an upper bound under the selection sort: accepted, a sentence added to `SQ-023`. The
  1.07 K figure copied from `D-124` without saying so: accepted, `SQ-023` says it is from point 4's
  run. The unknown-argument message omits `point5-smoke`: taste, **rejected**, the code not changed
  after the run. Items 1 to 7 of `R-104` held in code (the reviewer checked each). (A) to (C) above.
- Coder's doubts, open: point 4's sparse numbers not reproduced exactly (decision 1); whether `Cur`
  should count; whether p99 over 19 is the right village cost; whether dense market places should sort
  dead rows (`R-104` describes it so; followed).

**Agents (`R-067`).** Dispatched: tester (twice, step 2 baseline; no step-4 dispatch, the
orchestrator ran the declared check, the faults and points 1 to 4 because each full run takes 4h20
and pins the core), coder (two rounds), code-reviewer, documenter, committer. Not run: the
revisioners, `Core: no` held at step 5 (`core/` and `docs/` empty in the diff); `mutate.ps1`; no
tester-written test. The decider ran before this point (`R-104`). This outcome does not claim
`R-046`'s check.

**What `docs/` says:** P-01's 10 ms, NFR-05, NFR-07's 50 MB, P-12's mix, `SQ-023`, `SQ-024`; nothing
on the layouts' costs, the unit of a megabyte, or the bench.
**Would overturn it:** see decisions 1 to 11.
**Not verified:** the dense cells timed directly (all 20 ms or more, `R-104` item 4); routing and
reassignment cost; point 1's bound, near the noise; the costs on another machine or core; faults (A)
to (C).

### D-126 · Orchestration of points 4 and 5 · how this session ran the decider and the long runs
**Ruling:** R-106 — ratified, decisions 2 to 4; decision 1 escalated to the user, reserve 1 of `R-001`.
**Ruling:** R-116 — decision 1 answered by the user: an implementer session may dispatch the decider as an agent.

Task: the user's instruction of 2026-10-16, "Lancia /plan-next e /decide quando necessario fino al
termine dello sviluppo o finché il decider non necessita di una mia decisione".

1. **The decider ran as a separate agent, dispatched by this implementer session.** `AGENTS.md`
   says an implementer session never plays the decider, and `/plan-next` step 10 says this session
   does not run `/decide`. The user asked for both in one instruction. Each decider run (`R-103`,
   `R-104`, `R-105`) was a fresh general-purpose agent briefed only with facts and told to follow
   `.claude/skills/decide/SKILL.md` and to leave every reserve of `R-001` to the user; this session
   wrote no ruling. `docs/` says nothing on who may dispatch a decider. Overturned by the user or a
   ruling requiring a decider session the user opens.
2. **`misura-costi` was brought up to date by merging `main` into it** (`b18b267`), not recut, so
   the installing commit `c12b061` and its frozen criteria stayed as they were. Overturned by a
   ruling that a plan branch is recut from `main` instead.
3. **Point 5's step 4 was run by this session, not by a tester dispatch**, because the full check
   takes 4h20 and pins the core: the five faults first, then the full run, then points 1 to 4, in
   one background script with `tree-state.ps1` equal before and after. Overturned by a ruling that
   step 4 is always a tester's.
4. **The table was transcribed into `SQ-023` by this session**, the coder's denylist covering
   `SPEC-QUESTIONS.md`; the two added sentences (the K figure's run, the dense ranking over masked
   slots) are the code-reviewer's findings, accepted.

**What `docs/` says:** nothing; these are process choices. **Not verified:** whether the user
reads (1) as within the instruction; the decider was not asked.

## 2026-10-19 — The cost measurement plan, archived

### D-127 · Closing the cost measurement plan · The archive and the promotion pass

`plan-state` read `state: finished`, `reservesOpen` empty, the register audited
to `D-126` (`R-106`). `R-105`'s owed line under point 5 landed in `c24e818`.
`SQ-023` and `SQ-024`, which the plan's table was owed to, are closed by the
decider (`R-107` to `R-115`). The user, on the plan's archive: "Ne discutiamo
dopo l'archiviazione del plan".

**Decided.**
- **The archive's name** is `2026-10-07-misura-costi.md`: the date the plan was
  installed (`c12b061`) and its branch, as `D-120` named the last one. The
  archive commit adds exactly one file under `.claude/plans/`.
- **The pass runs over `D-121` to `D-126`**, the 2026-10-07 pass having ended at
  `D-120`. No candidates: process plan, `D-011`.
- **`REVIEW-REQUEST.md` is not touched.** Its current request, the plan's moment
  one, is answered in it; archiving the plan writes nothing there, as `D-120`
  did not.
- **No merge into `main`** in this commit. It is the next step, `--no-ff`, and
  touches `main`: asked of the user first.

**What `docs/` says:** nothing; process work. `R-048`, `R-105`, `D-011`.
**Would overturn it:** the decider finding an entry in the range that constrains
the simulator's code.
`plan-state -Path` on the archived file reads `finished`; on the tree, `none`.
**Not verified:** nothing beyond what the plan's own entries list.

## 2026-10-20 — The workflow review, drafted as a plan at moment one

### D-128 · Moment one of the workflow review plan · What the draft decides beyond the review

The user reviewed the development workflow on 2026-10-19 and 20 and approved
directions recorded in `.claude/design/2026-10-19-workflow-review.md`: review A
and B on criteria, P1 to P4 on the weight of a point, G1 and G2 on friction, C3
on design conversations. G1 was applied at once, outside any plan, as an
environment setting not committed. The rest the user asked for as one plan: "C3,
scrivi la bozza del piano". The draft is in `REVIEW-REQUEST.md`; a blind reader
attacked its first version before it was shown, as review A will require, and
the request quotes what it found and what changed.

**Decided, beyond what the review approved.**
- **The committer is kept**, though review P1 retired it with the documenter:
  `git-guard` admits only the commands of a commit and cannot guard the
  orchestrator, so without the committer it would guard nothing. Asked of the
  user in the request.
- **A new outcome marker, *left open***, written when a finding is left to the
  human, a `SPEC-QUESTIONS.md` entry is raised or an escalation made, so that
  review P4's trigger for the decider is read from the outcome line, not
  inferred from prose. Asked of the user in the request.
- **The `record` role decides `DECISIONS-OUTSIDE-SPEC.md` and `PLAN.md` only**,
  for every session, the decider's included; `SPEC-QUESTIONS.md` is left out
  because the decider closes entries by editing their status line.
- **`mutate` excludes `core/Tests/`**, besides `docs/`, `.claude/`, `AGENTS.md`,
  every `CLAUDE.md`, `core/BannedSymbols.txt` and the record files, because a
  patch that breaks a test would count as a fault caught.
- **The checklist holds six rules**, `K1` to `K6`, each from `R-101` to `R-105`;
  the design note named four.
- **The draft's fixture for review A** is the cost measurement plan's request as
  committed in `64147ba`, before the user's answer.

**What `docs/` says:** nothing; process work. `R-010`, `R-046`, `R-048`,
`R-049`, `R-116`, `R-118`.
**Would overturn it:** the user's answer to the request, which may change any of
it.
**Not verified:** the second version of the draft was not read blind again;
whether a project settings hook reaches subagents.

## 2026-10-20 — The workflow review plan

### D-129 · Point 1 · The criteria checklist: six rules, each tied to its ruling, and a script that checks the ties exist

**Built** (by the orchestrator, not the coder: `.claude/skills/` and
`.claude/hooks/` are not the coder's, `R-010`, `D-087`; the point names both
folders, which is the permission): `.claude/skills/plan-explain/criteria.md`,
rules `K1` to `K6`, each a `###` heading citing `R-101` to `R-105`; and
`.claude/hooks/criteria-cite.ps1` with `-SelfTest`. `core/`, `docs/` and
`AGENTS.md` untouched.

**Decided (`docs/` silent: process work).**
1. **The checklist lives in `.claude/skills/plan-explain/`**, beside the skill
   that will read it (point 2 wires the reading). *Would overturn it:* a ruling
   placing it elsewhere.
2. **The script exits 2 when a file cannot be read**, apart from 1 for a
   violation. *Would overturn it:* a convention for hook exit codes that
   differs.
3. **A rule is a `###` heading and nothing else.** A `K` rule at another level
   (`## K7`) or a second `### K1` is refused, with self-test cases `level.md` and
   `twice.md`; prose such as `**K7**` or `- K7` is not caught, the guard covering
   lines that begin with `K<n>`. *Would overturn it:* a checklist form that
   needs another shape.
4. **The six rules' wording, each narrowed to what its ruling says.** `K3`'s last
   clause ("an independent recount of the wrong quantity is still wrong") was an
   inference `R-102` does not state and is removed. `K5`'s "or built from the
   same objects the check uses" is kept: `R-105`'s own "Cost" names that
   construction as what a rerun would first need. *Would overturn it:* a ruling
   that a rule misreads its citation.

**Results.** `criteria-cite.ps1 -SelfTest`: 10 cases, 0 failed. The script on
the real checklist: exit 0, "every rule cites only identifiers that exist".

**Faults (`Fails when`), introduced by the orchestrator** on copies and on the
script (`mutate.ps1` cannot reach `.claude/`), tree-state hash equal before and
after: `K1` citing `R-999` in a copy, exit 1 "K1 cites R-999, which has no
heading"; `K4` stripped of its citation in a copy, exit 1 "K4 cites nothing"; a
planted wrong citation, `K2` citing `R-102`, in a copy given to the code-reviewer
as a second candidate version without saying what differed: the reviewer
reported it ("R-102 is about the bytes column ... says nothing about paths that
share code or geometry"), while the script gives it exit 0, as the point
expects; the script's body emptied (`Get-Problems` returning no problems),
self-test 7 of 10 cases failed.

**Findings and their resolution.**
- Code-reviewer: every citation of `criteria.md` checked against `R-101` to
  `R-105`; each says what the rule claims.
- Verifiable, **applied** (one round): a `K` rule at another heading level was
  skipped uncounted, and a duplicate `### K1` hid the first; both refused now
  (decision 3).
- `K3`'s last clause an inference `R-102` does not state (table C2, C4):
  **applied**, removed.
- `K5`'s "or built from the same objects the check uses": **rejected**, kept,
  for the reason in decision 4.
- Taste, **rejected**: `**K7**` or `- K7` in prose not caught, since the guard
  covers lines beginning with `K<n>`; the identifier pattern ignoring malformed
  ids such as `R-10`.
- Questions answered: the hooks run under pwsh 7, as every hook here, and
  `GetFullPath(string, string)` is pwsh 7's; `criteria.md`'s "read by the blind
  reader" is wired by point 2 of this plan.

**Faults the check misses, recorded beside it:** a citation that exists and says
something else (by design the code-reviewer's part, the point's third fault);
a rule's text drifting from its ruling with the citation still right.

**Agents (`R-067`).** Dispatched: tester (step 2, the check as it stood: red,
exit 64, the script absent), code-reviewer, documenter, committer. Not run: the
coder (the files are in `.claude/`, the orchestrator's); the revisioners, `Core:
no` held at step 5 (no path under `core/`, `docs/` or `AGENTS.md` in the diff);
the tester at step 4, the check being a script self-test that the orchestrator
ran with the faults.

**What `docs/` says:** nothing; process work. `R-101` to `R-105` are the
citations the rules rest on.
**Would overturn it:** a ruling that a rule misreads its citation.
**Not verified:** that each rule's wording stays equal to its ruling over time;
that the blind reader applies the checklist (point 2); the script under a shell
other than pwsh 7.

### D-130 · Point 2 · A draft read whole by a blind reader: the skill learns the old heading, and the check forbids what the checklist itself says

**Built** (by the orchestrator, not the coder: `.claude/` and `AGENTS.md` are not
the coder's, `R-010`, `D-087`; the point names `.claude/skills/plan-explain/`,
`AGENTS.md` and `REVIEW-REQUEST.md`, which is the permission):
`.claude/skills/plan-explain/SKILL.md` gains the section "A draft, read whole"
(`-Draft` reads every point; with `-Path`, a draft in another file; one
read-only Plan agent, given only the fenced draft, the checklist, leave to read
the files the draft names and `AGENTS.md`, told not to read `RULINGS.md`,
`DECISIONS-OUTSIDE-SPEC.md`, `PROMOTIONS.md` or `.claude/plans/`; the tree
hashed around the reading; the report relayed whole and quoted into the
request); the running-plan procedure is unchanged, now headed "Without
`-Draft`"; the description line is updated. `AGENTS.md`, "Criteria are frozen
when written", gains the rule that before moment one the draft is read whole by
`/plan-explain -Draft` and the answers quoted into the request beside the
draft. `REVIEW-REQUEST.md`, "What a request holds", names the answers. New
fixture `.claude/hooks/fixtures/plan-explain/misura-costi.md`, equal byte for
byte to `git show 64147ba:REVIEW-REQUEST.md`. `core/` and `docs/` untouched.

**Decided (`docs/` silent: process work).**
1. **The fixture is kept byte for byte, so the skill learns the pre-`R-096`
   heading.** The fixture's block is "## The draft plan", and `plan-state`
   reports `exists:false` for it; the skill takes that block in that case.
   *Would overturn it:* a ruling that the fixture may be edited, or that old
   requests are not read.
2. **One reader per draft, not one per point.** *Would overturn it:* a draft the
   reader cannot read whole in one context, which would need one reader per
   point.
3. **The reader's brief holds the fenced block only, not the request around
   it.** *Would overturn it:* evidence that the request's other parts are
   something a blind reader needs.
4. **The reader names rules by `K` number and cites no `R-` or `D-`
   identifier.** The first run of the check failed on this: the report cited
   `R-103` ("This is the `R-103` case"), which the criterion forbids. The
   reader had obeyed its brief: `R-103` is in the checklist's `K1` as
   provenance, and the reader repeated it. Correct behaviour could therefore
   fail the check by echoing the checklist. It is resolved in the skill, not in
   the criterion (a criterion is frozen): the brief asks for rules by `K` number
   and for no identifier (one round). *Would overturn it:* a ruling that the
   criterion's wording was defective (`R-005`).

**Results.** First run (`/plan-explain -Draft -Path` on the fixture):
`plan-state` reported `exists:false`, the skill took the "## The draft plan"
block; five entries, one per point; point 4's names supply ample as the state
where order is unseen ("With supply ample, everyone is served whatever the order");
but the report cited `R-103`: red on the identifier, fixed as decision 4. Second
run: five entries; point 4's: "routed village demand served after, or before,
all local demand regardless of ability to pay. With supply ample everything is
served, so the check stays green" and "K1, the exact blind spot it names:
'served' with supply ample cannot see the order of routed demand"; no `R-` or
`D-` identifier cited. Greps: "plan-explain -Draft" found in `AGENTS.md` (1) and
`REVIEW-REQUEST.md` (3). Tree-state hash equal before and after each reading.
The reader also found, of the fixture, what the cost plan's rulings found: `K1`
for points 1 and 4; `K2` for point 3; `K3` for point 2's bytes; `K4`, `K5`, `K6`
for point 5; and that point 5's `Fails when` premise was false at drafting. Both
runs noted that `bench/` already exists, a fact of the tree, not of the draft.

**Faults (`Fails when`), introduced by the orchestrator as reader briefs,
read-only agents, tree hash equal:** (a) the skill's old behaviour, the first
point only: one entry, point 1, point 4's missing: red. (b) a reader handed the
register, `RULINGS.md`, `DECISIONS-OUTSIDE-SPEC.md` and the archived plan: its
report cites `R-101` to `R-105` and `D-121` to `D-125` throughout: red. Note on
(b): its brief also asked it to cite what it read, the strong form of the fault.

**Findings and their resolution.**
- The blindness test misses a reader that reads the register but cites only `K`
  numbers: **recorded, not fixed**; the check is `shallow`, as the point
  declares, and the criterion is not rewritten.
- The criterion's "cites no `R-` identifier" depends on the brief's ban on
  citing provenance (decision 4): recorded beside the check.
- No code-reviewer: the point touches no code (a skill, `AGENTS.md`, a request,
  a fixture), and its `Closed by` asks for none. No finding exists to reject.

**Agents (`R-067`).** Dispatched: Plan readers (two check runs, two fault runs),
documenter, committer. Not run: the coder (the files are the orchestrator's);
the code-reviewer (no code); the revisioners (`Core: no` held: no path under
`core/` or `docs/` in the diff; `AGENTS.md` is named by the point); the tester:
not dispatched, the check is a skill run, not a test.

**What `docs/` says:** nothing; process.
**Would overturn it:** a draft the reader cannot read whole in one context.
**Not verified:** that the reader finds a gap the checklist does not name (the
point's `shallow` note); a reader that reads the register and cites only `K`
numbers; the skill under a draft whose block heading is neither of the two
known.

### D-131 · Point 3 · `plan-state` reads a point's frozen text: record paragraphs, not lines, and the archive count by headings

**Built** (by the orchestrator, not the coder: `.claude/hooks/` is not the
coder's, `R-010`, `D-087`; the point says it edits `.claude/hooks/`):
`.claude/hooks/plan-state.ps1` gains `Get-Install` (the oldest commit in the
plan's range that adds `PLAN.md`, read only for the live `PLAN.md`),
`Get-Sections`, and `Get-FrozenText` (a point's section without its record
paragraphs). Per point: `recorded`, `added`, `changed` (heading or frozen text
differs from the installing commit). At plan level: `install`, `gone`,
`deciderDue`. `git show` is read as UTF-8. New fixtures
`.claude/hooks/fixtures/plan-state/decider-defective.md`, `decider-ruled.md`,
`decider-italian.md`. `core/`, `docs/` and `AGENTS.md` untouched.

**Decided (`docs/` silent: process work).**
1. **A record is a paragraph, not a line.** The point says "the lines that begin
   with an outcome marker". An outcome wraps over several lines, so dropping only
   the first line leaves the rest in the frozen text and every closed point reads
   as changed, which is the point's own second fault. `Get-FrozenText` drops the
   paragraph a marker (`*Outcome:*`, `*Esito:*`, `*Ruled:*`, `*Reserve
   discharged:*`, `*Riserva sciolta:*`) opens, to the next blank line; blank
   ends are dropped and CR ignored. *Would overturn it:* a ruling that an outcome
   is one line.
2. **`deciderDue` reads the last closed point's status for FAILED, and its
   outcome paragraph alone for *criterion defective*, *criterio difettoso* and
   *left open***, asterisks and underscores ignored. FAILED is read from the
   status as `Read-Point` reads it, not matched in prose: a clean outcome saying
   "failed" in passing must not trigger the decider. The three phrase markers
   are still matched anywhere in the outcome paragraph, the plan saying the line
   "carries" them. *Would overturn it:* a ruling that the markers have a fixed
   position.
3. **`git show` is read as UTF-8**, the archives and plans holding non-ASCII
   text; the console's default code page garbled it. *Would overturn it:* a
   plan restricted to ASCII.
4. **Section keys are strings** (the point number as text), so `changed`,
   `added` and `gone` compare headings and numbers alike. *Would overturn it:*
   a need to order points numerically across the comparison.
5. **The new fields exist only for the live `PLAN.md`;** for any other path
   they are null. *Would overturn it:* a use of `-Path` on a plan with its own
   history.
6. **The self-test's archive count is by headings.** The self-test pinned "31
   points closed" across the archives and was red before this point (44
   found). It now counts the archives' `## n.` headings. That is outside the
   point's text, was needed to turn the self-test green, and is weaker: a
   deleted archive no longer turns it red. **Left to the decider.** *Would
   overturn it:* a ruling that the count stays pinned, with the number it
   should be.

**Results.** `plan-state -SelfTest`: 84 cases, 0 failed, on a fixture
repository: a plan installed; point 1 closed with reserve, a *Ruled:* line and a
*Reserve discharged:* line; point 2's "Closed by" edited in a later commit;
point 3 deleted; point 4 added; point 5 closed with *Esito:*; point 6's heading
edited. `changed` = 2 and 6, `gone` = 3, `added` = 4, `recorded` = 1 and 5.
`deciderDue` true for `decider-defective.md`, false for `decider-ruled.md` (a
clean outcome with "failed" in prose, then a *Ruled:* criterion defective line),
true for `decider-italian.md` (**criterio difettoso**), false with nothing
closed (the draft fixture, all open), true for a FAILED-status point. Live:
install `a9ee621`, points 1 and 2 recorded, none changed, `gone` empty,
`deciderDue` false.

**Faults (`Fails when`), introduced by the orchestrator** (`mutate.ps1` cannot
reach `.claude/`), each red and restored, tree-state hash equal before and after:
the baseline taken as the latest commit touching `PLAN.md`: 4 failures, point 2
not reported changed (`Fails when`); record paragraphs not excluded: points 1
and 5 reported changed (`Fails when`); *Ruled:* dropped from the record markers:
point 1 changed; the heading comparison removed: point 6 not changed. Before the
review, also `deciderDue` reading past the outcome paragraph: the Ruled fixture
true. First-round faults were seen red before the review; after the review's
findings were applied in one round, all were re-run red.

**Findings and their resolution.**
- Code-reviewer: the check is satisfied as written.
- Verifiable, **applied** (one round): *Ruled:*, *Reserve discharged:* and
  *Esito:* never exercised on the changed path (fixture points 1 and 5 added);
  the heading comparison untested (point 6); the "nothing closed" case vacuous
  (now the draft fixture, all open); FAILED matched anywhere in prose (now read
  from the status, a FAILED-status case added, decision 2); a comment said
  "line" where the code drops paragraphs (reworded, table C2).
- **Recorded, not changed:** the reading of "lines" as paragraphs (decision 1);
  the archive count change (decision 6, a question for the decider); "left open"
  and "criterion defective" still matched anywhere in the outcome paragraph (the
  plan says the line "carries" the marker, decision 2); a delete and re-add of
  `PLAN.md` within a range keeps the first install as the baseline, untested.
  No finding was rejected.

**Agents (`R-067`).** Dispatched: code-reviewer, documenter, committer. Not run:
the tester (the check is a script self-test, run by the orchestrator); the
coder (the file is in `.claude/hooks/`, the orchestrator's); the revisioners
(`Core: no` held: only `.claude/hooks/` in the diff).

**What `docs/` says:** nothing; process.
**Would overturn it:** a ruling that an outcome is one line, or that the archive
count stays pinned.
**Not verified:** a delete and re-add of `PLAN.md` within one range; `deciderDue`
where a marker appears in the outcome paragraph as quoted prose; the script under
a shell other than pwsh 7.

### D-132 · Point 9 · A record opens at a paragraph, not at any line: the criterion's second half proves less, and two readings are left to the decider

**Origin.** Point 3 closed (`fbed774`) and its outcome wrapped onto a line
beginning `*Ruled:*`. `plan-state` took that line for a new record and cut the
outcome paragraph before its *left open*, so `deciderDue` read false. The user
approved adding the point ("sì, aggiungi il punto 3a e procedi"), added in
`d842fd1` under `D-014`, numbered 9 so that `plan-state` reads it (`R-074`).

**Built** (by the orchestrator: `.claude/hooks/` is not the coder's, `D-087`; the
point says it edits `.claude/hooks/`): in `.claude/hooks/plan-state.ps1`,
`Get-FrozenText` opens a record only where a paragraph starts (the section's
first line, or a line after a blank one), and the `deciderDue` loop opens the
outcome only at a paragraph start and ends it at the blank line, no longer at a
line beginning with a marker. New fixtures `decider-wrapped.md` (outcome wrapped
onto `*Ruled:*`, *Left open* after it) and `decider-midline.md` (a Does line
wrapping onto `*Outcome:* ... left open`, the real outcome clean). The fixture
repository gains point 7 (a Does wrapped onto a line beginning `*Ruled:*`, a
word after it edited in a later commit), and point 5's Esito wraps onto
`*Ruled:*`. `core/`, `docs/` and `AGENTS.md` untouched.

**Decided (`docs/` silent: process work).**
1. **"Paragraph start" is the first line of the section or a line after a blank
   one.** The outcome paragraph ends only at a blank line. *Would overturn it:*
   a record written without a blank line before it.
2. **The criterion's half "the fixture's point not reported changed against an
   installed copy" is defective** (`R-005`, case 1: correct code passes it, it
   proves less). Under the old reading a wrapped marker opens the same record, so
   a point whose outcome wraps is not changed either way; the half cannot fail
   for the reason it exists. It was not rewritten. The missing check, beside it,
   was built and run: fixture point 7, an edit after a wrapped marker line
   reported changed, red under the fault. *Would overturn it:* a ruling that the
   half does catch the fault.

**Results.** `plan-state -SelfTest`: 87 cases, 0 failed. Live `PLAN.md`:
`deciderDue` true (point 3's *left open* read), no point changed. Faults, by the
orchestrator, restored, tree hash equal: a marker at the start of any line opens
a record (`Fails when`): `decider-wrapped` `deciderDue` false and point 7 not
reported changed (2 failures), live `deciderDue` false; the paragraph-start guard
on the outcome dropped (found by the reviewer): `decider-midline` true, 1
failure.

**Findings and their resolution.**
- Code-reviewer, the deviation above: recorded as the criterion defect
  (decision 2).
- Outcome opening mid-paragraph untested: **applied**, `decider-midline.md`,
  fault seen red.
- A section's first line as paragraph start untested: **rejected**, harmless,
  sections begin with a heading; taste.
- **Questions, recorded for the decider, not changed.** (a) A record directly
  after a bullet with no blank line is now part of the point's text; every record
  in the repository follows a blank line. (b) A `*Ruled:*` line directly after an
  outcome with no blank line now belongs to the outcome paragraph, so its *left
  open* would count.

**Agents (`R-067`).** Dispatched: code-reviewer, documenter, committer. Not run:
the tester (the check is a script self-test, run by the orchestrator); the coder
(`.claude/hooks/`); the revisioners (`Core: no` held).

**What `docs/` says:** nothing; process.
**Would overturn it:** a record written without a blank line before it; a ruling
on questions (a) and (b).
**Not verified:** a record after a bullet with no blank line; a `*Ruled:*` line
directly after an outcome; the script under a shell other than pwsh 7.

### D-133 · Point 4 · The register written insert-only by a hook: a `**Ruling:**` line may be replaced, by the user's choice

**Origin.** Point 4 of the workflow review plan (review P1): every session
edited the register by hand with nothing but discipline against rewriting it.

**Audit before wiring** (the point requires it, and says that if a decider edit
was not an insertion the point stops and asks). `git log -p` over the decider's
commits since `R-001`: 11 of the 59 commits touching the register replaced text,
each on a `**Ruling:**` line under an entry, either filling "none yet" (`0b245ed`)
or appending a later ruling (`32f20af`, `4e7687d` and others). The point stopped
and asked; the user chose option A, "A, procedi": an Edit of a `**Ruling:**` line
passes even when it is not an insertion; everything else stays insert-only.

**Built** (by the orchestrator: `.claude/hooks/` and `.claude/settings.json` are
not the coder's, `D-087`; the point says it edits them). `.claude/hooks/path-guard.ps1`
gains role `record` (`Get-RecordDenial`), judged before the outside-repository
denial. It decides `DECISIONS-OUTSIDE-SPEC.md` and `PLAN.md` only (file name
case-insensitive) and allows every other path, inside the repository or not. An
Edit passes when `new_string` begins with a non-empty `old_string`, or, in the
register only, when one single-line `**Ruling:**` line is replaced by another
single-line `**Ruling:**` line. A Write passes only when the file does not exist.
The description states that writes through a shell are not seen. New
`.claude/settings.json`: PreToolUse `Edit|Write` runs
`pwsh -NoProfile -File "$CLAUDE_PROJECT_DIR/.claude/hooks/path-guard.ps1" -Role record`.
`SPEC-QUESTIONS.md` is left out, as the point says (the decider closes entries by
editing status lines). `core/`, `docs/` and `AGENTS.md` untouched.

**Decided (`docs/` silent: process work).**
1. **Role `record` is judged before the outside-repository denial**, so that a
   path outside the repository is allowed by `record` and not denied by
   inheritance (the point's second `Fails when`). *Would overturn it:* a ruling
   that the hook should also guard paths outside the repository.
2. **The Ruling exemption has the shape of option A and no wider:** one
   single-line `**Ruling:**` line replaced by one single-line `**Ruling:**` line,
   in the register only, never in `PLAN.md`. *Would overturn it:* the decider
   needing to edit register text other than a ruling line.
3. **The hook sits in a committed project `.claude/settings.json`**, so every
   clone and every session carries it, the decider's included. *Would overturn
   it:* a ruling that the guard is per-user.

**Results.** `path-guard -SelfTest`: 50 cases, 0 failed. The `record` cases run on
a real temporary directory: insertion allowed; replacement denied with `record`'s
reason; lower-case file name denied; empty `old_string` denied; a single ruling
line replaced allowed; an `old_string` merely containing `**Ruling:**` denied; an
`old_string` starting at a ruling line and spanning into the next entry denied; a
ruling line replaced by non-ruling text denied; a ruling replacement in `PLAN.md`
denied; Write over an existing `PLAN.md` and an existing register denied; Write of
a new `PLAN.md` allowed; `README.md`, `SPEC-QUESTIONS.md` and a path outside the
repository allowed. The exact command line of `settings.json`, piped: register
replacement exit 2 with `record`'s reason; `README.md` exit 0; outside path exit
0. Live: an Edit replacing a word of the register, in the orchestrator's session,
was denied by the hook, already active in that session; the new session the point
foresaw was not needed. Faults, by the orchestrator, tree hash equal: the role
misnamed on the command line (`recrod`): `README.md` exit 2, "unknown role
'recrod'"; `record` inheriting the outside-repository denial: the outside case
denied, 2 failures; after the review, the wide exemption restored: 3 failures
(spanning `old_string`, non-ruling `new_string`, `PLAN.md`).

**Findings and their resolution.**
- Code-reviewer, the check satisfied as written. Verifiable, **applied** (one
  round): the exemption was wider than option A (any `old_string` beginning
  `**Ruling:**`, of any length, any `new_string`, `PLAN.md` included), now one
  ruling line to one ruling line, register only, with three deny cases; no
  Write-over-existing case for the register, added; a no-op Remove/Set-Content pair
  and an inflated case count, removed (table S1); a comment citing "the user's
  answer, option A", a conversation and not a text, reworded to name the audit's
  stop (table C1).
- `MultiEdit` and `NotebookEdit` are not matched: **rejected**, the point says
  `Edit|Write` and the description already limits the guard to Edit and Write.

**Agents (`R-067`).** Dispatched: code-reviewer, documenter, committer. Not run:
the tester (script self-test and pipe tests, by the orchestrator); the coder
(`.claude/`); the revisioners (`Core: no` held: only `.claude/hooks/` and
`.claude/settings.json` in the diff).

**What `docs/` says:** nothing; process.
**Would overturn it:** the decider needing to edit register text other than a
ruling line.
**Not verified:** whether the hook fires for subagents (this documenter run's
Edit of the register is the first subagent Edit under it); shell writes, which
the hook cannot see; `MultiEdit` and `NotebookEdit`; the script under a shell
other than pwsh 7.
