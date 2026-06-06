namespace ColorSpaceComparisons.Core.TransferFunctions;

public static class BT1886TransferFunctions
{
    public static SDRTransferFunction OETF { get; } =
        new(
            0.018053968510807f,
            4.5f,
            0f,
            (float)Math.Pow(1.09929682680944, 1.0 / 0.45),
            0f,
            0.45f,
            -0.09929682680944f
        );

    public static SDRTransferFunction EOTF { get; } = OETF.Inverse();
}
