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

    internal ToolStripItem ToToolStripItem(IInformable informable)
    {
        var menuItem = new ToolStripMenuItem(Text);

        if (Description is not null) menuItem.MouseHover += (_, _) => informable.Info = Description;
        if (Command is not null) menuItem.Click += (_, _) => Command.Execute();
        if (Children is not null)
        {
            foreach (var child in Children)
                menuItem.DropDownItems.Add(child.ToToolStripItem(informable));
        }

        return menuItem;
    }
}