using JinGine.Core;

namespace JinGine.WinForms.Menu;

/// <summary>
/// A <see cref="ToolStripMenuItem"/> builder
/// </summary>
internal class ToolStripMenuItemBuilder
{
    private readonly string _text;
    private EventHandler? _clickHandler;
    private EventHandler? _mouseHoverHandler;
    private ToolStripItem[]? _children;

    internal ToolStripMenuItemBuilder(string text)
    {
        _text = text;
    }

    internal ToolStripMenuItemBuilder WithCommand(ICommand command)
    {
        _clickHandler = (_, _) => command.Execute();
        return this;
    }

    internal ToolStripMenuItemBuilder OnMouseHover(Action mouseHoverHandler)
    {
        _mouseHoverHandler = (_, _) => mouseHoverHandler();
        return this;
    }

    internal ToolStripMenuItemBuilder WithChildren(params ToolStripItem[] children)
    {
        _children = children;
        return this;
    }

    internal ToolStripMenuItem Build()
    {
        var item = new ToolStripMenuItem(_text);
        if (_clickHandler is not null) item.Click += _clickHandler;
        if (_mouseHoverHandler is not null) item.MouseHover += _mouseHoverHandler;
        if (_children is not null) item.DropDownItems.AddRange(_children);
        return item;
    }
}