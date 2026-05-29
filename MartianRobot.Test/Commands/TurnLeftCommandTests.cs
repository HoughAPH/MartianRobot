using MartianRobot.Commands;
using MartianRobot.Models;

namespace MartianRobot.Test.Commands;


/// <summary>
/// Contains unit tests for <see cref="TurnLeftCommand"/>.
/// </summary>
public class TurnLeftCommandTests
{
    /// <summary>
    /// Verifies that <see cref="TurnLeftCommand.CommandText"/> returns the expected command label
    /// when accessed on a valid command instance.
    /// </summary>
    [Fact]
    public void CommandText_WhenAccessed_ReturnsTurnLeft()
    {
        // Arrange
        TurnLeftCommand command = new();

        // Act
        string commandText = command.CommandText;

        // Assert
        Assert.NotNull(commandText);
        Assert.Equal("Turn Left", commandText);
    }

    /// <summary>
    /// Verifies that Execute rotates the robot one step to the left for each defined heading value when valid arguments are provided.
    /// </summary>
    /// <param name="initialHeading">The starting heading assigned to the robot.</param>
    /// <param name="expectedHeading">The heading expected after executing the command once.</param>
    [Theory]
    [InlineData(Heading.North, Heading.West)]
    [InlineData(Heading.West, Heading.South)]
    [InlineData(Heading.South, Heading.East)]
    [InlineData(Heading.East, Heading.North)]
    public void Execute_ValidHeading_UpdatesHeadingCounterClockwise(Heading initialHeading, Heading expectedHeading)
    {
        // Arrange
        Robot robot = CreateRobot(initialHeading);
        Grid grid = CreateGrid();
        TurnLeftCommand command = new();

        // Act
        Exception? exception = Record.Exception(() => command.Execute(robot, grid));

        // Assert
        Assert.Null(exception);
        Assert.Equal(expectedHeading, robot.Heading);
    }

    /// <summary>
    /// Verifies that Execute throws an ArgumentNullException when the robot argument is null and the grid argument is valid.
    /// </summary>
    [Fact]
    public void Execute_NullRobot_ThrowsArgumentNullException()
    {
        // Arrange
        Robot? robot = null;
        Grid grid = CreateGrid();
        TurnLeftCommand command = new();

        // Act
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => command.Execute(robot!, grid));

        // Assert
        Assert.Equal("robot", exception.ParamName);
    }

    /// <summary>
    /// Verifies that Execute throws an ArgumentNullException when the grid argument is null and the robot argument is valid.
    /// </summary>
    [Fact]
    public void Execute_NullGrid_ThrowsArgumentNullException()
    {
        // Arrange
        Robot robot = CreateRobot(Heading.North);
        Grid? grid = null;
        TurnLeftCommand command = new();

        // Act
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => command.Execute(robot, grid!));

        // Assert
        Assert.Equal("grid", exception.ParamName);
    }

    /// <summary>
    /// Verifies that Execute throws an ArgumentOutOfRangeException when the robot heading contains an undefined enum value.
    /// </summary>
    /// <param name="invalidHeadingValue">An integer outside the defined Heading enum range.</param>
    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void Execute_InvalidHeading_ThrowsArgumentOutOfRangeException(int invalidHeadingValue)
    {
        // Arrange
        Robot robot = CreateRobot((Heading)invalidHeadingValue);
        Grid grid = CreateGrid();
        TurnLeftCommand command = new();

        // Act
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() => command.Execute(robot, grid));

        // Assert
        Assert.Equal("Heading", exception.ParamName);
    }

    private static Robot CreateRobot(Heading heading)
    {
        return new Robot(0, 0, heading);
    }

    private static Grid CreateGrid()
    {
        return new Grid();
    }
}