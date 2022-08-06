using JinGine.Core.Collections;
using JinGine.WinForms.Menu;

namespace JinGine.WinForms
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeMenu();
        }
        
        private void InitializeMenu()
        {
            var lastMenuLevelPrefix = string.Empty;
            var menuLevelPrefixes = new UniqueStack<string>();
            var rootMenuItem = null as ToolStripMenuItem;
            var orderedMenuItems = MenuItems.Registrations.OrderBy(t => t.Level).ToArray();
            foreach (var menuItem in orderedMenuItems)
            {
                menuItem.DescriptionAvailable += description => statusBar.TextBox.Text = description;

                while (!menuItem.Level.StartsWith(lastMenuLevelPrefix))
                {
                    lastMenuLevelPrefix = menuLevelPrefixes.Pop();
                    rootMenuItem = rootMenuItem?.OwnerItem as ToolStripMenuItem;
                }

                if (rootMenuItem is not null)
                {
                    rootMenuItem.DropDownItems.Add(menuItem);
                }
                else
                {
                    MainMenuStrip.Items.Add(menuItem);
                }

                var levelPrefix = menuItem.Level[..menuItem.Level.LastIndexOf('_')];

                if (!menuLevelPrefixes.Push(levelPrefix)) continue;

                lastMenuLevelPrefix = levelPrefix;
                rootMenuItem = menuItem;
            }
        }
    }
}