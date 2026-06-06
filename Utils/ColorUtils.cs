using System.Numerics;
using ColorSpaceComparisons.Core.ColorTypes;

namespace ColorSpaceComparisons.Utils;

public static class ColorUtils
{
    public static Vector3 CalculatePrimaryWeights(
        CIExy whitePoint,
        CIExy primary1,
        CIExy primary2,
        CIExy primary3
    )
    {
        var whiteXYZ = whitePoint.ToCIExyY(1f).ToCIEXYZ();
        var redBright = primary1.ToCIExyY(1f).ToCIEXYZ();
        var greenBright = primary2.ToCIExyY(1f).ToCIEXYZ();
        var blueBright = primary3.ToCIExyY(1f).ToCIEXYZ();

        var systemMatrix = Matrix4x4.Identity;
        (systemMatrix.M11, systemMatrix.M12, systemMatrix.M13) = redBright;
        (systemMatrix.M21, systemMatrix.M22, systemMatrix.M23) = greenBright;
        (systemMatrix.M31, systemMatrix.M32, systemMatrix.M33) = blueBright;

        if (!Matrix4x4.Invert(systemMatrix, out systemMatrix))
        {
            throw new Exception("Singular matrix.");
        }

        return Vector3.Transform(whiteXYZ.ToVector3(), systemMatrix);
    }
}
