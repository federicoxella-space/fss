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

**Known limit:** two different *kinds* of non-entity subject drawing in the same
channel share one key space. Giving them separate channels is the answer
recorded in the source; nothing enforces it.

### 3. `Range` rejects by re-mixing the value, not by taking another index

DEC-002 records as a cost that "algorithms consuming a variable number of draws,
such as rejection sampling, need rewriting to a fixed draw count". An unbiased
reduction needs rejection somewhere. Read narrowly, that sentence forbids
`Range` entirely.

Decided: the cost DEC-002 names is a *variable number of draws*, because that is
what desynchronises subsystems and breaks constant-time replay. `Range` consumes
exactly one draw at one `index` and re-mixes the value internally, staying a
pure function of the five coordinates. Loop probability is below 2^-32 per call.

This is the entry most worth a human's second look. If the intent of DEC-002 was
to forbid unbounded loops rather than variable draw counts, `Range` has to go
back to a biased multiply-shift and the bias accepted in writing.

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

### 6. CI does not exercise the guard

The workflow builds and tests in Release only, so the guard and its test
(`A11_ApplyRefusesAProductThatLeaves64Bits`, under `#if DEBUG`) never run there.
Left as is: adding a Debug job was outside the task. Recorded so that the gap is
not mistaken for coverage.

### 7. Process choices

- One commit rather than two. Splitting DEC-002 from A-11 would have to split
  `PrimitivesTests.cs`, leaving an intermediate commit whose A-11 test fails in
  Debug. The message names both identifiers.
- Committed to a branch rather than to `main`, against the repository's own
  history of committing to `main` directly.
