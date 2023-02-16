using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace assignment_4
{
    /// <summary>
    /// This class is designed to help us consolidate a lot of code
    /// we need for reading in the input from the user.  We are taking
    /// code that existed in earlier implementations and refactoring it
    /// into individual methods that we can call for info.
    /// 
    /// Tommy Thompson 21/12/12
    /// </summary>
    public class InputHandler
    {
        //Need to keep reference of this for using the GamePad
        PlayerIndex player;

        //We use the Keys enum that is part of the Xna Input library, so we can set what keys to watch and change if we want to.
        const Keys upKey = Keys.Up,
                    downKey = Keys.Down,
                    leftKey = Keys.Left,
                    rightKey = Keys.Right;
                    

        //Information about the state of the game pad.
        GamePadState currentPadState, previousPadState;
        KeyboardState currentKBState, previousKBState;
        bool gamePadConnected;
        Vector2 leftStick;

        /// <summary>
        /// Create an instance by taking the index of the 
        /// game pad we want to look out for.
        /// </summary>
        /// <param name="inputForPlayer"></param>
        public InputHandler(PlayerIndex inputForPlayer)
        {
            player = inputForPlayer;
            UpdateInput();
        }

        /// <summary>
        /// Updates the current state of the game pad.
        /// We can grab the keyboard whenever we like.
        /// </summary>
        public void UpdateInput()
        {
            previousPadState = currentPadState;
            previousKBState = currentKBState;
            currentPadState = GamePad.GetState(player);
            currentKBState = Keyboard.GetState();
            gamePadConnected = currentPadState.IsConnected;
            leftStick = currentPadState.ThumbSticks.Left;
        }

        /// <summary>
        /// Grab the current value of the left stick.
        /// </summary>
        public Vector2 LeftStick
        {
            get
            {
                return leftStick;
            }
        }

        /// <summary>
        /// Determines whether the gamepad this input handler
        /// is responsible for is in fact active.
        /// </summary>
        /// <returns>True/false if this pad is active.</returns>
        public bool GamePadActive()
        {
            return gamePadConnected;
        }

        public bool IsButtonPressed(Buttons button)
        {
            return previousPadState.IsButtonUp(button) && currentPadState.IsButtonDown(button);
        }

        public bool IsKeyPressed(Keys key)
        {
            return previousKBState.IsKeyUp(key) && currentKBState.IsKeyDown(key);
        }

        public bool IsKeyTapped(Keys key)
        {
            return previousKBState.IsKeyDown(key) && currentKBState.IsKeyUp(key);
        }

        /// <summary>
        /// Dertermines whether left is pressed.
        /// </summary>
        /// <returns>True if left pressed.</returns>
        public bool IsLeftPressed()
        {
            if (gamePadConnected)
            {
                return currentKBState.IsKeyDown(Keys.Left) || leftStick.X < 0;
            }
            else
            {
                return currentKBState.IsKeyDown(leftKey);
            }
        }

        public bool IsPlayer2LeftPressed()
        {
            if (gamePadConnected)
            {
                return currentKBState.IsKeyDown(Keys.Left) || leftStick.X < 0;
            }
            else
            {
                return currentKBState.IsKeyDown(Keys.A);
            }
        }

        /// <summary>
        /// Dertermines whether right is pressed.
        /// </summary>
        /// <returns>True if right pressed.</returns>
        public bool IsRightPressed()
        {
            if (gamePadConnected)
            {
                return currentKBState.IsKeyDown(rightKey) || leftStick.X > 0;
            }
            else
            {
                return currentKBState.IsKeyDown(rightKey);
            }
        }

        public bool IsPlayer2RightPressed()
        {
            if (gamePadConnected)
            {
                return currentKBState.IsKeyDown(rightKey) || leftStick.X > 0;
            }
            else
            {
                return currentKBState.IsKeyDown(Keys.D);
            }
        }

        /// <summary>
        /// Dertermines whether down is pressed.
        /// </summary>
        /// <returns>True if down pressed.</returns>
        public bool IsDownPressed()
        {
            if (gamePadConnected)
            {
                return currentKBState.IsKeyDown(downKey) || leftStick.Y < 0;
            }
            else
            {
                return currentKBState.IsKeyDown(downKey);
            }
        }

        public bool IsPlayer2DownPressed()
        {
            if (gamePadConnected)
            {
                return currentKBState.IsKeyDown(downKey) || leftStick.Y < 0;
            }
            else
            {
                return currentKBState.IsKeyDown(Keys.S);
            }
        }

        /// <summary>
        /// Dertermines whether up is pressed.
        /// </summary>
        /// <returns>True if up pressed.</returns>
        public bool IsUpPressed()
        {
            if (gamePadConnected)
            {
                return currentKBState.IsKeyDown(upKey) || leftStick.Y > 0;
            }
            else
            {
                return currentKBState.IsKeyDown(upKey);
            }
        }

        public bool IsPlayer2UpPressed()
        {
            if (gamePadConnected)
            {
                return currentKBState.IsKeyDown(upKey) || leftStick.Y > 0;
            }
            else
            {
                return currentKBState.IsKeyDown(Keys.W);
            }
        }
    }
}
