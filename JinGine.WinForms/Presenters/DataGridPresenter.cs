using System.Data;
using JinGine.WinForms.Views;

namespace JinGine.WinForms.Presenters;

internal class DataGridPresenter
{
    internal DataGridPresenter(IDataGridView view, DataTable dataTable)
    {
        view.ShowTable(dataTable);
    }
}