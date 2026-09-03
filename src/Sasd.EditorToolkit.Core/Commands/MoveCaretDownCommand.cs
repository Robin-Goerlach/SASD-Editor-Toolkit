using Sasd.EditorToolkit.Text;

namespace Sasd.EditorToolkit.Commands;

/// <summary>Moves the caret one logical line down.</summary>
public sealed class MoveCaretDownCommand : IEditorCommand
{
    /// <inheritdoc />
    public EditorCommandId Id => EditorCommandIds.MoveCaretDown;

    /// <inheritdoc />
    public bool CanExecute(EditorCommandContext context)
    {
        var buffer = context.Document.Buffer;
        var caret = buffer.Normalize(context.ViewState.CaretPosition);
        return caret.Line < buffer.LineCount - 1;
    }

    /// <inheritdoc />
    public ValueTask<CommandResult> ExecuteAsync(EditorCommandContext context, CancellationToken cancellationToken = default)
    {
        var caret = context.Document.Buffer.Normalize(context.ViewState.CaretPosition);
        var target = new TextPosition(caret.Line + 1, caret.Column);
        context.ViewState.MoveCaret(context.Document.Buffer.Normalize(target));
        return ValueTask.FromResult(CommandResult.Success());
    }
}
