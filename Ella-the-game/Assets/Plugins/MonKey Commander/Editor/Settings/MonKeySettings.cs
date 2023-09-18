using System;
using MonKey.Editor.Internal;
using MonKey.Extensions;
using MonKey.Internal;
using MonKey.Settings.Internal;
using UnityEditor;
using UnityEngine;

public class MonKeySettings : Editor
{
    public static readonly string defaultMonKeyInstallFolder = "Assets/Plugins/MonKey Commander/Editor";

    public static MonKeySettings Instance
    {
        get { return !instance ? InitSettings() : instance; }
    }

    private static MonKeySettings instance;

    public static MonKeySettings InitSettings()
    {
        string[] settingsPaths = AssetDatabase.FindAssets("t:MonKeySettings");
        if (settingsPaths.Length == 0)
        {
            return CreateNewInstance();
        }

        if (settingsPaths.Length > 1)
        {
            Debug.LogWarning(
                "MonKey Warning: More than one MonKey Settings were found: this is not allowed, please leave only one");
        }

        instance = AssetDatabase.LoadAssetAtPath<MonKeySettings>(
            AssetDatabase.GUIDToAssetPath(settingsPaths[0]));

        if (!instance)
        {
            AssetDatabase.DeleteAsset(defaultMonKeyInstallFolder + "/Settings/MonKey Settings.asset");
            return CreateNewInstance();
        }

        SavePrefs();

        CommandManager.FindInstance();
        return instance;
    }

    private static MonKeySettings CreateNewInstance()
    {
        if (!AssetDatabase.IsValidFolder(defaultMonKeyInstallFolder))
            AssetDatabase.CreateFolder("Assets", "/Plugins/MonKey Commander/Editor/Settings");

        instance = CreateInstance<MonKeySettings>();

        AssetDatabase.CreateAsset(instance, defaultMonKeyInstallFolder + "/Settings/MonKey Settings.asset");
        AssetDatabase.SaveAssets();
        SavePrefs();
        return instance;
    }
	

#if UNITY_2018_3_OR_NEWER

    private class SettingsWindow : EditorWindow
    {
        [MenuItem("Tools/MonKey Commander/Settings")]
        public static void Settings()
        {
            SettingsWindow window = GetWindow<SettingsWindow>();
            window.Show();
        }

        private void OnGUI()
		{
			Instance.SetStyle();
            Instance.CheckOptions();
            Instance.SaveChanges();
		}
	}

#else
    [PreferenceItem("Monkey\nCommander")]

    public static void PreferencesGUI()
    {
        Instance.SetStyle();
        Instance.CheckOptions();
        Instance.SaveChanges();
    }
#endif
	
    private void SetStyle()
	{
		Instance.titleStyle = new GUIStyle(EditorStyles.largeLabel)
		{
			richText = true,
			alignment = TextAnchor.MiddleCenter,
			margin = new RectOffset(0, 0, 15, 5),                
		};

        Instance.titleStyle.normal.textColor = Color.black;

        GUI.contentColor = Color.white;
		GUI.color = Color.white;
		GUI.backgroundColor = Color.white;
	}
	
    private void CheckOptions()
	{
        scrollVector = EditorGUILayout.BeginScrollView(scrollVector);

        Instance.CheckHotKeyOptions();
		Instance.CheckPreferencesOptions();
		Instance.CheckMenuItemInclusion();
		Instance.CheckAssemblyInclusion();
		Instance.CheckNameSpaceInclusion();
		Instance.CheckSearchOptions();
		Instance.CheckPerformanceOptions();

        EditorGUILayout.EndScrollView();
	}
    

    private void CheckHotKeyOptions()
    {
        DrawHeader("Custom Monkey Console Toggle HotKey");
        DrawKeyCodeField();
        DrawKeyCodeHints();
    }

    private void DrawKeyCodeField()
    {
        EditorGUILayout.BeginHorizontal();

        // text based input
        MonkeyConsoleOverrideHotKey = GetCorrectKeyCodeString(MonkeyConsoleOverrideHotKey);

        MonkeyConsoleOverrideHotKey = EditorGUILayout.TextField(MonkeyConsoleOverrideHotKey);

        // Check, if keycode is a valid key
        KeyCode keycode;
        bool keyIsValid = Enum.TryParse(MonkeyConsoleOverrideHotKey.ToUpperInvariant(), out keycode);

        if (keyIsValid == false)
            keycode = KeyCode.None;

        // Enum select input
        KeyCode newKey = (KeyCode)EditorGUILayout.EnumPopup(keycode);

        // save
        SaveKeyCodeAsString(newKey);

        EditorGUILayout.EndHorizontal();
    }

	private string GetCorrectKeyCodeString(string keycode)
	{
		if (keycode.Equals(""))
			return "None";

        return keycode;
	}
	
    private void SaveKeyCodeAsString(KeyCode newKey)
	{
		if (newKey == KeyCode.None)
			MonkeyConsoleOverrideHotKey = "";
		else
			MonkeyConsoleOverrideHotKey = Enum.GetName(typeof(KeyCode), newKey);
	}
    
    private void DrawKeyCodeHints()
    {
        EditorGUILayout.HelpBox("Only single keys are supported.", MessageType.None);

        if (!MonkeyConsoleOverrideHotKey.IsNullOrEmpty())
        {
            EditorGUILayout.HelpBox("Using a custom hotkey will make the default hotkeys not work anymore:" +
                            " make sure you chose a convenient key!", MessageType.None);
        }
    }

    private void CheckPreferencesOptions()
    {
        DrawHeader("Preferences");
        PauseGameOnConsoleOpen = DrawToggle(MonKeyLocManager.CurrentLoc.PauseOnUsage, PauseGameOnConsoleOpen);
        ForceFocusOnDocked = DrawToggle("Force Focus In Dock Mode", ForceFocusOnDocked);
        ShowHelpOnSelectedOnly = DrawToggle("Show Command Help Only On Selected Command", ShowHelpOnSelectedOnly);
    }

    private void CheckMenuItemInclusion()
    {
        DrawHeader("Command Search Inclusion");
        IncludeMenuItems = DrawToggle(MonKeyLocManager.CurrentLoc.IncludeMenuItems, IncludeMenuItems);

        if (IncludeMenuItems)
        {
            EditorGUI.indentLevel++;
            IncludeOnlyMenuItemsWithHotKeys = DrawToggle(MonKeyLocManager.CurrentLoc.OnlyMenuItemsWithHotKeys, IncludeOnlyMenuItemsWithHotKeys);
            EditorGUI.indentLevel--;
        }
    }

	private void CheckAssemblyInclusion()
    {
        OnlyScanSpecified = DrawToggle("Limit To Selected Assemblies", OnlyScanSpecified);

        if (GUILayout.Button("Print List Of Assemblies"))
        {
            foreach (var assembly in  AppDomain.CurrentDomain.GetAssemblies())
            {
                Debug.Log(assembly.FullName);
            }
        }
        
        GUILayout.Label(OnlyScanSpecified ? "Assemblies To Scan (separated by ; )" : "Assemblies To Exclude (separated by ; )");
        ExcludedAssemblies = GUILayout.TextArea(ExcludedAssemblies);
    }

    private void CheckNameSpaceInclusion()
    {
        GUILayout.Label("Excluded Namespaces");
        ExcludedNameSpaces = GUILayout.TextArea(ExcludedNameSpaces);
    }

    private void CheckSearchOptions()
    {
        DrawHeader("Search Options");
        PutInvalidCommandAtEndOfSearch = DrawToggle(MonKeyLocManager.CurrentLoc.PutInvalidAtEnd, PutInvalidCommandAtEndOfSearch);
    }
    
    private void CheckPerformanceOptions()
	{
		DrawHeader("Performances");

		UseSortedSelection = DrawToggle("Use Sorted Selection", UseSortedSelection);

		if (!UseSortedSelection)
		{
			ShowSortedSelectionWarning = DrawToggle("Show Warning In Log On Sort Sensitive Command", ShowSortedSelectionWarning);
		}
		else
		{
			GUIContent content = new GUIContent("Maximum Sorted Objects", "To avoid Editor Freeze");
			MaxSortedSelectionSize = EditorGUILayout.IntField(content, MaxSortedSelectionSize);
		}
	}


    private void DrawHeader(string text)
    {
        GUILayout.Label(text.Bold(),new GUIStyle(GUI.skin.label){richText = true,alignment = TextAnchor.MiddleCenter});
    }

    private bool DrawToggle(string label, bool value)
	{
		return EditorGUILayout.ToggleLeft(label, value);
	}


    private void SaveChanges()
    {
        if (GUI.changed)
        {
            SavePrefs();
            EditorUtility.SetDirty(Instance);
            AssetDatabase.SaveAssets();
        }
    }

    private static void SavePrefs()
    {
        MonKeyInternalSettings internalSettings = MonKeyInternalSettings.Instance;

        if (!internalSettings)
            return;

        internalSettings.UseSortedSelection = instance.UseSortedSelection;
        internalSettings.MaxSortedSelectionSize = instance.MaxSortedSelectionSize;
        internalSettings.ShowSortedSelectionWarning = instance.ShowSortedSelectionWarning;
        internalSettings.MonkeyConsoleOverrideHotKey = instance.MonkeyConsoleOverrideHotKey;
        internalSettings.PauseGameOnConsoleOpen = instance.PauseGameOnConsoleOpen;
        internalSettings.PutInvalidCommandAtEndOfSearch = instance.PutInvalidCommandAtEndOfSearch;
        internalSettings.IncludeMenuItems = instance.IncludeMenuItems;
        internalSettings.IncludeOnlyMenuItemsWithHotKeys = instance.IncludeOnlyMenuItemsWithHotKeys;
        internalSettings.ExcludedAssemblies = instance.ExcludedAssemblies;
        internalSettings.ExcludedNameSpaces = instance.ExcludedNameSpaces;
        internalSettings.ForceFocusOnDocked = instance.ForceFocusOnDocked;
        internalSettings.ShowHelpOnlyOnActiveCommand = instance.ShowHelpOnSelectedOnly;
        internalSettings.OnlyScanSpecified = instance.OnlyScanSpecified;
        internalSettings.PostSave();
    }

    [HideInInspector] public bool UseSortedSelection = true;
    [HideInInspector] public int MaxSortedSelectionSize = 1000;
    [HideInInspector] public bool ShowSortedSelectionWarning = true;

    [HideInInspector] public string MonkeyConsoleOverrideHotKey = "";

    public bool UseCustomConsoleKey
    {
        get { return !MonkeyConsoleOverrideHotKey.IsNullOrEmpty(); }
    }

    [HideInInspector] public bool PauseGameOnConsoleOpen = true;

    [HideInInspector] public bool PutInvalidCommandAtEndOfSearch = false;

    [HideInInspector] public bool IncludeMenuItems = true;
    [HideInInspector] public bool IncludeOnlyMenuItemsWithHotKeys = false;

    [HideInInspector] public bool OnlyScanSpecified = false;

    [HideInInspector] public string ExcludedAssemblies = "";
    [HideInInspector] public string ExcludedNameSpaces = "";

    [HideInInspector] public bool ForceFocusOnDocked = false;

    [HideInInspector] public bool ShowHelpOnSelectedOnly = false;


    private GUIStyle titleStyle;
    private Vector2 scrollVector;
}