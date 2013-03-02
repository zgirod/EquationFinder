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
                && (_y <= 100))
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
                && (_y >= -10))
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
            var incrmentValue = 30;

            //add some text
            _screenText.Add(new ScreenText()
            {
                Text = "This is Test Text",
                Vector = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 10, y)
            });
            y += incrmentValue;

            //add some text
            _screenText.Add(new ScreenText()
            {
                Text = "This is Test Text2",
                Vector = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 10, y)
            });
            y += incrmentValue;

            //add some text
            _screenText.Add(new ScreenText()
            {
                Text = "This is Test Text3",
                Vector = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 10, y)
            });
            y += incrmentValue;

        }

        #endregion

    }
}
