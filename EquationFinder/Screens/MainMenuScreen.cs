using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EquationFinder;
using EquationFinder.Helpers;
using Microsoft.Xna.Framework;
using EquationFinder.Input;

namespace EquationFinder.Screens
{
    public class MainMenuScreen : MenuScreen
    {

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen() : base("Equation Finder")
        {

            var menuEntry = new MenuEntry("", "");

			//if we haven't finished the how to, make it first in the list
			if (StorageHelper.IsHowToFinished() == false)
			{
				//add the controls option
				menuEntry = new MenuEntry("Controls", "Controls");
				menuEntry.Selected += MenuEntry_Selected;
				MenuEntries.Add(menuEntry);
			}
		
            //add the play game
            menuEntry = new MenuEntry("Play Game", "Play");
            menuEntry.Selected += MenuEntry_Selected;
            MenuEntries.Add(menuEntry);

            //if we have finished the how to, make it second in the list
			if (StorageHelper.IsHowToFinished() == true)
			{
				//add the controls option
				menuEntry = new MenuEntry("Controls", "Controls");
				menuEntry.Selected += MenuEntry_Selected;
				MenuEntries.Add(menuEntry);
			}

            //add the how it works option
            menuEntry = new MenuEntry("F.A.Q", "FAQ");
            menuEntry.Selected += MenuEntry_Selected;
            MenuEntries.Add(menuEntry);

            //add the high scores option
            menuEntry = new MenuEntry("High Scores", "Scores");
            menuEntry.Selected += MenuEntry_Selected;
            MenuEntries.Add(menuEntry);

            //add the options entry
            menuEntry = new MenuEntry("Game Options", "Options");
            menuEntry.Selected += MenuEntry_Selected;
            MenuEntries.Add(menuEntry);

            //add the make your own game entry
            //menuEntry = new MenuEntry("Make your own game", "Make");
            //menuEntry.Selected += MenuEntry_Selected;
            //MenuEntries.Add(menuEntry);

            //add the exit option
            menuEntry = new MenuEntry("Exit", "Exit");
            menuEntry.Selected += MenuEntry_Selected;
            MenuEntries.Add(menuEntry);
            
        }

        void MenuEntry_Selected(object sender, EventArgs e)
        {

            //set our qui type
            var menuEntry = ((MenuEntry)sender);
            if (menuEntry == null)
                return;
            var key = menuEntry.Key;

            //if we want to play
            if (key == "Play")
            {

                //load the game play screen
                LoadingScreen.Load(ScreenManager, true, null, new GameplayScreen(GameplayOptions.StartNumber));

            }
            else if (key == "FAQ")
            {


                //load the game play screen
                LoadingScreen.Load(ScreenManager, true, null, new FAQScreen());

            }
            else if (key == "Exit")
            {

                //exit the game
                ScreenManager.Game.Exit();

            }
            else if (key == "Options")
            {

                //load the game options
                LoadingScreen.Load(ScreenManager, true, null, new OptionsScreen());

            }
            else if (key == "Scores")
            {

                //load the high scores screen
                LoadingScreen.Load(ScreenManager, true, null, new HighScoresScreen());

            }
            else if (key == "Make")
            {

                //load the make your own game screen
                LoadingScreen.Load(ScreenManager, true, null, new MYOScreen());

            }
            else if (key == "Controls")
            {

                //load the controls screen
                LoadingScreen.Load(ScreenManager, true, null, new ControlsScreen());

            }

        }

        public override void HandleMove(GameTime gameTime, Move move)
        {

            //if we hit the back button, don't do anything
            if (move.Name.ToLower() == "b")
                return;

            base.HandleMove(gameTime, move);
        }

    }

}
