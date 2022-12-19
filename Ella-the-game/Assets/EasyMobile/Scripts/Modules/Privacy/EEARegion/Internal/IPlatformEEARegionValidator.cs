using System;
using System.Collections.Generic;

namespace EasyMobile.Internal.Privacy
{
    internal interface IPlatformEEARegionValidator
    {
        void ValidateEEARegionStatus(List<EEARegionValidationMethods> methods, Action<EEARegionStatus> callback);
    }
}
