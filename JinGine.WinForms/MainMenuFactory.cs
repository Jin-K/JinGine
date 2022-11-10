using JinGine.App.Commands;
using JinGine.App.Events;

namespace JinGine.WinForms;

internal class MainMenuFactory // TODO move to infra ?
{
    private readonly IEventAggregator _eventAggregator;
    private readonly ICommandDispatcher _commandDispatcher;

    public MainMenuFactory(IEventAggregator eventAggregator, ICommandDispatcher commandDispatcher)
    {
        _eventAggregator = eventAggregator;
        _commandDispatcher = commandDispatcher;
    }

    internal ToolStripItem[] CreateItems(params MenuItemDef[] definitions)
    {
        var result = new ToolStripItem[definitions.Length];

        for (var i = 0; i < result.Length; i++)
        {
            var def = definitions[i];
            var item = new ToolStripMenuItem(def.Text);
            if (def.Description is not null)
                item.MouseHover += (_, _) => _eventAggregator.Publish(new UpdateStatusBarInfoEvent(def.Description));
            if (def.Command is not null)
                item.Click += (_, _) => _commandDispatcher.GetType().GetMethod(nameof(_commandDispatcher.Dispatch))
                    ?.MakeGenericMethod(def.Command.GetType())
                    .Invoke(_commandDispatcher, new object[] { def.Command });
            if (def.Children is not null)
                item.DropDownItems.AddRange(CreateItems(def.Children));
            result[i] = item;
        }

        return result;
    }
}

internal record MenuItemDefNode<T>(string Text, string? Description, ICommand? Command, T[]? Children)
    where T : MenuItemDefNode<T>;

internal record MenuItemDef(string Text, string? Description, ICommand? Command, MenuItemDef[]? Children)
    : MenuItemDefNode<MenuItemDef>(Text, Description, Command, Children)
{
    public static implicit operator MenuItemDef(
        (string Text, string? Description, ICommand? Command, MenuItemDef[]? Children) tuple) =>
        new (tuple.Text, tuple.Description, tuple.Command, tuple.Children);
}