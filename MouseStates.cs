using Microsoft.Xna.Framework.Input;
using System;
using System.Transactions;

namespace pizza_spy
{
    class MouseClicks
    {
        static MouseState currentMouseState, previousMouseState;


        public MouseClicks()
        {

        }

        public static MouseState GetState()
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            return currentMouseState;

        }

        public static bool IsPressed(bool left)
        {
            if (left)

                return currentMouseState.LeftButton == ButtonState.Pressed;


            else

                return currentMouseState.RightButton == ButtonState.Pressed;

        }

        public static bool HasNotBeenPressed(bool left)
        {
            if (left)

                return currentMouseState.LeftButton == ButtonState.Pressed && !(previousMouseState.LeftButton == ButtonState.Pressed);

            else

                return currentMouseState.RightButton == ButtonState.Pressed && !(previousMouseState.RightButton == ButtonState.Pressed);

        }
    }
}