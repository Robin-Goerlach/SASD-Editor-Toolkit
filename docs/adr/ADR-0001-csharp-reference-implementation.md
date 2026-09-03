# ADR-0001: C#/.NET as reference implementation

## Status

Accepted.

## Context

The SASD Editor Toolkit is useful beyond C#, but the first practical need is a Windows desktop/editor component for SASD applications.

## Decision

C#/.NET is the first and reference implementation. WinForms is the first adapter. Other languages are not developed in parallel during M1.

## Consequences

- The first repository is C#-centered.
- Core behavior must remain UI-independent.
- Language-neutral specs are kept in `docs/spec/`.
- Later Java/C++ ports should follow behavior and command IDs, not necessarily every C# API detail.
