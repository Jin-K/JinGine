using JinGine.Core;

namespace JinGine.WinForms.Menu;

internal class MenuItemDecorator : ToolStripMenuItem
{
    internal MenuItemDecorator(MenuItem menuItem, IInformable informable) : base(menuItem.Text)
    {
        if (menuItem.Description is not null) MouseHover += (_, _) => informable.Info = menuItem.Description;
        if (menuItem.Command is not null) Click += (_, _) => menuItem.Command.Execute();
        if (menuItem.Children is null) return;

        foreach (var child in menuItem.Children)
            DropDownItems.Add(new MenuItemDecorator(child, informable));
    }
}