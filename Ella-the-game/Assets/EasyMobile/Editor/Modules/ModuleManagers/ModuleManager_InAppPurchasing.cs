using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace EasyMobile.Editor
{
    internal class ModuleManager_InAppPurchasing : ModuleManager
    {
        #region Singleton

        private static ModuleManager_InAppPurchasing sInstance;

        private ModuleManager_InAppPurchasing()
        {
        }

        public static ModuleManager_InAppPurchasing Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = new ModuleManager_InAppPurchasing();
                return sInstance;
            }
        }

        #endregion

        #region implemented abstract members of ModuleManager

        protected override void InternalEnableModule()
        {
            // Check if UnityIAP is enable and act accordingly.
            if (EM_ExternalPluginManager.IsUnityIAPAvail())
            {
                // Generate dummy AppleTangle and GoogleTangle classes if they don't exist.
                // Note that AppleTangle and GooglePlayTangle only get compiled on following platforms,
                // therefore the compilational condition is needed, otherwise the code will repeat forever.
                #if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
                if (!EM_EditorUtil.AppleTangleClassExists())
                {
                    EM_EditorUtil.GenerateDummyAppleTangleClass();
                }

                if (!EM_EditorUtil.GooglePlayTangleClassExists())
                {
                    EM_EditorUtil.GenerateDummyGooglePlayTangleClass();
                }
                #endif

                GlobalDefineManager.SDS_AddDefineOnAllPlatforms(EM_ScriptingSymbols.UnityIAP);
            }
        }

        protected override void InternalDisableModule()
        {
            // Remove associated scripting symbol on all platforms it was defined.
            GlobalDefineManager.SDS_RemoveDefineOnAllPlatforms(EM_ScriptingSymbols.UnityIAP);
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
                return EM_Settings.InAppPurchasing as IAndroidPermissionRequired;
            }
        }

        public override IIOSInfoItemRequired iOSInfoItemsHolder
        {
            get
            {
                return EM_Settings.InAppPurchasing as IIOSInfoItemRequired;
            }
        }

        public override Module SelfModule
        {
            get
            {
                return Module.InAppPurchasing;
            }
        }

        #endregion
    }
}
