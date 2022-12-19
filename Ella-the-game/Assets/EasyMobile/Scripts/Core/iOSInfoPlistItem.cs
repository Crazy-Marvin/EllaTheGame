using UnityEngine;
using System.Collections;
using System;
using EasyMobile.Internal;

namespace EasyMobile
{
    /// <summary>
    /// iOS Info.plist key value pairs.
    /// </summary>
    [Serializable]
    public class iOSInfoPlistItem
    {
        #region Potential Keys used by us.

        public const string NSPhotoLibraryUsageDescription = "NSPhotoLibraryUsageDescription";
        public const string NSPhotoLibraryAddUsageDescription = "NSPhotoLibraryAddUsageDescription";
        public const string NSCameraUsageDescription = "NSCameraUsageDescription";
        public const string NSMicrophoneUsageDescription = "NSMicrophoneUsageDescription";
        public const string NSContactsUsageDescription = "NSContactsUsageDescription";

        #endregion

        public const string DefaultUsageDescription = "[Your usage description]";

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyMobile.iOSInfoPlistItem"/> class
        /// with the given key and value.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public iOSInfoPlistItem(string key, string value)
        {
            mKey = key;
            mValue = value; 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyMobile.iOSInfoPlistItem"/> class
        /// with the given key and the <see cref="DefaultUsageDescription"/>.
        /// </summary>
        /// <param name="key">Key.</param>
        public iOSInfoPlistItem(string key)
        {
            mKey = key;
            mValue = DefaultUsageDescription;
        }

        [SerializeField]
        private string mKey;
        [SerializeField]
        private string mValue = "";

        /// <summary>
        /// Info.plist key.
        /// </summary>
        /// <value>The key.</value>
        public string Key
        {
            get { return mKey; }
        }

        /// <summary>
        /// The value associated with this Info.plist key.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        public override bool Equals(object obj)
        {
            var other = obj as iOSInfoPlistItem;
            return other != null &&
            mKey == other.mKey &&
            mValue == other.mValue;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (Key != null ? Key.GetHashCode() : 0);
                hash = hash * 23 + (Value != null ? Value.GetHashCode() : 0);
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format("[iOSInfoPlistItem: Key={0}, Value={1}]", Key, Value);
        }
    }
}