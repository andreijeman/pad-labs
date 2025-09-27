using Granite.Graphics.Utilities;
using Color = Granite.Graphics.Components.Color;

namespace Granite.UI.EntitiesArgs;

public class TextBoxArgs : EntityArgs
{
    public Color IdleColor { get; set; }
    public Color FocusedColor { get; set; }
    public Color BackgroundColor { get; set; }
    public Color TextColor { get; set; }   
    public ModelBuilder.Border Border { get; set; }
    public string Text { get; set; }

    public TextBoxArgs()
    {
        IdleColor = UIConfig.TextBox.IdleColor;
        FocusedColor = UIConfig.TextBox.FocusedColor;
        BackgroundColor = UIConfig.TextBox.BackgroundColor;
        TextColor = UIConfig.TextBox.TextColor;
        Border = UIConfig.TextBox.Border;
        Text = UIConfig.TextBox.Text;
    }
}