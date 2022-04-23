using System;

namespace MonKey
{
    public enum DefaultValidationMessages
    {
        DEFAULT_SELECTED_OBJECTS,
        DEFAULT_SELECTED_GAMEOBJECTS,
        DEFAULT_PLAY_MODE_ONLY,
        DEFAULT_EDIT_MODE_ONLY
    }

    /// <summary>
    /// Put the attribute on a validation method to 
    /// customize the validation message that will appear in the MonKey console.
    /// </summary>
    public class CommandValidation : Attribute
    {

        public static readonly string DefaultSelectObjects = "Select some Objects first";
        public static readonly string DefaultSelectGameObjects = "Select some gameObjects first";
        public static readonly string DefaultPlayModeOnly = "Enter Play mode first";
        public static readonly string DefaultEditModeOnly = "Enter Edit mode first";

        /// <summary>
        /// The help message that will indicate to the user
        /// the reason why the command is not available
        /// </summary>
        public string InvalidCommandMessage;

        public CommandValidation()
        {
        }

        public CommandValidation(string invalidCommandMessage)
        {
            InvalidCommandMessage = invalidCommandMessage;
        }

        public CommandValidation(DefaultValidationMessages validationMessage)
        {
            InvalidCommandMessage = GetValidationMessage(validationMessage);
        }

        private string GetValidationMessage(DefaultValidationMessages validationMessage)
        {
            switch (validationMessage)
            {
                case DefaultValidationMessages.DEFAULT_SELECTED_OBJECTS:
                    return DefaultSelectObjects;
                case DefaultValidationMessages.DEFAULT_SELECTED_GAMEOBJECTS:
                    return DefaultSelectGameObjects;
                case DefaultValidationMessages.DEFAULT_PLAY_MODE_ONLY:
                    return DefaultPlayModeOnly;
                case DefaultValidationMessages.DEFAULT_EDIT_MODE_ONLY:
                    return DefaultEditModeOnly;
                default:
                    throw new ArgumentOutOfRangeException("validationMessage", validationMessage, null);
            }
        }

    }
}


