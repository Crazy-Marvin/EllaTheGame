using UnityEngine;
using UnityEditor;
using System.Collections;

namespace EasyMobile.Editor
{
    public static class EM_EditorGUI
    {
        private const int MODULE_LABEL_HEIGHT = 24;

        public static void HeaderLabel(string text)
        {
            EditorGUILayout.LabelField(text, EM_GUIStyleManager.GetCustomStyle("Header Label"), GUILayout.Height(26));
        }

        public static bool Foldout(bool foldout, string content)
        {
            return Foldout(foldout, new GUIContent(content));
        }

        public static bool Foldout(bool foldout, GUIContent content)
        {
            Rect rect = EditorGUILayout.GetControlRect();
            return EditorGUI.Foldout(rect, foldout, content, true, EM_GUIStyleManager.ModuleFoldout);
        }

        public static void ToolbarButton<Enum>(string text, Enum thisItem, ref Enum activeItem, GUIStyle style)
        {
            ToolbarButton(new GUIContent(text), thisItem, ref activeItem, style);
        }

        public static void ToolbarButton<Enum>(GUIContent content, Enum thisItem, ref Enum activeItem, GUIStyle style)
        {
            EditorGUI.BeginChangeCheck();
            if (GUILayout.Toggle(thisItem.Equals(activeItem), content, style))
            { 
                // Check if the toolbar active item has just changed.
                if (EditorGUI.EndChangeCheck())
                {                 
                    EditorGUI.FocusTextInControl(null);
                }

                activeItem = thisItem;   
            }           
        }

        public static void ModuleLabel(string label)
        {
            EditorGUILayout.LabelField(
                label, 
                EM_GUIStyleManager.GetCustomStyle("Module Toggle Label"),
                GUILayout.ExpandWidth(true),
                GUILayout.Height(MODULE_LABEL_HEIGHT)
            );
        }

        public static bool ModuleToggle(bool toggle, string label)
        {
            bool result = EditorGUILayout.Toggle(
                              toggle, 
                              EM_GUIStyleManager.GetCustomStyle("Module Toggle"),
                              GUILayout.Width(44));

            return result;
        }
    }
}

