using JinGine.WinForms.Views;
using static System.Windows.Forms.TextFormatFlags;

namespace JinGine.WinForms;

public partial class EditorTextViewer : UserControl
{
    private const TextFormatFlags TextFormatFlags = NoPadding | NoPrefix | SingleLine;

    private readonly CharsGrid _grid;    
    private readonly Win32Caret _caret;
    private readonly Selector _selector;
    private string[] _lines;
    private Point? _mouseDownScreenPoint;

    internal Point CaretPoint { get; set; }

    public event EventHandler<char>? KeyPressed;
    public event EventHandler<Point>? CaretPointChanged;

    public EditorTextViewer()
    {
        InitializeComponent();

        _grid = new CharsGrid(LegacyFwk.FontDescriptor.DefaultFixed);
        _caret = new Win32Caret(this) { Size = _grid.CellSize };
        _selector = new Selector();
        _lines = Array.Empty<string>();
        CaretPoint = Point.Empty;
        
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

    private void ApplyScrollBarOffsets(ref Point point)
    {
        point.X += _hScrollBar.Value;
        point.Y += _vScrollBar.Value;
    }

    private void EnsureVisibleCaret()
    {
        var caretPoint = CaretPoint;
        UnapplyScrollBarOffsets(ref caretPoint);
        var screenRect = _grid.CharPointToScreenRect(caretPoint);
        var bounds = _grid.Bounds;
        var height = _grid.CellSize.Height;
        var width = _grid.CellSize.Width;

        var desiredHScroll = _hScrollBar.Value
            - Math.Max(0, (bounds.Left - _grid.XMargin - screenRect.Left + width - 1) / width)
            + Math.Max(0, (screenRect.Right - (bounds.Right - _grid.XMargin) + width - 1) / width);
        var desiredVScroll = _vScrollBar.Value
            - Math.Max(0, (bounds.Top - screenRect.Top + height - 1) / height)
            + Math.Max(0, (screenRect.Bottom - bounds.Bottom + height - 1) / height);

        if (_hScrollBar.Value != desiredHScroll)
            _hScrollBar.RaiseMouseWheel((_hScrollBar.Value - desiredHScroll) * SystemInformation.MouseWheelScrollDelta);

        if (_vScrollBar.Value != desiredVScroll)
            _vScrollBar.RaiseMouseWheel((_vScrollBar.Value - desiredVScroll) * SystemInformation.MouseWheelScrollDelta);
    }

    // TODO there is an issue here, is being called twice and only the second call is valid.
    private void OnHScrollBarScroll(object? sender, ScrollEventArgs e)
    {
        var newCaretPoint = CaretPoint;
        UnapplyScrollBarOffsets(ref newCaretPoint);
        _caret.Position = _grid.CharPointToScreen(newCaretPoint);
        Invalidate();
    }
    
    // TODO there is an issue here, is being called twice and only the second call is valid.
    private void OnVScrollBarScroll(object? sender, ScrollEventArgs e)
    {
        var newCaretPoint = CaretPoint;
        UnapplyScrollBarOffsets(ref newCaretPoint);
        _caret.Position = _grid.CharPointToScreen(newCaretPoint);
        Invalidate();
    }
    
    // TODO still need to handle Home, End, Delete, PageUp, PageDown, Ctrl + A, Ctrl + C, Ctrl + X, Ctrl + V, Ctrl + Z, Ctrl + Y, etc.
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        // TODO notify arrow key via event to presenter
        var newCaretPoint = CaretPoint;
        switch (e.KeyCode)
        {
            case Keys.Left:
                if (newCaretPoint.X > 0) newCaretPoint.X--;
                else if (newCaretPoint.Y > 0)
                {
                    newCaretPoint.Y--;
                    newCaretPoint.X = _lines[newCaretPoint.Y].Length;
                }
                break;
            case Keys.Right:
                if (newCaretPoint.X < _lines[newCaretPoint.Y].Length) newCaretPoint.X++;
                else if (newCaretPoint.Y + 1 < _lines.Length)
                {
                    newCaretPoint.Y++;
                    newCaretPoint.X = 0;
                }
                break;
            case Keys.Up:
                if (newCaretPoint.Y > 0)
                {
                    newCaretPoint.Y--;
                    newCaretPoint.X = Math.Min(newCaretPoint.X, _lines[newCaretPoint.Y].Length);
                }
                break;
            case Keys.Down:
                if (newCaretPoint.Y + 1 < _lines.Length)
                {
                    newCaretPoint.Y++;
                    newCaretPoint.X = Math.Min(newCaretPoint.X, _lines[newCaretPoint.Y].Length);
                }
                break;
        }

        if (newCaretPoint == CaretPoint) return;

        if ((ModifierKeys & Keys.Shift) is Keys.Shift)
        {
            switch (_selector.State)
            {
                case SelectionState.Unselected:
                    _selector.StartSelect(newCaretPoint);
                    break;
                case SelectionState.Selecting:
                    _selector.Select(newCaretPoint);
                    break;
                case SelectionState.Selected:
                    _selector.Select(newCaretPoint, true);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        CaretPoint = newCaretPoint;
        CaretPointChanged?.Invoke(this, newCaretPoint);
        SetWin32CaretPos(newCaretPoint);
    }

    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        if ((ModifierKeys & Keys.Shift) is not Keys.Shift && _selector.State is SelectionState.Selecting)
        {
            _selector.EndSelect();
            Invalidate();
        }
    }

    private void OnKeyPress(object? sender, KeyPressEventArgs e)
    {
        KeyPressed?.Invoke(this, e.KeyChar);
        e.Handled = true;
    }

    private void OnMouseClick(object? sender, MouseEventArgs e)
    {
        var caretPoint = _grid.ScreenToCharPoint(e.Location);
        ApplyScrollBarOffsets(ref caretPoint);
        CaretPoint = caretPoint;
        CaretPointChanged?.Invoke(this, caretPoint);
        SetWin32CaretPos(caretPoint);
    }

    private void OnMouseDown(object sender, MouseEventArgs e)
    {
        _mouseDownScreenPoint = e.Location;
        _selector.Clear();
        Invalidate();
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_mouseDownScreenPoint is null) return;

        switch (_selector.State)
        {
            case SelectionState.Unselected:
            {
                var startPoint = _grid.ScreenToCharPoint(_mouseDownScreenPoint.Value);
                ApplyScrollBarOffsets(ref startPoint);
                _selector.StartSelect(startPoint);
                Invalidate();
                break;
            }
            case SelectionState.Selecting:
                var endPoint = _grid.ScreenToCharPoint(e.Location);
                ApplyScrollBarOffsets(ref endPoint);
                _selector.Select(endPoint);
                Invalidate();
                break;
            case SelectionState.Selected:
            default:
                break;
        }
    }

    private void OnMouseUp(object sender, MouseEventArgs e)
    {
        _mouseDownScreenPoint = null;
        if (_selector.State is not SelectionState.Selecting) return;
        var endPoint = _grid.ScreenToCharPoint(e.Location);
        ApplyScrollBarOffsets(ref endPoint);
        _selector.EndSelect();
        Invalidate();
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        var gridBounds = ClientRectangle;
        gridBounds.Width -= _vScrollBar.Width;
        gridBounds.Height -= _hScrollBar.Height;
        _grid.SetBounds(gridBounds);
        EnsureVisibleCaret();
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
            var line = _lines[i];
            var visibleCols = Math.Min(line.Length - _hScrollBar.Value, _grid.MaxVisibleColumns);
            if (visibleCols <= 0) continue;

            var screenY = i - _vScrollBar.Value;
            var firstCharRect = _grid.CharPointToScreenRect(new Point(0, screenY));
            var lastCharRect = _grid.CharPointToScreenRect(new Point(visibleCols - 1, screenY));
            var textRect = Rectangle.Union(firstCharRect, lastCharRect);
            var bgRect = textRect with { X = 0 };

            bgRect.Inflate(textRect.Left, 0);
            e.Graphics.FillRectangle(textBackgroundBrush, bgRect);

            var text = line.AsSpan(_hScrollBar.Value, visibleCols);
            TextRenderer.DrawText(e.Graphics, text, Font, textRect, Color.Black, TextFormatFlags);
        }

        // TODO integrate this in the loop above (make a grid selection per line ?)
        if (_selector.State is SelectionState.Selecting or SelectionState.Selected)
        {
            var startPoint = _selector.Start;
            var endPoint = _selector.End;
            UnapplyScrollBarOffsets(ref startPoint);
            UnapplyScrollBarOffsets(ref endPoint);
            var selectionRect = Rectangle.Union(
                _grid.CharPointToScreenRect(startPoint),
                _grid.CharPointToScreenRect(endPoint));

            e.Graphics.DrawRectangle(
                _selector.State is SelectionState.Selecting
                    ? new Pen(new SolidBrush(Color.BlueViolet))
                    : new Pen(new SolidBrush(Color.YellowGreen)),
                selectionRect);
        }
    }

    private void SetWin32CaretPos(Point charPoint)
    {
        UnapplyScrollBarOffsets(ref charPoint);
        _caret.Position = _grid.CharPointToScreen(charPoint);
        Invalidate();
    }

    #region Private Types

    private class CharsGrid
    {
        internal Size CellSize { get; }
        internal int XMargin { get; }
        internal Rectangle Bounds { get; private set; }
        internal int MaxVisibleColumns { get; private set; }

        internal CharsGrid(LegacyFwk.FontDescriptor fDescriptor)
        {
            XMargin = fDescriptor.LeftMargin;
            CellSize = new Size(fDescriptor.Width, fDescriptor.Height);
        }

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
        }
    }

    private enum SelectionState
    {
        Unselected,
        Selecting,
        Selected,
    }

    private class Selector
    {
        internal Point Start { get; private set; }
        internal Point End { get; private set; }
        internal SelectionState State { get; private set; } = SelectionState.Unselected;

        internal void Clear()
        {
            Start = End = Point.Empty;
            State = SelectionState.Unselected;
        }

        internal void EndSelect()
        {
            State = SelectionState.Selected;
        }

        internal void Select(Point endPoint, bool restart = false)
        {
            End = endPoint;
            if (restart) State = SelectionState.Selecting;
        }

        internal void StartSelect(Point startPoint)
        {
            Start = End = startPoint;
            State = SelectionState.Selecting;
        }
    }

    #endregion
}
