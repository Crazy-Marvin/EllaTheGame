using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace EasyMobile.Editor
{
    [CustomPropertyDrawer(typeof(AdPlacement))]
    public class AdPlacementDrawer : PropertyDrawer
    {
        protected const string NameField = "mName";
        protected const string UseCustomNameField = "mUseCustomName";

        private string[] BuiltinPlacementNames
        {
            get
            {
                if (sBuiltinPlacementNames == null)
                {
                    sBuiltinPlacementNames = Array.ConvertAll(AdPlacement.GetCustomPlacements(), item => item.Name);
                    Array.Sort(sBuiltinPlacementNames);
                }

                return sBuiltinPlacementNames;
            }
        }

        private static string[] sBuiltinPlacementNames = null;
        private static GUIContent[] sBuiltinPlacementPopupOptions = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var nameProp = property.FindPropertyRelative(NameField);
            var useCustomNameProp = property.FindPropertyRelative(UseCustomNameField);

            var labelWidth = EditorGUIUtility.labelWidth;
            var lineHeight = EditorGUIUtility.singleLineHeight;
            float toggleWidth = position.width * 0.3f;
            var toggleOffset = 15;  // for the toggle to be drawn nearer the placement rect    

            var placementRect = new Rect(position.x, position.y, position.width - toggleWidth, lineHeight);
            var toggleRect = new Rect(placementRect.x + placementRect.width - toggleOffset, placementRect.y, toggleWidth + toggleOffset, lineHeight);   

            useCustomNameProp.boolValue = EditorGUI.ToggleLeft(toggleRect, new GUIContent("Custom Placement"), useCustomNameProp.boolValue);

            if (useCustomNameProp.boolValue)  // if the user wants to enter the placement's name manually.
            {
                EditorGUI.PropertyField(placementRect, nameProp, label);
            }
            else    // if the user wants to choose among one of the built-in placements.
            {
                // Convert string[] to GUIContent[] to draw the popup.
                if (sBuiltinPlacementPopupOptions == null)
                    sBuiltinPlacementPopupOptions = Array.ConvertAll(BuiltinPlacementNames, name => new GUIContent(name));

                // Find the index of the last selected built-in placement.
                var lastIndex = Array.IndexOf(BuiltinPlacementNames, nameProp.stringValue);

                // Now draw the popup and store the result.
                var indexResult = EditorGUI.Popup(
                                      placementRect,
                                      label, 
                                      lastIndex < 0 ? 0 : lastIndex, 
                                      sBuiltinPlacementPopupOptions
                                  );
                nameProp.stringValue = BuiltinPlacementNames[indexResult];
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}