using Sasd.EditorToolkit.Text;

namespace Sasd.EditorToolkit.Commands;

/// <summary>Deletes one character right of the caret.</summary>
public sealed class DeleteRightCommand : IEditorCommand
{
    /// <inheritdoc />
    public EditorCommandId Id => EditorCommandIds.DeleteRight;

    /// <inheritdoc />
    public bool CanExecute(EditorCommandContext context)
    {
        var buffer = context.Document.Buffer;
        return buffer.GetOffset(context.ViewState.CaretPosition) < buffer.Length;
    }

    /// <inheritdoc />
    public ValueTask<CommandResult> ExecuteAsync(EditorCommandContext context, CancellationToken cancellationToken = default)
    {
        var buffer = context.Document.Buffer;
        var caret = buffer.Normalize(context.ViewState.CaretPosition);
        var caretOffset = buffer.GetOffset(caret);
        var nextPosition = buffer.GetPosition(caretOffset + 1);

        context.Document.Delete(new TextRange(caret, nextPosition));
        context.ViewState.MoveCaret(caret);
        return ValueTask.FromResult(CommandResult.Success());
    }
}
