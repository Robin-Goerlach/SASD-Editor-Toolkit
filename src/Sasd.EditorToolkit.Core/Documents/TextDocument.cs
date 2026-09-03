using Sasd.EditorToolkit.Text;
using Sasd.EditorToolkit.Undo;

namespace Sasd.EditorToolkit.Documents;

/// <summary>
/// UI-independent document object. It owns text content, metadata, dirty state and undo history.
/// </summary>
public sealed class TextDocument
{
    /// <summary>Creates a new document around the supplied buffer.</summary>
    public TextDocument(ITextBuffer? buffer = null, DocumentMetadata? metadata = null)
    {
        Id = DocumentId.NewId();
        Buffer = buffer ?? new LineTextBuffer();
        Metadata = metadata ?? new DocumentMetadata();
        SavedVersion = Buffer.Version;
    }

    /// <summary>Gets the stable document id.</summary>
    public DocumentId Id { get; }

    /// <summary>Gets the text buffer.</summary>
    public ITextBuffer Buffer { get; }

    /// <summary>Gets document metadata.</summary>
    public DocumentMetadata Metadata { get; }

    /// <summary>Gets the document-level undo manager.</summary>
    public UndoManager UndoManager { get; } = new();

    /// <summary>Gets the buffer version that is considered saved.</summary>
    public long SavedVersion { get; private set; }

    /// <summary>Gets whether current content differs from the last saved version.</summary>
    public bool IsDirty => Buffer.Version != SavedVersion;

    /// <summary>Marks the current buffer version as saved.</summary>
    public void MarkSaved() => SavedVersion = Buffer.Version;

    /// <summary>Inserts text and records undo information.</summary>
    public TextChangeSet Insert(TextPosition position, string text)
    {
        var change = Buffer.Insert(position, text);
        UndoManager.Register(change);
        return change;
    }

    /// <summary>Deletes text and records undo information.</summary>
    public TextChangeSet Delete(TextRange range)
    {
        var change = Buffer.Delete(range);
        UndoManager.Register(change);
        return change;
    }

    /// <summary>Replaces text and records undo information.</summary>
    public TextChangeSet Replace(TextRange range, string text)
    {
        var change = Buffer.Replace(range, text);
        UndoManager.Register(change);
        return change;
    }

    /// <summary>Undoes the last document change.</summary>
    public bool Undo()
    {
        if (!UndoManager.CanUndo)
        {
            return false;
        }

        var change = UndoManager.PopUndo();
        var insertedRange = new TextRange(change.StartPosition, Advance(change.StartPosition, change.InsertedText));
        Buffer.Replace(insertedRange, change.RemovedText);
        UndoManager.PushRedo(change);
        return true;
    }

    /// <summary>Redoes the last undone document change.</summary>
    public bool Redo()
    {
        if (!UndoManager.CanRedo)
        {
            return false;
        }

        var change = UndoManager.PopRedo();
        var removedRange = new TextRange(change.StartPosition, Advance(change.StartPosition, change.RemovedText));
        Buffer.Replace(removedRange, change.InsertedText);
        UndoManager.PushUndo(change);
        return true;
    }

    private static TextPosition Advance(TextPosition start, string text)
    {
        var line = start.Line;
        var column = start.Column;

        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == '\r')
            {
                if (i + 1 < text.Length && text[i + 1] == '\n')
                {
                    i++;
                }

                line++;
                column = 0;
            }
            else if (text[i] == '\n')
            {
                line++;
                column = 0;
            }
            else
            {
                column++;
            }
        }

        return new TextPosition(line, column);
    }
}
