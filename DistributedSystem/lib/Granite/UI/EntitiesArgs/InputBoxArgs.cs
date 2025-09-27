namespace Granite.UI.EntitiesArgs;

public class InputBoxArgs : TextBoxArgs
{
    public char Cursor { get; set; }

    public InputBoxArgs()
    {
        Cursor = UIConfig.InputBox.Cursor;
        IdleColor = UIConfig.InputBox.IdleColor;
        FocusedColor = UIConfig.InputBox.FocusedColor;
        BackgroundColor = UIConfig.InputBox.BackgroundColor;
        TextColor = UIConfig.InputBox.TextColor;
        Border = UIConfig.InputBox.Border;
        Text = UIConfig.InputBox.Text;
    }
}