using System.Numerics;
using ColorSpaceComparisons.Utils;

namespace ColorSpaceComparisons.Core.ColorTypes;

public record struct Oklch(float L, float C, float h)
{
    public readonly Oklab ToOklab() => new(L, C * MathF.Sin(h), C * MathF.Cos(h));

    public readonly Vector3 ToVector3() => new(L, C, h);
}

public static class OklchExtensions
{
    public static Oklch ToOklch(this Oklab oklab) =>
        new(oklab.L, MathUtils.Hypot(oklab.a, oklab.b), MathF.Atan2(oklab.b, oklab.a));

    public static Oklch ToOklch(this Vector3 vector) => new(vector.X, vector.Y, vector.Z);
}
