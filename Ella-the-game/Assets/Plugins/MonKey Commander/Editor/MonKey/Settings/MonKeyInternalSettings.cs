using MonKey.Editor.Internal;
using MonKey.Extensions;
using MonKey.Internal;
using UnityEditor;
using UnityEngine;

namespace MonKey.Settings.Internal
{
    [InitializeOnLoad]
    public class MonKeyInternalSettings : EditorSingleton<MonKeyInternalSettings>,IMonKeySingleton
    {

        static MonKeyInternalSettings()
        {
            SessionID = "MC_InternalSettings";
        }

        public static readonly string DefaultMonKeyInstallFolder
            = "Assets/Plugins/MonKey Commander/Editor";

        [HideInInspector]
        public bool UseSortedSelection = true;
        [HideInInspector]
        public int MaxSortedSelectionSize = 300;

        [HideInInspector]
        public bool ShowSortedSelectionWarning = true;

        [HideInInspector]
        public string MonkeyConsoleOverrideHotKey = "";

        public bool UseCustomConsoleKey => !MonkeyConsoleOverrideHotKey.IsNullOrEmpty();

        [HideInInspector]
        public bool PauseGameOnConsoleOpen = true;

        [HideInInspector]
        public bool PutInvalidCommandAtEndOfSearch = false;

        [HideInInspector]
        public bool IncludeMenuItems = true;
        [HideInInspector]
        public bool IncludeOnlyMenuItemsWithHotKeys = false;

        [HideInInspector]
        public bool OnlyScanSpecified;

        [HideInInspector]
        public string ExcludedAssemblies = "";
        [HideInInspector]
        public string ExcludedNameSpaces = "";

        [HideInInspector]
        public bool ForceFocusOnDocked = false;

        [HideInInspector]
        public bool UseCategoryMode = false;

        [HideInInspector]
        public bool PreventFocusOnPopup = true;

        [HideInInspector]
        public bool ShowHelpOnlyOnActiveCommand = false;

        [HideInInspector]
        public bool UseAdvancedFuzzySearch = true;

        [HideInInspector]
        public bool UseSceneCommandAsEditorWindow = true;

        public void PostSave()
        {
            if (!Instance.MonkeyConsoleOverrideHotKey.IsNullOrEmpty())
                HotKeysManager.CustomMonkeyConsoleCall = new KeyCombination(Instance.MonkeyConsoleOverrideHotKey);

            if (!CommandManager.Instance.IsLoading)
            {
                CommandManager.Instance.OnEnable();
            }
        }

        public void PostInstanceCreation()
        {       
        }
    }
}
