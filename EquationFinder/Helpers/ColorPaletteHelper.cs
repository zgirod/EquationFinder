using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EquationFinder.Objects;
using Microsoft.Xna.Framework;
using EquationFinder.Screens;

namespace EquationFinder.Helpers
{
    public static class ColorPaletteHelper
    {

        public static ColorPalette GetColorPalette()
        {

            if (GameplayOptions.BackgroundImage == "Map")
            {
                return new ColorPalette()
                {
                    BoardText = Color.White,
                    BoardTextSelected = Color.Red,
                    DisplayText = Color.White,
                    FlashTextCorrect = Color.Green,
                    FlashTextIncorrect = Color.DarkRed,
                    PausedMenuTextSelected = Color.Red,
                    PausedMenuTextUnselected = Color.White
                };

            }
            else if (GameplayOptions.BackgroundImage == "Tree")
            {
                return new ColorPalette()
                {
                    BoardText = Color.DarkTurquoise,
                    BoardTextSelected = Color.Red,
                    DisplayText = Color.Yellow,
                    FlashTextCorrect = Color.Green,
                    FlashTextIncorrect = Color.DarkRed,
                    PausedMenuTextSelected = Color.Red,
                    PausedMenuTextUnselected = Color.Black
                };
            }
            else if (GameplayOptions.BackgroundImage == "Fibers")
            {
                return new ColorPalette()
                {
                    BoardText = Color.Black,
                    BoardTextSelected = Color.Red,
                    DisplayText = Color.Black,
                    FlashTextCorrect = Color.Green,
                    FlashTextIncorrect = Color.DarkRed,
                    PausedMenuTextSelected = Color.Red,
                    PausedMenuTextUnselected = Color.Black
                };
            }
            else
            {
                throw new NotImplementedException();
            }

        }

    }
}
