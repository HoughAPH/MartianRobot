using MartianRobot.Models;

namespace MartianRobot.Commands;

public interface IRobotInstructionCommand
{
    char Symbol { get; }
    string CommandText { get; }
    void Execute(Robot robot, Grid grid);
}