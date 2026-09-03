using Sasd.EditorToolkit.Text;

namespace Sasd.EditorToolkit.Editing;

/// <summary>
/// UI-independent state of one view on a document.
/// </summary>
public sealed class EditorViewState
{
    /// <summary>Gets or sets the caret position.</summary>
    public TextPosition CaretPosition { get; set; } = TextPosition.Start;

    /// <summary>Gets the current selection.</summary>
    public TextSelection Selection { get; } = new();

    /// <summary>Gets or sets the first visible logical line.</summary>
    public int FirstVisibleLine { get; set; }

    /// <summary>Gets or sets the horizontal scroll offset.</summary>
    public int HorizontalOffset { get; set; }

    /// <summary>Gets or sets whether insert mode is enabled.</summary>
    public bool InsertMode { get; set; } = true;

    /// <summary>Gets or sets whether visual word wrap is enabled.</summary>
    public bool WordWrap { get; set; }

    /// <summary>Gets or sets whether auto-indent is enabled.</summary>
    public bool AutoIndent { get; set; }

    /// <summary>Gets or sets the tab size.</summary>
    public int TabSize { get; set; } = 4;

    /// <summary>Moves the caret and clears the selection.</summary>
    public void MoveCaret(TextPosition position)
    {
        CaretPosition = position;
        Selection.Clear(position);
    }
}
