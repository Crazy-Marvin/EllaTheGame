using UnityEngine;
using System.Collections;
using System;
using EasyMobile.Internal;
using System.Collections.Generic;

namespace EasyMobile
{
    /// <summary>
    /// Android permissions potentially used by Easy Mobile.
    /// </summary>
    [Serializable]
    public class AndroidPermission
    {
        #region Potential values used by us.

        public const string AndroidPermissionElementName = "uses-permission";
        public const string AndroidFeatureElementName = "uses-feature";

        public const string AndroidPermissionWriteExternalStorage = "android.permission.WRITE_EXTERNAL_STORAGE";
        public const string AndroidPermissionReadContacts = "android.permission.READ_CONTACTS";
        public const string AndroidPermissionWriteContacts = "android.permission.WRITE_CONTACTS";
        public const string AndroidPermissionReceiveBootCompleted = "android.permission.RECEIVE_BOOT_COMPLETED";
        public const string AndroidHardwareCamera = "android.hardware.camera";

        #endregion

        public AndroidPermission(string elementName, string value)
        {
            ElementName = elementName;
            Value = value;
        }

        [SerializeField]
        private string mElementName;
        [SerializeField]
        private string mValue = "";

        /// <summary>
        /// Name of the element used to store this value in AndroidManifest.
        /// </summary>
        /// <example>
        /// <see cref="AndroidPermissionElementName"/>, 
        /// <see cref="AndroidFeatureElementName"/>
        /// </example>
        public string ElementName
        {
            get { return mElementName; }
            set { mElementName = value; }
        }

        /// <summary>
        /// The value of the permission.
        /// </summary>
        public string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        public override bool Equals(object obj)
        {
            var other = obj as AndroidPermission;
            return other != null &&
            mElementName == other.mElementName &&
            mValue == other.mValue;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (ElementName != null ? ElementName.GetHashCode() : 0);
                hash = hash * 23 + (Value != null ? Value.GetHashCode() : 0);
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format("[AndroidPermission: ElementName={0}, Value={1}]", ElementName, Value);
        }
    }
}