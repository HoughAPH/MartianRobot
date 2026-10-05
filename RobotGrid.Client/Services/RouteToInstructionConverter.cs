using System.Text;
using MartianRobot.Commands;
using MartianRobot.Models;
using RobotGrid.Client.Models;

namespace RobotGrid.Client.Services;

public static class RouteToInstructionConverter
{
    private const int HeadingCount = 4;

    public static RouteConversionResult Convert(IReadOnlyList<(int X, int Y)> routeCells)
    {
        ArgumentNullException.ThrowIfNull(routeCells);

        if (routeCells.Count < 2)
        {
            throw new ArgumentException("At least two route cells are required.");
        }

        (int startX, int startY) = routeCells[0];

        // The first move defines the start heading, so it must be orthogonal.
        (int firstDx, int firstDy) = GetStepDelta(routeCells[0], routeCells[1]);
        if (!IsOrthogonalStep(firstDx, firstDy))
        {
            throw new ArgumentException("The first move must be orthogonal.");
        }

        Heading startHeading = GetHeadingFromOrthogonalStep(firstDx, firstDy);
        Heading currentHeading = startHeading;

        // Each step produces at most three instructions ("RRF").
        StringBuilder instructions = new((routeCells.Count - 1) * 3);
        instructions.Append('F');

        for (int i = 2; i < routeCells.Count; i++)
        {
            (int dx, int dy) = GetStepDelta(routeCells[i - 1], routeCells[i]);

            if (IsOrthogonalStep(dx, dy))
            {
                Heading targetHeading = GetHeadingFromOrthogonalStep(dx, dy);
                AppendTurnCommands(instructions, currentHeading, targetHeading);
                instructions.Append('F');
                currentHeading = targetHeading;
                continue;
            }

            // GetStepDelta guarantees a neighbouring cell, so a non-orthogonal step is diagonal.
            if (MoveDiagonalLeftCommand.MatchCommand(currentHeading, dx, dy))
            {
                instructions.Append('Q');
                continue;
            }

            if (MoveDiagonalRightCommand.MatchCommand(currentHeading, dx, dy))
            {
                instructions.Append('P');
                continue;
            }

            // Rear diagonals: turn first so the move becomes a forward diagonal.
            Heading leftHeading = Rotate(currentHeading, -1);
            if (MoveDiagonalLeftCommand.MatchCommand(leftHeading, dx, dy))
            {
                instructions.Append("LQ");
                currentHeading = leftHeading;
                continue;
            }

            Heading rightHeading = Rotate(currentHeading, 1);
            if (MoveDiagonalRightCommand.MatchCommand(rightHeading, dx, dy))
            {
                instructions.Append("RP");
                currentHeading = rightHeading;
                continue;
            }

            throw new ArgumentException("Each move must go to a neighboring cell.");
        }

        return new RouteConversionResult(
            StartX: startX,
            StartY: startY,
            StartHeading: startHeading,
            CurrentHeading: currentHeading,
            Instructions: instructions.ToString());
    }

    //This method adds implied turn commands to the instructions based on the current and target headings.
    private static void AppendTurnCommands(StringBuilder instructions, Heading current, Heading target)
    {
        int delta = ((int)target - (int)current + HeadingCount) % HeadingCount;  //Cycles clockwise through N, E, S, W

        switch (delta)
        {
            case 0:
                return;
            case 1:
                instructions.Append('R');
                return;
            case 2:
                instructions.Append("RR");
                return;
            case 3:
                instructions.Append('L');
                return;
            default:
                throw new InvalidOperationException("Invalid heading delta.");
        }
    }

    private static Heading Rotate(Heading heading, int quarterTurns)
    {
        return (Heading)(((int)heading + quarterTurns + HeadingCount) % HeadingCount);
    }

    private static (int Dx, int Dy) GetStepDelta((int X, int Y) from, (int X, int Y) to)
    {
        int dx = to.X - from.X;
        int dy = to.Y - from.Y;

        if (Math.Abs(dx) > 1 || Math.Abs(dy) > 1 || (dx == 0 && dy == 0))
        {
            throw new ArgumentException("Each move must go to a neighboring cell.");
        }

        return (dx, dy);
    }

    private static bool IsOrthogonalStep(int dx, int dy)
    {
        return Math.Abs(dx) + Math.Abs(dy) == 1;
    }

    private static Heading GetHeadingFromOrthogonalStep(int dx, int dy)
    {
        return (dx, dy) switch
        {
            (0, 1) => Heading.North,
            (1, 0) => Heading.East,
            (0, -1) => Heading.South,
            (-1, 0) => Heading.West,
            _ => throw new ArgumentException("Only orthogonal steps can define heading.")
        };
    }
}
