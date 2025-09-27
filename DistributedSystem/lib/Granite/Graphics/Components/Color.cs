namespace Granite.Graphics.Components;

public struct Color
{
    public byte R, G, B;

    public Color(byte r, byte g, byte b)
    {
        R = r; G = g; B = b;
    }

    public Color(string hex)
    {
        if (hex.StartsWith("#"))
            hex = hex.Substring(1);

        if (hex.Length != 6)
            throw new ArgumentException("Invalid hex color format.");

        R = Convert.ToByte(hex.Substring(0, 2), 16);
        G = Convert.ToByte(hex.Substring(2, 2), 16);
        B = Convert.ToByte(hex.Substring(4, 2), 16);
    }
}
