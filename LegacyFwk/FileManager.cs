using System.Text;

namespace LegacyFwk;

/// <summary>
/// Providers a set of helper functions on top of <see cref="Path"/>,
/// <see cref="File"/> & <see cref="Directory" />
/// </summary>
public static class FileManager
{
    /// <summary>
    /// Expands a file/directory path in case it has a custom virtual directory prefix
    /// </summary>
    /// <param name="path">File or Directory path.</param>
    /// <returns>Expanded path.</returns>
    /// <exception cref="ArgumentException">path hasn't the expected format to be expanded.</exception>
    public static string ExpandPath(string path)
    {
        var length = path.Length;

        if (path.IndexOf('\n') is not -1)
        {
            var builder = new StringBuilder();

            foreach (var line in path.ToListOfLines())
            {
                builder.AppendLine(ExpandPath(line));
            }

            return builder.ToString();
        }

        if (length < 2 || path[0] is not '$') return path;

        if (length is 2 || path[2] is '\\')
        {
            switch (path[1])
            {
                case 'J': return VirtualDirectory.J + path[2..];
            }
        }

        throw new ArgumentException(nameof(path) + " hasn't the expected format to be expanded.");
    }

    public static bool IsUrl(string fileName)
    {
        return fileName.StartsWith("http://") || fileName.StartsWith("https://");
    }

    public static void AskCreateFileIfNotFound(string fileName)
    {
        if (FileExists(fileName) is false)
        {
            throw new NotImplementedException("TODO ?");
        }
    }

    private static bool FileExists(string fileName)
    {
        fileName = ExpandPath(fileName);
        return FileCache.TryGetCachedFile(fileName) is not null || File.Exists(fileName);
    }

    public static string GetTextContent(string fileName, Encoding? encoding = null)
    {
        fileName = ExpandPath(fileName);
        if (IsUrl(fileName)) throw new NotImplementedException("TODO ??");
        return FileCache.GetTextContent(fileName, encoding);
    }
}