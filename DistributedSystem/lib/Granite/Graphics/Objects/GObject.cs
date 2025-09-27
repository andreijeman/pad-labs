using Granite.Graphics.Components;
using Granite.Graphics.EventArgs;
using Granite.Graphics.Maths;

namespace Granite.Graphics.Objects;

public class GObject 
{
    protected Model _model = new(0, 0);

    protected int _left;
    protected int _top;

    public event Action<GObject, DrawEventArgs>? DrawRequested;
    public event Action<GObject>? LayoutChanged;

    public virtual Model Model
    {
        get => _model;
        set
        {
            _model = value;
            InvokeLayoutChanged();
        }
    }

    public virtual int Left
    { 
        get => _left;
        set
        {
            _left = value;
            InvokeLayoutChanged();
        }
    }
    public virtual int Top
    {
        get => _top;
        set
        {
            _top = value;
            InvokeLayoutChanged();
        }
    }

    public virtual void Draw()
    {
        Draw(new Rect
        {
            X1 = 0,
            Y1 = 0,
            X2 = _model.Width - 1,
            Y2 = _model.Height - 1
        });
    }

    public virtual void Draw(Rect section)
    {
        InvokeDrawRequested(new DrawEventArgs
        {
            Model = _model,
            Section = section,
            Left = _left,
            Top = _top
        });
    }

    protected void InvokeDrawRequested(DrawEventArgs args) => DrawRequested?.Invoke(this, args);
    protected void InvokeLayoutChanged() => LayoutChanged?.Invoke(this);

    public int Width { get => _model.Width; }
    public int Height { get => _model.Height; }
}
