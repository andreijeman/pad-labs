using System.Reflection;
using Granite.Graphics.Components;
using Granite.Graphics.Maths;
namespace Granite.Graphics.Utilities;

public static class ModelBuilder
{
    public static Model Init(this Model model)
    {
        for (int i = 0; i < model.Height; i++)
        {
            for (int j = 0; j < model.Width; j++)
            {
                model.Data[i, j] = new Cell() { Character = ' '};
            }
        }

        return model;
    }

    public static Model FillCharacter(this Model model, char character)
    {
        for (int i = 0; i < model.Height; i++)
        {
            for (int j = 0; j < model.Width; j++)
            {
                model.Data[i, j].Character = character;
            }
        }

        return model;
    }
    
    public static Model FillBackground(this Model model, Color color)
    {
        for (int i = 0; i < model.Height; i++)
        {
            for (int j = 0; j < model.Width; j++)
            {
                model.Data[i, j].Background = color;
            }
        }

        return model;
    }

    public static Model FillBackground(this Model model, Color color, Rect section)
    {
        for (int i = section.Y1; i <= section.Y2; i++)
        {
            for (int j = section.X1; j <= section.X2; j++)
            {
                model.Data[i, j].Background = color;
            }
        }

        return model;
    }
    
    public static Model FillForeground(this Model model, Color color)
    {
        for (int i = 0; i < model.Height; i++)
        {
            for (int j = 0; j < model.Width; j++)
            {
                model.Data[i, j].Foreground = color;
            }
        }

        return model;
    }
    
    public static Model FillForeground(this Model model, Color color, Rect section)
    {
        for (int i = section.Y1; i <= section.Y2; i++)
        {
            for (int j = section.X1; j <= section.X2; j++)
            {
                model.Data[i, j].Foreground = color;
            }
        }

        return model;
    }

    public static Model DrawChessboard(this Model model, Color color1, Color color2)
    {
        Color color;

        for (int i = 0; i < model.Height; i++)
        {
            for (int j = 0; j < model.Width / 2; j++)
            {
                color = (i + j) % 2 == 0 ? color1 : color2;

                model.Data[i, 2 * j].Background = color;
                model.Data[i, 2 * j + 1].Background= color;
            }
        }

        return model;
    }

    public static Model InsertTextCentered(this Model model, string text, Color color)
    {
        var rect = GetCenteredInnerBox(model.Width, model.Height, text.Length);

        int index = 0;
        for (int i = rect.Y1; i <= rect.Y2; i++)
        {
            while(index < text.Length && text[index] == ' ') index++;
            
            for (int j = rect.X1; j <= rect.X2; j++)
            {
                if (index >= text.Length) goto End; 
                model.Data[i, j].Character = text[index++];
                model.Data[i, j].Foreground = color;
            }
        }
        
        End:
        
        return model;
    }
    
    private static Rect GetCenteredInnerBox(int outerWidth, int outerHeight, int innerArea)
    {
        if(innerArea >= outerWidth * outerHeight) 
            return new Rect(0, 0, outerWidth - 1, outerHeight - 1);
        
        double ratio = outerWidth / outerHeight;
        int h = (int)Math.Ceiling((Math.Sqrt(innerArea / ratio)));
        int w = (int)Math.Ceiling( h * ratio);
        
        int x = (outerWidth - (w - 1)) / 2;
        int y = (outerHeight - (h - 1)) / 2;

        return new Rect
        {
            X1 = x,
            Y1 = y,
            X2 = x + w,
            Y2 = y + h
        };
    }
    
    public static Model DrawRectangle(this Model model, Rect section, Char character, Color color)
    {
        for (int i = section.Y1; i <= section.Y2; i++)
        {
            for (int j = section.X1; j <= section.X2; j++)
            {
                model.Data[i, j].Character = character;
                model.Data[i, j].Foreground = color;
            }
        }

        return model;
    }

    public static Model DrawBorder(this Model model, Border border, Color color)
    {
        return model.DrawBorder(new Rect(0, 0, model.Width - 1, model.Height - 1), border, color);
    }
    
    public static Model DrawBorder(this Model model, Rect section, Border border, Color color)
    {
        model.DrawRectangle(new Rect { X1 = section.X1 + 1, Y1 = section.Y1, X2 = section.X2 - 1, Y2 = section.Y1 }, border.Top, color);
        model.DrawRectangle(new Rect { X1 = section.X1 + 1, Y1 = section.Y2, X2 = section.X2 - 1, Y2 = section.Y2 }, border.Bottom, color);
        model.DrawRectangle(new Rect { X1 = section.X1, Y1 = section.Y1 + 1, X2 = section.X1, Y2 = section.Y2 - 1 }, border.Left, color);
        model.DrawRectangle(new Rect { X1 = section.X2, Y1 = section.Y1 + 1, X2 = section.X2, Y2 = section.Y2 - 1 }, border.Right, color);

        model.Data[section.Y1, section.X1].Character = border.LeftTop;
        model.Data[section.Y1, section.X2].Character = border.RightTop;
        model.Data[section.Y2, section.X2].Character = border.RightBottom;
        model.Data[section.Y2, section.X1].Character = border.LeftBottom;
        
        model.Data[section.Y1, section.X1].Foreground = color;
        model.Data[section.Y1, section.X2].Foreground = color;
        model.Data[section.Y2, section.X2].Foreground = color;
        model.Data[section.Y2, section.X1].Foreground = color;

        return model;
    }
    
    public struct Border
    {
        public char Left, Top, Right, Bottom, LeftTop, RightTop, RightBottom, LeftBottom;

        public Border(char left, char top, char right, char bottom, char leftTop, char rightTop, char rightBottom, char leftBottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
            this.LeftTop = leftTop;
            this.RightTop = rightTop;
            this.RightBottom = rightBottom;
            this.LeftBottom = leftBottom;
        }
    }

    public static class Assets
    {
        public static readonly Border LineBorder = new Border('│', '─', '│', '─', '┌', '┐', '┘', '└');
        public static readonly Border DoubleLineBorder = new Border('║', '═', '║', '═', '╔', '╗', '╝', '╚');
        public static readonly Border FatBorder = new Border('█', '▀', '█', '▄', '█', '█', '█', '█');
    }
}
