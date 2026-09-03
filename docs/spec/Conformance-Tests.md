# Conformance Tests

Conformance tests describe observable editor behavior independent of implementation language.

## Planned vector shape

```json
{
  "name": "insert_text_simple",
  "initialText": "Hello\nWorld",
  "initialCaret": { "line": 1, "column": 5 },
  "commands": [
    { "id": "Edit.InsertText", "text": "!" }
  ],
  "expectedText": "Hello\nWorld!",
  "expectedCaret": { "line": 1, "column": 6 }
}
```

## M1 vector groups

- empty document;
- insert single-line text;
- insert multi-line text;
- delete within a line;
- delete across lines;
- undo/redo simple insert;
- preserve CRLF/LF basics;
- Unicode text roundtrip.

The first version of the C# tests may be ordinary xUnit tests. JSON vectors can be added once the behavior is stable enough to share with later implementations.
