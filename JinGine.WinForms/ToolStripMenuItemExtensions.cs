namespace JinGine.WinForms;

public static class ToolStripMenuItemExtensions
{
    public static ToolStripMenuItem OnMouseHover(this ToolStripMenuItem source, EventHandler mouseHoverEventHandler)
    {
        source.MouseHover += mouseHoverEventHandler;
        return source;
    }

    public static ToolStripMenuItem OnClick(this ToolStripMenuItem source, EventHandler clickEventHandler)
    {
        source.Click += clickEventHandler;
        return source;
    }

    public static ToolStripMenuItem WithChildren(this ToolStripMenuItem source, params ToolStripMenuItem[] children)
    {
        foreach (var child in children) source.DropDownItems.Add(child);
        return source;
    }
}