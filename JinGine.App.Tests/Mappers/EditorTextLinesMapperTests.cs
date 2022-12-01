using System.Linq;
using FluentAssertions;
using JinGine.App.Mappers;
using Xunit;

namespace JinGine.App.Tests.Mappers;

public class EditorTextLinesMapperTests
{
    [Fact]
    public void Returns_always_at_least_1_line()
    {
        // Arrange
        var chars = string.Empty.ToCharArray();

        // Act
        var lines = EditorTextLinesMapper.Map(chars);

        // Assert
        lines.Should().HaveCount(1);
    }

    [Theory]
    [MemberData(nameof(TextWithExpectedNumberOfLines))]
    public void Returns_always_1_more_line_than_number_of_line_terminators(
        string text,
        int expectedLinesCount,
        string? because)
    {
        // Arrange
        var chars = text.ToCharArray();

        // Act
        var lines = EditorTextLinesMapper.Map(chars);

        // Assert
        lines.Should().HaveCount(expectedLinesCount, because);
    }

    [Theory]
    [InlineData("Line1\nLine2\r\nLine3")]
    [InlineData("Line1\rLine2\r\nLine3\nLine4\n")]
    public void Returned_lines_should_be_separated_by_line_terminators_in_text(string text)
    {
        // Arrange
        var chars = text.ToCharArray();

        // Act
        var lines = EditorTextLinesMapper.Map(chars);

        // Assert
        lines.SkipLast(1).Should().AllSatisfy(line =>
            text[line.Offset + line.Count].Should().Match<char>(c => c == '\r' || c == '\n'));
    }


    public static TheoryData<string, int, string?> TextWithExpectedNumberOfLines => new()
    {
        { "\r\n\r\n\r\r\n\n", 6, "1 default begin line + 1 CRLF + 1 CRLF + 1 CR + 1 CRLF + 1 LF" },
        { "Line1\rLine2", 2, null },
        { "Line1\nLine2\r\nLine3", 3, null },
        { "Line1\nLine2\nLine3\n ", 4, null },
    };
}