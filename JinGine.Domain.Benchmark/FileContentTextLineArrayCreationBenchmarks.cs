using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Microsoft.Extensions.Primitives;

namespace JinGine.Domain.Benchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class FileContentTextLineArrayCreationBenchmarks
{
    private const int ArraySize = 10_000;

    /// <summary>
    /// Fastest with 100_000 elements
    /// Slowest with 10_000 elements
    /// </summary>
    [Benchmark]
    public void CreateStringSegmentsArray()
    {
        var segments = new StringSegment[ArraySize];
    }

    /// <summary>
    /// Slowest with 100_000 elements
    /// Fastest with 10_000 elements
    /// </summary>
    [Benchmark]
    public void CreateArraySegmentsArray()
    {
        var segments = new ArraySegment<char>[ArraySize];
    }
}