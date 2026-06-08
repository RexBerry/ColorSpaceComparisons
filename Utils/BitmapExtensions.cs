using ColorSpaceComparisons.Core;
using ColorSpaceComparisons.Core.ColorTypes;
using SkiaSharp;

namespace ColorSpaceComparisons.Utils;

public static class BitmapExtensions
{
    public static void ApplyOETF(this Bitmap bitmap, ColorSpace colorSpace)
    {
        var pixels = bitmap.Pixels;
        for (var i = 0; i < pixels.Length; ++i)
        {
            var color = pixels[i];
            var rgb = colorSpace.ConvertLinearToSignalRGB(color.ToRGB());
            pixels[i] = rgb.ToRGBA(color.A);
        }
    }

    public static void ConvertColors(this Bitmap bitmap, IColorSpaceConverter converter)
    {
        var pixels = bitmap.Pixels;
        for (var i = 0; i < pixels.Length; ++i)
        {
            var color = pixels[i];
            pixels[i] = converter.Convert(color.ToRGB()).ToRGBA(color.A);
        }
    }

    public static SKBitmap ToSKBitmap(this Bitmap bitmap)
    {
        var result = new SKBitmap(
            bitmap.Width,
            bitmap.Height,
            SKColorType.RgbaF32,
            SKAlphaType.Unpremul,
            // Just save as sRGB for simplicity
            // Use software such as GIMP to assign a new color profile to the saved image
            SKColorSpace.CreateSrgb()
        );

        unsafe
        {
            var pixelPtr = (float*)result.GetPixels().ToPointer();
            foreach (var color in bitmap.Pixels)
            {
                (*pixelPtr++) = color.R;
                (*pixelPtr++) = color.G;
                (*pixelPtr++) = color.B;
                (*pixelPtr++) = color.A;
            }
        }

        return result;
    }
}
