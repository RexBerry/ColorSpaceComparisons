using System.Numerics;
using ColorSpaceComparisons.Core.ColorTypes;
using ColorSpaceComparisons.Utils;

namespace ColorSpaceComparisons.Core.ColorSpaceConverters;

public class ClippingCIExyColorSpaceConverter : IColorSpaceConverter
{
    private readonly Matrix4x4 _sourceRGBToXYZ;
    private readonly Matrix4x4 _destinationXYZToRGB;

    private readonly CIExy _destinationWhite;
    private readonly CIExy _destinationRed;
    private readonly CIExy _destinationGreen;
    private readonly CIExy _destinationBlue;

    public ClippingCIExyColorSpaceConverter(
        ColorSpace sourceColorSpace,
        ColorSpace destinationColorSpace
    )
    {
        _sourceRGBToXYZ = sourceColorSpace.LinearRGBToXYZMatrix;
        _destinationXYZToRGB = destinationColorSpace.XYZToLinearRGBMatrix;

        // Use chromatically adapted white point and primaries
        _destinationWhite = destinationColorSpace.WhiteXYZ.ToCIExyY().ToCIExy();
        _destinationRed = destinationColorSpace.RedXYZ.ToCIExyY().ToCIExy();
        _destinationGreen = destinationColorSpace.GreenXYZ.ToCIExyY().ToCIExy();
        _destinationBlue = destinationColorSpace.BlueXYZ.ToCIExyY().ToCIExy();
    }

    public RGB Convert(RGB sourceLinearRGB)
    {
        var source = Vector3
            .Transform(sourceLinearRGB.ToVector3(), _sourceRGBToXYZ)
            .ToCIEXYZ()
            .ToCIExyY();

        var rayOrigin = _destinationWhite.ToVector2();
        var rayDirection = source.ToCIExy().ToVector2() - rayOrigin;

        Vector2 destination;
        if (
            !(
                GeometryUtils.RayLineSegmentIntersection(
                    rayOrigin,
                    rayDirection,
                    _destinationRed.ToVector2(),
                    _destinationGreen.ToVector2(),
                    out destination
                )
                || GeometryUtils.RayLineSegmentIntersection(
                    rayOrigin,
                    rayDirection,
                    _destinationGreen.ToVector2(),
                    _destinationBlue.ToVector2(),
                    out destination
                )
                || GeometryUtils.RayLineSegmentIntersection(
                    rayOrigin,
                    rayDirection,
                    _destinationBlue.ToVector2(),
                    _destinationRed.ToVector2(),
                    out destination
                )
            )
        )
        {
            throw new Exception("Unable to convert color.");
        }

        // Clamping only needed if there is floating point error
        var clamped = Vector3.Max(
            Vector3.Transform(
                destination.ToCIExy().ToCIExyY(source.Y).ToCIEXYZ().ToVector3(),
                _destinationXYZToRGB
            ),
            Vector3.Zero
        );
        return clamped.ToRGB();
    }
}
