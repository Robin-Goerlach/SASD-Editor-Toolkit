# Historical Traceability — SASD Editor Toolkit

[Deutsch](../de/Historische-Nachverfolgung.md) · [Requirements](Requirements-Specification.md) · [Implementation specification](Implementation-Specification.md) · [Architecture](Architecture.md)

**Snapshot:** 2026-11-03. This document maps historical *inspiration* to the independent modern toolkit architecture and requirements. It does **not** claim full behavioral parity with the original Turbo Editor Toolbox or certify product acceptance.

## 1. Purpose, copyright and product boundary

SASD Editor Toolkit is built as an **independent text-editing construction kit**. The 1985 *Turbo Editor Toolbox Version 1.0* handbook serves as historical context for concepts, command families and interaction styles—not as a source to copy code or protected presentation.

- No copied Pascal source, scans, logos, original artwork or long handbook excerpts.
- Never imply official Borland affiliation.
- “FIRST-ED” and “MicroStar” refer to historical concepts or example applications; they do not promise binary/UI compatibility.
- SASD retains independent requirements and design decisions.
- This document is not legal advice; trademarks and licensing need separate review before distribution.

**Crucially:** the separate **SASD Editor Toolbox** repository, and any possible future migration from it, are **out of scope**. This traceability evaluates only the originally independent SASD Editor Toolkit.

## 2. Source hierarchy

| Source | Purpose | Authority |
|---|---|---|
| *Turbo Editor Toolbox v1.0 — Owner's Handbook* | Historical feature/terminology context | Reference only; no copying authorization |
| [Requirements specification](Requirements-Specification.md), especially chs. 25–27 | Requirement families, historical chapters, procedure inventory | Functional source requirements |
| [Implementation specification](Implementation-Specification.md), especially chs. 38–45 | 208 mappings, 123 procedures and 46 historic global states | Planned technical allocation |
| \`docs/adr/\` | Accepted design decisions | Binding per AGENTS |
| \`docs/spec/\` | Platform-neutral observable behavior | Conformance contract |
| Code and tests on \`main\` | Features actually implemented | Implementation evidence, **not automatic acceptance** |
| \`docs/ai/CURRENT-STATE.md\` and \`NEXT-WORK.md\` | Dated state and upcoming work | Orientation, not release approval |

The implementation specification's “implementation” cells often describe an **intended realization**, not proof that a feature has already shipped.

## 3. Historical chapters mapped to modern concerns

| Historic chapter | Concept | Modern owner |
|---|---|---|
| Introduction | Editors as building blocks | Independent product, Core API, host |
| 1 | Characters, lines, editor terminology | \`TextDocument\`, \`ITextBuffer\`, views |
| 2 | Editor types and memory | Product/performance boundaries |
| 3 | Line arrays, fixed buffers, linked lists | \`ITextBuffer\`, \`LineTextBuffer\`, future engines |
| 4 | Simple editor | FIRST-ED reference sample |
| 5 | FIRST-ED, initialization, hooks | Host composition, default commands |
| 6 | UserCommand and prefix keys | Registry, dispatcher, later keyboard profiles |
| 7 | MicroStar and menus | Later host menus and optional demo |
| 8 | Popups, errors, status, dirty state, printing | Host services, Core results, optional print |
| 9 | Text/window state | Document, \`EditorViewState\`, workspace |
| 10 | Input loop and background work | Async tasks, cancellation, host event loop |
| 11 | Screen, colors, updates | Adapter rendering and invalidation |
| 12 | Editing commands, files, windows, blocks | Commands, text/storage/search/views |
| 13 | Overlays | **Not adopted**; assemblies and runtime |
| 14 | Embedding into programs | Core API, WinForms control, future adapters |
| 15 | Module/file inventory | Clear responsibility boundaries, not Pascal includes |
| 16 | Constants | Typed models, options and status |
| 17 | Types | Document/position/buffer/view |
| 18 | Globals | Encapsulated state rather than process globals |
| 19 | Procedures/functions | Inventory with explicit modern mappings |

Historical chapter traceability checks **completeness of decisions**, not whether historical data structures reappear as identical C# types.

## 4. Feature-family coverage matrix

| Family | Historic intent | SASD goal | M1 observation | Next work |
|---|---|---|---|---|
| Text/buffer | Lines, buffers and streams | UI-independent text core | \`LineTextBuffer\`, \`TextDocument\` exist | Unicode/performance hardening |
| Caret/navigation | Left/right, word/page, top/bottom | Command-driven movement | Basic navigation exists | Unicode/word/page cases |
| Editing | Insert, delete, reformat | Atomic commands | Insert/delete/newline exist | Selection/block M1c/M2 |
| Undo/dirty | Undo and change flag | Transactions/savepoint | Basic history and saved snapshot | Robust transactions M2 |
| Search/replace | Find, replace and options | Core find/replace | Find exists | Replace/selection M1c |
| Files | Read/write/confirm | Safe streams/files and host prompts | Encoding/BOM, temporary save | Conflict, binary warning, recovery |
| Views | Linked windows | Independent views on shared document | Workspace/view model exists | Complete host/split testing |
| Menus/keys | UserCommand, Ctrl-K/O/Q | Registry and serialized key profiles | Registry and limited key handling | Profiles/chords M2 |
| Status/errors | UserStatusLine, UserError | Replaceable host services | Host status and \`CommandResult\` | Structured errors/presenters |
| Background | UserTask/scheduler | Task/cancellation | No custom DOS scheduler | Host service on demand |
| Presentation | Screen, color, row updates | WinForms renderer and future adapters | Custom EditorSurface | DPI, IME, accessibility |
| Historic techniques | Pointers, overlays, BIOS | **Deliberate exclusion** | No DOS/BIOS architecture | Maintain clean-room boundary |

**Status rule:** “Present” only means code exists. “Passed” requires concrete test/acceptance evidence.

## 5. Historical hooks and modern equivalents

| Historic hook | Modern direction | Status |
|---|---|---|
| \`UserCommand\` | Command registry, future interceptors/key sequences | **Partial** |
| \`UserError\` | Structured Core errors and host presenter | **Planned**, initial CommandResult |
| \`UserStatusLine\` | Host status without UI-bound Core | **Partial** in FIRST-ED |
| \`UserReplace\` | Replacement strategy and optional prompt | **Open** |
| \`UserTask\` | Host-managed tasks/cancellation | **Later** |
| \`MakeWindow\`, \`RestoreWindow\` | Host/adapter dialog or overlay | **Partial** host dialogs |
| \`PulldownMenu\`, \`InitMainMenu\` | Commands-driven menus | **Partial** |
| \`SecondChar\` | Keyboard sequence resolver | **Open** |
| \`PrintNext\` | Optional print service | **Later** |
| \`SpellingCheck\` | External language service | **Later, optional** |

The complete hook, historic-module, constant and state inventory appears in requirements chs. 26–27 and implementation specification chs. 40–44. **We do not duplicate all 123 procedures here**, which avoids multiple diverging inventory copies.

## 6. Identifiers, verification and acceptance

- **208** formal product/functional/quality requirements are mapped in implementation-specification chapter 38.
- **15** use cases (\`UC-01\` through \`UC-15\`) have intended technical realizations.
- **15** acceptance criteria (\`AK-001\` through \`AK-015\`) have planned verification methods.
- **123** historical procedures are inventoried in the requirements and mapped to modern responsibilities or explicit non-adoption in the implementation specification.
- **46** historical global state elements are assigned modern responsibility boundaries.

These counts describe **documentation inventories**, not 208 passing tests, 123 implemented C# functions or an approved v1.0 release.

**Per-capability verification chain:** historic chapter → requirement ID → target component/command ID → ADR/spec → actual code → automated test → manual evidence (where UI matters) → milestone acceptance.

## 7. Deliberate exclusions and later extensions

**Not ported literally:** direct video-memory access, assembler-based row updates, BIOS, fixed 80×25 product limits, DOS polling, global typeahead ring buffers, global pointer lists, overlays/chain files and 255-character line caps.

**Later/optional:** complete historical key sequences, advanced MicroStar functions, printing/spelling, macros and additional UI adapters. Macros/scripts require their own security decision: historical compatibility **does not authorize automatic execution**.

**Modern SASD requirements:** Unicode, safer file state, portability, testability, localized UI and further editing services are independently defined, not historical source-copying obligations.

## 8. Maintenance process

For a historically inspired feature slice:

1. Record its requirements ID from the specification pair.
2. Define modern observable behavior in \`docs/spec/\`, without copying historical implementation.
3. Link code and evidence after implementation.
4. Update status only with proof; “planned” never automatically becomes “passed”.
5. Explain modern mapping when a historical name is not reproduced verbatim.
6. Update German and English documentation together.

See [Testing Guide](Testing-Guide.md) for acceptance procedures and [Roadmap](ROADMAP.md) for M1/M2 scope.
