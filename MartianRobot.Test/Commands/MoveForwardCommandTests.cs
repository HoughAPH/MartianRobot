using MartianRobot.Commands;
using MartianRobot.Models;

namespace MartianRobot.Test.Commands;


/// <summary>
/// Unit tests for <see cref="MoveForwardCommand"/>.
/// </summary>
public class MoveForwardCommandTests
{
    /// <summary>
    /// Verifies that accessing <see cref="MoveForwardCommand.CommandText"/> returns the expected command label
    /// for the move-forward instruction and that the value is not null or whitespace.
    /// </summary>
    /// <param name="expectedCommandText">The expected command text value.</param>
    [Theory]
    [InlineData("Move Forward")]
    public void CommandText_WhenAccessed_ReturnsExpectedDisplayText(string expectedCommandText)
    {
        // Arrange
        MoveForwardCommand command = new();

        // Act
        var actualCommandText = command.CommandText;

        // Assert
        Assert.Equal(expectedCommandText, actualCommandText);
        Assert.False(string.IsNullOrWhiteSpace(actualCommandText));
    }

    /// <summary>
    /// Verifies that <see cref="MoveForwardCommand.Execute(Robot, Grid)"/> throws an <see cref="ArgumentNullException"/>
    /// when the <paramref name="robot"/> argument is null.
    /// </summary>
    [Fact]
    public void Execute_NullRobot_ThrowsArgumentNullException()
    {
        // Arrange
        MoveForwardCommand command = new();
        Robot? robot = null;
        Grid grid = new(2, 2);

        // Act
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => command.Execute(robot!, grid));

        // Assert
        Assert.Equal("robot", exception.ParamName);
    }

    /// <summary>
    /// Verifies that <see cref="MoveForwardCommand.Execute(Robot, Grid)"/> throws an <see cref="ArgumentNullException"/>
    /// when the <paramref name="grid"/> argument is null.
    /// </summary>
    [Fact]
    public void Execute_NullGrid_ThrowsArgumentNullException()
    {
        // Arrange
        MoveForwardCommand command = new();
        Robot robot = new(1, 1, Heading.North);
        Grid? grid = null;

        // Act
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => command.Execute(robot, grid!));

        // Assert
        Assert.Equal("grid", exception.ParamName);
    }

    /// <summary>
    /// Verifies that <see cref="MoveForwardCommand.Execute(Robot, Grid)"/> moves the robot one step forward
    /// for each valid heading when the destination remains within grid bounds.
    /// </summary>
    /// <param name="heading">The robot heading to test.</param>
    /// <param name="startX">The starting X coordinate.</param>
    /// <param name="startY">The starting Y coordinate.</param>
    /// <param name="expectedX">The expected X coordinate after execution.</param>
    /// <param name="expectedY">The expected Y coordinate after execution.</param>
    [Theory]
    [InlineData(Heading.North, 1, 1, 1, 2)]
    [InlineData(Heading.East, 1, 1, 2, 1)]
    [InlineData(Heading.South, 1, 1, 1, 0)]
    [InlineData(Heading.West, 1, 1, 0, 1)]
    public void Execute_ValidHeadingWithinBounds_UpdatesPosition(
        Heading heading,
        int startX,
        int startY,
        int expectedX,
        int expectedY)
    {
        // Arrange
        MoveForwardCommand command = new();
        Robot robot = new(startX, startY, heading);
        Grid grid = new(2, 2);

        // Act
        command.Execute(robot, grid);

        // Assert
        Assert.False(robot.IsLost);
        Assert.Equal(new Position(expectedX, expectedY), robot.Position);
        Assert.False(grid.IsLostPosition(startX, startY));
    }

    /// <summary>
    /// Verifies that <see cref="MoveForwardCommand.Execute(Robot, Grid)"/> marks the robot as lost,
    /// leaves its position unchanged, and records a lost position when moving forward would leave the grid.
    /// </summary>
    /// <param name="heading">The robot heading that causes an off-grid move.</param>
    /// <param name="startX">The starting X coordinate at the grid edge.</param>
    /// <param name="startY">The starting Y coordinate at the grid edge.</param>
    [Theory]
    [InlineData(Heading.North, 1, 1)]
    [InlineData(Heading.East, 1, 1)]
    [InlineData(Heading.South, 0, 0)]
    [InlineData(Heading.West, 0, 0)]
    public void Execute_MoveWouldLeaveGrid_MarksRobotLostAndScentsPosition(
        Heading heading,
        int startX,
        int startY)
    {
        // Arrange
        MoveForwardCommand command = new();
        Robot robot = new(startX, startY, heading);
        Grid grid = new(1, 1);

        // Act
        command.Execute(robot, grid);

        // Assert
        Assert.True(robot.IsLost);
        Assert.Equal(new Position(startX, startY), robot.Position);
        Assert.True(grid.IsLostPosition(startX, startY));
    }

    /// <summary>
    /// Verifies that <see cref="MoveForwardCommand.Execute(Robot, Grid)"/> ignores an off-grid move
    /// from a previously scented position and leaves the robot unchanged.
    /// </summary>
    [Fact]
    public void Execute_OffGridFromScentedPosition_DoesNotLoseRobotOrChangePosition()
    {
        // Arrange
        MoveForwardCommand command = new();
        Grid grid = new(1, 1);

        Robot firstRobot = new(1, 1, Heading.North);
        command.Execute(firstRobot, grid);

        Robot secondRobot = new(1, 1, Heading.North);

        // Act
        command.Execute(secondRobot, grid);

        // Assert
        Assert.False(secondRobot.IsLost);
        Assert.Equal(new Position(1, 1), secondRobot.Position);
        Assert.True(grid.IsLostPosition(1, 1));
        Assert.Single(grid.LostPositions);
    }

    /// <summary>
    /// Verifies that <see cref="MoveForwardCommand.Execute(Robot, Grid)"/> throws an
    /// <see cref="ArgumentOutOfRangeException"/> when the robot heading is outside the defined <see cref="Heading"/> values.
    /// </summary>
    /// <param name="invalidHeading">An undefined enum value.</param>
    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void Execute_InvalidHeading_ThrowsArgumentOutOfRangeException(int invalidHeading)
    {
        // Arrange
        MoveForwardCommand command = new();
        Robot robot = new(1, 1, (Heading)invalidHeading);
        Grid grid = new(2, 2);

        // Act
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() => command.Execute(robot, grid));

        // Assert
        Assert.Equal("Heading", exception.ParamName);
    }
}