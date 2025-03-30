namespace Yodo1.MAS
{
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public class Yodo1AdPrerequisites : EditorWindow
    {
        static EditorWindow window;
        Vector2 scrollPosition;

        static Yodo1AdPrerequisites()
        {
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
        }

        private static void OnImportPackageCompleted(string packagename)
        {
            if (packagename.Contains("Rivendell"))
            {
                Yodo1AdPrerequisites.Initialize();
            }
        }

        //[MenuItem("Yodo1/MAS/MAS Prerequisites")]
        public static void Initialize()
        {
            if (window != null)
            {
                window.Close();
                window = null;
            }

            window = EditorWindow.GetWindow(typeof(Yodo1AdPrerequisites), false, "MAS Prerequisites", true);
            window.position = new Rect(0, 0, 800, 400);
            window.Show();
        }

        private void OnGUI()
        {
            this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[0]);

            DrawContents();

            GUILayout.Space(20);

            if (GUILayout.Button("OK"))
            {
                this.Close();
            }

            GUILayout.EndScrollView();
        }

        private void DrawContents()
        {
            GUIStyle headerLabelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                fixedHeight = 18

            };
            GUIStyle contentLabelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                fixedHeight = 18,
            };

            GUILayout.BeginVertical();

            GUILayout.Space(10);

            GUILayout.Label("Unity", headerLabelStyle);
            GUILayout.Label("1. Unity LTS 2019 or above", contentLabelStyle);
            GUILayout.Space(20);

            GUILayout.Label("Android", headerLabelStyle);
            GUILayout.Label("1. Minimum API Level 23 or above", contentLabelStyle);
            GUILayout.Label("2. Target API Level 34 or above", contentLabelStyle);
            GUILayout.Label("3. Unity Editor version 2021.3.41f1 or above", contentLabelStyle);
            GUILayout.Label("4. If you use Proguard, please refer to the documentation.", contentLabelStyle);


            GUIStyle hyperlinkStyle = new GUIStyle(GUI.skin.label);
            hyperlinkStyle.normal.textColor = Color.blue;
            hyperlinkStyle.hover.textColor = Color.cyan;

            if (GUILayout.Button("Click here for the Proguard details", hyperlinkStyle))
            {
                // Handle hyperlink click event
                Application.OpenURL("https://developers.yodo1.com/docs/sdk/advanced/proguard");
            }

            GUILayout.Space(20);
            GUILayout.Label("iOS", headerLabelStyle);
            GUILayout.Label("1. iOS 13.0 or above.", contentLabelStyle);
            GUILayout.Label("2. Xcode 15.2 or above", contentLabelStyle);
            GUILayout.Label("3. Cocoapods 1.10.0 or above", contentLabelStyle);

            GUILayout.EndVertical();
        }
    }
}