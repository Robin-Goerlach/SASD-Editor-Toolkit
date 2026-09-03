# ADR-0002: Language-neutral core concepts

## Status

Accepted.

## Decision

The concepts Document, Text Buffer, Position, Range, View, Workspace, Command and Adapter are part of the product model, not only C# class names.

## Consequences

- Documentation uses these names consistently.
- Command IDs are stable strings.
- Settings and keyboard profile formats are designed for later cross-language reuse.
- APIs should feel familiar across languages while staying idiomatic.
