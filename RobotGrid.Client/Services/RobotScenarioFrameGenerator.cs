using MartianRobot.Commands;
using MartianRobot.Models;
using MartianRobot.Services;

namespace RobotGrid.Client.Services;

public sealed class RobotScenarioFrameGenerator
{
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

        return CreateFrame(
            grid,
            startRobot.Position,
            startRobot.Heading,
            normalizedInstructions,
            currentRobot,
            visitedPositions);
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
            startRobot.Position,
            startRobot.Heading,
            normalizedInstructions,
            currentRobot,
            visitedPositions);

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

            if (!executor.TryExecuteCommand(currentRobot, commandSymbol, out IRobotInstructionCommand? command) || command is null)
            {
                throw new ArgumentException(
                    $"Invalid command: {commandSymbol}. Only {executor.AllowedCommandsText} are allowed.");
            }

            TrackVisitedPosition(currentRobot, grid, visitedPositions);

            yield return CreateFrame(
                grid,
                startRobot.Position,
                startRobot.Heading,
                normalizedInstructions,
                currentRobot,
                visitedPositions);
        }
    }

    private static RobotGridAnimationFrame CreateFrame(
        Grid grid,
        Position startPosition,
        Heading startHeading,
        string instructions,
        Robot currentRobot,
        HashSet<(int X, int Y)> visitedPositions)
    {
        return new RobotGridAnimationFrame(
            grid.Width,
            grid.Height,
            startPosition,
            startHeading,
            instructions,
            currentRobot.Position,
            currentRobot.Heading,
            currentRobot.IsLost,
            [.. visitedPositions.Select(position => new Position(position.X, position.Y))],
            [.. grid.LostPositions.Select(position => new Position(position.X, position.Y))]);
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

