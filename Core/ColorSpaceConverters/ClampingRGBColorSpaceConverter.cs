using System.Numerics;
using ColorSpaceComparisons.Core.ColorTypes;

namespace ColorSpaceComparisons.Core.ColorSpaceConverters;

public class ClampingRGBColorSpaceConverter : IColorSpaceConverter
{
    public Matrix4x4 TransformationMatrix { get; private init; }

    private readonly Vector3 _sourceLuminanceWeights;
    private readonly Vector3 _destinationLuminanceWeights;

    public ClampingRGBColorSpaceConverter(
        ColorSpace sourceColorSpace,
        ColorSpace destinationColorSpace
    )
    {
        TransformationMatrix =
            sourceColorSpace.LinearRGBToXYZMatrix
            * destinationColorSpace.XYZToLinearRGBMatrix;
        _sourceLuminanceWeights = new(
            sourceColorSpace.RedXYZ.Y,
            sourceColorSpace.GreenXYZ.Y,
            sourceColorSpace.BlueXYZ.Y
        );
        _destinationLuminanceWeights = new(
            destinationColorSpace.RedXYZ.Y,
            destinationColorSpace.GreenXYZ.Y,
            destinationColorSpace.BlueXYZ.Y
        );
    }

    public RGB Convert(RGB sourceLinearRGB)
    {
        var source = sourceLinearRGB.ToVector3();
        var clamped = Vector3.Max(
            Vector3.Transform(source, TransformationMatrix),
            Vector3.Zero
        );

        var luminance = Vector3.Dot(_destinationLuminanceWeights, clamped);
        if (luminance > 0f)
        {
            var sourceLuminance = Vector3.Dot(_sourceLuminanceWeights, source);
            clamped *= sourceLuminance / luminance;
        }

        return clamped.ToRGB();
    }
}
