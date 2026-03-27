using MartianRobot.Models;
using MartianRobot.Services;

namespace MartianRobot.Commands;

public class TurnLeftCommand : IRobotInstructionCommand
{
    public char Symbol => 'L';

    public void Execute(Robot robot, RobotMovementService movementService)
    {
        movementService.TurnLeft(robot);
    }
}