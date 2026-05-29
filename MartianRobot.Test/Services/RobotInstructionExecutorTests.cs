using MartianRobot.Commands;
using MartianRobot.Models;
using MartianRobot.Services;
using Moq;

namespace MartianRobot.Test.Services;


/// <summary>
/// Contains unit tests for <see cref="RobotInstructionExecutor"/>.
/// </summary>
public class RobotInstructionExecutorTests
{
    /// <summary>
    /// Verifies that constructing the executor with a null grid throws <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void Constructor_NullGrid_ThrowsArgumentNullException()
    {
        // Arrange
        Grid? grid = null;

        // Act
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new RobotInstructionExecutor(grid!));

        // Assert
        Assert.Equal("grid", exception.ParamName);
    }

    /// <summary>
    /// Verifies that constructing the executor with a valid grid loads the built-in command for the provided symbol.
    /// </summary>
    /// <param name="symbol">The known built-in command symbol to resolve.</param>
    /// <param name="expectedCommandType">The expected concrete command type registered for the symbol.</param>
    [Theory]
    [InlineData('F', typeof(MoveForwardCommand))]
    [InlineData('L', typeof(TurnLeftCommand))]
    [InlineData('R', typeof(TurnRightCommand))]
    public void Constructor_ValidGrid_LoadsKnownDefaultCommands(char symbol, Type expectedCommandType)
    {
        // Arrange
        Grid grid = new();

        // Act
        RobotInstructionExecutor executor = new(grid);
        bool found = executor.TryGetCommand(symbol, out IRobotInstructionCommand? command);

        // Assert
        Assert.True(found);
        Assert.NotNull(command);
        Assert.IsType(expectedCommandType, command);
    }

    /// <summary>
    /// Verifies that constructing the executor with a null grid throws <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void RobotInstructionExecutor_NullGrid_ThrowsArgumentNullException()
    {
        // Arrange
        Grid? grid = null;
        IEnumerable<IRobotInstructionCommand> commands = Array.Empty<IRobotInstructionCommand>();

        // Act
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new RobotInstructionExecutor(grid!, commands));

        // Assert
        Assert.Equal("grid", exception.ParamName);
    }

    /// <summary>
    /// Verifies that constructing the executor with a null commands sequence throws <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void RobotInstructionExecutor_NullCommands_ThrowsArgumentNullException()
    {
        // Arrange
        Grid grid = new();
        IEnumerable<IRobotInstructionCommand>? commands = null;

        // Act
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new RobotInstructionExecutor(grid, commands!));

        // Assert
        Assert.Equal("commands", exception.ParamName);
    }

    /// <summary>
    /// Verifies that constructing the executor with an empty commands sequence succeeds and exposes no allowed commands.
    /// </summary>
    [Fact]
    public void RobotInstructionExecutor_EmptyCommands_CreatesExecutorWithNoAllowedCommands()
    {
        // Arrange
        Grid grid = new();
        IEnumerable<IRobotInstructionCommand> commands = Array.Empty<IRobotInstructionCommand>();

        // Act
        RobotInstructionExecutor executor = new(grid, commands);

        // Assert
        Assert.Equal(string.Empty, executor.AllowedCommandsText);
        Assert.False(executor.TryGetCommand('L', out IRobotInstructionCommand? command));
        Assert.Null(command);
    }

    /// <summary>
    /// Verifies that constructing the executor with unique command symbols stores the commands and exposes sorted allowed command text.
    /// </summary>
    [Fact]
    public void RobotInstructionExecutor_UniqueCommands_PopulatesLookupAndSortsAllowedCommands()
    {
        // Arrange
        Grid grid = new();

        Mock<IRobotInstructionCommand> rightCommand = new();
        rightCommand.SetupGet(command => command.Symbol).Returns('R');

        Mock<IRobotInstructionCommand> leftCommand = new();
        leftCommand.SetupGet(command => command.Symbol).Returns('L');

        IEnumerable<IRobotInstructionCommand> commands = new[]
        {
            rightCommand.Object,
            leftCommand.Object,
        };

        // Act
        RobotInstructionExecutor executor = new(grid, commands);

        // Assert
        Assert.Equal("L, R", executor.AllowedCommandsText);

        Assert.True(executor.TryGetCommand('L', out IRobotInstructionCommand? leftResult));
        Assert.Same(leftCommand.Object, leftResult);

        Assert.True(executor.TryGetCommand('r', out IRobotInstructionCommand? rightResult));
        Assert.Same(rightCommand.Object, rightResult);
    }

    /// <summary>
    /// Verifies that constructing the executor with duplicate command symbols throws <see cref="ArgumentException"/>.
    /// </summary>
    [Fact]
    public void RobotInstructionExecutor_DuplicateCommandSymbols_ThrowsArgumentException()
    {
        // Arrange
        Grid grid = new();

        Mock<IRobotInstructionCommand> firstCommand = new();
        firstCommand.SetupGet(command => command.Symbol).Returns('L');

        Mock<IRobotInstructionCommand> secondCommand = new();
        secondCommand.SetupGet(command => command.Symbol).Returns('L');

        IEnumerable<IRobotInstructionCommand> commands = new[]
        {
            firstCommand.Object,
            secondCommand.Object,
        };

        // Act
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new RobotInstructionExecutor(grid, commands));

        // Assert
        Assert.NotNull(exception);
    }

    /// <summary>
    /// Verifies that trying to execute a command with a null robot throws <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void TryExecuteCommand_NullRobot_ThrowsArgumentNullException()
    {
        // Arrange
        Robot? robot = null;
        Grid grid = new();
        RobotInstructionExecutor executor = new(grid, Array.Empty<IRobotInstructionCommand>());

        // Act
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => executor.TryExecuteCommand(robot!, 'L', out _));

        // Assert
        Assert.Equal("robot", exception.ParamName);
    }

    /// <summary>
    /// Verifies that trying to execute a known command symbol returns <see langword="true"/>,
    /// outputs the matched command, and executes it once for both uppercase and lowercase input symbols.
    /// </summary>
    /// <param name="commandSymbol">The command symbol passed to the executor.</param>
    [Theory]
    [InlineData('L')]
    [InlineData('l')]
    public void TryExecuteCommand_KnownCommand_ReturnsTrueAndExecutesCommand(char commandSymbol)
    {
        // Arrange
        Robot robot = new(0, 0);
        Grid grid = new();
        Mock<IRobotInstructionCommand> commandMock = new(MockBehavior.Strict);

        commandMock.SetupGet(command => command.Symbol).Returns('L');
        commandMock.Setup(command => command.Execute(robot, grid));

        RobotInstructionExecutor executor = new(grid, new[] { commandMock.Object });

        // Act
        bool result = executor.TryExecuteCommand(robot, commandSymbol, out IRobotInstructionCommand? command);

        // Assert
        Assert.True(result);
        Assert.Same(commandMock.Object, command);
        commandMock.Verify(mock => mock.Execute(robot, grid), Times.Once);
    }

    /// <summary>
    /// Verifies that trying to execute an unknown command symbol returns <see langword="false"/>,
    /// outputs <see langword="null"/>, and does not execute any command.
    /// </summary>
    /// <param name="commandSymbol">The unknown command symbol passed to the executor.</param>
    [Theory]
    [InlineData('X')]
    [InlineData('\0')]
    public void TryExecuteCommand_UnknownCommand_ReturnsFalseAndDoesNotExecute(char commandSymbol)
    {
        // Arrange
        Robot robot = new(0, 0);
        Grid grid = new();
        Mock<IRobotInstructionCommand> commandMock = new(MockBehavior.Strict);

        commandMock.SetupGet(command => command.Symbol).Returns('L');

        RobotInstructionExecutor executor = new(grid, new[] { commandMock.Object });

        // Act
        bool result = executor.TryExecuteCommand(robot, commandSymbol, out IRobotInstructionCommand? command);

        // Assert
        Assert.False(result);
        Assert.Null(command);
        commandMock.Verify(mock => mock.Execute(It.IsAny<Robot>(), It.IsAny<Grid>()), Times.Never);
    }

    /// <summary>
    /// Verifies that <see cref="RobotInstructionExecutor.AllowedCommandsText"/> returns
    /// a comma-separated list of configured command symbols ordered in ascending character order
    /// for empty, single, multiple, and mixed-case command collections.
    /// </summary>
    /// <param name="symbols">The command symbols used to construct the executor.</param>
    /// <param name="expected">The expected ordered command text.</param>
    [Theory]
    [InlineData("", "")]
    [InlineData("L", "L")]
    [InlineData("RLF", "F, L, R")]
    [InlineData("zAa", "A, a, z")]
    public void AllowedCommandsText_CommandsProvided_ReturnsOrderedCommaSeparatedSymbols(string symbols, string expected)
    {
        // Arrange
        Grid grid = new();
        var commands = CreateCommands(symbols);
        RobotInstructionExecutor executor = new(grid, commands);

        // Act
        var result = executor.AllowedCommandsText;

        // Assert
        Assert.Equal(expected, result);
    }

    private static IEnumerable<IRobotInstructionCommand> CreateCommands(string symbols)
    {
        return symbols.Select(CreateCommand).ToArray();
    }

    private static IRobotInstructionCommand CreateCommand(char symbol)
    {
        Mock<IRobotInstructionCommand> commandMock = new(MockBehavior.Strict);
        commandMock.SetupGet(command => command.Symbol).Returns(symbol);
        return commandMock.Object;
    }

    /// <summary>
    /// Verifies that requesting a registered command returns <see langword="true"/> and the matching command instance.
    /// This covers both exact-uppercase input and lowercase input that is normalized with <see cref="char.ToUpperInvariant(char)"/>.
    /// </summary>
    /// <param name="commandSymbol">The command symbol passed to <see cref="RobotInstructionExecutor.TryGetCommand(char, out IRobotInstructionCommand?)"/>.</param>
    [Theory]
    [InlineData('L')]
    [InlineData('l')]
    public void TryGetCommand_CommandRegisteredWithUppercaseSymbol_ReturnsTrueAndMatchingCommand(char commandSymbol)
    {
        // Arrange
        Grid grid = new();
        Mock<IRobotInstructionCommand> expectedCommand = CreateCommandMock('L');
        RobotInstructionExecutor executor = new(grid, [expectedCommand.Object]);

        // Act
        bool result = executor.TryGetCommand(commandSymbol, out IRobotInstructionCommand? command);

        // Assert
        Assert.True(result);
        Assert.Same(expectedCommand.Object, command);
    }

    /// <summary>
    /// Verifies that requesting an unregistered command symbol returns <see langword="false"/> and sets the output command to <see langword="null"/>.
    /// </summary>
    /// <param name="commandSymbol">A command symbol that is not present in the executor command dictionary.</param>
    [Theory]
    [InlineData('X')]
    [InlineData('x')]
    public void TryGetCommand_CommandNotRegistered_ReturnsFalseAndNull(char commandSymbol)
    {
        // Arrange
        Grid grid = new();
        Mock<IRobotInstructionCommand> registeredCommand = CreateCommandMock('L');
        RobotInstructionExecutor executor = new(grid, [registeredCommand.Object]);

        // Act
        bool result = executor.TryGetCommand(commandSymbol, out IRobotInstructionCommand? command);

        // Assert
        Assert.False(result);
        Assert.Null(command);
    }

    /// <summary>
    /// Verifies that a command registered with a lowercase symbol is not found when queried with the same lowercase symbol,
    /// because lookup normalizes only the requested symbol to uppercase while the dictionary stores the original key.
    /// </summary>
    [Fact]
    public void TryGetCommand_CommandRegisteredWithLowercaseSymbol_ReturnsFalseAndNull()
    {
        // Arrange
        Grid grid = new();
        Mock<IRobotInstructionCommand> registeredCommand = CreateCommandMock('l');
        RobotInstructionExecutor executor = new(grid, [registeredCommand.Object]);

        // Act
        bool result = executor.TryGetCommand('l', out IRobotInstructionCommand? command);

        // Assert
        Assert.False(result);
        Assert.Null(command);
    }

    private static Mock<IRobotInstructionCommand> CreateCommandMock(char symbol)
    {
        Mock<IRobotInstructionCommand> command = new(MockBehavior.Strict);
        command.SetupGet(item => item.Symbol).Returns(symbol);
        return command;
    }

    /// <summary>
    /// Verifies that executing with a null robot throws <see cref="ArgumentNullException"/>
    /// before any other validation occurs.
    /// </summary>
    [Fact]
    public void Execute_NullRobot_ThrowsArgumentNullException()
    {
        // Arrange
        Robot? robot = null;
        RobotInstructionExecutor executor = new(new Grid(), Array.Empty<IRobotInstructionCommand>());

        // Act
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => executor.Execute(robot!, string.Empty));

        // Assert
        Assert.Equal("robot", exception.ParamName);
    }

    /// <summary>
    /// Verifies that executing with null instructions throws <see cref="ArgumentNullException"/>
    /// before any command processing begins.
    /// </summary>
    [Fact]
    public void Execute_NullInstructions_ThrowsArgumentNullException()
    {
        // Arrange
        Robot robot = new(0, 0);
        string? instructions = null;
        RobotInstructionExecutor executor = new(new Grid(), Array.Empty<IRobotInstructionCommand>());

        // Act
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => executor.Execute(robot, instructions!));

        // Assert
        Assert.Equal("instructions", exception.ParamName);
    }

    /// <summary>
    /// Verifies that executing with a starting position outside the grid boundaries throws
    /// <see cref="ArgumentException"/> for out-of-range coordinate values.
    /// </summary>
    /// <param name="startX">The robot's starting X coordinate.</param>
    /// <param name="startY">The robot's starting Y coordinate.</param>
    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(51, 0)]
    [InlineData(0, 51)]
    [InlineData(int.MinValue, 0)]
    [InlineData(0, int.MaxValue)]
    public void Execute_StartingPositionOutsideGrid_ThrowsArgumentException(int startX, int startY)
    {
        // Arrange
        Robot robot = new(startX, startY);
        RobotInstructionExecutor executor = new(new Grid(), Array.Empty<IRobotInstructionCommand>());

        // Act
        ArgumentException exception = Assert.Throws<ArgumentException>(() => executor.Execute(robot, string.Empty));

        // Assert
        Assert.Equal("Starting position is outside grid boundaries", exception.Message);
    }

    /// <summary>
    /// Verifies that executing with an instruction string longer than 100 characters throws
    /// <see cref="ArgumentException"/> with the configured validation message.
    /// </summary>
    [Fact]
    public void Execute_InstructionsLongerThanOneHundredCharacters_ThrowsArgumentException()
    {
        // Arrange
        Robot robot = new(0, 0);
        string instructions = new('L', 101);
        RobotInstructionExecutor executor = new(new Grid(), Array.Empty<IRobotInstructionCommand>());

        // Act
        ArgumentException exception = Assert.Throws<ArgumentException>(() => executor.Execute(robot, instructions));

        // Assert
        Assert.Equal("Instruction string exceeds the maximum length of 100 characters.", exception.Message);
    }

    /// <summary>
    /// Verifies that executing with an empty instruction string succeeds and does not invoke any commands.
    /// </summary>
    [Fact]
    public void Execute_EmptyInstructions_DoesNotExecuteAnyCommands()
    {
        // Arrange
        Grid grid = new();
        Robot robot = new(0, 0);
        Mock<IRobotInstructionCommand> leftCommandMock = CreateCommandMock('L');
        Mock<IRobotInstructionCommand> rightCommandMock = CreateCommandMock('R');
        RobotInstructionExecutor executor = CreateExecutor(grid, leftCommandMock, rightCommandMock);

        // Act
        Exception? exception = Record.Exception(() => executor.Execute(robot, string.Empty));

        // Assert
        Assert.Null(exception);
        leftCommandMock.Verify(command => command.Execute(robot, grid), Times.Never());
        rightCommandMock.Verify(command => command.Execute(robot, grid), Times.Never());
    }

    /// <summary>
    /// Verifies that executing with an unsupported instruction throws <see cref="ArgumentException"/>
    /// and reports the uppercased invalid command together with the sorted allowed command list.
    /// </summary>
    [Fact]
    public void Execute_InvalidCommand_ThrowsArgumentExceptionWithAllowedCommands()
    {
        // Arrange
        Grid grid = new();
        Robot robot = new(0, 0);
        Mock<IRobotInstructionCommand> rightCommandMock = CreateCommandMock('R');
        Mock<IRobotInstructionCommand> leftCommandMock = CreateCommandMock('L');
        RobotInstructionExecutor executor = CreateExecutor(grid, rightCommandMock, leftCommandMock);

        // Act
        ArgumentException exception = Assert.Throws<ArgumentException>(() => executor.Execute(robot, "x"));

        // Assert
        Assert.Equal("Invalid command: X. Only L, R are allowed.", exception.Message);
    }

    /// <summary>
    /// Verifies that lowercase instructions are normalized to uppercase and execute
    /// the matching commands in the original instruction order.
    /// </summary>
    [Fact]
    public void Execute_LowercaseInstructions_ExecutesMatchingCommandsInOrder()
    {
        // Arrange
        Grid grid = new();
        Robot robot = new(0, 0);
        Mock<IRobotInstructionCommand> leftCommandMock = CreateCommandMock('L');
        Mock<IRobotInstructionCommand> rightCommandMock = CreateCommandMock('R');
        MockSequence sequence = new();

        leftCommandMock
            .InSequence(sequence)
            .Setup(command => command.Execute(robot, grid));

        rightCommandMock
            .InSequence(sequence)
            .Setup(command => command.Execute(robot, grid));

        RobotInstructionExecutor executor = CreateExecutor(grid, leftCommandMock, rightCommandMock);

        // Act
        Exception? exception = Record.Exception(() => executor.Execute(robot, "lr"));

        // Assert
        Assert.Null(exception);
        leftCommandMock.Verify(command => command.Execute(robot, grid), Times.Once());
        rightCommandMock.Verify(command => command.Execute(robot, grid), Times.Once());
    }

    /// <summary>
    /// Verifies that command execution stops immediately once the robot becomes lost,
    /// preventing any remaining instructions from being executed.
    /// </summary>
    [Fact]
    public void Execute_RobotBecomesLost_StopsExecutingRemainingCommands()
    {
        // Arrange
        Grid grid = new();
        Robot robot = new(0, 0);
        Mock<IRobotInstructionCommand> leftCommandMock = CreateCommandMock('L');
        Mock<IRobotInstructionCommand> rightCommandMock = CreateCommandMock('R');

        leftCommandMock
            .Setup(command => command.Execute(robot, grid))
            .Callback(() => robot.IsLost = true);

        RobotInstructionExecutor executor = CreateExecutor(grid, leftCommandMock, rightCommandMock);

        // Act
        executor.Execute(robot, "LR");

        // Assert
        Assert.True(robot.IsLost);
        leftCommandMock.Verify(command => command.Execute(robot, grid), Times.Once());
        rightCommandMock.Verify(command => command.Execute(robot, grid), Times.Never());
    }

    private static RobotInstructionExecutor CreateExecutor(Grid grid, params Mock<IRobotInstructionCommand>[] commandMocks)
    {
        return new RobotInstructionExecutor(grid, commandMocks.Select(commandMock => commandMock.Object));
    }
}