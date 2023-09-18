using EasyMobile.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile
{
    [Serializable]
    public class AudienceNetworkSettings : AdNetworkSettings
    {
        /// <summary>
        /// Gets or sets default banner ad size.
        /// </summary>
        public FBAudienceBannerAdSize BannerAdSize
        {
            get { return mBannerAdSize; }
            set { mBannerAdSize = value; }
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
        /// Gets or sets the list of test devices.
        /// </summary>
        public string[] TestDevices
        {
            get { return mTestDevices; }
            set { mTestDevices = value; }
        }

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

        [SerializeField]
        private FBAudienceBannerAdSize mBannerAdSize;
        [SerializeField]
        private bool mEnableTestMode;
        [SerializeField]
        private string[] mTestDevices;

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

        public enum FBAudienceBannerAdSize
        {
            _50,
            _90,
            _250,
        }
    }
}
