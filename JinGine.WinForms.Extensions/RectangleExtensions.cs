// ReSharper disable once CheckNamespace
namespace System.Drawing;

public static class RectangleExtensions
{
    public static Rectangle InflateEnd(this Rectangle rect, int x, int y)
    {
        rect.Width += x;
        rect.Height += y;
        return rect;
    }
}