using JinGine.App.Commands;
using JinGine.App.Events;

namespace JinGine.App.Handlers;

public class OpenCSharpFileCommandHandler : ICommandHandler<OpenCSharpFileCommand>
{
    private readonly IFileManager _fileManager;
    private readonly IEventAggregator _eventAggregator;
    private readonly AppSettings _settings;

    public OpenCSharpFileCommandHandler(
        IFileManager fileManager,
        IEventAggregator eventAggregator,
        AppSettings settings)
    {
        _fileManager = fileManager;
        _eventAggregator = eventAggregator;
        _settings = settings;
    }

    public void Handle(OpenCSharpFileCommand command)
    {
        var fileName = _fileManager.ExpandPath(Path.Combine(_settings.FilesPath, command.FileName));
        
        if (_fileManager.IsUrl(fileName) is not true)
            _fileManager.AskCreateFileIfNotFound(fileName);

        var data = _fileManager.CreateEditorFile(fileName);

        _eventAggregator.Publish(new LoadFileDataEvent(data, fileName));
    }
}