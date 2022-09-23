using JinGine.Core.Models;
using JinGine.WinForms.Views;

namespace JinGine.WinForms.Presenters;

// TODO global search
internal class DataGridPresenter
{
    internal DataGridPresenter(IDataGridView view, DataGridModel model)
    {
        view.ShowTable(model.DataTable);
    }
}