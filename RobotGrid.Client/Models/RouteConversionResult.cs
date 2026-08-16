using MartianRobot.Models;

namespace RobotGrid.Client.Models;

public sealed record RouteConversionResult(
    int StartX,
    int StartY,
    Heading StartHeading,
    Heading CurrentHeading,
    string Instructions);