# Testing Guide and Quality Assurance — SASD Editor Toolkit

[Deutsch](../de/Testhandbuch.md) · [Architecture](Architecture.md) · [Developer Guide](Developer-Guide.md) · [Roadmap](ROADMAP.md)

**Snapshot:** 2026-11-03. **Scope:** independent SASD Editor Toolkit.  
**Test results are commit- and environment-specific.** This document defines verification procedures; it does not claim tests were run when they were not.

## 1. Quality goals

1. Protect text and file integrity ahead of UI polish or speed.
2. Verify functional behavior in UI-independent Core tests.
3. Adapter tests demonstrate interaction and hosting; they never replace Core tests.
4. Record public behavior as examples and, where possible, portable conformance vectors.
5. Do not silently regress architectural boundaries, security or accessibility.
6. A green test job is not a release approval. Missing tests and manual gaps remain visible.

## 2. Test pyramid and current infrastructure

| Layer | Purpose | Present today | Planned work |
|---|---|---|---|
| Unit / Core | Text model, commands, search, undo, storage | xUnit in \`tests/Sasd.EditorToolkit.Core.Tests/\` | Unicode, selection and M1c corner cases |
| Integration | Streams, filesystem, multiview, host boundaries | Some storage/workspace coverage; no full dedicated integration project | Failure, conflict and recovery scenarios |
| Architecture | Dependencies, command-ID uniqueness, no duplicated business logic | Written rules in AGENTS/ADRs | Automated assembly/project/ID checks |
| WinForms | Input, rendering, caret, DPI, IME, dialogs | Sample exists | Repeatable manual UI smoke and possible automation |
| Conformance | Observable behavior across future language ports | Proposed format, no complete vector suite | Machine-readable editing/search vectors |
| Performance | Measurable time/memory budgets | No comprehensive enforced gate | Benchmarks and large-file profiles |
| Release | API, packages, documentation, licensing, consumers | No production release acceptance claimed | M3 gates before publishing |

## 3. Running the existing checks

**Linux/macOS/WSL — Core:**

\`\`\`bash
./scripts/verify-core.sh
\`\`\`

The script restores, builds and tests the Core test project, then executes \`git diff --check\`. It does **not** compile the full WinForms solution.

**Windows — whole solution:**

\`\`\`powershell
powershell -ExecutionPolicy Bypass -File .\scripts\verify-all.ps1
\`\`\`

This restores, builds and tests \`SASD-Editor-Toolkit.sln\`. Equivalent commands:

\`\`\`powershell
dotnet restore .\SASD-Editor-Toolkit.sln
dotnet build .\SASD-Editor-Toolkit.sln -c Debug --no-restore
dotnet test .\SASD-Editor-Toolkit.sln -c Debug --no-build
\`\`\`

**GitHub CI:** \`.github/workflows/ci.yml\` builds/tests on \`windows-latest\`. The current workflow does **not yet** implement every architecture, packaging, release and performance gate described in the implementation specification. Inspect the run associated with the exact commit.

## 4. Risk-based test catalog

| Area | Minimum cases | Main invariant |
|---|---|---|
| Buffer/text | empty file, insert/delete/replace, line boundaries, CRLF/LF/CR, no final newline | deterministic content/ranges |
| Position/Unicode | UTF-16 indices, surrogates, combining marks, RTL, emoji, tabs, long lines | caret/grapheme boundaries |
| Undo/dirty | insert → undo → redo, savepoint, new edit after undo, multiple views | no lost edits |
| Search | forward/backward, wrap, comparison modes, no match, multiline | accurate match ranges |
| M1c replace | short/long replacements, selected text, Unicode, undo, repeated replace | pending slice |
| Storage | UTF-8/16/32 BOM, fallback, invalid UTF-8, binary suspicion, write failures | no loss of original data |
| File conflict | external edit, permissions, cancellation, temp file, concurrent writer | planned hardening |
| Commands | unknown ID, wrong parameter, CanExecute=false, registration, cancellation | no uncontrolled action |
| Workspace | linked views with independent carets; close last view | not yet fully accepted |
| WinForms | keys, focus, resize, DPI, IME, pointer selection, Save/Discard/Cancel | manual Windows evidence |
| Security | malformed JSON, unauthorized paths, document as script, logs | no execution or secret leakage |

Only executed tests count as coverage evidence. Planned tests must not be marked PASS.

## 5. Writing tests in project style

A focused xUnit example:

\`\`\`csharp
using Sasd.EditorToolkit.Documents;
using Sasd.EditorToolkit.Text;
using Xunit;

public sealed class DocumentMutationTests
{
    [Fact]
    public void Undo_restores_text_after_insert()
    {
        var document = new TextDocument(new LineTextBuffer("A"));
        document.Insert(new TextPosition(0, 1), "B");

        Assert.Equal("AB", document.Buffer.GetText());
        Assert.True(document.Undo());
        Assert.Equal("A", document.Buffer.GetText());
    }
}
\`\`\`

Add parameterized boundary cases as needed. Test file I/O in temporary directories and clean up via \`finally\` or \`IDisposable\`. Never use private documents as fixtures. Use controlled failing stream doubles to test write errors, not real user files.

## 6. Manual Windows acceptance

Record **commit SHA, OS version, .NET SDK, resolution/DPI, input method, fixture, outcome, expected and observed behavior** for each run.

| ID | Procedure | Expected outcome |
|---|---|---|
| UI-01 | Start sample; move caret using arrows/Home/End | Caret matches Core text position |
| UI-02 | Type, save, reopen Unicode text | Supported encoding preserves text |
| UI-03 | Undo and redo edits | No text loss |
| UI-04 | Find/Find Next, including no match | Clear selection/status; no crash |
| UI-05 | Try New/Open/Exit with dirty document | Host offers Save/Discard/Cancel |
| UI-06 | Vary window size, font/DPI, focus and IME | No invalid caret or lost text |
| UI-07 | Open suspicious binary input | **Target acceptance**, full host warning not implemented |
| UI-08 | Multiple views/split view | **Planned**, not an M1 PASS without working host path |

Use **PASS**, **FAIL**, **BLOCKED**, **NOT RUN** or **NOT IMPLEMENTED**, as appropriate. Do not call an intentionally postponed feature a failed current-milestone test.

## 7. File integrity and failure scenarios

**No loss of user files** is the most important release gate. Before M3/v1, demonstrate:

- correct BOM preservation and deliberate behavior for BOM-less text;
- mixed line endings and no trailing line terminator;
- no silent overwriting of unsupported/binary input;
- host confirmation when files changed externally;
- atomic replacement where OS/filesystem guarantees permit, with documented fallback;
- handling errors during write/flush/replace without marking the document clean;
- dirty-state guards for Close/Open;
- symlink, permissions and concurrent-writer scenarios on Windows and Linux Core.

Checking only \`File.Exists\` is insufficient: verify content, encoding, metadata, dirty state and undo behavior.

## 8. Portable conformance for future languages

\`docs/spec/Conformance-Tests.md\` proposes a neutral vector format. Until a runner/vector suite is executable, do not claim cross-language conformance.

**Target vector:** initial text/caret, command and parameters, expected final text, caret, selection, dirty, undo/redo and error category. Compare **observable results**, not identical .NET/Java/C++ class implementations. State the platform-specific encoding and coordinate model clearly.

## 9. Acceptance criteria and evidence

Requirements and implementation specifications assign \`AK-001\` through \`AK-015\`; chapter 39 of the implementation specification lists planned verification methods and milestones. It is **not** an already completed acceptance record.

**Slice done:** unambiguous requirement/spec; implemented behavior; passing relevant Core tests; passing Windows CI for UI modifications; disclosed risks/manual gaps; consistent DE/EN documentation; identified commit.

**Release done:** all applicable acceptance criteria plus reproducible packages, license/security review, public API baseline, consumer smoke tests and explicit owner authorization. Documentation alone cannot authorize a release.

## 10. Verification report template

\`\`\`text
Repository: Robin-Goerlach/SASD-Editor-Toolkit
Commit: <full SHA>
Date/timezone:
Platform / SDK:
Scope: Core | full Windows solution | UI | package | release
Commands actually executed:
Automated tests: pass / fail / skipped
Manual cases: UI-01 ... UI-08
Issues / deviations:
Untested areas:
CI run URL:
Result: PASS | FAIL | BLOCKED (for precisely this scope)
Reviewer / sign-off:
\`\`\`

See [Historical Traceability](Historical-Traceability.md) and [Developer Guide](Developer-Guide.md).
