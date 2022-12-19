using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMobile
{
    public enum EEARegionStatus
    {
        Unknown,
        InEEA,
        NotInEEA
    }

    public static class EEARegionStatusExtension
    {
        public static EEARegionStatus CheckEEARegionStatus(this string countryCode)
        {
            if (string.IsNullOrEmpty(countryCode))
                return EEARegionStatus.Unknown;

            if (countryCode.Equals(EEARegionStatus.Unknown.ToString()))
                return EEARegionStatus.Unknown;

            if (countryCode.IsEEACountry())
                return EEARegionStatus.InEEA;

            return EEARegionStatus.NotInEEA;
        }
    }
}
