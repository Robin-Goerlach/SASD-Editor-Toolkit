using Sasd.EditorToolkit.Text;

namespace Sasd.EditorToolkit.Commands;

/// <summary>Moves the caret to the start of the current logical line.</summary>
public sealed class MoveCaretLineStartCommand : IEditorCommand
{
    /// <inheritdoc />
    public EditorCommandId Id => EditorCommandIds.MoveCaretLineStart;

    /// <inheritdoc />
    public bool CanExecute(EditorCommandContext context)
    {
        var caret = context.Document.Buffer.Normalize(context.ViewState.CaretPosition);
        return caret.Column > 0;
    }

    /// <inheritdoc />
    public ValueTask<CommandResult> ExecuteAsync(EditorCommandContext context, CancellationToken cancellationToken = default)
    {
        var caret = context.Document.Buffer.Normalize(context.ViewState.CaretPosition);
        context.ViewState.MoveCaret(new TextPosition(caret.Line, 0));
        return ValueTask.FromResult(CommandResult.Success());
    }
}
