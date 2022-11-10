using System.Runtime.Serialization.Formatters.Binary;
using JinGine.App.Commands;
using JinGine.App.Events;

namespace JinGine.App.Handlers;

public class OpenBinaryFileCommandHandler : ICommandHandler<OpenBinaryFileCommand>
{
    private readonly IFileManager _fileManager;
    private readonly AppSettings _settings;

    public OpenBinaryFileCommandHandler(IFileManager fileManager, AppSettings settings)
    {
        _fileManager = fileManager;
        _settings = settings;
    }

    public void Handle(OpenBinaryFileCommand command)
    {
        var filePath = _fileManager.ExpandPath(Path.Combine(_settings.FilesPath, command.FileName));
        var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var formatter = new BinaryFormatter();
        var data = formatter.Deserialize(fs);
        fs.Close();

        EventAggregator.Instance.Publish(new LoadFileDataEvent(data, filePath));
    }
}