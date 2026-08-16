using MartianRobot.Models;

namespace RobotGrid.Components.Pages;

public sealed record RouteConversionResult(
    int StartX,
    int StartY,
    Heading StartHeading,
    Heading CurrentHeading,
    string Instructions);