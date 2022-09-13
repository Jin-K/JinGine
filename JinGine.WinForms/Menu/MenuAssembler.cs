using JinGine.Core;

namespace JinGine.WinForms.Menu;

internal static class MenuAssembler // static factory
{
    internal static ToolStripItem[] CreateToolStripItems(IEnumerable<MenuDefinition> definitions, IInformable informable)
    {
        return definitions.Select(def => def.ToToolStripItem(informable)).ToArray();
    }

    private static ToolStripItem ToToolStripItem(this MenuDefinition source, IInformable informable)
    {
        var result = new ToolStripMenuItem(source.Text);

        if (source.Description is not null) result.MouseHover += (_, _) => informable.Info = source.Description;
        if (source.Command is not null) result.Click += (_, _) => source.Command.Execute();
        if (source.Children is not null)
        {
            var childItems = source.Children.Select(child => child.ToToolStripItem(informable)).ToArray();
            result.DropDownItems.AddRange(childItems);
        }

        return result;
    }
}
