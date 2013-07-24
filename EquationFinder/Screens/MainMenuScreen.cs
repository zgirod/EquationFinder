﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EquationFinder;

namespace EquationFinder.Screens
{
    public class MainMenuScreen : MenuScreen
    {

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen() : base("Equation Finder")
        {

            //add the play game
            var menuEntry = new MenuEntry("Play Game", "Play");
            menuEntry.Selected += MenuEntry_Selected;
            MenuEntries.Add(menuEntry);

            //add the controls option
            menuEntry = new MenuEntry("Controls", "Controls");
            menuEntry.Selected += MenuEntry_Selected;
            MenuEntries.Add(menuEntry);

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
            var key = ((MenuEntry)sender).Key;

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



    }
}
