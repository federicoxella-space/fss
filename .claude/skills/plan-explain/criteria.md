# What a point's check must prove

Read when a plan is drafted, and by the blind reader of a draft before moment
one. Each rule is a gap a past criterion left open, named by the ruling that
found it; a new rule arrives the same way, from a ruling, never invented here.
`.claude/hooks/criteria-cite.ps1` checks that every rule cites a ruling or a
register entry that exists. Whether the citation says what the rule claims is
read, not checked.

A rule applies to a point when the point does what the rule names. A point the
checklist does not fit is read on its own terms.

### K1 — A computation is checked against a reference, where the branch that matters binds

A check that compares a computation's output with an independent reference
proves it only on a state that exercises what the point is about. A ranking
checked where supply never runs short is never read: every row is served
whatever its rank, and a ranking on a constant key passes (`R-101`). The same
blind spot: a "served" check run with supply ample cannot see the order in
which routed demand is served (`R-103`). Name the state where the branch binds,
and make the check fail if that state is not reached.

### K2 — Two paths that share their code cannot check each other

A check of equality between two paths that share their code and their geometry
cannot fail where they coincide: an off-by-one in the shared walk leaves them
equal. The check needs a reference that shares neither (`R-103`). This is about
the check, not the implementation: two layouts may run one update code, so that
a comparison between them is about data (`R-103`, `R-104`).

### K3 — A reported number is recomputed, and counts what the specification defines

A figure the point reports is recomputed from an independent source, not read
back from the code that produced it. And it counts the quantity the
specification defines, not the one the code happens to hold: a bytes column
read against NFR-07 counts the state of `SIM-STATE` at its widths, not every
array of the program (`R-102`).

### K4 — Cells that are compared are measured alike, in one run

Cells set against each other are measured by the same method, in the same run,
on the same update code. A cell the point names is measured or marked missing,
never derived from another or copied from an earlier run (`R-104`).

### K5 — A check of a table's arithmetic says nothing of its labels

A check that sums or compares a table's own cells proves that the arithmetic
holds, not that each cell is what its row and column say. The labels are read,
or built from the same objects the check uses (`R-105`).

### K6 — A `Fails when:` resting on a premise is read again when the premise moves

A `Fails when:` that rests on what the point does — "the point gathers numbers
already measured" — stops being true when a later ruling changes what the point
does. When a ruling adds work to a point not yet run, its `Fails when:` is read
against the new work, and a premise the ruling made untrue is reported
(`R-105`).
