# AGENTS.md - Sasd.EditorToolkit.WinForms

- This project adapts Core state/commands to WinForms; it must not become a second editor engine.
- Do not duplicate text mutation, search, undo, selection or document state machines from Core.
- Route keyboard/menu actions through stable Core commands where they exist.
- Rendering, hit testing, font metrics, colors and WinForms event translation belong here.
- File dialogs, dirty-close policy and application lifecycle normally belong in the sample/host.
- Keep public adapter APIs small and document them.
- Any behavior visible outside WinForms should be defined/tested in Core first.
- Validate changes through the full Windows solution or GitHub Actions.
