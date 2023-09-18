using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile.Internal;

namespace EasyMobile
{
    [Serializable]
    public class MoPubSettings : AdNetworkSettings
    {
        /// <summary>
        /// Gets or sets the default banner identifier.
        /// </summary>
        public AdId DefaultBannerId
        {
            get { return mDefaultBannerId; }
            set { mDefaultBannerId = value; }
        }

        /// <summary>
        /// Gets or sets the default interstitial ad identifier.
        /// </summary>
        public AdId DefaultInterstitialAdId
        {
            get { return mDefaultInterstitialAdId; }
            set { mDefaultInterstitialAdId = value; }
        }

        /// <summary>
        /// Gets or sets the default rewarded ad identifier.
        /// </summary>
        public AdId DefaultRewardedAdId
        {
            get { return mDefaultRewardedAdId; }
            set { mDefaultRewardedAdId = value; }
        }

        /// <summary>
        /// Gets or sets the list of custom banner identifiers.
        /// Each identifier is associated with an ad placement.
        /// </summary>
        public Dictionary<AdPlacement, AdId> CustomBannerIds
        {
            get { return mCustomBannerIds; }
            set { mCustomBannerIds = value as Dictionary_AdPlacement_AdId; }
        }

        /// <summary>
        /// Gets or sets the list of custom interstitial ad identifiers.
        /// Each identifier is associated with an ad placement.
        /// </summary>
        public Dictionary<AdPlacement, AdId> CustomInterstitialAdIds
        {
            get { return mCustomInterstitialAdIds; }
            set { mCustomInterstitialAdIds = value as Dictionary_AdPlacement_AdId; }
        }

        /// <summary>
        /// Gets or sets the list of custom rewarded ad identifiers.
        /// Each identifier is associated with an ad placement.
        /// </summary>
        public Dictionary<AdPlacement, AdId> CustomRewardedAdIds
        {
            get { return mCustomRewardedAdIds; }
            set { mCustomRewardedAdIds = value as Dictionary_AdPlacement_AdId; }
        }

        /// <summary>
        /// Enables or disables application open report.
        /// </summary>
        public bool ReportAppOpen
        {
            get { return mReportAppOpen; }
            set { mReportAppOpen = value; }
        }

        /// <summary>
        /// Gets or sets ITune App Id.
        /// </summary>
        public string ITuneAppID
        {
            get { return mITuneAppID; }
            set { mITuneAppID = value; }
        }

        /// <summary>
        /// Enables or disables location passing.
        /// </summary>
        public bool EnableLocationPassing
        {
            get { return mEnableLocationPassing; }
            set { mEnableLocationPassing = value; }
        }

        /// <summary>
        /// Enables or disables advanced settings.
        /// </summary>
        public bool UseAdvancedSetting
        {
            get { return mUseAdvancedSetting; }
            set { mUseAdvancedSetting = value; }
        }

        /// <summary>
        /// Allow supported SDK networks to collect user information on the basis of legitimate interest.
        /// </summary>
        public bool AllowLegitimateInterest
        {
            get { return mAllowLegitimateInterest; }
            set { mAllowLegitimateInterest = value; }
        }

        /// <summary>
        /// Set desired log level here to override default level of MPLogLevelNone
        /// </summary>
        public LogLevel MoPubLogLevel
        {
            get { return mLogLevel; }
            set { mLogLevel = value; }
        }

        /// <summary>
        /// Networks used in advanced initialization.
        /// </summary>
        /// <value>The mediated networks.</value>
        public MediatedNetwork[] MediatedNetworks
        {
            get { return mMediatedNetworks; }
            set { mMediatedNetworks = value; }
        }

        /// <summary>
        /// Enables or disables auto request consent.
        /// </summary>
        public bool AutoRequestConsent
        {
            get { return mAutoRequestConsent; }
            set { mAutoRequestConsent = value; }
        }

        /// <summary>
        /// Force GDPR applicable or not ?
        /// </summary>
        public bool ForceGdprApplicable
        {
            get { return mForceGdprApplicable; }
            set { mForceGdprApplicable = value; }
        }

        [SerializeField]
        private bool mReportAppOpen;
        [SerializeField]
        private string mITuneAppID;
        [SerializeField]
        private bool mEnableLocationPassing;

        [SerializeField]
        private bool mUseAdvancedSetting;
        [SerializeField]
        private bool mAllowLegitimateInterest;
        [SerializeField]
        private LogLevel mLogLevel = LogLevel.None;
        [SerializeField]
        private MediatedNetwork[] mMediatedNetworks;

        [SerializeField]
        private bool mAutoRequestConsent;
        [SerializeField]
        private bool mForceGdprApplicable;

        [SerializeField]
        private AdId mDefaultBannerId;
        [SerializeField]
        private AdId mDefaultInterstitialAdId;
        [SerializeField]
        private AdId mDefaultRewardedAdId;

        [SerializeField]
        private Dictionary_AdPlacement_AdId mCustomBannerIds;
        [SerializeField]
        private Dictionary_AdPlacement_AdId mCustomInterstitialAdIds;
        [SerializeField]
        private Dictionary_AdPlacement_AdId mCustomRewardedAdIds;

        public enum SupportedNetwork
        {
            None = 0,
            AdColony,
            AdMob,
            AppLovin,
            Chartboost,
            Facebook,
            Furry,
            IronSource,
            Pangle,
            TapJoy,
            Snap,
            Tapjoy,
            UnityAds,
            Verizon,
            Vungle,
        }

        public enum LogLevel
        {
            Debug = 20,
            Info = 30,
            None = 70
        }

        [Serializable]
        public class MediatedNetwork
        {
            /// <summary>
            /// Is one of MoPub's supported networks?
            /// </summary>
            public bool IsSupportedNetwork
            {
                get { return mIsSupportedNetwork; }
                set { mIsSupportedNetwork = value; }
            }

            /// <summary>
            /// Used if <see cref="IsSupportedNetwork"/> is <see langword="true"/>.
            /// </summary>
            public SupportedNetwork SupportedNetworkName
            {
                get { return mSupportedNetworkName; }
                set { mSupportedNetworkName = value; }
            }

            /// <summary>
            /// Specify the class name that implements the AdapterConfiguration interface.
            /// </summary>
            public string AdapterConfigurationClassName
            {
                get { return mAdapterConfigurationClassName; }
                set { mAdapterConfigurationClassName = value; }
            }

            /// <summary>
            /// Specify the class name that implements the MediationSettings interface.
            /// Note: Custom network mediation settings are currently not supported on Android.
            /// </summary>
            public string MediationSettingsClassName
            {
                get { return mMediationSettingsClassName; }
                set { mMediationSettingsClassName = value; }
            }

            /// <summary>
            /// Network adapter configuration settings (initialization).
            /// </summary>
            public Dictionary<string, string> NetworkConfiguration
            {
                get { return mNetworkConfiguration; }
                set { mNetworkConfiguration = value as StringStringSerializableDictionary; }
            }

            /// <summary>
            /// Global mediation settings (per ad request).
            /// </summary>
            public Dictionary<string, string> MediationSettings
            {
                get { return mMediationSettings; }
                set { mMediationSettings = value as StringStringSerializableDictionary; }
            }

            /// <summary>
            /// Additional options to pass to the MoPub servers (per ad request).
            /// </summary>
            public Dictionary<string, string> MoPubRequestOptions
            {
                get { return mMoPubRequestOption; }
                set { mMoPubRequestOption = value as StringStringSerializableDictionary; }
            }

            [SerializeField, Rename("Is Supported Network")]
            private bool mIsSupportedNetwork;

            [SerializeField, Rename("Network Name")]
            private SupportedNetwork mSupportedNetworkName;

            [SerializeField]
            private string mAdapterConfigurationClassName;

            [SerializeField]
            private string mMediationSettingsClassName;

            [SerializeField]
            private StringStringSerializableDictionary mNetworkConfiguration;

            [SerializeField]
            private StringStringSerializableDictionary mMediationSettings;

            [SerializeField]
            private StringStringSerializableDictionary mMoPubRequestOption;

        }
    }

    public static class MoPubSettingsExtension
    {
#if EM_MOPUB
        public static MoPub.LogLevel ToMoPubLogLevel(this MoPubSettings.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case MoPubSettings.LogLevel.Debug:
                    return MoPub.LogLevel.Debug;

                case MoPubSettings.LogLevel.Info:
                    return MoPub.LogLevel.Info;

                case MoPubSettings.LogLevel.None:
                default:
                    return MoPub.LogLevel.None;
            }
        }

        public static MoPubBase.MediatedNetwork ToMoPubMediatedNetwork(this MoPubSettings.MediatedNetwork mediatedNetwork)
        {
            if (mediatedNetwork == null)
                return null;

            return new MoPubBase.MediatedNetwork()
            {
                AdapterConfigurationClassName = mediatedNetwork.GetAdapterConfigurationClassName(),
                MediationSettingsClassName = mediatedNetwork.MediationSettingsClassName,
                NetworkConfiguration = mediatedNetwork.NetworkConfiguration,
                MediationSettings = StringStringDictToStringObjectDict(mediatedNetwork.MediationSettings),
                MoPubRequestOptions = mediatedNetwork.MoPubRequestOptions,
            };
        }

        private static string GetAdapterConfigurationClassName(this MoPubSettings.MediatedNetwork mediatedNetwork)
        {
            if (mediatedNetwork == null)
                return null;

            if (!mediatedNetwork.IsSupportedNetwork)
                return mediatedNetwork.AdapterConfigurationClassName;

            switch (mediatedNetwork.SupportedNetworkName)
            {
                case MoPubSettings.SupportedNetwork.AdColony:
                    return new MoPub.SupportedNetwork.AdColony().AdapterConfigurationClassName;
                case MoPubSettings.SupportedNetwork.AdMob:
                    return new MoPub.SupportedNetwork.AdMob().AdapterConfigurationClassName;
                case MoPubSettings.SupportedNetwork.AppLovin:
                    return new MoPub.SupportedNetwork.AppLovin().AdapterConfigurationClassName;
                case MoPubSettings.SupportedNetwork.Chartboost:
                    return new MoPub.SupportedNetwork.Chartboost().AdapterConfigurationClassName;
                case MoPubSettings.SupportedNetwork.Facebook:
                    return new MoPub.SupportedNetwork.Facebook().AdapterConfigurationClassName;
                case MoPubSettings.SupportedNetwork.Furry:
                    return new MoPub.SupportedNetwork.Flurry().AdapterConfigurationClassName;
                case MoPubSettings.SupportedNetwork.IronSource:
                    return new MoPub.SupportedNetwork.IronSource().AdapterConfigurationClassName;
                case MoPubSettings.SupportedNetwork.Pangle:
                    return new MoPub.SupportedNetwork.Pangle().AdapterConfigurationClassName;
                case MoPubSettings.SupportedNetwork.Snap:
                    return new MoPub.SupportedNetwork.Snap().AdapterConfigurationClassName;
                case MoPubSettings.SupportedNetwork.TapJoy:
                    return new MoPub.SupportedNetwork.Tapjoy().AdapterConfigurationClassName;
                case MoPubSettings.SupportedNetwork.UnityAds:
                    return new MoPub.SupportedNetwork.Unity().AdapterConfigurationClassName;
                case MoPubSettings.SupportedNetwork.Verizon:
                    return new MoPub.SupportedNetwork.Verizon().AdapterConfigurationClassName;
                case MoPubSettings.SupportedNetwork.Vungle:
                    return new MoPub.SupportedNetwork.Vungle().AdapterConfigurationClassName;
                default:
                    return null;
            }
        }

        private static string GetMediationSettingsClassName(this MoPubSettings.MediatedNetwork mediatedNetwork)
        {
            if (mediatedNetwork == null)
                return null;

            if (!mediatedNetwork.IsSupportedNetwork)
                return mediatedNetwork.MediationSettingsClassName;

            switch (mediatedNetwork.SupportedNetworkName)
            {
                case MoPubSettings.SupportedNetwork.AdColony:
                    return new MoPub.SupportedNetwork.AdColony().MediationSettingsClassName;
                case MoPubSettings.SupportedNetwork.AdMob:
                    return new MoPub.SupportedNetwork.AdMob().MediationSettingsClassName;
                case MoPubSettings.SupportedNetwork.AppLovin:
                    return new MoPub.SupportedNetwork.AppLovin().MediationSettingsClassName;
                case MoPubSettings.SupportedNetwork.Chartboost:
                    return new MoPub.SupportedNetwork.Chartboost().MediationSettingsClassName;
                case MoPubSettings.SupportedNetwork.Facebook:
                    return new MoPub.SupportedNetwork.Facebook().MediationSettingsClassName;
                case MoPubSettings.SupportedNetwork.Furry:
                    return new MoPub.SupportedNetwork.Flurry().MediationSettingsClassName;
                case MoPubSettings.SupportedNetwork.IronSource:
                    return new MoPub.SupportedNetwork.IronSource().MediationSettingsClassName;
                case MoPubSettings.SupportedNetwork.Pangle:
                    return new MoPub.SupportedNetwork.Pangle().MediationSettingsClassName;
                case MoPubSettings.SupportedNetwork.Snap:
                    return new MoPub.SupportedNetwork.Snap().MediationSettingsClassName;
                case MoPubSettings.SupportedNetwork.TapJoy:
                    return new MoPub.SupportedNetwork.Tapjoy().MediationSettingsClassName;
                case MoPubSettings.SupportedNetwork.UnityAds:
                    return new MoPub.SupportedNetwork.Unity().MediationSettingsClassName;
                case MoPubSettings.SupportedNetwork.Verizon:
                    return new MoPub.SupportedNetwork.Verizon().MediationSettingsClassName;
                case MoPubSettings.SupportedNetwork.Vungle:
                    return new MoPub.SupportedNetwork.Vungle().MediationSettingsClassName;
                default:
                    return null;
            }
        }
#endif

        private static Dictionary<string, object> StringStringDictToStringObjectDict(this Dictionary<string, string> dict)
        {
            if (dict == null)
                return null;

            return dict.ToDictionary(
                item => item.Key,
                item => string.IsNullOrEmpty(item.Value) ? (object)item.Value : null);
        }
    }
}
