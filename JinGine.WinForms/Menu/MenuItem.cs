using JinGine.Core;

namespace JinGine.WinForms.Menu;

internal class MenuItem
{
    internal string Text { get; }
    internal string? Description { get; }
    internal ICommand? Command { get; }
    internal MenuItem[]? Children { get; }

    internal MenuItem(
        string text,
        string? description = null,
        ICommand? command = null,
        MenuItem[]? children = null)
    {
        Text = text;
        Description = description;
        Command = command;
        Children = children;
    }
}