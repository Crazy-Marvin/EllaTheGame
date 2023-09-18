#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;

namespace EasyMobile.iOS.Foundation
{
    /// <summary>
    /// Information about standard time conventions associated with a specific geopolitical region.
    /// </summary>
    internal class NSTimeZone
    {
        /// <summary>
        /// The geopolitical region ID that identifies the local timezone.
        /// </summary>
        /// <returns>The local time zone name.</returns>
        public static string GetLocalTimeZoneName()
        {
            return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                C.NSTimeZone_localTimeZone_name(strBuffer, strLen));
        }

        /// <summary>
        /// The abbreviation for the local timezone, such as “EDT” (Eastern Daylight Time).
        /// </summary>
        /// <returns>The local time zone abbreviation.</returns>
        public static string GetLocalTimeZoneAbbreviation()
        {
            return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                C.NSTimeZone_localTimeZone_abbreviation(strBuffer, strLen));
        }

        /// <summary>
        /// The current difference in seconds between the local timezone and Greenwich Mean Time.
        /// </summary>
        /// <returns>The local time zone seconds from GM.</returns>
        public static int GetLocalTimeZoneSecondsFromGMT()
        {
            return C.NSTimeZone_localTimeZone_secondsFromGMT();
        }

        #region C wrapper

        private static class C
        {
            [DllImport("__Internal")]
            internal static extern int NSTimeZone_localTimeZone_name([In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

            [DllImport("__Internal")]
            internal static extern int NSTimeZone_localTimeZone_abbreviation([In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

            [DllImport("__Internal")]
            internal static extern int NSTimeZone_localTimeZone_secondsFromGMT();

        }

        #endregion
    }
}
#endif