using Sasd.EditorToolkit.Text;

namespace Sasd.EditorToolkit.Editing;

/// <summary>Represents a selection in a view.</summary>
public sealed class TextSelection
{
    /// <summary>Creates an empty selection at document start.</summary>
    public TextSelection()
    {
        Anchor = TextPosition.Start;
        Active = TextPosition.Start;
    }

    /// <summary>Gets or sets the fixed selection anchor.</summary>
    public TextPosition Anchor { get; set; }

    /// <summary>Gets or sets the active selection edge.</summary>
    public TextPosition Active { get; set; }

    /// <summary>Gets whether the selection is empty.</summary>
    public bool IsEmpty => Anchor.Equals(Active);

    /// <summary>Gets the selected range.</summary>
    public TextRange Range => new(Anchor, Active);

    /// <summary>Clears the selection and moves it to a position.</summary>
    public void Clear(TextPosition position)
    {
        Anchor = position;
        Active = position;
    }
}
