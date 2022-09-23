using System.Data;

namespace JinGine.Core.Models;

public class DataGridModel
{
    public DataTable DataTable { get; }

    public DataGridModel(DataTable dataTable)
    {
        DataTable = dataTable;
    }
}