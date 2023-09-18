using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EasyMobile.ManifestGenerator;
using EasyMobile.ManifestGenerator.Elements;

namespace EasyMobile.Editor
{
    public class AndroidManifestElementEditWindow : EditorWindow
    {
        public AndroidManifestElement EditingElement { get; set; }

        public AndroidManifestElement ManifestElement { get; set; }

        public GeneratableAndroidManifest GeneratableAndroidManifest { get; set; }

        public List<AndroidManifestElement> ManifestElementsFactory { get; set; }

        private bool CanGoBack
        {
            get
            {
                return EditingElement != ManifestElement;
            }
        }

        private int selectedElementStyleIndex = 0, selectedAttributeIndex = 0;
        private Vector2 scrollPosition;
        private GUIContent plusContent;
        private GUIContent minusContent;
        private GUIStyle miniButtonStyle;
        private int miniButtonSize = 22;

        public static void ShowWindow(SerializedProperty generatableManifestProperty, SerializedProperty manifestElementsFactory)
        {
            //Type inspectorType = Type.GetType("UnityEditor.InspectorWindow, UnityEditor.dll");
            var window = GetWindow<AndroidManifestElementEditWindow>(/*new Type[] { inspectorType }*/);

            var generatableManifest = generatableManifestProperty.GetTargetObject() as GeneratableAndroidManifest;
            window.GeneratableAndroidManifest = generatableManifest;
            window.ManifestElementsFactory = manifestElementsFactory.GetTargetObject() as List<AndroidManifestElement>;
            window.EditingElement = generatableManifest.ManifestElement;
            window.ManifestElement = generatableManifest.ManifestElement;
            window.titleContent = new GUIContent("Manifest", EM_GUIStyleManager.EasyMobileIcon, "Android Manifest Generation");
            window.Show();
        }

        private void OnGUI()
        {
            plusContent = EditorGUIUtility.IconContent("Toolbar Plus");
            minusContent = EditorGUIUtility.IconContent("Toolbar Minus");
            miniButtonStyle = new GUIStyle(GUIStyle.none)
            {
                fixedWidth = miniButtonSize,
                fixedHeight = miniButtonSize
            };

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            try
            {
                DrawNavigationHeader();
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                DrawAttributesGUI();
                DrawChildElementsGUI();

                EditorGUILayout.Space();
                if (GUILayout.Button("Generate Android Manifest"))
                    GenerateCustomManifest();
                EditorGUILayout.EndScrollView();
            }
            catch (Exception e)
            {
                EditorGUILayout.HelpBox(e.Message + "\n\nPlease restart this window.", MessageType.Error);
                Debug.Log(e);
            }

            EditorGUILayout.EndVertical();
        }

        private void GenerateCustomManifest()
        {
            GeneratableAndroidManifest.Save(EM_AndroidManifestBuilder.sCustomManifestPath, ManifestElementsFactory);
            EditorApplication.delayCall += () => EM_AndroidManifestBuilder.GenerateManifest(EM_EditorUtil.GetJdkPath(), true);
        }

        private void DrawAttributesGUI()
        {
            /// Label
            EditorGUILayout.LabelField("Attributes", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            /// Add attributes
            string[] remainedAttributes = EditingElement.RemainedAttributes.Select(attr => attr.ToString()).ToArray();
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(remainedAttributes == null || remainedAttributes.Length < 1);
            selectedAttributeIndex = EditorGUILayout.Popup(selectedAttributeIndex, remainedAttributes);
            if (GUILayout.Button(plusContent, miniButtonStyle))
            {
                EditingElement.AddAttribute(remainedAttributes[selectedAttributeIndex], "");
                selectedAttributeIndex = 0;
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            // Edit or remove attributes
            for (int i = 0; i < EditingElement.AttributesCount; i++)
            {
                EditorGUI.BeginDisabledGroup(EditingElement.AddedAttributesKey[i] == "xmlns:android");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(EditingElement.AddedAttributesKey[i]);
                EditingElement.AddedAttributesValue[i] = EditorGUILayout.TextField(EditingElement.AddedAttributesValue[i]);

                if (GUILayout.Button(minusContent, miniButtonStyle))
                {
                    EditingElement.AddedAttributesValue.RemoveAt(i);
                    EditingElement.AddedAttributesKey.RemoveAt(i);
                }

                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawChildElementsGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Child Elements", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (!EditingElement.CanAddChildElement)
            {
                var message = EditingElement.Style.ToAndroidManifestFormat() + " element can't contains any child element.";
                EditorGUILayout.HelpBox(message, MessageType.Info);
            }
            else
            {
                DrawAddChildElementGUI();
                DrawChildElementsInfoGUI(EditingElement);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawNavigationHeader()
        {
            if (CanGoBack)
            {
                if (GUILayout.Button(new GUIContent("Back To Root Element", EM_GUIStyleManager.HomeIcon), EM_GUIStyleManager.GetCustomStyle("Module Back Button Text")))
                    EditingElement = ManifestElement;
            }

            GUIStyle titleStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { fontSize = miniButtonSize };
            string navigationTitle = EditingElement.Style.ToAndroidManifestFormat().ToUpperInvariant();
            EditorGUILayout.LabelField(navigationTitle, titleStyle, GUILayout.MinHeight(40));
        }

        private void DrawAddChildElementGUI()
        {
            string[] availableStyles = EditingElement.ChildStyles.Select(style => "<" + style.ToAndroidManifestFormat() + ">").ToArray();

            EditorGUILayout.BeginHorizontal();
            selectedElementStyleIndex = EditorGUILayout.Popup(selectedElementStyleIndex, availableStyles);

            if (GUILayout.Button(plusContent, miniButtonStyle))
            {
                var element = EditingElement.ChildStyles.ToArray()[selectedElementStyleIndex].CreateElementClass();
                element.Id = element.GetHashCode();
                EditingElement.AddInnerElement(element);
                ManifestElementsFactory.Add(element);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawChildElementsInfoGUI(AndroidManifestElement target)
        {
            var childElements = target.GetChildElements(ManifestElementsFactory);

            if (childElements == null || childElements.Count < 1)
                return;

            for (int i = 0; i < childElements.Count; i++)
            {
                EditorGUI.indentLevel++;
                var childElement = childElements[i];

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField(childElement.Style.ToAndroidManifestFormat(), EditorStyles.boldLabel);

                if (GUILayout.Button("Edit", GUILayout.MaxWidth(50)))
                {
                    EditingElement = childElement;
                    ResetSelectionIndexes();
                }

                if (GUILayout.Button(minusContent, miniButtonStyle))
                {
                    childElements.Remove(childElement);
                    ManifestElementsFactory.Remove(childElement);
                    ResetSelectionIndexes();
                }

                EditorGUILayout.EndHorizontal();

                DrawChildElementsInfoGUI(childElement);
                EditorGUI.indentLevel--;
            }
        }

        private void ResetSelectionIndexes()
        {
            selectedElementStyleIndex = 0;
            selectedAttributeIndex = 0;
        }
    }
}
