using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EasyMobile.Editor
{
    internal partial class EM_SettingsEditor
    {
        private void DrawSectionSelectionGUI()
        {
            if (!callFromEditorWindow)
                return;

            EditorGUILayout.BeginHorizontal(EM_GUIStyleManager.FullWidthBottomSeparatorBox);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(Section.Settings.ToString().ToUpper(), GetSectionStyle(Section.Settings)))
                SelectSection(Section.Settings);

            if (GUILayout.Button(Section.Build.ToString().ToUpper(), GetSectionStyle(Section.Build)))
                SelectSection(Section.Build);

            if (GUILayout.Button(Section.Tools.ToString().ToUpper(), GetSectionStyle(Section.Tools)))
                SelectSection(Section.Tools);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Makes a section active so its UI is gonna be drawn.
        /// </summary>
        /// <param name="section">Section.</param>
        private void SelectSection(Section section)
        {
            if (activeSection != section)
                activeSection = section;
        }

        private GUIStyle GetSectionStyle(Section section)
        {
            return section != activeSection ? EM_GUIStyleManager.GetCustomStyle("Tab Label") : EM_GUIStyleManager.GetCustomStyle("Tab Label Selected");
        }
    }
}
