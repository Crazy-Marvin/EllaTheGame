using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile
{
    /// <summary>
    /// All settings of the Media submodule of the Native APIs module.
    /// </summary>
    [Serializable]
    public class MediaApiSettings : IAndroidPermissionRequired, IIOSInfoItemRequired
    {
        #region IAndroidPermissionRequired implementation

        [SerializeField]
        private List<AndroidPermission> mAndroidPermissions = new List<AndroidPermission>()
        {
            new AndroidPermission(AndroidPermission.AndroidPermissionElementName, AndroidPermission.AndroidPermissionWriteExternalStorage),
            new AndroidPermission(AndroidPermission.AndroidFeatureElementName, AndroidPermission.AndroidHardwareCamera),
        };

        public List<AndroidPermission> GetAndroidPermissions()
        {
            return mAndroidPermissions;
        }

        #endregion

        #region IIOSInfoItemRequired implementation

        [SerializeField]
        private List<iOSInfoPlistItem> mIOSInfoPlistKeys = new List<iOSInfoPlistItem>
        {
            new iOSInfoPlistItem(iOSInfoPlistItem.NSPhotoLibraryUsageDescription),
            new iOSInfoPlistItem(iOSInfoPlistItem.NSPhotoLibraryAddUsageDescription),
            new iOSInfoPlistItem(iOSInfoPlistItem.NSCameraUsageDescription),
            new iOSInfoPlistItem(iOSInfoPlistItem.NSMicrophoneUsageDescription),
        };

        public List<iOSInfoPlistItem> GetIOSInfoPlistKeys()
        {
            return mIOSInfoPlistKeys;
        }

        #endregion
    }
}
