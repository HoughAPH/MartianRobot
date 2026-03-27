using MartianRobot.Models;
using MartianRobot.Services;

namespace MartianRobot.Commands;

public interface IRobotInstructionCommand
{
    char Symbol { get; }

    void Execute(Robot robot, RobotMovementService movementService);
}