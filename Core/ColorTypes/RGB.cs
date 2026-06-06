using System.Numerics;

namespace ColorSpaceComparisons.Core.ColorTypes;

public record struct RGB(float R, float G, float B)
{
    public readonly Vector3 ToVector3() => new(R, G, B);

    public readonly RGB Pow(float power) =>
        new(MathF.Pow(R, power), MathF.Pow(G, power), MathF.Pow(B, power));

    public readonly RGB Bright(float brightness = 1f)
    {
        var result = this;

        var maxLevel = Math.Max(Math.Max(R, G), B);
        if (maxLevel > 0f)
        {
            var mult = brightness / maxLevel;
            result = (result.ToVector3() * mult).ToRGB();
        }

        return result;
    }
}

public static class RGBExtensions
{
    public static RGB ToRGB(this Vector3 vector) => new(vector.X, vector.Y, vector.Z);
}
