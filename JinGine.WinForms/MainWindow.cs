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
            var lastLevel = string.Empty;
            var levelsStack = new UniqueStack<string>();
            var rootMenuItem = null as ToolStripMenuItem;
            foreach (var menuItem in MenuItems.Registrations)
            {
                menuItem.MouseHover += description => statusBar.TextBox.Text = description;

                while (!menuItem.Level.StartsWith(lastLevel))
                {
                    lastLevel = levelsStack.Pop();
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

                if (!levelsStack.Push(menuItem.Level)) continue;

                lastLevel = menuItem.Level;
                rootMenuItem = menuItem;
            }
        }
    }
}