using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace EasyMobile.Editor
{
    internal class ModuleManager_Privacy : ModuleManager
    {
        #region Singleton

        private static ModuleManager_Privacy sInstance;

        private ModuleManager_Privacy()
        {
        }

        public static ModuleManager_Privacy Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = new ModuleManager_Privacy();
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
                return null;
            }
        }

        public override IAndroidPermissionRequired AndroidPermissionsHolder
        {
            get
            {
                return EM_Settings.Privacy as IAndroidPermissionRequired;
            }
        }

        public override IIOSInfoItemRequired iOSInfoItemsHolder
        {
            get
            {
                return EM_Settings.Privacy as IIOSInfoItemRequired;
            }
        }

        public override Module SelfModule
        {
            get
            {
                return Module.Privacy;
            }
        }

        #endregion
    }
}
