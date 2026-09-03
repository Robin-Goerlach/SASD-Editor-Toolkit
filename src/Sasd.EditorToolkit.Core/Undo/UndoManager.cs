using Sasd.EditorToolkit.Text;

namespace Sasd.EditorToolkit.Undo;

/// <summary>Maintains a minimal document-level undo/redo history.</summary>
public sealed class UndoManager
{
    private readonly Stack<TextChangeSet> _undo = new();
    private readonly Stack<TextChangeSet> _redo = new();

    /// <summary>Gets whether undo can be executed.</summary>
    public bool CanUndo => _undo.Count > 0;

    /// <summary>Gets whether redo can be executed.</summary>
    public bool CanRedo => _redo.Count > 0;

    /// <summary>Registers a user change and clears redo history.</summary>
    public void Register(TextChangeSet change)
    {
        _undo.Push(change);
        _redo.Clear();
    }

    /// <summary>Gets the next undo change.</summary>
    public TextChangeSet PopUndo() => _undo.Pop();

    /// <summary>Adds a change to the redo stack.</summary>
    public void PushRedo(TextChangeSet change) => _redo.Push(change);

    /// <summary>Gets the next redo change.</summary>
    public TextChangeSet PopRedo() => _redo.Pop();

    /// <summary>Adds a change back to the undo stack after redo.</summary>
    public void PushUndo(TextChangeSet change) => _undo.Push(change);

    /// <summary>Clears all history.</summary>
    public void Clear()
    {
        _undo.Clear();
        _redo.Clear();
    }
}
