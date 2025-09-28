using System.Text;
using DistributedSystem.Logger;
using DistributedSystem.Terminal.DefaultCommands;

namespace DistributedSystem.Terminal;

public class CommandPanel : ICommandPanel
{
    private readonly object _locker = new object();

    private readonly ILogger _logger = new ConsoleLogger();
    
    private readonly Dictionary<string, ICommand> _nameCommandDict = new();
    
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
        Clear();
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
        var args =  _input.ToString().Split(' ');
        
        ClearInputView();
        _input.Clear();

        if (_nameCommandDict.TryGetValue(args[0], out var command))
        {
            var argsList = args.Skip(1).ToList(); 
            
            command.Execute(
                Enumerable.Range(0, argsList.Count / 2)
                    .ToDictionary(
                        i => argsList[2 * i],        // key
                        i => argsList[2 * i + 1]     // value
                    )    
            );
        }
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

    public void ShowMessageAction(Action printAction)
    {
        ClearInputView();
        
        lock (_locker)
        {

            Console.SetCursorPosition(0, _messageLine);
            printAction();

            _messageLine = Console.CursorTop;
            _inputLine = Console.BufferHeight - 1;
        }
        
        ShowInput();
    }

    public event EventHandler<string>? MessageSent;
    
    public void AddCommand(ICommand command)
    {
        if (!_nameCommandDict.TryAdd(command.Name, command)) 
            LogWarning($"Command <{command.Name}> already registered!");
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
        AddCommand(new ClearCommand(this));
        AddCommand(new HelpCommand(this));
        LogInfo("Use: [help -c <command>] to view command information.");
    }

    public void Clear()
    {
        lock (_locker)
        {
            Console.Clear();
            _messageLine = Console.CursorTop;
        }
        
        ShowInput();
    }

    public void Help(string? context = null)
    {
        if (context is null)
        {
            ShowMessageAction(() =>
            {
                Console.WriteLine("Commands:");
                foreach (var command in _nameCommandDict.Keys)
                    Console.WriteLine("- " + command);
            });
        }
        else
        {
            if (_nameCommandDict.TryGetValue(context, out var command))
            {
                ShowMessageAction(() =>
                {
                    Console.WriteLine(command.Description);
                });
            }
            else LogWarning("Undefined command!");
        }
    }
}