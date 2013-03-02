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
    public class OptionsScreen : GameScreen
    {

        // The font used to display UI elements
        ContentManager _content;
        SpriteFont _gameFont;
        Texture2D _texture;

        public GamePadState GamePadState { get; private set; }
        public KeyboardState KeyboardState { get; private set; }


        private int row = 1;
       

        public OptionsScreen()
        {

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
            var x = (ScreenManager.GraphicsDevice.Viewport.Width / 2) - 200;
            var y = (ScreenManager.GraphicsDevice.Viewport.Height / 2) - 50;


            //draw the strings
            spriteBatch.DrawString(_gameFont, string.Format("Board Size:   {0}", GameplayOptions.BoardSize), new Vector2(x, y), row == 1 ? Color.Blue : Color.Black);
            spriteBatch.DrawString(_gameFont, string.Format("Play Sound Effects:   {0}", GameplayOptions.PlaySoundEffects ? "Yes" : "No"), new Vector2(x, y + 50), row == 2 ? Color.Blue : Color.Black);

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

                //save the game options
                StorageHelper.SaveGameSettings();

                //go back to the menu
                LoadingScreen.Load(ScreenManager, true, null, new MainMenuScreen());

            }

            base.HandleMove(gameTime, move);
        }

        #endregion

        #region Private Methods

        private void HandleDirection(Buttons buttons)
        {

            if (buttons.HasFlag(Buttons.DPadUp) || buttons.HasFlag(Buttons.LeftThumbstickUp)
                || buttons.HasFlag(Buttons.DPadDown) || buttons.HasFlag(Buttons.LeftThumbstickDown))
            {

                if (row == 1)
                    row = 2;
                else
                    row = 1;


            }
            else if (buttons.HasFlag(Buttons.DPadLeft) || buttons.HasFlag(Buttons.LeftThumbstickLeft))
            {

                if (row == 1)
                {

                    if (GameplayOptions.BoardSize == 5)
                        GameplayOptions.BoardSize = 8;
                    else if (GameplayOptions.BoardSize == 6)
                        GameplayOptions.BoardSize = 5;
                    else if (GameplayOptions.BoardSize == 7)
                        GameplayOptions.BoardSize = 6;
                    else if (GameplayOptions.BoardSize == 8)
                        GameplayOptions.BoardSize = 7;


                }
                else
                {

                    if (GameplayOptions.PlaySoundEffects)
                        GameplayOptions.PlaySoundEffects = false;
                    else 
                        GameplayOptions.PlaySoundEffects = true;

                }

            }
            else if (buttons.HasFlag(Buttons.DPadRight) || buttons.HasFlag(Buttons.LeftThumbstickRight))
            {

                if (row == 1)
                {

                    if (GameplayOptions.BoardSize == 5)
                        GameplayOptions.BoardSize = 6;
                    else if (GameplayOptions.BoardSize == 6)
                        GameplayOptions.BoardSize = 7;
                    else if (GameplayOptions.BoardSize == 7)
                        GameplayOptions.BoardSize = 8;
                    else if (GameplayOptions.BoardSize == 8)
                        GameplayOptions.BoardSize = 5;

                }
                else
                {

                    if (GameplayOptions.PlaySoundEffects)
                        GameplayOptions.PlaySoundEffects = false;
                    else
                        GameplayOptions.PlaySoundEffects = true;

                }

            }

        }

        #endregion

    }
}
