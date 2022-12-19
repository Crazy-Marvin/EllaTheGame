using UnityEngine;
using UnityEditor;
using System.Collections;

namespace EasyMobile.Editor
{
    public static class EditorGUILayoutExt
    {
        public static void AutoTrimTextField(SerializedProperty property, GUIContent label, params GUILayoutOption[] options)
        {
            property.stringValue = EditorGUILayout.TextField(label, property.stringValue, options).Trim();
        }
    }
}
