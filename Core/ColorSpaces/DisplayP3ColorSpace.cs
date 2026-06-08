using ColorSpaceComparisons.Core.TransferFunctions;

namespace ColorSpaceComparisons.Core.ColorSpaces;

public class DisplayP3ColorSpace()
    : SDRColorSpace(
        WhitePoints.D65,
        new(0.680f, 0.320f),
        new(0.265f, 0.690f),
        new(0.150f, 0.060f),
        sRGBTransferFunctions.EOTF,
        sRGBTransferFunctions.OETF
    ) { }
