# The review briefs

Read at step 6 of `/plan-next`. The two `revisioner`s run for a point that
declares `Core: yes` — one that changes behaviour under `core/Runtime/`, or
touches determinism, currency, or state; the `code-reviewer` for every point
that touches code. "What every brief holds" is the brief of all three; the two
sections after it are the revisioners' alone.

Dispatch them **in parallel, in one message**. The blind reviewer must not see
the briefed one's output, now or later; if a second pass is ever needed, brief a
fresh blind reviewer rather than showing this one what the other said.

## Agent type

`revisioner` and `code-reviewer`, defined in `.claude/agents/`. Never a
general-purpose agent. Both lack `Edit` and `Write` and both hold `Bash`, so the
tools make a write unlikely, not impossible. What notices one is the
`tree-state.ps1` hash that step 6 takes before the dispatch and after the last
reviewer returns.

## What every brief holds

- The diff under review, and the repository to read freely.
- `docs/`, which outranks the code, and `AGENTS.md`.
- The task as the human stated it, so that scope can be judged. Without it a
  reviewer flags work that was explicitly requested.
- **The point's frozen criterion, quoted verbatim from `PLAN.md`:** its
  statement, what it cites, its check, including a `Check: weak` marker if it
  carries one, and its `Fails when:` line. Paraphrasing it here would let the
  author soften the standard the work is measured against, so it is pasted, not
  summarised.

And with it, the two questions every reviewer is asked:

> **Does this code satisfy the check the plan declared, as written?**
>
> **Which fault would the check miss?** A change to the code that breaks what
> the point asks and leaves the check green.

Not whether the code is good, not whether it is what the author intended — the
reviewers are the only readers in the chain positioned to answer that one, and
they are the only ones who used to be given everything except the criterion. A
check that passes for a reason the criterion did not ask for is a finding. A
check marked weak that turns out to be verifiable properly is a finding too. So
is a fault the `Fails when:` line does not name and the check would miss: it
goes to the register with the others, accepted or rejected with its reason, and
the criterion does not change (`R-047`).

## The five shared rules

They live in `.claude/agents/revisioner.md`, the body every `revisioner`
receives, so that each rule has one home.

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

Findings are resolved at step 7 of `/plan-next`: verifiable ones applied, taste
and disagreements passed to the human, factual errors answered with the fact.
All of it reaches the register, including what was rejected.

Agent review does not replace human review. It is good at "you claimed X and the
code does Y" and at sweeping for specification compliance. It is weak at the kind
of insight that has already overturned a design in this repository once, which
arrived from a human reading one function closely.
