using MartianRobot.Commands;
using MartianRobot.Models;

namespace MartianRobot.Test.Commands;


/// <summary>
/// Contains unit tests for <see cref="MoveDiagonalLeftCommand"/>.
/// </summary>
public class MoveDiagonalLeftCommandTests
{
    /// <summary>
    /// Verifies that <see cref="MoveDiagonalLeftCommand.Execute(Robot, Grid)"/> throws
    /// <see cref="ArgumentNullException"/> when either required dependency argument is <see langword="null"/>.
    /// </summary>
    /// <param name="useNullRobot">
    /// <see langword="true"/> to pass a <see langword="null"/> robot; otherwise a <see langword="null"/> grid.
    /// </param>
    /// <param name="expectedParamName">The expected exception parameter name.</param>
    [Theory]
    [InlineData(true, "robot")]
    [InlineData(false, "grid")]
    public void Execute_NullDependency_ThrowsArgumentNullException(bool useNullRobot, string expectedParamName)
    {
        // Arrange
        MoveDiagonalLeftCommand command = new();
        Robot? robot = useNullRobot ? null : new Robot(1, 1, Heading.North);
        Grid? grid = useNullRobot ? new Grid(5, 5) : null;

        // Act
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => command.Execute(robot!, grid!));

        // Assert
        Assert.Equal(expectedParamName, exception.ParamName);
    }

    /// <summary>
    /// Verifies that <see cref="MoveDiagonalLeftCommand.Execute(Robot, Grid)"/> moves the robot
    /// to the correct diagonal-left position for each supported heading when the target is within bounds.
    /// </summary>
    /// <param name="heading">The robot heading to test.</param>
    /// <param name="startX">The starting X coordinate.</param>
    /// <param name="startY">The starting Y coordinate.</param>
    /// <param name="expectedX">The expected X coordinate after execution.</param>
    /// <param name="expectedY">The expected Y coordinate after execution.</param>
    [Theory]
    [InlineData(Heading.North, 1, 1, 0, 2)]
    [InlineData(Heading.East, 1, 1, 2, 2)]
    [InlineData(Heading.South, 1, 1, 2, 0)]
    [InlineData(Heading.West, 1, 1, 0, 0)]
    public void Execute_TargetIsWithinBounds_UpdatesPositionBasedOnHeading(
        Heading heading,
        int startX,
        int startY,
        int expectedX,
        int expectedY)
    {
        // Arrange
        MoveDiagonalLeftCommand command = new();
        Robot robot = new(startX, startY, heading);
        Grid grid = new(5, 5);

        // Act
        command.Execute(robot, grid);

        // Assert
        Assert.Equal(new Position(expectedX, expectedY), robot.Position);
        Assert.False(robot.IsLost);
        Assert.Empty(grid.LostPositions);
    }

    /// <summary>
    /// Verifies that <see cref="MoveDiagonalLeftCommand.Execute(Robot, Grid)"/> throws
    /// <see cref="ArgumentOutOfRangeException"/> when the robot heading is outside the defined enum range.
    /// </summary>
    [Fact]
    public void Execute_InvalidHeading_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        MoveDiagonalLeftCommand command = new();
        Robot robot = new(1, 1, (Heading)999);
        Grid grid = new(5, 5);

        // Act
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() => command.Execute(robot, grid));

        // Assert
        Assert.Equal("Heading", exception.ParamName);
    }

    /// <summary>
    /// Verifies that <see cref="MoveDiagonalLeftCommand.Execute(Robot, Grid)"/> marks the robot as lost
    /// and records the current position when the diagonal-left target is outside the grid and no lost marker exists there.
    /// </summary>
    [Fact]
    public void Execute_TargetIsOutOfBoundsAndCurrentPositionIsNotLost_MarksRobotLostAndStoresLostPosition()
    {
        // Arrange
        MoveDiagonalLeftCommand command = new();
        Robot robot = new(0, 0, Heading.West);
        Grid grid = new(5, 5);

        // Act
        command.Execute(robot, grid);

        // Assert
        Assert.True(robot.IsLost);
        Assert.Equal(new Position(0, 0), robot.Position);
        Assert.Contains((0, 0), grid.LostPositions);
        Assert.Single(grid.LostPositions);
    }

    /// <summary>
    /// Verifies that <see cref="MoveDiagonalLeftCommand.Execute(Robot, Grid)"/> does nothing
    /// when the diagonal-left target is outside the grid but the current position already has a lost marker.
    /// </summary>
    [Fact]
    public void Execute_TargetIsOutOfBoundsAndCurrentPositionIsAlreadyLost_DoesNotMoveOrMarkRobotLost()
    {
        // Arrange
        MoveDiagonalLeftCommand command = new();
        Robot robot = new(0, 0, Heading.West);
        Grid grid = new(5, 5);
        grid.AddLostPosition(0, 0);

        // Act
        command.Execute(robot, grid);

        // Assert
        Assert.False(robot.IsLost);
        Assert.Equal(new Position(0, 0), robot.Position);
        Assert.Contains((0, 0), grid.LostPositions);
        Assert.Single(grid.LostPositions);
    }

    /// <summary>
    /// Verifies that <see cref="MoveDiagonalLeftCommand.CommandText"/> returns the exact command label
    /// for a newly created command instance.
    /// </summary>
    [Fact]
    public void CommandText_NewInstance_ReturnsExpectedCommandLabel()
    {
        // Arrange
        MoveDiagonalLeftCommand command = new();

        // Act
        string commandText = command.CommandText;

        // Assert
        Assert.Equal("Move Diagonal Left", commandText);
        Assert.False(string.IsNullOrWhiteSpace(commandText));
    }
}