using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using EasyMobile;

namespace EasyMobile.Editor
{
    // Partial editor class for Sharing module.
    internal partial class EM_SettingsEditor
    {
        const string SharingModuleLabel = "SHARING";
        const string SharingModuleIntro = "The Sharing module makes it easy to share texts, images or URLs directly to social platforms such as Facebook and Twitter or via the native sharing popup.";
        const string iOSInfoPlistItemsInto = "The following Info.plist keys are needed when the user selects the \"Save Image\" option on the default native sharing popup, without them the app will crash.";

        private void SharingModuleGUI()
        {
            DrawModuleHeader();

            if (!isSharingModuleEnable.boolValue)
                return;

            DrawAndroidPermissionsRequiredSection(Module.Sharing);
            EditorGUILayout.Space();
            DrawIOSInfoPlistItemsRequiredSection(Module.Sharing, () =>
                {
                    EditorGUILayout.HelpBox(iOSInfoPlistItemsInto, MessageType.Info);
                });
        }
    }
}