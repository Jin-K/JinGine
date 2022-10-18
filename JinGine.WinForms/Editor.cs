﻿using JinGine.WinForms.Views;
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

    private IReadOnlyDictionary<int, string>? _textLinesByNumber;

    // TODO struct VisibleTextMask, and make it part of the model
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
    /// Scrolls the view to specific cartesian coordinates in the text it displays.
    /// </summary>
    /// <param name="line">The line number.</param>
    /// <param name="column">The column number in that line.</param>
    public void ScrollTo(int line, int column)
    {
        //// check if line is inside future VisibleTextMask, if no =>
            //// compute and set ScrollV value

        //// check if column is inside future VisibleTextMask, if no =>
            //// compute and set ScrollH value
    }

    private void ResetScrollbars()
    {
        //throw new NotImplementedException("TODO");
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

            const TextFormatFlags textFormatFlags = TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;
            var lineText = content.ToPrintable(ScrollH, visibleColumnsCount);
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

        ResetScrollbars();
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
