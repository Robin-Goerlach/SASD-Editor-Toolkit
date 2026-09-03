using Sasd.EditorToolkit.Documents;
using Sasd.EditorToolkit.Editing;

namespace Sasd.EditorToolkit.Commands;

/// <summary>Context supplied to editor commands.</summary>
public sealed class EditorCommandContext
{
    /// <summary>Creates a command context.</summary>
    public EditorCommandContext(TextDocument document, EditorViewState viewState, object? parameter = null)
    {
        Document = document;
        ViewState = viewState;
        Parameter = parameter;
    }

    /// <summary>The target document.</summary>
    public TextDocument Document { get; }

    /// <summary>The target view state.</summary>
    public EditorViewState ViewState { get; }

    /// <summary>Optional command parameter.</summary>
    public object? Parameter { get; }
}
