using Sasd.EditorToolkit.Text;

namespace Sasd.EditorToolkit.Commands;

/// <summary>Moves the caret one logical line up.</summary>
public sealed class MoveCaretUpCommand : IEditorCommand
{
    /// <inheritdoc />
    public EditorCommandId Id => EditorCommandIds.MoveCaretUp;

    /// <inheritdoc />
    public bool CanExecute(EditorCommandContext context) => context.ViewState.CaretPosition.Line > 0;

    /// <inheritdoc />
    public ValueTask<CommandResult> ExecuteAsync(EditorCommandContext context, CancellationToken cancellationToken = default)
    {
        var caret = context.Document.Buffer.Normalize(context.ViewState.CaretPosition);
        var target = new TextPosition(caret.Line - 1, caret.Column);
        context.ViewState.MoveCaret(context.Document.Buffer.Normalize(target));
        return ValueTask.FromResult(CommandResult.Success());
    }
}
