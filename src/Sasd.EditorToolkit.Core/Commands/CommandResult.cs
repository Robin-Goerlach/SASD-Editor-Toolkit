namespace Sasd.EditorToolkit.Commands;

/// <summary>Result of a command execution.</summary>
public sealed record CommandResult(bool Handled, string? Message = null)
{
    /// <summary>A successful handled command.</summary>
    public static CommandResult Success(string? message = null) => new(true, message);

    /// <summary>A command that could not be handled.</summary>
    public static CommandResult NotHandled(string? message = null) => new(false, message);
}
