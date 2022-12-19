using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMobile
{
    /// <summary>
    /// Available EEA region validation methods.
    /// </summary>
    public enum EEARegionValidationMethods
    {
        /// <summary>
        /// Validating using the ad service provided by Google
        /// https://adservice.google.com/getconfig/pubvendors
        /// </summary>
        GoogleService,

        /// <summary>
        /// Validating using the country code obtained from
        /// the device's mobile carrier information.
        /// </summary>
        Telephony,

        /// <summary>
        /// Validating using the device's timezone setting.
        /// Note that this setting can be changed by the user.
        /// </summary>
        Timezone,

        /// <summary>
        /// Validating using the device's locale setting.
        /// Note that this setting can be changed by the user.
        /// </summary>
        Locale
    }
}
