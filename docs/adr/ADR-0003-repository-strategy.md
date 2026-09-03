# ADR-0003: Repository strategy

## Status

Accepted.

## Decision

Start with one repository: `SASD-Editor-Toolkit` for the C#/.NET reference implementation and practical specs.

Later full language implementations get their own repositories only when there is concrete demand.

## Consequences

- M1 remains small.
- Build systems do not interfere with each other.
- A future `SASD-Editor-Toolkit-Spec` repository may be split out if the spec becomes independently useful.
