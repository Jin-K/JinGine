using System.Data;
using JinGine.Core.Serialization;

namespace JinGine.WinForms.Menu;

internal static class MenuItems
{
    internal static readonly MenuItemsTree RegisteredMenuItems = new()
    {
        // File
        {
            new MenuItem("File"), new MenuItemsTree
            {
                // File -> Open file 1
                {
                    new MenuItem("Open file 1", "Open file 1 operations"), new MenuItemsTree
                    {
                        // File -> Open file 1 -> Open file 1 A
                        {
                            new MenuItem("Open file 1 A", "Open file 1 A", OpenFile1A), null
                        }
                    }
                },
                // File -> Open file 2
                {
                    new MenuItem("Open file 2", "Open file 2"), null
                }
            }
        },
    };

    // TODO move this shit to a service or whatever else
    private static void OpenFile1A()
    {
        using var serializer = new FileSerializer("File1A.sez");
        var fileContent = serializer.Deserialize<DataTable>();
    }
}
