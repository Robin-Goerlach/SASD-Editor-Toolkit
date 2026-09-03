namespace Sasd.EditorToolkit.Text;

/// <summary>Event data for changes in an <see cref="ITextBuffer"/>.</summary>
public sealed class TextBufferChangedEventArgs : EventArgs
{
    /// <summary>Creates a new event args object.</summary>
    public TextBufferChangedEventArgs(TextChangeSet change) => Change = change;

    /// <summary>The atomic change that was applied.</summary>
    public TextChangeSet Change { get; }
}
