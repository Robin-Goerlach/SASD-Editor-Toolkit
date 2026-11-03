# Architecture — SASD Editor Toolkit

[Deutsch](../de/Architektur.md) · [English documentation index](README.md) · [Implementation specification](Pflichtenheft.md) · [Roadmap](ROADMAP.md)

**Status:** architecture baseline for the independent product, describing actual and target states; **not** a release approval.  
**Snapshot:** 2026-11-03, inspected against \`main\` (\`820dc9f\`).  
**Normative basis:** \`docs/adr/\`, \`docs/spec/\`, requirements and implementation specifications. In case of conflict, follow \`AGENTS.md\`.

> **Scope:** SASD Editor Toolkit is an **independent editor construction kit**, with a C#/.NET reference core. The SASD Editor Toolbox is **neither a prerequisite nor the source implementation** for this architecture. A potential future product integration is explicitly **out of scope**. The separate SASD UI Toolkit is not a mandatory dependency either.

## 1. Architectural intent and principles

The toolkit provides reusable text editing without binding the domain state to WinForms or one particular application. **Modern FIRST-ED** is the first visible host and integration proof, not the only intended product form.

1. **One authoritative document state:** \`TextDocument\` and \`ITextBuffer\` own the text. UI adapters must not maintain a second editable copy.
2. **Separate document from view:** multiple views should ultimately refer to one document while holding independent caret, selection and scroll state.
3. **Command-first editing:** keyboard, menu, toolbar and API actions should resolve to stable command IDs and the same operations.
4. **Protect user data:** text, paths, settings and extensions are untrusted input. Cancellation must not introduce silent data loss.
5. **Pragmatic modularity:** start with Core and WinForms product assemblies, one sample and tests; split assemblies only for demonstrated needs.
6. **Portable semantics, not a mandatory universal ABI:** share specifications and test vectors; later Java/C++ ports use idiomatic memory and runtime behavior.
7. **Understandability before optimization:** state invariants, contracts and tradeoffs explicitly; optimize based on evidence.

## 2. System context and runtime building blocks

\`\`\`mermaid
flowchart TD
  User[User] --> Host[Host / Modern FIRST-ED]
  Host --> UI[WinForms adapter]
  Host --> Storage[Cross-platform file/stream services]
  UI --> Core[Editor Core]
  Storage --> Core
  Core --> Spec[Language-neutral contracts]
\`\`\`

The edge to specifications means **conformance**, not a runtime dependency. File/stream services currently live inside the Core project; the host handles file selection and confirmation dialogs.

### 2.1 Physical projects

| Component | Responsibility | Current status |
|---|---|---|
| \`Sasd.EditorToolkit.Core\` (\`net10.0\`) | Documents, buffers, view/workspace state, commands, search, undo, storage, settings | **Implemented M1 foundation** |
| \`Sasd.EditorToolkit.WinForms\` (\`net10.0-windows\`) | \`SasdEditorView\`, \`EditorSurface\`, rendering and input | **First implemented adapter**, not fully accepted |
| \`Sample.FirstEd.WinForms\` | Menus, toolbar, search prompt, file/close dialogs, status | **Running reference sample** with limited M1 functionality |
| \`Sasd.EditorToolkit.Core.Tests\` | xUnit Core tests including edge cases | **Present** |
| WPF, web, Java, C++, Python | Alternative adapters or independent language implementations | **Not implemented, not approved as M1 scope** |

**Forbidden dependencies:** Core → WinForms/WPF/web; Core → sample; product assembly → tests; any cyclic dependency. The host may reference Core and an adapter. A future native UI must not accidentally expose C# memory or threading assumptions as a cross-language ABI.

### 2.2 Logical responsibility boundaries

| Concern | Owner | Must not own |
|---|---|---|
| Text model / \`ITextBuffer\` | Core | Pixels, focus, file-picker dialogs |
| Document / undo / dirty state | Core | MessageBox, autosave directory choice |
| ViewState / workspace | Core | WinForms controls and layout containers |
| Command registry / dispatcher | Core | Keyboard scan codes and menu labels |
| Search | Core | Search dialogs and user notifications |
| Encoding / streams / files | Core infrastructure | User confirmation and host access policy |
| Rendering / input | WinForms adapter | A separate editing or undo state machine |
| Workflow / dialogs | Host/sample | Reusable editing algorithms |

For M1, **application services are a logical layer within Core**, not another project. A physical split would need benefits demonstrated through evidence and an ADR.

## 3. What actually exists in M1

- \`TextDocument\` owns \`ITextBuffer\`, metadata and \`UndoManager\`; \`IsDirty\` currently compares the full text against a stored snapshot.
- \`LineTextBuffer\` stores logical lines with \`LineEndingKind\` but rebuilds the entire text representation on edits. This is **not** a large-file engine.
- \`EditorViewState\` holds caret, selection, scrolling and initial editing options. \`EditorWorkspace\` models several documents/views; the FIRST-ED host does **not yet demonstrate** a complete interactive split-view workflow.
- \`EditorCommandRegistry\` registers a small command set, and \`EditorCommandDispatcher\` validates and runs commands. \`Search.Replace\` is **not yet a registered/implemented default command**.
- \`TextSearchService\` searches a complete snapshot with literal text, direction, \`StringComparison\` and wrap. Cancellable replacement and regex are **not current features**.
- \`FileDocumentStorage\` supports streams, BOM detection, encoding and temporary-file-based save replacement. It currently buffers complete file bytes; binary suspicion is largely NUL-based and external-change conflict detection is not complete.
- \`EditorSurface\` draws directly and maps a limited key set. JSON keyboard profiles, generic host prompts and advanced accessibility in the implementation specification are **target architecture**.
- \`EditorSettings\` is a small settings seed; the JSON formats in \`docs/spec/\` are not yet a complete working configuration subsystem.

The implementation specification contains future-oriented C# interface examples. **An interface described in a design example does not necessarily exist in the current repository.**

## 4. Text model, positions and mutations

\`\`\`mermaid
sequenceDiagram
  participant H as Host / Adapter
  participant D as Command dispatcher
  participant C as Core command
  participant Doc as TextDocument
  participant Buf as ITextBuffer
  H->>D: Command ID + context
  D->>C: CanExecute / ExecuteAsync
  C->>Doc: Insert / Delete / Replace
  Doc->>Buf: Atomic edit
  Buf-->>Doc: TextChangeSet + Changed
  Doc-->>H: Updated document state
  H->>H: Update presentation
\`\`\`

**Required invariants:**

- Empty documents have at least one addressable empty line.
- \`TextPosition\` uses zero-based logical lines/columns; \`TextRange\` denotes a half-open range.
- Current .NET columns refer to UTF-16 code units. A language-neutral specification must distinguish *code units*, *Unicode scalars*, *grapheme clusters* and *visual columns* for future ports.
- Changes must be observably **fully applied** or **not applied**. No partial document mutations following a failure.
- CRLF, LF, CR and lack of a final newline should be preserved by supported storage paths.
- Each text mutation advances buffer version. View rendering responds to document changes without duplicating the document.
- User-facing caret/selection operations must not deliberately split UTF-16 surrogate pairs or connected grapheme clusters. The existing implementation does **not yet comprehensively enforce** this goal; dedicated tests are required.

**Outstanding ownership issue:** \`TextDocument.Buffer\` remains publicly mutable while document methods separately record undo entries. Direct buffer edits may therefore bypass document-level undo. Before advanced undo, choose a single **mutation/ownership boundary** (e.g., document-only mutations or controlled internal transactions). This is a public-contract decision requiring an ADR, not a silent refactor.

## 5. Undo, dirty state and multiple views

**Current:** \`UndoManager\` stores stacks of \`TextChangeSet\`, and \`TextDocument\` applies inverse operations. It is a useful M1 baseline, **not** a complete transaction system with coalescing, memory budgets and persistent anchors.

**Target:** all domain mutations follow a document-scoped transaction path. Undo/redo is independent of view count. A savepoint tracks successfully persisted content; presentation-only changes must not make a document dirty. Each view owns only local presentation/editing state.

**Lifetime rule:** do not discard a document while live views or host references still use it. When closing the last view of a modified document, let the host request Save/Discard/Cancel rather than opening a \`MessageBox\` from Core.

**Suggested hardening order:** mutation boundary → consistent undo transactions → anchor/selection transformation → multiview tests → performance. This is a documented proposal, not an already implemented decision.

## 6. Commands, host services and error contracts

Desired command route:

\`Keyboard/menu/API → command ID → context → CanExecute → handler → document mutation → result → host rendering\`.

- Command IDs are readable, language-neutral strings; see \`docs/spec/Command-Ids.md\`.
- **Existing:** registry, dispatcher and \`CommandResult\`; unknown/non-executable commands return \`NotHandled\`. A general interceptor pipeline, host-command versioning and JSON keyboard profiles remain future work.
- **Target:** stable structured error codes, keeping localized human-readable messages out of the machine contract. The host localizes messages. Cancellation must not imply an edit was rolled back if it already committed.
- **Security:** command invocation **does not grant permission** to access arbitrary files, run shell code, use networks or start processes. Macros and plugins require a dedicated security ADR.
- **On-demand host services:** clipboard, file and folder pickers, confirmations, error/status display, optional logging and autosave. Avoid introducing interfaces or service containers for functionality not yet needed.

## 7. File persistence, integrity and failure handling

| Stage | Required behavior | M1 observation |
|---|---|---|
| Load | Detect encoding/BOM; flag binary suspicion; preserve previous document on failure | Encoding/BOM and flag available; whole-byte buffering; no complete host-side binary dialog |
| Edit | Only explicit domain mutations change dirty/undo | Basic undo path present; direct buffer edits still possible |
| Save | Write temp file in target directory, flush, replace atomically where supported, then \`MarkSaved\` | Temp file and \`File.Move(overwrite: true)\` present; guarantees depend on filesystem |
| Conflict | Detect edits from another process; ask host before overwriting | Fingerprint/conflict workflow outstanding |
| Cancel | Protect original file and document | Additional tests needed, especially around replacement errors |
| Recovery | Explicit autosave and protected recovery snapshots | Later milestone |

**Design requirement:** the host determines allowed paths and confirms destructive/binary conflicts. Libraries do not display dialogs. Clean temporary files as well as possible on failures. Validate symlinks, permissions, concurrent writers and recovery directory security before any production release.

## 8. UI adapters and accessibility

\`SasdEditorView\` binds a document to \`EditorSurface\`. Today the surface draws text lines, line numbers, current line, selection background and caret; the host owns menus and dialogs.

**Permanent boundary:** Core decides *what* is selected/edited; the adapter decides *how* to draw it, take focus and handle IME/pointer input. Font metrics, pixels and DPI do not belong to \`TextDocument\`.

Next steps: defined view binding, visible-range layout, configurable contrast/theme values, redraw correctness, Unicode/IME behavior, keyboard support, screen-reader metadata and focus-safe caret. Never duplicate the text buffer inside a control.

## 9. Language portability and interoperability

Requiring a common native C++ library with .NET/Java/Python adapters **now** would be premature. It would impose ABI, ownership, error, threading and Unicode conversion rules before the functional contract is stable.

Instead:

- \`docs/spec/\` defines observable semantics, command IDs, serialized formats and conformance vectors.
- C#/.NET remains the first full reference implementation, WinForms its first adapter.
- Later Java/C++ implementations may use idiomatic garbage collection or RAII/ownership and choose internal encodings independently.
- If a genuine FFI/ABI use case emerges, design an **explicit, narrow, versioned C ABI** with ownership, lifetime, error and encoding rules; record that decision separately.
- Evaluate WPF/web as adapters or bridges rather than automatically as additional Core implementations.

## 10. Security, performance and diagnostics

**Threat model:** malicious or corrupt files, unusual Unicode sequences, unwritable paths, files changed between load and save, huge input, anomalous keystrokes and possibly untrusted hosts.

**Hard requirements:** never execute document content; no secrets in logging; no hidden network calls; no unconfirmed overwrite of detected external changes; understandable errors and conservative defaults.

**Performance approach:** M1 deliberately favors correctness and learning value. \`LineTextBuffer.GetText()\`, \`Length\`, offset conversions and edits currently work on complete text/line representations. \`TextDocument.IsDirty\` compares snapshots. Before claiming large-file readiness, add load tests, benchmarks and possibly indexed buffers/Piece Table/Rope behind \`ITextBuffer\`. Optimization must not bypass undo or event invariants.

**Observability:** emit useful technical events without document text; redact paths according to host policies; collect durations and sizes only deliberately and with privacy in mind.

## 11. Quality assurance and architecture gates

| Gate | Evidence | Status |
|---|---|---|
| UI-free Core | Project dependency check, later mechanical architecture test | Project boundary visible; automated gate pending |
| Deterministic Core | xUnit tests | Present |
| Complete Windows solution | \`scripts/verify-all.ps1\`, \`.github/workflows/ci.yml\` | Present; check current CI separately |
| Linux/macOS Core | \`scripts/verify-core.sh\` | Script exists |
| File integrity | BOM, line-ending and storage tests | Partially covered; conflict/crash tests pending |
| Unicode graphemes and IME | Core vectors and manual Windows tests | Incomplete |
| Command IDs / JSON profiles | Uniqueness/schema and conformance tests | Planned |
| Release/API/NuGet | Package, API baseline, consumer and license checks | M3 target |

A green build is **not** evidence of complete functionality, accessibility or release acceptance.

## 12. Decisions and prioritized architectural work

Accepted: ADR-0001 through ADR-0005 (C# reference, language-neutral concepts, repository strategy, M1 scope, Unicode/indexing). New public contracts require dedicated decisions.

| Priority | Decision or work | Timing |
|---|---|---|
| P1 | Complete M1c \`Search.Replace\`, selection editing and deterministic caret rules | Before M2 |
| P1 | Mechanically enforce Core/UI boundaries and unique command IDs | After M1c |
| P2 | Decide document/buffer mutation ownership via ADR | Before advanced undo |
| P2 | Specify savepoints, undo transactions and position tracking | Before multiview acceptance |
| P2 | Define host decisions for binary suspicion, file conflicts and close guard | Before v1 |
| P3 | Key profiles, structured errors, serialized settings | M2 |
| P3 | Choose performance/large-file strategy and accessibility based on evidence | M3 |
| Later | WPF/web/Java/C++/FFI only on demand and after separate design decision | Following reference stability |

**Not decided here:** new assembly/package splitting, a C++ interop ABI, new runtime dependencies or expanding M1 scope. This is architecture documentation, **not a product-code change**.

## 13. References and navigation

- [Requirements specification](Lastenheft.md), [Implementation specification](Pflichtenheft.md): product requirements and target solution.
- [Roadmap](ROADMAP.md): milestone planning and evidenced progress.
- [Architecture Decision Records](../adr/): accepted technical decisions.
- [Editor Model](../spec/Editor-Model.md), [Text Buffer Behavior](../spec/Text-Buffer-Behavior.md), [Command IDs](../spec/Command-Ids.md): normative language-neutral contracts.
- [Developer guide](Developer-Guide.md), [Testing guide](Testing-Guide.md), [User guide](User-Guide.md): implementation, verification and use.
- [Historical traceability](Historical-Traceability.md): origins, coverage and evidence gaps.
