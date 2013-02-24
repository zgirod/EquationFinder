using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Text; 
using Microsoft.Xna.Framework; 
using Microsoft.Xna.Framework.Audio; 
using Microsoft.Xna.Framework.Content; 
using Microsoft.Xna.Framework.GamerServices; 
using Microsoft.Xna.Framework.Graphics; 
using Microsoft.Xna.Framework.Input; 
using Microsoft.Xna.Framework.Media; 
 
namespace EquationFinder.Helpers  
{ 
    class ClockTimer 
    {    
        private int endTimer; 
        private int countTimerRef; 
        public String displayClock { get; private set; } 
        public bool isRunning { get; private set; } 
        public bool isFinished { get; private set; } 
         
        public ClockTimer() 
        { 
 
            displayClock = ""; 
            endTimer = 0; 
            countTimerRef = 0; 
            isRunning = false; 
            isFinished = false; 
 
        } 

        public void Start(int seconds) 
        { 
            endTimer = seconds; 
            isRunning = true; 
            displayClock = endTimer.ToString(); 
        }

        public void AddTime(int seconds)
        {
            endTimer += seconds;
        }

        public void NextRound(int second)
        {
            isFinished = false;
            endTimer += second;
            displayClock = endTimer.ToString();
        }
 
        public Boolean CheckTime(GameTime gameTime) 
        { 
            countTimerRef += (int)gameTime.ElapsedGameTime.TotalMilliseconds; 
            if (!isFinished) 
            { 
                if (countTimerRef >= 1000.0f) 
                { 
                    endTimer = endTimer - 1; 
                    displayClock = endTimer.ToString(); 
                    countTimerRef = 0; 
                    if (endTimer <= 0) 
                    { 
                        endTimer = 0; 
                        isFinished = true; 
                        displayClock = "Game Over"; 
                    } 
                } 
            } 
            else 
            { 
                displayClock = "Game Over"; 
            } 
            return isFinished; 
        } 

        public void Reset() 
        { 
            isRunning = false; 
            isFinished = false; 
            displayClock = "None"; 
            countTimerRef = 0; 
            endTimer = 0; 
        }   
    } 
} 