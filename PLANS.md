# PLANS.md - Execution plans for substantial work

This file defines how Codex should plan changes that are too large for a single obvious edit.

## When an execution plan is required

Create a living plan under `docs/notes/execplans/YYYY-MM-DD-<slug>.md` when at least one of these is true:

- the task changes more than one architectural subsystem;
- a public Core API or language-neutral contract will change;
- a migration or compatibility transition is required;
- the work is expected to require several implementation/test iterations;
- there are multiple plausible designs whose trade-offs must be recorded;
- the work spans Core plus an adapter plus a sample;
- a failure could corrupt user files or break persistence compatibility.

Do not create an execution plan for a small local bug fix, a straightforward test addition or a documentation-only correction.

## Planning is not a permission request

Codex should create or update the plan and continue working. Do not stop merely because a plan exists. Ask the user only if a documented stop condition from `docs/ai/CODEX-AUTONOMY.md` is reached.

## Required plan sections

Every plan should contain:

- **Goal** - observable outcome.
- **Non-goals** - scope intentionally excluded.
- **Relevant sources** - specs, ADRs, existing code and tests.
- **Baseline** - current behavior and validation state.
- **Design/invariants** - decisions that must remain true.
- **Implementation steps** - ordered, independently verifiable changes.
- **Validation** - exact commands/tests and expected evidence.
- **Progress** - checkbox list updated while working.
- **Decision log** - important choices and why.
- **Risks / rollback** - what could go wrong and how to recover.
- **Completion summary** - final commit(s), tests and remaining follow-up.

## Plan quality rules

A useful plan names concrete files or components where known, but it does not prescribe speculative line-by-line edits before the code has been inspected. It must stay current: delete obsolete steps, mark completed work, and record changed decisions.

The plan is repository knowledge, not a transcript. Do not paste chain-of-thought, terminal noise or long tool outputs into it.
