using ColorSpaceComparisons.Core;
using ColorSpaceComparisons.Core.ColorTypes;
using SkiaSharp;

namespace ColorSpaceComparisons.Utils;

public static class BitmapUtils
{
    public static void NormalizeBrightness(SKBitmap bitmap)
    {
        var maxLevel = 0f;
        for (var y = 0; y < bitmap.Height; ++y)
        {
            for (var x = 0; x < bitmap.Width; ++x)
            {
                var color = bitmap.ReadPixel(x, y);
                maxLevel = Math.Max(maxLevel, color.R);
                maxLevel = Math.Max(maxLevel, color.G);
                maxLevel = Math.Max(maxLevel, color.B);
            }
        }

        var mult = maxLevel == 0f ? 1f : 1f / maxLevel;
        for (var y = 0; y < bitmap.Height; ++y)
        {
            for (var x = 0; x < bitmap.Width; ++x)
            {
                var color = bitmap.ReadPixel(x, y);
                color.R = Math.Clamp(mult * color.R, 0f, 1f);
                color.G = Math.Clamp(mult * color.G, 0f, 1f);
                color.B = Math.Clamp(mult * color.B, 0f, 1f);
                bitmap.WritePixel(x, y, color);
            }
        }
    }

    public static void MakeBright(SKBitmap bitmap)
    {
        for (var y = 0; y < bitmap.Height; ++y)
        {
            for (var x = 0; x < bitmap.Width; ++x)
            {
                var color = bitmap.ReadPixel(x, y);

                var maxLevel = Math.Max(Math.Max(color.R, color.G), color.B);
                if (maxLevel > 0f)
                {
                    var mult = 1f / maxLevel;
                    color.R = Math.Clamp(mult * color.R, 0f, 1f);
                    color.G = Math.Clamp(mult * color.G, 0f, 1f);
                    color.B = Math.Clamp(mult * color.B, 0f, 1f);
                }

                bitmap.WritePixel(x, y, color);
            }
        }
    }

    public static void ApplyOetf(SKBitmap bitmap, ColorSpace colorSpace)
    {
        for (var y = 0; y < bitmap.Height; ++y)
        {
            for (var x = 0; x < bitmap.Width; ++x)
            {
                var color = bitmap.ReadPixel(x, y);
                var alpha = color.A;
                var rgb = colorSpace.ConvertLinearToSignalRGB(color.ToRGB());
                bitmap.WritePixel(x, y, rgb.ToRGBA(alpha));
            }
        }
    }

    internal static void CheckPixelCoordinates(SKBitmap bitmap, int x, int y)
    {
        if (x < 0 || x >= bitmap.Width)
        {
            throw new ArgumentOutOfRangeException(nameof(x));
        }

        if (y < 0 || y >= bitmap.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(x));
        }
    }
}
