using MartianRobot.Models;
using MartianRobot.Services;

namespace MartianRobot;

public class Program
{
    static void Main(string[] args)
    {
        var grid = new Grid(5, 3);
        var executor = new RobotInstructionExecutor(grid);

        var scenarios = new (Robot Robot, string Instructions)[]
        {
            (new Robot(1, 1, Heading.East), "RFRFRFRF"),
            (new Robot(3, 2, Heading.North), "FRRFLLFFRRFLL"),
            (new Robot(0, 3, Heading.West), "LLFFFLFLFL")
        };

        foreach (var scenario in scenarios)
        {
            executor.Execute(scenario.Robot, scenario.Instructions);
            Console.WriteLine(scenario.Robot);
        }
    }
}