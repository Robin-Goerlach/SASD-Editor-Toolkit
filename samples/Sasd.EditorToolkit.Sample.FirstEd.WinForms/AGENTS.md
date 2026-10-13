# AGENTS.md - FIRST-ED WinForms sample

- This is a host/demo application, not a product-logic layer.
- It may own dialogs, menus, status bars, window title, close confirmation and demo data.
- Reusable editor behavior belongs in Core; reusable WinForms integration belongs in the adapter.
- Keep the sample simple enough to teach integration.
- Add reusable behavior tests before relying on manual UI testing.
- Update README screenshots only from an actually built/running UI when practical.
- Validate on Windows or rely on Windows GitHub Actions.
