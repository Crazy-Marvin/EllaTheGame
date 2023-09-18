using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_ANDROID && EM_GPGS
using System.Reflection;
using GooglePlayGames.Editor;
#endif

namespace EasyMobile.Editor
{
    // Partial editor class for GameService module.
    internal partial class EM_SettingsEditor
    {
        const string GameServiceModuleLabel = "GAME SERVICES";
        const string GameServiceModuleIntro = "The Game Services module streamlines the integration of Game Center (iOS) and Google Play Games Services (Android) into your game.";
        const string GameServiceManualInitInstruction = "You can initialize manually from script by calling the GameServices.ManagedInit() or GameServices.Init() method.";
        const string AndroidGPGSImportInstruction = "Google Play Games plugin is required. Please download and import it to use this module on Android.";
        const string AndroidGPGSAvailMsg = "Google Play Games plugin is imported and ready to use.";
        const string AndroidGPGPSSetupInstruction = "Paste in the Android XML Resources from the Play Console and hit the Setup button.";
        const string AndroidGPGSMultiplayerDeprecatedMsg = "The Play Games Services multiplayer APIs have been deprecated since Mar 31, 2020 :(";
        const string GameServiceConstantGenerationIntro = "Generate the static class " + EM_Constants.RootNameSpace + "." + EM_Constants.GameServicesConstantsClassName + " that contains the constants of leaderboard and achievement names." +
                                                          " Remember to regenerate if you make changes to these names.";

        // GameServiceItem property names.
        const string GameServiceItem_NameProperty = "_name";
        const string GameServiceItem_IOSIdProperty = "_iosId";
        const string GameServiceItem_AndroidIdProperty = "_androidId";

#if !UNITY_ANDROID || (UNITY_ANDROID && EM_GPGS)
        // GPGS Web client ID.
        static string sGPGSWebClientId;

        // Foldout bools.
        static bool isLeadeboardsFoldout = false;
        static bool isAchievementsFoldout = false;
#endif

        // GPGS generated IDs.
        static string[] gpgsIds;

        // Android resources text area scroll position.
        Vector2 androidResourcesTextAreaScroll;

        static Dictionary<string, bool> gameServiceFoldoutStates = new Dictionary<string, bool>();

        void GameServiceModuleGUI()
        {
            DrawModuleHeader();

            // Now draw the GUI.
            if (!isGameServiceModuleEnable.boolValue)
                return;

#if UNITY_ANDROID && !EM_GPGS
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EM_GUIStyleManager.UppercaseSectionBox);
            EditorGUILayout.HelpBox(AndroidGPGSImportInstruction, MessageType.Error);
            if (GUILayout.Button("Download Google Play Games Plugin", GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
            {
                EM_ExternalPluginManager.DownloadGooglePlayGamesPlugin();
            }
            EditorGUILayout.EndVertical();
#elif UNITY_ANDROID && EM_GPGS
            EditorGUILayout.Space();
            DrawUppercaseSection("GAMESERVICES_DOWNLOAD_PLUGIN_FOLDOUT_KEY", "DOWNLOAD GPGS PLUGIN", () =>
                {
                    EditorGUILayout.HelpBox(AndroidGPGSAvailMsg, MessageType.Info);
                    if (GUILayout.Button("Download Google Play Games Plugin", GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
                    {
                        EM_ExternalPluginManager.DownloadGooglePlayGamesPlugin();
                    }
                });

            // Android Google Play Games setup
            EditorGUILayout.Space();
            DrawUppercaseSection("GOOGLE_PLAY_GAMES_SETUP_FOLDOUT_KEY", "GOOGLE PLAY GAMES SETUP", () =>
                {
                    // GPGPS debug log.
                    EditorGUILayout.PropertyField(GameServiceProperties.gpgsDebugLog.property, GameServiceProperties.gpgsDebugLog.content);

                    // GPGS popup gravity.
                    EditorGUILayout.PropertyField(GameServiceProperties.gpgsPopupGravity.property, GameServiceProperties.gpgsPopupGravity.content);

                    // GPGS request ServerAuthCode config.
                    EditorGUILayout.PropertyField(GameServiceProperties.gpgsShouldRequestServerAuthCode.property, GameServiceProperties.gpgsShouldRequestServerAuthCode.content);
                    EditorGUI.BeginDisabledGroup(!GameServiceProperties.gpgsShouldRequestServerAuthCode.property.boolValue);
                    EditorGUILayout.PropertyField(GameServiceProperties.gpgsForceRefreshServerAuthCode.property, GameServiceProperties.gpgsForceRefreshServerAuthCode.content);
                    EditorGUI.EndDisabledGroup();


                    // GPGS OAuth scopes.
                    EditorGUILayout.PropertyField(GameServiceProperties.gpgsOauthScopes.property, GameServiceProperties.gpgsOauthScopes.content, true);

                    // GPGS (optional) Web App Client ID.
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Web App Client ID (Optional)", EditorStyles.boldLabel);
                    EditorGUILayout.HelpBox("The web app client ID is needed to access the user's ID token and call other APIs on behalf of the user. It is not required for Game Services. " +
                        "Enter your oauth2 client ID below. To obtain this ID, generate a web linked app in Developer Console.\n" +
                        "Example: 123456789012-abcdefghijklm.apps.googleusercontent.com", MessageType.None);
                    sGPGSWebClientId = EditorGUILayout.TextField("Web Client Id", sGPGSWebClientId);

                    // Text area to input the Android Xml resource.
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(GameServiceProperties.gpgsXmlResources.content, EditorStyles.boldLabel);
                    EditorGUILayout.HelpBox(AndroidGPGPSSetupInstruction, MessageType.None);

                    // Draw text area inside a scroll view.
                    androidResourcesTextAreaScroll = GUILayout.BeginScrollView(androidResourcesTextAreaScroll, false, false, GUILayout.Height(EditorGUIUtility.singleLineHeight * 10));
                    GameServiceProperties.gpgsXmlResources.property.stringValue = EditorGUILayout.TextArea(
                        GameServiceProperties.gpgsXmlResources.property.stringValue,
                        GUILayout.ExpandHeight(true));
                    EditorGUILayout.EndScrollView();

                    EditorGUILayout.Space();

                    // Replicate the "Setup" button within the Android GPGS setup window.
                    if (GUILayout.Button("Setup Google Play Games", GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
                    {
                        EditorApplication.delayCall += SetupAndroidGPGSButtonHandler;
                    }
                });
#endif

#if !UNITY_ANDROID || (UNITY_ANDROID && EM_GPGS)
            // Auto-init config
            EditorGUILayout.Space();
            DrawUppercaseSection("GAMESERVICES_AUTO_INIT_CONFIG", "AUTO INITIALIZATION", () =>
                {
                    EditorGUILayout.PropertyField(GameServiceProperties.autoInit.property, GameServiceProperties.autoInit.content);

                    EditorGUI.BeginDisabledGroup(!GameServiceProperties.autoInit.property.boolValue);
                    EditorGUILayout.PropertyField(GameServiceProperties.autoInitAfterUserLogout.property, GameServiceProperties.autoInitAfterUserLogout.content);
                    EditorGUILayout.PropertyField(GameServiceProperties.autoInitDelay.property, GameServiceProperties.autoInitDelay.content);
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.PropertyField(GameServiceProperties.androidMaxLoginRequest.property, GameServiceProperties.androidMaxLoginRequest.content);
                    if (!GameServiceProperties.autoInit.property.boolValue)
                    {
                        EditorGUILayout.HelpBox(GameServiceManualInitInstruction, MessageType.Info);
                    }
                });

            // PRO-only features.
#if EASY_MOBILE_PRO
            // Saved Games config.
            EditorGUILayout.Space();
            DrawUppercaseSection("SAVED_GAMES_CONFIG_FOLDOUT_KEY", "SAVED GAMES", () =>
                {
                    GameServiceProperties.enableSavedGames.property.boolValue = EditorGUILayout.Toggle(GameServiceProperties.enableSavedGames.content, GameServiceProperties.enableSavedGames.property.boolValue);

                    if (GameServiceProperties.enableSavedGames.property.boolValue)
                    {
                        EditorGUILayout.PropertyField(GameServiceProperties.autoConflictResolutionStrategy.property,
                            GameServiceProperties.autoConflictResolutionStrategy.content);

                        EditorGUILayout.PropertyField(GameServiceProperties.gpgsDataSource.property,
                            GameServiceProperties.gpgsDataSource.content);
                    }
                });

            // Multiplayer config.
            EditorGUILayout.Space();
            DrawUppercaseSection("MULTIPLAYER_CONFIG_FOLDOUT_KEY", "MULTIPLAYER", () =>
                {
#if UNITY_ANDROID
                    EditorGUILayout.HelpBox(AndroidGPGSMultiplayerDeprecatedMsg, MessageType.Warning);
                    EditorGUI.BeginDisabledGroup(true);
                    GameServiceProperties.enableMultiplayer.property.boolValue = false;
                    EditorGUILayout.PropertyField(GameServiceProperties.enableMultiplayer.property, GameServiceProperties.enableMultiplayer.content);
                    EditorGUI.EndDisabledGroup();
#else
                    EditorGUILayout.PropertyField(GameServiceProperties.enableMultiplayer.property, GameServiceProperties.enableMultiplayer.content);
#endif
                });
#endif

            // Leaderboard setup.
            EditorGUILayout.Space();
            DrawUppercaseSection("LEADERBOARD_SETUP_FOLDOUT_KEY", "LEADERBOARDS", () =>
                {
                    DrawGameServiceItemArray("Leaderboard", GameServiceProperties.leaderboards, ref isLeadeboardsFoldout);
                });

            // Achievement setup.
            EditorGUILayout.Space();
            DrawUppercaseSection("ACHIVEMENT_SETUP_FOLDOUT_KEY", "ACHIEVEMENTS", () =>
                {
                    DrawGameServiceItemArray("Achievement", GameServiceProperties.achievements, ref isAchievementsFoldout);
                });

            // Constant generation.
            EditorGUILayout.Space();
            DrawUppercaseSection("GAMESERVICES_CONTANTS_GENERATION_FOLDOUT_KEY", "CONSTANTS GENERATION", () =>
                {
                    EditorGUILayout.HelpBox(GameServiceConstantGenerationIntro, MessageType.None);
                    if (GUILayout.Button("Generate Constants Class", GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
                    {
                        GenerateGameServiceConstants();
                    }
                });
#endif
        }

#if UNITY_ANDROID && EM_GPGS
        void SetupAndroidGPGSButtonHandler()
        {
            string webClientId = sGPGSWebClientId;          // Web ClientId, not required for Games Services.
            string folder = EM_Constants.GeneratedFolder;    // Folder to contain the generated id constant class.
            string className = EM_Constants.AndroidGPGSConstantClassName;    // Name of the generated id constant class.
            string resourceXmlData = GameServiceProperties.gpgsXmlResources.property.stringValue;    // The xml resources inputted.
            string nearbySvcId = null;  // Nearby Connection Id, not supported by us.
            bool requiresGooglePlus = false;    // Not required Google+ API.

            try
            {
                if (GPGSUtil.LooksLikeValidPackageName(className))
                {
                    SetupAndroidGPGS(webClientId, folder, className, resourceXmlData, nearbySvcId, requiresGooglePlus);
                }
            }
            catch (System.Exception e)
            {
                GPGSUtil.Alert(
                    GPGSStrings.Error,
                    "Invalid classname: " + e.Message);
            }
        }

        // Replicate the "DoSetup" method of the GPGSAndroidSetupUI class.
        void SetupAndroidGPGS(string webClientId, string folder, string className, string resourceXmlData, string nearbySvcId, bool requiresGooglePlus)
        {
            // Create the folder to store the generated cs file if it doesn't exist.
            FileIO.EnsureFolderExists(folder);

            // Invoke GPGSAndroidSetupUI's PerformSetup method via reflection.
            // In GPGPS versions below 0.9.37, this method has a trailing bool parameter (requiresGooglePlus),
            // while in version 0.9.37 and newer this parameter has been removed. So we need to use reflection
            // to detect the method's parameter list and invoke it accordingly.
            Type gpgsAndroidSetupClass = typeof(GPGSAndroidSetupUI);
            string methodName = "PerformSetup";
            bool isSetupSucceeded = false;

            // GPGS 0.9.37 and newer: PerformSetup has no trailing bool parameter
            MethodInfo newPerformSetup = gpgsAndroidSetupClass.GetMethod(methodName,
                                             BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                                             Type.DefaultBinder,
                                             new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) },
                                             new ParameterModifier[0]);

            if (newPerformSetup != null)
            {
                isSetupSucceeded = (bool)newPerformSetup.Invoke(null, new object[] { webClientId, folder, className, resourceXmlData, nearbySvcId });
            }
            else
            {
                // GPGS 0.9.36 and older: PerformSetup has a trailing bool parameter
                MethodInfo oldPerformSetup = gpgsAndroidSetupClass.GetMethod(methodName,
                                                 BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                                                 Type.DefaultBinder,
                                                 new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(bool) },
                                                 new ParameterModifier[0]);

                if (oldPerformSetup != null)
                {
                    isSetupSucceeded = (bool)oldPerformSetup.Invoke(null, new object[] { webClientId, folder, className, resourceXmlData, nearbySvcId, requiresGooglePlus });
                }
            }

            if (isSetupSucceeded)
            {
                GPGSAndroidSetupUI.CheckBundleId();

                EditorUtility.DisplayDialog(
                    GPGSStrings.Success,
                    GPGSStrings.AndroidSetup.SetupComplete,
                    GPGSStrings.Ok);

                GPGSProjectSettings.Instance.Set(GPGSUtil.ANDROIDSETUPDONEKEY, true);
            }
            else
            {
                GPGSUtil.Alert(
                    GPGSStrings.Error,
                    "Invalid or missing XML resource data.  Make sure the data is" +
                    " valid and contains the app_id element");
            }
        }
#endif

        // Generate a static class containing constants of leaderboard and achievement names.
        void GenerateGameServiceConstants()
        {
            // First create a hashtable containing all the names to be stored as constants.
            SerializedProperty ldbProp = GameServiceProperties.leaderboards.property;
            SerializedProperty acmProp = GameServiceProperties.achievements.property;

            // First check if there're duplicate names.
            string duplicateLdbName = EM_EditorUtil.FindDuplicateFieldInArrayProperty(ldbProp, GameServiceItem_NameProperty);
            if (!string.IsNullOrEmpty(duplicateLdbName))
            {
                EM_EditorUtil.Alert("Error: Duplicate Names", "Found duplicate leaderboard name of \"" + duplicateLdbName + "\".");
                return;
            }

            string duplicateAcmName = EM_EditorUtil.FindDuplicateFieldInArrayProperty(acmProp, GameServiceItem_NameProperty);
            if (!string.IsNullOrEmpty(duplicateAcmName))
            {
                EM_EditorUtil.Alert("Error: Duplicate Names", "Found duplicate achievement name of \"" + duplicateAcmName + "\".");
                return;
            }

            // Proceed with adding resource keys.
            Hashtable resourceKeys = new Hashtable();

            // Add the leaderboard names.
            for (int i = 0; i < ldbProp.arraySize; i++)
            {
                SerializedProperty element = ldbProp.GetArrayElementAtIndex(i);
                string name = element.FindPropertyRelative(GameServiceItem_NameProperty).stringValue;

                // Ignore all items with an empty name.
                if (!string.IsNullOrEmpty(name))
                {
                    string key = "Leaderboard_" + name;
                    resourceKeys.Add(key, name);
                }
            }

            // Add the achievement names.
            for (int j = 0; j < acmProp.arraySize; j++)
            {
                SerializedProperty element = acmProp.GetArrayElementAtIndex(j);
                string name = element.FindPropertyRelative(GameServiceItem_NameProperty).stringValue;

                // Ignore all items with an empty name.
                if (!string.IsNullOrEmpty(name))
                {
                    string key = "Achievement_" + name;
                    resourceKeys.Add(key, name);
                }
            }

            if (resourceKeys.Count > 0)
            {
                // Now build the class.
                EM_EditorUtil.GenerateConstantsClass(
                    EM_Constants.GeneratedFolder,
                    EM_Constants.RootNameSpace + "." + EM_Constants.GameServicesConstantsClassName,
                    resourceKeys,
                    true
                );
            }
            else
            {
                EM_EditorUtil.Alert("Constants Class Generation", "Please fill in required information for all leaderboards and achievements.");
            }
        }

        // Draw the array of leaderboards or achievements inside a foldout and the relevant buttons.
        void DrawGameServiceItemArray(string itemType, EMProperty myProp, ref bool isFoldout)
        {
            if (myProp.property.arraySize > 0)
            {
                EditorGUI.indentLevel++;
                isFoldout = EditorGUILayout.Foldout(isFoldout, myProp.property.arraySize + " " + myProp.content.text, true);
                EditorGUI.indentLevel--;

                if (isFoldout)
                {
                    // Update the string array of Android GPGPS ids to display in the leaderboards and achievements.
                    gpgsIds = new string[gpgsIdDict.Count + 1];
                    gpgsIds[0] = EM_Constants.NoneSymbol;
                    gpgsIdDict.Keys.CopyTo(gpgsIds, 1);

                    Func<SerializedProperty, bool> drawer;

                    if (itemType.Equals("Leaderboard"))
                        drawer = DrawGameServiceLeaderboard;
                    else if (itemType.Equals("Achievement"))
                        drawer = DrawGameServiceAchievement;
                    else
                        throw new System.Exception("Invalid itemType");

                    // Draw the array of achievements or leaderboards.
                    DrawArrayProperty(myProp.property, drawer);

                    // Detect duplicate names.
                    string duplicateName = EM_EditorUtil.FindDuplicateFieldInArrayProperty(myProp.property, GameServiceItem_NameProperty);
                    if (!string.IsNullOrEmpty(duplicateName))
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("Found duplicate name of \"" + duplicateName + "\".", MessageType.Warning);
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No " + itemType + " added.", MessageType.None);
            }

            if (GUILayout.Button("Add New " + itemType, GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
            {
                // Add new leaderboard.
                AddNewGameServiceItem(myProp.property);

                // Open the foldout if it's closed.
                isFoldout = true;
            }
        }

        bool DrawGameServiceLeaderboard(SerializedProperty property)
        {
            return DrawGameServiceItem(property, "Leaderboard");
        }

        bool DrawGameServiceAchievement(SerializedProperty property)
        {
            return DrawGameServiceItem(property, "Achievement");
        }

        // Draw leaderboard or achievement item.
        bool DrawGameServiceItem(SerializedProperty property, string label)
        {
            SerializedProperty name = property.FindPropertyRelative(GameServiceItem_NameProperty);
            SerializedProperty iosId = property.FindPropertyRelative(GameServiceItem_IOSIdProperty);
            SerializedProperty androidId = property.FindPropertyRelative(GameServiceItem_AndroidIdProperty);

            EditorGUILayout.BeginVertical(EM_GUIStyleManager.GetCustomStyle("Item Box"));

            string key = property.propertyPath;
            if (!gameServiceFoldoutStates.ContainsKey(key))
                gameServiceFoldoutStates.Add(key, false);

            string foldoutLabel = string.IsNullOrEmpty(name.stringValue) ? "[Untitled " + label + "]" : name.stringValue;
            EditorGUI.indentLevel++;
            gameServiceFoldoutStates[key] = EditorGUILayout.Foldout(gameServiceFoldoutStates[key], foldoutLabel, true);

            if (gameServiceFoldoutStates[key])
            {
                name.stringValue = EditorGUILayout.TextField("Name", name.stringValue);
                iosId.stringValue = EditorGUILayout.TextField("iOS Id", iosId.stringValue);
                // For Android Id, display a popup of Android leaderboards & achievements for the user to select
                // then assign its associated id to the property.
                EditorGUI.BeginChangeCheck();
                int currentIndex = Mathf.Max(System.Array.IndexOf(gpgsIds, EM_EditorUtil.GetKeyForValue(gpgsIdDict, androidId.stringValue)), 0);
                int newIndex = EditorGUILayout.Popup("Android Id", currentIndex, gpgsIds);
                if (EditorGUI.EndChangeCheck())
                {
                    // Position 0 is [None].
                    if (newIndex == 0)
                    {
                        androidId.stringValue = string.Empty;
                    }
                    else
                    {
                        // Record the new android Id.
                        string newName = gpgsIds[newIndex];
                        androidId.stringValue = gpgsIdDict[newName];
                    }
                }
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            return gameServiceFoldoutStates[key];
        }

        void AddNewGameServiceItem(SerializedProperty property)
        {
            if (property.isArray)
            {
                property.InsertArrayElementAtIndex(property.arraySize);

                // Reset the fields of newly added element or it will take the values of the preceding one.
                SerializedProperty newProp = property.GetArrayElementAtIndex(property.arraySize - 1);
                SerializedProperty name = newProp.FindPropertyRelative(GameServiceItem_NameProperty);
                SerializedProperty iosId = newProp.FindPropertyRelative(GameServiceItem_IOSIdProperty);
                SerializedProperty androidId = newProp.FindPropertyRelative(GameServiceItem_AndroidIdProperty);
                name.stringValue = string.Empty;
                iosId.stringValue = string.Empty;
                androidId.stringValue = string.Empty;
            }
        }
    }
}

