using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace MazeGrid
{
    static class MazeManager
    {
        private static Random random = new Random();
        private static Thread mazeThread;
        private static Thread solveThread;
        private static bool isAlive = false;
        private static bool isSolving = false;
        
        /// <summary>
        /// Start maze from random point
        /// </summary>
        public static void StartMazeBuild()
        {
            if(!isAlive)
            {
                mazeThread = new Thread(BuildMaze);
                mazeThread.IsBackground = true;
                mazeThread.Start();
                isAlive = true;
            }
        }

        /// <summary>
        /// Start maze from specific cell
        /// </summary>
        /// <param name="cell"></param>
        public static void StartMazeBuild(Cell cell)
        {
            if (!isAlive)
            {
                mazeThread = new Thread(() => BuildMaze(cell));
                mazeThread.IsBackground = true;
                mazeThread.Start();
                isAlive = true;
            }
        }

        /// <summary>
        /// Start the thread to solve the maze
        /// </summary>
        /// <param name="cell"></param>
        public static void StartSolveThread(Cell cell)
        {
            if (!isSolving)
            {
                solveThread = new Thread(() => SolveMaze(cell));
                solveThread.IsBackground = true;
                solveThread.Start();
                isSolving = true;
            }
        }
        /// <summary>
        /// Backrack until no more parent to find path between selected cell and maze start cell.
        /// </summary>
        /// <param name="goalCell">The cell where the backtracking starts from</param>
        public static void SolveMaze(Cell goalCell)
        {
            ColorCells();
            Cell currentCell = goalCell;
            currentCell.CellColor = Color.Green;
            int steps = 0;
            while (currentCell.MyNode.Parent != null)
            {
                //for (int i = 0; i < 500000; i++)
                //{
                //    int slow = 2 * i;
                //}
                steps++;
                GameWorld.Cells.TryGetValue(currentCell.MyNode.Parent.Data, out currentCell); //Change currentCell to parent
                currentCell.CellColor = new Color(steps/2+50, steps/50, 255-steps/200);
            }
            currentCell.CellColor = Color.Red;
            isSolving = false;
        }

        /// <summary>
        /// Backtracking maze builder
        /// </summary>
        public static void BuildMaze()
        {
            //Resets maze, which fixes rerun by rebuilding the wall
            ColorCells();
            foreach (Cell cell in GameWorld.Cells.Values)
            {
                cell.SetRectangles();
            }

            List<Node<Vector2>> allNodes = CreateNodes();
            Node<Vector2> currentNode = allNodes[random.Next(0, allNodes.Count)];
            Cell currentCell;
            GameWorld.Cells.TryGetValue(currentNode.Data, out currentCell);

            currentCell.MyNode.Discovered = true;
            currentCell.CellColor = Color.Red;
            
            int visited = 0;
            int totalUnvisited = 0;

            foreach (Cell cell in GameWorld.Cells.Values)
            {
                if (!cell.MyNode.Discovered)
                {
                    totalUnvisited++;
                }
            }
            while (totalUnvisited > visited)
            {
                Dictionary<string,Cell> neighbors = Cardinal(currentCell.Position);
                if (neighbors.Count > 0)
                {
                    foreach (Cell cell in neighbors.Values)
                    {
                        cell.CellColor = Color.Orange;
                    }
                    Cell neighbor = neighbors.ElementAt(random.Next(0, neighbors.Count)).Value; // Returns random neighbor from list
                    neighbor.MyNode.Parent = currentCell.MyNode; //Updates neighbor parent

                    //The Next 4 if statements remove the walls. TODO: make this into a single function call?
                    if (neighbor.Position.X > currentCell.Position.X)
                    {
                        currentCell.RightLine = Rectangle.Empty;
                        neighbor.LeftLine = Rectangle.Empty;
                    }
                    if (neighbor.Position.X < currentCell.Position.X)
                    {
                        currentCell.LeftLine = Rectangle.Empty;
                        neighbor.RightLine = Rectangle.Empty;
                    }
                    if (neighbor.Position.Y > currentCell.Position.Y)
                    {
                        currentCell.BottomLine = Rectangle.Empty;
                        neighbor.TopLine = Rectangle.Empty;
                    }
                    if (neighbor.Position.Y < currentCell.Position.Y)
                    {
                        currentCell.TopLine = Rectangle.Empty;
                        neighbor.BottomLine = Rectangle.Empty;
                    }

                    currentCell = neighbor; //Sets currentCell to neighbor
                    currentCell.MyNode.Discovered = true; //Sets the new cell to be discovered
                    visited++;
                    currentCell.CellColor = Color.White;

                } else
                {
                    //Backtrack to parent node
                    GameWorld.Cells.TryGetValue(currentCell.MyNode.Parent.Data, out currentCell); //Change currentCell to parent
                }
            }
            GameWorld.GameStateProp = GameState.Play;
            isAlive = false;
        }

        public static void BuildMaze(Cell startingCell)
        {
            //Resets maze, which fixes rerun by rebuilding the wall
            ColorCells();
            List<Node<Vector2>> allNodes = CreateNodes();
            //Rebuilds the cell walls
            foreach (Cell cell in GameWorld.Cells.Values)
            {
                cell.SetRectangles();
            }

            Cell currentCell = startingCell;
            currentCell.MyNode.Discovered = true;
            currentCell.CellColor = Color.Red;

            int discovered = 0;
            int totalUndiscovered = 0;
            //Get number of undiscovered cells
            foreach (Cell cell in GameWorld.Cells.Values)
            {
                if (!cell.MyNode.Discovered)
                {
                    totalUndiscovered++;
                }
            }
            while (totalUndiscovered > discovered)
            {
                //for (int i = 0; i < 50000; i++)
                //{
                //    int slow = 2 * i;
                //}
                Dictionary<string, Cell> neighbors = Cardinal(currentCell.Position);
                if (neighbors.Count > 0)
                {
                    foreach (Cell cell in neighbors.Values)
                    {
                        cell.CellColor = Color.Orange;
                    }
                    Cell neighbor = neighbors.ElementAt(random.Next(0, neighbors.Count)).Value; // Returns random neighbor from list
                    neighbor.MyNode.Parent = currentCell.MyNode; //Updates neighbor parent

                    //The Next 4 if statements remove the walls. TODO: make this into a single function call?
                    if (neighbor.Position.X > currentCell.Position.X)
                    {
                        currentCell.RightLine = Rectangle.Empty;
                        neighbor.LeftLine = Rectangle.Empty;
                    }
                    if (neighbor.Position.X < currentCell.Position.X)
                    {
                        currentCell.LeftLine = Rectangle.Empty;
                        neighbor.RightLine = Rectangle.Empty;
                    }
                    if (neighbor.Position.Y > currentCell.Position.Y)
                    {
                        currentCell.BottomLine = Rectangle.Empty;
                        neighbor.TopLine = Rectangle.Empty;
                    }
                    if (neighbor.Position.Y < currentCell.Position.Y)
                    {
                        currentCell.TopLine = Rectangle.Empty;
                        neighbor.BottomLine = Rectangle.Empty;
                    }

                    currentCell = neighbor; //Sets currentCell to neighbor
                    currentCell.MyNode.Discovered = true; //Sets the new cell to be discovered
                    discovered++;
                    currentCell.CellColor = Color.White;

                }
                else
                {
                    //Backtrack to parent node
                    GameWorld.Cells.TryGetValue(currentCell.MyNode.Parent.Data, out currentCell); //Change currentCell to parent
                }
            }
            GameWorld.GameStateProp = GameState.Play;
            isAlive = false;
        }


        /// <summary>
        /// This method returns one of the four cardinal positions
        /// </summary>
        /// <param name="parent">Vector2 of the parent to this neighbors position</param>
        /// <returns></returns>
        public static Dictionary<string, Cell> Cardinal(Vector2 parent)
        {
            Dictionary<string, Cell> n = new Dictionary<string, Cell>();
            Cell currentNeighbor;
            if (GameWorld.cells.TryGetValue(new Vector2(parent.X + 1, parent.Y), out currentNeighbor))
            {
                n.Add("right", currentNeighbor);
            }
            if (GameWorld.cells.TryGetValue(new Vector2(parent.X - 1, parent.Y), out currentNeighbor))
            {
                n.Add("left", currentNeighbor);
            }
            if (GameWorld.cells.TryGetValue(new Vector2(parent.X, parent.Y + 1), out currentNeighbor))
            {
                n.Add("top", currentNeighbor);
            }
            if (GameWorld.cells.TryGetValue(new Vector2(parent.X, parent.Y - 1), out currentNeighbor))
            {
                n.Add("bottom", currentNeighbor);
            }
            //Rmove null neighbors
            foreach (KeyValuePair<string, Cell> kvPair in n)
            {
                if (kvPair.Key.Equals(null))
                {
                    n.Remove(kvPair.Key);
                }
                if (kvPair.Value.MyNode.Discovered)
                {
                    n.Remove(kvPair.Key);
                }
            }
            return n;
        }

        /// <summary>
        /// Makes the cells white
        /// </summary>
        public static void ColorCells()
        {
            foreach (Cell c in GameWorld.Cells.Values)
            {
                c.CellColor = Color.White;
            }
        }

        /// <summary>
        /// Creates a list of nodes and adds the node to its cell
        /// </summary>
        /// <returns></returns>
        public static List<Node<Vector2>> CreateNodes()
        {
            List<Node<Vector2>> allNodes = new List<Node<Vector2>>();

            foreach (Cell cell in GameWorld.Cells.Values)
            {
                    cell.MyNode = new Node<Vector2>(cell.Position);
                    allNodes.Add(cell.MyNode);
            }
            return allNodes;
        }
    }
}
