using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EasyMobile.Editor
{
    internal partial class EM_SettingsEditor
    {
        public static float width, height;

        private List<Action> toolActions;

        private void DrawToolsSectionGUI()
        {
            GUILayout.Space(20);

            toolActions = new List<Action>
            {
                #if EASY_MOBILE_PRO
                () => DrawTool(InstallPlayMakerActions, "Install PlayMaker Actions", EM_GUIStyleManager.ToolImportPlaymakerActionsIcon),
                #endif

                () => DrawTool(ReimportNativePackage, "Import External Dependency Manager", EM_GUIStyleManager.ToolReimportGPSRIcon),
                () => DrawTool(ExportEMSettings, "Export Settings", EM_GUIStyleManager.ToolExportEMSettingsIcon),
                () => DrawTool(OpenDocumentation, "User Guide", EM_GUIStyleManager.ToolUserGuideIcon),
                () => DrawTool(OpenScriptingReference, "Scripting Reference", EM_GUIStyleManager.ToolScriptingRefIcon),
                () => DrawTool(OpenVideoTutorials, "Video Tutorials", EM_GUIStyleManager.ToolVideoTutorialsIcon),
                () => DrawTool(SendSupportEmail, "Support", EM_GUIStyleManager.ToolSupportIcon),
                () => DrawTool(OpenAssetStore, "Rate EM", EM_GUIStyleManager.ToolRateIcon),
                () => DrawTool(About, "About", EM_GUIStyleManager.ToolAboutIcon),
            };

            int index = 0;
            while (index < toolActions.Count)
            {
                int itemCount = GetCollumnItemCount();
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < itemCount && index < toolActions.Count; j++)
                {
                    if (j != itemCount)
                        GUILayout.Space(5f);

                    toolActions[index].Invoke();
                    index++;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }

        private void DrawTool(Action toolAction, string buttonLabel, Texture2D icon)
        {
            int itemCount = GetCollumnItemCount();
            float buttonWidth = width / itemCount - (30f / itemCount);
            float buttonHeight = GetButtonHeight(itemCount);

            GUIStyle style = new GUIStyle(GUI.skin.button)
            {
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(6, 6, 6, 6),
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true,
                fixedWidth = buttonWidth,
                fixedHeight = buttonHeight
            };

            if (GUILayout.Button(new GUIContent(buttonLabel, icon), style))
                toolAction();
        }

        // How many columns to arrange the items?
        private int GetCollumnItemCount()
        {
            if (width > 600)
                return 3;

            if (width > 400)
                return 2;

            return 1;
        }

        private int GetButtonHeight(int columnCount)
        {
            if (columnCount <= 1)
                return 40;
            else if (columnCount <= 2)
                return 50;
            else
                return 50;
        }

        public static void InstallPlayMakerActions()
        {
            EditorApplication.delayCall += () => EM_ExternalPluginManager.InstallPlayMakerActions(true);
        }

        public static void ReimportNativePackage()
        {
            EM_ExternalPluginManager.ImportPlayServicesResolver(true);
        }

        public static void ExportEMSettings()
        {
            var packageName = EM_Constants.SettingsAssetName + ".unitypackage";
            string exportDest = EditorUtility.OpenFolderPanel("Select Export Destination", Application.dataPath, "");
            AssetDatabase.ExportPackage(EM_Constants.SettingsAssetPath, exportDest + "/" + packageName, ExportPackageOptions.Default | ExportPackageOptions.Interactive);
        }

        public static void OpenDocumentation()
        {
            Application.OpenURL(EM_Constants.DocumentationURL);
        }

        public static void OpenScriptingReference()
        {
            Application.OpenURL(EM_Constants.ScriptingRefURL);
        }

        public static void OpenVideoTutorials()
        {
            Application.OpenURL(EM_Constants.VideoTutorialsURL);
        }

        public static void SendSupportEmail()
        {
            Application.OpenURL("mailto:" + EM_Constants.SupportEmail + "?subject=" + EM_EditorUtil.EscapeURL(EM_Constants.SupportEmailSubject));
        }

        public static void OpenAssetStore()
        {
            Application.OpenURL(EM_Constants.AssetStoreURL);
        }

        private static void ManuallyGenerateManifest()
        {
            string jdkPath = EM_EditorUtil.GetJdkPath();

            if (string.IsNullOrEmpty(jdkPath))
            {
                EM_EditorUtil.Alert("Missing JDK Path", "A JDK path needs to be specified for the generation of Easy Mobile's AndroidManifest.xml as well as for the Android build. " +
                   "Go to Preferences > External Tools > JDK to set it.");
            }
            else
            {
                EM_AndroidManifestBuilder.GenerateManifest(jdkPath, true, true);
                EM_EditorUtil.Alert("Android Manifest Updated", "Easy Mobile's Android manifest (Assets/Plugins/Android/EasyMobile/AndroidManifest.xml) has been updated!");
            }
        }

        public static void About()
        {
            EditorWindow.GetWindow<EM_About>(true);
        }
    }
}
