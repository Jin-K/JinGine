namespace JinGine.WinForms.Views;

public class GridProjector
{
    private readonly CharsGrid _grid;
    private Rectangle _bounds;

    internal Size CellSize { get; }

    internal int X { get; private set; }
    internal int Y { get; private set; }

    internal GridProjector(CharsGrid grid)
    {
        _grid = grid;
        CellSize = new Size(grid.CellWidth, grid.CellHeight);
    }

    internal void EnsureProjection(Point gridLoc)
    {
        var screenRect = ProjectToScreenRectangle(gridLoc);
        if (_bounds.Contains(screenRect)) return;

        var deltaTop = RoundedUpDivision(_bounds.Top - screenRect.Top, _grid.CellHeight);
        if (deltaTop > 0) Y -= deltaTop;
        var deltaBottom = RoundedUpDivision(screenRect.Bottom - _bounds.Bottom, _grid.CellHeight);
        if (deltaBottom > 0) Y += deltaBottom;
        var deltaLeft = RoundedUpDivision(_bounds.Left - _grid.XMargin - screenRect.Left, _grid.CellWidth);
        if (deltaLeft > 0) X -= deltaLeft;
        var deltaRight = RoundedUpDivision(screenRect.Right - (_bounds.Right - _grid.XMargin), _grid.CellWidth);
        if (deltaRight > 0) X += deltaRight;
    }

    internal Point GetGridLocationFromProjection(Point screenLoc) // TODO UnprojectFromPixels ?
    {
        var gridLocation = _grid.GetGridCellLocation(screenLoc.X, screenLoc.Y);
        ApplyOffsets(ref gridLocation);
        return gridLocation;
    }

    internal Point ProjectToScreenLocation(Point gridLoc) // TODO ProjectToPixels ?
    {
        RemoveOffsets(ref gridLoc);
        return _grid.GetScreenCellRectangle(gridLoc).Location;
    }

    internal Rectangle ProjectToScreenRectangle(Point gridLoc1, Point gridLoc2)
    {
        RemoveOffsets(ref gridLoc1);
        RemoveOffsets(ref gridLoc2);
        return Rectangle.Union(_grid.GetScreenCellRectangle(gridLoc1), _grid.GetScreenCellRectangle(gridLoc2));
    }

    internal Rectangle ProjectToScreenRectangle(Point gridLoc)
    {
        RemoveOffsets(ref gridLoc);
        return _grid.GetScreenCellRectangle(gridLoc);
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