using JinGine.WinForms.Views;
using LegacyFwk;

namespace JinGine.WinForms;

/// <summary>
/// Represents our custom code-editor control.
/// </summary>
public partial class Editor : UserControl, IEditorView
{
    [Flags]
    public enum Flags
    {
        None                = 0x00,
        HorizontalScroll    = 0x01,
        LeftArrowQuits      = 0x02,
        BaseHotKey          = 0x04,
    }

    private readonly FontDescriptor _font;

    private IDictionary<int, string>? _textLinesByNumber;

    // TODO struct VisibleTextMask
    private int _visibleColumnsCount;
    private int _visibleRowsCount;

    public event EventHandler<char>? PressedKey;

    private int ScrollH => _hScrollBar?.Value ?? 0;
    private int ScrollV => _vScrollBar.Value;

    /// <summary>
    /// Creates an instance of the <see cref="Editor"/> class.
    /// </summary>
    /// <remarks>Visual Studio Designer needs a constructor without parameters.</remarks>
    public Editor() : this(Flags.HorizontalScroll) {}

    /// <summary>
    /// Creates an instance of the <see cref="Editor"/> class.
    /// </summary>
    /// <param name="flags">flags for control style & behavior</param>
    public Editor(Flags flags)
    {
        InitializeComponent();

        _font = FontDescriptor.DefaultFixed;
        base.Font = _font.Font;
        SetStyle(ControlStylesHelper.DoubleBufferedInputControl, true);

        if ((flags & Flags.HorizontalScroll) is not Flags.HorizontalScroll)
        {
            _hScrollBar.Scroll -= HScrollBar_Scroll;
            Controls.Remove(_hScrollBar);
            //_hScrollBar1.Dispose(); // TODO check if needs to be disposed
            _hScrollBar = null;
        }
    }

    public void Render(IDictionary<int, string> textLinesByNumber)
    {
        _textLinesByNumber = textLinesByNumber;
        Invalidate();
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

        foreach (var (lineNumber, content) in _textLinesByNumber.Skip(ScrollV))
        {
            var visibleColumnsCountReq = content.Length - ScrollH;
            var visibleColumnsCount = visibleColumnsCountReq.Crop(0, _visibleColumnsCount);

            var paintZoneTop = ClientRectangle.Top + (lineNumber - 1 - ScrollV) * _font.Height;
            var paintZoneWidth = visibleColumnsCount * _font.Width + _font.RightMargin;
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

            if (visibleColumnsCount <= 0) continue;

            var lineText = content.ToPrintable(ScrollH, visibleColumnsCount);
            const TextFormatFlags textFormatFlags = TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;
            TextRenderer.DrawText(e.Graphics, lineText, Font, paintZone, Color.Black, textFormatFlags);
        }
    }

    private void Editor_SizeChanged(object sender, EventArgs e)
    {
        var hScrollBarHeight = _hScrollBar?.Height ?? 0;

        _visibleColumnsCount = Width / _font.Width;
        if (_visibleColumnsCount < 1) _visibleColumnsCount = 1;

        _visibleRowsCount = (Height - hScrollBarHeight) / _font.Height;
        if (_visibleRowsCount < 1) _visibleRowsCount = 1;

        //ResetScrollbars();
    }

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
