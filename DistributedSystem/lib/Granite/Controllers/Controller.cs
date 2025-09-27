namespace Granite.Controllers;

public class Controller 
{
    protected Dictionary<ConsoleKey, Action?> _keyActionDict = new();
    public Action<ConsoleKeyInfo>? KeyPressed;
    
    public event Action<bool>? Focused;
    protected bool _isFocused;

    public virtual void OnKeyPressed(ConsoleKeyInfo key)
    {
        KeyPressed?.Invoke(key);
        
        if (_keyActionDict.ContainsKey(key.Key))
        {
            _keyActionDict[key.Key]?.Invoke();
        }
    }

    public virtual void OnFocused(bool isFocused)
    {
        _isFocused = isFocused;
        Focused?.Invoke(isFocused);
    }

    public void AddKeyAction(ConsoleKey key, Action action)
    {
        if (_keyActionDict.ContainsKey(key))
        {
            _keyActionDict[key] += action;
        }
        else
        {
            _keyActionDict.Add(key, action);
        }
    }

    public void RemoveKeyAction(ConsoleKey key, Action action)
    {
        if (_keyActionDict.ContainsKey(key) && _keyActionDict[key] != null)
        {
            _keyActionDict[key] -= action;
        }
        else
        {
            _keyActionDict.Remove(key);
        }
    }
    
    public bool IsFocused { get => _isFocused; protected set => _isFocused = value; }
}
