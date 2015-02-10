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
            GamePadState = InputHelpers.GetGamePadStateForAllPLayers();
            KeyboardState = InputHelpers.GetKeyboardStateForAllPLayers();

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
            var x = (ScreenManager.GraphicsDevice.Viewport.Width / 2) - 200;
            var y = (ScreenManager.GraphicsDevice.Viewport.Height / 2) - 125;

            //draw the strings
            spriteBatch.DrawString(_gameFont, string.Format("Board Size:   {0}", GameplayOptions.BoardSize), new Vector2(x, y), row == 1 ? Color.Blue : Color.Black);
            spriteBatch.DrawString(_gameFont, string.Format("Play Sound Effects:   {0}", GameplayOptions.PlaySoundEffects ? "Yes" : "No"), new Vector2(x, y + 50), row == 2 ? Color.Blue : Color.Black);
            spriteBatch.DrawString(_gameFont, string.Format("Play Music:   {0}", GameplayOptions.PlayMusic), new Vector2(x, y + 100), row == 3 ? Color.Blue : Color.Black);
            spriteBatch.DrawString(_gameFont, string.Format("Start Number:   {0}", 
                GameplayOptions.StartNumber < 0 ? "Random" : GameplayOptions.StartNumber.ToString()), 
                new Vector2(x, y + 150), row == 4 ? Color.Blue : Color.Black);
            spriteBatch.DrawString(_gameFont, string.Format("Background:   {0}", GameplayOptions.BackgroundImage.Replace("_", " ")), new Vector2(x, y + 200), row == 5 ? Color.Blue : Color.Black);

            // stop drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public override void LoadContent()
        {

            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            // Load the background texture we will be using
            _texture = _content.Load<Texture2D>("img/bg/Fibers");

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

                //save the game options
                StorageHelper.SaveGameSettings();

                //go back to the menu
                LoadingScreen.Load(ScreenManager, true, null, new MainMenuScreen());

            }

            base.HandleMove(gameTime, move);
        }

        #endregion

        #region Private Methods

        private void HandleDirection(Buttons direction)
        {

            if (direction.Equals(Buttons.DPadUp) || direction.Equals(Buttons.LeftThumbstickUp))
            {

                if (row == 1)
                    row = 5;
                else if (row == 2)
                    row = 1;
                else if (row == 3)
                    row = 2;
                else if (row == 4)
                    row = 3;
                else
                    row = 4;


            }
            else if (direction.Equals(Buttons.DPadDown) || direction.Equals(Buttons.LeftThumbstickDown))
            {

                if (row == 1)
                    row = 2;
                else if (row == 2)
                    row = 3;
                else if (row == 3)
                    row = 4;
                else if (row == 4)
                    row = 5;
                else
                    row = 1;

            }
            else if (direction.Equals(Buttons.DPadLeft) || direction.Equals(Buttons.LeftThumbstickLeft))
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
                else if (row == 2)
                {

                    if (GameplayOptions.PlaySoundEffects)
                        GameplayOptions.PlaySoundEffects = false;
                    else
                        GameplayOptions.PlaySoundEffects = true;

                }
                else if (row == 3)
                {

                    if (GameplayOptions.PlayMusic == "Off")
                    {
                        GameplayOptions.PlayMusic = "Battle";
                    }
                    else if (GameplayOptions.PlayMusic == "Battle")
                    {
                        GameplayOptions.PlayMusic = "Dungeon";
                    }
                    else if (GameplayOptions.PlayMusic == "Dungeon")
                    {
                        GameplayOptions.PlayMusic = "Forest";
                    }
                    else if (GameplayOptions.PlayMusic == "Forest")
                    {
                        GameplayOptions.PlayMusic = "Off";
                    }
                    else
                    {
                        GameplayOptions.PlayMusic = "Off";
                    }

                }
                else if (row == 4)
                {

                    //go to the next number below
                    GameplayOptions.StartNumber--;

                    if (GameplayOptions.StartNumber < -1)
                        GameplayOptions.StartNumber = 50;

                    else if (GameplayOptions.StartNumber < 11)
                        GameplayOptions.StartNumber = -1;

                }
                else if (row == 5)
                {

                    if (GameplayOptions.BackgroundImage == "Color")
                    {
                        GameplayOptions.BackgroundImage = "Wood";
                    }
                    else if (GameplayOptions.BackgroundImage == "Cornflower_Blue")
                    {
                        GameplayOptions.BackgroundImage = "Color";
                    }
                    else if (GameplayOptions.BackgroundImage == "Fibers")
                    {
                        GameplayOptions.BackgroundImage = "Cornflower_Blue";
                    }
                    else if (GameplayOptions.BackgroundImage == "Ghost_White")
                    {
                        GameplayOptions.BackgroundImage = "Fibers";
                    }
                    else if (GameplayOptions.BackgroundImage == "Hulk_Green")
                    {
                        GameplayOptions.BackgroundImage = "Ghost_White";
                    }
                    else if (GameplayOptions.BackgroundImage == "Joker_Purple")
                    {
                        GameplayOptions.BackgroundImage = "Hulk_Green";
                    }
                    else if (GameplayOptions.BackgroundImage == "Map")
                    {
                        GameplayOptions.BackgroundImage = "Joker_Purple";
                    }
                    else if (GameplayOptions.BackgroundImage == "Midnight_Black")
                    {
                        GameplayOptions.BackgroundImage = "Map";
                    }
                    else if (GameplayOptions.BackgroundImage == "Round_Dreams")
                    {
                        GameplayOptions.BackgroundImage = "Midnight_Black";
                    }
                    else if (GameplayOptions.BackgroundImage == "Smoke")
                    {
                        GameplayOptions.BackgroundImage = "Round_Dreams";
                    }
                    else if (GameplayOptions.BackgroundImage == "Wood")
                    {
                        GameplayOptions.BackgroundImage = "Smoke";
                    }

                }

            }
            else if (direction.Equals(Buttons.DPadRight) || direction.Equals(Buttons.LeftThumbstickRight))
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
                else if (row == 2)
                {

                    if (GameplayOptions.PlaySoundEffects)
                        GameplayOptions.PlaySoundEffects = false;
                    else
                        GameplayOptions.PlaySoundEffects = true;

                }
                else if (row == 3)
                {

                    if (GameplayOptions.PlayMusic == "Off")
                    {
                        GameplayOptions.PlayMusic = "Forest";
                    }
                    else if (GameplayOptions.PlayMusic == "Battle")
                    {
                        GameplayOptions.PlayMusic = "Off";
                    }
                    else if (GameplayOptions.PlayMusic == "Dungeon")
                    {
                        GameplayOptions.PlayMusic = "Battle";
                    }
                    else if (GameplayOptions.PlayMusic == "Forest")
                    {
                        GameplayOptions.PlayMusic = "Dungeon";
                    }
                    else
                    {
                        GameplayOptions.PlayMusic = "Off";
                    }

                }
                else if (row == 4)
                {

                    GameplayOptions.StartNumber++;

                    //if the number is above 50, go to 1
                    if (GameplayOptions.StartNumber > 50)
                        GameplayOptions.StartNumber = -1;
                    else if (GameplayOptions.StartNumber < 11)
                        GameplayOptions.StartNumber = 11;

                }
                else if (row == 5)
                {

                    if (GameplayOptions.BackgroundImage == "Color")
                    {
                        GameplayOptions.BackgroundImage = "Cornflower_Blue";
                    }
                    else if (GameplayOptions.BackgroundImage == "Cornflower_Blue")
                    {
                        GameplayOptions.BackgroundImage = "Fibers";
                    }
                    else if (GameplayOptions.BackgroundImage == "Fibers")
                    {
                        GameplayOptions.BackgroundImage = "Ghost_White";
                    }
                    else if (GameplayOptions.BackgroundImage == "Ghost_White")
                    {
                        GameplayOptions.BackgroundImage = "Hulk_Green";
                    }
                    else if (GameplayOptions.BackgroundImage == "Hulk_Green")
                    {
                        GameplayOptions.BackgroundImage = "Joker_Purple";
                    }
                    else if (GameplayOptions.BackgroundImage == "Joker_Purple")
                    {
                        GameplayOptions.BackgroundImage = "Map";
                    }
                    else if (GameplayOptions.BackgroundImage == "Map")
                    {
                        GameplayOptions.BackgroundImage = "Midnight_Black";
                    }
                    else if (GameplayOptions.BackgroundImage == "Midnight_Black")
                    {
                        GameplayOptions.BackgroundImage = "Round_Dreams";
                    }
                    else if (GameplayOptions.BackgroundImage == "Round_Dreams")
                    {
                        GameplayOptions.BackgroundImage = "Smoke";
                    }
                    else if (GameplayOptions.BackgroundImage == "Smoke")
                    {
                        GameplayOptions.BackgroundImage = "Wood";
                    }
                    else if (GameplayOptions.BackgroundImage == "Wood")
                    {
                        GameplayOptions.BackgroundImage = "Color";
                    }

                }


            }

        }

        #endregion

    }
}
