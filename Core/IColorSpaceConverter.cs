using ColorSpaceComparisons.Core.ColorTypes;

namespace ColorSpaceComparisons.Core;

public interface IColorSpaceConverter
{
    /// Perform absolute colorimetric conversion
    public RGB Convert(RGB sourceLinearRGB);
}
