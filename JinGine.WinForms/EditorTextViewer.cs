using JinGine.WinForms.Views;

namespace JinGine.WinForms
{
    public partial class EditorTextViewer : UserControl
    {
        private const TextFormatFlags TextFFlags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;
        private readonly GridProjector _gridProjector;
        private readonly GridSelector _selector;
        private readonly Win32Caret _caret;
        private Point _caretGridLocation;
        private string[] _lines;

        public event EventHandler<char> KeyPressed;
        public event EventHandler<Point> CaretPointChanged;

        public EditorTextViewer()
        {
            InitializeComponent();

            _gridProjector = new GridProjector();
            _selector = new GridSelector(this, _gridProjector);
            _caret = new Win32Caret(this);
            _caretGridLocation = Point.Empty;
            _lines = Array.Empty<string>();

            KeyPressed = delegate { };
            CaretPointChanged = delegate { };
            
            base.DoubleBuffered = true;
            
            this.InitArrowKeyDownFiring();
            this.InitMouseWheelScrollDelegation(_vScrollBar);
        }
        
        internal void SetCaretGridLocation(Point location)
        {
            if (location == _caretGridLocation) return;
            _caretGridLocation = location;
            CaretPointChanged(this, location);
        }
        
        internal void SetGrid(CharsGrid grid)
        {
            _gridProjector.Grid = grid;
            _caret.Height = grid.CellHeight;
            _caret.Width = grid.CellWidth;
        }
        
        internal void SetLines(string[] lines)
        {
            if (_lines.SequenceEqual(lines)) return;
            _lines = lines;
            Invalidate();
        }
        
        private void OnMouseClick(object? sender, MouseEventArgs e)
        {
            var gridLocation = _gridProjector.GetGridLocationFromProjection(e.Location);
            SetCaretGridLocation(gridLocation);
            var caretLocation = _gridProjector.ProjectToScreenLocation(gridLocation);
            _caret.SetLocation(caretLocation);
            Invalidate();
        }
        
        private void OnHScrollBarScroll(object? sender, ScrollEventArgs e)
        {
            _gridProjector.SetX(e.NewValue);
            _caret.SetLocation(_gridProjector.ProjectToScreenLocation(_caretGridLocation));
            Invalidate();
        }
        
        private void OnVScrollBarScroll(object? sender, ScrollEventArgs e)
        {
            _gridProjector.SetY(e.NewValue);
            _caret.SetLocation(_gridProjector.ProjectToScreenLocation(_caretGridLocation));
            Invalidate();
        }
        
        // TODO still need to handle Home, End, Delete, PageUp, PageDown, Ctrl + A, Ctrl + C, Ctrl + X, Ctrl + V, Ctrl + Z, Ctrl + Y, etc.
        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            // TODO all this logic could go to the presenter ??
            var nCGridLoc = _caretGridLocation;
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

            if (nCGridLoc == _caretGridLocation) return;

            if ((ModifierKeys & Keys.Shift) is Keys.Shift)
            {
                // TODO fix
                _selector.StartSelect(_gridProjector.ProjectToScreenLocation(_caretGridLocation));
                _selector.EndSelect(_gridProjector.ProjectToScreenLocation(nCGridLoc));
            }

            SetCaretGridLocation(nCGridLoc);
            _caret.SetLocation(_gridProjector.ProjectToScreenLocation(nCGridLoc));
            Invalidate();
        }
        
        private void OnKeyPress(object? sender, KeyPressEventArgs e)
        {
            KeyPressed(this, e.KeyChar);
            e.Handled = true;
        }

        private void OnSizeChanged(object? sender, EventArgs e)
        {
            _gridProjector.SetBounds(ClientRectangle with
            {
                Width = ClientRectangle.Width - _vScrollBar.Width,
                Height = ClientRectangle.Height - _hScrollBar.Height
            });
            ScrollToCaretPoint();
        }
        
        private void OnPaint(object? sender, PaintEventArgs e)
        {
            var textBackgroundBrush = new SolidBrush(Color.White);
            // TODO all this visible and max things can be improved
            var maxVisibleColumns = (Width / _gridProjector.CellSize.Width).Crop(1, int.MaxValue);
            var maxScreenY = ClientRectangle.Bottom - _hScrollBar.Height - 1;
            var visibleGridLines = _lines.Skip(_gridProjector.Y)
                .Select((l, i) => (i + _gridProjector.Y, l));
            var firstVisGridLineX = _gridProjector.X;

            foreach (var (i, line) in visibleGridLines)
            {
                var visibleColumns = Math.Min(line.Length - firstVisGridLineX, maxVisibleColumns);
                if (visibleColumns <= 0) continue;

                var lastVisGridLineX = visibleColumns - 1 + firstVisGridLineX;
                var textRect = _gridProjector.ProjectToScreenRectangle(
                    new Point(firstVisGridLineX, i),
                    new Point(lastVisGridLineX, i));
                if (textRect.Y > maxScreenY) break;

                var bgRect = textRect with { X = 0 };
                bgRect.Inflate(textRect.Left, 0);
                e.Graphics.FillRectangle(textBackgroundBrush, bgRect);

                // TODO use ReadOnlySpan<char>
                var printableLine = line.ToPrintable(_gridProjector.X, visibleColumns);
                TextRenderer.DrawText(e.Graphics, printableLine, Font, textRect, Color.Black, TextFFlags);
            }

            // TODO integrate this in the loop above (make a grid selection per line ?)
            switch (_selector.State)
            {
                case SelectionState.Selecting:
                {
                    e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.BlueViolet)), _selector.Selection);
                    break;
                }
                case SelectionState.Selected:
                {
                    e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.YellowGreen)), _selector.Selection);
                    break;
                }
                case SelectionState.Unselected:
                default:
                    break;
            }
        }

        private void ScrollToCaretPoint()
        {
            _gridProjector.EnsureProjection(_caretGridLocation);

            // TODO raise scroll events
            if (_hScrollBar.Value != _gridProjector.X || _vScrollBar.Value != _gridProjector.Y)
                _caret.SetLocation(_gridProjector.ProjectToScreenLocation(_caretGridLocation), true);
            _hScrollBar.Value = _gridProjector.X;
            _vScrollBar.Value = _gridProjector.Y;
        }
    }
}
