using UnityEngine;
using System.Collections;
using EasyMobile.Internal;
using System;
using System.Collections.Generic;

namespace EasyMobile
{
    [System.Serializable]
    public class AdSettings
    {
        /// <summary>
        /// Gets or sets auto ad loading mode.
        /// </summary>
        public AutoAdLoadingMode AutoAdLoadingMode
        {
            get { return mAutoLoadAdsMode; }
            set { mAutoLoadAdsMode = value; }
        }

        /// <summary>
        /// Gets or sets the ad checking interval of the auto ad-loading feature.
        /// </summary>
        /// <value>The ad checking interval.</value>
        public float AdCheckingInterval
        {
            get { return mAdCheckingInterval; }
            set { mAdCheckingInterval = value; }
        }

        /// <summary>
        /// Gets or sets the ad loading interval of the auto ad-loading feature.
        /// </summary>
        /// <value>The ad loading interval.</value>
        public float AdLoadingInterval
        {
            get { return mAdLoadingInterval; }
            set { mAdLoadingInterval = value; }
        }

        /// <summary>
        /// Gets the default ad networks for iOS platform.
        /// </summary>
        /// <value>The ios default ad networks.</value>
        public DefaultAdNetworks IosDefaultAdNetworks
        {
            get { return mIosDefaultAdNetworks; }
        }

        /// <summary>
        /// Gets the default ad networks for Android platform.
        /// </summary>
        /// <value>The android default ad networks.</value>
        public DefaultAdNetworks AndroidDefaultAdNetworks
        {
            get { return mAndroidDefaultAdNetworks; }
        }

        /// <summary>
        /// Gets the AdColony settings.
        /// </summary>
        /// <value>The ad colony.</value>
        public AdColonySettings AdColony
        {
            get { return mAdColony; }
        }

        /// <summary>
        /// Gets the AdMob settings.
        /// </summary>
        public AdMobSettings AdMob
        {
            get { return mAdMob; }
        }

        /// <summary>
        /// Gets the AppLovin settings.
        /// </summary>
        public AppLovinSettings AppLovin
        {
            get { return mAppLovin; }
        }

        /// <summary>
        /// Gets the Facebook Audience Network setttings.
        /// </summary>
        public AudienceNetworkSettings AudienceNetwork
        {
            get { return mFBAudience; }
        }

        /// <summary>
        /// Gets the Chartboost Network settings.
        /// </summary>
        public ChartboostSettings Chartboost
        {
            get { return mChartboost; }
        }

        /// <summary>
        /// Gets the FairBid settings.
        /// </summary>
        public FairBidSettings FairBid
        {
            get { return mFairBid; }
        }

        /// <summary>
        /// Gets the IronSource settings.
        /// </summary>
        public IronSourceSettings IronSource
        {
            get { return mIronSource; }
        }

        /// <summary>
        /// Gets the MoPub settings.
        /// </summary>
        public MoPubSettings MoPub
        {
            get { return mMoPub; }
        }

        /// <summary>
        /// Gets the Tapjoy settings.
        /// </summary>
        public TapjoySettings Tapjoy
        {
            get { return mTapjoy; }
        }

        /// <summary>
        /// Gets the UnityAds settings.
        /// </summary>
        /// <value>The unity ads.</value>
        public UnityAdsSettings UnityAds
        {
            get { return mUnityAds; }
        }

        /// <summary>
        /// Gets the Vungle settings.
        /// </summary>
        /// <value>The vungle ads.</value>
        public VungleSettings VungleAds
        {
            get { return mVungleAds; }
        }

        [System.Serializable]
        public struct DefaultAdNetworks
        {
            public BannerAdNetwork bannerAdNetwork;
            public InterstitialAdNetwork interstitialAdNetwork;
            public RewardedAdNetwork rewardedAdNetwork;

            public DefaultAdNetworks(BannerAdNetwork banner, InterstitialAdNetwork interstitial, RewardedAdNetwork rewarded)
            {
                bannerAdNetwork = banner;
                interstitialAdNetwork = interstitial;
                rewardedAdNetwork = rewarded;
            }
        }

        // Automatic ad-loading config.
        [SerializeField]
        private AutoAdLoadingMode mAutoLoadAdsMode = AutoAdLoadingMode.LoadAllDefinedPlacements;
        [SerializeField]
        [Range(5, 100)]
        private float mAdCheckingInterval = 10f;
        [SerializeField]
        [Range(5, 100)]
        private float mAdLoadingInterval = 20f;

        // Default ad networks.
        [SerializeField]
        private DefaultAdNetworks mIosDefaultAdNetworks = new DefaultAdNetworks(BannerAdNetwork.None, InterstitialAdNetwork.None, RewardedAdNetwork.None);
        [SerializeField]
        private DefaultAdNetworks mAndroidDefaultAdNetworks = new DefaultAdNetworks(BannerAdNetwork.None, InterstitialAdNetwork.None, RewardedAdNetwork.None);

        // Network settings.
        [SerializeField]
        private AdColonySettings mAdColony = null;
        [SerializeField]
        private AdMobSettings mAdMob = null;
        [SerializeField]
        private AppLovinSettings mAppLovin = null;
        [SerializeField]
        private AudienceNetworkSettings mFBAudience = null;
        [SerializeField]
        private ChartboostSettings mChartboost = null;
        [SerializeField]
        private FairBidSettings mFairBid = null;
        [SerializeField]
        private IronSourceSettings mIronSource = null;
        [SerializeField]
        private MoPubSettings mMoPub = null;
        [SerializeField]
        private TapjoySettings mTapjoy = null;
        [SerializeField]
        private UnityAdsSettings mUnityAds = null;
        [SerializeField]
        private VungleSettings mVungleAds = null;
    }
}