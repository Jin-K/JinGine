using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using static JinGine.Domain.Benchmark.TextContentSample;

namespace JinGine.Domain.Benchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
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

    // ReSharper disable once UnusedMethodReturnValue.Local
    private static string CopyStringToPrintableString(string text)
    {
        return string.Create(text.Length, text, ConvertToPrintableChars);
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private static char[] CopyCharArrayToPrintableCharArray(char[] textChars)
    {
        var length = textChars.Length;
        var printableChars = new char[length];
        var destSpan = printableChars.AsSpan();
        
        textChars.CopyTo(destSpan);

        for (var i = 0; i < length; i++)
        {
            var ch = textChars[i];
            switch (ch)
            {
                case (char)0: destSpan[i] = ','; continue;
                case '\t': destSpan[i] = '·'; continue;
                case < ' ': destSpan[i] = '…'; continue;
                case < (char)0x7f: continue;
                case < (char)0xa1: destSpan[i] = '¡'; continue;
            }
        }

        return printableChars;
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private static char[] CopyCharArrayToPrintableCharArrayFromArrayPool(char[] textChars)
    {
        var length = textChars.Length;
        var destChars = ArrayPool<char>.Shared.Rent(length);
        var destSpan = destChars.AsSpan();
        textChars.CopyTo(destSpan);

        for (var i = 0; i < length; i++)
        {
            var ch = textChars[i];
            switch (ch)
            {
                case (char)0: destSpan[i] = ','; continue;
                case '\t': destSpan[i] = '·'; continue;
                case < ' ': destSpan[i] = '…'; continue;
                case < (char)0x7f: continue;
                case < (char)0xa1: destSpan[i] = '¡'; continue;
            }
        }

        ArrayPool<char>.Shared.Return(destChars);

        return destChars;
    }

    private static void ConvertToPrintableChars(Span<char> destSpan, string src)
    {
        src.CopyTo(destSpan);
        var length = src.Length;
        for (var i = 0; i < length; i++)
        {
            var ch = src[i];
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