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