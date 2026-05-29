using MartianRobot.Commands;
using MartianRobot.Models;

namespace MartianRobot.Test.Commands;


/// <summary>
/// Contains unit tests for <see cref="TurnRightCommand.Execute(Robot, Grid)"/>.
/// </summary>
public class TurnRightCommandTests
{
    /// <summary>
    /// Verifies that executing the command with each valid heading rotates the robot clockwise to the expected heading.
    /// </summary>
    /// <param name="initialHeading">The robot heading before execution.</param>
    /// <param name="expectedHeading">The expected robot heading after execution.</param>
    [Theory]
    [InlineData(Heading.North, Heading.East)]
    [InlineData(Heading.East, Heading.South)]
    [InlineData(Heading.South, Heading.West)]
    [InlineData(Heading.West, Heading.North)]
    public void Execute_ValidHeading_RotatesClockwise(Heading initialHeading, Heading expectedHeading)
    {
        // Arrange
        Robot robot = new(0, 0, initialHeading);
        Grid grid = new();
        TurnRightCommand command = new();

        // Act
        command.Execute(robot, grid);

        // Assert
        Assert.Equal(expectedHeading, robot.Heading);
    }

    /// <summary>
    /// Verifies that executing the command with a null robot throws <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void Execute_NullRobot_ThrowsArgumentNullException()
    {
        // Arrange
        Robot? robot = null;
        Grid grid = new();
        TurnRightCommand command = new();

        // Act
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => command.Execute(robot!, grid));

        // Assert
        Assert.Equal("robot", exception.ParamName);
    }

    /// <summary>
    /// Verifies that executing the command with a null grid throws <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void Execute_NullGrid_ThrowsArgumentNullException()
    {
        // Arrange
        Robot robot = new(0, 0, Heading.North);
        Grid? grid = null;
        TurnRightCommand command = new();

        // Act
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => command.Execute(robot, grid!));

        // Assert
        Assert.Equal("grid", exception.ParamName);
    }

    /// <summary>
    /// Verifies that executing the command with an undefined heading value throws <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    [Fact]
    public void Execute_InvalidHeading_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        Heading invalidHeading = (Heading)int.MaxValue;
        Robot robot = new(0, 0, invalidHeading);
        Grid grid = new();
        TurnRightCommand command = new();

        // Act
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() => command.Execute(robot, grid));

        // Assert
        Assert.Equal("Heading", exception.ParamName);
    }

    /// <summary>
    /// Verifies that <see cref="TurnRightCommand.CommandText"/> returns the expected display text
    /// when accessed on a valid command instance.
    /// </summary>
    [Fact]
    public void CommandText_WhenAccessed_ReturnsTurnRight()
    {
        // Arrange
        TurnRightCommand command = new();

        // Act
        string result = command.CommandText;

        // Assert
        Assert.Equal("Turn Right", result);
        Assert.False(string.IsNullOrWhiteSpace(result));
    }
}