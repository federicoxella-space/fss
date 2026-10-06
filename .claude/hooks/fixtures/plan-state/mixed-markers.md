# Plan — fixture, Italian and English markers in the same plan

**Kind:** process

## 1. Italian, closed

- **Core:** no — fixture.

*Esito:* 2026-10-01. Chiuso.

## 2. English, closed

- **Closed by:** a grep. `Check: shallow` for the prose.
- **Core:** no — fixture.

*Outcome:* 2026-10-02. Closed.

## 3. English, failed

- **Core:** no — fixture.

*Outcome:* 2026-10-03. FAILED — the check stayed red.

## 4. English, reserve discharged

- **Core:** no — fixture.

*Outcome:* 2026-10-03. Closed with reserve — "in CI" awaits the push.

*Reserve discharged:* 2026-10-04 — pushed, CI green.

## 5. Italian, reserve open

- **Core:** no — fixture.

*Esito:* 2026-10-03. Chiuso con riserva — "in CI" attende il push.

## 6. FALLITO named in prose

- **Core:** no — fixture.

*Esito:* 2026-10-03. Clause one met. Closed rather than
`FALLITO` — the work is done.
