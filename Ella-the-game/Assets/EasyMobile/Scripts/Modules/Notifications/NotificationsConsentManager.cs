using System;
using EasyMobile.Internal;

namespace EasyMobile
{
    /// <summary>
    /// Module-level consent manager for the Notifications module.
    /// </summary>
    public class NotificationsConsentManager : ConsentManager
    {
        private const string DATA_PRIVACY_CONSENT_KEY = "EM_Notifications_DataPrivacyConsent";

        public static NotificationsConsentManager Instance
        { 
            get
            {
                if (sInstance == null)
                    sInstance = new NotificationsConsentManager(DATA_PRIVACY_CONSENT_KEY);
                return sInstance;
            }
        }

        private static NotificationsConsentManager sInstance;

        private NotificationsConsentManager(string key)
        {
            this.mDataPrivacyConsentKey = key;
        }
    }
}
