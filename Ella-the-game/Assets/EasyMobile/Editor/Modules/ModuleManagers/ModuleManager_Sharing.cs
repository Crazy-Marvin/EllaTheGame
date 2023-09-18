using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace EasyMobile.Editor
{
    internal class ModuleManager_Sharing : ModuleManager
    {
        #region Singleton

        private static ModuleManager_Sharing sInstance;

        private ModuleManager_Sharing()
        {
        }

        public static ModuleManager_Sharing Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = new ModuleManager_Sharing();
                return sInstance;
            }
        }

        #endregion

        #region implemented abstract members of ModuleManager

        protected override void InternalEnableModule()
        {
            // Nothing.
        }

        protected override void InternalDisableModule()
        {
            // Nothing.
        }

        public override List<string> AndroidManifestTemplatePaths
        {
            get
            {
                return new List<string>() { FileIO.ToAbsolutePath("EasyMobile/Editor/Templates/Manifests/Manifest_Sharing.xml") };
            }
        }

        public override IAndroidPermissionRequired AndroidPermissionsHolder
        {
            get
            {
                return EM_Settings.Sharing as IAndroidPermissionRequired;
            }
        }

        public override IIOSInfoItemRequired iOSInfoItemsHolder
        {
            get
            {
                return EM_Settings.Sharing as IIOSInfoItemRequired;
            }
        }

        public override Module SelfModule
        {
            get
            {
                return Module.Sharing;
            }
        }

        #endregion
    }
}
