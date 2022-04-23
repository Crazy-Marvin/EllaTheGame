using System;
using MonKey.Editor.Internal;
using MonKey.Internal;
using UnityEditor;
using UnityEngine;

namespace MonKey.Editor.Console
{
    public class SceneCommandsWindow : EditorWindow
    {
        public static SceneCommandsWindow Instance;

        public int SelectedTab = 0;

        public static void ShowSceneCommandWindow()
        {
            if (!Instance)
            {
                Instance = GetWindow<SceneCommandsWindow>("MonKey Scene Command Manager");
                Instance.titleContent = new GUIContent(" MonKey Scene Command Manager"
                    , MonkeyStyle.Instance.MonkeyHead, "Monkeys are the best animals!");


            }

            Instance.Focus();
            Instance.SelectedTab = MonkeyEditorUtils.CurrentSceneCommands.Count - 1;
        }

        private void OnDestroy()
        {
            foreach (var command in MonkeyEditorUtils.CurrentSceneCommands)
            {
               command.Stop();
            }
        }

        private void OnGUI()
        {
            if (MonkeyEditorUtils.CurrentSceneCommands.Count == 0)
            {

                GUILayout.BeginVertical(MonkeyStyle.Instance.MonkeyLogoStyleSleepingGroup);
                GUILayout.FlexibleSpace();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();


                GUILayout.Label("", EditorGUIUtility.isProSkin ?
                    MonkeyStyle.Instance.MonkeyLogoStyleSleepingPro :
                    MonkeyStyle.Instance.MonkeyLogoStyleSleeping);

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Label("No Active Scene Command", !EditorGUIUtility.isProSkin ?
                    MonkeyStyle.Instance.HelpStyle : MonkeyStyle.Instance.HelpStyleProSleeping);


                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();

                return;
            }

            SelectedTab = GUILayout.Toolbar(SelectedTab, MonkeyEditorUtils.CurrentSceneCommands.ConvertAll(_ => _.SceneCommandName).ToArray());

            MonkeyEditorUtils.SelectedSceneCommand = MonkeyEditorUtils.CurrentSceneCommands[SelectedTab];

            if (GUILayout.Button("Stop Scene Command"))
            {
                MonkeyEditorUtils.SelectedSceneCommand.Stop();
                Event.current.Use();
            }

            MonkeyEditorUtils.SelectedSceneCommand.DisplayCommandPanel(0, 0, true);

          
        }
    }
}