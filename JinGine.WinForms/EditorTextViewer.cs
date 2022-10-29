using LegacyFwk;

namespace JinGine.WinForms
{
    public partial class EditorTextViewer : UserControl
    {
        private readonly FontDescriptor _font;
        private readonly PointsSelector _selector;
        private readonly InfiniteGrid _grid; // TODO use it !!
        private string[]? _lines; // TODO make non-nullable
        private Point _caretPoint;

        public event EventHandler<char> KeyPressed;
        public event EventHandler<Point> CaretPointChanged;

        internal Point CaretPoint
        {
            private get => _caretPoint;
            set
            {
                if (_caretPoint == value) return;
                _caretPoint = value;
                CaretPointChanged(this, value);
            }
        }

        public EditorTextViewer()
        {
            InitializeComponent();
            _font = FontDescriptor.DefaultFixed;
            _selector = new PointsSelector(this);
            _grid = new InfiniteGrid(_font.Width, _font.Height, _font.LeftMargin);
            base.Font = _font.Font;
            base.DoubleBuffered = true;
            
            this.InitArrowKeyDownFiring();
            this.InitMouseWheelScrolling(_vScrollBar);
            this.InitWin32Caret(_font.Width, _font.Height, GetCaretScreenPoint);

            KeyPressed = delegate {};
            CaretPointChanged = delegate {};
        }

        public void SetLines(string[] lines)
        {
            if (_lines is not null && _lines.SequenceEqual(lines)) return;

            _lines = lines;
            Invalidate();
        }

        private Point GetCaretScreenPoint() => ScreenPointFromCaretPoint(CaretPoint);

        private int GetPaintZoneTop(int lineIndex) => ClientRectangle.Top + (lineIndex - _vScrollBar.Value) * _font.Height;

        private void OnMouseClick(object? sender, MouseEventArgs e)
        {
            var caretPointX = _hScrollBar.Value + (e.X - _font.LeftMargin) / _font.Width;
            var caretPointY = _vScrollBar.Value + e.Y / _font.Height;
            CaretPoint = new Point(caretPointX, caretPointY);
            
            var caretScreenPos = GetCaretScreenPoint();
            this.SetCaretPos(caretScreenPos.X, caretScreenPos.Y);
        }

        private void OnHScrollBarScroll(object? sender, ScrollEventArgs e) => Invalidate();

        private void OnVScrollBarScroll(object? sender, ScrollEventArgs e) => Invalidate();

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (_lines is null) return;

            // TODO all this logic could go to the presenter ??
            var newCPoint = CaretPoint; // copying
            switch (e.KeyCode)
            {
                case Keys.Left:
                    if (newCPoint.X > 0) newCPoint.X--;
                    else if (newCPoint.Y > 0)
                    {
                        newCPoint.Y--;
                        newCPoint.X = _lines[newCPoint.Y].Length;
                    }
                    break;
                case Keys.Right:
                    if (newCPoint.X < _lines[newCPoint.Y].Length) newCPoint.X++;
                    else if (newCPoint.Y + 1 < _lines.Length)
                    {
                        newCPoint.Y++;
                        newCPoint.X = 0;
                    }
                    break;
                case Keys.Up:
                    if (newCPoint.Y > 0)
                    {
                        newCPoint.Y--;
                        newCPoint.X = Math.Min(newCPoint.X, _lines[newCPoint.Y].Length);
                    }
                    break;
                case Keys.Down:
                    if (newCPoint.Y + 1 < _lines.Length)
                    {
                        newCPoint.Y++;
                        newCPoint.X = Math.Min(newCPoint.X, _lines[newCPoint.Y].Length);
                    }
                    break;
            }

            if (newCPoint == CaretPoint) return;

            if ((ModifierKeys & Keys.Shift) is Keys.Shift)
            {
                // TODO fix and complete
                var startPoint = GetCaretScreenPoint();
                var endPoint = ScreenPointFromCaretPoint(newCPoint);
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        startPoint.X += _font.Width - 1;
                        startPoint.Y += _font.Height - 1;
                        break;
                }
                _selector.StartSelect(startPoint).EndSelect(endPoint);
            }

            CaretPoint = newCPoint;
            Invalidate();
        }

        private void OnKeyPress(object? sender, KeyPressEventArgs e)
        {
            KeyPressed(this, e.KeyChar);
            e.Handled = true;
        }

        private void OnSizeChanged(object? sender, EventArgs e) => ScrollToCaretPoint();

        private void OnPaint(object? sender, PaintEventArgs e)
        {
            if (_lines is null) return;

            var textBackgroundBrush = new SolidBrush(Color.White);
            var maxVisibleColumns = (Width / _font.Width).Crop(1, int.MaxValue);

            for (var lineIndex = _vScrollBar.Value; lineIndex < _lines.Length; lineIndex++)
            {
                var lineContent = _lines[lineIndex];
                var scrollRemainingColumns = lineContent.Length - _hScrollBar.Value;
                var visibleColumns = scrollRemainingColumns.Crop(0, maxVisibleColumns);

                var paintZoneTop = GetPaintZoneTop(lineIndex);
                var paintZoneWidth = visibleColumns * _font.Width + _font.RightMargin;
                var paintZone = new Rectangle(ClientRectangle.Left, paintZoneTop, paintZoneWidth, _font.Height);

                if (paintZone.Top >= ClientRectangle.Bottom - _hScrollBar.Height) break;
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
                var lineText = lineContent.ToPrintable(_hScrollBar.Value, visibleColumns);
                TextRenderer.DrawText(e.Graphics, lineText, Font, paintZone, Color.Black, textFormatFlags);
            }

            // TODO integrate this in the loop above
            switch (_selector.State)
            {
                case SelectionState.Selecting:
                    e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.BlueViolet)), _selector.GetSelectionMask());
                    break;
                case SelectionState.Selected:
                    e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.YellowGreen)), _selector.GetSelectionMask());
                    break;
                case SelectionState.Unselected:
                default:
                    break;
            }
        }

        private Point ScreenPointFromCaretPoint(Point caretPoint) => new(
            (caretPoint.X - _hScrollBar.Value) * _font.Width + _font.LeftMargin,
            (caretPoint.Y - _vScrollBar.Value) * _font.Height);

        private void ScrollToCaretPoint()
        {
            var paintZoneTop = GetPaintZoneTop(CaretPoint.Y);
            var deltaTop = ClientRectangle.Top - paintZoneTop;
            if (deltaTop > 0 && _vScrollBar.Value > 0)
            {
                var deltaScrollTop = (deltaTop + _font.Height - 1) / _font.Height;
                _vScrollBar.Value -= deltaScrollTop;
                paintZoneTop += deltaScrollTop * _font.Height;
            }

            var paintZoneBottom = paintZoneTop + _font.Height;
            var deltaBottom = paintZoneBottom - (ClientRectangle.Bottom - _hScrollBar.Height);
            if (deltaBottom > 0) _vScrollBar.Value += (deltaBottom + _font.Height - 1) / _font.Height;

            var paintZoneLeft = ClientRectangle.Left + (CaretPoint.X - _hScrollBar.Value) * _font.Width;
            var deltaLeft = ClientRectangle.Left - paintZoneLeft;
            if (deltaLeft > 0 && _hScrollBar.Value > 0)
            {
                var deltaScrollLeft = (deltaLeft + _font.Width - 1 - _font.LeftMargin) / _font.Width;
                _hScrollBar.Value -= deltaScrollLeft;
                paintZoneLeft += deltaScrollLeft * _font.Width;
            }

            var paintZoneRight = paintZoneLeft + _font.Width;
            var deltaRight = paintZoneRight - (ClientRectangle.Right - _vScrollBar.Width);
            if (deltaRight > 0) _hScrollBar.Value += (deltaRight + _font.Width - 1) / _font.Width;
        }

        #region Private helper types

        private enum SelectionState
        {
            Unselected,
            Selecting,
            Selected,
        }

        private class PointsSelector
        {
            private readonly Control _control;
            private Point _startPoint;
            private Point _endPoint;

            internal SelectionState State { get; private set; }

            internal PointsSelector(Control control)
            {
                _control = control;
                _startPoint = Point.Empty;
                _endPoint = Point.Empty;
                State = SelectionState.Unselected;

                control.MouseDown += OnMouseDown;
                control.MouseMove += OnMouseMove;
                control.MouseUp += OnMouseUp;
            }

            internal void EndSelect(Point endPoint)
            {
                if (_startPoint == endPoint)
                {
                    State = SelectionState.Unselected;
                    _startPoint = _endPoint = Point.Empty;
                    return;
                }

                _endPoint = endPoint;
                State = SelectionState.Selected;
            }

            internal Rectangle GetSelectionMask()
            {
                if (State is SelectionState.Unselected) return Rectangle.Empty;

                if (_startPoint == _endPoint)
                    throw new InvalidOperationException("Don't know how you arrived here.");

                var startX = _startPoint.X;
                var startY = _startPoint.Y;
                var endX = _endPoint.X;
                var endY = _endPoint.Y;

                if (startX <= endX)
                {
                    return startY <= endY ?
                        new Rectangle(startX, startY, endX - startX, endY - startY) :
                        new Rectangle(startX, endY, endX - startX, startY - endY);
                }

                return startY <= endY ?
                    new Rectangle(endX, startY, startX - endX, endY - startY) :
                    new Rectangle(endX, endY, startX - endX, startY - endY);
            }

            internal PointsSelector StartSelect(Point startPoint)
            {
                _endPoint = _startPoint = startPoint;
                State = SelectionState.Selecting;
                return this;
            }

            private void OnMouseDown(object? sender, MouseEventArgs e) => StartSelect(e.Location);

            private void OnMouseMove(object? sender, MouseEventArgs e)
            {
                if (State is SelectionState.Selecting) _endPoint = e.Location;
                _control.Invalidate();
            }

            private void OnMouseUp(object? sender, MouseEventArgs e)
            {
                if (State is SelectionState.Selecting) EndSelect(_endPoint);
            }
        }

        private class InfiniteGrid
        {
            private readonly int _cellWidth;
            private readonly int _cellHeight;
            private readonly int _leftMargin;

            internal Point this[int x, int y] => GetGridCoordinates(x, y);

            internal InfiniteGrid(int cellWidth, int cellHeight, int leftMargin)
            {
                _cellWidth = cellWidth;
                _cellHeight = cellHeight;
                _leftMargin = leftMargin;
            }

            private Point GetGridCoordinates(int x, int y) => new(
                (x - _leftMargin) / _cellWidth,
                y / _cellHeight);
        }

        #endregion
    }
}
