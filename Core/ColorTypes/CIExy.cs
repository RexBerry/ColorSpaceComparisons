using System.Numerics;

namespace ColorSpaceComparisons.Core.ColorTypes;

public record struct CIExy(float x, float y)
{
    public readonly CIExyY ToCIExyY(float Y) => new(x, y, Y);

    public readonly Vector2 ToVector2() => new(x, y);
}

public static class CIExyExtensions
{
    public static CIExy ToCIExy(this Vector2 vector) => new(vector.X, vector.Y);
}
