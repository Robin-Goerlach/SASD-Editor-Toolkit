using Sasd.EditorToolkit.Text;

namespace Sasd.EditorToolkit.Commands;

/// <summary>Inserts a newline at the caret.</summary>
public sealed class NewLineCommand : IEditorCommand
{
    /// <inheritdoc />
    public EditorCommandId Id => EditorCommandIds.NewLine;

    /// <inheritdoc />
    public bool CanExecute(EditorCommandContext context) => true;

    /// <inheritdoc />
    public ValueTask<CommandResult> ExecuteAsync(EditorCommandContext context, CancellationToken cancellationToken = default)
    {
        var text = context.Document.Metadata.PreferredLineEnding == LineEndingKind.Lf ? "\n" : "\r\n";
        var caret = context.ViewState.CaretPosition;
        context.Document.Insert(caret, text);
        context.ViewState.MoveCaret(new TextPosition(caret.Line + 1, 0));
        return ValueTask.FromResult(CommandResult.Success());
    }
}
