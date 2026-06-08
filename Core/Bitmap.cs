using ColorSpaceComparisons.Core.ColorTypes;

namespace ColorSpaceComparisons.Core;

public class Bitmap
{
    public int Width { get; private init; }
    public int Height { get; private init; }

    public RGBA[] Pixels { get; private init; }

    public RGBA this[int x, int y]
    {
        get => Pixels[GetIndex(x, y)];
        set => Pixels[GetIndex(x, y)] = value;
    }

    public Bitmap(int width, int height)
        : this(width, height, new RGBA(0f, 0f, 0f, 0f)) { }

    public Bitmap(int width, int height, RGBA fillColor)
    {
        Width = width;
        Height = height;
        Pixels = new RGBA[width * height];
        for (var i = 0; i < Pixels.Length; ++i)
        {
            Pixels[i] = fillColor;
        }
    }

    public int GetIndex(int x, int y)
    {
        if (x < 0 || x >= Width)
        {
            throw new ArgumentOutOfRangeException(nameof(x));
        }

        if (y < 0 || y >= Height)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        return Width * y + x;
    }
}
