# Settings Format

Settings must be serializable and portable where practical.

## Minimal JSON shape

```json
{
  "version": 1,
  "editing": {
    "tabSize": 4,
    "insertMode": true,
    "autoIndent": false
  },
  "view": {
    "showLineNumbers": true,
    "wordWrap": false,
    "showWhitespace": false
  },
  "input": {
    "keyboardProfile": "Modern"
  }
}
```

## Rules

- Unknown fields should be ignored by default.
- Invalid known values should be reported and replaced with safe defaults.
- Language implementations may add extension sections.
- Settings must not contain secrets.
