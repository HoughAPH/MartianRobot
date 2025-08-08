/*
 *In order to move the robot L, R, F; one must know in which direction the move will be given the start direction.
So if you're moving East and you turn Left, you will be facing North.
If you turn Right, you will be facing South.
etc.

So for left turns the robot will move North, West, South, East in that order.
for right turns it will move East, South, West, North in that order.
It's clear one needs and array or list to store the directions and their indexes.
Then right turns will increment the index and left turns will decrement it.
The move methods needs parameters. current location and direction, and the command
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
    public Robot(int x, int y, char direction = 'N')
    {
        X = x;
        Y = y;
        Direction = direction;
    }
    // The Robot class should have methods to move and turn
    //it will process the string of commands like LLRLFFLR
    //Maybe TurnLeft and TurnRight can be one Turn method with a parameter (L, R)
    //Forward movement is current direction + 1 in the grid
    // Pass the Robot object to the Move method with the command char.

    public void Move(string commands)
    {
        foreach (var command in commands)
        {
            switch (command)
            {
                case 'F':
                    MoveForward(this);
                    break;
                case 'L':
                    TurnLeft(this);
                    break;
                case 'R':
                    TurnRight(this);
                    break;
            }
        }
    }
    private (int newX, int newY) MoveForward(Robot robot)
    {
        // Logic to move forward based on current direction
        int currentX = robot.X;
        int currentY = robot.Y;
        string heading = robot.Direction.ToString().ToUpper();
        return heading switch

        {
            "N" => (currentX, currentY + 1),

            "S" => (currentX, currentY - 1),

            "E" => (currentX + 1, currentY),

            "W" => (currentX - 1, currentY),

            _ => throw new ArgumentException($"Invalid direction: {heading}")

        };
    }
    private void TurnLeft(Robot robot)
    {
        string[] Headings = { "N", "E", "S", "W" };
        var currentHeading = robot.Direction.ToString().ToUpper();
        int currentIndex = Array.IndexOf(Headings, currentHeading);
        // Calculate new index for left turn
        //This increments or decrements the index based on the current heading
        //When the index is 0, it wraps around to the end of the array and vice versa
        int newIndex = (currentIndex - 1 + Headings.Length) % Headings.Length;
    }
    private void TurnRight(Robot robot)
    {
        // Logic to turn right
        string[] Headings = { "N", "E", "S", "W" };
        var currentHeading = robot.Direction.ToString().ToUpper();
        int currentIndex = Array.IndexOf(Headings, currentHeading);
        // Calculate new index for right turn
        //This increments or decrements the index based on the current heading
        int newIndex = (currentIndex + 1) % Headings.Length;
        robot.Direction = Headings[newIndex][0]; // Update the direction
    }

    //Validations for valid moves.
    //New position must be within the grid boundaries
    //If not, the position must be stored to a LostList or similar structure
    //You can add a method to check if the new position is valid
    public bool IsValidMove(int newX, int newY, Grid grid)
    {
        bool valid = false;
        // Check if the new position is within the grid boundaries
        // upper right corner is (grid.Width, grid.Height)
        // so x must be between 1 and grid.Width  (Not -1 because of 0 based index)
        // and y must be between 1 and grid.Height (Not -1 because of 0 based index)
        valid = newX >= 0 && newX <= grid.Width && newY >= 0 && newY <= grid.Height;
        if (!valid)
        {
            // If not valid, you can log or store the lost position
            // persist a LostList somewhere
            grid.LostPositions.Add(new Coordinate(newX, newY));

            Console.WriteLine($"Lost position: {newX} {newY}");
        }
        return valid;

    }

    public struct Coordinate
    {
        public int X;
        public int Y;
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }
    }


    //2. Grid class to define the boundaries
    public class Grid
    {
        public int Width { get; set; } // 0 based index
        public int Height { get; set; } // 0 based index
        //a structure to store lost positions
      public List<Coordinate> LostPositions { get; set; } = new List<Coordinate>();


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