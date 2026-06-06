using System.Numerics;

namespace ColorSpaceComparisons.Core.ColorTypes;

public record struct HSV(float Hue, float Saturation, float Value)
{
    public readonly RGB ToRGB()
    {
        var hue = Hue / (2f * MathF.PI);
        hue -= MathF.Floor(hue);
        hue *= 6f;

        var color = Vector3.Zero;

        if (hue < 1f)
        {
            color.X = 1f;
            color.Y = hue;
        }
        else if (hue < 2f)
        {
            color.X = 2f - hue;
            color.Y = 1f;
        }
        else if (hue < 3f)
        {
            color.Y = 1f;
            color.Z = hue - 2f;
        }
        else if (hue < 4f)
        {
            color.Y = 4f - hue;
            color.Z = 1f;
        }
        else if (hue < 5f)
        {
            color.Z = 1f;
            color.X = hue - 4f;
        }
        else
        {
            color.Z = 6f - hue;
            color.X = 1f;
        }

        color = Vector3.Lerp(Vector3.One, color, Math.Clamp(Saturation, 0f, 1f));
        color *= Math.Max(Value, 0f);

        return color.ToRGB();
    }

    public readonly Vector3 ToVector3() => new(Hue, Saturation, Value);
}

public static class HSVExtensions
{
    public static HSV ToHSV(this RGB rgb)
    {
        var value = Math.Max(rgb.R, Math.Max(rgb.G, rgb.B));
        if (value == 0f)
        {
            return new(0f, 0f, 0f);
        }

        rgb = (rgb.ToVector3() / value).ToRGB();

        var min = Math.Min(rgb.R, Math.Min(rgb.G, rgb.B));

        var saturation = 1f - min;

        rgb = ((rgb.ToVector3() - new Vector3(min)) / (1f - min)).ToRGB();

        float hue;
        if (rgb.B == 0f)
        {
            if (rgb.R > rgb.G)
            {
                hue = rgb.G;
            }
            else
            {
                hue = 2f - rgb.R;
            }
        }
        else if (rgb.R == 0f)
        {
            if (rgb.G > rgb.B)
            {
                hue = rgb.B + 2f;
            }
            else
            {
                hue = 4f - rgb.G;
            }
        }
        else
        {
            if (rgb.B > rgb.R)
            {
                hue = rgb.R + 4f;
            }
            else
            {
                hue = 6f - rgb.B;
            }
        }

        hue /= 6f;
        hue *= 2f * MathF.PI;

        return new(hue, saturation, value);
    }

    public static HSV ToHSV(this Vector3 vector) => new(vector.X, vector.Y, vector.Z);
}
