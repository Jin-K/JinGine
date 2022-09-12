using JinGine.Core;
using JinGine.Core.Serialization;
using JinGine.Core.Serialization.Strategies;

namespace JinGine.WinForms.Menu.Commands;

internal class OpenFile1A : ICommand
{
    public void Execute()
    {
        var fileSerializer = new BinaryFileSerializer(Files.File1A);
        using var serializationContext = new FileSerializationContext(fileSerializer);
        var result = serializationContext.Deserialize();

        throw new NotImplementedException();
    }
}