using MartianRobot.Models;

namespace MartianRobot.Commands;

public class TurnLeftCommand : IRobotInstructionCommand
{
    public char Symbol => 'L';

    public void Execute(Robot robot, Grid grid)
    {
        ArgumentNullException.ThrowIfNull(robot);
        ArgumentNullException.ThrowIfNull(grid);

        robot.Heading = robot.Heading switch
        {
            Heading.North => Heading.West,
            Heading.West => Heading.South,
            Heading.South => Heading.East,
            Heading.East => Heading.North,
            _ => throw new ArgumentOutOfRangeException(nameof(robot.Heading))
        };
    }
}