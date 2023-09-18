using MonKey.Settings.Internal;
using UnityEditor;
using UnityEngine;

namespace MonKey.Internal
{
    [InitializeOnLoad]
    public class MonkeyLocalizationFile : EditorSingleton<MonkeyLocalizationFile>, IMonKeySingleton
    {
        static MonkeyLocalizationFile()
        {
            SessionID = "MC_LocFile";
        }

        public string LanguageID = "EN";

        public string Language = "Language";

        public string IncludeMenuItems = "Include Menu Items In Searchs";

        public string OnlyMenuItemsWithHotKeys = "Only Menu Items With Hot Keys";

        public static string ControlKey = "CTRL";

        public static string CommandKey = "CMD";
        private static bool IsMacKeys
        {
            get { return Application.platform == RuntimePlatform.OSXEditor; }
        }
        public static string SysIndependentControl
        {
            get
            {
                return (IsMacKeys) ? CommandKey : ControlKey;
            }
        }

        public KeyCode SysIndependentControlKey
        {
            get
            {
                return (IsMacKeys) ? KeyCode.LeftCommand : KeyCode.LeftControl;
            }
        }

        public KeyCode SysIndependentControlKeyRight
        {
            get
            {
                return (IsMacKeys) ? KeyCode.RightCommand : KeyCode.RightControl;
            }
        }


        public string ShiftKey = "SHIFT";

        public string AltKey = "ALT";

        public string CommandSearchLabel = "Start to type a command name";

        public string ParametricCommandLabel = "Command: ";
        public string ParametersLabel = "Parameters: ";
        public string ParametricSearchLabel = "Enter a value for the parameter ";


        public string HelpLabel = "Press ENTER Or Click To " +
                                  "Execute The Selected Command, ESCAPE to exit";

        public string NotFocusedHelpLabel = "MonKey's window is not focused: Click on it!";


        public string LoadingLabel = "MonKey is looking for commands! | ";

        public string EndLoadingLabel = "Assemblies Checked:";

        public string NoResultsFoundLabel = "No Commands Found";

        public string NoAutoCompleteFoundLabel = "No Auto-Complete values for the parameter";

        public string CommandValidationFailed = "Command Not Available!";

        public string PreLoadingNotice = "Loading Assemblies...";

        public string NoHotKey = "";

        public string ConflictWith = "Conflict With: ";

        public string PutInvalidAtEnd = "Put Invalid Commands at the end of the search";

        public string PauseOnUsage = "Pause Play Mode when Monkey opens";

        public string ParametricHelp = "Press " + SysIndependentControl + " + " +
                                       "ENTER to execute with default parameters";

        public string Value = "Values: ";

        public string MonkeySleeping = "MonKey is sleeping!";
        public string WakeUp = "Click Here or use the HotKey to wake him up!";

        public string Parameters = "Command Parameters:";

        public string NoValue = "No Value";

        public string Default = "Default";
        public string Error = "Error";
        public string Type = "Type: ";
        public string CurrentValue = "Value: ";
        public string CurrentArrayValues = "Array IDs: ";

        public string MenuItemAddedByMonkey = "Unity Menu Item Added By Monkey";
        public string TabForNext = " or press TAB for next param.";

        public void PostInstanceCreation()
        {
        }
    }
}
