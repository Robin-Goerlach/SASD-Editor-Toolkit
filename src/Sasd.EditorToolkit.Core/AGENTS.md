# AGENTS.md - Sasd.EditorToolkit.Core

- Core must compile without WinForms/WPF/ASP.NET/browser references.
- Do not use UI types, dialogs, message boxes, controls, clipboard APIs or platform-specific input events.
- Observable editor behavior belongs here or in a Core service; presentation belongs in adapters.
- Preserve document encoding/BOM/line-ending metadata through storage operations.
- Treat file paths/content as untrusted input and avoid data-loss-prone defaults.
- Public APIs require useful XML documentation.
- New stable command IDs must be documented in `docs/spec/Command-Ids.md`.
- Cross-language behavior changes require a `docs/spec/` update.
- Editing mutations need deterministic dirty-state and undo/redo semantics.
- Search/edit logic must be testable headlessly.
- Do not optimize the buffer architecture prematurely; keep replacement behind existing abstractions.
- Add/update Core tests with every behavior change.
