using MartianRobot.Models;

namespace MartianRobot.Services;

public class RobotMovementService
{
    public void Forward(Robot robot)
    {
        var (newX, newY) = CalculateNewPosition(robot.X, robot.Y, robot.Heading);

        // If the new position is outside the grid
        if (!Grid.IsWithinBounds(newX, newY))
        {
            // If the current position is already "scented" (lost), ignore the move
            if (Grid.IsLostPosition(robot.X, robot.Y))
            {
#if DEBUG
            Console.WriteLine($"  Move Forward: Ignored at scented position ({robot.X}, {robot.Y}) facing {robot.Heading}");
#endif
                return;
            }
            // Otherwise, mark robot as LOST and scent the current position
            robot.IsLost = true;
            Grid.AddLostPosition(robot.X, robot.Y);
#if DEBUG
        Console.WriteLine($"  Move Forward: Robot LOST at ({robot.X}, {robot.Y}) facing {robot.Heading}");
#endif
            return;
        }

        // If the move is valid, update the robot's position
        robot.X = newX;
        robot.Y = newY;
#if DEBUG
    Console.WriteLine($"  Move Forward: Robot moved to ({robot.X}, {robot.Y}) facing {robot.Heading}");
#endif
    }

    public void Turn(Robot robot, string turning)
    {
        if (turning.Length != 1 || turning[0] != 'L' && turning[0] != 'R')
        {
            throw new ArgumentException("Invalid parameter for Turn. Must be 'L' or 'R'.");
        }

        string oldHeading = robot.Heading;
        robot.Heading = ChangeHeading(robot.Heading, turning[0]);

#if DEBUG
        string direction = turning[0] == 'L' ? "Left" : "Right";
        Console.WriteLine($"  Turn {direction}: Robot turned from {oldHeading} to {robot.Heading}");
#endif
    }

    private string ChangeHeading(string currentHeading, char turning)
    {
        int currentIndex = Array.IndexOf(Grid.HeadingsIndex, currentHeading);
        if (currentIndex == -1)
        {
            throw new ArgumentException("Invalid heading");
        }

        int rotateIndex = turning == 'L' ? -1 : 1;
        int newIndex = (currentIndex + rotateIndex + Grid.HeadingsIndex.Length) % Grid.HeadingsIndex.Length;
        return Grid.HeadingsIndex[newIndex];
    }

    private (int newX, int newY) CalculateNewPosition(int currentX, int currentY, string heading)
    {
        return heading switch
        {
            "N" => (currentX, currentY + 1),
            "S" => (currentX, currentY - 1),
            "E" => (currentX + 1, currentY),
            "W" => (currentX - 1, currentY),
            _ => throw new ArgumentException($"Invalid direction: {heading}")
        };
    }
    
}    