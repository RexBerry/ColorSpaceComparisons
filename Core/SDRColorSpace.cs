using ColorSpaceComparisons.Core.ColorTypes;

namespace ColorSpaceComparisons.Core;

public class SDRColorSpace : ColorSpace
{
    public SDRTransferFunction TransferFunctionEOTF { get; private init; }

    public SDRTransferFunction TransferFunctionOETF { get; private init; }

    public SDRColorSpace(
        CIExy whitePoint,
        CIExy redPrimary,
        CIExy greenPrimary,
        CIExy bluePrimary,
        SDRTransferFunction eotf,
        SDRTransferFunction oetf
    )
        : base(whitePoint, redPrimary, greenPrimary, bluePrimary)
    {
        TransferFunctionEOTF = eotf;
        TransferFunctionOETF = oetf;
    }

    public SDRColorSpace(
        CIExy whitePoint,
        CIExy redPrimary,
        CIExy greenPrimary,
        CIExy bluePrimary,
        SDRTransferFunction eotf
    )
        : this(whitePoint, redPrimary, greenPrimary, bluePrimary, eotf, eotf.Inverse())
    { }

    public override float EOTF(float x) => TransferFunctionEOTF.Evaluate(x);

    public override float OETF(float x) => TransferFunctionOETF.Evaluate(x);

    public static bool CheckMatch(SDRColorSpace colorSpace1, SDRColorSpace colorSpace2) =>
        ArePrimariesEqual(colorSpace1, colorSpace2)
        && colorSpace1.TransferFunctionEOTF == colorSpace2.TransferFunctionEOTF
        && colorSpace1.TransferFunctionOETF == colorSpace2.TransferFunctionOETF;
}
