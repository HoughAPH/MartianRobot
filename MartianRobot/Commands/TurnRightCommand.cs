using MartianRobot.Models;

namespace MartianRobot.Commands;

public class TurnRightCommand : IRobotInstructionCommand
{
    public char Symbol => 'R';

    public void Execute(Robot robot, Grid grid)
    {
        ArgumentNullException.ThrowIfNull(robot);
        ArgumentNullException.ThrowIfNull(grid);

        robot.Heading = robot.Heading switch
        {
            Heading.North => Heading.East,
            Heading.East => Heading.South,
            Heading.South => Heading.West,
            Heading.West => Heading.North,
            _ => throw new ArgumentOutOfRangeException(nameof(robot.Heading))
        };
    }
}