#if UNITY_IOS
using System;
using UnityEngine;
using EasyMobile.iOS.Telephony;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.Internal.Privacy
{
    internal class iOSEEARegionValidator : BaseNativeEEARegionValidator
    {
        protected override string GoogleServiceUrl
        {
            get
            {
                // Fix NSURLConnection error code 1002 on iOS.
                return Uri.EscapeUriString(base.GoogleServiceUrl);
            }
        }

        public override string GetCountryCodeViaLocale()
        {
            try
            {
                string response = NSLocale.GetCurrentCountryCode();
                Debug.Log("[GetCountryCodeViaLocale]. Response: " + (response ?? "null"));

                return response;
            }
            catch (Exception e)
            {
                Debug.Log("[GetCountryCodeViaLocale]. Error: " + e.Message);
                return EEARegionStatus.Unknown.ToString();
            }
        }

        public override string GetCountryCodeViaTelephony()
        {
            try
            {
                string response = CTCarrier.GetIsoCountryCode();
                Debug.Log("[GetCountryCodeViaTelephony]. Response: " + (response ?? "null"));

                return response;
            }
            catch (Exception e)
            {
                Debug.Log("[GetCountryCodeViaTelephony]. Error: " + e.Message);
                return EEARegionStatus.Unknown.ToString();
            }
        }

        public override EEARegionStatus ValidateViaTimezone()
        {
            try
            {
                string response = NSTimeZone.GetLocalTimeZoneName();
                Debug.Log("[ValidateViaTimezone]. Response: " + (response ?? "null"));

                if (response == null)
                    return EEARegionStatus.Unknown;

                if (response.Contains("Europe"))
                    return EEARegionStatus.InEEA;

                return EEARegionStatus.NotInEEA;
            }
            catch (Exception e)
            {
                Debug.Log("[ValidateViaTimezone]. Error: " + e.Message);
                return EEARegionStatus.Unknown;
            }
        }
    }
}
#endif
