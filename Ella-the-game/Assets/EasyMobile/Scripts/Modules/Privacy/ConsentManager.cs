using System;

namespace EasyMobile
{
    public abstract class ConsentManager : IConsentRequirable
    {
        protected string mDataPrivacyConsentKey;

        /// <summary>
        /// Raised when the consent status is changed.
        /// </summary>
        public event Action<ConsentStatus> DataPrivacyConsentUpdated;

        /// <summary>
        /// The status of the consent on data privacy of this object, default to ConsentStatus.Unknown. 
        /// This value persists across app launches.
        /// </summary>
        public virtual ConsentStatus DataPrivacyConsent
        { 
            get
            {
                return ConsentStorage.ReadConsent(mDataPrivacyConsentKey);
            }
            private set
            {
                if (DataPrivacyConsent != value)
                {
                    // Store new consent to the persistent storage.
                    ConsentStorage.SaveConsent(mDataPrivacyConsentKey, value);

                    // Raise event.
                    if (DataPrivacyConsentUpdated != null)
                        DataPrivacyConsentUpdated(value);
                }
            }
        }

        /// <summary>
        /// Sets DataPrivacyConsent to ConsentStatus.Granted and stores this value so that it persists across app launches.
        /// </summary>
        public virtual void GrantDataPrivacyConsent()
        {
            DataPrivacyConsent = ConsentStatus.Granted;
        }

        /// <summary>
        /// Sets DataPrivacyConsent to ConsentStatus.Revoked and stores this value so that it persists across app launches.
        /// </summary>
        public virtual void RevokeDataPrivacyConsent()
        {
            DataPrivacyConsent = ConsentStatus.Revoked;
        }
    }
}
