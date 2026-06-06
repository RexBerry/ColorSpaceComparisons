using SkiaSharp;

namespace ColorSpaceComparisons.Core;

public record struct SDRTransferFunction(
    float Threshold,
    float SlopeLow,
    float InterceptLow,
    float InnerSlopeHigh,
    float InnerInterceptHigh,
    float GammaHigh,
    float OuterInterceptHigh
)
{
    public readonly float Evaluate(float x)
    {
        if (x <= 0f)
        {
            return 0f;
        }

        if (x >= 1f)
        {
            return 1f;
        }

        if (x < Threshold)
        {
            return SlopeLow * x + InterceptLow;
        }

        return MathF.Pow(InnerSlopeHigh * x + InnerInterceptHigh, GammaHigh)
            + OuterInterceptHigh;
    }

    public readonly SDRTransferFunction Inverse()
    {
        var mult = MathF.Pow(InnerSlopeHigh, -GammaHigh);
        return new(
            Evaluate(Threshold),
            1f / SlopeLow,
            -InterceptLow / SlopeLow,
            mult,
            -OuterInterceptHigh * mult,
            1f / GammaHigh,
            -InnerInterceptHigh / InnerSlopeHigh
        );
    }

    public readonly SKColorSpaceTransferFn SkiaTransferFunction() =>
        new(
            GammaHigh,
            InnerSlopeHigh,
            InnerInterceptHigh,
            SlopeLow,
            Threshold,
            OuterInterceptHigh,
            InterceptLow
        );
}
