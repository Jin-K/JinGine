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
            MainMenuStrip.Items.AddRange(CreateMenuItemsRange(MenuItems.MenuItemsRegister));
        }

        private ToolStripItem[] CreateMenuItemsRange(MenuItemDictionary menuItemDictionary)
        {
            var result = new List<MenuItem>(menuItemDictionary.Count);

            foreach (var (menuItem, children) in menuItemDictionary)
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