﻿using Microsoft.Xna.Framework;
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
        private static bool isAlive;
        private static int loop;
        private static int backs;

        public static int Loop { get => loop; set => loop = value; }
        public static int Backs { get => backs; set => backs = value; }
        
        /// <summary>
        /// Start maze from random point
        /// </summary>
        public static void StartMazeBuild()
        {
            if (mazeThread == null)
            {
                mazeThread = new Thread(BuildMaze);
                mazeThread.IsBackground = true;
                mazeThread.Start();
                isAlive = true;
            } else if(!isAlive)
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
            if (mazeThread == null)
            {
                mazeThread = new Thread(() => BuildMaze(cell));
                mazeThread.IsBackground = true;
                mazeThread.Start();
                isAlive = true;
            }
            else if (!isAlive)
            {
                mazeThread = new Thread(() => BuildMaze(cell));
                mazeThread.IsBackground = true;
                mazeThread.Start();
                isAlive = true;
            }
        }

        public static void SolveMaze(Cell goalCell)
        {
            ColorNodes();
            Cell currentCell = goalCell;
            currentCell.CellColor = Color.Green;
            int steps = 0;
            while (currentCell.MyNode.Parent != null)
            {
                steps++;
                GameWorld.Cells.TryGetValue(currentCell.MyNode.Parent.Data, out currentCell); //Change currentCell to parent
                currentCell.CellColor = new Color(steps/2+50, steps/50, steps);
            }
            currentCell.CellColor = Color.Red;
        }

        /// <summary>
        /// Backtracking maze builder
        /// </summary>
        public static void BuildMaze()
        {
            //Resets maze, which fixes rerun by rebuilding the wall
            ColorNodes();
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
            loop = 0;
            backs = 0;
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
            ColorNodes();
            List<Node<Vector2>> allNodes = CreateNodes();

            foreach (Cell cell in GameWorld.Cells.Values)
            {
                cell.SetRectangles();
            }

            Cell currentCell = startingCell;

            currentCell.MyNode.Discovered = true;

            currentCell.CellColor = Color.Red;
            int visited = 0;
            int totalUnvisited = 0;
            loop = 0;
            backs = 0;
            foreach (Cell cell in GameWorld.Cells.Values)
            {
                if (!cell.MyNode.Discovered)
                {
                    totalUnvisited++;
                }
            }
            while (totalUnvisited > visited)
            {
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
                    visited++;
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
        /// Colors 
        /// </summary>
        public static void ColorNodes()
        {
            foreach (Cell c in GameWorld.Cells.Values)
            {
                c.CellColor = Color.White;
            }
        }

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