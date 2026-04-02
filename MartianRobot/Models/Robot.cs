namespace MartianRobot.Models;

public class Robot
{
    public Position Position { get; set; }
    public Heading Heading { get; set; }
    public bool IsLost { get; set; }

    public Robot(int startX, int startY, Heading initialHeading = Heading.North)
    {
        Position = new Position(startX, startY);
        Heading = initialHeading;
        IsLost = false;
    }

    public override string ToString()
    {
        string status = IsLost ? " LOST" : "";
        return $"{Position.X} {Position.Y} {ToDisplayValue(Heading)}{status}";
    }

    private static string ToDisplayValue(Heading heading)
    {
        return heading switch
        {
            Heading.North => "N",
            Heading.East => "E",
            Heading.South => "S",
            Heading.West => "W",
            _ => throw new ArgumentOutOfRangeException(nameof(heading))
        };
    }
}