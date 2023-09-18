using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

#if EM_URP
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
#endif

namespace EasyMobile.Editor
{
    internal class ModuleManager_Gif : ModuleManager
    {
        #region Singleton

        private static ModuleManager_Gif sInstance;

        private ModuleManager_Gif()
        {
        }

        public static ModuleManager_Gif Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = new ModuleManager_Gif();
                return sInstance;
            }
        }

        #endregion

        #region implemented abstract members of ModuleManager

        protected override void InternalEnableModule()
        {
            List<string> symbols = new List<string>();

            if (EM_ExternalPluginManager.IsUsingURP())
            {
                symbols.Add(EM_ScriptingSymbols.UniversalRenderPipeline);
            }

            // Defines all ad symbols on all platforms.
            GlobalDefineManager.SDS_AddDefinesOnAllPlatforms(symbols.ToArray());
        }

        protected override void InternalDisableModule()
        {
            GlobalDefineManager.SDS_RemoveDefinesOnAllPlatforms(new string[] { EM_ScriptingSymbols.UniversalRenderPipeline });
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
                return null;
            }
        }

        public override IIOSInfoItemRequired iOSInfoItemsHolder
        {
            get
            {
                return null;
            }
        }

        public override Module SelfModule
        {
            get
            {
                return Module.Gif;
            }
        }

        #endregion
    }
}
