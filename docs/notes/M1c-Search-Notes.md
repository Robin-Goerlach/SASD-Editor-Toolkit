# M1c Search Notes

Date: 2026-09-03

## Goal

Add a first small, UI-independent search capability without turning the project into a full editor clone yet.

## Implemented

- `Sasd.EditorToolkit.Search.TextSearchService`
- `TextSearchOptions`
- `TextSearchMatch`
- `SearchRequest`
- `SearchDirection`
- `Search.Find` command integration
- simple Find / Find Next UI in the WinForms sample
- tests for forward search, backward search, wrapping, no-wrap behavior, case-sensitive comparison and matches spanning line endings

## Important design choice

The M1c search service searches a complete text snapshot produced by `ITextBuffer.GetText()`.

This is intentional for M1:

- behavior is simple to reason about;
- tests are deterministic;
- future adapters can use the same command contract;
- large-file performance can be improved later behind the same observable behavior.

## Not implemented in this step

- replace command;
- search history;
- regex;
- whole-word search;
- incremental search panel;
- visual scroll-to-selection behavior;
- multi-selection;
- syntax-aware search;
- large-file indexed search.

## Next likely steps

1. Add `Search.Replace` as a Core command and sample menu entry.
2. Add selection-aware edit commands: delete selection, replace selection, select all.
3. Improve the WinForms surface so selected text is painted with foreground/background contrast.
4. Add conformance vectors for search and replace once behavior is stable enough.
