namespace ColorSpaceComparisons.Core;

public interface ISpectralDistribution
{
    public float MinWavelength { get; }
    public float MaxWavelength { get; }

    public float GetValue(float wavelength);
}
