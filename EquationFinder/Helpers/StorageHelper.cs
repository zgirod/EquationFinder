using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using EquationFinder.Objects;
using EquationFinder.Screens;

namespace EquationFinder.Helpers
{
    public static class StorageHelper
    {

        public static string _gameSettingsFileName = "GameSettings.txt";

        public static string HighScroreFileName(int boardSize)
        {
            return string.Format("HighScores{0}.txt", boardSize);
        }

        public static void SaveInitials(string initials)
        {

            var fileName = "Initials.txt";
            EquationFinderGame.SaveFile(fileName, initials);

        }

        public static string LoadInitials()
        {

            var initials = "AAA";
            var fileName = "Initials.txt";

            //if the device is ready and we haven't loaded
            if (EquationFinderGame._saveDevice.IsReady)
            {

                EquationFinderGame._saveDevice.LoadAsync(
                    "EquationFinder",
                    fileName,
                    file =>
                    {

                        //loop through each file
                        using (StreamReader sr = new StreamReader(file))
                        {
                            while (!sr.EndOfStream)
                            {
                                initials = sr.ReadLine();
                            }
                        }

                    });

            }

            //return the initials
            return initials;

        }

        public static void SaveGameSettings()
        {

            //set the file contents
            string fileContents =
                GameplayOptions.BoardSize.ToString()
                + Environment.NewLine + GameplayOptions.PlaySoundEffects.ToString()
                + Environment.NewLine + GameplayOptions.PlayMusic.ToString();

            EquationFinderGame.SaveFile(_gameSettingsFileName, fileContents);


        }

        public static void SaveHighScores(List<HighScore> highScores, int boardSize)
        {

            //get the right file name
            var fileName = StorageHelper.HighScroreFileName(boardSize);

            //get the high scores
            StringBuilder sb = new StringBuilder();
            foreach (var highScore in highScores)
                sb.AppendLine(string.Format("{0}|{1}", highScore.Initials, highScore.Score));

            //save the high scores
            EquationFinderGame.SaveFile(fileName, sb.ToString());
            
        }

        public static List<HighScore> LoadHighScores(int boardSize)
        {

            var highScores = new List<HighScore>();
            var fileName = StorageHelper.HighScroreFileName(boardSize);

            //if the device is ready and we haven't loaded
            if (EquationFinderGame._saveDevice.IsReady)
            {

                EquationFinderGame._saveDevice.LoadAsync(
                    "EquationFinder",
                    fileName,
                    file =>
                    {

                        //loop through each file
                        using (StreamReader sr = new StreamReader(file))
                        {
                            while (!sr.EndOfStream)
                            {

                                //get the scores
                                var crumbs = sr.ReadLine().Split(new char[] { '|' });

                                //add the high score
                                highScores.Add(new HighScore()
                                {
                                    Initials = crumbs[0],
                                    Score = Convert.ToInt64(crumbs[1])
                                });

                            }
                        }

                    });

            }

            //return the scores
            return highScores;

        }

    }
}
