using Granite.Graphics.Components;
using Granite.Graphics.Utilities;

namespace Granite.UI.EntitiesArgs;

public class ContainerArgs : EntityArgs
{
    public Color IdleColor { get; set; }
    public Color FocusedColor { get; set; }
    public Color BackgroundCOlor { get; set; }
    public ModelBuilder.Border Border { get; set; }

    public ContainerArgs()
    {
        IdleColor = UIConfig.Container.IdleColor;
        FocusedColor = UIConfig.Container.FocusedColor;
        BackgroundCOlor = UIConfig.Container.BackgroundColor;
        Border = UIConfig.Container.Border;
    }
}