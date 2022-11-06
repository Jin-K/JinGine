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

    private void EnsureVisibleCaret()
    {
        var caretPoint = Point.Subtract(CaretPoint, ScrollValuesToSize());
        var screenRect = _grid.CharPointToScreenRect(caretPoint);
        var gridBounds = _grid.Bounds;
        var cellHeight = _grid.CellSize.Height;
        var cellWidth = _grid.CellSize.Width;

        var desiredHScroll = _hScrollBar.Value
            - Math.Max(0, (gridBounds.Left - _grid.XMargin - screenRect.Left + cellWidth - 1) / cellWidth)
            + Math.Max(0, (screenRect.Right - (gridBounds.Right - _grid.XMargin) + cellWidth - 1) / cellWidth);
        var desiredVScroll = _vScrollBar.Value
            - Math.Max(0, (gridBounds.Top - screenRect.Top + cellHeight - 1) / cellHeight)
            + Math.Max(0, (screenRect.Bottom - gridBounds.Bottom + cellHeight - 1) / cellHeight);

        if (_hScrollBar.Value != desiredHScroll)
            _hScrollBar.InvokeMouseWheel((_hScrollBar.Value - desiredHScroll) * SystemInformation.MouseWheelScrollDelta);

        if (_vScrollBar.Value != desiredVScroll)
            _vScrollBar.InvokeMouseWheel((_vScrollBar.Value - desiredVScroll) * SystemInformation.MouseWheelScrollDelta);
    }

    private void OnCaretPointChanged(Point caretPoint)
    {
        CaretPoint = caretPoint;
        CaretPointChanged?.Invoke(this, caretPoint);
        _caret.Position = _grid.CharPointToScreen(Point.Subtract(caretPoint, ScrollValuesToSize()));
        Invalidate();
    }
    
    private void OnHScrollBarScroll(object? sender, ScrollEventArgs e)
    {
        if (e.Type is ScrollEventType.EndScroll || e.OldValue.Equals(e.NewValue)) return;
        var newCaretPoint = CaretPoint;
        newCaretPoint.X -= e.NewValue;
        newCaretPoint.Y -= _vScrollBar.Value;
        _caret.Position = _grid.CharPointToScreen(newCaretPoint);
        Invalidate();
    }
    
    private void OnVScrollBarScroll(object? sender, ScrollEventArgs e)
    {
        if (e.Type is ScrollEventType.EndScroll || e.OldValue.Equals(e.NewValue)) return;
        var newCaretPoint = CaretPoint;
        newCaretPoint.X -= _hScrollBar.Value;
        newCaretPoint.Y -= e.NewValue;
        _caret.Position = _grid.CharPointToScreen(newCaretPoint);
        Invalidate();
    }
    
    // TODO still need to handle Home, End, Delete, PageUp, PageDown, Ctrl + A, Ctrl + C, Ctrl + X, Ctrl + V, Ctrl + Z, Ctrl + Y, etc.
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        // TODO notify direction intent via event to presenter ?
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
                    _selector.Select(newCaretPoint);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        else _selector.Clear();
        
        OnCaretPointChanged(newCaretPoint);
    }

    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        if (e.KeyCode is Keys.ShiftKey && _selector.State is SelectionState.Selecting)
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
        var caretPoint = Point.Add(_grid.ScreenToCharPoint(e.Location), ScrollValuesToSize());
        if (caretPoint.Y >= _lines.Length) return;
        var line = _lines[caretPoint.Y];
        if (caretPoint.X > line.Length) return;
        
        OnCaretPointChanged(caretPoint);
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
                _selector.StartSelect(Point.Add(startPoint, ScrollValuesToSize()));
                Invalidate();
                break;
            }
            case SelectionState.Selecting:
                var endPoint = _grid.ScreenToCharPoint(e.Location);
                _selector.Select(Point.Add(endPoint, ScrollValuesToSize()));
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
        _selector.EndSelect();
        Invalidate();
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        var gridBounds = ClientRectangle.InflateEnd(-_vScrollBar.Width, -_hScrollBar.Height);
        _grid.SetBounds(gridBounds);
        EnsureVisibleCaret();
    }

    private void OnPaint(object? sender, PaintEventArgs e)
    {
        var textBackBrush = new SolidBrush(Color.White);
        var selectedTextBackBrush = new SolidBrush(Color.LightYellow);

        ISet<int> selectedLineIndexes;
        if (_selector.State is SelectionState.Selected or SelectionState.Selecting)
        {
            var start = Math.Min(_selector.Start.Y, _selector.End.Y);
            var length = Math.Max(_selector.Start.Y, _selector.End.Y) - start + 1;
            selectedLineIndexes = Enumerable.Range(start, length).ToHashSet();
        }
        else selectedLineIndexes = new HashSet<int>();
        
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

            bgRect.Inflate(_grid.XMargin, 0);
            if (selectedLineIndexes.Contains(i))
            {
                e.Graphics.FillRectangle(selectedTextBackBrush, bgRect);
            }
            else
            {
                e.Graphics.FillRectangle(textBackBrush, bgRect);
            }

            var text = line.AsSpan(_hScrollBar.Value, visibleCols);
            TextRenderer.DrawText(e.Graphics, text, Font, textRect, Color.Black, TextFormatFlags);
        }

        // TODO integrate this in the loop above (make a grid selection per line ?)
        if (_selector.State is SelectionState.Selecting or SelectionState.Selected)
        {
            var startPoint = Point.Subtract(_selector.Start, ScrollValuesToSize());
            var endPoint = Point.Subtract(_selector.End, ScrollValuesToSize());
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

    private Size ScrollValuesToSize() => new(_hScrollBar.Value, _vScrollBar.Value);

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

        internal void Select(Point endPoint)
        {
            End = endPoint;
            State = SelectionState.Selecting;
        }

        internal void StartSelect(Point startPoint)
        {
            Start = End = startPoint;
            State = SelectionState.Selecting;
        }
    }

    #endregion
}
