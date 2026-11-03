# User Guide — Modern FIRST-ED (SASD Editor Toolkit)

[Deutsch](../de/Benutzerhandbuch.md) · [Documentation index](README.md) · [Architecture](Architecture.md)

**Scope:** the current **WinForms example application** of the independent SASD Editor Toolkit.  
**Snapshot:** 2026-11-03 · **Status:** M1 development version, **not a production-certified editor for critical files**.

## 1. What is this application?

Modern FIRST-ED demonstrates how an independent editor library can be embedded into a Windows application. It provides a custom-drawn text area, menus and toolbar, a status display, and basic file, editing and search workflows.

The application is currently a **reference/test host**, not a full word processor, IDE or complete recreation of the Turbo Editor Toolbox. This guide describes features grounded in the current source code.

## 2. Requirements and launch

- Windows with .NET SDK 10.x and support for building WinForms desktop applications.
- A local clone of this repository.
- Use **copies instead of original files** for early tests, especially for important or unusually encoded documents.

From the repository root:

\`\`\`powershell
dotnet restore .\SASD-Editor-Toolkit.sln
dotnet run --project .\samples\Sasd.EditorToolkit.Sample.FirstEd.WinForms\Sasd.EditorToolkit.Sample.FirstEd.WinForms.csproj
\`\`\`

If the SDK is not installed, see the Developer Guide. This document does not imply an installer, packaged distribution or production release exists.

## 3. Screen layout

| Area | Purpose |
|---|---|
| **File** | New, Open, Save, Save As and Exit |
| **Edit** | Undo and Redo |
| **Search** | Find and Find Next |
| **View / Tools / Help** | Currently menu placeholders/development surface; not complete feature sets |
| **Toolbar** | Quick access to New, Open, Save, Undo, Redo, Find and Find Next |
| **Editor surface** | Text, line numbers, highlighted current line, caret and search selection |
| **Status bar** | Host-generated document and position information |

Creating a new document currently supplies **demo text**; it does not necessarily start with an empty file.

## 4. Working with files

### New

Choose **File → New** to create a new example document. If the existing document has changed, the host asks how to handle unsaved edits.

### Open

**File → Open** launches a Windows file picker. Text-like extensions such as \`.txt\`, \`.md\`, \`.cs\`, \`.json\` and \`.xml\` are suggested; other files can be shown through the file filter.

**Caution:** the toolkit includes initial encoding/BOM detection, but does not yet safely handle every binary format or unsupported encoding. Do not use binary files or irreplaceable originals as test data.

### Save

**File → Save** writes to the current path; unnamed documents require a destination. **Save As** requests a different path.

For filesystem paths, the Core currently uses a temporary file followed by replacement by default. This is **not a substitute for backup or fully tested recovery**. External modifications are not yet comprehensively detected.

### Exit / close guard

When choosing **Exit**, opening another file or creating a new document with unsaved changes, the host provides a **Save / Discard / Cancel** decision. Cancel aborts the pending action.

## 5. Keyboard and editing

The existing M1 reference routes basic actions through Core commands:

| Input | Behavior |
|---|---|
| Type a character | Insert text at the caret |
| Enter | Insert newline |
| Backspace / Delete | Delete left/right; may join lines at boundaries |
| Arrow keys | Move caret left/right/up/down |
| Home / End | Move to logical line start/end |
| Ctrl+Z / Ctrl+Y | Undo / Redo |
| Find / Find Next | Enter a search term / repeat the previous search |

**Do not assume yet:** fully functional mouse selection, Copy/Cut/Paste workflows, complete grapheme-aware navigation, user-configurable keyboard profiles, WordStar prefix commands, word-wrap menus or advanced block editing. These areas remain future work.

## 6. Finding text

1. Choose **Search → Find…** or the toolbar **Find** action.
2. Enter and confirm a search term.
3. The UI-independent Core finds a match and updates selection/caret state.
4. **Find Next** repeats the last term entered in the sample.

The underlying search service supports direction and text comparison options; the sample does **not expose all of them** as dialog options yet. Regular expressions, general replacement and advanced filters are not promised features of this UI.

## 7. Features not yet available

- Not a general-purpose IDE or full WYSIWYG editor.
- No finished MicroStar/WordStar-compatible keyboard profile.
- No fully integrated split-view/multiview UI in the sample.
- No general Replace All or regex UI.
- No performance guarantee for very large files.
- No complete accessibility/IME acceptance.
- No production-approved autosave/crash recovery.
- No complete external-file-change warning mechanism.

The roadmap records the intended extensions. A concept screenshot is **not evidence** that every shown feature exists.

## 8. Troubleshooting

| Symptom | Suggested check |
|---|---|
| Application will not launch | Windows/.NET SDK, restore/build and CI |
| Cannot open file | Permissions, file type, encoding; preserve original |
| Search misses expected text | Term, start position, subsequent Find Next |
| Unwanted edit | Try Undo before saving again |
| Save fails | Check permissions/path; keep text; record error |
| Large document slow | M1 buffer is not yet a large-file implementation |
| Display/IME/focus wrong | Record Windows version, DPI, input method, reproduction |

## 9. Reporting a bug

Report the commit SHA, Windows/.NET version, short reproduction, expected and observed behavior, and preferably a synthetic **non-confidential** sample file. Do not publish sensitive document contents or personal paths.

See [Testing Guide](Testing-Guide.md), [Developer Guide](Developer-Guide.md) and [Roadmap](ROADMAP.md).
