using System;
using UnityEngine;

namespace MonKey
{
    public enum DefaultValidation
    {
        NONE,
        AT_LEAST_ONE_GAME_OBJECT,
        AT_LEAST_TWO_GAME_OBJECTS,
        AT_LEAST_ONE_OBJECT,
        AT_LEAST_TWO_OBJECTS,
        AT_LEAST_ONE_TRANSFORM,
        AT_LEAST_TWO_TRANSFORMS,
        IN_PLAY_MODE,
        IN_EDIT_MODE,
        IN_PLAY_MODE_AT_LEAST_ONE_GAME_OBJECT,
        IN_PLAY_MODE_AT_LEAST_TWO_GAME_OBJECTS,
        IN_PLAY_MODE_AT_LEAST_ONE_OBJECT,
        IN_PLAY_MODE_AT_LEAST_TWO_OBJECTS,
        IN_PLAY_MODE_AT_LEAST_ONE_TRANSFORM,
        IN_PLAY_MODE_AT_LEAST_TWO_TRANSFORMS,
        IN_EDIT_MODE_AT_LEAST_ONE_GAME_OBJECT,
        IN_EDIT_MODE_AT_LEAST_TWO_GAME_OBJECTS,
        IN_EDIT_MODE_AT_LEAST_ONE_OBJECT,
        IN_EDIT_MODE_AT_LEAST_TWO_OBJECTS,
        IN_EDIT_MODE_AT_LEAST_ONE_TRANSFORM,
        IN_EDIT_MODE_AT_LEAST_TWO_TRANSFORMS,
        AT_LEAST_ONE_ASSET,
        AT_LEAST_ONE_SCENE_OBJECT,
        AT_LEAST_ONE_FOLDER
    }

   /* [Flags]
    public enum CommandContext
    {
        HIERARCHY,
        SCENE,
        NONE
    }*/

    /// <summary>
    /// Add this attribute to a static method in an class
    ///  file in an editor folder, and it will be added to MonKey's list of commands!
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class Command : Attribute
    {
        /// <summary>
        /// Should the command be visible even when no search terms were entered
        /// </summary>
        public bool AlwaysShow = false;

        /// <summary>
        /// The name with which the command can be searched
        /// </summary>
        public string Name;

        /// <summary>
        /// The quick name with which the command can be searched (make it short to quickly access the command)
        /// </summary>
        public string QuickName;

        /// <summary>
        /// Some help to display for the user to understand the command
        /// </summary>
        public string Help;

        /// <summary>
        /// The secondary sorting order: the smaller the number, 
        /// the higher the placement (after the search terms sorting) 
        /// </summary>
        public int Order=int.MaxValue;

        /// <summary>
        /// If you want to use one of the default ways to validate the command, 
        /// select one here ( will get overridden by "ValidationMethodName")
        /// </summary>
        public DefaultValidation DefaultValidation;

        /// <summary>
        /// The name of the Validation Method that indicates 
        /// if the command is currently available or not. Must return a boolean
        /// </summary>
        public string ValidationMethodName;

        /// <summary>
        /// Set true if you want to ignore the fact that 
        /// the menu item hotkey is going to conflict with another menu item
        /// </summary>
        public bool IgnoreHotKeyConflict = false;

        /// <summary>
        /// If you want the command to be associated to a menu item 
        /// (essentially for hotkeys) without this menu item being added, specify the name 
        /// of the command that has a MenuItemLink attribute here
        /// </summary>
        public string MenuItemLink;

        /// <summary>
        /// If the MenuItemLink is in another type, enter the type's name here
        /// </summary>
        public string MenuItemLinkTypeOwner;

        /// <summary>
        /// The category of the commands to help you browse it
        /// </summary>
        public string Category="My Commands";

      //  public bool IsSceneContextual = false;

        public Command(string name)
        {
            Name = name;
        }

        public Command(string name, string help)
        {
            Name = name;
            Help = help;
        }
    }
}