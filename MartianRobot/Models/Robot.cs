namespace MartianRobot.Models;

public class Robot(int startX, int startY, Heading initialHeading = Heading.North)
{
    private Heading _heading = initialHeading;

    public Position Position { get; set; } = new Position(startX, startY);

    public Heading Heading
    {
        get => _heading;
        set
        {
            if (!Enum.IsDefined(value))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    "The heading must be a defined Heading value.");
            }

            _heading = value;
        }
    }

    public bool IsLost { get; set; } = false;

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