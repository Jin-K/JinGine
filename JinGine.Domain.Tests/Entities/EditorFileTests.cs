using FluentAssertions;
using JinGine.Domain.Models;
using Xunit;

namespace JinGine.Domain.Tests.Entities;

public class EditorFileTests
{
    [Fact]
    public void Prepared_EditorFile_should_have_no_path_or_content()
    {
        // Act
        var editorFile = EditorFile.PrepareNew();
            
        // Assert
        editorFile.Path.Should().BeNull();
        editorFile.Content.Should().Be(FileContent.Empty);
    }

    [Fact]
    public void EditorFile_opened_from_physical_file_should_have_path()
    {
        // Arrange
        var physicalFile = new PhysicalFile("c:\\aCoolFile.txt", string.Empty);

        // Act
        var editorFile = EditorFile.OpenFrom(physicalFile); // TODO is it a bad practice passing a value object to an entity constructor/factory method ?

        // Assert
        editorFile.Path.Should().NotBeNull();
    }
}