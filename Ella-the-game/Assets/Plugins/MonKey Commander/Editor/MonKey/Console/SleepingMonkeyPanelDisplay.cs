using MonKey.Editor.Internal;
using MonKey.Internal;
using UnityEditor;
using UnityEngine;

namespace MonKey.Editor.Console
{
    internal static class SleepingMonkeyPanelDisplay
    {
        internal static void DisplaySleepingMonkey()
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

            GUILayout.Label(MonKeyLocManager.CurrentLoc.MonkeySleeping, !EditorGUIUtility.isProSkin ?
                MonkeyStyle.Instance.HelpStyle : MonkeyStyle.Instance.HelpStyleProSleeping);
            GUILayout.Label(MonKeyLocManager.CurrentLoc.WakeUp, !EditorGUIUtility.isProSkin ?
                MonkeyStyle.Instance.HelpStyle : MonkeyStyle.Instance.HelpStyleProSleeping);

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
    }
}
