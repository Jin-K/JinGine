using FluentAssertions;
using JinGine.Domain.Models;
using Xunit;

namespace JinGine.Domain.Tests.Entities
{
    public class EditorFileTests
    {
        [Fact]
        public void New_prepared_editor_file_should_have_no_content()
        {
            // Act
            var editorFile = EditorFile.PrepareNew();
            
            // Assert
            editorFile.Path.Should().BeNull();
            editorFile.Content.Should().BeEmpty();
        }

        [Fact]
        public void Editor_file_created_from_physical_file_with_content_should_have_path_and_content()
        {
            // Arrange
            var physicalFile = new PhysicalTextFile("c:\\aCoolFile.txt", "cool content");

            // Act
            var editorFile = EditorFile.CreateFrom(physicalFile); // TODO is it a bad practice passing a value object to an entity constructor/factory method ?

            // Assert
            editorFile.Path.Should().NotBeNull();
            editorFile.Content.Should().Be(physicalFile.Content);
        }
    }
}