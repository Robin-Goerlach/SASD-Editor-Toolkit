# ADR-0006: Independent Editor Toolkit architecture and host boundaries

## Status

**Proposed — architectural clarification only.** The existing code and ADR-0001 to ADR-0005 remain in force. This proposal is not authorization to change public APIs or release packages.

## Date

2026-11-03

## Context

SASD Editor Toolkit was created as an independent reusable editor library, with a UI-independent C#/.NET Core and an initial WinForms adapter. The initial requirements and implementation specification describe many later features and conceptual interfaces. The implementation now includes a working M1 foundation, but several target contracts have not been implemented.

The architecture must remain valid even if **no SASD Editor Toolbox migration** ever occurs. The separate SASD UI Toolkit also remains optional. The concrete issue is maintaining one authoritative document/undo state as host surfaces and future language implementations grow.

## Decision proposed

1. Keep \`Sasd.EditorToolkit.Core\` as the authoritative domain and application-service boundary. Do not introduce a new shared \`Abstractions\` assembly merely to mirror an architectural diagram.
2. Keep WinForms and future UI adapters dependent on Core, with host-owned dialogs, file selection and security decisions. Core must never depend on an adapter.
3. Treat \`TextDocument\` as the mutation/undo authority; develop a separately reviewed contract to prevent direct public \`ITextBuffer\` mutation from bypassing undo. Do **not** silently break the current API in this documentation change.
4. Keep command IDs and observable semantics in \`docs/spec/\`; serializable settings and keyboard-profile formats are candidate cross-language contracts, **not proof of working loaders**.
5. Prefer independent idiomatic future Java/C++ implementations with conformance tests. Do not require a shared native C++ runtime or C ABI until a demonstrated integration use case warrants an ADR with ownership/encoding/threading rules.
6. Treat the full Lastenheft/Pflichtenheft as **target specifications**. Mark current features, open milestones and release gates separately.
7. Adopt only tested host-facing boundaries. New plugins/macros/native bridges require explicit security and compatibility reviews.

## Options considered

| Option | Benefit | Cost / reason not selected now |
|---|---|---|
| Keep Core + WinForms and improve contracts incrementally | Small, testable, works with current project structure | Requires discipline and staged API evolution |
| Introduce multiple layered assemblies immediately | Visible physical separation | More packages, versioning and DI without proven benefit |
| Make WinForms the editing source of truth | Easy short-term UI prototypes | Breaks headless use, multi-view semantics and portability |
| Force native C++ core with wrappers for all languages | One native engine | ABI, lifetime, Unicode, thread and support complexity too early |

## Consequences

- Current M1 implementation continues without a forced reorganization.
- Public mutation/ownership semantics require an explicit later ADR before breaking changes.
- Architecture and developer/testing/user docs must distinguish implemented, planned and accepted behavior.
- Future adapters remain possible but are not scheduled or assumed to exist.
- Add automated dependency-boundary checks after M1c and before broad M2 work.
- This proposal is **not a claim of completed design approval**; accepted ADRs and user instructions retain precedence.

## Related documents

- [English architecture](../en/Architecture.md) · [German architecture](../de/Architektur.md)
- [Editor model](../spec/Editor-Model.md), [Text buffer behavior](../spec/Text-Buffer-Behavior.md)
- [Roadmap](../en/ROADMAP.md) and [Current state](../ai/CURRENT-STATE.md)
