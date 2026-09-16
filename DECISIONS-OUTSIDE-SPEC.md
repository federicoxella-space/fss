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
