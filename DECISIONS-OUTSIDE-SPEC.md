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

`Range(draw, count)` returns `0 .. count - 1`; callers wanting `min .. max`
write `min + Range(draw, max - min + 1)`. No specification input. Rejected:
overloads for inclusive and exclusive bounds, as unrequested surface.

### D-004 · 5. A-11 is checked in debug builds only, and by `checked` rather than an assert

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

**Promoted 2026-09-19 as `DEC-083`.** It was a rule living in a source comment
and unenforced by the build; it is now a numbered decision, still unenforced by
the build.

Written into the `HashChannel` comment, next to the rest of the channel
contract, since that is what a caller reads when choosing one. Entity keys
separate themselves by generation; non-entity keys have nothing to separate them
but the channel. Unenforced by the build, like the rest of that contract.

### D-008 · 3. CI runs the debug build

A `dotnet test -c Debug` step, so the A-11 guard and
`A11_ApplyRefusesAProductThatLeaves64Bits` run somewhere other than a
developer's machine. Added as a step in the existing job rather than a matrix
over configurations: a matrix would also fan out the wealth-band steps, which
are release-configuration checks and have their own reason to run twice.

### D-009 · 4. The histogram tolerance was left alone

`NFR03_RangeCoversTheIntervalEvenly` keeps its 5% band. Review allowed widening
it if it had been sized around exact uniformity; it was not. 5% is about 5.4
standard deviations of sampling noise at seventy thousand draws, while the
modulo's own deviation is of order 2^-61. Widening would have loosened a bound
that has nothing to do with the claim that changed. The reasoning is now in the
test.

### D-010 · 5. Left as found

The remark on `Hash64.Of` still phrases channel separation as the sensible
answer rather than as the rule it now is. `HashChannel` states the rule; further
edits to `Hash64` were outside the review.

---

## 2026-09-17 — The plan mechanism and its three skills

Branch `plan-e-skill`, planned in `PLAN.md`, designed in
`.claude/design/2026-09-17-plan-e-skill.md`. Entries name the plan point they
belong to.

### D-011 · Whole plan — `docs/` is silent, and stays silent

Every point of this plan declares `docs/` silent, so the declaration is made once
here instead of five times in the plan. `SIM-REQ` §18 plans the simulator; it
does not plan how the simulator gets built, and it should not start. Process work
that wrote itself into the specification would be indistinguishable, a year from
now, from the design the specification exists to hold.

### D-012 · Point 1 — the rule in `AGENTS.md`

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
