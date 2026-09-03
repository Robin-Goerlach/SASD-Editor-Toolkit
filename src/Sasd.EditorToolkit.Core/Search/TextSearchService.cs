using Sasd.EditorToolkit.Text;

namespace Sasd.EditorToolkit.Search;

/// <summary>
/// Provides UI-independent text search over an <see cref="ITextBuffer"/>.
/// </summary>
/// <remarks>
/// M1 deliberately uses the complete text snapshot for predictable behavior and simple tests.
/// A future large-file buffer can replace this with an incremental implementation behind the
/// same observable contract.
/// </remarks>
public sealed class TextSearchService
{
    /// <summary>
    /// Finds the next occurrence of <paramref name="searchText"/> relative to <paramref name="startPosition"/>.
    /// </summary>
    /// <param name="buffer">The buffer to search.</param>
    /// <param name="startPosition">The logical position where the search starts.</param>
    /// <param name="searchText">The text to find.</param>
    /// <param name="options">Optional search behavior; defaults to forward, case-insensitive, wrapping search.</param>
    /// <returns>The match when found; otherwise <see langword="null"/>.</returns>
    public TextSearchMatch? FindNext(
        ITextBuffer buffer,
        TextPosition startPosition,
        string searchText,
        TextSearchOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (string.IsNullOrEmpty(searchText))
        {
            return null;
        }

        options ??= new TextSearchOptions();
        var text = buffer.GetText();
        if (text.Length == 0 || searchText.Length > text.Length)
        {
            return null;
        }

        var startOffset = buffer.GetOffset(startPosition);
        var matchOffset = options.Direction == SearchDirection.Backward
            ? FindBackward(text, searchText, startOffset, options)
            : FindForward(text, searchText, startOffset, options);

        return matchOffset < 0
            ? null
            : CreateMatch(buffer, text, searchText.Length, matchOffset);
    }

    private static int FindForward(string text, string searchText, int startOffset, TextSearchOptions options)
    {
        var safeStart = Math.Clamp(startOffset, 0, text.Length);
        var direct = text.IndexOf(searchText, safeStart, options.Comparison);
        if (direct >= 0 || !options.Wrap || safeStart == 0)
        {
            return direct;
        }

        var wrapped = text.IndexOf(searchText, 0, options.Comparison);
        return wrapped >= 0 && wrapped < safeStart ? wrapped : -1;
    }

    private static int FindBackward(string text, string searchText, int startOffset, TextSearchOptions options)
    {
        var safeStart = Math.Clamp(startOffset - 1, 0, text.Length - 1);
        var direct = text.LastIndexOf(searchText, safeStart, options.Comparison);
        if (direct >= 0 || !options.Wrap)
        {
            return direct;
        }

        return text.LastIndexOf(searchText, text.Length - 1, options.Comparison);
    }

    private static TextSearchMatch CreateMatch(ITextBuffer buffer, string text, int searchTextLength, int matchOffset)
    {
        var start = buffer.GetPosition(matchOffset);
        var end = buffer.GetPosition(matchOffset + searchTextLength);
        var range = new TextRange(start, end);
        var value = text.Substring(matchOffset, searchTextLength);
        return new TextSearchMatch(range, matchOffset, value);
    }
}
