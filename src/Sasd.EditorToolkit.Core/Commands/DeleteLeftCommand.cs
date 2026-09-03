using Sasd.EditorToolkit.Text;

namespace Sasd.EditorToolkit.Commands;

/// <summary>Deletes one character left of the caret.</summary>
public sealed class DeleteLeftCommand : IEditorCommand
{
    /// <inheritdoc />
    public EditorCommandId Id => EditorCommandIds.DeleteLeft;

    /// <inheritdoc />
    public bool CanExecute(EditorCommandContext context) => context.Document.Buffer.GetOffset(context.ViewState.CaretPosition) > 0;

    /// <inheritdoc />
    public ValueTask<CommandResult> ExecuteAsync(EditorCommandContext context, CancellationToken cancellationToken = default)
    {
        var buffer = context.Document.Buffer;
        var caretOffset = buffer.GetOffset(context.ViewState.CaretPosition);
        var newPosition = buffer.GetPosition(caretOffset - 1);
        context.Document.Delete(new TextRange(newPosition, context.ViewState.CaretPosition));
        context.ViewState.MoveCaret(newPosition);
        return ValueTask.FromResult(CommandResult.Success());
    }
}
