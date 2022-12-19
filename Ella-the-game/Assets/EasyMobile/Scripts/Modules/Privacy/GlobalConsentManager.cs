using System;
using EasyMobile.Internal;

namespace EasyMobile
{
    /// <summary>
    /// Global consent manager.
    /// </summary>
    public class GlobalConsentManager : ConsentManager
    {
        private const string DATA_PRIVACY_CONSENT_KEY = "EM_Global_DataPrivacyConsent";

        public static GlobalConsentManager Instance
        { 
            get
            {
                if (sInstance == null)
                    sInstance = new GlobalConsentManager(DATA_PRIVACY_CONSENT_KEY);
                return sInstance;
            }
        }

        private static GlobalConsentManager sInstance;

        private GlobalConsentManager(string key)
        {
            this.mDataPrivacyConsentKey = key;
        }
    }
}
