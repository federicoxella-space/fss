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

### D-038 · `SQ-003` decided, and the edit left to a human

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

`D-039` and `D-041` between them say when each applies: **replaced** when the
situation the request describes has moved, **amended** when one of its items is
answered and the rest still stands. Here the situation moved — the request
described moment three, and the moment is now one — so it was replaced whole.

One item of the old request survived the replacement and was carried forward
verbatim in substance: the look at the three `docs/` lines of `7decada`. It was
never answered, and an unanswered item does not close because the request around
it did. Replacing a request is not a way of retiring what it asked for.

### D-045 · Moment one was served without a plan to read, and says so

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

§20 was confirmed and `SQ-004` closed, both before any point started, so point 7
is unblocked. The plan says so in the precondition section that declared the
block, dated, with a line stating that nothing above it was rewritten.

The alternative was editing point 7 to drop its blocked clause. Rejected:
criteria are frozen when written, and while a met precondition is not a
criterion, the edit would be indistinguishable in the diff from one that was.
**The block was deliberately written outside the point for this reason**, and
taking the other path would have wasted the precaution.

### D-054 · The request is marked answered, not replaced

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

**Ruling:** R-003, R-004 — both open questions resolved; the rest of the entry awaits the register audit.

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
