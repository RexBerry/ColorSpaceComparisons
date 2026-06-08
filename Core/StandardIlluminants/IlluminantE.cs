namespace ColorSpaceComparisons.Core.StandardIlluminants;

public class IlluminantE : Illuminant
{
    private class EqualEnergySpectralPowerDistribution : ISpectralDistribution
    {
        public float MinWavelength => 0f;

        public float MaxWavelength => float.PositiveInfinity;

        public float GetValue(float wavelength) => 1f;
    }

    public IlluminantE()
        : base(new EqualEnergySpectralPowerDistribution(), new(1f / 3f, 1f / 3f)) { }
}
