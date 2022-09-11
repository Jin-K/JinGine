using JinGine.Core;

namespace JinGine.WinForms
{
    public partial class StatusBar : UserControl, IInformable
    {
        public string Info { set => textBox.Text = value; }

        public StatusBar()
        {
            InitializeComponent();
        }
    }
}
