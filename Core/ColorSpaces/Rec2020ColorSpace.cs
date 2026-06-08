using ColorSpaceComparisons.Core.TransferFunctions;

namespace ColorSpaceComparisons.Core.ColorSpaces;

public class Rec2020ColorSpace()
    : SDRColorSpace(
        WhitePoints.D65,
        new(0.70792f, 0.29203f),
        new(0.17024f, 0.79652f),
        new(0.13137f, 0.04588f),
        BT1886TransferFunctions.EOTF,
        BT1886TransferFunctions.OETF
    ) { }
