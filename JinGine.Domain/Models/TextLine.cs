using Microsoft.Extensions.Primitives;

namespace JinGine.Domain.Models;

// value object
public readonly record struct TextLine
{
    private readonly StringSegment _segment;

    public TextLine(StringSegment segment) => _segment = segment;

    public int OffsetInText => _segment.Offset;

    public int Length => _segment.Length;

    public override string ToString() => _segment.ToString();

    public static implicit operator TextLine(StringSegment segment) => new(segment);
}