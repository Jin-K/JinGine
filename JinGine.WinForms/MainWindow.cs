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

            foreach (var item in MenuDefinitions.Items)
            {
                MainMenuStrip.Items.Add(new MenuItemDecorator(item, statusBar));
            }

            ResumeLayout();
        }
    }
}