using MartianRobot.Models;


namespace MartianRobot.Test.Models;

/// <summary>
/// Contains unit tests for <see cref="Robot"/>.
/// </summary>
public class RobotTests
{
    /// <summary>
    /// Verifies that <see cref="Robot.ToString"/> returns the expected formatted output
    /// for each defined <see cref="Heading"/>, representative coordinate values, and both lost states.
    /// </summary>
    /// <param name="x">The robot X coordinate.</param>
    /// <param name="y">The robot Y coordinate.</param>
    /// <param name="heading">The robot heading.</param>
    /// <param name="isLost">A value indicating whether the robot is lost.</param>
    /// <param name="expected">The expected string representation.</param>
    [Theory]
    [InlineData(0, 0, Heading.North, false, "0 0 N")]
    [InlineData(-1, 1, Heading.East, false, "-1 1 E")]
    [InlineData(5, -5, Heading.South, true, "5 -5 S LOST")]
    [InlineData(int.MinValue, int.MaxValue, Heading.West, true, "-2147483648 2147483647 W LOST")]
    public void ToString_ValidState_ReturnsExpectedFormattedValue(int x, int y, Heading heading, bool isLost, string expected)
    {
        // Arrange
        Robot robot = new(x, y, heading)
        {
            IsLost = isLost
        };

        // Act
        string result = robot.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Verifies that setting an invalid value for <see cref="Robot.Heading"/> throws <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    [Fact]
    public void Heading_InvalidValue_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        Robot robot = new(0, 0);

        // Act
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => robot.Heading = (Heading)(-1));

        // Assert
        Assert.Equal("value", exception.ParamName);
    }
}