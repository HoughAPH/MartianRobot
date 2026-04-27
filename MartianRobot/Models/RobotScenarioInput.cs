using System.ComponentModel.DataAnnotations;

namespace MartianRobot.Models;

public sealed class RobotScenarioInput : IValidatableObject
{
    private string _instructions = "";

    [Range(0, 50)]
    public int GridWidth { get; set; } = 5;

    [Range(0, 50)]
    public int GridHeight { get; set; } = 3;

    [Range(0, 50)]
    public int StartX { get; set; } = 0;

    [Range(0, 50)]
    public int StartY { get; set; } = 0;

    public Heading Heading { get; set; } = Heading.North;

    [Required]
    [StringLength(100)]
    [RegularExpression("^[FLRQE]*$", ErrorMessage = "Use only F, L, R, Q, and E.")]
    public string Instructions
    {
        get => _instructions;
        set => _instructions = NormalizeInstructions(value);
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartX > GridWidth)
        {
            yield return new ValidationResult(
                $"Start X must be within the grid width (0 to {GridWidth}).",
                [nameof(StartX), nameof(GridWidth)]);
        }

        if (StartY > GridHeight)
        {
            yield return new ValidationResult(
                $"Start Y must be within the grid height (0 to {GridHeight}).",
                [nameof(StartY), nameof(GridHeight)]);
        }
    }

    private static string NormalizeInstructions(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "";
        }

        return new string(
            value
                .Where(c => !char.IsWhiteSpace(c))
                .Select(char.ToUpperInvariant)
                .ToArray());
    }
}