using LegacyFwk;

namespace JinGine.WinForms.Views;

public readonly record struct CharsGrid(int CellWidth, int CellHeight, int XMargin)
{
    internal static CharsGrid Create(FontDescriptor fDescriptor) =>
        new(fDescriptor.Width, fDescriptor.Height, fDescriptor.LeftMargin);

    internal Point GetGridCellLocation(int screenX, int screenY) => new((screenX - XMargin) / CellWidth, screenY / CellHeight);

    internal Rectangle GetScreenCellRectangle(Point location)
    {
        var x = location.X * CellWidth + XMargin;
        var y = location.Y * CellHeight;
        return new Rectangle(x, y, CellWidth, CellHeight);
    }
}