namespace MartianRobot.Models;

public sealed record RobotGridAnimationFrame(
    int Width,
    int Height,
    Position StartPosition,
    Heading StartHeading,
    string Instructions,
    Position CurrentPosition,
    Heading CurrentHeading,
    bool IsLost,
    IReadOnlyCollection<Position> VisitedPositions,
    IReadOnlyCollection<Position> LostPositions);