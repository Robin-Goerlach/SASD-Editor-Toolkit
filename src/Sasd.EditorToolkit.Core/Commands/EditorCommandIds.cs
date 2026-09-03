namespace Sasd.EditorToolkit.Commands;

/// <summary>Known command IDs for the reference implementation.</summary>
public static class EditorCommandIds
{
    /// <summary>Insert text at the caret.</summary>
    public static readonly EditorCommandId InsertText = new("Edit.InsertText");

    /// <summary>Insert a newline at the caret.</summary>
    public static readonly EditorCommandId NewLine = new("Edit.NewLine");

    /// <summary>Delete left of the caret.</summary>
    public static readonly EditorCommandId DeleteLeft = new("Edit.DeleteLeft");

    /// <summary>Delete right of the caret.</summary>
    public static readonly EditorCommandId DeleteRight = new("Edit.DeleteRight");

    /// <summary>Move the caret one text position to the left.</summary>
    public static readonly EditorCommandId MoveCaretLeft = new("Navigate.Left");

    /// <summary>Move the caret one text position to the right.</summary>
    public static readonly EditorCommandId MoveCaretRight = new("Navigate.Right");

    /// <summary>Move the caret one logical line up.</summary>
    public static readonly EditorCommandId MoveCaretUp = new("Navigate.Up");

    /// <summary>Move the caret one logical line down.</summary>
    public static readonly EditorCommandId MoveCaretDown = new("Navigate.Down");

    /// <summary>Undo the last edit.</summary>
    public static readonly EditorCommandId Undo = new("Edit.Undo");

    /// <summary>Redo the last undone edit.</summary>
    public static readonly EditorCommandId Redo = new("Edit.Redo");
}
