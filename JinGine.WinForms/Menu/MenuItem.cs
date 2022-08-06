namespace JinGine.WinForms.Menu;

internal class MenuItem : ToolStripMenuItem
{
    private readonly Action _handler;

    internal string? Description { get; }
    internal string? HotKey { get; } // TODO use ToolStripMenuItem.ShortcutKeys

    internal delegate void DescriptionAvailableHandler(string? description);
    internal new event DescriptionAvailableHandler? MouseHover;

    internal MenuItem(
        Action handler,
        string text,
        string? description = null,
        string? hotKey = null) : base(text)
    {
        _handler = handler;

        Description = description;
        HotKey = hotKey;

        Click += MenuItem_Click;
        base.MouseHover += MenuItem_MouseHover;
    }

    private void MenuItem_Click(object? sender, EventArgs e)
    {
        _handler();
    }

    private void MenuItem_MouseHover(object? sender, EventArgs e)
    {
        MouseHover?.Invoke(Description);
    }
}