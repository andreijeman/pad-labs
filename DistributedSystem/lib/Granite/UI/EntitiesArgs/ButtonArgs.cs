using Granite.Graphics.Components;

namespace Granite.UI.EntitiesArgs
{
    public class ButtonArgs : EntityArgs
    {
        public Color IdleColor { get; set; } 
        public Color FocusedColor { get; set; }
        public Color TextColor { get; set; }
        public String Text { get; set; }
        public Action? OnPressed { get; set; }

        public ButtonArgs()
        {
            IdleColor = UIConfig.Button.IdleColor;
            FocusedColor = UIConfig.Button.FocusedColor;
            TextColor = UIConfig.Button.TextColor;
            Text = UIConfig.Button.Text;
        }
    }
}

