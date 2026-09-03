# Next M1a Steps

These are practical follow-up tasks after `feat/m1a-core-hardening`. They keep the project within the Modern FIRST-ED 0.1 path.

## High priority

1. Add `Edit.DeleteRight` support to the WinForms surface so the Delete key uses the same behavior as the Core command.
2. Replace direct sample Undo/Redo calls with `EditorCommandDispatcher` calls.
3. Add a minimal Save/Discard/Cancel close-guard abstraction to Core instead of keeping that behavior only in the WinForms sample.
4. Add storage result/error objects before broadening file handling.
5. Add a small search service with literal forward search only.

## Medium priority

1. Add `Edit.SelectAll`, `Edit.Copy`, `Edit.Cut` and `Edit.Paste` contracts.
2. Add a minimal keyboard profile file format and one `Modern` profile.
3. Add line-ending display to the sample status bar.
4. Add basic architecture tests for Core dependency boundaries.

## Explicitly later

- WPF adapter.
- Web adapter.
- Syntax highlighting.
- Piece Table/Rope.
- Full MicroStar demo.
- Java/C++ implementation work.
- PHP integration work.
