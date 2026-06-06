using ColorSpaceComparisons.Core.TransferFunctions;

namespace ColorSpaceComparisons.Core.ColorSpaces;

// Derived from the EDID of my laptop's display
public class SDC4197ColorSpace()
    : SDRColorSpace(
        new(0.3125f, 0.3291015625f),
        new(0.68359375f, 0.31640625f),
        new(0.240234375f, 0.7138671875f),
        new(0.1396484375f, 0.0439453125f),
        GammaTwoDotTwoTransferFunctions.EOTF,
        GammaTwoDotTwoTransferFunctions.OETF
    ) { }
