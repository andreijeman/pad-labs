using Granite.Controllers;
using Granite.Graphics.Components;
using Granite.Graphics.Objects;
using Granite.Graphics.Utilities;
using Granite.UI.EntitiesArgs;

namespace Granite.UI.Entities;

public abstract class Entity
{
    public readonly GObject GObject;
    public readonly Controller Controller;
    
    public Entity(GObject obj, Controller ctrl, EntityArgs args)
    {
        GObject = obj;
        Controller = ctrl;
        
        Controller.Focused += (isFocused) =>
        {
            if (isFocused) OnFocused();
            else OnUnfocused();
        }; 

        foreach (var pair in args.keyActionDict)
        {
            Controller.AddKeyAction(pair.Key, pair.Value);
        }
        
        GObject.Left = args.Left;
        GObject.Top = args.Top;
        GObject.Model = new Model(args.Width, args.Height).Init();
    }
    
    protected abstract void OnFocused();
    protected abstract void OnUnfocused();
    
    public bool IsFocused { get => Controller.IsFocused; }
}