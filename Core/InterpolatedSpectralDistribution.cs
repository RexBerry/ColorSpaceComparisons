using System.Globalization;
using ColorSpaceComparisons.Utils;
using CsvHelper;

namespace ColorSpaceComparisons.Core;

public class InterpolatedSpectralDistribution : ISpectralDistribution
{
    private readonly float[] _lut;

    public float MinWavelength { get; private init; }
    public float MaxWavelength { get; private init; }

    public InterpolatedSpectralDistribution(
        string csvPath,
        int columnIndex = 1,
        int wavelengthColumnIndex = 0,
        float divisor = 1f
    )
    {
        using var reader = new StreamReader(csvPath);
        using var csv = new CsvParser(reader, CultureInfo.InvariantCulture);

        var values = new List<float>();
        var prevWavelength = float.NaN;
        while (csv.Read())
        {
            var row = csv.Record!;
            var wavelength = float.Parse(row[wavelengthColumnIndex]);
            var value = float.Parse(row[columnIndex]) / divisor;

            if (!float.IsInteger(wavelength) || wavelength <= 0f)
            {
                throw new Exception("Invalid wavelength.");
            }

            MaxWavelength = wavelength;

            if (float.IsNaN(prevWavelength))
            {
                MinWavelength = wavelength;
            }
            else if (wavelength - prevWavelength != 1f)
            {
                throw new Exception("Wavelengths must be in increments of 1 nm.");
            }

            values.Add(value);
            prevWavelength = wavelength;
        }

        _lut = values.ToArray();
    }

    public float GetValue(float wavelength) =>
        Interpolation.Sprague(_lut, MinWavelength, 1f, wavelength);
}
