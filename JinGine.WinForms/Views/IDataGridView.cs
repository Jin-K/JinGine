using System.Data;

namespace JinGine.WinForms.Views;

internal interface IDataGridView
{
    void ShowTable(DataTable dataTable);
}