using System.Text;

// ReSharper disable once CheckNamespace
namespace System.IO;

internal static class StreamExtensions
{
    internal static string ReadTextToEnd(this Stream @this, Encoding? encoding = null)
    {
        encoding ??= Encoding.Default;
        using var streamReader = new StreamReader(@this, encoding);
        return streamReader.ReadToEnd();
    }
}