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
        editorFile.Content.IsEmpty.Should().BeTrue();
        editorFile.Content.Should().BeEquivalentTo(FileContent.Empty);
    }

    [Fact]
    public void EditorFile_opened_from_physical_file_should_have_path()
    {
        // Arrange
        var path = "c:\\aCoolFile.txt";
        var textResolver = () => string.Empty;

        // Act
        var editorFile = EditorFile.OpenFromPath(path, textResolver);

        // Assert
        editorFile.Path.Should().NotBeNull().And.Be(path);
    }

    [Fact]
    public void EditorFile_Text_can_be_reset()
    {
        // Arrange
        var editorFile = EditorFile.PrepareNew();

        // Act
        var act = () => editorFile.ResetText("something else");

        // Assert
        act.Should().NotThrow();
    }
}