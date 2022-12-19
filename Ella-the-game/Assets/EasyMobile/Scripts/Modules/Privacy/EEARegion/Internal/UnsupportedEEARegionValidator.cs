using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile.Internal.Privacy
{
    internal class UnsupportedEEARegionValidator : IPlatformEEARegionValidator
    {
        private const string UNSUPPORTED_MSG = "EEA region validation is not supported on this platform.";

        public void ValidateEEARegionStatus(List<EEARegionValidationMethods> methods, Action<EEARegionStatus> callback)
        {
            Debug.Log(UNSUPPORTED_MSG);

            if (callback != null)
                callback(EEARegionStatus.Unknown);
        }
    }
}
