﻿#region File Description
//-----------------------------------------------------------------------------
// InputManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace RPG
{
    /// <summary>
    /// This class handles all keyboard and gamepad actions in the game.
    /// </summary>
    public static class InputManager
    {
        #region Action Enumeration
        /// <summary>
        /// Readable names of each action.
        /// </summary>
        private static readonly string[] actionNames = 
            {
                "Main Menu",
                "Ok",
                "Back",
                "Move Cursor - Up",
                "Move Cursor - Down",
                "Move Character - Up",
                "Move Character - Down",
                "Move Character - Left",
                "Move Character - Right",
            };

        /// <summary>
        /// Returns the readable name of the given action.
        /// </summary>
        public static string GetActionName(Action action)
        {
            int index = (int)action;

            if ((index < 0) || (index > actionNames.Length))
            {
                throw new ArgumentException("action");
            }

            return actionNames[index];
        }

        #endregion

        #region Support Types

    
        /// <summary>
        /// A combination of gamepad and keyboard keys mapped to a particular action.
        /// </summary>
        public class ActionMap
        {
            /// <summary>
            /// List of Keyboard controls to be mapped to a given action.
            /// </summary>
            public List<Keys> keyboardKeys = new List<Keys>();
        }


        #endregion

        #region Constants


        /// <summary>
        /// The value of an analog control that reads as a "pressed button".
        /// </summary>
        const float analogLimit = 0.5f;


        #endregion

        #region Keyboard Data


        /// <summary>
        /// The state of the keyboard as of the last update.
        /// </summary>
        private static KeyboardState currentKeyboardState;

        /// <summary>
        /// The state of the keyboard as of the last update.
        /// </summary>
        public static KeyboardState CurrentKeyboardState
        {
            get { return currentKeyboardState; }
        }


        /// <summary>
        /// The state of the keyboard as of the previous update.
        /// </summary>
        private static KeyboardState previousKeyboardState;


        /// <summary>
        /// Check if a key is pressed.
        /// </summary>
        public static bool IsKeyPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }


        /// <summary>
        /// Check if a key was just pressed in the most recent update.
        /// </summary>
        public static bool IsKeyTriggered(Keys key)
        {
            return (currentKeyboardState.IsKeyDown(key)) &&
                (!previousKeyboardState.IsKeyDown(key));
        }


        #endregion
      
        #region Action Mapping


        /// <summary>
        /// The action mappings for the game.
        /// </summary>
        private static ActionMap[] actionMaps;


        public static ActionMap[] ActionMaps
        {
            get { return actionMaps; }
        }


        /// <summary>
        /// Reset the action maps to their default values.
        /// </summary>
        private static void ResetActionMaps()
        {
            actionMaps = new ActionMap[(int)Action.TotalActionCount];
           
            actionMaps[(int)Action.MainMenu] = new ActionMap();
            actionMaps[(int)Action.MainMenu].keyboardKeys.Add(
                Keys.Tab);

            actionMaps[(int)Action.Ok] = new ActionMap();
            actionMaps[(int)Action.Ok].keyboardKeys.Add(
                Keys.Enter);

            actionMaps[(int)Action.Back] = new ActionMap();
            actionMaps[(int)Action.Back].keyboardKeys.Add(
                Keys.Escape);
       
            actionMaps[(int)Action.CursorUp] = new ActionMap();
            actionMaps[(int)Action.CursorUp].keyboardKeys.Add(
                Keys.Up);

            actionMaps[(int)Action.CursorDown] = new ActionMap();
            actionMaps[(int)Action.CursorDown].keyboardKeys.Add(
                Keys.Down);

            actionMaps[(int)Action.MoveCharacterUp] = new ActionMap();
            actionMaps[(int)Action.MoveCharacterUp].keyboardKeys.Add(
                Keys.Up);

            actionMaps[(int)Action.MoveCharacterDown] = new ActionMap();
            actionMaps[(int)Action.MoveCharacterDown].keyboardKeys.Add(
                Keys.Down);

            actionMaps[(int)Action.MoveCharacterLeft] = new ActionMap();
            actionMaps[(int)Action.MoveCharacterLeft].keyboardKeys.Add(
                Keys.Left);

            actionMaps[(int)Action.MoveCharacterRight] = new ActionMap();
            actionMaps[(int)Action.MoveCharacterRight].keyboardKeys.Add(
                Keys.Right);
        }


        /// <summary>
        /// Check if an action has been pressed.
        /// </summary>
        public static bool IsActionPressed(Action action)
        {
            return IsActionMapPressed(actionMaps[(int)action]);
        }


        /// <summary>
        /// Check if an action was just performed in the most recent update.
        /// </summary>
        public static bool IsActionTriggered(Action action)
        {
            return IsActionMapTriggered(actionMaps[(int)action]);
        }


        /// <summary>
        /// Check if an action map has been pressed.
        /// </summary>
        private static bool IsActionMapPressed(ActionMap actionMap)
        {
            for (int i = 0; i < actionMap.keyboardKeys.Count; i++)
            {
                if (IsKeyPressed(actionMap.keyboardKeys[i]))
                {
                    return true;
                }
            }
            
            return false;
        }


        /// <summary>
        /// Check if an action map has been triggered this frame.
        /// </summary>
        private static bool IsActionMapTriggered(ActionMap actionMap)
        {
            for (int i = 0; i < actionMap.keyboardKeys.Count; i++)
            {
                if (IsKeyTriggered(actionMap.keyboardKeys[i]))
                {
                    return true;
                }
            }
            
            return false;
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Initializes the default control keys for all actions.
        /// </summary>
        public static void Initialize()
        {
            ResetActionMaps();
        }


        #endregion

        #region Updating


        /// <summary>
        /// Updates the keyboard and gamepad control states.
        /// </summary>
        public static void Update()
        {
            // update the keyboard state
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
        }


        #endregion
    }
}
