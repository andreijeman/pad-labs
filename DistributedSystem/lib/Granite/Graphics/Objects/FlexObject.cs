using Granite.Graphics.Components;
using Granite.Graphics.Utilities;

namespace Granite.Graphics.Objects;

public class FlexObject : GObject
{
    private Action<Model> Sculpt = (Model model) => model.DrawChessboard(new Color("FFFFFF"), new Color("000000"));
    
    public new int Width
    {
        get => _model.Width;
        set
        {
            _model = new Model(value, _model.Height).Init();
            Sculpt(_model);
            InvokeLayoutChanged();
        }
    }

    public new int Height
    {
        get => _model.Height;
        set
        {
            _model = new Model(_model.Width, value).Init();
            Sculpt(_model);
            InvokeLayoutChanged();
        }
    }

    public (int Width, int Height) Size
    {
        get => (_model.Width, _model.Height);
        set
        {
            _model = new Model(value.Width, value.Height).Init();
            Sculpt(_model);
            InvokeLayoutChanged();
        }
    }
}