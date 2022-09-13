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
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);

        Serialize(fileStream);
        Deserialize(fileStream);
    }

    private static void Serialize(Stream stream)
    {
        var dataTable = new DataTable("New Table");
        
        var strategy = new BinaryStreamStrategy(stream);
        var serializer = new Serializer(strategy);

        serializer.Serialize(dataTable);
    }

    private static DataTable Deserialize(Stream stream)
    {
        var strategy = new BinaryStreamStrategy(stream);
        var serializer = new Serializer(strategy);

        return serializer.Deserialize<DataTable>();
    }
}