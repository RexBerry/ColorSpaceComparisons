using ColorSpaceComparisons.Core.ColorTypes;
using SkiaSharp;

namespace ColorSpaceComparisons.Utils;

public static class BitmapExtensions
{
    public static RGBA ReadPixel(this SKBitmap bitmap, int x, int y)
    {
        BitmapUtils.CheckPixelCoordinates(bitmap, x, y);

        unsafe
        {
            var pixelPtr = (float*)bitmap.GetPixels().ToPointer();
            var offset = 4 * (bitmap.Width * y + x);
            return new(
                pixelPtr[offset],
                pixelPtr[offset + 1],
                pixelPtr[offset + 2],
                pixelPtr[offset + 3]
            );
        }
    }

    public static void WritePixel(this SKBitmap bitmap, int x, int y, RGBA color)
    {
        BitmapUtils.CheckPixelCoordinates(bitmap, x, y);

        unsafe
        {
            var pixelPtr = (float*)bitmap.GetPixels().ToPointer();
            var offset = 4 * (bitmap.Width * y + x);
            pixelPtr[offset] = color.R;
            pixelPtr[offset + 1] = color.G;
            pixelPtr[offset + 2] = color.B;
            pixelPtr[offset + 3] = color.A;
        }
    }
}
