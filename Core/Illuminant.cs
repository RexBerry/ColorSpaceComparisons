using ColorSpaceComparisons.Core.ColorTypes;

namespace ColorSpaceComparisons.Core;

public abstract class Illuminant
{
    public ISpectralDistribution SpectralPowerDistribution { get; private init; }

    public CIExy WhitePoint { get; private init; }

    protected Illuminant(ISpectralDistribution spd, CIExy whitePoint)
    {
        SpectralPowerDistribution = spd;
        WhitePoint = whitePoint;
    }

    public float GetPower(float wavelength) =>
        SpectralPowerDistribution.GetValue(wavelength);
}
