using System;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile.Internal.Privacy;

namespace EasyMobile
{
    /// <summary>
    /// Entry class of the Privacy modules.
    /// </summary>
    public static class Privacy
    {
        /// <summary>
        /// The global data privacy consent status as managed by the GlobalConsentManager.
        /// </summary>
        public static ConsentStatus GlobalDataPrivacyConsent
        {
            get { return GlobalConsentManager.Instance.DataPrivacyConsent; }
        }

        /// <summary>
        /// Attempts to determine if the current device is in the European Economic Area (EEA) region.
        /// This method uses the default list of validating methods defined at <see cref="EEARegionValidator.DefaultMethods"/>.
        /// </summary>
        /// <param name="callback">Callback called with the validation result.</param>
        public static void IsInEEARegion(Action<EEARegionStatus> callback)
        {
            EEARegionValidator.ValidateEEARegionStatus(callback);
        }

        /// <summary>
        /// Returns the default consent dialog object as defined in Easy Mobile settings.
        /// </summary>
        public static ConsentDialog GetDefaultConsentDialog()
        {
            return EM_Settings.Privacy.DefaultConsentDialog;
        }

        public static ConsentDialog ShowDefaultConsentDialog(bool dismissible = false)
        {
            var dialog = GetDefaultConsentDialog();

            if (dialog != null)
                dialog.Show(dismissible);

            return dialog;
        }

        /// <summary>
        /// Grants global data privacy consent.
        /// </summary>
        /// <remarks>
        /// This method is a wrapper of <c>GlobalConsentManager.Instance.GrantDataPrivacyConsent</c>.
        /// </remarks>
        public static void GrantGlobalDataPrivacyConsent()
        {
            GlobalConsentManager.Instance.GrantDataPrivacyConsent();
        }

        /// <summary>
        /// Revokes global data privacy consent.
        /// </summary>
        /// <remarks>
        /// This method is a wrapper of <c>GlobalConsentManager.Instance.RevokeDataPrivacyConsent</c>.
        /// </remarks>
        public static void RevokeGlobalDataPrivacyConsent()
        {
            GlobalConsentManager.Instance.RevokeDataPrivacyConsent();
        }
    }
}
