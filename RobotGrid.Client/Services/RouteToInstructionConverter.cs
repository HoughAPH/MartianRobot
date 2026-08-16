using MartianRobot.Commands;
using MartianRobot.Models;

namespace RobotGrid.Client.Services;

public static class RouteToInstructionConverter
{
    public static RouteConversionResult Convert(IReadOnlyList<(int X, int Y)> routeCells)
    {
        ArgumentNullException.ThrowIfNull(routeCells);

        if (routeCells.Count < 2)
        {
            throw new ArgumentException("At least two route cells are required.");
        }

        (int X, int Y) = routeCells[0];
        Heading? startHeading = null;
        Heading? currentHeading = null;
        List<char> instructions = [];

        for (int i = 1; i < routeCells.Count; i++)
        {
            (int Dx, int Dy) = GetStepDelta(routeCells[i - 1], routeCells[i]);
            bool isOrthogonal = IsOrthogonalStep(Dx, Dy);
            bool isDiagonal = !isOrthogonal && IsDiagonalStep(Dx, Dy);

            if (i == 1 && !isOrthogonal)
            {
                throw new ArgumentException("The first move must be orthogonal.");
            }

            if (isOrthogonal)
            {
                Heading targetHeading = GetHeadingFromOrthogonalStep(Dx, Dy);

                if (i == 1)
                {
                    startHeading = targetHeading;
                    currentHeading = targetHeading;
                    instructions.Add('F');
                    continue;
                }

                AppendTurnCommands(instructions, currentHeading!.Value, targetHeading);
                instructions.Add('F');
                currentHeading = targetHeading;
                continue;
            }

            if (isDiagonal)
            {
                if (currentHeading is null)
                {
                    throw new ArgumentException("The first move must be orthogonal.");
                }

                //if (MatchesDiagonalLeft(currentHeading.Value, step.Dx, step.Dy))
                if (MoveDiagonalLeftCommand.MatchCommand(currentHeading.Value, Dx, Dy))
                {
                    instructions.Add('Q');
                    continue;
                }

                //  if (MatchesDiagonalRight(currentHeading.Value, step.Dx, step.Dy))
                if (MoveDiagonalRightCommand.MatchCommand(currentHeading.Value, Dx, Dy))
                {
                    instructions.Add('P');
                    continue;
                }

                if (MatchesDiagonalLeftRear(currentHeading.Value, Dx, Dy))
                {
                    instructions.Add('L');
                    instructions.Add('Q');
                    currentHeading = currentHeading switch
                    {
                        Heading.North => Heading.West,
                        Heading.East => Heading.North,
                        Heading.South => Heading.East,
                        Heading.West => Heading.South,
                        _ => throw new NotImplementedException(),
                    };
                    continue;
                }

                if (MatchesDiagonalRightRear(currentHeading.Value, Dx, Dy))
                {
                    instructions.Add('R');
                    instructions.Add('P');

                    currentHeading = currentHeading switch
                    {
                        Heading.North => Heading.East,
                        Heading.East => Heading.South,
                        Heading.South => Heading.West,
                        Heading.West => Heading.North,
                        _ => throw new NotImplementedException(),
                    };
                    continue;
                }
            }

            throw new ArgumentException("Each move must go to a neighboring cell.");
        }
        if (startHeading is null || currentHeading is null)
        {
            throw new ArgumentException("The first move must be orthogonal.");
        }

        return new RouteConversionResult(
            StartX: X,
            StartY: Y,
            //StartHeading: startHeading,
            StartHeading: startHeading.Value,
            CurrentHeading: currentHeading.Value,
            Instructions: new string([.. instructions]));
    }


    //This method add implied turn commands to the instructions list based on the current and target headings.
    private static void AppendTurnCommands(List<char> instructions, Heading current, Heading target)
    {
        int currentIndex = ToIndex(current);
        int targetIndex = ToIndex(target);
        int delta = (targetIndex - currentIndex + 4) % 4;  //Cycles clockwise or anti clockwise through N, E, S, W

        switch (delta)
        {
            case 0:
                return;
            case 1:
                instructions.Add('R');
                return;
            case 2:
                instructions.Add('R');
                instructions.Add('R');
                return;
            case 3:
                instructions.Add('L');
                return;
            default:
                throw new InvalidOperationException("Invalid heading delta.");
        }
    }

    private static int ToIndex(Heading heading) => heading switch
    {
        Heading.North => 0,
        Heading.East => 1,
        Heading.South => 2,
        Heading.West => 3,
        _ => throw new ArgumentOutOfRangeException(nameof(heading))
    };

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

    private static bool IsDiagonalStep(int dx, int dy)
    {
        return Math.Abs(dx) == 1 && Math.Abs(dy) == 1;
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

    private static bool MatchesDiagonalLeftRear(Heading heading, int dx, int dy)
    {
        return heading switch
        {
            Heading.North => (dx, dy) == (-1, -1),
            Heading.East => (dx, dy) == (-1, 1),
            Heading.South => (dx, dy) == (1, 1),
            Heading.West => (dx, dy) == (1, -1),
            _ => false
        };
    }


    private static bool MatchesDiagonalRightRear(Heading heading, int dx, int dy)
    {
        return heading switch
        {
            Heading.North => (dx, dy) == (1, -1),
            Heading.East => (dx, dy) == (-1, -1),
            Heading.South => (dx, dy) == (-1, 1),
            Heading.West => (dx, dy) == (1, 1),
            _ => false
        };
    }
}

public sealed record RouteConversionResult(
    int StartX,
    int StartY,
    Heading StartHeading,
    Heading CurrentHeading,
    string Instructions);