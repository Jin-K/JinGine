namespace JinGine.WinForms.Menu;

internal class MenuAssembler : IMenuAssembler
{
    private readonly IDescriber _describer;

    internal MenuAssembler(IDescriber describer) => _describer = describer;

    public ToolStripMenuItem[] CreateItems()
    {
        // level 3
        var openFile1AMenuItem = new ToolStripMenuItemBuilder("Open file 1 A")
            .OnMouseHover((_) => _describer.Description = "Open file 1 A for real")
            .OnClick((_) => new Commands.OpenFile1ACommand().Execute())
            .BuildItem();

        // level 2
        var openFile1MenuItem = new ToolStripMenuItemBuilder("Open file 1")
            .AddChildren(openFile1AMenuItem)
            .OnMouseHover((_) => _describer.Description = "Open file 1 operations")
            .BuildItem();
        var openFile2MenuItem = new ToolStripMenuItemBuilder("Open file 2")
            .OnMouseHover((_) => _describer.Description = "Open file 2 operations")
            .BuildItem();

        // level 1
        var fileMenuItem = new ToolStripMenuItemBuilder("File")
            .AddChildren(openFile1MenuItem, openFile2MenuItem)
            .BuildItem();

        return new[] { fileMenuItem };
    }
}