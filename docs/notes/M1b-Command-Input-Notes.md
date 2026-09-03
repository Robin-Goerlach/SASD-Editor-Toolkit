# M1b Command Input Notes

Date: 2026-09-03

## Purpose

The WinForms adapter should prove the command-first architecture without growing into a full IDE. Keyboard input is therefore routed through the same command IDs that menus, toolbars, tests and future adapters can reuse.

## Implemented in this branch

- Printable characters call `Edit.InsertText`.
- Enter calls `Edit.NewLine`.
- Backspace calls `Edit.DeleteLeft`.
- Delete calls `Edit.DeleteRight`.
- Arrow keys call `Navigate.Left`, `Navigate.Right`, `Navigate.Up` and `Navigate.Down`.
- Home and End call `Navigate.LineStart` and `Navigate.LineEnd`.
- Ctrl+Z and Ctrl+Y call `Edit.Undo` and `Edit.Redo`.

## Architectural rule

The WinForms surface may translate platform input into command IDs, but it should not duplicate editor semantics that belong in Core commands.

## Not included

- Selection expansion with Shift+Arrow.
- Ctrl+Left/Ctrl+Right word navigation.
- PageUp/PageDown.
- Clipboard commands.
- Keyboard profile loading from JSON.

These belong to later M1b/M2 steps after the basic command seam is validated locally.
