using System;
using EasyMobile.Internal;

namespace EasyMobile
{
    /// <summary>
    /// Module-level consent manager for the Advertising module.
    /// </summary>
    public class AdvertisingConsentManager : ConsentManager
    {
        private const string DATA_PRIVACY_CONSENT_KEY = "EM_Ads_DataPrivacyConsent";

        public static AdvertisingConsentManager Instance
        { 
            get
            {
                if (sInstance == null)
                    sInstance = new AdvertisingConsentManager(DATA_PRIVACY_CONSENT_KEY);
                return sInstance;
            }
        }

        private static AdvertisingConsentManager sInstance;

        private AdvertisingConsentManager(string key)
        {
            this.mDataPrivacyConsentKey = key;
        }
    }
}
