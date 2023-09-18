#if EASY_MOBILE_PRO
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using EasyMobile;

namespace EasyMobile.Editor
{
    // Partial editor class for Native APIs module.
    internal partial class EM_SettingsEditor
    {
        const string NativeApisModuleLabel = "NATIVE APIS";
        const string NativeApisModuleIntro = "The Native APIs module provides access to various native device APIs such as UI, Camera & Gallery, Contacts, etc.";
        const string MediaSubmoduleLabel = "CAMERA & GALLERY";
        const string ContactsSubmoduleLabel = "CONTACTS";
        const string MediaSectionKey = "NATIVE_API_CAMERA_GALLERY";
        const string ContactsSectionKey = "NATIVE_API_CONTACT";
        const string MediaSubmoduleIntroMsg = "Enable the Camera & Gallery submodule to gain access to the device camera for taking photos or recording video clips.";
        const string ContactsSubmoduleIntroMsg = "Enable the Contacts submodule to gain access to the device contacts for operations including fetching, adding and removing contacts.";

        private CompositeModuleManager mNativeApisModuleManager;

        private SerializedProperty IsMediaSubmoduleEnableProperty
        {
            get { return NativeApisProperties.isMediaEnabled.property; }
        }

        private SerializedProperty IsContactsSubmoduleEnableProperty
        {
            get { return NativeApisProperties.isContactsEnabled.property; }
        }

        private CompositeModuleManager NativeApisModuleManager
        {
            get
            { 
                if (mNativeApisModuleManager == null)
                    mNativeApisModuleManager = EM_PluginManager.GetModuleManager(Module.NativeApis) as CompositeModuleManager;
                return mNativeApisModuleManager; 
            }
            
        }

        private bool IsMediaSubmoduleEnabled
        {
            get
            { 
                return IsMediaSubmoduleEnableProperty != null ? IsMediaSubmoduleEnableProperty.boolValue : false; 
            }
            set
            {
                if (IsMediaSubmoduleEnableProperty == null || IsMediaSubmoduleEnableProperty.boolValue == value)
                    return;

                IsMediaSubmoduleEnableProperty.boolValue = value;

                if (NativeApisModuleManager != null)
                {
                    if (value)
                        NativeApisModuleManager.EnableSubmodule(Submodule.Media);
                    else
                        NativeApisModuleManager.DisableSubmodule(Submodule.Media);
                }
            }
        }

        private bool IsContactSubmoduleEnabled
        {
            get
            { 
                return IsContactsSubmoduleEnableProperty != null ? IsContactsSubmoduleEnableProperty.boolValue : false;
            }
            set
            {
                if (IsContactsSubmoduleEnableProperty == null || IsContactsSubmoduleEnableProperty.boolValue == value)
                    return;

                IsContactsSubmoduleEnableProperty.boolValue = value;

                if (NativeApisModuleManager != null)
                {
                    if (value)
                        NativeApisModuleManager.EnableSubmodule(Submodule.Contacts);
                    else
                        NativeApisModuleManager.DisableSubmodule(Submodule.Contacts);
                }
            }
        }

        private void NativeApiModuleGUI()
        {
            DrawModuleHeader();
            DrawMediaGUI();
            DrawContactsUI();
        }

        private void DrawMediaGUI()
        {
            EditorGUILayout.Space();

            IsMediaSubmoduleEnabled = DrawUppercaseSectionWithToggle(MediaSectionKey, MediaSubmoduleLabel, IsMediaSubmoduleEnabled, () =>
                {
                    if (IsMediaSubmoduleEnabled)
                    {
                        DrawAndroidPermissionsRequiredByNativeApisSubmodule(Submodule.Media);
                        EditorGUILayout.Space();
                        DrawIOSInfoPlistItemsRequiredByNativeApisSubmodule(Submodule.Media);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox(MediaSubmoduleIntroMsg, MessageType.Info);
                    }
                });
        }

        private void DrawContactsUI()
        {
            EditorGUILayout.Space();

            IsContactSubmoduleEnabled = DrawUppercaseSectionWithToggle(ContactsSectionKey, ContactsSubmoduleLabel, IsContactSubmoduleEnabled, () =>
                {
                    if (IsContactSubmoduleEnabled)
                    {
                        DrawAndroidPermissionsRequiredByNativeApisSubmodule(Submodule.Contacts);
                        EditorGUILayout.Space();
                        DrawIOSInfoPlistItemsRequiredByNativeApisSubmodule(Submodule.Contacts);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox(ContactsSubmoduleIntroMsg, MessageType.Info);
                    }
                });
        }

        private void DrawAndroidPermissionsRequiredByNativeApisSubmodule(Submodule submob)
        {
            if (NativeApisModuleManager != null)
            {
                var permHolder = NativeApisModuleManager.AndroidPermissionHolderForSubmodule(submob);

                if (permHolder != null)
                    DrawAndroidPermissionsRequiredSubsection(permHolder.GetAndroidPermissions(), new GUIContent("Required Android Permissions"));
            }
        }

        private void DrawIOSInfoPlistItemsRequiredByNativeApisSubmodule(Submodule submod)
        {
            if (NativeApisModuleManager != null)
            {
                var itemHolder = NativeApisModuleManager.iOSInfoItemsHolderForSubmodule(submod);

                if (itemHolder != null)
                    DrawIOSInfoPlistItemsRequiredSubsection(itemHolder.GetIOSInfoPlistKeys(), new GUIContent("Required iOS Info.plist Keys"));
            }
        }
    }
}
#endif