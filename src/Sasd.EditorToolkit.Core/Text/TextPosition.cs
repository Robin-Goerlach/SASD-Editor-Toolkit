namespace Sasd.EditorToolkit.Text;

/// <summary>
/// Represents a zero-based logical position inside a text document.
/// </summary>
/// <param name="Line">Zero-based line index.</param>
/// <param name="Column">Zero-based column index in the implementation's public indexing model.</param>
public readonly record struct TextPosition(int Line, int Column) : IComparable<TextPosition>
{
    /// <summary>The first position in a document.</summary>
    public static TextPosition Start => new(0, 0);

    /// <inheritdoc />
    public int CompareTo(TextPosition other)
    {
        var line = Line.CompareTo(other.Line);
        return line != 0 ? line : Column.CompareTo(other.Column);
    }

    /// <summary>Returns a copy with negative values clamped to zero.</summary>
    public TextPosition ClampToStart() => new(Math.Max(0, Line), Math.Max(0, Column));
}
