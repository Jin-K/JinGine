﻿using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Equivalency;
using JinGine.Domain.Models;
using Microsoft.Extensions.Primitives;
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
        var fileContent = new FileContent(content);

        // Assert
        fileContent.TextLines.Should().HaveCount(1);
    }

    [Fact]
    public void Adding_a_line_should_increase_number_of_lines_of_1()
    {
        // Arrange
        var fileContent = FileContent.Empty;

        // Act
        fileContent = fileContent.AddLine();

        // Assert
        fileContent.TextLines.Should().HaveCount(2);
    }

    [Fact]
    public void Adding_a_line_should_create_a_text_line_at_the_end_with_offset_in_text_plus_length_of_environment_new_line()
    {
        // Arrange
        var fileContent = FileContent.Empty;
        var expectedPositionInText = fileContent.TextLines[^1].Offset + Environment.NewLine.Length;

        // Act
        var updatedFileContent = fileContent.AddLine();

        // Assert
        var addedLine = updatedFileContent.TextLines[^1];
        fileContent.TextLines.Should().NotContainEquivalentOf(addedLine, SetTextLineEquivalencyOptions);
        addedLine.Offset.Should().Be(expectedPositionInText);
    }

    [Theory]
    [MemberData(nameof(TextContentWithNumberOfLinesData))]
    public void FileContent_text_content_with_line_terminators_should_have_expected_number_of_lines(
        string textContent,
        int expectedLinesCount,
        string because = "")
    {
        // Act
        var fileContent = new FileContent(textContent);

        // Assert
        fileContent.TextLines.Should().HaveCount(expectedLinesCount, because);
    }

    [Fact]
    public void Inserting_a_char_should_increase_text_content_length_of_1()
    {
        // Arrange
        var fileContent = FileContent.Empty;
        var expectedLength = fileContent.TextContent.Length + 1;

        // Act
        fileContent = fileContent.InsertChar(default, default, default);

        // Assert
        fileContent.TextContent.Should().HaveLength(expectedLength);
    }

    [Fact]
    public void Inserting_a_char_in_a_line_should_increase_that_line_length_of_1()
    {
        // Arrange
        var fileContent = new FileContent("Line1\r\nLine2\r\nLine3");
        var expectedLineLength = fileContent.TextLines[1].Length + 1;

        // Act
        fileContent = fileContent.InsertChar('c', 1, 1);

        // Assert
        fileContent.TextLines[1].Length.Should().Be(expectedLineLength, "we just inserted a char");
    }

    [Fact]
    public void Inserting_char_c_at_first_line_and_column_index_8_should_set_char_c_at_index_8_of_FileContent()
    {
        // Arrange
        var fileContent = new FileContent("A string larger than 8 characters.");

        // Act
        fileContent = fileContent.InsertChar('c', 0, 8);

        // Assert
        fileContent[8].Should().Be('c');
    }

    [Theory]
    [InlineData("Line1\nLine2\r\nLine3")]
    [InlineData("Line1\rLine2\r\nLine3\n")]
    public void Lines_followed_by_other_lines_should_have_a_line_terminator(string textContent)
    {
        // Act
        var fileContent = new FileContent(textContent);

        // Assert
        fileContent.TextLines.SkipLast(1).Should().AllSatisfy(textLine =>
            textLine.ToString().Should().Match(textLineContent =>
                textLineContent.EndsWith('\r') || textLineContent.EndsWith('\n')));
    }

    private static EquivalencyAssertionOptions<StringSegment> SetTextLineEquivalencyOptions(
        EquivalencyAssertionOptions<StringSegment> opts) => opts.Using(new FileTextLineComparer());

    public static TheoryData<string, int, string?> TextContentWithNumberOfLinesData => new()
    {
        { "\r\n\r\n\r\r\n\n", 6, "1 default begin line + 1 CRLF + 1 CRLF + 1 CR + 1 CRLF + 1 LF" },
        { "Line1\rLine2", 2, null },
        { "Line1\nLine2\r\nLine3", 3, null },
        { "Line1\nLine2\nLine3\n ", 4, null },
    };
}