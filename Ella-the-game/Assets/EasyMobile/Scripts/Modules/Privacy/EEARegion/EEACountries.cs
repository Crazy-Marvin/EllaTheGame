using System;
using System.Collections.Generic;

namespace EasyMobile
{
    public enum EEACountries
    {
        None = 0,
        AT, BE, BG, HR, CY, CZ, DK, EE, FI, FR, DE, GR, HU, IE, IT, LV, LT, LU, MT, NL, PL, PT, RO, SK, SI, ES, SE, GB, // 28 member states.
        GF, PF, TF, // French territories French Guiana, Polynesia, Southern Territories.
        EL, UK,  // Alternative EU names for GR and GB.
        IS, LI, NO, // Not EU but in EAA.
        CH, // Not in EU or EAA but in single market.
        AL, BA, MK, XK, ME, RS, TR // Candidate countries.
    }

    public static class EEACountriesExtension
    {
        /// <summary>
        /// Check if a string is a value in <see cref="EEACountries"/>. Case ignored.
        /// </summary>
        public static bool IsEEACountry(this string countryCode)
        {
            if (string.IsNullOrEmpty(countryCode))
                return false;

            string upperCountryCode = countryCode.ToUpper();
            foreach(EEACountries country in Enum.GetValues(typeof(EEACountries)))
            {
                if (country == EEACountries.None)
                    continue;

                if (upperCountryCode.Equals(country.ToString()))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Convert a string to <see cref="EEACountries"/> if possible. Case ignored.
        /// </summary>
        public static EEACountries ToEEACountry(this string countryCode)
        {
            if (IsEEACountry(countryCode))
                return EEACountries.None;

            try
            {
                string upperCountryCode = countryCode.ToUpper();
                return (EEACountries)Enum.Parse(typeof(EEACountries), upperCountryCode);
            }
            catch (Exception)
            {
                return EEACountries.None;
            }
        }
    }
}
