using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Sasd.EditorToolkit.Commands;
using Sasd.EditorToolkit.Documents;
using Sasd.EditorToolkit.Editing;
using Sasd.EditorToolkit.Text;

namespace Sasd.EditorToolkit.WinForms;

/// <summary>Minimal custom text rendering surface for the WinForms adapter.</summary>
public sealed class EditorSurface : Control
{
    private readonly EditorCommandDispatcher _defaultCommandDispatcher;
    private EditorCommandDispatcher _commandDispatcher;
    private TextDocument? _document;

    /// <summary>Creates a surface.</summary>
    public EditorSurface()
    {
        var registry = new EditorCommandRegistry();
        registry.RegisterM1Defaults();
        _defaultCommandDispatcher = new EditorCommandDispatcher(registry);
        _commandDispatcher = _defaultCommandDispatcher;

        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        Font = new Font(FontFamily.GenericMonospace, 10.0f);
        TabStop = true;
    }

    /// <summary>Raised after caret or view-state changes initiated by the surface.</summary>
    public event EventHandler? ViewStateChanged;

    /// <summary>Gets or sets the dispatcher used for keyboard-driven editor commands.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public EditorCommandDispatcher CommandDispatcher
    {
        get => _commandDispatcher;
        set => _commandDispatcher = value ?? _defaultCommandDispatcher;
    }

    /// <summary>Gets or sets the bound document.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TextDocument? Document
    {
        get => _document;
        set
        {
            if (_document is not null)
            {
                _document.Buffer.Changed -= BufferChanged;
            }

            _document = value;

            if (_document is not null)
            {
                _document.Buffer.Changed += BufferChanged;
                MoveCaret(_document.Buffer.Normalize(ViewState.CaretPosition));
            }

            Invalidate();
        }
    }

    /// <summary>Gets the view state.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public EditorViewState ViewState { get; } = new();

    /// <inheritdoc />
    protected override bool IsInputKey(Keys keyData) => true;

    /// <inheritdoc />
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.Clear(SystemColors.Window);

        if (Document is null)
        {
            TextRenderer.DrawText(e.Graphics, "No document", Font, ClientRectangle, SystemColors.GrayText);
            return;
        }

        var lineHeight = TextRenderer.MeasureText(e.Graphics, "Mg", Font, Size.Empty, TextFormatFlags.NoPadding).Height + 2;
        var gutterWidth = 56;
        var visibleLines = Math.Max(1, ClientSize.Height / Math.Max(1, lineHeight));
        var firstLine = Math.Clamp(ViewState.FirstVisibleLine, 0, Math.Max(0, Document.Buffer.LineCount - 1));
        var lastLine = Math.Min(Document.Buffer.LineCount, firstLine + visibleLines + 1);

        using var gutterBrush = new SolidBrush(Color.FromArgb(248, 248, 248));
        e.Graphics.FillRectangle(gutterBrush, 0, 0, gutterWidth, ClientSize.Height);
        e.Graphics.DrawLine(SystemPens.ControlLight, gutterWidth, 0, gutterWidth, ClientSize.Height);

        for (var line = firstLine; line < lastLine; line++)
        {
            var y = (line - firstLine) * lineHeight;
            if (line == ViewState.CaretPosition.Line)
            {
                using var currentLineBrush = new SolidBrush(Color.FromArgb(232, 242, 255));
                e.Graphics.FillRectangle(currentLineBrush, gutterWidth + 1, y, ClientSize.Width - gutterWidth - 1, lineHeight);
            }

            TextRenderer.DrawText(e.Graphics, (line + 1).ToString(), Font, new Rectangle(0, y, gutterWidth - 6, lineHeight), SystemColors.GrayText, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
            TextRenderer.DrawText(e.Graphics, Document.Buffer.GetLineText(line).ToString(), Font, new Point(gutterWidth + 8, y + 1), SystemColors.WindowText, TextFormatFlags.NoPadding);
        }

        DrawCaret(e.Graphics, gutterWidth, lineHeight, firstLine);
    }

    /// <inheritdoc />
    protected override void OnKeyPress(KeyPressEventArgs e)
    {
        base.OnKeyPress(e);
        if (Document is null || char.IsControl(e.KeyChar))
        {
            return;
        }

        if (ExecuteEditorCommand(EditorCommandIds.InsertText, e.KeyChar.ToString()))
        {
            e.Handled = true;
        }
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (Document is null)
        {
            return;
        }

        if (!TryGetKeyCommand(e, out var commandId))
        {
            return;
        }

        ExecuteEditorCommand(commandId);
        e.Handled = true;
        e.SuppressKeyPress = true;
    }

    /// <inheritdoc />
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        Focus();
    }

    private static bool TryGetKeyCommand(KeyEventArgs e, out EditorCommandId commandId)
    {
        if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Z)
        {
            commandId = EditorCommandIds.Undo;
            return true;
        }

        if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Y)
        {
            commandId = EditorCommandIds.Redo;
            return true;
        }

        if (e.Modifiers != Keys.None)
        {
            commandId = default;
            return false;
        }

        commandId = e.KeyCode switch
        {
            Keys.Left => EditorCommandIds.MoveCaretLeft,
            Keys.Right => EditorCommandIds.MoveCaretRight,
            Keys.Up => EditorCommandIds.MoveCaretUp,
            Keys.Down => EditorCommandIds.MoveCaretDown,
            Keys.Back => EditorCommandIds.DeleteLeft,
            Keys.Delete => EditorCommandIds.DeleteRight,
            Keys.Enter => EditorCommandIds.NewLine,
            _ => default
        };

        return !commandId.Equals(default(EditorCommandId));
    }

    private bool ExecuteEditorCommand(EditorCommandId commandId, object? parameter = null)
    {
        if (Document is null)
        {
            return false;
        }

        var context = new EditorCommandContext(Document, ViewState, parameter);
        var result = CommandDispatcher.ExecuteAsync(commandId, context).GetAwaiter().GetResult();
        if (!result.Handled)
        {
            return false;
        }

        MoveCaret(Document.Buffer.Normalize(ViewState.CaretPosition));
        Invalidate();
        return true;
    }

    private void DrawCaret(Graphics graphics, int gutterWidth, int lineHeight, int firstLine)
    {
        if (Document is null || ViewState.CaretPosition.Line < firstLine)
        {
            return;
        }

        var x = gutterWidth + 8 + TextRenderer.MeasureText(graphics, new string(' ', Math.Max(0, ViewState.CaretPosition.Column)), Font, Size.Empty, TextFormatFlags.NoPadding).Width;
        var y = (ViewState.CaretPosition.Line - firstLine) * lineHeight + 2;
        graphics.DrawLine(Pens.Black, x, y, x, y + lineHeight - 4);
    }

    private void MoveCaret(TextPosition position)
    {
        ViewState.MoveCaret(position);
        ViewStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void BufferChanged(object? sender, TextBufferChangedEventArgs e) => Invalidate();
}
