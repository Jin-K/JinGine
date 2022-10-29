using LegacyFwk;

namespace JinGine.WinForms
{
    public partial class EditorTextViewer : UserControl
    {
        private readonly FontDescriptor _font;
        private readonly PointsSelector _selector;
        private string[]? _lines;
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
            base.Font = _font.Font;
            base.DoubleBuffered = true;
            
            this.InitArrowKeyDownFiring();
            this.InitMouseWheelScrolling(_vScrollBar);
            this.InitWin32Caret(_font.Width, _font.Height, GetCaretScreenPosition);

            KeyPressed = delegate {};
            CaretPointChanged = delegate {};
        }

        public void SetLines(string[] lines)
        {
            if (_lines is not null && _lines.SequenceEqual(lines)) return;

            _lines = lines;
            Invalidate();
        }

        private Point GetCaretScreenPosition() => new(
            (CaretPoint.X - _hScrollBar.Value) * _font.Width + _font.LeftMargin,
            (CaretPoint.Y - _vScrollBar.Value) * _font.Height);

        private int GetPaintZoneTop(int lineIndex) => ClientRectangle.Top + (lineIndex - _vScrollBar.Value) * _font.Height;

        private void OnMouseClick(object? sender, MouseEventArgs e)
        {
            var caretPointX = _hScrollBar.Value + (e.X - _font.LeftMargin) / _font.Width;
            var caretPointY = _vScrollBar.Value + e.Y / _font.Height;
            CaretPoint = new Point(caretPointX, caretPointY);
            
            var caretScreenPos = GetCaretScreenPosition();
            this.SetCaretPos(caretScreenPos.X, caretScreenPos.Y);
        }

        private void OnHScrollBarScroll(object? sender, ScrollEventArgs e) => Invalidate();

        private void OnVScrollBarScroll(object? sender, ScrollEventArgs e) => Invalidate();

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (_lines is null) return;

            // TODO all this logic could go to the presenter ??
            var point = CaretPoint; // copying
            switch (e.KeyCode)
            {
                case Keys.Left:
                    if (point.X > 0) point.X--;
                    else if (point.Y > 0)
                    {
                        point.Y--;
                        point.X = _lines[point.Y].Length;
                    }
                    break;
                case Keys.Right:
                    if (point.X < _lines[point.Y].Length) point.X++;
                    else if (point.Y + 1 < _lines.Length)
                    {
                        point.Y++;
                        point.X = 0;
                    }
                    break;
                case Keys.Up:
                    if (point.Y > 0)
                    {
                        point.Y--;
                        point.X = Math.Min(point.X, _lines[point.Y].Length);
                    }
                    break;
                case Keys.Down:
                    if (point.Y + 1 < _lines.Length)
                    {
                        point.Y++;
                        point.X = Math.Min(point.X, _lines[point.Y].Length);
                    }
                    break;
            }

            if (point == CaretPoint) return;
            CaretPoint = point;
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
            if (_selector.IsSelecting)
            {
                e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.BlueViolet)), _selector.SelectionMask);
            }
        }

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

        private class PointsSelector
        {
            private readonly Control _control;
            private Point _mouseDownPoint;
            private Point _mousePoint;

            internal bool IsSelecting { get; private set; }

            internal ref Point Start => ref _mouseDownPoint;
            internal ref Point End => ref _mousePoint;

            internal Rectangle SelectionMask
            {
                get
                {
                    var x = Math.Min(Start.X, End.X);
                    var y = Math.Min(Start.Y, End.Y);
                    return new Rectangle(x, y, Math.Max(Start.X, End.X) - x, Math.Max(Start.Y, End.Y) - y);
                }
            }

            internal PointsSelector(Control control)
            {
                _control = control;
                _mouseDownPoint = Point.Empty;
                _mousePoint = Point.Empty;
                IsSelecting = false;

                control.MouseDown += OnMouseDown;
                control.MouseMove += OnMouseMove;
                control.MouseUp += OnMouseUp;
            }

            private void OnMouseDown(object? sender, MouseEventArgs e)
            {
                IsSelecting = true;
                _mousePoint = _mouseDownPoint = e.Location;
            }

            private void OnMouseMove(object? sender, MouseEventArgs e)
            {
                _mousePoint = e.Location;
                _control.Invalidate();
            }

            private void OnMouseUp(object? sender, MouseEventArgs e) => IsSelecting = false;
        }
    }
}
