namespace LegacyFwk;

public static class ControlStylesHelper
{
    public static ControlStyles DoubleBufferedInputControl
        => ControlStyles.UserPaint |
           ControlStyles.AllPaintingInWmPaint |
           ControlStyles.DoubleBuffer |
           ControlStyles.Selectable |
           ControlStyles.StandardClick;
}