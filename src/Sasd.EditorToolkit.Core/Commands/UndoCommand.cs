namespace Sasd.EditorToolkit.Commands;

/// <summary>Undoes the last edit.</summary>
public sealed class UndoCommand : IEditorCommand
{
    /// <inheritdoc />
    public EditorCommandId Id => EditorCommandIds.Undo;

    /// <inheritdoc />
    public bool CanExecute(EditorCommandContext context) => context.Document.UndoManager.CanUndo;

    /// <inheritdoc />
    public ValueTask<CommandResult> ExecuteAsync(EditorCommandContext context, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(context.Document.Undo() ? CommandResult.Success() : CommandResult.NotHandled());
    }
}
