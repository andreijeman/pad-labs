using System.Text;
using Granite.Graphics.Components;
using Granite.Graphics.EventArgs;
using Granite.Graphics.Frames;
using Granite.Graphics.Objects;
using Granite.Graphics.Utilities;

namespace Granite.IO;

public static class ConsoleOutput 
{
    private static readonly Lock _locker = new();
    
    private static readonly Frame _frame = new();
    
    static ConsoleOutput()
    {
        _frame.DrawRequested += OnDrawRequested;
        Console.CursorVisible = false;
        Task.Run(OnBufferSizeChanged);
    }

    public static void Bind(GObject obj)
    {
        _frame.Add(obj);
    }

    private static async Task OnBufferSizeChanged()
    {
        while (true)
        {
            if (_frame.Width != Console.BufferWidth - 1 || _frame.Height != Console.BufferHeight - 1)
            {
                _frame.Model = new Model(Console.BufferWidth - 1, Console.BufferHeight - 1).Init();
                _frame.Draw();
            }

            await Task.Delay(1000);
        }
    }
    
    private static void OnDrawRequested(GObject sender, DrawEventArgs args)
    {
        lock (_locker)
        {
            try
            {
                for (int i = args.Section.Y1; i <= args.Section.Y2; i++)
                {
                    Console.SetCursorPosition(args.Left + args.Section.X1, args.Top++ + args.Section.Y1);
                    StringBuilder result = new();

                    for (int j = args.Section.X1; j <= args.Section.X2; j++)
                    {
                        Cell cell = args.Model.Data[i, j];

                        result.Append(
                            RgbToAnsiEsForeground(cell.Foreground.R, cell.Foreground.G, cell.Foreground.B) +
                            RgbToAnsiEsBackground(cell.Background.R, cell.Background.G, cell.Background.B) +
                            cell.Character);
                    }

                    Console.Write(result);
                }
            }
            catch(Exception)
            {
                //Console.WriteLine(ex.Message);
            }
        }
    }
    
    private static string RgbToAnsiEsForeground(int r, int g, int b) => $"\u001b[38;2;{r};{g};{b}m";
    private static string RgbToAnsiEsBackground(int r, int g, int b) => $"\u001b[48;2;{r};{g};{b}m";
}
