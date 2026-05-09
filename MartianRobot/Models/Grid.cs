namespace MartianRobot.Models;

public sealed class Grid(int width = 50, int height = 50)
{
    public int Width { get; set; } = width;
    public int Height { get; set; } = height;
    public HashSet<(int X, int Y)> LostPositions { get; } = [];

    public bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x <= Width && y >= 0 && y <= Height;
    }

    public bool IsLostPosition(int x, int y)
    {
        return LostPositions.Contains((x, y));
    }

    public void AddLostPosition(int x, int y)
    {
        LostPositions.Add((x, y));
    }

    public void Reset()
    {
        LostPositions.Clear();
    }
}
