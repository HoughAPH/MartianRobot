using MartianRobot.Models;
using RobotGrid.Client.Models;

namespace RobotGrid.Client.Services;

public sealed class RobotScenarioAnimationService
{
    private readonly RobotScenarioFrameGenerator _frameGenerator = new();

    public List<RobotScenarioViewModel> CreateDefaultScenarios()
    {
        return
        [
            new("Robot 1", new Robot(1, 1, Heading.East), "RFRFRFRF"),
            new("Robot 2", new Robot(3, 2, Heading.North), "FRRFLLFFRRFLL"),
            new("Robot 3", new Robot(0, 3, Heading.West), "LLFFFLFLFL")
        ];
    }

    public void ResetScenarios(Grid grid, IEnumerable<RobotScenarioViewModel> scenarios)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(scenarios);

        grid.Reset();

        foreach (var scenario in scenarios)
        {
            scenario.Frames.Clear();
            scenario.GridText = _frameGenerator.BuildInitialFrame(
                grid,
                scenario.StartRobot,
                scenario.Instructions,
                resetGrid: false).GridText;
        }
    }

    public IReadOnlyList<RobotGridAnimationFrame> BuildScenarioFrames(Grid grid, RobotScenarioViewModel scenario)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(scenario);

        scenario.Frames.Clear();
        scenario.Frames.AddRange(_frameGenerator.BuildFrames(
            grid,
            scenario.StartRobot,
            scenario.Instructions,
            resetGrid: false));

        return scenario.Frames;
    }
}
