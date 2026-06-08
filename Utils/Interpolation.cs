namespace ColorSpaceComparisons.Utils;

public static class Interpolation
{
    public static float Sprague(
        float ym2,
        float ym1,
        float y0,
        float y1,
        float y2,
        float y3,
        float h
    )
    {
        var coeff0 = y0;
        var coeff1 = 1f / 12f * (-y2 + ym2 + 8 * y1 - 8 * ym1);
        var coeff2 = 1f / 24f * (-y2 - ym2 + 16 * y1 + 16 * ym1 - 30 * y0);
        var coeff3 =
            1f / 24f * (7 * y3 - 9 * ym2 - 33 * y2 + 39 * ym1 + 66 * y1 - 70 * y0);
        var coeff4 =
            1f / 24f * (-12 * y3 + 13 * ym2 + 61 * y2 - 64 * ym1 - 124 * y1 + 126 * y0);
        var coeff5 =
            1f / 24f * (5 * y3 - 5 * ym2 - 25 * y2 + 25 * ym1 + 50 * y1 - 50 * y0);

        return coeff0
            + h * (coeff1 + h * (coeff2 + h * (coeff3 + h * (coeff4 + h * coeff5))));
    }

    public static float Sprague(
        ReadOnlySpan<float> values,
        float begin,
        float step,
        float x,
        float fallbackValue = 0f
    )
    {
        var i = (int)((x - begin) / step);

        if (i + 3 < 0 || i - 2 >= values.Length)
        {
            return fallbackValue;
        }

        var ym2 = i - 2 < 0 || i - 2 >= values.Length ? fallbackValue : values[i - 2];
        var ym1 = i - 1 < 0 || i - 1 >= values.Length ? fallbackValue : values[i - 1];
        var y0 = i < 0 || i >= values.Length ? fallbackValue : values[i];
        var y1 = i + 1 < 0 || i + 1 >= values.Length ? fallbackValue : values[i + 1];
        var y2 = i + 2 < 0 || i + 2 >= values.Length ? fallbackValue : values[i + 2];
        var y3 = i + 3 < 0 || i + 3 >= values.Length ? fallbackValue : values[i + 3];

        var h = (x - (begin + i * step)) / step;

        return Sprague(ym2, ym1, y0, y1, y2, y3, h);
    }
}
