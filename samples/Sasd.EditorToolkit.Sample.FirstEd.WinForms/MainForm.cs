using System.Windows.Forms;
using Sasd.EditorToolkit.Documents;
using Sasd.EditorToolkit.Storage;
using Sasd.EditorToolkit.Text;
using Sasd.EditorToolkit.WinForms;

namespace Sasd.EditorToolkit.Sample.FirstEd.WinForms;

/// <summary>Small Modern FIRST-ED sample host.</summary>
public sealed class MainForm : Form
{
    private readonly SasdEditorView _editor = new() { Dock = DockStyle.Fill };
    private readonly ToolStripStatusLabel _status = new();
    private readonly FileDocumentStorage _storage = new();

    public MainForm()
    {
        Text = "SASD Editor Toolkit - Modern FIRST-ED Demo";
        Width = 1200;
        Height = 800;

        var menu = BuildMenu();
        var toolbar = BuildToolbar();
        var statusStrip = new StatusStrip();
        statusStrip.Items.Add(_status);

        Controls.Add(_editor);
        Controls.Add(statusStrip);
        Controls.Add(toolbar);
        Controls.Add(menu);
        MainMenuStrip = menu;

        NewDocument();
        UpdateStatus();
    }

    private MenuStrip BuildMenu()
    {
        var menu = new MenuStrip();
        var file = new ToolStripMenuItem("File");
        file.DropDownItems.Add("New", null, (_, _) => NewDocument());
        file.DropDownItems.Add("Open", null, async (_, _) => await OpenDocumentAsync());
        file.DropDownItems.Add("Save", null, async (_, _) => await SaveDocumentAsync(false));
        file.DropDownItems.Add("Save As", null, async (_, _) => await SaveDocumentAsync(true));
        file.DropDownItems.Add(new ToolStripSeparator());
        file.DropDownItems.Add("Exit", null, (_, _) => Close());

        var edit = new ToolStripMenuItem("Edit");
        edit.DropDownItems.Add("Undo", null, (_, _) => { _editor.Document?.Undo(); _editor.Refresh(); UpdateStatus(); });
        edit.DropDownItems.Add("Redo", null, (_, _) => { _editor.Document?.Redo(); _editor.Refresh(); UpdateStatus(); });

        menu.Items.Add(file);
        menu.Items.Add(edit);
        menu.Items.Add(new ToolStripMenuItem("Search"));
        menu.Items.Add(new ToolStripMenuItem("View"));
        menu.Items.Add(new ToolStripMenuItem("Tools"));
        menu.Items.Add(new ToolStripMenuItem("Help"));
        return menu;
    }

    private ToolStrip BuildToolbar()
    {
        var toolbar = new ToolStrip();
        toolbar.Items.Add("New", null, (_, _) => NewDocument());
        toolbar.Items.Add("Open", null, async (_, _) => await OpenDocumentAsync());
        toolbar.Items.Add("Save", null, async (_, _) => await SaveDocumentAsync(false));
        toolbar.Items.Add(new ToolStripSeparator());
        toolbar.Items.Add("Undo", null, (_, _) => { _editor.Document?.Undo(); _editor.Refresh(); UpdateStatus(); });
        toolbar.Items.Add("Redo", null, (_, _) => { _editor.Document?.Redo(); _editor.Refresh(); UpdateStatus(); });
        return toolbar;
    }

    private void NewDocument()
    {
        var text = "// SASD Editor Toolkit - Modern FIRST-ED demo" + Environment.NewLine
            + "using Sasd.EditorToolkit.Documents;" + Environment.NewLine
            + "using Sasd.EditorToolkit.Text;" + Environment.NewLine
            + Environment.NewLine
            + "var document = new TextDocument(new LineTextBuffer());" + Environment.NewLine
            + "document.Insert(TextPosition.Start, \"Hello Editor Toolkit!\");" + Environment.NewLine;

        _editor.Document = new TextDocument(new LineTextBuffer(text), new DocumentMetadata { DisplayName = "Untitled" });
        _editor.Document.MarkSaved();
        UpdateStatus();
    }

    private async Task OpenDocumentAsync()
    {
        using var dialog = new OpenFileDialog { Filter = "Text files|*.txt;*.md;*.cs;*.json;*.xml|All files|*.*" };
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var result = await _storage.LoadFileAsync(dialog.FileName);
        _editor.Document = result.Document;
        UpdateStatus();
    }

    private async Task SaveDocumentAsync(bool saveAs)
    {
        if (_editor.Document is null)
        {
            return;
        }

        var path = _editor.Document.Metadata.FilePath;
        if (saveAs || string.IsNullOrWhiteSpace(path))
        {
            using var dialog = new SaveFileDialog { Filter = "Text files|*.txt;*.md;*.cs;*.json;*.xml|All files|*.*" };
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            path = dialog.FileName;
        }

        await _storage.SaveFileAsync(_editor.Document, path);
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        if (_editor.Document is null)
        {
            _status.Text = "No document";
            return;
        }

        var dirty = _editor.Document.IsDirty ? "*" : string.Empty;
        _status.Text = $"{_editor.Document.Metadata.DisplayName}{dirty} | Ln {_editor.ViewState.CaretPosition.Line + 1}, Col {_editor.ViewState.CaretPosition.Column + 1} | UTF-8 | INS | Profile: Modern";
    }
}
