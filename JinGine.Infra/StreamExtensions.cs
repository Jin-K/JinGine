using System.IO;
using System.Text;

namespace JinGine.Infra;

internal static class StreamExtensions
{
    internal static string ReadTextToEnd(this Stream @this, Encoding? encoding = null)
    {
        encoding ??= Encoding.Default;
        using var streamReader = new StreamReader(@this, encoding);
        return streamReader.ReadToEnd();
    }
}