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

        private static string _gameSettingsFileName = "GameSettings.txt";

        public static string HighScroreFileName(int boardSize)
        {
            return string.Format("HighScores{0}.txt", boardSize);
        }

        public static void SaveInitials(string initials)
        {

            var fileName = "Initials.txt";

            // Open a storage container.
            IAsyncResult result = EquationFinderGame.StorageDevice.BeginOpenContainer("EquationFinder", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = EquationFinderGame.StorageDevice.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            //if we have a file, delete it
            if (container.FileExists(fileName))
                container.DeleteFile(fileName);

            // Create a new file.
            if (!container.FileExists(fileName))
            {
                Stream file = container.CreateFile(fileName);

                using (StreamWriter sw = new StreamWriter(file))
                {

                    sw.WriteLine(initials);

                }

                file.Close();
            }

            // Dispose the container, to commit the data.
            container.Dispose();
        }

        public static string LoadInitials()
        {

            var initials = "AAA";
            var fileName = "Initials.txt";

            // Open a storage container.
            IAsyncResult result = EquationFinderGame.StorageDevice.BeginOpenContainer("EquationFinder", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = EquationFinderGame.StorageDevice.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            // if we have a file
            if (container.FileExists(fileName))
            {

                //open the file
                Stream file = container.OpenFile(fileName, FileMode.Open);

                //loop through each file
                using (StreamReader sr = new StreamReader(file))
                {
                    while (!sr.EndOfStream)
                    {
                        initials = sr.ReadLine();
                    }
                }

                //close the file
                file.Close();

            }

            // Dispose the container, to commit the data.
            container.Dispose();

            return initials;

        }

        public static void SaveGameSettings()
        {

            // Open a storage container.
            IAsyncResult result = EquationFinderGame.StorageDevice.BeginOpenContainer("EquationFinder", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = EquationFinderGame.StorageDevice.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            //if we have a file, delete it
            if (container.FileExists(_gameSettingsFileName))
                container.DeleteFile(_gameSettingsFileName);

            // Create a new file.
            if (!container.FileExists(_gameSettingsFileName))
            {
                Stream file = container.CreateFile(_gameSettingsFileName);

                using (StreamWriter sw = new StreamWriter(file))
                {

                    sw.WriteLine(GameplayOptions.BoardSize.ToString());
                    sw.WriteLine(GameplayOptions.PlaySoundEffects.ToString());

                }


                file.Close();
            }

            // Dispose the container, to commit the data.
            container.Dispose();
        }

        public static void LoadGameSettings()
        {

            // Open a storage container.
            IAsyncResult result = EquationFinderGame.StorageDevice.BeginOpenContainer("EquationFinder", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = EquationFinderGame.StorageDevice.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            // if we have a file
            int row = 1;
            if (container.FileExists(_gameSettingsFileName))
            {

                //open the file
                Stream file = container.OpenFile(_gameSettingsFileName, FileMode.Open);

                //loop through each file
                using (StreamReader sr = new StreamReader(file))
                {
                    while (!sr.EndOfStream)
                    {

                        if (row == 1)
                            GameplayOptions.BoardSize = Convert.ToInt32(sr.ReadLine());
                        else
                            GameplayOptions.PlaySoundEffects = Convert.ToBoolean(sr.ReadLine());

                        row++;

                    }
                }

                //close the file
                file.Close();

            }

            // Dispose the container, to commit the data.
            container.Dispose();
        }

        public static void SaveHighScores(List<HighScore> highScores, int boardSize)
        {

            var fileName = StorageHelper.HighScroreFileName(boardSize);

            // Open a storage container.
            IAsyncResult result = EquationFinderGame.StorageDevice.BeginOpenContainer("EquationFinder", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = EquationFinderGame.StorageDevice.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            //if we have a file, delete it
            if (container.FileExists(fileName))
                container.DeleteFile(fileName);
            
            // Create a new file.
            if (!container.FileExists(fileName))
            {
                Stream file = container.CreateFile(fileName);

                using (StreamWriter sw = new StreamWriter(file))
                {

                    foreach (var highScore in highScores)
                    {
                        sw.WriteLine(string.Format("{0}|{1}", highScore.Initials, highScore.Score));
                    }

                }


                file.Close();
            }

            // Dispose the container, to commit the data.
            container.Dispose();
        }

        public static List<HighScore> LoadHighScores(int boardSize)
        {

            var highScores = new List<HighScore>();
            var fileName = StorageHelper.HighScroreFileName(boardSize);

            IAsyncResult result = EquationFinderGame.StorageDevice.BeginOpenContainer("EquationFinder", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = EquationFinderGame.StorageDevice.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            //if the file exists
            if (container.FileExists(fileName)) 
            {

                //open the file
                Stream file = container.OpenFile(fileName, FileMode.Open);

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

                //close the file
                file.Close();

            }

            // Dispose the container.
            container.Dispose();

            //return the scores
            return highScores;

        }

    }
}
