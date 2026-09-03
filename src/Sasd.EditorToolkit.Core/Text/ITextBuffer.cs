namespace Sasd.EditorToolkit.Text;

/// <summary>
/// UI-independent text buffer contract.
/// </summary>
public interface ITextBuffer
{
    /// <summary>Gets the number of logical lines.</summary>
    int LineCount { get; }

    /// <summary>Gets the monotonically increasing buffer version.</summary>
    long Version { get; }

    /// <summary>Gets the total text length including line endings.</summary>
    int Length { get; }

    /// <summary>Gets one line without its terminator.</summary>
    ReadOnlyMemory<char> GetLineText(int lineIndex);

    /// <summary>Gets the line ending stored after the specified line.</summary>
    LineEndingKind GetLineEnding(int lineIndex);

    /// <summary>Gets text in the supplied range.</summary>
    string GetText(TextRange range);

    /// <summary>Gets the complete text.</summary>
    string GetText();

    /// <summary>Normalizes a position into a valid document position.</summary>
    TextPosition Normalize(TextPosition position);

    /// <summary>Converts a position to a zero-based text offset.</summary>
    int GetOffset(TextPosition position);

    /// <summary>Converts a zero-based text offset to a text position.</summary>
    TextPosition GetPosition(int offset);

    /// <summary>Inserts text at the supplied position.</summary>
    TextChangeSet Insert(TextPosition position, string text);

    /// <summary>Deletes the supplied range.</summary>
    TextChangeSet Delete(TextRange range);

    /// <summary>Replaces the supplied range with text.</summary>
    TextChangeSet Replace(TextRange range, string text);

    /// <summary>Raised after a text change was applied.</summary>
    event EventHandler<TextBufferChangedEventArgs>? Changed;
}
