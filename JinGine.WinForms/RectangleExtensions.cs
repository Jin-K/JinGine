namespace JinGine.WinForms;

internal static class RectangleExtensions
{
    internal static void InflateStart(ref this Rectangle rect, int x, int y)
    {
        rect.Width += x;
        rect.Height += y;
        rect.X -= x;
        rect.Y -= y;
    }

    internal static void InflateEnd(ref this Rectangle rect, int x, int y)
    {
        rect.Width += x;
        rect.Height += y;
    }
}