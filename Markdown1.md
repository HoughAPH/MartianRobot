
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
