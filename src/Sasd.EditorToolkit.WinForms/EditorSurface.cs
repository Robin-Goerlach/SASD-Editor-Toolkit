using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Sasd.EditorToolkit.Documents;
using Sasd.EditorToolkit.Editing;
using Sasd.EditorToolkit.Text;

namespace Sasd.EditorToolkit.WinForms;

/// <summary>Minimal custom text rendering surface for the WinForms adapter.</summary>
public sealed class EditorSurface : Control
{
    private TextDocument? _document;

    /// <summary>Creates a surface.</summary>
    public EditorSurface()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        Font = new Font(FontFamily.GenericMonospace, 10.0f);
        TabStop = true;
    }

    /// <summary>Raised after caret or view-state changes initiated by the surface.</summary>
    public event EventHandler? ViewStateChanged;

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

        Document.Insert(ViewState.CaretPosition, e.KeyChar.ToString());
        MoveCaret(new TextPosition(ViewState.CaretPosition.Line, ViewState.CaretPosition.Column + 1));
        e.Handled = true;
        Invalidate();
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (Document is null)
        {
            return;
        }

        var buffer = Document.Buffer;
        var caret = buffer.Normalize(ViewState.CaretPosition);

        if (e.KeyCode == Keys.Left)
        {
            var offset = Math.Max(0, buffer.GetOffset(caret) - 1);
            MoveCaret(buffer.GetPosition(offset));
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.Right)
        {
            var offset = Math.Min(buffer.Length, buffer.GetOffset(caret) + 1);
            MoveCaret(buffer.GetPosition(offset));
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.Up)
        {
            MoveCaret(buffer.Normalize(new TextPosition(caret.Line - 1, caret.Column)));
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.Down)
        {
            MoveCaret(buffer.Normalize(new TextPosition(caret.Line + 1, caret.Column)));
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.Back && buffer.GetOffset(caret) > 0)
        {
            var offset = buffer.GetOffset(caret);
            var newPosition = buffer.GetPosition(offset - 1);
            Document.Delete(new TextRange(newPosition, caret));
            MoveCaret(newPosition);
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.Enter)
        {
            Document.Insert(caret, Environment.NewLine);
            MoveCaret(new TextPosition(caret.Line + 1, 0));
            e.Handled = true;
        }

        Invalidate();
    }

    /// <inheritdoc />
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        Focus();
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
