# Review request

The mechanism in this repository records everything and asks for nothing. A
human opinion therefore arrives whenever one happens to be offered, which on a
long branch means after the work that needed it is already built on.

This file interrupts. It holds the current request for a human's attention, and
is replaced by the next one; the old ones are in git.

## The three moments

Only three are worth interrupting for. Outside them the record is enough.

1. **Before a plan starts.** The cheapest moment there is: a wrong criterion
   found here costs one conversation, found at the end it costs every point
   built on it. And the plan is short — a few hundred words against a branch.
2. **At a phase gate**, from `docs/SIM-REQ.md` §18. Phase 3 closed, phase 4
   closed. This is the moment for looking at the whole rather than the point,
   which nothing else in this mechanism does: `/plan-next` sees one point,
   `/plan-status` sees one plan.
3. **When the work meets the specification.** A `SPEC-QUESTIONS.md` entry is
   raised, or the promotion pass produces candidates for `SIM-DEC`. These are
   the return channel, and a return channel nobody reads is a drawer.

A request is written **at** those moments, not near them, and never instead of
them: writing one does not authorise continuing past a blocking question.

## What a request holds

Six parts, all of them:

- **Which moment**, of the three.
- **Which branch or commit**, so that what is being talked about can be read.
- **What changed, in one line.** If it needs two, the request is covering more
  than one moment.
- **The decisions taken outside the specification in this block**, by their
  `D-NNN`, not restated.
- **The open questions about the specification**, by their `SQ-NNN`, with
  whether they block.
- **What was not verified.** Checks satisfied by reading, reviewers that did not
  run, clauses left pending. This is the part with no other home, and the part
  a reader cannot reconstruct.

The sixth is the reason the file exists. Everything above it is discoverable
from the repository by someone willing to look; what was *not* done leaves no
trace, and only the author knows it.

A request at moment one carries the draft whole in one fenced block under
`## Draft plan`, which `plan-state -Draft` reads (`R-096`), and beside it the
answers of `/plan-explain -Draft`, quoted, with what the draft did with each.

---

# Current request — 2026-10-20 — moment one, the `docs/` vault migration plan

Replaces the moment-one request of 2026-10-20 for the derived Obsidian vault
(`728b47d`), superseded by `R-121` and kept in git.

**Moment:** one — before a plan starts. The draft below is the plan `R-121`
owes; under `R-048` it is installed as `PLAN.md` on its branch only after the
answer, and under `R-049` its first point waits for that answer.

**Branch:** written on `design/obsidian-vault`, which carries `SQ-025` and
`R-121` on top of `workflow-review`. The plan would run on `docs-vault`, cut
from `main` once `workflow-review` is merged; this branch would then be merged
with it or its commits taken over.

**What changed, in one line:** a draft plan building, outside `docs/`, the
script that splits the six documents into the vault `R-121` shapes and the
check that proves the split corresponds to the originals.

## Decisions taken outside the specification

`D-139`: the readings of `R-121` the draft makes where the ruling's text leaves
a choice, listed in the draft's "The shape, as both scripts read `R-121`". Not
audited.

## Open questions about the specification

- `SQ-026`, **blocking point 1**: whether `R-121` 4's block ids reach the brakes
  of `SIM-LOOPS`, which are lines inside a paragraph, and the paragraph
  `**B1 does not apply here.**`. The draft takes the narrow reading.
  **Answered by `R-122`:** the narrow reading is the ruling's; `SIM-LOOPS` unchanged.

## The blind reading of the draft

One read-only `Plan` agent read version 1, given the draft, `criteria.md` and,
as an exception to `/plan-explain -Draft`'s brief, the text of `R-121`: the
ruling is the plan's requirement, and a reader barred from it could not judge
the draft against it. It was told not to read the rest of `RULINGS.md`, the
register, `PROMOTIONS.md` or the archived plans. The tree hash was equal before
and after. The draft below is version 2, read by no one.

### What version 2 did with the findings

| Finding | In version 2 |
|---|---|
| No check tests the shape: notes' boundaries, names, folders; the whole document left in the index passes | Check 5 derives the expected notes from the original and compares path for path, heading for heading |
| The script is never compared with the hand-made vault | Point 2's "Closed by": the script's fixture output equals it byte for byte |
| A link at its defining occurrence passes | Check 3 forbids it; planted in both points |
| `aliases` not required; `document` and `kind` not compared | Check 6 compares them with what check 5 derived; `kind` fault planted |
| Partial embeds (`#Occupations`) and unqualified ones (`Open`) pass | Check 2: full path, no `#`; both planted |
| Check 4 shares the script's reading; no continuation-line case | Paragraphs defined as runs of non-blank lines in both; fixture and faults for a continuation line and the `B3a` lines |
| Fixture holds no identifier in a code span, fence, table or non-defining heading | Added, with a bold-led list item and a bold span that is no lead |
| Document set fixed at six against a small fixture | Documents are the `SIM-*.md` files of the directory given |
| Recursion needs a cycle guard; front matter stripping must take the leading block only | Only an index embeds; the leading block alone is stripped |
| Point 2's group-intro fault predicted to turn check 1 red, which it does not | Re-targeted to check 5 |
| The heading and code-span faults cannot show on the real documents | Run on the fixture, said so |
| "Run twice" is behaviour, not a fault | Moved to "Closed by", with the tree hash |
| No link at all passes; "may" left unchecked | "May" taken as "does"; check 3 requires every eligible mention linked; planted |
| The group test is true of any `##` with no `###` | A group needs at least one `###`, all entries, and only in `SIM-DEC` and `SIM-LOOPS` |
| Links banned in table rows without reason; §18's and `SIM-LOOPS`' tables lose links | Links allowed in table rows, `\|` escaped |
| Unnumbered aliases as bare headings, against "citation form" | `SIM-ECON §Demand`, as `SIM-REQ` writes it; entries' titles after ` — ` |
| Where the `---` separators go is undecided | They stay in the index between embeds; check 5 rejects a note ending in one |
| Document names and `§` citations never linked, unsaid | Said, with the reason |
| `.obsidian/` files written on first open would be untracked under `docs/` | `docs/.obsidian/*` ignored except `app.json` |
| `.gitignore` ends with no newline | Said |
| `tools/` is new, unreasoned; waiting on `workflow-review` unreasoned; proceeding before `SQ-026` | Each given its reason; point 1 waits on `SQ-026` |
| The exact comparison is stricter than `R-121`'s | Kept, and said: a check passing exactly passes the ruling's |

### The reading, of version 1, quoted

> Neither check tests the shape `R-121` fixes: check 1 proves the text survives
> in order, not where note boundaries fall or how notes are named and placed;
> point 2's first fault would stay green.
>
> **Point 1.** Missed: a link at its own defining occurrence; the whole
> document left in the index, a section split in two, a `DEC-` note in the
> wrong group folder, a note named `18. Delivery phases.md` or `DEC-085 — State
> arrays….md` — checks 1, 2 and 5 green; `aliases` not required, `document`
> not compared with the folder, `kind` not compared with the note type; a
> partial embed `![[SIM-REQ/6 Goods and production#Occupations]]` resolves and
> check 1 substitutes the whole note; an unqualified `![[Open]]` passes if
> resolved in the document's folder, while Obsidian resolves from the root,
> where three notes are named `Open`; check 4 shares the script's reading, and
> a script putting ` ^B3a` on the brake lines passes; the fixture has no bold
> identifier at a continuation line's start. Unchecked: "reports by name",
> "writes nothing", the defining-occurrence rule, recursion. The first five
> faults are `R-121`'s and fair; the rest convenient; missing: a link at a
> defining occurrence, a block id on a continuation line, text moved across a
> boundary, an alias missing or wrong, a partial or unqualified embed. K2 met
> for code, not for the reading; K1 not met: no identifier in a code span,
> fence or table, no non-defining identifier in a heading, no defining
> occurrence, no bold-led list item, no bold identifier mid-paragraph. Doable
> but: the document set must come from `-Originals`; recursion needs a cycle
> guard; stripping must take only a leading `---` block, the notes holding
> `---` separators. No `[[` and no line ending in ` ^…` in the six documents.
>
> **Point 2.** The group-intro fault is wrongly predicted: `## Core
> representation` moved into `DEC-001.md` stays in place in the flattened text.
> The heading fault cannot show: the only identifiers in headings are defining
> ones. A script making no links passes; one linking inside code spans or
> fences passes the real run, none holding an identifier. Overwriting passes.
> Unchecked: `app.json`, `.gitignore`, "never deletes", refusal to overwrite,
> naming, aliases, rendering. Real faults: the final newline (`SIM-DEC`,
> `SIM-REQ`, `SIM-STATE`), word boundaries (`FR-P-03a/b/c/d` against block id
> `FR-P-03`), the `B1` paragraph at `SIM-LOOPS` 52. "Run twice" is behaviour,
> not a fault. Missing: `**B3a — …**` lines given block ids, a link at the
> `**FR-…**` lead, the `---` separator moved, wrong aliases, wrong group
> folder. K2 not met: no file-by-file comparison with the hand-made vault. K1
> not met on headings, code spans, fences. K6: the brake reading rests on
> `SQ-026`, open, and other premises on text the decider may change.
> `.gitignore` ends without a newline; "run twice" cannot go through
> `mutate.ps1`, planted script faults can; `tools/` is a new top-level folder
> with no reason given.
>
> **Whole draft.** The group test is true of any `##` with no `###` — most of
> `SIM-REQ`, all of `SIM-STATE` and `SIM-ECON` would become groups; the ruling
> names `### DEC-NNN` and `### R<n>` only. Links banned in table rows and
> headings without reason, so §18's table and the `SIM-LOOPS` gate table get
> none. "May" turned into "becomes", unchecked. Unnumbered aliases drift from
> "citation form" (`SIM-ECON §Demand`), three identical `Open`. OBS aliases
> undecided, and whether entry titles keep `DEC-NNN — `. Stricter than the
> ruling on blank lines. Point 2 relies on point 1 for boundaries, defining
> occurrences, continuation lines, embed resolution, shape, names, properties —
> none delivered. Unsaid: where `---` separators go; the folder naming of
> groups; document names and `§` citations never linked; `app.json` as `{}`
> sufficient, the ignore list stopping at `workspace*.json` and `.trash/`; the
> wait on `workflow-review`; proceeding before `SQ-026`.

## Draft plan

```markdown
# Plan — the migration of `docs/` into a vault, built and checked outside it

**Kind:** process
**Exit condition:** `tools/docs-vault/check-docs-vault.ps1` and
`tools/docs-vault/split-docs.ps1` exist, each with a green `-SelfTest`; the
script's output on the fixture equals, file by file, the vault written by hand
for it; the script, run on the six documents of `docs/` into a temporary
directory, passes every check against the six originals; every fault named
below, planted alone, turns the check named red. `.gitignore` holds the vault's
unversioned files. Nothing under `docs/` is written: running the script into
`docs/` and removing the originals are the two decider rulings `R-121` assigns.
**Phase:** none, process work; before phase 4. `R-121` rules the shape; `docs/`
itself does not change in this plan.
**Branch:** `docs-vault`, cut from `main` after the answer to moment one and
after `workflow-review` is merged, which carries `R-121`, `SQ-025` and the rules
this plan runs under. Point 1 starts only once `SQ-026` is answered, since
check 4 and a fault of point 2 rest on it.

Both scripts run under `pwsh` 7, read files as UTF-8 with CRLF normalised to
LF, and write UTF-8 without BOM, LF. The documents are those `.md` files of
`-Originals` or `-Source` whose name starts `SIM-`: on `docs/`, the six of
`R-121`, `docs/CLAUDE.md` not among them (`R-121` 7). `tools/` is a new folder:
these are repository tools a decider runs, not agent configuration, so they do
not belong in `.claude/hooks/`. The check is written first, from `R-121`'s text,
against a vault written by hand; the script is then judged by the check and by
that vault, a reference it shares nothing with.

**The shape, as both scripts read `R-121`**, where its text leaves a choice:
- **The index is the document with each note's text replaced by one embed
  line**, `![[<doc>/<path>]]`, the path from the vault root, without `#`, at the
  place the text held. Obsidian renders the index as the document in its order,
  and the embed lines are the "ordered link to every note". "Drop the index's
  link list, concatenate the notes in index order" is read as: replace each
  embed line by its note's body. Only an index embeds; a note embeds nothing.
- **A `##` section is a note** (`R-121` 2), **except** in `SIM-DEC` and
  `SIM-LOOPS` a `##` section holding at least one `###` heading, all of them
  `### DEC-` or `### R<n>`: that is a group, a folder named as a section is
  named, its heading and the text before its first entry in the index, one note
  per entry named by its identifier (`R-121` 3). `SIM-OBS`'s `## OBS-n` are
  notes named `OBS-n`. The preamble stays in the index (`R-121` 1).
- **A note ends before the run of blank lines and `---` lines that precedes the
  next heading of its level or above**; that run stays in the index, between
  embeds, so that no note ends in a separator and a new `DEC-` note added to a
  group under `decide` step 6 needs none.
- **Names.** A numbered section is `<n> <title>`, `18 Delivery phases`; an
  unnumbered one `<title>`; from either, `.` after the number and the characters
  `* " \ / < > : | ? # ^ [ ]` dropped.
- **Block ids** go on a paragraph or list item that opens with `**` followed by
  an identifier and then `**` or ` —` (`**FR-T-06** —`, `**AC-02 — …**`), at the
  end of its last line, ` ^<identifier>`. A paragraph is a run of non-blank
  lines. A bold span that merely begins with an identifier
  (`**B1 does not apply here.**`) is not one; the brakes of `SIM-LOOPS`,
  `**B3a — …**`, are lines inside their loop's paragraph, not its opening, and
  Obsidian attaches a block id only at a block's end, so they get none.
  `SQ-026` asks the decider whether that reading is the ruling's.
- **Links.** Every word-bounded mention (no letter, digit or `-` either side) of
  an identifier that names a note or a block id becomes `[[<doc>/<path>|ID]]` or
  `[[<doc>/<path>#^ID|ID]]`, its displayed text the identifier as written; in a
  table row the `|` is written `\|`. Never in a code span, a fenced block or a
  heading, never at a defining occurrence (the bold lead of a block id, the
  heading of an entry note). Document names and `§` citations are not linked:
  `R-121` 6 names identifiers. The ruling's "may" is taken as "does", so that a
  script that links nothing is a fault the check can see.
- **Properties**, one key per line, every value a JSON scalar or array of
  strings, a subset of YAML that `ConvertFrom-Json` reads: `document`; `kind`,
  `index`, `section`, `decision`, `loop` or `observable`; `id` on an entry note;
  `aliases`, for a section its citation form as the documents write it,
  `"SIM-REQ §18"` numbered and `"SIM-ECON §Demand"` not, for an entry its title
  after the ` — `.
- **The comparison is exact**, line for line: blank lines stay where the
  original held them, so the exception `R-121` allows at note boundaries is not
  used, and a check that passes exactly passes the ruling's.

## 1. The correspondence check

- **Does:** adds `tools/docs-vault/check-docs-vault.ps1 -Originals <dir> -Vault
  <dir>`, which writes nothing and, for each document, reports each check by
  number and the document by name, green or red, and exits non-zero if any is
  red:
  1. **Correspondence.** The index's body, each embed line replaced by its
     note's body, the leading front-matter block alone removed, every ` ^ID` at
     a line's end removed, every `[[…|X]]` and `[[…\|X]]` turned into `X`:
     equal to the original, line for line.
  2. **Embeds.** Embeds occur in index notes only, one per line, with a full
     path and no `#`; each resolves from the vault root; every note under the
     document's folder is embedded exactly once.
  3. **Links.** Every link resolves from the vault root to a note whose `id`, or
     a block id in it, equals the displayed text; no link is preceded or
     followed by a letter, digit or `-`, or sits in a heading, a fence, a code
     span or a defining occurrence; a table row's link uses `\|`; and every
     word-bounded mention in prose of an identifier that names a note or block
     id is a link.
  4. **Block ids.** Every paragraph or list item of the original opening in the
     form above, paragraphs read as runs of non-blank lines, carries
     ` ^<its identifier>` at its end in the vault, and no other line does.
  5. **Shape.** From the original, by the rules above, the check derives the
     notes the document must have: their paths, the heading each opens with,
     `kind`, `id` and `aliases`. The vault's notes equal that list exactly, path
     for path; each opens, after its front matter, with that heading; no note
     ends in a blank or `---` line.
  6. **Properties.** Every note's front matter parses, line by line, with
     `ConvertFrom-Json`, and holds `document`, equal to its folder's document,
     and the `kind`, `id` and `aliases` check 5 derived.
  `-SelfTest` runs on `tools/docs-vault/fixtures/`: an original holding a
  preamble, a numbered and an unnumbered section, a `###` subsection inside a
  section, a group of two entries with text before the first, `---`
  separators, a table row and a fence and a code span each holding an
  identifier that names a note, a heading holding a non-defining identifier, a
  bold-led paragraph, a bold-led list item, a bold identifier at the start of a
  continuation line, a bold span that is not a lead, two sections of one name
  in two documents, and a document with no final newline; a vault for it
  written by hand, green on every check; and one copy of that vault per fault
  below, each red on the check it names.
- **Closed by:** `-SelfTest` green, each fault copy red on its named check; the
  code-reviewer reads the check against `R-121`'s shape and "correspondence
  check" and against this point's "Does".
- **Fails when:** in a copy of the hand-made vault, each alone: a paragraph
  deleted (check 1 red); a paragraph duplicated (1); a word changed (1); a
  note's embed removed from the index (1, 2); two embeds swapped (1) — these
  five are `R-121`'s; a group's heading moved from the index into its first
  entry's note (5); a section split in two notes (5); a note named
  `18. Delivery phases` (5); an entry note filed in the wrong group's folder
  (5); a link at a defining occurrence (3); `R10` written `[[…|R1]]0` (3); an
  embed to `#Occupations`, part of a note (2); an embed named `Open` without
  its path (2); a block id put on a continuation line (4); `kind` of a `DEC-`
  note set to `section` (6); a value written unquoted (6).
- **Core:** no — the diff touches only `tools/docs-vault/` and the record.

## 2. The migration script

- **Does:** adds `tools/docs-vault/split-docs.ps1 -Source <dir> -Out <dir>`,
  which reads the documents of `-Source` and writes their vault under `-Out`
  in the shape above, and `-Out/.obsidian/app.json` holding `{}`, so that
  Obsidian opens `-Out` as a vault. It refuses, writing nothing, if any file it
  would write exists, so that a run into `docs/` overwrites neither an original
  nor a note; it never deletes. `.gitignore`, which ends without a newline,
  gains one and then `docs/.obsidian/*`, `!docs/.obsidian/app.json` and
  `docs/.trash/`.
- **Closed by:** the script on the fixture of point 1 into a temporary
  directory produces exactly the hand-made vault, file for file and byte for
  byte, and the check on it is green; the script with `-Source docs -Out
  <temp>`, then `check-docs-vault.ps1 -Originals docs -Vault <temp>`, reports
  every check green for the six documents; the same run a second time exits
  non-zero and leaves `<temp>`'s tree hash unchanged; `git status` shows
  nothing under `docs/` changed; the code-reviewer reads the script against
  this point's "Does". Each fault below, planted alone in the script and run
  on the real documents into another temporary directory, turns red the check
  named; a fault that cannot show on the real documents is run on the fixture.
- **Fails when:** a group's heading written into its first entry's note
  (check 5 red on `SIM-DEC`); the `**B1 does not apply here.**` paragraph given
  a block id (check 4 red on `SIM-LOOPS`); each `**B3a — …**` line given one
  (check 4 red on `SIM-LOOPS`); mentions linked without word boundaries,
  `FR-P-03a` as `[[…|FR-P-03]]a` (check 3 red on `SIM-REQ`); the `**FR-…**`
  lead linked (check 3 red); no link made at all (check 3 red); the trailing
  `---` left in the last note of a section (check 5 red); the last line of a
  document lost, `SIM-DEC`, `SIM-REQ` and `SIM-STATE` ending with no final
  newline (check 1 red); a link written inside a code span (check 3 red, on the
  fixture: no identifier sits in a code span of the six).
- **Core:** no — the diff touches only `tools/docs-vault/`, `.gitignore` and the
  record.
```

## What was not verified

- **Version 2 of the draft** was read by no blind reader; the table says what
  changed, not whether it holds. A second reading costs one agent.
- **Obsidian.** That a full-path embed renders a note inline, that ` ^ID`
  makes a block at a paragraph's end, that `[[…\|X]]` works in a table row,
  and that `{}` in `app.json` suffices, are known from Obsidian's
  documentation, not tried.
- **The count of notes**, about two hundred, is `R-121`'s estimate, not built.
- **Whether `workflow-review` is ready to merge** was not checked.

## What I need from you

1. **The draft**, approved, amended, or rejected; in particular the sixteen
   faults of point 1 and the nine of point 2.
2. **Links taken as compulsory.** `R-121` 6 says an identifier "may" become a
   link; the draft links every eligible mention, so that a script linking
   nothing is a visible fault. Keep it, or ask for fewer links.
3. **A second blind reading** of version 2, or not.
4. ~~**`SQ-026`** goes to the decider before point 1;~~ answered by `R-122`; then `workflow-review` is
   merged by you, the plan installed on `docs-vault`, and point 1 starts.
