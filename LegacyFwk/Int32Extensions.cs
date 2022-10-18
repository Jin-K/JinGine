// ReSharper disable once CheckNamespace
namespace System;

public static class Int32Extensions
{
    /// <summary>
    /// Returns an integer forced into a bounds range.
    /// </summary>
    /// <param name="this">The initial value.</param>
    /// <param name="min">The minimum boundary (limit).</param>
    /// <param name="max">The maximum boundary (limit).</param>
    /// <returns>The initial value if it fits in the range, otherwise, the closest boundary (min or max).</returns>
    /// <exception cref="ArgumentException">min should be smaller than or equal to max</exception>
    public static int Crop(this int @this, int min, int max)
    {
        if (min > max)
        {
            const string message = $"{nameof(min)} should be smaller than or equal to {nameof(max)}";
            throw new ArgumentException(message);
        }

        if (@this < min) return min;
        return @this > max ? max : @this;
    }
}