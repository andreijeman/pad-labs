namespace Granite.Controllers;

public class ControllerHolder : Controller
{
    private ControllerHolder? _parent;
    private List<Controller> _controllers = new();

    private Controller? _currentCtrl;
    private int _currentCtrlIndex = -1;

    protected bool _isSelected = false;
    
    private ConsoleKey _selectKey = ConsoleKey.Enter;
    private ConsoleKey _exitKey = ConsoleKey.Escape;
    private ConsoleKey _nextKey = ConsoleKey.Tab;

    public ControllerHolder()
    {
        AddKeyAction(_selectKey, OnSelectKey);
        AddKeyAction(_exitKey, OnExitKey);
        AddKeyAction(_nextKey, OnNextKey);
    }

    public override void OnKeyPressed(ConsoleKeyInfo key)
    {
        if (_currentCtrl is ControllerHolder holder)
        { 
            if(holder._isSelected) _currentCtrl.OnKeyPressed(key);
            else base.OnKeyPressed(key);
        }
        else
        {
            base.OnKeyPressed(key);
            if(key.Key != _nextKey && key.Key != _exitKey) _currentCtrl?.OnKeyPressed(key);
        }
    }
    private void OnSelectKey()
    {
        if(_currentCtrl is ControllerHolder holder)
            holder._isSelected = true;
    }

    private void OnExitKey()
    {
        _currentCtrl?.OnFocused(false);
        _currentCtrl = null;
        _currentCtrlIndex = -1;
        
        _isSelected = false;
    }

    private void OnNextKey()
    {
        if (_currentCtrlIndex < _controllers.Count - 1) _currentCtrlIndex++;
        else _currentCtrlIndex = 0;

        _currentCtrl?.OnFocused(false);
        _currentCtrl = _controllers[_currentCtrlIndex];
        _currentCtrl.OnFocused(true);
    }

    public void Add(Controller ctrl)
    {
        if (!_controllers.Contains(ctrl))
        {
            _controllers.Add(ctrl);
            if (ctrl is ControllerHolder holder) holder.Parent = this;
        }
    }

    public ControllerHolder? Parent { get => _parent; set => _parent = value; }

    public ConsoleKey SelectKey
    {
        get => _selectKey;
        set
        {
            RemoveKeyAction(_selectKey, OnSelectKey);
            _selectKey = value;
            AddKeyAction(_selectKey, OnNextKey);
        }
    }

    public ConsoleKey ExitKey
    {
        get => _exitKey;
        set
        {
            RemoveKeyAction(_exitKey, OnExitKey);
            _exitKey = value;
            AddKeyAction(_exitKey, OnNextKey);
        }
    }

    public ConsoleKey NextKey 
    { 
        get => _nextKey; 
        set
        {
            RemoveKeyAction(_nextKey, OnNextKey);
            _nextKey = value;
            AddKeyAction(_nextKey, OnNextKey);
        }
    }
}
