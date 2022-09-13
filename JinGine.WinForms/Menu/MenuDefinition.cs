using JinGine.Core;

namespace JinGine.WinForms.Menu;

internal class MenuDefinition
{
    internal string Text { get; }
    internal string? Description { get; }
    internal ICommand? Command { get; }
    internal MenuDefinition[]? Children { get; }

    internal MenuDefinition(
        string text,
        string? description = null,
        ICommand? command = null,
        MenuDefinition[]? children = null)
    {
        Text = text;
        Description = description;
        Command = command;
        Children = children;
    }
}
