using Granite.Controllers;
using Granite.Graphics.Components;
using Granite.Graphics.Objects;
using Granite.Graphics.Utilities;
using Granite.UI.EntitiesArgs;

namespace Granite.UI.Entities;

public class Button : Entity
{
    private Color _color;
    private Color _focusedColor;
    
    private Color _textColor;
    private String _text;

    public Button(ButtonArgs args) : base(new GObject(), new Controller(), args)
    {
        _color = args.IdleColor;
        _focusedColor = args.FocusedColor;
        _textColor = args.TextColor;
        _text = args.Text;
        
        GObject.Model 
            .FillBackground(_color)
            .InsertTextCentered(_text, _textColor);
        
        if(args.OnPressed != null) Controller.AddKeyAction(ConsoleKey.Enter, args.OnPressed);
    }
    
    protected override void OnFocused()
    {
        GObject.Model.FillBackground(_focusedColor);
        GObject.Draw();
    }

    protected override void OnUnfocused()
    {
        GObject.Model.FillBackground(_color);
        GObject.Draw();
    }
}