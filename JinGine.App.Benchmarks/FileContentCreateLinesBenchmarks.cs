using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using static JinGine.App.Benchmarks.TextSample;
// ReSharper disable UnusedMethodReturnValue.Local

namespace JinGine.App.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Needs to be non-static for BenchmarkDotNet")]
public class FileContentCreateLinesBenchmarks
{
    private static readonly char[] TextChars = Sample.ToCharArray();

    [Benchmark]
    public void CreateLinesWithForAndCounters() => CreateLinesWithForAndCounters(TextChars);

    [Benchmark]
    public void CreateLinesWithForAndIndexOfAnyOnSpan() => CreateLinesWithForAndIndexOfAnyOnSpan(TextChars);

    [Benchmark]
    public void CreateLinesWithDoWhileAndIndexOfAnyOnSpan() =>
        CreateLinesWithDoWhileAndIndexOfAnyOnSpan(TextChars);
    
    private static ArraySegment<char>[] CreateLinesWithForAndCounters(char[] textChars)
    {
        var res = new List<ArraySegment<char>>();
        var lineOffsetInText = 0;
        var lineLength = 0;
        var length = textChars.Length;

        for (var i = 0; i < length; i++)
        {
            var c = textChars[i];

            lineLength++;

            if (c == '\r' && i + 1 < length && textChars[i + 1] == '\n')
            {
                lineLength++;
                i++;
            }

            if (c is '\r' or '\n')
            {
                res.Add(new ArraySegment<char>(textChars, lineOffsetInText, lineLength));
                lineOffsetInText += lineLength;
                lineLength = 0;
            }
        }

        if (lineLength is 0 || lineOffsetInText != length)
            res.Add(new ArraySegment<char>(textChars, lineOffsetInText, length - lineOffsetInText));

        return res.ToArray();
    }
    
    private static ArraySegment<char>[] CreateLinesWithForAndIndexOfAnyOnSpan(char[] textChars)
    {
        var res = new List<ArraySegment<char>>();
        var textCharsAsSegment = new ArraySegment<char>(textChars);
        var textCharsAsSpan = new ReadOnlySpan<char>(textChars);
        var length = textChars.Length;

        for (var pos = 0; pos <= length;)
        {
            var indexOfLineTerminator = textCharsAsSpan.IndexOfAny('\r', '\n');
            if (indexOfLineTerminator is not -1)
            {
                if (textCharsAsSpan[indexOfLineTerminator] is '\r' &&
                    indexOfLineTerminator + 1 < textCharsAsSpan.Length &&
                    textCharsAsSpan[indexOfLineTerminator + 1] is '\n')
                    indexOfLineTerminator++;

                var segmentLength = indexOfLineTerminator + 1;
                var segment = textCharsAsSegment.Slice(pos, segmentLength);
                res.Add(segment);
                pos += segmentLength;
                textCharsAsSpan = textCharsAsSpan.Slice(segmentLength);
            }
            else
            {
                var segment = textCharsAsSegment.Slice(pos);
                res.Add(segment);
                break;
            }
        }

        return res.ToArray();
    }
    
    private static ArraySegment<char>[] CreateLinesWithDoWhileAndIndexOfAnyOnSpan(char[] textChars)
    {
        var res = new List<ArraySegment<char>>();
        var textCharsAsSegment = new ArraySegment<char>(textChars);
        var textCharsAsSpan = new ReadOnlySpan<char>(textChars);
        var length = textChars.Length;
        var pos = 0;

        do
        {
            var indexOfLineTerminator = textCharsAsSpan.IndexOfAny('\r', '\n');
            if (indexOfLineTerminator is not -1)
            {
                if (textCharsAsSpan[indexOfLineTerminator] is '\r' &&
                    indexOfLineTerminator + 1 < textCharsAsSpan.Length &&
                    textCharsAsSpan[indexOfLineTerminator + 1] is '\n')
                    indexOfLineTerminator++;

                var segmentLength = indexOfLineTerminator + 1;
                var segment = textCharsAsSegment.Slice(pos, segmentLength);
                res.Add(segment);
                pos += segmentLength;
                textCharsAsSpan = textCharsAsSpan.Slice(segmentLength);
            }
            else
            {
                var segment = textCharsAsSegment.Slice(pos);
                res.Add(segment);
                break;
            }
        } while (pos <= length);

        return res.ToArray();
    }
}