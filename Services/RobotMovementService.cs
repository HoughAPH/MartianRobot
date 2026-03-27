using MartianRobot.Models;

namespace MartianRobot.Services;

public class RobotMovementService
{
    public void MoveForward(Robot robot)
    {
        ArgumentNullException.ThrowIfNull(robot);

        var (newX, newY) = CalculateForwardPosition(
            robot.Position.X,
            robot.Position.Y,
            robot.Heading);

        if (!Grid.IsWithinBounds(newX, newY))
        {
            if (Grid.IsLostPosition(robot.Position.X, robot.Position.Y))
            {
                return;
            }

            robot.IsLost = true;
            Grid.AddLostPosition(robot.Position.X, robot.Position.Y);
            return;
        }

        robot.Position = new Position(newX, newY);
    }

    public void TurnLeft(Robot robot)
    {
        ArgumentNullException.ThrowIfNull(robot);

        robot.Heading = robot.Heading switch
        {
            Heading.North => Heading.West,
            Heading.West => Heading.South,
            Heading.South => Heading.East,
            Heading.East => Heading.North,
            _ => throw new ArgumentOutOfRangeException(nameof(robot.Heading))
        };
    }

    public void TurnRight(Robot robot)
    {
        ArgumentNullException.ThrowIfNull(robot);

        robot.Heading = robot.Heading switch
        {
            Heading.North => Heading.East,
            Heading.East => Heading.South,
            Heading.South => Heading.West,
            Heading.West => Heading.North,
            _ => throw new ArgumentOutOfRangeException(nameof(robot.Heading))
        };
    }

    private static (int newX, int newY) CalculateForwardPosition(int currentX, int currentY, Heading heading)
    {
        return heading switch
        {
            Heading.North => (currentX, currentY + 1),
            Heading.South => (currentX, currentY - 1),
            Heading.East => (currentX + 1, currentY),
            Heading.West => (currentX - 1, currentY),
            _ => throw new ArgumentOutOfRangeException(nameof(heading))
        };
    }
}