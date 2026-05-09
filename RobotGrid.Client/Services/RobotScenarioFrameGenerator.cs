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

        var normalizedInstructions = instructions.ToUpperInvariant();
        var currentRobot = CloneRobot(startRobot);
        HashSet<(int X, int Y)> visitedPositions = [];

        TrackVisitedPosition(currentRobot, grid, visitedPositions);

        var statusLabel = normalizedInstructions.Length == 0 ? "Final" : "Current";

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

        var normalizedInstructions = instructions.ToUpperInvariant();
        var currentRobot = CloneRobot(startRobot);
        var executor = new RobotInstructionExecutor(grid);
        HashSet<(int X, int Y)> visitedPositions = [];

        TrackVisitedPosition(currentRobot, grid, visitedPositions);
        //this return a yield frame instead of adding to a list of frames
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

        for (var i = 0; i < normalizedInstructions.Length; i++)
        {
            if (currentRobot.IsLost)
            {
                yield break;
            }

            var command = normalizedInstructions[i];

            if (!executor.TryExecuteCommand(currentRobot, command))
            {
                throw new ArgumentException($"Invalid command: {command}. Only F, L, and R are allowed.");
            }

            TrackVisitedPosition(currentRobot, grid, visitedPositions);

            var isFinalFrame = currentRobot.IsLost || i == normalizedInstructions.Length - 1;

            yield return CreateFrame(
                grid,
                startRobot,
                normalizedInstructions,
                currentRobot,
                visitedPositions,
                isFinalFrame ? "Final" : "Current");
        }
    }

    private RobotGridAnimationFrame CreateFrame(
        Grid grid,
        Robot startRobot,
        string instructions,
        Robot currentRobot,
        HashSet<(int X, int Y)> visitedPositions,
        string statusLabel)
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
                statusLabel));
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
        var x = robot.Position.X;
        var y = robot.Position.Y;

        if (x < 0 || x > grid.Width || y < 0 || y > grid.Height)
        {
            return;
        }

        visitedPositions.Add((x, y));
    }
}

