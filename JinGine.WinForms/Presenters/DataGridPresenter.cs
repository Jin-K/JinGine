using System.Data;
using JinGine.WinForms.Views;

namespace JinGine.WinForms.Presenters;

// TODO global search
internal class DataGridPresenter
{
    internal DataGridPresenter(IDataGridView view, DataTable dataTable)
    {
        view.ShowTable(dataTable);
    }
}