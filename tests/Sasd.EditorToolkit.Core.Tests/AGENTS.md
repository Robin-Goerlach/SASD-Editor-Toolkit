# AGENTS.md - Core tests

- Tests must be deterministic, headless and independent of network access.
- Prefer one observable behavior per test with descriptive names.
- Reproduce bugs with a regression test before or together with the fix.
- Use temporary directories/files for storage tests and clean them up.
- Do not weaken assertions merely to make a failing build green.
- Cover empty/boundary text, multi-line edits, Unicode, line endings, dirty state and undo/redo where relevant.
- Fix product seams rather than adding test-only production behavior.
