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
            new("Robot 1", new Robot(1, 1, Heading.East), "RFRFRFRF", ""),
            new("Robot 2", new Robot(3, 2, Heading.North), "FRRFLLFFRRFLL", ""),
            new("Robot 3", new Robot(0, 3, Heading.West), "LLFFFLFLFL", "")
        ];
    }

    public void ResetScenarios(Grid grid, IEnumerable<RobotScenarioViewModel> scenarios)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(scenarios);

        grid.Reset();

        foreach (RobotScenarioViewModel scenario in scenarios)
        {
            scenario.GridText = _frameGenerator.BuildInitialFrame(
                grid,
                scenario.StartRobot,
                scenario.Instructions,
                resetGrid: false).GridText;
        }
    }

    public IEnumerable<RobotGridAnimationFrame> BuildScenarioFrames(Grid grid, RobotScenarioViewModel scenario)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(scenario);

        return _frameGenerator.BuildFrames(
            grid,
            scenario.StartRobot,
            scenario.Instructions,
            resetGrid: false);
    }

    public IEnumerable<RobotGridAnimationFrame> BuildScenarioFrames(Grid grid, RobotScenarioInput input)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(input);

        return BuildScenarioFrames(grid, CreateScenario(input));
    }

    public IEnumerable<RobotGridAnimationFrame> BuildScenarioFrames(RobotScenarioInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        Grid grid = new(input.GridWidth, input.GridHeight);
        return BuildScenarioFrames(grid, input);
    }

    private static RobotScenarioViewModel CreateScenario(RobotScenarioInput input)
    {
        return new(
            "Custom Scenario",
            new Robot(input.StartX, input.StartY, input.Heading),
            input.Instructions,
            "");
    }
}
