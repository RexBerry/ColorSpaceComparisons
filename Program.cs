using System.Numerics;
using ColorSpaceComparisons.Core;
using ColorSpaceComparisons.Core.ColorSpaceConverters;
using ColorSpaceComparisons.Core.ColorSpaces;
using ColorSpaceComparisons.Core.ColorTypes;
using ColorSpaceComparisons.Utils;
using SkiaSharp;

namespace ColorSpaceComparisons;

internal class Program
{
    private static void Main()
    {
        var saveDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            "Color Space Comparisons"
        );

        GenerateColorRing(
            Path.Combine(saveDir, "Color Ring Display P3 vs sRGB.png"),
            new sRGBColorSpace(),
            new DisplayP3ColorSpace(),
            new DisplayP3ColorSpace(),
            2048,
            0.6f,
            0.8f
        );
        GenerateColorRing(
            Path.Combine(saveDir, "Color Ring Rec. 2020 vs sRGB.png"),
            new sRGBColorSpace(),
            new Rec2020ColorSpace(),
            new Rec2020ColorSpace(),
            2048,
            0.6f,
            0.8f
        );
        GenerateColorRing(
            Path.Combine(saveDir, "Color Ring Rec. 2020 vs Display P3.png"),
            new DisplayP3ColorSpace(),
            new Rec2020ColorSpace(),
            new Rec2020ColorSpace(),
            2048,
            0.6f,
            0.8f
        );

        GenerateColorRing(
            Path.Combine(saveDir, "Color Ring SDC4197 vs CMN15F5.png"),
            new CMN15F5ColorSpace(),
            new SDC4197ColorSpace(),
            new SDC4197ColorSpace(),
            2048,
            0.6f,
            0.8f
        );
        GenerateColorRing(
            Path.Combine(saveDir, "Color Ring SDC4197 vs sRGB.png"),
            new sRGBColorSpace(),
            new SDC4197ColorSpace(),
            new SDC4197ColorSpace(),
            2048,
            0.6f,
            0.8f
        );
        GenerateColorRing(
            Path.Combine(saveDir, "Color Ring SDC4197 vs Display P3.png"),
            new DisplayP3ColorSpace(),
            new SDC4197ColorSpace(),
            new SDC4197ColorSpace(),
            2048,
            0.6f,
            0.8f
        );
    }

    public static void GenerateColorRing(
        string savePath,
        SDRColorSpace bgColorSpace,
        SDRColorSpace ringColorSpace,
        SDRColorSpace imageColorSpace,
        int size,
        float innerRingRadius,
        float outerRingRadius,
        bool applyChromaticAdaptation = true
    )
    {
        if (applyChromaticAdaptation)
        {
            bgColorSpace.AdaptToIlluminant(imageColorSpace.Illuminant);
            ringColorSpace.AdaptToIlluminant(imageColorSpace.Illuminant);
        }

        var scale = size / 2f;
        innerRingRadius *= scale;
        outerRingRadius *= scale;

        var converterRingToBg = new ClippingCIExyColorSpaceConverter(
            ringColorSpace,
            bgColorSpace
        );
        var converterBgToRing = new SimpleColorSpaceConverter(
            bgColorSpace,
            ringColorSpace
        );

        using var bitmap = new SKBitmap(
            size,
            size,
            SKColorType.RgbaF32,
            SKAlphaType.Premul,
            // Just save as sRGB for simplicity
            // Use software such as GIMP to assign a new color profile to the saved image
            SKColorSpace.CreateSrgb()
        );

        bitmap.SetPixel(0, 0, new SKColor(200, 100, 50, 25));

        for (var y = 0; y < size; ++y)
        {
            var v = 1f - (y + 0.5f) / size * 2f;

            for (var x = 0; x < size; ++x)
            {
                var u = (x + 0.5f) / size * 2f - 1f;

                var distance = scale * MathF.Sqrt(u * u + v * v);

                var mix =
                    distance < innerRingRadius - 0.5f ? 0f
                    : distance < innerRingRadius + 0.5
                        ? distance - (innerRingRadius - 0.5f)
                    : distance < outerRingRadius - 0.5f ? 1f
                    : distance < outerRingRadius + 0.5f
                        ? (outerRingRadius + 0.5f) - distance
                    : 0f;

                var hue = MathF.Atan2(u, v);
                var color = new HSV(hue, 1f, 1f).ToRGB().Pow(2.4f);

                var bgColor = converterRingToBg.Convert(color);
                bgColor = converterBgToRing.Convert(bgColor).Bright();

                color = Vector3.Lerp(bgColor.ToVector3(), color.ToVector3(), mix).ToRGB();

                bitmap.WritePixel(x, y, color.ToRGBA());
            }
        }

        if (!SDRColorSpace.CheckMatch(ringColorSpace, imageColorSpace))
        {
            var converterRingToImage = new ClampingRGBColorSpaceConverter(
                ringColorSpace,
                imageColorSpace
            );

            for (var y = 0; y < size; ++y)
            {
                for (var x = 0; x < size; ++x)
                {
                    bitmap.WritePixel(
                        x,
                        y,
                        converterRingToImage
                            .Convert(bitmap.ReadPixel(x, y).ToRGB())
                            .ToRGBA()
                    );
                }
            }
        }

        BitmapUtils.NormalizeBrightness(bitmap);
        BitmapUtils.ApplyOetf(bitmap, ringColorSpace);

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = File.OpenWrite(savePath);
        data.SaveTo(stream);
    }
}
