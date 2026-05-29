using MartianRobot.Commands;
using MartianRobot.Models;

namespace MartianRobot.Test.Commands;


public class MoveDiagonalRightCommandTests
{
    /// <summary>
    /// Verifies that <see cref="MoveDiagonalRightCommand.CommandText"/> returns the expected user-facing label.
    /// The input condition is a newly created <see cref="MoveDiagonalRightCommand"/> instance with no additional setup.
    /// The expected result is the exact string <c>Move Diagonal Right</c> and a non-empty, non-whitespace value.
    /// </summary>
    [Fact]
    public void CommandText_WhenAccessed_ReturnsExpectedDisplayText()
    {
        // Arrange
        MoveDiagonalRightCommand command = new();

        // Act
        string result = command.CommandText;

        // Assert
        Assert.Equal("Move Diagonal Right", result);
        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    /// <summary>
    /// Verifies that <see cref="MoveDiagonalRightCommand.Execute(Robot, Grid)"/> throws when the robot argument is null.
    /// Input condition: a null robot and a valid grid.
    /// Expected result: an <see cref="ArgumentNullException"/> is thrown for robot.
    /// </summary>
    [Fact]
    public void Execute_RobotIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        MoveDiagonalRightCommand command = new();
        Robot? robot = null;
        Grid grid = new();

        // Act
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => command.Execute(robot!, grid));

        // Assert
        Assert.Equal("robot", exception.ParamName);
    }

    /// <summary>
    /// Verifies that <see cref="MoveDiagonalRightCommand.Execute(Robot, Grid)"/> throws when the grid argument is null.
    /// Input condition: a valid robot and a null grid.
    /// Expected result: an <see cref="ArgumentNullException"/> is thrown for grid.
    /// </summary>
    [Fact]
    public void Execute_GridIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        MoveDiagonalRightCommand command = new();
        Robot robot = new(1, 1, Heading.North);
        Grid? grid = null;

        // Act
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => command.Execute(robot, grid!));

        // Assert
        Assert.Equal("grid", exception.ParamName);
    }

    /// <summary>
    /// Verifies that <see cref="MoveDiagonalRightCommand.Execute(Robot, Grid)"/> moves the robot diagonally right for each defined heading.
    /// Input condition: a robot starting at a valid in-bounds position with each supported heading.
    /// Expected result: the robot position is updated to the correct diagonal coordinate and the robot is not lost.
    /// </summary>
    [Theory]
    [InlineData(Heading.North, 1, 1, 2, 2)]
    [InlineData(Heading.East, 1, 1, 2, 0)]
    [InlineData(Heading.South, 1, 1, 0, 0)]
    [InlineData(Heading.West, 1, 1, 0, 2)]
    public void Execute_HeadingIsValidAndMoveStaysWithinBounds_UpdatesPosition(
        Heading heading,
        int startX,
        int startY,
        int expectedX,
        int expectedY)
    {
        // Arrange
        MoveDiagonalRightCommand command = new();
        Robot robot = new(startX, startY, heading);
        Grid grid = new();

        // Act
        command.Execute(robot, grid);

        // Assert
        Assert.Equal(new Position(expectedX, expectedY), robot.Position);
        Assert.False(robot.IsLost);
    }

    /// <summary>
    /// Verifies that <see cref="MoveDiagonalRightCommand.Execute(Robot, Grid)"/> throws for an undefined heading value.
    /// Input condition: a robot whose heading is outside the defined <see cref="Heading"/> enum range.
    /// Expected result: an <see cref="ArgumentOutOfRangeException"/> is thrown for Heading.
    /// </summary>
    [Fact]
    public void Execute_HeadingIsUndefined_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        MoveDiagonalRightCommand command = new();
        Robot robot = new(1, 1, (Heading)int.MaxValue);
        Grid grid = new();

        // Act
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() => command.Execute(robot, grid));

        // Assert
        Assert.Equal("Heading", exception.ParamName);
    }

    /// <summary>
    /// Verifies that <see cref="MoveDiagonalRightCommand.Execute(Robot, Grid)"/> marks the robot as lost when the move leaves the grid from a non-lost position.
    /// Input condition: a robot at the grid edge with no existing lost marker at its current position.
    /// Expected result: the robot becomes lost, its current position is recorded as lost, and its position does not change.
    /// </summary>
    [Fact]
    public void Execute_MoveLeavesGridAndCurrentPositionIsNotLost_MarksRobotLostAndStoresLostPosition()
    {
        // Arrange
        MoveDiagonalRightCommand command = new();
        Robot robot = new(0, 0, Heading.North);
        Grid grid = new(0, 0);

        // Act
        command.Execute(robot, grid);

        // Assert
        Assert.True(robot.IsLost);
        Assert.Equal(new Position(0, 0), robot.Position);
        Assert.True(grid.IsLostPosition(0, 0));
    }

    /// <summary>
    /// Verifies that <see cref="MoveDiagonalRightCommand.Execute(Robot, Grid)"/> ignores an out-of-bounds move when the current position is already marked as lost.
    /// Input condition: a robot at the grid edge with an existing lost marker at its current position.
    /// Expected result: the robot is not marked lost again and its position remains unchanged.
    /// </summary>
    [Fact]
    public void Execute_MoveLeavesGridAndCurrentPositionIsAlreadyLost_IgnoresInstruction()
    {
        // Arrange
        MoveDiagonalRightCommand command = new();
        Robot robot = new(0, 0, Heading.North);
        Grid grid = new(0, 0);
        grid.AddLostPosition(0, 0);

        // Act
        command.Execute(robot, grid);

        // Assert
        Assert.False(robot.IsLost);
        Assert.Equal(new Position(0, 0), robot.Position);
        Assert.True(grid.IsLostPosition(0, 0));
        Assert.Single(grid.LostPositions);
    }
}