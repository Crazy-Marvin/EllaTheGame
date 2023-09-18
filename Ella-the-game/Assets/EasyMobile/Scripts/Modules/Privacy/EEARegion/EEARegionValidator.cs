using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile.Internal.Privacy;

namespace EasyMobile
{
    /// <summary>
    /// A facade class that holds all EEA region related methods.
    /// </summary>
    public static class EEARegionValidator
    {
        /// <summary>
        /// Default validating methods. These methods are used in the order
        /// that they appear in the list.
        /// </summary>
        public static readonly List<EEARegionValidationMethods> DefaultMethods = new List<EEARegionValidationMethods>()
        {
            EEARegionValidationMethods.GoogleService,
            EEARegionValidationMethods.Telephony,
            EEARegionValidationMethods.Timezone,
            EEARegionValidationMethods.Locale
        };

        private static IPlatformEEARegionValidator validator;

        static EEARegionValidator()
        {
            #if UNITY_EDITOR
            validator = new UnsupportedEEARegionValidator();
            #elif UNITY_ANDROID
            validator = new AndroidEEARegionValidator();
            #elif UNITY_IOS
            validator = new iOSEEARegionValidator();
            #else
            validator = new UnsupportedEEARegionValidator();
            #endif
        }

        /// <summary>
        /// Attempts to determine if the current device is in the European Economic Area (EEA) region 
        /// using the provided list of validating methods.
        /// </summary>
        /// <param name="callback">Callback.</param>
        /// <param name="methods">Validating methods, used in the order that they appear in the list.</param>
        public static void ValidateEEARegionStatus(Action<EEARegionStatus> callback, List<EEARegionValidationMethods> methods)
        {
            if (validator == null)
            {
                Debug.LogError("[ValidateEEARegionStatus]. Error: the validator hasn't been initialized.");
                return;
            }

            validator.ValidateEEARegionStatus(methods, callback);
        }

        /// <summary>
        /// Attempts to determine if the current device is in the European Economic Area (EEA) region 
        /// using the default validating methods <see cref="DefaultMethods"/>.
        /// </summary>
        /// <param name="callback">Callback.</param>
        public static void ValidateEEARegionStatus(Action<EEARegionStatus> callback)
        {
            ValidateEEARegionStatus(callback, DefaultMethods);
        }
    }
}
