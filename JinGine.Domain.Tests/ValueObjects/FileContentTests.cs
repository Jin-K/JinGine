using System;
using System.Linq;
using FluentAssertions;
using JinGine.Domain.Models;
using Xunit;

namespace JinGine.Domain.Tests.ValueObjects;

public class FileContentTests
{
    [Fact]
    public void FileContent_has_always_at_least_1_line()
    {
        // Arrange
        var content = string.Empty;

        // Act
        var fileContent = FileContent.CreateFromRawContent(content);

        // Assert
        fileContent.Should().HaveCount(1);
    }

    [Fact]
    public void Trying_to_create_a_FileContent_without_lines_should_throw_exception()
    {
        // Arrange
        var lines = Array.Empty<TextLine>();

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new FileContent(lines));
    }

    [Fact]
    public void Adding_a_line_should_increase_number_of_lines_of_1()
    {
        // Arrange
        var fileContent = FileContent.Empty;

        // Act
        fileContent = fileContent.AddLine();

        // Assert
        fileContent.Should().HaveCount(2);
    }

    [Fact]
    public void Adding_a_line_should_create_a_text_line_at_the_end_with_position_in_text_plus_2()
    {
        // Arrange
        var fileContent = FileContent.Empty;
        var expectedPositionInText = fileContent.First().OffsetInText + Environment.NewLine.Length;

        // Act
        fileContent = fileContent.AddLine();

        // Assert
        fileContent.Last().OffsetInText.Should().Be(expectedPositionInText);
    }

    [Theory]
    [InlineData("\r", 2)]
    [InlineData("\r\n\r\n\r\r\n\n", 6, "1 default begin line + 1 CRLF + 1 CRLF + 1 CR + 1 CRLF + 1 LF")]
    [InlineData("\n", 2)]
    [InlineData("Line1\rLine2", 2)]
    [InlineData("Line1\nLine2", 2)]
    [InlineData("Line1\r\nLine2", 2)]
    [InlineData("Line1\rLine2\rLine3", 3)]
    [InlineData("Line1\nLine2\r\nLine3", 3)]
    [InlineData("Line1\nLine2\r\nLine3\r", 4)]
    [InlineData("Line1\nLine2\nLine3\n ", 4)]
    public void FileContent_created_from_content_with_any_line_terminators_should_have_expected_number_of_lines(
        string content,
        int expectedLinesCount,
        string because = "")
    {
        // Act
        var fileContent = FileContent.CreateFromRawContent(content);

        // Assert
        fileContent.Should().HaveCount(expectedLinesCount, because);
    }

    //[Fact]
    //public void Inserting_a_char_in_a_text_line_should_increase_line_length_of_1()
    //{
    //    // Arrange
    //    var fileContent = new FileContent(string.Empty);
    //    var line = fileContent.TextLines.First();

    //    // Act
    //    line.

    //    // Assert
    //}
}