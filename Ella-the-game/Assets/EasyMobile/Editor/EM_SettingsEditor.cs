using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using EasyMobile;

namespace EasyMobile.Editor
{
    [CustomEditor(typeof(EM_Settings))]
    internal partial class EM_SettingsEditor : UnityEditor.Editor
    {
        #region Modules

        public enum Section
        {
            None = -1,
            Settings = 0,
            Build = 1,
            Tools = 2
        }

        static Section activeSection = Section.Settings;
        static Module activeModule = Module.Advertising;

        #endregion

        #region Target properties
        //Runtime auto initialization
        SerializedProperty autoInitializationProperty;

        // Module toggles
        SerializedProperty isAdModuleEnable;
        SerializedProperty isIAPModuleEnable;
        SerializedProperty isGameServiceModuleEnable;
        SerializedProperty isNotificationModuleEnable;
        SerializedProperty isSharingModuleEnable;

        // Active module (currently selected on the toolbar)
        SerializedProperty activeModuleIndex;
        SerializedProperty isSelectingModule;

        public class EMProperty
        {
            public SerializedProperty property;
            public GUIContent content;

            public EMProperty(GUIContent c)
            {
                content = c;
            }

            public EMProperty(SerializedProperty p, GUIContent c)
            {
                property = p;
                content = c;
            }
        }

        // Ad module properties
        private static class AdProperties
        {
            public static SerializedProperty mainProperty;
            public static SerializedProperty adColonySettings;
            public static SerializedProperty admobSettings;
            public static SerializedProperty appLovinSettings;
            public static SerializedProperty chartboostSettings;
            public static SerializedProperty fbAudienceSettings;
            public static SerializedProperty fairBidSettings;
            public static SerializedProperty ironSourceSettings;
            public static SerializedProperty mopubSettings;
            public static SerializedProperty tapjoySettings;
            public static SerializedProperty unityAdsSettings;
            public static SerializedProperty vungleAdsSettings;

            // Auto ad-loading settings.
            public static EMProperty autoLoadAdsMode = new EMProperty(null, new GUIContent("Auto Ad-Loading Mode"));
            public static EMProperty adCheckingInterval = new EMProperty(null, new GUIContent("Ad Checking Interval", "Time (seconds) between 2 ad-availability checks"));
            public static EMProperty adLoadingInterval = new EMProperty(null, new GUIContent("Ad Loading Interval", "Minimum time (seconds) between two ad-loading requests, this is to restrict the number of requests sent to ad networks"));

            // Default ad networks settings.
            public static EMProperty iosDefaultAdNetworks = new EMProperty(null, new GUIContent("iOS"));
            public static EMProperty androidDefaultAdNetworks = new EMProperty(null, new GUIContent("Android"));

            // AdColony properties.
            public static EMProperty adColonyEnabled = new EMProperty(new GUIContent("Ad Colony enable"));
            public static EMProperty adColonyAppId = new EMProperty(new GUIContent("App ID"));
            public static EMProperty adColonyAdOrientation = new EMProperty(new GUIContent("Ad Orientation"));
            public static EMProperty adColonyEnableRewardedAdPrePopup = new EMProperty(new GUIContent("Show Rewarded Ad PrePopup", "Show popup before the rewarded video starts"));
            public static EMProperty adColonyEnableRewardedAdPostPopup = new EMProperty(new GUIContent("Show Rewarded Ad PostPopup", "Show popup after the rewarded video has finished"));
            public static EMProperty adColonyDefaultInterstitialAdId = new EMProperty(new GUIContent("Interstitial Ad"));
            public static EMProperty adColonyDefaultRewardedAdId = new EMProperty(new GUIContent("Rewarded Ad"));
            public static EMProperty adColonyDefaultBannerAdId = new EMProperty(new GUIContent("Banner Ad"));
            public static EMProperty adColonyCustomInterstitialAdIds = new EMProperty(new GUIContent("Interstitial Ads"));
            public static EMProperty adColonyCustomRewardedAdIds = new EMProperty(new GUIContent("Rewarded Ads"));
            public static EMProperty adColonyCustomBannerAdIds = new EMProperty(new GUIContent("Banner Ads"));

            // Admob properties.
            public static EMProperty adMobEnabled = new EMProperty(new GUIContent("AdMob enable"));
            public static EMProperty admobAppId = new EMProperty(new GUIContent("App ID"));
            public static EMProperty admobDefaultBannerAdId = new EMProperty(new GUIContent("Banner Ad"));
            public static EMProperty admobDefaultInterstitialAdId = new EMProperty(new GUIContent("Interstitial Ad"));
            public static EMProperty admobDefaultRewardedAdId = new EMProperty(new GUIContent("Rewarded Ad"));
            public static EMProperty admobCustomBannerAdIds = new EMProperty(new GUIContent("Banner Ads"));
            public static EMProperty admobCustomInterstitialAdIds = new EMProperty(new GUIContent("Interstitial Ads"));
            public static EMProperty admobCustomRewardedAdIds = new EMProperty(new GUIContent("Rewarded Ads"));
            public static EMProperty admobEnableTestMode = new EMProperty(new GUIContent("Enable Test Mode"));
            public static EMProperty admobTestDeviceIds = new EMProperty(new GUIContent("Test Device IDs"));
            public static EMProperty admobTargetingSettings = new EMProperty(new GUIContent("Targeting Settings"));

            // AppLovin properties.
            public static EMProperty appLovinEnabled = new EMProperty(new GUIContent("Applovin enable"));
            public static EMProperty appLovinSDKKey = new EMProperty(new GUIContent("SDK Key"));
            public static EMProperty appLovinDefaultBannerAdId = new EMProperty(new GUIContent("Banner Ad"));
            public static EMProperty appLovinDefaultInterstitialAdId = new EMProperty(new GUIContent("Interstitial Ad"));
            public static EMProperty appLovinDefaultRewardedAdId = new EMProperty(new GUIContent("Rewarded Ad"));
            public static EMProperty appLovinCustomBannerAdIds = new EMProperty(new GUIContent("Banner Ads"));
            public static EMProperty appLovinCustomInterstitialAdIds = new EMProperty(new GUIContent("Interstitial Ads"));
            public static EMProperty appLovinCustomRewardedAdIds = new EMProperty(new GUIContent("Rewarded Ads"));
            public static EMProperty appLovinEnableAgeRestricted = new EMProperty(new GUIContent("Enable Age-Restricted"));

            // Chartboost properties.
            public static EMProperty chartboostEnabled = new EMProperty(new GUIContent("ChartBoost enable"));
            public static EMProperty chartboostCustomInterstitialPlacements = new EMProperty(new GUIContent("Interstitial Ads"));
            public static EMProperty chartboostCustomRewardedPlacements = new EMProperty(new GUIContent("Rewarded Ads"));

            // Facebook Audience properties.
            public static EMProperty facebookAdEnabled = new EMProperty(new GUIContent("Facebook Ad enable"));
            public static EMProperty fbAudienceDefaultBannerAdId = new EMProperty(new GUIContent("Banner Ad"));
            public static EMProperty fbAudienceDefaultInterstitialAdId = new EMProperty(new GUIContent("Interstitial Ad"));
            public static EMProperty fbAudienceDefaultRewardedAdId = new EMProperty(new GUIContent("Rewarded Ad"));
            public static EMProperty fbAudienceCustomBannerAdIds = new EMProperty(new GUIContent("Banner Ads"));
            public static EMProperty fbAudienceCustomInterstitialAdIds = new EMProperty(new GUIContent("Interstitial Ads"));
            public static EMProperty fbAudienceCustomRewardedAdIds = new EMProperty(new GUIContent("Rewarded Ads"));
            public static EMProperty fbAudienceBannerSize = new EMProperty(new GUIContent("Banner Size"));
            public static EMProperty fbAudienceEnableTestMode = new EMProperty(new GUIContent("Enable Test Mode"));
            public static EMProperty fbAudienceTestDeviceIds = new EMProperty(new GUIContent("Test Device IDs"));

            // FairBid properties.
            public static EMProperty fairBitEnabled = new EMProperty(new GUIContent("FairBit enable"));
            public static EMProperty fairBidPublisherId = new EMProperty(null, new GUIContent("FairBid Publisher ID"));
            public static EMProperty fairBidShowTestSuite = new EMProperty(null, new GUIContent("Show FairBid Test Suite"));

            public static EMProperty fairBidDefaultBannerAdId = new EMProperty(new GUIContent("Banner Ad"));
            public static EMProperty fairBidDefaultInterstitialAdId = new EMProperty(new GUIContent("Interstitial Ad"));
            public static EMProperty fairBidDefaultRewardedAdId = new EMProperty(new GUIContent("Rewarded Ad"));
            public static EMProperty fairBidCustomBannerPlacements = new EMProperty(new GUIContent("Banner Ads"));
            public static EMProperty fairBidCustomInterstitialPlacements = new EMProperty(new GUIContent("Interstitial Ads"));
            public static EMProperty fairBidCustomRewardedPlacements = new EMProperty(new GUIContent("Rewarded Ads"));

            // IronSource properties.
            public static EMProperty ironSourceEnabled = new EMProperty(new GUIContent("IronSource enable"));
            public static EMProperty ironSourceAppKey = new EMProperty(null, new GUIContent("App Key"));
            public static EMProperty ironSourceUseAdvancedSetting = new EMProperty(null, new GUIContent("Use Advanced Settings"));
            public static EMProperty ironSourceSegmentSettings = new EMProperty(null, new GUIContent("Segment Settings"));

            // Mopub properties.
            public static EMProperty mopubEnabled = new EMProperty(new GUIContent("Mopub enable"));
            public static EMProperty mopubDefaultBannerAdId = new EMProperty(new GUIContent("Banner Ad"));
            public static EMProperty mopubDefaultInterstitialAdId = new EMProperty(new GUIContent("Interstitial Ad"));
            public static EMProperty mopubDefaultRewardedAdId = new EMProperty(new GUIContent("Rewarded Ad"));
            public static EMProperty mopubCustomBannerAdIds = new EMProperty(new GUIContent("Banner Ads"));
            public static EMProperty mopubCustomInterstitialAdIds = new EMProperty(new GUIContent("Interstitial Ads"));
            public static EMProperty mopubCustomRewardedAdIds = new EMProperty(new GUIContent("Rewarded Ads"));
            public static EMProperty mopubReportAppOpen = new EMProperty(null, new GUIContent("Report App Open", "MoPub Report Application Opened Event Trigger"));
            public static EMProperty mopubITuneAppID = new EMProperty(null, new GUIContent("App ID", "App ID, used to report application opened event"));
            public static EMProperty mopubEnableLocationPassing = new EMProperty(null, new GUIContent("Enable Location Passing", "MoPub Location Passing Trigger"));
            public static EMProperty mopubEnableAdvancedSetting = new EMProperty(null, new GUIContent("Use Advanced Settings"));
            public static EMProperty mopubAllowLegitimateInterest = new EMProperty(null, new GUIContent("Allow Legitimate Interest", "Allow supported SDK networks to collect user information on the basis of legitimate interest."));
            public static EMProperty mopubLogLevel = new EMProperty(null, new GUIContent("Log Level", "Changes this to include more detailed information such as adapter version, SDK version and ad life cycle events and operations when logging."));
            public static EMProperty mopubMediatedNetworks = new EMProperty(null, new GUIContent("Mediated Networks", "Networks used in advanced initialization."));
            public static EMProperty mopubAutoRequestConsent = new EMProperty(null, new GUIContent("Auto Request Consent", "Auto request GDPR consent dialog when MoPub SDK is initialized and show it if applicable"));
            public static EMProperty mopubForceGdprApplicable = new EMProperty(null, new GUIContent("Force GDPR Applicable", "Enable this to show GPDR consent in all regions, FOR DEBUG PURPOSE ONLY"));

            // Tapjoy properties.
            public static EMProperty tapJoyEnabled = new EMProperty(new GUIContent("Tap joy enable"));
            public static EMProperty tapjoyDefaultInterstitialAdId = new EMProperty(new GUIContent("Interstitial Ad"));
            public static EMProperty tapjoyDefaultRewardedAdId = new EMProperty(new GUIContent("Rewarded Ad"));
            public static EMProperty tapjoyCustomInterstitialAdIds = new EMProperty(new GUIContent("Interstitial Ads"));
            public static EMProperty tapjoyCustomRewardedAdIds = new EMProperty(new GUIContent("Rewarded Ads"));
            public static EMProperty tapjoyAutoReconnect = new EMProperty(null, new GUIContent("Auto Reconnect", "Auto reconnect to TapJoy server until successfully connected"));
            public static EMProperty tapjoyAutoReconnectInterval = new EMProperty(null, new GUIContent("Auto Reconnect Interval"));

            // UnityAds properties.
            public static EMProperty unityAdEnabled = new EMProperty(new GUIContent("Unity Ads enable"));
            public static EMProperty unityAdsAppId = new EMProperty(new GUIContent("App ID"));
            public static EMProperty unityAdsDefaultBannerAdId = new EMProperty(new GUIContent("Banner Ad"));
            public static EMProperty unityAdsDefaultInterstitialAdId = new EMProperty(new GUIContent("Interstitial Ad"));
            public static EMProperty unityAdsDefaultRewardedAdId = new EMProperty(new GUIContent("Rewarded Ad"));
            public static EMProperty unityAdsCustomBannerAdId = new EMProperty(new GUIContent("Banner Ad"));
            public static EMProperty unityAdsCustomInterstitialAdIds = new EMProperty(new GUIContent("Interstitial Ads"));
            public static EMProperty unityAdsCustomRewardedAdIds = new EMProperty(new GUIContent("Rewarded Ads"));
            public static EMProperty unityAdsEnableTestMode = new EMProperty(new GUIContent("Enable Test Mode"));

            // Vungle properties
            public static EMProperty vungleAdEnabled = new EMProperty(new GUIContent("Vungle Ad enable"));
            public static EMProperty vungleAdsAppId = new EMProperty(new GUIContent("App ID"));
            public static EMProperty vungleDefaultInterstitialAdId = new EMProperty(new GUIContent("Interstitial Ad"));
            public static EMProperty vungleDefaultRewardedAdId = new EMProperty(new GUIContent("Rewarded Ad"));
            public static EMProperty vungleDefaultBannerAdId = new EMProperty(new GUIContent("Banner Ad"));
            public static EMProperty vungleCustomInterstitialAdIds = new EMProperty(new GUIContent("Interstitial Ads"));
            public static EMProperty vungleCustomRewardedAdIds = new EMProperty(new GUIContent("Rewarded Ads"));
            public static EMProperty vungleCustomBannerAdIds = new EMProperty(new GUIContent("Banner Ads"));
            public static EMProperty vungleUseAdvancedSettings = new EMProperty(new GUIContent("Advanced settings"));
            public static EMProperty vungleAdvancedSettings = new EMProperty(new GUIContent("Vungle advanced settings"));
        }

        // In App Purchase module properties
        private static class IAPProperties
        {
            public static SerializedProperty mainProperty;
            public static EMProperty autoInit = new EMProperty(null, new GUIContent("Auto Init", "Whether the module should automatically initialize itself"));
            public static EMProperty targetAndroidStore = new EMProperty(null, new GUIContent("Target Android Store", "Target Android store"));
            public static EMProperty enableAmazoneSandboxTesting = new EMProperty(null, new GUIContent("Enable Amazon Sandbox Testing", "Generate a JSON description of your product catalog on the device’s SD card to use Amazon’s local Sandbox testing app. " +
                                                                           "Remember to enable External (SDCard) write permission in Player settings for Android platform"));
            public static EMProperty simulateAppleAskToBuy = new EMProperty(null, new GUIContent("Simulate Ask-To-Buy", "Enable Apple's Ask To Buy simulation in the sandbox app store"));
            public static EMProperty interceptApplePromotionalPurchases = new EMProperty(null, new GUIContent("Intercept Promotional Purchases", "Whether to intercept Apple's promotional purchases"));
            public static EMProperty validateAppleReceipt = new EMProperty(null, new GUIContent("Validate Apple Receipt", "Validate receipts from Apple App stores"));
            public static EMProperty validateGooglePlayReceipt = new EMProperty(null, new GUIContent("Validate Google Play Receipt", "Validate receipts from Google Play store"));
            public static EMProperty products = new EMProperty(null, new GUIContent("Products"));
        }

        // Game Services module properties
        private static class GameServiceProperties
        {
            public static SerializedProperty mainProperty;
            public static EMProperty gpgsDebugLog = new EMProperty(null, new GUIContent("Debug Log", "Show debug log from Google Play Games plugin"));
            public static EMProperty gpgsPopupGravity = new EMProperty(null, new GUIContent("Popup Gravity", "Sets the gravity for popups on Google Play Games platform"));
            public static EMProperty gpgsXmlResources = new EMProperty(null, new GUIContent("Android XML Resources", "The XML resources exported from Google Play Console"));
            public static EMProperty autoInit = new EMProperty(null, new GUIContent("Auto Init", "Whether the module should automatically initialize itself"));
            public static EMProperty autoInitAfterUserLogout = new EMProperty(null, new GUIContent("Auto Init (User Logged Out)", "Whether the module should automatically initialize itself when the user has logged out in the previous session."));
            public static EMProperty autoInitDelay = new EMProperty(null, new GUIContent("Auto Init Delay", "Delay time (seconds) after Start() that the service is automatically initialized"));
            public static EMProperty androidMaxLoginRequest =
                new EMProperty(null,
                    new GUIContent("[Android] Max Login Requests",
                        "[Auto-init and ManagedInit only] The total number of times the login popup can be displayed if the user has not logged in. " +
                        "When this value is reached, the init process will stop thus not showing the login popup anymore (avoid annoying the user). " +
                        "Set to 0 to ignore this limit."));

            public static EMProperty gpgsShouldRequestServerAuthCode = new EMProperty(null, new GUIContent("Request ServerAuthCode", "Whether to request a server authentication code during initialization on Google Play Games platform."));
            public static EMProperty gpgsForceRefreshServerAuthCode = new EMProperty(null, new GUIContent("Force Refresh", "Whether to force refresh while requesting a server authentication code " +
                "during initialization on Google Play Games platform."));
            public static EMProperty gpgsOauthScopes = new EMProperty(null, new GUIContent("OAuth Scopes", "The OAuth scopes to be added during initialization on Google Play Games platform."));
            public static EMProperty leaderboards = new EMProperty(null, new GUIContent("Leaderboards"));
            public static EMProperty achievements = new EMProperty(null, new GUIContent("Achievements"));

            // Saved Games and Multiplayer are PRO features
#if EASY_MOBILE_PRO
            public static EMProperty enableSavedGames = new EMProperty(null, new GUIContent("Enable Saved Games", "Enable the Saved Games feature"));
            public static EMProperty autoConflictResolutionStrategy = new EMProperty(null, new GUIContent("Conflict Resolution Strategy", "The default strategy used for automatic resolution of saved game conflicts"));
            public static EMProperty gpgsDataSource = new EMProperty(null, new GUIContent("[Android] Data Source", "The source from where saved game data should be fetched when using Google Play Games on Android"));
            public static EMProperty enableMultiplayer = new EMProperty(null, new GUIContent("Enable Multiplayer", "Enable the Multiplayer feature"));
#endif
        }

        // Notification module properties
        private static class NotificationProperties
        {
            public static SerializedProperty mainProperty;
            public static EMProperty autoInit = new EMProperty(null, new GUIContent("Auto Init", "Whether the module should automatically initialize itself"));
            public static EMProperty autoInitDelay = new EMProperty(null, new GUIContent("Auto Init Delay", "Delay time (seconds) after Start() that the service is automatically initialized"));
            public static EMProperty iosAuthOptions = new EMProperty(null, new GUIContent("[iOS] Authorization Options", "The types of notification interaction your app requests on iOS"));
            public static EMProperty pushNotificationService = new EMProperty(null, new GUIContent("Push Notification Service", "The service used for push (remote) notifications"));
            public static EMProperty oneSignalAppId = new EMProperty(null, new GUIContent("OneSignal App ID", "The app ID obtained from OneSignal dashboard"));
            public static EMProperty firebaseTopics = new EMProperty(null, new GUIContent("Firebase Topics"));
            public static EMProperty categoryGroups = new EMProperty(null, new GUIContent("Category Groups", "Optional groups for notification categories"));
            public static EMProperty defaultCategory = new EMProperty(null, new GUIContent("Default Category", "The default notification category. Your app mush have at least one category, you can modify this default category but not remove it."));
            public static EMProperty userCategories = new EMProperty(null, new GUIContent("User Categories", "Custom notification categories for your app"));
        }

        // Privacy module properties
        private static class PrivacyProperties
        {
            public static SerializedProperty mainProperty;
            public static SerializedProperty consentDialogProperty;
            public static EMProperty consentDialogContent = new EMProperty(null, new GUIContent("Consent Dialog Content"));
            public static EMProperty consentDialogTitle = new EMProperty(null, new GUIContent("Consent Dialog Title"));
            public static EMProperty consentDialogToggles = new EMProperty(null, new GUIContent("Consent Dialog Toggles"));
            public static EMProperty consentDialogActionButtons = new EMProperty(null, new GUIContent("Consent Dialog Action Buttons"));
            public static EMProperty selectedToggleIndex = new EMProperty(null, new GUIContent("Selected Toggle Index"));
            public static EMProperty selectedButtonIndex = new EMProperty(null, new GUIContent("Selected Button Index"));
            public static EMProperty enableCopyPasteMode = new EMProperty(null, new GUIContent("Enable Copy & Paste", "We need to do this because GUILayout.TextArea, the only TextArea that work with TextEditor, doesn't support copy & paste."));
        }

        // Utility module consists other sub-module properties
        // RatingRequestSettings properties
        private static class RatingRequestProperties
        {
            public static SerializedProperty mainProperty;
            public static EMProperty defaultRatingDialogContent = new EMProperty(null, new GUIContent("Default Dialog Content", "Content of the rating dialog used on Android and iOS older than 10.3"));
            public static EMProperty minimumAcceptedStars = new EMProperty(null, new GUIContent("Minimum Accepted Rating", "[Android only] The lowest rating/stars accepted. If fewer stars are given, we'll suggest the user to give feedback instead. Set this to 0 to disable the feedback feature"));
            public static EMProperty supportEmail = new EMProperty(null, new GUIContent("Support Email", "The email address to receive feedback"));
            public static EMProperty iosAppId = new EMProperty(null, new GUIContent("iOS App ID", "App ID on the Apple App Store"));
            public static EMProperty annualCap = new EMProperty(null, new GUIContent("Annual Cap", "Maximum number of requests allowed each year, note that on iOS 10.3+ this value is governed by the OS and is always set to 3"));
            public static EMProperty delayAfterInstallation = new EMProperty(null, new GUIContent("Delay After Installation", "Waiting time (in days) after app installation before the first rating request can be made"));
            public static EMProperty coolingOffPeriod = new EMProperty(null, new GUIContent("Cooling-Off Period", "The mininum interval required (in days) between two consecutive requests."));
            public static EMProperty ignoreConstraintsInDevelopment = new EMProperty(null, new GUIContent("Ignore Constraints In Development", "Ignore all display constraints so the rating popup can be shown every time in development builds, unless it was disabled before"));
        }

#if EASY_MOBILE_PRO
        // Native APIs module properties
        private static class NativeApisProperties
        {
            public static SerializedProperty mainProperty;
            public static EMProperty isMediaEnabled = new EMProperty(null, new GUIContent("Enable Camera & Gallery", "Enable native camera & gallery apis."));
            public static EMProperty isContactsEnabled = new EMProperty(null, new GUIContent("Enable Contacts", "Enable native contacts apis."));
            public static EMProperty iOSPhotoUsageDescription = new EMProperty(null, new GUIContent("Photo Usage Description", "[iOS] Will be added into Info.plist if the Media module is enabled."));
            public static EMProperty iOSAddPhotoUsageDescription = new EMProperty(null, new GUIContent("Add Photo Usage Description", "[IOS] Will be added into Info.plist of the Media module is enabled. Used when adding photo(s) into device."));
            public static EMProperty iOSCameraUsageDescription = new EMProperty(null, new GUIContent("Camera Usage Description", "[iOS] Will be added into Info.plist if the Media module is enabled."));
            public static EMProperty iOSMicrophoneUsageDescription = new EMProperty(null, new GUIContent("Microphone Usage Description", "[iOS] Will be added into Info.plist if the Media module is enabled. Used when recording video."));
            public static EMProperty iOSContactUsageDescription = new EMProperty(null, new GUIContent("Contacts Usage Description", "[iOS] Will be added into Info.plist if the Contact module is enabled."));
            public static EMProperty iOSExtraUsageDescriptions = new EMProperty(null, new GUIContent("Extra iOS Usage Descriptions", "Define usage descriptions here to add them into Info.plist when building to iOS."));
        }
#endif

        #endregion

        #region Variables

        // List of Android leaderboard and achievement ids constructed from the generated GPGSIds class.
        private SortedDictionary<string, string> gpgsIdDict;

        /// <summary>
        /// Little trick to draw the setting GUI only in editor window.
        /// </summary>
        public static bool callFromEditorWindow = false;

        // Show this message to users if they open the EM_Setting ScriptableObject directly in the inspector.
        public const string OpenInInspectorWarning = "This ScriptableObject holds all the settings of EasyMobile. " +
                                                     "Please go to menu Window > EasyMobile or click the button below to edit it.";

#if EM_UIAP
        // Booleans indicating whether AppleTangle and GooglePlayTangle are not dummy classes.
        bool isAppleTangleValid;
        bool isGooglePlayTangleValid;
#endif

        #endregion

        #region GUI

        void OnEnable()
        {
            // Runtime properties.
            autoInitializationProperty = serializedObject.FindProperty("mRuntimeAutoInitialization");
            
            // Module-control properties.
            isAdModuleEnable = serializedObject.FindProperty("mIsAdModuleEnable");
            isIAPModuleEnable = serializedObject.FindProperty("mIsIAPModuleEnable");
            isGameServiceModuleEnable = serializedObject.FindProperty("mIsGameServiceModuleEnable");
            isNotificationModuleEnable = serializedObject.FindProperty("mIsNotificationModuleEnable");
            isSharingModuleEnable = serializedObject.FindProperty("mIsSharingModuleEnable");

            activeModuleIndex = serializedObject.FindProperty("mActiveModuleIndex");
            isSelectingModule = serializedObject.FindProperty("mIsSelectingModule");

            if (System.Enum.IsDefined(typeof(Module), activeModuleIndex.intValue))
            {
                activeModule = (Module)activeModuleIndex.intValue;
            }

            //--------------------------------------------------------------
            // Ad module properties.
            //--------------------------------------------------------------
            AdProperties.mainProperty = serializedObject.FindProperty("mAdvertisingSettings");

            // Auto ad-loading.
            AdProperties.autoLoadAdsMode.property = AdProperties.mainProperty.FindPropertyRelative("mAutoLoadAdsMode");
            AdProperties.adCheckingInterval.property = AdProperties.mainProperty.FindPropertyRelative("mAdCheckingInterval");
            AdProperties.adLoadingInterval.property = AdProperties.mainProperty.FindPropertyRelative("mAdLoadingInterval");

            // Default ad networks.
            AdProperties.iosDefaultAdNetworks.property = AdProperties.mainProperty.FindPropertyRelative("mIosDefaultAdNetworks");
            AdProperties.androidDefaultAdNetworks.property = AdProperties.mainProperty.FindPropertyRelative("mAndroidDefaultAdNetworks");

            // AdColony properties.
            AdProperties.adColonySettings = AdProperties.mainProperty.FindPropertyRelative("mAdColony");
            AdProperties.adColonyEnabled.property = AdProperties.adColonySettings.FindPropertyRelative("mEnable");
            AdProperties.adColonyAppId.property = AdProperties.adColonySettings.FindPropertyRelative("mAppId");
            AdProperties.adColonyAdOrientation.property = AdProperties.adColonySettings.FindPropertyRelative("mOrientation");
            AdProperties.adColonyEnableRewardedAdPrePopup.property = AdProperties.adColonySettings.FindPropertyRelative("mEnableRewardedAdPrePopup");
            AdProperties.adColonyEnableRewardedAdPostPopup.property = AdProperties.adColonySettings.FindPropertyRelative("mEnableRewardedAdPostPopup");
            AdProperties.adColonyDefaultInterstitialAdId.property = AdProperties.adColonySettings.FindPropertyRelative("mDefaultInterstitialAdId");
            AdProperties.adColonyDefaultRewardedAdId.property = AdProperties.adColonySettings.FindPropertyRelative("mDefaultRewardedAdId");
            AdProperties.adColonyDefaultBannerAdId.property = AdProperties.adColonySettings.FindPropertyRelative("mDefaultBannerAdId");
            AdProperties.adColonyCustomInterstitialAdIds.property = AdProperties.adColonySettings.FindPropertyRelative("mCustomInterstitialAdIds");
            AdProperties.adColonyCustomRewardedAdIds.property = AdProperties.adColonySettings.FindPropertyRelative("mCustomRewardedAdIds");
            AdProperties.adColonyCustomBannerAdIds.property = AdProperties.adColonySettings.FindPropertyRelative("mCustomBannerAdIds");

            // AdMob properties.
            AdProperties.admobSettings = AdProperties.mainProperty.FindPropertyRelative("mAdMob");
            AdProperties.adMobEnabled.property = AdProperties.admobSettings.FindPropertyRelative("mEnable");
            AdProperties.admobAppId.property = AdProperties.admobSettings.FindPropertyRelative("mAppId");
            AdProperties.admobDefaultBannerAdId.property = AdProperties.admobSettings.FindPropertyRelative("mDefaultBannerAdId");
            AdProperties.admobDefaultInterstitialAdId.property = AdProperties.admobSettings.FindPropertyRelative("mDefaultInterstitialAdId");
            AdProperties.admobDefaultRewardedAdId.property = AdProperties.admobSettings.FindPropertyRelative("mDefaultRewardedAdId");
            AdProperties.admobCustomBannerAdIds.property = AdProperties.admobSettings.FindPropertyRelative("mCustomBannerAdIds");
            AdProperties.admobCustomInterstitialAdIds.property = AdProperties.admobSettings.FindPropertyRelative("mCustomInterstitialAdIds");
            AdProperties.admobCustomRewardedAdIds.property = AdProperties.admobSettings.FindPropertyRelative("mCustomRewardedAdIds");
            AdProperties.admobEnableTestMode.property = AdProperties.admobSettings.FindPropertyRelative("mEnableTestMode");
            AdProperties.admobTestDeviceIds.property = AdProperties.admobSettings.FindPropertyRelative("mTestDeviceIds");
            AdProperties.admobTargetingSettings.property = AdProperties.admobSettings.FindPropertyRelative("mTargetingSettings");

            // AppLovin properties.
            AdProperties.appLovinSettings = AdProperties.mainProperty.FindPropertyRelative("mAppLovin");
            AdProperties.appLovinEnabled.property = AdProperties.appLovinSettings.FindPropertyRelative("mEnable");
            AdProperties.appLovinSDKKey.property = AdProperties.appLovinSettings.FindPropertyRelative("mSDKKey");
            AdProperties.appLovinDefaultBannerAdId.property = AdProperties.appLovinSettings.FindPropertyRelative("mDefaultBannerAdId");
            AdProperties.appLovinDefaultInterstitialAdId.property = AdProperties.appLovinSettings.FindPropertyRelative("mDefaultInterstitialAdId");
            AdProperties.appLovinDefaultRewardedAdId.property = AdProperties.appLovinSettings.FindPropertyRelative("mDefaultRewardedAdId");
            AdProperties.appLovinCustomBannerAdIds.property = AdProperties.appLovinSettings.FindPropertyRelative("mCustomBannerAdIds");
            AdProperties.appLovinCustomInterstitialAdIds.property = AdProperties.appLovinSettings.FindPropertyRelative("mCustomInterstitialAdIds");
            AdProperties.appLovinCustomRewardedAdIds.property = AdProperties.appLovinSettings.FindPropertyRelative("mCustomRewardedAdIds");
            AdProperties.appLovinEnableAgeRestricted.property = AdProperties.appLovinSettings.FindPropertyRelative("mAgeRestrictMode");

            // Chartboost properties.
            AdProperties.chartboostSettings = AdProperties.mainProperty.FindPropertyRelative("mChartboost");
            AdProperties.chartboostEnabled.property = AdProperties.chartboostSettings.FindPropertyRelative("mEnable");
            AdProperties.chartboostCustomInterstitialPlacements.property = AdProperties.chartboostSettings.FindPropertyRelative("mCustomInterstitialPlacements");
            AdProperties.chartboostCustomRewardedPlacements.property = AdProperties.chartboostSettings.FindPropertyRelative("mCustomRewardedPlacements");

            // Facebook Audience properties.
            AdProperties.fbAudienceSettings = AdProperties.mainProperty.FindPropertyRelative("mFBAudience");
            AdProperties.facebookAdEnabled.property = AdProperties.fbAudienceSettings.FindPropertyRelative("mEnable");
            AdProperties.fbAudienceDefaultBannerAdId.property = AdProperties.fbAudienceSettings.FindPropertyRelative("mDefaultBannerId");
            AdProperties.fbAudienceDefaultInterstitialAdId.property = AdProperties.fbAudienceSettings.FindPropertyRelative("mDefaultInterstitialAdId");
            AdProperties.fbAudienceDefaultRewardedAdId.property = AdProperties.fbAudienceSettings.FindPropertyRelative("mDefaultRewardedAdId");
            AdProperties.fbAudienceCustomBannerAdIds.property = AdProperties.fbAudienceSettings.FindPropertyRelative("mCustomBannerIds");
            AdProperties.fbAudienceCustomInterstitialAdIds.property = AdProperties.fbAudienceSettings.FindPropertyRelative("mCustomInterstitialAdIds");
            AdProperties.fbAudienceCustomRewardedAdIds.property = AdProperties.fbAudienceSettings.FindPropertyRelative("mCustomRewardedAdIds");

            AdProperties.fbAudienceEnableTestMode.property = AdProperties.fbAudienceSettings.FindPropertyRelative("mEnableTestMode");
            AdProperties.fbAudienceTestDeviceIds.property = AdProperties.fbAudienceSettings.FindPropertyRelative("mTestDevices");
            AdProperties.fbAudienceBannerSize.property = AdProperties.fbAudienceSettings.FindPropertyRelative("mBannerAdSize");

            // FairBid properties.
            AdProperties.fairBidSettings = AdProperties.mainProperty.FindPropertyRelative("mFairBid");
            AdProperties.fairBitEnabled.property = AdProperties.fairBidSettings.FindPropertyRelative("mEnable");
            AdProperties.fairBidPublisherId.property = AdProperties.fairBidSettings.FindPropertyRelative("mPublisherId");
            AdProperties.fairBidShowTestSuite.property = AdProperties.fairBidSettings.FindPropertyRelative("mShowTestSuite");
            AdProperties.fairBidDefaultBannerAdId.property = AdProperties.fairBidSettings.FindPropertyRelative("mDefaultBannerId");
            AdProperties.fairBidDefaultInterstitialAdId.property = AdProperties.fairBidSettings.FindPropertyRelative("mDefaultInterstitialAdId");
            AdProperties.fairBidDefaultRewardedAdId.property = AdProperties.fairBidSettings.FindPropertyRelative("mDefaultRewardedAdId");
            AdProperties.fairBidCustomBannerPlacements.property = AdProperties.fairBidSettings.FindPropertyRelative("mCustomBannerPlacements");
            AdProperties.fairBidCustomInterstitialPlacements.property = AdProperties.fairBidSettings.FindPropertyRelative("mCustomInterstitialPlacements");
            AdProperties.fairBidCustomRewardedPlacements.property = AdProperties.fairBidSettings.FindPropertyRelative("mCustomRewardedPlacements");

            // IronSource properties.
            AdProperties.ironSourceSettings = AdProperties.mainProperty.FindPropertyRelative("mIronSource");
            AdProperties.ironSourceEnabled.property = AdProperties.ironSourceSettings.FindPropertyRelative("mEnable");
            AdProperties.ironSourceAppKey.property = AdProperties.ironSourceSettings.FindPropertyRelative("mAppId");
            AdProperties.ironSourceUseAdvancedSetting.property = AdProperties.ironSourceSettings.FindPropertyRelative("mUseAdvancedSetting");
            AdProperties.ironSourceSegmentSettings.property = AdProperties.ironSourceSettings.FindPropertyRelative("mSegments");

            // Mopub properties.
            AdProperties.mopubSettings = AdProperties.mainProperty.FindPropertyRelative("mMoPub");
            AdProperties.mopubEnabled.property = AdProperties.mopubSettings.FindPropertyRelative("mEnable");
            AdProperties.mopubDefaultBannerAdId.property = AdProperties.mopubSettings.FindPropertyRelative("mDefaultBannerId");
            AdProperties.mopubDefaultInterstitialAdId.property = AdProperties.mopubSettings.FindPropertyRelative("mDefaultInterstitialAdId");
            AdProperties.mopubDefaultRewardedAdId.property = AdProperties.mopubSettings.FindPropertyRelative("mDefaultRewardedAdId");
            AdProperties.mopubCustomBannerAdIds.property = AdProperties.mopubSettings.FindPropertyRelative("mCustomBannerIds");
            AdProperties.mopubCustomInterstitialAdIds.property = AdProperties.mopubSettings.FindPropertyRelative("mCustomInterstitialAdIds");
            AdProperties.mopubCustomRewardedAdIds.property = AdProperties.mopubSettings.FindPropertyRelative("mCustomRewardedAdIds");

            AdProperties.mopubReportAppOpen.property = AdProperties.mopubSettings.FindPropertyRelative("mReportAppOpen");
            AdProperties.mopubITuneAppID.property = AdProperties.mopubSettings.FindPropertyRelative("mITuneAppID");
            AdProperties.mopubEnableLocationPassing.property = AdProperties.mopubSettings.FindPropertyRelative("mEnableLocationPassing");
            AdProperties.mopubEnableAdvancedSetting.property = AdProperties.mopubSettings.FindPropertyRelative("mUseAdvancedSetting");
            AdProperties.mopubAllowLegitimateInterest.property = AdProperties.mopubSettings.FindPropertyRelative("mAllowLegitimateInterest");
            AdProperties.mopubLogLevel.property = AdProperties.mopubSettings.FindPropertyRelative("mLogLevel");
            AdProperties.mopubMediatedNetworks.property = AdProperties.mopubSettings.FindPropertyRelative("mMediatedNetworks");
            AdProperties.mopubAutoRequestConsent.property = AdProperties.mopubSettings.FindPropertyRelative("mAutoRequestConsent");
            AdProperties.mopubForceGdprApplicable.property = AdProperties.mopubSettings.FindPropertyRelative("mForceGdprApplicable");

            // Tapjoy properties.
            AdProperties.tapjoySettings = AdProperties.mainProperty.FindPropertyRelative("mTapjoy");
            AdProperties.tapJoyEnabled.property = AdProperties.tapjoySettings.FindPropertyRelative("mEnable");
            AdProperties.tapjoyDefaultInterstitialAdId.property = AdProperties.tapjoySettings.FindPropertyRelative("mDefaultInterstitialAdId");
            AdProperties.tapjoyDefaultRewardedAdId.property = AdProperties.tapjoySettings.FindPropertyRelative("mDefaultRewardedAdId");
            AdProperties.tapjoyCustomInterstitialAdIds.property = AdProperties.tapjoySettings.FindPropertyRelative("mCustomInterstitialAdIds");
            AdProperties.tapjoyCustomRewardedAdIds.property = AdProperties.tapjoySettings.FindPropertyRelative("mCustomRewardedAdIds");
            AdProperties.tapjoyAutoReconnect.property = AdProperties.tapjoySettings.FindPropertyRelative("mAutoReconnect");
            AdProperties.tapjoyAutoReconnectInterval.property = AdProperties.tapjoySettings.FindPropertyRelative("mAutoReconnectInterval");

            // UnityAds properties.
            AdProperties.unityAdsSettings = AdProperties.mainProperty.FindPropertyRelative("mUnityAds");
            AdProperties.unityAdEnabled.property = AdProperties.unityAdsSettings.FindPropertyRelative("mEnable");
            AdProperties.unityAdsAppId.property = AdProperties.unityAdsSettings.FindPropertyRelative("mAppId");
            AdProperties.unityAdsDefaultBannerAdId.property = AdProperties.unityAdsSettings.FindPropertyRelative("mDefaultBannerAdId");
            AdProperties.unityAdsDefaultInterstitialAdId.property = AdProperties.unityAdsSettings.FindPropertyRelative("mDefaultInterstitialAdId");
            AdProperties.unityAdsDefaultRewardedAdId.property = AdProperties.unityAdsSettings.FindPropertyRelative("mDefaultRewardedAdId");
            AdProperties.unityAdsCustomBannerAdId.property = AdProperties.unityAdsSettings.FindPropertyRelative("mCustomBannerAdIds");
            AdProperties.unityAdsCustomInterstitialAdIds.property = AdProperties.unityAdsSettings.FindPropertyRelative("mCustomInterstitialAdIds");
            AdProperties.unityAdsCustomRewardedAdIds.property = AdProperties.unityAdsSettings.FindPropertyRelative("mCustomRewardedAdIds");
            AdProperties.unityAdsEnableTestMode.property = AdProperties.unityAdsSettings.FindPropertyRelative("mEnableTestMode");

            //Vungle properties.
            AdProperties.vungleAdsSettings = AdProperties.mainProperty.FindPropertyRelative("mVungleAds");
            AdProperties.vungleAdEnabled.property = AdProperties.vungleAdsSettings.FindPropertyRelative("mEnable");
            AdProperties.vungleAdsAppId.property = AdProperties.vungleAdsSettings.FindPropertyRelative("mAppId");
            AdProperties.vungleDefaultInterstitialAdId.property = AdProperties.vungleAdsSettings.FindPropertyRelative("mDefaultInterstitialAdId");
            AdProperties.vungleDefaultRewardedAdId.property = AdProperties.vungleAdsSettings.FindPropertyRelative("mDefaultRewardedAdId");
            AdProperties.vungleDefaultBannerAdId.property = AdProperties.vungleAdsSettings.FindPropertyRelative("mDefaultBannerAdId");
            AdProperties.vungleCustomInterstitialAdIds.property = AdProperties.vungleAdsSettings.FindPropertyRelative("mCustomInterstitialAdIds");
            AdProperties.vungleCustomRewardedAdIds.property = AdProperties.vungleAdsSettings.FindPropertyRelative("mCustomRewardedAdIds");
            AdProperties.vungleCustomBannerAdIds.property = AdProperties.vungleAdsSettings.FindPropertyRelative("mCustomBannerAdIds");
            AdProperties.vungleUseAdvancedSettings.property = AdProperties.vungleAdsSettings.FindPropertyRelative("mUseAdvancedSetting");
            AdProperties.vungleAdvancedSettings.property = AdProperties.vungleAdsSettings.FindPropertyRelative("mAdvancedSettings");

            // In App Purchase module properties.
            IAPProperties.mainProperty = serializedObject.FindProperty("mInAppPurchaseSettings");
            IAPProperties.autoInit.property = IAPProperties.mainProperty.FindPropertyRelative("mAutoInit");
            IAPProperties.targetAndroidStore.property = IAPProperties.mainProperty.FindPropertyRelative("mTargetAndroidStore");
            IAPProperties.enableAmazoneSandboxTesting.property = IAPProperties.mainProperty.FindPropertyRelative("mEnableAmazonSandboxTesting");
            IAPProperties.simulateAppleAskToBuy.property = IAPProperties.mainProperty.FindPropertyRelative("mSimulateAppleAskToBuy");
            IAPProperties.interceptApplePromotionalPurchases.property = IAPProperties.mainProperty.FindPropertyRelative("mInterceptApplePromotionalPurchases");
            IAPProperties.validateAppleReceipt.property = IAPProperties.mainProperty.FindPropertyRelative("mValidateAppleReceipt");
            IAPProperties.validateGooglePlayReceipt.property = IAPProperties.mainProperty.FindPropertyRelative("mValidateGooglePlayReceipt");
            IAPProperties.products.property = IAPProperties.mainProperty.FindPropertyRelative("mProducts");

            // Game Service module properties.
            GameServiceProperties.mainProperty = serializedObject.FindProperty("mGameServiceSettings");
            GameServiceProperties.gpgsDebugLog.property = GameServiceProperties.mainProperty.FindPropertyRelative("mGpgsDebugLogEnabled");
            GameServiceProperties.gpgsPopupGravity.property = GameServiceProperties.mainProperty.FindPropertyRelative("mGpgsPopupGravity");
            GameServiceProperties.gpgsXmlResources.property = GameServiceProperties.mainProperty.FindPropertyRelative("mAndroidXmlResources");
            GameServiceProperties.autoInit.property = GameServiceProperties.mainProperty.FindPropertyRelative("mAutoInit");
            GameServiceProperties.autoInitAfterUserLogout.property = GameServiceProperties.mainProperty.FindPropertyRelative("mAutoInitAfterUserLogout");
            GameServiceProperties.autoInitDelay.property = GameServiceProperties.mainProperty.FindPropertyRelative("mAutoInitDelay");
            GameServiceProperties.androidMaxLoginRequest.property = GameServiceProperties.mainProperty.FindPropertyRelative("mAndroidMaxLoginRequests");
            GameServiceProperties.gpgsShouldRequestServerAuthCode.property = GameServiceProperties.mainProperty.FindPropertyRelative("mGpgsShouldRequestServerAuthCode");
            GameServiceProperties.gpgsForceRefreshServerAuthCode.property = GameServiceProperties.mainProperty.FindPropertyRelative("mGpgsForceRefreshServerAuthCode");
            GameServiceProperties.gpgsOauthScopes.property = GameServiceProperties.mainProperty.FindPropertyRelative("mGpgsOauthScopes");
            GameServiceProperties.leaderboards.property = GameServiceProperties.mainProperty.FindPropertyRelative("mLeaderboards");
            GameServiceProperties.achievements.property = GameServiceProperties.mainProperty.FindPropertyRelative("mAchievements");

#if EASY_MOBILE_PRO
            GameServiceProperties.enableSavedGames.property = GameServiceProperties.mainProperty.FindPropertyRelative("mEnableSavedGames");
            GameServiceProperties.autoConflictResolutionStrategy.property = GameServiceProperties.mainProperty.FindPropertyRelative("mAutoConflictResolutionStrategy");
            GameServiceProperties.gpgsDataSource.property = GameServiceProperties.mainProperty.FindPropertyRelative("mGpgsDataSource");
            GameServiceProperties.enableMultiplayer.property = GameServiceProperties.mainProperty.FindPropertyRelative("mEnableMultiplayer");
#endif

            // Notification module properties.
            NotificationProperties.mainProperty = serializedObject.FindProperty("mNotificationSettings");
            NotificationProperties.autoInit.property = NotificationProperties.mainProperty.FindPropertyRelative("mAutoInit");
            NotificationProperties.autoInitDelay.property = NotificationProperties.mainProperty.FindPropertyRelative("mAutoInitDelay");
            NotificationProperties.iosAuthOptions.property = NotificationProperties.mainProperty.FindPropertyRelative("mIosAuthOptions");
            NotificationProperties.pushNotificationService.property = NotificationProperties.mainProperty.FindPropertyRelative("mPushNotificationService");
            NotificationProperties.oneSignalAppId.property = NotificationProperties.mainProperty.FindPropertyRelative("mOneSignalAppId");
            NotificationProperties.firebaseTopics.property = NotificationProperties.mainProperty.FindPropertyRelative("mFirebaseTopics");
            NotificationProperties.categoryGroups.property = NotificationProperties.mainProperty.FindPropertyRelative("mCategoryGroups");
            NotificationProperties.defaultCategory.property = NotificationProperties.mainProperty.FindPropertyRelative("mDefaultCategory");
            NotificationProperties.userCategories.property = NotificationProperties.mainProperty.FindPropertyRelative("mUserCategories");

            // Privacy module properties.
            PrivacyProperties.mainProperty = serializedObject.FindProperty("mPrivacySettings");
            PrivacyProperties.consentDialogProperty = PrivacyProperties.mainProperty.FindPropertyRelative("mDefaultConsentDialog");
            PrivacyProperties.consentDialogContent.property = PrivacyProperties.consentDialogProperty.FindPropertyRelative("mContent");
            PrivacyProperties.consentDialogTitle.property = PrivacyProperties.consentDialogProperty.FindPropertyRelative("mTitle");
            PrivacyProperties.consentDialogToggles.property = PrivacyProperties.consentDialogProperty.FindPropertyRelative("mToggles");
            PrivacyProperties.consentDialogActionButtons.property = PrivacyProperties.consentDialogProperty.FindPropertyRelative("mActionButtons");
            PrivacyProperties.selectedToggleIndex.property = PrivacyProperties.mainProperty.FindPropertyRelative("mConsentDialogComposerSettings.mToggleSelectedIndex");
            PrivacyProperties.selectedButtonIndex.property = PrivacyProperties.mainProperty.FindPropertyRelative("mConsentDialogComposerSettings.mButtonSelectedIndex");
            PrivacyProperties.enableCopyPasteMode.property = PrivacyProperties.mainProperty.FindPropertyRelative("mConsentDialogComposerSettings.mEnableCopyPasteMode");

            // Utility module consists of other sub-module properties.
            // RatingRequest properties.
            RatingRequestProperties.mainProperty = serializedObject.FindProperty("mRatingRequestSettings");
            RatingRequestProperties.defaultRatingDialogContent.property = RatingRequestProperties.mainProperty.FindPropertyRelative("mDefaultRatingDialogContent");
            RatingRequestProperties.minimumAcceptedStars.property = RatingRequestProperties.mainProperty.FindPropertyRelative("mMinimumAcceptedStars");
            RatingRequestProperties.supportEmail.property = RatingRequestProperties.mainProperty.FindPropertyRelative("mSupportEmail");
            RatingRequestProperties.iosAppId.property = RatingRequestProperties.mainProperty.FindPropertyRelative("mIosAppId");
            RatingRequestProperties.annualCap.property = RatingRequestProperties.mainProperty.FindPropertyRelative("mAnnualCap");
            RatingRequestProperties.delayAfterInstallation.property = RatingRequestProperties.mainProperty.FindPropertyRelative("mDelayAfterInstallation");
            RatingRequestProperties.coolingOffPeriod.property = RatingRequestProperties.mainProperty.FindPropertyRelative("mCoolingOffPeriod");
            RatingRequestProperties.ignoreConstraintsInDevelopment.property = RatingRequestProperties.mainProperty.FindPropertyRelative("mIgnoreContraintsInDevelopment");

#if EASY_MOBILE_PRO
            // Native Apis module properties.
            NativeApisProperties.mainProperty = serializedObject.FindProperty("mNativeApisSettings");
            NativeApisProperties.iOSCameraUsageDescription.property = NativeApisProperties.mainProperty.FindPropertyRelative("mIOSCameraUsageDescription");
            NativeApisProperties.iOSPhotoUsageDescription.property = NativeApisProperties.mainProperty.FindPropertyRelative("mIOSPhotoUsageDescription");
            NativeApisProperties.iOSAddPhotoUsageDescription.property = NativeApisProperties.mainProperty.FindPropertyRelative("mIOSAddPhotoUsageDescription");
            NativeApisProperties.iOSMicrophoneUsageDescription.property = NativeApisProperties.mainProperty.FindPropertyRelative("mIOSMicrophoneUsageDescription");
            NativeApisProperties.iOSContactUsageDescription.property = NativeApisProperties.mainProperty.FindPropertyRelative("mIOSContactUsageDescription");
            NativeApisProperties.isMediaEnabled.property = NativeApisProperties.mainProperty.FindPropertyRelative("mIsMediaEnabled");
            NativeApisProperties.isContactsEnabled.property = NativeApisProperties.mainProperty.FindPropertyRelative("mIsContactsEnabled");
            NativeApisProperties.iOSExtraUsageDescriptions.property = NativeApisProperties.mainProperty.FindPropertyRelative("miOSExtraUsageDescriptions");
#endif

            // Get the sorted list of GPGS leaderboard and achievement ids.
            gpgsIdDict = new SortedDictionary<string, string>(EM_EditorUtil.GetGPGSIds());

#if EM_UIAP
            // Determine if AppleTangle and GooglePlayTangle classes are valid ones (generated by UnityIAP's receipt validation obfuscator).
            isAppleTangleValid = EM_EditorUtil.IsValidAppleTangleClass();
            isGooglePlayTangleValid = EM_EditorUtil.IsValidGooglePlayTangleClass();
#endif
        }

        private enum UIMode
        {
            HomePage,
            ModulePage
        }

        private UIMode currentUIMode = UIMode.HomePage;

        public override void OnInspectorGUI()
        {
            if (!callFromEditorWindow)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(OpenInInspectorWarning, MessageType.Info);
                if (GUILayout.Button("Edit"))
                    EM_SettingsWindow.ShowWindow();
                return;
            }

            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();

            // Disable the UI when the editor is compiling.
            EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);

            // Not switching mode if the editor is compiling.
            if (!EditorApplication.isCompiling)
                currentUIMode = isSelectingModule.boolValue ? UIMode.HomePage : UIMode.ModulePage;

            if (activeSection == Section.Settings)
            {
                EditorGUILayout.Space();
                if (currentUIMode == UIMode.HomePage)
                {
                    DrawSelectModulePage();
                }
                else
                {
                    EditorGUILayout.BeginVertical();
                    if (GUILayout.Button(new GUIContent("Back", EM_GUIStyleManager.BackIcon), EM_GUIStyleManager.GetCustomStyle("Module Back Button Text")))
                    {
                        isSelectingModule.boolValue = true;
                    }
                    EditorGUILayout.EndVertical();
                    GetModuleDrawAction((Module)activeModuleIndex.intValue)();
                    EditorGUILayout.Space();
                }
            }
            else if (activeSection == Section.Build)
            {
                DrawBuildSectionGUI();
            }
            else if (activeSection == Section.Tools)
            {
                DrawToolsSectionGUI();
            }
            else
            {
                EditorGUILayout.HelpBox("Unable to display a valid GUI section.", MessageType.Error);
            }

            EditorGUI.EndDisabledGroup();

            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }

#if !DRAW_DEFAULT_INSPECTOR_HEADER
        /// <summary>
        /// Draws our custom header.
        /// </summary>
        protected override void OnHeaderGUI()
        {
            EditorGUILayout.BeginHorizontal(EM_GUIStyleManager.GetCustomStyle("Inspector Header"));

            // EM icon.
            GUILayout.Label(new GUIContent(EM_GUIStyleManager.EasyMobileIcon), EM_GUIStyleManager.InspectorHeaderIcon);

            EditorGUILayout.BeginVertical();

            // EM title.
#if EASY_MOBILE_PRO
            string title = "EASY MOBILE Pro";
#else
            string title = "EASY MOBILE Basic";
#endif
            EditorGUILayout.LabelField(title, EM_GUIStyleManager.InspectorHeaderTitle);

            // EM version.
            EditorGUILayout.LabelField("Version " + EM_Constants.versionString, EM_GUIStyleManager.InspectorHeaderSubtitle);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            DrawSectionSelectionGUI();
        }
#endif

        #endregion
    }
}
