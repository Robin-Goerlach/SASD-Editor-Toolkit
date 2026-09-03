using System.Text;
using System.Windows.Forms;
using Sasd.EditorToolkit.Commands;
using Sasd.EditorToolkit.Documents;
using Sasd.EditorToolkit.Search;
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
    private TextDocument? _currentDocument;
    private string _lastSearchText = string.Empty;

    public MainForm()
    {
        Text = "SASD Editor Toolkit - Modern FIRST-ED Demo";
        Width = 1200;
        Height = 800;
        _editor.ViewStateChanged += (_, _) => UpdateStatus();

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

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (!ConfirmSaveBeforeDestructiveAction())
        {
            e.Cancel = true;
            return;
        }

        base.OnFormClosing(e);
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
        edit.DropDownItems.Add("Undo", null, async (_, _) => await ExecuteEditorCommandAsync(EditorCommandIds.Undo));
        edit.DropDownItems.Add("Redo", null, async (_, _) => await ExecuteEditorCommandAsync(EditorCommandIds.Redo));

        menu.Items.Add(file);
        menu.Items.Add(edit);
        menu.Items.Add(BuildSearchMenu());
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
        toolbar.Items.Add("Undo", null, async (_, _) => await ExecuteEditorCommandAsync(EditorCommandIds.Undo));
        toolbar.Items.Add("Redo", null, async (_, _) => await ExecuteEditorCommandAsync(EditorCommandIds.Redo));
        toolbar.Items.Add(new ToolStripSeparator());
        toolbar.Items.Add("Find", null, async (_, _) => await FindTextAsync(prompt: true));
        toolbar.Items.Add("Find Next", null, async (_, _) => await FindTextAsync(prompt: false));
        return toolbar;
    }

    private ToolStripMenuItem BuildSearchMenu()
    {
        var search = new ToolStripMenuItem("Search");
        search.DropDownItems.Add("Find...", null, async (_, _) => await FindTextAsync(prompt: true));
        search.DropDownItems.Add("Find Next", null, async (_, _) => await FindTextAsync(prompt: false));
        return search;
    }

    private void NewDocument()
    {
        if (!ConfirmSaveBeforeDestructiveAction())
        {
            return;
        }

        var text = "// SASD Editor Toolkit - Modern FIRST-ED demo" + Environment.NewLine
            + "using Sasd.EditorToolkit.Documents;" + Environment.NewLine
            + "using Sasd.EditorToolkit.Text;" + Environment.NewLine
            + Environment.NewLine
            + "var document = new TextDocument(new LineTextBuffer());" + Environment.NewLine
            + "document.Insert(TextPosition.Start, \"Hello Editor Toolkit!\");" + Environment.NewLine;

        var document = new TextDocument(new LineTextBuffer(text), new DocumentMetadata { DisplayName = "Untitled" });
        document.MarkSaved();
        BindDocument(document);
    }

    private async Task OpenDocumentAsync()
    {
        if (!ConfirmSaveBeforeDestructiveAction())
        {
            return;
        }

        using var dialog = new OpenFileDialog { Filter = "Text files|*.txt;*.md;*.cs;*.json;*.xml|All files|*.*" };
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            var result = await _storage.LoadFileAsync(dialog.FileName);
            BindDocument(result.Document);
        }
        catch (IOException ex)
        {
            ShowOpenError(ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            ShowOpenError(ex);
        }
        catch (DecoderFallbackException ex)
        {
            ShowOpenError(ex);
        }
    }

    private async Task<bool> SaveDocumentAsync(bool saveAs)
    {
        if (_editor.Document is null)
        {
            return false;
        }

        var path = _editor.Document.Metadata.FilePath;
        if (saveAs || string.IsNullOrWhiteSpace(path))
        {
            using var dialog = new SaveFileDialog { Filter = "Text files|*.txt;*.md;*.cs;*.json;*.xml|All files|*.*" };
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return false;
            }

            path = dialog.FileName;
        }

        try
        {
            await _storage.SaveFileAsync(_editor.Document, path);
            UpdateStatus();
            return true;
        }
        catch (IOException ex)
        {
            ShowSaveError(ex);
            return false;
        }
        catch (UnauthorizedAccessException ex)
        {
            ShowSaveError(ex);
            return false;
        }
    }

    private async Task FindTextAsync(bool prompt)
    {
        if (_editor.Document is null)
        {
            return;
        }

        var searchText = _lastSearchText;
        if (prompt || string.IsNullOrEmpty(searchText))
        {
            var promptedText = PromptForSearchText(searchText);
            if (promptedText is null)
            {
                return;
            }

            searchText = promptedText;
            _lastSearchText = searchText;
        }

        await ExecuteEditorCommandAsync(EditorCommandIds.FindText, new SearchRequest(searchText));
    }

    private async Task ExecuteEditorCommandAsync(EditorCommandId commandId, object? parameter = null)
    {
        var result = await _editor.ExecuteCommandAsync(commandId, parameter);
        if (!string.IsNullOrWhiteSpace(result.Message))
        {
            _status.Text = result.Message;
            return;
        }

        UpdateStatus();
    }

    private void BindDocument(TextDocument document)
    {
        if (_currentDocument is not null)
        {
            _currentDocument.Buffer.Changed -= DocumentBufferChanged;
        }

        _currentDocument = document;
        _currentDocument.Buffer.Changed += DocumentBufferChanged;
        _editor.Document = document;
        UpdateStatus();
    }

    private void DocumentBufferChanged(object? sender, TextBufferChangedEventArgs e) => UpdateStatus();

    private bool ConfirmSaveBeforeDestructiveAction()
    {
        if (_editor.Document is null || !_editor.Document.IsDirty)
        {
            return true;
        }

        var displayName = _editor.Document.Metadata.DisplayName;
        var result = MessageBox.Show(
            this,
            $"Save changes to '{displayName}' before continuing?",
            "Unsaved changes",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Warning);

        if (result == DialogResult.Cancel)
        {
            return false;
        }

        if (result == DialogResult.No)
        {
            return true;
        }

        return SaveDocumentAsync(saveAs: false).GetAwaiter().GetResult();
    }

    private string? PromptForSearchText(string initialText)
    {
        using var dialog = new Form
        {
            Text = "Find",
            Width = 400,
            Height = 145,
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MinimizeBox = false,
            MaximizeBox = false
        };

        var label = new Label { Text = "Find text:", AutoSize = true, Left = 12, Top = 15 };
        var textBox = new TextBox { Left = 12, Top = 38, Width = 360, Text = initialText };
        var findButton = new Button { Text = "Find", DialogResult = DialogResult.OK, Left = 216, Top = 72, Width = 75 };
        var cancelButton = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Left = 297, Top = 72, Width = 75 };

        dialog.Controls.AddRange(new Control[] { label, textBox, findButton, cancelButton });
        dialog.AcceptButton = findButton;
        dialog.CancelButton = cancelButton;
        dialog.Shown += (_, _) =>
        {
            textBox.Focus();
            textBox.SelectAll();
        };

        return dialog.ShowDialog(this) == DialogResult.OK ? textBox.Text : null;
    }

    private void ShowOpenError(Exception exception)
    {
        MessageBox.Show(
            this,
            exception.Message,
            "Open failed",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    private void ShowSaveError(Exception exception)
    {
        MessageBox.Show(
            this,
            exception.Message,
            "Save failed",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
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
