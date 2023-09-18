using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using EasyMobile;
using System;
using System.Reflection;
using System.Text;

namespace EasyMobile.Demo
{
    public class PrivacyDemo : MonoBehaviour
    {

        #region Privacy Policy PlaceHolders & URLs

        public const string EasyMobilePolicyURL = "http://sglibgames.com/privacy-policies/easy-mobile-demo.html";

        public const string AdMobPolicyURL = "https://support.google.com/admob/answer/6128543?hl=en";

        public const string UnityAdsPolicyURL = "https://unity3d.com/legal/privacy-policy";

        public const string OneSignalPolicyURL = "https://onesignal.com/privacy_policy";

        // The opt-out URL for Unity Analytics must be fetched at runtime, so we use a placeholder
        // which we will replace with the actual URL once we fetched it.
        // https://assetstore.unity.com/packages/add-ons/services/unity-data-privacy-plug-in-118922
        public const string UnityAnalyticsOptOutURLPlaceholder = "UNITY_ANALYTICS_URL";

        // We use this property to store the Unity Analytics opt-out URL once it is fetched.
        public static string UnityAnalyticsOptOutURL { get; private set; }

        #endregion

        #region Disclaimer

        // Our disclaimer!
        public const string Disclaimer = "<i><b>Disclaimer:</b> The below content is for demonstration purpose only. " +
                                         "It is NOT intended to be used as-is in your apps. <b>SgLib Games</b> offers tools and information as a resource, " +
                                         "but we don’t offer legal advice. We recommend you contact your legal counsel to find out how GDPR affects you.</i>\n" +
                                         "---------\n";

        #endregion

        #region Demo Consent Dialog - Sample Content In English

        // The consent dialog title in English
        public const string EnTitle = "Data Privacy Consent";

        ///-----------------------------------------------------------------
        /// NOTE: You can use common HTML tags like <b>, <i>, <br> to style the texts
        /// (except titles). You can also use <a> tag for hyperlinks!
        ///-----------------------------------------------------------------

        // The first paragraph of the dialog content in English
        public const string EnFirstParagraph = "This app collects certain information about your use of our app that we would like your " +
                                               "permission to use for advertising, analytics and notification purposes. We strictly follow " +
                                               "our <a href=\"" + EasyMobilePolicyURL + "\">Privacy Policy</a> " +
                                               "to access and process your information.\n" +
                                               "Consent is optional, and you may use the app without granting consent. Please understand that some " +
                                               "features may not function properly if you deny our access.";

        // The title of the toggle for the Advertising module consent in English
        public const string EnAdsToggleTitle = "Advertising";

        // The On description of the toggle for the Advertising module consent in English
        public const string EnAdsToggleOnDesc = "You will receive relevant ads! " +
                                                "Our ad providers will collect data and use a unique identifier on your device. " +
                                                "You can review their policies: <a href=\"" + AdMobPolicyURL + "\">AdMob</a> " +
                                                "and <a href=\"" + UnityAdsPolicyURL + "\">Unity Ads</a>.";

        // The Off description of the toggle for the Advertising module consent in English
        public const string EnAdsToggleOffDesc = "Our ad providers will collect data and use a unique identifier on your device to show you relevant ads. " +
                                                 "Here're their policies: <a href=\"" + AdMobPolicyURL + "\">AdMob</a> " +
                                                 "and <a href=\"" + UnityAdsPolicyURL + "\">Unity Ads</a>.";

        // The title of the toggle for the Notifications module consent in English
        public const string EnNotifsToggleTitle = "Notifications";

        // The common description (both On & Off states) of the toggle for the Notifications module consent in English
        public const string EnNotifsToggleDesc = "Our service provider, OneSignal, will collect data and use a unique identifier on your device to send you push notifications. " +
                                                 "You can learn about <a href=\"" + OneSignalPolicyURL + "\">One Signal Privacy Policy</a>.";

        // The title of the toggle for Unity Analytics consent in English
        // Note that this toggle is On by default and cannot be changed by the user, because we can't opt-out
        // Unity Analytics locally.
        // Instead we use the Unity Data Privacy Plugin to fetch an opt-out URL and present it to the user.
        // https://assetstore.unity.com/packages/add-ons/services/unity-data-privacy-plug-in-118922
        public const string EnAnalyticsToggleTitle = "Analytics*";

        // The common description (both On & Off states) of the toggle for Unity Analytics consent in English
        public const string EnAnalyticsToogleDesc = "We use Unity Analytics service to collect certain analytical information necessary for us to improve this app. " +
                                                    "You can opt-out of this use by visiting <a href=\"" + UnityAnalyticsOptOutURLPlaceholder + "\">this link</a>.";

        // The description of the toggle for Unity Analytics consent that is used if the opt-out URL can't be fetched, in English.
        public const string EnAnalyticsToggleUnavailDesc = "We use Unity Analytics service to collect certain analytical information necessary for us to improve this app. " +
                                                           "You can opt-out of this use by visiting an opt-out URL, which unfortunately <b>can't be fetched now</b>. But you can opt-out later in the \"Privacy\" page of this app.";

        // The second paragraph of the dialog content in English
        public const string EnSecondParagraph = "Click the below button to confirm your consent. You can change this consent at any time in the \"Privacy\" page of this app.";

        // The button title in English.
        public const string EnButtonTitle = "Accept";

        #endregion

        #region Demo Consent Dialog - Sample Content In French

        ///-----------------------------------------------------------------
        /// Here we use Google Translate to manually translate the above content
        /// from English to French for demonstration purpose.
        /// In a production app you may want to use a professional localization
        /// tool to pick the appropriate translation in runtime.
        ///-----------------------------------------------------------------

        // The consent dialog title in French
        public const string FrTitle = "Consentement";

        // The first paragraph of the dialog content in French
        public const string FrFirstParagraph = "Cette application recueille certaines informations sur votre utilisation de notre application que nous aimerions que vous utilisiez à des fins de publicité, d'analyse et de notification. " +
                                               "Nous suivons strictement notre <a href=\"" + EasyMobilePolicyURL + "\">Privacy Policy</a> " +
                                               "pour accéder et traiter vos informations.\n" +
                                               "Le consentement est facultatif et vous pouvez utiliser l'application sans donner votre consentement. " +
                                               "S'il vous plaît comprendre que certaines fonctionnalités peuvent ne pas fonctionner correctement si vous refusez notre accès.";

        // The title of the toggle for the Advertising module consent in French
        public const string FrAdsToggleTitle = "La publicité";

        // The On description of the toggle for the Advertising module consent in French
        public const string FrAdsToggleOnDesc = "Vous recevrez des annonces pertinentes! " +
                                                "Nos fournisseurs de publicité collecteront des données et utiliseront un identifiant unique sur votre appareil. " +
                                                "Vous pouvez revoir leurs politiques: <a href=\"" + AdMobPolicyURL + "\">AdMob</a> " +
                                                "et <a href=\"" + UnityAdsPolicyURL + "\">Unity Ads</a>.";

        // The Off description of the toggle for the Advertising module consent in French
        public const string FrAdsToggleOffDesc = "Nos fournisseurs d'annonces collecteront des données et utiliseront un identifiant unique sur votre appareil pour vous montrer les annonces pertinentes. " +
                                                 "Voici leurs politiques: <a href=\"" + AdMobPolicyURL + "\">AdMob</a> " +
                                                 "et <a href=\"" + UnityAdsPolicyURL + "\">Unity Ads</a>.";

        // The title of the toggle for the Notifications module consent in French
        public const string FrNotifsToggleTitle = "Les notifications";

        // The common description (both On & Off states) of the toggle for the Notifications module consent in French
        public const string FrNotifsToggleDesc = "Notre fournisseur de services, OneSignal, collectera des données et utilisera un identifiant unique sur votre appareil pour vous envoyer des notifications push. " +
                                                 "Vous pouvez en apprendre davantage sur <a href=\"" + OneSignalPolicyURL + "\">One Signal Privacy Policy</a>.";

        // The title of the toggle for Unity Analytics consent in French
        public const string FrAnalyticsToggleTitle = "Analytique*";

        // The common description (both On & Off states) of the toggle for Unity Analytics consent in French
        public const string FrAnalyticsToogleDesc = "Nous utilisons le service Unity Analytics pour collecter certaines informations analytiques nécessaires à l'amélioration de cette application. " +
                                                    "Vous pouvez désactiver cette utilisation en visitant <a href=\"" + UnityAnalyticsOptOutURLPlaceholder + "\">ce lien</a>.";

        // The description of the toggle for Unity Analytics consent that is used if the opt-out URL can't be fetched, in French.
        public const string FrAnalyticsToggleUnavailDesc = "Nous utilisons le service Unity Analytics pour collecter certaines informations analytiques nécessaires à l'amélioration de cette application. " +
                                                           "Vous pouvez refuser cette utilisation en visitant une URL de désinscription, qui ne peut malheureusement pas être récupérée maintenant. " +
                                                           "Mais vous pouvez vous désinscrire plus tard dans la page \"Privacy\" de cette application";

        // The second paragraph of the dialog content in English
        public const string FrSecondParagraph = "Cliquez sur le bouton ci-dessous pour confirmer le consentement spécifié. " +
                                                "Vous pouvez modifier ce consentement à tout moment dans la page \"Privacy\" de cette application.";

        // The button title in English.
        public const string FrButtonTitle = "Acceptez";

        #endregion

        #region Toggle and Button IDs

        private const string AdsToggleId = "em-demo-consent-toggle-ads";
        private const string NotifsToggleId = "em-demo-consent-toggle-notifs";
        private const string UnityAnalyticsToggleId = "em-demo-consent-toggle-unity-analytics";
        private const string AcceptButtonId = "em-demo-consent-button-ok";

        #endregion

        #region Public Settings

        [Header("GDPR Settings")]
        [Tooltip("Whether we should request user consent for this app")]
        public bool shouldRequestConsent = true;

        [Header("Object References")]
        public GameObject isInEeaRegionDisplayer;
        public DemoUtils demoUtils;

        #endregion

        private static ConsentDialog mPreviewConsentDialog;
        private static ConsentDialog mDemoConsentDialog;
        private static ConsentDialog mDemoConsentDialogLocalized;
        private static bool mIsInEEARegion = false;

        void Awake()
        {
            // Checks if we're in EEA region.
            Privacy.IsInEEARegion(result =>
                {
                    mIsInEEARegion = result == EEARegionStatus.InEEA;
                });

            // Fetch Unity Analytics URL for use in case the consent dialog
            // is shown from the demo buttons.
            if (string.IsNullOrEmpty(UnityAnalyticsOptOutURL))
                FetchUnityAnalyticsOptOutURL(null, null);

            // If we think consent is not needed for our app (or the current device
            // is not in EEA region), we can just
            // go ahead and initialize EM runtime as normal.
            if (!shouldRequestConsent)
            {
                if (RuntimeManager.IsInitialized())
                    RuntimeManager.Init();

                return;
            }

            // Here's the case where we want to ask for consent before initializing.
            // Before initialization we need to check
            // if we have user's data privacy consent or not.
            DemoAppConsent consent = DemoAppConsent.LoadDemoAppConsent();

            // If there's a stored consent:
            // the implementation of this demo app guarantees
            // that this consent was forwarded to relevant modules before it was stored.
            // These modules would have automatically stored their own consent persistently
            // and use that consent during initialization.
            // In short we'll just go ahead with initializing the EM runtime.
            if (consent != null)
            {
                if (!RuntimeManager.IsInitialized())
                    RuntimeManager.Init();

                return;
            }

            // If there's no consent:
            // We'll show the demo consent dialog and ask the user for data privacy consent
            // before initializing EM runtime. In a real-world app, you would also want
            // to postpone the initialization of any 3rd-party SDK that requires consent until
            // such consent is obtained via the consent dialog.
            // ---    
            // First fetch the UnityAds opt-out URL which is needed for the consent dialog.
            // Once it's fetched, we'll show the dialog. Once the dialog completes, we'll
            // initialize EM runtime, see DemoDialog_Completed event handler below.
            FetchUnityAnalyticsOptOutURL(
                (url) =>
                {
                    // Success: show the demo consent dialog in English.
                    ShowDemoConsentDialog(false);
                },
                (error) =>
                {
                    // Failure: also show the demo consent dialog in English.
                    // The toogle for Unity Analytics will automatically update
                    // its description to reflect that the URL is not available.
                    ShowDemoConsentDialog(false);
                });
        }

        void Update()
        {
            demoUtils.DisplayBool(isInEeaRegionDisplayer,
                mIsInEEARegion,
                "Is In EEA Region: " + mIsInEEARegion.ToString().ToUpper());
        }

        public void PreviewDefaultConsentDialog(bool dismissible = true)
        {
            if (mPreviewConsentDialog == null)
            {
                // Grab the default consent dialog.
                mPreviewConsentDialog = Privacy.GetDefaultConsentDialog();
                SubscribePreviewConsentDialogEvents();
            }

            mPreviewConsentDialog.Show(dismissible);
        }

        public void ShowDemoConsentDialog(bool dismissible = true)
        {
            // Show a consent dialog in English.
            ShowDemoConsentDialog(false, dismissible);
        }

        public void ShowLocalizedDemoConsentDialog(bool dismissible = true)
        {
            // Show a consent dialog in localized language.
            ShowDemoConsentDialog(true, dismissible);
        }

        public static void ShowDemoConsentDialog(bool localize, bool dismissible)
        {
            if (localize)
            {
                if (mDemoConsentDialogLocalized == null)
                {
                    mDemoConsentDialogLocalized = ConstructConsentDialog(localize);
                    SubscribeConsentDialogEvents(mDemoConsentDialogLocalized);
                }
                mDemoConsentDialogLocalized.Show(dismissible);
            }
            else
            {
                if (mDemoConsentDialog == null)
                {
                    mDemoConsentDialog = ConstructConsentDialog(localize);
                    SubscribeConsentDialogEvents(mDemoConsentDialog);
                }
                mDemoConsentDialog.Show(dismissible);
            }
        }

        /// <summary>
        /// Constructs the consent dialog. Set localize to true to use the
        /// localized content.
        /// </summary>
        /// <returns>The consent dialog.</returns>
        /// <param name="localize">If set to <c>true</c> localize.</param>
        private static ConsentDialog ConstructConsentDialog(bool localize)
        {
            /// 
            /// Here we create a consent dialog completely from coding.
            /// The alternative is to use our consent dialog editor in
            /// the Privacy module settings UI to compose the dialog content.
            /// Here we manually pick the correct translation based on the
            /// localization requirement. In a real-world app, you may use
            /// a professional localization tool to do that.
            /// If you're showing the default consent dialog (constructed 
            /// with the consent dialog editor), you can insert placeholder 
            /// texts into the dialog content and then replace them
            /// with translated texts in script before showing
            /// the dialog for localization purpose.
            /// 

            // First check if there's any consent saved previously.
            // If there is, we will set the 'isOn' state of our toggles
            // according to the saved consent to reflect the current consent
            // status on the consent dialog.
            DemoAppConsent consent = DemoAppConsent.LoadDemoAppConsent();

            // First create a new consent dialog.
            ConsentDialog dialog = new ConsentDialog();

            // Title.
            dialog.Title = localize ? FrTitle : EnTitle;

            // Put our disclaimer on top.
            dialog.AppendText(Disclaimer);

            // Add the first paragraph.
            dialog.AppendText(localize ? FrFirstParagraph : EnFirstParagraph);

            // Build and append the Advertising toggle.
            ConsentDialog.Toggle adsToggle = new ConsentDialog.Toggle(AdsToggleId);
            adsToggle.Title = localize ? FrAdsToggleTitle : EnAdsToggleTitle;
            adsToggle.OnDescription = localize ? FrAdsToggleOnDesc : EnAdsToggleOnDesc;
            adsToggle.OffDescription = localize ? FrAdsToggleOffDesc : EnAdsToggleOffDesc;
            adsToggle.ShouldToggleDescription = true;   // make the description change with the toggle state.
            adsToggle.IsOn = consent != null ? consent.advertisingConsent == ConsentStatus.Granted : false;     // reflect previous ads consent if any

            dialog.AppendToggle(adsToggle);

            // Build and append the Notifications toggle.
            ConsentDialog.Toggle notifsToggle = new ConsentDialog.Toggle(NotifsToggleId);
            notifsToggle.Title = localize ? FrNotifsToggleTitle : EnNotifsToggleTitle;
            notifsToggle.OnDescription = localize ? FrNotifsToggleDesc : EnNotifsToggleDesc;
            notifsToggle.ShouldToggleDescription = false;   // use same description for both on & off states.
            notifsToggle.IsOn = consent != null ? consent.notificationConsent == ConsentStatus.Granted : false; // reflect previous notifs consent if any

            dialog.AppendToggle(notifsToggle);

            // Build and append the Unity Analytics toggle.
            // If the opt-out URL for Unity Analytics is available, we'll insert it
            // to the toggle description. Otherwise we'll use the "URL unavailable" description.
            // Note that this toggle is ON by default and is not interactable because we can't opt-out
            // Unity Analytics locally, instead the user must visit the fetched URL to opt-out.
            ConsentDialog.Toggle uaToggle = new ConsentDialog.Toggle(UnityAnalyticsToggleId);
            uaToggle.Title = localize ? FrAnalyticsToggleTitle : EnAnalyticsToggleTitle;
            uaToggle.ShouldToggleDescription = false;   // the description won't change when the toggle switches between on & off states.
            uaToggle.IsInteractable = false; // not interactable
            uaToggle.IsOn = true;   // consent for UnityAnalytics is ON by default, can opt-out via Unity URL

            if (!string.IsNullOrEmpty(UnityAnalyticsOptOutURL))
            {
                // Unity Analytics opt-out URL is available.
                var description = localize ? FrAnalyticsToogleDesc : EnAnalyticsToogleDesc;
                description = description.Replace(UnityAnalyticsOptOutURLPlaceholder, UnityAnalyticsOptOutURL); // replace placeholder with actual URL
                uaToggle.OnDescription = description;
            }
            else
            {
                // Unity Analytics opt-out URL is not available.
                uaToggle.OnDescription = localize ? FrAnalyticsToggleUnavailDesc : EnAnalyticsToggleUnavailDesc;
            }

            dialog.AppendToggle(uaToggle);

            // Append the second paragraph.
            dialog.AppendText(localize ? FrSecondParagraph : EnSecondParagraph);

            // Build and append the accept button.
            // A consent dialog should always have at least one button!
            ConsentDialog.Button okButton = new ConsentDialog.Button(AcceptButtonId);
            okButton.Title = localize ? FrButtonTitle : EnButtonTitle;
            okButton.TitleColor = Color.white;
            okButton.BodyColor = new Color(66 / 255f, 179 / 255f, 1);

            dialog.AppendButton(okButton);

            return dialog;
        }

        #region Unity Analytics URL Fetching

        /// <summary>
        /// Fetchs the Unity Analytics opt out URL.
        /// </summary>
        /// <param name="success">Success.</param>
        /// <param name="failure">Failure.</param>
        public static void FetchUnityAnalyticsOptOutURL(Action<string> success, Action<string> failure)
        {
            // If the URL was loaded before, just invoke the success callback immediately.
            if (!string.IsNullOrEmpty(UnityAnalyticsOptOutURL))
            {
                if (success != null)
                    success(UnityAnalyticsOptOutURL);
            }

#if UNITY_2018_3_OR_NEWER && UNITY_ANALYTICS
            // Since Unity 2018.3.0, the Unity Data Privacy plugin is embedded in the Analytics library,
            // so just call the method directly.
            UnityEngine.Analytics.DataPrivacy.FetchPrivacyUrl(url =>
                {
                    OnFetchUnityAnalyticsURLSuccess(url, success);
                },
                error =>
                {
                    OnFetchUnityAnalyticsURLFailure(error, failure);
                });
#else

            // Here we need to invokes the methods via reflection because we don't know if
            // you've imported the Unity Data Privacy plugin or not!
            // In your actual app you can simply import the plugin and call it methods as normal.
            // If you're using Unity 2018.3.0 or newer, there's no need to import the plugin because
            // the methods are included in the Analytics library package already.

            string dataPrivacyClassName = "UnityEngine.Analytics.DataPrivacy";
            string initMethodName = "Initialize";
            string fetchURLMethodName = "FetchPrivacyUrl";
            Type dataPrivacyClass = null;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                dataPrivacyClass = assembly.GetType(dataPrivacyClassName);
                if (dataPrivacyClass != null)
                    break;
            }

            if (dataPrivacyClass == null)
            {
                OnFetchUnityAnalyticsURLFailure("DataPrivacy class not found. Have you imported Unity Data Privacy plugin?", failure);
                return;
            }

            MethodInfo initMethod = dataPrivacyClass.GetMethod(initMethodName, BindingFlags.Public | BindingFlags.Static);
            MethodInfo fetchURLMethod = dataPrivacyClass.GetMethod(fetchURLMethodName, BindingFlags.Public | BindingFlags.Static);

            if (initMethod == null || fetchURLMethod == null)
            {
                OnFetchUnityAnalyticsURLFailure("Method not found. Have you imported Unity Data Privacy plugin?", failure);
                return;
            }

            // Initialize Unity's DataPrivacy plugin.
            initMethod.Invoke(null, null);

            // Now fetch the opt-out URL.
            fetchURLMethod.Invoke(null,
                new object[]
                {
                    (Action<string>)((url) =>
                    {
                        OnFetchUnityAnalyticsURLSuccess(url, success);
                    }),
                    (Action<string>)((error) =>
                    {
                        OnFetchUnityAnalyticsURLFailure(error, failure);
                    })
                });
#endif
        }

        private static void OnFetchUnityAnalyticsURLSuccess(string url, Action<string> callback)
        {
            UnityAnalyticsOptOutURL = url;
            if (callback != null)
                callback(url);

            Debug.Log("Unity Analytics opt-out URL is fetched successfully.");
        }

        private static void OnFetchUnityAnalyticsURLFailure(string error, Action<string> callback)
        {
            UnityAnalyticsOptOutURL = string.Empty;
            if (callback != null)
                callback(error);

            Debug.LogWarning("Fetching Unity Analytics opt-out URL failed with error: " + error);
        }

        #endregion

        #region Demo Consent Dialog Event Handlers

        private static void SubscribeConsentDialogEvents(ConsentDialog dialog)
        {
            if (dialog == null)
                return;

            dialog.Dismissed += DemoDialog_Dismissed;
            dialog.Completed += DemoDialog_Completed;
            dialog.ToggleStateUpdated += DemoDialog_ToggleStateUpdated;
        }

        private static void DemoDialog_Dismissed(ConsentDialog dialog)
        {
            Debug.Log("Demo consent dialog was dismissed.");
        }

        private static void DemoDialog_Completed(ConsentDialog dialog, ConsentDialog.CompletedResults results)
        {
            Debug.Log("Demo consent dialog completed with button ID: " + results.buttonId);

            // Construct the new consent.
            DemoAppConsent newConsent = new DemoAppConsent();

            if (results.toggleValues != null)
            {
                Debug.Log("Consent toggles:");
                foreach (KeyValuePair<string, bool> t in results.toggleValues)
                {
                    string toggleId = t.Key;
                    bool toggleValue = t.Value;
                    Debug.Log("Toggle ID: " + toggleId + "; Value: " + toggleValue);

                    if (toggleId == AdsToggleId)
                    {
                        // Whether the Advertising module is given consent.
                        newConsent.advertisingConsent = toggleValue ? ConsentStatus.Granted : ConsentStatus.Revoked;
                    }
                    else if (toggleId == NotifsToggleId)
                    {
                        // Whether the Notifications module is given consent.
                        newConsent.notificationConsent = toggleValue ? ConsentStatus.Granted : ConsentStatus.Revoked;
                    }
                    else if (toggleId == UnityAnalyticsToggleId)
                    {
                        // We don't store the UnityAnalytics consent ourselves as it is managed
                        // by the Unity Data Privacy plugin.
                    }
                    else
                    {
                        // Unrecognized toggle ID.
                    }
                }
            }

            Debug.Log("Now forwarding new consent to relevant modules and then store it...");

            // Forward the consent to relevant modules.
            DemoAppConsent.ApplyDemoAppConsent(newConsent);

            // Store the new consent.
            DemoAppConsent.SaveDemoAppConsent(newConsent);

            // So now we have applied the consent, we can initialize EM runtime
            // (as well as other 3rd-party SDKs in a real-world app).
            if (!RuntimeManager.IsInitialized())
            {
                RuntimeManager.Init();
            }
            else
            {
                // The initialization has already been done. Inform the user
                // that the changes will take effect during next initialization (next app launch).
                NativeUI.Alert("Consent Updated", "You've updated your data privacy consent. " +
                    "Since the initialization process has already completed, all changes will take effect in the next app launch.");
            }
        }

        private static void DemoDialog_ToggleStateUpdated(ConsentDialog dialog, string toggleId, bool isOn)
        {
            Debug.Log("ToggleStateUpdated. ID: " + toggleId + "; new value: " + isOn);
        }

        #endregion

        #region Preview Consent Dialog Event Handlers

        private static void SubscribePreviewConsentDialogEvents()
        {
            if (mPreviewConsentDialog == null)
                return;

            mPreviewConsentDialog.Dismissed += PreviewDialog_Dismissed;
            mPreviewConsentDialog.Completed += PreviewDialog_Completed;
            mPreviewConsentDialog.ToggleStateUpdated += PreviewDialog_ToggleStateUpdated;
        }

        private static void PreviewDialog_Dismissed(ConsentDialog dialog)
        {
            Debug.Log("The preview consent dialog was dismissed.");
        }

        private static void PreviewDialog_Completed(ConsentDialog dialog, ConsentDialog.CompletedResults results)
        {
            var sb = new StringBuilder();
            sb.AppendLine("The preview consent dialog completed with button ID: " + results.buttonId);

            if (results.toggleValues != null)
            {
                sb.AppendLine("Consent toggles:");
                foreach (KeyValuePair<string, bool> t in results.toggleValues)
                {
                    string toggleId = t.Key;
                    bool toggleValue = t.Value;
                    sb.AppendLine("Toggle ID: " + toggleId + "; Value: " + toggleValue);
                }
            }

            NativeUI.Alert("Consent Dialog Completed", sb.ToString());
        }

        private static void PreviewDialog_ToggleStateUpdated(ConsentDialog dialog, string toggleId, bool isOn)
        {
            Debug.Log("ToggleStateUpdated. ID: " + toggleId + "; new value: " + isOn);
        }

        #endregion
    }
}
