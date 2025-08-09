using MartianRobot.Models;
namespace MartianRobot;

public class Program
{
    static void Main(string[] args)
    {
        Grid.Width = 5;
        Grid.Height = 3;
        Robot robot = new(1, 1, "E");
        Console.WriteLine($"Initial state for Robot 1: {robot}");
        robot.ExecuteInstructions("RFRFRFRF");
        Console.WriteLine($"After RFRFRFRF: {robot}");

        robot = new Robot(3, 2, "N");
        Console.WriteLine($"Initialstate for Robot 2: {robot}");
        robot.ExecuteInstructions("FRRFLLFFRRFLL");
        Console.WriteLine($"After 'FRRFLLFFRRFLL': {robot}");

        robot = new Robot(0, 3, "W");
        Console.WriteLine($"Initial state for Robot 3: {robot}");
        robot.ExecuteInstructions("LLFFFLFLFL");
        Console.WriteLine($"After 'LLFFFLFLFL': {robot}");

        // Uncomment the following lines to enable interactive mode and remove the grid settings above


        // Read grid size
        //Console.WriteLine("Enter the upper-right coordinates of the grid (e.g., 5 5):");
        //var gridInput = Console.ReadLine()?.Split(' ');
        //if (gridInput == null || gridInput.Length != 2 ||
        //    !int.TryParse(gridInput[0], out int gridWidth) ||
        //    !int.TryParse(gridInput[1], out int gridHeight) ||
        //    gridWidth > 50 || gridHeight > 50)
        //{
        //    Console.WriteLine("Invalid grid size. Maximum coordinate value is 50.");
        //    return;
        //}
        //Grid.Width = gridWidth;
        //Grid.Height = gridHeight;

        //Console.WriteLine($"Grid size set to: {Grid.Width} x {Grid.Height}");

        //// Process robots
        //while (true)
        //{
        //    Console.WriteLine("\nEnter the robot's initial position and heading (e.g., 3 4 East), or type 'exit' to quit:");
        //    var robotInput = Console.ReadLine();
        //    if (robotInput?.ToLower() == "exit") break;

        //    var robotData = robotInput?.Split(' ');
        //    if (robotData == null || robotData.Length != 3 ||
        //        !int.TryParse(robotData[0], out int startX) ||
        //        !int.TryParse(robotData[1], out int startY) ||
        //        !Grid.HeadingsIndex.Contains(robotData[2]))
        //    {
        //        Console.WriteLine("Invalid robot data. Please try again.");
        //        continue;
        //    }

        //    var robot = new Robot(startX, startY, robotData[2]);
        //    Console.WriteLine($"Robot starting position: {robot}");

        //    Console.WriteLine("Enter the movement instructions (e.g., FFLRFF): F(Forward), L(Left), R(Right)");
        //    var instructions = Console.ReadLine();
        //    if (instructions == null || instructions.Length > 100)
        //    {
        //        Console.WriteLine("Invalid instructions. Maximum length is 100 characters.");
        //        continue;
        //    }

        //    robot.ExecuteInstructions(instructions);
        //    Console.WriteLine($"Robot final state: {robot}");
    }
}
