using Sasd.EditorToolkit.Documents;

namespace Sasd.EditorToolkit.Workspaces;

/// <summary>Maintains documents and views for a host session.</summary>
public sealed class EditorWorkspace
{
    private readonly List<TextDocument> _documents = new();
    private readonly List<EditorView> _views = new();

    /// <summary>Gets all documents.</summary>
    public IReadOnlyList<TextDocument> Documents => _documents;

    /// <summary>Gets all views.</summary>
    public IReadOnlyList<EditorView> Views => _views;

    /// <summary>Gets the active view if one exists.</summary>
    public EditorView? ActiveView { get; private set; }

    /// <summary>Adds a document and creates its first view.</summary>
    public EditorView AddDocument(TextDocument document)
    {
        _documents.Add(document);
        return AddView(document);
    }

    /// <summary>Adds another view for an existing document.</summary>
    public EditorView AddView(TextDocument document)
    {
        if (!_documents.Contains(document))
        {
            _documents.Add(document);
        }

        var view = new EditorView(document);
        _views.Add(view);
        ActiveView = view;
        return view;
    }

    /// <summary>Activates a view.</summary>
    public bool Activate(Guid viewId)
    {
        var view = _views.FirstOrDefault(v => v.Id == viewId);
        if (view is null)
        {
            return false;
        }

        ActiveView = view;
        return true;
    }
}
