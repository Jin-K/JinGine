using System.ComponentModel;
using JinGine.WinForms.Views.Models;
using static System.Windows.Forms.TextFormatFlags;

namespace JinGine.WinForms.Controls;

public partial class EditorTextViewer : UserControl
{
    private const TextFormatFlags TextFormatFlags = NoPadding | NoPrefix | SingleLine;
    private static readonly Brush SelectedRectBrush = new SolidBrush(Color.CadetBlue);
    private static readonly Brush SelectingRectBrush = new SolidBrush(Color.LightBlue);

    private readonly CharsGrid _grid;
    private readonly Helpers.Win32Caret _caret;
    private readonly Selector _selector;
    private string[] _lines;
    private Point? _mouseDownScreenPoint;
    private PaintZone _paintZone;

    internal Point CaretPoint { get; set; }

    [Bindable(true)]
    internal TextSelectionRange TextSelection { get; set; }

    public event EventHandler<char>? KeyPressed;
    public event EventHandler<Point>? CaretPointChanged;

    public EditorTextViewer()
    {
        InitializeComponent();

        var fontDescriptor = FontDescriptor.DefaultFixed;
        var cellSize = new Size(fontDescriptor.Width, fontDescriptor.Height);
        
        _grid = new CharsGrid(cellSize.Width, cellSize.Height, fontDescriptor.LeftMargin);
        _caret = new Helpers.Win32Caret(this) { Size = cellSize };
        _selector = new Selector();
        _lines = Array.Empty<string>();
        CaretPoint = Point.Empty;
        TextSelection = TextSelectionRange.Empty;
        
        base.DoubleBuffered = true;
        base.Font = fontDescriptor.Font;
        
        this.InitArrowKeyDownFiring();
        this.InitMouseWheelScrollDelegation(_vScrollBar);
    }

    internal void SetLines(string[] lines)
    {
        if (_lines.SequenceEqual(lines)) return;
        _lines = lines;
        Invalidate();
    }

    private Point ClientToCoords(Point point)
    {
        point.X -= _grid.XMargin;
        point.X /= _grid.CellWidth;
        point.Y /= _grid.CellHeight;
        point.X += _hScrollBar.Value;
        point.Y += _vScrollBar.Value;
        return point;
    }

    private Point CoordsToClient(int x, int y)
    {
        x -= _hScrollBar.Value;
        y -= _vScrollBar.Value;
        x *= _grid.CellWidth;
        x += _grid.XMargin;
        y *= _grid.CellHeight;
        return new Point(x, y);
    }

    private Rectangle CoordsToClientRect(int x, int y)
    {
        var clientPoint = CoordsToClient(x, y);
        return new Rectangle(clientPoint.X, clientPoint.Y, _grid.CellWidth, _grid.CellHeight);
    }

    private void EnsureVisibleCaret()
    {
        var clientRect = CoordsToClientRect(CaretPoint.X, CaretPoint.Y);

        var desiredHScroll = _hScrollBar.Value
            - Math.Max(0, (_paintZone.Bounds.Left - _grid.XMargin - clientRect.Left + _grid.CellWidth - 1) / _grid.CellWidth)
            + Math.Max(0, (clientRect.Right - (_paintZone.Bounds.Right - _grid.XMargin) + _grid.CellWidth - 1) / _grid.CellWidth);
        var desiredVScroll = _vScrollBar.Value
            - Math.Max(0, (_paintZone.Bounds.Top - clientRect.Top + _grid.CellHeight - 1) / _grid.CellHeight)
            + Math.Max(0, (clientRect.Bottom - _paintZone.Bounds.Bottom + _grid.CellHeight - 1) / _grid.CellHeight);

        if (_hScrollBar.Value != desiredHScroll)
            _hScrollBar.InvokeMouseWheel((_hScrollBar.Value - desiredHScroll) * SystemInformation.MouseWheelScrollDelta);

        if (_vScrollBar.Value != desiredVScroll)
            _vScrollBar.InvokeMouseWheel((_vScrollBar.Value - desiredVScroll) * SystemInformation.MouseWheelScrollDelta);
    }

    private void OnCaretPointChanged(Point caretPoint)
    {
        CaretPoint = caretPoint;
        CaretPointChanged?.Invoke(this, caretPoint);
        _caret.Position = CoordsToClient(caretPoint.X, caretPoint.Y);
        Invalidate();
    }
    
    private void OnHScrollBarScroll(object? sender, ScrollEventArgs e)
    {
        if (e.Type is ScrollEventType.EndScroll || e.OldValue.Equals(e.NewValue)) return;
        _caret.Position = CoordsToClient(CaretPoint.X - e.NewValue + _hScrollBar.Value, CaretPoint.Y);
        Invalidate();
    }
    
    private void OnVScrollBarScroll(object? sender, ScrollEventArgs e)
    {
        if (e.Type is ScrollEventType.EndScroll || e.OldValue.Equals(e.NewValue)) return;
        _caret.Position = CoordsToClient(CaretPoint.X, CaretPoint.Y - e.NewValue + _vScrollBar.Value);
        Invalidate();
    }
    
    // TODO still need to handle Home, End, Delete, PageUp, PageDown, Ctrl + A, Ctrl + C, Ctrl + X, Ctrl + V, Ctrl + Z, Ctrl + Y, etc.
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode is Keys.ShiftKey && _selector.State is SelectionState.Selected)
        {
            _selector.Select(CaretPoint);
            Invalidate();
        }

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
        var caretPoint = ClientToCoords(e.Location);
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
                var startPoint = ClientToCoords(_mouseDownScreenPoint.Value);
                _selector.StartSelect(startPoint);
                Invalidate();
                break;
            }
            case SelectionState.Selecting:
                var endPoint = ClientToCoords(e.Location);
                _selector.Select(endPoint);
                Invalidate();
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
        var paintRect = ClientRectangle;
        paintRect.InflateEnd(-_vScrollBar.Width, -_hScrollBar.Height);
        _paintZone = PaintZone.Create(paintRect, _grid.CellWidth, _grid.XMargin);
        EnsureVisibleCaret();
    }

    private void OnPaint(object? sender, PaintEventArgs e)
    {
        for (var y = _vScrollBar.Value; y < _lines.Length; y++)
        {
            var line = _lines[y];
            var visibleCols = Math.Min(_paintZone.VisibleColumnsCount, Math.Max(0, line.Length - _hScrollBar.Value));
            
            var startCharRect = CoordsToClientRect(_hScrollBar.Value, y);
            var endCharRect = CoordsToClientRect(_hScrollBar.Value + visibleCols - 1, y);
            var textRect = Rectangle.Union(startCharRect, endCharRect);

            if (_selector.IsLineSelected(y))
            {
                var selectionStartX = y == _selector.Start.Y ? _selector.Start.X : 0;
                var selectionEndX = y == _selector.End.Y
                    ? _selector.End.X
                    : y == _lines.Length - 1
                        ? line.Length - 1
                        : line.Length;
                var selectionRect = Rectangle.Union(
                    CoordsToClientRect(selectionStartX, y),
                    CoordsToClientRect(selectionEndX, y));
                
                if (selectionRect.Right > 0)
                {
                    if (selectionRect.X < 0) selectionRect.InflateStart(selectionRect.X, 0);
                    var brush = _selector.State is SelectionState.Selecting ? SelectingRectBrush : SelectedRectBrush;
                    e.Graphics.FillRectangle(brush, selectionRect);
                }
            }

            if (visibleCols is 0) continue;
            var text = line.AsSpan(_hScrollBar.Value, visibleCols);
            TextRenderer.DrawText(e.Graphics, text, Font, textRect, Color.Black, TextFormatFlags);
        }
    }

    #region Private Types

    private readonly record struct CharsGrid(int CellWidth, int CellHeight, int XMargin);

    private readonly record struct PaintZone(Rectangle Bounds, int VisibleColumnsCount)
    {
        internal static PaintZone Create(Rectangle bounds, int cellWidth, int xMargin)
            => new(bounds, (bounds.Width + cellWidth - 1 - xMargin) / cellWidth);
    }

    private enum SelectionState
    {
        Unselected,
        Selecting,
        Selected,
    }

    private class Selector
    {
        private Point _origin;

        internal Point Start { get; private set; }
        internal Point End { get; private set; }
        internal SelectionState State { get; private set; } = SelectionState.Unselected;

        internal void Clear()
        {
            _origin = Start = End = Point.Empty;
            State = SelectionState.Unselected;
        }

        internal void EndSelect()
        {
            State = SelectionState.Selected;
        }

        internal bool IsLineSelected(int index)
        {
            return State is not SelectionState.Unselected && Start.Y <= index && index <= End.Y;
        }

        internal void Select(Point point)
        {
            if (point.Y < _origin.Y || (point.Y == _origin.Y && point.X < _origin.X))
            {
                Start = point;
                End = _origin;
            }
            else
            {
                Start = _origin;
                End = point;
            }

            State = SelectionState.Selecting;
        }

        internal void StartSelect(Point startPoint)
        {
            _origin = Start = End = startPoint;
            State = SelectionState.Selecting;
        }
    }

    #endregion
}
