using FluentAssertions;
using JinGine.Domain.Models;
using Xunit;

namespace JinGine.Domain.Tests.Entities
{
    public class EditorFileTests
    {
        [Fact]
        public void New_editor_file_without_path_should_have_empty_content()
        {
            // Act
            var editorFile = new EditorFile();
            
            // Assert
            editorFile.Path.Should().BeNull();
            editorFile.Content.Should().BeEmpty();
        }

        [Fact]
        public void New_editor_file_with_path_and_content_should_have_same_content()
        {
            // Arrange
            const string expectedContent = "cool content";

            // Act
            var editorFile = new EditorFile("c:\\aCoolFile.txt", expectedContent);

            // Assert
            editorFile.Path.Should().NotBeNull();
            editorFile.Content.Should().Be(expectedContent);
        }
    }
}