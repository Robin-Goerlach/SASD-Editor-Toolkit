# Keyboard Profile Format

Keyboard profiles map one or more key gestures to command IDs.

## Minimal JSON shape

```json
{
  "name": "Modern",
  "version": 1,
  "bindings": [
    { "keys": ["Ctrl+S"], "command": "File.Save" },
    { "keys": ["Ctrl+Z"], "command": "Edit.Undo" },
    { "keys": ["Ctrl+K", "Ctrl+B"], "command": "Block.Begin" }
  ]
}
```

## Rules

- Key syntax is stored as text to remain language-independent.
- A binding may contain a sequence for prefix/chord behavior.
- Unknown commands must not execute uncontrolled behavior.
- Hosts may define additional commands if their IDs do not collide with reserved IDs.
