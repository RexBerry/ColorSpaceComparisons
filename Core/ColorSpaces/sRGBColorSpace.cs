using ColorSpaceComparisons.Core.TransferFunctions;

namespace ColorSpaceComparisons.Core.ColorSpaces;

public class sRGBColorSpace()
    : SDRColorSpace(
        WhitePoints.D65,
        new(0.64f, 0.33f),
        new(0.30f, 0.60f),
        new(0.15f, 0.06f),
        sRGBTransferFunctions.EOTF,
        sRGBTransferFunctions.OETF
    ) { }
