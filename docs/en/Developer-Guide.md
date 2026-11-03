# Developer Guide — SASD Editor Toolkit

[Deutsch](../de/Entwicklerhandbuch.md) · [Architecture](Architecture.md) · [Testing Guide](Testing-Guide.md) · [Roadmap](ROADMAP.md)

**Audience:** Core, WinForms, sample and host developers working on the **standalone** editor construction kit.  
**Snapshot:** 2026-11-03. Existing M1 features are described as implemented; designs that have not landed are clearly marked as future work.

## 1. Set up a development environment

| Activity | Requirement |
|---|---|
| Core work on Windows/Linux/macOS | .NET SDK 10.x and Git |
| Entire WinForms solution | Windows with .NET SDK 10.x and Windows desktop tooling |
| Tests | xUnit via \`dotnet test\` |
| SDK selection | \`global.json\`: SDK 10.0.100 with \`latestFeature\` roll-forward |
| IDE | Visual Studio or a suitable C# IDE/editor |

**Clone and inspect:**

\`\`\`bash
git clone https://github.com/Robin-Goerlach/SASD-Editor-Toolkit.git
cd SASD-Editor-Toolkit
git status --short --branch
git log -5 --oneline
dotnet --info
\`\`\`

**Verify the complete solution on Windows:**

\`\`\`powershell
powershell -ExecutionPolicy Bypass -File .\scripts\verify-all.ps1
\`\`\`

**Verify the Core on Debian/WSL, Linux or macOS:**

\`\`\`bash
./scripts/verify-core.sh
\`\`\`

A passing Linux Core test is **not** proof that WinForms compiled. Full Windows GitHub Actions is the complementary check. If an SDK, dependency feed or runner is inaccessible, record the limitation rather than claiming tests ran.

## 2. Read the repository before designing new layers

\`\`\`text
src/Sasd.EditorToolkit.Core/
  Documents/   TextDocument and metadata
  Text/        ITextBuffer, LineTextBuffer, positions/ranges
  Editing/     EditorViewState, TextSelection
  Commands/    IDs, registry, dispatcher and handlers
  Search/      UI-independent literal search
  Storage/     streams, encoding and filesystem persistence
  Undo/        minimal undo/redo history
  Workspaces/  documents and views
  Settings/    initial settings
src/Sasd.EditorToolkit.WinForms/
  SasdEditorView.cs / EditorSurface.cs
samples/Sasd.EditorToolkit.Sample.FirstEd.WinForms/
  MainForm.cs — host workflows/dialogs
tests/Sasd.EditorToolkit.Core.Tests/
docs/adr/      accepted design decisions
docs/spec/     language-neutral behavioral contracts
docs/ai/       status, Codex instructions and next work
\`\`\`

**Read order:** repository and subtree \`AGENTS.md\` → relevant ADR and spec → code/tests → roadmap and NEXT-WORK. The implementation specification is a **target design**, not a guarantee that every illustrated API already exists.

## 3. Minimal working headless example

The following uses real types already present in the M1 code:

\`\`\`csharp
using Sasd.EditorToolkit.Commands;
using Sasd.EditorToolkit.Documents;
using Sasd.EditorToolkit.Editing;
using Sasd.EditorToolkit.Text;

var document = new TextDocument(new LineTextBuffer("Hello"));
document.MarkSaved();

var view = new EditorViewState();
view.MoveCaret(new TextPosition(0, 5));

var registry = new EditorCommandRegistry();
registry.RegisterM1Defaults();
var dispatcher = new EditorCommandDispatcher(registry);

var result = await dispatcher.ExecuteAsync(
    EditorCommandIds.InsertText,
    new EditorCommandContext(document, view, "!"));

if (!result.Handled)
{
    throw new InvalidOperationException(result.Message);
}

Console.WriteLine(document.Buffer.GetText()); // Hello!
Console.WriteLine(document.IsDirty);           // True
document.Undo();
Console.WriteLine(document.Buffer.GetText()); // Hello
\`\`\`

The host creates the dispatcher and context explicitly; there is no required global editor instance. View state is independent of the document. Proposed interfaces such as \`ICommandInterceptor\` or \`ITextBufferFactory\` are **not yet fully implemented** merely because they appear in design documents.

## 4. How edits travel through the layers

1. Host/adapter maps input to a command ID and parameter.
2. Dispatcher finds the command and checks \`CanExecute\`.
3. A mutating command calls **\`TextDocument\` methods** and updates caret/selection.
4. \`TextDocument\` registers a \`TextChangeSet\` in \`UndoManager\`.
5. Buffer emits \`Changed\`; bound adapters update rendering.
6. Sample refreshes status/dirty indicators.

Avoid direct \`document.Buffer.Replace(...)\` when undo and dirty-state behavior matter: the public mutable buffer is a known boundary gap. Do not extend that pattern in new code. Restricting the public API later requires an ADR and compatibility review.

## 5. Adding a Core feature or command

**Recommended sequence:**

1. Define observable behavior: inputs, result, failures, Unicode/line endings, undo and cancellation.
2. Search existing commands and services first; avoid redundant state machines.
3. Design tests for empty text, boundaries, Unicode, errors and cancellation.
4. Put text/domain behavior in Core, UI interaction in WinForms and user decisions in the host.
5. Extend an existing handler or add an \`IEditorCommand\` implementation.
6. Register it, add an \`EditorCommandIds\` entry if appropriate and update \`docs/spec/Command-Ids.md\` for stable public IDs.
7. Add XML documentation and comments describing invariants, not merely syntax.
8. Check Core tests, Windows compilation, documentation and changelog.

A command invocation is **not permission** to delete arbitrary files, execute macros or open network connections. Extensions crossing security boundaries need a dedicated ADR.

## 6. Integrating a host or UI adapter

The WinForms adapter currently exposes \`SasdEditorView.Document\`, \`ViewState\`, \`CommandDispatcher\` and \`ExecuteCommandAsync\`. The FIRST-ED sample demonstrates binding documents and handling menus, file operations and search prompts.

A host should:

- share the same \`TextDocument\` rather than duplicate editable text;
- keep distinct view states for distinct editor views;
- listen to buffer changes and invalidate relevant rendering;
- own file pickers, Save/Discard/Cancel and error presentation;
- use \`IsDirty\` and metadata for status and close workflows;
- respect the UI framework's thread-affinity rules;
- test control characters, IME, surrogate pairs and multiline paste.

**Do not assume availability yet** of a complete split-view host, JSON key-profile loader, WPF control or web adapter. Examples in the implementation specification describe desired future behavior.

## 7. Files and encodings

\`FileDocumentStorage\` implements stream operations in \`IDocumentStorage\` and additionally exposes path-based methods.

\`\`\`csharp
using Sasd.EditorToolkit.Storage;

var storage = new FileDocumentStorage();
var loaded = await storage.LoadFileAsync("example.txt");

if (loaded.BinaryContentSuspected)
{
    // The host must make an explicit decision.
    // The current M1 library does not display a prompt.
}

loaded.Document.Insert(new(0, 0), "Note: ");
await storage.SaveFileAsync(loaded.Document);
\`\`\`

**Rules:** preserve encoding/BOM deliberately; do not call \`MarkSaved\` after a failed write; host validates authorized paths; prevent clobbering external changes before release; design autosave/recovery with explicit security and privacy controls.

**Current limits:** loading buffers the complete file; binary detection is basic; true large-file streaming and file-conflict handling remain future work. Temp-file replacement using \`File.Move(overwrite: true)\` is only as atomic as the underlying filesystem permits.

## 8. Unicode, search, selection and performance

- Current \`TextPosition.Column\` corresponds to UTF-16 code-unit indices in the C# implementation. User-facing caret movement needs stronger grapheme-boundary guarantees.
- \`TextSearchService\` currently provides literal find with direction, wrap and \`StringComparison\`; no complete replace engine or general regex support yet.
- \`FindTextCommand\` updates selection and caret in Core, not in WinForms.
- \`LineTextBuffer\` reconstructs complete text on changes; \`Length\` and offset mapping are not optimized for very large documents.
- Compare benchmarks using reproducible fixtures, warm-ups and specified hardware. Do not promise unmeasured performance.
- Error and cancellation handling must not leave partially modified documents.

## 9. Document, validate and commit

- Observable behavior change → \`docs/spec/\` and tests.
- Architectural choice → new/updated \`docs/adr/\`.
- User-visible function → user guide, DE/EN docs and changelog.
- Architecture change → \`Architecture.md\` and \`Architektur.md\`.
- Cross-subsystem change or public API modification → ExecPlan under \`docs/en/PLANS.md\`.
- Historically inspired capability → historical traceability and requirement IDs.

Before completion, check \`dotnet test\`, \`git diff --check\`, worktree status and Windows CI. Never discard unrelated work, commit secrets/user files or force-push. Direct development on \`main\` follows the repository's current single-developer AGENTS policy.

## 10. Troubleshooting

| Observation | Investigation |
|---|---|
| Linux Core green; WinForms unknown | Check Windows CI; do not claim whole-solution validation |
| Command returns \`NotHandled\` | Registration, \`CanExecute\` and parameter type |
| Search unexpectedly misses text | Start position, comparison, direction and wrap |
| Dirty state unexpected | Saved snapshot, direct buffer mutations and undo path |
| Line endings/BOM change | Encoding metadata, \`DocumentSaveOptions\` and input fixture |
| UI does not redraw | Buffer event, view binding, invalidation and UI thread |
| Huge file slow | M1 buffer is not large-file ready; measure and plan later implementation behind interface |

For actual implementation priorities, follow [NEXT-WORK](../ai/NEXT-WORK.md), not speculative roadmap expansion.
