namespace JinGine.WinForms.Menu;

internal static class MenuItems
{
    internal static readonly MenuItemDictionary MenuItemsRegister = new()
    {
        // File
        {
            new MenuItem(File, "File"), new MenuItemDictionary
            {
                // File -> Open file 1
                {
                    new MenuItem(OpenFile1, "Open file 1", "Open file 1 operations"), new MenuItemDictionary
                    {
                        // File -> Open file 1 -> Open file 1 A
                        {
                            new MenuItem(OpenFile1A, "Open file 1 A", "Open file 1 A"), null
                        }
                    }
                },
                // File -> Open file 2
                {
                    new MenuItem(OpenFile2, "Open file 2", "Open file 2"), null
                }
            }
        },
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
