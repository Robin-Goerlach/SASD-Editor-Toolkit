# Command IDs

Command IDs are stable language-neutral strings. They allow menus, toolbars, key profiles, tests and future language ports to share knowledge.

## M1 command IDs

| ID | Meaning |
|---|---|
| `File.New` | Create new document |
| `File.Open` | Open text file |
| `File.Save` | Save current document |
| `File.SaveAs` | Save current document under new path |
| `File.Close` | Close document/view with dirty-state guard |
| `Edit.InsertText` | Insert supplied text |
| `Edit.NewLine` | Insert a line break |
| `Edit.DeleteLeft` | Delete left of caret |
| `Edit.DeleteRight` | Delete right of caret |
| `Edit.Undo` | Undo last edit |
| `Edit.Redo` | Redo last undone edit |
| `Navigate.Left` | Move caret left |
| `Navigate.Right` | Move caret right |
| `Navigate.Up` | Move caret up |
| `Navigate.Down` | Move caret down |
| `Navigate.LineStart` | Move caret to line start |
| `Navigate.LineEnd` | Move caret to line end |
| `Search.Find` | Find text |
| `Search.Replace` | Replace text |
| `View.Split` | Split current view |

## Rule

Renaming a command ID after 1.0 is a breaking change. Implementations may expose language-specific constants, enums or records, but the string values remain stable.
