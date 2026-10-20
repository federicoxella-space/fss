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

**Branch:** written on `design/obsidian-vault`, which carries `SQ-025`,
`R-121`, `SQ-026` and `R-122` on top of `workflow-review`, and alone carries
them: `git branch --contains` lists no other branch for `3d97fe0` or
`7a37563`. The plan would run on `docs-vault`, cut from `main` once
`design/obsidian-vault`, and with it `workflow-review`, is merged into `main`
(version 3; versions 1 and 2 named `workflow-review` alone).

**What changed, in one line:** a draft plan building, outside `docs/`, the
script that splits the six documents into the vault `R-121` shapes and the
check that proves the split corresponds to the originals.

## Decisions taken outside the specification

`D-139`: the readings of `R-121` the draft makes where the ruling's text leaves
a choice, listed in the draft's "The shape, as both scripts read `R-121`".
`D-140`: what version 3 decides beyond it, after the second reading. Neither
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
and after. Version 2 was read once more after the user's answer: see "The
second reading, of version 2" below. The draft below is version 3, written in
answer to that reading and read by no one.

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

## The second reading, of version 2

Run on 2026-10-20 by `/plan-explain -Draft` in an implementer session, as the
user's answer to item 3 asked. One read-only `Plan` agent was given the fenced
draft alone, `criteria.md` and `AGENTS.md`, leave to read the files the draft
names, and the instruction not to read `RULINGS.md`, `DECISIONS-OUTSIDE-SPEC.md`,
`PROMOTIONS.md`, `REVIEW-REQUEST.md`, `SPEC-QUESTIONS.md`, `DECIDER-BRIEFS.md` or
`.claude/plans/`. Unlike the first reading, it was not given `R-121`'s text: it
took the draft's account of the ruling. The tree hash was equal before and after
(`7e4b6eb8…`).

### What version 3 did with the findings

The user, shown the reading, asked for version 3 (item 5 below). The author
re-checked the reading's facts about the repository before acting on them:
`core.autocrlf` is `true` and there is no `.gitattributes`; `3d97fe0` and
`7a37563` are on `design/obsidian-vault` alone and `workflow-review` is not in
`main`; `### DEC-006a`, `006b`, `007a`, `034a` exist; `FR-P-03a` is mentioned at
`SIM-OBS.md:76` and `FR-P-03b` at `SIM-REQ.md:111`; `SIM-DEC`, `SIM-REQ` and
`SIM-STATE` end without a newline; the six hold 178 notes by heading count and
260 identifier-led lines, all in `SIM-REQ`, all unique, none inside a paragraph.
Some findings rest on the reader not having `R-121`'s text, which this reading,
unlike the first, was not given; the table says so where it applies.

| Finding | In version 3 |
|---|---|
| Byte-for-byte comparison fails on a fresh checkout (`autocrlf`, no `.gitattributes`) | Point 1 adds `tools/docs-vault/fixtures/** text eol=lf`, checked by `git ls-files --eol`; CRLF output planted in point 2 |
| Where the index lives is unsaid; `docs/SIM-DEC.md` would collide | `R-121` 1 places it at `<doc>/<doc>.md`; the draft now says so, and that the check reads originals at the root and the vault in the folders |
| Check 3 does not compare a link's anchor with its text | Check 3 requires `#^ID` equal to the displayed text, and the anchor whenever the text is a block id; missing and wrong anchors planted in point 1, missing in point 2 |
| Fixture lacks two groups and `R1`/`R10` | Fixture lists two groups in `SIM-DEC`, `R1` and `R10` in `SIM-LOOPS`, and names each fixture document |
| Branch precondition names `workflow-review` | Names `design/obsidian-vault`, which carries `workflow-review` and the four entries |
| `SQ-026` gate stale | Gate removed; `R-122` cited as ruling the block ids |
| Unlinked mention has no fault in point 1 | Planted: a prose mention left plain |
| Identifier lookup per document or vault-wide unsaid | Vault-wide, said; a cross-document mention in the fixture; planted in both points (`SIM-OBS.md:76` on the real run) |
| Wrong `aliases` or `document` values pass | Check 6 compares values; both planted |
| Stray file at the vault root unchecked | Said: the check inspects the six folders only, since the in-place run leaves the originals and `CLAUDE.md` at the root; check 5 rejects any extra file inside a folder |
| Character-drop rule, `SIM-` filter, `OBS-n`, heading followed by text, group without intro unexercised | All in the fixture, which now holds a `:` heading, a `CLAUDE.md`, an `OBS-1` section, a `SIM-LOOPS` heading followed by text, a group with no intro; a `:` replaced by `-` planted |
| Fault "`Open` without path" needs sections named `Open` | Fixture names a section `Open` in two documents |
| K2: check and script may share code; on the six, check 5 is the only reference | Sharing forbidden, checked by `Select-String`; on the six, note and block-id counts taken by `Select-String` from the originals, independently of both scripts |
| K6: point 1's own continuation-line fault rests on `SQ-026` too | Moot: `R-122` has ruled |
| Aliases: the documents do not write `SIM-REQ §18` | The form is `R-121` 5's own example, now cited as such; the unnumbered form cites `SIM-REQ.md:826` |
| Refusal tested only when every file exists | The script lists every path before writing; two runs into a directory holding one planted file must leave it alone; checking path by path planted |
| "Tree hash" of a temporary directory undefined | A listing of path, size, SHA-256 and `LastWriteTimeUtc` |
| `docs/.obsidian/` instead of `docs/.obsidian/*` passes | `git check-ignore` on three paths; the wrong pattern planted |
| Extra final newline passes; its representation unsaid | Newline rule stated; check 1 compares the final LF; an added final newline planted in both points |
| `FR-P-03a` "on `SIM-REQ`" mislocated | Word-boundary fault moved to `FR-P-03b` at `SIM-REQ.md:111` |
| "Each `B3a` line": there is one | "The brake lines `**B<n> — …**`" |
| `DEC-006a` named by `DEC-\d+` | Names take the whole identifier; `DEC-002a` in the fixture; `DEC-[0-9]+` naming planted in point 2 |
| "No link made at all" is a convenient fault | Dropped in point 2: the anchor and per-document faults are the likely failures of linking |
| `app.json` not said to be in the hand-made vault | Said |
| Identifier grammar unsaid | Stated: `[A-Z]+(-[A-Z]+)*-[0-9]+[a-z]?` or `R[0-9]+`, taken whole |
| Table-only identifiers never linked, unsaid | Said, from `R-121` 4's "Table rows get none" |
| `OBS-n` aliases and the index's properties unsaid | `OBS-n` take their title after ` — `; the index holds `document` and `kind` only |
| Group rule depends on document names; the fixture's must match | Fixture documents named `SIM-DEC` and `SIM-LOOPS` |
| `.md` in embed and link paths unsaid | Without `.md`, said |
| Character-drop rule unused | Kept: `R-121` 2 requires it; the fixture exercises it |

### The reading, of version 2, quoted

> I read the whole draft against the checklist. Neither point can close as written, and the draft rests on several facts about the repository that are wrong. The main ones:
>
> - **Line endings break point 2's byte-for-byte comparison.** `core.autocrlf` is `true` and there is no `.gitattributes`, so a fresh checkout turns the committed hand-made vault into CRLF files. The script writes LF, so correct code fails the comparison.
> - **The two scripts can contradict each other on `docs/`.** The draft never says where each document's index file goes. If it is `docs/SIM-DEC.md`, the original's name, the script refuses to run into `docs/`. Its second-run test cannot see that.
> - **Check 3 does not compare a link's `#^` anchor with its displayed text.** A link that points at the right note but has a missing or wrong anchor passes.
> - **Point 1's fixture lacks what two of its own faults need.** It has one group but needs two, and it has no `R1`/`R10` pair.
> - **The branch precondition names the wrong branch.** The commits that carry `R-121` and `SQ-025` are only on `design/obsidian-vault`, not on `workflow-review`.
>
> #### Point 1 — the correspondence check
>
> **1. Faults the check would miss with "Closed by" green**
> - **Wrong or missing anchor.** Check 3 only asks that a link reach "a note whose `id`, or a block id in it, equals the displayed text". Two links pass:
>   - `[[SIM-REQ/4 Time|FR-T-06]]`, which has no `#^FR-T-06`.
>   - `[[SIM-REQ/4 Time#^FR-T-07|FR-T-06]]`, whose anchor names a different block.
>
>   No fault copy has a bad anchor, so a check that never reads anchors passes `-SelfTest`.
> - **Unlinked mentions.** The rule that every word-bounded mention must be a link has no fault copy in point 1. A check that never tests it passes `-SelfTest`.
> - **Cross-document mentions.** The draft never says whether "an identifier that names a note or a block id" is looked up across the whole vault or within one document. The fixture lists no mention of an identifier from another document. A check that builds its identifier set per document passes `-SelfTest`, yet on the real files it would accept `SIM-OBS` leaving `FR-P-03a` unlinked (SIM-OBS.md:76).
> - **Wrong property values.** No fault copy has a wrong `aliases` or `document`. A check 6 that only tests that these keys are present passes.
> - **Stray files.** A file at the vault root, outside any document's folder, is checked by nothing.
>
> **2. Parts of the "Does" nothing checks**
> - Table-row escaping (`\|`), fences and code spans: no fault copy in this point.
> - The rule that drops `* " \ / < > : | ? # ^ [ ]` from names. It never applies: no heading in the six has any of these characters, and the fixture lists no heading that has one.
> - The filter on `SIM-` names: the fixture lists no other `.md` file to exclude.
> - `SIM-OBS`'s `OBS-n` naming.
> - A heading followed directly by text, with no blank line. This happens at SIM-LOOPS.md:36–37 and on every loop through :125.
> - A group with no text before its first entry. This is how every `SIM-DEC` group looks (SIM-DEC.md:20–22).
>
> **3. Are the `Fails when:` faults the likely ones?**
> - The five the ruling requires are reasonable.
> - The rest are chosen to make each check fire on the shape it was built around. Several of them cannot be built from the fixture as listed:
>   - "Entry note filed in the wrong group's folder" needs two groups. The fixture lists "a group of two entries", which is one group.
>   - "`R10` written `[[…|R1]]0`" needs `R1` and `R10` in the fixture's original. They are not listed.
>   - "Embed named `Open` without its path" needs the two same-named sections to be called `Open`. The draft does not say they are.
> - The likely faults it leaves out are: a wrong or missing anchor, a mention left unlinked, a block id carrying the wrong identifier, and a wrong `document`.
>
> **4. Checklist rules**
> - **K1 applies, not met.** The self-test never fails if the fixture lacks a feature it is supposed to exercise. The branches listed in item 2 never come into play. The prefix pair needed for check 3's word boundary is not in the fixture.
> - **K2 applies, partly met.** The hand-made vault is an independent reference, but only for the fixture. On the six real documents, check 5 is the only reference, and it is a second implementation of the same reading by the same author. The draft does not forbid the check and the script from sharing a helper module, such as a heading or name parser. If they did, the comparison on `docs/` could not fail where they agree.
> - **K5 applies weakly, met.** The fault copies read the labels by check number, and point 2 reads the six document names.
> - **K6 applies, partly met.** Start is held until `SQ-026` is answered. But the draft says the question touches only "check 4 and a fault of point 2". This point's own fault ("a block id put on a continuation line") and the fixture's "bold identifier at the start of a continuation line" rest on the same reading. The current branch's log already has a commit whose subject reports `SQ-026` answered, so the gate may be stale.
> - **K3 and K4 do not apply.**
>
> **5. Can it be done as written?**
> - **Not as listed.** The fixture is missing what the faults above need.
> - **Aliases.** The draft says the citation form is "as the documents write it", for example `"SIM-REQ §18"`. The documents do not write that. They write:
>   - "SIM-REQ section 16", "section 20" and "section 17" (SIM-DEC.md:252, :394, :454, :464)
>   - "section 20 of SIM-REQ" (SIM-STATE.md:212)
>   - a bare "§16" (SIM-REQ.md:683), "§13" (:833) and "§Cohort" (SIM-STATE.md:226)
>
>   The only `SIM-X §` form is "SIM-ECON §Demand", once, inside a table row (SIM-REQ.md:826). The `"SIM-REQ §18"` form is the repository's convention, not the documents'.
> - **Claims that check out:**
>   - Every `###` in `SIM-DEC` is a `DEC-` entry: 109 of them, under 13 sections from :20 to :910. :10 has none.
>   - `SIM-LOOPS`'s only group is `## Loops` (:32), holding `### R1`–`R14`, with `R13` (:54) between `R3` and `R4`.
>   - `## OBS-1`–`OBS-5` are at SIM-OBS.md:20, 46, 68, 82, 96.
>   - `## Open` is in three documents: SIM-LOOPS.md:176, SIM-OBS.md:120, SIM-STATE.md:219.
>   - `### Occupations` is at SIM-REQ.md:217.
>   - No wikilink and no line-end ` ^id` exists in the originals.
>
> #### Point 2 — the migration script
>
> **1. Faults that leave "Closed by" green**
> - **Refusal only half tested.** "Run a second time, exits non-zero, tree hash unchanged" only tests the case where every target file already exists. A script that checks each path just before writing it, and aborts at the first collision, passes: on the second run the first file collides and nothing is written. On a target where only some paths exist, it writes part of the vault and then stops. `docs/` is exactly that case, because the originals are present.
> - **Wrong `.gitignore` pattern.** Writing `docs/.obsidian/` instead of `docs/.obsidian/*` stops git from re-including `app.json`, whatever the `!` line says. Nothing runs `git check-ignore`.
> - **Cross-document links.** If the script and the check both build identifiers per document, no cross-document link is made. The fixture has no such case, so the hand-vault comparison cannot see it.
> - **An extra final newline.** A script that adds one to `SIM-DEC`, `SIM-REQ` and `SIM-STATE` passes check 1 if "line for line" is implemented by splitting into lines, which drops the difference. The draft does not say how a missing final newline is represented, either in the last note or in the reconstruction.
>
> **2. Parts of the "Does" nothing checks**
> - `.gitignore`.
> - "Never deletes".
> - `-Out/.obsidian/app.json` holding `{}`. It is checked only if the hand-made vault contains it, and point 1 does not say it does.
> - The partial-collision refusal.
> - BOM and LF output, checked only through the fixture comparison. That comparison is unreliable here (see item 5).
>
> **3. Are the `Fails when:` faults the likely ones?**
> - **Likely:** the group heading, the `B1 does not apply` paragraph, the `B3a` line, linking a lead, and a trailing `---`.
> - **"No link made at all" is a convenient fault**; it is too gross to miss.
> - **"Last line lost" is the less likely of the two final-newline faults.** In pwsh, adding a final newline that should not be there is the more probable bug, and it is not named.
> - **`FR-P-03a` "on `SIM-REQ`" is mislocated.** `FR-P-03a` appears in `SIM-REQ` only as its own bold lead (SIM-REQ.md:157), which is a defining occurrence and is never linked. Its prose mention is SIM-OBS.md:76. Check 3 would still go red on `SIM-REQ`, through `(FR-P-03b)` at SIM-REQ.md:111.
> - "Each `**B3a — …**` line": there is one `B3a` line (SIM-LOOPS.md:48). The other brake lines at :49, :56–57 and :64 onward are where "each" applies.
> - **Missing likely faults:** the partial collision, a bad anchor, and `DEC-006a`/`006b`/`007a`/`034a` (SIM-DEC.md:98, 106, 122, 452) being named by `DEC-\d+` and colliding with `DEC-006`/`DEC-007`/`DEC-034`.
>
> **4. Checklist rules**
> - **K1 applies, partly met.** Planting faults on the real documents makes the branches matter. The refusal branch never matters for the partial case, and the cross-document linking branch has no reference.
> - **K2: same finding as point 1.** On the six, the script is judged only by a check that implements the same reading, and shared code is not ruled out.
> - **K6 applies.** The `B1` and `B3a` faults both rest on the `SQ-026` reading.
>
> **5. Can it be done as written?**
> - **"Byte for byte" against the committed hand-made vault fails on any fresh checkout.** `core.autocrlf` is `true`, there is no `.gitattributes`, and `git ls-files --eol` shows `i/lf w/crlf` for the `docs/` files. The fixture will be CRLF in the working tree while the script writes LF. It passes only in a tree where the fixture was written and never re-checked-out.
> - **"Tree hash" of `<temp>` is undefined.** `.claude/hooks/tree-state.ps1` hashes `git status` and `git diff` output, so it needs a repository and fails on a temporary directory. A content hash also cannot tell "wrote nothing" from "rewrote the same bytes".
> - **Verified as stated:**
>   - `.gitignore` ends with `publish/` and no newline (.gitignore:24).
>   - `SIM-DEC`, `SIM-REQ` and `SIM-STATE` end with no final newline (last byte `.`). `SIM-ECON`, `SIM-LOOPS` and `SIM-OBS` end with one.
>   - No identifier sits in a code span or a fence of the six (the fences are at SIM-DEC.md:210, :222, SIM-ECON.md:86, SIM-LOOPS.md:149, SIM-REQ.md:86).
>   - Identifiers sit in table rows in `SIM-ECON`, `SIM-LOOPS`, `SIM-REQ` and `SIM-STATE`, so `\|` will be exercised.
>   - `pwsh` is 7.6.7.
>
> #### Whole draft
>
> **What a later point relies on that an earlier one does not deliver**
> - **`app.json`.** Point 2's file-for-file comparison needs point 1's hand-made vault to contain `.obsidian/app.json`. Point 1 never says so, and its check would then have to tolerate a non-note file.
> - **Line endings.** Point 2's byte-for-byte comparison needs point 1's fixture to stay LF in git. Point 1 adds no `.gitattributes`.
> - **Branch precondition.** The draft says `workflow-review` "carries `R-121`, `SQ-025`". The commits carrying them (`3d97fe0`, `7a37563`) are only on `design/obsidian-vault`, which descends from `workflow-review`. `git branch --contains` lists no other branch, and `workflow-review` is not yet in `main`. Merging `workflow-review` would not bring them to `main`.
>
> **Decisions the draft makes without saying so**
> 1. **Where each index file lives.** If it is `docs/SIM-DEC.md`, it collides with the original. The no-overwrite rule then blocks the in-place run, and `-Originals docs -Vault docs` would read the index as an original.
> 2. **Newline handling.** Whether note files end in a newline, how a document with no final newline is rebuilt, and whether "line for line" compares the final newline.
> 3. **What an "identifier" is.** No grammar is given. `**Rationale.**`, `**Document:**` and `**Phase 4 in full.**` are excluded only by their punctuation.
> 4. **Scope of the identifier set for links.** Vault-wide or per document.
> 5. **Table identifiers are never linked.** `A-01`… (SIM-STATE.md:180 onward) and `P-nn` live only in table rows, so they get no block id and are never linked. The draft reads the ruling as linking identifiers, but this exclusion is not stated.
> 6. **`OBS-n` notes.** Whether their `aliases` take a section's citation form or an entry's title. The index's own properties are not given.
> 7. **Groups depend on document names.** The group rule applies only in `SIM-DEC` and `SIM-LOOPS`, so the fixture's group document must carry one of those names. The draft does not say so.
> 8. **Shared code.** Whether the check and the script may share a module.
> 9. **Extensions.** Whether `.md` appears in embed and link paths.
> 10. **Unused rule.** The character-drop rule is adopted although no real heading needs it and no fixture item exercises it.

## Draft plan

```markdown
# Plan — the migration of `docs/` into a vault, built and checked outside it

**Kind:** process
**Exit condition:** `tools/docs-vault/check-docs-vault.ps1` and
`tools/docs-vault/split-docs.ps1` exist, each with a green `-SelfTest`; the
script's output on the fixture equals, file for file and byte for byte, the
vault written by hand for it; the script, run on the six documents of `docs/`
into a temporary directory, passes every check against the six originals and
yields the note and block-id counts taken from the originals by a separate
count; every fault named below, planted alone, turns the check named red.
`.gitignore` holds the vault's unversioned files and `.gitattributes` keeps the
fixture LF. Nothing under `docs/` is written: running the script into `docs/`
and removing the originals are the two decider rulings `R-121` assigns.
**Phase:** none, process work; before phase 4. `R-121` rules the shape and
`R-122` its block ids; `docs/` itself does not change in this plan.
**Branch:** `docs-vault`, cut from `main` after the answer to moment one and
after `design/obsidian-vault` is merged into `main`. That branch descends from
`workflow-review`, which carries the rules this plan runs under, and alone
carries `SQ-025`, `R-121`, `SQ-026` and `R-122`; merging `workflow-review`
alone would bring none of the four.

Both scripts run under `pwsh` 7, read files as UTF-8 with CRLF normalised to
LF, and write UTF-8 without BOM, LF. The documents are the `.md` files directly
in `-Originals` or `-Source`, not in a subfolder, whose name starts `SIM-`: on
`docs/`, the six of `R-121`, `docs/CLAUDE.md` not among them (`R-121` 7).
`tools/` is a new folder: these are repository tools a decider runs, not agent
configuration, so they do not belong in `.claude/hooks/`. The check is written
first, from `R-121`'s text, against a vault written by hand; the script is then
judged by the check, by that vault, and on the real documents by counts taken
from the originals' heading and lead lines. **The two scripts share no code**:
neither dot-sources, imports or copies a function from the other or from a
common file, so that a reading wrong in one is not wrong in both.

**The shape, as both scripts read `R-121`**, where its text leaves a choice:
- **Files.** Each document's vault is the folder `<doc>/` of the vault root,
  holding its index `<doc>/<doc>.md` (`R-121` 1) and its notes. The originals
  sit at the root, so the in-place run of the ruling writes over none of them,
  and the check reads originals at the root and the vault in the folders,
  `-Originals docs -Vault docs` included. Nothing at the vault root but
  `.obsidian/app.json` is the vault's; the check inspects only the six folders.
- **The index is the document with each note's text replaced by one embed
  line**, `![[<doc>/<path>]]`, the path from the vault root without `.md` and
  without `#`, at the place the text held. Obsidian renders the index as the
  document in its order, and the embed lines are the "ordered link to every
  note". "Drop the index's link list, concatenate the notes in index order" is
  read as: replace each embed line by its note's body. Only an index embeds; a
  note embeds nothing.
- **A `##` section is a note** (`R-121` 2), **except** in `SIM-DEC` and
  `SIM-LOOPS` a `##` section holding at least one `###` heading, all of them
  `### DEC-` or `### R<n>`: that is a group, a folder named as a section is
  named, its heading and the text before its first entry, if any, in the index,
  one note per entry named by its whole identifier, `DEC-006a` included
  (`R-121` 3). `SIM-OBS`'s `## OBS-n` are notes named `OBS-n`. The preamble
  stays in the index (`R-121` 1). A `###` anywhere else stays in its section's
  note.
- **A note ends before the run of blank lines and `---` lines that precedes the
  next heading of its level or above**; that run stays in the index, between
  embeds, so that no note ends in a separator and a new `DEC-` note added to a
  group under `decide` step 6 needs none.
- **Newlines.** Every note ends in exactly one LF. The index ends in an LF if
  and only if its original does: `SIM-DEC`, `SIM-REQ` and `SIM-STATE` end
  without one today. Reconstruction splits each file on LF after dropping one
  final LF, and compares the line lists and whether both texts end in an LF.
- **Names.** A numbered section is `<n> <title>`, `18 Delivery phases`; an
  unnumbered one `<title>`; from either, `.` after the number and the characters
  `* " \ / < > : | ? # ^ [ ]` dropped, not replaced. No heading of the six holds
  one of them today; the fixture does.
- **Identifiers** are strings of the form `[A-Z]+(-[A-Z]+)*-[0-9]+[a-z]?`
  (`FR-P-03a`, `AC-02`, `DEC-006a`, `OBS-3`) or `R[0-9]+`. **A mention** is an
  occurrence with no letter, digit or `-` either side, taken whole: `FR-P-03a`
  is one mention, never `FR-P-03` followed by `a`.
- **Block ids** (`R-121` 4, as `R-122` rules it) go on a paragraph or list item
  that opens with `**` followed by an identifier and then `**` or ` —`
  (`**FR-T-06** —`, `**AC-02 — …**`), at the end of its last line,
  ` ^<identifier>`. A paragraph is a run of non-blank lines. A bold span that
  merely begins with an identifier (`**B1 does not apply here.**`) is not one;
  the brakes of `SIM-LOOPS`, `**B3a — …**`, are lines inside their loop's
  paragraph and get none. Table rows get none (`R-121` 4), so an identifier
  that lives only in a table, `A-01` or `P-12`, names nothing and is never
  linked.
- **Links.** Every mention, in any note of any document, index included, of an
  identifier that names a note or a block id anywhere in the vault becomes
  `[[<doc>/<path>|ID]]` for a note or `[[<doc>/<path>#^ID|ID]]` for a block id,
  the path without `.md`, its displayed text the identifier as written; in a
  table row the `|` is written `\|`. Never in a code span, a fenced block or a
  heading, never at a defining occurrence (the bold lead of a block id). A
  brake's identifier names nothing and is not linked (`R-122`). Document names
  and `§` citations are not linked: `R-121` 6 names identifiers. The ruling's
  "may" is taken as "does", so that a script that links nothing is a fault the
  check can see.
- **Properties**, between a first and a second `---` line, one key per line,
  every value a JSON scalar or array of strings, a subset of YAML that
  `ConvertFrom-Json` reads: `document`; `kind`, `index`, `section`, `decision`,
  `loop` or `observable`; `id` on a decision, loop or observable note;
  `aliases` on every note but the index — for a section `"<doc> §<n>"` when
  numbered, the form `R-121` 5 gives (`"SIM-REQ §18"`), `"<doc> §<title>"`
  otherwise, the form `SIM-REQ.md:826` uses (`"SIM-ECON §Demand"`); for a
  decision, loop or observable its heading's title after the ` — `. The index
  holds `document` and `kind` only.
- **The comparison is exact**, line for line: blank lines stay where the
  original held them, so the exception `R-121` allows at note boundaries is not
  used, and a check that passes exactly passes the ruling's.

## 1. The correspondence check

- **Does:** adds `tools/docs-vault/check-docs-vault.ps1 -Originals <dir> -Vault
  <dir>`, which writes nothing and, for each document, reports each check by
  number and the document by name, green or red, and exits non-zero if any is
  red:
  1. **Correspondence.** The index's body, each embed line replaced by its
     note's body, the leading front-matter block alone removed from each, every
     ` ^ID` at a line's end removed, every `[[…|X]]` and `[[…\|X]]` turned into
     `X`: equal to the original, line for line, and ending in an LF if and only
     if the original does.
  2. **Embeds.** Embeds occur in index notes only, one per line, with a full
     path and no `#`; each resolves from the vault root; every note under the
     document's folder but the index is embedded exactly once.
  3. **Links.** Every link resolves from the vault root: one without `#` to a
     note whose `id` equals the displayed text; one with `#^ID` to a note that
     holds the block id `ID`, `ID` equal to the displayed text. A displayed
     text that is a block id in the vault is linked with its anchor. No link
     is preceded or followed by a letter, digit or `-`, or sits in a heading, a
     fence, a code span or a defining occurrence; a table row's link uses `\|`;
     and every mention in prose of an identifier that names a note or block id
     anywhere in the vault is a link.
  4. **Block ids.** Every paragraph or list item of the original opening in the
     form above, paragraphs read as runs of non-blank lines, carries
     ` ^<its identifier>` at its end in the vault, and no other line does.
  5. **Shape.** From the original, by the rules above, the check derives the
     notes the document must have: their paths, the heading each opens with,
     `kind`, `id` and `aliases`. The files under the document's folder equal
     that list and the index exactly, path for path; each note opens, after its
     front matter, with that heading; every note ends in exactly one LF and in
     no blank or `---` line.
  6. **Properties.** Every note's front matter parses, line by line, with
     `ConvertFrom-Json`, and holds `document`, equal to its folder's document,
     and the `kind`, `id` and `aliases` check 5 derived, values compared.
  `-SelfTest` runs on `tools/docs-vault/fixtures/`: originals named
  `SIM-REQ.md`, `SIM-DEC.md`, `SIM-LOOPS.md` and `SIM-OBS.md`, with a
  `CLAUDE.md` beside them that is no document, holding between them: a
  preamble ending in `---`; numbered and unnumbered sections; a heading holding
  a character dropped from names (`:`); a `###` subsection inside a section; in
  `SIM-DEC` two groups, the first with no text before its first entry, the
  second with some, and entries `DEC-002` and `DEC-002a`; in `SIM-LOOPS` a
  group with entries `R1` and `R10`, a heading followed by text with no blank
  line, a brake line `**B1 — …**` inside a loop's paragraph and a bold span
  `**B1 does not apply here.**`; in `SIM-OBS` an `## OBS-1 — …` section and a
  mention of a block id of `SIM-REQ`; a section named `Open` in two documents;
  `---` separators; a table row, a fence and a code span each holding an
  identifier that names a note or block id; a heading holding a non-defining
  identifier; a bold-led paragraph, a bold-led list item, a bold identifier at
  the start of a continuation line; prose mentions of `R10`, `DEC-002a` and
  `FR-…` identifiers that have block ids; one document with no final newline.
  A vault for it written by hand, with `.obsidian/app.json` holding `{}`, green
  on every check; and, made by `-SelfTest` in a temporary directory from that
  vault, one copy per fault below, each red on the check it names and green on
  the others. `.gitattributes` gains `tools/docs-vault/fixtures/** text eol=lf`.
- **Closed by:** `-SelfTest` green, each fault copy red on its named check;
  `git ls-files --eol tools/docs-vault/fixtures` reports `attr/text eol=lf` for
  every file; neither script under `tools/docs-vault/` holds a dot-source, an
  `Import-Module` or a call to another file there, by `Select-String`; the
  code-reviewer reads the check against `R-121`'s shape and "correspondence
  check", `R-122`, and this point's "Does".
- **Fails when:** in a copy of the hand-made vault, each alone: a paragraph
  deleted (check 1 red); a paragraph duplicated (1); a word changed (1); a
  note's embed removed from the index (1, 2); two embeds swapped (1) — these
  five are `R-121`'s; the index of the document with no final newline given
  one (1); a group's heading moved from the index into its first entry's note
  (5); a section split in two notes (5); a numbered note named with its `.`
  kept (5); the `:` of a heading replaced by `-` in its note's name (5); an
  entry note filed in the other group's folder (5); `DEC-002a`'s note named by
  its whole heading (5); a note ending in `---` (5); a note without its final
  LF (5); a link at a defining occurrence (3); `R10` written `[[…|R1]]0` (3);
  `DEC-002a` written `[[…|DEC-002]]a` (3); a block-id link without its
  `#^ID` (3); a block-id link whose anchor names another block (3); one prose
  mention left plain (3); the `SIM-OBS` mention of a `SIM-REQ` block id left
  plain (3); a link inside a code span (3); inside a fence (3); inside a
  heading (3); a table row's link with a bare `|` (3); an embed to
  `#Occupations`-style part of a note (2); an embed `![[Open]]` without its
  path (2); an embed in a section note (2); a block id put on a continuation
  line (4); on the brake line (4); on `**B1 does not apply here.**` (4); a
  block id carrying another identifier (4); the list item's block id removed
  (4); `kind` of a `DEC-` note set to `section` (6); a value written unquoted
  (6); `document` naming another document (6); a numbered section's alias
  written with the `.` kept, `"SIM-REQ §<n>."` (6).
- **Core:** no — the diff touches only `tools/docs-vault/`, `.gitattributes`
  and the record.

## 2. The migration script

- **Does:** adds `tools/docs-vault/split-docs.ps1 -Source <dir> -Out <dir>`,
  which reads the documents of `-Source` and writes their vault under `-Out`
  in the shape above, and `-Out/.obsidian/app.json` holding `{}`, so that
  Obsidian opens `-Out` as a vault. Before writing anything it lists every file
  it would write, and refuses, writing nothing, if any of them exists, so that
  a run into `docs/` overwrites neither an original nor a note; it never
  deletes. `-SelfTest` runs it on the fixture of point 1 into a temporary
  directory and compares the result with the hand-made vault, byte for byte.
  `.gitignore`, which ends without a newline, gains one and then
  `docs/.obsidian/*`, `!docs/.obsidian/app.json` and `docs/.trash/`.
- **Closed by:**
  - `-SelfTest` green: the script's fixture output equals the hand-made vault
    file for file and byte for byte, and the check on it is green;
  - the script with `-Source docs -Out <temp>`, then `check-docs-vault.ps1
    -Originals docs -Vault <temp>`, reports every check green for the six
    documents;
  - in `<temp>`, the count of notes per document and of block ids equals the
    count taken from the originals by `Select-String` alone, independently of
    both scripts: `##` heading lines, less the groups, plus `### DEC-` and
    `### R` lines; and lines opening `**<identifier>` then `**` or ` —`. On the
    documents as of `5febba3` these are `SIM-REQ` 20, `SIM-ECON` 7,
    `SIM-STATE` 14, `SIM-OBS` 8, `SIM-LOOPS` 19 (5 sections, 14 loops),
    `SIM-DEC` 110 (1 section, 109 decisions), and 260 block ids, all in
    `SIM-REQ`; if the documents change before the run, the counts taken at
    the run stand;
  - the same run a second time exits non-zero, and a listing of every file
    under `<temp>` with its size, SHA-256 and `LastWriteTimeUtc` is equal
    before and after; two more runs, each into a new directory holding one file
    the script would write — `.obsidian/app.json`, then
    `SIM-REQ/18 Delivery phases.md` — exit non-zero and leave the directory
    holding that file alone, unchanged;
  - `git check-ignore` reports `docs/.obsidian/workspace.json` and
    `docs/.trash/x.md` ignored and `docs/.obsidian/app.json` not;
  - `git status` shows nothing under `docs/` changed;
  - the code-reviewer reads the script against this point's "Does".

  Each fault below, planted alone in the script and run on the real documents
  into another temporary directory, turns red the check named; a fault that
  cannot show on the real documents is run on the fixture.
- **Fails when:** a group's heading written into its first entry's note
  (check 5 red on `SIM-DEC`); entries named by `DEC-[0-9]+`, `DEC-006a` filed
  as `DEC-006` (5 on `SIM-DEC`); the `**B1 does not apply here.**` paragraph
  given a block id (4 on `SIM-LOOPS`); the brake lines `**B<n> — …**` given one
  (4 on `SIM-LOOPS`); mentions linked without word boundaries, `FR-P-03b` at
  `SIM-REQ.md:111` as `[[…|FR-P-03]]b` (3 on `SIM-REQ`); the `**FR-…**` lead
  linked (3 on `SIM-REQ`); block-id links written without `#^ID` (3 on
  `SIM-REQ`); identifiers looked up within their own document only, so that
  `FR-P-03a` at `SIM-OBS.md:76` stays plain (3 on `SIM-OBS`); the trailing
  `---` left in the last note of a section (5); a final newline added to the
  indexes of `SIM-DEC`, `SIM-REQ` and `SIM-STATE` (1 on each); the last line of
  a document lost (1); a link written inside a code span (3, on the fixture:
  no identifier sits in a code span of the six); files written CRLF
  (`-SelfTest` red); each path checked just before it is written rather than
  all before the first (the collision runs red); `.gitignore` given
  `docs/.obsidian/` instead of `docs/.obsidian/*` (`git check-ignore` red on
  `app.json`).
- **Core:** no — the diff touches only `tools/docs-vault/`, `.gitignore` and the
  record.
```

## What was not verified

- **Version 3 of the draft** was read by no blind reader; the table says what
  changed, not whether it holds. Version 2's reader was not given `R-121`'s
  text, so its findings judged the draft against the draft's own account of
  the ruling.
- **The counts in point 2** — 178 notes, 260 block ids — were taken by the
  author with `grep` and `awk` on the documents as of `5febba3`; the criterion
  makes the counts taken at the run the reference, not these numbers.
- **The fault lists** of version 3 — thirty-seven in point 1, fifteen in
  point 2 — were not tried against any code: whether each can be planted alone
  in a vault or a script as described is the author's reading.
- **Obsidian.** That a full-path embed renders a note inline, that ` ^ID`
  makes a block at a paragraph's end, that `[[…\|X]]` works in a table row,
  and that `{}` in `app.json` suffices, are known from Obsidian's
  documentation, not tried.
- **Whether `design/obsidian-vault` and `workflow-review` are ready to merge**
  was not checked.

## What I need from you

1. **The draft**, approved, amended, or rejected; in particular the sixteen
   faults of point 1 and the nine of point 2.
2. **Links taken as compulsory.** `R-121` 6 says an identifier "may" become a
   link; the draft links every eligible mention, so that a script linking
   nothing is a visible fault. Keep it, or ask for fewer links.
3. **A second blind reading** of version 2, or not.
4. ~~**`SQ-026`** goes to the decider before point 1;~~ answered by `R-122`; then `workflow-review` is
   merged by you, the plan installed on `docs-vault`, and point 1 starts.

**The user's answer, 2026-10-20**, given in a decider session and recorded there
verbatim: to items 1 and 2, "Approvo la bozza" — the draft approved as written,
its compulsory links included; to item 3, asked whether a second blind reading
of version 2 comes before the plan is installed, "Sì, prima la lettura". So
version 2 is read through `/plan-explain -Draft` by an implementer session
before `PLAN.md` is installed, and the user sees what it finds.

5. **The second reading.** Version 2 installed as approved, or amended in the
   light of the reading quoted above; and whether the branch precondition,
   which names `workflow-review` where `R-121` sits on `design/obsidian-vault`,
   stands.

**The user's answer, 2026-10-20**, in an implementer session, to item 5, after
the reading was explained and version 3 recommended: "Scrivi la versione 3
nella request". Version 3 is the draft above; the branch precondition names
`design/obsidian-vault`.

6. **Version 3**, approved, amended, or rejected; in particular the faults of
   both points and the counts of point 2.
7. **A third blind reading** of version 3 before it is installed, or not.
