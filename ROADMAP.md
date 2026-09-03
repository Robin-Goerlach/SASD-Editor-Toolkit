# Roadmap

This roadmap turns the broad Lastenheft/Pflichtenheft into small, buildable increments. The full Pflichtenheft is the product backlog, not the M1 implementation order.

## Strategic line

1. Build a working C#/.NET reference implementation first.
2. Keep the Core UI-independent from the first commit.
3. Keep the first product usable and testable, not academically overdesigned.
4. Document language-neutral behavior in short practical specs.
5. Add other language implementations only after the reference behavior is stable.

## M0 - Repository and scope control

Goal: create a clean starting point.

Deliverables:

- repository skeleton;
- imported Lastenheft/Pflichtenheft under `docs/de/`;
- practical `docs/spec/` documents;
- ADRs for reference implementation, repository strategy, scope and Unicode/indexing;
- README with screenshot;
- AGENTS.md for human and AI contributors;
- CI seed;
- explicit M1 scope guard.

Exit criteria:

- Repository can be cloned and understood within minutes.
- Nobody can reasonably read the Pflichtenheft as a complete M1 task.
- Historical materials are referenced, not copied.

## M1a - Core editing kernel

Goal: text editing works headless and is covered by tests.

Scope:

- `TextPosition`, `TextRange`, line endings;
- `ITextBuffer` and `LineTextBuffer`;
- `TextDocument` with metadata and dirty state;
- insert text, delete ranges, replace ranges;
- newline handling;
- minimal undo/redo;
- basic file load/save through streams/files;
- core tests.

Not in M1a:

- custom desktop rendering polish;
- syntax highlighting;
- markers;
- full block mode;
- word-wrap/reformat;
- performance engine for huge files.

Exit criteria:

- Core tests pass.
- Core references no WinForms/WPF/ASP.NET/browser types.
- Editing operations preserve Unicode text and line endings at basic level.

## M1b - Modern FIRST-ED WinForms demo

Goal: a small usable desktop demo proves the adapter boundary.

Scope:

- `SasdEditorView` custom WinForms surface;
- basic keyboard input;
- open/save dialogs in sample host;
- status bar;
- toolbar/menu commands routed through command IDs where practical;
- dirty indicator;
- demo text and README screenshot updated from real app when available.

Exit criteria:

- A user can open, edit, save and close a text file.
- UI logic does not duplicate the document model.
- The WinForms adapter remains replaceable.

## M2 - Historical functional breadth

Goal: cover the important editor-toolbox behavior beyond the minimal editor.

Scope:

- multiple documents and multiple views;
- split view and linked views on the same document;
- selection and block operations;
- search/replace improvements;
- marker support;
- keyboard profiles including a Turbo/WordStar-compatible profile;
- configurable settings and themes;
- host-replaceable status, prompts and errors.

Exit criteria:

- A MicroStar-like demo can be started without changing the Core model.
- Command IDs are stable enough for external hosts.
- Key profile format is documented and tested.

## M3 - 1.0 stabilization

Goal: make the .NET implementation publishable as a real library.

Scope:

- API baseline;
- NuGet packaging;
- stronger XML documentation;
- conformance test vectors;
- architecture tests;
- performance smoke tests;
- binary-file warnings and external-change detection;
- optional Piece Table or equivalent large-file buffer spike;
- accessibility improvements.

Exit criteria:

- `Sasd.EditorToolkit.Core` and `Sasd.EditorToolkit.WinForms` are packageable.
- Public APIs are documented.
- M1/M2 behavior is covered by tests and conformance vectors.

## M4 - Web adapter decision

Goal: choose the right web path before writing code.

Preferred options:

- Blazor adapter using the C# Core where appropriate;
- CodeMirror/Monaco integration with SASD command/settings/storage bridge;
- server-side Core services for validation, import/export and text transformations.

PHP is considered here as an integration/server-side package, not as a full rich editor UI.

## M5 - Java or C++ implementation decision

Goal: select the first non-.NET full implementation based on actual demand.

Decision drivers:

- Java first if cross-platform desktop/server reuse is more important.
- C++ first if native Linux/Qt/wxWidgets/GTK embedding is more important.
- Do not start both at the same time.

## M6 - Specialized components

Possible extensions:

- Markdown editor/viewer;
- log viewer;
- configuration editor;
- syntax highlighting;
- hex/text hybrid viewer;
- macro model with explicit security design;
- print service.

## Language support outlook

| Language/platform | Support target | Timing | Notes |
|---|---|---:|---|
| C#/.NET | Full reference implementation | now | Core + WinForms first, WPF/Web later |
| Java | Possible full implementation | after M3 | Good conceptual fit, Swing/JavaFX/SWT decision later |
| C++ | Possible full native implementation | after M3/M5 | Useful for Linux/native, but more expensive |
| TypeScript/Web | Adapter/bridge, likely not full independent core first | after M3 | Prefer existing editor engines where sensible |
| PHP | Limited integration package, not first full editor core | after web decision | Useful for SASD web hosting, storage, validation, transformations |
| Rust | Watchlist only | no commitment | Could be useful for a future portable buffer core |
