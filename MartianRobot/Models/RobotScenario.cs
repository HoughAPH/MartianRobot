namespace MartianRobot.Models;

public sealed class RobotScenario
{
    public string Title { get; init; } = "";
    public GridState Grid { get; init; } = default!;
    public Robot StartRobot { get; init; } = default!;
    public string Instructions { get; init; } = "";
    public List<RobotGridAnimationFrame> Frames { get; } = [];
    public string GridText { get; set; } = "";
}