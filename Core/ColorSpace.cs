using System.Numerics;
using ColorSpaceComparisons.Core.ColorTypes;
using ColorSpaceComparisons.Utils;

namespace ColorSpaceComparisons.Core;

public abstract class ColorSpace
{
    // Illuminant, white point, and primaries before chromatic adaptation
    public CIExy Illuminant { get; private set; }
    public CIExy WhitePoint { get; private init; }
    public CIExy RedPrimary { get; private init; }
    public CIExy GreenPrimary { get; private init; }
    public CIExy BluePrimary { get; private init; }

    // Illuminant, white point, and primaries after chromatic adaptation
    public CIEXYZ ActiveIlluminant { get; private set; }
    public CIEXYZ WhiteXYZ { get; private set; }
    public CIEXYZ RedXYZ { get; private set; }
    public CIEXYZ GreenXYZ { get; private set; }
    public CIEXYZ BlueXYZ { get; private set; }

    public Matrix4x4 LinearRGBToXYZMatrix { get; private set; }
    public Matrix4x4 XYZToLinearRGBMatrix { get; private set; }

    public ColorSpace(
        CIExy whitePoint,
        CIExy redPrimary,
        CIExy greenPrimary,
        CIExy bluePrimary
    )
        : this(whitePoint, whitePoint, redPrimary, greenPrimary, bluePrimary) { }

    public ColorSpace(
        CIExy illuminant,
        CIExy whitePoint,
        CIExy redPrimary,
        CIExy greenPrimary,
        CIExy bluePrimary
    )
    {
        Illuminant = illuminant;
        WhitePoint = whitePoint;
        RedPrimary = redPrimary;
        GreenPrimary = greenPrimary;
        BluePrimary = bluePrimary;

        AdaptToIlluminant(Illuminant);
    }

    public void AdaptToIlluminant(CIExy targetIlluminant)
    {
        ActiveIlluminant = targetIlluminant.ToCIExyY(1f).ToCIEXYZ();

        WhiteXYZ = WhitePoint.ToCIExyY(1f).ToCIEXYZ();

        var redBright = RedPrimary.ToCIExyY(1f).ToCIEXYZ();
        var greenBright = GreenPrimary.ToCIExyY(1f).ToCIEXYZ();
        var blueBright = BluePrimary.ToCIExyY(1f).ToCIEXYZ();

        var weights = ColorUtils.CalculatePrimaryWeights(
            WhitePoint,
            RedPrimary,
            GreenPrimary,
            BluePrimary
        );
        RedXYZ = (redBright.ToVector3() * weights.X).ToCIEXYZ();
        GreenXYZ = (greenBright.ToVector3() * weights.Y).ToCIEXYZ();
        BlueXYZ = (blueBright.ToVector3() * weights.Z).ToCIEXYZ();

        if (targetIlluminant != Illuminant)
        {
            var chromaticAdaptationMatrix =
                ChromaticAdaptation.CalculateChromaticAdaptationMatrix(
                    Illuminant,
                    targetIlluminant
                );

            WhiteXYZ = Vector3
                .Transform(WhiteXYZ.ToVector3(), chromaticAdaptationMatrix)
                .ToCIEXYZ();
            RedXYZ = Vector3
                .Transform(RedXYZ.ToVector3(), chromaticAdaptationMatrix)
                .ToCIEXYZ();
            GreenXYZ = Vector3
                .Transform(GreenXYZ.ToVector3(), chromaticAdaptationMatrix)
                .ToCIEXYZ();
            BlueXYZ = Vector3
                .Transform(BlueXYZ.ToVector3(), chromaticAdaptationMatrix)
                .ToCIEXYZ();
        }

        var rgbToXyzMatrix = Matrix4x4.Identity;
        (rgbToXyzMatrix.M11, rgbToXyzMatrix.M12, rgbToXyzMatrix.M13) = RedXYZ;
        (rgbToXyzMatrix.M21, rgbToXyzMatrix.M22, rgbToXyzMatrix.M23) = GreenXYZ;
        (rgbToXyzMatrix.M31, rgbToXyzMatrix.M32, rgbToXyzMatrix.M33) = BlueXYZ;
        if (!Matrix4x4.Invert(rgbToXyzMatrix, out var xyzToRgbMatrix))
        {
            throw new Exception("Singular matrix.");
        }

        LinearRGBToXYZMatrix = rgbToXyzMatrix;
        XYZToLinearRGBMatrix = xyzToRgbMatrix;
    }

    public CIEXYZ ConvertLinearRGBToXYZ(RGB linearRGB) =>
        Vector3.Transform(linearRGB.ToVector3(), LinearRGBToXYZMatrix).ToCIEXYZ();

    public RGB ConvertXYZToLinearRGB(CIEXYZ xyz) =>
        Vector3.Transform(xyz.ToVector3(), XYZToLinearRGBMatrix).ToRGB();

    public RGB ConvertXYZToLinearRGBClamped(CIEXYZ xyz) =>
        Vector3
            .Max(Vector3.Transform(xyz.ToVector3(), XYZToLinearRGBMatrix), Vector3.Zero)
            .ToRGB();

    public RGB ConvertSignalToLinearRGB(RGB linearRGB) =>
        new(EOTF(linearRGB.R), EOTF(linearRGB.G), EOTF(linearRGB.B));

    public RGB ConvertLinearToSignalRGB(RGB signalRGB) =>
        new(OETF(signalRGB.R), OETF(signalRGB.G), OETF(signalRGB.B));

    public abstract float EOTF(float signalValue);

    public abstract float OETF(float linearValue);

    public static bool ArePrimariesEqual(
        ColorSpace colorSpace1,
        ColorSpace colorSpace2
    ) =>
        colorSpace1.RedXYZ == colorSpace2.RedXYZ
        && colorSpace1.GreenXYZ == colorSpace2.GreenXYZ
        && colorSpace1.BlueXYZ == colorSpace2.BlueXYZ;
}
