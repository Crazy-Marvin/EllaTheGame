using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using EasyMobile.Internal;

namespace EasyMobile
{
#if UNITY_IOS
    using EasyMobile.Internal.StoreReview.iOS;
#endif

#if UNITY_ANDROID
    using EasyMobile.Internal.StoreReview.Android;
#endif

    [AddComponentMenu("")]
    public class StoreReview : MonoBehaviour
    {
        public enum UserAction
        {
            Refuse,
            Postpone,
            Feedback,
            Rate
        }

#if UNITY_ANDROID
        // The current active rating request dialog
        public static StoreReview Instance { get; private set; }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        private static readonly string RATING_DIALOG_GAMEOBJECT = "MobileNativeRatingDialog";
#endif

        // Remember to not show up again if the user refuses to give feedback.
        private const int IOS_SYSTEM_DEFAULT_ANNUAL_CAP = 3;
        private const int RATING_REQUEST_ENABLED = 1;
        private const int RATING_REQUEST_DISABLED = -1;
        private const string RATING_REQUEST_DISABLE_PPKEY = "EM_RATING_REQUEST_DISABLE";
        private const string ANNUAL_REQUESTS_MADE_PPKEY_PREFIX = "EM_RATING_REQUESTS_MADE_YEAR_";
        private const string LAST_REQUEST_TIMESTAMP_PPKEY = "EM_RATING_REQUEST_LAST_REQUEST_TIMESTAMP";

        private static Action<UserAction> customBehaviour;

        #region Public API

        /// <summary>
        /// Shows the rating request dialog with the default behavior and content. 
        /// </summary>
        public static void RequestRating()
        {
            DoRequestRating(null, null);
        }

        /// <summary>
        /// Shows the rating request dialog with the default behavior and the given content.
        /// Use this method if you want to localize the dialog on Android or iOS older than 10.3
        /// (on iOS 10.3 or newer, the rating dialog content is controlled by the system).
        /// </summary>
        /// <param name="dialogContent">Dialog content.</param>
        [Obsolete("This method has been deprecated since Easy Mobile Pro version 2.8.0")]
        public static void RequestRating(RatingDialogContent dialogContent)
        {
            DoRequestRating(dialogContent, null);
        }

        /// <summary>
        /// On Android or iOS older than 10.3, this method shows the rating request dialog 
        /// with the given content and the callback will be invoked when the popup closes. 
        /// You can use this method if you want to implement a custom behavior
        /// for the rating dialog. If you also want to localize it, pass a <see cref="RatingDialogContent"/>
        /// object holding the localized content, otherwise pass null to use the default content.
        /// On iOS 10.3 or newer, the system rating prompt is used, therefore the callback won't be invoked, 
        /// and the dialog content is controlled by the system.  
        /// </summary>
        /// <param name="dialogContent">Dialog content, pass null to use the default content.</param>
        /// <param name="callback">Callback receiving user selection as input. You need to
        /// implement this callback to take appropriate actions corresponding to the user input.</param>
        [Obsolete("This method has been deprecated since Easy Mobile Pro version 2.8.0")]
        public static void RequestRating(RatingDialogContent dialogContent, Action<UserAction> callback)
        {
            DoRequestRating(dialogContent, callback);
        }

        /// <summary>
        /// Gets the default content of the rating dialog as configured in the module settings.
        /// </summary>
        /// <returns>The default dialog content.</returns>
        [Obsolete("This method has been deprecated since Easy Mobile Pro version 2.8.0")]
        public static RatingDialogContent GetDefaultDialogContent()
        {
            return EM_Settings.RatingRequest.DefaultRatingDialogContent;
        }

        /// <summary>
        /// Determines if a rating request can be made, which means:
        /// - it was not previously disabled by the user (Don't ask again)
        /// - the user hasn't accepted to rate before 
        /// - other display constraints are satisfied
        /// This method always returns <c>true</c> if <see cref="Ignore Constraints In Development"/> option is
        /// checked in module settings and the current build is a development one, unless the rating request
        /// was disabled (the user either selected Don't ask again or already accepted to rate before).
        /// </summary>
        /// <returns><c>true</c> if can request rating; otherwise, <c>false</c>.</returns>
        [Obsolete("This method has been deprecated since Easy Mobile Pro version 2.8.0")]
        public static bool CanRequestRating()
        {
            if (IsDisplayConstraintIgnored())
            {
                return !IsRatingRequestDisabled();
            }

            return !IsRatingRequestDisabled() &&
            (GetRemainingDelayAfterInstallation() <= 0) && (GetThisYearRemainingRequests() > 0) && (GetRemainingCoolingOffDays() <= 0);
        }

        /// <summary>
        /// Returns <c>true</c> if the <see cref="Ignore Constraints In Development"/> option is checked in module settings
        /// and the current build is a development one.
        /// </summary>
        /// <returns><c>true</c> if all display constraints are ignored; otherwise, <c>false</c>.</returns>
        [Obsolete("This method has been deprecated since Easy Mobile Pro version 2.8.0")]
        public static bool IsDisplayConstraintIgnored()
        {
            return Util.IsUnityDevelopmentBuild() && EM_Settings.RatingRequest.IgnoreConstraintsInDevelopment;
        }

        /// <summary>
        /// Determines if the rating request dialog has been disabled.
        /// Disabling occurs if the user either selects the "refuse" button or the "rate" button.
        /// On iOS, this is only applicable to versions older than 10.3.
        /// </summary>
        /// <returns><c>true</c> if rating request is disabled; otherwise, <c>false</c>.</returns>
        [Obsolete("This method has been deprecated since Easy Mobile Pro version 2.8.0")]
        public static bool IsRatingRequestDisabled()
        {
            return StorageUtil.GetInt(RATING_REQUEST_DISABLE_PPKEY, RATING_REQUEST_ENABLED) == RATING_REQUEST_DISABLED;
        }

        /// <summary>
        /// Gets the remaining delay time (days) after installation that is required before the first
        /// rating request can be made.
        /// </summary>
        /// <returns>The remaining delay time after installation in days.</returns>
        [Obsolete("This method has been deprecated since Easy Mobile Pro version 2.8.0")]
        public static int GetRemainingDelayAfterInstallation()
        {
            int daysSinceInstallation = DateTime.Now.SameTimeZoneSubtract(RuntimeHelper.GetAppInstallationTime()).Days;
            int remainingDays = (int)EM_Settings.RatingRequest.DelayAfterInstallation - daysSinceInstallation;
            return remainingDays >= 0 ? remainingDays : 0;
        }

        /// <summary>
        /// Gets the remaining cooling-off days until the next request can be made.
        /// </summary>
        /// <returns>The remaining cooling-off days.</returns>
        [Obsolete("This method has been deprecated since Easy Mobile Pro version 2.8.0")]
        public static int GetRemainingCoolingOffDays()
        {
            int daysPast = DateTime.Now.SameTimeZoneSubtract(GetLastRequestTimestamp()).Days;
            int remainingDays = (int)EM_Settings.RatingRequest.CoolingOffPeriod - daysPast;
            return remainingDays >= 0 ? remainingDays : 0;
        }

        /// <summary>
        /// Gets the timestamp of the last request in local time. If no request was made previously,
        /// Epoch time (01/01/1970) will be returned.
        /// </summary>
        /// <returns>The last request timestamp.</returns>
        [Obsolete("This method has been deprecated since Easy Mobile Pro version 2.8.0")]
        public static DateTime GetLastRequestTimestamp()
        {
            return StorageUtil.GetTime(LAST_REQUEST_TIMESTAMP_PPKEY, Util.UnixEpoch.ToLocalTime());
        }

        /// <summary>
        /// Gets the number of requests used this year.
        /// </summary>
        /// <returns>The this year used requests.</returns>
        [Obsolete("This method has been deprecated since Easy Mobile Pro version 2.8.0")]
        public static int GetThisYearUsedRequests()
        {
            return GetAnnualUsedRequests(DateTime.Now.Year);
        }

        /// <summary>
        /// Gets the number of unused requests in this year.
        /// Note that this is not applicable to iOS 10.3 or newer.
        /// </summary>
        /// <returns>This year unused requests.</returns>
        [Obsolete("This method has been deprecated since Easy Mobile Pro version 2.8.0")]
        public static int GetThisYearRemainingRequests()
        {
            return GetAnnualRequestsLimit() - GetThisYearUsedRequests();
        }

        /// <summary>
        /// Gets the maximum number of requests that can be made per year.
        /// </summary>
        /// <returns>The annual requests limit.</returns>
        [Obsolete("This method has been deprecated since Easy Mobile Pro version 2.8.0")]
        public static int GetAnnualRequestsLimit()
        {
#if UNITY_IOS && !UNITY_EDITOR
            if (iOSNativeUtility.CanUseBuiltinRequestReview())
            {
                // iOS 10.3+.
                return IOS_SYSTEM_DEFAULT_ANNUAL_CAP;
            }
#endif
            return (int)EM_Settings.RatingRequest.AnnualCap;
        }

        /// <summary>
        /// Disables the rating request dialog show that it can't be shown anymore.
        /// </summary>
        [Obsolete("This method has been deprecated since Easy Mobile Pro version 2.8.0")]
        public static void DisableRatingRequest()
        {
            StorageUtil.SetInt(RATING_REQUEST_DISABLE_PPKEY, RATING_REQUEST_DISABLED);
            StorageUtil.Save();
        }

        #endregion

        #region Private methods

        private static int GetAnnualUsedRequests(int year)
        {
            string key = ANNUAL_REQUESTS_MADE_PPKEY_PREFIX + year.ToString();
            return StorageUtil.GetInt(key, 0);
        }

        private static void SetAnnualUsedRequests(int year, int requestNumber)
        {
            string key = ANNUAL_REQUESTS_MADE_PPKEY_PREFIX + year.ToString();
            StorageUtil.SetInt(key, requestNumber);
        }

        private static void DoRequestRating(RatingDialogContent content, Action<UserAction> callback)
        {
            //!Remove Constrains check since from version 2.8.0 EM Pro use native review popup on both platform.
            // if (!CanRequestRating())
            // {
            //     Debug.Log("Could not display the rating request popup because it was disabled, " +
            //         "or one or more display constraints are not satisfied.");
            //     return;
            // }

            // If no custom content was provided, use the default one.
            if (content == null)
            {
                content = EM_Settings.RatingRequest.DefaultRatingDialogContent;
            }

            // Callback register
            customBehaviour = callback;

#if UNITY_EDITOR
            Debug.Log("Request review is only available on iOS and Android devices.");
#elif UNITY_IOS
            if (iOSNativeUtility.CanUseBuiltinRequestReview())
            {
                // iOS 10.3+.
                iOSNativeUtility.RequestReview();
            }
            else
            {
                // iOS before 10.3.
                NativeUI.AlertPopup alert = NativeUI.ShowThreeButtonAlert(
                                              content.Title.Replace(RatingDialogContent.PRODUCT_NAME_PLACEHOLDER, Application.productName),
                                              content.Message.Replace(RatingDialogContent.PRODUCT_NAME_PLACEHOLDER, Application.productName),
                                              content.RefuseButtonText,
                                              content.PostponeButtonText,
                                              content.RateButtonText
                                          );

                if (alert != null)
                {
                    alert.OnComplete += OnIosRatingDialogCallback;
                }
            }

            // if (!IsDisplayConstraintIgnored())
            // {
            //     // Increment the number of requests used this year.
            //     SetAnnualUsedRequests(DateTime.Now.Year, GetAnnualUsedRequests(DateTime.Now.Year) + 1);

            //     // Store the request timestamp
            //     StorageUtil.SetTime(LAST_REQUEST_TIMESTAMP_PPKEY, DateTime.Now);
            // }
#elif UNITY_ANDROID
            if (Instance != null)
                return;    // only allow one alert at a time

            // Create a Unity game object to receive messages from native side
            Instance = new GameObject(RATING_DIALOG_GAMEOBJECT).AddComponent<StoreReview>();
            #region native_review_popup

            // // Replace placeholder texts if any.
            // var texts = new RatingDialogContent(
            //                 content.Title.Replace(RatingDialogContent.PRODUCT_NAME_PLACEHOLDER, Application.productName),
            //                 content.Message.Replace(RatingDialogContent.PRODUCT_NAME_PLACEHOLDER, Application.productName),
            //                 content.LowRatingMessage.Replace(RatingDialogContent.PRODUCT_NAME_PLACEHOLDER, Application.productName),
            //                 content.HighRatingMessage.Replace(RatingDialogContent.PRODUCT_NAME_PLACEHOLDER, Application.productName),
            //                 content.PostponeButtonText,
            //                 content.RefuseButtonText,
            //                 content.RateButtonText,
            //                 content.CancelButtonText,
            //                 content.FeedbackButtonText
            //             );

            // // Show the Android rating request
            // AndroidNativeUtility.RequestRating(texts, EM_Settings.RatingRequest);
            #endregion
            #region store_review
            AndroidNativeUtility.RequestStoreReview(Instance.name, "OnAndroidRatingDialogCallback");
            #endregion
            // if (!IsDisplayConstraintIgnored())
            // {
            //     // Increment the number of requests used this year.
            //     SetAnnualUsedRequests(DateTime.Now.Year, GetAnnualUsedRequests(DateTime.Now.Year) + 1);

            //     // Store the request timestamp
            //     StorageUtil.SetTime(LAST_REQUEST_TIMESTAMP_PPKEY, DateTime.Now);
            // }
#else
            Debug.Log("Request review is not supported on this platform.");
#endif
        }

        [Obsolete("This method has been deprecated since Easy Mobile Pro version 2.8.0")]
        private static void DefaultCallback(UserAction action)
        {
            if (customBehaviour != null)
                customBehaviour(action);
            else
                PerformDefaultBehaviour(action);
        }

        [Obsolete("This method has been deprecated since Easy Mobile Pro version 2.8.0")]
        private static void PerformDefaultBehaviour(UserAction action)
        {
            switch (action)
            {
                case UserAction.Refuse:
                    // Never ask again.
                    DisableRatingRequest();
                    break;
                case UserAction.Postpone:
                    // Do nothing, the dialog simply closes.
                    break;
                case UserAction.Feedback:
                    // Open email client.
                    Application.OpenURL("mailto:" + EM_Settings.RatingRequest.SupportEmail);
                    break;
                case UserAction.Rate:
                    // Open store page.
                    if (Application.platform == RuntimePlatform.IPhonePlayer)
                    {
                        Application.OpenURL("itms-apps://itunes.apple.com/app/id" + EM_Settings.RatingRequest.IosAppId + "?action=write-review");
                    }
                    else if (Application.platform == RuntimePlatform.Android)
                    {
#if UNITY_5_6_OR_NEWER
                        Application.OpenURL("market://details?id=" + Application.identifier);
#else
                        Application.OpenURL("market://details?id=" + Application.bundleIdentifier);
#endif
                    }
                    // The user has rated, don't ask again.
                    DisableRatingRequest();
                    break;
            }
        }

        // Map button index into UserAction.
        private static UserAction ConvertToUserAction(int index)
        {
#if UNITY_IOS
            // Applicable for pre-10.3 only. Note that there's no Feedback option.
            switch (index)
            {
                case 0:
                    return UserAction.Refuse;
                case 1:
                    return UserAction.Postpone;
                case 2:
                    return UserAction.Rate;
                default:
                    return UserAction.Postpone;
            }
#elif UNITY_ANDROID
            switch (index)
            {
                case 0:
                    return UserAction.Refuse;
                case 1:
                    return UserAction.Postpone;
                case 2:
                    return UserAction.Feedback;
                case 3:
                    return UserAction.Rate;
                default:
                    return UserAction.Postpone;
            }
#else
            return UserAction.Postpone;
#endif
        }

#if UNITY_IOS
        // Pre-10.3 iOS rating dialog callback
        private static void OnIosRatingDialogCallback(int button)
        {
            // Always go through the default callback first.
#pragma warning disable 0618
            DefaultCallback(ConvertToUserAction(button));
#pragma warning restore 0618
        }
#endif

#if UNITY_ANDROID
        // Callback to be called from Android native side with UnitySendMessage
        private void OnAndroidRatingDialogCallback(string userAction)
        {
            // int index = Convert.ToInt16(userAction);

            // Always go through the default callback first.
            // #pragma warning disable 0618
            //             DefaultCallback(ConvertToUserAction(index));
            // #pragma warning restore 0618

            // Destroy the used object
            Instance = null;
            Destroy(gameObject);
        }
#endif

        #endregion
    }
}

