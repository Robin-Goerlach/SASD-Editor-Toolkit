using Sasd.EditorToolkit.Documents;
using Sasd.EditorToolkit.Text;
using Xunit;

namespace Sasd.EditorToolkit.Core.Tests;

public sealed class TextDocumentTests
{
    [Fact]
    public void Insert_sets_dirty_state()
    {
        var document = new TextDocument(new LineTextBuffer("Hello"));
        document.MarkSaved();

        document.Insert(new TextPosition(0, 5), "!");

        Assert.True(document.IsDirty);
    }

    [Fact]
    public void Undo_and_redo_restore_text()
    {
        var document = new TextDocument(new LineTextBuffer("Hello"));
        document.Insert(new TextPosition(0, 5), "!");

        Assert.True(document.Undo());
        Assert.Equal("Hello", document.Buffer.GetText());

        Assert.True(document.Redo());
        Assert.Equal("Hello!", document.Buffer.GetText());
    }
}
