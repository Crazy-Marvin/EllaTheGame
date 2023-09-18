using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EasyMobile.Editor
{
    internal partial class EM_SettingsEditor
    {
        private const string AndroidPermissionsIntroMsg = "This section displays the Android permissions and features required by each active module. These values will be added to the AndroidManifest.xml of the exported APK or AndroidStudio project.";
        private const string iOSInfoPlistKeysIntroMsg = "This section displays the keys to be added to the Info.plist of the exported Xcode project. The values shown here are for summary purpose and are not editable. To modify them " +
                                                        "please go to the settings page of the corresponding module.";
        private const string AutoInitializationOnMsg = "Easy Mobile runtime will initialize itself automatically as soon as the app launches. If you want to perform the initialization manually, disable this option and call the RuntimeManager.Init() method in your code.";
        private const string AutoInitializationOffMsg = "Auto Runtime Initialization is off. Make sure you call RuntimeManager.Init() before using Easy Mobile API.";
        private const string AndroidPermissionGUIKey = "ANDROID_PERMISSIONS_BUILD_";
        private const string IOSUsageDescriptionGUIKey = "IOS_USAGE_DESCRIPTIONS_READONLY";
        private const string AutoInitializationGUIKey = "AUTO_EM_INITIALIZATION";
        
        private void DrawBuildSectionGUI()
        {
            GUILayout.Space(20);
            
            // Auto initialization setting
            DrawUppercaseSection(AndroidPermissionGUIKey, "RUNTIME MANAGER", () =>
            {
                string msg = AutoInitializationOnMsg;
                if (!autoInitializationProperty.boolValue)
                    msg = AutoInitializationOffMsg;
                EditorGUILayout.HelpBox(msg, MessageType.Info);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Auto Runtime Initialization", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(autoInitializationProperty, new GUIContent("Auto Initialization"));
            });
            EditorGUILayout.Space();

            // Android Permissions GUI.
            DrawUppercaseSection(AndroidPermissionGUIKey, "REQUIRED ANDROID PERMISSIONS", () =>
                {
                    EditorGUILayout.HelpBox(AndroidPermissionsIntroMsg, MessageType.Info);

                    foreach (var pair in EM_PluginManager.GetAllAndroidPermissionsRequired())
                        DrawAndroidPermissionForModule(pair.Key, pair.Value);
                });

            EditorGUILayout.Space();

            // iOS Usage Descriptions GUI.
            DrawUppercaseSection(IOSUsageDescriptionGUIKey, "REQUIRED IOS INFO.PLIST KEYS", () =>
                {
                    EditorGUILayout.HelpBox(iOSInfoPlistKeysIntroMsg, MessageType.Info);

                    foreach (var pair in EM_PluginManager.GetAllIOSInfoItemsRequired())
                        DrawReadonlyIOSInfoPlistItemsRequiredForModule(pair.Key, pair.Value);
                });
        }

        private void DrawReadonlyIOSInfoPlistItemsRequiredForModule(Module module, List<iOSInfoPlistItem> plistItems)
        {
            if (plistItems == null)
                return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField(module.ToString(), EditorStyles.boldLabel);

            foreach (var item in plistItems)
                DrawReadonlyIOSInfoPlistItem(item);

            EditorGUILayout.EndVertical();
        }

        private void DrawAndroidPermissionForModule(Module module, List<AndroidPermission> permissions)
        {
            if (permissions == null)
                return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField(module.ToString(), EditorStyles.boldLabel);

            foreach (var permission in permissions)
                DrawAndroidPermission(permission.ElementName, permission.Value);

            EditorGUILayout.EndVertical();
        }

    }
}
