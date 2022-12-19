using UnityEngine;
using System;
using System.Collections.Generic;
using EasyMobile.Internal;

namespace EasyMobile
{
    [System.Serializable]
    public class SharingSettings : IAndroidPermissionRequired, IIOSInfoItemRequired
    {
        #region IAndroidPermissionRequired

        [SerializeField]
        private List<AndroidPermission> mAndroidPermissions = new List<AndroidPermission>()
        {
            new AndroidPermission(AndroidPermission.AndroidPermissionElementName, AndroidPermission.AndroidPermissionWriteExternalStorage)
        };

        public List<AndroidPermission> GetAndroidPermissions()
        {
            return mAndroidPermissions;
        }

        #endregion

        #region IIOSInfoItemRequired implementation

        [SerializeField]
        private List<iOSInfoPlistItem> mIOSInfoPlistItems = new List<iOSInfoPlistItem>()
        {
            new iOSInfoPlistItem(iOSInfoPlistItem.NSPhotoLibraryUsageDescription),
            new iOSInfoPlistItem(iOSInfoPlistItem.NSPhotoLibraryAddUsageDescription)
        };

        public List<iOSInfoPlistItem> GetIOSInfoPlistKeys()
        {
            return mIOSInfoPlistItems;
        }

        #endregion
    }
}

