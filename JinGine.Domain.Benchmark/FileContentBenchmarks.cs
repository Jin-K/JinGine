using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Microsoft.Extensions.Primitives;

namespace JinGine.Domain.Benchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class FileContentBenchmarks
{
    private static readonly string TextContent = CreateTextContent();
    private static readonly char[] TextContentChars = TextContent.ToCharArray();
    private static readonly char[] LineTerminators = { '\r', '\n' };

    [Benchmark]
    public void CreateArraySegmentsFromCharArray() => CreateArraySegmentsFromCharArray(TextContentChars);

    [Benchmark]
    public void CreateArraySegmentsFromCharArrayAndSpan() => CreateArraySegmentsFromCharArrayAndSpan(TextContentChars, TextContentChars);

    [Benchmark]
    public void CreateStringSegmentsFromString() => CreateStringSegmentsFromString(TextContent, LineTerminators);

    [Benchmark]
    public void CreateStringSegmentsFromSpan() => CreateStringSegmentsFromSpan(TextContent, TextContent);

    // ReSharper disable once UnusedMethodReturnValue.Local
    private static ArraySegment<char>[] CreateArraySegmentsFromCharArray(char[] textChars)
    {
        var res = new List<ArraySegment<char>>();
        var testCharsAsSegment = new ArraySegment<char>(textChars);

        var lineOffsetInText = 0;
        while (lineOffsetInText <= textChars.Length)
        {
            var indexOfCr = Array.IndexOf(textChars, '\r', lineOffsetInText);
            var indexOfLf = Array.IndexOf(textChars, '\n', lineOffsetInText);
            if (indexOfCr is not -1)
            {
                var segmentLength = indexOfCr - lineOffsetInText + 1;
                if (indexOfLf == indexOfCr + 1) segmentLength++;
                res.Add(testCharsAsSegment.Slice(lineOffsetInText, segmentLength));
                lineOffsetInText += segmentLength;
            }
            else if (indexOfLf is not -1)
            {
                var segmentLength = indexOfLf - lineOffsetInText + 1;
                res.Add(testCharsAsSegment.Slice(lineOffsetInText, segmentLength));
                lineOffsetInText += segmentLength;
            }
            else
            {
                var segment = testCharsAsSegment[lineOffsetInText..];
                res.Add(segment);
                break;
            }
        }

        return res.ToArray();
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private static ArraySegment<char>[] CreateArraySegmentsFromCharArrayAndSpan(
        char[] textChars,
        ReadOnlySpan<char> textSpan)
    {
        var res = new List<ArraySegment<char>>();
        var testCharsAsSegment = new ArraySegment<char>(textChars);

        var lineOffsetInText = 0;
        while (lineOffsetInText <= textChars.Length)
        {
            var indexOfLineTerminator = textSpan[lineOffsetInText..].IndexOfAny('\r', '\n');
            if (indexOfLineTerminator is not -1)
            {
                indexOfLineTerminator += lineOffsetInText;
                var segmentStart = lineOffsetInText;
                var segmentLength = indexOfLineTerminator - lineOffsetInText + 1;
                lineOffsetInText = indexOfLineTerminator + 1;
                if (textSpan[indexOfLineTerminator] is '\r' &&
                    indexOfLineTerminator + 1 < textSpan.Length &&
                    textSpan[indexOfLineTerminator + 1] is '\n')
                {
                    lineOffsetInText++;
                    segmentLength++;
                }

                var segment = testCharsAsSegment.Slice(segmentStart, segmentLength);
                res.Add(segment);
            }
            else
            {
                var segment = testCharsAsSegment[lineOffsetInText..];
                res.Add(segment);
                break;
            }
        }

        return res.ToArray();
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private static StringSegment[] CreateStringSegmentsFromString(string text, char[] lineTerminators)
    {
        var res = new List<StringSegment>();
        StringSegment textAsSegment = text;

        var lineOffsetInText = 0;
        while (lineOffsetInText <= text.Length)
        {
            var indexOfLineTerminator = text.IndexOfAny(lineTerminators, lineOffsetInText);
            if (indexOfLineTerminator is not -1)
            {
                var segmentStart = lineOffsetInText;
                var segmentLength = indexOfLineTerminator - lineOffsetInText + 1;
                lineOffsetInText = indexOfLineTerminator + 1;
                if (text[indexOfLineTerminator] is '\r' &&
                    indexOfLineTerminator + 1 < text.Length &&
                    text[indexOfLineTerminator + 1] is '\n')
                {
                    lineOffsetInText++;
                    segmentLength++;
                }
                var segment = textAsSegment.Subsegment(segmentStart, segmentLength);
                res.Add(segment);
            }
            else
            {
                var segment = textAsSegment.Subsegment(lineOffsetInText, text.Length - lineOffsetInText);
                res.Add(segment);
                break;
            }
        }

        return res.ToArray();
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private static StringSegment[] CreateStringSegmentsFromSpan(string text, ReadOnlySpan<char> textSpan)
    {
        var res = new List<StringSegment>();
        var textAsSegment = new StringSegment(text);

        var lineOffsetInText = 0;
        while (lineOffsetInText <= textSpan.Length)
        {
            var indexOfLineTerminator = textSpan[lineOffsetInText..].IndexOfAny('\r', '\n');
            if (indexOfLineTerminator is not -1)
            {
                indexOfLineTerminator += lineOffsetInText;
                var segmentStart = lineOffsetInText;
                var segmentLength = indexOfLineTerminator - lineOffsetInText + 1;
                lineOffsetInText = indexOfLineTerminator + 1;
                if (textSpan[indexOfLineTerminator] is '\r' &&
                    indexOfLineTerminator + 1 < textSpan.Length &&
                    textSpan[indexOfLineTerminator + 1] is '\n')
                {
                    lineOffsetInText++;
                    segmentLength++;
                }

                var segment = textAsSegment.Subsegment(segmentStart, segmentLength);
                res.Add(segment);
            }
            else
            {
                var segment = textAsSegment.Subsegment(lineOffsetInText, textSpan.Length - lineOffsetInText);
                res.Add(segment);
                break;
            }
        }

        return res.ToArray();
    }

    private static string CreateTextContent() =>
        @"
<xs:schema xmlns="""" xmlns:xs=""http://www.w3.org/2001/XMLSchema"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
  <xs:element name=""Table1"">
    <xs:complexType>
      <xs:sequence>
        <xs:element name=""ColumnA"" type=""xs:string"" msdata:targetNamespace="""" minOccurs=""0"" />
        <xs:element name=""ColumnB"" type=""xs:string"" msdata:targetNamespace="""" minOccurs=""0"" />
        <xs:element name=""ColumnC"" type=""xs:string"" msdata:targetNamespace="""" minOccurs=""0"" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name=""tmpDataSet"" msdata:IsDataSet=""true"" msdata:MainDataTable=""Table1"" msdata:UseCurrentLocale=""true"">
    <xs:complexType>
      <xs:choice minOccurs=""0"" maxOccurs=""unbounded"" />
    </xs:complexType>
  </xs:element>
  <tmpDataSet>
    <Table1 diffgr:id=""Table11"" msdata:rowOrder=""0"" diffgr:hasChanges=""inserted"">
      <ColumnA>ValueA1</ColumnA>
      <ColumnB>ValueB1</ColumnB>
      <ColumnC>ValueC1</ColumnC>
    </Table1>
    <Table1 diffgr:id=""Table12"" msdata:rowOrder=""1"" diffgr:hasChanges=""inserted"">
      <ColumnA>ValueA2</ColumnA>
      <ColumnB>ValueB2</ColumnB>
      <ColumnC>ValueC2</ColumnC>
    </Table1>
    <Table1 diffgr:id=""Table13"" msdata:rowOrder=""2"" diffgr:hasChanges=""inserted"">
      <ColumnA>ValueA3</ColumnA>
      <ColumnB>ValueB3</ColumnB>
      <ColumnC>ValueC3</ColumnC>
    </Table1>
  </tmpDataSet>
</diffgr:diffgram>";
}