# AGENTS.md - SASD Editor Toolkit

These instructions apply to humans and AI coding agents. The repository is intentionally prepared for autonomous Codex work.

## Read this first

Before changing code, read:

1. `docs/ai/CURRENT-STATE.md` - dated repository snapshot and known limitations.
2. `docs/ai/NEXT-WORK.md` - ordered autonomous work queue.
3. `docs/ai/CODEX-AUTONOMY.md` - detailed execution, validation and stop rules.
4. `ROADMAP.md` - milestone boundaries.
5. Relevant files in `docs/spec/` and `docs/adr/`.

For work spanning several subsystems or changing public contracts, also read `PLANS.md`.

More-specific `AGENTS.md` files exist below `src/`, `samples/` and `tests/`. Read the one that governs every subtree you edit.

## Authority order

When guidance conflicts, use this order:

1. Current user/task instructions.
2. This `AGENTS.md` and applicable nested `AGENTS.md` files.
3. Accepted ADRs and language-neutral specs.
4. `ROADMAP.md`.
5. Current-state/work-queue documents.
6. Historical notes in `docs/notes/`.

A dated or branch-specific note must never override a newer repository-wide rule.

## Mission

Build a practical, understandable, reusable editor toolkit. C#/.NET is the reference implementation. Core behavior must stay UI-independent and portable enough for later Java, C++, Web or other adapters.

The historical Turbo Editor Toolbox is a behavioral inspiration/reference only. Do not copy historical source code, manual text, scans, logos or product identity.

## Autonomous operating mode

Unless the user says otherwise:

- work directly on `main`; this repository is currently single-developer;
- begin with `git status --short --branch` and `git pull --ff-only`;
- do not create a branch merely for routine work;
- do not force-push, rewrite history, amend published commits or use destructive resets;
- choose the first unfinished item in `docs/ai/NEXT-WORK.md` when the user says only “continue”;
- make reasonable local design decisions from the specs/ADRs without asking for routine confirmation;
- keep going through implementation, tests, documentation and a clean commit when the task is well-defined;
- fix failures caused by your change instead of stopping after the first red build;
- leave the worktree clean.

Ask the user only when a stop condition in `docs/ai/CODEX-AUTONOMY.md` applies.

## Architecture invariants

- `Sasd.EditorToolkit.Core` must not reference WinForms, WPF, ASP.NET, browser APIs or UI-specific types.
- UI adapters depend on Core; Core never depends on UI adapters.
- Samples contain host/demo logic, not reusable product logic.
- Command IDs are stable language-neutral strings and must be reflected in `docs/spec/Command-Ids.md`.
- Settings and keyboard profiles must remain serializable.
- Prefer small explicit interfaces over hidden/global mutable state.
- No hidden singleton editor state.
- Preserve Unicode correctness; do not intentionally split surrogate pairs or user-visible combined characters in caret operations.
- Treat paths, file contents and serialized settings as untrusted input.
- Do not add macro/script execution without a separate security ADR.

## Coding and documentation

- Public code, identifiers and package names: English.
- Use nullable reference types and XML documentation for public product APIs.
- Comments should explain intent, invariants, historical compatibility boundaries or trade-offs; do not narrate obvious syntax.
- Prefer correctness and clarity over micro-optimization. Performance redesigns belong behind stable interfaces and should be justified by measurement.
- Keep cross-language observable behavior in `docs/spec/`.
- Update English and German documentation in parallel when a user-visible or architectural behavior changes.
- Update `CHANGELOG.md` for meaningful behavior changes.

## Validation

On Windows, the required full check is:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\verify-all.ps1
```

On Linux/macOS, at minimum run:

```bash
./scripts/verify-core.sh
```

The full solution contains `net10.0-windows` WinForms projects. A non-Windows agent must not pretend it validated them; rely on Windows GitHub Actions for the full solution and report that distinction.

After every code change, run the strongest validation available. Documentation-only changes may skip compilation only when no project/build metadata changed.

## Git completion rule

A task is not complete until:

- relevant tests pass locally or the platform limitation is documented;
- docs/specs are updated where behavior changed;
- `git diff --check` is clean;
- `git status --short` is understood;
- the change is committed with a focused message;
- if network access exists, the commit is pushed and GitHub Actions is checked;
- failures introduced by the commit are fixed before declaring success.

Suggested commit style:

```text
feat(core): add replace command
fix(storage): preserve line endings on save
test(search): cover wrapped backward search
docs(codex): clarify autonomous workflow
adr: record security boundary
```
