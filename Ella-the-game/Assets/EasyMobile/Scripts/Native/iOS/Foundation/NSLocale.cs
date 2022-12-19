#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;

namespace EasyMobile.iOS.Foundation
{
    /// <summary>
    /// Information about linguistic, cultural, and technological 
    /// conventions for use in formatting data for presentation.
    /// </summary>
    internal class NSLocale
    {
        /// <summary>
        /// The identifier for the current locale.
        /// </summary>
        /// <returns>The current locale identifier.</returns>
        public static string GetCurrentLocaleIdentifier()
        {
            return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                C.NSLocale_currentLocale_localeIdentifier(strBuffer, strLen));
        }

        /// <summary>
        /// The country code for the current locale.
        /// </summary>
        /// <returns>The current country code.</returns>
        public static string GetCurrentCountryCode()
        {
            return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                C.NSLocale_currentLocale_countryCode(strBuffer, strLen));
        }

        /// <summary>
        /// The language code for the current locale.
        /// </summary>
        /// <returns>The current language code.</returns>
        public static string GetCurrentLanguageCode()
        {
            return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                C.NSLocale_currentLocale_languageCode(strBuffer, strLen));
        }

        /// <summary>
        /// The script code for the current locale.
        /// </summary>
        /// <returns>The current script code.</returns>
        public static string GetCurrentScriptCode()
        {
            return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                C.NSLocale_currentLocale_scriptCode(strBuffer, strLen));
        }

        /// <summary>
        /// The variant code for the current locale.
        /// </summary>
        /// <returns>The current variant code.</returns>
        public static string GetCurrentVariantCode()
        {
            return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                C.NSLocale_currentLocale_variantCode(strBuffer, strLen));
        }

        /// <summary>
        /// The currency code for the current locale.
        /// </summary>
        /// <returns>The current currency code.</returns>
        public static string GetCurrentCurrencyCode()
        {
            return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                C.NSLocale_currentLocale_currencyCode(strBuffer, strLen));
        }

        /// <summary>
        /// The currency symbol for the current locale.
        /// </summary>
        /// <returns>The current currency symbol.</returns>
        public static string GetCurrentCurrencySymbol()
        {
            return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                C.NSLocale_currentLocale_currencySymbol(strBuffer, strLen));
        }

        #region C wrapper

        private static class C
        {
            [DllImport("__Internal")]
            internal static extern int NSLocale_currentLocale_localeIdentifier([In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

            [DllImport("__Internal")]
            internal static extern int NSLocale_currentLocale_countryCode([In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

            [DllImport("__Internal")]
            internal static extern int NSLocale_currentLocale_languageCode([In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

            [DllImport("__Internal")]
            internal static extern int NSLocale_currentLocale_scriptCode([In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

            [DllImport("__Internal")]
            internal static extern int NSLocale_currentLocale_variantCode([In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

            [DllImport("__Internal")]
            internal static extern int NSLocale_currentLocale_currencyCode([In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

            [DllImport("__Internal")]
            internal static extern int NSLocale_currentLocale_currencySymbol([In,Out] /* from(char *) */ byte[] strBuffer, int strLen);
        }

        #endregion // Native Methods
    }
}
#endif