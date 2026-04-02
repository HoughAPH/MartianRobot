namespace MartianRobot.Models;

public sealed class GridState
{
    public int Width { get; init; }
    public int Height { get; init; }
    public HashSet<(int X, int Y)> LostPositions { get; } = [];

    public bool IsWithinBounds(int x, int y) =>
        x >= 0 && x <= Width && y >= 0 && y <= Height;

    public bool IsLostPosition(int x, int y) =>
        LostPositions.Contains((x, y));

    public void AddLostPosition(int x, int y) =>
        LostPositions.Add((x, y));
}