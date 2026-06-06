namespace ColorSpaceComparisons.Core.ColorTypes;

public record struct CIExyY(float x, float y, float Y)
{
    public readonly CIEXYZ ToCIEXYZ() => new(x / y * Y, Y, (1 - x - y) / y * Y);

    public readonly CIExy ToCIExy() => new(x, y);
}
