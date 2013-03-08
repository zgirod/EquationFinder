using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using EquationFinder.Screens;
using Microsoft.Xna.Framework.Storage;
using EquationFinder.Helpers;

namespace EquationFinder
{
    
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class EquationFinderGame : Microsoft.Xna.Framework.Game
    {

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ScreenManager screenManager;

        private IAsyncResult _asyncResult;
        private bool _requestedStorageDevice, _asyncFinsihed;
        
        private static StorageDevice _storageDevice;
        public static StorageDevice StorageDevice { get { return _storageDevice; } }
        public static bool IsTrailMode;

        public EquationFinderGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.Components.Add(new GamerServicesComponent(this));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            // Create the screen manager component.
            screenManager = new ScreenManager(this);

            //add the screen manager as a component
            Components.Add(screenManager);

            //reset the storage device settings
            this._requestedStorageDevice = false;
            this._asyncFinsihed = false;

            // Activate the first screens.
            GameplayOptions.BoardSize = 5;
            GameplayOptions.PlaySoundEffects = true;
            screenManager.AddScreen(new MainMenuScreen(), null);

            //set whether we are in a trail mode or not
            EquationFinderGame.IsTrailMode = false;// Guide.IsTrialMode;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            //if we are finished
            if (!_requestedStorageDevice)
            {

                _requestedStorageDevice = true;
                _asyncResult = StorageDevice.BeginShowSelector(null, null);

            }

            if (_requestedStorageDevice == true
                && _asyncFinsihed == false
                && _asyncResult.IsCompleted)
            {

                _asyncFinsihed = true;

                //save our storage device
                StorageDevice device = StorageDevice.EndShowSelector(_asyncResult);
                if (device != null && device.IsConnected)
                {

                    //set the device
                    _storageDevice = device;

                    //load the game settings
                    StorageHelper.LoadGameSettings();

                }

            }


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

    }

    #region Entry Point

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (var game = new EquationFinderGame())
            {
                game.Run();
            }
        }
    }

    #endregion

}
