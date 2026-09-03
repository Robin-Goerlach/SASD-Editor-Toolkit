# ADR-0005: Unicode and indexing

## Status

Accepted.

## Decision

The C# implementation uses .NET strings and may expose UTF-16 code-unit positions. The language-neutral specification focuses on observable behavior and conformance tests.

Future C++ implementations may use UTF-8 internally where that better matches native/Linux expectations.

## Consequences

- C# APIs remain practical for .NET developers.
- The project does not force UTF-16 on every future implementation.
- Grapheme-safe caret movement remains an explicit quality goal.
