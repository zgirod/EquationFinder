using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace EquationFinder.Objects
{
    public class ColorPalette
    {

        public Color DisplayText { get; set; }
        public Color FlashTextCorrect { get; set; }
        public Color FlashTextIncorrect { get; set; }
        public Color BoardText { get; set; }
        public Color BoardTextSelected { get; set; }
        public Color BoardTextPreviouslySelected { get; set; }
        public Color PausedMenuTextSelected { get; set; }
        public Color PausedMenuTextUnselected { get; set; }

    }
}
