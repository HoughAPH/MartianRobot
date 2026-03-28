using MartianRobot.Models;

namespace MartianRobot.Commands;

public interface IRobotInstructionCommand
{
    char Symbol { get; }

    void Execute(Robot robot);
}