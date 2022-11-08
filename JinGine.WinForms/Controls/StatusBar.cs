using JinGine.WinForms.Views;

namespace JinGine.WinForms.Controls
{
    public partial class StatusBar : UserControl, IStatusBarView
    {
        public string Info { set => textBox.Text = value; }

        public StatusBar()
        {
            InitializeComponent();
        }
    }
}
