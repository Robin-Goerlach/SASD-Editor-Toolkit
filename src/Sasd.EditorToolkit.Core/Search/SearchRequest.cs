namespace Sasd.EditorToolkit.Search;

/// <summary>
/// Command parameter used for a search command invocation.
/// </summary>
/// <param name="SearchText">Text to find.</param>
/// <param name="Options">Search options. When omitted, the default forward wrapping search is used.</param>
public sealed record SearchRequest(string SearchText, TextSearchOptions? Options = null);
