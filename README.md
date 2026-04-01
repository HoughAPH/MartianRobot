# MartianRobot

This repository contains a `.NET 9` solution for the Martian Robots coding exercise.

## Original test statement

The original exercise was to build a program that simulates robots moving on a rectangular Martian grid.

### Requirements
- The grid is bounded, with a maximum size of `50 x 50`
- Each robot has:
  - a starting position
  - a heading of `N`, `E`, `S`, or `W`
- Each robot receives an instruction string containing:
  - `L` to turn left
  - `R` to turn right
  - `F` to move forward
- If a robot moves off the grid, it becomes `LOST`
- When a robot is lost, it leaves a scent at its last valid position
- Future robots ignore a move that would cause them to be lost from the same scented position
- Instruction strings are limited to `100` characters

The sample input and output from the original exercise are documented in `MartianRobot/README.md`.

## Improvements made

This solution was extended beyond a minimal implementation to improve design, readability, and maintainability.

### Testing
- Added a separate test project: `MartianRobot.Test`
- Added unit coverage for key behavior such as:
  - robot initialization
  - turning left and right
  - forward movement
  - invalid starting positions
  - invalid instructions
  - over-length instruction strings
  - lost robot behavior
  - scented position behavior

### Refactoring
- Simplified `Program.cs` so it acts as a small runner for the sample scenarios
- Refactored `Robot` to focus on state rather than orchestration
- Added `RobotInstructionExecutor` to validate and execute instruction sequences
- Replaced primitive heading values with a strongly typed `Heading` enum
- Added a dedicated `Position` model
- Introduced command-based instruction handling through `IRobotInstructionCommand`
- Kept movement behavior encapsulated in command classes such as:
  - `MoveForwardCommand`
  - `TurnLeftCommand`
  - `TurnRightCommand`

## Benefits
- Clearer separation of concerns
- Better testability
- Stronger typing
- Easier maintenance
- Easier extension for future commands

## Solution structure
- `MartianRobot/` - main console application
- `MartianRobot.Test/` - unit test project

## Notes
The root `README.md` summarises the original exercise and the improvements made in this submission.

The app-specific rules and sample scenario are documented in `MartianRobot/README.md`.