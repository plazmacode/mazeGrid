using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MazeGrid
{
    enum GameState { Play, Build };
    public class GameWorld : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private static SpriteFont arial;

        public static Dictionary<Vector2, Cell> cells = new Dictionary<Vector2, Cell>();

        private static bool fullscreen = true;
        private static int cellCount = 80;
        private static int cellSize = 5;

        private static Vector2 screenSize;
        private static Vector2 oldScreenSize;
        private static GameState gameState = GameState.Play;

        private static MouseState mouseState;
        private static KeyboardState keyState;
        private static KeyboardState oldKeyState;
        private static MouseState oldMouseState;

        public static Vector2 ScreenSize { get => screenSize; set => screenSize = value; }
        public static Vector2 OldScreenSize { get => oldScreenSize; set => oldScreenSize = value; }
        public static Dictionary<Vector2, Cell> Cells { get => cells; set => cells = value; }
        internal static GameState GameStateProp { get => gameState; set => gameState = value; }
        public static int CellCount { get => cellCount; set => cellCount = value; }
        public static MouseState MouseState { get => mouseState; set => mouseState = value; }
        public static MouseState OldMouseState { get => oldMouseState; set => oldMouseState = value; }
        public static int CellSize { get => cellSize; set => cellSize = value; }

        public GameWorld()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 80;
            ScreenSize = new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            _graphics.SynchronizeWithVerticalRetrace = false; //Unlocks FPS
            this.IsFixedTimeStep = false;
        }

        public void OnResize(Object sender, EventArgs e)
        {
            OldScreenSize = screenSize;
            screenSize = new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        }

        protected override void Initialize()
        {
            arial = Content.Load<SpriteFont>("arial");

            //Maze size based on screenSize / cellSize
            if (fullscreen)
            {
                for (int y = 0; y <screenSize.Y/CellSize; y++)
                {
                    for (int x = 0; x < screenSize.X / CellSize; x++)
                    {
                        cells.Add(new Vector2(x, y), new Cell(x, y, CellSize));
                    }
                }
            }
            //Maze size based on cellCount
            else
            {
                for (int y = 0; y < CellCount; y++)
                {
                    for (int x = 0; x < CellCount; x++)
                    {
                        cells.Add(new Vector2(x, y), new Cell(x, y, CellSize));
                    }
                }
            }
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (Cell c in cells.Values)
            {
                c.LoadContent(Content);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            keyState = Keyboard.GetState();
            MouseState = Mouse.GetState();

            if (keyState.IsKeyUp(Keys.Space) && oldKeyState.IsKeyDown(Keys.Space))
            {
                gameState = GameState.Build;
                MazeManager.StartMazeBuild();
            }

            foreach (Cell cell in cells.Values)
            {
                cell.Update();
            }

            oldKeyState = keyState;
            OldMouseState = mouseState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            //_spriteBatch.Begin(SpriteSortMode.FrontToBack);
            _spriteBatch.Begin();

            foreach (Cell c in cells.Values)
            {
                c.Draw(_spriteBatch);
            }

            //_spriteBatch.DrawString(arial, MazeManager.Loop.ToString(), new Vector2(mouseState.Position.X+10, mouseState.Position.Y-80), Color.Black, default, default, 2f, SpriteEffects.None, default);
            //_spriteBatch.DrawString(arial, MazeManager.Backs.ToString(), new Vector2(mouseState.Position.X+10, mouseState.Position.Y-40), Color.Red, default, default, 2f, SpriteEffects.None, default);

            //if (gameState == GameState.Build)
            //{
            //    _spriteBatch.DrawString(arial, "BUILDING MAZE", Vector2.Zero, Color.Red, default, default, 3f, SpriteEffects.None, default);
            //}

            //float ratio = 0.001f;
            //if (MazeManager.Backs != 0)
            //{
            //    ratio = (float)MazeManager.Loop / (float)MazeManager.Backs;
            //    Debug.WriteLine(ratio);
            //}
            //else
            //{
            //    ratio = 0.0f;
            //}
            //_spriteBatch.DrawString(arial, ratio.ToString(), new Vector2(mouseState.Position.X + 10, mouseState.Position.Y - 120), Color.Red, default, default, 2f, SpriteEffects.None, default);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
