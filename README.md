# MartianRobot - UI branch

This branch is my follow-on version of the original `MartianRobot` challenge and solution project.

The original project is still the main reference point, but this branch extends the robot logic and adds a web UI so the scenarios can be created and run visually in a browser.

## What this branch is

This branch is an extension of the original console app.

The goal of this branch is to keep the original Martian Robot rules in the core project and expose them through UI projects. The console app was changed to build as a library project, and a new Blazor WebAssembly UI was added to run the robot scenarios in a browser.

## Where to look for the original console application

If you want to review the original console app as it was first implemented, look at the `master` branch.

## Important demonstration of the Command Pattern.
The original console app was implemented using the Command Pattern. 
Theorerically any command can be implemented as a class that implements the `IRobotCommand` interface. The `RobotCommandFactory` class is responsible for creating the appropriate command object based on the input character. The `Robot` class executes the commands by calling the `Execute` method on each command object.
However the 'RouteToInstructionConverter' class will have to be changed for new commands as is not possible to include it in the Command itself yet.

## What changed in this branch

The main change is that the original `MartianRobot` project is no longer the app you start directly.

Instead, it acts as the core logic project used by the UI.

In other words:

- `MartianRobot` contains the robot rules, models, and instruction execution logic
- `RobotGrid.Client` provides the Blazor WebAssembly UI
- `RobotGrid` is the ASP.NET Core host project for the UI

## Project structure

- `MartianRobot/` - reusable robot logic and domain model
- `RobotGrid.Client/` - Blazor WebAssembly front end
- `RobotGrid/` - ASP.NET Core host project

## Why it was changed this way

The robot movement command logic is kept separate from the UI.

That makes the solution easier to understand because:

- the Martian Robot behaviour stays in one place
- the UI can use that logic without duplicating it
- the original rules are still preserved
- the application is easier to extend beyond a console-only version

## Important note

`MartianRobot` is now a library project in this branch, not the startup project.

If you want to run the UI version, start the web host project rather than `MartianRobot` directly.

## UI rendering techniques
- The animation is displayed as a sequence of frames, one frame per robot instruction.  
- animation frames are built as `RobotGridAnimationFrame` data
- the grid is rendered as an HTML table
- a reusable `HTMLGrid` component is used to display that frame data

That shared component is used by:

- `RobotGrid.Client/Pages/RobotGrid.razor`
- `RobotGrid.Client/Pages/RobotScenarioRunner.razor`

The `HTMLGrid` component shows:

- the robot start position
- the initial heading
- the current robot position
- the current heading
- visited cells
- lost scent positions
- step numbers for visited cells

## UI pages in this branch

### RobotGrid

`RobotGrid` is a demo page that shows the default robot scenarios and animates them in the browser.

It uses the shared `HTMLGrid` component to display each scenario.

### Robot Scenario Runner

This branch includes a separate `Robot Scenario Runner` page for running a single robot scenario directly.

On that page you can enter:

- grid size
- robot start position
- starting heading
- instruction string

You can then run the scenario and watch it animate step by step.

This page accepts input in two ways:

- manual input entered directly by the user
- generated input passed from the `Route Builder` page

It also supports stopping the animation while it is running.

### Route Builder

This branch also includes a `Route Builder` page that lets you create a route by clicking cells on a grid.

As the route is built, the instruction string is generated from the selected cells.

The page treats the grid as 0-based:

- the first click sets the start cell
- each next click adds the next move in the route

The route builder also supports diagonal movement commands:

- diagonal-left = `Q`
- diagonal-right = `P`

Because heading is inferred from movement between cells, the route must follow a few rules:

- the first click sets the robot's starting position
- the second click must be orthogonally adjacent to the first click
- the second click establishes the robot's initial heading
- each new step must go to a neighboring cell
- diagonal steps are allowed
- the first move must be orthogonal
- diagonal moves must be forward-left or forward-right relative to the current heading
- When a diagonal move is backwards, the instruction builder will add the appropriate turn command before adding a valid diagonal move. For example, if the robot is facing north and the next step is to the southwest, the instruction builder will add a left turn command before adding the diagonal-left command.

If a movement command breaks those rules, the page shows a validation error and the step can be reversed.

Once instructions have been generated, the route can be sent directly to `RobotScenarioRunner` and animated there.

