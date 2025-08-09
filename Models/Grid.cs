namespace MartianRobot.Models;

public static class Grid
{
    public static int Width { get; set; } = 50;
    public static int Height { get; set; } = 50;
    public static HashSet<(int X, int Y)> LostPositions { get; } = new HashSet<(int, int)>();
    public static readonly string[] HeadingsIndex = ["N", "E", "S", "W"];
    
    public static bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x <= Width && y >= 0 && y <= Height;
    }
    
    public static bool IsLostPosition(int x, int y)
    {
        return LostPositions.Contains((x, y));
    }

    public static void AddLostPosition(int x, int y)
    {
        LostPositions.Add((x, y));
    }
    
    public static void Reset()
    {
        LostPositions.Clear();
    }
}