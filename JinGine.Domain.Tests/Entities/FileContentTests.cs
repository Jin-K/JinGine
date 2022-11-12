using FluentAssertions;
using JinGine.Domain.Models;
using Xunit;

namespace JinGine.Domain.Tests.Entities;

public class FileContentTests
{
    [Fact]
    public void FileContent_has_always_at_least_1_line()
    {
        // Arrange
        var content = string.Empty;

        // Act
        var fileContent = new FileContent(content);

        // Assert
        fileContent.TextLines.Should().HaveCount(1);
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
        var fileContent = new FileContent(content);

        // Assert
        fileContent.TextLines.Should().HaveCount(expectedLinesCount, because);
    }
}