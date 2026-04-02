using MartianRobot.Models;

namespace RobotGrid.Client.Models;

public sealed class RobotScenarioViewModel(string title, Robot startRobot, string instructions)
{
    public string Title { get; } = title;
    public Robot StartRobot { get; } = startRobot;
    public string Instructions { get; } = instructions;
    public List<RobotGridAnimationFrame> Frames { get; } = [];
    public string GridText { get; set; } = "";
}
