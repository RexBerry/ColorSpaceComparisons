namespace ColorSpaceComparisons.Utils;

public static class MathUtils
{
    public static float Lerp(float x, float y, float t) => (1f - t) * x + t * y;

    public static float Hypot(float x, float y)
    {
        x = MathF.Abs(x);
        y = MathF.Abs(y);

        var max = Math.Max(x, y);
        if (max == 0f)
        {
            return 0f;
        }

        x /= max;
        y /= max;
        return max * MathF.Sqrt(x * x + y * y);
    }
}
