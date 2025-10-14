using System.Drawing;
using System.Text;
using GrpcDS.Logger;
using GrpcDS.Terminal.DefaultCommands;

namespace GrpcDS.Terminal;

public class CommandPanel : ICommandPanel
{
    private readonly object _locker = new object();

    private readonly ILogger _logger = new ConsoleLogger();
    
    private readonly Dictionary<string, ICommand> _nameCommandDict = new();
    
    private readonly StringBuilder _input = new();
    
    private Point _inputCursorPos;
    private int _inputIndex;
    private int _messageLine;

    private const int InputCursorPosYOffset = 2;
    private const string InputHowToNameIt = ": ";
    
    public CommandPanel()
    {
        SetupDefaultCommands();
    }
    
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        SetupEnvironmentVariables();
        LogInfo("Use: [help] and [help -c <command>] to view command information.");

        return Task.Run(() =>
        {
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
                    case ConsoleKey.LeftArrow:
                        MoveCursorLeft();
                        break;
                    case ConsoleKey.RightArrow:
                        MoveCursorRight();
                        break;
                    default:
                        HandleCharacter(keyInfo.KeyChar);
                        break;
                }
            }
        });
    }

    private void HandleCharacter(char character)
    {
        lock (_locker)
        {
            if (_inputCursorPos.X < Console.BufferWidth - 1)
            {
                Console.SetCursorPosition(_inputCursorPos.X, _inputCursorPos.Y);
                Console.Write(character + _input.ToString(_inputIndex, _input.Length - _inputIndex));

                _input.Insert(_inputIndex, character);
                _inputCursorPos.X++;
                _inputIndex++;

                Console.SetCursorPosition(_inputCursorPos.X, _inputCursorPos.Y);
            }
            else
            {
                LogError("Why do you write so much? Are you dumb!?");
            }
        }
    }
    
    private void HandleEnterBtn()
    {
        ExecuteTextCommand(_input.ToString());
        ClearInputView();
        ResetInput();
        ShowInput();
    }

    private void HandleBackspaceBtn()
    {
        if (_inputIndex > 0)
        {
            lock (_locker)
            {
                Console.SetCursorPosition(InputCursorPosYOffset + _input.Length - 1, _inputCursorPos.Y);
                Console.Write(' ');
                
                _inputIndex--;
                _inputCursorPos.X--;
                _input.Remove(_inputIndex, 1);
                
                Console.SetCursorPosition(_inputCursorPos.X, _inputCursorPos.Y);
                Console.Write(_input.ToString(_inputIndex, _input.Length - _inputIndex));
                
                Console.SetCursorPosition(_inputCursorPos.X, _inputCursorPos.Y);
            }
        }
    }

    private void MoveCursorLeft()
    {
        lock (_locker)
        {
            if (_inputIndex > 0)
            {
                _inputCursorPos.X--;
                _inputIndex--;
                Console.SetCursorPosition(_inputCursorPos.X, _inputCursorPos.Y);
            }
        }
    }
    
    private void MoveCursorRight()
    {
        lock (_locker)
        {
            if (_inputIndex < _input.Length)
            {
                _inputCursorPos.X++;
                _inputIndex++;
                Console.SetCursorPosition(_inputCursorPos.X, _inputCursorPos.Y);
            }
        }
    }
    
    private void ShowInput()
    {
        lock(_locker)
        {
            Console.SetCursorPosition(0, _inputCursorPos.Y);
            Console.Write($": {_input}");
            Console.SetCursorPosition(_inputCursorPos.X, _inputCursorPos.Y);
        }
    }
    
    private void ClearInputView()
    {
        lock(_locker)
        {
            Console.SetCursorPosition(0, _inputCursorPos.Y);
            Console.Write(new string(' ', InputCursorPosYOffset + _input.Length));
        }
    }

    private void ResetInput()
    {
        lock (_locker)
        {
            _input.Clear();
            _inputCursorPos.X = InputCursorPosYOffset;
            _inputCursorPos.Y = Console.BufferHeight - 1;
            _inputIndex = 0;
        }
    }

    public void ExecuteTextCommand(string text)
    {
        LogInfo("Executing command: " + text);
        
        var args =  text.Split(' ');
        
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

    public void ShowMessageAction(Action printAction)
    {
        ClearInputView();
        
        lock (_locker)
        {
            Console.SetCursorPosition(0, _messageLine);
            printAction();

            _messageLine = Console.CursorTop;
            _inputCursorPos.Y = Console.BufferHeight - 1;
        }
        
        ShowInput();
    }

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
    }

    public void Clear()
    {
        lock (_locker)
        {
            Console.Clear();
            _messageLine = Console.CursorTop;
        }
        
        ResetInput();
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

    private void SetupEnvironmentVariables()
    {
        _messageLine = Console.CursorTop;
        _inputCursorPos.X = InputCursorPosYOffset;
        _inputCursorPos.Y = Console.BufferHeight - 1;
        _inputIndex = 0;
    }
}