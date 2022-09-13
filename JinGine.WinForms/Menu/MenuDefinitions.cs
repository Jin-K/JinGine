namespace JinGine.WinForms.Menu;

internal static class MenuDefinitions
{
    internal static MenuDefinition[] Items = {
        new("File", null, null, new MenuDefinition[]
        {
            new("Open file 1", "Open file 1 operations", null, new MenuDefinition[]
            {
                new("Open file 1 A", "Open file 1 A for real", new Commands.OpenFile1A())
            }),
            new("Open file 2", "Open file 2 operations"),
        })
    };
}