using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace JinGine.Domain.Models;

// value object
public readonly struct TextLines : IReadOnlyCollection<StringSegment>, IEquatable<TextLines>
{
    private readonly IList<StringSegment> _lines;

    public int Count => _lines.Count;

    public TextLines(IList<StringSegment> lines) : this() => _lines = lines;

    public IEnumerator<StringSegment> GetEnumerator() => _lines.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override bool Equals(object? obj) => obj is TextLines other && Equals(other);

    public bool Equals(TextLines other) => _lines.SequenceEqual(other);

    public override int GetHashCode() => _lines.GetHashCode();

    public static bool operator ==(TextLines left, TextLines right) => left.Equals(right);

    public static bool operator !=(TextLines left, TextLines right) => !(left == right);
}