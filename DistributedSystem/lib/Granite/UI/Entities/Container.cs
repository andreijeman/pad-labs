using Granite.Controllers;
using Granite.Graphics.Components;
using Granite.Graphics.Frames;
using Granite.Graphics.Maths;
using Granite.Graphics.Objects;
using Granite.Graphics.Utilities;
using Granite.UI.EntitiesArgs;

namespace Granite.UI.Entities;

public class Container : Entity
{
    protected readonly Frame _frame;
    protected readonly ControllerHolder _ctrlHolder;
    
    protected Color _idleColor;
    protected Color _focusedColor;
    
    public Container(ContainerArgs args) : base(new Frame(), new ControllerHolder(), args)
    {
        var mainFrame = (Frame)this.GObject ;
        _ctrlHolder = (ControllerHolder)this.Controller;
        
        _idleColor = args.IdleColor;
        _focusedColor = args.FocusedColor;
        
        mainFrame.Model = new Model(args.Width, args.Height)
            .Init()
            .FillBackground(args.BackgroundCOlor)
            .DrawBorder(args.Border, _idleColor);

        _frame = new Frame
        {
            Left = 1,
            Top = 1,
            Model = new Model(args.Width - 2, args.Height - 2)
                .Init()
                .FillBackground(args.BackgroundCOlor)
        };
        
        mainFrame.Add(_frame);
    }

    protected override void OnFocused()
    {
        DrawFrameBorder(_focusedColor);
    }

    protected override void OnUnfocused()
    {
        DrawFrameBorder(_idleColor);
    }
    

    public void Add(Entity entity)
    {
        _frame.Add(entity.GObject);
        _ctrlHolder.Add(entity.Controller);
    }
    
    public void Add(GObject obj)
    {
        _frame.Add(obj);
    }

    private void DrawFrameBorder(Color color)
    {
        GObject.Model
            .FillForeground(color, new Rect(0, 0, GObject.Width - 1, 0))
            .FillForeground(color, new Rect(0, 0, 0, GObject.Height - 1))
            .FillForeground(color, new Rect(GObject.Width - 1, 0, GObject.Width - 1, GObject.Height - 1))
            .FillForeground(color, new Rect(0, GObject.Height - 1, GObject.Width - 1, GObject.Height - 1));
        
        GObject.Draw(new Rect(0, 0, GObject.Width - 1, 0));
        GObject.Draw(new Rect(0, 0, 0, GObject.Height - 1));
        GObject.Draw(new Rect(GObject.Width - 1, 0, GObject.Width - 1, GObject.Height - 1));
        GObject.Draw(new Rect(0, GObject.Height - 1, GObject.Width - 1, GObject.Height - 1));
    }
}