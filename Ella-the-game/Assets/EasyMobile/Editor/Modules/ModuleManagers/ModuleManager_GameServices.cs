using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace EasyMobile.Editor
{
    internal class ModuleManager_GameServices : ModuleManager
    {
        #region Singleton

        private static ModuleManager_GameServices sInstance;

        private ModuleManager_GameServices()
        {
        }

        public static ModuleManager_GameServices Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = new ModuleManager_GameServices();
                return sInstance;
            }
        }

        #endregion

        #region implemented abstract members of ModuleManager

        protected override void InternalEnableModule()
        {
            // Check if Google Play Games plugin is available.
            bool isGPGSAvail = EM_ExternalPluginManager.IsGPGSAvail();
            if (isGPGSAvail)
            {
                // We won't use Google Play Game Services on iOS, so we'll define NO_GPGS symbol to disable it
                // (if needed, as newer versions of GPGS defines this themselves).
                GlobalDefineManager.SDS_AddDefine(EM_ScriptingSymbols.NoGooglePlayGames, BuildTargetGroup.iOS);

                // Define EM_GPGS symbol on Android platform
                GlobalDefineManager.SDS_AddDefine(EM_ScriptingSymbols.GooglePlayGames, BuildTargetGroup.Android);
            }
        }

        protected override void InternalDisableModule()
        {
            // Removed associated scripting symbols if any was defined.
            // Note that we won't remove the NO_GPGS symbol automatically on iOS.
            // Rather we'll let the user delete it manually if they want.
            // This helps prevent potential build issues on iOS due to GPGS dependencies.
            GlobalDefineManager.SDS_RemoveDefineOnAllPlatforms(EM_ScriptingSymbols.GooglePlayGames);
        }

        public override List<string> AndroidManifestTemplatePaths
        {
            get
            {
                return null;
            }
        }

        public override IAndroidPermissionRequired AndroidPermissionsHolder
        {
            get
            {
                return EM_Settings.GameServices as IAndroidPermissionRequired;
            }
        }

        public override IIOSInfoItemRequired iOSInfoItemsHolder
        {
            get
            {
                return EM_Settings.GameServices as IIOSInfoItemRequired;
            }
        }

        public override Module SelfModule
        {
            get
            {
                return Module.GameServices;
            }
        }

        #endregion
    }
}
