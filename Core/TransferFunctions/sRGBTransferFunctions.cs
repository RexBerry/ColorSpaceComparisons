namespace ColorSpaceComparisons.Core.TransferFunctions;

public static class sRGBTransferFunctions
{
    public static SDRTransferFunction EOTF { get; } =
        new(
            0.04045f,
            (float)(1.0 / 12.92),
            0f,
            (float)(1.0 / 1.055),
            (float)(0.055 / 1.055),
            2.4f,
            0f
        );

    public static SDRTransferFunction OETF { get; } = EOTF.Inverse();
}
