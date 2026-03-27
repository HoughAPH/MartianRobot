using MartianRobot.Commands;
using MartianRobot.Models;

namespace MartianRobot.Services;

public class RobotInstructionExecutor
{
    private readonly RobotMovementService _movementService;
    private readonly IReadOnlyDictionary<char, IRobotInstructionCommand> _commands;

    public RobotInstructionExecutor()
        : this(
            new RobotMovementService(),
            [
                new MoveForwardCommand(),
                new TurnLeftCommand(),
                new TurnRightCommand()
            ])
    {
    }

    public RobotInstructionExecutor(
        RobotMovementService movementService,
        IEnumerable<IRobotInstructionCommand> commands)
    {
        _movementService = movementService ?? throw new ArgumentNullException(nameof(movementService));
        ArgumentNullException.ThrowIfNull(commands);

        _commands = commands.ToDictionary(command => command.Symbol);
    }

    public void Execute(Robot robot, string instructions)
    {
        ArgumentNullException.ThrowIfNull(robot);
        ArgumentNullException.ThrowIfNull(instructions);

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

            command.Execute(robot, _movementService);
        }
    }
}