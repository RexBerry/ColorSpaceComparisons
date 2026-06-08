using ColorSpaceComparisons.Core.ColorTypes;

namespace ColorSpaceComparisons.Core;

public class CIE1931
{
    public float MinWavelength { get; private init; }
    public float MaxWavelength { get; private init; }

    public InterpolatedSpectralDistribution X_CMF { get; private init; }
    public InterpolatedSpectralDistribution Y_CMF { get; private init; }
    public InterpolatedSpectralDistribution Z_CMF { get; private init; }

    public CIE1931()
    {
        var csvPath = Path.Combine(
            AppContext.BaseDirectory,
            "Static",
            "SpectralDistributions",
            "CIE_xyz_1931_2deg.csv"
        );

        X_CMF = new(csvPath, 1);
        Y_CMF = new(csvPath, 2);
        Z_CMF = new(csvPath, 3);

        MinWavelength = X_CMF.MinWavelength;
        MaxWavelength = X_CMF.MaxWavelength;
    }

    public CIEXYZ CMF(float wavelength)
    {
        var x = X_CMF.GetValue(wavelength);
        var y = Y_CMF.GetValue(wavelength);
        var z = Z_CMF.GetValue(wavelength);
        return new(x, y, z);
    }
}
