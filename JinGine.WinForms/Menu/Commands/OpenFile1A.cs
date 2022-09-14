using System.Data;
using JinGine.Core;
using JinGine.Core.Serialization;
using JinGine.Core.Serialization.Strategies;
using JinGine.WinForms.Properties;

namespace JinGine.WinForms.Menu.Commands;

internal class OpenFile1A : ICommand
{
    public void Execute()
    {
        var filePath = Path.Combine(Settings.Default.FilesPath, "File1A.bin");
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
        using var strategy = new BinaryStreamStrategy(fileStream);

        Serialize(strategy, new DataTable("New Table"));
        var dataTable = Deserialize(strategy);

        // TODO Houston, we have a problem
        //(Form.ActiveForm as MainWindow).ShowDaTable(dataTable);
    }

    private static void Serialize(IStrategy strategy, DataTable data) => new Serializer<DataTable>(strategy).Serialize(data);

    private static DataTable Deserialize(IStrategy strategy) => new Serializer<DataTable>(strategy).Deserialize();
}
