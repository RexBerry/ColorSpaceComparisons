using System.Numerics;
using ColorSpaceComparisons.Core;
using ColorSpaceComparisons.Core.ColorSpaceConverters;
using ColorSpaceComparisons.Core.ColorSpaces;
using ColorSpaceComparisons.Core.ColorTypes;
using ColorSpaceComparisons.Core.StandardIlluminants;
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

        GenerateRainbows(saveDir);
        GenerateColorRings(saveDir);
    }

    private static void GenerateRainbows(string saveDir)
    {
        SaveRainbow(
            Path.Combine(saveDir, "Rainbow sRGB D65.png"),
            new sRGBColorSpace(),
            new IlluminantD65(),
            2048,
            256,
            400f,
            700f
        );
        SaveRainbow(
            Path.Combine(saveDir, "Rainbow Display P3 D65.png"),
            new DisplayP3ColorSpace(),
            new IlluminantD65(),
            2048,
            256,
            400f,
            700f
        );
        SaveRainbow(
            Path.Combine(saveDir, "Rainbow SDC4197 D65.png"),
            new SDC4197ColorSpace(),
            new IlluminantD65(),
            2048,
            256,
            400f,
            700f
        );
        SaveRainbow(
            Path.Combine(saveDir, "Rainbow Rec. 2020 D65.png"),
            new Rec2020ColorSpace(),
            new IlluminantD65(),
            2048,
            256,
            400f,
            700f
        );
    }

    private static void GenerateColorRings(string saveDir)
    {
        SaveColorRing(
            Path.Combine(saveDir, "Color Ring Display P3 vs sRGB.png"),
            new DisplayP3ColorSpace(),
            new sRGBColorSpace(),
            new DisplayP3ColorSpace(),
            2048,
            0.6f,
            0.8f
        );
        SaveColorRing(
            Path.Combine(saveDir, "Color Ring Rec. 2020 vs sRGB.png"),
            new Rec2020ColorSpace(),
            new sRGBColorSpace(),
            new Rec2020ColorSpace(),
            2048,
            0.6f,
            0.8f
        );
        SaveColorRing(
            Path.Combine(saveDir, "Color Ring Rec. 2020 vs Display P3.png"),
            new Rec2020ColorSpace(),
            new DisplayP3ColorSpace(),
            new Rec2020ColorSpace(),
            2048,
            0.6f,
            0.8f
        );

        SaveColorRing(
            Path.Combine(saveDir, "Color Ring SDC4197 vs CMN15F5.png"),
            new SDC4197ColorSpace(),
            new CMN15F5ColorSpace(),
            new SDC4197ColorSpace(),
            2048,
            0.6f,
            0.8f
        );
        SaveColorRing(
            Path.Combine(saveDir, "Color Ring SDC4197 vs sRGB.png"),
            new SDC4197ColorSpace(),
            new sRGBColorSpace(),
            new SDC4197ColorSpace(),
            2048,
            0.6f,
            0.8f
        );
        SaveColorRing(
            Path.Combine(saveDir, "Color Ring SDC4197 vs Display P3.png"),
            new SDC4197ColorSpace(),
            new DisplayP3ColorSpace(),
            new SDC4197ColorSpace(),
            2048,
            0.6f,
            0.8f
        );
    }

    public static void SaveRainbow(
        string savePath,
        SDRColorSpace colorSpace,
        Illuminant illuminant,
        int width,
        int height,
        float leftWavelength,
        float rightWavelength,
        bool applyChromaticAdaptation = true
    )
    {
        if (applyChromaticAdaptation)
        {
            colorSpace.ApplyChromaticAdaptation(illuminant.WhitePoint);
        }

        var cie1931 = new CIE1931();
        var converter = new ClippingCIExyColorSpaceConverter(colorSpace, colorSpace);

        var bitmap = new Bitmap(width, height);
        var pixels = bitmap.Pixels;
        for (var x = 0; x < width; ++x)
        {
            var wavelength = MathUtils.Lerp(
                leftWavelength,
                rightWavelength,
                (x + 0.5f) / width
            );

            var xyz = (
                cie1931.CMF(wavelength).ToVector3() * illuminant.GetPower(wavelength)
            ).ToCIEXYZ();
            var rgb = converter.Convert(xyz, illuminant.WhitePoint, true);

            var color = rgb.ToRGBA(1f);

            for (var i = x; i < pixels.Length; i += width)
            {
                pixels[i] = color;
            }
        }

        BitmapUtils.NormalizeBrightness(bitmap);
        bitmap.ApplyOETF(colorSpace);

        SaveBitmap(bitmap, savePath);
    }

    public static void SaveColorRing(
        string savePath,
        SDRColorSpace ringColorSpace,
        SDRColorSpace bgColorSpace,
        SDRColorSpace imageColorSpace,
        int size,
        float innerRingRadius,
        float outerRingRadius,
        bool applyChromaticAdaptation = true
    )
    {
        if (applyChromaticAdaptation)
        {
            ringColorSpace.ApplyChromaticAdaptation(imageColorSpace.WhitePoint);
            bgColorSpace.ApplyChromaticAdaptation(imageColorSpace.WhitePoint);
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

        var bitmap = new Bitmap(size, size);

        var pixels = bitmap.Pixels;
        var i = 0;
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

                pixels[i++] = color.ToRGBA();
            }
        }

        if (!SDRColorSpace.CheckMatch(ringColorSpace, imageColorSpace))
        {
            var converterRingToImage = new ClampingRGBColorSpaceConverter(
                ringColorSpace,
                imageColorSpace
            );
            bitmap.ConvertColors(converterRingToImage);
        }

        BitmapUtils.NormalizeBrightness(bitmap);
        bitmap.ApplyOETF(imageColorSpace);

        SaveBitmap(bitmap, savePath);
    }

    private static void SaveBitmap(Bitmap bitmap, string savePath)
    {
        using var skBitmap = bitmap.ToSKBitmap();
        using var image = SKImage.FromBitmap(skBitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = File.OpenWrite(savePath);
        data.SaveTo(stream);

        Console.WriteLine($"Saved to {savePath}");
    }
}
