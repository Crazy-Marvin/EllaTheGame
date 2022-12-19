/// Unity updated WebRequest in 5.4.0
/// https://unity3d.com/unity/whats-new/unity-5.4.0
#if UNITY_5_4_OR_NEWER
using UnityEngine.Networking;

#else
using UnityEngine.Experimental.Networking;
#endif

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile.Internal;

namespace EasyMobile.Internal.Privacy
{
    internal abstract class BaseNativeEEARegionValidator : IPlatformEEARegionValidator
    {
        [Serializable]
        protected class GoogleServiceResponse
        {
            [SerializeField]
            private bool is_request_in_eea_or_unknown = false;

            public bool IsFromEEA { get { return is_request_in_eea_or_unknown; } }
        }

        protected virtual string GoogleServiceUrl
        {
            get
            {
                return "https://adservice.google.com/getconfig/pubvendors?pubs=%1$s&amp;es=2";
            }
        }

        public abstract string GetCountryCodeViaLocale();

        public abstract string GetCountryCodeViaTelephony();

        public abstract EEARegionStatus ValidateViaTimezone();

        protected bool isValidateCoroutineRunning;

        public void ValidateEEARegionStatus(List<EEARegionValidationMethods> methods, Action<EEARegionStatus> callback)
        {
            if (isValidateCoroutineRunning)
            {
                Debug.Log("Another validation progress is running.");
                return;
            }

            RuntimeHelper.RunCoroutine(ValidateEEARegionStatusCoroutine(methods, callback));
        }

        public IEnumerator ValidateEEARegionStatusCoroutine(List<EEARegionValidationMethods> methods, Action<EEARegionStatus> callback)
        {
            isValidateCoroutineRunning = true;

            if (methods == null)
            {
                if (callback != null)
                    callback(EEARegionStatus.Unknown);

                isValidateCoroutineRunning = false;
                yield break;
            }

            var results = new Stack<EEARegionStatus>();

            foreach (var method in methods.Distinct()) // Remove duplicate elements.
            {
                if (method == EEARegionValidationMethods.GoogleService)
                {
                    yield return RuntimeHelper.RunCoroutine(ValidateViaGoogleServiceCoroutine(results));
                }
                else if (method == EEARegionValidationMethods.Timezone)
                {
                    results.Push(ValidateViaTimezone());
                }
                else
                {
                    string countryCode = method == EEARegionValidationMethods.Locale ? GetCountryCodeViaLocale() :
                                         method == EEARegionValidationMethods.Telephony ? GetCountryCodeViaTelephony() :
                                         EEARegionStatus.Unknown.ToString();
                    results.Push(countryCode.CheckEEARegionStatus());
                }

                /// If a method found out that user is in EEA region,
                /// stop the coroutine and notify that to user.
                if (results.Count > 0 && results.Peek() == EEARegionStatus.InEEA)
                {
                    if (callback != null)
                        callback(EEARegionStatus.InEEA);

                    isValidateCoroutineRunning = false;
                    yield break;
                }
            }

            /// If we can reach here, that mean all the previous results are either "Unknown" or "NotInEEA".
            /// If the results stack contains "NotInEEA" value, 
            /// that mean at least one of the methods found out that the user is not in EEA region,
            /// so we're gonna use that as final result.
            /// Otherwise we return just return "Unknown" since we counldn't find anything.
            var result = results.Contains(EEARegionStatus.NotInEEA) ? EEARegionStatus.NotInEEA : EEARegionStatus.Unknown;
            if (callback != null)
                callback(result);

            isValidateCoroutineRunning = false;
        }

        protected virtual IEnumerator ValidateViaGoogleServiceCoroutine(Stack<EEARegionStatus> resultsStack)
        {
            UnityWebRequest www = UnityWebRequest.Get(GoogleServiceUrl);

#if UNITY_2017_1_OR_NEWER
            yield return www.SendWebRequest();
#else
            yield return www.Send();
#endif

            EEARegionStatus status = EEARegionStatus.Unknown;

            bool errorFlag = www.error != null;

            if (errorFlag)
            {
                Debug.Log("Validate EEA region status via Google service error: " + www.error);
                status = EEARegionStatus.Unknown;
            }
            else
            {
                try
                {
                    var responce = JsonUtility.FromJson<GoogleServiceResponse>(www.downloadHandler.text);
                    if (responce == null)
                    {
                        Debug.Log("Validate EEA region status via Google service error: Response object is null.");
                        status = EEARegionStatus.Unknown;
                    }
                    else
                    {
                        status = responce.IsFromEEA ? EEARegionStatus.InEEA : EEARegionStatus.NotInEEA;
                        Debug.Log("[ValidateViaGoogleService]. Response: " + status);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Validate EEA region status via Google service error: " + e.Message);
                    status = EEARegionStatus.Unknown;
                }
            }

            resultsStack.Push(status);
        }
    }
}
