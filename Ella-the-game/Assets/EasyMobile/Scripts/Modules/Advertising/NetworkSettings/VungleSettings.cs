using System;
using System.Collections.Generic;
using EasyMobile.Internal;
using UnityEngine;

namespace EasyMobile
{
    [Serializable]
    public class VungleSettings : AdNetworkSettings
    {
        /// <summary>
        /// Gets or sets the Vungle app identifier.
        /// </summary>
        /// <value>The app identifier.</value>
        public AdId AppId
        {
            get { return mAppId; }
            set { mAppId = value; }
        }

        /// <summary>
        /// Gets or sets the default interstitial ad identifier.
        /// </summary>
        /// <value>The default interstitial ad identifier.</value>
        public AdId DefaultInterstitialAdId
        {
            get { return mDefaultInterstitialAdId; }
            set { mDefaultInterstitialAdId = value; }
        }

        /// <summary>
        /// Gets or sets the default rewarded ad identifier.
        /// </summary>
        /// <value>The default rewarded ad identifier.</value>
        public AdId DefaultRewardedAdId
        {
            get { return mDefaultRewardedAdId; }
            set { mDefaultRewardedAdId = value; }
        }

        /// <summary>
        /// Gets or sets the default banner ad identifier.
        /// </summary>
        /// <value>The default rewarded ad identifier.</value>
        public AdId DefaultBannerAdId
        {
            get { return mDefaultBannerAdId; }
            set { mDefaultBannerAdId = value; }
        }

        /// <summary>
        /// Gets or set wherethe Vungle Client will use the advanced settings or not
        /// </summary>
        /// <value>Will Vungle client use the advanced settings.</value>
        public bool UseAdvancedSetting
        {
            get { return mUseAdvancedSetting; }
            set { mUseAdvancedSetting = value; }
        }

        /// <summary>
        /// Gets or set the advanced settings to be used by Vungle Ads Client.
        /// </summary>
        /// <value>The advanced settings to be used by Vungle Ads Client.</value>
        public VungleAdvancedSettings AdvancedSettings
        {
            get { return mAdvancedSettings; }
            set { mAdvancedSettings = value; }
        }

        /// <summary>
        /// Gets or sets the list of custom interstitial ad identifiers.
        /// Each identifier is associated with an ad placement.
        /// </summary>
        /// <value>The custom interstitial ad identifiers.</value>
        public Dictionary<AdPlacement, AdId> CustomInterstitialAdIds
        {
            get { return mCustomInterstitialAdIds; }
            set { mCustomRewardedAdIds = (Dictionary_AdPlacement_AdId)value; }
        }

        /// <summary>
        /// Gets or sets the list of custom rewarded ad identifiers.
        /// Each identifier is associated with an ad placement.
        /// </summary>
        /// <value>The custom rewarded ad identifiers.</value>
        public Dictionary<AdPlacement, AdId> CustomRewardedAdIds
        {
            get { return mCustomRewardedAdIds; }
            set { mCustomRewardedAdIds = (Dictionary_AdPlacement_AdId)value; }
        }

        /// <summary>
        /// Gets or sets the list of custom banner ad identifiers.
        /// Each identifier is associated with an ad placement.
        /// </summary>
        /// <value>The custom rewarded ad identifiers.</value>
        public Dictionary<AdPlacement, AdId> CustomBannerAdIds
        {
            get { return mCustomBannerAdIds; }
            set { mCustomBannerAdIds = (Dictionary_AdPlacement_AdId)value; }
        }

        [SerializeField]
        private AdId mAppId = null;
        [SerializeField]
        private AdId mDefaultInterstitialAdId = null;
        [SerializeField]
        private AdId mDefaultRewardedAdId = null;
        [SerializeField]
        private AdId mDefaultBannerAdId = null;
        [SerializeField]
        private Dictionary_AdPlacement_AdId mCustomInterstitialAdIds = null;
        [SerializeField]
        private Dictionary_AdPlacement_AdId mCustomRewardedAdIds = null;
        [SerializeField]
        private Dictionary_AdPlacement_AdId mCustomBannerAdIds = null;
        [SerializeField]
        private bool mUseAdvancedSetting;
        [SerializeField]
        private VungleAdvancedSettings mAdvancedSettings;
        [Serializable]
        public class VungleAdvancedSettings
        {
            public enum AdOrientation
            {
                Portrait = 1,
                LandscapeLeft = 2,
                LandscapeRight = 3,
                PortraitUpsideDown = 4,
                Landscape = 5,
                All = 6,
                AllButUpsideDown = 7,
                MatchVideo = 8
            }

            [Serializable]
            public struct PrematureAdClosePopup
            {
                public string alertTitle;
                public string alertText;
                public string closeText;
                public string continueText;
            }

            public AdOrientation adOrientation = AdOrientation.All;
            public PrematureAdClosePopup prematureAdClosePopup = new PrematureAdClosePopup()
            {
                alertTitle = "Alert!!!",
                alertText = "You are about to close this ad. If this is a rewarded ad, you might not receive the reward.",
                closeText = "Close",
                continueText = "Continue"
            };

            public bool enableAdSound = true;
            public long minimumDiskSpaceForInitialization = 51000000;
            public long minimumDiskSpaceForAds = 50000000;
            public bool disableHardwareId;
        }
    }
}