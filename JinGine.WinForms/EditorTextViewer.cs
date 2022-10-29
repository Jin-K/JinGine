using System.Runtime.InteropServices;
using LegacyFwk;

namespace JinGine.WinForms
{
    public partial class EditorTextViewer : UserControl
    {
        private readonly FontDescriptor _font;
        private string[]? _lines;

        public event EventHandler<char> KeyPressed;
        public event EventHandler<Point> CaretPointChanged;

        internal Point CaretPoint { private get; set; }

        public EditorTextViewer()
        {
            InitializeComponent();
            MouseWheel += OnMouseWheel;
            GotFocus += OnGotFocus;
            LostFocus += OnLostFocus;
            _font = FontDescriptor.DefaultFixed;
            base.Font = FontDescriptor.DefaultFixed.Font;
            base.DoubleBuffered = true;

            KeyPressed = delegate {};
            CaretPointChanged = delegate {};
        }

        public void SetLines(string[] lines)
        {
            _lines = lines;
            Invalidate();
        }

        private int GetPaintZoneTop(int lineIndex) => ClientRectangle.Top + (lineIndex - _vScrollBar.Value) * _font.Height;

        private void OnGotFocus(object? sender, EventArgs e)
        {
            _ = User32.CreateCaret(Handle, new IntPtr(0), _font.Width, _font.Height);
            ResetCaretPos();
            _ = User32.ShowCaret(Handle);
        }

        private void OnLostFocus(object? sender, EventArgs e)
        {
            _ = User32.DestroyCaret();
        }

        private void OnMouseClick(object? sender, MouseEventArgs e)
        {
            // TODO find cartesian coordinates from e and set CaretPoint
        }

        private void OnMouseWheel(object? sender, MouseEventArgs e)
        {
            var deltaScroll = Math.Sign(e.Delta) * SystemInformation.MouseWheelScrollLines;
            if (deltaScroll is not 0)
            {
                var newScrollV = _vScrollBar.Value - deltaScroll;
                if (newScrollV < 0 || newScrollV > _vScrollBar.Maximum) return;
                _vScrollBar.Value = newScrollV;
                Invalidate();
            }
        }

        private void OnHScrollBarScroll(object? sender, ScrollEventArgs e) => Invalidate();

        private void OnVScrollBarScroll(object? sender, ScrollEventArgs e) => Invalidate();

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (_lines is null) return;
            if (e.KeyCode is not Keys.Left and not Keys.Right and not Keys.Down and not Keys.Up) return;

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
            CaretPointChanged(this, point);
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
            
            ResetCaretPos();
        }

        private void OnPreviewKeyDown(object? sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode is Keys.Left or Keys.Right or Keys.Down or Keys.Up)
                e.IsInputKey = true;
        }

        private void ResetCaretPos()
        {
            _ = User32.SetCaretPos(
                (CaretPoint.X - _hScrollBar.Value) * _font.Width + _font.LeftMargin,
                (CaretPoint.Y - _vScrollBar.Value) * _font.Height);
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
                var deltaScrollLeft = (deltaLeft + _font.Width - 1) / _font.Width;
                _hScrollBar.Value -= deltaScrollLeft;
                paintZoneLeft += deltaScrollLeft * _font.Width;
            }

            var paintZoneRight = paintZoneLeft + _font.Width;
            var deltaRight = paintZoneRight - (ClientRectangle.Right - _vScrollBar.Width);
            if (deltaRight > 0) _hScrollBar.Value += (deltaRight + _font.Width - 1) / _font.Width;
        }

        private class User32
        {
            [DllImport("user32")]
            internal static extern int CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

            [DllImport("user32")]
            internal static extern int DestroyCaret();

            [DllImport("user32")]
            internal static extern int SetCaretPos(int x, int y);

            [DllImport("user32")]
            internal static extern int ShowCaret(IntPtr hWnd);
        }
    }
}
