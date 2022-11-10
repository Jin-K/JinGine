using System;
using JinGine.App.Commands;
using JinGine.App.Events;
using JinGine.App.Handlers;
using JinGine.App.Serialization;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace JinGine.App.Tests.Handlers
{
    public class OpenBinaryFileCommandHandlerTests
    {
        private const string FileName = "TheCoolestFileOnEarth.bin";

        private readonly AutoMocker _autoMocker;
        private readonly ICommandHandler<OpenBinaryFileCommand> _sut;

        public OpenBinaryFileCommandHandlerTests()
        {
            _autoMocker = new AutoMocker();
            
            var fileManagerMock = _autoMocker.GetMock<IFileManager>();
            fileManagerMock
                .Setup(x => x.ExpandPath(It.IsAny<string>()))
                .Returns($"C:\\{FileName}");
            
            var serializerMock = _autoMocker.GetMock<IBinaryFileSerializer>();
            serializerMock
                .Setup(x => x.Deserialize(It.IsAny<string>()))
                .Returns(new object());

            var eventAggregatorMock = _autoMocker.GetMock<IEventAggregator>(true);
            
            _sut = new OpenBinaryFileCommandHandler(
                fileManagerMock.Object,
                serializerMock.Object,
                new AppSettings(string.Empty),
                eventAggregatorMock.Object);
        }

        [Fact]
        public void File_name_is_expanded()
        {
            // Arrange
            var command = new OpenBinaryFileCommand(FileName);
            var fileManagerMock = _autoMocker.GetMock<IFileManager>();

            // Act
            _sut.Handle(command);

            // Assert
            fileManagerMock.Verify(x =>
                x.ExpandPath(It.Is(FileName, StringComparer.Ordinal)), Times.Once);
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
                    @event.FileName.Equals(FileName))), Times.Once);
        }
    }
}