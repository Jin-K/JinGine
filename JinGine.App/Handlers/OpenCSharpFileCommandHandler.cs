using JinGine.App.Commands;
using JinGine.App.Events;
using JinGine.Domain.Models;

namespace JinGine.App.Handlers;

public class OpenCSharpFileCommandHandler : ICommandHandler<OpenCSharpFileCommand>
{
    private readonly IFileManager _fileManager;
    private readonly AppSettings _settings;

    public OpenCSharpFileCommandHandler(IFileManager fileManager, AppSettings settings)
    {
        _fileManager = fileManager;
        _settings = settings;
    }

    public void Handle(OpenCSharpFileCommand command)
    {
        var fileName = _fileManager.ExpandPath(Path.Combine(_settings.FilesPath, command.FileName));
        
        if (_fileManager.IsUrl(fileName) is not true)
            _fileManager.AskCreateFileIfNotFound(fileName);

        var data = new Editor2DText(_fileManager.GetTextContent(fileName));

        EventAggregator.Instance.Publish(new LoadFileDataEvent(data, fileName));
    }
}