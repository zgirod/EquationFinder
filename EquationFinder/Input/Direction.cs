using System;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace EquationFinder.Input
{
    /// <summary>
    /// Helper class for working with the 8-way directions stored in a Buttons enum.
    /// </summary>
    static class Direction
    {
        // Helper bit masks for directions defined with the Buttons flags enum.
        public const Buttons None = 0;
        //public const Buttons Up = Buttons.DPadUp | Buttons.LeftThumbstickUp;
        //public const Buttons Down = Buttons.DPadDown | Buttons.LeftThumbstickDown;
        //public const Buttons Left = Buttons.DPadLeft | Buttons.LeftThumbstickLeft;
        //public const Buttons Right = Buttons.DPadRight | Buttons.LeftThumbstickRight;
        public const Buttons Up = Buttons.DPadUp;
        public const Buttons Down = Buttons.DPadDown;
        public const Buttons Left = Buttons.DPadLeft;
        public const Buttons Right = Buttons.DPadRight;
        public const Buttons UpLeft = Up | Left;
        public const Buttons UpRight = Up | Right;
        public const Buttons DownLeft = Down | Left;
        public const Buttons DownRight = Down | Right;
        public const Buttons Any = Up | Down | Left | Right;

        /// <summary>
        /// Gets the current direction from a game pad and keyboard.
        /// </summary>
        public static Buttons FromInput(GamePadState gamePad, 
            KeyboardState keyboard,
            GamePadState oldGamePad)
        {

            //if we don't have an old game pad state, just return null
            Buttons direction = None;
            if (oldGamePad == null)
                return direction;

            // Get vertical direction.
            if ((gamePad.DPad.Up == ButtonState.Pressed && oldGamePad.DPad.Up == ButtonState.Released)
                || (gamePad.ThumbSticks.Left.Y >= .24f && oldGamePad.ThumbSticks.Left.Y <= .24f)
                || keyboard.IsKeyDown(Keys.Up))
            {
                direction |= Up;
            }
            else if ((gamePad.DPad.Down == ButtonState.Pressed && oldGamePad.DPad.Down == ButtonState.Released)
                || (gamePad.ThumbSticks.Left.Y <= -0.24f && oldGamePad.ThumbSticks.Left.Y >= -0.24f)
                || keyboard.IsKeyDown(Keys.Down))
            {
                direction |= Down;
            }

            // Comebine with horizontal direction.
            if ((gamePad.DPad.Left == ButtonState.Pressed && oldGamePad.DPad.Left == ButtonState.Released)
                || (gamePad.ThumbSticks.Left.X <= -0.24f && oldGamePad.ThumbSticks.Left.X >= -0.24f)
                || keyboard.IsKeyDown(Keys.Left))
            {
                direction |= Left;
            }
            else if ((gamePad.DPad.Right == ButtonState.Pressed && oldGamePad.DPad.Right == ButtonState.Released)
                || (gamePad.ThumbSticks.Left.X >= 0.24f && oldGamePad.ThumbSticks.Left.X <= 0.24f)
                || keyboard.IsKeyDown(Keys.Right))
            {
                direction |= Right;
            }

            return direction;
        }

        /// <summary>
        /// Gets the direction without non-direction buttons from a set of Buttons flags.
        /// </summary>
        public static Buttons FromButtons(Buttons buttons)
        {
            // Extract the direction from a full set of buttons using a bit mask.
            return buttons & Any;
        }
    }
}
