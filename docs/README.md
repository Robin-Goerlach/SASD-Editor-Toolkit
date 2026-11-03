# SASD Editor Toolkit — documentation

Reader-facing documentation is grouped by language; shared engineering materials remain grouped by purpose.

**File naming:** reader-facing document names match their language: `docs/de/Lastenheft.md` ↔ `docs/en/Requirements-Specification.md`, `docs/de/Pflichtenheft.md` ↔ `docs/en/Implementation-Specification.md`, and `docs/de/Historische-Nachverfolgung.md` ↔ `docs/en/Historical-Traceability.md`. Conventional repository filenames such as `README.md`, `CHANGELOG.md`, `SECURITY.md` and `AGENTS.md` retain their established names in both languages.

- [English documentation](en/README.md)
- [Deutsche Dokumentation](de/README.md)

## Bilingual product documentation

| German | English | Purpose |
|---|---|---|
| [Architektur](de/Architektur.md) | [Architecture](en/Architecture.md) | M1 code, target design, ownership and decisions |
| [Entwicklerhandbuch](de/Entwicklerhandbuch.md) | [Developer Guide](en/Developer-Guide.md) | Build, embedding and extension |
| [Testhandbuch](de/Testhandbuch.md) | [Testing Guide](en/Testing-Guide.md) | Automated/manual verification and evidence |
| [Benutzerhandbuch](de/Benutzerhandbuch.md) | [User Guide](en/User-Guide.md) | Current Modern FIRST-ED and its limitations |
| [Historische Nachverfolgung](de/Historische-Nachverfolgung.md) | [Historical Traceability](en/Historical-Traceability.md) | Historical concepts mapped to SASD |
| [Release-Checkliste](de/Release-Checkliste.md) | [Release Checklist](en/Release-Checklist.md) | Future acceptance and release gates |

These guides describe the **independent SASD Editor Toolkit**. Design targets are not proof of implemented features or product approval.

## Shared technical and historical material

| Directory | Contents |
|---|---|
| [`adr/`](adr/) | Architecture Decision Records, including [ADR-0006 (proposed standalone boundaries)](adr/ADR-0006-standalone-toolkit-architecture.md) |
| [`spec/`](spec/) | Language-neutral editor/command/settings/conformance contracts |
| [`ai/`](ai/) | Codex autonomy rules, current-state snapshot and ordered next work |
| [`notes/`](notes/) | Historical engineering notes, reviews and worklogs |
| [`research/`](research/) | Research, sources and archived context |

**Root entry points:** [README](../README.md), [AGENTS](../AGENTS.md) and [LICENSE](../LICENSE) stay in the repository root because GitHub and development tooling expect them there.

**English repository documents:** [CHANGELOG](en/CHANGELOG.md), [CONTRIBUTING](en/CONTRIBUTING.md), [SECURITY](en/SECURITY.md), [PLANS](en/PLANS.md) and [GitHub description](en/GITHUB_DESCRIPTION.txt). [German translations](de/README.md) accompany them; GitHub discovery pointers remain under `.github/`.

**Translation status (3 November 2026):** The [English requirements specification](en/Requirements-Specification.md) and [English implementation specification](en/Implementation-Specification.md) now complement their authoritative [German originals](de/README.md). Translation completeness was checked against requirement identifiers, all 46 implementation-specification chapters, 208 traceability rows, use cases, command IDs and historical procedure names. Roadmap, language strategy, changelog, contributing instructions, security policy, execution-plan rules and GitHub description also have German and English counterparts. The detailed Codex runbook remains canonical in English under `docs/ai/CODEX-AUTONOMY.md`, with a full German translation in `docs/de/CODEX-AUTONOMIE.md`. Language-neutral specs and ADRs remain in shared technical directories.

This is the independent SASD Editor Toolkit documentation; SASD Editor Toolbox is a different repository.
