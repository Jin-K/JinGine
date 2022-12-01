using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using JinGine.App.Mappers;

namespace JinGine.App.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
[SuppressMessage("Performance", "CA1822:Mark members as static")]
public class EditorTextLinesMapperBenchmarks
{
    private static readonly char[] TextChars = TextSample.Sample.ToCharArray();

    [Benchmark]
    public void CreateLinesWithOldMapper() => OldEditorTextLinesMapper.Map(TextChars);

    [Benchmark]
    public void CreateLinesWithNewMapper() => EditorTextLinesMapper.Map(TextChars);
}

file static class OldEditorTextLinesMapper
{
    public static ArraySegment<char>[] Map(ArraySegment<char> charsSegment)
    {
        var length = charsSegment.Count;
        ReadOnlySpan<char> charsSpan = charsSegment;
        var res = new List<ArraySegment<char>>();

        for (var pos = 0; pos <= length;)
        {
            var indexOfLineTerminator = charsSpan.IndexOfAny('\r', '\n');
            if (indexOfLineTerminator is not -1)
            {
                var segmentLength = indexOfLineTerminator;

                if (charsSpan[indexOfLineTerminator] is '\r' &&
                    indexOfLineTerminator + 1 < length &&
                    charsSpan[indexOfLineTerminator + 1] is '\n')
                    indexOfLineTerminator++;

                res.Add(charsSegment.Slice(pos, segmentLength));

                var deltaPos = indexOfLineTerminator + 1;
                pos += deltaPos;
                charsSpan = charsSpan[deltaPos..];
            }
            else
            {
                res.Add(charsSegment.Slice(pos, length - pos));
                break;
            }
        }

        return res.ToArray();
    }
}