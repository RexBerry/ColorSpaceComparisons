using ColorSpaceComparisons.Core.ColorTypes;

namespace ColorSpaceComparisons.Core;

public static class Illuminants
{
    public static CIExy D50 => new(0.34567f, 0.35850f);
    public static CIExy D55 => new(0.33242f, 0.34743f);
    public static CIExy D65 => new(0.31272f, 0.32903f);
    public static CIExy D75 => new(0.29902f, 0.31485f);
    public static CIExy D93 => new(0.28315f, 0.29711f);
}
