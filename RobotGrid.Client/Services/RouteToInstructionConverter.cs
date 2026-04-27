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

        var (X, Y) = routeCells[0];
        var currentHeading = Heading.North;
        List<char> instructions = [];

        for (var i = 1; i < routeCells.Count; i++)
        {
            var step = GetStepDelta(routeCells[i - 1], routeCells[i]);
            var isOrthogonal = IsOrthogonalStep(step.Dx, step.Dy);
            var isDiagonal = !isOrthogonal && IsDiagonalStep(step.Dx, step.Dy);

            if (i == 1 && !isOrthogonal)
            {
                throw new ArgumentException("The first move must be orthogonal.");
            }

            if (isOrthogonal)
            {
                var targetHeading = GetHeadingFromOrthogonalStep(step.Dx, step.Dy);
                AppendTurnCommands(instructions, currentHeading, targetHeading);
                instructions.Add('F');
                currentHeading = targetHeading;
                continue;
            }

            if (isDiagonal)
            {
                if (MatchesDiagonalLeft(currentHeading, step.Dx, step.Dy))
                {
                    instructions.Add('Q');
                    continue;
                }

                if (MatchesDiagonalRight(currentHeading, step.Dx, step.Dy))
                {
                    instructions.Add('E');
                    continue;
                }

                throw new ArgumentException("Diagonal moves must be forward-left or forward-right relative to the current heading.");
            }

            throw new ArgumentException("Each move must go to a neighboring cell.");
        }

        return new RouteConversionResult(
            X,
            Y,
            currentHeading,
            new string(instructions.ToArray()));
    }

    public static RouteConversionResult Convert(string[][] cells)
    {
        ArgumentNullException.ThrowIfNull(cells);

        if (cells.Length == 0 || cells[0].Length == 0)
        {
            throw new ArgumentException("The grid must contain at least one cell.");
        }

        var height = cells.Length;
        var width = cells[0].Length;

        for (var row = 1; row < height; row++)
        {
            if (cells[row].Length != width)
            {
                throw new ArgumentException("All grid rows must have the same width.");
            }
        }

        Dictionary<(int Col, int Row), Heading> arrows = [];

        for (var row = 0; row < height; row++)
        {
            for (var col = 0; col < width; col++)
            {
                var value = NormalizeCell(cells[row][col]);

                if (value is null)
                {
                    continue;
                }

                arrows[(col, row)] = ParseHeading(value.Value);
            }
        }

        if (arrows.Count == 0)
        {
            throw new ArgumentException("Enter at least one arrow in the grid.");
        }

        var incomingCounts = arrows.Keys.ToDictionary(key => key, _ => 0);

        foreach (var arrow in arrows)
        {
            var next = Step(arrow.Key, arrow.Value);

            if (incomingCounts.TryGetValue(next, out var incomingCount))
            {
                incomingCounts[next] = incomingCount + 1;
            }
        }

        (int Col, int Row)? start = null;

        foreach (var entry in incomingCounts)
        {
            if (entry.Value != 0)
            {
                continue;
            }

            if (start is not null)
            {
                throw new ArgumentException("The route must have exactly one start cell.");
            }

            start = entry.Key;
        }

        if (start is null)
        {
            throw new ArgumentException("The route must have exactly one start cell.");
        }

        HashSet<(int Col, int Row)> visited = [];
        List<Heading> path = [];

        var current = start.Value;

        while (arrows.TryGetValue(current, out var heading))
        {
            if (!visited.Add(current))
            {
                throw new ArgumentException("The route contains a loop.");
            }

            path.Add(heading);
            current = Step(current, heading);
        }

        if (visited.Count != arrows.Count)
        {
            throw new ArgumentException("The route must be a single continuous path.");
        }

        var instructions = BuildInstructions(path);

        return new RouteConversionResult(
            StartX: start.Value.Col,
            StartY: height - 1 - start.Value.Row,
            CurrentHeading: path[0],
            Instructions: instructions);
    }

    private static string BuildInstructions(IReadOnlyList<Heading> path)
    {
        if (path.Count == 0)
        {
            throw new ArgumentException("The route must contain at least one movement.");
        }

        List<char> instructions = ['F'];
        var currentHeading = path[0];

        for (var i = 1; i < path.Count; i++)
        {
            var targetHeading = path[i];
            AppendTurnCommands(instructions, currentHeading, targetHeading);
            instructions.Add('F');
            currentHeading = targetHeading;
        }

        return new string(instructions.ToArray());
    }

    private static char? NormalizeCell(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim()[0];
    }

    private static Heading ParseHeading(char value)
    {
        return value switch
        {
            '^' => Heading.North,
            '>' => Heading.East,
            'v' or 'V' => Heading.South,
            '<' => Heading.West,
            _ => throw new ArgumentException($"Unsupported route marker '{value}'. Use only ^, >, v, <.")
        };
    }

    private static (int Col, int Row) Step((int Col, int Row) current, Heading heading)
    {
        return heading switch
        {
            Heading.North => (current.Col, current.Row - 1),
            Heading.East => (current.Col + 1, current.Row),
            Heading.South => (current.Col, current.Row + 1),
            Heading.West => (current.Col - 1, current.Row),
            _ => throw new ArgumentOutOfRangeException(nameof(heading))
        };
    }

    //this calculates the minimal turn commands (L/R) needed to change from current heading to target heading
    private static void AppendTurnCommands(List<char> instructions, Heading current, Heading target)
    {
        var currentIndex = ToIndex(current);
        var targetIndex = ToIndex(target);
        var delta = (targetIndex - currentIndex + 4) % 4;

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
        var dx = to.X - from.X;
        var dy = to.Y - from.Y;

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

    private static bool MatchesDiagonalLeft(Heading heading, int dx, int dy)
    {
        return heading switch
        {
            Heading.North => (dx, dy) == (-1, 1),
            Heading.East => (dx, dy) == (1, 1),
            Heading.South => (dx, dy) == (1, -1),
            Heading.West => (dx, dy) == (-1, -1),
            _ => false
        };
    }

    private static bool MatchesDiagonalRight(Heading heading, int dx, int dy)
    {
        return heading switch
        {
            Heading.North => (dx, dy) == (1, 1),
            Heading.East => (dx, dy) == (1, -1),
            Heading.South => (dx, dy) == (-1, -1),
            Heading.West => (dx, dy) == (-1, 1),
            _ => false
        };
    }
}

public sealed record RouteConversionResult(
    int StartX,
    int StartY,
    Heading CurrentHeading,
    string Instructions);