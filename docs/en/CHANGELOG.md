# Changelog

## Unreleased

- Renamed English requirements documents to `Requirements-Specification.md` and `Implementation-Specification.md`, and the German historical traceability file to `Historische-Nachverfolgung.md`. Updated links across the repository without changing requirement identifiers or product behavior.

- Added bilingual architecture baseline, developer/testing/user guides, historical traceability and future release checklist for the independent Toolkit. Documented observed M1 code separately from target design, recorded ADR-0006 as proposed and updated navigation. No product code changed.

- Added complete English translations of the German Lastenheft and Pflichtenheft. Verified 30/46 major chapters, all 208 requirement traceability rows, 15 use cases, 67 command catalog IDs and historical procedure inventories against the German originals; updated bilingual documentation navigation. No product code changed.


- Added German counterparts for the changelog, contributing guide, security policy, execution-plan rules and GitHub repository description; expanded the German Codex autonomy guide into a full translation of the English canonical runbook; updated bilingual documentation indexes. At that stage, English requirements translations were outstanding; they were added in a subsequent documentation update.


- Moved the full repository changelog, contributing/security guidance, execution-plan rules and GitHub description into `docs/en/`; removed the superseded root roadmap pointer and retained GitHub discovery entry points under `.github/`. Updated related documentation references without changing editor behavior.

- Reorganized bilingual documentation navigation and added English/German roadmap locations under `docs/` with an initial root roadmap pointer (subsequently retired). No product behavior changed.

- Added a Codex-oriented autonomous development harness: root/subtree AGENTS instructions, execution-plan rules, current-state and next-work documents, bilingual autonomy guidance and platform-aware verification scripts.
- Formalized M1c search/selection completion in the roadmap and clarified that historical branch stop notes are not a repository-wide freeze.

- Added UI-independent M1c text search service with forward, backward, wrapping and comparison options.
- Added `Search.Find` command handling that updates selection and caret state through the Core command model.
- Added simple Find / Find Next support to the Modern FIRST-ED WinForms sample.
- Hardened M1a storage handling for BOM/preamble detection and preservation.
- Added storage tests for UTF-8 BOM, UTF-32 BOM ordering, binary-content suspicion and file overwrite metadata.
- Added the M1 `Edit.DeleteRight` command and dispatcher coverage tests.
- Added caret navigation commands for left, right, up, down, line start and line end.
- Routed WinForms editor keyboard input through the command dispatcher for printable text, Enter, Backspace, Delete, arrows, Home, End, Ctrl+Z and Ctrl+Y.
- Exposed command dispatching through `EditorSurface` and `SasdEditorView` for host applications.
- Added dirty-document prompts to the Modern FIRST-ED sample before New, Open and Exit replace the current document.
- Disabled XML documentation generation for test and sample assemblies while keeping it enabled for product assemblies.

## 0.0.0-repository-seed - 2026-09-03

- Added repository seed for the C#/.NET reference implementation.
- Added Core, WinForms, sample and test project structure.
- Added practical roadmap, AGENTS instructions, ADRs and language-neutral specs.
- Added German Lastenheft and Pflichtenheft as source documents.
- Added UI concept screenshot to README.
