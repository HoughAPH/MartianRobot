/*
 * Further development of the classes and movement logic for a robot simulation.
 * I decided to implement a delegate for robot commands, instead of an interface.  
* And interface would be rather simple and done before lots of times.  
* 
 * Sample input:
 * // 5 3
 * // 1 1 E
 * // RFRFRFRF
 * // 3 2 N
 * // FRRFLLFFRRFLL
 * // 0 3 W
 * // LLFFFLFLFL
 */

public class Program
{
    public delegate void RobotCommand(Robot robot, string parameter);
    private static readonly string[] sourceArray = { "N", "E", "S", "W" };

    public static void Main(string[] args)
    {
        // Read grid size
        Console.WriteLine("Enter the upper-right coordinates of the grid (e.g., 5 5):");
        var gridInput = Console.ReadLine()?.Split(' ');

        if (gridInput == null || gridInput.Length != 2 ||
            !int.TryParse(gridInput[0], out int gridWidth) ||
            !int.TryParse(gridInput[1], out int gridHeight) ||
            gridWidth > 50 || gridHeight > 50)
        {
            Console.WriteLine("Invalid grid size. Maximum coordinate value is 50.");
            return;
        }

        Grid.Width = gridWidth;
        Grid.Height = gridHeight;

        Console.WriteLine($"Grid size set to: {Grid.Width} x {Grid.Height}");

        // Process robots
        while (true)
        {
            Console.WriteLine("\nEnter the robot's initial position and heading (e.g., 3 4 E), or type 'exit' to quit:");
            var robotInput = Console.ReadLine();

            if (robotInput?.ToLower() == "exit") break;

            var robotData = robotInput?.Split(' ');
            if (robotData == null || robotData.Length != 3 ||
                !int.TryParse(robotData[0], out int startX) ||
                !int.TryParse(robotData[1], out int startY) ||
                !sourceArray.Contains(robotData[2].ToUpper()))
            {
                Console.WriteLine("Invalid robot data. Please try again.");
                continue;
            }
            var robot = new Robot(startX, startY, robotData[2].ToUpper());
            Console.WriteLine("Enter the movement instructions (e.g., FFLRFF):");
            var instructions = Console.ReadLine();
            if (instructions == null || instructions.Length > 100)
            {
                Console.WriteLine("Invalid instructions. Maximum length is 100 characters.");
                continue;
            }
            robot.ExecuteInstructions(instructions);
            Console.WriteLine($"Robot final state: {robot}");
        }
    }
}

public static class Grid
{
    public static int Width { get; set; } = 50;
    public static int Height { get; set; } = 50;
    public static HashSet<(int X, int Y)> LostPositions { get; } = new HashSet<(int, int)>();

    public static bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x <= Width && y >= 0 && y <= Height;
    }

    public static bool IsLostPosition(int x, int y)
    {
        return LostPositions.Contains((x, y));
    }

    public static void AddLostPosition(int x, int y)
    {
        LostPositions.Add((x, y));
    }

    public static void Reset()
    {
        LostPositions.Clear();
    }
}

public static class MoveRobot
{
    private static readonly string[] Headings = { "N", "E", "S", "W" };

    public static void Forward(Robot robot, string parameter)
    {
        var (newX, newY) = CalculateNewPosition(robot.X, robot.Y, robot.Heading);

        var (canMove, robotLost) = ValidateMove(robot.X, robot.Y, newX, newY);

        if (robotLost)
        {
            robot.IsLost = true;
        }
        else if (canMove)
        {
            robot.X = newX;
            robot.Y = newY;
        }
    }

    public static void Turn(Robot robot, string parameter)
    {
        if (parameter.Length != 1 || (parameter[0] != 'L' && parameter[0] != 'R'))
        {
            throw new ArgumentException("Invalid parameter for Turn. Must be 'L' or 'R'.");
        }

        robot.Heading = ChangeHeading(robot.Heading, parameter[0]);
    }

    private static string ChangeHeading(string currentHeading, char rotation)
    {
        int currentIndex = Array.IndexOf(Headings, currentHeading);
        if (currentIndex == -1)
        {
            throw new ArgumentException("Invalid heading");
        }

        int rotateIndex = rotation == 'L' ? -1 : 1;
        int newIndex = (currentIndex + rotateIndex + Headings.Length) % Headings.Length;
        return Headings[newIndex];
    }

    private static (int newX, int newY) CalculateNewPosition(int currentX, int currentY, string heading)
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

    private static (bool canMove, bool robotLost) ValidateMove(int currentX, int currentY, int newX, int newY)
    {
        if (!Grid.IsWithinBounds(newX, newY))
        {
            if (!Grid.IsLostPosition(currentX, currentY))
            {
                Grid.AddLostPosition(currentX, currentY);
            }
            return (false, true);
        }

        if (Grid.IsLostPosition(newX, newY))
        {
            return (false, false);
        }

        return (true, false);
    }
}

public class Robot
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Heading { get; set; }
    public bool IsLost { get; set; }

    public Robot(int startX, int startY, string initialHeading = "N")
    {
        if (!Grid.IsWithinBounds(startX, startY))
        {
            throw new ArgumentException("Starting position is outside grid boundaries");
        }

        X = startX;
        Y = startY;
        Heading = initialHeading;
        IsLost = false;
    }

    public void ExecuteInstructions(string instructions)
    {
        if (instructions.Length > 100)
        {
            throw new ArgumentException("Instruction string exceeds the maximum length of 100 characters.");
        }

        foreach (char command in instructions.ToUpper())
        {
            if (IsLost) break;

            Program.RobotCommand commandDelegate = command switch
            {
                'F' => MoveRobot.Forward,
                'L' => (robot, _) => MoveRobot.Turn(robot, "L"),
                'R' => (robot, _) => MoveRobot.Turn(robot, "R"),
                _ => throw new ArgumentException($"Invalid command: {command}. Only L, R and F are allowed.")
            };

            commandDelegate(this, command.ToString());
        }
    }

    public override string ToString()
    {
        string status = IsLost ? " LOST" : "";
        return $"Robot at ({X}, {Y}) facing {Heading}{status}";
    }
}