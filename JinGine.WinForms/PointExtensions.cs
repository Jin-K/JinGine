namespace JinGine.WinForms;

internal static class PointExtensions
{
    internal static void CropX(ref this Point point, int min, int max) =>
        point.X = Math.Max(min, Math.Min(max, point.X));

    internal static void CropY(ref this Point point, int min, int max) =>
        point.Y = Math.Max(min, Math.Min(max, point.Y));
}