using MartianRobot.Commands;
using MartianRobot.Models;
using MartianRobot.Services;

namespace MartianRobot.Test;

public class RobotTest1
{
    public RobotTest1()
    {
        Grid.Width = 50;
        Grid.Height = 50;
        Grid.Reset();
    }

    [Fact]
    public void Robot_Initializes_Correctly()
    {
        var robot = new Robot(1, 2, Heading.East);

        Assert.Equal(new Position(1, 2), robot.Position);
        Assert.Equal(Heading.East, robot.Heading);
        Assert.False(robot.IsLost);
    }

    [Fact]
    public void Robot_Throws_On_Invalid_Starting_Position()
    {
        Assert.Throws<ArgumentException>(() => new Robot(-1, 0, Heading.North));
    }

    [Fact]
    public void RobotInstructionExecutor_Executes_Valid_Instruction_Sequence()
    {
        var robot = new Robot(1, 1, Heading.East);
        var executor = new RobotInstructionExecutor();

        executor.Execute(robot, "RFRFRFRF");

        Assert.Equal(new Position(1, 1), robot.Position);
        Assert.Equal(Heading.East, robot.Heading);
        Assert.False(robot.IsLost);
    }

    [Fact]
    public void TurnRightCommand_Changes_Heading()
    {
        var robot = new Robot(0, 0, Heading.North);
        var command = new TurnRightCommand();

        command.Execute(robot);

        Assert.Equal(Heading.East, robot.Heading);
    }

    [Fact]
    public void TurnLeftCommand_Changes_Heading()
    {
        var robot = new Robot(0, 0, Heading.North);
        var command = new TurnLeftCommand();

        command.Execute(robot);

        Assert.Equal(Heading.West, robot.Heading);
    }

    [Fact]
    public void MoveForwardCommand_Updates_Position()
    {
        var robot = new Robot(2, 2, Heading.North);
        var command = new MoveForwardCommand();

        command.Execute(robot);

        Assert.Equal(new Position(2, 3), robot.Position);
    }

    [Fact]
    public void RobotInstructionExecutor_Throws_On_Invalid_Instruction()
    {
        var robot = new Robot(1, 1, Heading.North);
        var executor = new RobotInstructionExecutor();

        Assert.Throws<ArgumentException>(() => executor.Execute(robot, "FX"));
    }

    [Fact]
    public void RobotInstructionExecutor_Throws_On_Too_Long_Instruction()
    {
        var robot = new Robot(1, 1, Heading.North);
        var executor = new RobotInstructionExecutor();
        string longInstructions = new('F', 101);

        Assert.Throws<ArgumentException>(() => executor.Execute(robot, longInstructions));
    }

    [Fact]
    public void MoveForwardCommand_Off_Grid_Sets_IsLost_And_Scents_Position()
    {
        Grid.Width = 2;
        Grid.Height = 2;

        var robot = new Robot(2, 2, Heading.North);
        var command = new MoveForwardCommand();

        command.Execute(robot);

        Assert.True(robot.IsLost);

        var robot2 = new Robot(2, 2, Heading.North);
        command.Execute(robot2);

        Assert.False(robot2.IsLost);
        Assert.Equal(new Position(2, 2), robot2.Position);
    }

    [Fact]
    public void TurnLeftCommand_Wraps_Around_Correctly()
    {
        var robot = new Robot(1, 1, Heading.North);
        var command = new TurnLeftCommand();

        command.Execute(robot);
        Assert.Equal(Heading.West, robot.Heading);

        command.Execute(robot);
        Assert.Equal(Heading.South, robot.Heading);

        command.Execute(robot);
        Assert.Equal(Heading.East, robot.Heading);

        command.Execute(robot);
        Assert.Equal(Heading.North, robot.Heading);
    }

    [Fact]
    public void MoveForwardCommand_Does_Not_Change_Position_If_Scented()
    {
        Grid.Width = 1;
        Grid.Height = 1;

        var robot = new Robot(1, 1, Heading.North);
        var command = new MoveForwardCommand();

        command.Execute(robot);
        Assert.True(robot.IsLost);

        var robot2 = new Robot(1, 1, Heading.North);
        command.Execute(robot2);

        Assert.False(robot2.IsLost);
        Assert.Equal(new Position(1, 1), robot2.Position);
    }
}