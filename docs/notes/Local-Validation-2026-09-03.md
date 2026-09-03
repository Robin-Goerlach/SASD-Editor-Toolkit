# Local Validation Checklist - 2026-09-03

Use this checklist after pulling `feat/m1a-core-hardening`.

```powershell
git switch main
git pull --ff-only origin main
git fetch origin
git switch --track origin/feat/m1a-core-hardening

dotnet restore .\SASD-Editor-Toolkit.sln
dotnet build .\SASD-Editor-Toolkit.sln -c Debug
dotnet test .\SASD-Editor-Toolkit.sln -c Debug
```

Expected direction:

- no build errors;
- no XML documentation warnings from test or sample projects;
- existing 7 tests plus the new command/storage/buffer tests pass;
- the WinForms sample starts and still allows basic typing, New, Open, Save, Save As, Undo and Redo.

Manual sample checks:

1. Start `Sasd.EditorToolkit.Sample.FirstEd.WinForms`.
2. Type a few characters and confirm the status bar dirty marker appears.
3. Move the caret and confirm line/column changes.
4. Use New or Exit with unsaved changes and confirm the save/discard/cancel prompt appears.
5. Open a UTF-8 file with BOM and save it again.
