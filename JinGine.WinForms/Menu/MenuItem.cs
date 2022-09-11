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

    public static implicit operator MenuItem(
        (string Text, string? Description, ICommand? Command, MenuItem[]? Children) tuple) =>
        new(tuple.Text, tuple.Description, tuple.Command, tuple.Children);

    public static implicit operator MenuItem(
        (string Text, string? Description, ICommand? Command) tuple) =>
        new(tuple.Text, tuple.Description, tuple.Command);

    public static implicit operator MenuItem(
        (string Text, string? Description) tuple) =>
        new(tuple.Text, tuple.Description);

    public static implicit operator MenuItem(string text) => new(text);
}