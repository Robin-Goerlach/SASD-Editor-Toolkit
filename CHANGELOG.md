# Changelog

## Unreleased

- Hardened M1a storage handling for BOM/preamble detection and preservation.
- Added storage tests for UTF-8 BOM, UTF-32 BOM ordering, binary-content suspicion and file overwrite metadata.
- Added the M1 `Edit.DeleteRight` command and dispatcher coverage tests.
- Added caret navigation commands for left, right, up, down, line start and line end.
- Routed WinForms editor keyboard input through the command dispatcher for printable text, Enter, Backspace, Delete, arrows, Home, End, Ctrl+Z and Ctrl+Y.
- Exposed command dispatching through `EditorSurface` and `SasdEditorView` for host applications.
- Added dirty-document prompts to the Modern FIRST-ED sample before New, Open and Exit replace the current document.
- Disabled XML documentation generation for test and sample assemblies while keeping it enabled for product assemblies.

## 0.0.0-repository-seed - 2026-09-03

- Added repository seed for the C#/.NET reference implementation.
- Added Core, WinForms, sample and test project structure.
- Added practical roadmap, AGENTS instructions, ADRs and language-neutral specs.
- Added German Lastenheft and Pflichtenheft as source documents.
- Added UI concept screenshot to README.
