# The two reviewer briefs

Read at step 4 of `/plan-next`, and only for a point that declares `Core: yes` —
one that changes behaviour under `core/Runtime/`, or touches determinism,
currency, or state.

Dispatch both **in parallel, in one message**. The blind reviewer must not see
the briefed one's output, now or later; if a second pass is ever needed, brief a
fresh blind reviewer rather than showing this one what the other said.

## Agent type

Use a read-only agent type — `Plan`, or `Explore` if that is unavailable. Both
lack `Edit` and `Write`, which is the point: **"do not edit the code" is enforced
by the tools the reviewer has, not by its willingness to comply.** Never use a
general-purpose agent for a review.

Neither type is a purpose-built reviewer. The brief carries the role.

## What both receive

- The diff under review, and the repository to read freely.
- `docs/`, which outranks the code, and `AGENTS.md`.
- The task as the human stated it, so that scope can be judged. Without it a
  reviewer flags work that was explicitly requested.
- **The point's frozen criterion, quoted verbatim from `PLAN.md`:** its
  statement, what it cites, and its check, including a `Check: weak` marker if
  it carries one. Paraphrasing it here would let the author soften the standard
  the work is measured against, so it is pasted, not summarised.

And with it, the question both are asked first:

> **Does this code satisfy the check the plan declared, as written?**

Not whether the code is good, not whether it is what the author intended — the
reviewers are the only readers in the chain positioned to answer that one, and
they are the only ones who used to be given everything except the criterion. A
check that passes for a reason the criterion did not ask for is a finding. A
check marked weak that turns out to be verifiable properly is a finding too.

## The four shared rules

Put these in both briefs, in these words or closer:

1. **Read the specification before judging the code.** `docs/` predates the code
   and outranks it. A finding that contradicts `docs/` is a finding against the
   code, and a finding that ignores `docs/` is noise.
2. **Build it and run the tests.** `dotnet build core/Sim.Core.csproj -c
   Release`, then the suite in Release and in Debug. A review that only reads
   prose misses what only execution shows.
3. **Verify the numerical claims you find in comments.** Orders of magnitude,
   probability bounds, "cannot overflow", "uniform", "terminates". This
   repository has had a comment that was confidently wrong, and reading it was
   not enough to tell.
4. **Do not edit anything. Report.** The author applies what you find.

## And the fifth, which matters most

**Finding nothing is a valid result, and will be reported as such.**

State it explicitly in both briefs. A review agent exists to find something, so
it tends to find something; and an author with a section to fill tends to promote
the weakest finding to fill it. Both failures produce a review that reads as
thorough and is worthless. If the point is sound, say the point is sound.

## The briefed reviewer

Receives, in addition to the above, the author's doubts and the alternatives the
author rejected.

**Pass the doubts as questions, never as conclusions.** "Does this reading of
DEC-002 hold, given that the document names rejection sampling as a cost?" is a
review. "I read DEC-002 as forbidding variable draw counts rather than loops —
confirm?" is a request for agreement, and it will be granted.

Ask it to answer each doubt with a position and the evidence for it, and to say
when a doubt is unanswerable from what the repository contains.

## The blind reviewer

Receives the diff, the repository, the specification and the task — and **nothing
of the author's reasoning**: no doubts, no rejected alternatives, no explanation
of why the code is shaped as it is. It is there to find what the author did not
think to doubt, which it cannot do if handed the author's list of doubts.

Do not summarise the design intent for it. If the code needs an explanation to
be judged, that absence is itself a finding.

## After both return

Findings are resolved at step 5 of `/plan-next`: verifiable ones applied, taste
and disagreements passed to the human, factual errors answered with the fact.
All of it reaches the register, including what was rejected.

Agent review does not replace human review. It is good at "you claimed X and the
code does Y" and at sweeping for specification compliance. It is weak at the kind
of insight that has already overturned a design in this repository once, which
arrived from a human reading one function closely.
