using System;
using System.Collections.Generic;

namespace EasyMobile
{
    public interface IConsentRequirable
    {
        /// <summary>
        /// Raised when the consent status is changed.
        /// </summary>
        event Action<ConsentStatus> DataPrivacyConsentUpdated;

        /// <summary>
        /// The status of the consent on data privacy of this object, default to ConsentStatus.Unknown. 
        /// This value persists across app launches.
        /// </summary>
        ConsentStatus DataPrivacyConsent { get; }

        /// <summary>
        /// Sets DataPrivacyConsent to ConsentStatus.Granted and stores this value so that it persists across app launches.
        /// </summary>
        void GrantDataPrivacyConsent();

        /// <summary>
        /// Sets DataPrivacyConsent to ConsentStatus.Revoked and stores this value so that it persists across app launches.
        /// </summary>
        void RevokeDataPrivacyConsent();
    }
}
