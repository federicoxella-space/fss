# Plan — the review request, and the three moments that call for one

**Kind:** process
**Exit condition:** `REVIEW-REQUEST.md` exists with its six required parts and
the three moments that produce one; `AGENTS.md` and the design state when it is
due; the three skills say, at each of those moments, that one is owed.

**Phase:** none, process work.
**Branch:** `plan-e-skill`, third plan on this branch.

---

Asked for on 2026-09-19 by the human, who is away: the mechanism records
everything and asks for a human opinion at no particular time, so the opinion
arrives when they happen to look. Three moments are worth interrupting for, and
the review that named them is quoted in point 1.

## 1. `REVIEW-REQUEST.md` and the three moments

- **Does:** creates the file and defines what it holds — which moment, which
  branch or commit, what changed in one line, the decisions taken outside the
  specification in this block, the open questions about the specification, and
  what was not verified. And names the three moments that produce one: **before
  a plan starts**, where a wrong criterion costs least and the plan is short;
  **at a phase gate**, where the whole is looked at rather than each point; and
  **when the work meets the specification**, meaning a `SPEC-QUESTIONS.md`
  entry or a register that has accumulated promotion candidates.
- **Closed by:** the file exists, lists the six parts and the three moments,
  and carries a first real request rather than a template with nothing in it —
  this branch has three plans, two archived, and a human who has not seen any
  of it since `046751f`. `AGENTS.md` and the design say when one is due.
- **Check: shallow** — greps prove the parts are there, not that the request is
  worth reading. A better criterion would be the human saying whether the first
  one told them what they needed; that is the point of the artefact and it
  cannot be checked from inside.
- **Core:** no — touches no file under `core/`.

*Esito:* 2026-09-19. `REVIEW-REQUEST.md` exists with the three moments, the six
parts, and a first request that is real: it reports seventeen unpushed commits,
two open specification questions, four promotion candidates, and five things
that were not verified — including that the review protocol has never run at
all. `AGENTS.md` and the design say when one is due. The marker held: six greps
proved the parts are present and none of them says the request is worth
reading. Decisions: register, `D-033` — six entries, including why the first
request covers all three moments at once and why that is written into the
request rather than smoothed out of it. Review: not run, `Core: no`, exemption
verified against the diff.

## 2. The three skills say when a request is owed

- **Does:** wires the moments into the procedure, so that the reminder does not
  depend on anyone remembering. `/plan-explain` and `/plan-next`, on the first
  point of a plan, say a request is owed before the work starts.
  `/plan-next` says one is owed when a point files a `SPEC-QUESTIONS.md` entry
  or the promotion pass produces candidates. `/plan-status` says one is owed at
  a phase gate, and reports when the last one was written.
- **Closed by:** each of the three names the moments it is responsible for; the
  reminder is a statement that one is owed, never a request written on the
  human's behalf.
- **Check: shallow** — greps again. A better criterion would be a plan run
  start to finish under these rules, counting the moments that produced a
  request against the moments that should have.
- **Core:** no — touches no file under `core/`.

*Esito:* 2026-09-19. `/plan-explain` announces moment one, `/plan-next`
announces moments one and three, `/plan-status` announces moment two and
reports when the last request was written; all three say the request is owed
and none writes one. The marker held: the grep proves each names its moments
and nothing proves the reminder lands at the right time. Running the check
caught two of its own clauses failing on a case-sensitive pattern, which is the
argument for running a shallow check rather than reading it. Decisions:
register, `D-034` — four entries. Review: not run, `Core: no`, exemption
verified against the diff.
