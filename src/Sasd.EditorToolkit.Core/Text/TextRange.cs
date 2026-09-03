namespace Sasd.EditorToolkit.Text;

/// <summary>
/// Represents a half-open text range from <see cref="Start"/> to <see cref="End"/>.
/// </summary>
public readonly record struct TextRange
{
    /// <summary>Creates a normalized range. If start is after end, the values are swapped.</summary>
    public TextRange(TextPosition start, TextPosition end)
    {
        if (start.CompareTo(end) <= 0)
        {
            Start = start;
            End = end;
        }
        else
        {
            Start = end;
            End = start;
        }
    }

    /// <summary>The first position included in the range.</summary>
    public TextPosition Start { get; }

    /// <summary>The first position after the range.</summary>
    public TextPosition End { get; }

    /// <summary>Gets whether the range contains no characters.</summary>
    public bool IsEmpty => Start.Equals(End);

    /// <summary>Creates an empty range at the supplied position.</summary>
    public static TextRange Empty(TextPosition position) => new(position, position);
}
