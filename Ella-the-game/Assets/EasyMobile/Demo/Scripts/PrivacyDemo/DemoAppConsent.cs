using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EasyMobile.Demo
{
    /// <summary>
    /// This class represents a collection of individual consents for
    /// all 3rd-party services that require consent in our app.
    /// This class can be used to manage consent for any
    /// 3rd-party service, not just those managed by Easy Mobile.
    /// Note that this class is serializable so we can serialize it
    /// to a JSON string and store in PlayerPrefs.
    /// </summary>
    [Serializable]
    public class DemoAppConsent
    {
        public const string DemoStorageKey = "EM_Demo_AppConsent";

        #region 3rd-party Services Consent

        // The consent for the whole Advertising module.
        // (we could have had different consents for individual ad networks, but for
        // the sake of simplicity in this demo, we'll ask the user a single consent
        // for the whole module and use it for all ad networks).
        public ConsentStatus advertisingConsent = ConsentStatus.Unknown;

        // The consent for the whole Notifications module.
        // Note that data consent is only applicable to push notifications,
        // local notifications don't require any consent.
        public ConsentStatus notificationConsent = ConsentStatus.Unknown;

        // Since this demo app also has In-App Purchase, which forces the use of
        // Unity Analytics, we could have had to ask a consent for that too. However,
        // according to Unity it's sufficient to provide the user with an URL
        // so they can opt-out on Unity website. So we will include that URL in our
        // consent dialog and not need to ask and store any explicit consent locally.

        // Here you can add consent variables for other 3rd party services if needed,
        // including those not managed by Easy Mobile...

        #endregion

        /// <summary>
        /// To JSON string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="EasyMobile.Demo.AppConsent"/>.</returns>
        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }

        /// <summary>
        /// Converts this object to JSON and stores in PlayerPrefs with the provided key.
        /// </summary>
        /// <param name="key">Key.</param>
        public void Save(string key)
        {
            PlayerPrefs.SetString(key, ToString());
        }

        /// <summary>
        /// Forwards the consent to relevant modules of EM.
        /// </summary>
        /// <param name="consent">Consent.</param>
        /// <remarks>
        /// In a real-world app, you'd want to write similar method
        /// to forward the obtained consent not only to relevant EM modules
        /// and services, but also to other relevant 3rd-party SDKs in your app.
        public static void ApplyDemoAppConsent(DemoAppConsent consent)
        {
            // Forward the consent to the Advertising module.
            if (consent.advertisingConsent == ConsentStatus.Granted)
                Advertising.GrantDataPrivacyConsent();
            else if (consent.advertisingConsent == ConsentStatus.Revoked)
                Advertising.RevokeDataPrivacyConsent();

            // Forward the consent to the Notifications module.
            if (consent.notificationConsent == ConsentStatus.Granted)
                Notifications.GrantDataPrivacyConsent();
            else if (consent.notificationConsent == ConsentStatus.Revoked)
                Notifications.RevokeDataPrivacyConsent();

            // Here you can forward the consent to other relevant 3rd-party services if needed...

        }

        /// <summary>
        /// Saves the give app consent to PlayerPrefs as JSON using the demo storage key.
        /// </summary>
        /// <param name="consent">Consent.</param>
        public static void SaveDemoAppConsent(DemoAppConsent consent)
        {
            if (consent != null)
                consent.Save(DemoStorageKey);
        }

        /// <summary>
        /// Loads the demo app consent from PlayerPrefs, returns null if nothing stored previously.
        /// </summary>
        /// <returns>The demo app consent.</returns>
        public static DemoAppConsent LoadDemoAppConsent()
        {
            string json = PlayerPrefs.GetString(DemoStorageKey, null);

            if (!string.IsNullOrEmpty(json))
                return JsonUtility.FromJson<DemoAppConsent>(json);
            else
                return null;
        }
    }
}