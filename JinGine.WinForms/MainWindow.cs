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
            var menuItems = MenuItemsFactory.CreateItems(MenuDefinitions.Items, statusBar);
            this.MainMenuStrip.Items.AddRange(menuItems);
            ResumeLayout();
        }
    }
}