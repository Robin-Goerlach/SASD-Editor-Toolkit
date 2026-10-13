# Autonomous next-work queue

This is the default ordered queue when the user says only “continue”, “go on” or otherwise delegates milestone choice to Codex.

Do not skip an earlier unfinished item merely because a later item is more interesting. If the user names a different task, the user request wins.

## Queue rules

For each item: inspect existing code/tests/specs; use an ExecPlan only when `PLANS.md` requires one; implement the smallest complete behavioral slice; add deterministic tests; update specs and English/German docs; update `CHANGELOG.md`; validate; commit a green checkpoint; then mark the queue item complete and record follow-up.

Do not start M2 merely because an M1c item feels inconvenient.

## P1 - Complete M1c search and selection

### M1c.1 - Implement `Search.Replace`

**Status:** NEXT

Goal: add the first Core replace operation using the existing search service and command model, then expose a minimal host flow in the WinForms sample.

Required investigation:

- `src/Sasd.EditorToolkit.Core/Search/`
- command request/dispatcher types under `src/Sasd.EditorToolkit.Core/Commands/`
- document selection/caret model
- `tests/.../TextSearchServiceTests.cs`
- `tests/.../FindTextCommandTests.cs`
- `docs/spec/Command-Ids.md`

Expected scope:

- define clear literal replace behavior;
- support at least replace-current/next semantics consistent with existing find options;
- keep search mechanics UI-independent;
- route behavior through `Search.Replace`;
- update caret/selection deterministically;
- add Core tests for found/not-found, comparison options, wrapping where applicable, shorter/longer replacement, Unicode text and dirty/undo behavior;
- add minimal WinForms sample UI without putting replacement logic in the sample.

Non-goals: regex, huge-file replace-all optimization, search history persistence, incremental search panel, syntax-aware replace.

Acceptance: command works headlessly; Core tests pass; sample compiles on Windows CI; command/spec/changelog/docs agree.

### M1c.2 - Selection-aware edit commands

**Status:** AFTER M1c.1

Goal: make selection a first-class editing input rather than only a search display result.

Define/test delete selection, typed insertion replacing selection, newline over selection, select-all if useful, caret/selection collapse rules, and coherent dirty/undo behavior.

Before inventing command IDs, inspect existing naming and update `docs/spec/Command-Ids.md` for every stable string.

Acceptance: no WinForms-only selection semantics; single-line and multi-line tests; Unicode boundaries respected; dirty/undo coherent.

### M1c.3 - Selected-text rendering in WinForms

**Status:** AFTER M1c.2

Goal: render the Core selection with clear foreground/background contrast.

Selection geometry comes from Core; painting stays in the adapter; no second WinForms selection model.

Acceptance: sample clearly shows found/selected text, keyboard editing still routes through commands, Windows CI passes.

### M1c.4 - Search/replace conformance vectors

**Status:** AFTER M1c.3

Turn stable search/replace behavior into language-neutral vectors under `docs/spec/`.

Cover forward/backward find, wrap/no-wrap, case comparison, line-ending-spanning match if intentional, shorter/longer Unicode replacement, not-found behavior and portable caret/selection outcomes.

**M1c exit:** search + basic replace + selection editing + sample integration are stable and documented without regex or advanced UI scope.

## P2 - Strengthen architecture before broad M2

### Architecture boundary tests

**Status:** AFTER M1c

Add mechanical coverage proving Core has no UI-framework dependency, stable command IDs are unique, serialization contracts are deterministic where implemented, and product projects do not depend on sample assemblies.

### Command/error boundary audit

**Status:** AFTER architecture tests

Review dispatcher failure behavior, unknown command behavior and host-facing errors. Prefer typed/structured Core results over UI message boxes inside Core.

## P3 - Begin M2 in small slices

Only after M1c exits green. Suggested order: multiple documents/views; split/linked views; whole-line/block operations and markers; richer keyboard profiles; replaceable status/prompts/errors; settings/themes; search/replace improvements.

Each M2 slice must preserve the Core/adapter boundary and add language-neutral behavior to `docs/spec/`.

## Explicitly deferred

Do not autonomously start Rope/Piece Table rewrites, huge-file optimization, syntax highlighting, plugin frameworks, arbitrary macro/script execution, WPF/Web adapters, Java/C++ ports, a full MicroStar clone or NuGet release automation unless the user/roadmap explicitly moves them forward.
