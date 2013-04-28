using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace EquationFinder.Helpers
{
    public static class InputHelpers
    {

        public static GamePadState GetGamePadStateForAllPLayers()
        {

            for (int i = 0; i < 4; i++)
            {

                var gamePadState = GamePad.GetState((PlayerIndex)i);

                if (gamePadState.IsButtonDown(Buttons.A)
                    || gamePadState.IsButtonDown(Buttons.B)
                    || gamePadState.IsButtonDown(Buttons.DPadDown)
                    || gamePadState.IsButtonDown(Buttons.DPadLeft)
                    || gamePadState.IsButtonDown(Buttons.DPadRight)
                    || gamePadState.IsButtonDown(Buttons.DPadUp)
                    || gamePadState.IsButtonDown(Buttons.LeftShoulder)
                    || gamePadState.IsButtonDown(Buttons.LeftStick)
                    || gamePadState.IsButtonDown(Buttons.LeftThumbstickDown)
                    || gamePadState.IsButtonDown(Buttons.LeftThumbstickLeft)
                    || gamePadState.IsButtonDown(Buttons.LeftThumbstickRight)
                    || gamePadState.IsButtonDown(Buttons.LeftThumbstickUp)
                    || gamePadState.IsButtonDown(Buttons.LeftTrigger)
                    || gamePadState.IsButtonDown(Buttons.RightShoulder)
                    || gamePadState.IsButtonDown(Buttons.RightStick)
                    || gamePadState.IsButtonDown(Buttons.RightThumbstickDown)
                    || gamePadState.IsButtonDown(Buttons.RightThumbstickLeft)
                    || gamePadState.IsButtonDown(Buttons.RightThumbstickRight)
                    || gamePadState.IsButtonDown(Buttons.RightThumbstickUp)
                    || gamePadState.IsButtonDown(Buttons.RightTrigger)
                    || gamePadState.IsButtonDown(Buttons.Start)
                    || gamePadState.IsButtonDown(Buttons.X)
                    || gamePadState.IsButtonDown(Buttons.Y))
                    return gamePadState;

            }

            return new GamePadState();

        }

        public static KeyboardState GetKeyboardStateForAllPLayers()
        {

            for (int i = 0; i < 4; i++)
            {

                var keyboardState = Keyboard.GetState((PlayerIndex)i);
                if (keyboardState.GetPressedKeys().Length > 0)
                    return keyboardState;

            }

            return new KeyboardState();

        }

    }
}
