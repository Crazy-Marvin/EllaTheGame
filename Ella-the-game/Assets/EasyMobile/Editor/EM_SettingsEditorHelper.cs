using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace EasyMobile.Editor
{
    internal partial class EM_SettingsEditor
    {
        #region Uppercase Sections GUI

        private Dictionary<string, bool> uppercaseSectionsFoldoutStates = new Dictionary<string, bool>();
        private Dictionary<string, bool> uppercaseSectionsToggleStates = new Dictionary<string, bool>();

        private const int DefaultSectionHeaderIconWidth = 20;
        private const int DefaultSectionHeaderIconHeight = 20;
        private const int ChevronIconWidth = 10;
        private const int ChevronIconRightMargin = 5;

        private void DrawUppercaseSection(string key, string sectionName, Action drawer, Action disableModule, Action enableModule, SerializedProperty triggerProperty, Texture2D sectionIcon = null, bool defautFoldout = true)
        {
            if (!uppercaseSectionsFoldoutStates.ContainsKey(key))
                uppercaseSectionsFoldoutStates.Add(key, defautFoldout);

            bool foldout = uppercaseSectionsFoldoutStates[key];

            EditorGUILayout.BeginVertical(EM_GUIStyleManager.GetCustomStyle("Uppercase Section Box"), GUILayout.MinHeight(foldout ? 30 : 0));

            EditorGUILayout.BeginHorizontal(foldout ? EM_GUIStyleManager.UppercaseSectionHeaderExpand : EM_GUIStyleManager.UppercaseSectionHeaderCollapse);

            // Header icon.
            EditorGUILayout.LabelField(new GUIContent(sectionIcon ?? EM_GUIStyleManager.UppercaseSectionHeaderIcon),
                EM_GUIStyleManager.GetCustomStyle("Uppercase Section Header Icon"),
                GUILayout.Width(DefaultSectionHeaderIconWidth),
                GUILayout.Height(DefaultSectionHeaderIconHeight));

            // Header label (and button).
            if (GUILayout.Button(sectionName, EM_GUIStyleManager.GetCustomStyle("Uppercase Section Header Label")))
                uppercaseSectionsFoldoutStates[key] = !uppercaseSectionsFoldoutStates[key];

            // The expand/collapse icon.
            var buttonRect = GUILayoutUtility.GetLastRect();
            var iconRect = new Rect(buttonRect.x + buttonRect.width - ChevronIconWidth - ChevronIconRightMargin, buttonRect.y, ChevronIconWidth, buttonRect.height);
            GUI.Label(iconRect, GetChevronIcon(foldout), EM_GUIStyleManager.GetCustomStyle("Uppercase Section Header Chevron"));

            /// Draw toggle.
            Rect toggleRect = Rect.zero;
            EditorGUI.BeginChangeCheck();
            if (triggerProperty != null)
            {
                triggerProperty.boolValue = EM_EditorGUI.ModuleToggle(triggerProperty.boolValue, name);
                if (EditorGUI.EndChangeCheck())
                {
                    if (triggerProperty.boolValue)
                    {
                        enableModule();
                    }
                    else
                    {
                        disableModule();
                    }
                }
                toggleRect = GUILayoutUtility.GetLastRect();

            }

            EditorGUILayout.EndHorizontal();

            // Draw the section content.
            if (foldout)
                GUILayout.Space(5);

            if (foldout && drawer != null)
                drawer();

            EditorGUILayout.EndVertical();
        }

        private void DrawUppercaseSection(string key, string sectionName, Action drawer, Texture2D sectionIcon = null, bool defaultFoldout = true)
        {
            if (!uppercaseSectionsFoldoutStates.ContainsKey(key))
                uppercaseSectionsFoldoutStates.Add(key, defaultFoldout);

            bool foldout = uppercaseSectionsFoldoutStates[key];

            EditorGUILayout.BeginVertical(EM_GUIStyleManager.GetCustomStyle("Uppercase Section Box"), GUILayout.MinHeight(foldout ? 30 : 0));

            EditorGUILayout.BeginHorizontal(foldout ? EM_GUIStyleManager.UppercaseSectionHeaderExpand : EM_GUIStyleManager.UppercaseSectionHeaderCollapse);

            // Header icon.
            EditorGUILayout.LabelField(new GUIContent(sectionIcon ?? EM_GUIStyleManager.UppercaseSectionHeaderIcon),
                EM_GUIStyleManager.GetCustomStyle("Uppercase Section Header Icon"),
                GUILayout.Width(DefaultSectionHeaderIconWidth),
                GUILayout.Height(DefaultSectionHeaderIconHeight));

            // Header label (and button).
            if (GUILayout.Button(sectionName, EM_GUIStyleManager.GetCustomStyle("Uppercase Section Header Label")))
                uppercaseSectionsFoldoutStates[key] = !uppercaseSectionsFoldoutStates[key];

            // The expand/collapse icon.
            var buttonRect = GUILayoutUtility.GetLastRect();
            var iconRect = new Rect(buttonRect.x + buttonRect.width - ChevronIconWidth - ChevronIconRightMargin, buttonRect.y, ChevronIconWidth, buttonRect.height);
            GUI.Label(iconRect, GetChevronIcon(foldout), EM_GUIStyleManager.GetCustomStyle("Uppercase Section Header Chevron"));

            EditorGUILayout.EndHorizontal();

            // Draw the section content.
            if (foldout)
                GUILayout.Space(5);

            if (foldout && drawer != null)
                drawer();

            EditorGUILayout.EndVertical();
        }

        private bool DrawUppercaseSectionWithToggle(string key, string sectionName, bool toggle, Action drawer, Texture2D sectionIcon = null, bool defaultFoldout = true)
        {
            if (!uppercaseSectionsFoldoutStates.ContainsKey(key))
                uppercaseSectionsFoldoutStates.Add(key, defaultFoldout);

            if (!uppercaseSectionsToggleStates.ContainsKey(key))
                uppercaseSectionsToggleStates.Add(key, false);

            bool foldout = uppercaseSectionsFoldoutStates[key];
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            float horizontalSize = GUILayoutUtility.GetLastRect().width;

            EditorGUILayout.BeginVertical(EM_GUIStyleManager.GetCustomStyle("Uppercase Section Box"), GUILayout.MinHeight(foldout ? 30 : 0));

            EditorGUILayout.BeginHorizontal(foldout ? EM_GUIStyleManager.UppercaseSectionHeaderExpand : EM_GUIStyleManager.UppercaseSectionHeaderCollapse);

            // Header icon.
            EditorGUILayout.LabelField(new GUIContent(sectionIcon ?? EM_GUIStyleManager.UppercaseSectionHeaderIcon),
                EM_GUIStyleManager.GetCustomStyle("Uppercase Section Header Icon"),
                GUILayout.Width(DefaultSectionHeaderIconWidth),
                GUILayout.Height(DefaultSectionHeaderIconHeight));

            // The toggle.
            var headerRect = GUILayoutUtility.GetLastRect();
            var toogleX = horizontalSize - 40 - 5 - ChevronIconWidth - ChevronIconRightMargin;
            var toggleRect = new Rect(toogleX, headerRect.y - 2, 40, 24);
            var result = EditorGUI.Toggle(toggleRect, toggle, EM_GUIStyleManager.GetCustomStyle("Module Toggle"));

            // Expand or contract the foldout according to the toggle state.
            if ((result && !uppercaseSectionsToggleStates[key]) || (!result && uppercaseSectionsToggleStates[key]))
            {
                // Just toggled on or off.
                uppercaseSectionsFoldoutStates[key] = result;
                foldout = uppercaseSectionsFoldoutStates[key];
            }

            // Save toggle state.
            uppercaseSectionsToggleStates[key] = result;

            // Header label (and button to control the foldout).
            if (GUILayout.Button(sectionName, EM_GUIStyleManager.GetCustomStyle("Uppercase Section Header Label")))
                uppercaseSectionsFoldoutStates[key] = !uppercaseSectionsFoldoutStates[key];

            // The expand/collapse icon.
            var buttonRect = GUILayoutUtility.GetLastRect();
            var iconRect = new Rect(buttonRect.x + buttonRect.width - ChevronIconWidth - ChevronIconRightMargin, buttonRect.y, ChevronIconWidth, buttonRect.height);
            GUI.Label(iconRect, GetChevronIcon(foldout), EM_GUIStyleManager.GetCustomStyle("Uppercase Section Header Chevron"));

            EditorGUILayout.EndHorizontal();

            // Draw the section content.
            if (foldout)
                GUILayout.Space(5);

            if (foldout && drawer != null)
                drawer();

            EditorGUILayout.EndVertical();

            return result;
        }

        private void DrawUppercaseSectionWithToggle(string key, string sectionName, SerializedProperty toggleProperty, Action drawer, Texture2D sectionIcon = null, bool defaultFoldout = true)
        {
            toggleProperty.boolValue = DrawUppercaseSectionWithToggle(key, sectionName, toggleProperty.boolValue, drawer, sectionIcon, defaultFoldout);
        }

        private Texture2D GetChevronIcon(bool foldout)
        {
            return foldout ? EM_GUIStyleManager.ChevronUp : EM_GUIStyleManager.ChevronDown;
        }

        #endregion

        #region Android Permissions & iOS Usage Descriptions

        private void DrawAndroidPermissionsRequiredSection(Module module, Action priorDrawAct = null, Action afterDrawAct = null)
        {
            var modulePermissions = EM_PluginManager.GetAndroidPermissionsRequiredByModule(module);
            if (modulePermissions == null || modulePermissions.Count < 1)
                return;

            DrawUppercaseSection("ANDROID_PERMISSIONS_MODULE_" + module, "REQUIRED ANDROID PERMISSIONS", () =>
                {
                    if (priorDrawAct != null)
                        priorDrawAct();

                    foreach (var permission in modulePermissions)
                        DrawAndroidPermission(permission.ElementName, permission.Value);

                    if (afterDrawAct != null)
                        afterDrawAct();
                });
        }

        private void DrawIOSInfoPlistItemsRequiredSection(Module module, Action priorDrawAct = null, Action afterDrawAct = null)
        {
            var modulePlistItems = EM_PluginManager.GetIOSInfoItemsRequiredByModule(module);

            if (modulePlistItems == null || modulePlistItems.Count < 1)
                return;

            DrawUppercaseSection("IOS_USAGE_DESCRIPTIONS_EDITABLE_" + module, "REQUIRED IOS INFO.PLIST KEYS", () =>
                {
                    if (priorDrawAct != null)
                        priorDrawAct();

                    foreach (var item in modulePlistItems)
                        DrawEditableIOSInfoPlistItem(item);

                    if (afterDrawAct != null)
                        afterDrawAct();
                });
        }

        private void DrawAndroidPermissionsRequiredSubsection(List<AndroidPermission> permissions, GUIContent label = null, GUIStyle labelStyle = null)
        {
            if (permissions == null || permissions.Count < 1)
                return;

            if (label != null)
                EditorGUILayout.LabelField(label, labelStyle != null ? labelStyle : EditorStyles.boldLabel);

            foreach (var permission in permissions)
                DrawAndroidPermission(permission.ElementName, permission.Value);
        }

        private void DrawIOSInfoPlistItemsRequiredSubsection(List<iOSInfoPlistItem> plistItems, GUIContent label = null, GUIStyle labelStyle = null)
        {
            if (plistItems == null || plistItems.Count < 1)
                return;

            if (label != null)
                EditorGUILayout.LabelField(label, labelStyle != null ? labelStyle : EditorStyles.boldLabel);

            foreach (var item in plistItems)
                DrawEditableIOSInfoPlistItem(item);
        }

        private void DrawReadonlyIOSInfoPlistItem(iOSInfoPlistItem item)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField(item.Key, EditorStyles.label);
            EditorGUILayout.SelectableLabel(item.Value, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));

            EditorGUILayout.EndVertical();
        }

        private string DrawEditableIOSInfoPlistItem(iOSInfoPlistItem item)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField(item.Key, EditorStyles.label);
            item.Value = EditorGUILayout.TextField(item.Value);

            EditorGUILayout.EndVertical();

            return item.Value;
        }

        private void DrawAndroidPermission(string elementName, string value)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(elementName, EditorStyles.label, GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.LabelField(value, EditorStyles.label, GUILayout.ExpandWidth(true));

            EditorGUILayout.EndHorizontal();
        }

        #endregion

        private struct ArrayItemToolboxButtonResults
        {
            public bool isDeleteButton;
            public bool isMoveUpButton;
            public bool isMoveDownButton;
        }

        /// <summary>
        /// Draws the array element control toolbox with standard buttons (Move Up, Move Down, Delete).
        /// </summary>
        /// <param name="buttonResults">Button results.</param>
        /// <param name="foldout">Foldout.</param>
        /// <param name="allowMoveUp">If set to <c>true</c> allow move up.</param>
        /// <param name="allowMoveDown">If set to <c>true</c> allow move down.</param>
        static void DrawArrayElementControlToolbox(ref ArrayItemToolboxButtonResults buttonResults, bool foldout, bool allowMoveUp = true, bool allowMoveDown = true)
        {
            GUIContent deleteContent = EditorGUIUtility.IconContent("Toolbar Minus");
            GUIStyle deleteButtonStyle = new GUIStyle(GUIStyle.none)
            {
                fixedHeight = EM_GUIStyleManager.smallButtonHeight,
                fixedWidth = EM_GUIStyleManager.smallButtonWidth,
                padding = new RectOffset(2, 0, 4, 0),
            };
            GUIStyle moveButtonsStyle = new GUIStyle(GUIStyle.none)
            {
                fixedHeight = 20,
                fixedWidth = 20,
            };

            EditorGUILayout.BeginVertical(EM_GUIStyleManager.GetCustomStyle("Tool Box"),
                GUILayout.Width(EM_GUIStyleManager.toolboxWidth));

            if (foldout)
            {
                // Move up button.
                EditorGUI.BeginDisabledGroup(!allowMoveUp);
                if (GUILayout.Button(EM_GUIStyleManager.ArrowUp, moveButtonsStyle))
                {
                    buttonResults.isMoveUpButton = true;
                }
                EditorGUI.EndDisabledGroup();
            }

            // Delete button.
            if (GUILayout.Button(deleteContent, deleteButtonStyle))
            {
                // DeleteArrayElementAtIndex seems working fine even while iterating
                // through the array, but it's still a better idea to move it outside the loop.
                buttonResults.isDeleteButton = true;
            }

            // Move down button.
            if (foldout)
            {
                EditorGUI.BeginDisabledGroup(!allowMoveDown);
                if (GUILayout.Button(EM_GUIStyleManager.ArrowDown, moveButtonsStyle))
                {
                    buttonResults.isMoveDownButton = true;
                }
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws an array element with toolbox (Delete, Move Up & Move Down buttons).
        /// </summary>
        /// <param name="element">Element.</param>
        /// <param name="elementDrawer">Element drawer.</param>
        /// <param name="buttonResults">Button results.</param>
        /// <param name="allowMoveUp">If set to <c>true</c> allow move up.</param>
        /// <param name="allowMoveDown">If set to <c>true</c> allow move down.</param>
        static void DrawArrayElementWithToolbox(SerializedProperty element, Func<SerializedProperty, bool> elementDrawer, ref ArrayItemToolboxButtonResults buttonResults, bool allowMoveUp, bool allowMoveDown)
        {
            EditorGUILayout.BeginHorizontal();

            // Draw array element
            bool foldout = elementDrawer(element);

            // Draw control toolbox
            DrawArrayElementControlToolbox(ref buttonResults, foldout, allowMoveUp, allowMoveDown);

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws an array property, each element is associated with a standard toolbox with Delete, Move Up & Move Down buttons.
        /// </summary>
        /// <param name="property">Property.</param>
        /// <param name="elementDrawer">Element drawer.</param>
        static void DrawArrayProperty(SerializedProperty property, Func<SerializedProperty, bool> elementDrawer)
        {
            if (!property.isArray)
            {
                Debug.Log("Invalid argument. Require array property.");
                return;
            }

            // Index of the element on which buttons are clicked.
            int deleteIndex = -1;
            int moveUpIndex = -1;
            int moveDownIndex = -1;

            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty element = property.GetArrayElementAtIndex(i);

                var buttonResults = new ArrayItemToolboxButtonResults();
                buttonResults.isDeleteButton = false;
                buttonResults.isMoveUpButton = false;
                buttonResults.isMoveDownButton = false;

                DrawArrayElementWithToolbox(
                    element,
                    elementDrawer,
                    ref buttonResults,
                    i > 0,
                    i < property.arraySize - 1
                );

                if (buttonResults.isDeleteButton)
                    deleteIndex = i;
                if (buttonResults.isMoveUpButton)
                    moveUpIndex = i;
                if (buttonResults.isMoveDownButton)
                    moveDownIndex = i;
            }

            // Delete.
            if (deleteIndex >= 0)
            {
                property.DeleteArrayElementAtIndex(deleteIndex);
            }

            // Move up.
            if (moveUpIndex > 0)
            {
                property.MoveArrayElement(moveUpIndex, moveUpIndex - 1);
            }

            // Move down.
            if (moveDownIndex >= 0 && moveDownIndex < property.arraySize - 1)
            {
                property.MoveArrayElement(moveDownIndex, moveDownIndex + 1);
            }
        }

        static string DrawListAsPopup(GUIContent label, string[] values, string currentVal, params GUILayoutOption[] options)
        {
            var contents = new GUIContent[values.Length];

            for (int i = 0; i < values.Length; i++)
                contents[i] = new GUIContent(values[i]);

            // If the current value doesn't belong to the list, select the first value, which normally should be "None".
            int currentIndex = Mathf.Max(Array.IndexOf(values, currentVal), 0);
            int newIndex = EditorGUILayout.Popup(
                               label,
                               currentIndex,
                               contents,
                               options
                           );

            return values[newIndex];
        }
    }
}