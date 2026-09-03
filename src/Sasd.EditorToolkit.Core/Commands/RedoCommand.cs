namespace Sasd.EditorToolkit.Commands;

/// <summary>Redoes the last undone edit.</summary>
public sealed class RedoCommand : IEditorCommand
{
    /// <inheritdoc />
    public EditorCommandId Id => EditorCommandIds.Redo;

    /// <inheritdoc />
    public bool CanExecute(EditorCommandContext context) => context.Document.UndoManager.CanRedo;

    /// <inheritdoc />
    public ValueTask<CommandResult> ExecuteAsync(EditorCommandContext context, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(context.Document.Redo() ? CommandResult.Success() : CommandResult.NotHandled());
    }
}
