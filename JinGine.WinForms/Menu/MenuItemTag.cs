namespace JinGine.WinForms.Menu
{
    internal delegate void ClickEventHandler(Action action);
    internal delegate void MouseHoverEventHandler(string? description);

    internal class MenuItemTag
    {
        private readonly Action _clickAction;
        private readonly MenuItemAttribute _attribute;
        private readonly ClickEventHandler _clickHandler;
        private readonly MouseHoverEventHandler _mouseHoverHandler;

        internal MenuItemTag(
            Action clickAction,
            MenuItemAttribute attribute,
            ClickEventHandler clickHandler,
            MouseHoverEventHandler mouseHoverHandler)
        {
            _clickAction = clickAction;
            _attribute = attribute;
            _clickHandler = clickHandler;
            _mouseHoverHandler = mouseHoverHandler;
        }

        internal ToolStripMenuItem ToMenuItem()
        {
            var menuItem = new ToolStripMenuItem(_attribute.Title) { Tag = this };
            menuItem.Click += MenuItem_Click;
            menuItem.MouseHover += MenuItem_MouseHover;
            return menuItem;
        }

        private void MenuItem_Click(object? sender, EventArgs e)
        {
            _clickHandler(_clickAction);
        }

        private void MenuItem_MouseHover(object? sender, EventArgs e)
        {
            _mouseHoverHandler(_attribute.Description);
        }
    }
}
