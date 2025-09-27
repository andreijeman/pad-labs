using Granite.Controllers;

namespace Granite.IO;

public static class KeyboardInput
{
    public static bool IsRunning { get; private set; } = false;
    private static ControllerHolder _ctrlHolder = new();
    private static event Action<ConsoleKeyInfo>? KeyPressed;

    private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    static KeyboardInput()
    {
        KeyPressed += _ctrlHolder.OnKeyPressed;
        _ctrlHolder.OnFocused(true);  
    }

    public static void Bind(Controller ctrl)
    {
        if (!IsRunning)  
        { 
            Start();
            IsRunning = true;
        }
        
        _ctrlHolder.Add(ctrl);
    }
    
    private static async Task ListenAsync(CancellationToken cancellationToken)
    {
        ConsoleKeyInfo key;

        while (!cancellationToken.IsCancellationRequested)
        {
            if (Console.KeyAvailable)
            {
                key = Console.ReadKey(intercept: true);
                KeyPressed?.Invoke(key);   
            }

            await Task.Delay(20);
        }
    }

    private static void Start()
    {
        Task.Run(() => ListenAsync(_cancellationTokenSource.Token));
    }

    private static void Stop()
    {
        _cancellationTokenSource.Cancel();
    }
}
