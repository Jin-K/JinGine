using FluentAssertions;
using JinGine.Domain.Models;
using Xunit;

namespace JinGine.Domain.Tests.Entities;

public class EditorFileTests
{
    [Fact]
    public void Prepared_EditorFile_should_have_no_id_or_content()
    {
        // Act
        var editorFile = EditorFile.PrepareNew();
            
        // Assert
        editorFile.Id.Should().BeNull();
        editorFile.Content.Should().BeEmpty();
    }

    [Fact]
    public void EditorFile_opened_from_physical_file_should_have_path_as_id()
    {
        // Arrange
        var path = "c:\\aCoolFile.txt";

        // Act
        var editorFile = EditorFile.OpenFromPath(path, string.Empty);

        // Assert
        editorFile.Id.Should().Be(path);
    }
}