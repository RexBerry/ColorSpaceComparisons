using System.Numerics;

namespace ColorSpaceComparisons.Core.ColorTypes;

public record struct RGBA(float R, float G, float B, float A)
{
    public readonly Vector4 ToVector4() => new(R, G, B, A);

    public readonly RGB ToRGB() => new(R, G, B);
}

public static class RGBAExtensions
{
    public static RGBA ToRGBA(this Vector4 vector) =>
        new(vector.X, vector.Y, vector.Z, vector.W);

    public static RGBA ToRGBA(this RGB rgb, float alpha = 1f) =>
        new(rgb.R, rgb.G, rgb.B, alpha);
}
