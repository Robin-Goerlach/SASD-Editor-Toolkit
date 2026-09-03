# Merge Hint for PR 2

This branch has many small commits because it was developed through the GitHub connector. Prefer **Squash and merge** when accepting the PR so `main` receives one readable commit.

Suggested squash title:

```text
feat(core): harden M1a storage and command behavior
```

Suggested squash body:

```text
- harden file storage BOM/preamble handling
- add storage and command dispatcher tests
- add Edit.DeleteRight
- improve Modern FIRST-ED dirty guard and status updates
- keep XML docs enabled for product assemblies while suppressing noise in tests/sample
```
