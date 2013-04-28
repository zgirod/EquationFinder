using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using EquationFinder.Objects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using EquationFinder.Input;
using Microsoft.Xna.Framework.Input;
using EquationFinder.Helpers;

namespace EquationFinder.Screens
{
    public class HighScoresScreen : GameScreen
    {

        // The font used to display UI elements
        ContentManager _content;
        SpriteFont _gameFont;
        Texture2D _texture;

        public GamePadState GamePadState { get; private set; }
        public KeyboardState KeyboardState { get; private set; }

        private int _boardSize = 5;
        Dictionary<int, List<HighScore>> _highScores;

        public HighScoresScreen()
        {

            _highScores = new Dictionary<int, List<HighScore>>();
            _highScores.Add(5, StorageHelper.LoadHighScores(5).OrderByDescending(x => x.Score).ToList());
            _highScores.Add(6, StorageHelper.LoadHighScores(6).OrderByDescending(x => x.Score).ToList());
            _highScores.Add(7, StorageHelper.LoadHighScores(7).OrderByDescending(x => x.Score).ToList());
            _highScores.Add(8, StorageHelper.LoadHighScores(8).OrderByDescending(x => x.Score).ToList());

        }

        #region Public Override Methods

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

            GamePadState lastGamePadState = GamePadState;
            KeyboardState lastKeyboardState = KeyboardState;
            GamePadState = InputHelpers.GetGamePadStateForAllPLayers();
            KeyboardState = InputHelpers.GetKeyboardStateForAllPLayers();

            //get the direction
            //get the direction
            var direction = Direction.FromInput(GamePadState, KeyboardState);
            if (Direction.FromInput(lastGamePadState, lastKeyboardState) != direction && direction != 0.0)
                this.HandleDirection(direction);

            //call the base update
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

        }

        public override void Draw(GameTime gameTime)
        {

            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Start drawing
            spriteBatch.Begin();

            //draw the background
            spriteBatch.Draw(_texture,
                new Rectangle(
                    ScreenManager.GraphicsDevice.Viewport.X,
                    ScreenManager.GraphicsDevice.Viewport.Y,
                    ScreenManager.GraphicsDevice.Viewport.Width,
                    ScreenManager.GraphicsDevice.Viewport.Height), Color.White);

            //calculate my x and y
            var titleText = string.Format("Board Size: {0}", _boardSize);
            var x = (ScreenManager.GraphicsDevice.Viewport.Width / 2) - (Convert.ToInt32(_gameFont.MeasureString(titleText).Length()) / 2);
            var y = (ScreenManager.GraphicsDevice.Viewport.Height / 10);

            //draw the strings
            spriteBatch.DrawString(_gameFont, titleText, new Vector2(x, y), Color.Blue);

            //get the high scores that we want to show
            var highScores = _highScores.FirstOrDefault(s => s.Key == _boardSize).Value;

            //calculate the new x and y
            x = (ScreenManager.GraphicsDevice.Viewport.Width / 2) - 130;
            y = (ScreenManager.GraphicsDevice.Viewport.Height / 10 * 3);

            //for each high score
            foreach (var highScore in highScores)
            {

                //draw the high score
                spriteBatch.DrawString(_gameFont, highScore.Initials, new Vector2(x, y), Color.Black);
                spriteBatch.DrawString(_gameFont, string.Format("{0:n0}", highScore.Score), new Vector2(x + 150, y), Color.Black);
                y = y + 40;

            }

            // stop drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public override void LoadContent()
        {

            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            // Load the background texture we will be using
            _texture = _content.Load<Texture2D>("img/paper_fibers");

            // Load the score font
            _gameFont = _content.Load<SpriteFont>("gameFont");

            base.LoadContent();

        }

        public override void UnloadContent()
        {

            _content.Unload();

        }

        public override void HandleMove(GameTime gameTime, Move move)
        {

            //if we want to go back
            if (move.Name == "B"
                || move.Name == "A")
            {

                //go back to the menu
                LoadingScreen.Load(ScreenManager, true, null, new MainMenuScreen());

            }

            base.HandleMove(gameTime, move);
        }

        #endregion

        #region Private Methods

        private void HandleDirection(Buttons direction)
        {

            if (direction.Equals(Buttons.DPadLeft) || direction.Equals(Buttons.LeftThumbstickLeft))
            {

                if (_boardSize == 5)
                    _boardSize = 8;
                else if (_boardSize == 6)
                    _boardSize = 5;
                else if (_boardSize == 7)
                    _boardSize = 6;
                else if (_boardSize == 8)
                    _boardSize = 7;

            }
            else if (direction.Equals(Buttons.DPadRight) || direction.Equals(Buttons.LeftThumbstickRight))
            {

                if (_boardSize == 5)
                    _boardSize = 6;
                else if (_boardSize == 6)
                    _boardSize = 7;
                else if (_boardSize == 7)
                    _boardSize = 8;
                else if (_boardSize == 8)
                    _boardSize = 5;

            }

        }

        #endregion

    }
}
