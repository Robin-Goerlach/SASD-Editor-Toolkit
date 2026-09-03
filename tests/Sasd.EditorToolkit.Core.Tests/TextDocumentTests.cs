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

    [Fact]
    public void Undo_to_saved_text_clears_dirty_state()
    {
        var document = new TextDocument(new LineTextBuffer("Hello"));
        document.MarkSaved();

        document.Insert(new TextPosition(0, 5), "!");
        Assert.True(document.IsDirty);

        Assert.True(document.Undo());
        Assert.False(document.IsDirty);
    }

    [Fact]
    public void Redo_after_undo_to_saved_text_sets_dirty_state_again()
    {
        var document = new TextDocument(new LineTextBuffer("Hello"));
        document.MarkSaved();
        document.Insert(new TextPosition(0, 5), "!");
        document.Undo();

        Assert.True(document.Redo());
        Assert.True(document.IsDirty);
    }
}
