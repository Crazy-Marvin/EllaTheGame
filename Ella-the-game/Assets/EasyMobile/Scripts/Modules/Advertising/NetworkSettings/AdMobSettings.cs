using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using EasyMobile.Internal;

namespace EasyMobile
{
    [Serializable]
    public class AdMobSettings : AdNetworkSettings
    {
        [Obsolete("AppId has been deprecated since Easy Mobile Pro version 2.6.0 because the GoogleMobileAds SDK no longer allows access to this value in runtime.")]
        /// <summary>
        /// Gets or sets the AdMob app identifier.
        /// </summary>
        public AdId AppId
        {
            get { return mAppId; }
            set { mAppId = value; }
        }

        /// <summary>
        /// Gets or sets the default banner identifier.
        /// </summary>
        public AdId DefaultBannerAdId
        {
            get { return mDefaultBannerAdId; }
            set { mDefaultBannerAdId = value; }
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
        /// Enables or disables test mode.
        /// </summary>
        public bool EnableTestMode
        {
            get { return mEnableTestMode; }
            set { mEnableTestMode = value; }
        }


        /// <summary>
        /// Gets or sets the target settings.
        /// </summary>
        public AdMobTargetingSettings TargetingSettings
        {
            get { return mTargetingSettings; }
            set { mTargetingSettings = value; }
        }

        /// <summary>
        /// Gets or sets the list of custom banner identifiers.
        /// Each identifier is associated with an ad placement.
        /// </summary>
        public Dictionary<AdPlacement, AdId> CustomBannerAdIds
        {
            get { return mCustomBannerAdIds; }
            set { mCustomBannerAdIds = value as Dictionary_AdPlacement_AdId; }
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

        [SerializeField]
        private AdMobTargetingSettings mTargetingSettings;
        [SerializeField]
        private bool mEnableTestMode;

        [SerializeField]
        private AdId mAppId;
        [SerializeField]
        private AdId mDefaultBannerAdId;
        [SerializeField]
        private AdId mDefaultInterstitialAdId;
        [SerializeField]
        private AdId mDefaultRewardedAdId;
        [SerializeField]
        private Dictionary_AdPlacement_AdId mCustomBannerAdIds;
        [SerializeField]
        private Dictionary_AdPlacement_AdId mCustomInterstitialAdIds;
        [SerializeField]
        private Dictionary_AdPlacement_AdId mCustomRewardedAdIds;

        [Serializable]
        public class AdMobTargetingSettings
        {
            public AdChildDirectedTreatment TagForChildDirectedTreatment
            {
                get { return mTagForChildDirectedTreatment; }
                set { mTagForChildDirectedTreatment = value; }
            }

            public Dictionary<string, string> ExtraOptions
            {
                get { return mExtraOptions; }
                set { mExtraOptions = value as StringStringSerializableDictionary; }
            }

            [SerializeField]
            private AdChildDirectedTreatment mTagForChildDirectedTreatment = AdChildDirectedTreatment.Unspecified;
            [SerializeField]
            private StringStringSerializableDictionary mExtraOptions;
        }
    }
}