# Editor Model Specification

This is a practical, language-neutral model. It should be small enough for implementers to actually use.

## Core concepts

| Concept | Meaning |
|---|---|
| Document | Owns text content, metadata, dirty state and undo history. |
| Text Buffer | Stores and changes text behind an implementation boundary. |
| Position | Identifies a location in a document. |
| Range | Identifies a half-open text interval. |
| View | Holds caret, selection, scrolling, wrapping and display state for one document. |
| Workspace | Owns documents and views in a host/session. |
| Command | Stable user/API action identified by a language-neutral command ID. |
| Adapter | Platform-specific rendering, input, clipboard, dialogs and accessibility. |

## Design rule

A document may have multiple views. A view has exactly one document. Editing through one view changes the document and must be observable through all other views bound to that document.

## M1 minimum behavior

- Create an empty document.
- Insert text.
- Delete a range.
- Replace a range.
- Preserve basic line structure.
- Track dirty state.
- Save and load text.
- Undo and redo simple edits.
