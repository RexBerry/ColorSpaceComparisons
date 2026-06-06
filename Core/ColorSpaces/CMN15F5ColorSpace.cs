using ColorSpaceComparisons.Core.TransferFunctions;

namespace ColorSpaceComparisons.Core.ColorSpaces;

// Derived from the EDID of my old laptop's display
public class CMN15F5ColorSpace()
    : SDRColorSpace(
        new(0.3134765625f, 0.3291015625f),
        new(0.58984375f, 0.349609375f),
        new(0.330078125f, 0.5546875f),
        new(0.1533203125f, 0.119140625f),
        GammaTwoDotTwoTransferFunctions.EOTF,
        GammaTwoDotTwoTransferFunctions.OETF
    ) { }
