using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile
{
    /// <summary>
    /// All settings of the Contacts submodule of the Native APIs module.
    /// </summary>
    [Serializable]
    public class ContactsApiSettings : IAndroidPermissionRequired, IIOSInfoItemRequired
    {
        #region IAndroidPermissionRequired implementation

        [SerializeField]
        private List<AndroidPermission> mAndroidPermissions = new List<AndroidPermission>()
        {
            new AndroidPermission(AndroidPermission.AndroidPermissionElementName, AndroidPermission.AndroidPermissionWriteContacts),
            new AndroidPermission(AndroidPermission.AndroidPermissionElementName, AndroidPermission.AndroidPermissionReadContacts)
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
            new iOSInfoPlistItem(iOSInfoPlistItem.NSContactsUsageDescription)
        };

        public List<iOSInfoPlistItem> GetIOSInfoPlistKeys()
        {
            return mIOSInfoPlistKeys;
        }

        #endregion
    }
}
