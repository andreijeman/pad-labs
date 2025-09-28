namespace Logger;

public class ConsoleLogger : ILogger
{
    public void LogInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"[{DateTime.Now}] ");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Info: ");

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);
    }

    public void LogWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"[{DateTime.Now}] ");

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write("Warning: ");

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);
    }

    public void LogError(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"[{DateTime.Now}] ");

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("Error: ");

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);            
        
    }
}
