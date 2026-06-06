using System.Numerics;

namespace ColorSpaceComparisons.Core.ColorTypes;

public record struct Oklab(float L, float a, float b)
{
    // csharpier-ignore
    private static readonly Matrix4x4 _xyzToLmsMatrix = new(
        0.8189330101f, 0.0329845436f, 0.0482003018f, 0f,
        0.3618667424f, 0.9293118715f, 0.2643662691f, 0f,
        -0.1288597137f, 0.0361456387f, 0.6338517070f, 0f,
        0f, 0f, 0f, 1f
    );

    // csharpier-ignore
    private static readonly Matrix4x4 _lmsToLabMatrix = new(
        0.2104542553f, 1.9779984951f, 0.0259040371f, 0f,
        0.7936177850f, -2.4285922050f, 0.7827717662f, 0f,
        -0.0040720468f, 0.4505937099f, -0.8086757660f, 0f,
        0f, 0f, 0f, 1f
    );

    // csharpier-ignore
    private static readonly Matrix4x4 _labToLmsMatrix = new(
        0.9999999985f, 1.0000000089f, 1.0000000547f, 0f,
        0.3963377922f, -0.1055613423f, -0.0894841821f, 0f,
        0.2158037581f, -0.0638541748f, -1.2914855379f, 0f,
        0f, 0f, 0f, 1f
    );

    // csharpier-ignore
    private static readonly Matrix4x4 _lmsToXyzMatrix =
        new(
            1.2270138511f,
            -0.0405801784f,
            -0.0763812845f,
            0f,
            -0.5577999807f,
            1.1122568696f,
            -0.4214819784f,
            0f,
            0.2812561490f,
            -0.0716766787f,
            1.5861632204f,
            0f,
            0f,
            0f,
            0f,
            1f
        );

    public static Oklab FromCIEXYZ_D65(CIEXYZ xyz)
    {
        var lms = Vector3.Transform(xyz.ToVector3(), _xyzToLmsMatrix);
        lms.X = MathF.Cbrt(lms.X);
        lms.Y = MathF.Cbrt(lms.Y);
        lms.Z = MathF.Cbrt(lms.Z);

        return Vector3.Transform(lms, _lmsToLabMatrix).ToOklab();
    }

    public readonly CIEXYZ ToCIEXYZ_D65()
    {
        var lms = Vector3.Transform(ToVector3(), _labToLmsMatrix);
        lms *= lms * lms;
        return Vector3.Transform(lms, _lmsToXyzMatrix).ToCIEXYZ();
    }

    public readonly Vector3 ToVector3() => new(L, a, b);
}

public static class OklabExtensions
{
    public static Oklab ToOklab(this CIEXYZ xyz) => Oklab.FromCIEXYZ_D65(xyz);

    public static Oklab ToOklab(this Vector3 vector) => new(vector.X, vector.Y, vector.Z);
}
