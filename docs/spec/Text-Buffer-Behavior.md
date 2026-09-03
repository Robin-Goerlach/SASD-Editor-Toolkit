# Text Buffer Behavior

## M1 default buffer

The M1 reference implementation uses a line-oriented buffer. This is intentionally simple and testable. It is not the final large-file strategy.

## Required behavior

- Empty documents contain one addressable empty line.
- Line endings may be CRLF, LF, CR or None.
- Implementations must preserve supported line endings unless a save policy says otherwise.
- Insert, delete and replace operations are atomic from the public API perspective.
- Positions and ranges must be normalized or rejected deterministically.
- Implementations may use different internal encodings as long as observable behavior and conformance tests match.

## Future buffers

A later C# version may add Piece Table, Rope or another large-file strategy behind the same behavioral contract.

A future C++ implementation may use UTF-8 internally when that fits native/Linux usage better.
