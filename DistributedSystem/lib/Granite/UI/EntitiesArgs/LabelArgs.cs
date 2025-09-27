using Granite.Graphics.Components;

namespace Granite.UI.EntitiesArgs;

public class LabelArgs : EntityArgs
{
    public Color Color { get; set; }
    public Color TextColor { get; set; }
    public String Text { get; set; }

    public LabelArgs()
    {
        Color = UIConfig.Label.Color;
        TextColor = UIConfig.Label.TextColor;
        Text = UIConfig.Label.Text;
    }
}