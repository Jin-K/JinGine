namespace JinGine.WinForms.Views;

// TODO and now I realize it could just be ints, and simplify EditorTextViewer.Selector
public record TextSelectionRange(Point Start, Point End)
{
    public static readonly TextSelectionRange Empty = new(default, default);
}