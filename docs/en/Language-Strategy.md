# Multi-language strategy

[Deutsch](../de/Mehrsprachenstrategie.md) · [English documentation index](README.md)

## Decision

SASD Editor Toolkit starts with **C#/.NET as its reference implementation**. Additional languages are anticipated through shared specifications but are not developed in parallel during M1.

> Build one reliable product first, then ports; design the specifications to remain portable from the beginning.

## Why not develop all ports simultaneously?

Parallel C#, Java, C++, Web and PHP development multiplies build systems, package managers, UI frameworks, Unicode representations and test environments, endangering the small M1 scope.

## Target languages

| Language/platform | Intended role | Planning status |
|---|---|---|
| C#/.NET | Full reference implementation | Active |
| Java | Possible full port | After stable C# behavior/API |
| C++ | Possible native Linux/desktop/embedding port | After stable specification |
| TypeScript/Web | Adapter/bridge, possibly CodeMirror/Monaco or Blazor | Later |
| PHP | Limited integration library, not first full editor core | When needed |
| Rust | Research/watchlist | No commitment |

## PHP assessment

PHP is useful for SASD web hosting and server-side logic, but not the first choice for an interactive editor core. A later PHP component could offer:

- common settings and keyboard-profile read/write;
- command-ID validation;
- server-side search/replacement;
- web-host import/export;
- storage adapters for SASD web applications.

A complete PHP editor core only makes sense for a concrete host requirement.

## API strategy

Do not enforce mechanically identical APIs; preserve reusable concepts and externally observable behavior.

**Stable across languages:** Document, Buffer, View, Position, Range, Selection, Command and Workspace concepts; command IDs; settings; keyboard profiles; observable behavior; conformance vectors.

**Idiomatic per language:** error handling, async/threading, string and memory models, package conventions and UI adapters.

## Unicode and positions

C# and Java tend toward UTF-16; C++ on Linux often prefers UTF-8.

- C# may expose UTF-16 code-unit positions.
- Language-neutral specifications describe behavior, not .NET internals.
- A C++ implementation may use UTF-8 internally when it passes the same conformance tests.
- Caret navigation must not deliberately corrupt composed visible characters.

## Repository strategy

Initially: `SASD-Editor-Toolkit` — C#/.NET reference implementation with shared practical specifications.

Only where real demand warrants it, possible siblings include:

- `SASD-Editor-Toolkit-Spec`
- `SASD-Editor-Toolkit-Java`
- `SASD-Editor-Toolkit-Cpp`
- `SASD-Editor-Toolkit-Web`
- `SASD-Editor-Toolkit-PHP` (integration/server package, not a rich UI commitment).

This strategy is about the independent SASD Editor Toolkit; it does not require an Editor Toolbox migration.
