using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace EasyMobile.Editor
{
    public class KeyValuePairDrawer : PropertyDrawer
    {
        protected string keyFieldName = "key";
        protected string valueFieldName = "value";
        protected int keyLabelWidth = 50;
        protected int valueLabelWidth = 60;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty key = property.FindPropertyRelative(keyFieldName);
            SerializedProperty value = property.FindPropertyRelative(valueFieldName);

            // Calculate rects.
            int valueWidth = (int)((position.width - keyLabelWidth - valueLabelWidth) / 2);

            var idLabelRect = new Rect(position.x, position.y, keyLabelWidth, position.height);
            var idValueRect = new Rect(position.x + keyLabelWidth, position.y, valueWidth, position.height);
            var nameLabelRect = new Rect(position.x + keyLabelWidth + valueWidth, position.y, valueLabelWidth, position.height);
            var nameValueRect = new Rect(position.x + keyLabelWidth + valueWidth + valueLabelWidth, position.y, valueWidth, position.height);

            EditorGUI.LabelField(idLabelRect, key.displayName);
            EditorGUI.PropertyField(idValueRect, key, GUIContent.none);
            EditorGUI.LabelField(nameLabelRect, value.displayName);
            EditorGUI.PropertyField(nameValueRect, value, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}