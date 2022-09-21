// ReSharper disable once CheckNamespace
namespace System.IO;

public static class StreamExtensions
{
    /// <summary>
    /// Sets a stream length equal to the current stream position
    /// </summary>
    /// <param name="stream"></param>
    internal static void Truncate(this Stream stream)
    {
        stream.SetLength(stream.Position);
    }
}