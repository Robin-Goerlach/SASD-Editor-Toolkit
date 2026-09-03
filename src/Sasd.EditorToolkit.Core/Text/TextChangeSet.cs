namespace Sasd.EditorToolkit.Text;

/// <summary>
/// Describes one atomic text replacement.
/// </summary>
public sealed record TextChangeSet(
    TextPosition StartPosition,
    string RemovedText,
    string InsertedText,
    long OldVersion,
    long NewVersion);
