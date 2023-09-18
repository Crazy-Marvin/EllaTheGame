using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace EasyMobile.Editor
{
    internal class ModuleManager_Utilities : ModuleManager
    {
        #region Singleton

        private static ModuleManager_Utilities sInstance;

        private ModuleManager_Utilities()
        {
        }

        public static ModuleManager_Utilities Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = new ModuleManager_Utilities();
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
                return Module.Utilities;
            }
        }

        #endregion
    }
}
