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

namespace EquationFinder.Screens
{
    public class MYOScreen : GameScreen
    {

        // The font used to display UI elements
        ContentManager _content;
        SpriteFont _gameFont;
        Texture2D _texture;

        public GamePadState GamePadState { get; private set; }
        public KeyboardState KeyboardState { get; private set; }

        bool _init = false;
        int _y;

        private List<ScreenText> _screenText;

        public MYOScreen()
        {

        }

        #region Public Override Methods

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

            //make sure we initialize the screen
            if (_init == false)
            {

                _init = true;
                this.Init();

            }

            GamePadState lastGamePadState = GamePadState;
            KeyboardState lastKeyboardState = KeyboardState;
            GamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState = Keyboard.GetState(PlayerIndex.One);

            //get the direction
            var direction = Direction.FromInput(GamePadState, KeyboardState);
            if (direction != 0.0)
            {

                this.HandleDirection(direction);

            }

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

            //if we have text
            if (_screenText != null)
            {

                //we need to draw all the items
                foreach (var screenText in _screenText)
                {

                    //draw the string
                    spriteBatch.DrawString(_gameFont,
                        screenText.Text,
                        screenText.Vector,
                        Color.Black);

                }

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

            if ((direction.Equals(Buttons.DPadUp) || direction.Equals(Buttons.LeftThumbstickUp))
                && (_y <= 0))
            {

                for (int i = 0; i < _screenText.Count; i++)
                {
                    _screenText[i].Vector = new Vector2(
                        _screenText[i].Vector.X,
                        _screenText[i].Vector.Y + 1);
                }

                _y++;

            }
            else if ((direction.Equals(Buttons.DPadDown) || direction.Equals(Buttons.LeftThumbstickDown))
                && (_y >= -150))
            {

                for (int i = 0; i < _screenText.Count; i++)
                {
                    _screenText[i].Vector = new Vector2(
                        _screenText[i].Vector.X,
                        _screenText[i].Vector.Y - 1);
                }

                _y--;

            }

        }

        private void Init()
        {

            _screenText = new List<ScreenText>();
            _y = 0;

            //set the y axis
            var y = ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y;

            //add some text
            y = AddTextToList("This game is open source, you can check out the code here:", y, false, false);
            y = AddTextToList("https://github.com/zgirod/EquationFinder", y, true, true);

        }

        private int AddTextToList(string text, int y, bool newLine, bool addNewLine)
        {


            float stringWidth;
            var availableWidth = (ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Width * 0.95);
            string newText = "";

            //get the width of the string
            stringWidth = _gameFont.MeasureString(text).X;

            //if our string is too big
            if (stringWidth > availableWidth)
            {

                int i = 0;
                newText = text;
                bool first = true;

                //while we have new text to add
                while (!string.IsNullOrEmpty(newText))
                {

                    //go to the next line
                    if (addNewLine || !first)
                        y = y + _gameFont.LineSpacing;

                    //find out how long we can go
                    newText = "";
                    stringWidth = 0.0f;
                    while (i < text.Length && stringWidth < availableWidth)
                    {

                        //add a character to the new text
                        newText += text[i];

                        //get the length of the new string
                        stringWidth = _gameFont.MeasureString(newText).X;

                        //go to the next character
                        i++;
                        first = false;

                    }

                    //if we are not at the end of the string, now we need to go backwwards to make sure we only have full words
                    if (i != text.Length)
                    {

                        int j = newText.Length - 1;
                        var newChar = newText[j].ToString();
                        while (!string.IsNullOrEmpty(newChar.Trim()) && j > 0)
                        {

                            j--;
                            i--;
                            newChar = newText[j].ToString();

                        }

                        //get the new ending position
                        if (j > 0)
                            newText = newText.Substring(0, j);

                    }

                    //add the new text
                    _screenText.Add(new ScreenText()
                    {
                        Text = newText.Trim(),
                        Vector = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 10, y)
                    });

                    //get the rest of the text
                    newText = text.Substring(i);

                }


            }
            else
            {

                //go to the next line
                if (addNewLine)
                    y = y + _gameFont.LineSpacing;

                //just all the full text
                _screenText.Add(new ScreenText()
                {
                    Text = text.Trim(),
                    Vector = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 10, y)
                });


            }

            //if we want a new line
            if (newLine)
                y += _gameFont.LineSpacing;

            //return where we left off
            return y;


        }

        #endregion

    }
}
