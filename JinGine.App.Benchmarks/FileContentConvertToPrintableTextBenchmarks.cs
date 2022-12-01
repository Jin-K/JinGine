using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using static JinGine.App.Benchmarks.TextSample;

namespace JinGine.App.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Needs to be non-static for BenchmarkDotNet")]
public class FileContentConvertToPrintableTextBenchmarks
{
    private static readonly string Text = Sample;
    private static readonly char[] TextChars = Sample.ToCharArray();

    [Benchmark]
    public void CopyStringToPrintableString() => CopyStringToPrintableString(Text);

    [Benchmark]
    public void CopyCharArrayToPrintableCharArray() => CopyCharArrayToPrintableCharArray(TextChars);

    [Benchmark]
    public void CopyCharArrayToPrintableCharArrayFromArrayPool() =>
        CopyCharArrayToPrintableCharArrayFromArrayPool(TextChars);
    
    private static void CopyStringToPrintableString(string text) =>
        string.Create(text.Length, text, (span, s) => CopyOrReplaceChars(s, span));
    
    private static void CopyCharArrayToPrintableCharArray(char[] textChars)
    {
        var printableChars = new char[textChars.Length];
        CopyOrReplaceChars(textChars, printableChars);
    }
    
    private static void CopyCharArrayToPrintableCharArrayFromArrayPool(char[] textChars)
    {
        var printableChars = ArrayPool<char>.Shared.Rent(textChars.Length);
        CopyOrReplaceChars(textChars, printableChars);
        //ArrayPool<char>.Shared.Return(printableChars);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CopyOrReplaceChars(ReadOnlySpan<char> srcSpan, Span<char> destSpan)
    {
        srcSpan.CopyTo(destSpan);
        var length = srcSpan.Length;
        for (var i = 0; i < length; i++)
        {
            var ch = srcSpan[i];
            switch (ch)
            {
                case (char)0: destSpan[i] = ','; continue;
                case '\t': destSpan[i] = '·'; continue;
                case < ' ': destSpan[i] = '…'; continue;
                case < (char)0x7f: continue;
                case < (char)0xa1: destSpan[i] = '¡'; continue;
            }
        }
    }
}