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
using EasyStorage;
using System.IO;

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

        private bool _asyncFinsihed;
        public static IAsyncSaveDevice _saveDevice;
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

            //set the supported languages
            EasyStorageSettings.SetSupportedLanguages(Language.English);

            // create and add our SaveDevice
            SharedSaveDevice sharedSaveDevice = new SharedSaveDevice();
            Components.Add(sharedSaveDevice);

            // make sure we hold on to the device
            _saveDevice = sharedSaveDevice;

            // hook two event handlers to force the user to choose a new device if they cancel the
            // device selector or if they disconnect the storage device after selecting it
            sharedSaveDevice.DeviceSelectorCanceled += (s, e) => e.Response = SaveDeviceEventResponse.Force;
            sharedSaveDevice.DeviceDisconnected += (s, e) => e.Response = SaveDeviceEventResponse.Force;

            // prompt for a device on the first Update we can
            sharedSaveDevice.PromptForDevice();

//#if XBOX
//            // add the GamerServicesComponent
//            Components.Add(new Microsoft.Xna.Framework.GamerServices.GamerServicesComponent(this));
//#endif

            //add the screen manager as a component
            Components.Add(screenManager);

            //reset the storage device settings
            this._asyncFinsihed = false;

            // Activate the first screens.
            GameplayOptions.BoardSize = 5;
            GameplayOptions.PlaySoundEffects = true;
            GameplayOptions.PlayMusic = "Battle";
            GameplayOptions.StartNumber = 19;
            GameplayOptions.BackgroundImage = "Map";
            screenManager.AddScreen(new MainMenuScreen());

            //set whether we are in a trail mode or not
            EquationFinderGame.IsTrailMode = Guide.IsTrialMode;

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
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            //if the device is ready and we haven't loaded
            if (_saveDevice.IsReady && _asyncFinsihed == false)
            {

                _saveDevice.LoadAsync(
                    "EquationFinder",
                    StorageHelper._gameSettingsFileName,
                    file =>
                    {

                        int row = 1;

                        //loop through each file
                        using (StreamReader sr = new StreamReader(file))
                        {
                            while (!sr.EndOfStream)
                            {

                                //set the proper gameplay property
                                if (row == 1)
                                    GameplayOptions.BoardSize = Convert.ToInt32(sr.ReadLine());
                                else if (row == 2)
                                    GameplayOptions.PlaySoundEffects = Convert.ToBoolean(sr.ReadLine());
                                else if (row == 3)
                                    GameplayOptions.PlayMusic = sr.ReadLine();
                                else if (row == 4)
                                    GameplayOptions.StartNumber = Convert.ToInt32(sr.ReadLine());
                                else if (row == 5)
                                    GameplayOptions.BackgroundImage = sr.ReadLine();

                                //go to the next row
                                row++;

                            }
                        }

                        _asyncFinsihed = true;

                    });

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

            base.Draw(gameTime);
        }

        public static void SaveFile(string fileName, string fileContents) 
        {


            // make sure the device is ready
            if (_saveDevice.IsReady)
            {
                // save a file asynchronously. this will trigger IsBusy to return true
                // for the duration of the save process.
                _saveDevice.SaveAsync(
                    "EquationFinder",
                    fileName,
                    stream =>
                    {
                        //write the contents of the file to the file system
                        using (StreamWriter writer = new StreamWriter(stream))
                            writer.Write(fileContents);
                    });
            }

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
