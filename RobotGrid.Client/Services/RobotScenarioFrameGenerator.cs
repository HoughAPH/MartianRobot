using MartianRobot.Commands;
using MartianRobot.Models;
using MartianRobot.Services;

namespace RobotGrid.Client.Services;

public sealed class RobotScenarioFrameGenerator
{
    private readonly RobotGridTextRenderer _renderer = new();

    public RobotGridAnimationFrame BuildInitialFrame(
        Grid grid,
        Robot startRobot,
        string instructions,
        bool resetGrid = true)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(startRobot);
        ArgumentNullException.ThrowIfNull(instructions);

        if (resetGrid)
        {
            grid.Reset();
        }

        string normalizedInstructions = instructions.ToUpperInvariant();
        Robot currentRobot = CloneRobot(startRobot);
        HashSet<(int X, int Y)> visitedPositions = [];

        TrackVisitedPosition(currentRobot, grid, visitedPositions);

        string statusLabel = normalizedInstructions.Length == 0 ? "Final" : "Current";

        return CreateFrame(
            grid,
            startRobot,
            normalizedInstructions,
            currentRobot,
            visitedPositions,
            statusLabel);
    }

    public IEnumerable<RobotGridAnimationFrame> BuildFrames(
        Grid grid,
        Robot startRobot,
        string instructions,
        bool resetGrid = true)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(startRobot);
        ArgumentNullException.ThrowIfNull(instructions);

        if (resetGrid)
        {
            grid.Reset();
        }

        string normalizedInstructions = instructions.ToUpperInvariant();
        Robot currentRobot = CloneRobot(startRobot);
        RobotInstructionExecutor executor = new(grid);
        HashSet<(int X, int Y)> visitedPositions = [];

        TrackVisitedPosition(currentRobot, grid, visitedPositions);

        yield return CreateFrame(
            grid,
            startRobot,
            normalizedInstructions,
            currentRobot,
            visitedPositions,
            normalizedInstructions.Length == 0 ? "Final" : "Current");

        if (normalizedInstructions.Length == 0)
        {
            yield break;
        }

        for (int i = 0; i < normalizedInstructions.Length; i++)
        {
            if (currentRobot.IsLost)
            {
                yield break;
            }

            char commandSymbol = normalizedInstructions[i];

            if (!executor.TryExecuteCommand(currentRobot, commandSymbol, out IRobotInstructionCommand? command))
            {
                throw new ArgumentException(
                    $"Invalid command: {commandSymbol}. Only {executor.AllowedCommandsText} are allowed.");
            }

            TrackVisitedPosition(currentRobot, grid, visitedPositions);

            bool isFinalFrame = currentRobot.IsLost || i == normalizedInstructions.Length - 1;

            yield return CreateFrame(
                grid,
                startRobot,
                normalizedInstructions,
                currentRobot,
                visitedPositions,
                isFinalFrame ? "Final" : "Current",
                BuildCommandDescription(command));
        }
    }

    private RobotGridAnimationFrame CreateFrame(
        Grid grid,
        Robot startRobot,
        string instructions,
        Robot currentRobot,
        HashSet<(int X, int Y)> visitedPositions,
        string statusLabel,
        string? currentCommandDescription = null)
    {
        return new RobotGridAnimationFrame(
            RobotGridTextRenderer.BuildGridText(
                grid.Width,
                grid.Height,
                startRobot,
                instructions,
                currentRobot,
                grid.LostPositions,
                visitedPositions,
                statusLabel,
                currentCommandDescription));
    }

    private static string BuildCommandDescription(IRobotInstructionCommand command)
    {
        return $"{command.Symbol}: {command.CommandText}";
    }

    private static Robot CloneRobot(Robot robot)
    {
        return new Robot(robot.Position.X, robot.Position.Y, robot.Heading)
        {
            IsLost = robot.IsLost
        };
    }

    private static void TrackVisitedPosition(
        Robot robot,
        Grid grid,
        HashSet<(int X, int Y)> visitedPositions)
    {
        int x = robot.Position.X;
        int y = robot.Position.Y;

        if (x < 0 || x > grid.Width || y < 0 || y > grid.Height)
        {
            return;
        }

        visitedPositions.Add((x, y));
    }
}

