using Sasd.EditorToolkit.Text;

namespace Sasd.EditorToolkit.Commands;

/// <summary>Moves the caret one text position to the right.</summary>
public sealed class MoveCaretRightCommand : IEditorCommand
{
    /// <inheritdoc />
    public EditorCommandId Id => EditorCommandIds.MoveCaretRight;

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
        var nextOffset = Math.Min(buffer.Length, buffer.GetOffset(caret) + 1);
        context.ViewState.MoveCaret(buffer.GetPosition(nextOffset));
        return ValueTask.FromResult(CommandResult.Success());
    }
}
