using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Yodo1.MAS
{
    public class Yodo1ConflictData
    {
        public string name;
        public string[] paths;
    }

    public class Yodo1AdIntegrationManagerWindow : EditorWindow
    {
        private const string keyMasIdentifier = "com.yodo1.rivendell_sdk";

        private const string integrationWindowTitle = "MAS Integration Manager";

        private const string documentationLink = "https://developers.yodo1.com/article-categories/unity/";
        private const string documentationFireBaseLink = "https://developers.yodo1.com/knowledge-base/known-issues/#3-firebase-compatibility";
        private const string note = "You can consult the MAS official integration documentation to resolve problems. Click the link below for more info:";

        private Vector2 scrollPosition;
        private static readonly Vector2 windowMinSize = new Vector2(750, 350);
        private const float actionFieldWidth = 60f;
        private const float networkFieldMinWidth = 100f;
        private const float versionFieldMinWidth = 400f;
        private const float privacySettingLabelWidth = 200f;
        private const float networkFieldWidthPercentage = 0.22f;
        private const float versionFieldWidthPercentage = 0.36f; // There are two version fields. Each take 40% of the width, network field takes the remaining 20%.
        private static float previousWindowWidth = windowMinSize.x;
        private static GUILayoutOption networkWidthOption = GUILayout.Width(networkFieldMinWidth);
        private static GUILayoutOption versionWidthOption = GUILayout.Width(versionFieldMinWidth);

        private static GUILayoutOption sdkKeyTextFieldWidthOption = GUILayout.Width(520);

        private static GUILayoutOption privacySettingFieldWidthOption = GUILayout.Width(400);
        private static readonly GUILayoutOption fieldWidth = GUILayout.Width(actionFieldWidth);

        private static readonly Color darkModeTextColor = new Color(0.29f, 0.6f, 0.8f);

        private GUIStyle titleLabelStyle;
        private GUIStyle headerLabelStyle;
        private GUIStyle wrapTextLabelStyle;
        private GUIStyle linkLabelStyle;

        private Yodo1ConflictData[] pluginData = new Yodo1ConflictData[] { };
        private bool pluginDataLoadFailed;
        private bool shouldMarkNewLocalizations;


        public static void ShowManager()
        {
            var manager = GetWindow<Yodo1AdIntegrationManagerWindow>(utility: true, title: integrationWindowTitle, focus: true);
            manager.minSize = windowMinSize;
        }

        #region Editor Window Lifecyle Methods

        private void Awake()
        {
            titleLabelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                fixedHeight = 20
            };

            headerLabelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                fixedHeight = 18
            };

            linkLabelStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                normal = { textColor = EditorGUIUtility.isProSkin ? darkModeTextColor : Color.blue }
            };

            wrapTextLabelStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true
            };
        }

        private void OnEnable()
        {
            shouldMarkNewLocalizations = !EditorPrefs.GetBool(keyMasIdentifier, false);
            // Plugin downloaded and imported. Update current versions for the imported package.
            Load();
        }

        private void OnDisable()
        {
            // New localizations have been shown to the publisher, now remove them.
            if (shouldMarkNewLocalizations)
            {
                EditorPrefs.SetBool(keyMasIdentifier, true);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
        }

        private void OnGUI()
        {
            // OnGUI is called on each frame draw, so we don't want to do any unnecessary calculation if we can avoid it. So only calculate it when the width actually changed.
            if (Math.Abs(previousWindowWidth - position.width) > 1)
            {
                previousWindowWidth = position.width;
                CalculateFieldWidth();
            }

            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition, false, false))
            {
                scrollPosition = scrollView.scrollPosition;

                GUILayout.Space(5);
                EditorGUILayout.LabelField("Conflict SDKs", titleLabelStyle);
                if (Yodo1AdBuildCheck.HasConflict())
                {
                    // Draw mediated networks
                    DrawMediatedNetworks();

                    // Draw documentation notes
                    EditorGUILayout.LabelField(new GUIContent(note), wrapTextLabelStyle);
                    if (GUILayout.Button(new GUIContent(documentationLink), linkLabelStyle))
                    {
                        Application.OpenURL(documentationLink);
                    }
                }
                else
                {
                    GUIStyle gUIStyle2 = new GUIStyle();
                    gUIStyle2.padding = new RectOffset(0, 10, 10, 0);
                    GUILayout.BeginVertical(gUIStyle2, new GUILayoutOption[0]);
                    GUILayout.Label("There is no conflict with MAS SDK");
                    GUILayout.EndVertical();
                }


            }
        }

        #endregion

        #region UI Methods

        /// <summary>
        /// Draws the headers for a table.
        /// </summary>
        private void DrawHeaders(string firstColumnTitle, bool drawAction)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(5);
                EditorGUILayout.LabelField(firstColumnTitle, headerLabelStyle, networkWidthOption);
                GUILayout.Space(3);
                if (drawAction)
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Button("Actions", headerLabelStyle, fieldWidth);
                    GUILayout.Space(5);
                }
            }

            GUILayout.Space(4);
        }

        /// <summary>
        /// Draws the platform specific version details for AppLovin MAX plugin.
        /// </summary>
        private void DrawPluginDetailRow(string name, string path)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(5);
                EditorGUILayout.LabelField(new GUIContent(name), networkWidthOption);
                GUILayout.Space(3);
            }

            GUILayout.Space(4);
        }

        /// <summary>
        /// Draws mediated network details table.
        /// </summary>
        private void DrawMediatedNetworks()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            using (new EditorGUILayout.VerticalScope("box"))
            {
                DrawHeaders("SDK", true);
                if (pluginData.Length == 0)
                {
                    return;
                }
                foreach (var network in pluginData)
                {
                    DrawNetworkDetailRow(network);
                }

                GUILayout.Space(5);
            }

            GUILayout.Space(5);
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws the network specific details for a given network.
        /// </summary>
        private void DrawNetworkDetailRow(Yodo1ConflictData network)
        {
            string action;
            if (network.name.Equals("FireBase"))
            {
                action = "Solution";
            }
            else
            {
                action = "Remove";
            }

            GUILayout.Space(4);
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandHeight(false)))
            {
                GUILayout.Space(5);
                EditorGUILayout.LabelField(new GUIContent(network.name), headerLabelStyle, networkWidthOption);
                GUILayout.Space(3);
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(new GUIContent(action), fieldWidth))
                {
                    EditorUtility.DisplayProgressBar("Integration Manager", "Removing " + network.name + "...", 0.5f);
                    if (network.name.Equals("FireBase"))
                    {
                        Application.OpenURL(documentationFireBaseLink);
                    }
                    else if (network.name.Equals("Admob"))
                    {
                        Yodo1AdBuildCheck.RemoveAdmobAds();
                        RemoveFromList(network);
                        AssetDatabase.Refresh();
                    }
                    else if (network.name.Equals("UnityAds"))
                    {
                        Yodo1AdBuildCheck.RemoveUnityAds();
                    }
                    else if (network.name.Equals("Facebook"))
                    {
                        Yodo1AdBuildCheck.RemoveFacebookCoreKit();
                        RemoveFromList(network);
                        AssetDatabase.Refresh();
                    }
                    else
                    {

                    }


                    //if(!network.name.Equals("UnityAds")) // refresh asset. unityads no need
                    //{
                    //AssetDatabase.Refresh();
                    //}
                    EditorUtility.ClearProgressBar();
                }
            }
            foreach (string path in network.paths)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    GUILayout.Space(2);
                    DrawTextLabel(path, networkWidthOption);
                }

                GUILayout.EndHorizontal();
            }
        }

        private void DrawTextLabel(string text, GUILayoutOption labelWidth, GUILayoutOption textFieldWidthOption = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(2);
            if (textFieldWidthOption == null)
            {
                EditorGUILayout.LabelField(text);
            }
            else
            {
                EditorGUILayout.LabelField(text, linkLabelStyle, textFieldWidthOption);
            }
            GUILayout.Space(2);
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
        }

        /// <summary>
        /// Calculates the fields width based on the width of the window.
        /// </summary>
        private void CalculateFieldWidth()
        {
            var currentWidth = position.width;
            var availableWidth = currentWidth - actionFieldWidth - 80; // NOTE: Magic number alert. This is the sum of all the spacing the fields and other UI elements.
            var networkLabelWidth = Math.Max(networkFieldMinWidth, availableWidth * networkFieldWidthPercentage);
            networkWidthOption = GUILayout.Width(networkLabelWidth);

            var versionLabelWidth = Math.Max(versionFieldMinWidth, availableWidth * versionFieldWidthPercentage);
            versionWidthOption = GUILayout.Width(versionLabelWidth);

            const int textFieldOtherUiElementsWidth = 45; // NOTE: Magic number alert. This is the sum of all the spacing the fields and other UI elements.
            var availableTextFieldWidth = currentWidth - networkLabelWidth - textFieldOtherUiElementsWidth;
            sdkKeyTextFieldWidthOption = GUILayout.Width(availableTextFieldWidth);

            var availableUserDescriptionTextFieldWidth = currentWidth - privacySettingLabelWidth - textFieldOtherUiElementsWidth;
            privacySettingFieldWidthOption = GUILayout.Width(availableUserDescriptionTextFieldWidth);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Loads the plugin data to be displayed by this window.
        /// </summary>
        private void Load()
        {
            bool admobadsFlag = Yodo1AdBuildCheck.IsAdmobAdsExist();
            bool fireBaseFlag = Yodo1AdBuildCheck.IsFirebaseExist();
            bool unityadsFlag = Yodo1AdBuildCheck.IsUnityAdsExist();
            bool fbsdkcorekitFlag = Yodo1AdBuildCheck.IsFBSDKCoreKitExist();
            var list = pluginData.ToList();
            if (admobadsFlag)
            {
                Yodo1ConflictData data = new Yodo1ConflictData();
                data.name = "Admob";
                data.paths = new string[] { "/Assets/GoogleMobileAds", "/Assets/Plugins/Android/GoogleMobileAdsPlugin.androidlib", "/Assets/Plugins/Android/googlemobileads-unity.aar", "/Assets/Plugins/iOS/unity-plugin-library.a" };
                list.Add(data);
            }
            if (fireBaseFlag)
            {
                Yodo1ConflictData data = new Yodo1ConflictData();
                data.name = "FireBase";
                data.paths = new string[] { "/Assets/Firebase", "/Assets/Firebase/m2repository/com/google/firebase/firebase-app-unity", "/Assets/Firebase/Plugins/iOS/Firebase.App.dll" };
                list.Add(data);
            }
            if (fbsdkcorekitFlag)
            {
                Yodo1ConflictData data = new Yodo1ConflictData();
                data.name = "Facebook";
                data.paths = new string[] { "/Assets/FacebookSDK/Plugins/Editor/Dependencies.xml" };
                list.Add(data);
            }
            if (unityadsFlag)
            {
                Yodo1ConflictData data = new Yodo1ConflictData();
                data.name = "UnityAds";
                data.paths = new string[] { "/Packages/manifest.json" };
                list.Add(data);
            }
            pluginData = list.ToArray();
            CalculateFieldWidth();
            // Repaint();
        }

        private void RemoveFromList(Yodo1ConflictData network)
        {

            var list = pluginData.ToList();
            if (list != null && list.Contains(network))
            {
                list.Remove(network);
                pluginData = list.ToArray();
            }

        }

        #endregion
    }
}
