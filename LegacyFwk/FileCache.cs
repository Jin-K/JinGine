﻿using System.Text;

namespace LegacyFwk;

/// <summary>
/// Represents a cached file.
/// </summary>
internal class FileCache
{
    private const int CacheSize = 4;

    private readonly string _fileName;
    private readonly string _textContent;
    private readonly Encoding? _encoding;

    private static readonly FileCache?[] CachedFiles = new FileCache[CacheSize];

    /// <summary>
    /// Creates an instance of the <see cref="FileCache"/> class.
    /// </summary>
    /// <param name="fileName">File name path.</param>
    /// <param name="textContent">Text content of the file.</param>
    /// <param name="encoding">Text encoding for file reading/writing.</param>
    private FileCache(string fileName, string? textContent = null, Encoding? encoding = null)
    {
        _fileName = fileName;
        _textContent = textContent ?? string.Empty;
        _encoding = encoding;
    }

    /// <summary>
    /// Search the cached file for the given file name.
    /// </summary>
    /// <param name="fileName">File name path.</param>
    /// <returns>An instance of <see cref="FileCache"/> or <see langword="null"/> if the file is not cached.</returns>
    /// <exception cref="ArgumentException">Can't handle unexpanded file names.</exception>
    internal static FileCache? TryGetCachedFile(string fileName)
    {
        if (fileName[0] is '$')
            throw new ArgumentException("Can\'t handle unexpanded file names.", nameof(fileName));

        return CachedFiles.FirstOrDefault(cachedFile =>
            cachedFile is not null &&
            cachedFile._fileName.Length == fileName.Length &&
            string.Compare(
                fileName,
                0,
                cachedFile._fileName,
                0,
                fileName.Length,
                StringComparison.OrdinalIgnoreCase) is 0);
    }

    /// <summary>
    /// Gets the content of a text file.
    /// </summary>
    /// <remarks>
    /// Tries to find the cached version first, otherwise, gets the disk version.
    /// </remarks>
    /// <param name="fileName">File name path.</param>
    /// <param name="encoding">Text encoding for file reading.</param>
    /// <returns></returns>
    internal static string GetTextContent(string fileName, Encoding? encoding = null)
    {
        var cachedFile = TryGetCachedFile(fileName);
        if (cachedFile is not null) return cachedFile._textContent;
        using var fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return fileStream.ReadTextToEnd(encoding);
    }
}