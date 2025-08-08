// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
/*
 * Start the project with some basics and pseudo code.
There are a few identifiable objects:
1. Robot with properties like position (x, y), direction
2. Grid on Mars is a rectangular (0, 0) / (X, Y) where notrh is Y+1 and east is X+1 
3. Commands to move the robot like "move forward", "turn left", "turn right", etc.
4. Input instructions will be a string of  F-forward, L-Left, R-Right.  E.g. "FFRFLF"
5. A robot is LOST when it moves out of the grid. (This must be stored to prevent other Robots to fall of the grid there.
6. Other validations: Max grid size is 50x50, and instructions not exceeding 100 characters.
7. The requirement to add other movements in the future hints at implimenting delegates maybe.
But let's keep it simple for now.
--------
sample input:
// 5 3
// 1 1 E
// RFRFRFRF

// 3 2 N
// FRRFLLFFRRFLL
// 0 3 W
// LLFFFLFLFL

 */

//1. Robot class with properties
public class Robot
{
    public int X { get; set; }
    public int Y { get; set; }
    public char Direction { get; set; } // N, E, S, W
    public Robot(int x, int y, char direction)
    {
        X = x;
        Y = y;
        Direction = direction;
    }
    // The Robot class should have methods to move and turn
    //it will process the string of commands like LLRLFFLR
    //Maybe TurnLeft and TurnRoght can be one Turn method with a parameter (L, R)
    //Forward movement is current direction + 1 in the grid
    public void Move(string commands)
    {
        foreach (var command in commands)
        {
            switch (command)
            {
                case 'F':
                    MoveForward();
                    break;
                case 'L':
                    TurnLeft();
                    break;
                case 'R':
                    TurnRight();
                    break;
            }
        }
    }
    private void MoveForward()
    {
        // Logic to move forward based on current direction
    }
    private void TurnLeft()
    {
        // Logic to turn left
    }
    private void TurnRight()
    {
        // Logic to turn right
    }

    //2. Grid class to define the boundaries
    public class Grid
    {
        public int Width { get; set; } // 0 based index
        public int Height { get; set; } // 0 based index
        //a structure to store lost positions
        

        public Grid(int width, int height)
        {
            Width = width;
            Height = height;
        }
        //validations for valid moves.
        //New position must be within the grid boundaries
        //If not, the position must be stored to a LostList or similar structure
    }

    //There should be a main method to run the program that accepts inputs 
    //Or
    //hardcode some movements to test the logic against expected outputs.

    public static void Main(string[] args)
    {
        // Example inputs
        Grid marsGrid = new Grid(5, 3);
        //new robot with inistial position and direction
        Robot robot1 = new Robot(1, 1, 'E');
        // Move the robot with a string of commands
        robot1.Move("RFRFRFRF");
        
        Robot robot2 = new Robot(3, 2, 'N');
        robot2.Move("FRRFLLFFRRFLL");
        
        Robot robot3 = new Robot(0, 3, 'W');
        robot3.Move("LLFFFLFLFL");
        //Process the robots' movements and validate them.
        // Output the final positions of the robots
        Console.WriteLine($"Robot 1 Position: {robot1.X} {robot1.Y} {robot1.Direction}");
        Console.WriteLine($"Robot 2 Position: {robot2.X} {robot2.Y} {robot2.Direction}");
        Console.WriteLine($"Robot 3 Position: {robot3.X} {robot3.Y} {robot3.Direction}");
    }
}