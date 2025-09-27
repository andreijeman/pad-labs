using Granite.Graphics.Maths;
using Granite.UI.EntitiesArgs;

namespace Granite.UI.Entities;

public class InputBox : TextBox
{
    public event Action<string>? EnterKeyPressed;
    private char _cursor;
    
    public InputBox(InputBoxArgs args) : base(args)
    {
        Controller.AddKeyAction(ConsoleKey.Backspace, OnBackspaceKeyPressed);
        Controller.AddKeyAction(ConsoleKey.Enter, OnEnterKeyPressed);
        Controller.KeyPressed += OnTextKeyPressed;

        _cursor = args.Cursor;
    }

    private void OnBackspaceKeyPressed()
    {
        _chunk.Model.Data[_chunkY, _chunkX].Character = ' ';
        _chunk.Draw(new Rect(_chunkX, _chunkY, _chunkX, _chunkY));
        
        _chunkX--;
        if (_chunkX < 0)
        {
            _chunkX = _chunk.Width - 1;
            _chunkY--;

            if (_chunkY < 0)
            {
                if (_chunks.Count > 1)
                {
                    _chunks.Remove(_chunk);
                    _frame.Remove(_chunk);
                    _chunk = _chunks.Last();
                    
                    _chunkY = _chunk.Height - 1;
                }
                else
                {
                    _chunkX = _chunkY = 0;
                }
            }

            if (_frame.OriginTop > 0) _frame.OriginTop--;
        };
        
        _chunk.Model.Data[_chunkY, _chunkX].Character = _cursor;
        _chunk.Draw(new Rect(_chunkX, _chunkY, _chunkX, _chunkY));
    }
    
    private void OnEnterKeyPressed()
    {
        EnterKeyPressed?.Invoke(_text.ToString());
        Clear();
    }

    private void OnTextKeyPressed(ConsoleKeyInfo keyInfo)
    {
        if (!char.IsControl(keyInfo.KeyChar))
            AddText(char.ToString(keyInfo.KeyChar));
        
        _chunk.Model.Data[_chunkY, _chunkX].Character = _cursor;
        _chunk.Draw(new Rect(_chunkX, _chunkY, _chunkX, _chunkY));

        if (_chunk.Top + _chunkY - _frame.OriginTop == _chunk.Height) _frame.OriginTop++;
    }
}