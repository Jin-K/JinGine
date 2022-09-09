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
            MenuManager.BindMenuItems(this, desc => statusBar.TextBox.Text = desc);
            ResumeLayout();
        }
    }
}