namespace Sasd.EditorToolkit.Search;

/// <summary>
/// Options controlling one text search operation.
/// </summary>
/// <param name="Comparison">String comparison used for matching.</param>
/// <param name="Direction">Search direction relative to the start position.</param>
/// <param name="Wrap">Whether the search may wrap around the document boundary.</param>
public sealed record TextSearchOptions(
    StringComparison Comparison = StringComparison.OrdinalIgnoreCase,
    SearchDirection Direction = SearchDirection.Forward,
    bool Wrap = true);
