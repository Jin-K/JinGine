using JinGine.WinForms.Views;

namespace JinGine.WinForms
{
    public partial class EditorTextViewer : UserControl
    {
        private const TextFormatFlags TextFFlags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;
        
        private readonly Win32Caret _caret;
        private GridProjector? _gridProjector;
        private GridSelector? _selector;
        private Point _caretGridLocation;
        private string[] _lines;

        public event EventHandler<char>? KeyPressed;
        public event EventHandler<Point>? CaretPointChanged;

        public EditorTextViewer()
        {
            InitializeComponent();

            _caret = new Win32Caret(this);
            _gridProjector = null;
            _selector = null;
            _caretGridLocation = Point.Empty;
            _lines = Array.Empty<string>();
            
            base.DoubleBuffered = true;
            
            this.InitArrowKeyDownFiring();
            this.InitMouseWheelScrollDelegation(_vScrollBar);
        }
        
        internal void SetCaretGridLocation(Point location)
        {
            if (location == _caretGridLocation) return;
            _caretGridLocation = location;
            if (CaretPointChanged is not null)
                CaretPointChanged(this, location);
        }
        
        internal void SetLines(string[] lines)
        {
            if (_lines.SequenceEqual(lines)) return;
            _lines = lines;
            Invalidate();
        }

        internal void SetProjector(GridProjector projector)
        {
            _gridProjector = projector;
            _selector = new GridSelector(this, projector);
            _caret.Height = projector.CellSize.Height;
            _caret.Width = projector.CellSize.Width;
        }

        private void OnMouseClick(object? sender, MouseEventArgs e)
        {
            if (_gridProjector is null) return;

            var gridLocation = _gridProjector.GetGridLocationFromProjection(e.Location);
            SetCaretGridLocation(gridLocation);
            var caretLocation = _gridProjector.ProjectToScreenLocation(gridLocation);
            _caret.SetLocation(caretLocation);
            Invalidate();
        }
        
        private void OnHScrollBarScroll(object? sender, ScrollEventArgs e)
        {
            if (_gridProjector is null) return;

            _gridProjector.SetX(e.NewValue);
            _caret.SetLocation(_gridProjector.ProjectToScreenLocation(_caretGridLocation));
            Invalidate();
        }
        
        private void OnVScrollBarScroll(object? sender, ScrollEventArgs e)
        {
            if (_gridProjector is null) return;

            _gridProjector.SetY(e.NewValue);
            _caret.SetLocation(_gridProjector.ProjectToScreenLocation(_caretGridLocation));
            Invalidate();
        }
        
        // TODO still need to handle Home, End, Delete, PageUp, PageDown, Ctrl + A, Ctrl + C, Ctrl + X, Ctrl + V, Ctrl + Z, Ctrl + Y, etc.
        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (_gridProjector is null || _selector is null) return;

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
            if (KeyPressed is not null)
                KeyPressed(this, e.KeyChar);
            e.Handled = true;
        }

        private void OnSizeChanged(object? sender, EventArgs e)
        {
            _gridProjector?.SetBounds(ClientRectangle with
            {
                Width = ClientRectangle.Width - _vScrollBar.Width,
                Height = ClientRectangle.Height - _hScrollBar.Height
            });
            ScrollToCaretPoint();
        }
        
        private void OnPaint(object? sender, PaintEventArgs e)
        {
            if (_gridProjector is null || _gridProjector.CellSize == Size.Empty || _selector is null) return;

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
            if (_gridProjector is null) return;

            _gridProjector.EnsureProjection(_caretGridLocation);

            // TODO raise scroll events
            if (_hScrollBar.Value != _gridProjector.X || _vScrollBar.Value != _gridProjector.Y)
                _caret.SetLocation(_gridProjector.ProjectToScreenLocation(_caretGridLocation), true);
            _hScrollBar.Value = _gridProjector.X;
            _vScrollBar.Value = _gridProjector.Y;
        }
    }
}
