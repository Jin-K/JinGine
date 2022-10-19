namespace JinGine.Core.Models;

/// <summary>
/// Represents offset and cartesian coordinates in an editor text.
/// </summary>
/// <remarks>Have a look at <see cref="EditorText"/> who creates instances of this.</remarks>
/// <param name="TextOffset">Offset in the text.</param>
/// <param name="ColumnIndex">0-based column index.</param>
/// <param name="LineIndex">0-based line index.</param>
internal readonly record struct EditorCaret(int TextOffset, int ColumnIndex, int LineIndex)
{
    internal static readonly EditorCaret Origin = default;
}