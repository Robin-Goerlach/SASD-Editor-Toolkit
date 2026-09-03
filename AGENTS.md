# AGENTS.md - SASD Editor Toolkit

These instructions apply to human contributors and AI coding agents working in this repository.

## Mission

Build a practical, understandable, reusable editor toolkit. The first implementation is C#/.NET. The design must keep later Java, C++, Web and limited PHP integration possible without turning M1 into a multi-language research project.

## Scope discipline

The Pflichtenheft is a product backlog. It is not the M1 task list.

M1 means:

- Core text model;
- line-oriented buffer;
- positions and ranges;
- basic editing;
- dirty state;
- minimal undo/redo;
- file load/save;
- command dispatcher seed;
- small WinForms demo;
- tests.

Do not add syntax highlighting, plugin frameworks, WPF, Web, Java, C++, PHP, Rope, Piece Table, macro engines or a full MicroStar clone unless the current task explicitly moves the roadmap forward.

## Architecture rules

- `Sasd.EditorToolkit.Core` must not reference WinForms, WPF, ASP.NET, browser APIs or UI-specific types.
- UI adapters depend on Core; Core never depends on UI adapters.
- Samples demonstrate integration; they must not contain product logic that belongs in Core or adapter packages.
- Command IDs are stable language-neutral strings.
- Settings and keyboard profiles must be serializable.
- Prefer small, explicit interfaces over global mutable state.
- No hidden singleton editor state.
- No direct copies of historical Borland source code or manual text beyond short cited references in documentation.

## Language strategy

C# is the reference implementation, but C# details are not automatically cross-language law.

Keep cross-language knowledge in `docs/spec/`:

- editor model;
- buffer behavior;
- command IDs;
- settings format;
- keyboard profile format;
- conformance test vectors.

API shape should be familiar across languages, but idiomatic. Reuse concepts and names where reasonable; do not force C# patterns into C++ or Java when they damage the target-language user experience.

## Unicode and text indexing

- C# may expose UTF-16 code-unit positions because that matches .NET strings.
- The language-neutral spec must describe observable behavior, line/column concepts, grapheme-safe caret movement and test vectors.
- A future C++ implementation may use UTF-8 internally if that better fits Linux/native expectations.
- Never split surrogate pairs or combined user-visible characters intentionally in cursor movement.

## Coding style

- Public code and package names: English.
- Documentation: German and English when practical.
- C#: nullable enabled, XML documentation on public APIs.
- Keep methods short and name them after business/editor concepts.
- Prefer deterministic tests over clever abstractions.
- Add comments where they explain intent, invariants or trade-offs.

## Commit discipline

Suggested commit style:

```text
feat(core): add line text buffer
fix(storage): preserve line endings on save
docs(roadmap): clarify M1 scope
adr: record C# reference implementation decision
```

## Safety and legal boundaries

- Do not commit historic PDFs, scanned manual pages, Borland logos, covers or source code.
- Do not claim official Borland/Embarcadero affiliation.
- Historical names are for documentation and traceability only.
- Treat file paths and document contents as untrusted input.
- Do not add macro or script execution without a separate security ADR.

## Build checks

Before a pull request:

```bash
dotnet restore SASD-Editor-Toolkit.sln
dotnet build SASD-Editor-Toolkit.sln -c Debug
dotnet test SASD-Editor-Toolkit.sln -c Debug
```

If working on Linux, at least build/test the Core project and clearly note that WinForms requires Windows.
