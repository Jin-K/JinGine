namespace JinGine.WinForms;

public static class RectangleExtensions
{
    public static void InflateStart(ref this Rectangle rect, int x, int y)
    {
        rect.Width += x;
        rect.Height += y;
        rect.X -= x;
        rect.Y -= y;
    }

    public static void InflateEnd(ref this Rectangle rect, int x, int y)
    {
        rect.Width += x;
        rect.Height += y;
    }
}