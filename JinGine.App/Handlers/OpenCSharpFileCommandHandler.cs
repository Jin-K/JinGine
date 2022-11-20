using JinGine.App.Commands;
using JinGine.App.Events;
using JinGine.Domain.Repositories;

namespace JinGine.App.Handlers;

public class OpenCSharpFileCommandHandler : ICommandHandler<OpenCSharpFileCommand>
{
    private readonly IEventAggregator _eventAggregator;
    private readonly IEditorFileRepository _editorFileRepository;
    private readonly AppSettings _settings;

    public OpenCSharpFileCommandHandler(
        IEventAggregator eventAggregator,
        IEditorFileRepository editorFileRepository,
        AppSettings settings)
    {
        _eventAggregator = eventAggregator;
        _editorFileRepository = editorFileRepository;
        _settings = settings;
    }

    public void Handle(OpenCSharpFileCommand command)
    {
        var fileName = Path.Combine(_settings.FilesPath, command.FileName);
        var editorFile = _editorFileRepository.Get(fileName);
        _eventAggregator.Publish(new LoadFileDataEvent(editorFile, fileName));
    }
}