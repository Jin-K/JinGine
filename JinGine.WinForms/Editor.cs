using JinGine.WinForms.Views;
using LegacyFwk;

namespace JinGine.WinForms;

/// <summary>
/// Represents our custom code-editor control.
/// </summary>
public partial class Editor : UserControl, IEditorView
{
    private readonly FontDescriptor _font;

    private IReadOnlyDictionary<int, string>? _textLinesByNumber;
    private int _currentLine = 1;
    private int _currentColumn = 1;

    public event EventHandler<char>? PressedKey;

    private int ScrollH
    {
        get => _hScrollBar?.Value ?? throw new InvalidOperationException("No horizontal scrollbar !!");
        set => (_hScrollBar ?? throw new InvalidOperationException("No horizontal scrollbar !!")).Value = value;
    }

    private int ScrollV
    {
        get => _vScrollBar.Value;
        set => _vScrollBar.Value = value;
    }

    /// <summary>
    /// Creates an instance of the <see cref="Editor"/> class.
    /// </summary>
    /// <remarks>Visual Studio Designer needs a constructor without parameters.</remarks>
    public Editor() : this(true) {}

    /// <summary>
    /// Creates an instance of the <see cref="Editor"/> class.
    /// </summary>
    /// <param name="withHorizontalScroll">Indicates if we want an horizontal scrollbar.</param>
    private Editor(bool withHorizontalScroll)
    {
        InitializeComponent();

        _font = FontDescriptor.DefaultFixed;
        base.Font = _font.Font;
        SetStyle(ControlStylesHelper.DoubleBufferedInputControl, true);

        if (withHorizontalScroll is false)
        {
            _hScrollBar.Scroll -= HScrollBar_Scroll;
            Controls.Remove(_hScrollBar);
            //_hScrollBar1.Dispose(); // TODO check if needs to be disposed
            _hScrollBar = null;
        }
    }

    /// <summary>
    /// Renders the control view by invalidating the control.
    /// </summary>
    /// <remarks>
    /// Additionally stores the <paramref name="textLinesByNumber"/> in a private field
    /// for next <see cref="Editor_Paint"/> calls.
    /// </remarks>
    /// <param name="textLinesByNumber">
    /// A dictionary of text-line entries where the keys represent the line numbers.
    /// </param>
    public void Render(IReadOnlyDictionary<int, string> textLinesByNumber)
    {
        _textLinesByNumber = textLinesByNumber;
        Invalidate();
    }

    /// <summary>
    /// Scrolls the view to specific cartesian coordinates in the text that is shown.
    /// </summary>
    /// <param name="line">The line number.</param>
    /// <param name="column">The column number in that line.</param>
    public void ScrollTo(int line, int column)
    {
        _currentLine = line;
        _currentColumn = column;
        ResetScrollbars();
    }

    private int GetPaintZoneTop(int line) => ClientRectangle.Top + (line - 1 - ScrollV) * _font.Height;

    private void ResetScrollbars()
    {
        // TODO optimize
        var paintZoneTop = GetPaintZoneTop(_currentLine);
        while (paintZoneTop < ClientRectangle.Top)
        {
            ScrollV--;
            paintZoneTop += _font.Height;
        }

        var hScrollBarHeight = _hScrollBar?.Height ?? 0;
        var paintZoneBottom = paintZoneTop + _font.Height;
        while (paintZoneBottom > ClientRectangle.Bottom - hScrollBarHeight)
        {
            ScrollV++;
            paintZoneBottom -= _font.Height;
        }

        var paintZoneLeft = ClientRectangle.Left + (_currentColumn - 1 - ScrollH) * _font.Width;
        while (paintZoneLeft < ClientRectangle.Left)
        {
            ScrollH--;
            paintZoneLeft += _font.Width;
        }

        var paintZoneRight = paintZoneLeft + _font.Width;
        while (paintZoneRight > ClientRectangle.Right - _vScrollBar.Width)
        {
            ScrollH++;
            paintZoneRight -= _font.Width;
        }
    }

    private void Editor_KeyPress(object sender, KeyPressEventArgs e)
    {
        PressedKey?.Invoke(this, e.KeyChar);
        e.Handled = true;
    }

    private void Editor_Paint(object sender, PaintEventArgs e)
    {
        if (_textLinesByNumber is null) return;

        var textBackgroundBrush = new SolidBrush(Color.White);
        var maxVisibleColumns = (Width / _font.Width).Crop(1, int.MaxValue);

        foreach (var (line, content) in _textLinesByNumber.Skip(ScrollV))
        {
            var visibleColumnsReq = content.Length - ScrollH;
            var visibleColumns = visibleColumnsReq.Crop(0, maxVisibleColumns);

            var paintZoneTop = GetPaintZoneTop(line);
            var paintZoneWidth = visibleColumns * _font.Width + _font.RightMargin;
            var paintZone = new Rectangle(ClientRectangle.Left, paintZoneTop, paintZoneWidth, _font.Height);

            if (paintZone.Top >= ClientRectangle.Bottom) break;
            if (paintZone.Height > ClientRectangle.Height) paintZone.Height = ClientRectangle.Height;
            if (paintZone.Width > ClientRectangle.Width) paintZone.Width = ClientRectangle.Width;

            e.Graphics.FillRectangle(
                textBackgroundBrush,
                paintZone.X + _font.LeftMargin,
                paintZone.Y,
                paintZone.Width - _font.RightMargin,
                paintZone.Height + 1);

            if (visibleColumns <= 0) continue;

            const TextFormatFlags textFormatFlags = TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;
            var lineText = content.ToPrintable(ScrollH, visibleColumns);
            TextRenderer.DrawText(e.Graphics, lineText, Font, paintZone, Color.Black, textFormatFlags);
        }
    }

    private void Editor_SizeChanged(object sender, EventArgs e) => ResetScrollbars();

    private void HScrollBar_Scroll(object? sender, ScrollEventArgs e)
    {
        if (_hScrollBar is null)
        {
            const string message = $"Impossible to scroll horizontally without a {nameof(HScrollBar)} control.";
            throw new InvalidOperationException(message);
        }

        _hScrollBar.Value = e.NewValue;
        Invalidate();
    }

    private void VScrollBar_Scroll(object? sender, ScrollEventArgs e)
    {
        if (_textLinesByNumber is null) return;
        
        _vScrollBar.Value = e.NewValue.Crop(0, _textLinesByNumber.Count - 1);
        Invalidate();
    }
}
