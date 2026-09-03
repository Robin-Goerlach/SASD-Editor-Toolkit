using Sasd.EditorToolkit.Text;

namespace Sasd.EditorToolkit.Commands;

/// <summary>Moves the caret one text position to the left.</summary>
public sealed class MoveCaretLeftCommand : IEditorCommand
{
    /// <inheritdoc />
    public EditorCommandId Id => EditorCommandIds.MoveCaretLeft;

    /// <inheritdoc />
    public bool CanExecute(EditorCommandContext context)
    {
        var buffer = context.Document.Buffer;
        return buffer.GetOffset(context.ViewState.CaretPosition) > 0;
    }

    /// <inheritdoc />
    public ValueTask<CommandResult> ExecuteAsync(EditorCommandContext context, CancellationToken cancellationToken = default)
    {
        var buffer = context.Document.Buffer;
        var caret = buffer.Normalize(context.ViewState.CaretPosition);
        var nextOffset = Math.Max(0, buffer.GetOffset(caret) - 1);
        context.ViewState.MoveCaret(buffer.GetPosition(nextOffset));
        return ValueTask.FromResult(CommandResult.Success());
    }
}
