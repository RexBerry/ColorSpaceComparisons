using ColorSpaceComparisons.Core;

namespace ColorSpaceComparisons.Utils;

public static class BitmapUtils
{
    public static void NormalizeBrightness(Bitmap bitmap)
    {
        var maxLevel = 0f;
        foreach (var color in bitmap.Pixels)
        {
            maxLevel = Math.Max(maxLevel, color.R);
            maxLevel = Math.Max(maxLevel, color.G);
            maxLevel = Math.Max(maxLevel, color.B);
        }

        var mult = maxLevel == 0f ? 1f : 1f / maxLevel;
        var pixels = bitmap.Pixels;
        for (var i = 0; i < pixels.Length; ++i)
        {
            var color = pixels[i];
            color.R = Math.Clamp(mult * color.R, 0f, 1f);
            color.G = Math.Clamp(mult * color.G, 0f, 1f);
            color.B = Math.Clamp(mult * color.B, 0f, 1f);
            pixels[i] = color;
        }
    }

    public static void MakeBright(Bitmap bitmap)
    {
        var pixels = bitmap.Pixels;
        for (var i = 0; i < pixels.Length; ++i)
        {
            var color = pixels[i];

            var maxLevel = Math.Max(Math.Max(color.R, color.G), color.B);
            if (maxLevel > 0f)
            {
                var mult = 1f / maxLevel;
                color.R = Math.Clamp(mult * color.R, 0f, 1f);
                color.G = Math.Clamp(mult * color.G, 0f, 1f);
                color.B = Math.Clamp(mult * color.B, 0f, 1f);
            }

            pixels[i] = color;
        }
    }
}
