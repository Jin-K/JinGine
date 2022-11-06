using System.Diagnostics;
using JinGine.WinForms.Views;

namespace JinGine.WinForms;

class CharsGrid
{
    internal Rectangle Bounds { get; private set;}
    internal Size CellSize { get; }
    internal int MaxVisibleColumns { get; private set; }
    internal int MaxVisibleLines { get; private set; }
    internal int XMargin { get; }

    private CharsGrid(Size size, int xMargin)
    {
        XMargin = xMargin;
        CellSize = size;
    }

    internal static CharsGrid Create(LegacyFwk.FontDescriptor fDescriptor) =>
        new(new Size(fDescriptor.Width, fDescriptor.Height), fDescriptor.LeftMargin);

    internal Point ScreenToCharPoint(Point screenPoint)
    {
        var x = (screenPoint.X - XMargin) / CellSize.Width;
        var y = screenPoint.Y / CellSize.Height;
        return new Point(x, y);
    }

    internal Point CharPointToScreen(Point charPoint)
    {
        var x = charPoint.X * CellSize.Width + XMargin;
        var y = charPoint.Y * CellSize.Height;
        return new Point(x, y);
    }

    internal Rectangle CharPointToScreenRect(Point charPoint)
    {
        var screenPoint = CharPointToScreen(charPoint);
        return new Rectangle(screenPoint, CellSize);
    }

    internal void SetBounds(Rectangle bounds)
    {
        Bounds = bounds;
        MaxVisibleColumns = (bounds.Width + CellSize.Width - 1 - XMargin) / CellSize.Width;
        MaxVisibleLines = (bounds.Height + CellSize.Height - 1) / CellSize.Height;
    }
}

public partial class EditorTextViewer : UserControl
{
    private const TextFormatFlags TextFFlags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;

    private readonly CharsGrid _grid;    
    private readonly Win32Caret _caret;
    private readonly GridSelector _selector;
    private string[] _lines;

    internal Point CaretLocation { get; set; }

    public event EventHandler<char>? KeyPressed;
    public event EventHandler<Point>? CaretLocationChanged;

    public EditorTextViewer()
    {
        InitializeComponent();

        _grid = CharsGrid.Create(LegacyFwk.FontDescriptor.DefaultFixed);
        _caret = new Win32Caret(this);
        _selector = new GridSelector(this, _grid);
        _lines = Array.Empty<string>();
        CaretLocation = Point.Empty;
        
        base.DoubleBuffered = true;
        base.Font = LegacyFwk.FontDescriptor.DefaultFixed.Font;
        
        this.InitArrowKeyDownFiring();
        this.InitMouseWheelScrollDelegation(_vScrollBar);
    }

    internal void SetLines(string[] lines)
    {
        if (_lines.SequenceEqual(lines)) return;
        _lines = lines;
        Invalidate();
    }

    internal void SetProjector(TextProjector projector)
    {
        _selector = new GridSelector(this, projector);
        _caret.Size = projector.CellSize;
    }

    private void OnMouseClick(object? sender, MouseEventArgs e)
    {
        var caretLocation = _grid.ScreenToCharPoint(e.Location);
        ApplyScrollBarOffsets(ref caretLocation);
        CaretLocation = caretLocation;
        CaretLocationChanged?.Invoke(this, caretLocation);
        SetWin32CaretPos(caretLocation);
    }
    
    // TODO there is an issue here, is being called twice and only the second call is valid.
    private void OnHScrollBarScroll(object? sender, ScrollEventArgs e)
    {
        var newCaretPoint = CaretLocation;
        UnapplyScrollBarOffsets(ref newCaretPoint);
        _caret.Position = _grid.CharPointToScreen(newCaretPoint);
        Debug.WriteLine(_caret.Position, nameof(OnHScrollBarScroll));
        Invalidate();
    }
    
    // TODO there is an issue here, is being called twice and only the second call is valid.
    private void OnVScrollBarScroll(object? sender, ScrollEventArgs e)
    {
        var newCaretPoint = CaretLocation;
        UnapplyScrollBarOffsets(ref newCaretPoint);
        _caret.Position = _grid.CharPointToScreen(newCaretPoint);
        Debug.WriteLine(_caret.Position, nameof(OnHScrollBarScroll));
        Invalidate();
    }
    
    // TODO still need to handle Home, End, Delete, PageUp, PageDown, Ctrl + A, Ctrl + C, Ctrl + X, Ctrl + V, Ctrl + Z, Ctrl + Y, etc.
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (_selector is null) return;

        // TODO all this logic could go to the presenter ??
        var nCGridLoc = CaretLocation;
        switch (e.KeyCode)
        {
            case Keys.Left:
                if (nCGridLoc.X > 0) nCGridLoc.X--;
                else if (nCGridLoc.Y > 0)
                {
                    nCGridLoc.Y--;
                    nCGridLoc.X = _lines[nCGridLoc.Y].Length;
                }
                break;
            case Keys.Right:
                if (nCGridLoc.X < _lines[nCGridLoc.Y].Length) nCGridLoc.X++;
                else if (nCGridLoc.Y + 1 < _lines.Length)
                {
                    nCGridLoc.Y++;
                    nCGridLoc.X = 0;
                }
                break;
            case Keys.Up:
                if (nCGridLoc.Y > 0)
                {
                    nCGridLoc.Y--;
                    nCGridLoc.X = Math.Min(nCGridLoc.X, _lines[nCGridLoc.Y].Length);
                }
                break;
            case Keys.Down:
                if (nCGridLoc.Y + 1 < _lines.Length)
                {
                    nCGridLoc.Y++;
                    nCGridLoc.X = Math.Min(nCGridLoc.X, _lines[nCGridLoc.Y].Length);
                }
                break;
        }

        if (nCGridLoc == CaretLocation) return;

        if ((ModifierKeys & Keys.Shift) is Keys.Shift)
        {
            // TODO fix
            var startRect = _grid.CharPointToScreenRect(CaretLocation);
            var endRect = _grid.CharPointToScreenRect(nCGridLoc);
            startRect.X -= _hScrollBar.Value;
            startRect.Y -= _vScrollBar.Value;
            endRect.X -= _hScrollBar.Value;
            endRect.Y -= _vScrollBar.Value;

            Debug.WriteLine(nameof(OnKeyDown));
            Debug.Indent();
            Debug.WriteLine(CaretLocation, nameof(CaretLocation));
            Debug.WriteLine(nCGridLoc, nameof(nCGridLoc));
            Debug.WriteLine(startRect, nameof(startRect));
            Debug.WriteLine(endRect, nameof(endRect));
            Debug.Unindent();

            _selector.StartSelect(CaretLocation);
            _selector.EndSelect(nCGridLoc);
        }

        CaretLocation = nCGridLoc;
        CaretLocationChanged?.Invoke(this, nCGridLoc);
        SetWin32CaretPos(nCGridLoc);
    }
    
    private void OnKeyPress(object? sender, KeyPressEventArgs e)
    {
        KeyPressed?.Invoke(this, e.KeyChar);
        e.Handled = true;
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        var gridBounds = ClientRectangle;
        gridBounds.Width -= _vScrollBar.Width;
        gridBounds.Height -= _hScrollBar.Height;
        _grid.SetBounds(gridBounds);
        EnsureProjection(CaretLocation);
    }

    private void EnsureProjection(Point charPoint)
    {
        UnapplyScrollBarOffsets(ref charPoint);
        var screenRect = _grid.CharPointToScreenRect(charPoint);
        var x = _hScrollBar.Value;
        var y = _vScrollBar.Value;
        var bounds = _grid.Bounds;
        var height = _grid.CellSize.Height;
        var width = _grid.CellSize.Width;
        
        y -= Math.Max(0, (bounds.Top - screenRect.Top + height - 1) / height);
        y += Math.Max(0, (screenRect.Bottom - bounds.Bottom + height - 1) / height);
        x -= Math.Max(0, (bounds.Left - _grid.XMargin - screenRect.Left + width - 1) / width);
        x += Math.Max(0, (screenRect.Right - (bounds.Right - _grid.XMargin) + width - 1) / width);
        
        if (_hScrollBar.Value != x)
            _hScrollBar.RaiseMouseWheel((_hScrollBar.Value - x) * SystemInformation.MouseWheelScrollDelta);

        if (_vScrollBar.Value != y)
            _vScrollBar.RaiseMouseWheel((_vScrollBar.Value - y) * SystemInformation.MouseWheelScrollDelta);
    }

    private void ApplyScrollBarOffsets(ref Point point) {
        point.X += _hScrollBar.Value;
        point.Y += _vScrollBar.Value;
    }
    
    private void UnapplyScrollBarOffsets(ref Point point)
    {
        point.X -= _hScrollBar.Value;
        point.Y -= _vScrollBar.Value;
    }
    
    private void OnPaint(object? sender, PaintEventArgs e)
    {
        var textBackgroundBrush = new SolidBrush(Color.White);
        for (var i = _vScrollBar.Value; i < _lines.Length; i++)
        {
            var visibleCols = Math.Min(_lines[i].Length - _hScrollBar.Value, _grid.MaxVisibleColumns);
            if (visibleCols <= 0) continue;

            var firstCharRect = _grid.CharPointToScreenRect(new Point(0, i - _vScrollBar.Value));
            var lastCharRect = _grid.CharPointToScreenRect(new Point(visibleCols - 1, i - _vScrollBar.Value));
            var textRect = Rectangle.Union(firstCharRect, lastCharRect);
            var bgRect = textRect with { X = 0 };

            bgRect.Inflate(textRect.Left, 0);
            e.Graphics.FillRectangle(textBackgroundBrush, bgRect);

            var text = _lines[i].AsSpan(_hScrollBar.Value, visibleCols);
            TextRenderer.DrawText(e.Graphics, text, Font, textRect, Color.Black, TextFFlags);
        }

        if (_selector is null) return;

        // TODO integrate this in the loop above (make a grid selection per line ?)
        switch (_selector.State)
        {
            case GridSelector.Status.Selecting:
                {
                    e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.BlueViolet)), _selector.Selection);
                    break;
                }
            case GridSelector.Status.Selected:
                {
                    e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.YellowGreen)), _selector.Selection);
                    break;
                }
            case GridSelector.Status.Unselected:
            default:
                break;
        }
    }

    private void SetWin32CaretPos(Point location)
    {
        UnapplyScrollBarOffsets(ref location);
        _caret.Position = _grid.CharPointToScreen(location);
        Invalidate();
    }
}
