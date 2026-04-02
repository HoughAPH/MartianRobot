using System.Text;
using MartianRobot.Models;

namespace RobotGrid.Client.Services;

public sealed class RobotGridTextRenderer
{
    private const char VisitedCellSymbol = '*';

    public string BuildGridText(
        int width,
        int height,
        Robot startRobot,
        string instructions,
        Robot currentRobot,
        IReadOnlySet<(int X, int Y)> lostPositions,
        IReadOnlySet<(int X, int Y)> visitedPositions,
        string statusLabel = "Current")
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Start:        {startRobot}");
        sb.AppendLine($"Instructions: {instructions}");
        sb.AppendLine($"{statusLabel}:".PadRight(14) + currentRobot);
        sb.AppendLine();

        for (int y = height; y >= 0; y--)
        {
            for (int x = 0; x <= width; x++)
            {
                if (currentRobot.Position.X == x && currentRobot.Position.Y == y)
                {
                    sb.Append(GetRobotSymbol(currentRobot));
                    sb.Append(' ');
                    continue;
                }

                if (lostPositions.Contains((x, y)))
                {
                    sb.Append('X');
                    sb.Append(' ');
                    continue;
                }

                if (visitedPositions.Contains((x, y)))
                {
                    sb.Append(VisitedCellSymbol);
                    sb.Append(' ');
                    continue;
                }

                sb.Append(". ");
            }

            sb.AppendLine($"| {y}");
        }

        for (int x = 0; x <= width; x++)
        {
            sb.Append("--");
        }

        sb.AppendLine();

        for (int x = 0; x <= width; x++)
        {
            sb.Append($"{x % 10} ");
        }

        sb.AppendLine();

        return sb.ToString();
    }

    private static char GetRobotSymbol(Robot robot) => robot.Heading switch
    {
        Heading.North => '^',
        Heading.East => '>',
        Heading.South => 'v',
        Heading.West => '<',
        _ => '?'
    };
}