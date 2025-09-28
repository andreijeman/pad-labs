using System.Text;
using DistributedSystem.Logger;

namespace DistributedSystem.Terminal;

public class CommandPanel : ICommandPanel
{
    private readonly object _locker = new object();

    private readonly ILogger _logger = new ConsoleLogger();
    
    private readonly Dictionary<string, CommandAction> _commandActionDict = new();
    
    private readonly StringBuilder _input = new();
    private const int InputCollOffset = 2;
    
    private int _inputLine;
    private int _messageLine;

    public CommandPanel()
    {
        SetupDefaultCommands();
    }
    
    
    
    public void Start(CancellationToken cancellationToken = default)
    {
        _messageLine = Console.CursorTop;
        _inputLine = Console.BufferHeight - 1;
        
        ShowInput();
        
        while (!cancellationToken.IsCancellationRequested)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            switch (keyInfo.Key)
            {
                case ConsoleKey.Enter:
                    HandleEnterBtn();
                    break;
                case ConsoleKey.Backspace:
                    HandleBackspaceBtn();
                    break;
                default:
                    HandleCharacter(keyInfo.KeyChar);
                    break;
            }
        }
    }

    private void HandleCharacter(char character)
    {
        lock (_locker)
        {
            Console.SetCursorPosition(InputCollOffset + _input.Length, _inputLine);
            Console.Write(character);
            _input.Append(character);
        }
    }
    
    private void HandleEnterBtn()
    {
        var options =  _input.ToString().Split(' ');
        
        ClearInputView();
        _input.Clear();
        
        if (_commandActionDict.TryGetValue(options[0], out var command))
            command(options.Skip(1).ToList(), this);
        else LogWarning("Undefined command!");
    }

    private void HandleBackspaceBtn()
    {
        if (_input.Length > 0)
        {
            lock (_locker)
            {
                _input.Remove(_input.Length - 1, 1);
                Console.SetCursorPosition(InputCollOffset + _input.Length, _inputLine);
                Console.Write(' ');
                Console.SetCursorPosition(InputCollOffset + _input.Length, _inputLine);
            }
        }
    }
    
    private void ShowInput()
    {
        lock(_locker)
        {
            Console.SetCursorPosition(0, _inputLine);
            Console.Write($": {_input}");
        }
    }
    
    private void ClearInputView()
    {
        lock(_locker)
        {
            Console.SetCursorPosition(0, _inputLine);
            Console.Write(new string(' ', _input.Length + InputCollOffset));
        }
    }
    
    public void OnMessageReceived(string message)
    {
        ShowMessageAction(() => Console.WriteLine(message));
    }

    private void ShowMessageAction(Action printAction)
    {
        lock (_locker)
        {
            ClearInputView();

            Console.SetCursorPosition(0, _messageLine);
            printAction();

            _messageLine = Console.CursorTop;
            _inputLine = Console.BufferHeight - 1;

            ShowInput();
        }
    }

    public event EventHandler<string>? MessageSent;
    
    public void AddCommand(string command, CommandAction action)
    {
        if (_commandActionDict.TryAdd(command, action)) ;
        else LogWarning("Command already exists!");
    }

    public void LogInfo(string message)
    {
        ShowMessageAction(() => _logger.LogInfo(message));
    }

    public void LogWarning(string message)
    {
        ShowMessageAction(() => _logger.LogWarning(message));
    }

    public void LogError(string message)
    {
        ShowMessageAction(() => _logger.LogError(message));
    }

    private void SetupDefaultCommands()
    {
        _commandActionDict.TryAdd("clear", (options, logger) =>
        {
            Console.Clear();
            _messageLine = Console.CursorTop;
            ShowInput();
        });

        _commandActionDict.TryAdd("help", (options, logger) =>
        {
            ShowMessageAction(() =>
            {
                Console.WriteLine("Commands:");
                foreach (var command in _commandActionDict.Keys)
                    Console.WriteLine(command);
            });
        });
    }
}