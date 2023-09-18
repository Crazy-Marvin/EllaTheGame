using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EasyMobile.Internal;

namespace EasyMobile.Editor
{
    [CustomPropertyDrawer(typeof(Dictionary_AdPlacement_AdId))]
    public class DictionaryDrawer_AdPlacement_AdId : SerializableDictionaryPropertyDrawer
    {
        private const string AdPlacementNameField = "mName";
        private const string iOSIdField = "mIosId";
        private const string androidIdField = "mAndroidId";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
        }

        protected override float DrawKeyValueLineSimple(SerializedProperty keyProperty, SerializedProperty valueProperty, string keyLabel, string valueLabel, UnityEngine.Rect linePosition)
        {
            var adPlacementNameProp = keyProperty.FindPropertyRelative(AdPlacementNameField);
            var iOSIdProp = valueProperty.FindPropertyRelative(iOSIdField);
            var androidIdProp = valueProperty.FindPropertyRelative(androidIdField);

            var lineHeight = EditorGUIUtility.singleLineHeight;
            var spacing = EditorGUIUtility.standardVerticalSpacing;
            var foldoutRect = new Rect(linePosition.x, linePosition.y, linePosition.width, lineHeight);
            string foldoutContent = (string.IsNullOrEmpty(adPlacementNameProp.stringValue) ? "[Untitled Placement]" : adPlacementNameProp.stringValue);

            keyProperty.isExpanded = EditorGUI.Foldout(foldoutRect, keyProperty.isExpanded, foldoutContent, true);
            var totalHeight = lineHeight + spacing;

            if (keyProperty.isExpanded)
            {
                var placementRect = new Rect(linePosition.x, linePosition.y + lineHeight + spacing, linePosition.width, lineHeight);
                var iOSIdRect = new Rect(placementRect.x, placementRect.y + lineHeight + spacing, linePosition.width, lineHeight);
                var androidIdRect = new Rect(iOSIdRect.x, iOSIdRect.y + lineHeight + spacing, linePosition.width, lineHeight);

                EditorGUI.PropertyField(placementRect, keyProperty, new GUIContent("Placement"));
                EditorGUI.PropertyField(iOSIdRect, iOSIdProp);
                EditorGUI.PropertyField(androidIdRect, androidIdProp);

                totalHeight += lineHeight * 3 + spacing * 3;
            }

            return totalHeight;
        }

        protected override float DrawKeyValueLineExpand(SerializedProperty keyProperty, SerializedProperty valueProperty, Rect linePosition)
        {
            return base.DrawKeyValueLineExpand(keyProperty, valueProperty, linePosition);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float propertyHeight = EditorGUIUtility.singleLineHeight;

            if (property.isExpanded)
            {
                var keysProperty = property.FindPropertyRelative(KeysFieldName);
                var valuesProperty = property.FindPropertyRelative(ValuesFieldName);

                foreach (var entry in EnumerateEntries(keysProperty, valuesProperty))
                {
                    if (entry.keyProperty.isExpanded)
                        propertyHeight += 4 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                    else
                        propertyHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                ConflictState conflictState = GetConflictState(property);

                if (conflictState.conflictIndex != -1)
                {
                    if (conflictState.conflictKeyPropertyExpanded)
                        propertyHeight += 4 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                    else
                        propertyHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }
            }

            return propertyHeight;
        }
    }
}
