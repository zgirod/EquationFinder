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
    public class FAQScreen : GameScreen
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

        public FAQScreen()
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
                    ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X,
                    ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y,
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

            if ((buttons.HasFlag(Buttons.DPadUp) || buttons.HasFlag(Buttons.LeftThumbstickUp))
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
            else if ((buttons.HasFlag(Buttons.DPadDown) || buttons.HasFlag(Buttons.LeftThumbstickDown))
                && (_y >= -1000))
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
            y = AddTextToList("Q: Why does it start on 19?", y, false, false);
            y = AddTextToList("Because that is my birthday", y, true, true);
            y = AddTextToList("Q: How do I go to the next number?", y, false, true);
            y = AddTextToList("If you get 4 or more equations correct you can move to the next number.", y, true, true);
            y = AddTextToList("Q: What do I get in the trail mode?", y, false, true);
            y = AddTextToList("You get to play the full game however to don't get to go to the next number when your round ends.", y, true, true);
            y = AddTextToList("Q: Why does it start on 19?", y, false, true);
            y = AddTextToList("Q: Why does it start on 19?", y, false, true);
            y = AddTextToList("Q: Why does it start on 19?", y, false, true);
            y = AddTextToList("Q: Why does it start on 19?", y, false, true);
            y = AddTextToList("Q: Why does it start on 19?", y, false, true);

  

            ////add some text
            //_screenText.Add(new ScreenText()
            //{
            //    Text = "A: ",
            //    Vector = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 10, y)
            //});
            //y += _gameFont.LineSpacing * 2;

            ////add some text
            //_screenText.Add(new ScreenText()
            //{
            //    Text = "Q: How are scores calculated?",
            //    Vector = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 10, y)
            //});
            //y += _gameFont.LineSpacing;

            ////add some text
            //_screenText.Add(new ScreenText()
            //{
            //    Text = "A: We count the number of numbers and operators, excluding parenthesis, and multiple that count by the target number.",
            //    Vector = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 10, y)
            //});
            //y += _gameFont.LineSpacing * 2;

            ////add some text
            //_screenText.Add(new ScreenText()
            //{
            //    Text = "Q: Can you show me an example?",
            //    Vector = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 10, y)
            //});
            //y += _gameFont.LineSpacing;

            ////add some text
            //_screenText.Add(new ScreenText()
            //{
            //    Text = "Say your target is 19 and your equations was (9 * 3) - 10 + 2.  There are 4 numbers and 3 operators which = 7.  7 * 19 = 133.",
            //    Vector = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 10, y)
            //});
            //y += _gameFont.LineSpacing * 2;

            ////add some text
            //_screenText.Add(new ScreenText()
            //{
            //    Text = "Q: What operators are allowed?",
            //    Vector = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 10, y)
            //});
            //y += _gameFont.LineSpacing;

            ////add some text
            //_screenText.Add(new ScreenText()
            //{
            //    Text = "Parentheses, Multiplication and Division, and Addition and Subtraction",
            //    Vector = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 10, y)
            //});
            //y += _gameFont.LineSpacing * 2;

            ////add some text
            //_screenText.Add(new ScreenText()
            //{
            //    Text = "Q: Why are the controls so odd?",
            //    Vector = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 10, y)
            //});
            //y += _gameFont.LineSpacing;

            ////add some text
            //_screenText.Add(new ScreenText()
            //{
            //    Text = "A: It is the best I could come up with.",
            //    Vector = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 10, y)
            //});
            //y += _gameFont.LineSpacing;

            ////add some text
            //_screenText.Add(new ScreenText()
            //{
            //    Text = "Email equationfinder@gmail.com with suggestions.",
            //    Vector = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 10, y)
            //});
            //y += _gameFont.LineSpacing;

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
                while (!string.IsNullOrWhiteSpace(newText))
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
                        while (!string.IsNullOrWhiteSpace(newChar) && j > 0)
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
