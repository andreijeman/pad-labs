using Granite.Graphics.Components;
using Granite.Graphics.Maths;

namespace Granite.Graphics.EventArgs;

public struct DrawEventArgs
{
    public Model Model;
    public Rect Section;
    public int Left, Top;
}
