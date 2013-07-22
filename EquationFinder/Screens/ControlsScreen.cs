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
    public class ControlsScreen : GameScreen
    {

        // The font used to display UI elements
        ContentManager _content;
        SpriteFont _gameFont;
        Texture2D _texture;
        Texture2D[] _controls;
        int screenCount = 0;

        public GamePadState GamePadState { get; private set; }
        public KeyboardState KeyboardState { get; private set; }

        public ControlsScreen()
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

            //draw the controllers images
            spriteBatch.Draw(_controls[screenCount],
                new Rectangle(
                    (ScreenManager.GraphicsDevice.Viewport.Width / 2) - (_controls[screenCount].Width / 2),
                    (ScreenManager.GraphicsDevice.Viewport.Height / 2) - (_controls[screenCount].Height / 2),
                    _controls[screenCount].Width,
                    _controls[screenCount].Height), Color.White);

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
            
            //load all the images 
            _controls = new Texture2D[21];
            for (int i = 0; i < 21; i++)
                _controls[i] = _content.Load<Texture2D>("img/howto/" + (i+1).ToString());

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
            if (move.Name == "A")
                screenCount++;
            else if (move.Name == "B")
                screenCount--;

            //do not allow the screen count to go below 0
            if (screenCount < 0) screenCount = 0;

            //if we have gone through all the screen, go back to the main menu
            if (this.screenCount >= _controls.Count())
                LoadingScreen.Load(ScreenManager, true, null, new MainMenuScreen());

            base.HandleMove(gameTime, move);
        }

        private void HandleDirection(Buttons direction)
        {
            if (direction.Equals(Buttons.DPadRight) || direction.Equals(Buttons.LeftThumbstickRight))
                screenCount++;
            else if (direction.Equals(Buttons.DPadLeft) || direction.Equals(Buttons.LeftThumbstickLeft))
                screenCount--;

            //do not allow the screen count to go below 0
            if (screenCount < 0) screenCount = 0;

            //if we have gone through all the screen, go back to the main menu
            if (this.screenCount >= _controls.Count())
                LoadingScreen.Load(ScreenManager, true, null, new MainMenuScreen());

        }

        #endregion

    }
}
