using System.Numerics;
using ColorSpaceComparisons.Core.ColorTypes;
using ColorSpaceComparisons.Utils;

namespace ColorSpaceComparisons.Core.ColorSpaceConverters;

public class ClippingCIExyColorSpaceConverter : IColorSpaceConverter
{
    private readonly Matrix4x4 _sourceRGBToXYZ;
    private readonly Matrix4x4 _destinationXYZToRGB;
    private readonly Vector3 _destinationLuminanceWeightsRGB;

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
        _destinationLuminanceWeightsRGB = destinationColorSpace.LuminanceWeightsRGB;

        // Use chromatically adapted white point and primaries
        _destinationWhite = destinationColorSpace.AdaptedWhitePoint;
        _destinationRed = destinationColorSpace.RedXYZ.ToCIExyY().ToCIExy();
        _destinationGreen = destinationColorSpace.GreenXYZ.ToCIExyY().ToCIExy();
        _destinationBlue = destinationColorSpace.BlueXYZ.ToCIExyY().ToCIExy();
    }

    public RGB Convert(RGB sourceLinearRGB)
    {
        var sourceXyz = Vector3
            .Transform(sourceLinearRGB.ToVector3(), _sourceRGBToXYZ)
            .ToCIEXYZ();
        var rgb = Vector3.Transform(sourceXyz.ToVector3(), _destinationXYZToRGB).ToRGB();
        if (rgb.R >= 0f && rgb.G >= 0f && rgb.B >= 0f)
        {
            return rgb;
        }

        return Convert(sourceXyz, _destinationWhite);
    }

    public RGB Convert(
        CIEXYZ sourceXYZ,
        CIExy whitePoint,
        bool preservePurpleSaturation = false
    )
    {
        if (sourceXYZ.Y <= 0f)
        {
            return new(0f, 0f, 0f);
        }

        var source = sourceXYZ.ToCIExyY();

        var rayOrigin = whitePoint.ToVector2();
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
            )
        )
        {
            if (preservePurpleSaturation)
            {
                var rgb = Vector3.Max(
                    Vector3.Transform(sourceXYZ.ToVector3(), _destinationXYZToRGB),
                    Vector3.Zero
                );
                var luminance = Vector3.Dot(rgb, _destinationLuminanceWeightsRGB);
                rgb *= sourceXYZ.Y / luminance;
                return rgb.ToRGB();
            }

            if (
                !GeometryUtils.RayLineSegmentIntersection(
                    rayOrigin,
                    rayDirection,
                    _destinationBlue.ToVector2(),
                    _destinationRed.ToVector2(),
                    out destination
                )
            )
            {
                throw new Exception("Unable to convert color.");
            }
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
