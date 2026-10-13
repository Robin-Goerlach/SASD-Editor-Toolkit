# Current repository state for autonomous agents

**Snapshot date:** 2026-10-13  
**Repository:** `Robin-Goerlach/SASD-Editor-Toolkit`  
**Default branch:** `main`

This file is a dated orientation aid. Git and CI are authoritative; always refresh the baseline before work.

## Baseline observed while preparing the Codex harness

- `main` HEAD: `deead345f248784b12021e0e8860f27d5e85fabd`
- HEAD message: `docs(notes): add M1c search notes`
- Latest observed main CI run: `33743661386`
- Full Windows CI at that baseline: build succeeded, 0 warnings, 0 errors.
- Core test result at that baseline: 34 passed, 0 failed, 0 skipped.

Do not assume these numbers remain current. Re-read Git and the latest workflow run.

## Implemented product state

The current main branch already contains more than the original repository seed:

- UI-independent `Sasd.EditorToolkit.Core`;
- WinForms adapter and Modern FIRST-ED sample;
- line-oriented text buffer, text positions/ranges and document dirty state;
- basic insert/delete/newline operations;
- minimal undo/redo;
- file storage with encoding/BOM and line-ending handling;
- binary-content suspicion handling;
- command dispatcher;
- delete-left and delete-right commands;
- caret left/right/up/down plus line start/end commands;
- WinForms keyboard routing through Core commands;
- dirty-document prompts in the sample;
- UI-independent text search with forward/backward direction, wrapping and comparison options;
- `Search.Find` command integration;
- simple Find / Find Next UI;
- xUnit Core tests;
- Windows GitHub Actions CI.

See `CHANGELOG.md` and the code/tests for exact current behavior.

## Current milestone interpretation

M1a and the essential M1b adapter/host path are implemented enough that the repository has already moved into an M1c search/selection slice.

M1c is **not complete**. `docs/notes/M1c-Search-Notes.md` leaves these likely next areas:

- `Search.Replace`;
- selection-aware edit commands;
- better selected-text painting in WinForms;
- search/replace conformance vectors.

The updated `ROADMAP.md` formalizes M1c so an autonomous agent does not jump directly from M1b to broad M2 work.

## Important note about old branch instructions

`docs/notes/No-Further-M1-Scope-Expansion.md` was written for a specific historical branch/review stop point. It is not a repository-wide freeze and must not override `ROADMAP.md`, `AGENTS.md` or `docs/ai/NEXT-WORK.md`.

Several PR-2 notes are likewise historical review artifacts.

## Build topology

- Core: `net10.0`, cross-platform.
- Core tests: `net10.0`, cross-platform.
- WinForms adapter: `net10.0-windows`.
- FIRST-ED WinForms sample: `net10.0-windows`.
- CI: Windows, full solution.
- SDK policy: `global.json` requests .NET 10.0.100 with `latestFeature` roll-forward.

## Validation commands

Cross-platform Core validation:

```bash
./scripts/verify-core.sh
```

Windows full validation:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\verify-all.ps1
```

## Refresh checklist

At the start of autonomous work:

```bash
git status --short --branch
git pull --ff-only
git log -5 --oneline
```

Then inspect `CHANGELOG.md`, `docs/ai/NEXT-WORK.md`, the most relevant tests and the relevant specs/ADRs.

Update this snapshot after a meaningful milestone if its status becomes materially misleading. Do not churn the date/SHA after every documentation typo.
