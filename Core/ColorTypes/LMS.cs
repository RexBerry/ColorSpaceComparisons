using System.Numerics;

namespace ColorSpaceComparisons.Core.ColorTypes;

public record struct LMS(float L, float M, float S)
{
    // Hunt matrix (normalized to D65)
    // csharpier-ignore
    private static readonly Matrix4x4 _xyzD65ToLmsD65Matrix = new(
        0.40024f, -0.22630f, 0.00000f, 0f,
        0.70760f, 1.16532f, 0.00000f, 0f,
        -0.08081f, 0.04570f, 0.91822f, 0f,
        0f, 0f, 0f, 1f
    );

    // csharpier-ignore
    private static readonly Matrix4x4 _lmsD65ToXyzD65Matrix = new(
        1.8599364f, 0.3611914f, 0.0000000f, 0f,
        -1.1293816f, 0.6388125f, 0.0000000f, 0f,
        0.2198974f, -0.0000064f, 1.0890636f, 0f,
        0f, 0f, 0f, 1f
    );

    public static LMS FromCIEXYZ_D65(CIEXYZ xyz) =>
        Vector3.Transform(xyz.ToVector3(), _xyzD65ToLmsD65Matrix).ToLMS();

    public readonly CIEXYZ ToCIEXYZ_D65() =>
        Vector3.Transform(ToVector3(), _lmsD65ToXyzD65Matrix).ToCIEXYZ();

    public readonly HSV ToHSV() => new RGB(L, M, S).Pow(1f / 3f).ToHSV();

    public readonly Vector3 ToVector3() => new(L, M, S);

    public readonly LMS Pow(float power) =>
        new(MathF.Pow(L, power), MathF.Pow(M, power), MathF.Pow(S, power));

    public readonly LMS Bright(float brightness = 1f)
    {
        var result = this;

        var maxLevel = Math.Max(Math.Max(L, M), S);
        if (maxLevel > 0f)
        {
            var mult = brightness / maxLevel;
            result = (result.ToVector3() * mult).ToLMS();
        }

        return result;
    }
}

public static class LMSExtensions
{
    public static LMS ToLMS_D65(this CIEXYZ xyz) => LMS.FromCIEXYZ_D65(xyz);

    public static LMS ToLMS(this HSV hsv)
    {
        var rgb = hsv.ToRGB().Pow(3f);
        return new(rgb.R, rgb.G, rgb.B);
    }

    public static LMS ToLMS(this Vector3 vector) => new(vector.X, vector.Y, vector.Z);
}
