# MartianRobot - UI branch

This branch is my follow-on version of the original `MartianRobot` assessment.

The original assessment solution is still the main reference point, but in this branch I took the core robot logic and built a web UI around it so the behaviour can be exercised through a browser instead of only through a console entry point.

## What this branch is

This branch is not the original assessment submission.

It is an extension of that work.

The purpose of this branch is to keep the original Martian Robot logic, but expose it through UI projects so the scenarios can be run visually.

## Where to look for the original assessment

If you want to review the original Martian Robot assessment as it was first implemented, look at the `master` branch.

That is the branch to use if you want to see the original assessment version directly.

## What changed in this branch

The main change is that the original `MartianRobot` project is no longer being treated as the app you start directly.

Instead, it now acts as the core logic project that the UI uses.

In other words:

- `MartianRobot` contains the robot rules and instruction execution logic
- `RobotGrid.Client` provides the Blazor WebAssembly UI
- `RobotGrid` is the ASP.NET Core host project for the UI

## Project structure

- `MartianRobot/` - reusable robot logic and domain model
- `RobotGrid.Client/` - Blazor WebAssembly front end
- `RobotGrid/` - ASP.NET Core host project

## Why I changed it this way

I wanted to keep the robot logic separate from the UI.

That makes the solution easier to reason about because:

- the Martian Robot behaviour stays in one place
- the UI can use that logic without duplicating it
- the original rules are still preserved
- the application is easier to extend beyond a console-only version

## Important note

`MartianRobot` is now a library project in this branch, not the startup project.

If you want to run the UI version, start the web host project rather than `MartianRobot` directly.

## Summary

So the simplest way to read this branch is:

- `master` = original assessment solution
- this branch = the same core logic, extended with a web UI

### UI pages in this branch
## Robot Scenario Runner

This branch includes a separate `Robot Scenario Runner` page for running a single robot scenario directly.

On that page you can enter the grid size, the robot start position, the starting heading, and the instruction string, then run the scenario and watch the simulation update step by step.

This is useful on its own for trying manual scenarios, and it also pairs with the `Route Builder` page because the generated instruction string from the builder can be pasted into the runner to see the route play out visually.

## Route Builder

This branch also includes a `Route Builder` page that lets you create a route by clicking cells on a grid.
This page introduces new movement commands for diagonal movement, and it generates the instruction string for the route as you build it.
It has to calculate which turn commands must be added for a diagonal move based on the current heading, so it has some rules about how the route must be built in order to be valid.
The page treats the grid as 0-based. The first click sets the start cell, and each next click adds the next move in the route. As the route is built, the instruction string is generated automatically.

The following rules were made because it is not practical to input the heading for each step in the route builder, so the heading is inferred from the direction of movement between cells:
- the first click sets the robot's starting position
- the second click must be orthogonally adjacent to the first click and establishes the robot's initial heading
- each new step must go to a neighboring cell
- diagonal steps are allowed
- the first move must be orthogonal
- diagonal moves are treated as relative moves from the current heading:
  - diagonal-left = `Q`
  - diagonal-right = `E`

Because diagonal movement is interpreted relative to the robot's current heading rather than as a separate absolute heading, 
the route converter assumes the robot starts facing north, requires the first move to establish orientation with an orthogonal step, 
and only accepts diagonals that are forward-left or forward-right from the current heading.

If a route breaks those rules, the page shows the validation error instead of generating instructions.

Once the instruction string has been generated, you can used as input for the `RobotScenarioRunner` page in `RobotScenarioRunner.razor` and watch the simulation there.

