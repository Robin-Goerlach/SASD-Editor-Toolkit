using System.Text;

namespace Sasd.EditorToolkit.Text;

/// <summary>
/// Simple line-oriented M1 buffer implementation.
/// </summary>
public sealed class LineTextBuffer : ITextBuffer
{
    private readonly List<LineEntry> _lines = new();

    /// <summary>Creates an empty buffer.</summary>
    public LineTextBuffer() : this(string.Empty)
    {
    }

    /// <summary>Creates a buffer from text.</summary>
    public LineTextBuffer(string text)
    {
        ResetFromText(text ?? string.Empty);
    }

    /// <inheritdoc />
    public int LineCount => _lines.Count;

    /// <inheritdoc />
    public long Version { get; private set; }

    /// <inheritdoc />
    public int Length => GetText().Length;

    /// <inheritdoc />
    public event EventHandler<TextBufferChangedEventArgs>? Changed;

    /// <inheritdoc />
    public ReadOnlyMemory<char> GetLineText(int lineIndex)
    {
        ValidateLineIndex(lineIndex);
        return _lines[lineIndex].Text.AsMemory();
    }

    /// <inheritdoc />
    public LineEndingKind GetLineEnding(int lineIndex)
    {
        ValidateLineIndex(lineIndex);
        return _lines[lineIndex].LineEnding;
    }

    /// <inheritdoc />
    public string GetText(TextRange range)
    {
        var start = GetOffset(Normalize(range.Start));
        var end = GetOffset(Normalize(range.End));
        return GetText().Substring(start, Math.Max(0, end - start));
    }

    /// <inheritdoc />
    public string GetText()
    {
        var builder = new StringBuilder();
        foreach (var line in _lines)
        {
            builder.Append(line.Text);
            builder.Append(ToText(line.LineEnding));
        }

        return builder.ToString();
    }

    /// <inheritdoc />
    public TextPosition Normalize(TextPosition position)
    {
        if (_lines.Count == 0)
        {
            _lines.Add(new LineEntry(string.Empty, LineEndingKind.None));
        }

        var line = Math.Clamp(position.Line, 0, _lines.Count - 1);
        var column = Math.Clamp(position.Column, 0, _lines[line].Text.Length);
        return new TextPosition(line, column);
    }

    /// <inheritdoc />
    public int GetOffset(TextPosition position)
    {
        position = Normalize(position);
        var offset = 0;
        for (var i = 0; i < position.Line; i++)
        {
            offset += _lines[i].Text.Length + ToText(_lines[i].LineEnding).Length;
        }

        return offset + position.Column;
    }

    /// <inheritdoc />
    public TextPosition GetPosition(int offset)
    {
        offset = Math.Clamp(offset, 0, Length);
        var remaining = offset;

        for (var lineIndex = 0; lineIndex < _lines.Count; lineIndex++)
        {
            var line = _lines[lineIndex];
            if (remaining <= line.Text.Length)
            {
                return new TextPosition(lineIndex, remaining);
            }

            remaining -= line.Text.Length;
            var endingLength = ToText(line.LineEnding).Length;
            if (remaining <= endingLength)
            {
                return new TextPosition(Math.Min(lineIndex + 1, _lines.Count - 1), 0);
            }

            remaining -= endingLength;
        }

        var lastLine = _lines.Count - 1;
        return new TextPosition(lastLine, _lines[lastLine].Text.Length);
    }

    /// <inheritdoc />
    public TextChangeSet Insert(TextPosition position, string text) => Replace(TextRange.Empty(position), text);

    /// <inheritdoc />
    public TextChangeSet Delete(TextRange range) => Replace(range, string.Empty);

    /// <inheritdoc />
    public TextChangeSet Replace(TextRange range, string text)
    {
        text ??= string.Empty;
        var normalizedRange = new TextRange(Normalize(range.Start), Normalize(range.End));
        var before = GetText();
        var startOffset = GetOffset(normalizedRange.Start);
        var endOffset = GetOffset(normalizedRange.End);
        var removed = before.Substring(startOffset, endOffset - startOffset);
        var after = before.Remove(startOffset, endOffset - startOffset).Insert(startOffset, text);

        var oldVersion = Version;
        ResetFromText(after);
        Version = oldVersion + 1;

        var change = new TextChangeSet(normalizedRange.Start, removed, text, oldVersion, Version);
        Changed?.Invoke(this, new TextBufferChangedEventArgs(change));
        return change;
    }

    private void ResetFromText(string text)
    {
        _lines.Clear();

        var current = new StringBuilder();
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (c == '\r')
            {
                if (i + 1 < text.Length && text[i + 1] == '\n')
                {
                    _lines.Add(new LineEntry(current.ToString(), LineEndingKind.CrLf));
                    current.Clear();
                    i++;
                }
                else
                {
                    _lines.Add(new LineEntry(current.ToString(), LineEndingKind.Cr));
                    current.Clear();
                }
            }
            else if (c == '\n')
            {
                _lines.Add(new LineEntry(current.ToString(), LineEndingKind.Lf));
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        _lines.Add(new LineEntry(current.ToString(), LineEndingKind.None));
    }

    private void ValidateLineIndex(int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= _lines.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(lineIndex));
        }
    }

    private static string ToText(LineEndingKind lineEnding) => lineEnding switch
    {
        LineEndingKind.CrLf => "\r\n",
        LineEndingKind.Lf => "\n",
        LineEndingKind.Cr => "\r",
        _ => string.Empty
    };

    private sealed record LineEntry(string Text, LineEndingKind LineEnding);
}
