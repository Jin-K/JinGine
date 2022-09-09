namespace JinGine.WinForms.Menu;

internal static class MenuManager
{
    internal static void BindMenuItems(Form form, Action<string> descriptionHandler)
    {
        // level 3
        var openFile1AMenuItem = new ToolStripMenuItemBuilder("Open file 1 A")
            .OnMouseHover(() => descriptionHandler("Open file 1 A for real"))
            .WithCommand(new Commands.OpenFile1ACommand())
            .Build();

        // level 2
        var openFile1MenuItem = new ToolStripMenuItemBuilder("Open file 1")
            .OnMouseHover(() => descriptionHandler("Open file 1 operations"))
            .WithChildren(openFile1AMenuItem)
            .Build();
        var openFile2MenuItem = new ToolStripMenuItemBuilder("Open file 2")
            .OnMouseHover(() => descriptionHandler("Open file 2 operations"))
            .Build();

        // level 1
        var fileMenuItem = new ToolStripMenuItemBuilder("File")
            .WithChildren(openFile1MenuItem, openFile2MenuItem)
            .Build();

        form.MainMenuStrip.Items.Add(fileMenuItem);
    }
}