# MartianRobot

A simple, extensible console application moving robots on a Martian grid. (stricly Euclidean)

## Overview

MartianRobot allows you to control one or more robots on a rectangular grid representing the surface of Mars. 
Robots can move forward, turn left, or turn right based on a sequence of commands. 
If a robot moves off the grid, it is marked as LOST, and its last position is recorded to prevent future robots from falling off at the same spot.

## Features

- Define grid size (maximum 50x50).
- Add robots with initial position and heading (N, E, S, W).
- Input movement instructions as a string (e.g., `FFRFLF`).
- Robots execute commands: Forward (`F`), Left (`L`), Right (`R`).
- Robots are marked LOST if they move out of bounds; lost positions are tracked.
- Validates input: grid size and instruction length.
- Designed for easy extension (e.g., adding new movement commands).

## Sample Usage

**Input:**
```
5 3
1 1 E
RFRFRFRF

3 2 N
FRRFLLFFRRFLL

0 3 W
LLFFFLFLFL
```

**Output:**
```
1 1 E
3 3 N LOST
2 3 S
```

## Extensibility

The app uses delegates and a movement service to process robot commands. To add a new movement (e.g., Backward), simply:

1. Implement a new method in the movement service (e.g., `Backward(Robot robot)`).
2. Update the command switch in the robot class to handle the new command (e.g., `'B' => movementService.Backward`).
3. No major refactoring required.

This design makes it easy to extend the robot’s capabilities.

## Project Structure

- **Models**: Data structures for robots and grid.
- **Services**: Movement logic and command processing.
- **Common**: Shared delegates and types.
- **Program.cs**: Entry point and user interaction.

## Prerequisites for running the App

1. Make sure you have .NET 9.0 SDK installed on your machine.
2. Build and run the app:
3. To input your own commands, you can modify the `Program.cs` file and uncomment the section below the default inputs.
4. Please run the application in "Release" mode. Runnning in Debug mode will write the result of every step to the console.

## Not included
- Unit tests for the application logic.
- Robots can't move siumultaneously and the biggest grid size is 50x50. (as requested)
- No example of adding a new movement like Backwords.  
