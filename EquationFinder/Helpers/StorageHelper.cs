using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using EquationFinder.Objects;

namespace EquationFinder.Helpers
{
    public static class StorageHelper
    {

        public static void SaveFile(StorageDevice device, List<HighScore> highScores, string fileName)
        {

            // Open a storage container.
            IAsyncResult result = device.BeginOpenContainer("EquationFinder", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

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

        public static List<HighScore> GetHighScores(StorageDevice device, string fileName)
        {

            var highScores = new List<HighScore>();

            IAsyncResult result = device.BeginOpenContainer("EquationFinder", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

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
