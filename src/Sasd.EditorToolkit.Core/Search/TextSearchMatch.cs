using Sasd.EditorToolkit.Text;

namespace Sasd.EditorToolkit.Search;

/// <summary>
/// Represents one search hit in a text buffer.
/// </summary>
/// <param name="Range">Half-open range covered by the match.</param>
/// <param name="Offset">Zero-based document offset where the match starts.</param>
/// <param name="Value">Matched text value as found in the document.</param>
public sealed record TextSearchMatch(TextRange Range, int Offset, string Value);
