namespace JinGine.WinForms.Views;

internal class GridProjector
{
    private Rectangle _bounds;
    private CharsGrid _grid;

    internal ref CharsGrid Grid => ref _grid;
    internal int X { get; private set; }
    internal int Y { get; private set; }

    internal Size CellSize => new(Grid.CellWidth, Grid.CellHeight);

    internal void EnsureProjection(Point gridLoc)
    {
        var screenRect = ProjectToScreenRectangle(gridLoc);
        if (_bounds.Contains(screenRect)) return;

        var deltaTop = RoundedUpDivision(_bounds.Top - screenRect.Top, Grid.CellHeight);
        if (deltaTop > 0) Y -= deltaTop;
        var deltaBottom = RoundedUpDivision(screenRect.Bottom - _bounds.Bottom, Grid.CellHeight);
        if (deltaBottom > 0) Y += deltaBottom;
        var deltaLeft = RoundedUpDivision(_bounds.Left - Grid.XMargin - screenRect.Left, Grid.CellWidth);
        if (deltaLeft > 0) X -= deltaLeft;
        var deltaRight = RoundedUpDivision(screenRect.Right - (_bounds.Right - Grid.XMargin), Grid.CellWidth);
        if (deltaRight > 0) X += deltaRight;
    }

    internal Point GetGridLocationFromProjection(Point screenLoc) // TODO UnprojectFromPixels ?
    {
        var gridLocation = Grid.GetGridCellLocation(screenLoc.X, screenLoc.Y);
        ApplyOffsets(ref gridLocation);
        return gridLocation;
    }

    internal Point ProjectToScreenLocation(Point gridLoc) // TODO ProjectToPixels ?
    {
        RemoveOffsets(ref gridLoc);
        return Grid.GetScreenCellRectangle(gridLoc).Location;
    }

    internal Rectangle ProjectToScreenRectangle(Point gridLoc1, Point gridLoc2)
    {
        RemoveOffsets(ref gridLoc1);
        RemoveOffsets(ref gridLoc2);
        return Rectangle.Union(Grid.GetScreenCellRectangle(gridLoc1), Grid.GetScreenCellRectangle(gridLoc2));
    }

    internal Rectangle ProjectToScreenRectangle(Point gridLoc)
    {
        RemoveOffsets(ref gridLoc);
        return Grid.GetScreenCellRectangle(gridLoc);
    }

    internal void SetBounds(Rectangle bounds) => _bounds = bounds;

    internal void SetOffsets(int x, int y)
    {
        if (x < 0) throw new ArgumentOutOfRangeException(nameof(x), ExceptionMessages.GridProjector_SetOffsets_Should_be_zero_or_positive_);
        if (y < 0) throw new ArgumentOutOfRangeException(nameof(y), ExceptionMessages.GridProjector_SetOffsets_Should_be_zero_or_positive_);
        X = x;
        Y = y;
    }

    internal void SetX(int x) => SetOffsets(x, Y);

    internal void SetY(int y) => SetOffsets(X, y);

    private void ApplyOffsets(ref Point gridLoc)
    {
        gridLoc.X += X;
        gridLoc.Y += Y;
    }

    private void RemoveOffsets(ref Point gridLoc)
    {
        gridLoc.X -= X;
        gridLoc.Y -= Y;
    }

    private static int RoundedUpDivision(int count, int divider) => (count + (divider - 1)) / divider;
}