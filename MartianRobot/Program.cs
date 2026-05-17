using MartianRobot.Models;
using MartianRobot.Services;

namespace MartianRobot;

public class Program
{
    private static void Main(string[] args)
    {
        Grid grid = new(5, 3);
        RobotInstructionExecutor executor = new(grid);

        (Robot Robot, string Instructions)[] scenarios = new (Robot Robot, string Instructions)[]
        {
            (new Robot(1, 1, Heading.East), "RFRFRFRF"),
            (new Robot(3, 2, Heading.North), "FRRFLLFFRRFLL"),
            (new Robot(0, 3, Heading.West), "LLFFFLFLFL")
        };

        foreach ((Robot Robot, string Instructions) scenario in scenarios)
        {
            executor.Execute(scenario.Robot, scenario.Instructions);
            Console.WriteLine(scenario.Robot);
        }
    }
}