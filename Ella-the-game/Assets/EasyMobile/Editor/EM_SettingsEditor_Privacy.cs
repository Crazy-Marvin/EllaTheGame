using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEditor;

using EGL = UnityEditor.EditorGUILayout;
using SP = UnityEditor.SerializedProperty;
using ES = UnityEditor.EditorStyles;
using E_SM = EasyMobile.Editor.EM_GUIStyleManager;

namespace EasyMobile.Editor
{
    internal partial class EM_SettingsEditor
    {
        #region Fields and Properties

        const string PrivacyModuleLabel = "PRIVACY";
        const string PrivacyModuleIntro = "The Privacy module provides convenient tools and resources that help with getting compliant with " +
                                          "user privacy regulations such as GDPR.";
        const string ContentDescription = "The main content description of the dialog. You can use HTML tags like <b>, <i> and <a> in the text as well as " +
                                          "insert toggles and buttons to form the dialog layout.";
        const string TogglesArrayDescription = "The toggles to be inserted to the dialog content.";
        const string ButtonsArrayDescription = "The buttons to be inserted to the dialog content. Your dialog must have at least one button.";
        const string PreviewSectionDescription = "Preview the consent dialog using the Privacy demo scene.";
        const string EmptySelectedTextMsg = "Please select some texts by highlighting them first.";

        const string BoldStartTag = "<b>";
        const string BoldEndTag = "</b>";
        const string ItalicStartTag = "<i>";
        const string ItalicEndTag = "</i>";
        const string HyperlinkPrefix = "http://";
        const int CharacterPixelWidth = 7;
        const int MinTogglesCount = 0;
        const int MinActionButtonsCount = 1;

        static Dictionary<string, bool> privacyFoldoutStates = new Dictionary<string, bool>();

        ConsentDialog DefaultConsentDialog
        {
            get { return EM_Settings.Privacy.DefaultConsentDialog; }
        }

        int SelectedToggleIndex
        {
            get { return PrivacyProperties.selectedToggleIndex.property.intValue; }
            set { PrivacyProperties.selectedToggleIndex.property.intValue = value; }
        }

        int SelectedButtonIndex
        {
            get { return PrivacyProperties.selectedButtonIndex.property.intValue; }
            set { PrivacyProperties.selectedButtonIndex.property.intValue = value; }
        }

        bool EnableCopyPasteMode
        {
            get { return PrivacyProperties.enableCopyPasteMode.property.boolValue; }
            set { PrivacyProperties.enableCopyPasteMode.property.boolValue = value; }
        }

        bool isHyperlinkButtonClicked;
        string hyperlinkSavedText, savedLink;
        int hyperLinkSavedStartIndex, hyperlinkSavedEndIndex;

        private string textAreaControlName = "ConsentDialogTextArea";
        private bool loadedIndexes;
        private int saveSelectIndex, saveCursorIndex;
        private Vector2 mainContentTextAreaScroll;
        private static bool togglesFoldout, buttonsFoldout;

        #endregion

        void PrivacyModuleGUI()
        {
            DrawModuleHeader();
            EGL.Space();
            DrawModuleMainGUI();
        }

        void DrawModuleMainGUI()
        {
            DrawUppercaseSection("CONSENT_DIALOG_COMPOSER_FOLDOUT_KEY", "DEFAULT CONSENT DIALOG COMPOSER", () =>
                {
                    // Dialog title.
                    EGL.Space();
                    EGL.LabelField("Dialog Title", ES.boldLabel);
                    EGL.PropertyField(PrivacyProperties.consentDialogTitle.property);

                    // Draw the main content.
                    EGL.Space();
                    EGL.LabelField("Main Content", ES.boldLabel);
                    EGL.LabelField(ContentDescription, ES.helpBox);
                    DrawConsentDialogContentEditor();

                    // Draw the toggles array.
                    SP togglesArray = PrivacyProperties.consentDialogToggles.property;

                    if (togglesArray.arraySize < MinTogglesCount)
                        togglesArray.arraySize += MinTogglesCount - togglesArray.arraySize;

                    EGL.Space();
                    EGL.LabelField("Toggle List", ES.boldLabel);
                    EGL.LabelField(TogglesArrayDescription, ES.helpBox);
                    togglesFoldout = DrawResizableArray(togglesArray, togglesFoldout, "Toggles", DrawToggleArrayElement, UpdateNewTogglePropertyInfo);

                    // Draw the buttons array.
                    SP buttonsArray = PrivacyProperties.consentDialogActionButtons.property;

                    if (buttonsArray.arraySize < MinActionButtonsCount)
                        buttonsArray.arraySize += MinActionButtonsCount - buttonsArray.arraySize;

                    EGL.Space();
                    EGL.LabelField("Button List", ES.boldLabel);
                    EGL.LabelField(ButtonsArrayDescription, ES.helpBox);
                    buttonsFoldout = DrawResizableArray(buttonsArray, buttonsFoldout, "Buttons", DrawActionButtonArrayElement, UpdateNewButtonPropertyInfo);

                    // Preview button.
                    EGL.Space();
                    EGL.LabelField("Preview", ES.boldLabel);
                    EGL.LabelField(PreviewSectionDescription, ES.helpBox);
                    if (GUILayout.Button("Run Preview Scene") && !EditorApplication.isPlaying)
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        {
                            EditorSceneManager.OpenScene(EM_Constants.DemoPrivacyScenePath);
                            EditorApplication.isPlaying = true;
                        }
                    }
                });
        }

        #region Dialog editor

        void DrawConsentDialogContentEditor()
        {
            // TextEditor for selecting and editing portions of text.
            TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
            SP dialog = PrivacyProperties.consentDialogContent.property;

            /// Fix error that make indexes reset to 0 when lose focus.
            int startIndex, endIndex;
            FixIndexesResetError(textEditor, out startIndex, out endIndex);

            // TextEditor inherently requires GUILayout.TextArea which doesn't work with copy & paste.
            // To enable copy & paste we'll need to use EditorGUILayout.TextArea which will make
            // text selection unavailable.
            // Since we can't have both things at the same time, we need a toggle to switch between them.
            EnableCopyPasteMode = EGL.ToggleLeft(PrivacyProperties.enableCopyPasteMode.content, EnableCopyPasteMode);

            /// These functions are not available when enabling copy & paste.
            if (!EnableCopyPasteMode)
            {
                EGL.BeginHorizontal();

                EGL.BeginHorizontal(ES.helpBox);
                DrawHyperlinkButton(textEditor.SelectedText, startIndex, endIndex);
                DrawEditButtons(textEditor.SelectedText, dialog, startIndex, endIndex);
                EGL.EndHorizontal();

                // Draw insert toggle dropdown.
                DrawInsertDropdown(dialog, endIndex, textEditor, "ToggleDropdown",
                    DefaultConsentDialog.GetAllToggleIds(), "Insert Toggle", ConsentDialog.TogglePattern,
                    () => SelectedToggleIndex, param => SelectedToggleIndex = param);

                // Draw insert button dropwdown.
                DrawInsertDropdown(dialog, endIndex, textEditor, "ButtonDropDown",
                    DefaultConsentDialog.GetAllButtonIds(), "Insert Button", ConsentDialog.ButtonPattern,
                    () => SelectedButtonIndex, param => SelectedButtonIndex = param);

                EGL.EndHorizontal();
            }
            else
            {
                EGL.Space();
            }

            DrawHyperLinkInputSection(dialog);

            /// We need to do this because GUILayout.TextArea, the only TextArea that work with TextEditor, doesn't work with copy & paste.
            GUIStyle style = new GUIStyle(ES.textArea)
            {
                wordWrap = true
            };

            mainContentTextAreaScroll = GUILayout.BeginScrollView(mainContentTextAreaScroll,
                false, false, GUILayout.Height(EditorGUIUtility.singleLineHeight * 10));

            GUI.SetNextControlName(textAreaControlName);
            dialog.stringValue = EnableCopyPasteMode ?
                EGL.TextArea(dialog.stringValue, style, GUILayout.ExpandHeight(true)) :
                GUILayout.TextArea(dialog.stringValue, style, GUILayout.ExpandHeight(true));

            EGL.EndScrollView();
        }

        void DrawEditTextButtons(string label, GUIStyle buttonStyle, Action callback)
        {
            if (GUILayout.Button(label, buttonStyle, GUILayout.MinWidth(E_SM.smallButtonWidth), GUILayout.ExpandHeight(true)))
            {
                callback();
            }
        }

        void DrawEditButtons(string selectedText, SP dialog, int startIndex, int endIndex)
        {
            Action editBoldTag = () => EditTagInProperty(dialog, startIndex, endIndex, BoldStartTag, BoldEndTag);
            Action editItalicTag = () => EditTagInProperty(dialog, startIndex, endIndex, ItalicStartTag, ItalicEndTag);
            DrawEditTextButtons("B", GetBoldButtonStyle(selectedText), editBoldTag);
            DrawEditTextButtons("I", GetItalicButtonStyle(selectedText), editItalicTag);
        }

        void DrawHyperlinkButton(string selectedText, int startIndex, int endIndex)
        {
            if (GUILayout.Button(isHyperlinkButtonClicked ? E_SM.HyperlinkIconActive : E_SM.HyperlinkIcon,
                    GetHyperlinkButtonStyle(),
                    GUILayout.MinWidth(E_SM.smallButtonWidth),
                    GUILayout.MinHeight(15f)))
            {
                if (!isHyperlinkButtonClicked)
                {
                    if (string.IsNullOrEmpty(selectedText))
                    {
                        Debug.Log(EmptySelectedTextMsg);
                        return;
                    }

                    isHyperlinkButtonClicked = true;
                    hyperlinkSavedText = selectedText;
                    hyperLinkSavedStartIndex = startIndex;
                    hyperlinkSavedEndIndex = endIndex;
                }
                else
                {
                    isHyperlinkButtonClicked = false;
                }
            }
        }

        void DrawHyperLinkInputSection(SP dialog)
        {
            if (!isHyperlinkButtonClicked)
                return;

            if (string.IsNullOrEmpty(hyperlinkSavedText) || hyperLinkSavedStartIndex < 0 || hyperlinkSavedEndIndex < 0)
                return;

            EGL.BeginVertical(ES.helpBox);
            EGL.LabelField("Selected Text:", ES.miniBoldLabel);
            EGL.LabelField(hyperlinkSavedText, ES.helpBox);
            EGL.LabelField("Link:", ES.miniBoldLabel);
            savedLink = EGL.TextField(string.IsNullOrEmpty(savedLink) ? HyperlinkPrefix : savedLink);
            if (GUILayout.Button("Insert hyperlink"))
            {
                string newText = string.Format("<a href=\"{0}\">{1}</a>", savedLink, hyperlinkSavedText);
                ReplaceTextInProperty(dialog, hyperLinkSavedStartIndex, hyperlinkSavedEndIndex, newText);
                isHyperlinkButtonClicked = false;
            }
            EGL.EndVertical();
            EGL.Space();
        }

        void DrawInsertDropdown(SP dialog, int cursorIndex, TextEditor textEditor, string name,
                                List<string> ids, string insertButtonText, string pattern, Func<int> getSelectedIndex, Action<int> setSelectedIndex)
        {
            if (ids == null || ids.Count < 1)
                return;

            EGL.BeginHorizontal(ES.helpBox);

            float insertButtonWidth = insertButtonText.Length * CharacterPixelWidth;
            if (GUILayout.Button(insertButtonText, ES.miniButton, GUILayout.MinWidth(insertButtonWidth), GUILayout.ExpandHeight(true)))
            {
                string id = ids[getSelectedIndex()];
                string insertText = pattern + "Id = " + id + ">";
                dialog.stringValue = dialog.stringValue.Insert(cursorIndex, insertText);

                Debug.Log(string.Format("Inserted {0} into content at index: {1}", insertText, cursorIndex));
            }

            string[] displayIdsText = ids.Select(id => id = "Id: " + id).ToArray();
            float maxPopupWidth = displayIdsText[getSelectedIndex()].Length * CharacterPixelWidth;
            GUIStyle popupStyle = new GUIStyle(ES.popup)
            {
                alignment = TextAnchor.MiddleCenter
            };
            GUI.SetNextControlName(name);
            int index = EGL.Popup(
                                getSelectedIndex(),
                                displayIdsText,
                                popupStyle,
                                GUILayout.MinWidth(maxPopupWidth),
                                GUILayout.MinHeight(GUILayoutUtility.GetLastRect().height)
                            );

            setSelectedIndex(index);
            EGL.EndHorizontal();
        }

        /// <summary>
        /// Draws the resizable array.
        /// </summary>
        /// <param name="arrayProperty">Array property.</param>
        /// <param name="drawElement">Action to draw target property.</param>
        /// <param name="updateNewElement">
        /// Action to update new element's values 
        /// so it won't have same values with the previous one when created.
        /// </param>
        bool DrawResizableArray(SP arrayProperty, bool foldout, string foldoutLabel, Action<SP, int> drawElement, Action<SP> updateNewElement = null, bool noLabel = false, int maxSize = int.MaxValue)
        {
            /// Draw array name and a button to add new element into array.
            GUILayout.BeginHorizontal();

            EditorGUI.indentLevel++;
            foldout = EGL.Foldout(foldout, noLabel ? "" : foldoutLabel, true);
            EditorGUI.indentLevel--;

            GUIContent plusButtonContent = EditorGUIUtility.IconContent("Toolbar Plus");
            GUIStyle plusButtonStyle = GUIStyle.none;
            GUILayoutOption plusButtonMinWidth = GUILayout.MaxWidth(E_SM.smallButtonWidth);
            if (GUILayout.Button(plusButtonContent, plusButtonStyle, plusButtonMinWidth) && arrayProperty.arraySize < maxSize)
            {
                arrayProperty.arraySize++;

                /// Update new property so it won't have same values with the previous one.
                if (updateNewElement != null)
                    updateNewElement(arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1));

                /// We should expand the array so user can see
                /// the element that has just been added.
                foldout = true;
            }
            GUILayout.EndHorizontal();

            /// Draw all array's elements.
            if (!foldout || arrayProperty.arraySize <= 0)
                return foldout;

            for (int i = 0; i < arrayProperty.arraySize; i++)
            {
                drawElement(arrayProperty, i);
                if (i < arrayProperty.arraySize - 1)
                    EGL.Space();
            }

            EGL.Space();
            return foldout;
        }

        void DrawToggleArrayElement(SP dialog, int elementIndex)
        {
            DrawArrayElement(dialog, elementIndex, "", MinTogglesCount,
                () => SelectedToggleIndex,
                param => SelectedToggleIndex = param,
                obj =>
                {
                    var shouldToggleContent = new GUIContent("Toggle Description", "Change description when toggle is updated.");
                    var shouldToggleOnDescriptionContent = new GUIContent("Toggle On Description",
                                                               "This description will be displayed if the toggle value is true.");
                    var shouldToggleOffDescriptionContent = new GUIContent("Description",
                                                                "This description will be displayed regardless of the toggle value.");
                    var offDescriptionContent = new GUIContent("Toggle Off Description",
                                                    "This description will be displayed if the toggle value is false.");

                    var title = obj.FindPropertyRelative("title");

                    string key = obj.propertyPath;
                    if (!privacyFoldoutStates.ContainsKey(key))
                        privacyFoldoutStates.Add(key, false);

                    EditorGUI.indentLevel++;
                    string titleValue = !string.IsNullOrEmpty(title.stringValue) ? title.stringValue : "[Untitled Toggle]";
                    privacyFoldoutStates[key] = EGL.Foldout(privacyFoldoutStates[key], titleValue, true);
                    EditorGUI.indentLevel--;

                    if (privacyFoldoutStates[key])
                    {
                        EGL.PropertyField(obj.FindPropertyRelative("id"));
                        EGL.PropertyField(title);
                        EGL.PropertyField(obj.FindPropertyRelative("isOn"));
                        EGL.PropertyField(obj.FindPropertyRelative("interactable"));

                        var shouldToggle = obj.FindPropertyRelative("shouldToggleDescription");
                        EGL.PropertyField(shouldToggle, shouldToggleContent);

                        // On description.
                        var onDescription = obj.FindPropertyRelative("onDescription");
                        onDescription.stringValue = EGL.TextField(
                            shouldToggle.boolValue ? shouldToggleOnDescriptionContent : shouldToggleOffDescriptionContent,
                            onDescription.stringValue,
                            new GUIStyle(ES.textField) { wordWrap = true },
                            GUILayout.Height(EditorGUIUtility.singleLineHeight * 3));

                        if (shouldToggle.boolValue)
                        {
                            var offDescription = obj.FindPropertyRelative("offDescription");
                            offDescription.stringValue = EGL.TextField(
                                offDescriptionContent,
                                offDescription.stringValue,
                                new GUIStyle(ES.textField) { wordWrap = true },
                                GUILayout.Height(EditorGUIUtility.singleLineHeight * 3));
                        }
                    }
                });
        }

        void DrawActionButtonArrayElement(SP dialog, int elementIndex)
        {
            DrawArrayElement(dialog, elementIndex, "Consent dialog should have at least 1 button.", MinActionButtonsCount,
                () => SelectedButtonIndex,
                param => SelectedButtonIndex = param,
                obj =>
                {
                    var title = obj.FindPropertyRelative("title");

                    EditorGUI.indentLevel++;
                    string key = obj.propertyPath;

                    if (!privacyFoldoutStates.ContainsKey(key))
                        privacyFoldoutStates.Add(key, false);

                    string titleValue = !string.IsNullOrEmpty(title.stringValue) ? title.stringValue : "[Untitled Button]";
                    privacyFoldoutStates[key] = EGL.Foldout(privacyFoldoutStates[key], titleValue, true);
                    EditorGUI.indentLevel--;

                    if (privacyFoldoutStates[key])
                    {
                        EGL.PropertyField(obj.FindPropertyRelative("id"));
                        EGL.PropertyField(title);
                        EGL.PropertyField(obj.FindPropertyRelative("interactable"));
                        EGL.PropertyField(obj.FindPropertyRelative("titleColor"));
                        EGL.PropertyField(obj.FindPropertyRelative("backgroundColor"));
                        EGL.PropertyField(obj.FindPropertyRelative("uninteractableTitleColor"));
                        EGL.PropertyField(obj.FindPropertyRelative("uninteractableBackgroundColor"));
                    }
                });
        }

        /// <summary>
        /// Draw an element inside the target array property.
        /// </summary>
        /// <param name="arrayProperty">Array property.</param>
        /// <param name="elementIndex">Element index to get the element out of the array property.</param>
        /// <param name="getSelectedIndex">Get selected index.</param>
        /// <param name="setSelectedIndex">Set selected index.</param>
        /// <param name="drawInnerProperties">Action to draw inner properties of the element property.</param>
        void DrawArrayElement(SP arrayProperty, int elementIndex, string minSizeWarning, int minSize,
                              Func<int> getSelectedIndex, Action<int> setSelectedIndex, Action<SP> drawInnerProperties)
        {
            var elementProperty = arrayProperty.GetArrayElementAtIndex(elementIndex);
            
            EGL.BeginHorizontal();

            EGL.BeginVertical(E_SM.GetCustomStyle("Item Box"));

            if (drawInnerProperties != null)
                drawInnerProperties(elementProperty);

            EGL.EndVertical();

            var deleteIcon = EditorGUIUtility.IconContent("Toolbar Minus");
            var deleteButtonStyle = GUIStyle.none;
            var deleteButtonMinWidth = GUILayout.MaxWidth(E_SM.smallButtonWidth);

            /// Draw a button to remove element property from array property.
            bool canDelete = arrayProperty.arraySize > minSize;
            EditorGUI.BeginDisabledGroup(!canDelete); // Disable the "-" button 
            if (GUILayout.Button(deleteIcon, deleteButtonStyle, deleteButtonMinWidth))
            {
                if (arrayProperty.arraySize > minSize)
                {
                    if (getSelectedIndex() > elementIndex)
                    {
                        setSelectedIndex(getSelectedIndex() - 1);
                    }

                    arrayProperty.DeleteArrayElementAtIndex(elementIndex);

                    /// We need to return here so the deleted element won't be displayed in the codes below,
                    /// causing unexpected error.
                    return;
                }

                if (!string.IsNullOrEmpty(minSizeWarning))
                    Debug.Log(minSizeWarning);
            }
            EditorGUI.EndDisabledGroup();

            EGL.EndHorizontal();
        }

        void UpdateNewTogglePropertyInfo(SP toggle)
        {
            var id = toggle.FindPropertyRelative("id");
            var title = toggle.FindPropertyRelative("title");
            var onDescription = toggle.FindPropertyRelative("onDescription");
            var offDescription = toggle.FindPropertyRelative("offDescription");
            var interactable = toggle.FindPropertyRelative("interactable");

            id.stringValue = "TG" + (Math.Abs(toggle.GetHashCode())).ToString();
            title.stringValue = String.Empty;
            onDescription.stringValue = String.Empty;
            offDescription.stringValue = String.Empty;
            interactable.boolValue = true;
        }

        void UpdateNewButtonPropertyInfo(SP button)
        {
            var id = button.FindPropertyRelative("id");
            var title = button.FindPropertyRelative("title");
            var interactable = button.FindPropertyRelative("interactable");
            var titleColor = button.FindPropertyRelative("titleColor");
            var backgroundColor = button.FindPropertyRelative("backgroundColor");
            var uninteractableTitleColor = button.FindPropertyRelative("uninteractableTitleColor");
            var uninteractableBackgroundColor = button.FindPropertyRelative("uninteractableBackgroundColor");

            id.stringValue = "BT" + (Math.Abs(button.GetHashCode())).ToString();
            title.stringValue = String.Empty;
            interactable.boolValue = true;
            titleColor.colorValue = Color.white;
            backgroundColor.colorValue = Color.gray;
            uninteractableTitleColor.colorValue = Color.white;
            uninteractableBackgroundColor.colorValue = new Color(.25f, .25f, .25f, 1.0f);
        }

        void ReplaceTextInProperty(SP dialog, int startIndex, int endIndex, string newText)
        {
            dialog.stringValue = dialog.stringValue.Remove(startIndex, endIndex - startIndex);
            dialog.stringValue = dialog.stringValue.Insert(startIndex, newText);
        }

        void EditTagInProperty(SP dialog, int startIndex, int endIndex, string startTag, string endTag)
        {
            if (endIndex <= startIndex)
            {
                Debug.Log(EmptySelectedTextMsg);
                return;
            }

            string selectedString = dialog.stringValue.Substring(startIndex, endIndex - startIndex);
            if (StringHasTag(selectedString, startTag, endTag))
            {
                RemoveTagFromString(dialog, startIndex, endIndex, startTag, endTag);
            }
            else
            {
                AddTagToProperty(dialog, startIndex, endIndex, startTag, endTag);
            }
        }

        void AddTagToProperty(SP dialog, int startIndex, int endIndex, string startTag, string endTag)
        {
            dialog.stringValue = dialog.stringValue.Insert(startIndex, startTag);
            dialog.stringValue = dialog.stringValue.Insert(endIndex + startTag.Length, endTag);
        }

        void RemoveTagFromString(SP property, int startIndex, int endIndex, string startTag, string endTag)
        {
            property.stringValue = property.stringValue.Remove(startIndex, startTag.Length);
            property.stringValue = property.stringValue.Remove(endIndex - startTag.Length - endTag.Length, endTag.Length);
        }

        bool StringHasTag(string targetString, string startTag, string endTag)
        {
            return !string.IsNullOrEmpty(targetString) && targetString.StartsWith(startTag) && targetString.EndsWith(endTag);
        }

        GUIStyle GetHyperlinkButtonStyle()
        {
            return new GUIStyle(ES.miniButton)
            {
                richText = true,
                padding = new RectOffset(-2, -2, -2, -2)
            };
        }

        GUIStyle GetBoldButtonStyle(string selectedString)
        {
            return new GUIStyle(StringHasTag(selectedString, BoldStartTag, BoldEndTag) ? ES.toolbarButton : ES.miniButtonLeft)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
            };
        }

        GUIStyle GetItalicButtonStyle(string selectedString)
        {
            return new GUIStyle(StringHasTag(selectedString, ItalicStartTag, ItalicEndTag) ? ES.toolbarButton : ES.miniButtonRight)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Italic,
            };
        }

        void FixIndexesResetError(TextEditor textEditor, out int startIndex, out int endIndex)
        {
            string focusedControlName = GUI.GetNameOfFocusedControl();

            if (string.IsNullOrEmpty(focusedControlName))
            {
                startIndex = Mathf.Min(textEditor.selectIndex, textEditor.cursorIndex);
                endIndex = Mathf.Max(textEditor.selectIndex, textEditor.cursorIndex);
                return;
            }

            if (focusedControlName.Equals(textAreaControlName))
            {
                if (!loadedIndexes)
                {
                    loadedIndexes = true;
                    textEditor.selectIndex = saveSelectIndex;
                    textEditor.cursorIndex = saveCursorIndex;

                    startIndex = saveSelectIndex;
                    endIndex = saveCursorIndex;
                }
                else
                {
                    startIndex = Mathf.Min(textEditor.selectIndex, textEditor.cursorIndex);
                    endIndex = Mathf.Max(textEditor.selectIndex, textEditor.cursorIndex);

                    saveSelectIndex = textEditor.selectIndex;
                    saveCursorIndex = textEditor.cursorIndex;
                }
            }
            else
            {
                loadedIndexes = false;
                startIndex = saveSelectIndex;
                endIndex = saveCursorIndex;

                textEditor.selectIndex = startIndex;
                textEditor.cursorIndex = endIndex;
            }
        }

        #endregion
    }
}
