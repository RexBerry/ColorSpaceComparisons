using System.Numerics;

namespace ColorSpaceComparisons.Core.ColorTypes;

public record struct CIEXYZ(float X, float Y, float Z)
{
    public readonly CIExyY ToCIExyY()
    {
        var sum = X + Y + Z;
        return new(X / sum, Y / sum, Y);
    }

    public readonly Vector3 ToVector3() => new(X, Y, Z);
}

public static class CIEXYZExtensions
{
    public static CIEXYZ ToCIEXYZ(this Vector3 vector) =>
        new(vector.X, vector.Y, vector.Z);
}
