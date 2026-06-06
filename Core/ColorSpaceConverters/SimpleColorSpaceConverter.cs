using System.Numerics;
using ColorSpaceComparisons.Core.ColorTypes;

namespace ColorSpaceComparisons.Core.ColorSpaceConverters;

public class SimpleColorSpaceConverter : IColorSpaceConverter
{
    public Matrix4x4 TransformationMatrix { get; private init; }

    public SimpleColorSpaceConverter(
        ColorSpace sourceColorSpace,
        ColorSpace destinationColorSpace
    )
    {
        TransformationMatrix =
            sourceColorSpace.LinearRGBToXYZMatrix
            * destinationColorSpace.XYZToLinearRGBMatrix;
    }

    public RGB Convert(RGB sourceLinearRGB) =>
        Vector3.Transform(sourceLinearRGB.ToVector3(), TransformationMatrix).ToRGB();
}
