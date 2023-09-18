using UnityEngine;
using System.Collections;
using EasyMobile.Internal;

namespace EasyMobile
{
    /// <summary>
    /// Helper class with methods to store an retrieve user consent
    /// to and from the persistent storage.
    /// </summary>
    public static class ConsentStorage
    {
        public const int UnknownConsentStoredValue = -1;
        public const int RevokedConsentStoredValue = 0;
        public const int GrantedConsentStoredValue = 1;

        /// <summary>
        /// Reads the consent status with given key from the persistent storage.
        /// Returns <c>ConsentStatus.Unknown</c> if no saved value found.
        /// </summary>
        /// <returns>The consent status.</returns>
        /// <param name="key">Key.</param>
        public static ConsentStatus ReadConsent(string key)
        {
            var consentInt = StorageUtil.GetInt(key, UnknownConsentStoredValue);
            return consentInt == UnknownConsentStoredValue ? ConsentStatus.Unknown : 
                consentInt == RevokedConsentStoredValue ? ConsentStatus.Revoked : ConsentStatus.Granted;
        }

        /// <summary>
        /// Saves the consent status with the given key into the persistent storage.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="consent">Consent.</param>
        public static void SaveConsent(string key, ConsentStatus consent)
        {
            var consentInt = consent == ConsentStatus.Unknown ? UnknownConsentStoredValue :
                consent == ConsentStatus.Revoked ? RevokedConsentStoredValue : GrantedConsentStoredValue;
            StorageUtil.SetInt(key, consentInt);
        }
    }
}