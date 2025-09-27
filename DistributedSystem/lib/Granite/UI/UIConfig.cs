using Granite.Graphics.Components;
using Granite.Graphics.Utilities;

namespace Granite.UI;

public static class UIConfig
{
    public static class Container
    {
        public static readonly Color IdleColor = new Color("404040");
        public static readonly Color FocusedColor = new Color("E0E0E0");
        public static readonly Color BackgroundColor = new Color("202020");
        public static readonly ModelBuilder.Border Border = ModelBuilder.Assets.DoubleLineBorder;
    }
    
    public static class Button
    {
        public static readonly Color IdleColor = new Color("9933FF");
        public static readonly Color FocusedColor = new Color("FF33FF");
        public static readonly Color TextColor = new Color("FFFFFF");
        public const string Text = "";
    }


    public static class Label
    {
        public static readonly Color Color = new Color("FFFF00");
        public static readonly Color TextColor = new Color("000000");
        public const string Text = "Label";
    }

    public static class TextBox
    {
        public static readonly Color IdleColor = new Color("4C9900");
        public static readonly Color FocusedColor = new Color("80FF00");
        public static readonly Color BackgroundColor = new Color("202020");
        public static readonly ModelBuilder.Border Border = ModelBuilder.Assets.LineBorder;
        public static readonly Color TextColor = new Color("FFFFFF");
        public const string Text = "";
    }
    
    public static class InputBox
    {
        public static readonly Color IdleColor = new Color("009999");
        public static readonly Color FocusedColor = new Color("00FFFF");
        public static readonly Color BackgroundColor = new Color("202020");
        public static readonly ModelBuilder.Border Border = ModelBuilder.Assets.LineBorder;
        public static readonly Color TextColor = new Color("FFFFFF");
        public const string Text = "";
        public const Char Cursor = '_';
    }
}