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


            if (GameplayOptions.BackgroundImage == "Color")
            {
                return new ColorPalette()
                {
                    BoardText = Color.LawnGreen,
                    BoardTextSelected = Color.Red,
                    BoardTextPreviouslySelected = Color.Blue,
                    DisplayText = Color.LawnGreen,
                    FlashTextCorrect = Color.GreenYellow,
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
                    BoardTextPreviouslySelected = Color.Blue,
                    DisplayText = Color.Black,
                    FlashTextCorrect = Color.Green,
                    FlashTextIncorrect = Color.DarkRed,
                    PausedMenuTextSelected = Color.Red,
                    PausedMenuTextUnselected = Color.Black
                };
            }
            else if (GameplayOptions.BackgroundImage == "Map")
            {
                return new ColorPalette()
                {
                    BoardText = Color.White,
                    BoardTextSelected = Color.Red,
                    BoardTextPreviouslySelected = Color.Blue,
                    DisplayText = Color.White,
                    FlashTextCorrect = Color.Green,
                    FlashTextIncorrect = Color.Red,
                    PausedMenuTextSelected = Color.Red,
                    PausedMenuTextUnselected = Color.White,
                };

            }
            else if (GameplayOptions.BackgroundImage == "Round_Dreams")
            {
                return new ColorPalette()
                {
                    BoardText = Color.DarkRed,
                    BoardTextSelected = Color.Violet,
                    BoardTextPreviouslySelected = Color.Green,
                    DisplayText = Color.DarkRed,
                    FlashTextCorrect = Color.LawnGreen,
                    FlashTextIncorrect = Color.DarkRed,
                    PausedMenuTextSelected = Color.Red,
                    PausedMenuTextUnselected = Color.Black
                };
            }
            else if (GameplayOptions.BackgroundImage == "Smoke")
            {
                return new ColorPalette()
                {
                    BoardText = Color.Black,
                    BoardTextSelected = Color.LawnGreen,
                    BoardTextPreviouslySelected = Color.Red,
                    DisplayText = Color.DodgerBlue,
                    FlashTextCorrect = Color.LawnGreen,
                    FlashTextIncorrect = Color.DarkRed,
                    PausedMenuTextSelected = Color.Red,
                    PausedMenuTextUnselected = Color.Black
                };
            }
            else if (GameplayOptions.BackgroundImage == "Wood")
            {
                return new ColorPalette()
                {
                    BoardText = Color.WhiteSmoke,
                    BoardTextSelected = Color.Black,
                    BoardTextPreviouslySelected = Color.Blue,
                    DisplayText = Color.WhiteSmoke,
                    FlashTextCorrect = Color.LawnGreen,
                    FlashTextIncorrect = Color.DarkRed,
                    PausedMenuTextSelected = Color.DarkRed,
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
