using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace EasyMobile.Editor
{
    [CustomPropertyDrawer(typeof(NotificationCategory.ActionButton))]
    public class NotificationActionButtonDrawer : KeyValuePairDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            keyFieldName = "id";
            valueFieldName = "title";
            base.OnGUI(position, property, label);
        }
    }
}