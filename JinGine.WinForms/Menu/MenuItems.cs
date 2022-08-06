namespace JinGine.WinForms.Menu;

internal static class MenuItems
{
    internal static readonly MenuItem[] Registrations =
    {
        new("0", File, "File"),
        new("0_FILE", OpenFile1, "Open file 1", "Open file 1 operations"),
        new("0_FILE_1", OpenFile1A, "Open file 1 A", "Open file 1 A"),
        new("0_FILE", OpenFile2, "Open file 2", "Open file 2"),
    };
    
    private static void File()
    {
    }

    private static void OpenFile1()
    {
    }

    private static void OpenFile1A()
    {
    }

    private static void OpenFile2()
    {
    }
}
