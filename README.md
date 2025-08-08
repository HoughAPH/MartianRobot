# MartianRobot
1. Robot with properties like position (x, y), direction
2. Grid on Mars is a rectangular (0, 0) / (X, Y) where notrh is Y+1 and east is X+1 
3. Commands to move the robot like "move forward", "turn left", "turn right", etc.
4. Input instructions will be a string of  F-forward, L-Left, R-Right.  E.g. "FFRFLF"
5. A robot is LOST when it moves out of the grid. (This must be stored to prevent other Robots to fall of the grid there.
6. Other validations: Max grid size is 50x50, and instructions not exceeding 100 characters.
7. The requirement to add other movements in the future hints at implimenting delegates maybe.
8. Keep it simple


sample input:
 5 3
 1 1 E
 RFRFRFRF

 3 2 N
 FRRFLLFFRRFLL

 0 3 W
 LLFFFLFLFL.

 Sample Output:
 1 1 E
 3 3 N LOST
 2 3 S