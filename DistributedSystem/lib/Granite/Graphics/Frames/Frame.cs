using Granite.Graphics.Components;
using Granite.Graphics.EventArgs;
using Granite.Graphics.Maths;
using Granite.Graphics.Objects;

namespace Granite.Graphics.Frames;

public class Frame : GObject
{
    private int _originLeft;
    private int _originTop;

    private Rect _mainRect;

    private List<GObject> _objects = new();
    private Dictionary<GObject, Rect> _objectRectDict = new();

    public override Model Model
    {
        get => _model;
        set
        {
            _model = value;
            _mainRect = GetMainRect(this);
        }
    }

    public override void Draw()
    {
        foreach (var obj in _objects)
        {
            obj.Draw();
        }

        DrawBackground();
    }

    public override void Draw(Rect section)
    {
        DrawFrameRect(GetMainRect(this, section));
    }

    public void DrawBackground()
    {
        DrawModelRects(
            GetObjectDrawableRects(this, _mainRect),
            _model, _originLeft, _originTop);
    }

    public int OriginLeft
    {
        get => _originLeft;
        set
        {
            _originLeft = value;
            _mainRect = GetMainRect(this);
            Draw();
        }
    }

    public int OriginTop
    {
        get => _originTop;
        set
        {
            _originTop = value;
            _mainRect = GetMainRect(this);
            Draw();
        }
    }

    public void Add(GObject obj) => Add(obj, _objects.Count);

    public void Add(GObject obj, int layer)
    {
        if (!_objectRectDict.ContainsKey(obj))
        {
            layer = Math.Max(0, Math.Min(layer, _objectRectDict.Count));
            
            _objects.Insert(layer, obj);
            _objectRectDict.Add(obj, GetObjectRect(obj));

            obj.DrawRequested += OnDrawRequested;
            obj.LayoutChanged += OnLayoutChanged;
            
            obj.Draw();
        }
    }

    public void Remove(GObject obj)
    {
        obj.DrawRequested -= OnDrawRequested;
        obj.LayoutChanged -= OnLayoutChanged;
        
        DrawFrameRect(_objectRectDict[obj]);
        
        _objects.Remove(obj);
        _objectRectDict.Remove(obj);    
    }

    public void BringForward(GObject obj)
    {
        int index = _objects.IndexOf(obj);

        if(index < _objects.Count - 1)
        {
            var temp = _objects[index];
            _objects[index] = _objects[index + 1];
            _objects[index + 1] = temp;
        }

        obj.Draw();
    }

    public void SendBackward(GObject obj)
    {
        int index = _objects.IndexOf(obj);

        if (index > 0)
        {
            var temp = _objects[index];
            _objects[index] = _objects[index - 1];
            _objects[index - 1] = temp;
        }

        if (_mainRect.TryGetIntersection(_objectRectDict[obj], out var intersection))
        {
            DrawFrameRect(intersection);
        }
    }

    private void OnDrawRequested(GObject sender, DrawEventArgs args)
    {
        var rect = ModelSectionToRect(args.Section, args.Left, args.Top);
        var rects = GetObjectDrawableRects(sender, rect);
        
        DrawModelRects(rects, args.Model, args.Left, args.Top);
    }

    private void OnLayoutChanged(GObject sender)
    {
        var oldRect = _objectRectDict[sender];
        var newRect = GetObjectRect(sender);
        _objectRectDict[sender] = newRect;

        if (_mainRect.TryGetIntersection(oldRect, out var intersection))
        {
            if (intersection.TryGetUncoveredSections(newRect, out var rects))
            {
                foreach (var rect in rects)
                {
                    DrawFrameRect(rect);
                }
            }
        }

        sender.Draw();
    }

    private List<Rect> GetObjectDrawableRects(GObject target, Rect targetRect)
    {
        List<Rect> rects = new List<Rect>();

        if (_mainRect.TryGetIntersection(targetRect, out Rect intersection))
        {
            rects.Add(intersection);

            for (int i = _objects.IndexOf(target) + 1; i < _objects.Count; i++)
            {
                var obj = _objects[i];
                List<Rect> temp = new List<Rect>();

                foreach (var rect in rects)
                {
                    if (rect.TryGetUncoveredSections(_objectRectDict[obj], out var results))
                    {
                        temp.AddRange(results);
                    }
                }
                rects = temp;
            }
        }

        return rects;
    }

    private void DrawModelRects(List<Rect> rects, Model model, int modelLeft, int modelTop)
    {
        foreach (var rect in rects)
        {
            InvokeDrawRequested(new DrawEventArgs
            {
                Model = model,
                Section = RectToModelSection(rect, modelLeft, modelTop),
                Left = _left + modelLeft - _originLeft,
                Top = _top + modelTop - _originTop
            });
        }
    }

    private void DrawFrameRect(Rect rect)
    {
        var temp = _mainRect;
        _mainRect = rect;

        foreach (var obj in _objects)
        {
            obj.Draw();
        }
        
        _mainRect = temp;

        var rects = GetObjectDrawableRects(this, rect);
        DrawModelRects(rects, _model, _originLeft, _originTop);
    }

    private static Rect GetObjectRect(GObject obj)
    {
        return new Rect
        {
            X1 = obj.Left,
            Y1 = obj.Top,
            X2 = obj.Left + obj.Width - 1,
            Y2 = obj.Top + obj.Height - 1
        };
    }

    private static Rect GetMainRect(Frame frame)
    {
        return new Rect
        {
            X1 = frame.OriginLeft,
            Y1 = frame.OriginTop,
            X2 = frame.OriginLeft + frame.Width - 1,
            Y2 = frame.OriginTop + frame.Height - 1
        };
    }

    private static Rect GetMainRect(Frame frame, Rect section)
    {
        return new Rect
        {
            X1 = frame.OriginLeft + section.X1,
            Y1 = frame.OriginTop + section.Y1,
            X2 = frame.OriginLeft + section.X2,
            Y2 = frame.OriginTop + section.Y2
        };
    }

    private static Rect ModelSectionToRect(Rect section, int modelLeft, int modelTop)
    {
        return new Rect
        {
            X1 = modelLeft + section.X1,
            Y1 = modelTop + section.Y1,
            X2 = modelLeft + section.X2,
            Y2 = modelTop + section.Y2
        };
    }

    private static Rect RectToModelSection(Rect rect, int modelLeft, int modelTop)
    {
        return new Rect
        {
            X1 = rect.X1 - modelLeft,
            Y1 = rect.Y1 - modelTop,
            X2 = rect.X2 - modelLeft,
            Y2 = rect.Y2 - modelTop
        };
    }
}
