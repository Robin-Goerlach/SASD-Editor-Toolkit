# Release and Acceptance Checklist — SASD Editor Toolkit

[Deutsch](../de/Release-Checkliste.md) · [Roadmap](ROADMAP.md) · [Testing Guide](Testing-Guide.md) · [Architecture](Architecture.md)

**Status:** future-looking process for the **independent** Editor Toolkit. **No release has been approved by this document.** Snapshot: 2026-11-03.

## 1. Release principle

A running FIRST-ED example, a green CI job or a complete documentation set does not by itself authorize a public product release. A release needs evidenced requirements, reproducible artifacts, safe file operations, a supportable API and explicit Product Owner approval.

## 2. Required gates

| Gate | Required evidence | Status on 2026-11-03 |
|---|---|---|
| Scope | approved M1/M2/M3 boundary and known non-goals | Roadmap exists; full v1 acceptance not complete |
| Functionality | passing tests for applicable \`AK-001\`–\`AK-015\` | not fully demonstrated |
| Data integrity | encoding/BOM, line endings, failures, conflict-safe saves, close guard | partial; conflict/recovery outstanding |
| Architecture | Core/UI boundaries, command IDs, no cycles, mutation/undo contract | design documented; automated gates pending |
| Windows | full-solution build/tests for exact commit SHA | CI exists; fresh result needed at release |
| Linux/macOS | Core restore/build/tests | script exists; not a full GUI check |
| Usability | UI smoke, DPI, focus, IME, keyboard, accessibility | complete acceptance evidence pending |
| Security | source/license review, path/input handling, logging, no macro execution | written principles exist |
| Packaging | NuGet packages, metadata, XML documentation, checksums | M3 target |
| Consumer | clean external host using published packages | not accepted |
| Documentation | DE/EN specifications, architecture, developer/test/user guides, traceability | present; must match actual release |
| Approval | signed-off SHA, artifacts and known risks | **pending** |

## 3. Release procedure

1. Freeze target version and scope; document every deliberate deferral.
2. Close open security and data-loss risks.
3. Run all applicable automated checks on the **exact commit**.
4. Record manual Windows/UI evidence on the same revision.
5. Build packages, review license and metadata, record checksums.
6. Install packages in a clean external consumer sample.
7. Reconcile README, DE/EN guides, changelog, limits and upgrade notes.
8. Prepare release record; Product Owner makes the decision.
9. Only then create tag/release/publish artifacts; document rollback and support route.

## 4. Release record template

\`\`\`text
Product: SASD Editor Toolkit
Target version:
Commit SHA:
Artifacts and checksums:
Windows CI:
Linux/macOS Core:
Manual UI/accessibility checks:
AK-001 ... AK-015: PASS | FAIL | NOT RUN | DEFERRED with rationale
Open issues / accepted risks:
License, trademark and security review:
Documentation revision:
Product Owner / approval:
Date:
Rollback/hotfix plan:
\`\`\`

Creating documentation or a Proposed ADR does not accept a product milestone. See [Testing Guide](Testing-Guide.md) and [Historical Traceability](Historical-Traceability.md).
