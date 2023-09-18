using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace EasyMobile.Editor
{
    internal class ModuleManager_Notifications : ModuleManager
    {
        #region Singleton

        private static ModuleManager_Notifications sInstance;

        private ModuleManager_Notifications()
        {
        }

        public static ModuleManager_Notifications Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = new ModuleManager_Notifications();
                return sInstance;
            }
        }

        #endregion

        #region implemented abstract members of ModuleManager

        protected override void InternalEnableModule()
        {
            // Define related symbols.
            var symbols = new List<string>();

            // Check if OneSignal is available.
            if (EM_ExternalPluginManager.IsOneSignalAvail())
                symbols.Add(EM_ScriptingSymbols.OneSignal);

            // Check if Firebase Messaging is available.
            if (EM_ExternalPluginManager.IsFirebaseMessagingAvail())
                symbols.Add(EM_ScriptingSymbols.FirebaseMessaging);

            // Define the symbols on all platforms.
            GlobalDefineManager.SDS_AddDefinesOnAllPlatforms(symbols.ToArray());
        }

        protected override void InternalDisableModule()
        {
            // Remove associated scripting symbol on all platforms it was defined.
            GlobalDefineManager.SDS_RemoveDefinesOnAllPlatforms(
                new string[] { EM_ScriptingSymbols.OneSignal, EM_ScriptingSymbols.FirebaseMessaging });
        }

        public override List<string> AndroidManifestTemplatePaths
        {
            get
            {
                return new List<string>() { FileIO.ToAbsolutePath("EasyMobile/Editor/Templates/Manifests/Manifest_Notifications.xml") };
            }
        }

        public override IAndroidPermissionRequired AndroidPermissionsHolder
        {
            get
            {
                return EM_Settings.Notifications as IAndroidPermissionRequired;
            }
        }

        public override IIOSInfoItemRequired iOSInfoItemsHolder
        {
            get
            {
                return EM_Settings.Notifications as IIOSInfoItemRequired;
            }
        }

        public override Module SelfModule
        {
            get
            {
                return Module.Notifications;
            }
        }

        #endregion
    }
}
