using System;
using System.IO;

namespace JinGine.Infra;

/// <summary>
/// Virtual directories abstraction
/// </summary>
internal readonly struct VirtualDirectory
{
    /// <summary>
    /// The real directory path
    /// </summary>
    private readonly string _path;

    /// <summary>
    /// Files directory
    /// </summary>
    internal static readonly VirtualDirectory J = @"C:\Projects\JinGine\JinGine.Forms\Files";

    /// <summary>
    /// Creates an instance of <see cref="VirtualDirectory"/>.
    /// </summary>
    /// <param name="path">Real directory path.</param>
    /// <exception cref="ArgumentException"></exception>
    private VirtualDirectory(string path)
    {
        if (Directory.Exists(path) is not true)
        {
            throw new ArgumentException("Directory doesn't exist.");
        }

        _path = path;
    }
    
    /// <summary>
    /// Gets the virtual directory string representation.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => _path;

    /// <summary>
    /// Converts a <see cref="VirtualDirectory"/> to a <see cref="String"/> automatically.
    /// </summary>
    /// <param name="virtualDirectory">The virtual directory.</param>
    public static implicit operator string(VirtualDirectory virtualDirectory) => virtualDirectory.ToString();

    /// <summary>
    /// Converts a <see cref="String"/> to a <see cref="VirtualDirectory"/> automatically.
    /// </summary>
    /// <param name="path">The real directory path.</param>
    public static implicit operator VirtualDirectory(string path) => new(path);
}