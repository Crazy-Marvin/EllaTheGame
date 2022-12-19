using UnityEngine;
using System;
using System.Collections.Generic;
using EasyMobile.Internal;

namespace EasyMobile
{
    [System.Serializable]
    public class NotificationsSettings : IAndroidPermissionRequired
    {
        public const string DEFAULT_CATEGORY_ID = "notification.category.default";
        public const string DEFAULT_CATEGORY_NAME = "Default";

        /// <summary>
        /// Whether the Notifications module should initialize itself automatically.
        /// </summary>
        /// <value><c>true</c> if is auto init; otherwise, <c>false</c>.</value>
        public bool IsAutoInit { get { return mAutoInit; } set { mAutoInit = value; } }

        /// <summary>
        /// The delay (in seconds) after the Easy Mobile runtime has been initialized that this module is initializes itself automatically.
        /// </summary>
        /// <value>The auto init delay.</value>
        public float AutoInitDelay { get { return mAutoInitDelay; } set { mAutoInitDelay = value; } }

        /// <summary>
        /// Gets or sets the iOS authentication options.
        /// </summary>
        /// <value>The i OSA uth options.</value>
        public NotificationAuthOptions iOSAuthOptions { get { return mIosAuthOptions; } set { mIosAuthOptions = value; } }

        /// <summary>
        /// Gets or sets the push notification service.
        /// </summary>
        /// <value>The push notification service.</value>
        public PushNotificationProvider PushNotificationService { get { return mPushNotificationService; } set { mPushNotificationService = value; } }

        /// <summary>
        /// Gets or sets the OneSignal app identifier.
        /// </summary>
        /// <value>The one signal app identifier.</value>
        public string OneSignalAppId { get { return mOneSignalAppId; } set { mOneSignalAppId = value; } }

        /// <summary>
        /// Gets or sets the Firebase topics.
        /// </summary>
        /// <value>The firebase topics.</value>
        public string[] FirebaseTopics { get { return mFirebaseTopics; } set { mFirebaseTopics = value; } }

        /// <summary>
        /// Gets or sets the notification category groups.
        /// </summary>
        /// <value>The category groups.</value>
        public NotificationCategoryGroup[] CategoryGroups { get { return mCategoryGroups; } set { mCategoryGroups = value; } }

        /// <summary>
        /// Gets or sets the default notification category.
        /// </summary>
        /// <value>The default category.</value>
        public NotificationCategory DefaultCategory { get { return mDefaultCategory; } set { mDefaultCategory = value; } }

        /// <summary>
        /// Gets or sets the user notification categories.
        /// </summary>
        /// <value>The user categories.</value>
        public NotificationCategory[] UserCategories { get { return mUserCategories; } set { mUserCategories = value; } }

        // Initialization config
        [SerializeField]
        private bool mAutoInit = true;
        [SerializeField]
        [Range(0, 60)]
        private float mAutoInitDelay = 0f;

        // iOS authorization options
        [SerializeField]
        [EnumFlags]
        private NotificationAuthOptions mIosAuthOptions = NotificationAuthOptions.Alert | NotificationAuthOptions.Badge | NotificationAuthOptions.Sound;

        // Remote notification settings
        [SerializeField]
        private PushNotificationProvider mPushNotificationService = PushNotificationProvider.None;

        [SerializeField]
        private string mOneSignalAppId = null;

        [SerializeField]
        private string[] mFirebaseTopics = null;

        // Category groups
        [SerializeField]
        private NotificationCategoryGroup[] mCategoryGroups = null;

        // Default notification category
        [SerializeField]
        private NotificationCategory mDefaultCategory = new NotificationCategory()
        {
            id = DEFAULT_CATEGORY_ID,
            name = DEFAULT_CATEGORY_NAME
        };

        // User categories
        [SerializeField]
        private NotificationCategory[] mUserCategories = null;

        public NotificationCategory GetCategoryWithId(string categoryId)
        {
            if (!string.IsNullOrEmpty(categoryId))
            {
                if (categoryId.Equals(DefaultCategory.id))
                {
                    return DefaultCategory;
                }
                else if (UserCategories != null)
                {
                    foreach (var c in UserCategories)
                    {
                        if (categoryId.Equals(c.id))
                            return c;
                    }
                }
            }

            return null;
        }

        #region IAndroidPermissionRequired

        [SerializeField]
        private List<AndroidPermission> mAndroidPermissions = new List<AndroidPermission>()
        {
            new AndroidPermission(AndroidPermission.AndroidPermissionElementName, AndroidPermission.AndroidPermissionReceiveBootCompleted)
        };

        public List<AndroidPermission> GetAndroidPermissions()
        {
            return mAndroidPermissions;
        }

        #endregion
    }

    public enum PushNotificationProvider
    {
        None,
        OneSignal,
        Firebase,
    }

    [Flags]
    public enum NotificationAuthOptions
    {
        Alert = 1 << 0,
        Badge = 1 << 1,
        Sound = 1 << 2,
    }
}

