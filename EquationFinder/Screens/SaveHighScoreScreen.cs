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
    public class SaveHighScoreScreen : GameScreen
    {

        // The font used to display UI elements
        ContentManager _content;
        SpriteFont _gameFont;
        Texture2D _texture;

        public GamePadState GamePadState { get; private set; }
        public KeyboardState KeyboardState { get; private set; }

        private string _availableCharacters = " ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private int _boardSize;
        private long _score;
        private bool _hasHighScore;
        private List<HighScore> _highScores;
        private int _letterNumber;
        private int _highScoreToEnter;
        private string _first, _second, _third;

        public SaveHighScoreScreen(int boardSize, int score, bool hasHighScore)
        {

            //set our options
            _boardSize = boardSize;
            _score = score;
            _hasHighScore = hasHighScore;
            _letterNumber = 1;
            _highScoreToEnter = -1;

            //load the high scores 
            _highScores = StorageHelper.LoadHighScores(_boardSize);

            //create the high score
            var highScore = new HighScore()
                {
                    Initials = "[Enter]",
                    Score = _score
                };

            //if we have a had score
            if (hasHighScore)
            {

                //add the high score
                _highScores.Add(highScore);

            }

            //order the high scores by the score
            _highScores = _highScores.OrderByDescending(x => x.Score).ToList();

            //if we have more than 5 high scores, delete the last one
            if (_highScores.Count > 5)
                _highScores.RemoveAt(5);

            //if we have a high score, get our index
            if (hasHighScore)
                _highScoreToEnter = _highScores.IndexOf(highScore);

            //set the default letters for the initials
            _first = "A";
            _second = "A"; 
            _third = "A";

            
        }

        #region Public Override Methods

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

            GamePadState lastGamePadState = GamePadState;
            KeyboardState lastKeyboardState = KeyboardState;
            GamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState = Keyboard.GetState(PlayerIndex.One);

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
                    ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X,
                    ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y,
                    ScreenManager.GraphicsDevice.Viewport.Width,
                    ScreenManager.GraphicsDevice.Viewport.Height), Color.White);

            //calculate my x and y
            var titleText = string.Format("Board Size: {0}, Your Score: {1:n0}", _boardSize, _score);
            var x = (ScreenManager.GraphicsDevice.Viewport.Width / 2) - (Convert.ToInt32(_gameFont.MeasureString(titleText).Length()) / 2);
            var y = (ScreenManager.GraphicsDevice.Viewport.Height / 10);

            //draw the strings
            spriteBatch.DrawString(_gameFont, titleText, new Vector2(x, y), Color.Black);

            //calculate the new x and y
            x = (ScreenManager.GraphicsDevice.Viewport.Width / 2) - 130;
            y = (ScreenManager.GraphicsDevice.Viewport.Height / 10 * 3);

            //for each high score
            int i = 0;
            foreach (var highScore in _highScores)
            {

                //if we are not showing the high score we are on
                if (i != _highScoreToEnter)
                {

                    //draw the high score
                    spriteBatch.DrawString(_gameFont, highScore.Initials, new Vector2(x, y), Color.Black);
                    spriteBatch.DrawString(_gameFont, string.Format("{0:n0}", highScore.Score), new Vector2(x + 150, y), Color.Black);

                }
                else//we need to draw the input high score line
                {


                    spriteBatch.DrawString(_gameFont, _first, new Vector2(x, y), _letterNumber == 1 ? Color.Blue : Color.OrangeRed);
                    spriteBatch.DrawString(_gameFont, _second,
                        new Vector2(x + _gameFont.MeasureString(_first).X + 3, y), _letterNumber == 2 ? Color.Blue : Color.OrangeRed);
                    spriteBatch.DrawString(_gameFont, _third,
                        new Vector2(x + _gameFont.MeasureString(string.Format("{0}{1}", _first, _second)).X + 6, y), _letterNumber == 3 ? Color.Blue : Color.OrangeRed);

                    //draw the high score
                    spriteBatch.DrawString(_gameFont, string.Format("{0:n0}", highScore.Score), new Vector2(x + 150, y), Color.Blue);


                }
                    
                //go to the next row
                y = y + _gameFont.LineSpacing;
                i++;

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
            if (move.Name == "B")
            {

                //go back to the menu
                LoadingScreen.Load(ScreenManager, true, null, new MainMenuScreen());

            }

            base.HandleMove(gameTime, move);
        }

        #endregion

        #region Private Methods

        private void HandleDirection(Buttons buttons)
        {

            if (buttons.HasFlag(Buttons.DPadLeft) || buttons.HasFlag(Buttons.LeftThumbstickLeft))
            {

                if (_letterNumber == 1)
                    _letterNumber = 3;
                else if (_letterNumber == 2)
                    _letterNumber = 1;
                else if (_letterNumber == 3)
                    _letterNumber = 2;

            }
            else if (buttons.HasFlag(Buttons.DPadRight) || buttons.HasFlag(Buttons.LeftThumbstickRight))
            {

                if (_letterNumber == 1)
                    _letterNumber = 2;
                else if (_letterNumber == 2)
                    _letterNumber = 3;
                else if (_letterNumber == 3)
                    _letterNumber = 1;

            }

        }

        #endregion

    }
}
