# Branch Scope - PR 2

Branch: `feat/m1a-core-hardening`

This branch intentionally stays inside the Editor Toolkit repository and keeps the roadmap discipline from `AGENTS.md`.

## Product code touched

- Core storage.
- Core command catalog and default registry.
- WinForms adapter notification surface.
- Modern FIRST-ED sample host.

## Product code not touched

- WPF.
- Web.
- Java.
- C++.
- PHP.
- Syntax highlighting.
- Large-file buffer implementations.
- Plugin/macro systems.

## Review priority

1. Does the branch build on Windows with .NET 10?
2. Do all tests pass?
3. Does the sample still open and edit text?
4. Are the new storage tests correct for BOM/preamble behavior?
5. Is the dirty-document sample prompt acceptable for M1b?
