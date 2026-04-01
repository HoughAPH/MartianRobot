# MartianRobot - UI branch

This branch extends the original `MartianRobot` assessment by adding web UI projects on top of the existing robot logic.

## Purpose of this branch

The goal of this branch is to expose the Martian Robot logic through a web-based user interface rather than a console application.

In this branch:

- the original console app code has been repurposed into a reusable class library
- UI projects have been added to provide a browser-based experience
- the robot movement and instruction logic remain in the shared `MartianRobot` project

## Important

If you want to review the original assessment submission as a standalone Martian Robot solution, look at the `master` branch.

The `master` branch shows the original assessment version more directly.

## Branch structure

This branch is organised as follows:

- `MartianRobot/` - shared domain and execution logic, now used as a library
- `RobotGrid/` - ASP.NET Core host project
- `RobotGrid.Client/` - Blazor WebAssembly UI project

## Key change from the original version

### `MartianRobot` is now a library

The original console application project has been converted into a reusable library so that the robot logic can be consumed by the UI projects.

That means:

- `MartianRobot` is no longer intended to be started directly
- the web host project should be used as the startup project
- the robot rules and command-processing behavior are shared by the UI

## Why this change was made

This branch separates responsibilities more clearly:

- UI concerns live in the web projects
- robot behavior remains in the shared library
- the same core logic can be reused without depending on a console entry point

## Which branch to inspect

- Look at **this branch** for the UI-based version
- Look at **`master`** for the original Martian Robot assessment solution

## Notes

This branch should be understood as an extension of the original exercise, not as the original assessment submission itself.