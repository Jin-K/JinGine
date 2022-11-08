using System.Data;
using JinGine.WinForms.Views;

namespace JinGine.WinForms.Controls
{
    public partial class DataGrid : UserControl, IDataGridView
    {
        public DataGrid()
        {
            InitializeComponent();
            base.Dock = DockStyle.Fill;
        }

        public void ShowTable(DataTable dataTable)
        {
            dataGridView1.DataSource = dataTable;
        }
    }
}
