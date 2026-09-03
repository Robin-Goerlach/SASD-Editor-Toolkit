# M1a Hardening Notes

This note records the practical scope of the `feat/m1a-core-hardening` branch.

## Intent

Keep the project moving toward a reliable Modern FIRST-ED 0.1 without expanding into M2/M3 topics.

## Covered

- Test and sample projects do not generate XML documentation warnings. Product assemblies still generate XML documentation.
- Storage now detects BOM/preamble information explicitly and strips it from loaded text.
- Saving writes the encoding preamble only when the selected encoding provides one and the save option allows it.
- UTF-32 BOM detection is checked before UTF-16 because UTF-32 little endian starts with the UTF-16 little endian prefix.
- File saving through the atomic path writes to a temporary file and moves it over the target path without manually deleting the existing target first.
- The M1 command set now includes `Edit.DeleteRight`.

## Not covered

- No WPF, Web, Java, C++, PHP or plugin work.
- No syntax highlighting.
- No Piece Table or Rope buffer.
- No full MicroStar demo.
- No macro language.

## Local validation requested

```powershell
dotnet restore .\SASD-Editor-Toolkit.sln
dotnet build .\SASD-Editor-Toolkit.sln -c Debug
dotnet test .\SASD-Editor-Toolkit.sln -c Debug
```
