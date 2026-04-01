using System.ComponentModel.DataAnnotations;

namespace MartianRobot.Models;

public sealed class RobotScenarioInput
{
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
    [RegularExpression("^[FLRflr]*$", ErrorMessage = "Use only F, L, and R.")]
    public string Instructions { get; set; } = "";
}