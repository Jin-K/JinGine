using System.ComponentModel;
using JinGine.WinForms.ViewModels;
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
    private readonly List<IDisposable> _vmSubs;
    private EditorFileViewModel _viewModel;
    private bool _mouseIsDown;
    private PaintZone _paintZone;
    private Point _caretPoint;

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
        _caret = new Helpers.Win32Caret(this, cellSize with { Width = 2 });
        _selector = new Selector();
        _vmSubs = new List<IDisposable>();
        _viewModel = EditorFileViewModel.Default;
        _mouseIsDown = false;
        _caretPoint = Point.Empty;

        TextSelection = TextSelectionRange.Empty;
        
        base.DoubleBuffered = true;
        base.Font = fontDescriptor.Font;
        Disposed += ClearViewModelSubscriptions;
        
        this.InitArrowKeyDownFiring();
        this.InitMouseWheelScrollDelegation(_vScrollBar);
    }

    internal void SetViewModel(EditorFileViewModel viewModel)
    {
        if (viewModel.TextLines.SequenceEqual(_viewModel.TextLines)) return;
        
        _viewModel = viewModel;

        ClearViewModelSubscriptions();
        
        _vmSubs.Add(viewModel
            .ObserveChanges(vm => vm.ColumnNumber)
            .Subscribe(_ =>
                _caretPoint = new Point(_viewModel.ColumnNumber - 1, _viewModel.LineNumber - 1)));
        _vmSubs.Add(viewModel
            .ObserveChanges(vm => vm.LineNumber)
            .Subscribe(_ =>
                _caretPoint = new Point(_viewModel.ColumnNumber - 1, _viewModel.LineNumber - 1)));

        Invalidate();
    }

    private void ClearViewModelSubscriptions(object? sender = null, EventArgs? args = null)
    {
        _vmSubs.ForEach(sub => sub.Dispose());
        _vmSubs.Clear();
    }

    private Point ClientToClosestCoords(Point clientPoint)
    {
        var coords = ClientToCoords(clientPoint);

        var y = coords.Y.Crop(0, _viewModel.TextLines.Count - 1);

        return new Point
        {
            X = coords.X.Crop(0, _viewModel.TextLines[y].Count),
            Y = y,
        };
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
        var clientRect = CoordsToClientRect(_caretPoint.X, _caretPoint.Y);

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

    private Point GetLastCaretPoint()
    {
        var lastLineY = _viewModel.TextLines.Count - 1;
        var lastLineLastX = _viewModel.TextLines[lastLineY].Count;
        return new Point(lastLineLastX, lastLineY);
    }

    private void OnCaretPointChanged(Point caretPoint)
    {
        if (caretPoint == _caretPoint) return;

        _caretPoint = caretPoint;
        CaretPointChanged?.Invoke(this, caretPoint);
        _caret.Position = CoordsToClient(caretPoint.X, caretPoint.Y);
        EnsureVisibleCaret();
        Invalidate();
    }

    private void OnHScrollBarScroll(object? sender, ScrollEventArgs e)
    {
        if (e.Type is ScrollEventType.EndScroll || e.OldValue.Equals(e.NewValue)) return;
        _caret.Position = CoordsToClient(_caretPoint.X - e.NewValue + _hScrollBar.Value, _caretPoint.Y);
        Invalidate();
    }
    
    private void OnVScrollBarScroll(object? sender, ScrollEventArgs e)
    {
        if (e.Type is ScrollEventType.EndScroll || e.OldValue.Equals(e.NewValue)) return;
        _caret.Position = CoordsToClient(_caretPoint.X, _caretPoint.Y - e.NewValue + _vScrollBar.Value);
        Invalidate();
    }
    
    // TODO still need to handle Delete, PageUp, PageDown, Ctrl + C, Ctrl + X, Ctrl + V, Ctrl + Z, Ctrl + Y, etc.
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode is Keys.ShiftKey && _selector.State is SelectionState.Selected)
        {
            _selector.Reselect();
            Invalidate();
            return;
        }

        var ctrlModifier = (e.Modifiers & Keys.Control) is Keys.Control;
        var shiftModifier = (e.Modifiers & Keys.Shift) is Keys.Shift;

        switch (e.KeyCode)
        {
            case Keys.A when ctrlModifier:
                SelectAll();
                e.SuppressKeyPress = true;
                return;
            case Keys.Home:
                OnCaretPointChanged(Point.Empty);
                if (shiftModifier && _selector.State is SelectionState.Selecting)
                {
                    _selector.Select(_caretPoint);
                    Invalidate();
                }
                return;
            case Keys.End:
                var lastCaretPoint = GetLastCaretPoint();
                OnCaretPointChanged(lastCaretPoint);
                if (shiftModifier && _selector.State is SelectionState.Selecting)
                {
                    _selector.Select(_caretPoint);
                    Invalidate();
                }
                return;
        }

        // TODO notify direction intent via event to presenter ?
        var newCaretPoint = _caretPoint;

        switch (e.KeyCode)
        {
            case Keys.Left:
                if (newCaretPoint.X > 0)
                {
                    newCaretPoint.X--;
                }
                else if (newCaretPoint.Y > 0)
                {
                    newCaretPoint.Y--;
                    newCaretPoint.X = _viewModel.TextLines[newCaretPoint.Y].Count;
                }
                break;
            case Keys.Right:
                if (newCaretPoint.X < _viewModel.TextLines[newCaretPoint.Y].Count)
                {
                    newCaretPoint.X++;
                }
                else if (newCaretPoint.Y + 1 < _viewModel.TextLines.Count)
                {
                    newCaretPoint.Y++;
                    newCaretPoint.X = 0;
                }
                break;
            case Keys.Up:
                if (newCaretPoint.Y > 0)
                {
                    newCaretPoint.Y--;
                    newCaretPoint.X = Math.Min(newCaretPoint.X, _viewModel.TextLines[newCaretPoint.Y].Count);
                }
                break;
            case Keys.Down:
                if (newCaretPoint.Y + 1 < _viewModel.TextLines.Count)
                {
                    newCaretPoint.Y++;
                    newCaretPoint.X = Math.Min(newCaretPoint.X, _viewModel.TextLines[newCaretPoint.Y].Count);
                }
                break;
        }

        if (newCaretPoint == _caretPoint) return;

        if ((ModifierKeys & Keys.Shift) is Keys.Shift)
        {
            switch (_selector.State)
            {
                case SelectionState.Unselected:
                    _selector.StartSelect(_caretPoint);
                    _selector.Select(newCaretPoint);
                    break;
                case SelectionState.Selecting:
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
        Invalidate();
    }

    private void OnMouseClick(object? sender, MouseEventArgs e)
    {
        var caretPoint = ClientToCoords(e.Location);
        if (caretPoint.Y >= _viewModel.TextLines.Count) return;
        var line = _viewModel.TextLines[caretPoint.Y];
        if (caretPoint.X > line.Count) return;
        
        OnCaretPointChanged(caretPoint);
    }

    private void OnMouseDown(object sender, MouseEventArgs e)
    {
        _mouseIsDown = true;

        var oldCaretPoint = _caretPoint;
        var caretPoint = ClientToClosestCoords(e.Location);
        OnCaretPointChanged(caretPoint);
        
        switch (_selector.State)
        {
            case SelectionState.Selected:
                _selector.Clear();
                break;
            case SelectionState.Selecting:
                _selector.Select(caretPoint);
                break;
            case SelectionState.Unselected when (ModifierKeys & Keys.Shift) is not 0:
                _selector.StartSelect(oldCaretPoint);
                break;
        }

        Invalidate();
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_mouseIsDown is false) return;

        var caretPoint = ClientToClosestCoords(e.Location);
        OnCaretPointChanged(caretPoint);

        switch (_selector.State)
        {
            case SelectionState.Unselected:
            {
                _selector.StartSelect(_caretPoint);
                Invalidate();
                return;
            }
            case SelectionState.Selecting:
            {
                var endPoint = ClientToClosestCoords(e.Location);
                _selector.Select(endPoint);
                Invalidate();
                return;
            }
        }
    }

    private void OnMouseUp(object sender, MouseEventArgs e)
    {
        _mouseIsDown = false;

        if (_selector.State is not SelectionState.Selecting)
        {
            return;
        }

        if ((ModifierKeys & Keys.Shift) is not Keys.Shift)
        {
            _selector.EndSelect();
        }

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
        var textLinesCount = _viewModel.TextLines.Count;
        for (var y = _vScrollBar.Value; y < textLinesCount; y++)
        {
            var textLine = _viewModel.TextLines[y];
            var textLineLength = textLine.Count;

            if (_selector.IsLineSelected(y))
            {
                var x1 = y == _selector.Start.Y ? _selector.Start.X : 0;
                var x2 = y == _selector.End.Y ? _selector.End.X : textLineLength + 1;
                var start = CoordsToClient(x1, y);
                var end = CoordsToClient(x2, y + 1);
                var size = new Size(end.X - start.X, end.Y - start.Y);
                var selectionRect = new Rectangle(start, size);
                
                // if is in client rectangle
                if (selectionRect.Right > 0)
                {
                    if (selectionRect.X < 0) selectionRect.InflateStart(selectionRect.X, 0);
                    var brush = _selector.State is SelectionState.Selecting ? SelectingRectBrush : SelectedRectBrush;
                    e.Graphics.FillRectangle(brush, selectionRect);
                }
            }

            var visibleCols = (textLineLength - _hScrollBar.Value).Crop(0, _paintZone.VisibleColumnsCount);
            if (visibleCols is 0) continue;
            
            var startCharRect = CoordsToClientRect(_hScrollBar.Value, y);
            var endCharRect = CoordsToClientRect(_hScrollBar.Value + visibleCols - 1, y);
            var textRect = Rectangle.Union(startCharRect, endCharRect);
            var text = textLine.AsSpan(_hScrollBar.Value, visibleCols);
            TextRenderer.DrawText(e.Graphics, text, Font, textRect, Color.Black, TextFormatFlags);
        }
    }

    private void SelectAll()
    {
        _selector.StartSelect(Point.Empty);
        var lastCaretPoint = GetLastCaretPoint();
        OnCaretPointChanged(lastCaretPoint);
        _selector.Select(_caretPoint);
        _selector.EndSelect();
        Invalidate();
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

        internal void EndSelect() => State = SelectionState.Selected;

        internal bool IsLineSelected(int index) =>
            State is not SelectionState.Unselected && Start.Y <= index && index <= End.Y;

        internal void Reselect() => State = SelectionState.Selecting;

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
