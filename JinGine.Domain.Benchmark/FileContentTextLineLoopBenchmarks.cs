using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Microsoft.Extensions.Primitives;

namespace JinGine.Domain.Benchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class FileContentTextLineLoopBenchmarks
{
    private static readonly StringSegment[] StringSegments = new StringSegment[500];
    private static readonly ArraySegment<char>[] ArraySegments = new ArraySegment<char>[500];

    [Benchmark]
    public void IterateStringSegments_For() => IterateStringSegments_For(StringSegments);

    [Benchmark]
    public void IterateStringSegments_ForEach() => IterateStringSegments_ForEach(StringSegments);

    [Benchmark]
    public void IterateArraySegments_For() => IterateArraySegments_For(ArraySegments);

    [Benchmark]
    public void IterateArraySegments_ForEach() => IterateArraySegments_ForEach(ArraySegments);

    private static void IterateStringSegments_For(StringSegment[] items)
    {
        for (var i = 0; i < items.Length; i++) _ = items[i];
    }

    private static void IterateStringSegments_ForEach(StringSegment[] items)
    {
        foreach (var i in items) { var _ = i; }
    }

    private static void IterateArraySegments_For(ArraySegment<char>[] items)
    {
        for (var i = 0; i < items.Length; i++) _ = items[i];
    }

    private static void IterateArraySegments_ForEach(ArraySegment<char>[] items)
    {
        foreach (var i in items) { var _ = i; }
    }
}