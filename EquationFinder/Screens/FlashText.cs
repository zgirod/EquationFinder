using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EquationFinder.Screens
{
    public class FlashText
    {

        public bool Active;
        public string Text;
        public double StartTime;
        public bool RunClock;
        public bool IsErrorText;

        public void SetFlashText(string text, double startTime, bool runClock, bool isErrorText) 
        {
            this.Active = true;
            this.Text = text;
            this.StartTime = startTime;
            this.RunClock = runClock;
            this.IsErrorText = isErrorText;
        }

    }
}
