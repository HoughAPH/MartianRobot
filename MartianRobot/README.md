# MartianRobot

A small, extensible `.NET 9` console application that simulates robots moving on a rectangular Martian grid.

## Overview

`MartianRobot` simulates robots moving on a bounded grid using a sequence of commands.

Supported commands:

- `F` — move forward one grid point
- `L` — turn left
- `R` — turn right

If a robot moves off the grid, it becomes `LOST`. The last valid position is then marked as scented so a future robot can ignore the same fatal move.

## Rules

- Grid coordinates are bounded by a maximum of `50 x 50`
- A robot has:
  - a `Position`
  - a `Heading`
  - an `IsLost` state
- Valid headings are:
  - `N`
  - `E`
  - `S`
  - `W`
- Instruction strings are limited to 100 characters
- Invalid instructions throw an exception

## Sample Input / Output

### Input
```
5 3
1 1 E
RFRFRFRF

3 2 N
FRRFLLFFRRFLL

0 3 W
LLFFFLFLFL
```

### Output
```
1 1 E
3 3 N LOST
2 3 S
```

## Extensibility

The app is designed to be extensible. To add new robot commands:

1. Create a new command class implementing `ICommand`.
2. Implement the command's logic in the `Execute` method.
3. Register the new command in the `RobotInstructionExecutor`.

### Example: Adding a Backward Command

1. Create `BackwardCommand` class.

   ```csharp
   public class BackwardCommand : ICommand
   {
       public void Execute(Robot robot)
       {
           // Implement backward movement
       }
   }
   ```

2. Register the command.

   ```csharp
   executor.RegisterCommand('B', new BackwardCommand());
   ```

3. Use `B` in instruction strings for backward movement.

## Project Structure

- **Models**: Data structures for robots and grid.
- **Services**: Movement logic and command processing.
- **Common**: Shared delegates and types.
- **Program.cs**: Entry point and user interaction.

# # Test

The project includes unit tests for core functionality, ensuring correct robot movement, instruction execution, and edge case handling.

## Not included
- Robots can't move siumultaneously and the biggest grid size is 50x50. (as requested)
- No example of adding a new movement like Backwords.  

## Refactoring Summary

This project was refactored to improve structure, readability, and extensibility.

### Key changes
- Simplified `Program.cs` into a small runner for valid sample scenarios
- Refactored `Robot` to focus on state only
- Added `RobotInstructionExecutor` to handle instruction validation and execution
- Replaced switch-based instruction handling with extensible command objects
- Replaced string headings with a strongly typed `Heading` enum
- Added a dedicated `Position` model for robot coordinates
- Simplified `RobotMovementService` to explicit operations:
  - `MoveForward(Robot)`
  - `TurnLeft(Robot)`
  - `TurnRight(Robot)`

### Benefits
- Better separation of concerns
- Stronger typing and fewer invalid states
- Easier to maintain and extend
- New move types can be added by introducing a new command and registering it in `RobotInstructionExecutor`

### Future improvement
- `Grid` is still static and mutable
- A future enhancement would be to make grid state instance-based for better isolation