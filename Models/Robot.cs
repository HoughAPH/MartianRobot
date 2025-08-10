using MartianRobot.Common;
using MartianRobot.Services;

namespace MartianRobot.Models;

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

    //public delegate void RobotCommand(Robot robot);

    public void ExecuteInstructions(string instructions)
    {
        if (instructions.Length > 100)
        {
            throw new ArgumentException("Instruction string exceeds the maximum length of 100 characters.");
        }

        Console.WriteLine($"Executing instructions: {instructions}");
        var movementService = new RobotMovementService();

        foreach (char command in instructions.ToUpper())
        {
            if (IsLost) break;
#if DEBUG
            Console.WriteLine($"Executing command: {command}");
#endif

            RobotCommand robotCommand = command switch
            {
                'F' => movementService.Forward,
                'L' => r => movementService.Turn(r, "L"),
                'R' => r => movementService.Turn(r, "R"),
                _ => throw new ArgumentException($"Invalid command: {command}. Only L, R and F are allowed.")
            };

            robotCommand(this);
        }
    }

    public override string ToString()
    {
        string status = IsLost ? " LOST" : "";
        return $"Robot at ({X}, {Y}) facing {Heading}{status}";
    }
}