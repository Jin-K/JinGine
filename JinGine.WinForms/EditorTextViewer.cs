using JinGine.WinForms.Views;

namespace JinGine.WinForms
{
    public partial class EditorTextViewer : UserControl
    {
        private const TextFormatFlags TextFFlags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;
        
        private readonly Win32Caret _caret;
        private TextProjector? _textProjector;
        private GridSelector? _selector;
        private string[] _lines;

        internal Point CaretLocation { get; set; }

        public event EventHandler<char>? KeyPressed;
        public event EventHandler<Point>? CaretLocationChanged;

        public EditorTextViewer()
        {
            InitializeComponent();

            _caret = new Win32Caret(this);
            _textProjector = null;
            _selector = null;
            _lines = Array.Empty<string>();
            CaretLocation = Point.Empty;
            
            base.DoubleBuffered = true;
            
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
            _textProjector = projector;
            _selector = new GridSelector(this, projector);
            _caret.Size = projector.CellSize;
        }

        private void OnMouseClick(object? sender, MouseEventArgs e)
        {
            if (_textProjector is null) return;
            var caretLocation = _textProjector.ScreenToGridLocation(e.Location);
            CaretLocation = caretLocation;
            CaretLocationChanged?.Invoke(this, caretLocation);
            SetWin32CaretPos(caretLocation);
        }
        
        private void OnHScrollBarScroll(object? sender, ScrollEventArgs e)
        {
            if (_textProjector is null) return;

            _textProjector.SetX(e.NewValue);
            _caret.Position = _textProjector.GridLocationToScreen(CaretLocation);
            Invalidate();
        }
        
        private void OnVScrollBarScroll(object? sender, ScrollEventArgs e)
        {
            if (_textProjector is null) return;

            _textProjector.SetY(e.NewValue);
            _caret.Position = _textProjector.GridLocationToScreen(CaretLocation);
            Invalidate();
        }
        
        // TODO still need to handle Home, End, Delete, PageUp, PageDown, Ctrl + A, Ctrl + C, Ctrl + X, Ctrl + V, Ctrl + Z, Ctrl + Y, etc.
        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (_textProjector is null || _selector is null) return;

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
                _selector.StartSelect(_textProjector.GridLocationToScreen(CaretLocation));
                _selector.EndSelect(_textProjector.GridLocationToScreen(nCGridLoc));
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
            if (_textProjector is null) return;

            var textProjBounds = ClientRectangle;
            textProjBounds.Width -= _vScrollBar.Width;
            textProjBounds.Height -= _hScrollBar.Height;
            _textProjector.SetBounds(textProjBounds);

            _textProjector.EnsureProjection(CaretLocation);
            
            if (_hScrollBar.Value != _textProjector.X)
                _hScrollBar.RaiseMouseWheel((_hScrollBar.Value - _textProjector.X) * SystemInformation.MouseWheelScrollDelta);

            if (_vScrollBar.Value != _textProjector.Y)
                _vScrollBar.RaiseMouseWheel((_vScrollBar.Value - _textProjector.Y) * SystemInformation.MouseWheelScrollDelta);
        }
        
        private void OnPaint(object? sender, PaintEventArgs e)
        {
            if (_textProjector is null || _textProjector.CellSize == Size.Empty || _selector is null) return;

            var textBackgroundBrush = new SolidBrush(Color.White);
            for (var i = _textProjector.Y; i < _lines.Length; i++)
            {
                var visibleCols = Math.Min(_lines[i].Length - _textProjector.X, _textProjector.MaxVisibleColumns);
                if (visibleCols <= 0) continue;
                
                var firstCharLoc = new Point(_textProjector.X, i);
                var lastCharLoc = new Point(_textProjector.X + visibleCols - 1, i);
                var textRect = _textProjector.GridLocationsToScreenRect(firstCharLoc, lastCharLoc);
                var bgRect = textRect with { X = 0 };

                bgRect.Inflate(textRect.Left, 0);
                e.Graphics.FillRectangle(textBackgroundBrush, bgRect);

                var text = _lines[i].AsSpan(_textProjector.X, visibleCols);
                TextRenderer.DrawText(e.Graphics, text, Font, textRect, Color.Black, TextFFlags);
            }

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
            if (_textProjector is null) return;
            _caret.Position = _textProjector.GridLocationToScreen(location);
            Invalidate();
        }
    }
}
