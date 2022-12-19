using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using EasyMobile.Internal;

namespace EasyMobile
{
    [Serializable]
    public class AppLovinSettings : AdNetworkSettings
    {
        /// <summary>
        /// Gets or sets the AppLovin SDKKey.
        /// </summary>
        public string SDKKey
        {
            get { return mSDKKey; }
            set { mSDKKey = value; }
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
        /// age-restricted category.
        /// </summary>
        public bool AgeRestrictMode
        {
            get { return mAgeRestrictMode; }
            set { mAgeRestrictMode = value; }
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
        private bool mAgeRestrictMode;

        [SerializeField]
        private string mSDKKey;
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


    }
}