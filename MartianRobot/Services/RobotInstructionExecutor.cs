using MartianRobot.Commands;
using MartianRobot.Models;

namespace MartianRobot.Services;

public class RobotInstructionExecutor
{
    private readonly Grid _grid;
    private readonly IReadOnlyDictionary<char, IRobotInstructionCommand> _commands;

    public RobotInstructionExecutor(Grid grid)
        : this(
            grid,
            [
                new MoveForwardCommand(),
                new TurnLeftCommand(),
                new TurnRightCommand()
            ])
    {
    }

    public RobotInstructionExecutor(Grid grid, IEnumerable<IRobotInstructionCommand> commands)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(commands);

        _grid = grid;
        _commands = commands.ToDictionary(command => command.Symbol);
    }

    public void Execute(Robot robot, string instructions)
    {
        ArgumentNullException.ThrowIfNull(robot);
        ArgumentNullException.ThrowIfNull(instructions);

        if (!_grid.IsWithinBounds(robot.Position.X, robot.Position.Y))
        {
            throw new ArgumentException("Starting position is outside grid boundaries");
        }

        if (instructions.Length > 100)
        {
            throw new ArgumentException("Instruction string exceeds the maximum length of 100 characters.");
        }

        foreach (char commandSymbol in instructions.ToUpperInvariant())
        {
            if (robot.IsLost)
            {
                break;
            }

            if (!_commands.TryGetValue(commandSymbol, out IRobotInstructionCommand? command))
            {
                throw new ArgumentException(
                    $"Invalid command: {commandSymbol}. Only {string.Join(", ", _commands.Keys.Order())} are allowed.");
            }

            command.Execute(robot, _grid);
        }
    }

    public bool TryExecuteCommand(Robot robot, char commandSymbol)
    {
        ArgumentNullException.ThrowIfNull(robot);

        if (!_commands.TryGetValue(commandSymbol, out var command))
        {
            return false;
        }

        command.Execute(robot, _grid);
        return true;
    }
}