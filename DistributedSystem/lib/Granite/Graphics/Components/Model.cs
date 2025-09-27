namespace Granite.Graphics.Components;

public class Model
{
    public readonly Cell[,] Data;
    
    public readonly int Width;
    public readonly int Height;

    public Model(int  width, int height) 
    {
        Width = width;
        Height = height;

        Data = new Cell[height, width];
    }

}
