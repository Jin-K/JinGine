namespace JinGine.WinForms.Views;

public class TextProjector
{
    private readonly Grid _grid;
    private Rectangle _bounds;

    internal Size CellSize => _grid.CellSize;
    internal int X { get; private set; }
    internal int Y { get; private set; }
    internal int MaxVisibleColumns { get; private set; }
    internal int MaxVisibleLines { get; private set; }

    private TextProjector(Grid grid) => _grid = grid;

    internal static TextProjector Create(LegacyFwk.FontDescriptor fDescriptor) =>
        new(new Grid(new Size(fDescriptor.Width, fDescriptor.Height), fDescriptor.LeftMargin));

    internal void EnsureProjection(Point gridLoc)
    {
        var cellRect = GridLocationToScreenRect(gridLoc);
        if (_bounds.Contains(cellRect)) return; // TODO do we really need to check ?
        
        Y -= Math.Max(0, (_bounds.Top - cellRect.Top + CellSize.Height - 1) / CellSize.Height);
        Y += Math.Max(0, (cellRect.Bottom - _bounds.Bottom + CellSize.Height - 1) / CellSize.Height);
        X -= Math.Max(0, (_bounds.Left - _grid.XMargin - cellRect.Left + CellSize.Width - 1) / CellSize.Width);
        X += Math.Max(0, (cellRect.Right - (_bounds.Right - _grid.XMargin) + CellSize.Width - 1) / CellSize.Width);
    }

    internal Point GridLocationToScreen(Point gridLoc)
    {
        var x = (gridLoc.X - X) * CellSize.Width + _grid.XMargin;
        var y = (gridLoc.Y - Y) * CellSize.Height;
        return new Point(x, y);
    }

    internal Rectangle GridLocationToScreenRect(Point gridLoc)
    {
        var screenLoc = GridLocationToScreen(gridLoc);
        return new Rectangle(screenLoc, CellSize);
    }

    internal Rectangle GridLocationsToScreenRect(Point gridLoc1, Point gridLoc2)
    {
        var rect1 = GridLocationToScreenRect(gridLoc1);
        var rect2 = GridLocationToScreenRect(gridLoc2);
        return Rectangle.Union(rect1, rect2);
    }

    internal void SetBounds(Rectangle bounds)
    {
        _bounds = bounds;
        MaxVisibleColumns = (_bounds.Width + CellSize.Width - 1 - _grid.XMargin) / CellSize.Width;
        MaxVisibleLines = (_bounds.Height + CellSize.Height - 1) / CellSize.Height;
    }

    internal void SetX(int x)
    {
        if (x < 0) throw new ArgumentOutOfRangeException(nameof(x), ExceptionMessages.GridProjector_SetOffsets_Should_be_zero_or_positive_);
        X = x;
    }

    internal void SetY(int y)
    {
        if (y < 0) throw new ArgumentOutOfRangeException(nameof(y), ExceptionMessages.GridProjector_SetOffsets_Should_be_zero_or_positive_);
        Y = y;
    }

    internal Point ScreenToGridLocation(Point screenLoc)
    {
        var x = (screenLoc.X - _grid.XMargin) / CellSize.Width + X;
        var y = screenLoc.Y / CellSize.Height + Y;
        return new Point(x, y);
    }

    private readonly record struct Grid(Size CellSize, int XMargin);
}