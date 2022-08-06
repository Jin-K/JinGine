namespace JinGine.WinForms.Menu;

internal class MenuItem : ToolStripMenuItem
{
    private readonly Action _clickHandler;

    internal string Level { get; }
    internal string? Description { get; }
    internal string? HotKey { get; } // TODO use ToolStripMenuItem.ShortcutKeys

    internal delegate void DescriptionAvailableHandler(string? description);
    internal event DescriptionAvailableHandler? DescriptionAvailable;

    private MenuItem(
        Action clickHandler,
        string text,
        string level,
        string? description,
        string? hotKey
    ) : base(text)
    {
        _clickHandler = clickHandler;

        Level = level;
        Description = description;
        HotKey = hotKey;

        Click += MenuItem_Click;
        MouseHover += MenuItem_MouseHover;
    }

    public static implicit operator MenuItem((Action ClickHandler, string Text, string Level) tuple)
        => new(tuple.ClickHandler, tuple.Text, tuple.Level, null, null);

    public static implicit operator MenuItem((Action ClickHandler, string Text, string Level, string Description) tuple)
        => new(tuple.ClickHandler, tuple.Text, tuple.Level, tuple.Description, null);

    public static implicit operator MenuItem((Action ClickHandler, string Text, string Level, string Description, string HotKey) tuple)
        => new(tuple.ClickHandler, tuple.Text, tuple.Level, tuple.Description, tuple.HotKey);

    private void MenuItem_Click(object? sender, EventArgs e)
    {
        _clickHandler();
    }

    private void MenuItem_MouseHover(object? sender, EventArgs e)
    {
        DescriptionAvailable?.Invoke(Description);
    }
}