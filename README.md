# Maze Generator and Solver

This project is meant to present an efficient method for not only generating a maze, but also solving one.

For the generation of said maze, I selected a starting cell in a 2D array. From there, I used a random walk method, backtracking to the last branch-able cell whenever I hit a dead end (cell already surrounded on all sides).

![](https://github.com/CristianDejica00/Maze-Generator-and-Solver/blob/main/GitPres/Maze_Gen.gif)

To solve the previously generated maze, I used a Weighted A* pathfinding algorithm. This method assigns 3 values to each cell: G, H, and F.

G is equal to the approximate distance from the current cell to the starting cell. I chose to represent the distance between horizontal or vertical neighbors with 1 and the distance between diagonal neighbors with √2.

H is called the "Heuristic" value, and represents the approximate distance between the current cell and the end cell. I chose to calculate this using the Manhattan Distance:

```
H = |x1 - x2| + |y1 - y2|
```

Finally, F is the final value of the cell, which represents the sum of G and H. I mentioned earlier that this is a "weighted" A* algorithm. This means that I multiplied the H value within the sum in order to prioritize following the direction of the end cell, instead of following more random paths.

The algorithm also uses 2 lists (open list and closed list) that refer to nodes that we can visit and nodes that we've already visited.

Here's an explanation of the pathfinding process:

1. Initialize the G, H, and F values of the starting cell and add it to the open list.
2. Pick the cell with the lowest F value from the open list.
3. We check all neighboring cells of the previously selected cell. If they are walls or already in the closed list (already visited), we skip them. Otherwise, we calculate their G, H, and F values, and we add them to the open list. If they are already in the open list, we recalculate their values.
4. We move the selected cell in the closed list.
5. We repeat steps 2-4 until we find the end cell or we run out of cells in the open list (meaning we have no path).

Afterwards we "backtrack" towards the starting cell to determine the best path to follow.

![](https://github.com/CristianDejica00/Maze-Generator-and-Solver/blob/main/GitPres/Pres_2.gif)
