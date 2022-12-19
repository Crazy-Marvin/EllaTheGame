using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EasyMobile.Editor
{
    /// <summary>
    /// Easymobile's settings editor window.
    /// </summary>
    public class EM_SettingsWindow : EditorWindow
    {
        public const string Title = EM_Constants.ProductName;

        private Vector2 scrollPos;
        private UnityEditor.Editor editor;
        private readonly int left = 10, right = 10, top = 3, bottom = 3;
        // padding

        private void OnGUI()
        {
            // Try to create the editor object if it hasn't been initialized.
            if (editor == null)
                editor = UnityEditor.Editor.CreateEditor(EM_Settings.Instance);

            // If it's still null.
            if (editor == null)
            {
                EditorGUILayout.HelpBox("Coundn't create the settings resources editor.", MessageType.Error);
                return;
            }

            EM_SettingsEditor.callFromEditorWindow = true;
            EM_SettingsEditor.width = position.width;
            EM_SettingsEditor.height = position.height;

            editor.DrawHeader();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.BeginVertical(new GUIStyle() { padding = new RectOffset(left, right, top, bottom) });

            editor.OnInspectorGUI();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

            EM_SettingsEditor.callFromEditorWindow = false;
        }

        private static EditorWindow GetWindow()
        {
            // Get the window and make sure it will be opened in the same panel with inspector window.
            Type inspectorType = EM_EditorUtil.GetInspectorWindowType();
            var window = GetWindow<EM_SettingsWindow>(new Type[] { inspectorType });
            window.titleContent = new GUIContent(Title);

            return window;
        }

        public static void ShowWindow()
        {
            var window = GetWindow();
            if (window == null)
            {
                Debug.LogError("Coundn't open the Easy Mobile settings window.");
                return;
            }

            window.Show();
        }
    }
}
