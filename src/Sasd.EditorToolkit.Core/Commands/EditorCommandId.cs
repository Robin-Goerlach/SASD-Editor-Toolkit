namespace Sasd.EditorToolkit.Commands;

/// <summary>Stable language-neutral command id.</summary>
public readonly record struct EditorCommandId(string Value)
{
    /// <inheritdoc />
    public override string ToString() => Value;
}
