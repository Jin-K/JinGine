namespace JinGine.WinForms.Menu;

internal static class MenuDefinitions
{
    internal static MenuItem[] Items = {
        ("File", null, null, new MenuItem[]
        {
            ("Open file 1", "Open file 1 operations", null, new MenuItem[]
            {
                ("Open file 1 A", "Open file 1 A for real", new Commands.OpenFile1ACommand())
            }),
            ("Open file 2", "Open file 2 operations"),
        })
    };
}