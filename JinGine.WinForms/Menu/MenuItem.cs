// ReSharper disable UseNullPropagation on delegates
// because it requires a call to "?.Invoke()" and that affects performance
namespace JinGine.WinForms.Menu;

/// <summary>
/// Our <see cref="ToolStripMenuItem"/> wrapper
/// </summary>
internal class MenuItem : ToolStripMenuItem
{
    private readonly Action? _handler;

    internal string? Description { get; }
    internal string? HotKey { get; } // TODO use ToolStripMenuItem.ShortcutKeys

    internal delegate void DescriptionAvailableHandler(string? description);
    internal new event DescriptionAvailableHandler? MouseHover;

    internal MenuItem(
        string text,
        string? description = null,
        Action? handler = null,
        string? hotKey = null) : base(text)
    {
        _handler = handler;

        Description = description;
        HotKey = hotKey;

        if (handler is not null) Click += MenuItem_Click;
        base.MouseHover += MenuItem_MouseHover;
    }

    private void MenuItem_Click(object? sender, EventArgs e)
    {
        if (_handler is not null) _handler();
    }

    private void MenuItem_MouseHover(object? sender, EventArgs e)
    {
        if (MouseHover is not null) MouseHover(Description);
    }
}