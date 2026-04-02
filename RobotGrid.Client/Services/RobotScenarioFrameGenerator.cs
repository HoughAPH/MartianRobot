using MartianRobot.Models;
using MartianRobot.Services;

namespace RobotGrid.Client.Services;

public sealed class RobotScenarioFrameGenerator
{
    private readonly RobotGridTextRenderer _renderer = new();
    private readonly RobotInstructionExecutor _executor = new();

    public RobotGridAnimationFrame BuildInitialFrame(
        int width,
        int height,
        Robot startRobot,
        string instructions,
        bool resetGrid = true)
    {
        return BuildFramesCore(
            width,
            height,
            startRobot,
            instructions,
            resetGrid,
            includeInstructionFrames: false)[0];
    }

    public IReadOnlyList<RobotGridAnimationFrame> BuildFrames(
        int width,
        int height,
        Robot startRobot,
        string instructions,
        bool resetGrid = true)
    {
        return BuildFramesCore(
            width,
            height,
            startRobot,
            instructions,
            resetGrid,
            includeInstructionFrames: true);
    }

    private List<RobotGridAnimationFrame> BuildFramesCore(
        int width,
        int height,
        Robot startRobot,
        string instructions,
        bool resetGrid,
        bool includeInstructionFrames)
    {
        ArgumentNullException.ThrowIfNull(startRobot);
        ArgumentNullException.ThrowIfNull(instructions);

        Grid.Width = width;
        Grid.Height = height;

        if (resetGrid)
        {
            Grid.Reset();
        }

        var normalizedInstructions = instructions.ToUpperInvariant();
        var currentRobot = CloneRobot(startRobot);
        HashSet<(int X, int Y)> visitedPositions = [];
        List<RobotGridAnimationFrame> frames = [];

        TrackVisitedPosition(currentRobot, width, height, visitedPositions);

        frames.Add(CreateFrame(
            width,
            height,
            startRobot,
            normalizedInstructions,
            currentRobot,
            visitedPositions,
            statusLabel: "Current"));

        if (!includeInstructionFrames)
        {
            return frames;
        }

        if (normalizedInstructions.Length == 0)
        {
            frames[0] = CreateFrame(
                width,
                height,
                startRobot,
                normalizedInstructions,
                currentRobot,
                visitedPositions,
                statusLabel: "Final");

            return frames;
        }

        for (var i = 0; i < normalizedInstructions.Length; i++)
        {
            var command = normalizedInstructions[i];

            if (currentRobot.IsLost)
            {
                break;
            }

            if (!_executor.TryExecuteCommand(currentRobot, command))
            {
                throw new ArgumentException($"Invalid command: {command}. Only F, L, and R are allowed.");
            }

            TrackVisitedPosition(currentRobot, width, height, visitedPositions);

            var isFinalFrame = currentRobot.IsLost || i == normalizedInstructions.Length - 1;

            frames.Add(CreateFrame(
                width,
                height,
                startRobot,
                normalizedInstructions,
                currentRobot,
                visitedPositions,
                isFinalFrame ? "Final" : "Current"));
        }

        return frames;
    }

    private RobotGridAnimationFrame CreateFrame(
        int width,
        int height,
        Robot startRobot,
        string instructions,
        Robot currentRobot,
        HashSet<(int X, int Y)> visitedPositions,
        string statusLabel)
    {
        return new RobotGridAnimationFrame(
            _renderer.BuildGridText(
                width,
                height,
                startRobot,
                instructions,
                currentRobot,
                Grid.LostPositions,
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
        int width,
        int height,
        HashSet<(int X, int Y)> visitedPositions)
    {
        var x = robot.Position.X;
        var y = robot.Position.Y;

        if (x < 0 || x > width || y < 0 || y > height)
        {
            return;
        }

        visitedPositions.Add((x, y));
    }
}

public sealed record RobotGridAnimationFrame(string GridText);