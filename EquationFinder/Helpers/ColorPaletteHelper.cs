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
                    FlashTextIncorrect = Color.Violet,
                    PausedMenuTextSelected = Color.Violet,
                    PausedMenuTextUnselected = Color.Black
                };
            }
            else if (GameplayOptions.BackgroundImage == "Cornflower_Blue")
            {
                return new ColorPalette()
                {
                    BoardText = Color.Blue,
                    BoardTextSelected = Color.Red,
                    BoardTextPreviouslySelected = Color.DarkRed,
                    DisplayText = Color.White,
                    FlashTextCorrect = Color.Yellow,
                    FlashTextIncorrect = Color.Violet,
                    PausedMenuTextSelected = Color.Red,
                    PausedMenuTextUnselected = Color.Black
                };
            }
            else if (GameplayOptions.BackgroundImage == "Ghost_White")
            {
                return new ColorPalette()
                {
                    BoardText = Color.Black,
                    BoardTextSelected = Color.Yellow,
                    BoardTextPreviouslySelected = Color.DarkOrange,
                    DisplayText = Color.Black,
                    FlashTextCorrect = Color.LawnGreen,
                    FlashTextIncorrect = Color.Red,
                    PausedMenuTextSelected = Color.Yellow,
                    PausedMenuTextUnselected = Color.Black
                };
            }
            else if (GameplayOptions.BackgroundImage == "Hulk_Green")
            {
                return new ColorPalette()
                {
                    BoardText = new Color(6, 21, 57),
                    BoardTextSelected = new Color(139, 86, 168),
                    BoardTextPreviouslySelected = new Color(255, 161, 78),
                    DisplayText = new Color(6, 21, 57),
                    FlashTextCorrect = new Color(172, 120, 184),
                    FlashTextIncorrect = Color.Red,
                    PausedMenuTextSelected = new Color(139, 86, 168),
                    PausedMenuTextUnselected = Color.Black
                };
            }
            else if (GameplayOptions.BackgroundImage == "Joker_Purple")
            {
                return new ColorPalette()
                {
                    BoardText = new Color(255, 105, 61),
                    BoardTextSelected = Color.Blue,
                    BoardTextPreviouslySelected = new Color(34, 102, 102),
                    DisplayText = new Color(255, 105, 61),
                    FlashTextCorrect = new Color(105, 187, 1),
                    FlashTextIncorrect = Color.DarkOrchid,
                    PausedMenuTextSelected = Color.Red,
                    PausedMenuTextUnselected = Color.White
                };
            }
            else if (GameplayOptions.BackgroundImage == "Midnight_Black")
            {
                return new ColorPalette()
                {
                    BoardText = Color.Gold,
                    BoardTextSelected = Color.White,
                    BoardTextPreviouslySelected = Color.LightGray,
                    DisplayText = Color.Gold,
                    FlashTextCorrect = Color.LawnGreen,
                    FlashTextIncorrect = Color.Red,
                    PausedMenuTextSelected = Color.Gold,
                    PausedMenuTextUnselected = Color.White
                };
            }
            else
            {
                throw new NotImplementedException();
            }

        }

    }
}
