using System.Collections.ObjectModel;
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
            SuspendLayout();
            MainMenuStrip.Items.AddRange(CreateMenuItemsRange(MenuItems.RegisteredMenuItems));
            ResumeLayout();
        }

        private ToolStripItem[] CreateMenuItemsRange(MenuItemsTree menuItemsTree)
        {
            var result = new Collection<MenuItem>();

            foreach (var (menuItem, children) in menuItemsTree)
            {
                menuItem.MouseHover += description => statusBar.TextBox.Text = description;

                if (children?.Any() ?? false)
                {
                    menuItem.DropDownItems.AddRange(CreateMenuItemsRange(children));
                }

                result.Add(menuItem);
            }

            return result.Cast<ToolStripItem>().ToArray();
        }
    }
}