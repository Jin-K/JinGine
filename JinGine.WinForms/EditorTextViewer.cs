﻿using LegacyFwk;

namespace JinGine.WinForms
{
    public partial class EditorTextViewer : UserControl
    {
        private readonly FontDescriptor _font;
        private string[]? _lines;

        public event EventHandler<char>? KeyPressed;

        internal Point CaretPoint { private get; set; }

        public EditorTextViewer()
        {
            InitializeComponent();
            _font = FontDescriptor.DefaultFixed;
            base.Font = _font.Font;
            base.DoubleBuffered = true;
        }

        public void SetLines(string[] lines)
        {
            _lines = lines;
            Invalidate();
        }

        private int GetPaintZoneTop(int lineIndex) => ClientRectangle.Top + (lineIndex - _vScrollBar.Value) * _font.Height;

        private void OnHScrollBarScroll(object? sender, ScrollEventArgs e) => Invalidate();

        private void OnVScrollBarScroll(object? sender, ScrollEventArgs e) => Invalidate();

        private void OnKeyPress(object? sender, KeyPressEventArgs e)
        {
            KeyPressed?.Invoke(this, e.KeyChar);
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
    }
}
