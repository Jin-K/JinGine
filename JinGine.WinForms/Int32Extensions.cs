namespace JinGine.WinForms;

internal static class Int32Extensions
{
    internal static int Crop(this int source, int min, int max) =>
        Math.Max(min, Math.Min(max, source));
}