using JinGine.App.Commands;
using JinGine.App.Events;
using JinGine.App.Serialization;

namespace JinGine.App.Handlers;

public class OpenBinaryFileCommandHandler : ICommandHandler<OpenBinaryFileCommand>
{
    private readonly IBinaryFileSerializer _serializer;
    private readonly AppSettings _settings;
    private readonly IEventAggregator _eventAggregator;

    public OpenBinaryFileCommandHandler(
        IBinaryFileSerializer serializer,
        AppSettings settings,
        IEventAggregator eventAggregator)
    {
        _serializer = serializer;
        _settings = settings;
        _eventAggregator = eventAggregator;
    }

    public void Handle(OpenBinaryFileCommand command)
    {
        var filePath = Path.Combine(_settings.FilesPath, command.FileName);
        var data = _serializer.Deserialize(filePath);

        _eventAggregator.Publish(new LoadFileDataEvent(data, command.FileName));
    }
}