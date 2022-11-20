using JinGine.App.Commands;
using JinGine.App.Events;
using JinGine.App.Handlers;
using JinGine.App.Serialization;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace JinGine.App.Tests.Handlers;

public class OpenBinaryFileCommandHandlerTests
{
    private const string FileName = "TheCoolestFileOnEarth.bin";
    private const string FilesPath = "nasa://";

    private readonly AutoMocker _autoMocker;
    private readonly object _expectedDeserializeResult;
    private readonly ICommandHandler<OpenBinaryFileCommand> _sut;

    public OpenBinaryFileCommandHandlerTests()
    {
        _autoMocker = new AutoMocker();
        _expectedDeserializeResult = new object();
            
        _autoMocker.GetMock<IBinaryFileSerializer>()
            .Setup(x => x.Deserialize(It.IsAny<string>()))
            .Returns(_expectedDeserializeResult);

        _autoMocker.GetMock<IEventAggregator>(true);
            
        _sut = new OpenBinaryFileCommandHandler(
            _autoMocker.Get<IBinaryFileSerializer>(),
            new AppSettings(FilesPath),
            _autoMocker.Get<IEventAggregator>());
    }

    [Fact]
    public void Path_should_be_composed_with_settings_file_path_for_deserialize_method()
    {
        // Arrange
        var command = new OpenBinaryFileCommand(FileName);
        var serializerMock = _autoMocker.GetMock<IBinaryFileSerializer>();
        var expectedComposedPath = System.IO.Path.Combine(FilesPath, FileName);

        // Act
        _sut.Handle(command);

        // Assert
        serializerMock.Verify(x => x.Deserialize(expectedComposedPath));
    }

    [Fact]
    public void Uses_non_generic_deserialize_method()
    {
        // Arrange
        var command = new OpenBinaryFileCommand(FileName);
        var serializerMock = _autoMocker.GetMock<IBinaryFileSerializer>();

        // Act
        _sut.Handle(command);

        // Assert
        serializerMock.Verify(x =>
            x.Deserialize(It.IsAny<string>()), Times.Once);
        serializerMock.Verify(x =>
            x.Deserialize<It.IsAnyType>(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Publishes_a_valid_LoadFileDataEvent()
    {
        // Arrange
        var command = new OpenBinaryFileCommand(FileName);
        var eventAggregatorMock = _autoMocker.GetMock<IEventAggregator>();

        // Act
        _sut.Handle(command);

        // Assert
        eventAggregatorMock.Verify(x =>
            x.Publish(It.Is<LoadFileDataEvent>(@event =>
                @event.FileName.Equals(FileName) &&
                @event.FileData.Equals(_expectedDeserializeResult))), Times.Once);
    }
}