using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace MazeGrid
{
    public class Cell
    {
        private Color cellColor = Color.White;
        private Color edgeColor = Color.Black;
        private Vector2 position;
        private int size;
        private float layerDepth = 1f;

        private Node<Vector2> myNode;

        private Rectangle topLine;

        private Rectangle bottomLine;

        private Rectangle rightLine;

        private Rectangle leftLine;

        private Rectangle background;

        private Texture2D sprite;

        public Vector2 Position { get => position; set => position = value; }
        public Node<Vector2> MyNode { get => myNode; set => myNode = value; }
        public Color CellColor { get => cellColor; set => cellColor = value; }
        public Rectangle TopLine { get => topLine; set => topLine = value; }
        public Rectangle BottomLine { get => bottomLine; set => bottomLine = value; }
        public Rectangle RightLine { get => rightLine; set => rightLine = value; }
        public Rectangle LeftLine { get => leftLine; set => leftLine = value; }

        public Cell(int x, int y, int size)
        {
            this.Position = new Vector2(x, y);
            this.size = size;
        }

        public void OnResize()
        {
            //TODO change background & lines to new Rectangle() with proper zoomScale
        }

        public void LoadContent(ContentManager content)
        {
            sprite = content.Load<Texture2D>("pixel");
            SetRectangles();
        }

        public void SetRectangles()
        {
            TopLine = new Rectangle((int)Position.X * size, (int)Position.Y * size, size, 1);

            BottomLine = new Rectangle((int)Position.X * size, ((int)Position.Y * size) + size, size, 1);

            RightLine = new Rectangle(((int)Position.X * size) + size, (int)Position.Y * size, 1, size);

            LeftLine = new Rectangle((int)Position.X * size, (int)Position.Y * size, 1, size);

            background = new Rectangle((int)Position.X * size, (int)Position.Y * size, size, size);
        }

        public void Update()
        {
            //Left click to build maze
            if (background.Contains(Mouse.GetState().Position) && Mouse.GetState().LeftButton == ButtonState.Pressed &&
                GameWorld.OldMouseState.LeftButton == ButtonState.Released)
            {
                MazeManager.StartMazeBuild(this);
            }

            //Right click to pathfind to start of maze
            if (background.Contains(Mouse.GetState().Position) && Mouse.GetState().RightButton == ButtonState.Pressed &&
                GameWorld.OldMouseState.RightButton == ButtonState.Released)
            {
                MazeManager.StartSolveThread(this);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, background, null, CellColor, default, default, SpriteEffects.None, layerDepth);
            if (topLine != Rectangle.Empty)
            {
                spriteBatch.Draw(sprite, TopLine, null, edgeColor, default, default, SpriteEffects.None, layerDepth);

            }
            if (bottomLine != Rectangle.Empty)
            {
                spriteBatch.Draw(sprite, bottomLine, null, edgeColor, default, default, SpriteEffects.None, layerDepth);

            }
            if (leftLine != Rectangle.Empty)
            {
                spriteBatch.Draw(sprite, leftLine, null, edgeColor, default, default, SpriteEffects.None, layerDepth);

            }
            if (rightLine != Rectangle.Empty)
            {
                spriteBatch.Draw(sprite, rightLine, null, edgeColor, default, default, SpriteEffects.None, layerDepth);

            }
        }
    }
}
