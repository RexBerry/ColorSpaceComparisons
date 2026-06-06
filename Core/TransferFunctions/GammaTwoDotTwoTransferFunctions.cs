namespace ColorSpaceComparisons.Core.TransferFunctions;

public static class GammaTwoDotTwoTransferFunctions
{
    public static SDRTransferFunction EOTF { get; } = new(0f, 1f, 0f, 1f, 0f, 2.2f, 0f);

    public static SDRTransferFunction OETF { get; } = EOTF.Inverse();
}
