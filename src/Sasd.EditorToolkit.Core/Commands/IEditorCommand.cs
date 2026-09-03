namespace Sasd.EditorToolkit.Commands;

/// <summary>Executable editor command.</summary>
public interface IEditorCommand
{
    /// <summary>Gets the stable command id.</summary>
    EditorCommandId Id { get; }

    /// <summary>Checks whether the command can execute in the supplied context.</summary>
    bool CanExecute(EditorCommandContext context);

    /// <summary>Executes the command.</summary>
    ValueTask<CommandResult> ExecuteAsync(EditorCommandContext context, CancellationToken cancellationToken = default);
}
