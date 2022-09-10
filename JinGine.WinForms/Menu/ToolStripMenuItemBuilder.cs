namespace JinGine.WinForms.Menu;

/// <summary>
/// A <see cref="ToolStripMenuItem"/> builder
/// </summary>
internal class ToolStripMenuItemBuilder
{
    private readonly ToolStripMenuItem _menuItem;

    internal ToolStripMenuItemBuilder(string text)
    {
        _menuItem = new ToolStripMenuItem(text);
    }

    internal ToolStripMenuItemBuilder AddChildren(params ToolStripItem[] children)
    {
        _menuItem.DropDownItems.AddRange(children);
        return this;
    }

    internal ToolStripMenuItemBuilder OnClick(Action<ToolStripMenuItem> clickHandler)
    {
        _menuItem.Click += (_, _) => clickHandler(_menuItem);
        return this;
    }

    internal ToolStripMenuItemBuilder OnMouseHover(Action<ToolStripMenuItem> mouseHoverHandler)
    {
        _menuItem.MouseHover += (_, _) => mouseHoverHandler(_menuItem);
        return this;
    }

    internal ToolStripMenuItem BuildItem() => _menuItem;
}