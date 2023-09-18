#if UNITY_ANDROID
using System;
using UnityEngine;

namespace EasyMobile.Internal.Privacy
{
    internal class AndroidEEARegionValidator : BaseNativeEEARegionValidator
    {
        protected const string AndroidClassName = "com.sglib.easymobile.androidnative.gdpr.EEARegionChecker";
        protected const string NativeLocaleValidationMethod = "GetCountryCodeViaLocale";
        protected const string NativeTelephonyValidationMethod = "GetCountryCodeViaTelephony";
        protected const string NativeTimezoneValidationMethod = "ValidateEEARegionViaTimezone";

        protected AndroidJavaClass nativeValidator;

        protected override string GoogleServiceUrl
        {
            get
            {
                // Fix URL issues on low Android versions.
                return Uri.EscapeUriString(base.GoogleServiceUrl);
            }
        }

        public AndroidEEARegionValidator()
        {
            nativeValidator = new AndroidJavaClass(AndroidClassName);
        }

        public override string GetCountryCodeViaLocale()
        {
            if (nativeValidator == null)
                return EEARegionStatus.Unknown.ToString();

            try
            {
                string response = nativeValidator.CallStatic<string>(NativeLocaleValidationMethod);
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
            if (nativeValidator == null)
                return EEARegionStatus.Unknown.ToString();

            try
            {
                string response = nativeValidator.CallStatic<string>(NativeTelephonyValidationMethod);
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
            if (nativeValidator == null)
                return EEARegionStatus.Unknown;

            try
            {
                string response = nativeValidator.CallStatic<string>(NativeTimezoneValidationMethod);
                Debug.Log("[ValidateViaTimezone]. Response: " + (response ?? "null"));

                if (response == null)
                {
                    Debug.Log("[ValidateViaTimezone]. Error: native response is null.");
                    return EEARegionStatus.Unknown;
                }

                if (response.Equals(EEARegionStatus.InEEA.ToString()))
                    return EEARegionStatus.InEEA;

                if (response.Equals(EEARegionStatus.NotInEEA.ToString()))
                    return EEARegionStatus.NotInEEA;

                if (response.Equals(EEARegionStatus.Unknown.ToString()))
                    return EEARegionStatus.Unknown;

                Debug.Log("[ValidateViaTimezone]. Error: unexpected native response: " + response);
                return EEARegionStatus.Unknown;
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
