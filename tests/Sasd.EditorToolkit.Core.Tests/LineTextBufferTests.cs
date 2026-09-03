using Sasd.EditorToolkit.Text;
using Xunit;

namespace Sasd.EditorToolkit.Core.Tests;

public sealed class LineTextBufferTests
{
    [Fact]
    public void Empty_buffer_has_one_line()
    {
        var buffer = new LineTextBuffer();
        Assert.Equal(1, buffer.LineCount);
        Assert.Equal(string.Empty, buffer.GetLineText(0).ToString());
    }

    [Fact]
    public void Insert_text_updates_content()
    {
        var buffer = new LineTextBuffer("Hello");
        buffer.Insert(new TextPosition(0, 5), " World");
        Assert.Equal("Hello World", buffer.GetText());
    }

    [Fact]
    public void Delete_range_across_lines_updates_content()
    {
        var buffer = new LineTextBuffer("Hello\nWorld");
        buffer.Delete(new TextRange(new TextPosition(0, 5), new TextPosition(1, 0)));
        Assert.Equal("HelloWorld", buffer.GetText());
    }

    [Fact]
    public void Constructor_preserves_mixed_line_endings()
    {
        var buffer = new LineTextBuffer("A\r\nB\nC\rD");

        Assert.Equal(4, buffer.LineCount);
        Assert.Equal(LineEndingKind.CrLf, buffer.GetLineEnding(0));
        Assert.Equal(LineEndingKind.Lf, buffer.GetLineEnding(1));
        Assert.Equal(LineEndingKind.Cr, buffer.GetLineEnding(2));
        Assert.Equal(LineEndingKind.None, buffer.GetLineEnding(3));
        Assert.Equal("A\r\nB\nC\rD", buffer.GetText());
    }

    [Fact]
    public void Replace_replaces_normalized_reversed_range()
    {
        var buffer = new LineTextBuffer("Hello");

        buffer.Replace(new TextRange(new TextPosition(0, 4), new TextPosition(0, 1)), "i");

        Assert.Equal("Hio", buffer.GetText());
    }
}
