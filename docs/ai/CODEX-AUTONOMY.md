# Codex autonomous development runbook

This document is the detailed operating contract for using Codex on SASD Editor Toolkit. The root `AGENTS.md` stays intentionally compact; this file contains the longer rationale and execution rules.

## 1. Goal

Codex should be able to receive a short instruction such as:

> Continue with the next SASD Editor Toolkit milestone. Work autonomously until the next coherent green checkpoint.

and then discover the repository state, choose the next approved work item, implement it, test it, update documentation, commit it and verify CI without repeatedly asking for routine decisions.

Autonomy does **not** mean unlimited authority. It means high independence inside a clear repository, security and product boundary.

## 2. Repository sources of truth

Use the following hierarchy.

### Product and architecture

- `ROADMAP.md` defines milestone order and scope.
- `docs/adr/` records accepted architecture decisions.
- `docs/spec/` records language-neutral observable behavior.
- `docs/de/Pflichtenheft.md` and `docs/de/Lastenheft.md` are broad source requirements, not a request to implement everything at once.

### Agent execution

- `AGENTS.md` contains the short mandatory rules.
- subtree `AGENTS.md` files contain component-specific rules.
- `docs/ai/CURRENT-STATE.md` is a dated snapshot, not a substitute for inspecting Git.
- `docs/ai/NEXT-WORK.md` is the default queue when the user says only “continue”.
- `PLANS.md` defines when and how to keep an execution plan.

### Historical notes

`docs/notes/` contains work logs and branch-scoped notes. They are useful evidence, but they are lower authority than current repository-wide instructions. In particular, an old note that says “stop this branch here” must not be interpreted as a permanent project freeze.

## 3. Autonomy granted inside this repository

For a well-defined queued task, Codex may without asking:

- inspect any repository file and Git history;
- edit Core, adapters, samples, tests, docs, specs and CI configuration;
- add or remove internal implementation files when the architecture remains consistent;
- refactor code needed to complete the task;
- add deterministic tests and fixtures;
- update XML documentation, English/German docs and changelog entries;
- run restore/build/test commands;
- fix warnings or test failures introduced by the current work;
- make small adjacent correctness fixes discovered while implementing the task, when covered by tests and documented in the final summary;
- commit coherent green checkpoints directly to `main`;
- push ordinary fast-forward commits;
- inspect GitHub Actions and fix failures attributable to the commit;
- continue to the next sub-step of the same milestone when acceptance criteria make the next action unambiguous.

The project currently assumes a single developer and direct-`main` development. This can be changed by a future user instruction without rewriting the architecture.

## 4. Actions that require a stop or explicit approval

Stop and ask when one of these conditions applies:

1. **Secrets or credentials**: a task requires a new secret, token, signing key, private feed credential or user account action.
2. **Irreversible external effect**: publishing packages, creating a public release/tag intended as a release, deleting external data, changing billing, transferring ownership or modifying unrelated repositories.
3. **History destruction**: force-push, deleting remote branches with unmerged work, rebasing published shared history, or destructive reset of user work.
4. **Security boundary change**: adding arbitrary script/macro execution, loading untrusted plugins in-process, weakening path validation, or opening a network capability not already approved.
5. **Legal uncertainty**: a proposed implementation would require copying historical Borland source/manual material rather than independently implementing behavior.
6. **Product ambiguity with incompatible public outcomes**: two or more choices would create meaningfully different public APIs/file formats and neither specs nor ADRs choose one.
7. **Unrecoverable validation gap**: after reasonable diagnosis, the agent cannot validate a risky change and no CI path can cover it.
8. **Unexpected unrelated local changes**: the worktree contains user changes that would be overwritten or mixed into the task.

Do **not** stop for naming choices, local private helper design, test organization, comments, routine refactors or other decisions that existing patterns can answer.

## 5. Standard autonomous loop

### Step A - establish a safe baseline

Run:

```bash
git status --short --branch
git pull --ff-only
git log -5 --oneline
```

If the worktree is dirty, identify whether the changes belong to the current Codex session. Never discard unknown user work.

Read root instructions, current state, next work, roadmap and the relevant specs/ADRs. Before editing a subtree, read its local `AGENTS.md`.

### Step B - understand before editing

Inspect the existing implementation and tests that are closest to the requested behavior. Search for command IDs, interfaces and patterns rather than inventing parallel abstractions.

Prefer extending an existing seam over adding another near-duplicate service.

For behavioral changes, identify:

- current observable behavior;
- required observable behavior;
- affected invariants;
- tests that should fail before the fix or feature;
- documentation/spec changes.

### Step C - decide whether an ExecPlan is needed

Use `PLANS.md`. If required, create the plan and keep working. The plan is a living engineering artifact, not a pause point.

### Step D - implement in small coherent increments

Favor a correctness-first implementation that is easy to review.

For Core behavior:

- add/adjust tests with the behavior;
- keep Core independent of UI frameworks;
- preserve line-ending and Unicode rules;
- avoid hidden mutable global state;
- keep command/string contracts stable or update the language-neutral spec deliberately.

For adapter behavior:

- translate host input/rendering into Core concepts;
- do not duplicate editing state machines in WinForms;
- host dialogs and lifecycle decisions belong in the sample/host when they are not reusable adapter behavior.

### Step E - validate locally

On Linux/macOS:

```bash
./scripts/verify-core.sh
```

On Windows:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\verify-all.ps1
```

For targeted iteration, run the narrowest useful test first, but finish with the strongest platform-appropriate verification.

Also run:

```bash
git diff --check
git status --short
```

### Step F - update documentation as part of the feature

When observable behavior changes, update the relevant `docs/spec/` file. When architecture changes, add or update an ADR. Update English and German narrative docs in parallel when practical. Add a concise `CHANGELOG.md` bullet for meaningful changes.

Comments in code should capture invariants and “why”. They are not a substitute for specs or tests.

### Step G - commit and verify

Use a focused conventional-style commit message. Do not amend an existing published commit.

After committing:

```bash
git status --short --branch
git log -1 --oneline
```

Push when network access and credentials are available. Check the corresponding GitHub Actions run. If CI fails because of the commit, diagnose, fix, revalidate, commit the fix and check again.

Do not declare a code task done while its current CI is red.

## 6. Platform-aware validation

The full solution includes WinForms projects targeting `net10.0-windows`.

### Linux/macOS agent

The reliable local contract is Core + Core tests. Use `scripts/verify-core.sh`. An agent may still edit WinForms code after careful inspection, but it must say that full WinForms compilation was verified by Windows CI rather than pretending a Linux Core build covered it.

### Windows agent

Use `scripts/verify-all.ps1`, which restores, builds and tests the complete solution.

### GitHub Actions

Windows GitHub Actions is the canonical full-solution validation when local work happens on Linux. A green Core-only local run plus a red Windows CI run is **not** complete.

## 7. Failure handling

Treat failures as information, not as an automatic handoff to the user.

Classify a failure:

- **product regression** - fix code/test/docs;
- **test expectation stale** - update only after confirming the intended behavior from specs;
- **platform/toolchain issue** - record the exact limitation and use another supported validation path;
- **network/transient restore failure** - retry a small bounded number of times;
- **pre-existing failure** - prove it is pre-existing before proceeding, and avoid masking it;
- **new architectural conflict** - consult ADR/spec hierarchy; create an ADR if the decision is substantial.

Never solve a failing test by weakening the assertion without proving that the requirement changed.

## 8. Dependency policy

Prefer the BCL and existing packages. Adding a new NuGet dependency should be justified by clear value and should not duplicate trivial functionality.

Before adding a dependency, check maintenance/activity, license compatibility, package size/transitive dependencies, architectural placement and the cost imposed on later non-.NET ports.

A small implementation is often preferable to a large framework dependency in this toolkit.

## 9. Security and file-safety policy

Editor code touches user files, so data-loss risks are high priority.

- Preserve encoding/BOM/line-ending metadata according to current storage contracts.
- Do not overwrite a file merely to “test” save behavior; use temporary directories in tests.
- Keep dirty-state guards in host workflows.
- Treat binary-looking content and malformed settings conservatively.
- Never execute document contents.
- Do not add arbitrary macro/script/plugin execution without a security ADR and explicit task.

## 10. Historical compatibility / clean-room policy

The Turbo Editor Toolbox can inform behavior and terminology, but implementation must be independently written.

Allowed: document behavior in your own words, implement observed concepts with modern C# architecture, and use short historical names in documentation for traceability.

Not allowed: copying historical Pascal source, transcribing long manual passages into code comments/docs, adding scanned manuals/covers/logos, or claiming official affiliation.

## 11. Documentation and comment standard

Good comments explain an invariant that is easy to violate, why an unusual branch exists, a compatibility decision, a Unicode/line-ending boundary, or why a simple implementation is intentionally chosen before optimization.

Avoid comments that merely narrate syntax.

Public APIs should have useful XML documentation describing behavior, parameters, non-obvious exceptions and mutability/ownership semantics where relevant.

## 12. Definition of done

A work item is complete only when all applicable points are true:

- behavior meets the task/queue acceptance criteria;
- tests cover success plus important edge cases;
- Core architecture boundaries remain intact;
- public behavior is reflected in specs;
- English/German docs are updated where needed;
- changelog is updated for meaningful behavior;
- local validation passes at the strongest available level;
- Windows CI passes for changes that touch buildable product code;
- no generated binaries or temporary files are committed;
- commit is focused and worktree is clean;
- `docs/ai/NEXT-WORK.md` is updated if the completed item was taken from that queue.

## 13. Useful prompt style

A short continuation prompt is enough once this harness is present:

> Continue autonomously with the first unfinished item in docs/ai/NEXT-WORK.md. Follow AGENTS.md, keep Core UI-independent, add tests and English/German documentation, commit green checkpoints directly to main, and stop only for a documented stop condition.

For a specific feature, describe it like a GitHub issue: desired behavior, affected component and acceptance criteria. Codex should still inspect the repository before choosing implementation details.

## 14. Codex local configuration

Repository instructions do not require a dangerous global Codex configuration. A balanced setup is a writable repository sandbox with approvals only when Codex needs to cross that boundary.

Current OpenAI Codex versions no longer support `approval_policy = "untrusted"`. Keep machine-specific configuration in the user's `~/.codex/config.toml`, not in this repository.

Where available, automatic review/approval of low-risk sandbox requests can reduce interruptions while preserving a boundary around destructive or external actions.
