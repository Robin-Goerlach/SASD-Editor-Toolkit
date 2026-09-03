using Sasd.EditorToolkit.Text;

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
}
