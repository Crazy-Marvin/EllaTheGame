using UnityEngine;
using System.Collections;
using System;
using EasyMobile.Internal;

namespace EasyMobile
{
    /// <summary>
    /// Generic cross-platform identifier for ad resources.
    /// </summary>
    [Serializable]
    public class CrossPlatformId
    {
        [SerializeField]
        [Rename("iOS ID")]
        protected string mIosId;
        [SerializeField]
        [Rename("Android ID")]
        protected string mAndroidId;

        /// <summary>
        /// Gets the ad ID corresponding to the current platform.
        /// Returns <c>string.Empty</c> if no ID was defined for this platform.
        /// </summary>
        /// <value>The identifier.</value>
        public virtual string Id
        {
            get
            {
#if UNITY_ANDROID
                return AndroidId;
#elif UNITY_IOS
                return IosId;
#else
                return string.Empty;
#endif
            }
        }

        /// <summary>
        /// Gets the ad ID for iOS platform.
        /// </summary>
        /// <value>The ios identifier.</value>
        public virtual string IosId { get { return mIosId; } }

        /// <summary>
        /// Gets the ad ID for Android platform.
        /// </summary>
        /// <value>The android identifier.</value>
        public virtual string AndroidId { get { return mAndroidId; } }

        public CrossPlatformId(string iOSId, string androidId)
        {
            this.mIosId = iOSId;
            this.mAndroidId = androidId;
        }

        public override string ToString()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            var item = obj as CrossPlatformId;

            if (item == null)
            {
                return false;
            }

            return this.Id.Equals(item.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(CrossPlatformId a, CrossPlatformId b)
        {
            if (ReferenceEquals(a, null))
                return ReferenceEquals(b, null);

            return a.Equals(b);
        }

        public static bool operator !=(CrossPlatformId a, CrossPlatformId b)
        {
            return !(a == b);
        }
    }
}