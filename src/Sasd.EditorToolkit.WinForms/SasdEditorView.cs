using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Sasd.EditorToolkit.Documents;
using Sasd.EditorToolkit.Editing;

namespace Sasd.EditorToolkit.WinForms;

/// <summary>
/// WinForms editor view control. M1 uses a small custom surface to keep product logic out of WinForms controls.
/// </summary>
public sealed class SasdEditorView : UserControl
{
    private readonly EditorSurface _surface = new() { Dock = DockStyle.Fill };

    /// <summary>Creates the editor view control.</summary>
    public SasdEditorView()
    {
        _surface.ViewStateChanged += (_, _) => ViewStateChanged?.Invoke(this, EventArgs.Empty);
        Controls.Add(_surface);
        BackColor = SystemColors.Window;
    }

    /// <summary>Raised when the underlying surface changes caret or view state.</summary>
    public event EventHandler? ViewStateChanged;

    /// <summary>Gets or sets the bound document.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TextDocument? Document
    {
        get => _surface.Document;
        set => _surface.Document = value;
    }

    /// <summary>Gets the view state.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public EditorViewState ViewState => _surface.ViewState;

    /// <summary>Refreshes the editor surface.</summary>
    public override void Refresh()
    {
        _surface.Invalidate();
        base.Refresh();
    }
}
