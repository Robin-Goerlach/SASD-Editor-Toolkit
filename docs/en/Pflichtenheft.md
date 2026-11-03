# Implementation Specification (Pflichtenheft)
## SASD Editor Toolkit — Technical Realization of the Modern Turbo Editor Toolbox Concept

| Attribute | Decision |
|---|---|
| Document type | Implementation specification / technical and organizational delivery requirements |
| Project | SASD Editor Toolkit |
| Document version | 0.1 |
| Status | Detailed working draft for architecture and implementation approval |
| Date | 2026-07-24 |
| Basis | SASD Editor Toolkit requirements specification (Lastenheft) version 0.1 |
| Source evidence | `Lastenheft_SASD_Editor_Toolkit_Version_0.1.md`, SHA-256 `0d70684646c59d43f8144d51036b8636abda5a9f6df5f32b55d652b35fd167f3` |
| Core target | .NET 10 (`net10.0`) |
| First UI adapter | WinForms (`net10.0-windows`) |
| Later adapters | WPF and Blazor/Razor; ASPX/Web Forms as documented compatibility route |
| Project language | English for code, public APIs and package names; German and English for docs |
| Repository | Independent `SASD-Editor-Toolkit` repository |

> **Relation to the requirements specification:** This document explains **how** the requirements are implemented, tested, packaged and delivered incrementally. Every formal source requirement appears in the complete traceability matrix in ch. 38. Requirements must not be dropped silently; future/optional functionality receives an explicit milestone.

---

## 1. Purpose and scope

This document defines the technical realization of SASD Editor Toolkit: UI-independent Core, first WinForms adapter, reference applications, tests, documentation, packaging and planned extension points for WPF/web.

It specifies:

- architecture and dependency direction;
- repository and solution structure;
- functional models and public interfaces;
- editor command implementation;
- file, undo, search, multiview and rendering concepts;
- quality, security, test and documentation measures;
- mapping of source requirements to milestones.

Detailed icon designs, marketing copy and full technical designs for later WPF/web adapters are outside this document. Their integration contracts are nevertheless defined here.

## 2. Completeness and change management

The source requirements include **208 formal product, functional and quality requirements**, **15 use cases**, **15 acceptance criteria** and **7 milestones**. This document incorporates all of them.

Requirement changes follow this procedure:

1. Record the proposed change and affected identifiers.
2. Assess impact on architecture, API, tests, documentation and milestones.
3. Version source and implementation specifications together.
4. Update traceability.
5. Record breaking changes in `docs/en/CHANGELOG.md` and an Architecture Decision Record.

| Evidence type | Authoritative location |
|---|---|
| Requirements specification | `docs/de/Lastenheft.md` (English translation: `docs/en/Lastenheft.md`) |
| Implementation specification | `docs/de/Pflichtenheft.md` (this English translation under `docs/en/`) |
| Architecture decisions | `docs/adr/ADR-XXXX-*.md` |
| Requirement traceability | `docs/de/Historical-Traceability.md` (navigation and coverage) and the full 208-row appendix in this document |
| Test evidence | Automated tests and CI artifacts |
| Release evidence | `docs/en/CHANGELOG.md`, release notes and NuGet metadata |

## 3. Solution strategy

The product is not a WinForms application with embedded editing rules. It is a product family consisting of functional Core, platform adapter and sample hosts.

```text
Host / Sample / SASD Desktop Components
                |
                v
Sasd.EditorToolkit.WinForms
                |
                v
Sasd.EditorToolkit.Core
                |
                v
BCL + optional host services
```

Core decisions:

- **Single source of truth:** `TextDocument` in Core owns the text.
- **Multiple views:** several `EditorViewState` instances reference one document.
- **Command-first:** menus, keyboard, toolbar, context menus and host APIs trigger the same command IDs.
- **Custom WinForms rendering:** the first production adapter provides its own editor surface rather than coupling Core to `RichTextBox`.
- **Replaceable text buffer:** 0.1 uses a line-oriented buffer; the contract allows Piece Table/Rope implementations from 1.0 onward.
- **No obsolete technical debt:** no DOS overlays, BIOS calls, pointer lists, fixed 80×25 layout or global editor variables.
- **Incremental delivery:** Modern FIRST-ED in M1, historical feature breadth in M2, stabilization/large-file route in M3.

## 4. Technology decisions

| Area | Choice | Rationale |
|---|---|---|
| Language | C# with nullable reference types enabled | Safe public APIs and maintainability |
| Core target framework | `net10.0` | Modern cross-platform .NET core |
| WinForms target framework | `net10.0-windows` with `UseWindowsForms=true` | First binding reference UI |
| Testing | xUnit | Used elsewhere by SASD; automatable |
| Serialization | `System.Text.Json` | Close to BCL; avoid unnecessary runtime dependencies |
| Logging | `Microsoft.Extensions.Logging.Abstractions` or equivalent thin boundary | Integrate desktop, server and web hosts |
| File I/O | Asynchronous `Stream`/`FileStream` with `CancellationToken` | Responsiveness and headless operation |
| WinForms rendering | Custom GDI/TextRenderer-based EditorSurface with layout cache | Controlled state, several views, later adapters |
| Build | `dotnet` CLI, deterministic builds, shared `Directory.Build.props` | Reproducibility |
| CI | GitHub Actions | Build, test, package, documentation and architecture checks |
| Packages | Separate NuGet packages for Core and WinForms | Independent reuse/versioning |

Except for the logging abstraction, Core should not require third-party runtime packages. Test/build dependencies are permitted for development.

## 5. Repository and solution structure

```text
SASD-Editor-Toolkit/
├── .github/
│   ├── workflows/
│   └── ISSUE_TEMPLATE/
├── artefacts/
│   ├── screenshots/
│   └── diagrams/
├── docs/
│   ├── adr/
│   ├── de/                       # German documentation
│   └── en/                       # English documentation
│       ├── ROADMAP.md
│       ├── CHANGELOG.md
│       ├── CONTRIBUTING.md
│       ├── SECURITY.md
│       └── PLANS.md
├── samples/
│   ├── Sasd.EditorToolkit.Sample.FirstEd.WinForms/
│   └── Sasd.EditorToolkit.Sample.MicroStar.WinForms/  # from M2
├── src/
│   ├── Sasd.EditorToolkit.Core/
│   └── Sasd.EditorToolkit.WinForms/
├── tests/
│   ├── Sasd.EditorToolkit.Core.Tests/
│   ├── Sasd.EditorToolkit.WinForms.Tests/
│   ├── Sasd.EditorToolkit.Integration.Tests/
│   └── Sasd.EditorToolkit.Architecture.Tests/
├── Directory.Build.props
├── Directory.Packages.props
├── global.json
├── SASD-Editor-Toolkit.slnx
├── README.md
├── AGENTS.md
└── LICENSE
```

**Documentation structure (updated 2026-11-03):** Root `README.md`, `AGENTS.md` and `LICENSE` remain because of their repository roles. Detailed English project documentation lives under `docs/en/` and roadmaps are present in both language folders. Small pointers in `.github/CONTRIBUTING.md` and `.github/SECURITY.md` preserve GitHub's discovery behavior. The diagram is a **target structure**, not evidence that all shown samples/tests already exist.

Version 0.1 intentionally does not introduce a separate `Abstractions` project. Interfaces belong to explicit Core namespaces. A genuinely shared contract assembly may be introduced through an ADR only when several independently versioned packages require one.

## 6. Project responsibilities and dependency rules

| Project | Responsibility | Allowed references |
|---|---|---|
| `Sasd.EditorToolkit.Core` | Documents, buffers, view state, commands, search, undo, file operations, settings | BCL and approved logging abstraction |
| `Sasd.EditorToolkit.WinForms` | Rendering, keyboard/mouse/IME, clipboard, scrollbars, dialog adapter, accessibility | Core and WinForms |
| `Sample.FirstEd.WinForms` | Small M1 reference application | Core and WinForms |
| `Sample.MicroStar.WinForms` | Advanced menu/multiview/compatibility demo from M2 | Core and WinForms |
| Tests | Automated functional, integration and architecture validation | Relevant product and test assemblies |

Mandatory rules:

- Core never references WinForms, WPF, ASP.NET or browser types.
- WinForms must not duplicate document or undo logic.
- Samples must not contain reusable core logic.
- SASD Desktop Components consumes a NuGet package/project reference; never duplicate source.
- Data Toolbox, Numerics, Graphics and GameWorks are not required dependencies.
- Extensions register commands and services through public contracts and do not reach into internal state.

## 7. Namespace and API conventions

```text
Sasd.EditorToolkit
Sasd.EditorToolkit.Text
Sasd.EditorToolkit.Documents
Sasd.EditorToolkit.Editing
Sasd.EditorToolkit.Commands
Sasd.EditorToolkit.Input
Sasd.EditorToolkit.Search
Sasd.EditorToolkit.Undo
Sasd.EditorToolkit.Storage
Sasd.EditorToolkit.Settings
Sasd.EditorToolkit.Diagnostics
Sasd.EditorToolkit.WinForms
Sasd.EditorToolkit.WinForms.Rendering
Sasd.EditorToolkit.WinForms.Input
Sasd.EditorToolkit.WinForms.Accessibility
```

Public types use English names. Public APIs have XML documentation for purpose, parameters, return values, failure states, threading and cancellation. Internal implementation details are `internal` by default.

## 8. Functional object model

| Type | Kind | Responsibility |
|---|---|---|
| `TextDocument` | class | Owns buffer, metadata, UndoManager, markers, dirty state and events |
| `DocumentId` | readonly record struct | Stable identity independent of path |
| `DocumentMetadata` | class/record | Display name, path, encoding, line-ending strategy, timestamp, file fingerprint |
| `ITextBuffer` | interface | Reading and atomic mutation without UI dependency |
| `LineTextBuffer` | class | Default M1 buffer: lines with individual line endings |
| `TextPosition` | readonly record struct | Zero-based line and UTF-16 column, validated/normalized |
| `TextRange` | readonly record struct | Normalized half-open range `[Start, End)` |
| `TextSelection` | class/record | Anchor, active position, direction and mode |
| `TextChange` | record | Start position, removed and inserted text, change metadata |
| `TextChangeSet` | record | Atomic group of related edits |
| `TextAnchor` | class | Position tracking with forward/backward affinity |
| `TextMarker` | class | Named/numbered marker based on an anchor |
| `TextAnnotation` | abstraction | Extensible highlighting or metadata for a range/line |
| `EditorViewState` | class | Caret, selection, scroll, margins, tabs, modes and display options |
| `EditorWorkspace` | class | Documents, views, active view and close workflows |
| `EditorSession` | class | Optionally persistent workspace/layout state |

`TextDocument` and `EditorViewState` are strictly separate. A document can exist without a view, but a view cannot exist without a document.

## 9. Text buffers and text representation

### 9.1 Internal representation

- Internally store text as Unicode .NET strings (`string`, UTF-16).
- Each logical line stores content without its terminator plus optional `LineEndingKind`: `None`, `CrLf`, `Lf` or `Cr`.
- Preserve mixed line endings on load. Save policy can select `Preserve`, `NormalizeToCrLf`, `NormalizeToLf` or `NormalizeToCr`.
- Empty documents always provide at least one addressable empty logical line.
- No fixed 255-character limit.
- Text positions are zero-based; user interfaces normally display one-based line/column numbers.

### 9.2 ITextBuffer contract

```csharp
public interface ITextBuffer
{
    int LineCount { get; }
    long Version { get; }
    int Length { get; }

    ReadOnlyMemory<char> GetLineText(int lineIndex);
    LineEndingKind GetLineEnding(int lineIndex);
    string GetText(TextRange range);
    TextPosition Normalize(TextPosition position);
    int GetOffset(TextPosition position);
    TextPosition GetPosition(int offset);

    TextChangeSet Insert(TextPosition position, string text);
    TextChangeSet Delete(TextRange range);
    TextChangeSet Replace(TextRange range, string text);

    event EventHandler<TextBufferChangedEventArgs>? Changed;
}
```

Edits are atomic. The buffer emits a `TextChangeSet` that is consumed by undo, markers, views and rendering. It does not expose directly mutable lists or string references.

### 9.3 Caret and Unicode rules

- Public column values are UTF-16 indices for lossless interoperability with .NET APIs.
- Caret movement uses valid text-element boundaries and must not intentionally split surrogate pairs or composed sequences.
- `VisualColumnCalculator` determines visible columns considering tabs and optionally proportional fonts.
- A replaceable `IWordBoundaryService` determines word boundaries; the default is Unicode-oriented and configurable.

### 9.4 Large-file route

M1 uses `LineTextBuffer` for clarity. M3 introduces a planned second implementation, `PieceTableTextBuffer`. A future `ITextBufferFactory` selects one based, for example, on file size and host options. Public document and command APIs must not depend on the choice.

## 10. Document lifecycle

The lifecycle includes `New`, `Load`, `Edit`, `Save`, `SaveAs`, `Reload`, `Close` and `Dispose`.

```text
New/Load
   -> Clean document + savepoint
   -> Editing changes buffer and undo stack
   -> Dirty document
   -> Save succeeds: update metadata + savepoint
   -> Close request: Save / Discard / Cancel
   -> Close and dispose when no view/reference remains
```

On close, Core does not show a dialog. It returns `CloseDecisionRequired`; the host asks the user and retries the workflow with `Save`, `Discard` or `Cancel`. Core therefore stays UI-free.

## 11. Workspace, views and multiple windows

`EditorWorkspace` manages documents and views. Each view has a distinct `ViewId`, references exactly one `TextDocument` and owns an independent `EditorViewState`.

| Operation | Implementation |
|---|---|
| New file | Create `TextDocument` and first view |
| Second view of same document | Bind another `EditorViewState` to existing document |
| Close view | Remove only view; keep document if other views/host references exist |
| Close document | Close all associated views, applying dirty guard |
| Next/previous view | Workspace maintains ordered view list and active view |
| Go to view | Navigate by `ViewId`, index or host choice |
| Hidden view | Keep view state in workspace, detach rendering host |
| Split view | WinForms sample creates second view in `SplitContainer` |

Document change events reach all views. Each view invalidates only affected layout regions and normalizes caret/selection.

## 12. Undo, redo and dirty state

### 12.1 Undo architecture

Every mutation occurs in an `UndoTransaction`. The transaction accumulates one or more `TextChangeSet` objects plus view changes where those changes are meaningfully undoable.

```csharp
public interface IUndoUnit
{
    string Description { get; }
    long EstimatedBytes { get; }
    void Undo(TextDocument document);
    void Redo(TextDocument document);
}
```

### 12.2 Rules

- Group consecutive typing by time and position.
- Insert, delete, paste, block operations, replace and reformat must be undoable.
- Discard redo history when a new edit follows undo.
- Configure undo limits by operation count and estimated memory budget.
- `Savepoint` is a stable index/fingerprint in the undo history. `IsDirty` compares current state with that savepoint rather than merely trusting a manually set boolean.
- Host commands must mutate through document APIs to preserve undo and dirty-state correctness.

## 13. Selection, blocks and clipboard

Default selection is character-oriented and half-open. Historical line-block mode is added from M2 as `SelectionMode.LineBlock`.

| Function | Implementation |
|---|---|
| Selection boundaries | `TextSelection` with Anchor and ActivePosition |
| Extend selection | NavigationCommand with `ExtendSelection=true` |
| Copy | Extract normalized range through `IClipboardService` |
| Cut | Copy plus atomic delete transaction |
| Paste | Normalize Unicode text and insert atomically |
| Move | For same document, transform target position correctly around delete/insert |
| Hide/show block | Retain logical selection; rendering option controls visibility |
| Cross-document | Immutable source-text snapshot; separate target undo transaction |

Core has no dependency on the operating-system clipboard. WinForms implements `WinFormsClipboardService`; headless tests use `InMemoryClipboardService`.

## 14. Command system

### 14.1 Core contracts

```csharp
public readonly record struct EditorCommandId(string Value);

public interface IEditorCommand
{
    EditorCommandId Id { get; }
    bool CanExecute(EditorCommandContext context);
    ValueTask<CommandResult> ExecuteAsync(
        EditorCommandContext context,
        CancellationToken cancellationToken);
}

public interface ICommandInterceptor
{
    ValueTask<CommandInterceptionResult> BeforeExecuteAsync(
        EditorCommandInvocation invocation,
        CancellationToken cancellationToken);
}
```

### 14.2 Dispatcher workflow

1. The input adapter triggers a KeyGesture or API command.
2. `KeyboardSequenceResolver` resolves complete commands, prefix states or unmapped keys.
3. `EditorCommandDispatcher` builds context from workspace, active view, document and host services.
4. Interceptors may replace, block, mark as handled or forward the command.
5. Evaluate `CanExecute`.
6. Execute the command with a CancellationToken.
7. Publish result, error, status and change events.

### 14.3 Keyboard profiles

`KeyboardProfile` holds serializable mappings from `KeySequence` to `EditorCommandId`. Profiles are loaded from JSON. M1 supplies `Modern.json`; M2 adds `TurboEditorCompatible.json`. Prefix sequences use a configurable timeout, can be cancelled by Escape and may show the expected second key in the host.

## 15. Command catalog

| Command ID | Meaning | Milestone |
|---|---|---|
| File.New | New document | M1 |
| File.Open | Open file | M1 |
| File.Save | Save | M1 |
| File.SaveAs | Save As | M1 |
| File.Close | Close document/view | M1 |
| App.Exit | Request host shutdown | M1 |
| Edit.InsertText | Insert/overwrite text | M1 |
| Edit.NewLine | New line | M1 |
| Edit.InsertLine | Insert empty line | M1 |
| Edit.DeleteLeft | Delete to left | M1 |
| Edit.DeleteRight | Delete to right/join lines | M1 |
| Edit.DeleteLine | Delete line | M1 |
| Edit.DeleteToLineEnd | Delete to line end | M1 |
| Edit.DeleteWordRight | Delete word to right | M1 |
| Edit.ChangeCase | Change letter case | M2 |
| Edit.InsertControlCharacter | Insert control character | M2 |
| Edit.Undo | Undo | M1 |
| Edit.Redo | Redo | M1 |
| Edit.Cut | Cut | M1 |
| Edit.Copy | Copy | M1 |
| Edit.Paste | Paste | M1 |
| Edit.SelectAll | Select all | M1 |
| Navigate.Left | One character left | M1 |
| Navigate.Right | One character right | M1 |
| Navigate.Up | One line up | M1 |
| Navigate.Down | One line down | M1 |
| Navigate.WordLeft | One word left | M1 |
| Navigate.WordRight | One word right | M1 |
| Navigate.LineStart | Line start | M1 |
| Navigate.LineEnd | Line end | M1 |
| Navigate.DocumentStart | Document start | M1 |
| Navigate.DocumentEnd | Document end | M1 |
| Navigate.PageUp | Page up | M1 |
| Navigate.PageDown | Page down | M1 |
| Navigate.GoToLine | Go to line | M1 |
| Navigate.GoToColumn | Go to column | M2 |
| Navigate.SelectionStart | Go to selection start | M2 |
| Navigate.SelectionEnd | Go to selection end | M2 |
| Search.Find | Find | M1 |
| Search.FindNext | Find next | M1 |
| Search.FindPrevious | Find previous | M2 |
| Search.Replace | Replace one | M1 |
| Search.ReplaceAll | Replace all | M1 |
| Marker.Set | Set marker | M2 |
| Marker.GoTo | Go to marker | M2 |
| Format.ToggleWordWrap | Toggle word wrap | M2 |
| Format.ToggleAutoIndent | Toggle auto-indent | M2 |
| Format.ReformatParagraph | Reformat paragraph | M2 |
| Format.CenterLine | Center line | M2 |
| Format.SetLeftMargin | Set left margin | M2 |
| Format.SetRightMargin | Set right margin | M2 |
| Format.SetTabSize | Set tab width | M2 |
| Edit.ToggleInsertMode | Toggle insert/overwrite | M1 |
| View.New | New view | M2 |
| View.Split | Split view | M2 |
| View.Close | Close view | M1 |
| View.Next | Next view | M2 |
| View.Previous | Previous view | M2 |
| View.GoTo | Go to view | M2 |
| View.LinkToDocument | Bind view to document | M2 |
| Block.Begin | Set historical block start | M2 |
| Block.End | Set historical block end | M2 |
| Block.Copy | Copy block | M2 |
| Block.Move | Move block | M2 |
| Block.Delete | Delete block | M2 |
| Block.ToggleVisibility | Toggle block highlighting | M2 |
| Operation.Cancel | Cancel long-running operation | M1 |

Command IDs are public stable strings. New commands may be added; renaming them after v1.0 is a breaking change.

## 16. Editing and navigation algorithms

### 16.1 Insertion and overwriting

- Insert mode inserts text at the caret position.
- Overwrite mode replaces the next text element on the same line for each entered text element; insertion occurs at end of line.
- Multiline text is split into lines, preserving or normalizing line-ending metadata according to the paste policy.
- Consecutive typing is coalesced into one undo step while document, position, mode and time window remain compatible.

### 16.2 Deletion and line joining

- Backspace at the start of a line joins it with the preceding line.
- Delete at the end of a line joins it with the next line.
- DeleteLine removes content and terminator according to defined edge cases; a document always retains at least one empty line.
- DeleteWordRight uses `IWordBoundaryService`.

### 16.3 Navigation

- Vertical movement retains a desired visual X coordinate so differing line lengths do not cause lateral drift.
- PageUp/PageDown moves `VisibleLineCount - 1` lines, at least one.
- Home/End refers to logical lines; an optional Smart Home first visits the first non-whitespace character and then column zero.
- Selection extension only moves the active endpoint, not the anchor.

## 17. Word wrap, auto-indent, margins and tabs

The toolkit distinguishes:

1. **Visual word wrap:** presentation only; never changes document text.
2. **Reformat Paragraph:** historical text-changing paragraph layout based on margins.

| Function | Implementation |
|---|---|
| Visual word wrap | `ViewLayoutService` generates visual fragments based on view width and wrap setting |
| Auto-indent | `IAutoIndentPolicy` determines indentation for `NewLine` |
| Left/right margins | Values in `EditorViewState`; validate `0 <= left < right` |
| Reformat | Determine paragraph boundaries, normalize spaces, wrap words between margins, produce undo transaction |
| Center line | Add leading spaces within margins; process tabs first according to policy |
| Tab | `TabSettings`, default width 4; configurable InsertTab or MoveToNextTabStop |
| Wrapped annotation | Optional `TextAnnotation` for historical import/export, not regular visual wrapping |

## 18. Search, replacement and markers

### 18.1 Search model

```csharp
public sealed record SearchOptions(
    bool MatchCase,
    bool WholeWord,
    SearchDirection Direction,
    SearchScope Scope,
    bool UseRegularExpressions);

public sealed record SearchMatch(TextRange Range, string Value);
public sealed record ReplaceResult(int MatchCount, int ReplacementCount, bool WasCancelled);
```

M1 provides literal search, forward direction, MatchCase, selection/document scopes, Find Next, Replace and Replace All. M2 adds backward search, WholeWord, persisted options and interactive replacement strategies. Regular expressions belong to M3.

### 18.2 Cancellation and results

Long searches/replacements periodically check `CancellationToken`. “Not found”, “cancelled” and “error” are distinct result states. The UI decides how messages are presented.

### 18.3 Markers

Markers use `TextAnchor`. The compatibility profile offers at least 20 numbered markers, while the internal model has no fixed upper bound. Markers have an Affinity that determines whether insertion at precisely the marker's position is placed before or after it.

## 19. File, stream and encoding design

### 19.1 Contracts

```csharp
public interface IDocumentStorage
{
    Task<DocumentLoadResult> LoadAsync(
        Stream source,
        DocumentLoadOptions options,
        CancellationToken cancellationToken);

    Task<DocumentSaveResult> SaveAsync(
        TextDocument document,
        Stream destination,
        DocumentSaveOptions options,
        CancellationToken cancellationToken);
}

public interface IFileDocumentStorage : IDocumentStorage
{
    Task<DocumentLoadResult> LoadFileAsync(string path, DocumentLoadOptions options, CancellationToken token);
    Task<DocumentSaveResult> SaveFileAsync(TextDocument document, string path, DocumentSaveOptions options, CancellationToken token);
}
```

### 19.2 Encoding detection

- Detect UTF-8, UTF-16 LE/BE and UTF-32 by BOM.
- Without BOM, favor strictly valid UTF-8.
- Hosts may configure a fallback encoding; without one, do not enforce an unjustified silent conversion.
- NUL bytes, unusually many control characters or decoding failures raise binary-content suspicion. Host receives `BinaryContentSuspected` and must explicitly accept or reject opening.

### 19.3 Atomic save

For filesystem paths, write and flush a temporary file in the destination directory before replacing/moving it into place. Update metadata and savepoint only after success. On failure, retain document content and, where possible, original file.

### 19.4 External modifications

`DocumentFileFingerprint` tracks path, length, LastWriteTimeUtc and optionally a fast hash. A conflict may be reported before overwriting. The host chooses `Overwrite`, `SaveAs`, `Reload` or `Cancel`.

## 20. WinForms adapter

### 20.1 Component structure

```text
SasdEditorView : UserControl
├── EditorSurface : Control
├── VScrollBar
└── HScrollBar

EditorWorkspaceControl : UserControl (Sample/optional reusable host)
├── TabControl / Document selector
└── SplitContainer(s) with SasdEditorView instances
```

`SasdEditorView` binds precisely one `EditorViewState`. `EditorSurface` paints text, selection, caret, search matches, markers, optional control characters and margins.

### 20.2 Rendering pipeline

1. Determine viewport and visible logical lines.
2. Validate layout cache using document version, width, font, DPI, tabs and wrap option.
3. Re-layout only invalidated lines/fragments.
4. Paint background, active line, selection and search highlighting.
5. Paint text fragments.
6. Compute caret position and update native caret or focus-safe rendering.
7. Update scrollbar ranges without recursive layout loops.

### 20.3 Input

- `OnKeyDown` handles non-text gestures and command sequences.
- `OnKeyPress` or text input handles printable characters.
- WinForms adapter encapsulates IME; composed text is written to the document only after commit.
- Mouse sets caret, supports drag selection, double-click word selection and optional drag/drop from M2.
- Clipboard is accessed exclusively through `IClipboardService`.

### 20.4 DPI, fonts and accessibility

- Invalidate layout caches on font, DPI or size changes.
- M1 guarantees monospaced rendering; M2 adds proportional text with glyph-accurate measurement.
- Full keyboard operation is mandatory in M1.
- A dedicated `AccessibleObject` must expose text role, caret, selection and document name by M3 at the latest.

## 21. Modern FIRST-ED reference application

The M1 sample is proof of integration, not a monolithic product.

| UI region | Contents |
|---|---|
| File menu | New, Open, Save, Save As, Close, Exit |
| Edit menu | Undo, Redo, Cut, Copy, Paste, Select All, Find, Replace |
| View menu | Status bar, word-wrap preparation, New/Split View according to milestone |
| Tools menu | Keyboard Profile, Settings |
| Toolbar | New, Open, Save, Undo, Redo, Cut, Copy, Paste, Find |
| Main area | One or more `SasdEditorView` controls |
| Status bar | Filename, dirty indicator, line, column, insert/overwrite, encoding, line endings, profile |
| Dialogs | Open/Save, Find/Replace, Go To Line, close guard, encoding/binary warnings |

From M2, the advanced MicroStar example demonstrates pulldown menus, prefix profiles, several views, block files and replaceable status/error presenters.

## 22. WPF and web preparation

### 22.1 WPF

Future `Sasd.EditorToolkit.Wpf` reimplements rendering, input, clipboard, commands and accessibility while referencing only Core. `EditorViewState`, `TextDocument`, commands, search, undo and file workflows remain the same.

### 22.2 Blazor/Razor

The web adapter uses Core on the server or in WebAssembly depending on host constraints. DOM selection, browser clipboard, IME and rendering belong to the adapter. It triggers shared command IDs.

### 22.3 ASPX/Web Forms

No complete new server-side text control is promised for existing Web Forms applications. Instead document three routes: Core as server-side text service, embedded Blazor component or JavaScript interop with browser adapter.

## 23. Errors, messages and prompts

For expected user/file conditions, Core returns structured results instead of leaking UI-specific exceptions.

```csharp
public sealed record EditorError(
    string Code,
    EditorErrorCategory Category,
    string ResourceKey,
    Exception? Exception = null,
    IReadOnlyDictionary<string, object?>? Context = null);

public readonly record struct EditorResult<T>(T? Value, EditorError? Error)
{
    public bool IsSuccess => Error is null;
}
```

Host services:

- `IEditorErrorPresenter` for messages/logging;
- `IUserPromptService` for confirmations and input;
- `IFileDialogService` for platform-specific file selection;
- `IReplaceDecisionProvider` for interactive replacements;
- `IEditorStatusSink` for status.

These modernize the historical `UserError`, `EditAskfor`, `UserReplace` and `UserStatusLine` hooks.

## 24. Background tasks, autosave and printing

Core has no proprietary scheduler. Long-running operations are standard tasks with CancellationToken; the host owns threading and UI dispatch.

| Service | Contract/implementation | Milestone |
|---|---|---|
| Background runner | `IBackgroundOperationRunner` for progress, cancellation and forwarding errors | M1/M3 |
| Autosave | `IAutosaveService` writes recovery snapshots outside original file path | M3 |
| Recovery | Inspect newer valid snapshots at startup; explicit user choice | M3 |
| Background print | `IPrintService` and incremental sample job | M6 |
| Modem/upload/backup samples | Not an editor function; generic task contract covers the pattern | Not planned |

## 25. Settings and persistence

`EditorSettings` separates global defaults, document metadata and view-specific options.

| Group | Examples |
|---|---|
| General | DefaultEncoding, NewLinePolicy, ConfirmDestructiveOperations |
| Editing | InsertMode, AutoIndent, TabSize, UseTabs, WordBoundaryMode |
| View | Font, Zoom, ShowWhitespace, ShowControlCharacters, ShowLineNumbers, HighlightCurrentLine |
| Formatting | WordWrap, LeftMargin, RightMargin |
| Undo | MaxOperations, MaxEstimatedBytes, TypingCoalescingTimeout |
| Input | KeyboardProfileName, PrefixTimeout |
| Theme | Styles for text, background, selection, status, markers, annotations |

Persistence uses versioned JSON. Unknown fields are tolerated; invalid values are validated, logged and replaced with safe defaults. Historical fixed values only appear in an optional retro profile.

## 26. Embedding and extension contracts

Host can replace at least:

- storage, file dialogs, close prompts;
- clipboard, logging, error display;
- status display and theme;
- word boundaries and auto-indent;
- search/replace decisions;
- autosave, print, background runner;
- custom commands and command interceptors.

Syntax highlighting, Markdown, log viewer, hex viewer and property editor use `TextAnnotation`, commands and host services. They do not belong in the M1 Core and must not introduce cyclic dependencies.

## 27. Logging and diagnostics

Logging is optional and may not force UI output. Categories include:

- opening/saving/closing documents;
- encoding/binary warnings;
- external file conflicts;
- command errors;
- autosave/recovery;
- performance warnings and layout-cache statistics in debug mode.

Document text is not logged by default. Hosts may redact paths. Exceptions are logged only at technical boundaries and then forwarded as structured errors.

## 28. Security and legal approach

- Never automatically run text, macros or scripts.
- Normalize paths; host chooses allowed directories.
- Use predictable host-configurable temporary/recovery locations.
- Saving follows “write successfully first, then mark document clean.”
- Plugins and macros need a future separate authorization model.
- Do not include historical source, logos, covers or scanned handbook pages in the product repository.
- The public name must clearly identify an independent SASD implementation. “Turbo Editor Toolbox” is used only as historical reference in documentation, subject to rights review.

## 29. Performance and resource requirements

The numbers below are technical targets on documented reference hardware. They concretize qualitative requirements and may be adjusted after measurement through an ADR, never silently.

| Scenario | M1 target | M3 target |
|---|---|---|
| Normal input to visible update | p95 < 50 ms at 1 MiB / 50,000 lines | p95 < 30 ms at 10 MiB / 250,000 lines |
| Open UTF-8 | 10 MiB in < 3 s | 100 MiB in < 8 s with large-file buffer |
| Save | 10 MiB in < 3 s | 100 MiB in < 8 s, atomic if filesystem permits |
| Literal search | 10 MiB in < 2 s | 100 MiB in < 5 s |
| Scrolling | No persistent full-surface redraw | Incremental layout and caching |
| Memory | No unbounded leaks; closing releases documents | Measurable/configurable buffer and undo budget |

Test UI performance with realistic typing, scrolling, resizing and multiview workloads. Cancellation of search/file operations must become effective at the next defined checkpoint.

## 30. Internationalization and accessibility

- Messages/UI strings come from RESX resources maintained in German and English.
- Document text is culture-independent; search settings explicitly choose ordinal/cultural behavior, default ordinal comparison with optional case matching.
- Complete keyboard access for core functions.
- Visible focus and selection.
- Document and test theme contrast in the sample.
- WinForms accessibility exposes role, name, document state, caret and selection; advanced screen-reader support belongs to M3.

## 31. Events and threading

| Event | Source | Payload |
|---|---|---|
| `TextChanging` | TextDocument/ITextBuffer | Pre-change; cannot be mutated by UI handlers |
| `TextChanged` | TextDocument/ITextBuffer | TextChangeSet, versions, affected lines |
| `DirtyStateChanged` | TextDocument | Old/new state |
| `MetadataChanged` | TextDocument | Path, encoding, line endings, fingerprint |
| `SelectionChanged` | EditorViewState | Previous/new selection |
| `CaretChanged` | EditorViewState | Previous/new position and visual column |
| `ViewportChanged` | EditorViewState | Scroll/viewport information |
| `ActiveViewChanged` | EditorWorkspace | Previous/new view |
| `CommandExecuted` | Dispatcher | ID, result, duration |
| `StatusChanged` | StatusProvider | Immutable status snapshot |

Core objects are not generally thread-safe. Document mutations run serially in the owning/host context. Async I/O/search may run in the background but results are applied to a controlled current document version. Stale results are discarded or reported as conflicts.

## 32. Use-case realization

### UC-01 — New document

| Aspect | Specification |
|---|---|
| Goal | Create an empty document, type text and save it. |
| Precondition | No existing document required. |
| Main flow | Workspace creates document/view; typing generates undo transactions; SaveAs invokes FileDocumentStorage. |
| Postcondition | Saved clean state and updated metadata. |
| Error/alternative | Operation is cancelled or returned to host as a structured error; existing text remains consistent. |

### UC-02 — Edit file

| Aspect | Specification |
|---|---|
| Goal | Open, modify and safely save a file. |
| Precondition | Readable path/stream. |
| Main flow | Storage loads encoding/line endings; view binds; edits; atomic save. |
| Postcondition | Updated file or clear failure without losing content. |
| Error/alternative | Operation is cancelled or returned to host as a structured error; existing text remains consistent. |

### UC-03 — Embed editor

| Aspect | Specification |
|---|---|
| Goal | Embed WinForms control in a business application and provide host services. |
| Precondition | Host references Core and WinForms. |
| Main flow | Host registers services/commands, creates workspace and SasdEditorView. |
| Postcondition | Editor runs without duplicate domain logic. |
| Error/alternative | Operation is cancelled or returned to host as a structured error; existing text remains consistent. |

### UC-04 — Compare several files

| Aspect | Specification |
|---|---|
| Goal | Display multiple document views at the same time. |
| Precondition | At least two documents. |
| Main flow | Workspace manages multiple documents/views; host uses tabs or splits. |
| Postcondition | Separate content and state. |
| Error/alternative | Operation is cancelled or returned to host as a structured error; existing text remains consistent. |

### UC-05 — Split same document

| Aspect | Specification |
|---|---|
| Goal | Show different locations of one document in two views. |
| Precondition | One open document. |
| Main flow | Workspace creates second ViewState pointing to the same document. |
| Postcondition | Shared content, independent caret/scroll. |
| Error/alternative | Operation is cancelled or returned to host as a structured error; existing text remains consistent. |

### UC-06 — Edit block

| Aspect | Specification |
|---|---|
| Goal | Copy, move or delete selected text. |
| Precondition | Nonempty selection. |
| Main flow | Copy/Move/Delete via SelectionService and UndoTransaction. |
| Postcondition | Deterministic selection and undo/redo. |
| Error/alternative | Operation is cancelled or returned to host as a structured error; existing text remains consistent. |

### UC-07 — Find and replace

| Aspect | Specification |
|---|---|
| Goal | Find matches, navigate and replace individually or in bulk. |
| Precondition | Search pattern. |
| Main flow | SearchService returns matches; Replace/ReplaceAll creates atomic changes. |
| Postcondition | Match state or NotFound/Cancelled. |
| Error/alternative | Operation is cancelled or returned to host as a structured error; existing text remains consistent. |

### UC-08 — Undo/redo

| Aspect | Specification |
|---|---|
| Goal | Undo and redo multiple edits. |
| Precondition | At least one edit. |
| Main flow | UndoManager applies inverse/repeated ChangeSets. |
| Postcondition | Consistent document, markers and views. |
| Error/alternative | Operation is cancelled or returned to host as a structured error; existing text remains consistent. |

### UC-09 — Historical keyboard profile

| Aspect | Specification |
|---|---|
| Goal | Activate Turbo/WordStar prefix commands. |
| Precondition | Compatibility profile installed. |
| Main flow | Load JSON profile; SequenceResolver handles Ctrl-K/O/Q chords. |
| Postcondition | Historical interaction without recompiling Core. |
| Error/alternative | Operation is cancelled or returned to host as a structured error; existing text remains consistent. |

### UC-10 — Register custom command

| Aspect | Specification |
|---|---|
| Goal | Register host-specific command for menu, toolbar and key. |
| Precondition | Host command implemented. |
| Main flow | Registry registers command and binding; menu/key share its ID. |
| Postcondition | Host feature accessible through all input paths. |
| Error/alternative | Operation is cancelled or returned to host as a structured error; existing text remains consistent. |

### UC-11 — Protect unsaved changes

| Aspect | Specification |
|---|---|
| Goal | Require Save/Discard/Cancel on close/open when necessary. |
| Precondition | Document is dirty. |
| Main flow | CloseGuard returns DecisionRequired; host prompts; Save/Discard/Cancel. |
| Postcondition | No unintentional loss. |
| Error/alternative | Operation is cancelled or returned to host as a structured error; existing text remains consistent. |

### UC-12 — Large file

| Aspect | Specification |
|---|---|
| Goal | Open large file responsively and allow cancellation. |
| Precondition | Large file. |
| Main flow | BufferFactory and Storage stream/load; report progress/cancellation. |
| Postcondition | Responsive UI; cancellation leaves no half-built document. |
| Error/alternative | Operation is cancelled or returned to host as a structured error; existing text remains consistent. |

### UC-13 — Autosave

| Aspect | Specification |
|---|---|
| Goal | Host saves recovery snapshots in background. |
| Precondition | Autosave enabled. |
| Main flow | AutosaveService writes versioned snapshot; recovery check at startup. |
| Postcondition | Recoverable state without overwriting original. |
| Error/alternative | Operation is cancelled or returned to host as a structured error; existing text remains consistent. |

### UC-14 — Build another UI adapter

| Aspect | Specification |
|---|---|
| Goal | WPF or web adapter uses the same Core. |
| Precondition | Core API stable. |
| Main flow | New adapter implements rendering/input/host services. |
| Postcondition | No duplicate functional implementation. |
| Error/alternative | Operation is cancelled or returned to host as a structured error; existing text remains consistent. |

### UC-15 — Desktop Components showcase

| Aspect | Specification |
|---|---|
| Goal | Components project demonstrates editor, Markdown or log viewer. |
| Precondition | Editor packages available. |
| Main flow | Desktop Components references packages and builds specialized controls. |
| Postcondition | No cyclic dependency or copied source. |
| Error/alternative | Operation is cancelled or returned to host as a structured error; existing text remains consistent. |

## 33. Test concept

### 33.1 Test pyramid

| Level | Goal | Examples |
|---|---|---|
| Unit | Deterministic Core logic | Buffer, positions, commands, undo, search, markers, settings |
| Integration | Streams/filesystem/workspace interaction | Encoding, atomic save, external edits, multiple views |
| WinForms | Rendering/input integration | Caret, selection, scrolling, DPI, key sequences, clipboard |
| Architecture | Enforce dependency rules | Core without UI references; no cycles; no core logic in samples |
| Performance | Measurable targets | Open/save/search/input/scroll/memory |
| Manual exploratory | UX and accessibility | IME, screen reader, high DPI, dialog flows |

### 33.2 Mandatory tests

| Test ID | Test case |
|---|---|
| T-CORE-001 | Insert/delete across line boundaries and in empty document |
| T-CORE-002 | Load/save CRLF/LF/CR and mixed line endings |
| T-CORE-003 | Unicode, surrogate pairs, combining characters and emoji caret stops |
| T-UNDO-001 | Undo/redo typing, paste, ReplaceAll, block move and reformat |
| T-UNDO-002 | Savepoint/dirty state before/after save and undo back to savepoint |
| T-VIEW-001 | Two views share text but not caret/scroll state |
| T-SEARCH-001 | Find/Replace/ReplaceAll, MatchCase, NotFound, cancellation |
| T-FILE-001 | Failed save leaves document and existing file consistent |
| T-FILE-002 | BOM/UTF-8 detection and binary warning |
| T-CMD-001 | CommandInterceptor blocks, replaces and handles commands |
| T-INPUT-001 | Modern and Turbo-compatible key profiles |
| T-UI-001 | Resize/DPI/font correctly invalidates layout |
| T-UI-002 | Clipboard with multiline Unicode |
| T-ARCH-001 | Core assembly does not reference UI framework assembly |
| T-PERF-001 | M1 target file meets open/save/search/input budgets |

## 34. Build, CI, packages and documentation

### 34.1 Build rules

- `Nullable=enable`, `ImplicitUsings=enable`, deterministic build and SourceLink.
- Treat warnings as errors in CI; document justified exceptions centrally.
- Package versions follow semantic versioning.
- Check public APIs against an API baseline.
- Release build produces symbol packages and XML documentation.

### 34.2 CI pipeline

```text
restore
-> build Debug
-> unit tests
-> architecture tests
-> integration tests
-> build Release
-> pack
-> documentation/link checks
-> optional performance smoke tests
-> publish artifacts (release workflow only)
```

### 34.3 NuGet packages

| Package | Contents |
|---|---|
| `Sasd.EditorToolkit.Core` | UI-independent editor library |
| `Sasd.EditorToolkit.WinForms` | WinForms control, renderer and platform services |

Samples are not distributed as runtime packages. WPF/web get separate packages later.

## 35. Milestones and deliverables

| Milestone | Goal | Deliverables | Acceptance |
|---|---|---|---|
| M0 | Specification/technical baseline | Requirements and implementation specs, ADRs, name/license review, solution skeleton, CI, renderer spike | Approved architecture and green skeleton build |
| M1 / 0.1 | Modern FIRST-ED | Core, LineTextBuffer, basic editing/navigation, file, undo/redo, search, commands, WinForms, sample, tests | AK-001–003, AK-008–012, key parts of AK-013/014 |
| M2 / 0.5 | Historical breadth | Word wrap/reformat, auto-indent, margins, tabs, markers, blocks, views, profiles, themes, hooks | AK-004–007 and complete AK-013 |
| M3 / 1.0 | Stabilization | Piece Table option, performance, accessibility, NuGet, API baseline, full DE/EN docs, traceability | All AK including AK-015 |
| M4 | WPF | WPF adapter and sample | Unchanged Core; adapter acceptance tests |
| M5 | Web | Blazor/Razor and documented ASPX route | Browser integration without Core coupling |
| M6 | Extensions | Syntax, Markdown, log/hex viewers, macros, print, language services | Separate extension acceptance |

## 36. Definition of done

A feature is finished when:

- associated requirements IDs are cited in the pull request;
- implementation and public API are documented;
- unit/integration tests exist and are green;
- error, cancellation and edge cases are addressed;
- no prohibited dependency was introduced;
- relevant German/English docs were updated;
- affected historical traceability was updated;
- performance/security impact was assessed;
- code review according to SASD Development Standard completed.

A milestone additionally requires all assigned acceptance criteria, generated NuGet/sample artifacts and release notes documenting known limitations.

## 37. Open decisions with proposed defaults

| Topic | Proposed default | Decision point |
|---|---|---|
| Public product name | SASD Editor Toolkit; historical name explanatory only | M0 |
| License | MIT for newly written code, subject to Product Owner approval | M0 |
| Renderer | Custom WinForms EditorSurface | M0 ADR |
| M1 buffer | LineTextBuffer | M0 ADR |
| M3 large-file buffer | Piece Table | Before M3 |
| Default encoding for new files | UTF-8 without BOM, host-configurable | M1 |
| Default line ending | Platform/host default; preserve loaded endings | M1 |
| Default tab width | 4 | M1 |
| Default WinForms font | Monospace system/Cascadia Mono fallback; don't bundle font file | M1 |
| Regex | M3, with timeout and cancellation | M3 |
| Autosave directory | Host-configured LocalAppData subfolder | M3 |


## 38. Complete traceability of source requirements

The following matrix covers all **208** formal source requirements. Requirement statements are taken from the full English translation of the source requirements; IDs, priorities, milestones and implementation mappings retain their original meaning. This is the *planned implementation*, not evidence of completed features.

| ID | Priority | Source requirement | Implementation spec | Component | Milestone | Implementation approach | Verification |
|---|---|---|---|---|---|---|---|
| PROD-001 | MUST | The product must be supplied as a reusable editor construction kit, not only as a finished application. | Chapter 4-8 | Repository, packaging and product architecture | M1 | Implemented by Repository, packaging and product architecture as defined in chapter 4-8 | Architecture test, package/repository review and integration sample |
| PROD-002 | MUST | The toolkit must support simple editors, sophisticated editors and embedded text editing in business applications. | Chapter 4-8 | Repository, packaging and product architecture | M1 | Implemented by Repository, packaging and product architecture as defined in chapter 4-8 | Architecture test, package/repository review and integration sample |
| PROD-003 | MUST | The functional core must be independent of WinForms, WPF, ASP.NET and browser technologies. | Chapter 4-8 | Repository, packaging and product architecture | M4 | SasdEditorView/UserControl with its own EditorSurface; separate WPF adapter using unchanged Core; separate web adapter; Core stays DOM- and browser-independent | Architecture test, package/repository review and integration sample |
| PROD-004 | MUST | WinForms is the first production-capable reference interface. | Chapter 4-8 | Repository, packaging and product architecture | M1 | SasdEditorView/UserControl with its own EditorSurface | Architecture test, package/repository review and integration sample |
| PROD-005 | MUST | WPF must be able to reuse the same core later without reimplementing behavior. | Chapter 4-8 | Repository, packaging and product architecture | M4 | separate WPF adapter using unchanged Core | Architecture test, package/repository review and integration sample |
| PROD-006 | SHOULD | The core must be usable from ASP.NET-based hosts; modern interactive web integration should preferably use Blazor or Razor. | Chapter 4-8 | Repository, packaging and product architecture | M5 | separate web adapter; Core stays DOM- and browser-independent | Architecture test, package/repository review and integration sample |
| PROD-007 | MUST | SASD Editor Toolkit is maintained as its own repository, package and release unit. | Chapter 4-8 | Repository, packaging and product architecture | M0/M1 | Implemented by Repository, packaging and product architecture as defined in chapter 4-8 | Architecture test, package/repository review and integration sample |
| PROD-008 | MUST | SASD Desktop Components may use and demonstrate the editor but must not own the editor core. | Chapter 4-8 | Repository, packaging and product architecture | M0/M1 | Implemented by Repository, packaging and product architecture as defined in chapter 4-8 | Architecture test, package/repository review and integration sample |
| PROD-009 | MUST | Editor Toolkit must remain independently versionable from Data Toolbox, Numerics, Graphics and GameWorks. | Chapter 4-8 | Repository, packaging and product architecture | M0/M1 | Implemented by Repository, packaging and product architecture as defined in chapter 4-8 | Architecture test, package/repository review and integration sample |
| PROD-010 | MUST | Modernize historical behavior functionally; do not port DOS, Turbo Pascal, overlays or video-memory mechanisms one-to-one. | Chapter 4-8 | Repository, packaging and product architecture | M0/M1 | IDocumentStorage/FileDocumentStorage with atomic saving | Architecture test, package/repository review and integration sample |
| PROD-011 | SHOULD | Code, public APIs and package names should be English; documentation should be available in German and English. | Chapter 4-8 | Repository, packaging and product architecture | M1 | RESX resources and bilingual documentation | Architecture test, package/repository review and integration sample |
| PROD-012 | MUST | Libraries, UI components and runnable samples must be provided together. | Chapter 4-8 | Repository, packaging and product architecture | M1 | Implemented by Repository, packaging and product architecture as defined in chapter 4-8 | Architecture test, package/repository review and integration sample |
| CORE-001 | MUST | Model text as characters, words, lines, streams, views, blocks/selections and files. | Chapter 9-12 | Sasd.EditorToolkit.Core / document and text model | M1 | IDocumentStorage/FileDocumentStorage with atomic saving | Unit tests, supplemented by integration/acceptance tests |
| CORE-002 | MUST | Keep document content separate from view state. | Chapter 9-12 | Sasd.EditorToolkit.Core / document and text model | M1 | Implemented by Sasd.EditorToolkit.Core / document and text model as defined in chapter 9-12 | Unit tests, supplemented by integration/acceptance tests |
| CORE-003 | MUST | Allow several views to display and edit one document simultaneously. | Chapter 9-12 | Sasd.EditorToolkit.Core / document and text model | M1 | shared TextDocument with several independent EditorViewState instances | Unit tests, supplemented by integration/acceptance tests |
| CORE-004 | MUST | Document supports text, optional path, display name, encoding, line endings, dirty state, markers and undo/redo. | Chapter 9-12 | Sasd.EditorToolkit.Core / document and text model | M1 | LineEndingKind per line with configurable save policy; Savepoint in UndoManager and CloseGuard workflow; transactional UndoManager with coalescing and savepoint; TextAnchor-based MarkerCollection with affinity | Unit tests, supplemented by integration/acceptance tests |
| CORE-005 | MUST | View holds caret, selection, scroll position, insert/overwrite, word wrap, auto-indent, margins and tabs. | Chapter 9-12 | Sasd.EditorToolkit.Core / document and text model | M1 | ViewLayoutService with optional ReformatParagraphCommand; AutoIndentPolicy in NewLineCommand; TabSettings and VisualColumnCalculator | Unit tests, supplemented by integration/acceptance tests |
| CORE-006 | MUST | Encapsulate text buffer behind a replaceable abstraction. | Chapter 9-12 | Sasd.EditorToolkit.Core / document and text model | M1 | Implemented by Sasd.EditorToolkit.Core / document and text model as defined in chapter 9-12 | Unit tests, supplemented by integration/acceptance tests |
| CORE-007 | SHOULD | First implementation may be line-oriented but should allow later large-file engines. | Chapter 9-12 | Sasd.EditorToolkit.Core / document and text model | M1 | Implemented by Sasd.EditorToolkit.Core / document and text model as defined in chapter 9-12 | Unit tests, supplemented by integration/acceptance tests |
| CORE-008 | MUST | Lines must not have a 255-character limit. | Chapter 9-12 | Sasd.EditorToolkit.Core / document and text model | M1 | no fixed line-length limit; only resource limits | Unit tests, supplemented by integration/acceptance tests |
| CORE-009 | MUST | Internal text representation must support Unicode. | Chapter 9-12 | Sasd.EditorToolkit.Core / document and text model | M1 | UTF-16 .NET strings, Unicode-safe caret stops and explicit encoding metadata | Unit tests, supplemented by integration/acceptance tests |
| CORE-010 | MUST | Detect, preserve and convert CRLF, LF and other supported line endings. | Chapter 9-12 | Sasd.EditorToolkit.Core / document and text model | M1 | LineEndingKind per line with configurable save policy | Unit tests, supplemented by integration/acceptance tests |
| CORE-011 | MUST | Support empty and unnamed documents. | Chapter 9-12 | Sasd.EditorToolkit.Core / document and text model | M1 | Implemented by Sasd.EditorToolkit.Core / document and text model as defined in chapter 9-12 | Unit tests, supplemented by integration/acceptance tests |
| CORE-012 | MUST | Represent changes as atomic and traceable operations. | Chapter 9-12 | Sasd.EditorToolkit.Core / document and text model | M1 | Implemented by Sasd.EditorToolkit.Core / document and text model as defined in chapter 9-12 | Unit tests, supplemented by integration/acceptance tests |
| CORE-013 | MUST | Core must be testable without a UI thread or UI framework. | Chapter 9-12 | Sasd.EditorToolkit.Core / document and text model | M1 | Implemented by Sasd.EditorToolkit.Core / document and text model as defined in chapter 9-12 | Unit tests, supplemented by integration/acceptance tests |
| CORE-014 | SHOULD | Expose document/non-document editing modes as configurable options. | Chapter 9-12 | Sasd.EditorToolkit.Core / document and text model | M1 | Implemented by Sasd.EditorToolkit.Core / document and text model as defined in chapter 9-12 | Unit tests, supplemented by integration/acceptance tests |
| CORE-015 | SHOULD | Preserve control characters and optionally display them visibly. | Chapter 9-12 | Sasd.EditorToolkit.Core / document and text model | M1 | IDocumentStorage/FileDocumentStorage with atomic saving | Unit tests, supplemented by integration/acceptance tests |
| CORE-016 | SHOULD | Lines/ranges may carry extensible annotations for wrap, selection, marking and specialized display. | Chapter 9-12 | Sasd.EditorToolkit.Core / document and text model | M1 | Implemented by Sasd.EditorToolkit.Core / document and text model as defined in chapter 9-12 | Unit tests, supplemented by integration/acceptance tests |
| CORE-017 | MUST | Positions/ranges must be unambiguous, validatable and deterministic under edits. | Chapter 9-12 | Sasd.EditorToolkit.Core / document and text model | M1 | Implemented by Sasd.EditorToolkit.Core / document and text model as defined in chapter 9-12 | Unit tests, supplemented by integration/acceptance tests |
| NAV-001 | MUST | Move caret one character left or right. | Chapter 15 | NavigationCommandHandler | M1 | Implemented by NavigationCommandHandler as defined in chapter 15 | Unit tests, supplemented by integration/acceptance tests |
| NAV-002 | MUST | Move caret one line up or down. | Chapter 15 | NavigationCommandHandler | M1 | Implemented by NavigationCommandHandler as defined in chapter 15 | Unit tests, supplemented by integration/acceptance tests |
| NAV-003 | MUST | Scroll view one line up or down. | Chapter 15 | NavigationCommandHandler | M1 | Implemented by NavigationCommandHandler as defined in chapter 15 | Unit tests, supplemented by integration/acceptance tests |
| NAV-004 | MUST | Navigate one page up or down. | Chapter 15 | NavigationCommandHandler | M1 | Implemented by NavigationCommandHandler as defined in chapter 15 | Unit tests, supplemented by integration/acceptance tests |
| NAV-005 | MUST | Navigate one word left or right. | Chapter 15 | NavigationCommandHandler | M1 | Implemented by NavigationCommandHandler as defined in chapter 15 | Unit tests, supplemented by integration/acceptance tests |
| NAV-006 | MUST | Jump to line start/end; optionally toggle between these positions. | Chapter 15 | NavigationCommandHandler | M1 | LineEndingKind per line with configurable save policy; EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | Unit tests, supplemented by integration/acceptance tests |
| NAV-007 | MUST | Jump to document start/end. | Chapter 15 | NavigationCommandHandler | M1 | Implemented by NavigationCommandHandler as defined in chapter 15 | Unit tests, supplemented by integration/acceptance tests |
| NAV-008 | MUST | Jump to start/end of selection or block. | Chapter 15 | NavigationCommandHandler | M1 | Implemented by NavigationCommandHandler as defined in chapter 15 | Unit tests, supplemented by integration/acceptance tests |
| NAV-009 | MUST | Jump to a specified line or column. | Chapter 15 | NavigationCommandHandler | M1 | Implemented by NavigationCommandHandler as defined in chapter 15 | Unit tests, supplemented by integration/acceptance tests |
| NAV-010 | MUST | Support horizontal scrolling for wide lines. | Chapter 15 | NavigationCommandHandler | M1 | Implemented by NavigationCommandHandler as defined in chapter 15 | Unit tests, supplemented by integration/acceptance tests |
| NAV-011 | SHOULD | Navigation should optionally extend a selection. | Chapter 15 | NavigationCommandHandler | M1 | Implemented by NavigationCommandHandler as defined in chapter 15 | Unit tests, supplemented by integration/acceptance tests |
| NAV-012 | SHOULD | Retain desired visual column during vertical caret movement where possible. | Chapter 15 | NavigationCommandHandler | M1 | Implemented by NavigationCommandHandler as defined in chapter 15 | Unit tests, supplemented by integration/acceptance tests |
| EDIT-001 | MUST | Normal typing must insert text in insert mode and replace text in overwrite mode. | Chapter 14-15 | EditingCommandHandler and ITextBuffer | M1 | TextSearchService with SearchOptions, CancellationToken and SearchResult | Unit tests, supplemented by integration/acceptance tests |
| EDIT-002 | MUST | Allow switching between insert and overwrite modes. | Chapter 14-15 | EditingCommandHandler and ITextBuffer | M1 | Implemented by EditingCommandHandler and ITextBuffer as defined in chapter 14-15 | Unit tests, supplemented by integration/acceptance tests |
| EDIT-003 | MUST | Support inserting new and empty lines. | Chapter 14-15 | EditingCommandHandler and ITextBuffer | M1 | Implemented by EditingCommandHandler and ITextBuffer as defined in chapter 14-15 | Unit tests, supplemented by integration/acceptance tests |
| EDIT-004 | MUST | Delete characters at/right of the caret and to the left of it. | Chapter 14-15 | EditingCommandHandler and ITextBuffer | M1 | Implemented by EditingCommandHandler and ITextBuffer as defined in chapter 14-15 | Unit tests, supplemented by integration/acceptance tests |
| EDIT-005 | MUST | Delete the current line, next word and text to the end of the line. | Chapter 14-15 | EditingCommandHandler and ITextBuffer | M1 | LineEndingKind per line with configurable save policy | Unit tests, supplemented by integration/acceptance tests |
| EDIT-006 | SHOULD | Deleting the entire document should require an explicitly confirmable action. | Chapter 14-15 | EditingCommandHandler and ITextBuffer | M1 | Implemented by EditingCommandHandler and ITextBuffer as defined in chapter 14-15 | Unit tests, supplemented by integration/acceptance tests |
| EDIT-007 | MUST | Allow lines to be split, joined, shortened, extended, compressed and shifted as required by internal or public commands. | Chapter 14-15 | EditingCommandHandler and ITextBuffer | M1 | EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | Unit tests, supplemented by integration/acceptance tests |
| EDIT-008 | MUST | Change the letter case of a character or selected text. | Chapter 14-15 | EditingCommandHandler and ITextBuffer | M1 | Implemented by EditingCommandHandler and ITextBuffer as defined in chapter 14-15 | Unit tests, supplemented by integration/acceptance tests |
| EDIT-009 | SHOULD | Permit explicit insertion of control characters. | Chapter 14-15 | EditingCommandHandler and ITextBuffer | M1 | EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | Unit tests, supplemented by integration/acceptance tests |
| EDIT-010 | MUST | Support cut, copy and paste using the platform clipboard. | Chapter 14-15 | EditingCommandHandler and ITextBuffer | M1 | Implemented by EditingCommandHandler and ITextBuffer as defined in chapter 14-15 | Unit tests, supplemented by integration/acceptance tests |
| EDIT-011 | MUST | Correctly process multiline Unicode clipboard text. | Chapter 14-15 | EditingCommandHandler and ITextBuffer | M1 | UTF-16 .NET strings, Unicode-safe caret stops and explicit encoding metadata | Unit tests, supplemented by integration/acceptance tests |
| EDIT-012 | MUST | Editing operations must be atomic and able to check preconditions before execution. | Chapter 14-15 | EditingCommandHandler and ITextBuffer | M1 | Implemented by EditingCommandHandler and ITextBuffer as defined in chapter 14-15 | Unit tests, supplemented by integration/acceptance tests |
| WP-001 | MUST | Enable or disable word wrap per view. | Chapter 16 | TextFormattingService and EditorViewState | M2 | ViewLayoutService with optional ReformatParagraphCommand | Unit tests, supplemented by integration/acceptance tests |
| WP-002 | MUST | Word wrap and paragraph formatting must respect left and right margins. | Chapter 16 | TextFormattingService and EditorViewState | M2 | ViewLayoutService with optional ReformatParagraphCommand | Unit tests, supplemented by integration/acceptance tests |
| WP-003 | MUST | Support paragraph reformatting. | Chapter 16 | TextFormattingService and EditorViewState | M2 | Implemented by TextFormattingService and EditorViewState as defined in chapter 16 | Unit tests, supplemented by integration/acceptance tests |
| WP-004 | MUST | Auto-indent must be configurable per view and carry over the previous line's indentation. | Chapter 16 | TextFormattingService and EditorViewState | M2 | AutoIndentPolicy in NewLineCommand | Unit tests, supplemented by integration/acceptance tests |
| WP-005 | MUST | Allow setting the left and right margins. | Chapter 16 | TextFormattingService and EditorViewState | M2 | Implemented by TextFormattingService and EditorViewState as defined in chapter 16 | Unit tests, supplemented by integration/acceptance tests |
| WP-006 | MUST | Center a line between the active margins. | Chapter 16 | TextFormattingService and EditorViewState | M2 | Implemented by TextFormattingService and EditorViewState as defined in chapter 16 | Unit tests, supplemented by integration/acceptance tests |
| WP-007 | MUST | Support tab characters and configurable tab width. | Chapter 16 | TextFormattingService and EditorViewState | M2 | TabSettings and VisualColumnCalculator | Unit tests, supplemented by integration/acceptance tests |
| WP-008 | SHOULD | Distinguish actual tab characters from visual tab navigation. | Chapter 16 | TextFormattingService and EditorViewState | M2 | TabSettings and VisualColumnCalculator | Unit tests, supplemented by integration/acceptance tests |
| WP-009 | SHOULD | Optionally annotate automatically wrapped lines. | Chapter 16 | TextFormattingService and EditorViewState | M2 | Implemented by TextFormattingService and EditorViewState as defined in chapter 16 | Unit tests, supplemented by integration/acceptance tests |
| WP-010 | MAY | Historical WordStar wrap markers may only be exposed via an explicit compatibility import/export mode. | Chapter 16 | TextFormattingService and EditorViewState | M2 | Implemented by TextFormattingService and EditorViewState as defined in chapter 16 | Unit tests, supplemented by integration/acceptance tests |
| WP-011 | SHOULD | Word boundaries and separators must be extensible and Unicode-aware. | Chapter 16 | TextFormattingService and EditorViewState | M2 | UTF-16 .NET strings, Unicode-safe caret stops and explicit encoding metadata | Unit tests, supplemented by integration/acceptance tests |
| SEL-001 | MUST | Mark the start and end of a selection or historical block. | Chapter 13 and 17 | TextSelection, clipboard and block services | M1 | Implemented by TextSelection, clipboard and block services as defined in chapter 13 and 17 | Unit tests, supplemented by integration/acceptance tests |
| SEL-002 | MUST | Copy, move and delete selected text. | Chapter 13 and 17 | TextSelection, clipboard and block services | M1 | Implemented by TextSelection, clipboard and block services as defined in chapter 13 and 17 | Unit tests, supplemented by integration/acceptance tests |
| SEL-003 | SHOULD | Allow hiding selection highlighting without necessarily discarding the logical range. | Chapter 13 and 17 | TextSelection, clipboard and block services | M1 | Implemented by TextSelection, clipboard and block services as defined in chapter 13 and 17 | Unit tests, supplemented by integration/acceptance tests |
| SEL-004 | SHOULD | Support operations between views/documents where semantically valid. | Chapter 13 and 17 | TextSelection, clipboard and block services | M1 | Implemented by TextSelection, clipboard and block services as defined in chapter 13 and 17 | Unit tests, supplemented by integration/acceptance tests |
| SEL-005 | MUST | Default modern selection must work at character granularity across lines. | Chapter 13 and 17 | TextSelection, clipboard and block services | M1 | Implemented by TextSelection, clipboard and block services as defined in chapter 13 and 17 | Unit tests, supplemented by integration/acceptance tests |
| SEL-006 | MAY | A historical line-oriented block mode may be offered as a compatibility profile. | Chapter 13 and 17 | TextSelection, clipboard and block services | M2 | Implemented by TextSelection, clipboard and block services as defined in chapter 13 and 17 | Unit tests, supplemented by integration/acceptance tests |
| SEL-007 | MUST | Support selecting the entire document. | Chapter 13 and 17 | TextSelection, clipboard and block services | M2 | Implemented by TextSelection, clipboard and block services as defined in chapter 13 and 17 | Unit tests, supplemented by integration/acceptance tests |
| SEL-008 | MUST | Update selection positions deterministically after edits. | Chapter 13 and 17 | TextSelection, clipboard and block services | M2 | Implemented by TextSelection, clipboard and block services as defined in chapter 13 and 17 | Unit tests, supplemented by integration/acceptance tests |
| SRCH-001 | MUST | Support text search, find next and find/replace. | Chapter 18 | TextSearchService and MarkerCollection | M1 | TextSearchService with SearchOptions, CancellationToken and SearchResult | Unit tests, supplemented by integration/acceptance tests |
| SRCH-002 | SHOULD | Support replace all. | Chapter 18 | TextSearchService and MarkerCollection | M1 | TextSearchService with SearchOptions, CancellationToken and SearchResult | Unit tests, supplemented by integration/acceptance tests |
| SRCH-003 | SHOULD | Search options should include at least case sensitivity, direction and whole word. | Chapter 18 | TextSearchService and MarkerCollection | M1 | TextSearchService with SearchOptions, CancellationToken and SearchResult | Unit tests, supplemented by integration/acceptance tests |
| SRCH-004 | MAY | Regular expressions may be offered optionally. | Chapter 18 | TextSearchService and MarkerCollection | M1 | Implemented by TextSearchService and MarkerCollection as defined in chapter 18 | Unit tests, supplemented by integration/acceptance tests |
| SRCH-005 | SHOULD | Search within document, selection or a defined range. | Chapter 18 | TextSearchService and MarkerCollection | M2/M3 | TextSearchService with SearchOptions, CancellationToken and SearchResult | Unit tests, supplemented by integration/acceptance tests |
| SRCH-006 | MUST | Search and replacement operations must be cancellable. | Chapter 18 | TextSearchService and MarkerCollection | M2/M3 | TextSearchService with SearchOptions, CancellationToken and SearchResult | Unit tests, supplemented by integration/acceptance tests |
| SRCH-007 | SHOULD | Preserve recent search/replacement strings and options where useful. | Chapter 18 | TextSearchService and MarkerCollection | M2/M3 | TextSearchService with SearchOptions, CancellationToken and SearchResult | Unit tests, supplemented by integration/acceptance tests |
| SRCH-008 | MUST | Report a missing match as an unambiguous result state. | Chapter 18 | TextSearchService and MarkerCollection | M2/M3 | Implemented by TextSearchService and MarkerCollection as defined in chapter 18 | Unit tests, supplemented by integration/acceptance tests |
| SRCH-009 | MUST | Allow setting, jumping to and choosing markers via a prompt. | Chapter 18 | TextSearchService and MarkerCollection | M2/M3 | TextAnchor-based MarkerCollection with affinity | Unit tests, supplemented by integration/acceptance tests |
| SRCH-010 | SHOULD | Compatibility should support at least 20 markers; internal capacity may be greater. | Chapter 18 | TextSearchService and MarkerCollection | M2/M3 | TextAnchor-based MarkerCollection with affinity | Unit tests, supplemented by integration/acceptance tests |
| SRCH-011 | MUST | Markers must belong to documents and track positions reliably through edits. | Chapter 18 | TextSearchService and MarkerCollection | M2/M3 | TextAnchor-based MarkerCollection with affinity; TabSettings and VisualColumnCalculator | Unit tests, supplemented by integration/acceptance tests |
| VIEW-001 | MUST | Display several document views simultaneously. | Chapter 11 and 20 | EditorWorkspace, EditorViewState and WinForms Workspace | M1 | Implemented by EditorWorkspace, EditorViewState and WinForms Workspace as defined in chapter 11 and 20 | WinForms integration, UI and manual reference tests |
| VIEW-002 | MUST | Create, close, activate and resize views. | Chapter 11 and 20 | EditorWorkspace, EditorViewState and WinForms Workspace | M2 | Implemented by EditorWorkspace, EditorViewState and WinForms Workspace as defined in chapter 11 and 20 | WinForms integration, UI and manual reference tests |
| VIEW-003 | MUST | Closing a view must not inadvertently destroy unsaved document content. | Chapter 11 and 20 | EditorWorkspace, EditorViewState and WinForms Workspace | M1 | Savepoint in UndoManager and CloseGuard workflow; IDocumentStorage/FileDocumentStorage with atomic saving | WinForms integration, UI and manual reference tests |
| VIEW-004 | MUST | Switch forward and backward among views. | Chapter 11 and 20 | EditorWorkspace, EditorViewState and WinForms Workspace | M1 | Implemented by EditorWorkspace, EditorViewState and WinForms Workspace as defined in chapter 11 and 20 | WinForms integration, UI and manual reference tests |
| VIEW-005 | MUST | Activate views by number, identifier or selection. | Chapter 11 and 20 | EditorWorkspace, EditorViewState and WinForms Workspace | M2 | Implemented by EditorWorkspace, EditorViewState and WinForms Workspace as defined in chapter 11 and 20 | WinForms integration, UI and manual reference tests |
| VIEW-006 | MUST | Allow two or more views to share the same document. | Chapter 11 and 20 | EditorWorkspace, EditorViewState and WinForms Workspace | M2 | shared TextDocument with several independent EditorViewState instances | WinForms integration, UI and manual reference tests |
| VIEW-007 | MUST | Changes made through one view must appear consistently in linked views. | Chapter 11 and 20 | EditorWorkspace, EditorViewState and WinForms Workspace | M2 | shared TextDocument with several independent EditorViewState instances | WinForms integration, UI and manual reference tests |
| VIEW-008 | SHOULD | Keep hidden documents/views in memory and show them later. | Chapter 11 and 20 | EditorWorkspace, EditorViewState and WinForms Workspace | M2 | IDocumentStorage/FileDocumentStorage with atomic saving | WinForms integration, UI and manual reference tests |
| VIEW-009 | MUST | Each view maintains its own caret, selection, scrolling, mode and margin state. | Chapter 11 and 20 | EditorWorkspace, EditorViewState and WinForms Workspace | M2 | Implemented by EditorWorkspace, EditorViewState and WinForms Workspace as defined in chapter 11 and 20 | WinForms integration, UI and manual reference tests |
| VIEW-010 | SHOULD | Modern desktop adapters should support horizontal and vertical splits. | Chapter 11 and 20 | EditorWorkspace, EditorViewState and WinForms Workspace | M1 | Implemented by EditorWorkspace, EditorViewState and WinForms Workspace as defined in chapter 11 and 20 | WinForms integration, UI and manual reference tests |
| VIEW-011 | MUST | Do not artificially cap views at eight or nine. | Chapter 11 and 20 | EditorWorkspace, EditorViewState and WinForms Workspace | M2 | Implemented by EditorWorkspace, EditorViewState and WinForms Workspace as defined in chapter 11 and 20 | WinForms integration, UI and manual reference tests |
| VIEW-012 | MUST | Manage active view and view ordering unambiguously. | Chapter 11 and 20 | EditorWorkspace, EditorViewState and WinForms Workspace | M2 | Implemented by EditorWorkspace, EditorViewState and WinForms Workspace as defined in chapter 11 and 20 | WinForms integration, UI and manual reference tests |
| FILE-001 | MUST | Read text files into new or existing documents. | Chapter 19 | IDocumentStorage and FileDocumentStorage | M1 | IDocumentStorage/FileDocumentStorage with atomic saving | Filesystem/stream integration tests including failures |
| FILE-002 | MUST | Support Save, Save As and writing to a named file. | Chapter 19 | IDocumentStorage and FileDocumentStorage | M1 | IDocumentStorage/FileDocumentStorage with atomic saving | Filesystem/stream integration tests including failures |
| FILE-003 | MUST | Invoke file operations interactively through host prompts or programmatically with parameters. | Chapter 19 | IDocumentStorage and FileDocumentStorage | M1 | IDocumentStorage/FileDocumentStorage with atomic saving | Filesystem/stream integration tests including failures |
| FILE-004 | SHOULD | File operations should be asynchronous and cancellable. | Chapter 19 | IDocumentStorage and FileDocumentStorage | M1 | IDocumentStorage/FileDocumentStorage with atomic saving | Filesystem/stream integration tests including failures |
| FILE-005 | MUST | Detect, preserve and intentionally convert encodings and line endings. | Chapter 19 | IDocumentStorage and FileDocumentStorage | M1 | LineEndingKind per line with configurable save policy | Filesystem/stream integration tests including failures |
| FILE-006 | MUST | Never silently overwrite binary/unsupported files as ordinary text. | Chapter 19 | IDocumentStorage and FileDocumentStorage | M2/M3 | IDocumentStorage/FileDocumentStorage with atomic saving | Filesystem/stream integration tests including failures |
| FILE-007 | MUST | Handle read/write failures without losing the existing document content. | Chapter 19 | IDocumentStorage and FileDocumentStorage | M2/M3 | structured EditorError codes with host-side presenter | Filesystem/stream integration tests including failures |
| FILE-008 | SHOULD | Where possible, save atomically using a temporary file followed by replacement. | Chapter 19 | IDocumentStorage and FileDocumentStorage | M2/M3 | IDocumentStorage/FileDocumentStorage with atomic saving | Filesystem/stream integration tests including failures |
| FILE-009 | SHOULD | Warn before overwriting externally modified files where possible. | Chapter 19 | IDocumentStorage and FileDocumentStorage | M2/M3 | IDocumentStorage/FileDocumentStorage with atomic saving | Filesystem/stream integration tests including failures |
| FILE-010 | SHOULD | Support streams in addition to filesystem paths. | Chapter 19 | IDocumentStorage and FileDocumentStorage | M2/M3 | IDocumentStorage/FileDocumentStorage with atomic saving | Filesystem/stream integration tests including failures |
| FILE-011 | MAY | Historical WordStar wrap markers may be supported via optional import/export. | Chapter 19 | IDocumentStorage and FileDocumentStorage | M1 | Implemented by IDocumentStorage and FileDocumentStorage as defined in chapter 19 | Filesystem/stream integration tests including failures |
| FILE-012 | MAY | Reading/writing a marked block as a separate file is optional. | Chapter 19 | IDocumentStorage and FileDocumentStorage | M2/M3 | IDocumentStorage/FileDocumentStorage with atomic saving | Filesystem/stream integration tests including failures |
| FILE-013 | MAY | Renaming, copying, deleting and directory listing are optional host services, not mandatory core capabilities. | Chapter 19 | IDocumentStorage and FileDocumentStorage | M2/M3 | IDocumentStorage/FileDocumentStorage with atomic saving | Filesystem/stream integration tests including failures |
| FILE-014 | MUST | Before discarding unsaved changes, host must be able to offer Save/Discard/Cancel. | Chapter 19 | IDocumentStorage and FileDocumentStorage | M2/M3 | Savepoint in UndoManager and CloseGuard workflow; IDocumentStorage/FileDocumentStorage with atomic saving | Filesystem/stream integration tests including failures |
| UNDO-001 | MUST | Editing and deletion operations must be undoable. | Chapter 12 | UndoManager and TextChangeSet | M1 | Implemented by UndoManager and TextChangeSet as defined in chapter 12 | Unit tests, supplemented by integration/acceptance tests |
| UNDO-002 | MUST | Support multiple undo steps and redo. | Chapter 12 | UndoManager and TextChangeSet | M1 | transactional UndoManager with coalescing and savepoint | Unit tests, supplemented by integration/acceptance tests |
| UNDO-003 | MUST | Group related edits into transactions. | Chapter 12 | UndoManager and TextChangeSet | M1 | Implemented by UndoManager and TextChangeSet as defined in chapter 12 | Unit tests, supplemented by integration/acceptance tests |
| UNDO-004 | MUST | Undo limit must be configurable. | Chapter 12 | UndoManager and TextChangeSet | M1 | transactional UndoManager with coalescing and savepoint | Unit tests, supplemented by integration/acceptance tests |
| UNDO-005 | SHOULD | Support limiting by operation count, memory budget or both. | Chapter 12 | UndoManager and TextChangeSet | M1 | IDocumentStorage/FileDocumentStorage with atomic saving | Unit tests, supplemented by integration/acceptance tests |
| UNDO-006 | MUST | Undo/redo belongs to the document, independent of the number of views. | Chapter 12 | UndoManager and TextChangeSet | M2 | transactional UndoManager with coalescing and savepoint | Unit tests, supplemented by integration/acceptance tests |
| UNDO-007 | MUST | Dirty state must reliably reflect differences from the saved state. | Chapter 12 | UndoManager and TextChangeSet | M1 | Savepoint in UndoManager and CloseGuard workflow; IDocumentStorage/FileDocumentStorage with atomic saving | Unit tests, supplemented by integration/acceptance tests |
| UNDO-008 | MUST | Mark the saved state clean after successful save. | Chapter 12 | UndoManager and TextChangeSet | M1 | IDocumentStorage/FileDocumentStorage with atomic saving | Unit tests, supplemented by integration/acceptance tests |
| UNDO-009 | MUST | Custom extension commands must be able to integrate with undo/redo and dirty state. | Chapter 12 | UndoManager and TextChangeSet | M1 | Savepoint in UndoManager and CloseGuard workflow; transactional UndoManager with coalescing and savepoint; EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | Unit tests, supplemented by integration/acceptance tests |
| CMD-001 | MUST | Invoke all user operations via uniquely identifiable editor commands. | Chapter 14 | EditorCommandRegistry, Dispatcher and KeyboardProfile | M1 | EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | Unit tests, supplemented by integration/acceptance tests |
| CMD-002 | MUST | Use a central dispatcher to map input to commands. | Chapter 14 | EditorCommandRegistry, Dispatcher and KeyboardProfile | M1 | EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | Unit tests, supplemented by integration/acceptance tests |
| CMD-003 | MUST | Commands must be callable independently from menu, keyboard, toolbar, context menu or API. | Chapter 14 | EditorCommandRegistry, Dispatcher and KeyboardProfile | M1 | EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | Unit tests, supplemented by integration/acceptance tests |
| CMD-004 | MUST | Commands must check executability and be callable synchronously or asynchronously. | Chapter 14 | EditorCommandRegistry, Dispatcher and KeyboardProfile | M1 | EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | Unit tests, supplemented by integration/acceptance tests |
| CMD-005 | MUST | Hooks/interceptors must be able to filter, replace, suppress or handle commands. | Chapter 14 | EditorCommandRegistry, Dispatcher and KeyboardProfile | M1 | TextSearchService with SearchOptions, CancellationToken and SearchResult; EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | Unit tests, supplemented by integration/acceptance tests |
| CMD-006 | MUST | A fully handled command must not be executed again by the default dispatcher. | Chapter 14 | EditorCommandRegistry, Dispatcher and KeyboardProfile | M1 | EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | Unit tests, supplemented by integration/acceptance tests |
| CMD-007 | MUST | Support individual keypresses and multistage prefix sequences. | Chapter 14 | EditorCommandRegistry, Dispatcher and KeyboardProfile | M1 | EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | Unit tests, supplemented by integration/acceptance tests |
| CMD-008 | MUST | Keyboard profiles must be replaceable and serializable as user configurations. | Chapter 14 | EditorCommandRegistry, Dispatcher and KeyboardProfile | M1 | IDocumentStorage/FileDocumentStorage with atomic saving; EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | Unit tests, supplemented by integration/acceptance tests |
| CMD-009 | SHOULD | Provide a modern default and Turbo Editor/WordStar-compatible profile. | Chapter 14 | EditorCommandRegistry, Dispatcher and KeyboardProfile | M2 | Implemented by EditorCommandRegistry, Dispatcher and KeyboardProfile as defined in chapter 14 | Unit tests, supplemented by integration/acceptance tests |
| CMD-010 | MUST | Hosts must be able to register custom commands and replace/disable built-in commands. | Chapter 14 | EditorCommandRegistry, Dispatcher and KeyboardProfile | M2 | TextSearchService with SearchOptions, CancellationToken and SearchResult; EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | Unit tests, supplemented by integration/acceptance tests |
| CMD-011 | MAY | Optionally enqueue several input events programmatically. | Chapter 14 | EditorCommandRegistry, Dispatcher and KeyboardProfile | M2 | Implemented by EditorCommandRegistry, Dispatcher and KeyboardProfile as defined in chapter 14 | Unit tests, supplemented by integration/acceptance tests |
| CMD-012 | MAY | Architecture should permit macros/command languages, but these are not first-milestone features. | Chapter 14 | EditorCommandRegistry, Dispatcher and KeyboardProfile | M6 | EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | Unit tests, supplemented by integration/acceptance tests |
| CMD-013 | MUST | Provide global cancellation of long-running operations. | Chapter 14 | EditorCommandRegistry, Dispatcher and KeyboardProfile | M2 | EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | Unit tests, supplemented by integration/acceptance tests |
| CMD-014 | MUST | Unknown input must not trigger uncontrolled behavior. | Chapter 14 | EditorCommandRegistry, Dispatcher and KeyboardProfile | M6 | Implemented by EditorCommandRegistry, Dispatcher and KeyboardProfile as defined in chapter 14 | Unit tests, supplemented by integration/acceptance tests |
| UI-001 | MUST | Provide WinForms as a reusable control or cleanly encapsulated adapter component. | Chapter 20-22 | Sasd.EditorToolkit.WinForms | M1 | SasdEditorView/UserControl with its own EditorSurface | WinForms integration, UI and manual reference tests |
| UI-002 | MUST | Display text, caret, selection, matches, markers and special annotations. | Chapter 20-22 | Sasd.EditorToolkit.WinForms | M1 | TextAnchor-based MarkerCollection with affinity; TextSearchService with SearchOptions, CancellationToken and SearchResult | WinForms integration, UI and manual reference tests |
| UI-003 | MUST | Status display must show at least filename, line, column, dirty state and editing mode. | Chapter 20-22 | Sasd.EditorToolkit.WinForms | M1 | Savepoint in UndoManager and CloseGuard workflow; IDocumentStorage/FileDocumentStorage with atomic saving | WinForms integration, UI and manual reference tests |
| UI-004 | MUST | Host must be able to replace, extend or hide status display. | Chapter 20-22 | Sasd.EditorToolkit.WinForms | M1 | Implemented by Sasd.EditorToolkit.WinForms as defined in chapter 20-22 | WinForms integration, UI and manual reference tests |
| UI-005 | SHOULD | Command/message line or functionally equivalent prompts should be possible. | Chapter 20-22 | Sasd.EditorToolkit.WinForms | M1 | EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | WinForms integration, UI and manual reference tests |
| UI-006 | MUST | Menus, toolbars and context menus must use the same command model. | Chapter 20-22 | Sasd.EditorToolkit.WinForms | M1 | EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | WinForms integration, UI and manual reference tests |
| UI-007 | MAY | Optional retro/MicroStar sample may recreate historic pulldown menus. | Chapter 20-22 | Sasd.EditorToolkit.WinForms | M1 | Implemented by Sasd.EditorToolkit.WinForms as defined in chapter 20-22 | WinForms integration, UI and manual reference tests |
| UI-008 | MUST | Host services must be able to replace modal and modeless prompts. | Chapter 20-22 | Sasd.EditorToolkit.WinForms | M1 | Implemented by Sasd.EditorToolkit.WinForms as defined in chapter 20-22 | WinForms integration, UI and manual reference tests |
| UI-009 | MUST | Error and confirmation presentation must be platform-replaceable. | Chapter 20-22 | Sasd.EditorToolkit.WinForms | M1 | structured EditorError codes with host-side presenter | WinForms integration, UI and manual reference tests |
| UI-010 | MUST | Themes must distinguish ordinary text, selection, status, commands and special content. | Chapter 20-22 | Sasd.EditorToolkit.WinForms | M1 | EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | WinForms integration, UI and manual reference tests |
| UI-011 | MUST | UI must not depend on video memory, retrace timing or an 80×25 grid. | Chapter 20-22 | Sasd.EditorToolkit.WinForms | M1 | IDocumentStorage/FileDocumentStorage with atomic saving | WinForms integration, UI and manual reference tests |
| UI-012 | MUST | WinForms sample must demonstrate menus, toolbar, status bar, multiple views and file dialogs. | Chapter 20-22 | Sasd.EditorToolkit.WinForms | M1 | shared TextDocument with several independent EditorViewState instances; IDocumentStorage/FileDocumentStorage with atomic saving; SasdEditorView/UserControl with its own EditorSurface | WinForms integration, UI and manual reference tests |
| UI-013 | SHOULD | WPF and web obtain their own adapters to the same core. | Chapter 20-22 | Sasd.EditorToolkit.WinForms | M4 | separate WPF adapter using unchanged Core; separate web adapter; Core stays DOM- and browser-independent | WinForms integration, UI and manual reference tests |
| UI-014 | SHOULD | Legacy ASPX/Web Forms is not a first-class initial control but may integrate the core. | Chapter 20-22 | Sasd.EditorToolkit.WinForms | M4 | separate web adapter; Core stays DOM- and browser-independent | WinForms integration, UI and manual reference tests |
| UI-015 | SHOULD | Optionally display invisibles: line endings, tabs, spaces and control characters. | Chapter 20-22 | Sasd.EditorToolkit.WinForms | M5 | LineEndingKind per line with configurable save policy; TabSettings and VisualColumnCalculator | WinForms integration, UI and manual reference tests |
| UI-016 | SHOULD | Optionally display line numbers, margins and current-caret line. | Chapter 20-22 | Sasd.EditorToolkit.WinForms | M5 | Implemented by Sasd.EditorToolkit.WinForms as defined in chapter 20-22 | WinForms integration, UI and manual reference tests |
| UI-017 | MUST | Rendering and scrolling must remain responsive during typing. | Chapter 20-22 | Sasd.EditorToolkit.WinForms | M1 | Implemented by Sasd.EditorToolkit.WinForms as defined in chapter 20-22 | WinForms integration, UI and manual reference tests |
| UI-018 | MUST | WinForms adapter must account for high DPI, resizing, focus and IME. | Chapter 20-22 | Sasd.EditorToolkit.WinForms | M1 | SasdEditorView/UserControl with its own EditorSurface; DPI-aware WinForms layout with font/measure-cache invalidation | WinForms integration, UI and manual reference tests |
| ERR-001 | MUST | Core returns structured errors and result objects, not only human-readable messages. | Chapter 23 | EditorResult, ErrorCatalog and host presenter | M1 | structured EditorError codes with host-side presenter | Unit tests for error codes and host-presenter integration |
| ERR-002 | MUST | Hosts may handle errors entirely or forward them to default handling. | Chapter 23 | EditorResult, ErrorCatalog and host presenter | M1 | structured EditorError codes with host-side presenter | Unit tests for error codes and host-presenter integration |
| ERR-003 | MUST | Error messages must be localizable. | Chapter 23 | EditorResult, ErrorCatalog and host presenter | M1 | structured EditorError codes with host-side presenter; RESX resources and bilingual documentation | Unit tests for error codes and host-presenter integration |
| ERR-004 | MUST | Distinguish file, memory, input, search and state errors. | Chapter 23 | EditorResult, ErrorCatalog and host presenter | M1 | TextSearchService with SearchOptions, CancellationToken and SearchResult; IDocumentStorage/FileDocumentStorage with atomic saving; structured EditorError codes with host-side presenter | Unit tests for error codes and host-presenter integration |
| ERR-005 | MUST | Errors must not cause partial edits or data loss. | Chapter 23 | EditorResult, ErrorCatalog and host presenter | M1 | structured EditorError codes with host-side presenter | Unit tests for error codes and host-presenter integration |
| ERR-006 | MUST | Configure confirmations for destructive operations centrally. | Chapter 23 | EditorResult, ErrorCatalog and host presenter | M1 | Implemented by EditorResult, ErrorCatalog and host presenter as defined in chapter 23 | Unit tests for error codes and host-presenter integration |
| ERR-007 | MUST | Library code must not force UI dialogs. | Chapter 23 | EditorResult, ErrorCatalog and host presenter | M1 | Implemented by EditorResult, ErrorCatalog and host presenter as defined in chapter 23 | Unit tests for error codes and host-presenter integration |
| BG-001 | SHOULD | Provide a generic facility for incremental background/host work without a proprietary editor scheduler. | Chapter 24 | IBackgroundOperationRunner, autosave and print adapters | M3/M6 | Implemented by IBackgroundOperationRunner, autosave and print adapters as defined in chapter 24 | Cancellation, timing and restart/recovery tests |
| BG-002 | MUST | Background tasks must react promptly to input/cancellation and preserve state. | Chapter 24 | IBackgroundOperationRunner, autosave and print adapters | M3/M6 | Implemented by IBackgroundOperationRunner, autosave and print adapters as defined in chapter 24 | Cancellation, timing and restart/recovery tests |
| BG-003 | SHOULD | Optional autosave must be integrable as a host service. | Chapter 24 | IBackgroundOperationRunner, autosave and print adapters | M3/M6 | Implemented by IBackgroundOperationRunner, autosave and print adapters as defined in chapter 24 | Cancellation, timing and restart/recovery tests |
| BG-004 | MAY | Background printing may be documented as an example/optional extension. | Chapter 24 | IBackgroundOperationRunner, autosave and print adapters | M6 | Implemented by IBackgroundOperationRunner, autosave and print adapters as defined in chapter 24 | Cancellation, timing and restart/recovery tests |
| BG-005 | EXCLUDED | Historical modem dialing, upload/download and disk backup are not editor features. | Chapter 24 | IBackgroundOperationRunner, autosave and print adapters | M6 | Implemented by IBackgroundOperationRunner, autosave and print adapters as defined in chapter 24 | Cancellation, timing and restart/recovery tests |
| BG-006 | SHOULD | Modern hosts should use tasks, CancellationToken and platform dispatchers rather than polling. | Chapter 24 | IBackgroundOperationRunner, autosave and print adapters | M6 | Implemented by IBackgroundOperationRunner, autosave and print adapters as defined in chapter 24 | Cancellation, timing and restart/recovery tests |
| INT-001 | MUST | Embed editor in larger applications. | Chapter 7, 25 and 26 | Public Core API and platform adapters | M1 | Implemented by Public Core API and platform adapters as defined in chapter 7, 25 and 26 | Architecture test, package/repository review and integration sample |
| INT-002 | MUST | Hosts provide documents, commands, services and settings programmatically. | Chapter 7, 25 and 26 | Public Core API and platform adapters | M1 | EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | Architecture test, package/repository review and integration sample |
| INT-003 | MUST | Core must run headlessly for tests, batch and server-side work. | Chapter 7, 25 and 26 | Public Core API and platform adapters | M1 | Implemented by Public Core API and platform adapters as defined in chapter 7, 25 and 26 | Architecture test, package/repository review and integration sample |
| INT-004 | MUST | Clipboard, file dialogs, printing, themes, input, logging and notifications must be replaceable through adapters/services. | Chapter 7, 25 and 26 | Public Core API and platform adapters | M1 | IDocumentStorage/FileDocumentStorage with atomic saving; ILogger abstraction without forced UI | Architecture test, package/repository review and integration sample |
| INT-005 | MUST | Integration must not require special include order, globals, overlays or chain files. | Chapter 7, 25 and 26 | Public Core API and platform adapters | M1 | Implemented by Public Core API and platform adapters as defined in chapter 7, 25 and 26 | Architecture test, package/repository review and integration sample |
| INT-006 | SHOULD | Support dependency injection and public interfaces. | Chapter 7, 25 and 26 | Public Core API and platform adapters | M1 | Implemented by Public Core API and platform adapters as defined in chapter 7, 25 and 26 | Architecture test, package/repository review and integration sample |
| INT-007 | SHOULD | Library family should be packagable through NuGet. | Chapter 7, 25 and 26 | Public Core API and platform adapters | M1 | Implemented by Public Core API and platform adapters as defined in chapter 7, 25 and 26 | Architecture test, package/repository review and integration sample |
| INT-008 | MUST | Public APIs must be versioned and documented. | Chapter 7, 25 and 26 | Public Core API and platform adapters | M1 | Implemented by Public Core API and platform adapters as defined in chapter 7, 25 and 26 | Architecture test, package/repository review and integration sample |
| INT-009 | SHOULD | Desktop Components should only reference/showcase the editor. | Chapter 7, 25 and 26 | Public Core API and platform adapters | M1 | Implemented by Public Core API and platform adapters as defined in chapter 7, 25 and 26 | Architecture test, package/repository review and integration sample |
| INT-010 | MUST | Data Toolbox may supply optional storage, never a mandatory dependency. | Chapter 7, 25 and 26 | Public Core API and platform adapters | M1 | IDocumentStorage/FileDocumentStorage with atomic saving; TabSettings and VisualColumnCalculator | Architecture test, package/repository review and integration sample |
| INT-011 | MUST | Numerics, Graphics and GameWorks may consume the editor without cyclic dependencies. | Chapter 7, 25 and 26 | Public Core API and platform adapters | M1 | Implemented by Public Core API and platform adapters as defined in chapter 7, 25 and 26 | Architecture test, package/repository review and integration sample |
| INT-012 | SHOULD | Future syntax highlighting and Markdown/log/hex/property editors should use public extension points. | Chapter 7, 25 and 26 | Public Core API and platform adapters | M6 | Implemented by Public Core API and platform adapters as defined in chapter 7, 25 and 26 | Architecture test, package/repository review and integration sample |
| SET-001 | MUST | Settings must cover keyboard profiles, themes, fonts, tab width, margins, word wrap, auto-indent, undo limits and display options. | Chapter 25 | EditorSettings and JSON serialization | M1 | transactional UndoManager with coalescing and savepoint; EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile; ViewLayoutService with optional ReformatParagraphCommand; AutoIndentPolicy in NewLineCommand; TabSettings and VisualColumnCalculator | Unit tests, supplemented by integration/acceptance tests |
| SET-002 | SHOULD | Separate global defaults from per-document/per-view overrides. | Chapter 25 | EditorSettings and JSON serialization | M1 | Implemented by EditorSettings and JSON serialization as defined in chapter 25 | Unit tests, supplemented by integration/acceptance tests |
| SET-003 | SHOULD | Settings should be serializable, validatable and recoverable. | Chapter 25 | EditorSettings and JSON serialization | M1 | Implemented by EditorSettings and JSON serialization as defined in chapter 25 | Unit tests, supplemented by integration/acceptance tests |
| SET-004 | MUST | Invalid values must cause clear errors or safe defaults. | Chapter 25 | EditorSettings and JSON serialization | M1 | structured EditorError codes with host-side presenter | Unit tests, supplemented by integration/acceptance tests |
| SET-005 | MUST | 80 columns, 25 lines and historical colors may only be demonstration/compatibility settings. | Chapter 25 | EditorSettings and JSON serialization | M1 | Implemented by EditorSettings and JSON serialization as defined in chapter 25 | Unit tests, supplemented by integration/acceptance tests |
| NFR-ARCH-001 | MUST | Clearly separate Core, application services, UI adapters and samples. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M1-M3 | Implemented by Cross-cutting architecture, quality and operational measures as defined in chapter 27-34 | Architecture, performance, security, accessibility or documentation gate |
| NFR-ARCH-002 | MUST | Do not introduce cyclic project or package dependencies. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M1-M3 | TabSettings and VisualColumnCalculator | Architecture, performance, security, accessibility or documentation gate |
| NFR-ARCH-003 | MUST | UI-framework types must not appear in public Core APIs. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M1-M3 | Implemented by Cross-cutting architecture, quality and operational measures as defined in chapter 27-34 | Architecture, performance, security, accessibility or documentation gate |
| NFR-PERF-001 | MUST | Normal editing and caret operations must feel immediate to the user. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M1-M3 | Implemented by Cross-cutting architecture, quality and operational measures as defined in chapter 27-34 | Architecture, performance, security, accessibility or documentation gate |
| NFR-PERF-002 | SHOULD | Rendering should update only affected visible regions. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M1-M3 | Implemented by Cross-cutting architecture, quality and operational measures as defined in chapter 27-34 | Architecture, performance, security, accessibility or documentation gate |
| NFR-PERF-003 | MUST | Long-running file and search operations must not permanently block the UI. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M1-M3 | TextSearchService with SearchOptions, CancellationToken and SearchResult; IDocumentStorage/FileDocumentStorage with atomic saving | Architecture, performance, security, accessibility or documentation gate |
| NFR-PERF-004 | MUST | Architecture must allow later optimized buffers for very large files. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M3 | IDocumentStorage/FileDocumentStorage with atomic saving | Architecture, performance, security, accessibility or documentation gate |
| NFR-ROB-001 | MUST | Protect unsaved changes before destructive operations. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M1-M3 | Savepoint in UndoManager and CloseGuard workflow; IDocumentStorage/FileDocumentStorage with atomic saving | Architecture, performance, security, accessibility or documentation gate |
| NFR-ROB-002 | MUST | Saving must be as robust as possible against I/O errors. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M1-M3 | IDocumentStorage/FileDocumentStorage with atomic saving; structured EditorError codes with host-side presenter | Architecture, performance, security, accessibility or documentation gate |
| NFR-ROB-003 | MUST | Core must be deterministic and testable without global mutable singletons. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M1-M3 | TabSettings and VisualColumnCalculator | Architecture, performance, security, accessibility or documentation gate |
| NFR-SEC-001 | MUST | Treat file paths and contents as untrusted. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M1-M3 | IDocumentStorage/FileDocumentStorage with atomic saving | Architecture, performance, security, accessibility or documentation gate |
| NFR-SEC-002 | MUST | Never execute macros, scripts or embedded content automatically. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M6 | Implemented by Cross-cutting architecture, quality and operational measures as defined in chapter 27-34 | Architecture, performance, security, accessibility or documentation gate |
| NFR-SEC-003 | SHOULD | Store temporary and autosave files in controlled locations with suitable permissions. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M1-M3 | IDocumentStorage/FileDocumentStorage with atomic saving | Architecture, performance, security, accessibility or documentation gate |
| NFR-ACC-001 | MUST | Desktop adapters must be fully usable via keyboard. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M1-M3 | EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | Architecture, performance, security, accessibility or documentation gate |
| NFR-ACC-002 | SHOULD | Consider contrast, focus and screen-reader information. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M3 | AccessibleObject and full keyboard operability | Architecture, performance, security, accessibility or documentation gate |
| NFR-TEST-001 | MUST | Core must have comprehensive unit tests. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M1-M3 | Implemented by Cross-cutting architecture, quality and operational measures as defined in chapter 27-34 | Architecture, performance, security, accessibility or documentation gate |
| NFR-TEST-002 | MUST | Architecture tests must enforce dependency direction. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M1-M3 | Implemented by Cross-cutting architecture, quality and operational measures as defined in chapter 27-34 | Architecture, performance, security, accessibility or documentation gate |
| NFR-TEST-003 | MUST | Integration tests must cover file workflows, undo/redo, search, views and keyboard profiles. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M1-M3 | transactional UndoManager with coalescing and savepoint; TextSearchService with SearchOptions, CancellationToken and SearchResult; IDocumentStorage/FileDocumentStorage with atomic saving; EditorCommandRegistry, Dispatcher and serialisierbare KeyboardProfile | Architecture, performance, security, accessibility or documentation gate |
| NFR-DOC-001 | MUST | Public APIs must include XML documentation comments. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M1-M3 | Implemented by Cross-cutting architecture, quality and operational measures as defined in chapter 27-34 | Architecture, performance, security, accessibility or documentation gate |
| NFR-DOC-002 | MUST | Plan README, requirements/implementation specifications, architecture, developer, test and user manuals. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M1-M3 | Implemented by Cross-cutting architecture, quality and operational measures as defined in chapter 27-34 | Architecture, performance, security, accessibility or documentation gate |
| NFR-DOC-003 | MUST | Map historical functions to modern requirements or deliberate exclusions. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M3 | Implemented by Cross-cutting architecture, quality and operational measures as defined in chapter 27-34 | Architecture, performance, security, accessibility or documentation gate |
| NFR-COMP-001 | SHOULD | The first production version should use a supported .NET LTS version, selected in the implementation specification. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M1-M3 | Implemented by Cross-cutting architecture, quality and operational measures as defined in chapter 27-34 | Architecture, performance, security, accessibility or documentation gate |
| NFR-COMP-002 | MUST | WinForms and WPF are Windows adapters; Core remains platform-neutral. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M4 | SasdEditorView/UserControl with its own EditorSurface; separate WPF adapter using unchanged Core | Architecture, performance, security, accessibility or documentation gate |
| NFR-LIC-001 | MUST | Do not adopt original source, trademarks or protected visual design without review. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M0 | Implemented by Cross-cutting architecture, quality and operational measures as defined in chapter 27-34 | Architecture, performance, security, accessibility or documentation gate |
| NFR-LIC-002 | MUST | Review the name, trademark references and copyright notices before publication. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M0 | Implemented by Cross-cutting architecture, quality and operational measures as defined in chapter 27-34 | Architecture, performance, security, accessibility or documentation gate |
| NFR-MAINT-001 | MUST | Code must be clearly structured, generously commented and documented. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M1-M3 | Implemented by Cross-cutting architecture, quality and operational measures as defined in chapter 27-34 | Architecture, performance, security, accessibility or documentation gate |
| NFR-MAINT-002 | MUST | Document and version breaking changes. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M3 | Implemented by Cross-cutting architecture, quality and operational measures as defined in chapter 27-34 | Architecture, performance, security, accessibility or documentation gate |
| NFR-OBS-001 | SHOULD | Errors and important file operations should support abstracted logging. | Chapter 27-34 | Cross-cutting architecture, quality and operational measures | M1-M3 | IDocumentStorage/FileDocumentStorage with atomic saving; structured EditorError codes with host-side presenter; ILogger abstraction without forced UI | Architecture, performance, security, accessibility or documentation gate |


## 39. Acceptance criteria and verification methods

| ID | Criterion | Verification | Due |
|---|---|---|---|
| AK-001 | Create, edit, save, close and reopen an initially empty document with identical content. | End-to-end New/Edit/Save/Close/Open with byte and text comparison. | M1 |
| AK-002 | Preserve German umlauts, non-Latin scripts, emoji and other Unicode characters in supported encodings. | Unicode corpus with umlauts, CJK, RTL example, emoji and combining characters. | M1 |
| AK-003 | Undo/redo for insertion, deletion, multiline changes, block operations and replacement. | Undo/redo integration covering all listed operation types. | M1 |
| AK-004 | Two views share document content but have distinct caret and scroll state. | Workspace test with two views and separate caret/scroll. | M2 |
| AK-005 | All command categories in historical ch. 12 implemented, modernized or expressly deferred/omitted. | Command catalog and historical traceability review. | M2 |
| AK-006 | Load a keyboard profile without recompiling Core. | Load JSON profile; test binding without Core rebuild. | M2 |
| AK-007 | Register a host command and invoke it via menu/keyboard. | Register example host command and trigger through menu/key sequence. | M2 |
| AK-008 | Test Core without Windows UI thread. | Run Core tests without initializing WinForms. | M1 |
| AK-009 | Core project has no WinForms, WPF or ASP.NET dependencies. | Architecture test of assembly references. | M1 |
| AK-010 | Host gets Save/Discard/Cancel decision when closing modified documents. | CloseGuard integration test for all choices. | M1 |
| AK-011 | File failures do not lose existing document content. | Fault injection for file/stream errors. | M1 |
| AK-012 | Search/replace is cancellable and unambiguously reports NotFound. | Cancellation and NotFound tests. | M1 |
| AK-013 | WinForms sample shows status, views, file operations, search, undo/redo and at least two key profiles. | Manual and automated sample checklist. | M2 |
| AK-014 | Public APIs have XML documentation; core behavior has automated tests. | XML-documentation and test CI gate. | M1 |
| AK-015 | Repository contains historical chapter/API traceability. | Document review of chapters, hooks, modules and API inventory. | M3 |

## 40. Historical hooks and extension points

| Historical hook | Original responsibility | Concrete modern implementation | Phase |
|---|---|---|---|
| `UserCommand` | Filter/remap/handle/suppress input | `ICommandInterceptor`, `EditorCommandRegistry`, `KeyboardProfile` | M1/M2 |
| `UserError` | Handle errors or delegate to standard presentation | `IEditorErrorPresenter`, structured `EditorError` results | M1/M2 |
| `UserStatusLine` | Replace/extend standard status | `EditorStatusSnapshot`, `IEditorStatusSink`, status events | M1/M2 |
| `UserReplace` | Guide interactive Find/Replace decisions | `IReplaceDecisionProvider` | M1/M2 |
| `UserTask` | Cooperative idle-time background work | `IBackgroundOperationRunner`, tasks and CancellationToken | M6/Adapter |
| `MakeWindow` / `RestoreWindow` | Paint popup and preserve underlying screen | WinForms/WPF/web dialog or overlay service | M6/Adapter |
| `PulldownMenu` / `InitMainMenu` | Define menus and invoke commands | Host menus driven by command IDs | M6/Adapter |
| `PrintNext` | Incremental background printing | `IPrintService` from M6 | M6/Adapter |
| `ErrorCheck` | MicroStar-specific error display | Host-side presenter/interceptor | M6/Adapter |
| `SecondChar` | Request second key in a prefix sequence | `KeyboardSequenceResolver` | M1/M2 |
| `ReadBlock` / Block Write | Read/write block as file | Selection import/export from M6 | M6/Adapter |
| `SpellingCheck` | Spell checking | External language service from M6 | M6/Adapter |

## 41. Historical module and file mapping

| Historical file/group | Responsibility | Modern target | Implementation rule |
|---|---|---|---|
| `VARS.ED`, `VARS.MS` | Constants, types, globals | Encapsulated Core models/options/state | Never copy historical source; map responsibility only. |
| `USER.ED`, `USER.MS` | Low-level core routines | Core/text buffer/document operations | Never copy historical source; map responsibility only. |
| `SCREEN.ED`, `SCREEN.MS` | Screen drawing | WinForms/WPF/web rendering adapters | Never copy historical source; map responsibility only. |
| `INIT.ED`, `INIT.MS` | Initialization | Composition root, factory, dependency injection | Never copy historical source; map responsibility only. |
| `CMD.ED`, `CMD.MS`, `FASTCMD.MS` | Individual commands | Command implementations | Never copy historical source; map responsibility only. |
| `KCMD.*`, `OCMD.*`, `QCMD.*` | Prefix commands | Command catalog, key-chord profiles | Never copy historical source; map responsibility only. |
| `K.*`, `O.*`, `Q.*`, `DISP.*` | Dispatchers | Command dispatcher/input mapping | Never copy historical source; map responsibility only. |
| `TASK.*` | Scheduler and main loop | Host event loop, tasks, cancellation | Never copy historical source; map responsibility only. |
| `INPUT.*` | Keyboard and typeahead | Platform input adapters | Never copy historical source; map responsibility only. |
| `EDITERR.MSG` | Error catalog | Localized resources/structured errors | Never copy historical source; map responsibility only. |
| `FIRST-ED.PAS` | Simple sample | Modern FIRST-ED sample | Never copy historical source; map responsibility only. |
| `MS.PAS` and `MS.*` | Sophisticated editor/MicroStar | Advanced sample or showcase | Never copy historical source; map responsibility only. |
| `PRINT.MS` | Background printing | Optional print service | Never copy historical source; map responsibility only. |
| `MSCMD.MS` | MicroStar commands | Sample/host commands | Never copy historical source; map responsibility only. |
| `SPELL.MS` | Spell checking | Optional external extension | Never copy historical source; map responsibility only. |
| `PULLDOWN.MS` | Pulldown menus | UI adapter and command menus | Never copy historical source; map responsibility only. |
| `.OVL` and chain/overlay files | Memory overlays | Eliminated; assemblies and standard runtime | Never copy historical source; map responsibility only. |
| `README.COM`, `READ.ME` | Version and distribution notes | README, changelog and release notes | Never copy historical source; map responsibility only. |

## 42. Historical constants and types

### 42.1 Constants

| Historical element | Meaning | Technical implementation |
|---|---|---|
| `Col1`, `Line1` | Logical start positions | Normal position abstraction |
| `Colored`, `Inblock`, `Wrapped` | Line flags | Generic annotations, selection and wrap metadata |
| `Ctrla` through `Ctrlz`, `Del`, `Escape`, `Nul` | ASCII key codes | Key gesture/input abstraction |
| `Defhelplen` | Help window height | UI layout configuration |
| `Defnocols`, `Defnorows` | 80×25 display | Retro sample only; not a product limit |
| `Deftypahd` | Typeahead buffer capacity | Configurable queue or framework input |
| `Errorfile` | `EDITERR.MSG` | Localizable error catalog |
| `Maxlinelength` | 255 characters | No fixed limit |
| `Maxmarker` | 20 markers | At least compatible; internally extensible |
| `Nofile` | `NONAME` | Untitled document state |
| `Screenadr` | Video memory address | Removed entirely |
| Status labels | AI, Col, File, INS, Line, TE, Window, WW | Localizable status model |
| `Zero` through `Nine` | ASCII digits | Standard input handling |

### 42.2 Types

| Historical type | Meaning | Technical implementation |
|---|---|---|
| `Character` | Character and color | Rendering glyph/style in UI adapter |
| `Insflag` | Insert or typeover | EditorInsertMode |
| `Linedesc` / `Plinedesc` | Linked line descriptor | Internal buffer implementation |
| `Textline` / `Ptextline` | Text in a line | Unicode line/span abstraction |
| `Windesc` / `Pwindesc` | Window state and text stream | EditorViewState and DocumentReference |
| Fixed-length Pascal strings | String80, Varstring, Strvartype, St6 | `string`/Spans without historical length cap |


## 43. Historical global state

| Historical variable | Modern responsibility |
|---|---|
| `Abortcmd` | CancellationToken/OperationState |
| `Aborting` | CancellationToken/OperationState |
| `Asking` | Host prompt/status state |
| `Blockcolor` | EditorTheme/AnnotationStyle |
| `Blockfrom` | TextSelection/SelectionRenderOptions |
| `Blockhide` | TextSelection/SelectionRenderOptions |
| `Blockto` | TextSelection/SelectionRenderOptions |
| `Bordcolor` | EditorTheme/AnnotationStyle |
| `Circbuf` | KeyboardSequenceResolver/input adapter; no global ring buffer |
| `Circin` | KeyboardSequenceResolver/input adapter; no global ring buffer |
| `Circout` | KeyboardSequenceResolver/input adapter; no global ring buffer |
| `Cmdcol` | Host prompt/status state |
| `Cmdcolor` | EditorTheme/AnnotationStyle |
| `Cmdlinest` | Host prompt/status state |
| `Curwin` | EditorWorkspace and view list |
| `EditChangeflag` | UndoManager.Savepoint / TextDocument.IsDirty |
| `EditUsercommandInput` | KeyboardSequenceResolver/input adapter; no global ring buffer |
| `Interactive` | Host prompt/status state |
| `Intrflag` | CancellationToken/OperationState |
| `Linelength` | WinForms viewport/layout/renderer; not a Core global |
| `Logscrcols` | WinForms viewport/layout/renderer; not a Core global |
| `Logscrrows` | WinForms viewport/layout/renderer; not a Core global |
| `Logtopscr` | WinForms viewport/layout/renderer; not a Core global |
| `Marker` | MarkerCollection |
| `Nextstream` | DocumentId/workspace factory |
| `Notfound` | SearchSession/SearchOptions/SearchResult |
| `Optstr` | SearchSession/SearchOptions/SearchResult |
| `Physcrcols` | WinForms viewport/layout/renderer; not a Core global |
| `Physcrrows` | WinForms viewport/layout/renderer; not a Core global |
| `Physcrsig` | WinForms viewport/layout/renderer; not a Core global |
| `Replacestr` | SearchSession/SearchOptions/SearchResult |
| `Retracemode` | WinForms viewport/layout/renderer; not a Core global |
| `Rundown` | CancellationToken/OperationState |
| `Screen` | WinForms viewport/layout/renderer; not a Core global |
| `Searchstr` | SearchSession/SearchOptions/SearchResult |
| `Tabsize` | EditorViewState.TabSettings |
| `Txtcolor` | EditorTheme/AnnotationStyle |
| `Typbufovl` | KeyboardSequenceResolver/input adapter; no global ring buffer |
| `Undocount` | UndoManager instance per document |
| `Undoend` | UndoManager instance per document |
| `Undolimit` | UndoManager instance per document |
| `Undostack` | UndoManager instance per document |
| `Updcurflag` | WinForms viewport/layout/renderer; not a Core global |
| `Usercolor` | EditorTheme/AnnotationStyle |
| `Window1` | EditorWorkspace and view list |
| `Winstack` | EditorWorkspace and view list |

## 44. Historical procedure and function mapping

| Historical routine | Modern category | Concrete treatment |
|---|---|---|
| `Advance` | Core Utility | Internal helper or idiomatic .NET API replacement |
| `EditAbort` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditAppchar` | Commands/Input | Registry, dispatcher, profiles and sequence resolver |
| `EditAppcmdnam` | Commands/Input | Registry, dispatcher, profiles and sequence resolver |
| `EditAskfor` | Errors/Prompts | EditorError and host services |
| `EditBackground` | Background/Cancellation | Tasks and CancellationToken; DOS scheduler eliminated |
| `EditBeginningEndLine` | Navigation | Navigation commands |
| `EditBeginningLine` | Navigation | Navigation commands |
| `EditBlockBegin` | Selection/Block | TextSelection and block commands |
| `EditBlockCopy` | Selection/Block | TextSelection and block commands |
| `EditBlockDelete` | Selection/Block | TextSelection and block commands |
| `EditBlockEnd` | Selection/Block | TextSelection and block commands |
| `EditBlockHide` | Selection/Block | TextSelection and block commands |
| `EditBlockMove` | Selection/Block | TextSelection and block commands |
| `EditBottomBlock` | Selection/Block | TextSelection and block commands |
| `EditBreathe` | Background/Cancellation | Tasks and CancellationToken; DOS scheduler eliminated |
| `EditCenterLine` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditChangeCase` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditClsinp` | Commands/Input | Registry, dispatcher, profiles and sequence resolver |
| `EditColorFile` | WinForms Rendering/Layout | Adapter-internal; no public one-to-one API |
| `EditColorLine` | WinForms Rendering/Layout | Adapter-internal; no public one-to-one API |
| `EditCompressLine` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditCpcrewin` | Workspace/View | EditorWorkspace and view commands |
| `EditCpdelwin` | Workspace/View | EditorWorkspace and view commands |
| `EditCpexit` | Lifecycle | CloseGuard/App.Exit Command |
| `EditCpFileSave` | Storage/Document Lifecycle | IDocumentStorage or file commands |
| `EditCpFind` | Search | TextSearchService and search/replace commands |
| `EditCpgotocl` | Navigation | Navigation commands |
| `EditCpgotoln` | Navigation | Navigation commands |
| `EditCpgotowin` | Workspace/View | EditorWorkspace and view commands |
| `EditCpjmpmrk` | Marker | MarkerCollection and marker commands |
| `EditCplnkwin` | Workspace/View | EditorWorkspace and view commands |
| `EditCpReplace` | Search | TextSearchService and search/replace commands |
| `EditCprfw` | Core Utility | Internal helper or idiomatic .NET API replacement |
| `EditCpsetlm` | Core Utility | Internal helper or idiomatic .NET API replacement |
| `EditCpsetmrk` | Marker | MarkerCollection and marker commands |
| `EditCpsetrm` | Core Utility | Internal helper or idiomatic .NET API replacement |
| `EditCptabdef` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditCpundlim` | Core Utility | Internal helper or idiomatic .NET API replacement |
| `EditCpwfw` | Core Utility | Internal helper or idiomatic .NET API replacement |
| `EditCvts2i` | Core Utility | Internal helper or idiomatic .NET API replacement |
| `EditDecline` | Errors/Prompts | EditorError and host services |
| `EditDefineTab` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditDeleteLeftChar` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditDeleteLine` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditDeleteRightChar` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditDeleteRightWord` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditDeleteTextRight` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditDelline` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditDestxtdes` | Core Utility | Internal helper or idiomatic .NET API replacement |
| `EditDownLine` | Navigation | Navigation commands |
| `EditDownPage` | Navigation | Navigation commands |
| `EditEndLine` | Navigation | Navigation commands |
| `EditErrormsg` | Errors/Prompts | EditorError and host services |
| `EditExit` | Lifecycle | CloseGuard/App.Exit Command |
| `EditFileRead` | Storage/Document Lifecycle | IDocumentStorage or file commands |
| `EditFileWrite` | Storage/Document Lifecycle | IDocumentStorage or file commands |
| `EditFind` | Search | TextSearchService and search/replace commands |
| `EditGenlineno` | Core Utility | Internal helper or idiomatic .NET API replacement |
| `EditGotoColumn` | Navigation | Navigation commands |
| `EditGotoLine` | Navigation | Navigation commands |
| `EditHscroll` | WinForms Rendering/Layout | Adapter-internal; no public one-to-one API |
| `EditIncline` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditInitialize` | Composition | EditorToolkitFactory/Composition Root |
| `EditInsertCtrlChar` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditInsertLine` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditJoinLine` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditJumpMarker` | Marker | MarkerCollection and marker commands |
| `EditK` | Commands/Input | Registry, dispatcher, profiles and sequence resolver |
| `EditLeftChar` | Navigation | Navigation commands |
| `EditLeftWord` | Navigation | Navigation commands |
| `EditLongLine` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditMarkblock` | Selection/Block | TextSelection and block commands |
| `EditNewLine` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditO` | Commands/Input | Registry, dispatcher, profiles and sequence resolver |
| `EditOffblock` | Selection/Block | TextSelection and block commands |
| `EditPrccmd` | Commands/Input | Registry, dispatcher, profiles and sequence resolver |
| `EditPrctxt` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditPushtbf` | Commands/Input | Registry, dispatcher, profiles and sequence resolver |
| `EditQ` | Commands/Input | Registry, dispatcher, profiles and sequence resolver |
| `EditRealign` | WinForms Rendering/Layout | Adapter-internal; no public one-to-one API |
| `EditReatxtfil` | Storage/Document Lifecycle | IDocumentStorage or file commands |
| `EditReformat` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditReplace` | Search | TextSearchService and search/replace commands |
| `EditRightChar` | Navigation | Navigation commands |
| `EditRightWord` | Navigation | Navigation commands |
| `EditSchedule` | Background/Cancellation | Tasks and CancellationToken; DOS scheduler eliminated |
| `EditScrollDown` | Navigation | Navigation commands |
| `EditScrollUp` | Navigation | Navigation commands |
| `EditSetLeftMargin` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditSetMarker` | Marker | MarkerCollection and marker commands |
| `EditSetRightMargin` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditSetUndoLimit` | Undo | UndoManager and settings |
| `EditShiftLine` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditShortLine` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditSystem` | Background/Cancellation | Tasks and CancellationToken; DOS scheduler eliminated |
| `EditTab` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditToggleAutoindent` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditToggleInsert` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditToggleWordwrap` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditTopBlock` | Selection/Block | TextSelection and block commands |
| `EditUndo` | Undo | UndoManager and settings |
| `EditUpcase` | Editing/Formatting | ITextBuffer and editing/formatting commands |
| `EditUpdphyscr` | WinForms Rendering/Layout | Adapter-internal; no public one-to-one API |
| `EditUpdrowasm` | WinForms Rendering/Layout | Adapter-internal; no public one-to-one API |
| `EditUpdwindow` | WinForms Rendering/Layout | Adapter-internal; no public one-to-one API |
| `EditUpdwinsl` | WinForms Rendering/Layout | Adapter-internal; no public one-to-one API |
| `EditUpLine` | Navigation | Navigation commands |
| `EditUpPage` | Navigation | Navigation commands |
| `EditUserpush` | Commands/Input | Registry, dispatcher, profiles and sequence resolver |
| `EditWindowBottomFile` | Workspace/View | EditorWorkspace and view commands |
| `EditWindowCreate` | Workspace/View | EditorWorkspace and view commands |
| `EditWindowDelete` | Workspace/View | EditorWorkspace and view commands |
| `EditWindowDeleteText` | Workspace/View | EditorWorkspace and view commands |
| `EditWindowDown` | Workspace/View | EditorWorkspace and view commands |
| `EditWindowGoto` | Workspace/View | EditorWorkspace and view commands |
| `EditWindowLink` | Workspace/View | EditorWorkspace and view commands |
| `EditWindowTopFile` | Workspace/View | EditorWorkspace and view commands |
| `EditWindowUp` | Workspace/View | EditorWorkspace and view commands |
| `EditZapcmdnam` | Commands/Input | Registry, dispatcher, profiles and sequence resolver |
| `MoveFromScreen` | WinForms Rendering/Layout | Adapter-internal; no public one-to-one API |
| `MoveToScreen` | WinForms Rendering/Layout | Adapter-internal; no public one-to-one API |
| `Pokechr` | Commands/Input | Registry, dispatcher, profiles and sequence resolver |


## 45. Historical technologies deliberately not ported literally

| Historical technology | Reason for exclusion | Modern replacement |
|---|---|---|
| Direct video memory and assembly-based row updates | Ties code to frameworks, hardware and platforms | WinForms/WPF/web rendering adapters with invalidation and caching |
| DOS polling scheduler | Blocking, proprietary and incompatible with modern event loops | Tasks, async/await, CancellationToken, dispatcher |
| Typeahead ring buffers and raw scan codes | Nonportable and hard to localize | KeyGesture/KeySequence and platform input adapter |
| Overlays and chain files | Historic memory constraints do not exist in the same form | Assemblies, packages, lazy loading and standard process management |
| Global pointer lists | Difficult to test, error-prone and unsuitable for threaded/hosted code | Encapsulated document, buffer, view and workspace objects |
| ASCII and fixed-length Pascal strings | Insufficient for modern text | Unicode .NET strings and streams |
| 255-character line limit | Unnecessary historical restriction | Resource-based limits and a replaceable buffer |

## 46. Conclusion and implementation approval

This implementation specification details SASD Editor Toolkit sufficiently to create a repository skeleton, Architecture Decision Records and then the first milestone, **Modern FIRST-ED 0.1**. The Core is deliberately small and UI-independent; historical functionality is covered transparently and added in increments.

Approving this specification particularly affirms:

1. independent repository and packages;
2. .NET 10 Core and WinForms as first adapter;
3. custom WinForms editor surface rather than domain-logic coupling to RichTextBox;
4. LineTextBuffer for M1 and replaceable large-file strategy;
5. command-first architecture and serializable keyboard profiles;
6. strict separation of document and view;
7. complete source-requirement traceability;
8. incremental expansion to WPF, web and specialized Desktop Components without forming a monolith.

<!-- English translation based on docs/de/Pflichtenheft.md. Source IDs, use cases, command IDs and traceability mappings preserved; this is not an implementation status report. -->
