namespace Sasd.EditorToolkit.Commands;

/// <summary>Dispatches command IDs to registered command implementations.</summary>
public sealed class EditorCommandDispatcher
{
    private readonly EditorCommandRegistry _registry;

    /// <summary>Creates a dispatcher.</summary>
    public EditorCommandDispatcher(EditorCommandRegistry registry) => _registry = registry;

    /// <summary>Executes a command by id.</summary>
    public async ValueTask<CommandResult> ExecuteAsync(EditorCommandId id, EditorCommandContext context, CancellationToken cancellationToken = default)
    {
        if (!_registry.TryGet(id, out var command))
        {
            return CommandResult.NotHandled($"Unknown command: {id.Value}");
        }

        if (!command.CanExecute(context))
        {
            return CommandResult.NotHandled($"Command cannot execute: {id.Value}");
        }

        return await command.ExecuteAsync(context, cancellationToken).ConfigureAwait(false);
    }
}
