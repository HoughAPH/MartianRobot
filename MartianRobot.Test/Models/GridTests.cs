using MartianRobot.Models;


namespace MartianRobot.Test.Models;

/// <summary>
/// Contains unit tests for <see cref="Grid"/>.
/// </summary>
public sealed class GridTests
{
    /// <summary>
    /// Verifies that <see cref="Grid.Reset"/> does not throw and keeps
    /// <see cref="Grid.LostPositions"/> empty when the grid has no recorded lost positions.
    /// </summary>
    [Fact]
    public void Reset_NoLostPositions_KeepsCollectionEmpty()
    {
        // Arrange
        Grid grid = new();
        HashSet<(int X, int Y)> lostPositionsBeforeReset = grid.LostPositions;

        // Act
        Exception? exception = Record.Exception(() => grid.Reset());

        // Assert
        Assert.Null(exception);
        Assert.Same(lostPositionsBeforeReset, grid.LostPositions);
        Assert.Empty(grid.LostPositions);
    }

    /// <summary>
    /// Verifies that <see cref="Grid.Reset"/> clears all previously recorded lost positions
    /// and preserves the existing <see cref="Grid.LostPositions"/> collection instance.
    /// </summary>
    [Fact]
    public void Reset_LostPositionsExist_ClearsAllRecordedPositions()
    {
        // Arrange
        Grid grid = new();
        grid.AddLostPosition(0, 0);
        grid.AddLostPosition(1, 2);
        grid.AddLostPosition(int.MaxValue, int.MinValue);
        HashSet<(int X, int Y)> lostPositionsBeforeReset = grid.LostPositions;

        // Act
        grid.Reset();

        // Assert
        Assert.Same(lostPositionsBeforeReset, grid.LostPositions);
        Assert.Empty(grid.LostPositions);
        Assert.DoesNotContain((0, 0), grid.LostPositions);
        Assert.DoesNotContain((1, 2), grid.LostPositions);
        Assert.DoesNotContain((int.MaxValue, int.MinValue), grid.LostPositions);
    }
}