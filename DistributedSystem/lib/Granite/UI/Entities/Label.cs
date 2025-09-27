using Granite.Graphics.Components;
using Granite.Graphics.Objects;
using Granite.Graphics.Utilities;
using Granite.UI.EntitiesArgs;

namespace Granite.UI.Entities;

public class Label : GObject
{
    public Label(LabelArgs args)
    {
        _left = args.Left;
        _top = args.Top;
        
        int w = args.Text.Length < args.Width ? args.Text.Length : args.Width;
        int h = (int)Math.Ceiling((double)args.Height / args.Text.Length);
        
        _model = new Model(w, h)
            .Init()
            .FillBackground(args.Color)
            .InsertTextCentered(args.Text, args.TextColor);
    }
}