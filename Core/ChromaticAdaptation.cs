using System.Numerics;
using ColorSpaceComparisons.Core.ColorTypes;

namespace ColorSpaceComparisons.Core;

public static class ChromaticAdaptation
{
    // csharpier-ignore
    private static readonly Matrix4x4 _bradfordMatrix = new(
        0.8951f, -0.7502f, 0.0389f, 0f,
        0.2664f, 1.7135f, -0.0685f, 0f,
        -0.1614f, 0.0367f, 1.0296f, 0f,
        0f, 0f, 0f, 1f
    );

    // csharpier-ignore
    private static readonly Matrix4x4 _inverseBradfordMatrix = new(
        0.9869929f, 0.4323053f, -0.0085287f, 0f,
        -0.1470543f, 0.5183603f, 0.0400428f, 0f,
        0.1599627f, 0.0492912f, 0.9684867f, 0f,
        0f, 0f, 0f, 1f
    );

    public static Matrix4x4 CalculateChromaticAdaptationMatrix(
        CIExy sourceWhitePoint,
        CIExy destinationWhitePoint
    ) =>
        CalculateChromaticAdaptationMatrix(
            sourceWhitePoint.ToCIExyY(1f).ToCIEXYZ(),
            destinationWhitePoint.ToCIExyY(1f).ToCIEXYZ()
        );

    public static Matrix4x4 CalculateChromaticAdaptationMatrix(
        CIEXYZ sourceWhitePoint,
        CIEXYZ destinationWhitePoint
    )
    {
        var sourceIlluminantTransformed = Vector3.Transform(
            sourceWhitePoint.ToVector3(),
            _bradfordMatrix
        );
        var destinationIlluminantTransformed = Vector3.Transform(
            destinationWhitePoint.ToVector3(),
            _bradfordMatrix
        );

        var scalingFactors =
            destinationIlluminantTransformed / sourceIlluminantTransformed;

        return _bradfordMatrix
            * Matrix4x4.CreateScale(scalingFactors)
            * _inverseBradfordMatrix;
    }
}
