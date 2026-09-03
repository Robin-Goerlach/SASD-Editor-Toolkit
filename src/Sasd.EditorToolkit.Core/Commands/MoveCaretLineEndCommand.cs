using Sasd.EditorToolkit.Text;

namespace Sasd.EditorToolkit.Commands;

/// <summary>Moves the caret to the end of the current logical line.</summary>
public sealed class MoveCaretLineEndCommand : IEditorCommand
{
    /// <inheritdoc />
    public EditorCommandId Id => EditorCommandIds.MoveCaretLineEnd;

    /// <inheritdoc />
    public bool CanExecute(EditorCommandContext context)
    {
        var buffer = context.Document.Buffer;
        var caret = buffer.Normalize(context.ViewState.CaretPosition);
        return caret.Column < buffer.GetLineText(caret.Line).Length;
    }

    /// <inheritdoc />
    public ValueTask<CommandResult> ExecuteAsync(EditorCommandContext context, CancellationToken cancellationToken = default)
    {
        var buffer = context.Document.Buffer;
        var caret = buffer.Normalize(context.ViewState.CaretPosition);
        var lineEnd = buffer.GetLineText(caret.Line).Length;
        context.ViewState.MoveCaret(new TextPosition(caret.Line, lineEnd));
        return ValueTask.FromResult(CommandResult.Success());
    }
}
