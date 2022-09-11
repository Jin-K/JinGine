using System.Collections.ObjectModel;

namespace JinGine.WinForms.Menu;

internal static class MenuItemsFactory
{
    internal static ToolStripItem[] CreateItems(IEnumerable<MenuItem> menuItems, IInformable informable)
    {
        var result = new Collection<ToolStripItem>();

        foreach (var menuItem in menuItems)
        {
            var item = new ToolStripMenuItem(menuItem.Text);

            if (menuItem.Description is not null)
            {
                item.MouseHover += (_, _) => informable.Info = menuItem.Description;
            }

            if (menuItem.Command is not null)
            {
                item.Click += (_, _) => menuItem.Command.Execute();
            }

            if (menuItem.Children is not null)
            {
                var children = CreateItems(menuItem.Children, informable);
                item.DropDownItems.AddRange(children);
            }

            result.Add(item);
        }

        return result.ToArray();
    }
}