namespace LegacyFwk;

public record FontDescriptor(Font Font, int Height, int Width, int LeftMargin, int RightMargin)
{
    public static readonly FontDescriptor DefaultFixed = Create("Courier New", 8.2f, true);

    private static FontDescriptor Create(string familyName, float emSize, bool isFixed, FontStyle style = FontStyle.Regular)
    {
        var font = new Font(familyName, emSize, style, GraphicsUnit.Point, 0);
        var height = font.Height + 1;

        if (!isFixed) return new FontDescriptor(font, height, 0, 0, 0);

        var width = (int)(TextRenderer.MeasureText("123456789", font).Width / 10d);
        if (width > 8) width++;
        var leftMargin = width / 2;
        var rightMargin = width - leftMargin;

        return new FontDescriptor(font, height, width, leftMargin, rightMargin);
    }
}