namespace Sasd.EditorToolkit.Commands;

/// <summary>Registry of editor commands.</summary>
public sealed class EditorCommandRegistry
{
    private readonly Dictionary<EditorCommandId, IEditorCommand> _commands = new();

    /// <summary>Registers or replaces a command.</summary>
    public void Register(IEditorCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        _commands[command.Id] = command;
    }

    /// <summary>Attempts to get a command.</summary>
    public bool TryGet(EditorCommandId id, out IEditorCommand command) => _commands.TryGetValue(id, out command!);

    /// <summary>Registers the minimal M1 command set.</summary>
    public void RegisterM1Defaults()
    {
        Register(new InsertTextCommand());
        Register(new NewLineCommand());
        Register(new DeleteLeftCommand());
        Register(new DeleteRightCommand());
        Register(new MoveCaretLeftCommand());
        Register(new MoveCaretRightCommand());
        Register(new MoveCaretUpCommand());
        Register(new MoveCaretDownCommand());
        Register(new UndoCommand());
        Register(new RedoCommand());
    }
}
