namespace ColorSpaceComparisons.Core.StandardIlluminants;

public class IlluminantD65 : Illuminant
{
    public IlluminantD65()
        : base(
            new InterpolatedSpectralDistribution(
                Path.Combine(
                    AppContext.BaseDirectory,
                    "Static",
                    "SpectralDistributions",
                    "CIE_std_illum_D65.csv"
                ),
                divisor: 100f
            ),
            WhitePoints.D65
        ) { }
}
