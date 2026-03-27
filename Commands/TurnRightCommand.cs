using MartianRobot.Models;
using MartianRobot.Services;

namespace MartianRobot.Commands;

public class TurnRightCommand : IRobotInstructionCommand
{
    public char Symbol => 'R';

    public void Execute(Robot robot, RobotMovementService movementService)
    {
        movementService.TurnRight(robot);
    }
}