using MartianRobot.Models;

namespace MartianRobot.Commands;

public class MoveForwardCommand : IRobotInstructionCommand
{
    public char Symbol => 'F';

    public void Execute(Robot robot, Grid grid)
    {
        ArgumentNullException.ThrowIfNull(robot);
        ArgumentNullException.ThrowIfNull(grid);

        var (newX, newY) = robot.Heading switch
        {
            Heading.North => (robot.Position.X, robot.Position.Y + 1),
            Heading.South => (robot.Position.X, robot.Position.Y - 1),
            Heading.East => (robot.Position.X + 1, robot.Position.Y),
            Heading.West => (robot.Position.X - 1, robot.Position.Y),
            _ => throw new ArgumentOutOfRangeException(nameof(robot.Heading))
        };

        if (!grid.IsWithinBounds(newX, newY))
        {
            if (grid.IsLostPosition(robot.Position.X, robot.Position.Y))
            {
                return;
            }

            robot.IsLost = true;
            grid.AddLostPosition(robot.Position.X, robot.Position.Y);
            return;
        }

        robot.Position = new Position(newX, newY);
    }
}