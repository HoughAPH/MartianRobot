using MartianRobot.Models;
using MartianRobot.Services;

namespace MartianRobot.Commands;

public class MoveForwardCommand : IRobotInstructionCommand
{
    public char Symbol => 'F';

    public void Execute(Robot robot, RobotMovementService movementService)
    {
        movementService.MoveForward(robot);
    }
}