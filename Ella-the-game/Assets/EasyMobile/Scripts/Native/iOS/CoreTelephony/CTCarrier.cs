#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;

namespace EasyMobile.iOS.Telephony
{
    /// <summary>
    /// Use the CTCarrier class to obtain information about the user’s home cellular service provider, 
    /// such as its unique identifier and whether or not it allows VoIP (Voice over Internet Protocol) calls on its network.
    /// </summary>
    internal class CTCarrier
    {
        /// <summary>
        /// Indicates if the carrier allows VoIP calls to be made on its network.
        /// </summary>
        /// <returns><c>true</c>, if allows VoIP, <c>false</c> otherwise.</returns>
        public static bool GetAllowsVOIP()
        {
            return C.CTCarrier_allowsVOIP();
        }

        /// <summary>
        /// The name of the user’s home cellular service provider.
        /// </summary>
        /// <returns>The carrier name.</returns>
        public static string GetCarrierName()
        {
            return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                C.CTCarrier_carrierName(strBuffer, strLen));
        }

        /// <summary>
        /// The ISO country code for the user’s cellular service provider.
        /// </summary>
        /// <returns>The iso country code.</returns>
        public static string GetIsoCountryCode()
        {
            return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                C.CTCarrier_isoCountryCode(strBuffer, strLen));
        }

        /// <summary>
        /// The mobile country code (MCC) for the user’s cellular service provider.
        /// </summary>
        /// <returns>The mobile country code.</returns>
        public static string GetMobileCountryCode()
        {
            return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                C.CTCarrier_mobileCountryCode(strBuffer, strLen));
        }

        /// <summary>
        /// The mobile network code (MNC) for the user’s cellular service provider.
        /// </summary>
        /// <returns>The mobile network code.</returns>
        public static string GetMobileNetworkCode()
        {
            return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                C.CTCarrier_mobileNetworkCode(strBuffer, strLen));
        }

        #region C wrapper

        private static class C
        {
            [DllImport("__Internal")]
            internal static extern bool CTCarrier_allowsVOIP();

            [DllImport("__Internal")]
            internal static extern int CTCarrier_carrierName([In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

            [DllImport("__Internal")]
            internal static extern int CTCarrier_isoCountryCode([In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

            [DllImport("__Internal")]
            internal static extern int CTCarrier_mobileCountryCode([In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

            [DllImport("__Internal")]
            internal static extern int CTCarrier_mobileNetworkCode([In,Out] /* from(char *) */ byte[] strBuffer, int strLen);
        }

        #endregion
    }
}
#endif