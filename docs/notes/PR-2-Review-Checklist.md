# PR 2 Review Checklist

- [ ] Build succeeds on Windows with .NET 10.
- [ ] Tests pass.
- [ ] No XML documentation warnings from sample/test projects.
- [ ] `Edit.DeleteRight` is registered by the M1 default registry.
- [ ] UTF-8 BOM content loads without leading `\uFEFF`.
- [ ] UTF-32 BOM is not misdetected as UTF-16.
- [ ] Save with BOM preserves the preamble when configured.
- [ ] Save without BOM stays BOM-free when configured.
- [ ] Modern FIRST-ED prompts before losing dirty documents.
- [ ] Status bar follows caret movements in the sample.
