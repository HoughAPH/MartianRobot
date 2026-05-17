using MartianRobot.Commands;
using MartianRobot.Models;
using MartianRobot.Services;

namespace MartianRobot.Test;

public class RobotTest1
{
    private readonly Grid _grid = new(50, 50);

    public RobotTest1()
    {
        _grid.Reset();
    }

    [Fact]
    public void Robot_Initializes_Correctly()
    {
        Robot robot = new(1, 2, Heading.East);

        Assert.Equal(new Position(1, 2), robot.Position);
        Assert.Equal(Heading.East, robot.Heading);
        Assert.False(robot.IsLost);
    }

    [Fact]
    public void Robot_Throws_On_Invalid_Starting_Position()
    {
        Robot robot = new(-1, 0, Heading.North);
        RobotInstructionExecutor executor = new(_grid);

        Assert.Throws<ArgumentException>(() => executor.Execute(robot, ""));
    }

    [Fact]
    public void RobotInstructionExecutor_Executes_Valid_Instruction_Sequence()
    {
        Robot robot = new(1, 1, Heading.East);
        RobotInstructionExecutor executor = new(_grid);

        executor.Execute(robot, "RFRFRFRF");

        Assert.Equal(new Position(1, 1), robot.Position);
        Assert.Equal(Heading.East, robot.Heading);
        Assert.False(robot.IsLost);
    }

    [Fact]
    public void TurnRightCommand_Changes_Heading()
    {
        Robot robot = new(0, 0, Heading.North);
        TurnRightCommand command = new();

        command.Execute(robot, _grid);

        Assert.Equal(Heading.East, robot.Heading);
    }

    [Fact]
    public void TurnLeftCommand_Changes_Heading()
    {
        Robot robot = new(0, 0, Heading.North);
        TurnLeftCommand command = new();

        command.Execute(robot, _grid);

        Assert.Equal(Heading.West, robot.Heading);
    }

    [Fact]
    public void MoveForwardCommand_Updates_Position()
    {
        Robot robot = new(2, 2, Heading.North);
        MoveForwardCommand command = new();

        command.Execute(robot, _grid);

        Assert.Equal(new Position(2, 3), robot.Position);
    }

    [Fact]
    public void RobotInstructionExecutor_Throws_On_Invalid_Instruction()
    {
        Robot robot = new(1, 1, Heading.North);
        RobotInstructionExecutor executor = new(_grid);

        Assert.Throws<ArgumentException>(() => executor.Execute(robot, "FX"));
    }

    [Fact]
    public void RobotInstructionExecutor_Throws_On_Too_Long_Instruction()
    {
        Robot robot = new(1, 1, Heading.North);
        RobotInstructionExecutor executor = new(_grid);
        string longInstructions = new('F', 101);

        Assert.Throws<ArgumentException>(() => executor.Execute(robot, longInstructions));
    }

    [Fact]
    public void MoveForwardCommand_Off_Grid_Sets_IsLost_And_Scents_Position()
    {
        Grid grid = new(2, 2);

        Robot robot = new(2, 2, Heading.North);
        MoveForwardCommand command = new();

        command.Execute(robot, grid);

        Assert.True(robot.IsLost);

        Robot robot2 = new(2, 2, Heading.North);
        command.Execute(robot2, grid);

        Assert.False(robot2.IsLost);
        Assert.Equal(new Position(2, 2), robot2.Position);
    }

    [Fact]
    public void TurnLeftCommand_Wraps_Around_Correctly()
    {
        Robot robot = new(1, 1, Heading.North);
        TurnLeftCommand command = new();

        command.Execute(robot, _grid);
        Assert.Equal(Heading.West, robot.Heading);

        command.Execute(robot, _grid);
        Assert.Equal(Heading.South, robot.Heading);

        command.Execute(robot, _grid);
        Assert.Equal(Heading.East, robot.Heading);

        command.Execute(robot, _grid);
        Assert.Equal(Heading.North, robot.Heading);
    }

    [Fact]
    public void MoveForwardCommand_Does_Not_Change_Position_If_Scented()
    {
        Grid grid = new(1, 1);

        Robot robot = new(1, 1, Heading.North);
        MoveForwardCommand command = new();

        command.Execute(robot, grid);
        Assert.True(robot.IsLost);

        Robot robot2 = new(1, 1, Heading.North);
        command.Execute(robot2, grid);

        Assert.False(robot2.IsLost);
        Assert.Equal(new Position(1, 1), robot2.Position);
    }
}