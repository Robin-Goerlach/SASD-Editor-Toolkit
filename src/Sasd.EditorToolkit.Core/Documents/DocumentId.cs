namespace Sasd.EditorToolkit.Documents;

/// <summary>Stable document identity independent from file path.</summary>
public readonly record struct DocumentId(Guid Value)
{
    /// <summary>Creates a new random document id.</summary>
    public static DocumentId NewId() => new(Guid.NewGuid());

    /// <inheritdoc />
    public override string ToString() => Value.ToString("N");
}
