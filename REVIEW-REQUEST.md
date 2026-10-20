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

# Current request — 2026-10-20 — moment one, the Obsidian vault plan

Replaces the moment-one request of 2026-10-20 for the workflow review plan,
whose items are all answered and recorded there; it is kept in git.

**Moment:** one — before a plan starts. The draft below is the plan; under
`R-048` it is installed as `PLAN.md` on its branch only after the answer, in a
commit that cites it, and under `R-049` its first point waits for that answer.

**Branch:** the request is written on `design/obsidian-vault`, cut from
`workflow-review` at `3985300`, so that it reads the register and the rules as
they now stand. The plan would run on its own branch, `obsidian-vault`, cut
from `main` once `workflow-review` is merged; merging it is not this request's
business.

**What changed, in one line:** a draft plan for an Obsidian vault in `vault/`,
generated from `docs/`, the registers and the process files, read against them
by one agent and grouped into topics by another.

The user asked, 2026-10-20: *"Crea un vault obsidian e spostaci tutta la
conoscenza del progetto. Quando hai terminato lancia un agente che si occupi di
controllare che il contenuto rispetti l'attuale documentazione presente e poi
uno che si occupi di collegare tra di loro gli argomenti. Sfrutta le proprietà
di Obsidian per ottimizzare l'accessibilità della documentazione"*. Asked how,
the user chose: a derived vault, the sources staying where they are and staying
the authority, since moving `docs/` is not the implementer's to do and fourteen
hooks, skills and agents read those paths; inside the repository; one note per
entry.

## Decisions taken outside the specification

`D-138`: the choices the draft makes that the user's answers did not. Not
audited; the watermark is the decider's.

## Open questions about the specification

None. The plan is process work; `docs/` describes the simulated world and
nothing in it changes.

## The blind reading of the draft

Two readings, each by a read-only `Plan` agent given the draft alone,
`criteria.md` and leave to read the files the draft names, told not to read the
register or the archived plans; the tree hash was equal before and after each.
The first read version 1, the second version 2; the draft below is version 3,
which no reader has seen.

### What the drafts did with the findings

| Reading | Finding | In version 3 |
|---|---|---|
| 1 | Cuts one line late, names shifted by one: reconstruction stays green | Check 3, the head of each entry note is its own opener line; planted |
| 1 | `R10` as `[[R1]]0`, no linking at all, links in fences: invertible, so green | Word boundaries in the rule; check 4 counts every mention and finds no link in a fence; planted |
| 1 | Pipe in a table row breaks the column | `[[ID\|ID]]` in table rows; fixture |
| 1 | Property values unchecked; field lines are free text, wrapped, repeated | Free-text fields (verdict, status, blocks) are no longer properties; the ones kept are recomputed by check 5 from the source |
| 1 | Prose paragraphs opening with a bold identifier open bogus slices | Bold openers only in `SIM-REQ` and `SIM-LOOPS`, and only before a dash; fixture `**B1 does not apply here.**` |
| 1 | Committed blobs are LF, the tree CRLF | Sources read with CRLF normalised; vault pinned LF by `.gitattributes` |
| 1 | Illegal characters in section names | Stripped, listed; fixture |
| 1 | Families with no type: `K-`, `Z-`, `PD-`, `P-`, `A-` | Not entry notes; their headings make section notes, and they are not linked (`D-138` 4) |
| 1 | Plants for checks 2 and 3 also turn check 1 red | Checks reported by number; each plant names the check it turns red |
| 1 | `mutate` hashes `vault/`; a planted generator fault rewrites it | Faults built with `-Out` into a temporary directory |
| 1 | Point 2's sample unseeded and too small to meet the plants | Seeded `-Sample`; the plants go into notes of the sample |
| 1 | Point 2 writes the register though its `Core:` says otherwise; several commits | The record named in the header; one commit |
| 1 | Topic titles, groupings and prose unchecked | Topic notes are front matter only; check 7 requires an empty body |
| 1 | `Home.md` rewritten by the generator loses topic links | The generator links every note it finds in `Topics/` |
| 1 | Check 4 contradicts index and topic notes | Index and topic notes exempted by type |
| 1 | Rebuild deletes the user's `.obsidian/` | `.obsidian/` spared; only `types.json` written |
| 1 | "Opens in Obsidian" checked by nothing | The exit condition closes with reserve on the user opening it (`R-011`) |
| 1 | Committed vault goes stale; nothing regenerates it | Said in the header (`D-138` 1) |
| 2 | Duplicate stems: `CLAUDE.md` twice, `SKILL.md` four times, one design and plan name | Section notes named by repository path; fixture |
| 2 | Cutting at `##` inside fences | Cutting outside fences; fixture |
| 2 | `**B1 does not apply here.**` opens a second `B1` | An opener needs the dash; check 2 rejects duplicate `id`s |
| 2 | Checks 3 and 4 contradict each other on the opener's own identifier | A note never links its own identifier; check 3 compares after un-linking |
| 2 | Check 5 contradicts the linking rule and covers non-field properties | Check 5 recomputes named properties from the source |
| 2 | Front matter unparsed; pwsh has no YAML parser | Every value a JSON scalar or array, read by `ConvertFrom-Json`; fixture with `: ` and a backtick |
| 2 | Only first mention linked, identifiers inside longer code spans | Check 4 counts mentions; longer code spans excluded |
| 2 | `type`, `line`, `cites` unchecked | Check 2 checks `type` by family, check 5 `line`, check 4 `cites` |
| 2 | `-Sample` built by no point | In point 1 |
| 2 | Point 2's brief named the planted fields | New plants (a paragraph moved, an alias reworded), seen by no check, and the brief names neither |
| 2 | Code-reviewer only if the generator changed | Code-reviewer on the diff of `tools/vault/` in every point |
| 2 | Topic agent could write outside `Topics/` | `git status` after it, in "Closed by" |
| 2 | Unquoted `members` invisible to Obsidian | JSON array; planted |
| 2 | "By member count" needs a formula | Dropped |
| 2 | `-Out .` would wipe the root | Refused unless it holds `Home.md` or does not exist |
| 2 | Five sources lack a final newline | Written as read; fixture; planted |
| 2 | Archiving `PLAN.md` adds a source | Said: the archive leaves the vault stale |
| 2 | Both scripts from one table miss the same unknown shapes | Said in the "Does"; point 2 is the reader for it |
| 2 | Bases need a recent Obsidian | Not resolved: `D-138` 6, and the reserve |

### The first reading, of version 1, quoted

> **Point 1.** Faults missed: check 1 does not care where a slice is cut — every
> cut one line late, or identifiers assigned one slice off, keeps bytes and
> count; linking without word boundaries (`R10` → `[[R1]]0`, `DEC-006a` →
> `[[DEC-006]]a`), no linking at all, links inside fences, `[[X|X]]` in a table
> cell breaking the column — all invertible, so checks 1 and 3 stay green; any
> wrong property value is invisible, check 1 strips front matter and check 4
> tests presence only; malformed `types.json` or `.base` read by no check;
> prose paragraphs opening with a bold identifier (`RULINGS.md` 2647, 2782,
> 4724, 5492, 5574; `DECISIONS-OUTSIDE-SPEC.md` 657, 1864, 1873, 2273, 3723,
> 3920) open bogus slices counted alike by generator and check; `NFR-` missing
> from both. Unchecked: sources never written, `Topics/` and `workspace*.json`
> spared, most properties, `cites`, slice boundaries and naming, the link
> conversion, index notes, Bases, `types.json`, `.gitignore`, "opens in
> Obsidian". The `Fails when:` is convenient: under the draft's own rule there
> is no text between the last entry and the next `##` in §3; the plants for
> checks 2 and 3 also turn check 1 red, so those checks could be no-ops; a
> planted generator fault rewrites `vault/`, which `mutate` hashes. K2 not met:
> the inverse is the generator's transform reversed, and the count pattern
> comes from the same sentence. K1 not met: no state named for prefix pairs,
> fences, table cells, `###` subsections without identifier, `**B…**` lines,
> identifiers opening prose. K5 applies to index notes and Bases. Not doable as
> written: committed blobs LF, tree CRLF, `DECIDER-BRIEFS.md` mixed; section
> names with `:`, `<>`, `/`, backticks; `**Date:** … **Origin:** …` on one
> line, `**Verdict:**` bare in 60 of 120, nested bold in `**Status:**`, 33
> Status lines for 24 questions, `**Raised:**` not a date, the promotion line in
> a code span, wrapped field lines; design documents hold `K-`, `Z-`, `PD-`
> headings and `SIM-LOOPS` `B*` loops with no type; `SIM-STATE` and `SIM-ECON`
> have no section notes for `defines`; check 4 contradicts the index notes; the
> Obsidian list type is `multitext`.
>
> **Point 2.** A sample of ten per type misses most entries; unseeded; findings
> "recorded" close the point; the brief is not quoted; nothing maps findings to
> commits; `check-vault` green says nothing of properties or slicing. The plants
> fall in the sample about one time in twelve; "another valid value" assumes a
> verdict list that does not exist; the planted link must be in front matter.
> K1 not met; K3 weakly. Conflicts: writes the register though `Core:` says
> otherwise; several commits; code-reviewer omitted; the register is a source,
> so each entry stales the vault.
>
> **Point 3.** Wrong titles, wrong topics, wrong headings, prose of its own all
> green; `Home.md` rewritten by the next build drops the topic links. K5 not
> met; K3: the list of notes in no topic must come from the check. Topic notes
> lack `source` and `order` and turn check 4 red; `Home.md` is generator-owned;
> the agent is not named and `Explore` is read-only; no code-reviewer, no
> fixtures; "check 3 now covering `Topics/`" changes nothing.
>
> **Whole draft.** Point 3 needs `Home.md` to survive a rebuild and check 4 to
> accept notes without `source`; point 2 needs a verdict list point 1 never
> extracts; "opens in Obsidian" is delivered by no check. The header's "no file
> outside" contradicts the record every point writes, and the register being a
> source makes the ordering matter. Unstated: a committed vault going stale;
> sources left out (`criteria.md`, `reviewers.md`, root `CLAUDE.md`); families
> with no note (`P-`, `A-`, `B*`); code spans losing monospace; each rebuild
> deleting `.obsidian/`; Bases needing a recent Obsidian; encoding and line
> endings; `pwsh` 7 against 5.1; the topic list partly fixed by the draft.

### The second reading, of version 2, quoted

> Not doable as written: point 1's check is red on the current tree whatever
> the generator does — duplicate stems overwrite notes, checks 3 and 4
> contradict each other, check 5 contradicts the linking rule, one source holds
> a duplicate opener; point 2 depends on a `-Sample` no point builds, and its
> brief names the two planted fields.
>
> **Point 1.** Missed: invalid YAML (46 of 120 verdicts hold identifiers, one
> starts with a backtick, 7 hold `: `; bold leads start with `*`), no YAML
> parser in pwsh 7.6.7, no check parses front matter; `[[ID|ID]]` unescaped in
> a table row passes checks 1 and 4; only the first mention linked passes a set
> comparison; an identifier inside a longer code span linked; `type`, `line`,
> `aliases`, `defines`, `cites` against the body unchecked; sources never
> written, `Topics/` and `.obsidian/` surviving, folders, `Home.md`, Bases
> sorting, the types inside `types.json` unchecked; `promoted_as`, `closed`,
> `task` not comparable by check 5. Plants are shaped to one check each; the
> likely faults are YAML quoting, name collisions, cuts inside fences, field
> lines repeated or wrapped, a final newline added to `SIM-DEC`, `SIM-REQ`,
> `SIM-STATE`, `AGENTS.md`, `CLAUDE.md`. K2 partly met: one author, one opener
> table — `### Not a candidate — …`, `### Later in the first pass…` absorbed into
> the preceding `C-`; un-linking is linking reversed. K1 partly met: no state
> for a field value holding an identifier, a wrapped field line, a repeated
> mention, a `##` in a fence. Not doable: stems collide (`CLAUDE` twice, `SKILL`
> four times, `2026-09-17-plan-e-skill` in design and plans); cutting ignores
> fences (`DECIDER-BRIEFS.md:11`, `.claude/design/2026-09-17-plan-e-skill.md:104,111`);
> `docs/SIM-LOOPS.md:52` `**B1 does not apply here.**` opens a second `B1`; 16
> of 120 Verdict and 21 of 24 Blocks lines wrap; five register entries carry
> two `**Ruling:**` lines, one none, one cites a ruling in passing; SQ-024 open
> with Status lines saying "closed"; check 3 against check 4 on the opener's
> own identifier; check 5's "verbatim" against linked bodies and against
> properties on no field line; check 6's "common set" undefined. Line endings
> fine once normalised.
>
> **Point 2.** A fix may regress what no check reads; "recorded" is never
> judged; `-Sample` not in point 1; code-reviewer only if the generator
> changed. `promoted_as` in 4 of 137 entries, rarely sampled. The brief says
> "`closed`, `task` and `promoted_as` above all" and the plants are `closed` and
> `task`: "without the agent told" is empty; `closed` ill-defined for SQ-024.
> K6 now: "neither is caught by check 5" contradicts check 5's text. K2: one
> author fixes generator and check in one commit.
>
> **Point 3.** Degenerate topics pass; unquoted `[[X]]` in `members` is a
> nested list to YAML, no links in Obsidian; writes outside `Topics/` limited by
> the brief only, `path-guard` allowing them; `Topics.base` by member count
> needs a formula, outside check 6; check 7 parses YAML by hand. K3 met.
>
> **Whole draft.** Point 2 needs `-Sample` and a narrowed check 5; point 3 needs
> `Topics/` kept, `Home.md` linking it, `members` multitext, none checked;
> archiving `PLAN.md` adds a source and turns check 1 red; CI runs nothing
> under `tools/`. Unstated: YAML quoting and parsing; duplicate stems and index
> note names; cutting and fences; self-links; several field lines or none,
> wrapped ones; `closed` false or omitted; `NFR-` typed `requirement`; the
> common set; what `.gitignore` gets; a guard on `-Out`; whether the
> code-reviewer reads hundreds of generated files; the Obsidian version.

## Draft plan

```markdown
# Plan — an Obsidian vault derived from the project's written knowledge

**Kind:** process
**Exit condition:** `vault/` at the repository root holds one note per entry and
per section of every source listed in point 1, with properties; at the last
point's commit `check-vault.ps1` reports every check green; a read-only agent has
read a sample of the vault against the sources and each finding is fixed or
recorded; a second agent has grouped the entries into topic notes; and the user
has opened `vault/` in Obsidian and seen the properties, the links and the
Bases render, which closes the plan *with reserve* until done (`R-011`).
**Phase:** none, process work; before phase 4. `docs/` does not cover the
vault: nothing in it asks for one, and nothing in it changes.
**Branch:** `obsidian-vault`, cut from `main` after the answer to moment one and
after `workflow-review` is merged.

The vault is derived and committed. `docs/` and the registers stay where they
are and stay the only authority; a note that disagrees with its source is a
defect in the note. The plan writes `tools/vault/`, `vault/`, `.gitignore`,
`.gitattributes`, and the record every point owes (the register and `PLAN.md`'s
outcome lines); nothing else. The register is itself a source: each point's
commit regenerates the vault after its register entry is written, and the
check runs on that state. Nothing regenerates the vault after the plan, and
nothing runs the check in CI: a later commit to a source, the plan's own
archive into `.claude/plans/` first among them, leaves the vault stale until
someone runs `build-vault.ps1`, and check 1 says so when run.

Both scripts run under `pwsh` 7, read sources as UTF-8 with CRLF normalised to
LF, and write UTF-8 without BOM, LF, adding no final newline a source lacks.
`.gitattributes` gains `vault/** text eol=lf`; `.gitignore` gains
`vault/.obsidian/*` and `!vault/.obsidian/types.json`. Every front-matter value
is written as a JSON scalar or array of strings (`id: "R-001"`,
`cites: ["[[DEC-002]]"]`), a subset of YAML that the check reads with
`ConvertFrom-Json`, one key per line.

## 1. The generator and its check

- **Does:** adds `tools/vault/build-vault.ps1 [-Out <dir>]`, default `vault/`.
  It refuses an `-Out` that exists and holds no `Home.md`. It reads, and never
  writes: `docs/*.md`, `RULINGS.md`, `DECISIONS-OUTSIDE-SPEC.md`,
  `SPEC-QUESTIONS.md`, `PROMOTIONS.md`, `DECIDER-BRIEFS.md`, `AGENTS.md`,
  `README.md`, `CLAUDE.md`, `.claude/design/*.md`, `.claude/plans/*.md`,
  `.claude/skills/**/*.md`, `.claude/agents/*.md`. `PLAN.md` and
  `REVIEW-REQUEST.md` are not sources: they hold the current development only.
  It deletes and rewrites everything under the output directory except `Topics/`
  and `.obsidian/`; under `.obsidian/` it writes `types.json` only.
  - **Cutting.** Outside fenced blocks, every source is cut at each `##` and
    `###` heading, and the text before the first is a slice of its own; in
    `SIM-REQ.md` also at a line opening `**FR-`, `**NFR-` or `**AC-` followed by
    an identifier and ` —` or `** —`, and in `SIM-LOOPS.md` at a line opening
    `**B<n>[a-z] —` the same way. Nothing else opens a slice: not a `####`
    heading, a table row, a bold identifier followed by anything but a dash, a
    heading inside a fence. A slice whose heading or lead opens with an
    identifier of a family below is an entry note named by it; any other is a
    section note.

    | Family | `type` | Source |
    |---|---|---|
    | `FR-`, `NFR-` | `requirement` | `SIM-REQ.md` |
    | `AC-` | `acceptance` | `SIM-REQ.md` |
    | `DEC-` | `decision` | `SIM-DEC.md` |
    | `OBS-` | `observable` | `SIM-OBS.md` |
    | `R<n>`, `B<n>` | `loop` | `SIM-LOOPS.md` |
    | `R-` | `ruling` | `RULINGS.md` |
    | `D-` | `register-entry` | `DECISIONS-OUTSIDE-SPEC.md` |
    | `SQ-` | `spec-question` | `SPEC-QUESTIONS.md` |
    | `C-` (after `~~`, if struck) | `promotion` | `PROMOTIONS.md` |

    A section note is named `<path> § <n> <heading>`: `<path>` the source's
    repository path without `.md`, `/` turned into `.` and a leading `.`
    dropped (`claude.skills.plan-next.SKILL`), `<n>` its order, the heading
    stripped of `` ` ``, `~`, `*` and of `\ / : * ? " < > | # ^ [ ]`; the
    preamble is `<path> § 0`. Entry notes sit in a folder per type, section
    notes under `vault/Sections/<path>/`.
  - **Properties.** On every note: `type` (`section` for a section note),
    `source` (repository path), `line` (its first line in the source), `order`,
    `aliases` (its first heading or bold lead as written), `cites` (a link to
    each identifier its body links). On an entry note: `id`. On a ruling: `date`,
    the first ISO date on its `**Date:**` line. On a register entry: `task`, the
    `##` heading above it; `rulings`, a link to every `R-` on any of its
    `**Ruling:**` lines, which can include a ruling cited in passing;
    `promoted_as`, the `DEC-` on its `**Promoted … as …**` line. On a spec
    question: `raised`, the first ISO date on its `**Raised:**` line. A value
    whose source line is absent is omitted. Free-text fields — a ruling's
    verdict, a question's status or what it blocks — are not properties: they
    wrap, repeat and change within an entry, and stay in the body.
  - **Links.** In a body, outside fenced blocks and outside code spans longer
    than one identifier, every mention of an identifier that has a note, other
    than the note's own, standing between word boundaries (no letter, digit or
    `-` either side), becomes a link: bare, `[[ID]]`; as a code span `` `ID` ``,
    `[[ID|ID]]`; inside a table row, `[[ID\|ID]]`. Nothing else changes.
  - **Index notes**, `type: index`: `vault/Home.md`, linking each source's index
    note, each Base and each note found in `Topics/`; for each source,
    `<path> · index`, listing its notes in `order`. `vault/Bases/`:
    `Rulings.base` (by `date`), `Register.base` (by `task`, `rulings`),
    `Questions.base` (by `raised`), `Entries.base` (by `type`, `source`).
    `.obsidian/types.json`: `date`, `raised` as `date`; `cites`, `rulings`,
    `aliases`, `members` as `multitext`; `order`, `line` as `number`.
  - `-Sample <seed>` on the check, below, lists ten notes per type by a draw from
    the seed, all of them when fewer.
  Adds `tools/vault/check-vault.ps1 [-Vault <dir>] [-Sample <seed>]`, sharing no
  code and no pattern with the generator: each pattern below is written in the
  check, from this text. Both are written from one table, so a source shape the
  table does not know is missed by both; point 2 is the reader for that. It
  reports each check by number, green or red, and exits non-zero if any is red.
  1. **Reconstruction.** For each source, its non-index notes sorted by `order`,
     front matter removed, `[[X\|X]]` and `[[X|X]]` turned back into `` `X` ``
     and `[[X]]` into `X`, concatenated: equal to the source read as above.
  2. **Census.** The identifiers opening a slice in each source, found by the
     check's own patterns outside fences, equal as sets to the `id`s of that
     source's entry notes; no two notes share an `id` or a file name; every
     note's `type` is the one its family's row gives.
  3. **Heads.** An entry note's body, links turned back as in check 1, opens with
     the line that opens its `id`'s slice in the source, and holds no second
     such line.
  4. **Links.** Every link, in bodies and in properties, resolves to a note; none
     sits inside a fence; in every body outside fences, every word-bounded
     mention of an identifier that has a note, other than the note's own, is a
     link, counted mention by mention; and `cites` is exactly the set of the
     body's link targets.
  5. **Properties.** Every front-matter line parses with `ConvertFrom-Json`;
     every note has `type`, and every non-index note `source`, `line`, `order`;
     `line` is the source line where check 3 finds its slice opening; `date`,
     `raised`, `task`, `rulings` and `promoted_as` equal what the check reads
     from the source by the rules above.
  6. **Config.** `types.json` parses; every property a `.base` file names is one
     the generator writes, listed in the check.
  Both scripts take `-SelfTest` on fixtures under `tools/vault/fixtures/`: each
  opener; prefix pairs (`R1`/`R10`, `DEC-006`/`DEC-006a`); an identifier in a
  fence, and a `##` in a fence; a code span in a table row; a bold identifier
  followed by no dash (`**B1 does not apply here.**`); an identifier mentioned
  twice; two `**Ruling:**` lines; a heading with `:` and `<>`; two sources with
  the same file name in different folders; a source with no final newline; a
  field value holding `: ` and a backtick.
- **Closed by:** both `-SelfTest`s green; `build-vault.ps1` on the tree, then
  `check-vault.ps1` reports checks 1 to 6 green; the code-reviewer reads
  `tools/vault/` against this "Does", and a sample of `vault/` of its choosing.
  Each fault below, planted alone in the generator and built with `-Out` into a
  temporary directory, turns red the check named, reported by number.
- **Fails when:** every cut falls one line late (check 3 red); `R10` linked as
  `[[R1]]0` (check 4 red); only the first mention of an identifier linked
  (check 4 red); a `##` inside a fence cut (check 1 or 3 red, the fence split);
  two sources named `SKILL.md` written to one note name (check 2 red); a
  value written unquoted (check 5 red); a final newline added (check 1 red).
- **Core:** no — the diff touches only `tools/vault/`, `vault/`, `.gitignore`,
  `.gitattributes` and the record.

## 2. The vault read against the sources

- **Does:** `check-vault.ps1 -Sample 2026` gives the list. Before dispatch, in
  two notes of that list, two faults no check of point 1 sees: a paragraph of
  one entry note's body moved to the head of the next entry's body, and one
  section note's `aliases` reworded. Then one read-only `Explore` agent, whose
  brief is quoted in the outcome and says nothing of the generator, the checks
  or the plants, reads each listed note against its source: is the text the
  whole entry and nothing of another; does each property say what the source
  says; does each link mean the entry the source means; does the source hold an
  entry or section the vault has cut wrong or missed. Each finding is fixed in
  `build-vault.ps1` or recorded in the register with the reason; the vault is
  regenerated, which undoes the plants.
- **Closed by:** the report quoted whole; both plants reported; a list mapping
  each other finding to its fix in this point's one commit or to its register
  entry; `check-vault.ps1` green after; the code-reviewer on the diff of
  `tools/vault/`.
- **Fails when:** the agent does not report one of the plants: the point stops.
- **Check: shallow** for the findings beyond the plants, a reading of a sample;
  what would prove the rest is the same reading of every note.
- **Core:** no — the diff touches only `tools/vault/`, `vault/` and the record.

## 3. Topics grouping the entries

- **Does:** dispatches one `general-purpose` agent, whose brief limits its
  writes to `vault/Topics/`, to read the vault and write one note per topic it
  finds across the documents. A topic note is front matter only, in the JSON
  form above: `type: "topic"` and `members`, an array of links to entry notes.
  Its body is empty, so the vault holds no prose of its own; the members show as
  links in the note's properties, and each member's backlinks name its topics.
  `build-vault.ps1` is rerun, so that `Home.md` links every topic, and writes
  `Bases/Topics.base` listing them. `check-vault.ps1` gains check 7: every file
  under `Topics/` parses, has `type: "topic"`, a `members` array of at least two
  links to entry notes, and an empty body; `Home.md` links every topic; and it
  lists the entry notes of type `requirement`, `acceptance`, `decision` and
  `ruling` that belong to no topic.
- **Closed by:** `git status` after the agent shows changes under
  `vault/Topics/` only; checks 1 to 7 green; the list of notes in no topic,
  produced by check 7, quoted in the outcome; `-SelfTest` gains a fixture per
  clause of check 7; the code-reviewer on the diff of `tools/vault/`.
- **Fails when:** a topic with a paragraph in its body, or with `members`
  written as a YAML list of bare links (check 7 red); a topic linking `R-999`
  (check 4 red); a topic missing from `Home.md` because the build ran before
  it was written (check 7 red).
- **Check: shallow** for whether a member belongs to its topic, a judgement no
  script reads; what proves it is the user browsing the topics.
- **Core:** no — the diff touches only `vault/`, `tools/vault/` and the record.
```

## What was not verified

- **Version 3 of the draft** was given to no blind reader. The table says what
  changed, not whether the changes hold; a third reading costs one agent and
  you may ask for it.
- **Obsidian itself.** No Obsidian is run in this session: that the JSON-form
  front matter, `types.json` and the `.base` files render as the draft expects
  is known from Obsidian's documentation, not tried. The exit condition's
  reserve is where it is checked.
- **The number of notes**, about seven hundred, is estimated from heading
  counts, not built.
- **Whether `workflow-review` is ready to merge**, which the branch line
  assumes, was not checked against its exit condition.

## What I need from you

1. **Answered by the user, 2026-10-20: "L'obiettivo è spostare /doc ne vault"**
   — the derived vault is not the goal; `docs/` itself is to move. Raised as
   `SQ-025`, blocking, for the decider; the draft below waits on its ruling.
   ~~**The draft**, approved, amended, or rejected. In particular the seven
   `Fails when:` lines of point 1, the two plants of point 2, and the
   `Check: shallow` markers on points 2 and 3.~~
2. **Free-text properties.** Version 3 drops a ruling's verdict and a
   question's status from the properties, since they wrap and repeat; the
   Bases then filter rulings by date and questions by date raised only. Keep
   them dropped, or ask for them with a rule for which line counts.
3. **Topic notes without prose.** A topic is a list of links and nothing else,
   so that the vault says nothing `docs/` does not. Keep it, or allow a
   description, which then needs a reader of its own.
4. **A third blind reading** of version 3 before installing, or not.
5. Then `workflow-review` is merged by you, the plan is installed on
   `obsidian-vault`, and point 1 starts.
