using Sasd.EditorToolkit.Documents;
using Sasd.EditorToolkit.Editing;

namespace Sasd.EditorToolkit.Workspaces;

/// <summary>Connects one view state to one document.</summary>
public sealed class EditorView
{
    /// <summary>Creates a view for a document.</summary>
    public EditorView(TextDocument document)
    {
        Id = Guid.NewGuid();
        Document = document;
    }

    /// <summary>Gets the view id.</summary>
    public Guid Id { get; }

    /// <summary>Gets the referenced document.</summary>
    public TextDocument Document { get; }

    /// <summary>Gets the view state.</summary>
    public EditorViewState State { get; } = new();
}
