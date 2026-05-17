using MartianRobot.Models;
using System.Text;

namespace RobotGrid.Client.Services;

public sealed class RobotGridTextRenderer
{
    private const char VisitedCellSymbol = '*';
    private const int MinimumCellWidth = 3;

    public static string BuildGridText(
        int width,
        int height,
        Robot startRobot,
        string instructions,
        Robot currentRobot,
        IReadOnlySet<(int X, int Y)> lostPositions,
        IReadOnlySet<(int X, int Y)> visitedPositions,
        string statusLabel = "Current",
        string? currentCommandDescription = null)
    {
        StringBuilder sb = new();
        int cellWidth = GetCellWidth(width);
        const string axisPadding = "  ";
        const string verticalBorder = "| ";

        AppendSummaryLine(sb, "Start", startRobot.ToString());
        AppendSummaryLine(sb, "Instructions", instructions);
        AppendSummaryLine(sb, statusLabel, currentRobot.ToString());

        if (!string.IsNullOrWhiteSpace(currentCommandDescription))
        {
            AppendSummaryLine(sb, "Move", currentCommandDescription);
        }

        sb.AppendLine();

        DrawHorizontalBorder(sb, width, cellWidth, axisPadding);

        for (int y = height; y >= 0; y--)
        {
            DrawVerticalBorder(sb, verticalBorder);

            for (int x = 0; x <= width; x++)
            {
                string cellText = GetCellText(x, y, currentRobot, lostPositions, visitedPositions);
                sb.Append(cellText.PadRight(cellWidth));
            }

            DrawVerticalBorder(sb, verticalBorder);
            DrawYAxisLabel(sb, y);
            sb.AppendLine();
        }

        DrawHorizontalBorder(sb, width, cellWidth, axisPadding);
        DrawXAxis(sb, width, cellWidth, axisPadding);

        return sb.ToString();
    }

    private static void AppendSummaryLine(StringBuilder sb, string label, string value)
    {
        sb.AppendLine($"{label}:".PadRight(14) + value);
    }

    private static void DrawHorizontalBorder(StringBuilder sb, int width, int cellWidth, string leftPadding)
    {
        sb.Append(leftPadding);

        for (int x = 0; x <= width; x++)
        {
            sb.Append(new string('-', cellWidth));
        }

        sb.AppendLine();
    }

    private static void DrawVerticalBorder(StringBuilder sb, string borderText)
    {
        sb.Append(borderText);
    }

    private static void DrawYAxisLabel(StringBuilder sb, int y)
    {
        sb.Append(y);
    }

    private static void DrawXAxis(StringBuilder sb, int width, int cellWidth, string leftPadding)
    {
        sb.Append(leftPadding);

        for (int x = 0; x <= width; x++)
        {
            sb.Append(x.ToString().PadRight(cellWidth));
        }

        sb.AppendLine();
    }

    private static int GetCellWidth(int width)
    {
        return Math.Max(MinimumCellWidth, width.ToString().Length + 1);
    }

    private static string GetCellText(
        int x,
        int y,
        Robot currentRobot,
        IReadOnlySet<(int X, int Y)> lostPositions,
        IReadOnlySet<(int X, int Y)> visitedPositions)
    {
        if (currentRobot.Position.X == x && currentRobot.Position.Y == y)
        {
            return GetRobotSymbol(currentRobot).ToString();
        }

        if (lostPositions.Contains((x, y)))
        {
            return "X";
        }

        if (visitedPositions.Contains((x, y)))
        {
            return VisitedCellSymbol.ToString();
        }

        return ".";
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