namespace JinGine.Core.Models;

/// <summary>
/// Represents offset and cartesian coordinates in an editor text.
/// </summary>
/// <remarks>Have a look at <see cref="EditorText"/> who creates instances of this.</remarks>
/// <param name="TextOffset">Offset in the text.</param>
/// <param name="Column">0-based column index.</param>
/// <param name="Line">0-based line index.</param>
internal readonly record struct EditorCaret(int TextOffset, int Column, int Line)
{
    internal static readonly EditorCaret Origin = default;
}