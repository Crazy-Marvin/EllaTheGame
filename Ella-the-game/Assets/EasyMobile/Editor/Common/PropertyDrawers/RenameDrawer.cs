using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;
using EasyMobile.Internal;

namespace EasyMobile.Editor
{
    [CustomPropertyDrawer(typeof(RenameAttribute))]
    public class RenameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, new GUIContent((attribute as RenameAttribute).NewName));
        }
    }
}