using Sasd.EditorToolkit.Text;

namespace Sasd.EditorToolkit.Commands;

/// <summary>Inserts text at the current caret position.</summary>
public sealed class InsertTextCommand : IEditorCommand
{
    /// <inheritdoc />
    public EditorCommandId Id => EditorCommandIds.InsertText;

    /// <inheritdoc />
    public bool CanExecute(EditorCommandContext context) => context.Parameter is string;

    /// <inheritdoc />
    public ValueTask<CommandResult> ExecuteAsync(EditorCommandContext context, CancellationToken cancellationToken = default)
    {
        var text = (string)context.Parameter!;
        context.Document.Insert(context.ViewState.CaretPosition, text);
        context.ViewState.MoveCaret(Advance(context.ViewState.CaretPosition, text));
        return ValueTask.FromResult(CommandResult.Success());
    }

    private static TextPosition Advance(TextPosition start, string text)
    {
        var line = start.Line;
        var column = start.Column;
        foreach (var c in text)
        {
            if (c == '\n')
            {
                line++;
                column = 0;
            }
            else if (c != '\r')
            {
                column++;
            }
        }

        return new TextPosition(line, column);
    }
}
